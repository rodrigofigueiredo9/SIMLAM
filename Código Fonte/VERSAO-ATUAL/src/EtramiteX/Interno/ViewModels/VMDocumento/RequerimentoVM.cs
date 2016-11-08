using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMDocumento
{
	public class RequerimentoVM
	{
		public Int32 Id { get; set; }
		public Int32 Numero { get { return Id; } }
		public String DataCriacao { get; set; }
		public String Situacao { get; set; }
		public Int32 SituacaoId { get; set; }
		public Boolean IsEditar { get; set; }
		public bool IsVisualizar { get; set; }

		public int ProtocoloId { get; set; }
		public int ProtocoloTipo { get; set; }

		private bool _isRequerimentoDocumento = true;
		public bool IsRequerimentoDocumento
		{
			get { return _isRequerimentoDocumento; }
			set { _isRequerimentoDocumento = value; }
		}

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

		public RequerimentoVM() { this.DataCriacao = string.Empty; }

		public RequerimentoVM(Requerimento requerimento)
		{
			CarregarRequerimentoVM(requerimento);
		}

		public void CarregarListas(List<ResponsavelFuncoes> lstResponsavelFuncoes)
		{
			ResponsavelFuncoes = ViewModelHelper.CriarSelectList(lstResponsavelFuncoes, true);
		}

		public void CarregarRequerimentoVM(Requerimento requerimento)
		{
			this.Id = requerimento.Numero;
			this.DataCriacao = requerimento.DataCadastro.ToShortDateString();
			this.Situacao = requerimento.SituacaoTexto;
			this.Responsaveis = requerimento.Responsaveis;
			this.AtividadesSolicitadas = requerimento.Atividades;
			this.Interessado = requerimento.Interessado;
			this.Empreendimento = requerimento.Empreendimento;
			this.ProtocoloId = requerimento.ProtocoloId;
			this.ProtocoloTipo = requerimento.ProtocoloTipo;
			this.SituacaoId = requerimento.SituacaoId;

			CarregarAtividadeSolicitada();
		}

		private void CarregarAtividadeSolicitada()
		{
			foreach (var item in this.AtividadesSolicitadas)
			{
				AtividadeSolicitadaVM atividade = new AtividadeSolicitadaVM();

				atividade.Id = item.Id;
				atividade.IdRelacionamento = item.IdRelacionamento.Value;
				atividade.NomeAtividade = item.NomeAtividade;
				atividade.Finalidades = item.Finalidades;
				atividade.Situacao = item.SituacaoId;
				atividade.SituacaoTexto = item.SituacaoTexto;
				atividade.RetirarIconeExcluirAtividade = true;
				atividade.SetorId = item.SetorId;

				AtividadesSolicitadasVM.Add(atividade);
				
			}
		}

		public void SetSituacaoAtividadeCadastro(string situacaoCadastro)
		{
			foreach (AtividadeSolicitadaVM atividade in this.AtividadesSolicitadasVM)
			{
				atividade.Situacao = 1;
				atividade.SituacaoTexto = situacaoCadastro;
			}
		}

		public void ResetIdRelacionamento()
		{
			foreach (ResponsavelTecnico responsavel in this.Responsaveis)
			{
				responsavel.IdRelacionamento = 0;
			}

			foreach (AtividadeSolicitadaVM atividade in this.AtividadesSolicitadasVM)
			{
				atividade.IdRelacionamento = 0;
			}
		}
	}
}