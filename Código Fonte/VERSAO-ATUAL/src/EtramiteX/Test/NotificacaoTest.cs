using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Test
{
	[TestClass]
	public class NotificacaoTest
	{
		NotificacaoBus _busNotificacao = new NotificacaoBus();
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		public NotificacaoTest()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		[TestMethod]
		public void CadastrarNotificacaoTest()
		{
			var entity = _busNotificacao.Obter(8851);
			if (entity == null)
			{
				entity = new Notificacao()
				{
					FiscalizacaoId = 8851,
					FormaIUF = 1,
					DataIUF = new DateTecno() { Data = new DateTime() }
				};
			}

			Assert.IsTrue(_busNotificacao.Salvar(entity));
		}

		[TestMethod]
		public void ObterNotificacaoTest()
		{
			var entity = _busNotificacao.Obter(8851);
			Assert.AreEqual(entity.FiscalizacaoId, 8851);
		}

		[TestMethod]
		public void ExcluirNotificacaoTest() => Assert.IsTrue(_busNotificacao.Excluir(8851));
	}
}
