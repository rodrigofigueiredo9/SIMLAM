namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoUsuario : ConfiguracaoBase
	{
		public const string keyTamanhoSenha = "TamanhoSenha";
		public int TamanhoSenha { get { return 6; } }

		public const string keyTamanhoMinLogin = "TamanhoMinLogin";
		public int TamanhoMinLogin { get { return 6; } }

		public const string keyQtdVerificaoUltimaSenha = "QtdVerificaoUltimaSenha";
		public int QtdVerificaoUltimaSenha { get { return 5; } }

		public const string keySenhaExpiracaoDias = "SenhaExpiracaoDias";
		public int SenhaExpiracaoDias { get { return 90; } }
	}
}