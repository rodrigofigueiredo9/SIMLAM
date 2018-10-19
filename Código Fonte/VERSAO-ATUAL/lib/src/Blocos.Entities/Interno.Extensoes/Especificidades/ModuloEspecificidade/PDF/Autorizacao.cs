using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Autorizacao : IAnexoPdf
	{
		public int Id { get; set; }
		public String Observacao { set; get; }
		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public PessoaPDF Destinatario { set; get; }
		public ExploracaoFlorestalPDF ExploracaoFlorestal { set; get; }
		public QueimaControladaPDF QueimaControlada { get; set; }
		public DominialidadePDF Dominialidade { get; set; }
		public String TotalPonto { get; set; }
		public String TotalPoligono { set; get; }
		private List<ExploracaoFlorestalAutorizacaoDetalhePDF> _exploracaoFlorestalPonto = new List<ExploracaoFlorestalAutorizacaoDetalhePDF>();
		public List<ExploracaoFlorestalAutorizacaoDetalhePDF> ExploracaoFlorestalPonto
		{
			get { return _exploracaoFlorestalPonto; }
			set { _exploracaoFlorestalPonto = value; }
		}

		public List<AnexoPDF> Anexos { set; get; }

		public Autorizacao()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
			ExploracaoFlorestal = new ExploracaoFlorestalPDF();
			QueimaControlada = new QueimaControladaPDF();
			Dominialidade = new DominialidadePDF();

			Anexos = new List<AnexoPDF>();
		}

		public List<Arquivo.Arquivo> AnexosPdfs { get; set; }
	}
}