namespace Tecnomapas.Blocos.Entities.Interno.ModuloSobre
{
	public class SobreItem
	{
		public int Id { get; set; }
		public int VersaoId { get; set; }
		public string NumeroTP { get; set; }
		public string Descricao { get; set; }
		public string Tipo { get; set; }

		public SobreItem()
		{
			this.NumeroTP =
			this.Descricao =
			this.Tipo = string.Empty;
		}
	}
}
