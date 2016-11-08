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
	public class TermoCPFARLCRVM
	{
		public bool IsCondicionantes { get; set; }
		public bool IsVisualizar { get; set; }
		public TermoCPFARLCR Especificidade { get; set; }
		public AtividadeEspecificidadeVM Atividades { get; set; }

		private TituloCondicionanteVM _condicionantes = new TituloCondicionanteVM();
		public TituloCondicionanteVM Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}
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
					CedenteARLCompensacaoObrigatoria = Mensagem.TermoCPFARLCR.CedenteARLCompensacaoObrigatoria,
					CedenteARLCompensacaoDuplicada = Mensagem.TermoCPFARLCR.CedenteARLCompensacaoDuplicada,
					ResponsavelEmpreendimentoObrigatorio = Mensagem.TermoCPFARLCR.ResponsavelEmpreendimentoObrigatorio(string.Empty, "#TEXTO#"),
					ResponsavelEmpreendimentoDuplicado = Mensagem.TermoCPFARLCR.ResponsavelEmpreendimentoDuplicado(string.Empty, "#TEXTO#")
				});
			}
		}

		public TermoCPFARLCRVM() : this(new List<Protocolos>(), new List<AtividadeSolicitada>(), new List<TituloCondicionante>(), null, null, false, 0) { }

		public TermoCPFARLCRVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<TituloCondicionante> condicionantes, TermoCPFARLCR especificidade, 
			string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Especificidade = especificidade ?? new TermoCPFARLCR();

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

			#region Remover depois

			//Condicionantes = Condicionantes ?? new TituloCondicionanteVM();

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