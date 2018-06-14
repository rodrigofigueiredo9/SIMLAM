using System;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Tecnomapas.Blocos.Entities.WebService;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA
{
	public class WSDUA
	{
		private string _arquivoCertificado { get; set; } = HttpContext.Current.Server.MapPath(@"~/Content/_chave/Chaves Pública e Privada.pfx");
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public WSDUA(string arquivoCertificado = "")
		{
			if (!string.IsNullOrWhiteSpace(arquivoCertificado))
				this._arquivoCertificado = arquivoCertificado;
		}

		public DUA ObterDUA(string numeroDUA, string cpfCnpj, eTipoPessoa tipoPessoa = 0)
		{
			try
			{
				string duaSenhaCertificado = _configSys.Obter<String>(ConfiguracaoSistema.KeyDUASenhaCertificado);

				var duaService = new DuaEService(_arquivoCertificado, duaSenhaCertificado);

				if(tipoPessoa == 0)
					tipoPessoa = cpfCnpj.Contains("/") ? eTipoPessoa.Juridica : eTipoPessoa.Fisica;

				var resultado = duaService.ConsultarDua(numeroDUA, cpfCnpj.Replace(".", "").Replace("-", "").Replace("/", ""), tipoPessoa);

				var xser = new XmlSerializer(typeof(RespostaConsultaDua));

				var xml = (RespostaConsultaDua)xser.Deserialize(new StringReader(resultado));

				if (xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua == null)
				{
					Validacao.Add(Mensagem.PTV.ErroSefaz(xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.XMotivo));
					return null;
				}

				DUA retorno = new DUA();

				retorno.OrgaoSigla = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Orgao.XSigla;
				retorno.ServicoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Area.CArea;
				retorno.ReferenciaData = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Data.DRef;
				retorno.CPF = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cpf;
				retorno.CNPJ = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cnpj;
				retorno.ReceitaValor = (float)xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Rece.VRece;
				retorno.PagamentoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Pgto.CPgto;
				retorno.ValorTotal = float.Parse(xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Valor.VTot);
				retorno.CodigoServicoRef = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Serv.CServ;

				return retorno;
			}
			catch (Exception exc)
			{
				Validacao.Add(Mensagem.PTV.ErroAoConsultarDua);
				Log.Error("CATCH: ", exc);
			}

			return null;
		}
	}
}