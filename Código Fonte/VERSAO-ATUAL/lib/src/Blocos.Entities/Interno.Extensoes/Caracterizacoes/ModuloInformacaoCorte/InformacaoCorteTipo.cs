
using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorteTipo
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Corte { get; set; }

		public Int32 TipoCorte { get; set; }
		public Int32 EspecieInformada { get; set; }

		public Decimal AreaCorte { get; set; }
		public Int32 IdadePlantio { get; set; }

		private List<InformacaoCorteDestinacao> _informacaoCorteDestinacao = new List<InformacaoCorteDestinacao>();
		public List<InformacaoCorteDestinacao> InformacaoCorteDestinacao
		{
			get { return _informacaoCorteDestinacao; }
			set { _informacaoCorteDestinacao = value; }
		}
	}
}
