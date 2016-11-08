using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMPessoa;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCredenciado
{
	public class CredenciadoVM
	{
		public String Chave { get; set; }
		public String Senha { get; set; }
		public String ConfirmarSenha { get; set; }
		public String DataNascimento { get; set; }

		private List<SelectListItem> _orgaosParceiros = new List<SelectListItem>();
		private List<SelectListItem> _orgaosParceirosUnidades = new List<SelectListItem>();

		public List<SelectListItem> OrgaosParceirosUnidades
		{
			get { return _orgaosParceirosUnidades; }
			set { _orgaosParceirosUnidades = value; }
		}
		public List<SelectListItem> OrgaosParceiros
		{
			get { return _orgaosParceiros; }
			set { _orgaosParceiros = value; }
		}

		public String Login
		{
			get { return _credenciado.Usuario.Login; }
			set { _credenciado.Usuario.Login = value; }
		}

		private ContatoVME _contato;
		public ContatoVME Contato
		{
			get { return _contato; }
			set { _contato = value; }
		}

		private PessoaVM _pessoaVM = new PessoaVM();
		public PessoaVM PessoaVM
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
	}
}