using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Gerencial.ViewModels.VMFuncionario
{
	public class FuncionarioVM
	{
		public String Nome { get; set; }
		public String Login { get; set; }
		public String Email { get; set; }
		public String Cpf { get; set; }
		public String Senha { get; set; }
		public String ConfirmarSenha { get; set; }
		public List<String> ListaCargos { get; set; }
		public List<Setor> ListaSetores { get; set; }
		public List<PapeisVME> papeis { get; set; }
		public String forcaSenha { get; set; }
		public bool AlterarSenha { get; set; }

		public FuncionarioVM()
		{
			AlterarSenha = true;
		}
	}
}