using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado
{
	public class CertificadoRegistro : Especificidade
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }
		public Int32 Destinatario { get; set; }
		public String DestinatarioNomeRazao { get; set; }
		public String Classificacao { get; set; }
		public String Registro { get; set; }

		private List<AnaliseItemEsp> _itens = new List<AnaliseItemEsp>();
		public List<AnaliseItemEsp> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}
	}
}