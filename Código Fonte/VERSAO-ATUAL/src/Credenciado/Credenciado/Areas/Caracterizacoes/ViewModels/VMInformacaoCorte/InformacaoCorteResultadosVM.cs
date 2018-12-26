using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels.VMInformacaoCorte
{
	public class InformacaoCorteResultadosVM
	{
		public int? Id { get; set; }
		public int? DestinacaoId { get; set; }
		public string TipoCorte { get; set; }
		public string Especie { get; set; }
		public Decimal AreaCorte { get; set; }
		public string IdadePlantio { get; set; }
		public Int32 DestinacaoMaterialTexto { get; set; }
		public Int32 ProdutoTexto { get; set; }
		public Decimal Quantidade { get; set; }
		public int Linhas { get; set; }
	}
}