namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CobrancaMsg _cobrancaMsg = new CobrancaMsg();
		public static CobrancaMsg CobrancaMsg
		{
			get { return _cobrancaMsg; }
			set { _cobrancaMsg = value; }
		}
	}

	public class CobrancaMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Cobrança salva com sucesso." }; } }
		public Mensagem NovoParcelamento { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Novo parcelamento gerado com sucesso." }; } }
		public Mensagem Excluir { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Cobrança removida com sucesso." }; } }

		public Mensagem NumeroAutosObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroAutos", Tipo = eTipoMensagem.Advertencia, Texto = "Nº Autuação(SEP) é obrigatório." }; } }
		public Mensagem NumeroFiscalizacaoObrigatorio { get { return new Mensagem() { Campo = "Cobranca_Fiscalizacao", Tipo = eTipoMensagem.Advertencia, Texto = "Nº Fiscalização é obrigatório." }; } }
		public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Nº AI/IUF é obrigatório." }; } }
		public Mensagem CodigoReceitaObrigatorio { get { return new Mensagem() { Campo = "Cobranca_CodigoReceita", Tipo = eTipoMensagem.Advertencia, Texto = "Código Receita é obrigatório." }; } }
		public Mensagem QuantidadeParcelasObrigatorio { get { return new Mensagem() { Campo = "Cobranca_QuantidadeParcelas", Tipo = eTipoMensagem.Advertencia, Texto = "Parcelas é obrigatório." }; } }
	}
}
