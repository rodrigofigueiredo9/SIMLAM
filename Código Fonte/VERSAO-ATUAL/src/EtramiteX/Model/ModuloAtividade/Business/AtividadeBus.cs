using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business
{
	public class AtividadeBus
	{
		AtividadeValidar _validar = new AtividadeValidar();
		AtividadeDa _da = new AtividadeDa();
		ProtocoloBus _busProtocolo = new ProtocoloBus();

		#region Ações de DML

		public void AlterarSituacao(Atividade atividade, BancoDeDados banco = null)
		{
			if (!_validar.AlterarSituacao(atividade))
			{
				return;
			}

			_da.AlterarSituacao(atividade, banco);
		}

		public void AlterarSituacao(List<Atividade> lstAtividades, eAtividadeSituacao situacao, BancoDeDados banco = null)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				foreach (Atividade item in lstAtividades)
				{
					item.SituacaoId = (int)situacao;
					AlterarSituacao(item, bancoDeDados);
				}

				if (!Validacao.EhValido)
				{
					return;
				}

				bancoDeDados.Commit();
			}
		}

		public void TituloAnteriores(List<Atividade> atividades, List<Atividade> atividadesAtuais, BancoDeDados banco = null)
		{
			AtividadeBus atividadeBus = new AtividadeBus();
			TituloBus tituloBus = new TituloBus();
			List<Atividade> lstTituloAntigoAtividade = null;
			List<Finalidade> lstFinalidades = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Ao Remover titulos anteriores

				if (atividadesAtuais != null && atividadesAtuais.Count > 0)
				{
					foreach (Atividade atividadeAtual in atividadesAtuais)
					{
						lstFinalidades = atividadeAtual.Finalidades.Where(y => (y.Id == 1 || y.Id == 3)).ToList();//1 - Nova fase, 3 - Renovacao
						foreach (Finalidade finalidade in lstFinalidades)
						{
							if ((finalidade.TituloAnteriorId ?? 0) == 0)
							{
								continue;
							}

							lstTituloAntigoAtividade = null;

							Atividade atividadeNova = atividades.SingleOrDefault(x => x.IdRelacionamento == atividadeAtual.IdRelacionamento);
							if (atividadeNova == null)
							{
								lstTituloAntigoAtividade = tituloBus.ObterAtividades(finalidade.TituloAnteriorId.Value, bancoDeDados);
							}
							else
							{
								Finalidade finalidadeNova = atividadeNova.Finalidades.SingleOrDefault(x => x.IdRelacionamento == finalidade.IdRelacionamento);
								if (finalidadeNova != null && (finalidadeNova.TituloAnteriorId ?? 0) <= 0)
								{
									lstTituloAntigoAtividade = tituloBus.ObterAtividades(finalidade.TituloAnteriorId.Value, bancoDeDados);
								}
							}

							if (lstTituloAntigoAtividade != null && lstTituloAntigoAtividade.Count > 0)
							{
								foreach (var tituloAnteriorAtividade in lstTituloAntigoAtividade)
								{
									eAtividadeSituacao atividadeSituacao = eAtividadeSituacao.EmAndamento;
									if (atividadeBus.VerificarDeferir(tituloAnteriorAtividade, bancoDeDados))
									{
										atividadeSituacao = eAtividadeSituacao.Deferida;
									}
									tituloAnteriorAtividade.SituacaoId = (int)atividadeSituacao;
									atividadeBus.AlterarSituacao(tituloAnteriorAtividade, bancoDeDados);
								}
							}
						}
					}
				}

				#endregion

				#region Ao Adicionar titulos anteriores

				lstFinalidades = null;
				foreach (var atividade in atividades)
				{
					lstFinalidades = atividade.Finalidades.Where(y => (y.Id == 1 || y.Id == 3)).ToList();//1 - Novo, 3 - Renovacao
					foreach (Finalidade finalidade in lstFinalidades)
					{
						if ((finalidade.TituloAnteriorId ?? 0) <= 0)
						{
							continue;
						}

						//1 - Interno
						if (finalidade.TituloAnteriorTipo == 1)
						{
							lstTituloAntigoAtividade = tituloBus.ObterAtividades(finalidade.TituloAnteriorId.Value, bancoDeDados);

							if (lstTituloAntigoAtividade != null)
							{
								//finalidade 1 - Novo = "Nova fase"
								eAtividadeSituacao atividadeSituacao = (finalidade.Id == 1) ? eAtividadeSituacao.NovaFase : eAtividadeSituacao.EmRenovacao;
								atividadeBus.AlterarSituacao(lstTituloAntigoAtividade, atividadeSituacao, bancoDeDados);
							}
						}
					}
				}

				#endregion
			}
		}

		public void AlterarSituacaoProcDoc(List<Atividade> atividades, List<Atividade> atividadesAtuais, BancoDeDados banco = null)
		{
			if (atividadesAtuais == null || atividadesAtuais.Count == 0)
			{
				return;
			}

			foreach (var item in atividades)
			{
				Atividade atividadeAtual = atividadesAtuais.SingleOrDefault(x => x.Id == item.Id);

				if (atividadeAtual == null || atividadeAtual.Finalidades.Count != item.Finalidades.Count || item.Finalidades.Any(x => (x.IdRelacionamento ?? 0) == 0))
				{
					if (atividadeAtual != null &&
						(atividadeAtual.SituacaoId == (int)eAtividadeSituacao.ComPendencia ||
						 atividadeAtual.SituacaoId == (int)eAtividadeSituacao.Indeferida))
					{
						continue;
					}

					if (VerificarDeferir(item, banco))
					{
						if (item.SituacaoId != (int)eAtividadeSituacao.Deferida)
						{
							item.SituacaoId = (int)eAtividadeSituacao.Deferida;
							AlterarSituacao(item, banco);

							continue;
						}
					}

					if (VerificarEncerrar(item, banco))
					{
						if (item.SituacaoId != (int)eAtividadeSituacao.Encerrada)
						{
							item.SituacaoId = (int)eAtividadeSituacao.Encerrada;
							AlterarSituacao(item, banco);

							continue;
						}
					}

					item.SituacaoId = (int)eAtividadeSituacao.EmAndamento;
					AlterarSituacao(item, banco);
				}
			}
		}

		public int EncerrarAtividadeMotivo(Atividade atividade, int protocolo, bool isProcesso)
		{
			try
			{
				if (_validar.EncerrarAtividadeManual(atividade) && _validar.EncerrarAtividadeMotivo(atividade.Motivo, protocolo, isProcesso))
				{
					atividade.SituacaoId = (int)eAtividadeSituacao.Encerrada;
					AlterarSituacao(atividade);

					if (Validacao.EhValido)
					{
						Validacao.Add(Mensagem.Atividade.EncerradaSucesso);
					}

					return atividade.SituacaoId;
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		#endregion

		#region Obter / Filtrar

		public String ObterSituacaoTexto(int SitucaoId)
		{
			try
			{
				return _da.ObterSituacaoTexto(SitucaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public Resultados<AtividadeListarFiltro> Filtrar(AtividadeListarFiltro FiltrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<AtividadeListarFiltro> filtros = new Filtro<AtividadeListarFiltro>(FiltrosListar, paginacao);
				return _da.Filtrar(filtros);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public Atividade ObterAtividadeSituacao(Atividade atividade, BancoDeDados banco = null)
		{
			atividade.SituacaoId = _da.ObterAtividadeProtocoloSituacao(atividade, banco);
			return atividade;
		}

		public Atividade ObterAtividadePorCodigo(int codigo)
		{
			return _da.ObterAtividadePorCodigo(codigo);
		}

		public List<AtividadeSolicitada> ObterAtividadesLista(Protocolo protocolo, bool isApensadosJuntados = false)
		{
			try
			{
				if (protocolo == null || protocolo.Id <= 0)
				{
					return new List<AtividadeSolicitada>();
				}

				return _da.ObterAtividadesLista(protocolo, isApensadosJuntados);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public List<AtividadeSolicitada> ObterAtividadesListaReq(int requerimentoId)
		{
			try
			{
				return _da.ObterAtividadesListaReq(requerimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		public IProtocolo ObterProtocoloAtividadesSolicitadas(int id)
		{
			IProtocolo protocolo = null;
			Processo processo = null;
			try
			{
				if (_busProtocolo.ExisteProtocolo(id))
				{
					protocolo = _busProtocolo.ObterProcessosDocumentos(id);

					if (protocolo.IsProcesso)
					{
						processo = protocolo as Processo;

						//remove os processos que não tem atividade
						for (var i = 0; i < processo.Processos.Count; i++)
						{
							processo.Processos[i] = (_busProtocolo.ExisteAtividade(processo.Processos[i].Id.Value)) ? processo.Processos[i] : null;
						}
						processo.Processos.RemoveAll(x => x == null);

						//remove os documentos que não tem atividade
						for (var i = 0; i < processo.Documentos.Count; i++)
						{
							processo.Documentos[i] = (_busProtocolo.ExisteAtividade(processo.Documentos[i].Id.Value)) ? processo.Documentos[i] : null;
						}
						processo.Documentos.RemoveAll(x => x == null);

						return processo;
					}
					else
					{
						return protocolo as Documento;
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Validações

		internal bool VerificarEncerrar(Atividade atividade, BancoDeDados banco = null)
		{
			if (atividade.Protocolo.Id == 0)
			{
				throw new Exception("erro ao validar Encerrar atividade.Protocolo.id = 0");
			}

			return _da.VerificarEncerrar(atividade, banco);
		}

		internal bool VerificarDeferir(Atividade atividade, BancoDeDados banco = null)
		{
			if (atividade.Protocolo.Id == 0)
			{
				throw new Exception("erro ao validar deferimento atividade.Protocolo.id = 0");
			}

			return _da.VerificarDeferir(atividade, banco);
		}

		public bool ValidarExcluirAtividadeFinalidade(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			if (_da.VerificarAtividadeAssociadaTitulo(protocolo, isProcesso, atividade, modelo))
			{
				Validacao.Add(Mensagem.Atividade.ExcluirAssociadaTitulo);
			}

			return Validacao.EhValido;
		}

		public bool ValidarAtividadeComTituloOuEncerrada(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			return _da.ValidarAtividadeComTituloOuEncerrada(protocolo, isProcesso, atividade, modelo);
		}

		#endregion

		internal bool ExisteAtividadeNoSetor(int atividadeId, int setorId)
		{
			try
			{
				return _da.ExisteAtividadeNoSetor(atividadeId, setorId);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}
			return false;
		}

		public bool AtividadeAtiva(int atividadeId)
		{
			try
			{
				return _da.AtividadeAtiva(atividadeId);
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}
			return false;
		}
	}
}