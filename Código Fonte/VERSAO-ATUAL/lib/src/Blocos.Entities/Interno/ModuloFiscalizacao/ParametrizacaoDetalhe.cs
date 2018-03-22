using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ParametrizacaoDetalhe
	{
		#region Constructor
		public ParametrizacaoDetalhe() { }
		#endregion

		#region Properties
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 ParametrizacaoId { get; set; }
		public Int32 MaximoParcelas { get; set; }
		public Decimal ValorInicial { get; set; }
		public Decimal ValorFinal { get; set; }
		public bool Excluir { get; set; }
		public bool? Editado { get; set; }
		#endregion
	}
}