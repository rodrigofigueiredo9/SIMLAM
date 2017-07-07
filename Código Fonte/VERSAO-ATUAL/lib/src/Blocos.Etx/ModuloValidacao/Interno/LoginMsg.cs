

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LoginMsg _loginMsg = new LoginMsg();

		public static LoginMsg Login
		{
			get { return _loginMsg; }
		}
	}

	public class LoginMsg
	{
		public Mensagem ObrigatorioLogin { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "É necessário informar Login para efetuar o acesso ao sistema" }; } }
		public Mensagem ObrigatorioSenha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = "É necessário informar Senha para efetuar o acesso ao sistema" }; } }
		public Mensagem ObrigatorioNovaSenha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovaSenha", Texto = "É necessário informar Senha para efetuar o acesso ao sistema" }; } }
		public Mensagem ObrigatorioConfirmarNovaSenha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConfirmarNovaSenha", Texto = "É necessário informar Senha para efetuar o acesso ao sistema" }; } }

        public Mensagem ObrigatorioCpf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CPF", Texto = "É necessário informar o CPF cadastrado no sistema para solicitar a recuperação de senha." }; } }
        public Mensagem ObrigatorioEmail { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Email", Texto = "É necessário informar o e-mail cadastrado no sistema para solicitar a recuperação de senha." }; } }

		public Mensagem LoginSenhaInvalido { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Login ou senha incorretos. Atenção com as teclas \"Caps Lock\" e \"Shift\", pois o sistema diferencia letras maiúsculas e minúsculas. Por favor tente novamente" }; } }
		public Mensagem SenhaAlterada { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso,   Texto = "A senha foi alterada com sucesso." }; } }

        public Mensagem EmailCPFNaoEncontrados { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi encontrado usuário cadastrado para o e-mail e cpf informados." }; } }

		public Mensagem SituacaoInvalida(string situacao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = String.Format("Usuário {0}. Entre em contato com o administrador", situacao) };
		}

		public Mensagem AguardandoChave { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Usuário aguardando chave de ativação, favor verifique seu e-mail" }; } }

		public Mensagem NumTentativas(Int32 numTentativas, Int32 numMaxTentativas)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = String.Format("Tentativa {0}/{1}. Após {1} tentativas, o usuário será bloqueado", numTentativas, numMaxTentativas) };
		}

		public Mensagem FuncionarioBloqueado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia,   Texto = "Usuário bloqueado. Entre em contato com um administrador" }; } }
		public Mensagem AcessoSimultaneo { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = "Este usuário foi desconectado por acesso simultâneo" }; } }
		public Mensagem SessaoDerrubada { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = "Este usuário estava sendo utilizado em outra sessão que foi desconectada" }; } }

        public Mensagem SenhaNaoEnviada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "Não foi possível gerar uma nova senha." }; } }

		public Mensagem HistoricoSenha(int qtd)
		{
			Mensagem msg = new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "" };

			msg.Texto = String.Format("A nova senha não pode ser igual às {0} últimas senhas utilizadas. Informe uma nova senha.", qtd);
			if (qtd == 1)
			{
				msg.Texto = "A nova senha não pode ser igual a ultima senha utilizada. Informe uma nova senha.";
			}
			return msg;
		}

		public Mensagem AlterarSenhaCadastroNovo { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = "É necessário que você troque a sua senha antes do acesso ao sistema." }; } }
		public Mensagem AlterarSenhaAlteradaPorAdmin { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = "Sua senha foi redefinida pelo administrador. É necessário que você troque a sua senha antes do acesso ao sistema." }; } }
		public Mensagem AlterarSenhaExpirada(int prazoExpiracao)
		{
			return new Mensagem() { Tipo = eTipoMensagem.Informacao,   Texto = String.Format("Faz mais de {0} dias que sua senha não é alterada, para sua segurança é necessário que você troque a sua senha antes do acesso ao sistema.", prazoExpiracao) };
		}

		public Mensagem UsuarioBloqueado { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Texto = "Seu usuário não está ativo." }; } }
	}
}