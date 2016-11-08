using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloNotificacao;
using Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Especificidade;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Notificacao
{
	public class NotificacaoEmbargoVM
	{
		public bool IsVisualizar { set; get; }

		private DestinatarioEspecificidadeVM _destinatariosVM = new DestinatarioEspecificidadeVM();
		public DestinatarioEspecificidadeVM DestinatariosVM
		{
			get { return _destinatariosVM; }
			set { _destinatariosVM = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<SelectListItem> _atividadesEmbargo = new List<SelectListItem>();
		public List<SelectListItem> AtividadesEmbargo
		{
			get { return _atividadesEmbargo; }
			set { _atividadesEmbargo = value; }
		}

		private NotificacaoEmbargo _notificacao = new NotificacaoEmbargo();
		public NotificacaoEmbargo Notificacao
		{
			get { return _notificacao; }
			set { _notificacao = value; }
		}

		public NotificacaoEmbargoVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, NotificacaoEmbargo notificacao, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Notificacao = notificacao;
			
			DestinatariosVM.IsVisualizar = isVisualizar;
			DestinatariosVM.IsVisualizar = isVisualizar;
			DestinatariosVM.DestinatariosLst = ViewModelHelper.CriarSelectList(destinatarios, true, true);
			DestinatariosVM.Destinatarios = (notificacao != null && notificacao.Destinatarios != null) ? notificacao.Destinatarios : new List<DestinatarioEspecificidade>();
			DestinatariosVM.DestinatariosLst = ViewModelHelper.CriarSelectList(destinatarios, true, true);

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;
		}

		public NotificacaoEmbargoVM() { }

	}
}