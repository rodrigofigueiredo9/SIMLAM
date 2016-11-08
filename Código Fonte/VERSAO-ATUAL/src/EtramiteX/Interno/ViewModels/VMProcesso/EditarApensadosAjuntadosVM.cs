using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloProcesso;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMProcesso
{
	public class EditarApensadosJuntadosVM
	{
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					RequerimentoSituacaoInvalida = Mensagem.Processo.RequerimentoSituacaoInvalida,
					AtividadejaAdicionada = Mensagem.Processo.AtividadejaAdicionada,
					FinalidadeModeloTituloExistente = Mensagem.Requerimento.FinalidadeModeloTituloExistente,
					AssocieAtividade = Mensagem.Requerimento.NaoExisteAssocicao,
					FinalidadeObrigatorio = Mensagem.Requerimento.FinalidadeObrigatorioCad,
					TituloModeloObrigatorio = Mensagem.Requerimento.TituloObrigatorioCad,

					@BuscarObrigatorio = Mensagem.Requerimento.BuscarObrigatorio,
					@NumeroAnteriorObrigatorio = Mensagem.Requerimento.NumeroAnteriorObrigatorioModal,
					@TituloAnteriorObrigatorio = Mensagem.Requerimento.TituloAnteriorObrigatorioModal,
					@OrgaoExpedidorObrigatorio = Mensagem.Requerimento.OrgaoExpedidorObrigatorio
				});
			}
		}

		private Processo _processo = new Processo();
		public Processo Processo
		{
			get { return _processo; }
			set { _processo = value; }
		}

		private List<RequerimentoVM> _documentosRequerimentos = new List<RequerimentoVM>();
		public List<RequerimentoVM> DocumentosRequerimentos
		{
			get { return _documentosRequerimentos; }
			set { _documentosRequerimentos = value; }
		}

		private List<RequerimentoVM> _processosRequerimentos = new List<RequerimentoVM>();
		public List<RequerimentoVM> ProcessosRequerimentos
		{
			get { return _processosRequerimentos; }
			set { _processosRequerimentos = value; }
		}

		public void CarregarRequerimentoVM(List<Processo> processos, List<Documento> documentos)
		{
			foreach (Processo processo in Processo.Processos)
			{
				processo.Requerimento.Atividades = processo.Atividades;
				RequerimentoVM requerimentoVM = new RequerimentoVM(processo.Requerimento);
				
				ProcessosRequerimentos.Add(requerimentoVM);
			}

			foreach (Documento documento in Processo.Documentos)
			{
				documento.Requerimento.Atividades = documento.Atividades;
				RequerimentoVM requerimentoVM = new RequerimentoVM(documento.Requerimento);
				DocumentosRequerimentos.Add(requerimentoVM);
			}
		}
	}
}