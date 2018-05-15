

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class ProtocoloTipo : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public bool IsAtivo { get; set; }

		public bool PossuiProcesso { get; set; }
		public bool PossuiDocumento { get; set; }
		public bool PossuiChecagemPendencia { get; set; }
		public bool PossuiChecagemRoteiro { get; set; }
		public bool PossuiRequerimento { get; set; }
		public bool PossuiFiscalizacao { get; set; }
		public bool PossuiInteressado { get; set; }
		public bool PossuiInteressadoLivre { get; set; }
		public bool PossuiNome { get; set; }
		public bool PossuiQuantidadeDocumento { get; set; }
		public bool PossuiAssunto { get; set; }
		public bool PossuiDescricao { get; set; }

		public bool ProcessoObrigatorio { get; set; }
		public bool DocumentoObrigatorio { get; set; }
		public bool ChecagemPendenciaObrigatorio { get; set; }
		public bool ChecagemRoteiroObrigatorio { get; set; }
		public bool RequerimentoObrigatorio { get; set; }
		public bool FiscalizacaoObrigatorio { get; set; }
		public bool InteressadoObrigatorio { get; set; }
		public bool NomeObrigatorio { get; set; }
		public bool QuantidadeDocumentoObrigatorio { get; set; }
		public bool AssuntoObrigatorio { get; set; }
		public bool DescricaoObrigatoria { get; set; }

		public String LabelInteressado { get; set; }
	}
}