using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Licenca
	{
		public int Id { get; set; }
		public String Vias { get; set; }
		public String AnoExercicio { get; set; }
		public String NumeroRegistro { get; set; }
		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public PessoaPDF Destinatario { set; get; }

		public CaracterizacaoPDF Caracterizacao { set; get; }
		public SilviculturaPDF Silvicultura { set; get; }
		public MotosserraPDF Motosserra { set; get; }
		public SilviculturaPPFFPDF SilviculturaPPFF { get; set; }

		public byte[] LogoOrgao { get; set; }
		public string GovernoNome { get; set; }
		public string SecretariaNome { get; set; }
		public string OrgaoNome { get; set; }
		public string SetorNome { get; set; }

		public Licenca()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
			Motosserra = new MotosserraPDF();
		}
	}
}