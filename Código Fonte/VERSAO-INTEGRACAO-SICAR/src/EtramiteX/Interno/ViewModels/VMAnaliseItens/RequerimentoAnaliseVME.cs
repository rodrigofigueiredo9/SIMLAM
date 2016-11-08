namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAnaliseItens
{
	public class RequerimentoAnaliseVME
	{
		public int ProtocoloId { get; set; }
		public int ChecagemId { get; set; }
		public string NumeroProtocolo { get; set; }
		public int Tipo { get; set; }
		public int NumeroRequerimento { get; set; }
		public string DataCriacaoRequerimento { get; set; }
		public bool Atualizar { get; set; }
		public bool IsProcesso { get; set; }
	}
}