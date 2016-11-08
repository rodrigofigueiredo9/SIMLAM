using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Certidao
	{
		public Int32 Id { get; set; }
		public String Certificacao { get; set; }
		public String Matricula { get; set; }
		public String Cartorio { get; set; }
		public String Livro { get; set; }
		public String Folha { get; set; }
		public String NumeroCCIR { get; set; }
		public String Descricao { get; set; }

		public AtividadePDF Atividade { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public PessoaPDF Destinatario { get; set; }
		public List<PessoaPDF> Destinatarios { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public ResponsavelPDF Responsavel { get; set; }
		public TituloPDF Titulo { get; set; }
		public CertidaoDebitoPDF CertidaoDebito { get; set; }

		public Certidao()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
			Destinatarios = new List<PessoaPDF>();
			Responsavel = new ResponsavelPDF();
			Atividade = new AtividadePDF();
			CertidaoDebito = new CertidaoDebitoPDF();
		}
	}
}