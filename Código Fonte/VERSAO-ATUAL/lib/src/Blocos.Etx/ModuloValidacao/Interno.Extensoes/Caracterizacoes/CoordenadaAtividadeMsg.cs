namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CoordenadaAtividadeMsg _coordenadaAtividadeMsg = new CoordenadaAtividadeMsg();
		public static CoordenadaAtividadeMsg CoordenadaAtividade
		{
			get { return _coordenadaAtividadeMsg; }
			set { _coordenadaAtividadeMsg = value; }
		}
	}
	public class CoordenadaAtividadeMsg
	{
		public Mensagem GeometriaTipoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CoordenadaAtividade_Tipo", Texto = @"Tipo geométrico da coordenada de atividade  é obrigatório." }; } }
		public Mensagem CoordenadaAtividadeObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "CoordenadaAtividade_CoordenadaAtividade", Texto = @"Coordenada da atividade  é obrigatória." }; } }
	}
}
