using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC
{
	public class EmissaoCFOC
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int CredenciadoId { get; set; }
		public int? TipoNumero { get; set; }
		public string Numero { get; set; }
		public string SituacaoTexto { get; set; }
		public int SituacaoId { get; set; }
		public int EmpreendimentoId { get; set; }
		public string EmpreendimentoTexto { get; set; }
		public bool? PossuiLaudoLaboratorial { get; set; }
		public string NomeLaboratorio { get; set; }
		public string NumeroLaudoResultadoAnalise { get; set; }
		public int EstadoId { get; set; }
		public int MunicipioId { get; set; }
		public int ProdutoEspecificacao { get; set; }
		public bool? PossuiTratamentoFinsQuarentenario { get; set; }
		public bool PartidaLacradaOrigem { get; set; }
		public string NumeroLacre { get; set; }
		public string NumeroPorao { get; set; }
		public string NumeroContainer { get; set; }
		public int ValidadeCertificado { get; set; }
		public string DeclaracaoAdicional { get; set; }
		public string DeclaracaoAdicionalHtml { get; set; }
		public int EstadoEmissaoId { get; set; }
		public int MunicipioEmissaoId { get; set; }
		public string CulturaCultivar { get; set; }

		public DateTecno DataEmissao { get; set; }
		public DateTecno DataAtivacao { get; set; }
		public List<IdentificacaoProduto> Produtos { get; set; }
		public List<Praga> Pragas { get; set; }
		public List<TratamentoFitossanitario> TratamentosFitossanitarios { get; set; }

		public EmissaoCFOC()
		{
			DataEmissao = new DateTecno();
			DataAtivacao = new DateTecno();
			Produtos = new List<IdentificacaoProduto>();
			Pragas = new List<Praga>();
			TratamentosFitossanitarios = new List<TratamentoFitossanitario>();
		}
	}
}