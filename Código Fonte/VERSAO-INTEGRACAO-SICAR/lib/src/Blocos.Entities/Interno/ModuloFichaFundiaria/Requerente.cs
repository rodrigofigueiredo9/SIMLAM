

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria
{
	public class Requerente
	{
		public String Nome { get; set; }
		public String DocumentoTipo { get; set; }
		public String DocumentoNumero { get; set; }
		public String NomePai { get; set; }
		public String NomeMae { get; set; }
		public String Endereco { get; set; }

		public String DocumentoTipoNumero 
		{
			get 
			{
				if (!String.IsNullOrWhiteSpace(this.DocumentoTipo) && !String.IsNullOrWhiteSpace(this.DocumentoNumero)) 
				{
					return this.DocumentoTipo + " - " + this.DocumentoNumero;
				}

				return String.Empty;
			}
		}
	}
}
