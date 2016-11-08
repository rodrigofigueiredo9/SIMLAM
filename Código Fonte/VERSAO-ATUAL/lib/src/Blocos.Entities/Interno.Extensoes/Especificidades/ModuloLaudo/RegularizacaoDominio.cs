using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo
{
	public class RegularizacaoDominio
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 DominioId { get; set; }
		public String Comprovacao { get; set; }
		public String AreaCroqui { get; set; }

		public String ComprovacaoAreaCroqui { get { return Comprovacao + " - " + AreaCroqui; } }

		public RegularizacaoDominio()
		{

		}
	}
}
