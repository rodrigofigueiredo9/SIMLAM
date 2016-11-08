namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LaudoFundiarioSimplificadoMsg _LaudoFundiarioSimplificadoMsg = new LaudoFundiarioSimplificadoMsg();
		public static LaudoFundiarioSimplificadoMsg LaudoFundiarioSimplificadoMsg
		{
			get { return _LaudoFundiarioSimplificadoMsg; }
			set { _LaudoFundiarioSimplificadoMsg = value; }
		}
	}

	public class LaudoFundiarioSimplificadoMsg
	{
		public Mensagem ObjetivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Objetivo", Texto = "Objetivo é obrigatório." }; } }
        public Mensagem DescricaoParecerObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_ParecerDescricao", Texto = "Descrição do Parecer é obrigatória." }; } }
    }
}