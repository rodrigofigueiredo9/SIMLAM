using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;

namespace Tecnomapas.EtramiteX.Credenciado.ViewModels.VMCARSolicitacao
{
	public class CARSolicitacaoVM
	{
		public bool IsVisualizar { get; set; }
		public bool IsInterno { get; set; }

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

		public CARSolicitacaoVM() { }

		public CARSolicitacaoVM(CARSolicitacao solicitacao, List<Lista> situacaoLst, List<Lista> atividadeLst, List<PessoaLst> declarantesLst, List<Protocolos> processosDocumentos = null, bool isVisualizar = false)
		{
			Solicitacao = solicitacao;
			IsVisualizar = isVisualizar;

			String requerimentoSelecionado = null;
			if (solicitacao.Id > 0)
			{
				requerimentoSelecionado = solicitacao.ProtocoloSelecionado.Id + "@" + (solicitacao.ProtocoloSelecionado.IsProcesso ? "1" : "2") + "@" + solicitacao.Requerimento.Id;
			}

			SituacaoLst = ViewModelHelper.CriarSelectList(situacaoLst, true, true, (Solicitacao.Id > 0) ? solicitacao.SituacaoId.ToString() : ((int)eCARSolicitacaoSituacao.Valido).ToString());
			AtividadeLst = ViewModelHelper.CriarSelectList(atividadeLst, true, true, Solicitacao.Atividade.Id.ToString());
			DeclaranteLst = ViewModelHelper.CriarSelectList(declarantesLst, true, true, Solicitacao.Declarante.Id.ToString());

			if (processosDocumentos != null)
			{
				RequerimentoLst = ViewModelHelper.CriarSelectList(processosDocumentos, true, true, requerimentoSelecionado);
			}
		}
	}
}