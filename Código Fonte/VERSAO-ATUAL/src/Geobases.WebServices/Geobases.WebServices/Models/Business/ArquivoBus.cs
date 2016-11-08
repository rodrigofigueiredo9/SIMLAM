using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Tecnomapas.Geobases.WebServices.Models.Entities;

namespace Tecnomapas.Geobases.WebServices.Models.Business
{
	public class ArquivoBus
	{
        public static byte[] BuscarArquivoDiretorio(string nome, string diretorio)
        {
            byte[] arquivo = null;
            if (!string.IsNullOrEmpty(diretorio))
            {
                diretorio += "\\" + nome;
            }
            else
            {
                diretorio = nome;
            }
            if (File.Exists(diretorio))
            {
                arquivo = File.ReadAllBytes(diretorio);
            }

            return arquivo;
        }

		public static bool ExisteArquivo(string nome, string diretorio)
		{
			if (!string.IsNullOrEmpty(diretorio))
			{
				diretorio += "\\" + nome;
			}
			else
			{
				diretorio = nome;
			}
			return File.Exists(diretorio);
		}

	}
}