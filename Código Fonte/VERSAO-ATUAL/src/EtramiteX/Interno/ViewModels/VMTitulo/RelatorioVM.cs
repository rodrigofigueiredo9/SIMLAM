using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class RelatorioVM
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private Titulo _titulo = new Titulo();
		public Titulo Titulo { get { return _titulo; } set { _titulo = value; } }

		public List<SelectListItem> LstModelos { get; set; }
		public List<SelectListItem> LstMunicipio { get; set; }

		public string LabelTipoPrazo { get; set; }
		public string ProtocoloSelecionado { get; set; }
		public bool TemEmpreendimento { get; set; }
		public bool SetoresEditar { get; set; }
		public bool IsEditar { get; set; }
		public bool IsVisualizar { get; set; }
		public bool CarregarEspecificidade { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@PeriodoObrigatorio = Mensagem.Titulo.PeriodoObrigatorio,
					@PeriodoFormato = Mensagem.Titulo.PeriodoFormato,
					@PeriodoInvalido = Mensagem.Titulo.PeriodoInvalido
				});
			}
		}

		private TituloCondicionanteVM _tituloCondicionanteVM = new TituloCondicionanteVM();
		public TituloCondicionanteVM TituloCondicionanteVM { get { return _tituloCondicionanteVM; } set { _tituloCondicionanteVM = value; } }

		private AssinantesVM _assinantesVM = new AssinantesVM();
		public AssinantesVM AssinantesVM
		{
			get { return _assinantesVM; }
			set { _assinantesVM = value; }
		}

		private DestinatarioEmailVM _destinatarioEmailVM = new DestinatarioEmailVM();
		public DestinatarioEmailVM DestinatarioEmailVM
		{
			get { return _destinatarioEmailVM; }
			set { _destinatarioEmailVM = value; }
		}

		private TituloModelo _modelo = new TituloModelo();
		public TituloModelo Modelo
		{
			get { return _modelo; }
			set { _modelo = value; }
		}

		public void PreencheCondicionantesVM()
		{
			TituloCondicionanteVM.Condicionantes = Titulo.Condicionantes;
			TituloCondicionanteVM.MostrarBotoes = (Titulo.Situacao.Id == 1); // Cadastrado
		}

		public RelatorioVM()
		{
		}

		public RelatorioVM(List<TituloModeloLst> modelos, List<Municipio> municipios)
		{
			LstModelos = ViewModelHelper.CriarSelectList(modelos, true);
			LstMunicipio = ViewModelHelper.CriarSelectList(municipios, true);
		}
	}
}