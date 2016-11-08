using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Oficio
{
	public class OficioNotificacaoVM
	{
		public bool IsVisualizar { get; set; }

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private OficioNotificacao _oficio = new OficioNotificacao();
		public OficioNotificacao Oficio
		{
			get { return _oficio; }
			set { _oficio = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		public OficioNotificacaoVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, 
			string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, destinatarios.Count > 1);
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
		}
	}
}