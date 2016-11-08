using System;
using System.ComponentModel.DataAnnotations;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloArquivo
{
	public class Anexo
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		[Display(Order = 1, Name = "Descrição")]
		public String Descricao { get; set; }
		public String ArquivoJson { get; set; }
		public int Ordem { get; set; }
		public int SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }

		private Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
		public Arquivo.Arquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}
	}
}