using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProcesso
{
	public class ProtocoloLocalizacao
	{
		public eLocalizacaoProtocolo Localizacao { get; set; }

		public int SetorRemetenteId { get; set; }
		public string SetorRemetenteNome { get; set; }
		public int SetorDestinatarioId { get; set; }
		public string SetorDestinatarioNome { get; set; }

		public int FuncionarioRemetenteId { get; set; }
		public string FuncionarioRemetenteNome { get; set; }
		public int FuncionarioDestinatarioId { get; set; }
		public string FuncionarioDestinatarioNome { get; set; }

		public int OrgaoExternoId { get; set; }
		public string OrgaoExternoNome { get; set; }

		public int ArquivoPrateleiraId { get; set; }
		public string ArquivoNome { get; set; }

		public int ProcessoPaiId { get; set; }
		public string ProcessoPaiNumero { get; set; }

		private Tramitacao _tramitacao = new Tramitacao();
		public Tramitacao Tramitacao
		{
			get { return _tramitacao; }
			set { _tramitacao = value; }
		}

		public ProtocoloLocalizacao()
		{
			Tramitacao = new Tramitacao();
		}
	}
}

