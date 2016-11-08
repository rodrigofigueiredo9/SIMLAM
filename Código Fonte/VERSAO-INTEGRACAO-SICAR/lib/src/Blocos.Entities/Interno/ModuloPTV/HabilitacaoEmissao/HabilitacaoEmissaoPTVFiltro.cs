using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV.HabilitacaoEmissao
{
	public class HabilitacaoEmissaoPTVFiltro
	{
		public string Funcionario { get; set; }
		public string CPF { get; set; }
		public string NumeroHabilitacao { get; set; }
		public string SituacaoTexto { get; set; }
		public int SetorId{ get; set; }

		public int Id { get; set; }

	}
}
