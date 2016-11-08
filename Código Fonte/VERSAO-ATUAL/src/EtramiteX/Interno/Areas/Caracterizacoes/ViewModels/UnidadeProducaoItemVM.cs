using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class UnidadeProducaoItemVM
	{
		public Boolean IsVisualizar { get; set; }
		public UnidadeProducaoItem UnidadeProducaoItem { get; set; }
		public List<SelectListItem> LstCultivar { get; set; }
		public List<SelectListItem> LstUnidadeMedida { get; set; }
		public List<SelectListItem> TiposCoordenada { get; set; }
		public List<SelectListItem> Datuns { get; set; }
		public List<SelectListItem> Fusos { get; set; }
		public List<SelectListItem> Hemisferios { get; set; }
		public List<SelectListItem> LstProdutores { get; set; }

		public String IdsTelaTipoProducao
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Frutos = eUnidadeProducaoTipoProducao.Frutos,
					@Madeira = eUnidadeProducaoTipoProducao.Madeira,
					MaterialPropagacao = eUnidadeProducaoTipoProducao.MaterialPropagacao
				});
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ResponsavelTecnicoObrigatorio = Mensagem.UnidadeProducao.ResponsavelTecnicoObrigatorio,
					@ResponsavelTecnicoJaAdicionado = Mensagem.UnidadeProducao.ResponsavelTecnicoJaAdicionado,
					@NumeroARTObrigatorio = Mensagem.UnidadeProducao.NumeroARTObrigatorio,
					@DataValidadeARTObrigatorio = Mensagem.UnidadeProducao.DataValidadeARTObrigatorio,
					@CodigoUnidadeProducaoUnidadeObrigatorio = Mensagem.UnidadeProducao.CodigoUPObrigatorio,
					@EstimativaProducaoUnidadeMedidaObrigatorio = Mensagem.UnidadeProducao.EstimativaProducaoUnidadeMedidaObrigatorio,
					@EstimativaProducaoQuantidadeAnoObrigatorio = Mensagem.UnidadeProducao.EstimativaProducaoQuantidadeAnoObrigatorio,
					@DataPlantioAnoProducaoObrigatorio = Mensagem.UnidadeProducao.DataPlantioAnoProducaoObrigatorio,
					@CultivarObrigatorio = Mensagem.UnidadeProducao.CultivarObrigatorio,
					@ProdutorObrigatorio = Mensagem.UnidadeProducao.ProdutorObrigatorio,
					@ProdutorJaAdicionado = Mensagem.UnidadeProducao.ProdutorJaAdicionado,
					@AreaHAObrigatorio = Mensagem.UnidadeProducao.AreaHAObrigatorio,
					@DataValidadeRenasemObrigatorio = Mensagem.UnidadeProducao.DataValidadeRenasemObrigatorio,
					@NumeroRenasemObrigatorio = Mensagem.UnidadeProducao.NumeroRenasemObrigatorio,
				});
			}
		}

		public UnidadeProducaoItemVM()
		{
			UnidadeProducaoItem = new UnidadeProducaoItem();
			LstProdutores = new List<SelectListItem>();
			LstUnidadeMedida = new List<SelectListItem>();
			TiposCoordenada = new List<SelectListItem>();
			Datuns = new List<SelectListItem>();
			Fusos = new List<SelectListItem>();
			Hemisferios = new List<SelectListItem>();
		}
	}
}