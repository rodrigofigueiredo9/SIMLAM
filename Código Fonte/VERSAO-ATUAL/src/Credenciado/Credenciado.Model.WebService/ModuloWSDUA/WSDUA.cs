using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Tecnomapas.Blocos.Entities.WebService;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA
{
	public class WSDUA
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public DUA ObterDUAPF(string DUANumero, string DUACPF)
		{
			try
			{
				string duaSenhaCertificado = _configSys.Obter<String>(ConfiguracaoSistema.KeyDUASenhaCertificado);

				//var duaService = new DuaEService(HttpContext.Current.Server.MapPath(@"~/Content/_chave/Chaves Pública e Privada.pfx"), duaSenhaCertificado,"http://localhost:8888");
				var duaService = new DuaEService(HttpContext.Current.Server.MapPath(@"~/Content/_chave/Chaves Pública e Privada.pfx"), duaSenhaCertificado);

				var resultado = duaService.ConsultaDuaCPF(DUANumero, DUACPF.Replace(".", "").Replace("-", ""));

				var xser = new XmlSerializer(typeof(RespostaConsultaDua));
				var xml = (RespostaConsultaDua)xser.Deserialize(new StringReader(resultado));

				if (xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua == null)
				{
					return null;
				}

				DUA retorno = new DUA();

				retorno.OrgaoSigla = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Orgao.XSigla;
				retorno.ServicoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Area.CArea;

				retorno.ReferenciaData = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Data.DRef;
				retorno.CPF = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cpf;

				retorno.ReceitaValor = (float)xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Rece.VRece;
				retorno.PagamentoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Pgto.CPgto;

				return retorno;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public DUA ObterDUAPJ(string DUANumero, string DUACNPJ)
		{
			try
			{
				string duaSenhaCertificado = _configSys.Obter<String>(ConfiguracaoSistema.KeyDUASenhaCertificado);

				//var duaService = new DuaEService(HttpContext.Current.Server.MapPath(@"~/Content/_chave/Chaves Pública e Privada.pfx"), duaSenhaCertificado,"http://localhost:8888");
				var duaService = new DuaEService(HttpContext.Current.Server.MapPath(@"~/Content/_chave/Chaves Pública e Privada.pfx"), duaSenhaCertificado);

				var resultado = duaService.ConsultaDuaCNPJ(DUANumero, DUACNPJ.Replace(".", "").Replace("/", "").Replace("-", ""));

				var xser = new XmlSerializer(typeof(RespostaConsultaDua));
				var xml = (RespostaConsultaDua)xser.Deserialize(new StringReader(resultado));

				if (xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua == null)
				{
					return null;
				}

				DUA retorno = new DUA();

				retorno.OrgaoSigla = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Orgao.XSigla;
				retorno.ServicoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Area.CArea;

				retorno.ReferenciaData = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Data.DRef;
				retorno.CNPJ = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Contri.Cnpj;

				retorno.ReceitaValor = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Rece.VRece;
				retorno.PagamentoCodigo = xml.Body.DuaConsultaResponse.DuaConsultaResult.RetConsDua.Dua.InfDUAe.Pgto.CPgto;

				return retorno;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}