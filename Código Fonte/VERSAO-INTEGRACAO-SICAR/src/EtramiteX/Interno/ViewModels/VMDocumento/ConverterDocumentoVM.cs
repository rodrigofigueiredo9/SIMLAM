using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using namespaceVMProcesso = Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento
{
	public class ConverterDocumentoVM
	{
		public Boolean IsEditar { get; set; }
		public Boolean PodeAutuar { get; set; }
		public String ConfiguracoesProtocoloTiposJson { get; set; }

		private List<ProtocoloTipo> _configuracoesProtocoloTipos = new List<ProtocoloTipo>();
		public List<ProtocoloTipo> ConfiguracoesProtocoloTipos
		{
			get { return _configuracoesProtocoloTipos; }
			set { _configuracoesProtocoloTipos = value; }
		}

		private Documento _processo = new Documento();
		public Documento Processo
		{
			get { return _processo; }
			set { _processo = value; }
		}

		private namespaceVMProcesso.RequerimentoVM _requerimentoVM = new namespaceVMProcesso.RequerimentoVM();
		public namespaceVMProcesso.RequerimentoVM RequerimentoVM
		{
			get { return _requerimentoVM; }
		}

		private List<SelectListItem> _processoTipos = new List<SelectListItem>();
		public List<SelectListItem> ProcessoTipos
		{
			get { return _processoTipos; }
			set { _processoTipos = value; }
		}

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					RequerimentoSituacaoInvalida = Mensagem.Processo.RequerimentoSituacaoInvalida,
					AtividadejaAdicionada = Mensagem.Processo.AtividadejaAdicionada,
					ResponsaveljaAdicionado = Mensagem.Processo.ResponsaveljaAdicionado,
					ResponsavelTecnicoObrigatorio = Mensagem.Processo.ResponsavelTecnicoObrigatorio,
					ArquivoObrigatorio = Mensagem.Processo.ArquivoObrigatorio,
					ResponsavelTecnicoSemPreencher = Mensagem.Processo.ResponsavelTecnicoSemPreencher,
					ResponsavelTecnicoRemover = Mensagem.Processo.ResponsavelTecnicoRemover,
					NumeroAutuacaoObrigatorio = Mensagem.Processo.NumeroAutuacaoObrigatorio,
					PossuiNumeroSEPObrigatorio = Mensagem.Processo.PossuiNumeroSEPObrigatorio,
					DataAutuacaoObrigatoria = Mensagem.Processo.DataAutuacaoObrigatoria,

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

		public ConverterDocumentoVM() { }

		public ConverterDocumentoVM(List<ProtocoloTipo> lstProcessoTipo, int? processoSelecionado = null)
		{
			ProcessoTipos = ViewModelHelper.CriarSelectList(lstProcessoTipo, true, selecionado: processoSelecionado.ToString());
			this.Processo.DataCadastro.Data = DateTime.Now;
			this.Processo.Tipo.Id = processoSelecionado.Value;
			this.ConfiguracoesProtocoloTipos = lstProcessoTipo;
			this.ConfiguracoesProtocoloTiposJson = ViewModelHelper.Json(lstProcessoTipo);
		}

		public void CarregarSetores(List<Setor> lstSetores)
		{
			lstSetores = lstSetores ?? new List<Setor>();
			if (lstSetores.Count > 1)
			{
				Setores = ViewModelHelper.CriarSelectList(lstSetores, true);
			}
			else
			{
				int id = (lstSetores.FirstOrDefault() ?? new Setor()).Id;
				Setores = ViewModelHelper.CriarSelectList(lstSetores, true, selecionado: id.ToString());
			}
		}

		public void SetProcesso(Documento processo, List<ResponsavelFuncoes> responsavelFuncoes)
		{
			this.Processo = processo;

			processo.Requerimento.Atividades = processo.Atividades;
			processo.Requerimento.Interessado = processo.Interessado;
			processo.Requerimento.Responsaveis = processo.Responsaveis;
			processo.Requerimento.Empreendimento = processo.Empreendimento;

			this.RequerimentoVM.CarregarListas(responsavelFuncoes);
			this.RequerimentoVM.CarregarRequerimentoVM(processo.Requerimento);
		}

		public String ObterJSon(Object objeto)
		{
			return ViewModelHelper.JsSerializer.Serialize(objeto);
		}
	}
}