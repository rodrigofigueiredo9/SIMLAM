namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static EscrituraPublicaDoacaoMsg _escrituraPublicaDoacaoMsg = new EscrituraPublicaDoacaoMsg();
		public static EscrituraPublicaDoacaoMsg EscrituraPublicaDoacao
		{
			get { return _escrituraPublicaDoacaoMsg; }
			set { _escrituraPublicaDoacaoMsg = value; }
		}
	}

	public class EscrituraPublicaDoacaoMsg
	{
		public Mensagem LivroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Escritura_Livro", Texto = "Livro é obrigatório." }; } }
		public Mensagem FolhasObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Escritura_Folhas", Texto = "Folhas é obrigatório." }; } }
	}
}
