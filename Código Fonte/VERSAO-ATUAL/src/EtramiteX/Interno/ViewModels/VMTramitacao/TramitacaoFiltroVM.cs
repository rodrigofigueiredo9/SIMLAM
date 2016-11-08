namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class TramitacaoFiltroVM
	{
		public int TramitacaoId { get; set; }
		public int SetorId { get; set; }
		public int FuncionarioId { get; set; }
		public bool IsProcesso{ get; set; }
		public int ProtocoloId { get; set; }
		public string ProtocoloNumero { get; set; }
	}
}