using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class InformacaoCortePDF
	{
		public String ArvoresRestantes { get; set; }
		public String AreaRestantes { get; set; }

		public String DiaEmissao { get; set; }
		public String MesEmissao { get; set; }
		public String AnoEmissao { get; set; }

		public String Caracterizacao { get; set; }
		public String LicençaAmbiental { get; set; }
		public Decimal AreaPlantada { get; set; }
		public Decimal AreaCroqui { get; set; }

		public List<InformacaoCorteEspeciePDF> Especies { get; set; }
		public List<InformacaoCorteProdutoPDF> Produtos { get; set; }

		public List<InformacaoCorteInfoPDF> InformacoesDeCorte { get; set; }

		public InformacaoCortePDF()
		{
			InformacoesDeCorte = new List<InformacaoCorteInfoPDF>();
		}

		public InformacaoCortePDF(InformacaoCorte informacao)
		{
			this.Especies = informacao.InformacaoCorteTipo.Select(x => new InformacaoCorteEspeciePDF(x)).ToList();
			this.Produtos = informacao.InformacaoCorteTipo.SelectMany(x => x.InformacaoCorteDestinacao).Select(x => new InformacaoCorteProdutoPDF(x)).ToList();

			this.ArvoresRestantes = Convert.ToDecimal(informacao.AreaFlorestaPlantada).ToString("N0");
			this.AreaRestantes = Convert.ToDecimal(informacao.AreaCorteCalculada).ToString("N4");

			this.DiaEmissao = informacao.DataInformacao.Data.Value.ToString("dd");
			this.MesEmissao = informacao.DataInformacao.Data.Value.ToString("MMMM");
			this.AnoEmissao = informacao.DataInformacao.Data.Value.Year.ToString();
		}
	}
}