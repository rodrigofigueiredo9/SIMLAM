namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital
{
	public class MotivoRecusaVM
	{
		public int ProjetoId { get; set; }
		public string Motivo { get; set; }
		public bool IsVisualizar { get; set; }



		public MotivoRecusaVM(bool visualizar)
		{
			IsVisualizar = visualizar;
		}
	
		public MotivoRecusaVM()
		{

		}
	}
}