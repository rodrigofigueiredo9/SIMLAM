using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;

namespace Tecnomapas.EtramiteX.Gerencial.ViewModels.VMFuncionario
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