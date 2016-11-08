using System.Web.Mvc;
using System.Web.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Etx.Security;
using Tecnomapas.EtramiteX.Gerencial.ViewModels.VMFuncionario;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Gerencial.Controllers
{
	public class FuncionarioController : DefaultController
	{
		#region Propriedades

		ListaBus _busLista = new ListaBus();
		FuncionarioBus _bus = new FuncionarioBus(new FuncionarioValidar());

		#endregion

		#region Alterar Funcionário

		public ActionResult AlterarFuncionario(int id)
		{
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				EditarVM viewModel = new EditarVM();
				EtramiteIdentity func = User.Identity as EtramiteIdentity;
				if (func == null || func.FuncionarioTipo != 3) // se não for "funcionário" mostra mensagem
				{
					Validacao.Add(Mensagem.Funcionario.SemPermissao);
				}
				else
				{
					if (_bus.VerificarAlterarFuncionario(func.FuncionarioId))
					{
						viewModel.Funcionario = _bus.Obter(func.FuncionarioId);
					}
				}

				return View("AlterarFuncionario", viewModel);
			}

			return Redirect(FormsAuthentication.LoginUrl);
		}

		[HttpPost]
		public ActionResult AlterarFuncionario(int id, FuncionarioVM funcVM)
		{
			if (HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
			{
				EditarVM viewModel = new EditarVM();
				EtramiteIdentity func = User.Identity as EtramiteIdentity;

				if (func == null || func.FuncionarioTipo != 3) // se não for "funcionário" mostra mensagem
				{
					Validacao.Add(Mensagem.Funcionario.SemPermissao);
				}
				else
				{
					if (_bus.VerificarAlterarFuncionario(id))
					{
						viewModel.Funcionario = _bus.Obter((User.Identity as EtramiteIdentity).FuncionarioId);
						_bus.AlterarSenhaFuncionario(viewModel.Funcionario, funcVM.Senha, funcVM.ConfirmarSenha);
					}
				}

				return View("AlterarFuncionario", viewModel);
			}
			else
			{
				return Redirect(FormsAuthentication.LoginUrl);
			}
		}

		#endregion
	}
}