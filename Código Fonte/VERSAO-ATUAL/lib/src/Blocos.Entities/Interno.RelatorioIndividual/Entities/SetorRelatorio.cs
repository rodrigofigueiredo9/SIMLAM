using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public class SetorRelatorio
	{
		public int Id { get; set; }		
		public String Nome { get; set; }
		public String Texto { get { return Nome; } }
		public string HierarquiaCabecalho { get; set; }
		public String Sigla { get; set; }		
		public int? Responsavel { get; set; }
		public bool EhResponsavel { get; set; }
		public bool IsAtivo { get; set; }
		public int IdRelacao { get; set; }
		public int TramitacaoTipoId { get; set; }

		private EnderecoRelatorio _endereco = new EnderecoRelatorio();
		public EnderecoRelatorio Endereco
		{
			get { return _endereco; }
			set { _endereco = value; }
		}

		private List<FuncionarioRelatorio> _funcionarios = new List<FuncionarioRelatorio>();
		public List<FuncionarioRelatorio> Funcionarios
		{
			get { return _funcionarios; }
			set { _funcionarios = value; }
		}
	}
}
