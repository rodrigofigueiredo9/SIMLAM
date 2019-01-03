using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Business
{
	public class DuaValidar
	{
		#region Propriedades

		CaracterizacaoBus _busCaracterizacao = null;
		ProjetoGeograficoBus _busProjetoGeografico = null;
		ProjetoDigitalCredenciadoBus _busProjetoDigital = null;
		RequerimentoCredenciadoBus _busRequerimento = null;
		DuaDa _daDua = null;
		DuaInternoDa _DuaInternoDa = null;
		RequerimentoCredenciadoValidar _requerimentoValidar = null;
        TituloCredenciadoBus _busTitulo = null;
		CredenciadoBus _busCred = new CredenciadoBus();

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public DuaValidar()
		{
			_requerimentoValidar = new RequerimentoCredenciadoValidar();
			_busCaracterizacao = new CaracterizacaoBus();
			_busProjetoGeografico = new ProjetoGeograficoBus();
			_busProjetoDigital = new ProjetoDigitalCredenciadoBus();
			_busRequerimento = new RequerimentoCredenciadoBus();
			_daDua = new DuaDa();
			_DuaInternoDa = new DuaInternoDa();
            _busTitulo = new TituloCredenciadoBus();
            
		}

		#endregion

	}
}