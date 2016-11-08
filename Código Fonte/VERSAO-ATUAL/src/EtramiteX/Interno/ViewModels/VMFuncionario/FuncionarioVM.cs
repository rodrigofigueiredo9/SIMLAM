using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFuncionario
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

        public Arquivo Arquivo { get; set; }


        public string ArquivoContentType { get; set; }
        public string ArquivoExtensao { get; set; }
        public int? ArquivoId { get; set; }
        public string ArquivoNome { get; set; }
        public string ArquivoTemporarioNome { get; set; }


		public FuncionarioVM()
		{
			AlterarSenha = true;
		}
	}
}