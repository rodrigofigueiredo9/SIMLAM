using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMRequerimento
{
	public class RequerimentoVM
	{
		public int Id { get; set; }
		public int ProjetoDigitalId { get; set; }
		public int Numero { get { return Id; } }
		public string DataCriacao { get; set; }
		public int SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }
		public bool IsAbaFinalizar { get; set; }
		public bool IsVisualizar { get; set; }
        public bool IsRequestAjax { get; set; }
		public int AgendamentoVistoriaId { get; set; }
		public int SetorId { get; set; }
		public string InformacaoComplementar { get; set; }

		//utilizado somente para requerimentos que possuam título declaratório de barragem
		#region Barragem

		public int? ResponsabilidadeRT { get; set; }
		public int? AbastecimentoPublico { get; set; }
		public int? UnidadeConservacao { get; set; }
		public int? SupressaoVegetacao { get; set; }
		public int? Realocacao { get; set; }
		public int? BarragensContiguas { get; set; }

		public bool contemBarragemDeclaratoria
		{
			get
			{
				return (this.AtividadesSolicitadas != null && this.AtividadesSolicitadas.Exists(x => x.Id == 327));
			}
		}

		#endregion Barragem

		private List<ResponsavelTecnico> _responsaveis = new List<ResponsavelTecnico>();
		public List<ResponsavelTecnico> Responsaveis
		{
			get { return _responsaveis; }
			set { _responsaveis = value; }
		}

		private List<SelectListItem> _responsavelFuncoes = new List<SelectListItem>();
		public List<SelectListItem> ResponsavelFuncoes
		{
			get { return _responsavelFuncoes; }
			set { _responsavelFuncoes = value; }
		}

		public List<SelectListItem> AgendamentoVistoria { get; set; }

		private List<Roteiro> _roteiros = new List<Roteiro>();
		public List<Roteiro> Roteiros
		{
			get { return _roteiros; }
			set { _roteiros = value; }
		}

		private List<Atividade> _atividadesSolicitadas = new List<Atividade>();
		public List<Atividade> AtividadesSolicitadas
		{
			get { return _atividadesSolicitadas; }
			set { _atividadesSolicitadas = value; }
		}

		private List<Finalidade> _finalidades = new List<Finalidade>();
		public List<Finalidade> Finalidades
		{
			get { return _finalidades; }
			set { _finalidades = value; }
		}

		private List<AtividadeSolicitadaVM> _atividadesSolicitadasVM = new List<AtividadeSolicitadaVM>();
		public List<AtividadeSolicitadaVM> AtividadesSolicitadasVM
		{
			get { return _atividadesSolicitadasVM; }
		}

		private Pessoa _interessado = new Pessoa();
		public Pessoa Interessado
		{
			get { return _interessado; }
			set { _interessado = value; }
		}

		private Empreendimento _empreendimento = new Empreendimento();
		public Empreendimento Empreendimento
		{
			get { return _empreendimento; }
			set { _empreendimento = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@RoteiroJaAdicionado = Mensagem.ChecagemRoteiro.RoteiroJaAdicionado,
					@RequerimentoSalvar = Mensagem.Requerimento.Salvar,
					@AtividadejaAdicionada = Mensagem.Requerimento.AtividadejaAdicionada,
					@ResponsavelSalvar = Mensagem.Requerimento.SalvarResponsavelTec,
					@RequerimentoEditar = Mensagem.Requerimento.Editar,
					@NaoExisteAssocicao = Mensagem.Requerimento.NaoExisteAssocicao,

					@FinalidadeModeloTituloExistente = Mensagem.Requerimento.FinalidadeModeloTituloExistente,
					@AssocieAtividade = Mensagem.Requerimento.NaoExisteAssocicao,
					@FinalidadeObrigatorio = Mensagem.Requerimento.FinalidadeObrigatorioCad,
					@TituloModeloObrigatorio = Mensagem.Requerimento.TituloObrigatorioCad,
					@NumeroAnteriorObrigatorio = Mensagem.Requerimento.NumeroAnteriorObrigatorioModal,
					@TituloAnteriorObrigatorio = Mensagem.Requerimento.TituloAnteriorObrigatorioModal,
					@InteressadoEditado = Mensagem.Requerimento.InteressadoEditar,
					@CpfObrigatorio = Mensagem.Pessoa.CpfObrigatorio,
					@CnpjObrigatorio = Mensagem.Pessoa.CnpjObrigatorio,
					@OrgaoExpedidorObrigatorio = Mensagem.Requerimento.OrgaoExpedidorObrigatorio,
					@BuscarObrigatorio = Mensagem.Requerimento.BuscarObrigatorio,

					@ResponsaveljaAdicionado = Mensagem.ResponsavelTecnico.ResponsaveljaAdicionado,
					@ResponsavelObrigatorio = Mensagem.ResponsavelTecnico.ResponsavelObrigatorio,
					@ResponsavelExcluir = Mensagem.ResponsavelTecnico.ResponsavelExcluir,
					@AtividadeObrigatorio = Mensagem.Requerimento.AtividadeObrigatorioMsg,
					@DeveExitirAtividade = Mensagem.Requerimento.AtividadeObrigatorio,
					@CarregarRoteiroObrigatorio = Mensagem.Requerimento.CarregarRoteiroObrigatorio,
					@TituloObrigatorio = Mensagem.Requerimento.TituloObrigatorio("#Texto"),
					@FinalidadeObrigatorioSalvar = Mensagem.Requerimento.FinalidadeObrigatorio("#Texto"),

					@AtividadesSetoresDiferentes = Mensagem.Requerimento.AtividadesSetoresDiferentes,
					@ConfirmarEdicao = Mensagem.Requerimento.ConfirmarEdicao

				});
			}
		}

		public RequerimentoVM() { this.DataCriacao = string.Empty; }

		public RequerimentoVM(Requerimento requerimento, bool isVisualizar = false)
		{
			this.IsVisualizar = isVisualizar;
			CarregarRequerimentoVM(requerimento);
		}

		public void CarregarListas(List<ResponsavelFuncoes> lstResponsavelFuncoes, List<AgendamentoVistoria> agendamentoVistoria)
		{
			ResponsavelFuncoes = ViewModelHelper.CriarSelectList(lstResponsavelFuncoes, true);
			AgendamentoVistoria = ViewModelHelper.CriarSelectList(agendamentoVistoria, true);
		}

		public void CarregarRequerimentoVM(Requerimento requerimento)
		{
			this.Id = requerimento.Numero;
			this.ProjetoDigitalId = requerimento.ProjetoDigitalId;
			this.DataCriacao = requerimento.DataCadastro.ToShortDateString();
			this.SituacaoId = requerimento.SituacaoId;
			this.SituacaoTexto = requerimento.SituacaoTexto;
			this.Responsaveis = requerimento.Responsaveis;
			this.Roteiros = requerimento.Roteiros;
			this.AtividadesSolicitadas = requerimento.Atividades;
			this.Interessado = requerimento.Interessado;
			this.Empreendimento = requerimento.Empreendimento;
			this.AgendamentoVistoriaId = requerimento.AgendamentoVistoria;
			this.SetorId = requerimento.SetorId;
			this.InformacaoComplementar = requerimento.Informacoes;

			CarregarAtividadeSolicitada();
		}

		private void CarregarAtividadeSolicitada()
		{
			for (int i = 0; i < this.AtividadesSolicitadas.Count; i++)
			{
				AtividadeSolicitadaVM atividade = new AtividadeSolicitadaVM();

				atividade.Id = this.AtividadesSolicitadas[i].Id;
				atividade.IdRelacionamento = this.AtividadesSolicitadas[i].IdRelacionamento.Value;
				atividade.NomeAtividade = this.AtividadesSolicitadas[i].NomeAtividade;
				atividade.Finalidades = this.AtividadesSolicitadas[i].Finalidades;
				atividade.SetorId = this.AtividadesSolicitadas[i].SetorId;
				atividade.IsRequerimentoVisualizar = IsVisualizar;

				AtividadesSolicitadasVM.Add(atividade);
			}
		}
	}
}