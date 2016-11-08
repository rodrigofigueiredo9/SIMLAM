

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRoteiro
{
	public class ItemRelatorio
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public String Condicionante { get; set; }
		public int Tipo { get; set; }
		public string TipoTexto { get; set; }
		public string DataAnalise { get; set; }
		public int SituacaoId { get; set; }
		public string Situacao { get; set; }
		public string Analista { get; set; }
		public string SetorNome { get; set; }
		public string Descricao { get; set; }
		public string Motivo { get; set; }
		public int Ordem { get; set; }
		public int Numero { get; set; }

		public ItemRelatorio() { }
	}
}
