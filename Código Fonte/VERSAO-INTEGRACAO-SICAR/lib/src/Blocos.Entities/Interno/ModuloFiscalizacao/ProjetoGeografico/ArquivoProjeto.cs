

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ArquivoProjeto : Arquivo.Arquivo
	{
		public Int32 ProjetoId { get; set; }
		public Int32 Mecanismo { get; set; }
		public Int32 IdRelacionamento { get; set; }
		public Int32 Tipo { get; set; }
		public Int32 FilaTipo { get; set; }
		public Boolean isValido { get; set; }
		public Int32 Etapa { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		public String Chave { get; set; }
		public DateTime ChaveData { get; set; }
	}
}
