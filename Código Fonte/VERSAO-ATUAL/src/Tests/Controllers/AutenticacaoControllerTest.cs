using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Tecnomapas.EtramiteX.Credenciado.ViewModels.VMAutenticacao;
using Moq;

namespace Tests
{
    [TestClass]
    public class AutenticacaoControllerTest
    {
        public AutenticacaoController testController = new AutenticacaoController();

        /**
         * Método Initialize()
         * 
         * É chamado na construção do teste. 
         * Cria um objeto de mock para os requests Http do framework MVC no inicio dos testes,
         * de forma que possamos usar a variavel testController para realizar asserções.
         * 
         **/
        [TestInitialize()]
        public void Initialize()
        {
            var request = new Mock<HttpRequestBase>();

            request
                .SetupGet(x => x.Headers)
                .Returns(new System.Net.WebHeaderCollection());

            var context = new Mock<HttpContextBase>();

            context
                .SetupGet(x => x.Request)
                .Returns(request.Object);

            testController.ControllerContext = new ControllerContext(context.Object, new RouteData(), testController);
        }

        [TestMethod]
        public void DeveRetornarViewModelDeLogOn()
        {
            var view = testController.LogOn(null) as ViewResult;

            Assert.IsInstanceOfType(view.Model, typeof(LogonVM));
        }
    }
}
