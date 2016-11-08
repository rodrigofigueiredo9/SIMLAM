namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static EscrituraPublicaCompraVendaMsg _escrituraPublicaCompraVenda = new EscrituraPublicaCompraVendaMsg();
		public static EscrituraPublicaCompraVendaMsg EscrituraPublicaCompraVenda
		{
			get { return _escrituraPublicaCompraVenda; }
			set { _escrituraPublicaCompraVenda = value; }
		}
	}

	public class EscrituraPublicaCompraVendaMsg
	{
		public Mensagem LivroObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Escritura_Livro", Texto = "Livro é obrigatório." }; } }
		public Mensagem FolhasObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Escritura_Folhas", Texto = "Folhas é obrigatório." }; } }
	}
}
