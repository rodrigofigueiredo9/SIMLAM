using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Especificidades.ViewModels.Outros
{
	public class OutrosInformacaoCorteVM
	{
		public bool IsVisualizar { get; set; }

		private OutrosInformacaoCorte _outros = new OutrosInformacaoCorte();
		public OutrosInformacaoCorte Outros
		{
			get { return _outros; }
			set { _outros = value; }
		}

		private List<SelectListItem> _atividade = new List<SelectListItem>();
		public List<SelectListItem> Atividade
		{
			get { return _atividade; }
			set { _atividade = value; }
		}

		private List<SelectListItem> _vinculoPropriedade = new List<SelectListItem>();
		public List<SelectListItem> VinculoPropriedade
		{
			get { return _vinculoPropriedade; }
			set { _vinculoPropriedade = value; }
		}

		public OutrosInformacaoCorteVM() { }

		public OutrosInformacaoCorteVM(OutrosInformacaoCorte outros, List<AtividadeSolicitada> atividades, List<Lista> vincPropriedade, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Outros = outros;
			//Atividade = ViewModelHelper.CriarSelectList(atividades, true, true, outros.Atividade.ToString());
			//VinculoPropriedade = ViewModelHelper.CriarSelectList(vincPropriedade, true, true, outros.VinculoPropriedade.ToString());
		}
	}
}