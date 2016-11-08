using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada
{
	public class QueimaControladaQueima
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Identificacao { get; set; }
		public Decimal AreaCroqui { get; set; }
		public String AreaRequerida { get; set; }

		private List<Cultivo> _cultivos = new List<Cultivo>();
		public List<Cultivo> Cultivos
		{
			get { return _cultivos; }
			set { _cultivos = value; }
		}
	}
}
