using System;
using Tecnomapas.Blocos.Arquivo;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class AnexoPDF
	{
		public String Descricao { get; set; }
		public Int32 Tipo { get; set; }
		public Arquivo.Arquivo Arquivo { get; set; }

		public AnexoPDF()
		{
			Arquivo = new Arquivo.Arquivo();
		}
	}
}