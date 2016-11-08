using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPapel
{
	public class Papel
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public String Tid { get; set; }
		public List<Permissao> Permissoes { get; set; }
	}
}