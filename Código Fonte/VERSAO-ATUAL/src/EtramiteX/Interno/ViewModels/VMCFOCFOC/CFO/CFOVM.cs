using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCFOCFOC.CFO
{
	public class CFOVM
	{
		public EmissaoCFO CFO { get; set; }
		public bool IsVisualizar { get; set; }
		public List<SelectListItem> Produtores { get; set; }
		public List<SelectListItem> Empreendimentos { get; set; }
		public List<SelectListItem> Pragas { get; set; }
		public List<SelectListItem> Estados { get; set; }
		public List<SelectListItem> Municipios { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> UnidadesProducao { get; set; }
		public List<Lista> CFOProdutosEspecificacoes { get; set; }
		public List<SelectListItem> EstadosEmissao { get; set; }
		public List<SelectListItem> MunicipiosEmissao { get; set; }

		public CFOVM(EmissaoCFO cfo, List<Lista> lstProdutores, List<Estado> lstEstados, List<Municipio> lstMunicipios, List<Lista> lstEmpreendimentos,
		List<Lista> lstPragas, List<Lista> lstCFOProdutosEspecificacoes, List<Lista> lstSituacoes, List<Lista> lstUnidadesProducao, List<Municipio> lstMunicipiosEmissao)
		{
			CFO = cfo ?? new EmissaoCFO();

			if (CFO.Id <= 0)
			{
				if (lstProdutores != null && lstProdutores.Count == 1)
				{
					CFO.ProdutorId = Convert.ToInt32(lstProdutores.First().Id);
				}

				if (lstEmpreendimentos != null && lstEmpreendimentos.Count == 1)
				{
					CFO.EmpreendimentoId = Convert.ToInt32(lstEmpreendimentos.First().Id);
				}
			}

			Produtores = ViewModelHelper.CriarSelectList(lstProdutores, false, true, CFO.ProdutorId.ToString());
			Empreendimentos = ViewModelHelper.CriarSelectList(lstEmpreendimentos, false, true, cfo.EmpreendimentoId.ToString());
			Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes, false, true, cfo.SituacaoId.ToString());
			UnidadesProducao = ViewModelHelper.CriarSelectList(lstUnidadesProducao);
			CFOProdutosEspecificacoes = lstCFOProdutosEspecificacoes;
			Pragas = ViewModelHelper.CriarSelectList(lstPragas);
			Estados = ViewModelHelper.CriarSelectList(lstEstados);
			Municipios = ViewModelHelper.CriarSelectList(lstMunicipios);
			EstadosEmissao = ViewModelHelper.CriarSelectList(lstEstados, selecionado: CFO.EstadoEmissaoId.ToString());
			MunicipiosEmissao = ViewModelHelper.CriarSelectList(lstMunicipiosEmissao, selecionado: CFO.MunicipioEmissaoId.ToString());

			if (CFO.EstadoId > 0)
			{
				Estados.Single(x => x.Value == CFO.EstadoId.ToString()).Selected = true;
			}

			if (CFO.MunicipioId > 0)
			{
				Municipios.Single(x => x.Value == CFO.MunicipioId.ToString()).Selected = true;
			}
		}
	}
}