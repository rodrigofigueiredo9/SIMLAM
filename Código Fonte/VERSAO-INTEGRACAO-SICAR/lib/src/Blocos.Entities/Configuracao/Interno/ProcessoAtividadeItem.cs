

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class ProcessoAtividadeItem : IListaValor
	{
		public int Id { get; set; }
		public int Codigo { get; set; }
		public string Tid { get; set; }
		public string Cnae { get; set; }
		public string Secao { get; set; }
		public String Atividade { get; set; }
		public int TipoProcessoId { get; set; }
		public string TipoProcessoTexto { get; set; }
		public int TipoDocumento { get; set; }
		public int Original { get; set; }
		public int IdRelacionamento { get; set; }
		public String Texto { get { return Atividade; } }
		public bool IsAtivo { get; set; }

		public ProcessoAtividadeItem() { }
	}
}