using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFichaFundiaria
{
	public class FichaFundiariaVM
	{
		public Boolean IsVisualizar { get; set; }
		public Boolean PodeEditar { get; set; }

		private FichaFundiaria _FichaFundiaria = new FichaFundiaria();
		public FichaFundiaria FichaFundiaria
		{
			get { return _FichaFundiaria; }
			set { _FichaFundiaria = value; }
		}

		public FichaFundiariaVM() { }

		public FichaFundiariaVM(FichaFundiaria entidade, bool isVisualizar = false)
		{
			FichaFundiaria = entidade;
			IsVisualizar = isVisualizar;
		}
	}
}