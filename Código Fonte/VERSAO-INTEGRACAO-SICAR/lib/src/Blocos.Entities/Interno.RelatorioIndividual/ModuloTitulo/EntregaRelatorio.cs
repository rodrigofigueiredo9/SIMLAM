using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTitulo
{
	public class EntregaRelatorio
	{
		public string Nome { get; set; }
		public string CPF { get; set; }
		public string ProtocoloNumero { get; set; }
		public string ProtocoloTipo { get; set; }
		public int ProtocoloSetorCriacao { get; set; }
		public DateTime DataEntrega { get; set; }
		public string LocalEntrega { get; set; }

		private List<TituloRelatorio> _titulos = new List<TituloRelatorio>();
		public List<TituloRelatorio> Titulos
		{
			get { return _titulos; }
			set { _titulos = value; }
		}
	}
}
