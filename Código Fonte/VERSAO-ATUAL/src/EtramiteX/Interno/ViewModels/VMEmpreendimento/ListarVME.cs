

using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMEmpreendimento
{
	public class ListarVME
	{
		private Empreendimento _empreendimento = new Empreendimento();
		public Empreendimento Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}
	}
}