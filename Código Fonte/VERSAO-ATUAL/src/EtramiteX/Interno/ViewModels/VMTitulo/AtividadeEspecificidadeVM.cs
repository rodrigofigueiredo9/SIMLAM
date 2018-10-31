using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class AtividadeEspecificidadeVM
	{
		public bool IsVisualizar { get; set; }

		private List<SelectListItem> _processosDocumentosLst = new List<SelectListItem>();
		public List<SelectListItem> ProcessosDocumentosLst
		{
			get { return _processosDocumentosLst; }
			set { _processosDocumentosLst = value; }
		}

		private List<SelectListItem> _atividadeLst = new List<SelectListItem>();
		public List<SelectListItem> AtividadeLst
		{
			get { return _atividadeLst; }
			set { _atividadeLst = value; }
		}

		private List<Atividade> _atividades = new List<Atividade>();
		public List<Atividade> Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private bool _mostrarBotoes = true;
		public bool MostrarBotoes
		{
			get { return _mostrarBotoes; }
			set { _mostrarBotoes = value; }
		}

		private List<SelectListItem> _destinatarios = new List<SelectListItem>();
		public List<SelectListItem> Destinatarios
		{
			get { return _destinatarios; }
			set { _destinatarios = value; }
		}

		public eEspecificidade Especificidade { get; set; }
		public eEspecificidadeTipo EspecificidadeTipo { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@AtividadeJaAdicionada = Mensagem.AtividadeEspecificidade.AtividadeJaAdicionada,
					@AtividadeObrigatoria = Mensagem.AtividadeEspecificidade.AtividadeObrigatoria
				});
			}
		}

		public AtividadeEspecificidadeVM()
		{
		}

		public AtividadeEspecificidadeVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, string processoDocumentoSelecionado = null, int atividadeSelecionada = 0, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;

			if (processosDocumentos.Count <= 0)
			{
				ProcessosDocumentosLst = ViewModelHelper.CriarSelectList(processosDocumentos, null, true, selecionado: processoDocumentoSelecionado);
			}
			else
			{
				ProcessosDocumentosLst = ViewModelHelper.CriarSelectList(processosDocumentos, itemTextoPadrao: (processosDocumentos.Count > 0), selecionado: processoDocumentoSelecionado);
			}

			if ((atividades ?? new List<AtividadeSolicitada>()).Count <= 0)
			{
				AtividadeLst = ViewModelHelper.CriarSelectList(atividades, null, true, selecionado: atividadeSelecionada.ToString());
			}
			else
			{
				AtividadeLst = ViewModelHelper.CriarSelectList(atividades, itemTextoPadrao: (atividades.Count > 1), selecionado: atividadeSelecionada.ToString());
			}

			if (atividades != null && atividades.Count == 1 && atividades[0].Id > 0)
			{
				this.Atividades.Add(new Atividade() { Id = atividades[0].Id, NomeAtividade = atividades[0].Texto });
			}
		}
	}
}