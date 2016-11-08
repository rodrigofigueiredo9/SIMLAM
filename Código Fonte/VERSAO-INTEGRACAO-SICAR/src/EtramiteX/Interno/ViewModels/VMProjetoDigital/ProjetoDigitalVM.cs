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

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProjetoDigital
{
	public class ProjetoDigitalVM
	{
		public int Id { get; set; }
		public int Numero { get { return Id; } }
		public string DataCriacao { get; set; }
		public int SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }
		public bool IsAbaFinalizar { get; set; }
		public bool IsVisualizar { get; set; }
		public int AgendamentoVistoriaId { get; set; }
		public int SetorId { get; set; }
		public string InformacaoComplementar { get; set; }

		public Requerimento Requerimento { get; set; }

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

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores
		{
			get { return _setores; }
			set { _setores = value; }
		}

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
					SetorObrigatorio = Mensagem.Requerimento.SetorObrigatorio,
					NaoExisteAssocicao = Mensagem.Requerimento.NaoExisteAssocicao
				});
			}
		}

		public ProjetoDigitalVM() { }

		public ProjetoDigitalVM(Requerimento requerimento, bool isVisualizar = false)
		{
			this.IsVisualizar = isVisualizar;
			this.Requerimento = requerimento;
			CarregarRequerimentoVM(requerimento);
		}

		public void CarregarListas(List<ResponsavelFuncoes> lstResponsavelFuncoes, List<AgendamentoVistoria> agendamentoVistoria, List<Setor> setores)
		{
			ResponsavelFuncoes = ViewModelHelper.CriarSelectList(lstResponsavelFuncoes, true);
			AgendamentoVistoria = ViewModelHelper.CriarSelectList(agendamentoVistoria, true);
			Setores = ViewModelHelper.CriarSelectList(setores, true, setores.Count > 1);
		}

		public void CarregarRequerimentoVM(Requerimento requerimento)
		{
			this.Id = requerimento.Numero;
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

				AtividadesSolicitadasVM.Add(atividade);
			}
		}

		public String ObterJSon()
		{
			return ViewModelHelper.Json(new
			{
				Id = Requerimento.Id,
				ProjetoDigitalId = Requerimento.ProjetoDigitalId,
				IdRelacionamento = Requerimento.IdRelacionamento,
				Tid = Requerimento.Tid,
				CredenciadoId = Requerimento.CredenciadoId,
				SetorId = Requerimento.SetorId,
				InteressadoCpfCnpj = Requerimento.Interessado.CPFCNPJ,
				InteressadoSelecaoTipo = (int)Requerimento.Interessado.SelecaoTipo,
				EmpreendimentoId = Requerimento.Empreendimento.Id,
				EmpreendimentoInternoId = Requerimento.Empreendimento.InternoId,
				EmpreendimentoSelecaoTipo = (int)Requerimento.Empreendimento.SelecaoTipo,
				EmpreendimentoDenominador = Requerimento.Empreendimento.Denominador,
				Pessoas = Requerimento.Pessoas
			});
		}
	}
}