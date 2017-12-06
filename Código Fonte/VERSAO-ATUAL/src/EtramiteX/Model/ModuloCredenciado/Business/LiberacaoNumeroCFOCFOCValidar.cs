using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloConfiguracaoDocumentoFitossanitario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA;
using Tecnomapas.Blocos.Entities.WebService;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using System.Xml.Serialization;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Business
{
	public class LiberacaoNumeroCFOCFOCValidar
	{
		#region Propriedades

		LiberacaoNumeroCFOCFOCDa _da = new LiberacaoNumeroCFOCFOCDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		ConfiguracaoDocumentoFitossanitarioBus _busConfiguracaoCfoCfoc = new ConfiguracaoDocumentoFitossanitarioBus();


        private Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus _PTVBusCred = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Business.PTVBus();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		internal void ValidarCPF(string cpf)
		{
			if (string.IsNullOrEmpty(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFObrigatorio);
				return;
			}

			if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CpfInvalido);
				return;
			}

			if (!_da.VerificarCPFAssociadoCredenciado(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFNaoAssociadoACredenciado);
				return;
			}

			if (!_da.PossuiRegistroOrgaoClasse(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFNaoPossuiRegistroOrgaoClasse);
				return;
			}

			int habilitacaoId = _da.ObterIDHabilitacaoCFOCFOC(cpf);

			if (habilitacaoId < 1)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFNaoPossuiHabilitacaoCFOCFOC);
				return;
			}

			HabilitarEmissaoCFOCFOC habilitacao = new HabilitarEmissaoCFOCFOCBus().Obter(habilitacaoId, isEditar: true);
			if (habilitacao.Situacao == (int)eHabilitacaoCFOCFOCSituacao.Inativo)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.HabilitacaoCFOCCFOCInativa);
			}
		}

        internal bool ValidarDadosWebServiceDuaCFO(DUA dua, string numero, string cpfCnpj, LiberaracaoNumeroCFOCFOC liberacao = null)
        {
            if (dua == null)
            {
                Validacao.Add(Mensagem.PTV.DuaNaoEncontrado);
                return Validacao.EhValido;
            }

            if (dua.PagamentoCodigo != "2"/*Pago e Consolidado*/ && dua.PagamentoCodigo != "1"/*Pago e não Consolidado*/)
            {
                Validacao.Add(Mensagem.PTV.DuaInvalido(numero));
            }

            if (dua.CodigoServicoRef != "21213")
            {
                Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CodigoDuaInvalido);
            }

            if (liberacao != null)
            {
                int quantidadeDuaEmitido = _da.ObterQuantidadeDuaEmitidos(numero, cpfCnpj);

                float ValorUnitario = _da.ObterValorUnitarioDua();

                int totalPagos = (int)((float)dua.ValorTotal / (float)ValorUnitario);

                long totalCfo = liberacao.NumeroFinalCFO - liberacao.NumeroInicialCFO;
                long totalCfoc = liberacao.NumeroFinalCFOC - liberacao.NumeroInicialCFOC;

                totalCfo += liberacao.QuantidadeDigitalCFO + liberacao.QuantidadeDigitalCFOC;

                if ((totalPagos * 25) < ( quantidadeDuaEmitido + totalCfo + totalCfoc) )
                {
                    Validacao.Add(Mensagem.PTV.DuaSemSaldo(numero));
                }
            }

            return Validacao.EhValido;
        }


		internal bool Salvar(LiberaracaoNumeroCFOCFOC liberacao)
		{
			ValidarCPF(liberacao.CPF);

			if (!(liberacao.LiberarBlocoCFO) && !(liberacao.LiberarBlocoCFOC) && !(liberacao.LiberarDigitalCFO) && !(liberacao.LiberarDigitalCFOC))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.MarqueUmTipoNumero);
				return false;
			}

            
            //try
            //{
            //    DUA dua = new DUA();

            //    var duaRequisicao = _da.BuscarRespostaConsultaDUA(liberacao.FilaID);

            //    if (duaRequisicao == null)
            //        return false;

            //    if (!duaRequisicao.Sucesso)
            //    {
            //        Validacao.Add(Mensagem.PTV.ErroAoConsultarDua);
            //        return false;
            //    }

            //    var xser = new XmlSerializer(typeof(RespostaConsultaDua));

            //    RespostaConsultaDua xml = null;

            //    try
            //    {
            //        xml = (RespostaConsultaDua)xser.Deserialize(new StringReader(duaRequisicao.Resultado));
            //    }
            //    catch
            //    {
            //        Validacao.Add(Mensagem.PTV.ErroAoConsultarDua);
            //        return false;
            //    }

            //    if (xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua == null)
            //    {
            //        Validacao.Add(Mensagem.PTV.ErroSefaz(xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.XMotivo));
            //        return false;
            //    }

            //    dua.OrgaoSigla = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Orgao.XSigla;
            //    dua.ServicoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Area.CArea;

            //    dua.ReferenciaData = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Data.DRef;
            //    dua.CPF = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cpf;
            //    dua.CNPJ = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cnpj;

            //    dua.ReceitaValor = (float)xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Rece.VRece;
            //    dua.PagamentoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Pgto.CPgto;
            //    dua.ValorTotal = float.Parse(xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Valor.VTot.Replace(".",","));
            //    dua.CodigoServicoRef = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Serv.CServ;

            //    ValidarDadosWebServiceDuaCFO(dua, liberacao.NumeroDua, liberacao.CPF, liberacao);
            //}
            //catch (Exception exc)
            //{
            //    Validacao.AddErro(exc);
            //}

			if (liberacao.LiberarBlocoCFO)
			{
				ValidarBlocoCFO(liberacao);
			}

			if (liberacao.LiberarBlocoCFOC)
			{
				ValidarBlocoCFOC(liberacao);
			}

			if (liberacao.LiberarDigitalCFO)
			{
				ValidarNumeroDigitalCFO(liberacao);
			}

			if (liberacao.LiberarDigitalCFOC)
			{
				ValidarNumeroDigitalCFOC(liberacao);
			}

			return Validacao.EhValido;
		}

		private void ValidarNumeroDigitalCFO(LiberaracaoNumeroCFOCFOC liberacao)
		{
			if (liberacao.QuantidadeDigitalCFO <= 0)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeNumeroDigitalCFONaoPodeSerMenorIgualZero);
			}

			if (_da.DigitalPossuiNumeroCFONaoConfigurado(liberacao.QuantidadeDigitalCFO))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeNumerosDigitaisCFONaoPodeExcederAConfiguradaNoSistema);
			}

			if (!_da.VerificarQuantidadeMaximaNumDigitalCadastradoCFO(liberacao))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeMaximaNumeroDigitalCFOCadastradosUltrapassa50);
			}
		}

		private void ValidarNumeroDigitalCFOC(LiberaracaoNumeroCFOCFOC liberacao)
		{
			if (liberacao.QuantidadeDigitalCFOC <= 0)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeNumeroDigitalCFOCNaoPodeSerMenorIgualZero);
			}

			if (_da.DigitalPossuiNumeroCFOCNaoConfigurado(liberacao.QuantidadeDigitalCFOC))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeNumerosDigitaisCFOCNaoPodeExcederAConfiguradaNoSistema);
			}

			if (!_da.VerificarQuantidadeMaximaNumDigitalCadastradoCFOC(liberacao))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeMaximaNumeroDigitalCFOCCadastradosUltrapassa50);
			}
		}

		private void ValidarBlocoCFOC(LiberaracaoNumeroCFOCFOC liberacao)
		{
			if (liberacao.NumeroInicialCFOC <= 0)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroInicialCFOCObrigatorio);
			}
			else if (liberacao.NumeroInicialCFOC.ToString().Length != 8)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.InicialQuantidadeInvalida(eDocumentoFitossanitarioTipo.CFOC.ToString()));
            }

            if (liberacao.NumeroInicialCFOC.ToString().Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2))
            {
                Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.AnoCFOCInvalido);
            }

			if (liberacao.NumeroFinalCFOC <= 0)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroInicialCFOCObrigatorio);
			}
			else if (liberacao.NumeroFinalCFOC.ToString().Length != 8)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.FinalQuantidadeInvalida(eDocumentoFitossanitarioTipo.CFOC.ToString()));
            }

            if (liberacao.NumeroFinalCFOC.ToString().Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2))
            {
                Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.AnoCFOCInvalido);
            }

			if (liberacao.NumeroFinalCFOC < liberacao.NumeroInicialCFOC)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroFinalNaoPodeSerMaiorInicialCFOC);
			}

			if (!Validacao.EhValido)
			{
				return;
			}

			if (_da.BlocoPossuiNumeroCFOCNaoConfigurado(liberacao.NumeroInicialCFOC, liberacao.NumeroFinalCFOC))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.IntervaloCFOCNaoConfigurado);
				return;
			}

			if (!_da.VerificarNumeroCFOCJaAtribuido(liberacao.NumeroInicialCFO, liberacao.NumeroFinalCFO))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.IntervaloCFOCJaExiste);
				return;
			}

			if (((liberacao.NumeroFinalCFOC - liberacao.NumeroInicialCFOC) + 1) < 25)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeCFOCDeveSerIgual25);
				return;
			}

			if (!_da.ValidarBlocoQuantidadeCadastradaCFOC(liberacao))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeCFOCCadastradaNaoPodeUltrapassar25);
			}
		}

		private void ValidarBlocoCFO(LiberaracaoNumeroCFOCFOC liberacao)
		{
			if (liberacao.NumeroInicialCFO <= 0)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroInicialCFOObrigatorio);
			}
			else if (liberacao.NumeroInicialCFO.ToString().Length != 8)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.InicialQuantidadeInvalida(eDocumentoFitossanitarioTipo.CFO.ToString()));
            }

            if (liberacao.NumeroInicialCFO.ToString().Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2))
            {
                Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.AnoCFOInvalido);
            }

			if (liberacao.NumeroFinalCFO <= 0)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroFinalCFOObrigatorio);
			}
			else if (liberacao.NumeroFinalCFO.ToString().Length != 8)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.FinalQuantidadeInvalida(eDocumentoFitossanitarioTipo.CFO.ToString()));
            }

            if (liberacao.NumeroFinalCFO.ToString().Substring(2, 2) != DateTime.Now.Year.ToString().Substring(2))
            {
                Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.AnoCFOInvalido);
            }

			if (liberacao.NumeroFinalCFO < liberacao.NumeroInicialCFO)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.NumeroFinalNaoPodeSerMaiorInicialCFO);
			}

			if(!Validacao.EhValido)
			{
				return;
			}

			if (_da.BlocoPossuiNumeroCFONaoConfigurado(liberacao.NumeroInicialCFO, liberacao.NumeroFinalCFO))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.IntervaloCFONaoConfigurado);
				return;
			}

			if (!_da.VerificarNumeroCFOJaAtribuido(liberacao.NumeroInicialCFO, liberacao.NumeroFinalCFO))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.IntervaloCFOJaExiste);
				return;
			}

			if (((liberacao.NumeroFinalCFO - liberacao.NumeroInicialCFO) + 1) < 25)
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeCFODeveSerIgual25);
				return;
			}

			if (!_da.ValidarBlocoQuantidadeCadastradaCFO(liberacao))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.QuantidadeCFOCadastradaNaoPodeUltrapassar25);
			}
		}

		internal void ValidarCPFConsulta(string cpf)
		{
			if (string.IsNullOrEmpty(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFObrigatorio);
				return;
			}

			if (!ValidacoesGenericasBus.Cpf(cpf))
			{
				Validacao.Add(Mensagem.Pessoa.CpfInvalido);
				return;
			}

			if (!_da.VerificarCPFAssociadoALiberacao(cpf))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CPFNaoAssociadoAUmaLiberacao);
			}
		}

		internal bool Filtrar(ConsultaFiltro filtro)
		{
			if (!string.IsNullOrEmpty(filtro.DataInicialEmissao))
			{
				if(!ValidacoesGenericasBus.ValidarData(filtro.DataInicialEmissao))
				{
					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.DataInvalida("Data inicial da emissão", "DataInicial" + ((eDocumentoFitossanitarioTipoNumero)filtro.TipoNumero).ToString()));
				}
			}

			if (!string.IsNullOrEmpty(filtro.DataFinalEmissao))
			{
				if (!ValidacoesGenericasBus.ValidarData(filtro.DataFinalEmissao))
				{
					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.DataInvalida("Data final da emissão", "DataFinal" + ((eDocumentoFitossanitarioTipoNumero)filtro.TipoNumero).ToString()));
				}
			}

			return Validacao.EhValido;
		}

		internal bool Cancelar(NumeroCFOCFOC objeto)
		{
			if (_da.NumeroCancelado(objeto.Id))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.CancelarSituacaoInvalida(objeto.TipoDocumentoTexto, objeto.Numero.ToString()));
				return false;
			}

			int utilizadoID = 0;
			switch ((eDocumentoFitossanitarioTipo)objeto.Tipo)
			{
				case eDocumentoFitossanitarioTipo.CFO:
					EmissaoCFODa daCFO = new EmissaoCFODa();
					utilizadoID = daCFO.NumeroUtilizado(objeto.Numero);
					break;

				case eDocumentoFitossanitarioTipo.CFOC:
					EmissaoCFOCDa daCFOC = new EmissaoCFOCDa();
					utilizadoID = daCFOC.NumeroUtilizado(objeto.Numero);
					break;
			}

			if(utilizadoID > 0)
			{
				var aux = _da.CFOCFOCJaAssociado(objeto.Tipo, utilizadoID);

				if(aux.Count == 1)
				{
					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.DocumentoJaAssociado(objeto.TipoDocumentoTexto, objeto.Numero.ToString(), aux.First().Key, aux.First().Value));
				}
				else if (aux.Count > 1)
				{
					Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.DocumentoJaAssociadoEOutros(objeto.TipoDocumentoTexto, objeto.Numero.ToString(), aux.First().Key, aux.First().Value));
				}
			}

			if (string.IsNullOrWhiteSpace(objeto.Motivo))
			{
				Validacao.Add(Mensagem.LiberacaoNumeroCFOCFOC.MotivoCancelamentoObrigatorio);
			}

			return Validacao.EhValido;
		}
	}
}