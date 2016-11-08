using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class UnidadeConsolidacaoVM
	{
		public bool IsVisualizar { get; set; }
		public int ProjetoDigitalId { get; set; }
		public int ProtocoloId { get; set; }
		public int RequerimentoId { get; set; }
		public String UrlRetorno { get; set; }

		public UnidadeConsolidacao UnidadeConsolidacao { get; set; }
		public List<SelectListItem> LstUnidadeMedida { get; set; }

		private List<SelectListItem> _lstCultivar = new List<SelectListItem>();

		public List<SelectListItem> LstCultivar
		{
			get { return _lstCultivar; }
			set { _lstCultivar = value; }
		}

		public UnidadeConsolidacaoVM()
		{
			UnidadeConsolidacao = new UnidadeConsolidacao();
			LstUnidadeMedida = new List<SelectListItem>();
		}

		public String CultivarJson(Cultivar item)
		{
			return ViewModelHelper.Json(new
			{
				@Id = item.Id,
				@IdRelacionamento = item.IdRelacionamento,
				@Nome = item.Nome,
				@CapacidadeMes = item.CapacidadeMes,
				@UnidadeMedida = item.UnidadeMedida
			});
		}

		public String ResponsavelTecnicoJson(ResponsavelTecnico item)
		{
			return ViewModelHelper.Json(new
			{
				@Id = item.Id,
				@IdRelacionamento = item.IdRelacionamento,
				@Nome = item.NomeRazao,
				@CFONumero = item.CFONumero,
				@NumeroArt = item.NumeroArt,
				@ArtCargoFuncao = item.ArtCargoFuncao,
				@DataValidadeART = item.DataValidadeART
			});
		}
	}
}