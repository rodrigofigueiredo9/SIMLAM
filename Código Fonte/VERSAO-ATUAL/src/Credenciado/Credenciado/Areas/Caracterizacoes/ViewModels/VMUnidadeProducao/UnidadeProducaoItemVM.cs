using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMUnidadeProducao
{
	public class UnidadeProducaoItemVM
	{
		public bool IsVisualizar { get; set; }
		public List<SelectListItem> TiposCoordenada { get; set; }
		public List<SelectListItem> Datuns { get; set; }
		public List<SelectListItem> Fusos { get; set; }
		public List<SelectListItem> Hemisferios { get; set; }
		public List<SelectListItem> LstCultivar { get; set; }
		public UnidadeProducaoItem UnidadeProducaoItem { get; set; }
		public List<SelectListItem> LstProdutores { get; set; }

		public string IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Frutos = eUnidadeProducaoTipoProducao.Frutos,
					@Madeira = eUnidadeProducaoTipoProducao.Madeira,
					@MaterialPropagacao = eUnidadeProducaoTipoProducao.MaterialPropagacao
				});
			}
		}

		public string Mensagens
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

		public UnidadeProducaoItemVM(UnidadeProducaoItem item, List<CoordenadaTipo> lstTiposCoordenada = null, List<Datum> lstDatuns = null,
									 List<Fuso> lstFusos = null, List<CoordenadaHemisferio> lstHemisferios = null, List<Lista> lstCultivar = null, List<PessoaLst> lstProdutores = null)
		{
			this.UnidadeProducaoItem = item ?? new UnidadeProducaoItem();
			this.TiposCoordenada = ViewModelHelper.CriarSelectList(lstTiposCoordenada.Where(x => x.Id == 3).ToList(), true, false);
			this.Datuns = ViewModelHelper.CriarSelectList(lstDatuns.Where(x => x.Id == 1).ToList(), true, false);
			this.Fusos = ViewModelHelper.CriarSelectList(lstFusos.Where(x => x.Id == 24).ToList(), true, false);
			this.Hemisferios = ViewModelHelper.CriarSelectList(lstHemisferios.Where(x => x.Id == 1).ToList(), true, false);
			this.LstCultivar = ViewModelHelper.CriarSelectList(lstCultivar, false, true);
			this.LstProdutores = ViewModelHelper.CriarSelectList(lstProdutores);
		}
	}
}