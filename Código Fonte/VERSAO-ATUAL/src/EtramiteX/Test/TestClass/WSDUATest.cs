using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
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
		private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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

		[TestMethod]
		public void DeveGerarArquivoLog()
		{
			Log.Error("DUA: 4324465234 - CPF/CNPJ: 146.568.898-89", new Exception("Teste de erro"));
			var diretorioLog = new DirectoryInfo(string.Concat(AppDomain.CurrentDomain.BaseDirectory, "\\logs"));

			Assert.IsTrue(diretorioLog.GetFiles().Length > 0);
		}
	}
}
