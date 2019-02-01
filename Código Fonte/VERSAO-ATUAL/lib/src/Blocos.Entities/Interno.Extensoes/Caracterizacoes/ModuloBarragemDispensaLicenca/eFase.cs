using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public enum eFase
	{
		Nulo = 0,
		[Description("Contruída")]
		Construida = 1,
		[Description("A construir")]
		AConstruir = 2
	}
}
