﻿namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class TipoItem : IListaValor
	{
		public int Id { get; set; }
		public string Texto { get; set; }
		public bool IsAtivo { get; set; }
	}
}