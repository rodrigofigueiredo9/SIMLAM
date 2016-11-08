using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMMotosserra
{
	public class MotosserraVM
	{
		public Boolean IsVisualizar { get; set; }
		public Motosserra Motosserra { get; set; }

		public MotosserraVM(Motosserra motosserra = null)
		{
			Motosserra = motosserra ?? new Motosserra();
		}
	}
}