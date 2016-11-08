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

		public InformacaoCorteInfoPDF(InformacaoCorteInformacao informacao)
		{
			this.Especies = informacao.Especies.Select(x => new InformacaoCorteEspeciePDF(x)).ToList();
			this.Produtos = informacao.Produtos.Select(x => new InformacaoCorteProdutoPDF(x)).ToList();

			this.ArvoresRestantes = Convert.ToDecimal(informacao.ArvoresIsoladasRestantes).ToString("N0");
			this.AreaRestantes = Convert.ToDecimal(informacao.AreaCorteRestante).ToString("N4");

			this.DiaEmissao = informacao.DataInformacao.Data.Value.ToString("dd");
			this.MesEmissao = informacao.DataInformacao.Data.Value.ToString("MMMM");
			this.AnoEmissao = informacao.DataInformacao.Data.Value.Year.ToString();
		}
	}
}