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
	public class AberturaLivroUnidadeProducaoVM
	{
		public bool IsVisualizar { set; get; }

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<SelectListItem> _unidadesProducoes = new List<SelectListItem>();
		public List<SelectListItem> UnidadesProducoes
		{
			get { return _unidadesProducoes; }
			set { _unidadesProducoes = value; }
		}

		private AberturaLivroUnidadeProducao _termo = new AberturaLivroUnidadeProducao();
		public AberturaLivroUnidadeProducao Termo
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
					UnidadeProducaoObrigatorio = Mensagem.AberturaLivroUnidadeProducao.UnidadeProducaoObrigatorio,
					UnidadeProducaoJaAdicionada = Mensagem.AberturaLivroUnidadeProducao.UnidadeProducaoJaAdicionada
				});
			}
		}

		public AberturaLivroUnidadeProducaoVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<Lista> unidades, AberturaLivroUnidadeProducao termo, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			string unidade = "0";

			if (unidades.Count ==1)
			{
				unidade = unidades.First().Id;
			}

			IsVisualizar = isVisualizar;
			Termo = termo;
			UnidadesProducoes = ViewModelHelper.CriarSelectList(unidades, true, true, unidade);
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

		}

		public AberturaLivroUnidadeProducaoVM() { }

	}
}