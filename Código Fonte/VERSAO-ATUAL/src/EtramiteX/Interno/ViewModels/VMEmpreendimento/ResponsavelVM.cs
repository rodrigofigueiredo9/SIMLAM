using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento
{
	public class ResponsavelVM
	{
		public Responsavel Responsavel = new Responsavel();
		public List<SelectListItem> TiposResponsavel = new List<SelectListItem>();

		public ResponsavelVM() { }

		public ResponsavelVM(Responsavel responsavel, List<TipoResponsavel> lstResponsaveis)
		{
			TiposResponsavel = ViewModelHelper.CriarSelectList(lstResponsaveis, selecionado: (responsavel.Tipo ?? 0).ToString());
			this.Responsavel = responsavel;
		}
	}
}
