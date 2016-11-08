using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;

namespace Tecnomapas.EtramiteX.Interno.Model.Security
{
	public class Controle
	{
		public int ArtefatoTipo { get; set; }
		public int ArtefatoId { get; set; }
		public int Acao { get; set; }
		public int Caracterizacao { get; set; }
		public String Ip { get; set; }
		public EtramiteIdentity Usuario { get; set; }
		public Executor Executor { get; set; }

		public Controle(){}

	}
}
