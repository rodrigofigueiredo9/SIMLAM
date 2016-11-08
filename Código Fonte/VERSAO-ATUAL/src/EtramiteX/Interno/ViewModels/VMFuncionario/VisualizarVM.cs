using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario
{
	public class VisualizarVM
	{
		public Funcionario Funcionario { get; set; }
		public string TextoPermissoes { get; set; }
		public List<PapeisVME> Papeis { get; set; }

		public VisualizarVM()
		{
			Funcionario = new Funcionario();
			Papeis = new List<PapeisVME>();
		}
	}
}