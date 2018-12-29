using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business
{
	public class DuaBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		DuaValidar _validar = null;
		ProjetoDigitalCredenciadoBus _busProjetoDigital = null;
		RequerimentoCredenciadoBus _busRequerimento = null;
		DuaDa _da = null;
		DuaInternoDa _daInterno = null;
		AtividadeCredenciadoBus _busAtividade = null;
		ConsultaCredenciado _consultaCredenciado = null;

		public DuaValidar Validar { get { return _validar; } }
		internal ConsultaCredenciado ConsultaCredenciado { get { return _consultaCredenciado; } }

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
			_daInterno = new DuaInternoDa();
			_busProjetoDigital = new ProjetoDigitalCredenciadoBus();
			_busRequerimento = new RequerimentoCredenciadoBus();
			_busAtividade = new AtividadeCredenciadoBus();
			_validar = new DuaValidar();
			_consultaCredenciado = new ConsultaCredenciado();
		}

		public Dua ObterInformacaoCorte(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				return _da.Obter(id, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}