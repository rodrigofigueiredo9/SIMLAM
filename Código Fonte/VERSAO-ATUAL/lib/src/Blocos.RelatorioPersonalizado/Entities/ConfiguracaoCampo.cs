namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class ConfiguracaoCampo
	{
		public string Alias { get; set; }
		public int Posicao { get; set; }
		public Campo Campo { get; set; }
		public int CampoId { get; set; }
		public int CampoCodigo { get; set; }
		public int Tamanho { get; set; }

		public ConfiguracaoCampo()
		{
			Campo = new Campo();
		}
	}
}