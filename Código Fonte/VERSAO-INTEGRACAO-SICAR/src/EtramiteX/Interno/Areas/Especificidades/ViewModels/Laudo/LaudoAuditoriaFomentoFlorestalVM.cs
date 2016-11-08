using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo
{
	public class LaudoAuditoriaFomentoFlorestalVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsCondicionantes { get; set; }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EspecificidadeResultadoConforme = eEspecificidadeResultado.Conforme,
					@EspecificidadeResultadoNaoConforme = eEspecificidadeResultado.NaoConforme
				});
			}
		}

		private LaudoAuditoriaFomentoFlorestal _laudo = new LaudoAuditoriaFomentoFlorestal();
		public LaudoAuditoriaFomentoFlorestal Laudo
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

		private List<SelectListItem> _resultados = new List<SelectListItem>();
		public List<SelectListItem> Resultados
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private List<SelectListItem> _conclusoes = new List<SelectListItem>();
		public List<SelectListItem> Conclusoes
		{
			get { return _resultados; }
			set { _resultados = value; }
		}

		private List<SelectListItem> _responsaveisTecnico = new List<SelectListItem>();
		public List<SelectListItem> ResponsaveisTecnico
		{
			get { return _responsaveisTecnico; }
			set { _responsaveisTecnico = value; }
		}

		private AtividadeEspecificidadeVM _atividades = new AtividadeEspecificidadeVM();
		public AtividadeEspecificidadeVM Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private TituloCondicionanteVM _condicionantes = new TituloCondicionanteVM();
		public TituloCondicionanteVM Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}

		private ArquivoVM _arquivoVM = new ArquivoVM();
		public ArquivoVM ArquivoVM
		{
			get { return _arquivoVM; }
			set { _arquivoVM = value; }
		}

		public LaudoAuditoriaFomentoFlorestalVM(LaudoAuditoriaFomentoFlorestal laudo,List<Protocolos> processosDocumentos,List<AtividadeSolicitada> atividades,
			List<PessoaLst> destinatarios, List<PessoaLst> responsaveisTecnicos, List<Lista> resultados,
			List<TituloCondicionante> condicionantes = null, string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			Laudo = laudo;
			IsVisualizar = isVisualizar;
			ArquivoVM.IsVisualizar = isVisualizar;
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, Laudo.Destinatario.ToString());
			Resultados = ViewModelHelper.CriarSelectList(resultados, true, true, Laudo.ResultadoTipo.ToString()); ;
			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();
		}

		public LaudoAuditoriaFomentoFlorestalVM() 
		{

		}

	}
}