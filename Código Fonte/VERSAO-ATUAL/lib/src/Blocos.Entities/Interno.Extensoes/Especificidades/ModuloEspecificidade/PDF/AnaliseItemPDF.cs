using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class AnaliseItemPDF
	{
		public Int32 Id { get; set; }
		public String Nome { get; set; }
		public String Motivo { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }

		public String SituacaoMotivo
		{
			get
			{
				return SituacaoTexto + (String.IsNullOrWhiteSpace(Motivo) ? String.Empty : " - " + Motivo);
			}
		}

		public AnaliseItemPDF() { }
	}
}