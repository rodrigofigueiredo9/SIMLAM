using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloAquicultura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloAvicultura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicensa.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBeneficiamentoMadeira.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDespolpamentoCafe.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPatioLavagem.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProducaoCarvaoVegetal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSecagemMecanicaGraos.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilvicultura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaPPFF.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSuinocultura.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloTerraplanagem.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business
{
	public static class CaracterizacaoBusFactory
	{
		public static ICaracterizacaoBus Criar(eCaracterizacao caracterizacao)
		{
			switch (caracterizacao)
			{
				case eCaracterizacao.Nulo:
					return null;

				case eCaracterizacao.Dominialidade:
					return new DominialidadeBus();
				case eCaracterizacao.RegularizacaoFundiaria:
					return new RegularizacaoFundiariaBus();
				case eCaracterizacao.ExploracaoFlorestal:
					return new ExploracaoFlorestalBus();
				case eCaracterizacao.QueimaControlada:
					return new QueimaControladaBus();
				case eCaracterizacao.SecagemMecanicaGraos:
					return new SecagemMecanicaGraosBus();
				case eCaracterizacao.ProducaoCarvaoVegetal:
					return new ProducaoCarvaoVegetalBus();
				case eCaracterizacao.DespolpamentoCafe:
					return new DespolpamentoCafeBus();
				case eCaracterizacao.Avicultura:
					return new AviculturaBus();
				case eCaracterizacao.Suinocultura:
					return new SuinoculturaBus();
				case eCaracterizacao.BeneficiamentoMadeira:
					return new BeneficiamentoMadeiraBus();
				case eCaracterizacao.Terraplanagem:
					return new TerraplanagemBus();
				case eCaracterizacao.SilviculturaPPFF:
					return new SilviculturaPPFFBus();
				case eCaracterizacao.Barragem:
					return new BarragemBus();
				case eCaracterizacao.RegistroAtividadeFlorestal:
					return new RegistroAtividadeFlorestalBus();
				case eCaracterizacao.Aquicultura:
					return new AquiculturaBus();
				case eCaracterizacao.Silvicultura:
					return new SilviculturaBus();
				case eCaracterizacao.SilviculturaATV:
					return new SilviculturaATVBus();
				case eCaracterizacao.InformacaoCorte:
					return new InformacaoCorteBus();
				case eCaracterizacao.PulverizacaoProduto:
					return new PulverizacaoProdutoBus();
				case eCaracterizacao.PatioLavagem:
					return new PatioLavagemBus();
				case eCaracterizacao.UnidadeConsolidacao:
					return new UnidadeConsolidacaoBus();
				case eCaracterizacao.UnidadeProducao:
					return new UnidadeProducaoBus();
				case eCaracterizacao.BarragemDispensaLicenca:
					return new BarragemDispensaLicencaBus();
				default:
					return null;
			}
		}
	}
}