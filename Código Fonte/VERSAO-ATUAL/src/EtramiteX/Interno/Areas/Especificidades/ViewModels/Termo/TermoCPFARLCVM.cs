using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo
{
	public class TermoCPFARLCVM
	{
		public bool IsCondicionantes { get; set; }
		public bool IsVisualizar { get; set; }
		public TermoCPFARLC Especificidade { get; set; }
		public AtividadeEspecificidadeVM Atividades { get; set; }
		public TituloCondicionanteVM Condicionantes { get; set; }
		public List<SelectListItem> CedenteDominios { get; set; }
		public List<SelectListItem> CedenteReservas { get; set; }
		public List<SelectListItem> CedenteResponsaveisEmpreendimento { get; set; }
		public List<SelectListItem> ReceptorEmpreendimentos { get; set; }
		public List<SelectListItem> ReceptorDominios { get; set; }
		public List<SelectListItem> ReceptorResponsaveisEmpreendimento { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					CedenteEmpreendimentoObrigatorio = Mensagem.TermoCPFARLC.CedenteEmpreendimentoObrigatorio,
					CedenteARLCompensacaoObrigatoria = Mensagem.TermoCPFARLC.CedenteARLCompensacaoObrigatoria,
					CedenteARLCompensacaoDuplicada = Mensagem.TermoCPFARLC.CedenteARLCompensacaoDuplicada,
					ResponsavelEmpreendimentoObrigatorio = Mensagem.TermoCPFARLC.ResponsavelEmpreendimentoObrigatorio(string.Empty, "#TEXTO#"),
					ResponsavelEmpreendimentoDuplicado = Mensagem.TermoCPFARLC.ResponsavelEmpreendimentoDuplicado(string.Empty, "#TEXTO#")
				});
			}
		}

		public TermoCPFARLCVM() : this(new List<Protocolos>(), new List<AtividadeSolicitada>(), new List<TituloCondicionante>(), null, null, false, 0) { }

		public TermoCPFARLCVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<TituloCondicionante> condicionantes, TermoCPFARLC especificidade,
			string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Especificidade = especificidade ?? new TermoCPFARLC();

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

			#region Remover depois

			Condicionantes = Condicionantes ?? new TituloCondicionanteVM();

			CedenteDominios = ViewModelHelper.CriarSelectList(new List<Lista>(), true, true);
			CedenteReservas = ViewModelHelper.CriarSelectList(new List<Lista>(), true, true);
			CedenteResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(new List<Lista>(), true, true);
			ReceptorEmpreendimentos = ViewModelHelper.CriarSelectList(new List<Lista>(), true, true);
			ReceptorDominios = ViewModelHelper.CriarSelectList(new List<Lista>(), true, true);
			ReceptorResponsaveisEmpreendimento = ViewModelHelper.CriarSelectList(new List<Lista>(), true, true);

			#endregion

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();
		}
	}
}