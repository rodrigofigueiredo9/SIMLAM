using System;
using Tecnomapas.Blocos.Entities.Interno.Security;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador
{
	public class Permissao
	{
		public Int32 Id { get; set; }
		public int IdRelacao { get; set; }
		public String Nome { get; set; }
		public ePermissao Codigo { get; set; }
		public String CodigoString { get { return Codigo.ToString(); } }
		public Int32 AdministradorTipo { get; set; }
		public String Descricao { get; set; }
	}
}
