using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo
{
	public class TermoCompromissoAmbiental : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }

		public Int32 Destinatario { get; set; }
		public Int32 DestinatarioTipo { get; set; }
		public String DestinatarioTid { get; set; }
		public String DestinatarioNomeRazao { get; set; }

		public Int32 Representante { get; set; }
		public String RepresentanteNomeRazao { get; set; }

		public Int32 Licenca { set; get; }
		public String LicencaNumero { set; get; }

		public String Descricao { set; get; }

		public TermoCompromissoAmbiental()
		{

		}

	}
}
