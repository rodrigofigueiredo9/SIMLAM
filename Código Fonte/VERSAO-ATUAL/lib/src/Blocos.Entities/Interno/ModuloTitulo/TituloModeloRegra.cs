using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloModeloRegra
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public bool Valor { get; set; }
		public int Tipo { get; set; }
		public eRegra TipoEnum { get { return (eRegra)Tipo; } set { Tipo = (Int32)value; } }

		private List<TituloModeloResposta> _listaRespostas = new List<TituloModeloResposta>();
		public List<TituloModeloResposta> Respostas
		{
			get { return _listaRespostas; }
			set { _listaRespostas = value; }
		}

		public TituloModeloRegra() { }
	}
}
