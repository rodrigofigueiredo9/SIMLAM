using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.EtramiteX.Configuracao.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Publico.Data
{
	public class ListaValoresDa
	{
		public String UsuarioPublicoGeo
		{
			get { return new ConfiguracaoSistema().Obter<String>(ConfiguracaoSistema.KeyUsuarioPublicoGeo); }
		}

		public Dictionary<Int32, String> ObterDiretorioArquivoTemp()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 1", schema:UsuarioPublicoGeo);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}

		public Dictionary<Int32, String> ObterDiretorioArquivo()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 2", schema: UsuarioPublicoGeo);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}
	}
}