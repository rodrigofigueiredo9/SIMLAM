

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao
{
	public class Protocolo
	{
		public int Id { get; set; }
		public String Numero { get; set; }
		public int Tipo { get; set; }
		public String TipoTexto { get; set; }
		public bool IsProcesso { get; set; }

		public String Texto
		{
			get
			{
				if (IsProcesso)
				{
					return "Processo";
				}

				return "Documento";
			}
		}

		public Protocolo()
		{
			IsProcesso = true;
		}
	}
}