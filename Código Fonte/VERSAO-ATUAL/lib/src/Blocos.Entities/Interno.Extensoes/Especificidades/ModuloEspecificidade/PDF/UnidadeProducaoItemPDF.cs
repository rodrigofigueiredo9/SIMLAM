using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class UnidadeProducaoItemPDF
	{
		public Int64 CodigoUP { get; set; }
		public CultivarPDF Cultivar { get; set; }
		public String AreaHa { get; set; }
		public String QuantidadeAno { get; set; }
		public String UnidadeMedida { get; set; }
		public List<ResponsavelPDF> ResponsaveisTecnicos { get; set; }
		public List<ResponsavelPDF> Produtores { get; set; }

		public UnidadeProducaoItemPDF(UnidadeProducaoItem item)
		{
			CodigoUP = item.CodigoUP;
			AreaHa = item.AreaHA.ToStringTrunc(4);
			QuantidadeAno = item.EstimativaProducaoQuantidadeAno.ToString();
			UnidadeMedida = item.EstimativaProducaoUnidadeMedida;

			Cultivar = new CultivarPDF()
			{
				Nome = item.CultivarTexto,
				Cultura = item.CulturaTexto
			};

			//Produtores
			Produtores = new List<ResponsavelPDF>();
			foreach (var produtor in item.Produtores)
			{
				if(Produtores.Exists(x=>x.CPFCNPJ == produtor.CpfCnpj))
				{
					continue;
				}
				Produtores.Add(new ResponsavelPDF() { NomeRazaoSocial = produtor.NomeRazao, CPFCNPJ = produtor.CpfCnpj, Tipo = produtor.Tipo.GetValueOrDefault() });
			}

			#region Responsaveis Tecnicos

			string dadosCompletos = string.Empty;
			ResponsaveisTecnicos = new List<ResponsavelPDF>();

			foreach (var responsavel in item.ResponsaveisTecnicos)
			{
				dadosCompletos = responsavel.NomeRazao;

				if (!string.IsNullOrEmpty(responsavel.ProfissaoTexto))
				{
					dadosCompletos += ", " + responsavel.ProfissaoTexto;
				}

				if (!string.IsNullOrEmpty(responsavel.OrgaoClasseSigla))
				{
					dadosCompletos += ", " + responsavel.OrgaoClasseSigla;
				}

				if (!string.IsNullOrEmpty(responsavel.NumeroRegistro))
				{
					dadosCompletos += " " + responsavel.NumeroRegistro;
				}

				ResponsaveisTecnicos.Add(new ResponsavelPDF()
				{
					CPFCNPJ = responsavel.CpfCnpj,
					NomeRazaoSocial = responsavel.NomeRazao,
					CFONumero = responsavel.CFONumero,
					Profissao = responsavel.ProfissaoTexto,
					OrgaoClasseSigla = responsavel.OrgaoClasseSigla,
					NumeroRegistro = responsavel.NumeroRegistro,
					DadosCompletos = dadosCompletos
				});
			}

			#endregion Responsaveis Tecnicos
		}
	}
}