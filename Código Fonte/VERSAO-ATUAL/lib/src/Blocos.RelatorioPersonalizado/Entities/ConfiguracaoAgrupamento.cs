namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class ConfiguracaoAgrupamento
	{
		public Campo Campo { get; set; }
		public int CampoId { get; set; }
		public int CampoCodigo { get; set; }
		public int Prioridade { get; set; }
		public string Alias { get; set; }

		public ConfiguracaoAgrupamento()
		{
			Campo = new Campo();
		}
	}
}