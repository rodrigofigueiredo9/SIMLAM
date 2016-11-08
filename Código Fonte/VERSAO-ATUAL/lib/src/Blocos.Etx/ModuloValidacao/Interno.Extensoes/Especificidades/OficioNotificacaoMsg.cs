namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static OficioNotificacaoMsg _oficioNotificacaoMsg = new OficioNotificacaoMsg();
		public static OficioNotificacaoMsg OficioNotificacao
		{
			get { return _oficioNotificacaoMsg; }
			set { _oficioNotificacaoMsg = value; }
		}
	}

	public class OficioNotificacaoMsg
	{
		public Mensagem RequerimentoSemPendencias { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "RequerimentoPadrao", Texto = "O requerimento não pode ser selecionado, pois não possui pendências." }; } }
		public Mensagem HierarquiaSemInformacao { get { return new Mensagem() { Tipo = eTipoMensagem.Informacao, Campo = "PDF", Texto = "Sem informação" }; } }
	}
}