using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class RespostaInfracao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }

		private List<Item> _itens = new List<Item>();
		public List<Item> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}
	}
}