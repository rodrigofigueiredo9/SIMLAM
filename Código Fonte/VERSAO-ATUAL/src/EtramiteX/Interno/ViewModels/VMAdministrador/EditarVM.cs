using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador
{
	public class EditarVM
	{
		private Administrador _administrador = new Administrador();
		public Administrador Administrador
		{
			get { return _administrador; }
			set { _administrador = value; }
		}
		
		public string TextoPermissoes { get; set; }
		private List<PapeisVME> _papeis = new List<PapeisVME>();
		public List<PapeisVME> Papeis
		{
			get { return _papeis; }
			set { _papeis = value; }
		}

		public bool AlterarSenha { get; set; }

		public EditarVM()
		{
		}
	}
}