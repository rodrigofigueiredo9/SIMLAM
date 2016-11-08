using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCARSolicitacao
{
	public class CARSolicitacaoAlterarSituacaoVM
	{
		private CARSolicitacao _solicitacao = new CARSolicitacao();
		public CARSolicitacao Solicitacao
		{
			get { return _solicitacao; }
			set { _solicitacao = value; }
		}

		public List<SelectListItem> Situacoes { get; private set; }

		public CARSolicitacaoAlterarSituacaoVM() : this(new CARSolicitacao(), new List<Lista>()) { }

		public CARSolicitacaoAlterarSituacaoVM(CARSolicitacao solicitacao, List<Lista> _situacoes)
		{
			Solicitacao = solicitacao;
			Situacoes = ViewModelHelper.CriarSelectList(_situacoes, true, true);
		}
	}
}