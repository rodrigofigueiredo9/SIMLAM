using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario
{
	public class Papel
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public int IdRelacao { get; set; }
		public bool IsAtivo { get; set; }
		private List<Permissao> _permissoes = new List<Permissao>();
		public List<Permissao> Permissoes
		{
			get { return _permissoes; }
			set { _permissoes = value; }
		}
	}
}
