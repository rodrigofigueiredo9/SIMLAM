namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static AlterarSenhaMsg _alterarSenha = new AlterarSenhaMsg();

		public static AlterarSenhaMsg AlterarSenha
		{
			get { return _alterarSenha; }
		}
	}

	public class AlterarSenhaMsg
	{
		public Mensagem ObrigatorioLogin { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Login", Texto = "É necessário informar Login para alterar a senha" }; } }
		public Mensagem ObrigatorioSenha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Senha", Texto = "É necessário informar Senha para alterar a senha" }; } }
		public Mensagem ObrigatorioNovaSenha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovaSenha", Texto = "É necessário informar Nova Senha para alterar a senha" }; } }
		public Mensagem ObrigatorioConfirmarNovaSenha { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "ConfirmarNovaSenha", Texto = "É necessário informar Confirmar a senha nova para alterar a senha" }; } }

		public Mensagem NovaSenhaDiferente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "NovaSenha", Texto = "\"Nova senha\" e \"Confirmar a senha\" são diferentes" }; } }


	}
}