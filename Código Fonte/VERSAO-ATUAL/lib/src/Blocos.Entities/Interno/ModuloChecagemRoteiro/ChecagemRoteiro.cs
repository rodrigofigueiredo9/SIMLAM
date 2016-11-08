using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro
{
	
	public class ChecagemRoteiro
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Interessado { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		public bool TemPendencia { get; set; }

		private List<Item> _itens = new List<Item>();
		public List<Item> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		private List<Roteiro> _roteiros = new List<Roteiro>();
		public List<Roteiro> Roteiros
		{
			get { return _roteiros; }
			set { _roteiros = value; }
		}

		public ChecagemRoteiro() { }
	}
}
