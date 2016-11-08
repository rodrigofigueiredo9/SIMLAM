using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMHabilitarEmissaoCFOCFOC
{
	public class HabilitarEmissaoCFOCFOCVM
	{
		public HabilitarEmissaoCFOCFOC HabilitarEmissao { get; set; }
		//Tela
		public Boolean IsVisualizar { get; set; }		
		public Boolean IsAjaxRequest { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> SituacaoMotivos { get; set; }
		public List<SelectListItem> Estados { get; set; }

		public String ObterJSon(Object objeto)
		{
			return ViewModelHelper.JsSerializer.Serialize(objeto);
		}

		public HabilitarEmissaoCFOCFOCVM(List<Situacao> situacoes, List<Estado> estados, List<Lista> motivos)
		{
			SituacaoMotivos = ViewModelHelper.CriarSelectList(motivos, true);
			Situacoes = ViewModelHelper.CriarSelectList(situacoes, true);
			Estados = ViewModelHelper.CriarSelectList(estados, itemTextoPadrao: false);
			HabilitarEmissao = new HabilitarEmissaoCFOCFOC();
			IsVisualizar = false;			
			IsAjaxRequest = false;
		}
		public HabilitarEmissaoCFOCFOCVM()
		{
			Situacoes = new List<SelectListItem>();
			Estados = new List<SelectListItem>();
			HabilitarEmissao = new HabilitarEmissaoCFOCFOC();
			IsVisualizar = false;
			IsAjaxRequest = false;
		}
	}
}