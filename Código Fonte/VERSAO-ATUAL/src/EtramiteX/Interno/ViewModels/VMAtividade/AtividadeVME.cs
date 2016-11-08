

using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMAtividade
{	
	public class AtividadeVME
	{
		public int Id { get; set; }
		public int ProcessoId { get; set; }
		private bool _isProcesso = true;
		public bool IsProcesso { get { return _isProcesso; } set { _isProcesso = value; } }
		public bool IsVisualizar { get; set; }

		private Atividade _atividade;

		public Atividade Atividade
		{
			get { return _atividade; }
			set { _atividade = value; }
		}

		public AtividadeVME() { }

		public AtividadeVME(Atividade procAtividade)
		{
			Atividade = procAtividade;
		}
	}
}