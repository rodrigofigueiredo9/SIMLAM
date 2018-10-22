using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Laudo
	{
		public Int32 Id { get; set; }
		public Int32 RegularizacaoDominio { get; set; }
		public String RegularizacaoDominioTid { get; set; }
		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public PessoaPDF Destinatario { get; set; }
		public ResponsavelPDF Responsavel { get; set; }
		public AtividadePDF Atividade { get; set; }
		public DominioPDF Dominio { get; set; }
		public DominialidadePDF Dominialidade { get; set; }
		public List<ExploracaoFlorestalPDF> ExploracaoFlorestal { get; set; }
		public RegularizacaoFundiariaPDF RegularizacaoFundiaria { get; set; }
		public SilviculturaPDF Silvicultura { get; set; }
		public QueimaControladaPDF QueimaControlada { get; set; }
		public SilviculturaATVPDF SilviculturaATV { get; set; }

		public Int32 CaracterizacaoTipo { get; set; }
		public String Parecer { get; set; }
		public String ConclusaoTipoTexto { get; set; }
		public String Consideracao { get; set; }
		public String Restricao { get; set; }
		public String Objetivo { get; set; }
		public String Observacao { get; set; }
		public String ParecerFavoravel { get; set; }
		public String ParecerDesfavoravel { get; set; }
		public String DescricaoParecer { get; set; }
		public String DescricaoParecerDesfavoravel { get; set; }
		public String DataVistoria { get; set; }

		public String ResultadoTipoTexto { get; set; }
		public String ResultadoQuais { get; set; }
		public String PlantioAPP { get; set; }
		public String PlantioAPPArea { get; set; }
		public String PlantioMudasEspeciesFlorestNativas { get; set; }
		public String PlantioMudasEspeciesFlorestNativasQtd { get; set; }
		public String PlantioMudasEspeciesFlorestNativasArea { get; set; }
		public String PreparoSolo { get; set; }
		public String PreparoSoloArea { get; set; }

		private List<AnexoPDF> _anexos = new List<AnexoPDF>();
		public List<AnexoPDF> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		private List<AnaliseItemPDF> _analiseItens = new List<AnaliseItemPDF>();
		public List<AnaliseItemPDF> AnaliseItens
		{
			get { return _analiseItens; }
			set { _analiseItens = value; }
		}

		public Laudo()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
			Responsavel = new ResponsavelPDF();
			Atividade = new AtividadePDF();
			Dominio = new DominioPDF();
			Dominialidade = new DominialidadePDF();
			ExploracaoFlorestal = new List<ExploracaoFlorestalPDF>();
			RegularizacaoFundiaria = new RegularizacaoFundiariaPDF();
			Silvicultura = new SilviculturaPDF();
			QueimaControlada = new QueimaControladaPDF();
			SilviculturaATV = new SilviculturaATVPDF();
		}
	}
}