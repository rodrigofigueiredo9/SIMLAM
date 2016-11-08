using System;

namespace Tecnomapas.Blocos.Entities.Credenciado.Security
{
	public class Permissao
	{
		public Int32 Id { get; set; }
		public int IdRelacao { get; set; }
		public String Nome { get; set; }
		public ePermissao Codigo { get; set; }
		public String CodigoString { get { return Codigo.ToString(); } }
		public Int32 CredenciadoTipo { get; set; }
		public String Descricao { get; set; }
	}
}