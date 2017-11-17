using System;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class LocalInfracaoRelatorioNovo
	{
		public Int32 EmpreendimentoId { get; set; }
		public String EmpreendimentoTid { get; set; }
		public String EmpreendimentoNomeRazaoSocial { get; set; }
		public String EmpreendimentoCnpj { get; set; }
        public String CodEmp { get; set; }
		public Int32 Id { get; set; }
		public Int32 SetorId { get; set; }
		public Int32 AutuadoEmpResponsavelId { get; set; }
		public String AutuadoEmpResponsavelTid { get; set; }
		public Int32 PropResponsavelId { get; set; }
		public String PropResponsavelTid { get; set; }
		public String SistemaCoordenada { get; set; }
		public String Fuso { get; set; }
		public String Datum { get; set; }
		public String CoordenadaEasting { get; set; }
		public String CoordenadaNorthing { get; set; }
		public String Local { get; set; }
		public String DataFiscalizacao { get; set; }
        public String HoraFiscalizacao { get; set; }
		public String Municipio { get; set; }
		public String UF { get; set; }

		

		private EnderecoRelatorio _empEndereco = new EnderecoRelatorio();
		public EnderecoRelatorio EmpEndereco
		{
			get { return _empEndereco; }
			set { _empEndereco = value; }
		}

		private PessoaRelatorio _autuado = new PessoaRelatorio();
		public PessoaRelatorio Autuado
		{
			get { return _autuado; }
			set { _autuado = value; }
		}

		private PessoaRelatorio _empResponsavel = new PessoaRelatorio();
		public PessoaRelatorio EmpResponsavel
		{
			get { return _empResponsavel; }
			set { _empResponsavel = value; }
		}	
	}
}
