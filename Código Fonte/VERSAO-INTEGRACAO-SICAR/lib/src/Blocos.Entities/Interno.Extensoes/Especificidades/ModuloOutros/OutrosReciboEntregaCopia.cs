using System;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros
{
	public class OutrosReciboEntregaCopia : Especificidade
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 Requerimento { get; set; }
		public Int32 Destinatario { set; get; }
		public String DestinatarioNomeRazao { set; get; }
	}
}