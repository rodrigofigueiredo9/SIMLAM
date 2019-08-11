using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Home;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Business
{
	public class LogBus
	{
		LogDa _da = new LogDa();

		public class KeyCompare: IComparer<string>
		{
			public int Compare(string x, string y)
			{
				if (x!=y && (x == "MENSAGEM" || y == "MENSAGEM"))
				{
					return 1;
				}

				return String.Compare(y, x);
			}
		}

		public List<Dictionary<string, string>> Obter(Log log)
		{
            List<Dictionary<string, string>> retorno = null;
            try
            {
                retorno = _da.Obter(log);

                if (retorno.Count < 1)
                {
                    Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
                }
            }
            catch (Exception exc) 
            {
                Validacao.AddErro(exc);
            }

            return retorno;
		}

		public void Inserir(string log) => _da.Inserir(log);

        public List<string> ObterListSource()
        {
            return _da.ObterListaSource();
        }
	}
}
