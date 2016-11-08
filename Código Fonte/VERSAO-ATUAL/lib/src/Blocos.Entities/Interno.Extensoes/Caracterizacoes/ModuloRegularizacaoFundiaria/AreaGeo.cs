namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria
{
	public class AreaGeo
	{
		public string Classe { get; set; }
		public string Descricao { get; set; }
		public string SubTipo { get; set; }
		public decimal AreaM2 { get; set; }

		public AreaGeo()
		{
			this.Classe =
			this.Descricao =
			this.SubTipo = string.Empty;
			this.AreaM2 = 0;
		}
	}
}
