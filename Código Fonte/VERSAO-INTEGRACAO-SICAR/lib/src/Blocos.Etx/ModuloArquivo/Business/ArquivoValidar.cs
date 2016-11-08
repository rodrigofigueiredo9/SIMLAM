using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.Blocos.Etx.ModuloArquivo.Business
{
	public class ArquivoValidar
	{
		public bool VerificarTiposArquivosValidos(List<String> tiposArquivos, List<Arquivo.Arquivo> arquivos)
		{
			foreach (Arquivo.Arquivo arquivo in arquivos)
			{
				if (!tiposArquivos.Any(x => String.Equals("." + x.ToString(), arquivo.Extensao, StringComparison.CurrentCultureIgnoreCase)))
				{
					Validacao.Add(Mensagem.Arquivo.ArquivoTipoInvalido(arquivo.Nome, tiposArquivos));
				}
			}

			return Validacao.EhValido;
		}
	}
}
