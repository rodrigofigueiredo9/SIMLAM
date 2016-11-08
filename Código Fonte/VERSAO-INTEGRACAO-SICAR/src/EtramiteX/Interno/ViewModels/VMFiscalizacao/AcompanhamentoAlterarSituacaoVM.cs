using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class AcompanhamentoAlterarSituacaoVM
	{
		public Acompanhamento Acompanhamento { get; set; }
		public List<SelectListItem> SituacaoNova { get; set; }
		public int SituacaoCancelado { get { return (int)eAcompanhamentoSituacao.Cancelado; } }

		public AcompanhamentoAlterarSituacaoVM() : this(new List<Lista>()) { }

		public AcompanhamentoAlterarSituacaoVM(List<Lista> situacoes)
		{
			SituacaoNova = ViewModelHelper.CriarSelectList(situacoes, true);
			Acompanhamento = new Acompanhamento();
		}
	}
}