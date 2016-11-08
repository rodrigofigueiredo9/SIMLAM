using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPTVOutro
{
	public class PTVOutroVM
	{
		public bool IsVisualizar { get; set; }
		public PTVOutro PTV { get; set; }
		public List<SelectListItem> OrigemTipoList { get; set; }

		private List<SelectListItem> _estados = new List<SelectListItem>();
		public List<SelectListItem> Estados { get; set; }

		private List<SelectListItem> _municipios = new List<SelectListItem>();
		public List<SelectListItem> Municipios { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> LstUnidades { get; set; }
		public List<SelectListItem> EstadosInteressado { get; set; }
		public List<SelectListItem> MunicipiosInteressado { get; set; }
		public List<SelectListItem> UnidadeMedida { get; set; }

		public PTVOutroVM() { }

		public PTVOutroVM(
			PTVOutro ptv,
			List<Lista> lstSituacoes,
			List<Lista> lsOrigem,
			List<Estado> estados,
			List<Municipio> municipios,
			List<Estado> estadosInteressado,
			List<Municipio> municipiosInteressado,
			bool isVisualizar = false)
		{
			this.PTV = ptv ?? new PTVOutro();
			this.IsVisualizar = IsVisualizar;

			this.EstadosInteressado = ViewModelHelper.CriarSelectList(estadosInteressado, true, true, ptv.InteressadoEstadoId.ToString()); //estadosInteressado;
			this.MunicipiosInteressado = ViewModelHelper.CriarSelectList(municipiosInteressado, true, true, ptv.InteressadoMunicipioId.ToString()); //municipiosInteressado;

			this.Estados = ViewModelHelper.CriarSelectList(estados, true, true, ptv.Estado.ToString());
			this.Municipios = ViewModelHelper.CriarSelectList(municipios, true, true, ptv.Municipio.ToString());

			this.Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes, true, true, ptv.Situacao.ToString());

			OrigemTipoList = ViewModelHelper.CriarSelectList(lsOrigem.OrderBy(x => x.Id).ToList(), true, true);
		}
	}
}