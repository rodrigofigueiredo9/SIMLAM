



using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario
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