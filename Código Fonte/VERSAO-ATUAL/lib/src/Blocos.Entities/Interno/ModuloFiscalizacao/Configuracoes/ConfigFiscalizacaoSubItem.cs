namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ConfigFiscalizacaoSubItem
	{
		public int Id { get; set; }
		public int SubItemId { get; set; }
		public string SubItemTexto { get; set; }
		public string Tid { get; set; }

		public ConfigFiscalizacaoSubItem()
		{
			this.SubItemTexto = 
			this.Tid = string.Empty;
		}
	}
}
