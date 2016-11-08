namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class ConfiguracaoOrdenacao
	{
		public Campo Campo { get; set; }
		public int CampoId { get; set; }
		public int CampoCodigo { get; set; }
		public int Prioridade { get; set; }
		public bool Crescente { get; set; }

		public ConfiguracaoOrdenacao()
		{
			Campo = new Campo();
		}
	}
}