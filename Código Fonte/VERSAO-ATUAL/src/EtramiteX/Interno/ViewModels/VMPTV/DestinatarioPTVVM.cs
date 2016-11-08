using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV
{
	public class DestinatarioPTVVM
	{
		public bool IsVisualizar { get; set; }

		private DestinatarioPTV _destinatario = new DestinatarioPTV();
		public DestinatarioPTV Destinatario
		{
			get { return _destinatario; }
			set { _destinatario = value; }
		}

		private List<SelectListItem> _uf = new List<SelectListItem>();
		public List<SelectListItem> Uf
		{
			get { return _uf; }
			set { _uf = value; }
		}

		private List<SelectListItem> _municipios = new List<SelectListItem>();
		public List<SelectListItem> Municipios
		{
			get { return _municipios; }
			set { _municipios = value; }
		}

		public DestinatarioPTVVM(DestinatarioPTV destinatario, List<Estado> uf, List<Municipio> municipios, bool isVisualizar = false)
		{
			Destinatario = destinatario;
			IsVisualizar = isVisualizar;
			Uf = ViewModelHelper.CriarSelectList(uf, isFiltrarAtivo: true, selecionado: destinatario.EstadoID.ToString());
			Municipios = ViewModelHelper.CriarSelectList(municipios, isFiltrarAtivo: true, selecionado: destinatario.MunicipioID.ToString());
		}
	}
}