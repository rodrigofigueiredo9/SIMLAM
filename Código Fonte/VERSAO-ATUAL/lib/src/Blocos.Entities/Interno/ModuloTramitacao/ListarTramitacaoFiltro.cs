using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class ListarTramitacaoFiltro
	{
		public int Id { get; set; }
		public ProtocoloNumero Protocolo { get; set; }
		public String Numero { get; set; }
		public int ProtocoloTipo { get; set; }
		public int RemetenteId { get; set; }
		public int RemetenteSetorId { get; set; }
		public int DestinatarioId { get; set; }
		public int DestinatarioSetorId { get; set; }
		public bool DestinatarioNulo { get; set; }
		public bool DestinatarioNaoNulo { get; set; }
		public int EmposseId { get; set; }
		public int EmposseSetorId { get; set; }
		public int FuncionarioSetorDestinoId { get; set; }
		public int RegistradorDestinatarioId { get; set; }
		public int RegistradorDestinatarioSetorId { get; set; }
		public int RegistradorRemetenteId { get; set; }
		public int RegistradorRemetenteSetorId { get; set; }		
		public int TramitacaoTipoId { get; set; }
		public int TramitacaoSituacaoId { get; set; }
		public int OrgaoExternoId { get; set; }
		public int AtividadeId { get; set; }
		public int AtividadeSituacaoId { get; set; }
		public int ArquivoId { get; set; }

		public int ArquivoEstanteId { get; set; }
		public int ArquivoPrateleiraModoId { get; set; }
		public string ArquivoIdentificacao { get; set; }

		public String InteressadoNomeRazao { get; set; }
		public String InteressadoCpfCnpj { get; set; }
		public String EmpreendimentoUf { get; set; }
		public String EmpreendimentoMunicipio { get; set; }

		public ListarTramitacaoFiltro()
		{
			Protocolo = new ProtocoloNumero();				
		}
	}
}