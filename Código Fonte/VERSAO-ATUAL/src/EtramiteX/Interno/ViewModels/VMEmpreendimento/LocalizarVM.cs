using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento
{
	public class LocalizarVM
	{
		public List<SelectListItem> Segmentos { get; set; }
		public List<SelectListItem> TiposResponsavel { get; set; }
		public List<SelectListItem> TiposCoordenada { get; set; }
		public List<SelectListItem> Datuns { get; set; }
		public List<SelectListItem> Fusos { get; set; }
		public List<SelectListItem> Hemisferios { get; set; }
		public List<SelectListItem> Estados { get; set; }
		public List<SelectListItem> Municipios { get; set; }
		public Boolean ModoEmpreendimento { get; set; }
		public Int32 EstadoDefault { get; set; }
		public String EstadoDefaultSigla { get; set; }
		public Int32 MunicipioDefault { get; set; }

		public Paginacao Paginacao { get; set; }
		public ListarEmpreendimentoFiltro Filtros { get; set; }
		public List<LocalizarVME> Resultados { get; set; }

		public Boolean PodeEditar { get; set; }
		public Boolean PodeVisualizar { get; set; }
		public Boolean IsInformacaoCorte { get; set; }

		public LocalizarVM()
		{
			Paginacao = new Paginacao();
			Filtros = new ListarEmpreendimentoFiltro();
			Resultados = new List<LocalizarVME>();

			EstadoDefault = ViewModelHelper.EstadoDefaultId();
			MunicipioDefault = ViewModelHelper.MunicipioDefaultId();
		}

		public LocalizarVM(List<Estado> lstEstados, List<Municipio> lstMunicipios, List<Segmento> lstSegmentos, List<CoordenadaTipo> lstTiposCoordenada,
			List<Datum> lstDatuns, List<Fuso> lstFusos, List<CoordenadaHemisferio> lstHemisferios)
		{
			Paginacao = new Paginacao();
			Filtros = new ListarEmpreendimentoFiltro();
			Resultados = new List<LocalizarVME>();

			EstadoDefault = ViewModelHelper.EstadoDefaultId();
			EstadoDefaultSigla = ViewModelHelper.EstadoDefaultSigla();
			MunicipioDefault = ViewModelHelper.MunicipioDefaultId();

			Estados = ViewModelHelper.CriarSelectList(lstEstados, true, selecionado: EstadoDefault.ToString());
			Municipios = ViewModelHelper.CriarSelectList(lstMunicipios, true, selecionado: MunicipioDefault.ToString());
			Segmentos = ViewModelHelper.CriarSelectList(lstSegmentos, true);

			TiposCoordenada = ViewModelHelper.CriarSelectList(lstTiposCoordenada.Where(x => x.Id == 3).ToList(), true, false);//UTM
			Datuns = ViewModelHelper.CriarSelectList(lstDatuns.Where(x => x.Id == 1).ToList(), true, false);//SIRGAS2000
			Fusos = ViewModelHelper.CriarSelectList(lstFusos.Where(x => x.Id == 24).ToList(), true, false);
			Hemisferios = ViewModelHelper.CriarSelectList(lstHemisferios.Where(x => x.Id == 1).ToList(), true, false);//Sul
		}

		public void SetResultados(List<Empreendimento> resultados)
		{
			Resultados = new List<LocalizarVME>();

			if (resultados != null && resultados.Count > 0)
			{
				LocalizarVME resultadoItem = null;

				foreach (Empreendimento item in resultados)
				{
					resultadoItem = new LocalizarVME();
					resultadoItem.Id = item.Id;
					resultadoItem.SegmentoTexto = item.SegmentoTexto;
					resultadoItem.Denominador = item.Denominador;
					resultadoItem.Cnpj = item.CNPJ;
					resultadoItem.Codigo = item.Codigo;
					
					Resultados.Add(resultadoItem);
				}
			}
		}
	}
}