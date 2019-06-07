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
			////simula o login
			//ControllerContextMock.SetupNormalContext(testController, "jessica.rossi");
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
		public void RetornaIdFuncionarioLogadoTest()
		{
			//simula o login
			ControllerContextMock.SetupNormalContext(testController, "jessica.rossi");

			//O funcionário logado é Jessica Lorena Natalli Rossi. Seu id é 733.
			int funcionarioId = (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId;

			Assert.AreEqual(733, funcionarioId);
		}

		[TestMethod]
		public void RetornaTrueCasoUsuarioComHabilitacaoAtivaEmissaoPTVTest()
		{
			//simula o login
			ControllerContextMock.SetupNormalContext(testController, "jessica.rossi");

			PTVValidar _validar = new PTVValidar();

			//A funcionária Jessica Rossi possui habilitação ativa para PTV ativa
			bool habilitado = _validar.FuncionarioHabilitadoValido();

			Assert.IsTrue(habilitado);
		}

		[TestMethod]
		public void RetornaFalseCasoUsuarioComHabilitacaoInativaEmissaoPTVTest()
		{
			//simula o login
			ControllerContextMock.SetupNormalContext(testController, "leonardo.costa");

			PTVValidar _validar = new PTVValidar();

			//o funcionário Leonardo Costa possui habilitação inativa para emissão de PTV
			bool habilitado = _validar.FuncionarioHabilitadoValido();

			Assert.IsFalse(habilitado);
		}

		[TestMethod]
		public void RetornaFalseCasoUsuarioSemHabilitacaoEmissaoPTVTest()
		{
			//simula o login
			ControllerContextMock.SetupNormalContext(testController, "jose.osmar");

			PTVValidar _validar = new PTVValidar();

			//o funcionário José Osmar não possui habilitação para emissão de PTV
			bool habilitado = _validar.FuncionarioHabilitadoValido();

			Assert.IsFalse(habilitado);
		}

		[TestMethod]
		public void RetornaZeroCasoNaoHajaEPTVAguardandoAnaliseSetorTest()
		{
			//simula o login
			//foi verificado que não existem EPTVs aguardando análise no setor desse usuário
			ControllerContextMock.SetupNormalContext(testController, "stefania.sgulmaro");
			int funcionarioId = (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId;

			PTVBus _bus = new PTVBus();
			int quantidade = 0;//_bus.QuantidadeEPTVAguardandoAnaliseFuncionario(funcionarioId);

			Assert.AreEqual(0, quantidade);
		}

		[TestMethod]
		public void RetornaMaiorQueZeroCasoHajaEPTVAguardandoAnaliseSetorTest()
		{
			//simula o login
			//foi verificado que existem EPTVs aguardando análise no setor desse usuário
			ControllerContextMock.SetupNormalContext(testController, "jessica.rossi");
			int funcionarioId = (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId;

			PTVBus _bus = new PTVBus();
			int quantidade = 1 //_bus.QuantidadeEPTVAguardandoAnaliseFuncionario(funcionarioId);

			Assert.IsTrue(quantidade > 0);
		}

		[TestMethod]
		public void RetornaTrueCasoDevaSerExibidoAlertaEPTVTest()
		{
			//simula o login
			//foi verificado que existem alertas para esse usuário
			ControllerContextMock.SetupNormalContext(testController, "jessica.rossi");
			int funcionarioId = (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId;

			PTVBus _bus = new PTVBus();
			bool existeAlerta = _bus.VerificaAlertaEPTV();

			Assert.IsTrue(existeAlerta);
		}

		[TestMethod]
		public void RetornaFalseCasoNaoDevaSerExibidoAlertaEPTVTest()
		{
			//simula o login
			//foi verificado que não existem alertas para esse usuário
			ControllerContextMock.SetupNormalContext(testController, "stefania.sgulmaro");
			int funcionarioId = (HttpContext.Current.User.Identity as EtramiteIdentity).FuncionarioId;

			PTVBus _bus = new PTVBus();
			bool existeAlerta = _bus.VerificaAlertaEPTV();

			Assert.IsFalse(existeAlerta);
		}
	}
}
