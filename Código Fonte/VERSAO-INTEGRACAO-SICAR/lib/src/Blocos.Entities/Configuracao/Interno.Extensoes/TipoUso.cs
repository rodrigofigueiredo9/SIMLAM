﻿namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class TipoUso : IListaValor
	{
		public int Id { set; get; }
		public string Texto { set; get; }
		public bool IsAtivo { set; get; }
	}
}