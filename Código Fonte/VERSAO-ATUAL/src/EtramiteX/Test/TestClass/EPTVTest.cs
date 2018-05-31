using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Business;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Test.TestClass
{
	[TestClass]
	public class EPTVTest : HttpApplication
	{
		#region Constructor

		public EPTVTest()
		{
			//simula o login
			ControllerContextMock.SetupNormalContext(testController, "jessica.rossi");
		}

		#endregion

		#region Properties
		
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		#endregion Properties

		#region Testes de cookies
		[Ignore]
		[TestMethod]
		public void SeEstiverLogadoRetornaCookieTest()
		{
			bool inseriu = testController.InsereCookieEPTV();

			//HttpCookie cookieEPTV = Request.Cookies["eptv"];

			Assert.IsTrue(inseriu);
		}

		[Ignore]
		[TestMethod]
		public void SeNaoEstiverLogadoRetornaCookieNuloTest()
		{
			
			Assert.Fail();
		}

		[Ignore]
		[TestMethod]
		public void SeCookieNaoNuloRetornaValueStringDataTest()
		{

			Assert.Fail();
		}

		[Ignore]
		[TestMethod]
		public void SeCookiePassouUmaHoraRetornaTrueTest()
		{

			Assert.Fail();
		}

		[Ignore]
		[TestMethod]
		public void SeCookieNaoPassouUmaHoraRetornaFalseTest()
		{

			Assert.Fail();
		}

		#endregion Testes de cookies

		[TestMethod]
		public void RetornaTrueCasoUsuarioComHabilitacaoAtivaEmissaoPTVTest()
		{
			PTVValidar _validar = new PTVValidar();
			bool habilitado = _validar.FuncionarioHabilitadoValido();

			Assert.IsTrue(habilitado);
		}

		[TestMethod]
		public void RetornaIdFuncionarioLogadoTest()
		{
			//O funcionário logado é Jessica Lorena Natalli Rossi. Seu id é 733.
			int funcionarioId = (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId;

			Assert.AreEqual(733, funcionarioId);
		}

		[TestMethod]
		public void RetornaFalseCasoUsuarioComHabilitacaoInativaEmissaoPTVTest()
		{
			HabilitacaoEmissaoPTVBus _bus = new HabilitacaoEmissaoPTVBus();
			int funcionarioId = (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId;

			Assert.Fail();
		}

		[TestMethod]
		public void RetornaFalseCasoUsuarioSemHabilitacaoEmissaoPTVTest()
		{

			Assert.Fail();
		}

		[TestMethod]
		public void RetornaZeroCasoNaoHajaEPTVAguardandoAnaliseSetorTest()
		{

			Assert.Fail();
		}

		[TestMethod]
		public void RetornaMaiorQueZeroCasoHajaEPTVAguardandoAnaliseSetorTest()
		{

			Assert.Fail();
		}
	}
}
