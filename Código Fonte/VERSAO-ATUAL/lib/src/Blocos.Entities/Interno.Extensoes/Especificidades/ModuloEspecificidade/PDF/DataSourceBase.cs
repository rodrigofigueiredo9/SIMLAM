using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class DataSourceBase : IAssinanteDataSource
	{
		public IAssinante Assinante { get; set; }
		public List<IAssinante> Assinantes1 { get; set; }
		public List<IAssinante> Assinantes2 { get; set; }

		private List<IAssinante> _assinanteSource = new List<IAssinante>();
		public List<IAssinante> AssinanteSource
		{
			get { return _assinanteSource; }
			set { _assinanteSource = value; }
		}

		public List<CondicionantePDF> Condicionantes { set; get; }
	}
}