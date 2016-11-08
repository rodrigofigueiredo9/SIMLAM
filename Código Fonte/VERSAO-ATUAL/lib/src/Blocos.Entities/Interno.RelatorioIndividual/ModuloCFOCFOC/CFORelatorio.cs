using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCFOCFOC
{
	public class CFORelatorio
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int Situacao { get; set; }
		public string Numero { get; set; }
		public string PropriedadeCodigo { get; set; }
		public string NomeLaboratorio { get; set; }
		public string NumeroLaudoResultadoAnalise { get; set; }
		public string EstadoSigla { get; set; }
		public string MunicipioTexto { get; set; }
		public string DeclaracaoAdicionalHtml { get; set; }
		public string EstadoEmissaoSigla { get; set; }
		public string MunicipioEmissaoTexto { get; set; }
		public string NumeroLacre { get; set; }
		public string NumeroPorao { get; set; }
		public string NumeroContainer { get; set; }
		public int ValidadeCertificado { get; set; }
		public bool PartidaLacradaOrigem { get; set; }
		public string PartidaLacradaOrigemSim { get; set; }
		public string PartidaLacradaOrigemNao { get; set; }
		public int ProdutoEspecificacao { get; set; }
		public string ProdutoEspecificacao1 { get; set; }
		public string ProdutoEspecificacao2 { get; set; }
		public string ProdutoEspecificacao3 { get; set; }
		public string ProdutoEspecificacao4 { get; set; }
		public string DataAtivacao { get; set; }
		public DateTime DataExecucao { get; set; }
		public PessoaRelatorio Produtor { get; set; }
		public EmpreendimentoRelatorio Empreendimento { get; set; }
		public List<IdentificacaoProdutoRelatorio> Produtos { get; set; }
		public List<TratamentoFitossanitarioRelatorio> TratamentosFitossanitarios { get; set; }
		public ResponsavelTecnicoRelatorio ResponsavelTecnico { get; set; }

		public CFORelatorio()
		{
			Produtor = new PessoaRelatorio();
			Empreendimento = new EmpreendimentoRelatorio();
			Produtos = new List<IdentificacaoProdutoRelatorio>();
			TratamentosFitossanitarios = new List<TratamentoFitossanitarioRelatorio>();
			ResponsavelTecnico = new ResponsavelTecnicoRelatorio();
		}
	}
}