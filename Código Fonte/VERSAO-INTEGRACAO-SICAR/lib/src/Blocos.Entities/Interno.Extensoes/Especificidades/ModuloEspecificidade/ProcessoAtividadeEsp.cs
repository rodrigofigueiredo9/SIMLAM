using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{
	public class ProcessoAtividadeEsp
	{
		public Int32 Id { get; set; }
		public Int32 Codigo { get; set; }
		public String NomeAtividade { get; set; }
		public Int32 Finalidade { get; set; }
		public Int32 SituacaoId { get; set; }
		public Boolean Ativada { get; set; }
		public String SituacaoTexto { get; set; }
		public String Motivo { get; set; }
		public String Tid { get; set; }
	}
}