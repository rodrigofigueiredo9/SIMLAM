using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Test
{
	[TestClass]
	public class VrteTest
	{
		ConfigFiscalizacaoBus _busConfiguracao = new ConfigFiscalizacaoBus();
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		public VrteTest()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		[TestMethod]
		public void CadastrarVrteTest()
		{
			var entity = new Vrte()
			{
				Ano = 2018,
				VrteEmReais = 3.2658M
			};

			var listVrte = _busConfiguracao.ObterVrte();
			listVrte.Add(entity);

			Assert.IsTrue(_busConfiguracao.SalvarVrte(listVrte));
		}

		[TestMethod]
		public void ObterVrteTest()
		{
			var entity = _busConfiguracao.ObterVrte().FindLast(x => x.Id > 0);
			Assert.IsNotNull(entity);
		}

		[TestMethod]
		public void ExcluirVrteTest()
		{
			var listVrte = _busConfiguracao.ObterVrte();
			var entity = listVrte.FindLast(x => x.Ano == 2018 && x.VrteEmReais == 3.2658M);

			Assert.IsTrue(_busConfiguracao.PermiteExcluirVrte(entity));

			entity.Excluir = true;

			Assert.IsTrue(_busConfiguracao.SalvarVrte(listVrte));
		}
	}
}
