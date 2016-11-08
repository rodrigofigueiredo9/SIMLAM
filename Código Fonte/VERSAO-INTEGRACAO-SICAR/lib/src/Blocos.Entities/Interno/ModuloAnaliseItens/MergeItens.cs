using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens
{
	public class MergeItens
	{
		private List<Roteiro> _roteiros = new List<Roteiro>();
		public List<Roteiro> Roteiros
		{
			get { return _roteiros; }
			set { _roteiros = value; }
		}

		private List<Item> _itens = new List<Item>();
		public List<Item> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		private List<Item> _itensAtuais = new List<Item>();
		public List<Item> ItensAtuais
		{
			get { return _itensAtuais; }
			set { _itensAtuais = value; }
		}

		public List<Item> ItensAtualizados
		{
			get
			{
				return Itens.Where(item => item.StatusId > 0).ToList();
			}
		}
	}
}