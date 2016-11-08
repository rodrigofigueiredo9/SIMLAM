using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMOrgaosParceirosConveniados
{
    public class GerenciarVM
    {
        public string Sigla { get; set; }
        public string NomeOrgao { get; set; }
		public int IdOrgao { get; set; }
        private List<SelectListItem> _unidades;
		private ListarCredenciadoParceiroVM _credenciadosAguardandoAtivacao;
		private ListarCredenciadoParceiroVM _credenciadosAtivos;
		private ListarCredenciadoParceiroVM _credenciadosBloqueados;

		public ListarCredenciadoParceiroVM CredenciadosAguardandoAtivacao
		{
			get { return _credenciadosAguardandoAtivacao; }
			set { _credenciadosAguardandoAtivacao = value; }
		}
		
		public ListarCredenciadoParceiroVM CredenciadosAtivos
		{
			get { return _credenciadosAtivos; }
			set { _credenciadosAtivos = value; }
		}
		
		public ListarCredenciadoParceiroVM CredenciadosBloqueados
		{
			get { return _credenciadosBloqueados; }
			set { _credenciadosBloqueados = value; }
		}

		public List<SelectListItem> Unidades
		{
			get { return _unidades; }
			set { _unidades = value; }
		}

		public string Mensagens 
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SelecioneUmCredenciado = Mensagem.OrgaoParceiroConveniado.SelecioneUmCredenciado,
					@ConfirmGerarChave = Mensagem.OrgaoParceiroConveniado.ConfirmGerarChave,
					@TituloConfirmGerarChave = Mensagem.OrgaoParceiroConveniado.TituloConfirmGerarChave,
					@TituloBloquear = Mensagem.OrgaoParceiroConveniado.TituloConfirmBloquearCredenciado,
					@ConfirmBloquear = Mensagem.OrgaoParceiroConveniado.ConfirmBloquearCredenciado,
					@TituloDesbloquear = Mensagem.OrgaoParceiroConveniado.TituloConfirmDesbloquearCredenciado,
					@ConfirmDesbloquear = Mensagem.OrgaoParceiroConveniado.ConfirmDesbloquearCredenciado,
				});
			}
		}
    }
}