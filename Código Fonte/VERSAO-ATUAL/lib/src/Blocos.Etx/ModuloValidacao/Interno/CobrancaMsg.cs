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

		public Mensagem NumeroAutosObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroAutos", Tipo = eTipoMensagem.Advertencia, Texto = "Nº Autuação(SEP) é obrigatório." }; } }
		public Mensagem NumeroFiscalizacaoObrigatorio { get { return new Mensagem() { Campo = "Cobranca_Fiscalizacao", Tipo = eTipoMensagem.Advertencia, Texto = "Nº Fiscalização é obrigatório." }; } }
		public Mensagem NumeroIUFObrigatorio { get { return new Mensagem() { Campo = "Cobranca_NumeroIUF", Tipo = eTipoMensagem.Advertencia, Texto = "Nº AI/IUF é obrigatório." }; } }
	}
}
