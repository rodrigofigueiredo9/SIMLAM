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
		public Mensagem NumeroFiscalizacaoObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroFiscalizacao", Tipo = eTipoMensagem.Advertencia, Texto = "Nº Fiscalização é obrigatório." }; } }
		public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Nº AI/IUF é obrigatório." }; } }
		public Mensagem DataIUFObrigatorio { get { return new Mensagem() { Campo = "Cobranca_DataIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Data da Notificação IUF é obrigatória." }; } }
		public Mensagem CodigoReceitaObrigatorio { get { return new Mensagem() { Campo = "Cobranca_CodigoReceita", Tipo = eTipoMensagem.Advertencia, Texto = "Código Receita é obrigatório." }; } }
		public Mensagem QuantidadeParcelasObrigatorio { get { return new Mensagem() { Campo = "Cobranca_QuantidadeParcelas", Tipo = eTipoMensagem.Advertencia, Texto = "Parcelas é obrigatório." }; } }
		public Mensagem NomeAutuadoObrigatorio { get { return new Mensagem() { Campo = "Cobranca_AutuadoPessoa_NomeRazaoSocial", Tipo = eTipoMensagem.Advertencia, Texto = "Nome do autuado é obrigatório." }; } }
		public Mensagem CpfCnpjObrigatorio { get { return new Mensagem() { Campo = "Cobranca_AutuadoPessoa_CPFCNPJ", Tipo = eTipoMensagem.Advertencia, Texto = "CPF/CNPJ do autuado é obrigatório." }; } }
		public Mensagem DataVencimentoObrigatoria { get { return new Mensagem() { Campo = "Cobranca_Data1Vencimento", Tipo = eTipoMensagem.Advertencia, Texto = "Data 1º Vencimento é obrigatória." }; } }
		public Mensagem DataEmissaoObrigatoria { get { return new Mensagem() { Campo = "Cobranca_DataEmissao", Tipo = eTipoMensagem.Advertencia, Texto = "Data Emissão é obrigatória." }; } }
		public Mensagem ValorMultaObrigatorio { get { return new Mensagem() { Campo = "Cobranca_ValorMultaAtualizado", Tipo = eTipoMensagem.Advertencia, Texto = "Valor (R$) é obrigatório." }; } }
	}
}
