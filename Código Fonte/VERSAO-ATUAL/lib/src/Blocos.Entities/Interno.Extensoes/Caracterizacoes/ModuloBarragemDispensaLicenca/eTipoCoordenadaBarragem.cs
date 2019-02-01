using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public enum eTipoCoordenadaBarragem
	{
		[Description("Barramento")]
		barramento = 1,
		[Description("Área de bota-fora")]
		areaBotaFora,
		[Description("Área de empréstimo")]
		areaEmprestimo,
	}
}
