using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class UnidadeConsolidacaoPDF
	{
		public String CodigoUC { get; set; }
		public String ResponsaveisTecnicosStragg { get; set; }
		public String ResponsaveisTecnicosCFONumeroStragg { get; set; }

		public String ResponsaveisEmpreendimentoStragg
		{
			get
			{
				if (ResponsaveisEmpreendimento != null && ResponsaveisEmpreendimento.Count > 0)
				{
					if (ResponsaveisEmpreendimento.Count > 1)
					{
						return "dos proprietários " + EntitiesBus.Concatenar(ResponsaveisEmpreendimento.Select(x => x.Texto + ", " + (x.VinculoTipo == PessoaTipo.FISICA ? "CPF " : "CNPJ ") + x.CPFCNPJ).ToList());
					}
					else
					{
						return "do proprietário " + ResponsaveisEmpreendimento.FirstOrDefault().Texto + ", " + (ResponsaveisEmpreendimento.FirstOrDefault().VinculoTipo == PessoaTipo.FISICA ? "CPF " : "CNPJ ") + ResponsaveisEmpreendimento.FirstOrDefault().CPFCNPJ;
					}
				}

				return String.Empty;
			}
		}

		private List<PessoaLst> _responsaveisEmp = new List<PessoaLst>();
		public List<PessoaLst> ResponsaveisEmpreendimento
		{
			get { return _responsaveisEmp; }
			set { _responsaveisEmp = value; }
		}

		private List<ResponsavelPDF> _responsaveisTecnicos = new List<ResponsavelPDF>();
		public List<ResponsavelPDF> ResponsaveisTecnicos
		{
			get { return _responsaveisTecnicos; }
			set { _responsaveisTecnicos = value; }
		}

		private List<CultivarPDF> _cultivares = new List<CultivarPDF>();
		public List<CultivarPDF> Cultivares
		{
			get { return _cultivares; }
			set { _cultivares = value; }
		}

		public UnidadeConsolidacaoPDF() { }

		public UnidadeConsolidacaoPDF(UnidadeConsolidacao unidade, List<PessoaLst> responsaveisEmp)
		{
			unidade = unidade ?? new UnidadeConsolidacao();

			CodigoUC = unidade.CodigoUC.ToString();
			ResponsaveisEmpreendimento = responsaveisEmp;

			//Cultivares
			foreach (var item in unidade.Cultivares)
			{
				Cultivares.Add(new CultivarPDF()
				{
					Nome = item.Nome,
					Cultura = item.CulturaTexto,
					Unidade = item.UnidadeMedidaTexto,
					Capacidade = item.CapacidadeMes.ToStringTrunc(4)
				});
			}

			//ResponsaveisTecnicos
			List<ResponsavelPDF> responsaveis = new List<ResponsavelPDF>();
			string dadosCompletos = string.Empty;

			foreach (var responsavel in unidade.ResponsaveisTecnicos)
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

				if (!responsaveis.Exists(x => x.CPFCNPJ == responsavel.CpfCnpj))
				{
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
			}

			ResponsaveisTecnicosStragg = EntitiesBus.Concatenar(ResponsaveisTecnicos.Select(x => x.DadosCompletos.Trim()).ToList());
			ResponsaveisTecnicosCFONumeroStragg = EntitiesBus.Concatenar(ResponsaveisTecnicos.Select(x => x.CFONumero.Trim()).ToList());

			if (ResponsaveisTecnicos.Count > 1)
			{
				ResponsaveisTecnicosCFONumeroStragg += " respectivamente";
			}
		}
	}
}