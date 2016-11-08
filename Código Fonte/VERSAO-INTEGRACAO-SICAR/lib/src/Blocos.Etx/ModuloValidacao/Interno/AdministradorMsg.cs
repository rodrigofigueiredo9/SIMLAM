

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AdministradorMsg _msg = new AdministradorMsg();
		public static AdministradorMsg Administrador
		{
			get { return _msg; }
		}
	}

	public class AdministradorMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Administrador salvo com sucesso." }; } }
		public Mensagem Editar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Administrador editado com sucesso." }; } }
		public Mensagem AlterarSituacao { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Situação alterada com sucesso." }; } }

		public Mensagem SemPermissao { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Você não tem permissão para acessar essa funcionalidade." }; } }
		

		public Mensagem NomeObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Nome", Texto = "Nome é obrigatório." }; } }
		public Mensagem EmailObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Email", Texto = "E-mail é obrigatório." }; } }
		public Mensagem EmailInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Email", Texto = "E-mail inválido." }; } }
		public Mensagem LoginObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "Login é obrigatório." }; } }
		public Mensagem LoginExistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "O login já está sendo utilizado por um administrador." }; } }
		public Mensagem SenhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = "Senha é obrigatório." }; } }
		public Mensagem ConfirmarSenhaObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConfirmarSenha", Texto = "Confirmar senha é obrigatório." }; } }
		public Mensagem SenhaDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = "A confirmação de senha está diferente da senha." }; } }
		public Mensagem CpfObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cpf", Texto = "CPF é obrigatório." }; } }
		public Mensagem CpfInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Cpf", Texto = "CPF é inválido." }; } }
		public Mensagem NaoEncontrouRegistros { get { return Mensagem.Padrao.NaoEncontrouRegistros; } }
		public Mensagem MotivoAusente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Motivo", Texto = "O motivo da ausência é obrigatório." }; } }
		public Mensagem Inexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Administrador inexistente." }; } }
		public Mensagem CpfEncontrado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = "O CPF já está associado a um administrador." }; } }
		public Mensagem NovaSitucaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovaSituacao", Texto = "Nova situação é obrigatória." }; } }
		public Mensagem FormatoLogin { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "Formato do login é inválido, use apenas letras e números." }; } }

		public Mensagem AlterarSenhaAdministrador { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "A senha foi alterada com sucesso." }; } }
		public Mensagem AlterarAdministradorOutro { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Não é permitido alterar outro administrador." }; } }

		public Mensagem TransferirSistemaNaoEhAdmin { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "O funcionário destinado a receber as permissões de sistema deve ser um administrador." }; } }
		public Mensagem TransferirSistemaJaEhSistema { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "O funcionário destinado a receber as permissões de sistema já as possui." }; } }
		public Mensagem TransferirSistemaMotivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "O motivo da transferência é obrigatório." }; } }

		public Mensagem SenhaTamanho(int configuracao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = String.Format("A senha precisa ter no mínimo {0} caracteres.", configuracao) };
		}

		public Mensagem LoginTamanho(int configuracao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = String.Format("O login precisa ter no mínimo {0} caracteres.", configuracao) };
		}

		public Mensagem SetorComResponsavel(string setor)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Setor", Texto = String.Format("O Setor {0} já possui responsável.", setor) };
		}

		public Mensagem Transferido { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "Permissões de sistema transferidas com sucesso." }; } }
	}
}


