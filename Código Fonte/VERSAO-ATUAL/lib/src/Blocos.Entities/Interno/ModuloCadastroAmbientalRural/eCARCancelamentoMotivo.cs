using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
	public enum eCARCancelamentoMotivo
	{
		Nulo,
		[Description("Decisão judicial")]
		DecisaoJudicial,
		[Description("Decisão administrativa")]
		DecisaoAdmnistrativa,
		[Description("Solicitação do proprietário")]
		SolicitacaoProprietaro
	}
}
