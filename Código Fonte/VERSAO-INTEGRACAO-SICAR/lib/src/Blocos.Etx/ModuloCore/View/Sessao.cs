

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Etx.ModuloCore.View
{
	public class Sessao
	{
		public string Titulo { get; set; }
		public List<Objeto> Objetos { get; private set; }

		public Sessao(string titulo = "")
		{
			Objetos = new List<Objeto>();
			Titulo = titulo;
		}

		public override string ToString()
		{
			return string.Format("[Titulo = {0}; Objetos = {1} ]", Titulo, Objetos.Count);
		}
	}
}
