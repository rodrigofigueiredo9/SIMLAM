

using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMManual
{
	public class ManualVM
	{
		public List<ManualItemVM> Itens { get; set; }

		public ManualVM()
		{
			Itens = new List<ManualItemVM>();
		}
	}
}