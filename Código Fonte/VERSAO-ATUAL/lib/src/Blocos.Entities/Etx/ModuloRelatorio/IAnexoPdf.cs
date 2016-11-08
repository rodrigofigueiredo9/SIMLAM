using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio
{
	public interface IAnexoPdf
	{
		List<Arquivo.Arquivo> AnexosPdfs { get; set; }
	}
}