using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC
{
	public class LiberaracaoNumeroCFOCFOC
	{
		public int Id { get; set; }
		public int CredenciadoId { get; set; }
		public string CPF { get; set; }
		public string Nome { get; set; }

        public string NumeroDua { get; set; }

        public int FilaID { get; set; } 

		#region Numero de Bloco CFO

		public bool LiberarBlocoCFO { get; set; }
		public long NumeroInicialCFO { get; set; }
		public long NumeroFinalCFO { get; set; }

		#endregion

		#region Numero de Bloco CFOC

		public bool LiberarBlocoCFOC { get; set; }
		public long NumeroInicialCFOC { get; set; }
		public long NumeroFinalCFOC { get; set; }

		#endregion

		#region Numero Digital CFO

		public bool LiberarDigitalCFO { get; set; }
		public int QuantidadeDigitalCFO { get; set; }

		#endregion

		#region  Numero Digital CFOC

		public bool LiberarDigitalCFOC { get; set; }
		public int QuantidadeDigitalCFOC { get; set; }

		#endregion

		public int SituacaoBlocoCFOC { get; set; }
		public int SituacaoBlocoCFO { get; set; }
		public int SituacaoNumeroDigitalCFO { get; set; }
		public int SituacaoNumeroDigitalCFOC { get; set; }
	}
}