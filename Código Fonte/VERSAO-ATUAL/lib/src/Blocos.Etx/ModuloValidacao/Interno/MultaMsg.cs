namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
        private static MultaMsg _multaMsg = new MultaMsg();
		public static MultaMsg MultaMsg
		{
			get { return _multaMsg; }
			set { _multaMsg = value; }
		}
	}

	public class MultaMsg
	{
		public Mensagem  Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Multa salva com sucesso." }; } }
		
		public Mensagem DigitalOuBlocoObrigatorio { get { return new Mensagem() { Campo = "Multa_IsDigital", Tipo = eTipoMensagem.Advertencia, Texto = "É obrigatório selecionar Digital ou Bloco." }; } }
		public Mensagem SerieObrigatorio { get { return new Mensagem() { Campo = "Multa_Serie", Tipo = eTipoMensagem.Advertencia, Texto = "Série é obrigatório." }; } }
		public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "Multa_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Número do IUF é obrigatório." }; } }
		public Mensagem JustificativaObrigatorio { get { return new Mensagem() { Campo = "Multa_Justificativa", Tipo = eTipoMensagem.Advertencia, Texto = "Justificar o valor da penalidade é obrigatório." }; } }
		public Mensagem CodigoReceitaObrigatorio { get { return new Mensagem() { Campo = "Multa_CodigoReceita", Tipo = eTipoMensagem.Advertencia, Texto = "Código da receita obrigatório." }; } }
		public Mensagem ValorMultaObrigatorio { get { return new Mensagem() { Campo = "Multa_ValorMulta", Tipo = eTipoMensagem.Advertencia, Texto = "Valor da multa obrigatório." }; } }
		public Mensagem ValorMultaInvalido { get { return new Mensagem() { Campo = "Multa_ValorMulta", Tipo = eTipoMensagem.Advertencia, Texto = "Valor da multa inválido." }; } }
		public Mensagem ArquivoObrigatorio { get { return new Mensagem() { Campo = "file", Tipo = eTipoMensagem.Advertencia, Texto = "Arquivo é obrigatório." }; } }
        public Mensagem ArquivoNaoEhPdf { get { return new Mensagem() { Tipo = eTipoMensagem.Advertencia, Campo = "Documento_Arquivo_Nome", Texto = "Arquivo não é do tipo pdf" }; } }
	}
}
