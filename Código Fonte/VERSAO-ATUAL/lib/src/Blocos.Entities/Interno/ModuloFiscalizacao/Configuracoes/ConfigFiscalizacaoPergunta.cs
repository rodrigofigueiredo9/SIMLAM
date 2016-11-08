namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ConfigFiscalizacaoPergunta
	{
		public int Id { get; set; }
		public int PerguntaId { get; set; }
		public string PerguntaTexto { get; set; }
		public string Tid { get; set; }

		public ConfigFiscalizacaoPergunta()
		{
			this.PerguntaTexto = 
			this.Tid = string.Empty;
		}
	}
}
