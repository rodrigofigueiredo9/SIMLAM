using Interno.Model.WebService.ModuloWSSicar;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tests.Fakes;
using Tests.TestHelpers;

namespace Test.TestClass
{
	[TestClass]
	public class SicarAnaliseTest
	{
		#region Constructor

		public SicarAnaliseTest()
		{
			ControllerContextMock.SetupNormalContext(testController);
		}

		#endregion

		#region Properties

		CobrancaBus _bus = new CobrancaBus();
		PessoaBus _busPessoa = new PessoaBus();
		FiscalizacaoBus _busFiscalizacao = new FiscalizacaoBus();
		NotificacaoBus _busNotificacao = new NotificacaoBus();
		public AutenticacaoController testController = new AutenticacaoController(new FakeFormsAuthenticationService());

		#endregion Properties

		#region Private Methods

		private Cobranca GetCobranca(int codigoReceita)
		{
			var cobranca = _bus.Obter(1);

			if (cobranca == null)
			{
				cobranca = new Cobranca()
				{
					Id = 1,
					DataEmissaoIUF = new DateTecno() { Data = DateTime.Now.AddDays(-60) },
					DataIUF = new DateTecno() { Data = DateTime.Now.AddDays(-60) },
					CodigoReceitaId = codigoReceita,
					AutuadoPessoa = _busPessoa.Obter(2525)
				};
				cobranca.AutuadoPessoaId = cobranca.AutuadoPessoa.Id;

				var parcelamento = new CobrancaParcelamento()
				{
					Id = 1,
					ValorMulta = 1959.48M,
					DataEmissao = new DateTecno() { Data = DateTime.Now },
					Data1Vencimento = new DateTecno() { Data = DateTime.Now.AddDays(30) },
					CobrancaId = 1
				};

				cobranca.Parcelamentos = new List<CobrancaParcelamento>() { parcelamento };
			}

			return cobranca;
		}

		#endregion Private Methods

		[TestMethod]
		public void GetAnaliseSicar()
		{
			CarAnaliseService car = new CarAnaliseService(new SicarAnalise());

			Assert.AreEqual(maximoParcelas, 2);
		}

		[TestMethod]
		public void ResultadoMaximoDeParcelasDeveSerIgual2Test()
		{
			var cobranca = this.GetCobranca(3);
			var parcelamento = cobranca.Parcelamentos[0];
			var maximoParcelas = _bus.GetMaximoParcelas(cobranca, parcelamento);

			Assert.AreEqual(maximoParcelas, 2);
		}

		[TestMethod]
		public void DeveRetornarTrueAoGerarMaisDeUmaParcelaTest()
		{
			var cobranca = this.GetCobranca(3);
			var parcelamento = cobranca.Parcelamentos[0];
			parcelamento.QuantidadeParcelas = _bus.GetMaximoParcelas(cobranca, parcelamento);
			parcelamento.DUAS = _bus.GerarParcelas(cobranca, parcelamento);

			Assert.IsTrue(parcelamento.DUAS.Count > 1);
		}

		[TestMethod]
		public void DeveRetornarTrueAoCalcularDuaComParametrizacaoExistenteTest()
		{
			var cobranca = this.GetCobranca(3);
			var parcelamento = cobranca.Parcelamentos[0];
			Assert.IsTrue(_bus.CalcularParcelas(cobranca, parcelamento));
		}

		[TestMethod]
		public void DeveRetornarFalseAoCalcularDuaComParametrizacaoNaoExistenteTest()
		{
			var cobranca = this.GetCobranca(23);
			var parcelamento = cobranca.Parcelamentos[0];
			Assert.IsFalse(_bus.CalcularParcelas(cobranca, parcelamento));
		}
	}
}
