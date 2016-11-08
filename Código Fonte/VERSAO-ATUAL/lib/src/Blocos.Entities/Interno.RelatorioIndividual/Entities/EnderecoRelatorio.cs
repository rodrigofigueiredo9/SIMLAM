

using System;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities
{
	public class EnderecoRelatorio
	{
		public Int32? Id { get; set; }
		public Int32? ArtefatoId { get; set; }
		public Int32? Correspondencia { get; set; }
		public String Cep { get; set; }
		public String Logradouro { get; set; }
		public String Bairro { get; set; }
		public Int32 EstadoId { get; set; }
		public String EstadoTexto { get; set; }
		public String EstadoSigla { get; set; }
		public String Uf { get; set; }
		public Int32 MunicipioId { get; set; }
		public String MunicipioTexto { get; set; }
		public String Numero { get; set; }
		public String Complemento { get; set; }
		public String Detalhes { get; set; }
		public String Tid { get; set; }
		public String Distrito { get; set; }
		public String Zona { get; set; }
		public String Corrego { get; set; }

		public String EnderecoFormatado 
		{ 
			get 
			{
				var retorno = Logradouro;

				if(!string.IsNullOrWhiteSpace(Numero))
				{
					retorno += ", " + Numero;
				}

				if (!string.IsNullOrWhiteSpace(Bairro))
				{
					retorno += ", " + Bairro;
				}

				if (!string.IsNullOrWhiteSpace(Distrito))
				{
					retorno += ", " + Distrito;
				}

				return retorno; 
			} 
		}


		public EnderecoRelatorio()
		{

		}
	}
}