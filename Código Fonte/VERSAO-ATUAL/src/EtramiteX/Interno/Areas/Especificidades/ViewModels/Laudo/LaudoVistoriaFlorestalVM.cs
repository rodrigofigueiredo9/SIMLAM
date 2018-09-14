using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Laudo
{
	public class LaudoVistoriaFlorestalVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsCondicionantes { get; set; }

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@EspecificidadeConclusaoFavoravelId = eEspecificidadeConclusao.Favoravel
				});
			}
		}

		private LaudoVistoriaFlorestal _laudo = new LaudoVistoriaFlorestal();
		public LaudoVistoriaFlorestal Laudo
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

		private List<SelectListItem> _conclusoes = new List<SelectListItem>();
		public List<SelectListItem> Conclusoes
		{
			get { return _conclusoes; }
			set { _conclusoes = value; }
		}

		private List<SelectListItem> _responsaveisTecnico = new List<SelectListItem>();
		public List<SelectListItem> ResponsaveisTecnico
		{
			get { return _responsaveisTecnico; }
			set { _responsaveisTecnico = value; }
		}

		private List<SelectListItem> _caracterizacoes = new List<SelectListItem>();
		public List<SelectListItem> Caracterizacoes
		{
			get { return _caracterizacoes; }
			set { _caracterizacoes = value; }
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

		public List<ExploracaoFlorestal> ExploracaoFlorestal { get; set; }

		public LaudoVistoriaFlorestalVM(LaudoVistoriaFlorestal laudo,List<Protocolos> processosDocumentos,List<AtividadeSolicitada> atividades,
			List<CaracterizacaoLst> caracterizacoes, List<PessoaLst> destinatarios, List<PessoaLst> responsaveisTecnicos, List<Lista> parecerTecnico, 
			List<TituloCondicionante> condicionantes = null, string processoDocumentoSelecionado = null, bool isVisualizar = false)
		{
			Laudo = laudo;
			caracterizacoes = caracterizacoes.Where(x => x.Id == (int)eCaracterizacao.ExploracaoFlorestal || x.Id == (int)eCaracterizacao.QueimaControlada || x.Id == (int)eCaracterizacao.Silvicultura).ToList();

			IsVisualizar = isVisualizar;
			ArquivoVM.IsVisualizar = isVisualizar;
			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, 0, isVisualizar);
			Caracterizacoes = ViewModelHelper.CriarSelectList(caracterizacoes, true, true);
			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true, Laudo.Destinatario.ToString());
			ResponsaveisTecnico = ViewModelHelper.CriarSelectList(responsaveisTecnicos, true, true, Laudo.Responsavel.ToString());
			Conclusoes = ViewModelHelper.CriarSelectList(parecerTecnico, true, true, Laudo.Conclusao.ToString());

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();
		}

		public LaudoVistoriaFlorestalVM() 
		{

		}

	}
}