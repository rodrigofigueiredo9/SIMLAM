using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloDUA;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Business
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
				HttpClient _client = new HttpClient();

				var apiUri = ConfigurationManager.AppSettings["apiCredenciadoLocal"];
				var token = ConfigurationManager.AppSettings["tokenCredenciado"];

				_client.DefaultRequestHeaders.Add("Authorization", String.Concat("Bearer ", token));
				HttpResponseMessage response = _client.GetAsync($"{apiUri}/SefazDua/EmitirDuaSefaz/titulo/{titulo}").Result;
				if (response.IsSuccessStatusCode)
					Validacao.Add(Mensagem.Dua.Sucesso);
				else
					Validacao.Add(Mensagem.Dua.Falha);
				response.EnsureSuccessStatusCode();

				return response.Content.ReadAsStringAsync().Result;
			}
			catch (Exception exc)
			{
				Validacao.Add(Mensagem.Dua.Falha);
			}

			return null;
		}

		public string Reemitir(string dua)
		{
			try
			{
				HttpClient _client = new HttpClient();

				var apiUri = ConfigurationManager.AppSettings["apiCredenciadoLocal"];
				var token = ConfigurationManager.AppSettings["tokenCredenciado"];

				_client.DefaultRequestHeaders.Add("Authorization", String.Concat("Bearer ", token));
				HttpResponseMessage response = _client.GetAsync($"{apiUri}/SefazDua/RemitirDuaSefaz/NumeroDua/{dua}").Result;
				response.EnsureSuccessStatusCode();

				string responseBody = response.Content.ReadAsStringAsync().Result;

				if (response.IsSuccessStatusCode)
					Validacao.Add(eTipoMensagem.Sucesso, responseBody);
				else
					Validacao.Add(Mensagem.Dua.Falha);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}