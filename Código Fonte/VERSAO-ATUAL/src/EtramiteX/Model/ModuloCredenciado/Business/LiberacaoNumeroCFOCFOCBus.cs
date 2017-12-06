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

        public void VerificarDUA(int filaID, string numero, string cpfCnpj, LiberaracaoNumeroCFOCFOC liberacao = null)
        {
            try
            {
                DUA dua = new DUA();

                var duaRequisicao = _da.BuscarRespostaConsultaDUA(filaID);

                if (duaRequisicao == null)
                    return;

                if (!duaRequisicao.Sucesso)
                {
                    Validacao.Add(Mensagem.PTV.ErroAoConsultarDua);
                    return;
                }

                var xser = new XmlSerializer(typeof(RespostaConsultaDua));

                RespostaConsultaDua xml = null;

                try
                {
                    xml = (RespostaConsultaDua)xser.Deserialize(new StringReader(duaRequisicao.Resultado));
                }
                catch
                {
                    Validacao.Add(Mensagem.PTV.ErroAoConsultarDua);
                    return;
                }

                if (xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua == null)
                {
                    Validacao.Add(Mensagem.PTV.ErroSefaz(xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.XMotivo));
                    return;
                }

                dua.OrgaoSigla = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Orgao.XSigla;
                dua.ServicoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Area.CArea;

                dua.ReferenciaData = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Data.DRef;
                dua.CPF = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cpf;
                dua.CNPJ = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cnpj;

                dua.ReceitaValor = (float)xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Rece.VRece;
                dua.PagamentoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Pgto.CPgto;
                dua.ValorTotal = float.Parse(xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Valor.VTot);
                dua.CodigoServicoRef = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Serv.CServ;

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

					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroCanceladoSucesso(objeto.Numero.ToString()));
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}
	}
}