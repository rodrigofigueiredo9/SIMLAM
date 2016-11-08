using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.EtramiteX.Gerencial.ViewModels.VMFuncionario
{
	public class EditarVM
	{
		public Funcionario Funcionario { get; set; }
		public string TextoPermissoes { get; set; }

		public List<SelectListItem> Cargos { get; private set; }
		public List<SelectListItem> Setores { get; private set; }
		public List<PapeisVME> Papeis { get; set; }

		public EditarVM() { }

		public EditarVM(List<Cargo> cargos, List<Setor> setores)
		{
			Funcionario = new Funcionario();
			Papeis = new List<PapeisVME>();

			Cargos = ViewModelHelper.CriarSelectList(cargos, true);
			Setores = ViewModelHelper.CriarSelectList(setores, true);
		}
	}
}