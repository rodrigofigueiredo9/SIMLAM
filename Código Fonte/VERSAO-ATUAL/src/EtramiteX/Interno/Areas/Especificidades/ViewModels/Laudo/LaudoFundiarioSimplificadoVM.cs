using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo
{
	public class LaudoFundiarioSimplificadoVM
	{
		public bool IsVisualizar { get; set; }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EspecificidadeConclusaoFavoravelId = eEspecificidadeConclusao.Favoravel
				});
			}
		}

		private LaudoFundiarioSimplificado _laudo = new LaudoFundiarioSimplificado();
		public LaudoFundiarioSimplificado Laudo
		{
			get { return _laudo; }
			set
			{
				_laudo = value;
				ArquivoVM.Anexos = _laudo == null ? new List<Anexo>() : _laudo.Anexos;
			}
		}

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private ArquivoVM _arquivoVM = new ArquivoVM();
		public ArquivoVM ArquivoVM
		{
			get { return _arquivoVM; }
			set { _arquivoVM = value; }
		}

		public LaudoFundiarioSimplificadoVM(LaudoFundiarioSimplificado laudo, List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			Laudo = laudo;
			IsVisualizar = isVisualizar;
			ArquivoVM.IsVisualizar = isVisualizar;
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, Laudo.Destinatario.ToString());

		}

		public LaudoFundiarioSimplificadoVM()
		{

		}

	}
}