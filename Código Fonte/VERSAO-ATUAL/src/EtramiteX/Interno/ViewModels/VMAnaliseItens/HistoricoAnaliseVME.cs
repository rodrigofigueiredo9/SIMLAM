

using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens
{
	public class HistoricoAnaliseVME
	{
		public HistoricoAnalise Historico { get; set; }

		public HistoricoAnaliseVME() { }
		public HistoricoAnaliseVME(HistoricoAnalise historico) 
		{
			Historico = historico;
		}	
	}
}