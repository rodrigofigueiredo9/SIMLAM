using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo
{
	public class TermoCompromissoAmbientalVM
	{
		public bool IsVisualizar { set; get; }

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private List<SelectListItem> _representantes = new List<SelectListItem>();
		public List<SelectListItem> Representantes
		{
			get { return _representantes; }
			set { _representantes = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private TermoCompromissoAmbiental _termo = new TermoCompromissoAmbiental();
		public TermoCompromissoAmbiental Termo
		{
			get { return _termo; }
			set { _termo = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@DestinatarioObrigatorio = Mensagem.TermoCPFARLMsg.DestinatarioObrigatorio,
					@DestinatarioJaAdicionado = Mensagem.TermoCPFARLMsg.DestinatarioJaAdicionado
				});
			}
		}

		public String ModelosCodigosTitulo
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@LicencaAmbientalRegularizacao = (int)eEspecificidade.LicencaAmbientalRegularizacao
				});
			}
		}

		public TermoCompromissoAmbientalVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, List<PessoaLst> representantes, TermoCompromissoAmbiental termo, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Termo = termo;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, termo.Destinatario.ToString());
			Representantes = ViewModelHelper.CriarSelectList(representantes, true, true, termo.Representante.ToString());
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

		}

		public TermoCompromissoAmbientalVM() { }

	}
}