namespace Tecnomapas.Blocos.RelatorioPersonalizado.Entities
{
	public class Termo
	{
		public int Ordem { get; set; }
		public string Valor { get; set; }
		public bool DefinirExecucao { get; set; }
		public Campo Campo { get; set; }
		public int CampoId { get; set; }
		public int CampoCodigo { get; set; }
		public int Operador { get; set; }
		public int Tipo { get; set; }

		public Termo()
		{
			Campo = new Campo();
		}
	}
}