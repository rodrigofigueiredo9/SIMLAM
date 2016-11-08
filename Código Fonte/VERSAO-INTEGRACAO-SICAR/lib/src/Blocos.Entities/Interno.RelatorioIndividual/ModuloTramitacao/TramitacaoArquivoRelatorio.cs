

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao
{
	public class TramitacaoArquivoRelatorio
	{
		public int? Id { get; set; }
		public String Tid { get; set; }
		public String Nome { get; set; }
		public int? SetorId { get; set; }
		public String SetorNome { get; set; }
		public int? TipoId { get; set; }
		public String TipoTexto { get; set; }
		public int? ProcessoSituacao { get; set; }
		public int? DocumentoSituacao { get; set; }

		public String EstanteNome { get; set; }
		public String PrateleiraNome { get; set; }
		public String SituacaoNome { get; set; }
		public String Despacho { get; set; }

		public TramitacaoArquivoRelatorio()
		{
			this.Nome = string.Empty;
		}
	}
}
