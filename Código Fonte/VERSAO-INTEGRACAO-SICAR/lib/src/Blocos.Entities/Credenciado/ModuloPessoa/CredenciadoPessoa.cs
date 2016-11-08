using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa
{
	public class CredenciadoPessoa
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public String TipoTexto { get; set; }
		public String Chave { get; set; }
		public String UltimaVisita { get; set; }
		public String SessionId { get; set; }
		public int Situacao { get; set; }
		public int Tipo { get; set; }
		public int Tentativa { get; set; }
		public bool Logado { get; set; }
		public bool ForcarLogout { get; set; }
		public bool AlterarSenha { get; set; }
		public bool IsUsuario { get; set; }

		

		public Int32 OrgaoParceiroId { get; set; }
		public String OrgaoParceiroSiglaNome { get; set; }
		public Int32 OrgaoParceiroUnidadeId { get; set; }
		public String OrgaoParceiroUnidadeSiglaNome { get; set; }

		public Usuario Usuario { get; set; }
		public Pessoa Pessoa { get; set; }
		public List<Permissao> Permissoes { get; set; }		
		public List<Papel> Papeis { get; set; }
		
		public String RegistroNumero
		{
			get
			{
				if (Pessoa != null && Pessoa.Fisica != null && Pessoa.Fisica.Profissao !=null)
				{
					return Pessoa.Fisica.Profissao.Registro;
				}

				return string.Empty;
			}
			
		}

		public String Email
		{
			get
			{
				if (Pessoa != null && Pessoa.MeiosContatos != null && Pessoa.MeiosContatos.Count > 0)
				{
					return (Pessoa.MeiosContatos.FirstOrDefault(x => x.TipoContato == eTipoContato.Email) ?? new Contato()).Valor;
				}

				return string.Empty;
			}
			
		}

		public string Nome
		{
			get { return Pessoa.NomeRazaoSocial; }
			set
			{
				if (Pessoa.IsFisica)
				{
					Pessoa.Fisica.Nome = value;
				}
				else
				{
					Pessoa.Juridica.RazaoSocial = value;
				}
			}
		}

		public CredenciadoPessoa()
		{
			Usuario = new Usuario();
			Pessoa = new Pessoa();
			Permissoes = new List<Permissao>();
			Papeis = new List<Papel>();
		}
	}
}