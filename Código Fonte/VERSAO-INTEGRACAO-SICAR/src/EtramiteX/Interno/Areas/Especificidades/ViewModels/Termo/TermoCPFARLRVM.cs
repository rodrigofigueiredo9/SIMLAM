﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.Areas.Especificidades.ViewModels.Termo
{
	public class TermoCPFARLRVM
	{
		public bool IsVisualizar { set; get; }
		public bool IsCondicionantes { get; set; }

		private TermoCPFARLRTituloAnterior _tituloAnterior = new TermoCPFARLRTituloAnterior();
		public TermoCPFARLRTituloAnterior TituloAnterior
		{
			get { return _tituloAnterior; }
			set { _tituloAnterior = value; }
		}

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

		private List<SelectListItem> _dominios = new List<SelectListItem>();
		public List<SelectListItem> Dominios
		{
			get { return _dominios; }
			set { _dominios = value; }
		}

		private ArquivoVM _arquivoVM = new ArquivoVM();
		public ArquivoVM ArquivoVM
		{
			get { return _arquivoVM; }
			set { _arquivoVM = value; }
		}

		private TermoCPFARLR _termo = new TermoCPFARLR();
		public TermoCPFARLR Termo
		{
			get { return _termo; }
			set
			{
				_termo = value;
				ArquivoVM.Anexos = _termo == null ? new List<Anexo>() : _termo.Anexos;
			}
		}

		private TituloCondicionanteVM _condicionantes = new TituloCondicionanteVM();
		public TituloCondicionanteVM Condicionantes
		{
			get { return _condicionantes; }
			set { _condicionantes = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@DestinatarioObrigatorio = Mensagem.TermoCPFARLR.DestinatarioObrigatorio,
					@DestinatarioJaAdicionado = Mensagem.TermoCPFARLR.DestinatarioJaAdicionado
				});
			}
		}

		public TermoCPFARLRVM(List<Protocolos> processosDocumentos, List<AtividadeSolicitada> atividades, List<PessoaLst> destinatarios, List<TituloCondicionante> condicionantes, TermoCPFARLR termo, string processoDocumentoSelecionado, bool isVisualizar, int atividadeSelecionada)
		{
			IsVisualizar = isVisualizar;
			Termo = termo;

			Destinatarios = ViewModelHelper.CriarSelectList(destinatarios, true, true);

			Atividades = new AtividadeEspecificidadeVM(processosDocumentos, atividades, processoDocumentoSelecionado, atividadeSelecionada, isVisualizar);
			Atividades.MostrarBotoes = false;

			Condicionantes.MostrarBotoes = !isVisualizar;
			Condicionantes.Condicionantes = condicionantes ?? new List<TituloCondicionante>();

			ArquivoVM.IsVisualizar = isVisualizar;
		}

		public TermoCPFARLRVM() { }
	}
}