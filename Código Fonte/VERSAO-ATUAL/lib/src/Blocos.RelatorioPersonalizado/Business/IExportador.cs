using System.IO;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	interface IExportador
	{
		void Exportar(DadosRelatorio dados, Stream stream);
		byte[] Exportar(DadosRelatorio dados);
		void Exportar(DadosRelatorio dados, out Arquivo.Arquivo arquivo);
	}
}
