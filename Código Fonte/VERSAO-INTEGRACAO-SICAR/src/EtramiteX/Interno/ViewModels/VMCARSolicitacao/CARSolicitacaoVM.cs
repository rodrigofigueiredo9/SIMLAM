using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao
{
	public class CARSolicitacaoVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsCredenciado { get; set; }

		private CARSolicitacao _solicitacao = new CARSolicitacao();
		public CARSolicitacao Solicitacao
		{
			get { return _solicitacao; }
			set { _solicitacao = value; }
		}

		private List<SelectListItem> _situacaoLst = new List<SelectListItem>();
		public List<SelectListItem> SituacaoLst
		{
			get { return _situacaoLst; }
			set { _situacaoLst = value; }
		}

		private List<SelectListItem> _requerimentoLst = new List<SelectListItem>();
		public List<SelectListItem> RequerimentoLst
		{
			get { return _requerimentoLst; }
			set { _requerimentoLst = value; }
		}

		private List<SelectListItem> _atividadeLst = new List<SelectListItem>();
		public List<SelectListItem> AtividadeLst
		{
			get { return _atividadeLst; }
			set { _atividadeLst = value; }
		}

		private List<SelectListItem> _declaranteLst = new List<SelectListItem>();
		public List<SelectListItem> DeclaranteLst
		{
			get { return _declaranteLst; }
			set { _declaranteLst = value; }
		}

		public CARSolicitacaoVM(){}

		public CARSolicitacaoVM(CARSolicitacao solicitacao, List<Lista> situacaoLst, List<Protocolos> processosDocumentos,List<ProcessoAtividadeItem> atividadeLst, List<PessoaLst> declaranteLst, bool isVisualizar = false)
		{
			Solicitacao = solicitacao;
			IsVisualizar = isVisualizar;

			String requerimentoSelecionado = null;
			if (solicitacao.Id > 0)
			{
				requerimentoSelecionado = solicitacao.ProtocoloSelecionado.Id + "@" + (solicitacao.ProtocoloSelecionado.IsProcesso ? "1" : "2") + "@" + solicitacao.Requerimento.Id;
			}

			RequerimentoLst = ViewModelHelper.CriarSelectList(processosDocumentos, true, true, requerimentoSelecionado);
			SituacaoLst = ViewModelHelper.CriarSelectList(situacaoLst, true, true, (Solicitacao.Id > 0) ? solicitacao.SituacaoId.ToString() : ((int)eCARSolicitacaoSituacao.EmCadastro).ToString());
			AtividadeLst = ViewModelHelper.CriarSelectList(atividadeLst, true, true, Solicitacao.Atividade.Id.ToString());
			DeclaranteLst = ViewModelHelper.CriarSelectList(declaranteLst, true, true, Solicitacao.Declarante.Id.ToString());
		}
	}
}