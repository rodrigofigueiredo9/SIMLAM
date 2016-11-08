namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ConfigFiscalizacaoCampo
	{
		public int Id { get; set; }
		public int CampoId { get; set; }
		public string CampoTexto { get; set; }
		public string Tid { get; set; }

		public ConfigFiscalizacaoCampo()
		{
			this.CampoTexto = 
			this.Tid = string.Empty;
		}
	}
}
