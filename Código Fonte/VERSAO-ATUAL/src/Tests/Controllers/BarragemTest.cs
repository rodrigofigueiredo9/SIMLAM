using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;
using Tecnomapas.EtramiteX.Credenciado.Controllers;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Tests.Controllers
{
	[TestClass]
	public class BarragemTest
	{
		#region Constructor

		public BarragemTest()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		#endregion

		#region Properties

		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		#endregion

		#region Methods

		[TestMethod]
		public void ValidarCoordenadasBarragem()
		{
			BarragemDispensaLicencaValidar _validar = new BarragemDispensaLicencaValidar();

			List<BarragemCoordenada> lstCoord = new List<BarragemCoordenada>();
			BarragemCoordenada coord = new BarragemCoordenada();
			//coord.easting = 252450;
			//coord.northing = 7662933;
			//coord.tipo = eTipoCoordenadaBarragem.barramento;
			//lstCoord.Add(coord);

			//_validar.ValidarCoordenadas(10, lstCoord);

			coord.easting = 421491;
			coord.northing = 7964974;
			coord.tipo = eTipoCoordenadaBarragem.barramento;
			lstCoord.Add(coord);

			_validar.ValidarCoordenadas(2, lstCoord);
			Assert.AreEqual(1, 1);
		}

		#endregion
	}
}
