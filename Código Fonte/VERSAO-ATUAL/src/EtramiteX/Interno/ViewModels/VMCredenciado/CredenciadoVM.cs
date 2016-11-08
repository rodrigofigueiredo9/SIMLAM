using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Pes = Tecnomapas.EtramiteX.Interno.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado
{
	public class CredenciadoVM
	{
		public String Chave { get; set; }
		public String Senha { get; set; }
		public String ConfirmarSenha { get; set; }
		public String DataNascimento { get; set; }
		private List<SelectListItem> _orgaosParceiros;
		private List<SelectListItem> _orgaosParceirosUnidades;
		public String Login
		{
			get { return _credenciado.Usuario.Login; }
			set { _credenciado.Usuario.Login = value; }
		}
		
		private Pes.ContatoVME _contato = new Pes.ContatoVME();
		public Pes.ContatoVME Contato
		{
			get { return _contato; }
			set { _contato = value; }
		}

		private Pes.SalvarVM _pessoaVM = new Pes.SalvarVM();
		public Pes.SalvarVM PessoaVM
		{
			get { return _pessoaVM; }
			set { _pessoaVM = value; }
		}

		
		public Pessoa Pessoa
		{
			get { return Credenciado.Pessoa; }
			set { Credenciado.Pessoa = value; }
		}
		
		private CredenciadoPessoa _credenciado = new CredenciadoPessoa();
		public CredenciadoPessoa Credenciado
		{
			get { return _credenciado; }
			set { _credenciado = value; }
		}

		public List<SelectListItem> OrgaosParceiros
		{
			get { return _orgaosParceiros; }
			set { _orgaosParceiros = value; }
		}

		public List<SelectListItem> OrgaosParceirosUnidades
		{
			get { return _orgaosParceirosUnidades; }
			set { _orgaosParceirosUnidades = value; }
		}

		public CredenciadoVM() { }
	}
}