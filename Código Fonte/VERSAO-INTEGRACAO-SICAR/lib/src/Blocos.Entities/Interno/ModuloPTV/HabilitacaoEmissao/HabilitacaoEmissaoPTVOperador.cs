using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao
{
	public class HabilitacaoEmissaoPTVOperador
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int FuncionarioId { get; set; }
		public string FuncionarioNome { get; set; }
		public HabilitacaoEmissaoPTVOperador() { }
		public List<Funcionario> Funcionario { get; set; }
	}
}