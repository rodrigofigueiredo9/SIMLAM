

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario
{
	public class FuncionarioListarFiltro
	{
		public int Id { get; set; }
		public String Nome { get; set; }
		public String Login { get; set; }
		public String Cpf { get; set; }
		public int Situacao { get; set; }
		public int Cargo { get; set; }
		public int Setor { get; set; }
	}
}