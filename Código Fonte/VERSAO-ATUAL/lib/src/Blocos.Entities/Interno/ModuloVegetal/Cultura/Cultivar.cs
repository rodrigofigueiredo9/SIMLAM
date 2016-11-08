using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura
{
	public class Cultivar
	{
		public int IdRelacionamento { get; set; }
		public int Id { get; set; }
		public string Nome { get; set; }
		public string Tid { get; set; }
		public decimal CapacidadeMes { get; set; }
		public int UnidadeMedida { get; set; }
		public string UnidadeMedidaTexto { get; set; }
		public string CulturaTexto { get; set; }
		public Int32 CulturaId { get; set; }
		public int TipoProducao { get; set; }
		public string TipoProducaoTexto { get; set; }
		public int DeclaracaoAdicional { get; set; }
		public string DeclaracaoAdicionalTexto { get; set; }

		private List<CultivarConfiguracao> _lsCultivarConfiguracao = new List<CultivarConfiguracao>();
		public List<CultivarConfiguracao> LsCultivarConfiguracao
		{
			get { return _lsCultivarConfiguracao; }
			set { _lsCultivarConfiguracao = value; }
		}
	}
}