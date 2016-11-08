using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico
{
	public class ArquivoProjeto : Arquivo.Arquivo
	{
		public Int32 IdRelacionamento { get; set; }
		public Int32 ProjetoId { get; set; }
		public Int32 Tipo { get; set; }

		/*Remover Propriedades ao fazer refactor do projeto do institucional*/
		public Int32 Mecanismo { get; set; }
		public Int32 FilaTipo { get; set; }
		public Boolean isValido { get; set; }
		public Int32 Etapa { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		/*End: Remover Propriedades*/

		private ProcessamentoGeo _processamento = new ProcessamentoGeo();
		public ProcessamentoGeo Processamento
		{
			get { return _processamento; }
			set { _processamento = value; }
		}

		public String Chave { get; set; }
		public DateTime ChaveData { get; set; }

		public ArquivoProjeto() {}

	}
}
