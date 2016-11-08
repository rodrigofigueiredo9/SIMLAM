using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOficio
{
	public class OficioNotificacao : Especificidade
	{
		public int? Id { get; set; }
		public String Tid { get; set; }
		public Int32? Destinatario { get; set; }
		public String DestinatarioNomeRazao { set; get; }

		private List<AnaliseItemEsp> _itens = new List<AnaliseItemEsp>();
		public List<AnaliseItemEsp> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}
	}
}