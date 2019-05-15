using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business
{
	public class AtividadeInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		AtividadeInternoDa _da;
		AtividadeInternoValidar _validar;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public AtividadeInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new AtividadeInternoDa(UsuarioInterno);
			_validar = new AtividadeInternoValidar();
		}

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

		#endregion

		#region Obter / Filtrar

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

		public IProtocolo ObterProtocoloAtividadesSolicitadas(int id)
		{
			ProtocoloInternoBus protocoloBus = new ProtocoloInternoBus();
			IProtocolo protocolo = null;
			Processo processo = null;

			try
			{
				if (protocoloBus.ExisteProtocolo(id))
				{
					protocolo = protocoloBus.ObterProcessosDocumentos(id);

					if (protocolo.IsProcesso)
					{
						processo = protocolo as Processo;

						//remove os processos que não tem atividade
						for (var i = 0; i < processo.Processos.Count; i++)
						{
							processo.Processos[i] = (protocoloBus.ExisteAtividade(processo.Processos[i].Id.Value)) ? processo.Processos[i] : null;
						}
						processo.Processos.RemoveAll(x => x == null);

						//remove os documentos que não tem atividade
						for (var i = 0; i < processo.Documentos.Count; i++)
						{
							processo.Documentos[i] = (protocoloBus.ExisteAtividade(processo.Documentos[i].Id.Value)) ? processo.Documentos[i] : null;
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

		#endregion

		#region Validações

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

		internal bool VerificarDeferir(Atividade atividade, BancoDeDados banco = null)
		{
			if (atividade.Protocolo.Id == 0)
			{
				throw new Exception("erro ao validar deferimento atividade.Protocolo.id = 0");
			}

			return _da.VerificarDeferir(atividade, banco);
		}

		#endregion

		public Atividade ObterAtividadePorCodigo(int codigo)
		{
			return _da.ObterAtividadePorCodigo(codigo);
		}
	}
}