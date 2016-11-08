namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CertidaoAnuenciaMsg _certidaoAnuencia = new CertidaoAnuenciaMsg();
		public static CertidaoAnuenciaMsg CertidaoAnuencia
		{
			get { return _certidaoAnuencia; }
			set { _certidaoAnuencia = value; }
		}
	}

	public class CertidaoAnuenciaMsg
	{
		public Mensagem CertificacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CertidaoAnuencia_Certificacao", Texto = @"Certificação é obrigatória." }; } }
	}
}
