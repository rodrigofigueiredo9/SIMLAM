using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloAutorizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Autorizacao
{
	public class AutorizacaoExploracaoFlorestalVM
	{
		public bool IsVisualizar { set; get; }
		public bool IsCondicionantes { get; set; }

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

		private TituloAssociadoEsp _tituloAssociado = new TituloAssociadoEsp();
		public TituloAssociadoEsp TituloAssociado
		{
			get { return _tituloAssociado; }
			set { _tituloAssociado = value; }
		}

		private TituloCondicionanteVM _condicionantes = new TituloCondicionanteVM();
		public TituloCondicionanteVM Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}

		private AutorizacaoExploracaoFlorestal _autorizacao = new AutorizacaoExploracaoFlorestal();
		public AutorizacaoExploracaoFlorestal Autorizacao
		{
			get { return _autorizacao; }
			set { _autorizacao = value; }
		}

		private List<SelectListItem> _exploracoes = new List<SelectListItem>();
		public List<SelectListItem> Exploracoes
		{
			get { return _exploracoes; }
			set { _exploracoes = value; }
		}

		private List<TituloExploracaoFlorestalExploracao> _tituloExploracaoDetalhes = new List<TituloExploracaoFlorestalExploracao>();
		public List<TituloExploracaoFlorestalExploracao> TituloExploracaoDetalhes { get { return _tituloExploracaoDetalhes; } set { _tituloExploracaoDetalhes = value; } }

		public String LaudoVistoriaTextoTela
		{
			get
			{
				return TituloAssociado.ModeloSigla == null ? "" : TituloAssociado.ModeloSigla + " - " + TituloAssociado.TituloNumero;
			}
		}

		public AutorizacaoExploracaoFlorestalVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, List<TituloCondicionante> condicionantes = null,
			string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true);
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
			Atividades.Especificidade = eEspecificidade.AutorizacaoExploracaoFlorestal;
			Atividades.Destinatarios = Destinatarios;

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();

			ArquivoVM.IsVisualizar = IsVisualizar;
		}

		public AutorizacaoExploracaoFlorestalVM() { }
	}
}