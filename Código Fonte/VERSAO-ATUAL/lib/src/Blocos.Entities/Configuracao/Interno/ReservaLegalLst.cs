﻿namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class ReservaLegalLst : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
		public int Codigo { get; set; }
	}
}