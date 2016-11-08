using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class LaudoLaboratorial
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public string Nome { get; set; }
		public string LaudoResultadoAnalise { get; set; }
		public int Estado { get; set; }
		public string EstadoTexto { get; set; }
		public int Municipio { get; set; }
		public string MunicipioTexto { get; set; }
	}
}
