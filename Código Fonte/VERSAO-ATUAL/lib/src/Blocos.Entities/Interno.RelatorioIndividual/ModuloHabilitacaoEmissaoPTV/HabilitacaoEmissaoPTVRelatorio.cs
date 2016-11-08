using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV
{
	public class HabilitacaoEmissaoPTVRelatorio
	{
		public int Id { get; set; }
		public String Tid { get; set; }

		public string TermoNumero { get; set; }
		public Arquivo.Arquivo Foto { get; set; }
		public string HabilitacaoNumero { get; set; }
		public string MatriculaNumero { get; set; }
		public string Nome { get; set; }
		public string Profissao { get; set; }
		public string CreaNumero { get; set; }
		public string CPF { get; set; }
		public string RG { get; set; }
		public string Endereco { get; set; }
		public string Municipio { get; set; }
		public string UF { get; set; }
		public string CEP { get; set; }
		public string TelResidencial { get; set; }
		public string TelComercial { get; set; }
		public string TelCelular { get; set; }
		public string Email { get; set; }
		public string CreaRegistro { get; set; }
		public string OrgaoClasse { get; set; }

		public string Extensao { get; set; }
		public string HabilitacaoOrigem { get; set; }

		public HabilitacaoEmissaoPTVRelatorio() 
		{
			Foto = new Blocos.Arquivo.Arquivo();
			Foto.Conteudo = new byte[0];
		}
	}
}