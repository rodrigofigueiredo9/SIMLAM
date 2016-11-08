using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao
{
	public class CertidaoAnuencia : Especificidade
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public List<DestinatarioEspecificidade> Destinatarios { set; get; }
		public String Certificacao { get; set; }

		public CertidaoAnuencia()
		{
			Destinatarios = new List<DestinatarioEspecificidade>();
		}
	}
}