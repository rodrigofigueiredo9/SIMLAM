

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAtividade
{
	public class AtividadeListarFiltro
	{
		public Int32 Id { get; set; }
		public String AtividadeNome { get; set; }
		public String Secao { get; set; }
		public Int32? Divisao { get; set; }
		public Int32 SetorId { get; set; }
		public String SetorTexto { get; set; }
		public String SetorSigla { get; set; }
		public Int32? AgrupadorId { get; set; }
		public String AgrupadorTexto { get; set; }
		public String CNAE { get; set; }
		public Boolean? ExibirCredenciado { get; set; }
	}
}