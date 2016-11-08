using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Publico.ViewModels.VMTitulo
{
	public class SalvarVM
	{
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private Titulo _titulo = new Titulo();
		public Titulo Titulo { get { return _titulo; } set { _titulo = value; } }

		public List<SelectListItem> LstModelos { get; set; }
		public List<SelectListItem> LstLocalEmissao { get; set; }
		public List<SelectListItem> LstSetores { get; set; }

		public string LabelTipoPrazo { get; set; }
		public string ProtocoloSelecionado { get; set; }
		public bool TemEmpreendimento { get; set; }		
		public bool SetoresEditar { get; set; }
		public bool IsEditar { get; set; }
		public bool IsVisualizar { get; set; }
		public bool CarregarEspecificidade { get; set; }

		public Int32? ArquivoId { get; set; }
		public String ArquivoTexto { get; set; }
		public String ArquivoJSon { get; set; }

		public bool IsNumeracaoAutomatica { get { return Modelo.Regra(eRegra.NumeracaoAutomatica); } }
		public bool IsProtocoloObrigatorio { get { return Modelo.Regra(eRegra.ProtocoloObrigatorio); } }
		public bool IsProtocoloProcesso { get { return (Modelo != null && (Modelo.TipoProtocolo == 1 || Modelo.TipoProtocolo == 3)); } }
		public bool IsProtocoloDocumento { get { return (Modelo != null && (Modelo.TipoProtocolo == 2 || Modelo.TipoProtocolo == 3)); } }
		public bool IsExibirCamposModelo { get { return IsExibirAnexoTituloPdf || IsEnviarEmail || IsExibirAssinate; } }
		public bool IsCondicionante { get { return Modelo.Regra(eRegra.Condicionantes); } }
		
		public bool IsExibirAnexoTituloPdf 
		{ 
			get 
			{
 				if (Modelo.Regras == null || Modelo.Regras.Count == 0)
				{
					return false;
				}
				return !Modelo.Regra(eRegra.PdfGeradoSistema); 
			} 
		}

		public bool IsEnviarEmail 
		{ 
			get 
			{ 
				return Modelo.Regra(eRegra.EnviarEmail) || (DestinatarioEmailVM.Destinatarios != null && DestinatarioEmailVM.Destinatarios.Count > 0); 
			} 
		}

		public bool IsExibirAssinate
		{
			get
			{
				return (Modelo != null && Modelo.Assinantes != null && Modelo.Assinantes.Count > 0) ||
					(AssinantesVM != null && AssinantesVM.Assinantes != null && AssinantesVM.Assinantes.Count > 0);
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					ProcDocSemEmpAssociado = Mensagem.Titulo.ProcDocSemEmpAssociado,
					ArquivoObrigatorio = Mensagem.Titulo.ArquivoObrigatorio,
					ArquivoTipoPdf = Mensagem.Titulo.ArquivoTipoPdf
				});
			}
		}

		private TituloCondicionanteVM _tituloCondicionanteVM = new TituloCondicionanteVM();
		public TituloCondicionanteVM TituloCondicionanteVM { get { return _tituloCondicionanteVM; } set { _tituloCondicionanteVM = value; } }

		private AssinantesVM _assinantesVM = new AssinantesVM();
		public AssinantesVM AssinantesVM { 
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
			set {  _modelo = value; }
		}

		public void PreencheCondicionantesVM()
		{
			TituloCondicionanteVM.Condicionantes = Titulo.Condicionantes;
			TituloCondicionanteVM.MostrarBotoes = (Titulo.Situacao.Id == 1); // Cadastrado
		}

		public SalvarVM()
		{
		}

		public void SetaSetores(List<Setor> setores, int setorSelecionado = 0)
		{
			LstSetores = ViewModelHelper.CriarSelectList(setores, true, (setores.Count > 1), setorSelecionado.ToString());
		}

		public SalvarVM(List<Setor> setores, List<TituloModeloLst> modelos, List<Municipio> locais, int setorSelecionado = 0, int modeloSelecionado = 0, int localSelecionado = 0)
		{
			if (setores != null)
			{
				LstSetores = ViewModelHelper.CriarSelectList(setores, true, (setores.Count > 1), setorSelecionado.ToString());
			}

			LstModelos = ViewModelHelper.CriarSelectList(modelos, true, selecionado: modeloSelecionado.ToString());

			if (localSelecionado > 0)
			{
				LstLocalEmissao = ViewModelHelper.CriarSelectList(locais, true, selecionado: localSelecionado.ToString());
			}
			else
			{
				LstLocalEmissao = ViewModelHelper.CriarSelectList(locais, true, selecionadoTexto: _configSys.Obter<String>(ConfiguracaoSistema.KeyMunicipioDefault)); 
			}
		}
	}
}