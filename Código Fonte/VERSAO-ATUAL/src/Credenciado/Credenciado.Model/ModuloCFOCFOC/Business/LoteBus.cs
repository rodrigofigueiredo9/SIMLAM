using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC.Lote;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCFOCFOC.Business
{
	public class LoteBus
	{
		#region Propriedades

		private GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private LoteDa _da = new LoteDa();
		private LoteValidar _validar = new LoteValidar();

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		#region DML

		public void Salvar(Lote lote)
		{
			try
			{
				if (lote.Id <= 0)
				{
					lote.Numero = _da.ObterNumero(lote.EmpreendimentoId);
				}

				if (!_validar.Salvar(lote))
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(lote, bancoDeDados);

					Validacao.Add(Mensagem.Lote.Salvar(lote.NumeroCompleto));

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public bool Excluir(int id)
		{
			try
			{
				if (!_validar.Excluir(id))
				{
					return false;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.Lote.ExcluidoSucesso);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void AlterarSituacaoLote(int id, eLoteSituacao situacao, BancoDeDados banco = null)
		{
			try
			{
				if (!_validar.LoteSituacao(id, banco))
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

					_da.AlterarSituacaoLote(id, situacao, bancoDeDados);

					//Validacao.Add(Mensagem.Lote.SituacaoAlterdoSucesso);

					bancoDeDados.Commit();
				}
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
		}

		#endregion

		#region Obter

		public List<ListaValor> ObterEmpreendimentosResponsaveis()
		{
			try
			{
				UnidadeConsolidacaoInternoBus unidadeConsolidacaoInternoBus = new UnidadeConsolidacaoInternoBus();
				return unidadeConsolidacaoInternoBus.ObterEmpreendimentosResponsaveis();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Lote Obter(int id)
		{
			try
			{
				return _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<Lote> Filtrar(Lote filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<Lote> filtro = new Filtro<Lote>(filtrosListar, paginacao);
				Resultados<Lote> resultados = _da.Filtrar(filtro);

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

		public Lista ObterCodigoUC(int id)
		{
			try
			{
				return _da.ObterCodigoUC(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> ObterCultivar(int origemTipo, int origemID, int culturaID)
		{
			try
			{
				if (origemTipo == (int)eDocumentoFitossanitarioTipo.CFCFR || origemTipo == (int)eDocumentoFitossanitarioTipo.TF)
				{
					CulturaInternoBus culturaInternoBus = new CulturaInternoBus();
					return culturaInternoBus.ObterLstCultivar(culturaID);
				}
				else
				{
					return _da.ObterCultivar(origemTipo, origemID, culturaID);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Lista> ObterUnidadeMedida(int origemTipo, int origemID, int culturaID, int cultivarID, out decimal Quantidade)
		{
            Quantidade = 0;
			try
			{
               
				if (origemTipo == (int)eDocumentoFitossanitarioTipo.CFCFR || origemTipo == (int)eDocumentoFitossanitarioTipo.TF)
				{
					return ListaCredenciadoBus.PTVUnidadeMedida;
				}
				else
				{
					return _da.ObterUnidadeMedida(origemTipo, origemID, culturaID, cultivarID, out Quantidade);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		public bool VerificarSeDocumentoUtilizadoPorMesmaUC(int origemID, int empreendimentoID)
		{
			try
			{
				return _da.VerificarSeDocumentoUtilizadoPorMesmaUC(origemID, empreendimentoID);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool VerificarSeCfoJaAssociadaALote(int origemID)
		{
			try
			{
				return _da.VerificarSeCfoJaAssociadaALote(origemID);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}

		public bool VerificarSeDocumentoJaAssociadaALote(int origemID, eDocumentoFitossanitarioTipo origemTipo)
		{
			try
			{
				return _da.VerificarSeDocumentoJaAssociadaALote(origemID, (int)origemTipo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}
	}
}