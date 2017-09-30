

using System;

namespace Tecnomapas.Blocos.Etx.ModuloValidacao
{
	public partial class Mensagem
	{
		private static Mensagem _Mensagem = new Mensagem();
		public static Mensagem Credenciado
		{
			get { return _Mensagem; }
		}
	}
}