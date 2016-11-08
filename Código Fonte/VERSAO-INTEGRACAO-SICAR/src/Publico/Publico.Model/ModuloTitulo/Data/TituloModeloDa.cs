using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloTitulo.Data
{
	public class TituloModeloDa
	{
		public List<TituloModeloLst> ObterModelos(int exceto = 0, bool todos = false)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.codigo, t.nome from tab_titulo_modelo t");

				if (exceto > 0 || !todos)
				{

					comando.DbCommand.CommandText += " where ";

					if (exceto > 0)
					{
						comando.DbCommand.CommandText += "t.id <> :id";
						comando.AdicionarParametroEntrada("id", exceto, DbType.Int32);
					}

					if (!todos)
					{
						if (exceto > 0)
						{
							comando.DbCommand.CommandText += " and ";
						}

						comando.DbCommand.CommandText += "t.situacao = 1";
					}
				}

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloModeloLst()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						Codigo = Convert.IsDBNull(item["codigo"]) ? 0 : Convert.ToInt32(item["codigo"]),
						IsAtivo = true
					});
				}
			}
			return lst;
		}
	}
}
