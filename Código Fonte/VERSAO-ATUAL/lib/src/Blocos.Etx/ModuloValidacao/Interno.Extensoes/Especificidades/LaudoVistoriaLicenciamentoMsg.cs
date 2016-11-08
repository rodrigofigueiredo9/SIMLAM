namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LaudoVistoriaLicenciamentoMsg _laudoVistoriaLicenciamentoMsg = new LaudoVistoriaLicenciamentoMsg();
		public static LaudoVistoriaLicenciamentoMsg LaudoVistoriaLicenciamentoMsg
		{
			get { return _laudoVistoriaLicenciamentoMsg; }
			set { _laudoVistoriaLicenciamentoMsg = value; }
		}
	}

	public class LaudoVistoriaLicenciamentoMsg
	{
		public Mensagem ObjetivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Objetivo", Texto = "Objetivo é obrigatório." }; } }
		public Mensagem ConsideracoesObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Consideracao", Texto = "Considerações é obrigatório." }; } }
		public Mensagem ParecerTecnicoDescricaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_ParecerDescricao", Texto = "Descrição do Parecer técnico é obrigatório." }; } }
        public Mensagem ConclusaoObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Conclusao", Texto = "Conclusão é obrigatória." }; } }
        public Mensagem ResponsavelTecnicoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_ResponsaveisTecnico", Texto = "Responsável Técnico é obrigatório." }; } }

	}
}