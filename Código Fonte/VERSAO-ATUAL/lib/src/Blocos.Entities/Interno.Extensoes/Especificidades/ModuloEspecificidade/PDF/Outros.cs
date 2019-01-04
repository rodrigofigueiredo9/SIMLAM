using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Model.Business;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class Outros
	{
		public int Id { get; set; }
		public string SetorNome { get; set; }
		public string ValorTerreno { get; set; }
		public bool IsInalienabilidade { get; set; }
		public string Municipio { get; set; }
		public TituloPDF Titulo { get; set; }
		public ProtocoloPDF Protocolo { get; set; }
		public EmpreendimentoPDF Empreendimento { get; set; }
		public PessoaPDF Interessado { set; get; }
		public PessoaPDF Destinatario { set; get; }
		public DominioPDF Dominio { get; set; }
		public DominialidadePDF Dominialidade { get; set; }
		public RegularizacaoFundiariaPDF RegularizacaoFundiaria { get; set; }
		public InformacaoCortePDF InformacaoCorte { get; set; }
		public InformacaoCorteInfoPDF InformacaoCorteInfo { get; set; }
		public List<CondicionantePDF> Condicionantes { set; get; }

		public List<PessoaPDF> Destinatarios { get; set; }
		public List<PessoaPDF> ResponsaveisEmpreendimento { get; set; }
		public List<PessoaPDF> Interessados { get; set; }

		public String ResponsaveisEmpreendimentoStragg
		{
			get
			{
				if (ResponsaveisEmpreendimento != null)
				{
					return String.Join(", ", ResponsaveisEmpreendimento.Select(x => x.NomeRazaoSocial));
				}

				return string.Empty;
			}
		}

		public String TituladoTratamento
		{
			get
			{
				if (Destinatarios[0].Tipo == 1)
				{
					return "Ao(a) Sr(a) ";
				}

				return "À ";
			}
		}

		public String InteressadosStragg
		{
			get
			{
				if (Interessados != null)
				{
					return String.Join(", ", Interessados.Select(x => x.NomeRazaoSocial));
				}

				return string.Empty;
			}
		}

		public string ValorTerrenoTexto
		{
			get
			{
				string valorExtenso = EntitiesBus.NumeroExtenso(decimal.Parse(ValorTerreno));
				if (valorExtenso.Contains("Milhões Reais"))
				{
					return valorExtenso.Replace("Milhões Reais", "Milhões de Reais");
				}

				if (valorExtenso.Contains("Milhão Reais"))
				{
					return valorExtenso.Replace("Milhão Reais", "Milhão de Reais");
				}

				return valorExtenso;
			}
		}

		public Outros()
		{
			Titulo = new TituloPDF();
			Protocolo = new ProtocoloPDF();
			Empreendimento = new EmpreendimentoPDF();
			Destinatario = new PessoaPDF();
			Interessado = new PessoaPDF();
			Destinatarios = new List<PessoaPDF>();
			ResponsaveisEmpreendimento = new List<PessoaPDF>();
			Interessados = new List<PessoaPDF>();
			Dominio = new DominioPDF();
			Dominialidade = new DominialidadePDF();
			RegularizacaoFundiaria = new RegularizacaoFundiariaPDF();
			Condicionantes = new List<CondicionantePDF>();
			InformacaoCorte = new InformacaoCortePDF();
		}
	}
}