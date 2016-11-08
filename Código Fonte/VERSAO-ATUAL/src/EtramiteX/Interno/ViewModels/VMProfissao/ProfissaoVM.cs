using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProfissao
{
	public class ProfissaoVM
	{
		public Profissao Profissao { get; set; }
		public bool IsVisualizar { get; set; }

		public ProfissaoVM(Profissao profissao = null)
		{
			Profissao = profissao ?? new Profissao();
		}
	}
}