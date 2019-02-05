using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public enum eTipoCoordenadaBarragem
	{
		[Description("Nulo")]
		Nulo,
		[Description("Barramento")]
		barramento,
		[Description("Área de bota-fora")]
		areaBotaFora,
		[Description("Área de empréstimo")]
		areaEmprestimo,
	}
}
