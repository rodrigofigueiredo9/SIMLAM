

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemPendencia
{
	public class ChecagemPendenciaItemRelatorio
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public int SituacaoId { get; set; } // Chave estrangeira para lov_checagem_pend_item_sit
		public string SituacaoTexto { get; set; }
		public int ChecagemId { get; set; }

		public ChecagemPendenciaItemRelatorio() { }
	}
}