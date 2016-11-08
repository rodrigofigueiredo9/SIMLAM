using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class Filtro
	{
		public int TramitacaoTipoId { get; set; }
		public bool DestinatarioNulo { get; set; }
		public int SituacaoId { get; set; }
		public int FuncSetorDestinoId { get; set; }
		public int RegistradorSetorId { get; set; }
		public int EmposseId { get; set; }
		public int EmposseRegistradorId { get; set; }

		public int RemetenteId { get; set; }
		public int DestinatarioId { get; set; }

		public int ObjetivoId { get; set; }
		public DateTecno DataEnvio { get; set; }
		public String Despacho { get; set; }

		public int SetorRemetenteId { get; set; }
		public int SetorDestinatarioId { get; set; }
		public int SetorExecutorId { get; set; }

		public int TipoProcessoId { get; set; }
		public int TipoDocumentoId { get; set; }

		public int FuncionariosRegistrado { get; set; }
		public int DestinoRegistrador { get; set; }

		public int RemetenteRegistrador { get; set; }

		public int ProtocoloId { get; set; }//id do processo ou documento
		public String ProcDocNumero { get; set; }//id do processo ou documento
		public int Tipo { get; set; }//processo ou documento

		private ProtocoloNumero _protocolo = new ProtocoloNumero();
		public ProtocoloNumero Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		public string InteressadoNome { get; set; }
		public string InteressadoCpfCnpj { get; set; }
		public int EmpreendimentoUF { get; set; }
		public int EmpreendimentoMunicipio { get; set; }

		public int OrgaoExterno { get; set; }

		public int Arquivo { get; set; }
		public int AtividadeSolicitada { get; set; }
		public int SituacaoAtividadeSolicitada { get; set; }
	}
}