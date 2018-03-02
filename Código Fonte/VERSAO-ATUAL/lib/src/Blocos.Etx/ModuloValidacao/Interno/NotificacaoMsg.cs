namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static NotificacaoMsg _notificacaoMsg = new NotificacaoMsg();
		public static NotificacaoMsg NotificacaoMsg
		{
			get { return _notificacaoMsg; }
			set { _notificacaoMsg = value; }
		}
	}

	public class NotificacaoMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Notificacao salva com sucesso." }; } }

		public Mensagem FormaIUFObrigatorio { get { return new Mensagem() { Campo = "Notificacao_FormaIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Forma de notificação da IUF é obrigatório." }; } }
		public Mensagem DataIUFObrigatorio { get { return new Mensagem() { Campo = "Notificacao_DataIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação IUF obrigatório." }; } }
		public Mensagem DataJIAPIObrigatorio { get { return new Mensagem() { Campo = "Notificacao_DataJIAPI", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação JIAPI obrigatório." }; } }
		public Mensagem DataCOREObrigatorio { get { return new Mensagem() { Campo = "Notificacao_DataCORE", Tipo = eTipoMensagem.Advertencia, Texto = "Data da notificação CORE obrigatório." }; } }
		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "fsArquivos", Texto = "Arquivo complementar é obrigatório." }; } }
	}
}
