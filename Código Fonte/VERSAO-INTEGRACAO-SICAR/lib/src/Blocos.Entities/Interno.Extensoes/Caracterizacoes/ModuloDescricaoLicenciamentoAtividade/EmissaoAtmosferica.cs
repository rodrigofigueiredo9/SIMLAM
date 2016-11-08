namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade
{
	public class EmissaoAtmosferica
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }		
		public int TipoCombustivelId { get; set; }
		public string TipoCombustivelTexto { get; set; }
		public string SubstanciaEmitida { get; set; }
		public string EquipamentoControle { get; set; }
		public string Tid { get; set; }

		public EmissaoAtmosferica()
		{
			this.SubstanciaEmitida =
			this.EquipamentoControle =
			this.Tid = "";
		}
	}
}
