using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tecnomapas.EtramiteX.Credenciado.Model.WebService.ModuloWSDUA;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Test.TestClass
{
	[TestClass]
	public class WSDUATest
	{
		#region Constructor

		public WSDUATest()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		#endregion

		#region Properties

		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		#endregion Properties

		[TestMethod]
		public void DeveRetornarObjetoDuaPreenchidoSeDuaEncontradoNaSefaz()
		{
			var wSDUA = new WSDUA(@"D:\Projetos\SIMLAM\Código Fonte\VERSAO-ATUAL\src\EtramiteX\Interno\Content\_chave\Chaves Pública e Privada.pfx");
			var dua = wSDUA.ObterDUA("2442324231", "131.684.517-62");

			Assert.IsNotNull(dua);
		}

		[TestMethod]
		public void DeveRetornarObjetoDuaNuloSeDuaNaoEncontradoNaSefaz()
		{
			var wSDUA = new WSDUA(@"D:\Projetos\SIMLAM\Código Fonte\VERSAO-ATUAL\src\EtramiteX\Interno\Content\_chave\Chaves Pública e Privada.pfx");
			var dua = wSDUA.ObterDUA("99999999", "131.684.517-62");

			Assert.IsNull(dua);
		}
	}
}
