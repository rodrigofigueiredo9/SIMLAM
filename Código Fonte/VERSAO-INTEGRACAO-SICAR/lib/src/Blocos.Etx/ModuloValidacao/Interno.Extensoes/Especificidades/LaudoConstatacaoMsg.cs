namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static LaudoConstatacaoMsg _laudoConstatacaoMsg = new LaudoConstatacaoMsg();
		public static LaudoConstatacaoMsg LaudoConstatacao
		{
			get { return _laudoConstatacaoMsg; }
			set { _laudoConstatacaoMsg = value; }
		}
	}

	public class LaudoConstatacaoMsg
	{
		public Mensagem RequerimentoSemPendencias { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "RequerimentoPadrao", Texto = "O requerimento não pode ser selecionado, pois não possui pendências." }; } }
		public Mensagem ObjetivoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Objetivo", Texto = "Objetivo é obrigatório." }; } }
		public Mensagem ObjetivoMuitoGrande { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Objetivo", Texto = "Objetivo deve ter no máximo 500 caracteres." }; } }
		public Mensagem ConstatacaoObrigatorio { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Constatacao", Texto = "Constatação é obrigatória." }; } }
		public Mensagem DataVistoriaObrigatoria { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_DataVistoria_DataTexto", Texto = "A Data de Vistoria é obrigatória." }; } }
		public Mensagem ConstatacaoMuitoGrande { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_Constatacao", Texto = "Constatação deve ter no máximo 2000 caracteres." }; } }
		public Mensagem DataVistoriaIvalida { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_DataVistoria_DataTexto", Texto = "A Data de Vistoria é inválida." }; } }
		public Mensagem DataVistoriaMaior { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Laudo_DataVistoria_DataTexto", Texto = "A Data de Vistoria não pode ser maior que data atual." }; } }

	}
}