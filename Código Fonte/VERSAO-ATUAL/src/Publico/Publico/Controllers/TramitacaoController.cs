using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProtocolo.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTramitacao.Business;
using Tecnomapas.EtramiteX.Publico.ViewModels.VMTramitacao;

namespace Tecnomapas.EtramiteX.Publico.Controllers
{
	public class TramitacaoController : DefaultController
    {
		TramitacaoBus _bus = new TramitacaoBus();
		ProtocoloBus _busProtocolo = new ProtocoloBus();

		public ActionResult Historico(int id, int tipo)
		{
			HistoricoVM vm = new HistoricoVM();

			ListarTramitacaoFiltro filtro = new ListarTramitacaoFiltro();
			filtro.Protocolo.Id = id;
			filtro.Protocolo.IsProcesso = (tipo == (int)eTipoProtocolo.Processo);

			Resultados<Tramitacao> hstTramitacao = _bus.FiltrarHistorico(filtro);
			Resultados<HistoricoProtocolo> hstProtocolo;
			IProtocolo protocolo = new Protocolo();

			hstProtocolo = _busProtocolo.FiltrarHistoricoAssociados(new ListarProtocoloFiltro()
			{
				Id = id,
				ProtocoloId = filtro.Protocolo.IsProcesso ? (int)eTipoProtocolo.Processo : (int)eTipoProtocolo.Documento
			});

			protocolo = _busProtocolo.ObterSimplificado(id);
			vm.TipoHistorico = filtro.Protocolo.IsProcesso ? "Processo" : "documento";
			vm.TipoHistoricoId = tipo;
			vm.AcaoHistoricoMostrarPdf = _bus.ObterHistoricoAcoesMostrarPdf();
			vm.CarregarHistorico(hstTramitacao.Itens, hstProtocolo, protocolo.Numero, protocolo.Tipo.Texto);

			ProtocoloLocalizacao loc = _busProtocolo.ObterLocalizacao(id);
			if (loc.Localizacao == eLocalizacaoProtocolo.OrgaoExterno)
			{
				vm.Localizacao = loc.OrgaoExternoNome;
			}
			else if (loc.Localizacao == eLocalizacaoProtocolo.Arquivado)
			{
				vm.Localizacao = loc.ArquivoNome;
			}
			else if (loc.Localizacao == eLocalizacaoProtocolo.EnviadoParaSetor || loc.Localizacao == eLocalizacaoProtocolo.EnviadoParaFuncionario)
			{
				vm.Localizacao = "Em tramitação";
			}
			else if (loc.ProcessoPaiId > 0 || loc.Localizacao == eLocalizacaoProtocolo.PosseFuncionario)
			{
				vm.Localizacao = loc.SetorDestinatarioNome;
			}

			return PartialView("HistoricoPartial", vm);
		}
	}
}
