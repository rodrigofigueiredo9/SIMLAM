using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class UnidadeProducaoPDF
	{
		public String CodigoPropriedade { get; set; }
		public UnidadeProducaoItemPDF Unidade { get { return Unidades.FirstOrDefault(); } }
		public String ResponsaveisTecnicosStragg { get; set; }
		public String ResponsaveisTecnicosCFONumeroStragg { get; set; }
		public String ProdutoresStragg { get; set; }

		private List<UnidadeProducaoItemPDF> _unidades = new List<UnidadeProducaoItemPDF>();
		public List<UnidadeProducaoItemPDF> Unidades
		{
			get { return _unidades; }
			set { _unidades = value; }
		}

		public UnidadeProducaoPDF(){}

		public UnidadeProducaoPDF(UnidadeProducao unidadeProducao)
		{
			CodigoPropriedade = String.Format("{0:D4}", unidadeProducao.CodigoPropriedade);

			//Unidades Item
			foreach (var item in unidadeProducao.UnidadesProducao)
			{
				Unidades.Add(new UnidadeProducaoItemPDF(item));
			}

			//Straggs
			List<ResponsavelPDF> responsaveis = new List<ResponsavelPDF>();
			Unidades.SelectMany(x => x.ResponsaveisTecnicos).ToList().ForEach(resp =>
			{
				if (!responsaveis.Exists(x => x.CPFCNPJ == resp.CPFCNPJ))
				{
					responsaveis.Add(resp);
				}
			});

			ProdutoresStragg = EntitiesBus.Concatenar(Unidades.SelectMany(x => x.Produtores).GroupBy(x => new { x.NomeRazaoSocial, x.Tipo, x.CPFCNPJ }).Select(g => g.First()).Select(x => x.NomeRazaoSocial + ", " + (x.Tipo == PessoaTipo.FISICA ? "CPF " : "CNPJ ") + x.CPFCNPJ).ToList());
			ResponsaveisTecnicosStragg = EntitiesBus.Concatenar(responsaveis.Select(x => x.DadosCompletos).ToList());
			ResponsaveisTecnicosCFONumeroStragg = EntitiesBus.Concatenar(responsaveis.Select(x => x.CFONumero.Trim()).ToList());

			if(responsaveis.Count > 1)
			{
				ResponsaveisTecnicosCFONumeroStragg += " respectivamente";
			}
		}
	}
}