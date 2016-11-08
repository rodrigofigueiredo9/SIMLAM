namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CertificadoRegistroMsg _certificadoRegistroMsg = new CertificadoRegistroMsg();
		public static CertificadoRegistroMsg CertificadoRegistro
		{
			get { return _certificadoRegistroMsg; }
			set { _certificadoRegistroMsg = value; }
		}
	}

	public class CertificadoRegistroMsg
	{
		public Mensagem RequerimentoSemPendencias { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "RequerimentoPadrao", Texto = "O requerimento não pode ser selecionado, pois não possui pendências." }; } }
		public Mensagem ClassificacaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Classificacao", Texto = "A classificação é obrigatória." }; } }
		public Mensagem RegistroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Registro", Texto = "O número do registro é obrigatório." }; } }
	}
}