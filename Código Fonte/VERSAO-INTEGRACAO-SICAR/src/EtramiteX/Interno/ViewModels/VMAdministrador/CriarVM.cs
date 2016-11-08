using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador
{
	public class CriarVM
	{
		public Administrador Administrador { get; set; }
		public Boolean CpfValido { get; set; }
		public string TextoPermissoes { get; set; }

		public List<PapeisVME> Papeis { get; set; }

		public CriarVM()
		{
			Administrador = new Administrador();
			Papeis = new List<PapeisVME>();
		}
	}
}
