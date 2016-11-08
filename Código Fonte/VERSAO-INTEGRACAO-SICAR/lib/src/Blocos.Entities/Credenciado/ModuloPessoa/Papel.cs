using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Credenciado.Security;

namespace Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa
{
	public class Papel
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public int IdRelacao { get; set; }
		private List<Permissao> _permissoes = new List<Permissao>();
		public List<Permissao> Permissoes
		{
			get { return _permissoes; }
			set { _permissoes = value; }
		}
	}
}