using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV
{
	public class EmissaoEPTVRelatorio
	{
		public int Id { get; set; }		
		public String Tid { get; set; }
        public int FuncId { get; set; }
        public string FuncTid { get; set; }
		public int Situacao { get; set; }
		public string NumeroPTV { get; set; }
		public string ResponsavelCPF { get; set; }
		public int PartidaLacrada { get; set; }
		public int Rota_transito_definida { get; set; }
		public int ApresentacaoNotaFiscal { get; set; }
		public int TipoTransporte { get; set; }
		public string NumeroLacre { get; set; }
		public string NumeroPorao { get; set; }
		public string NumeroConteiner { get; set; }
		public string LaboratorioMunicipio { get; set; }
		public string LaboratorioUF { get; set; }
		public string DeclaracaoAdicional { get; set; }
		public string IsLacrada { get; set; }
		public string IsNaoLacrada { get; set; }
		public string IsRod { get; set; }
		public string IsAer { get; set; }
		public string IsFer { get; set; }
		public string IsHid { get; set; }
		public string IsOut { get; set; }
		public string VeiculoNumero { get; set; }
		public string IsRota { get; set; }
		public string IsNaoRota { get; set; }
		public string IsNota { get; set; }
		public string IsNaoNota { get; set; }
		public string NumeroNotaFiscal { get; set; }
		public string Itinerario { get; set; }
		public string LaboratorioNome { get; set; }
		public string NumeroLaudo { get; set; }
        public string MunicipioEmissao { get; set; }
        public string DataValidade { get; set; }
        public string DataAtivacao { get; set; }
        public DateTime DataExecucao { get; set; }
		
		public List<PTVProdutoRelatorio> Produtos { get; set; }
		public List<TratamentosRelatorio> Tratamentos { get; set; }
		public ResponsavelTecnicoRelatorio ResponsavelTecnico { get; set; }
		public DestinatarioPTVRelatorio Destinatario { get; set; }
		public FuncionarioRelatorio FuncionarioHabilitado { get; set; }
		public Arquivo.Arquivo Foto { get; set; }

        public EmissaoEPTVRelatorio()
		{	
			Produtos = new List<PTVProdutoRelatorio>();
			Tratamentos = new List<TratamentosRelatorio>();
			ResponsavelTecnico = new ResponsavelTecnicoRelatorio();
			Destinatario = new DestinatarioPTVRelatorio();
			FuncionarioHabilitado = new FuncionarioRelatorio();

			Foto = new Blocos.Arquivo.Arquivo();
			Foto.Conteudo = new byte[0];
		}
	}
}