using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV
{
	public class PTVListarResultado
	{
		public int ID { get; set; }
		public int NumeroTipo { get; set; }
		public string Numero { get; set; }
		public string Empreendimento { get; set; }
		public string Destinatario { get; set; }
		public string CulturaCultivar { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public int ResponsavelTecnicoId { get; set; }
		public string Tipo { get; set; }
	}
}