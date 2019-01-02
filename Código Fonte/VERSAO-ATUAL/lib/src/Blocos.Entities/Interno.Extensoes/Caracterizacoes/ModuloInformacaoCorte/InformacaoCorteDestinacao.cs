﻿
using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorteDestinacao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 TipoCorteId { get; set; }

		public Int32 DestinacaoMaterial { get; set; }
		public String DestinacaoMaterialTexto { get; set; }
		public Int32 Produto { get; set; }
		public String ProdutoTexto { get; set; }

		public Decimal Quantidade { get; set; }
	}
}
