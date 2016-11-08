

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Finalidade : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		
		public int Codigo { get; set; }
		public bool Marcado { get; set; }
		public bool IsAtivo { get; set; }
		public Int32? IdRelacionamento { get; set; }
		
		public Int32 TituloModelo { get; set; }
		public String TituloModeloTexto { get; set; }

		public bool EmitidoPorInterno { get; set; }
		public bool EhRenovacao { get; set; }

		public Int32? TituloAnteriorTipo { get; set; }
		public Int32? TituloModeloAnteriorId { get; set; }
		public String TituloModeloAnteriorTexto { get; set; }
		public String TituloModeloAnteriorSigla { get; set; }

		public Int32? TituloAnteriorId { get; set; }
		
		public String TituloAnteriorNumero { get; set; }

		public String OrgaoExpedidor { get; set; }

		public int AtividadeId { get; set; }
		public String AtividadeNome { get; set; }
		public int AtividadeSetorId { get; set; }
	}
}