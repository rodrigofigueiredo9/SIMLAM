using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.QualityTools.Testing.Fakes;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao;
using Tests.TestHelpers;
using Moq;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.Model.Security.Interfaces;
using Tecnomapas.EtramiteX.Credenciado.Model.Security.Fakes;
using Tests.Fakes;
using System.Security.Principal;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;

namespace Tests
{
    [TestClass]
    public class AutenticacaoControllerTest
    {
        public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

        private void assertLoginSession(String login)
        {
            ControllerContextMock.SetupNormalContext(testController);

            var result = testController.LogOn(login, "123456", null, null, null, null) as RedirectToRouteResult;

            EtramitePrincipal user = HttpContext.Current.User as EtramitePrincipal;
            EtramiteIdentity identity = user.Identity as EtramiteIdentity;

            Assert.AreEqual(login, identity.Login);
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

            var result = testController.LogOn("antonio", "123456", null, null, null, null) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
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
        public void DeveDiferenciarLoginsComAcentoESemAcento()
        {
            this.assertLoginSession("Antônio");
            this.assertLoginSession("antonio");
            this.assertLoginSession("andressa");
            this.assertLoginSession("Anderson");
        }
    }
}
