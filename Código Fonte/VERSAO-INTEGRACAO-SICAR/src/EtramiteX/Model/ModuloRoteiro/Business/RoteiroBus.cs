using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemRoteiro.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business
{
	public class RoteiroBus
	{
		#region Propriedades

		RoteiroValidar _validar = null;
		ItemValidar _validarItem = new ItemValidar();
		RoteiroDa _da = new RoteiroDa();
		ListaBus _listaBus = new ListaBus();
		AtividadeConfiguracaoBus _atividadeBus = new AtividadeConfiguracaoBus();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		private List<Roteiro> RoteiroPadrao
		{
			get { return _listaBus.RoteiroPadrao; }
		}

		#endregion

		#region Acoes DML

		public RoteiroBus()
		{
			_validar = new RoteiroValidar();
		}

		public RoteiroBus(RoteiroValidar roteiroValidar)
		{
			_validar = roteiroValidar;
		}

		public void Salvar(Roteiro roteiro)
		{
			try
			{
				if (_validar.Salvar(roteiro))
				{
					Mensagem msgSucesso;
					bool isEditar = false;

					if (roteiro.Id > 0)
					{
						isEditar = true;
					}

					#region Arquivos/Diretorio
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					if (roteiro.Anexos != null && roteiro.Anexos.Count > 0)
					{
						foreach (Anexo anexo in roteiro.Anexos)
						{
							if (anexo.Arquivo.Id == 0)
							{
								anexo.Arquivo = _busArquivo.Copiar(anexo.Arquivo);
							}
						}
					}
					#endregion

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						#region Arquivos/Banco
						ArquivoDa arquivoDa = new ArquivoDa();

						if (roteiro.Anexos != null && roteiro.Anexos.Count > 0)
						{
							foreach (Anexo anexo in roteiro.Anexos)
							{
								if (anexo.Arquivo.Id == 0)
								{
									arquivoDa.Salvar(anexo.Arquivo, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);
								}
							}
						}
						#endregion

						int roteiroIdOriginal = roteiro.Id;
						_da.Salvar(roteiro, bancoDeDados);
						if (roteiroIdOriginal > 0)
						{
							AlterarSituacaoChecagensComRoteiros(roteiro.Id, 3, bancoDeDados);
						}

						bancoDeDados.Commit();
					}

					if (isEditar)
					{
						msgSucesso = Mensagem.Roteiro.Editar(roteiro.Numero, roteiro.Versao, _da.ObterVersaoAtual(roteiro.Id));
					}
					else
					{
						msgSucesso = Mensagem.Roteiro.Salvar(roteiro.Numero, _da.ObterVersaoAtual(roteiro.Id));
					}

					Validacao.Add(msgSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool VerificarTidAtual(int id, string tid)
		{
			return _da.VerificarTidAtual(id, tid);
		}

		private void AlterarSituacaoChecagensComRoteiros(int idRoteiro, int situacaoChecagem, BancoDeDados banco = null)
		{
			AlterarSituacaoChecagensComRoteiros(new List<int>() { idRoteiro }, situacaoChecagem, banco);
		}

		private void AlterarSituacaoChecagensComRoteiros(List<int> lstIdRoteiros, int situacaoChecagem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				ChecagemRoteiroBus _checkListBus = new ChecagemRoteiroBus();
				List<int> idsChecagens = _da.ObterChecagensFinalizadasContendoRoteiros(lstIdRoteiros);
				if (idsChecagens != null)
				{
					foreach (int idCheckListRoteiro in idsChecagens)
					{
						_checkListBus.AlterarSituacao(idCheckListRoteiro, situacaoChecagem, banco);
					}
				}
			}
		}

		public bool AlterarSituacao(int id, int situacao)
		{
			try
			{
				if (_validar.ValidarDesativarRoteiro(id))
				{
					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.AlterarSituacao(id, situacao, bancoDeDados);

						if (situacao == 1)
						{
							Validacao.Add(Mensagem.Roteiro.Ativar(id));
						}
						else
						{
							AlterarSituacaoChecagensComRoteiros(id, 3, bancoDeDados);
							Validacao.Add(Mensagem.Roteiro.Desativar(id));
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void SalvarItem(Item item)
		{
			try
			{
				if (_validarItem.Salvar(item))
				{
					//nao remova
					item.Condicionante = item.Condicionante ?? string.Empty;
					Mensagem msg;

					if (item.Id > 0)
					{
						msg = Mensagem.Item.EditarSucesso;
					}
					else
					{
						msg = Mensagem.Item.CadastrarSucesso;
					}

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						int itemIdOriginal = item.Id;
						_da.SalvarItem(item);
						if (itemIdOriginal > 0)
						{
							List<int> idRoteirosComItem = _da.ObterRoteirosDeItem(item.Id, bancoDeDados);
							AlterarSituacaoChecagensComRoteiros(idRoteirosComItem, 3, bancoDeDados);
						}

						item.Tid = GerenciadorTransacao.ObterIDAtual();

						bancoDeDados.Commit();
					}

					Validacao.Add(msg);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool ExcluirItem(int id)
		{
			try
			{
				if (_validarItem.Excluir(id))
				{
					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.ExcluirItem(id, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.Item.ItemExcluido);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;

		}

		#endregion

		#region Obter

		public Resultados<Roteiro> Filtrar(ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltro> filtro = new Filtro<ListarFiltro>(filtrosListar, paginacao);
				Resultados<Roteiro> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<Item> FiltrarItem(ListarFiltroItem filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltroItem> filtro = new Filtro<ListarFiltroItem>(filtrosListar, paginacao);
				Resultados<Item> resultados = _da.FiltrarItem(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Roteiro> ObterRoteirosPorAtividades(List<Atividade> atividades)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			if (atividades == null || atividades.Count <= 0)
			{
				return roteiros;
			}

			Roteiro roteiroPadrao = _listaBus.RoteiroPadrao.FirstOrDefault(x => x.Setor == atividades[0].SetorId);

			if (roteiroPadrao != null)
			{
				roteiroPadrao = ObterSimplificado(roteiroPadrao.Id);
			}

			List<String> titulos = new List<String>();

			foreach (var atividade in atividades)
			{
				foreach (var finalidade in atividade.Finalidades)
				{
					finalidade.AtividadeId = atividade.Id;
					finalidade.AtividadeNome = atividade.NomeAtividade;
					finalidade.AtividadeSetorId = atividade.SetorId;

					String modeloTituloNaoAdicionadoRoteiro = _da.ModeloTituloNaoAdicionadoRoteiro(finalidade);
					if (!String.IsNullOrWhiteSpace(modeloTituloNaoAdicionadoRoteiro))
					{
						titulos.Add("\"" + modeloTituloNaoAdicionadoRoteiro + "\"");
						continue;
					}

					Roteiro roteiroAux = _da.ObterRoteirosPorAtividades(finalidade);
					if (roteiroAux == null) 
					{
						roteiroPadrao.AtividadeTexto = atividade.NomeAtividade;
						roteiros.Add(roteiroPadrao);
						continue;
					}

					roteiros.Add(roteiroAux);

				}
			}

			if (titulos.Count > 0) 
			{
				Validacao.Add(Mensagem.Roteiro.TituloNaoAdicionadoRoteiroInterno(Mensagem.Concatenar(titulos)));
			}

			#region Faz a magica de agrupar os resultados

			roteiros = roteiros.GroupBy(x => x.Id).Select(y => new Roteiro
			{
				Id = y.First().Id,
				Nome = y.First().Nome,
				VersaoAtual = y.First().VersaoAtual,
				Tid = y.First().Tid,
				AtividadeTexto = y.Select(w => w.AtividadeTexto).Distinct().Aggregate((total, atual) => total + " / " + atual)
			}).ToList();

			#endregion

			return roteiros;
		}

		public Roteiro Obter(int id, string tid = null)
		{
			Roteiro roteiro = null;

			try
			{
				if (tid == null || _da.VerificarTidAtual(id, tid))
				{
					roteiro = _da.Obter(id);

					if (roteiro.Atividades.Count > 0)
					{
						roteiro.ModelosAtuais = ObterModelosAtividades(roteiro.Atividades);
					}

					if (roteiro.ModelosAtuais.Count > 0)
					{
						roteiro.Modelos.ForEach(x =>
						{
							TituloModeloLst modelo = roteiro.ModelosAtuais.SingleOrDefault(y => y.Id == x.Id);
							if (modelo != null)
							{
								modelo.IsAtivo = true;
							}
						});
					}
				}
				else
				{
					roteiro = _da.ObterHistorico(id, tid);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return roteiro;
		}

		public Roteiro ObterSimplificado(int id, string tid = null)
		{
			Roteiro roteiro = null;

			try
			{
				if (tid == null || _da.VerificarTidAtual(id, tid))
				{
					roteiro = _da.ObterSimplificado(id);
				}
				else
				{
					roteiro = _da.ObterHistoricoSimplificado(id, tid);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return roteiro;
		}

		public int ObterSituacao(int id)
		{
			return _da.ObterSituacao(id);
		}

		public Atividade ObterAtividade(Atividade atividades)
		{
			return _da.ObterAtividade(atividades);
		}

		public List<TituloModeloLst> ObterModelosAtividades(List<AtividadeSolicitada> atividades)
		{
			return _atividadeBus.ObterModelosAtividades(atividades);
		}

		public int ObterFinalidadeCodigo(int finalidadeId)
		{
			return _da.ObterFinalidadeCodigo(finalidadeId);
		}

		public Finalidade ObterFinalidade(Finalidade finalidade)
		{
			return _da.ObterFinalidade(finalidade);
		}

		public List<Roteiro> ObterRoteirosAtuais(AnaliseItem analise)
		{
			List<Roteiro> roteirosAtuais = new List<Roteiro>();

			for (int i = 0; i < analise.Roteiros.Count; i++)
			{
				Roteiro roteiro = ObterSimplificado(analise.Roteiros[i].Id);

				roteirosAtuais.Add(roteiro);
			}

			return roteirosAtuais;
		}

		public Item ObterItem(int id)
		{
			try
			{
				return _da.ObterItem(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Roteiro> ObterRoteiros(Item item)
		{
			try
			{
				return _da.ObterRoteiros(item);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}

		#endregion

		#region Validacao

		public bool ValidarAssociarAtividade(int roteiroSetor, int atividadeId)
		{
			return _validar.ValidarAssociarAtividade(roteiroSetor, atividadeId);
		}

		#endregion

	}
}