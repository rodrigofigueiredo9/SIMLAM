using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Model.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using System.IO;

namespace Tecnomapas.Blocos.Entities.Model.Business
{
	public static class Excel
	{
		public static MemoryStream GerarPlanilha(List<List<string>> linhas)
		{
			var memStream = new MemoryStream();
			StreamWriter arquivo = new StreamWriter(memStream);

				linhas.ForEach(x => {
				arquivo.Write(String.Join(" ; ", x));
			});

			arquivo.Flush(); 
			memStream.Seek(0, SeekOrigin.Begin);

			return memStream;
		}
	}
}