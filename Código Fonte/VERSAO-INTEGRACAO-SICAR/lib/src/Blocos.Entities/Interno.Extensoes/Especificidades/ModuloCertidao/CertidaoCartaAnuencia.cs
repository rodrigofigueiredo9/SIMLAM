using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertidao
{
	public class CertidaoCartaAnuencia : Especificidade
	{
		public Int32 Id { set; get; }
		public Int32 Requerimento { get; set; }
		public List<DestinatarioEspecificidade> Destinatarios { set; get; }
		public Int32 Dominio { set; get; }
		public String Descricao { get; set; }
		public String Tid { set; get; }

		public CertidaoCartaAnuencia()
		{
			Destinatarios = new List<DestinatarioEspecificidade>();
		}
	}
}