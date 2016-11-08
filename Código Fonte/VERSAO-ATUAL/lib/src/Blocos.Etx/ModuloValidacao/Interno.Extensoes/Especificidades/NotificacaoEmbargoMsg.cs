namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static NotificacaoEmbargoMsg _notificacaoEmbargoMsg = new NotificacaoEmbargoMsg();
		public static NotificacaoEmbargoMsg NotificacaoEmbargoMsg
		{
			get { return _notificacaoEmbargoMsg; }
			set { _notificacaoEmbargoMsg = value; }
		}
	}

	public class NotificacaoEmbargoMsg
	{
		public Mensagem AtividadeSolicitadaIgualEmbargada { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A atividade solicitada e atividade a ser embargada devem ser diferentes." }; } }
		public Mensagem AtividadeEmbargoSetorErrado { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Texto = "A atividade a ser embarga deve ser do setor DTCAR." }; } }
	}
}