using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
	public enum eSituacaoRL
	{
		[Description("Aprovada")]
		Aprovada,
		[Description("Não aprovada")]
		NaoAprovada,
		[Description("Não analisada")]
		NaoAnalisada
	}
}
