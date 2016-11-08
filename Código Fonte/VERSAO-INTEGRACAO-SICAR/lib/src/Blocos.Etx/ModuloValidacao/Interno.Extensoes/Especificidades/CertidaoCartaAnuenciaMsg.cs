namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CertidaoCartaAnuenciaMsg _certidaoCartaAnuenciaMsg = new CertidaoCartaAnuenciaMsg();
		public static CertidaoCartaAnuenciaMsg CertidaoCartaAnuenciaMsg
		{
			get { return _certidaoCartaAnuenciaMsg; }
			set { _certidaoCartaAnuenciaMsg = value; }
		}
	}

	public class CertidaoCartaAnuenciaMsg
	{
		public Mensagem DescricaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certidao_Descricao", Texto = @"Descrição é obrigatória." }; } }
		public Mensagem DominioObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certidao_Dominio", Texto = @"Matrícula é obrigatória." }; } }
		public Mensagem DominioInexistente { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Certidao_Dominio", Texto = @"A matrícula não existe mais na caracterização de Dominialidade." }; } }
	}
}
