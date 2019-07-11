using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Home;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Data
{
	public class LogDa
	{
		public List<Dictionary<string, string>> Obter(Log log)
		{
			List<Dictionary<string, string>> lstDyn = new List<Dictionary<string, string>>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
                Comando comando = bancoDeDados.CriarComando(@"");
                string filtros = "";

                if (!string.IsNullOrEmpty(log.DataDe)) 
                {
                    filtros += " and data >= :dataDe";
                    comando.AdicionarParametroEntrada("dataDe", log.DataDe, System.Data.DbType.Date);
                }

                if (!string.IsNullOrEmpty(log.DataAte))
                {
                    filtros += " and data <= :dataAte";
                    comando.AdicionarParametroEntrada("dataAte", log.DataAte, System.Data.DbType.Date);
                }

                if (!string.IsNullOrEmpty(log.Source) && log.Source != "0")
                {
                    filtros += " and source = :source";
                    comando.AdicionarParametroEntrada("source", log.Source, System.Data.DbType.String);
                }

                if (!string.IsNullOrEmpty(log.Mensagem))
                {
                    filtros += " and mensagem like :mensagem";
                    comando.AdicionarParametroEntrada("mensagem", "%" + log.Mensagem + "%", System.Data.DbType.String);
                }

                comando.DbCommand.CommandText = "select * from log_servicos where 1 = 1 "+ filtros +"  order by 1 desc";
                
				bancoDeDados.ExecutarReader(comando, (reader) =>
				{
					lstDyn.Add(reader.CriarDictionary<String>());
				});
			}

			return lstDyn;
		}

		public void Inserir(string log)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"insert into log_servicos (id, data, source, mensagem)
					values (seq_log_servicos.nextval, sysdate, 'SIMLAM', :mensagem)");
				comando.AdicionarParametroEntrada("mensagem", log, System.Data.DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		public List<string> ObterListaSource()
        {
            List<string> lst = new List<string>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select distinct source from log_servicos");

                bancoDeDados.ExecutarReader(comando, (reader) =>
                {
                    lst.Add(reader["source"].ToString());
                });
            }

            return lst;
        }
	}
}
