using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPapel;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado
{
	public class CredenciadoIntEnt
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public Usuario Usuario { get; set; }
		public Pessoa Pessoa { get; set; }
		public int Situacao { get; set; }	// Aguardando Ativação = 1, Ativo = 2, Bloqueado = 3, Cadastrado = 3, Senha Vencida = 4
		public String SituacaoTexto { get; set; }
		public String SituacaoMotivo { get; set; }
		public int Tipo { get; set; }
		public int TipoCredenciado { get; set; }
		public String TipoTexto { get; set; }
		public String Chave { get; set; }
		public String Email { get; set; }







		public String UltimaVisita { get; set; }
		public int Tentativa { get; set; }
		public String SessionId { get; set; }
		public bool Logado { get; set; }
		public bool ForcarLogout { get; set; }
		public bool AlterarSenha { get; set; }

		public string Nome
		{
			get { return Pessoa.Fisica.Nome; }
			set { Pessoa.Fisica.Nome = value; }
		}

		private List<Permissao> _permissoes = new List<Permissao>();
		public List<Permissao> Permissoes
		{
			get { return _permissoes; }
			set { _permissoes = value; }
		}

		private List<Papel> _papeis = new List<Papel>();
		public List<Papel> Papeis
		{
			get { return _papeis; }
			set { _papeis = value; }
		}

		public CredenciadoIntEnt()
		{
			Usuario = new Usuario();
			Pessoa = new Pessoa();
		}
	}
}