

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio
{
	public interface IAssinanteDataSource
	{
		IAssinante Assinante { get; set; }
		List<IAssinante> Assinantes1 { get; set; }
		List<IAssinante> Assinantes2 { get; set; }
		List<IAssinante> AssinanteSource { get; set; }
	}
}