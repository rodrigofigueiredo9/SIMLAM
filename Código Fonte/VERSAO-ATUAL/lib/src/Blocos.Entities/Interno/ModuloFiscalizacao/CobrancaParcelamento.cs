using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class CobrancaParcelamento
	{
		#region Constructor
		public CobrancaParcelamento() { }
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 CobrancaId { get; set; }
		public List<CobrancaDUA> DUAS { get; set; }
		#endregion
	}
}