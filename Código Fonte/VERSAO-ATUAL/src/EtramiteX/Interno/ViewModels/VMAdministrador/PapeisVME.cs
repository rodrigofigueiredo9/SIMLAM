



using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador
{
	public class PapeisVME
	{
		public PapeisVME()
		{
			Papel = new Papel();
			IsAtivo = false;
		}
		public Papel Papel { get; set; }
		public bool IsAtivo { get; set; }
	}
}