using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Test
{
	[TestClass]
	public class ParametrizacaoTest
	{
		ConfigFiscalizacaoBus _busConfiguracao = new ConfigFiscalizacaoBus();
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		public ParametrizacaoTest()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		[TestMethod]
		public void CadastrarParametrizacaoTest()
		{
			var entity = new Parametrizacao()
			{
				CodigoReceitaId = _busConfiguracao.ObterCodigosReceita()[0].Id,
				InicioVigencia = new DateTecno() { Data = new DateTime(2000,1,1) },
				ValorMinimoPF = 200,
				ValorMinimoPJ = 400,
				ParametrizacaoDetalhes = new List<ParametrizacaoDetalhe>()
				{
					new ParametrizacaoDetalhe()
					{
						MaximoParcelas = 10,
						ValorInicial = 0.01M
					}
				}
			};

			Assert.IsTrue(_busConfiguracao.SalvarParametrizacao(entity));
		}


		[TestMethod]
		public void ObterParametrizacaoTest()
		{
			var entity = _busConfiguracao.ObterParametrizacao().FindLast(x => x.Id > 0);
			Assert.IsNotNull(entity);

			var findOneEntity = _busConfiguracao.ObterParametrizacao(entity.Id);
			Assert.AreEqual(findOneEntity.Id, entity.Id);
		}

		[TestMethod]
		public void ExcluirParametrizacaoTest()
		{
			var parametrizacaoId = _busConfiguracao.ObterParametrizacao().FindLast(x => x.Id > 0).Id;
			Assert.IsTrue(_busConfiguracao.ExcluirParametrizacao(parametrizacaoId));
		}
	}
}
