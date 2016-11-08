using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CertidaoDispensaLicenciamentoAmbientalPDF
	{
		public int Id { get; set; }
		public string VinculoPropriedade { get; set; }
		public string VinculoPropriedadeOutro { get; set; }
		public AtividadePDF Atividade { get; set; }
		public RequerimentoPDF Requerimento { get; set; }
		public PessoaPDF Interessado { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public TituloPDF Titulo { get; set; }
		public BarragemDispensaLicencaPDF Caracterizacao { get; set; }

		public CertidaoDispensaLicenciamentoAmbientalPDF()
		{
			this.Atividade = new AtividadePDF();
			this.Requerimento = new RequerimentoPDF();
			this.Interessado = new PessoaPDF();
			this.Empreendimento = new EmpreendimentoPDF();
			this.Titulo = new TituloPDF();
		}
	}
}
