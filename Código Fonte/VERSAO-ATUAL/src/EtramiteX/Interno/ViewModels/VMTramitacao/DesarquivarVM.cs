using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTramitacao
{
	public class DesarquivarVM
	{
		private Funcionario _executor = new Funcionario();
		public Funcionario Executor
		{
			get { return _executor; }
			set { _executor = value; }
		}

		private Situacao _situacao = new Situacao();
		public Situacao Situacao
		{
			get { return _situacao; }
			set { _situacao = value; }
		}

		private DateTecno _recebimentoData = new DateTecno();
		public DateTecno RecebimentoData
		{
			get { return _recebimentoData; }
			set { _recebimentoData = value; }
		}

		private Setor _remetenteSetor = new Setor();
		public Setor RemetenteSetor
		{
			get { return _remetenteSetor; }
			set { _remetenteSetor = value; }
		}

		private Setor _destinatarioSetor = new Setor();
		public Setor DestinatarioSetor
		{
			get { return _destinatarioSetor; }
			set { _destinatarioSetor = value; }
		}

		private TramitacaoArquivoLista _arquivo = new TramitacaoArquivoLista();
		public TramitacaoArquivoLista Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

		private List<Tramitacao> _tramitacoes = new List<Tramitacao>();
		public List<Tramitacao> Tramitacoes
		{
			get { return _tramitacoes; }
			set { _tramitacoes = value; }
		}

		private List<SelectListItem> _setoresOrigem = new List<SelectListItem>();
		public List<SelectListItem> SetoresOrigem
		{
			get { return _setoresOrigem; }
			set { _setoresOrigem = value; }
		}

		private List<SelectListItem> _setoresDestino = new List<SelectListItem>();
		public List<SelectListItem> SetoresDestino
		{
			get { return _setoresDestino; }
			set { _setoresDestino = value; }
		}

		private List<SelectListItem> _arquivosCadastrados = new List<SelectListItem>();
		public List<SelectListItem> ArquivosCadastrados
		{
			get { return _arquivosCadastrados; }
			set { _arquivosCadastrados = value; }
		}

		private List<SelectListItem> _atividades = new List<SelectListItem>();
		public List<SelectListItem> Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<SelectListItem> _situacoesAtividade = new List<SelectListItem>();
		public List<SelectListItem> SituacoesAtividade
		{
			get { return _situacoesAtividade; }
			set { _situacoesAtividade = value; }
		}

		private List<SelectListItem> _tipoTitulo = new List<SelectListItem>();
		public List<SelectListItem> TiposTitulo
		{
			get { return _tipoTitulo; }
			set { _tipoTitulo = value; }
		}

		private List<SelectListItem> _estado = new List<SelectListItem>();
		public List<SelectListItem> Estados
		{
			get { return _estado; }
			set { _estado = value; }
		}

		private List<SelectListItem> _municipioEmpreendimento = new List<SelectListItem>();
		public List<SelectListItem> MunicipiosEmpreendimento
		{
			get { return _municipioEmpreendimento; }
			set { _municipioEmpreendimento = value; }
		}

		private List<SelectListItem> _prateleirasIdentificacoes = new List<SelectListItem>();
		public List<SelectListItem> PrateleirasIdentificacoes
		{
			get { return _prateleirasIdentificacoes; }
			set { _prateleirasIdentificacoes = value; }
		}

		private List<SelectListItem> _prateleiraModos = new List<SelectListItem>();
		public List<SelectListItem> PrateleiraModos
		{
			get { return _prateleiraModos; }
			set { _prateleiraModos = value; }
		}

		private List<SelectListItem> _estantes = new List<SelectListItem>();
		public List<SelectListItem> Estantes
		{
			get { return _estantes; }
			set { _estantes = value; }
		}

		public String MsgNaoEncontrouRegistros = Mensagem.Arquivamento.DesarquivarArquivoSemProtocolo.Texto;

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@SetorOrigemObrigatorio = Mensagem.Arquivamento.SetorOrigemObrigatorio,
					@SetorDestinoObrigratorio = Mensagem.Arquivamento.SetorDestinoObrigatorio,
					@SetorSemArquivo = Mensagem.Arquivamento.SetorOrigemSemArquivo,
					@ArquivoSemProtocolo = Mensagem.Arquivamento.DesarquivarArquivoSemProtocolo,
					@ProtocoloJaAdicionado = Mensagem.Arquivamento.DesarquivarProtocoloJaAdicionado
				});
			}
		}

		public DesarquivarVM() { }

		public DesarquivarVM(Funcionario executor,
							List<Setor> setoresOrigem,
							List<Setor> setoresDestino,
							List<TramitacaoArquivoLista> arquivosCadastrados,
							List<ProcessoAtividadeItem> atividades,
							List<Situacao> situacoesAtividade,
							List<Estado> estados,
							String estadoDefault,
							List<Municipio> municipios,
							List<Lista> prateleiraIdentificadoes,
							List<TramitacaoArquivoModo> prateleiraModos,
							List<Lista> estantes)
		{
			SetoresOrigem = ViewModelHelper.CriarSelectList(setoresOrigem, true);
			SetoresDestino = ViewModelHelper.CriarSelectList(setoresDestino, true);
			ArquivosCadastrados = ViewModelHelper.CriarSelectList(arquivosCadastrados, true);
			Atividades = ViewModelHelper.CriarSelectList(atividades, true);
			SituacoesAtividade = ViewModelHelper.CriarSelectList(situacoesAtividade, true);

			TiposTitulo = ViewModelHelper.CriarSelectList(estados, true);
			Estados = ViewModelHelper.CriarSelectList(estados, true);
			MunicipiosEmpreendimento = ViewModelHelper.CriarSelectList(municipios, true);

			PrateleirasIdentificacoes = ViewModelHelper.CriarSelectList(prateleiraIdentificadoes, true);
			PrateleiraModos = ViewModelHelper.CriarSelectList(prateleiraModos, true);
			Estantes = ViewModelHelper.CriarSelectList(estantes, true);

			RecebimentoData.Data = DateTime.Now;
			Executor.Id = executor.Id;
			Executor.Nome = executor.Nome;
		}
	}
}