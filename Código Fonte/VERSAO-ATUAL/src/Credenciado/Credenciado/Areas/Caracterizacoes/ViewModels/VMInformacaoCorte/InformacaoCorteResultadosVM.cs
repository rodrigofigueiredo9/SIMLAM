﻿using System;
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
		public int TipoCorte { get; set; }
		public string TipoCorteTexto { get; set; }
		public int Especie { get; set; }
		public string EspecieTexto { get; set; }
		public Decimal AreaCorte { get; set; }
		public int IdadePlantio { get; set; }
		public int DestinacaoMaterial { get; set; }
		public string DestinacaoMaterialTexto { get; set; }
		public int Produto { get; set; }
		public string ProdutoTexto { get; set; }
		public int Quantidade { get; set; }
		public int Linhas { get; set; }
	}
}