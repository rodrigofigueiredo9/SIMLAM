using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Model.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using System.IO;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Model.Business
{
	public static class Excel
	{
		public static MemoryStream GerarPlanilha(List<List<string>> linhas)
		{
			var memStream = new MemoryStream();
			StreamWriter arquivo = new StreamWriter(memStream, Encoding.UTF8);

				linhas.ForEach(x => {
					arquivo.Write(String.Join(" ; ", x));
					arquivo.Write("\n");
				});

			arquivo.Flush(); 
			memStream.Seek(0, SeekOrigin.Begin);

			return memStream;
		}
	}
}