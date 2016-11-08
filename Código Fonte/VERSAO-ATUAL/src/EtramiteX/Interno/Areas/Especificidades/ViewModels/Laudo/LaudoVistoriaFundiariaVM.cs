using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo
{
	public class LaudoVistoriaFundiariaVM
	{
		public bool IsVisualizar { get; set; }

		private LaudoVistoriaFundiaria _laudo = new LaudoVistoriaFundiaria();
		public LaudoVistoriaFundiaria Laudo
		{
			get { return _laudo; }
			set { _laudo = value; }
		}

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		private List<SelectListItem> _posses = new List<SelectListItem>();
		public List<SelectListItem> Posses
		{
			get { return _posses; }
			set { _posses = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@RegularizacaoDominioJaAdicionado = Mensagem.LaudoVistoriaFundiariaMsg.RegularizacaoDominioJaAdicionado,
					@RegularizacaoComprovacaoObrigatoria = Mensagem.LaudoVistoriaFundiariaMsg.RegularizacaoComprovacaoObrigatoria
				});
			}
		}

		public LaudoVistoriaFundiariaVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, List<Lista> posses,
			string processoDocumentoSelecionado = null, int atividadeSelecionada = 0, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true);
			Posses = ViewModelHelper.CriarSelectList(posses, true);
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades ?? new List<AtividadeSolicitada>(), processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;
		}
	}
}