using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class AnaliseSituacaoGrupoPDF
	{
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }

		private List<AnaliseItemPDF> _itens = new List<AnaliseItemPDF>();
		public List<AnaliseItemPDF> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}
	}
}