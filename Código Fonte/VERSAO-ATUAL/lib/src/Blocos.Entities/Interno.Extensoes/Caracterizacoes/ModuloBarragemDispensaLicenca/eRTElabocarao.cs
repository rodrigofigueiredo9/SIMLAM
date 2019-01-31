using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public enum eRTElabocarao
	{
		Nulo = 0,
		[Description("Projeto técnico / laudo de barragem contruída")]
		projetoTecnicoLaudoBarragemConstruida,
		[Description("Estudo ambiental")]
		estudoAmbiental,
		[Description("Ambos")]
		ambos
	}
}
