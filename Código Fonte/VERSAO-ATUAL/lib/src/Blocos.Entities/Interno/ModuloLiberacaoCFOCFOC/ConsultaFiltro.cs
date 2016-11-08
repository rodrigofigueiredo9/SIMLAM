using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloLiberacaoCFOCFOC
{
	public class ConsultaFiltro
	{
		public int TipoDocumento { get; set; }
		public long Numero { get; set; }
		public string DataInicialEmissao { get; set; }
		public string DataFinalEmissao { get; set; }
		public int CredenciadoId { get; set; }
		public int TipoNumero { get; set; }
	}
}