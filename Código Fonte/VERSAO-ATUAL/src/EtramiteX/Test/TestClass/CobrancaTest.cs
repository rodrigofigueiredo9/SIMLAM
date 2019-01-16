using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.EtramiteX.Interno.Controllers;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Business;
using Tests.Fakes;
using Tests.TestHelpers;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;

namespace Test.TestClass
{
	[TestClass]
	public class CobrancaTest
	{
		#region Constructor

		public CobrancaTest()
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

		#region Methods Private

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

				parcelamento.QuantidadeParcelas = _bus.GetMaximoParcelas(cobranca, parcelamento);
				parcelamento.DUAS = _bus.GerarParcelas(cobranca, parcelamento);
				cobranca.Parcelamentos = new List<CobrancaParcelamento>() { parcelamento };
			}

			return cobranca;
		}

		#endregion Methods Private

		[TestMethod]
		public void CalculoDuaComParametrizacaoExistenteTest()
		{
			var cobranca = this.GetCobranca(3);
			var parcelamento = cobranca.Parcelamentos[0];
			Assert.IsTrue(_bus.CalcularParcelas(cobranca, parcelamento));
		}

		[TestMethod]
		public void CalculoDuaComParametrizacaoNaoExistenteTest()
		{
			var cobranca = this.GetCobranca(23);
			var parcelamento = cobranca.Parcelamentos[0];
			Assert.IsFalse(_bus.CalcularParcelas(cobranca, parcelamento));
		}

		[TestMethod]
		public void ObterPDFInformacaoCorteTest()
		{
			OutrosInformacaoCorteDeclaratorioBus _bus = new OutrosInformacaoCorteDeclaratorioBus();
			var outros = _bus.Obter(151048) as Especificidade;
			outros.Titulo.Id = 151048;
			_bus.ObterDadosPdf(outros, null) ;
			Assert.IsFalse(true);
		}
	}
}
