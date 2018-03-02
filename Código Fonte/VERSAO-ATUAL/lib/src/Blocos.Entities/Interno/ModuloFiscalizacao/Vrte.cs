using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Vrte
	{
		#region Constructor
		public Vrte() { }
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Ano { get; set; }
		public Decimal VrteEmReais { get; set; }
		public bool Excluir { get; set; }
		public bool? Editado { get; set; }
		#endregion
	}
}