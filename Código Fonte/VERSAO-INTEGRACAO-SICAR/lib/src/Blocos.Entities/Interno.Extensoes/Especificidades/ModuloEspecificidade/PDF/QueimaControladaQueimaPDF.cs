using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class QueimaControladaQueimaPDF
	{
		private List<Cultivo> _cultivos = new List<Cultivo>();
		public List<Cultivo> Cultivos
		{
			get { return _cultivos; }
			set { _cultivos = value; }
		}
	}
}