using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA;
using Tecnomapas.Blocos.Entities.WebService;
using System.Xml.Serialization;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business
{
	public class LiberacaoNumeroCFOCFOCBus
	{
		#region Propriedades

		LiberacaoNumeroCFOCFOCValidar _validar = new LiberacaoNumeroCFOCFOCValidar();
		LiberacaoNumeroCFOCFOCDa _da = new LiberacaoNumeroCFOCFOCDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Validações

		public void VerificarCPF(string cpf)
		{
			try
			{
				_validar.ValidarCPF(cpf);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		public void Salvar(LiberaracaoNumeroCFOCFOC liberacao)
		{
			try
			{
				if (!_validar.Salvar(liberacao))
				{
					return;
				}

				#region Configurando Objeto

				if (liberacao.LiberarBlocoCFO)
				{
					liberacao.SituacaoBlocoCFO = 1;
				}

				if (liberacao.LiberarBlocoCFOC)
				{
					liberacao.SituacaoBlocoCFOC = 1;
				}

				if (liberacao.LiberarDigitalCFO)
				{
					liberacao.SituacaoNumeroDigitalCFO = 1;
				}

				if (liberacao.LiberarDigitalCFOC)
				{
					liberacao.SituacaoNumeroDigitalCFOC = 1;
				}

				#endregion

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados banco = BancoDeDados.ObterInstancia())
				{
					banco.IniciarTransacao();

					_da.Salvar(liberacao, banco);

					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.SalvoSucesso);

					banco.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

        public void VerificarDUA(string numero, string cpfCnpj, LiberaracaoNumeroCFOCFOC liberacao = null)
        {
            try
            {
				var wSDUA = new WSDUA();
				var dua = wSDUA.ObterDUA(numero, cpfCnpj);
				cpfCnpj = cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", "");

				_validar.ValidarDadosWebServiceDuaCFO(dua, numero, cpfCnpj, liberacao);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
        }

		public Resultados<ListarResultados> Filtrar(ListarFiltro filtroListar, Paginacao paginacao)
		{
			Resultados<ListarResultados> retorno = new Resultados<ListarResultados>();
			Filtro<ListarFiltro> filtros = new Filtro<ListarFiltro>(filtroListar, paginacao);

			try
			{
				retorno = _da.Filtrar(filtros);

				if (retorno == null || retorno.Itens.Count < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public LiberaracaoNumeroCFOCFOC Obter(int id)
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

		public void VerificarCPFConsulta(string cpf)
		{
			_validar.ValidarCPFConsulta(cpf);
		}

		public List<NumeroCFOCFOC> FiltrarConsulta(ConsultaFiltro filtro)
		{
			List<NumeroCFOCFOC> retorno = new List<NumeroCFOCFOC>();
			try
			{
				if (!_validar.Filtrar(filtro))
				{
					return retorno;
				}

				retorno = _da.FiltrarConsulta(filtro);

				if (retorno == null || retorno.Count <= 0)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public void Cancelar(NumeroCFOCFOC objeto, BancoDeDados banco = null)
		{
			try
			{
				if (!_validar.Cancelar(objeto))
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Cancelar(objeto, bancoDeDados);

					switch ((eDocumentoFitossanitarioTipo)objeto.Tipo)
					{
						case eDocumentoFitossanitarioTipo.CFO:
							EmissaoCFOBus busCFO = new EmissaoCFOBus();
							busCFO.Cancelar(new EmissaoCFO() { Numero = objeto.Numero.ToString(), Serie = objeto.Serie });
							break;

						case eDocumentoFitossanitarioTipo.CFOC:
							EmissaoCFOCBus busCFOC = new EmissaoCFOCBus();
                            busCFOC.Cancelar(new EmissaoCFOC() { Numero = objeto.Numero.ToString(), Serie = objeto.Serie });
							break;
					}

					if(!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return;
					}

					bancoDeDados.Commit();

                    String numero = !string.IsNullOrWhiteSpace(objeto.Serie) ? objeto.Numero.ToString() + "/" + objeto.Serie : objeto.Numero.ToString();

					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroCanceladoSucesso(numero));
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}
	}
}