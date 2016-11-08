using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo
{
	public class AberturaLivroUnidadeConsolidacaoVM
	{
		public bool IsVisualizar { set; get; }

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<SelectListItem> _culturas = new List<SelectListItem>();
		public List<SelectListItem> Culturas
		{
			get { return _culturas; }
			set { _culturas = value; }
		}

		private AberturaLivroUnidadeConsolidacao _termo = new AberturaLivroUnidadeConsolidacao();
		public AberturaLivroUnidadeConsolidacao Termo
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
					CulturaObrigatoria = Mensagem.AberturaLivroUnidadeConsolidacao.CulturaObrigatoria,
					CulturaJaAdicionada = Mensagem.AberturaLivroUnidadeConsolidacao.CulturaJaAdicionada
				});
			}
		}

		public AberturaLivroUnidadeConsolidacaoVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<Lista> culturas, AberturaLivroUnidadeConsolidacao termo, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Termo = termo;
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;
			Culturas = ViewModelHelper.CriarSelectList(culturas, true, true);
		}

		public AberturaLivroUnidadeConsolidacaoVM() { }

	}
}