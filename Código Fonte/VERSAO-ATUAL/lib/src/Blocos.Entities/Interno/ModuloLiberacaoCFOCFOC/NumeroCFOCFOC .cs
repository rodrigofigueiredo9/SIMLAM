using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC
{
	public class NumeroCFOCFOC
	{
		public int Id { get; set; }
		public long Numero { get; set; }
		public int Tipo { get; set; }
		public bool Situacao { get; set; }
		public bool Utilizado { get; set; }
		public int TipoNumero { get; set; }
        public string Serie { get; set; }
		public string Motivo { get; set; }
		public string TipoDocumentoTexto { 
			get 
			{
				return (eCFOCFOCTipo)Tipo == eCFOCFOCTipo.CFO ? "CFO" : "CFOC";
			} 
		}

		public string UtilizadoTexto
		{
			get
			{
				return Utilizado ? "Sim" : "Não";
			}
		}

		public string SituacaoTexto
		{
			get
			{
				return Situacao ? "Válido" : "Cancelado";
			}
		}

		public NumeroCFOCFOC() 
		{

		}
	}
}
