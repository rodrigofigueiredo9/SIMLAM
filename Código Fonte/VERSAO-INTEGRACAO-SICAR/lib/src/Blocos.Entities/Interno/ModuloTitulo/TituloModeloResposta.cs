

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloModeloResposta
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public object Valor { get; set; }
		public int Tipo { get; set; }
		public eResposta TipoEnum { get { return (eResposta)Tipo; } set { Tipo = (Int32)value; } }

		public TituloModeloResposta() { }
	}
}
