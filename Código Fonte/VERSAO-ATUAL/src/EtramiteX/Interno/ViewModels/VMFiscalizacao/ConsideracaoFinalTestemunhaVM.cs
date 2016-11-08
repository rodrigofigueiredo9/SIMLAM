using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class ConsideracaoFinalTestemunhaVM
	{
		public bool IsVisualizar { get; set; }
		public List<SelectListItem> Funcionarios { get; set; }
		public ConsideracaoFinalTestemunha Testemunha { get; set; }
		public List<SelectListItem> FuncionarioIDAF { get; set; }
		public List<SelectListItem> Setores { get; set; }
		
		public ConsideracaoFinalTestemunhaVM()
		{
			this.Testemunha = new ConsideracaoFinalTestemunha();
			this.Funcionarios = new List<SelectListItem>();
			this.FuncionarioIDAF = new List<SelectListItem>();
			this.Setores = new List<SelectListItem>();
		}
	}
}