using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao;
using Tests.TestHelpers;

namespace Tests
{
    [TestClass]
    public class AutenticacaoControllerTest
    {
        public AutenticacaoController testController = new AutenticacaoController();

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
        public void DeveLogarComSucesso()
        {
            ControllerContextMock.SetupNormalContext(testController);

            var result = testController.LogOn("antonio", "123456", null, null, null, null) as RedirectToRouteResult;

            Assert.AreEqual("Index", result.RouteValues["action"]);
            Assert.AreEqual("Home", result.RouteValues["controller"]);
        }
    }
}
