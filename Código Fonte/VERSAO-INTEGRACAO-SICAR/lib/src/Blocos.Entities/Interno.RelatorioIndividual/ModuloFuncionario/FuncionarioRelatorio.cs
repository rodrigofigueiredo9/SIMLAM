

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFuncionario
{
	public class FuncionarioRelatorio
	{
		public int Id { get; set; }
		public int Situacao { get; set; }	// Ativo = 1, Bloqueado = 2, Cadastrado = 3, Ausente = 4 (ficou algum tempo sem atividade depois de logar), Alterar Senha = 5 (admin trocou a senha), Senha Vencida = 6
		public int Tipo { get; set; }
		public int Tentativa { get; set; }
		public int CargoId { get; set; }
		public int SetorId { get; set; }
		public String Tid { get; set; }
		public String Nome { get; set; }
		public String Cpf { get; set; }
		public String Email { get; set; }
		public String TipoTexto { get; set; }
		public String UltimaVisita { get; set; }
		public String SessionId { get; set; }
		public String SituacaoMotivo { get; set; }
		public String CargoTexto { get; set; }
		public bool IsSistema { get; set; }
		public bool Logado { get; set; }
		public bool ForcarLogout { get; set; }
		public bool AlterarSenha { get; set; }
	}
}