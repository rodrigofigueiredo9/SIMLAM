using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural
{
	public enum eStatusImovelSicar
	{
		Nulo,
		[Description("Ativo")]
		Ativo,
		[Description("Pendente")]
		Pendente,
		[Description("Cancelado")]
		Cancelado
	}
}
