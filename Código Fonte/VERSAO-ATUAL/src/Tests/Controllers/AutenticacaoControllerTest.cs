using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.Model.Security.Fakes;
using Tecnomapas.EtramiteX.Credenciado.Model.Security.Interfaces;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao;
using Tests.Fakes;
using Tests.TestHelpers;
using Tecnomapas.EtramiteX.Credenciado.Services;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tests
{
	[TestClass]
	public class AutenticacaoControllerTest
	{
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		private void assertLoginSession(String login)
		{
			ControllerContextMock.SetupNormalContext(testController);

			String senha = "123456";
			String sessionId = "";

			if (GerenciarAutenticacao.ValidarLogOn(login, senha, out sessionId))
			{
				if (!testController.HasToAlterarSenha(login))
				{
					this.assertNormalLoginBehavior(login, senha);
				}
				else
				{
					this.assertChangePassLoginBehavior(login, senha);
				}
			}
		}

		private void assertChangePassLoginBehavior(String login, String senha)
		{
			var result = testController.LogOn(login, senha, null, null, null, null) as ViewResult;

			Assert.IsNull(HttpContext.Current.User);
		}

		private void assertNormalLoginBehavior(String login, String senha)
		{
			var result = testController.LogOn(login, senha, null, null, null, null) as RedirectToRouteResult;

			EtramitePrincipal user = HttpContext.Current.User as EtramitePrincipal;
			EtramiteIdentity identity = user.Identity as EtramiteIdentity;

			Assert.AreEqual(login, identity.Login);
			Assert.AreEqual("Index", result.RouteValues["action"]);
			Assert.AreEqual("Home", result.RouteValues["controller"]);
		}

		[TestMethod]
		public void DeveInstanciarPorDefaultComFormsAuthenticationService()
		{
			Assert.IsInstanceOfType(
				new AutenticacaoController().formsAuthenticationService,
				typeof(FormsAuthenticationService)
			);
		}

		[TestMethod]
		public void DeveRetornarViewResultDeLogOn()
        {
            ControllerContextMock.SetupNormalContext(testController);

            var result = testController.LogOn(null) as ViewResult;

            Assert.IsInstanceOfType(result.Model, typeof(LogonVM));
        }

        [TestMethod]
        public void DeveRetornarPartialViewResultDeLogOn()
        {
            ControllerContextMock.SetupAjaxContext(testController);

            var result = testController.LogOn(null) as PartialViewResult;

            Assert.IsInstanceOfType(result.Model, typeof(LogonVM));
            Assert.AreEqual("LogOnPartial", result.ViewName);
        }

        [TestMethod]
        public void DeveRetornarHomeIndexAoLogarComSucesso()
        {
			ControllerContextMock.SetupNormalContext(testController);

			this.assertNormalLoginBehavior("antonio", "123456");
        }

        [TestMethod]
        public void DeveRetornarPartialViewAoLogarComSucesso()
        {
            ControllerContextMock.SetupAjaxContext(testController);

            var result = testController.LogOn("antonio", "123456", null, null, null, null) as PartialViewResult;

            Assert.IsInstanceOfType(result.Model, typeof(LogonVM));
            Assert.AreEqual("LogOnPartial", result.ViewName);
        }

		[TestMethod]
		public void DeveRetornarAPaginaDeLogonAoTentarLogar()
		{
			ControllerContextMock.SetupNormalContext(testController);

			var result = testController.LogOn("randomuser", "1234567", null, null, null, null) as ViewResult;

			Assert.IsInstanceOfType(result.Model, typeof(LogonVM));
		}

		[TestMethod]
		public void DeveRetornarPartialViewAoTentarLogar()
		{
			ControllerContextMock.SetupAjaxContext(testController);

			var result = testController.LogOn("Antônio", "1234567", null, null, null, null) as PartialViewResult;

			Assert.IsInstanceOfType(result.Model, typeof(LogonVM));
			Assert.AreEqual("LogOnPartial", result.ViewName);
		}

		[TestMethod]
		public void DeveRedirecionarAoLogar()
		{
			ControllerContextMock.SetupNormalContext(testController);
			String url = "Caracterizacoes/Caracterizacao/Index/54512";

			var result = testController.LogOn("antonio", "123456", null, null, null, url) as RedirectResult;

			Assert.AreEqual(url, result.Url);
		}

		[TestMethod]
		public void DeveLogarCaseSensitive()
		{
			this.assertLoginSession("Antônio");
			this.assertLoginSession("antonio");
			this.assertLoginSession("Andressa");
			this.assertLoginSession("andressa");
			this.assertLoginSession("Anderson");
			this.assertLoginSession("anderson");
		}

		[TestMethod]
		public void DeveLancarErroNoCatcher()
		{
			Validacao.Erros.Clear();

			AutenticacaoController buggyController = new AutenticacaoController(new FakeBuggyFormsAuthenticationService());

			ControllerContextMock.SetupNormalContext(buggyController);

			Assert.AreEqual(0, Validacao.Erros.Count);

			var result = buggyController.LogOn("antonio", "123456", null, null, null, null) as ViewResult;

			Assert.AreEqual(1, Validacao.Erros.Count);
			Assert.IsInstanceOfType(result.Model, typeof(LogonVM));

			Validacao.Erros.Clear();
		}
    }
}