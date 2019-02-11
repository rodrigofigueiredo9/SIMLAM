using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento
{
	public class SalvarVM
	{
		public Boolean IsEditar { get; set; }
		public Boolean MostrarSetor { get; set; }
		public String ConfiguracoesProtocoloTiposJson { get; set; }

		private List<ProtocoloTipo> _configuracoesProtocoloTipos = new List<ProtocoloTipo>();
		public List<ProtocoloTipo> ConfiguracoesProtocoloTipos
		{
			get { return _configuracoesProtocoloTipos; }
			set { _configuracoesProtocoloTipos = value; }
		}

		private ProtocoloTipo _tipo = new ProtocoloTipo();
		public ProtocoloTipo Tipo
		{
			get
			{
				if (_tipo == null || _tipo.Id == 0)
					_tipo = ConfiguracoesProtocoloTipos.FirstOrDefault(x => x.Id == Documento.Tipo.Id) ?? new ProtocoloTipo() { LabelInteressado = "Interessado" };
				return _tipo;
			}
			set { _tipo = value; }
		}

		private Documento _documento = new Documento();
		public Documento Documento
		{
			get { return _documento; }
			set { _documento = value; }
		}

		private RequerimentoVM _requerimentoVM = new RequerimentoVM();
		public RequerimentoVM RequerimentoVM
		{
			get { return _requerimentoVM; }
		}

		private List<SelectListItem> _documentoTipos = new List<SelectListItem>();
		public List<SelectListItem> DocumentoTipos
		{
			get { return _documentoTipos; }
			set { _documentoTipos = value; }
		}

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		public List<SelectListItem> SetoresDestinatario
		{
			get { return _setoresDestinatario; }
			set { _setoresDestinatario = value; }
		}
		private List<SelectListItem> _setoresDestinatario = new List<SelectListItem>();

		public List<SelectListItem> DestinatarioFuncionarios
		{
			get { return _destinatarioFuncionarios; }
			set { _destinatarioFuncionarios = value; }
		}
		private List<SelectListItem> _destinatarioFuncionarios = new List<SelectListItem>();

		private AssinantesVM _assinantesVM = new AssinantesVM();
		public AssinantesVM AssinantesVM
		{
			get { return _assinantesVM; }
			set { _assinantesVM = value; }
		}

		public bool ExibirGrupoProtocolo
		{
			get
			{
				return (Tipo.PossuiProcesso || Tipo.ProcessoObrigatorio ||
						Tipo.PossuiDocumento || Tipo.DocumentoObrigatorio);
			}
		}
		public bool ExibirRadioProcDoc
		{
			get
			{
				return ((Tipo.PossuiProcesso || Tipo.ProcessoObrigatorio) && (Tipo.PossuiDocumento || Tipo.DocumentoObrigatorio));
			}
		}

		public bool RadioProcessoChecked
		{
			get
			{
				if (Documento.Id.GetValueOrDefault(0) == 0 || Documento.ProtocoloAssociado.Id.GetValueOrDefault(0) == 0)
				{
					return (Tipo.PossuiProcesso || Tipo.ProcessoObrigatorio);
				}

				return Documento.ProtocoloAssociado.IsProcesso;
			}
		}
		public bool RadioDocumentoChecked
		{
			get
			{
				if (Documento.Id.GetValueOrDefault(0) == 0 || Documento.ProtocoloAssociado.Id.GetValueOrDefault(0) == 0)
				{
					return ((!(Tipo.PossuiProcesso || Tipo.ProcessoObrigatorio)) && (Tipo.PossuiDocumento || Tipo.DocumentoObrigatorio));
				}

				return !Documento.ProtocoloAssociado.IsProcesso;
			}
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					RequerimentoSituacaoInvalida = Mensagem.Documento.RequerimentoSituacaoInvalida,
					AtividadejaAdicionada = Mensagem.Documento.AtividadejaAdicionada,
					ResponsaveljaAdicionado = Mensagem.Documento.ResponsaveljaAdicionado,
					ChecagemPendenciaSituacaoInvalida = Mensagem.Documento.ChecagemPendenciaSituacaoInvalida,
					ResponsavelTecnicoSemPreencher = Mensagem.Documento.ResponsavelTecnicoSemPreencher,
					ResponsavelTecnicoRemover = Mensagem.Documento.ResponsavelTecnicoRemover,
					ArquivoObrigatorio = Mensagem.Documento.ArquivoObrigatorio,

					FinalidadeModeloTituloExistente = Mensagem.Requerimento.FinalidadeModeloTituloExistente,
					AssocieAtividade = Mensagem.Requerimento.NaoExisteAssocicao,
					FinalidadeObrigatorio = Mensagem.Requerimento.FinalidadeObrigatorioCad,
					TituloModeloObrigatorio = Mensagem.Requerimento.TituloObrigatorioCad,
					NumeroAnteriorObrigatorio = Mensagem.Requerimento.NumeroAnteriorObrigatorioModal,
					TituloAnteriorObrigatorio = Mensagem.Requerimento.TituloAnteriorObrigatorioModal,
					OrgaoExpedidorObrigatorio = Mensagem.Requerimento.OrgaoExpedidorObrigatorio,
					BuscarObrigatorio = Mensagem.Requerimento.BuscarObrigatorio
				});
			}
		}

		public SalvarVM() { }

		public SalvarVM(List<ProtocoloTipo> lstDocumentoTipo, List<Setor> setoresDestinario, int? documentoSelecionado = null)
		{
			DocumentoTipos = ViewModelHelper.CriarSelectList(lstDocumentoTipo, true, selecionado: documentoSelecionado.ToString());
			this.Documento.DataCadastro.Data = DateTime.Now;

			this.ConfiguracoesProtocoloTipos = lstDocumentoTipo;
			this.ConfiguracoesProtocoloTiposJson = ViewModelHelper.Json(lstDocumentoTipo);
			CarregarSetoresDestinario(setoresDestinario);
		}

		public void CarregarSetores(List<Setor> lstSetores)
		{
			lstSetores = lstSetores ?? new List<Setor>();
			MostrarSetor = lstSetores.Count > 1;
			if (MostrarSetor)
			{
				Setores = ViewModelHelper.CriarSelectList(lstSetores, true);
			}
			else
			{
				int id = (lstSetores.FirstOrDefault() ?? new Setor()).Id;
				Setores = ViewModelHelper.CriarSelectList(lstSetores, true, selecionado: id.ToString());
			}
		}

		public void CarregarSetoresDestinario(List<Setor> setoresDestinario)
		{
			SetoresDestinatario = ViewModelHelper.CriarSelectList(setoresDestinario, true);
		}

		public void SetDocumento(Documento documento, List<ResponsavelFuncoes> responsavelFuncoes)
		{
			this.Documento = documento;

			documento.Requerimento.Atividades = documento.Atividades;
			documento.Requerimento.Interessado = documento.Interessado;
			documento.Requerimento.Responsaveis = documento.Responsaveis;
			documento.Requerimento.Empreendimento = documento.Empreendimento;

			this.RequerimentoVM.CarregarListas(responsavelFuncoes);
			this.RequerimentoVM.CarregarRequerimentoVM(documento.Requerimento);
		}

		public String ObterJSon(Object objeto)
		{
			return ViewModelHelper.JsSerializer.Serialize(objeto);
		}

		public string Asterisco(bool exibir)
		{
			return exibir ? " *" : string.Empty;
		}
	}
}