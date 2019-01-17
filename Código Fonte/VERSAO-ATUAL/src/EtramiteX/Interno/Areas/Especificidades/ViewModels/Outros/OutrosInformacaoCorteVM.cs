using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Outros
{
	public class OutrosInformacaoCorteVM
	{
		public bool IsVisualizar { set; get; }
		public bool IsCondicionantes { set; get; }
		public bool IsDeclaratorio { set; get; }

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

		private List<SelectListItem> _atividadeList = new List<SelectListItem>();
		public List<SelectListItem> AtividadeList
		{
			get { return _atividadeList; }
			set { _atividadeList = value; }
		}

		private List<SelectListItem> _informacaoCortes = new List<SelectListItem>();
		public List<SelectListItem> InformacaoCortes
		{
			get { return _informacaoCortes; }
			set { _informacaoCortes = value; }
		}

		private OutrosInformacaoCorte _outros = new OutrosInformacaoCorte();
		public OutrosInformacaoCorte Outros
		{
			get { return _outros; }
			set { _outros = value; }
		}        
		
		private TituloCondicionanteVM _condicionantes = new TituloCondicionanteVM();
		public TituloCondicionanteVM Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}
		
		public OutrosInformacaoCorteVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, List<ListaValor> informacaoCortes, OutrosInformacaoCorte outros, List<TituloCondicionante> condicionantes, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			this.IsVisualizar = isVisualizar;
			this.Outros = outros;

			this.InformacaoCortes = ViewModelHelper.CriarSelectList(informacaoCortes, true, true, outros.InformacaoCorte.ToString());

			this.Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			this.Atividades.MostrarBotoes = false;

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();

			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true);
		}

		public OutrosInformacaoCorteVM(OutrosInformacaoCorte outros, List<AtividadeSolicitada> atividades, List<Lista> infCorte, bool isVisualizar = false)
		{
			IsVisualizar = isVisualizar;
			Outros = outros;
			AtividadeList = ViewModelHelper.CriarSelectList(atividades, true, true, outros.Atividade.ToString());
			InformacaoCortes = ViewModelHelper.CriarSelectList(infCorte, true, true, outros.InformacaoCorte.ToString());
		}

		public OutrosInformacaoCorteVM() { }
	}
}