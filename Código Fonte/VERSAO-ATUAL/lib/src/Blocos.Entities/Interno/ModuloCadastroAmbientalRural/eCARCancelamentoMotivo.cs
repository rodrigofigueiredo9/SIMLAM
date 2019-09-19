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
		[Description("Cancelado por decisão judicial")]
		DecisaoJudicial,
		[Description("Cancelado por decisão administrativa")]
		DecisaoAdmnistrativa,
		[Description("Cancelaodo por solicitação do proprietário")]
		SolicitacaoProprietaro
	}
}
