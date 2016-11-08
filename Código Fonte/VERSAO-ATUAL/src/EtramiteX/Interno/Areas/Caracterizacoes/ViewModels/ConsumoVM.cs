using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class ConsumoVM
	{
		public Consumo Consumo { get; set; }

		public string Indice { get; set; }
		public bool IsVisualizar { get; set; }
		public bool PossuiFonte { get; set; }
		public List<SelectListItem> Atividade { get; set; }
		public List<SelectListItem> FonteTipos { get; set; }
		public List<SelectListItem> Unidades { get; set; }
		public TituloAdicionarVM LicencaVM { get; set; }

		public ConsumoVM() { }

		public ConsumoVM(Consumo consumo, List<ListaValor> atividades, List<ListaValor> fonteTipos, List<ListaValor> unidades, List<TituloModeloLst> modelosLicenca, string indice, bool isVisualizar = false)
		{
			Indice = indice;
			IsVisualizar = isVisualizar;
			Consumo = consumo;

			LicencaVM = new TituloAdicionarVM(modelosLicenca, consumo.Licenca, indice, isVisualizar);

			PossuiFonte = 
				!(consumo.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.ComercianteMotosserra)
				|| consumo.Atividade == ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.FabricanteMotosserra));

			Atividade = ViewModelHelper.CriarSelectList(atividades, true, true, consumo.Atividade.ToString());
			FonteTipos = ViewModelHelper.CriarSelectList(fonteTipos, true, true);
			Unidades = ViewModelHelper.CriarSelectList(unidades, true, true);

			if (consumo.Atividade > 0 && !atividades.Exists(x => x.Id == consumo.Atividade))
			{
				Atividade.Insert(1, new SelectListItem() { Value = consumo.Atividade.ToString(), Text = (consumo.AtividadeCategoria + " - " + consumo.AtividadeNome), Selected = true });
			}
		}
	}
}