using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloPTV.Destinatario
{
	public class DestinatarioPTV
	{
		public int ID { get; set; }
        public decimal? CodigoUC { get; set; }
        public int EmpreendimentoId { get; set; }
		public string TID { get; set; }
		public int PessoaTipo { get; set; }
		public string CPFCNPJ { get; set; }
		public string NomeRazaoSocial { get; set; }
		public string Endereco { get; set; }
		public int EstadoID { get; set; }
		public string EstadoTexto { get; set; }
		public string EstadoSigla { get; set; }
		public int MunicipioID { get; set; }
		public string MunicipioTexto { get; set; }
		public string Itinerario { get; set; }
		public string Pais { get; set; }
	}
}