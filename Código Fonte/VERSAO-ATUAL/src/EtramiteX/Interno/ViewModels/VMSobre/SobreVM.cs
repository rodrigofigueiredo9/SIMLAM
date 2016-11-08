using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.ModuloSobre;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMSobre
{
	public class SobreVM
	{
		public Sobre Sobre { get; set; }
		public List<Sobre> Versoes { get; set; }
		public List<Sobre> VersoesAntigas { get { return this.Versoes.Where(x => x.Id != this.Sobre.Id).ToList(); } }

		public SobreVM()
		{
			this.Sobre = new Sobre();
			this.Versoes = new List<Sobre>();
		}
	}
}