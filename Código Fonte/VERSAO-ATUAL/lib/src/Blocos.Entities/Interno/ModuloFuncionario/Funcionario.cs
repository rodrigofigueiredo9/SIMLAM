using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Autenticacao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario
{
	public class Funcionario
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public Usuario Usuario { get; set; }
		public String Nome { get; set; }
		public String Cpf { get; set; }

        public Arquivo.Arquivo Arquivo { get; set; }

		public int Situacao { get; set; }	// Ativo = 1, Bloqueado = 2, Cadastrado = 3, Ausente = 4 (ficou algum tempo sem atividade depois de logar), Alterar Senha = 5 (admin trocou a senha), Senha Vencida = 6
		public string SituacaoTexto { get; set; }
		public string SituacaoMotivo { get; set; }
		public String Email { get; set; }
		public int Tipo { get; set; }
		public String TipoTexto { get; set; }
		public String UltimaVisita { get; set; }
		public int Tentativa { get; set; }
		public String SessionId { get; set; }
		public bool Logado { get; set; }
		public bool ForcarLogout { get; set; }
		public bool AlterarSenha { get; set; }
		public int CargoId { get; set; }
		public int SetorId { get; set; }
		public String CargosStragg { get; set; }
		public String SetoresStragg { get; set; }
		public bool IsSistema {get; set; }

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

		private List<Cargo> _cargos = new List<Cargo>();
		public List<Cargo> Cargos
		{
			get { return _cargos; }
			set { _cargos = value; }
		}

		private List<Setor> _setores = new List<Setor>();
		public List<Setor> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		public Funcionario()
		{
			Usuario = new Usuario();
            Arquivo = new Blocos.Arquivo.Arquivo();
		}

		public Executor Executor()
		{
			Executor executor = new Executor();
			executor.Id = Id;
			executor.Login = Usuario.Login;
			executor.Tid = Tid;
			executor.Tipo = eExecutorTipo.Interno;
			executor.Nome = Nome;
			return executor;
		}
	}
}