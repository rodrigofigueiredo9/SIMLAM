using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloDUA;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloDUA.Business
{
	public class DuaBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		DuaValidar _validar = null;
		DuaDa _da = null;

		public DuaValidar Validar { get { return _validar; } }

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public DuaBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new DuaDa();
			_validar = new DuaValidar();
		}

		public List<Dua> Obter(int titulo, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				return _da.Obter(titulo, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public string Emitir(int titulo)
		{
			try
			{
				RequestJson requestJson = new RequestJson();

				var urlGerar = $"{ConfigurationManager.AppSettings["api"]}SefazDua/EmitirDuaSefaz/titulo/{titulo}";
				var strResposta = requestJson.Executar(urlGerar);
				var resposta = requestJson.Deserializar<dynamic>(strResposta);
				
				return resposta;

			}
			catch (Exception exc)
			{
				//Validacao.AddErro(exc);
				Validacao.AddAdvertencia("Houve um erro ao emitir o Dua");
			}

			return null;
		}

		public string Reemitir(string dua)
		{
			try
			{
				RequestJson requestJson = new RequestJson();

				var urlGerar = $"{ConfigurationManager.AppSettings["api"]}SefazDua/RemitirDuaSefaz/NumeroDua/{dua}";
				var strResposta = requestJson.Executar(urlGerar);
				var resposta = requestJson.Deserializar<dynamic>(strResposta);
				return resposta;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		
	}
}