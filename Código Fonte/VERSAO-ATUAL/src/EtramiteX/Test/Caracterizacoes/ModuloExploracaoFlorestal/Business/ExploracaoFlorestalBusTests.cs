using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.TestHelpers;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tests.Fakes;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business.Tests
{
	[TestClass()]
	public class ExploracaoFlorestalBusTests
	{
		#region Constructor

		public ExploracaoFlorestalBusTests()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		#endregion

		#region Properties

		ExploracaoFlorestalBus _bus = new ExploracaoFlorestalBus();
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		#endregion Properties

		[TestMethod()]
		public void FinalizarExploracaoTest()
		{
			_bus.FinalizarExploracao(15);

			Assert.Fail();
		}
	}
}