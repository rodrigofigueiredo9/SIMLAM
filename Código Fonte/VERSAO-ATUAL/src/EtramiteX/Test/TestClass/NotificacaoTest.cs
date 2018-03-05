using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Test
{
	[TestClass]
	public class NotificacaoTest
	{
		private int fiscalizacaoId = 8851; //Ao executar o teste, preencher com o Id de uma fiscalização válida
		NotificacaoBus _busNotificacao = new NotificacaoBus();
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		public NotificacaoTest()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		[TestMethod]
		public void CadastrarNotificacaoTest()
		{
			var entity = _busNotificacao.Obter(fiscalizacaoId);
			if (entity.Id == 0)
			{
				entity.FormaIUF = 1;
				entity.DataIUF = new DateTecno() { Data = new DateTime() };
			}

			Assert.IsTrue(_busNotificacao.Salvar(entity));
		}

		[TestMethod]
		public void ObterNotificacaoTest()
		{
			var entity = _busNotificacao.Obter(fiscalizacaoId);
			Assert.IsNotNull(entity);
		}

		[TestMethod]
		public void ExcluirNotificacaoTest() => Assert.IsTrue(_busNotificacao.Excluir(fiscalizacaoId));
	}
}
