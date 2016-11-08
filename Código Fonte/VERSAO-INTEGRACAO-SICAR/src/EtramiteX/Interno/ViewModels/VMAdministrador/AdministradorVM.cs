using System;
using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAdministrador
{
	public class AdministradorVM
	{
		public String Nome { get; set; }
		public String Login { get; set; }
		public String Email { get; set; }
		public String Cpf { get; set; }
		public String Senha { get; set; }
		public String ConfirmarSenha { get; set; }
		public List<PapeisVME> papeis { get; set; }
		public String forcaSenha { get; set; }
		public bool AlterarSenha { get; set; }
	}
}