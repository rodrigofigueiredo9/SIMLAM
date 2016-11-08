

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloIndicador
{
	public class IndicadorTituloRelatorio
	{
		public int Id { get; set; }
		public string TituloTipo { get; set; }
		public string ProcessoNumero { get; set; }
		public string DataVencimento { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }

		public int AtividadeId { get; set; }
		public string AtividadeTexto { get; set; }

		public string VencimentoInicial { get; set; }
		public string VencimentoFinal { get; set; }

		public string ClassificacaoFiscal { get; set; }
		public string Setor { get; set; }
		public string Quadra { get; set; }
		public string Lote { get; set; }
		public string Atividade { get; set; }
		public string Interessado { get; set; }

		private List<IndicadorTituloCondicionanteRelatorio> _condicionantes = new List<IndicadorTituloCondicionanteRelatorio>();
		public List<IndicadorTituloCondicionanteRelatorio> Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}

		public IndicadorTituloRelatorio() { }
	}
}