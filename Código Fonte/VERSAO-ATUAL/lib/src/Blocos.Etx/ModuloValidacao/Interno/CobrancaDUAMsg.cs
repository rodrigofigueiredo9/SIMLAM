namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static CobrancaDUAMsg _cobrancaDUAMsg = new CobrancaDUAMsg();
		public static CobrancaDUAMsg CobrancaDUAMsg
		{
			get { return _cobrancaDUAMsg; }
			set { _cobrancaDUAMsg = value; }
		}
	}

	public class CobrancaDUAMsg
	{
		public Mensagem Salvar { get { return new Mensagem() { Tipo = eTipoMensagem.Sucesso, Texto = "Parceça salva com sucesso." }; } }

		public Mensagem VrteObrigatorio { get { return new Mensagem() { Campo = "Cobranca_Vrte", Tipo = eTipoMensagem.Advertencia, Texto = "VRTE é obrigatório." }; } }
	}
}
