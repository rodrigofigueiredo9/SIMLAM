using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class InformacaoCorteInfoPDF
	{
		public String ArvoresRestantes { get; set; }
		public String AreaRestantes { get; set; }

		public String DiaEmissao { get; set; }
		public String MesEmissao { get; set; }
		public String AnoEmissao { get; set; }

		public List<InformacaoCorteEspeciePDF> Especies { get; set; }
		public List<InformacaoCorteProdutoPDF> Produtos { get; set; }

		public InformacaoCorteInfoPDF(InformacaoCorte informacao)
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