using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL.Entities;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;

namespace Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL.Data
{
	internal class SqlBuilderDa
	{
		private string _tabela;
		private string _sequencia;
		ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

		public SqlBuilderDa(string tabela, string sequencia)
		{
			_tabela = tabela;
			_sequencia = sequencia;
		}

		public List<ColunaOracle> ObterColunas()
		{
			List<ColunaOracle> colunas = new List<ColunaOracle>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(_configSys.UsuarioRelatorio))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.COLUMN_NAME, c.DATA_TYPE, c.DATA_LENGTH 
					from user_tab_columns c where c.TABLE_NAME = upper(:tabela) order by c.COLUMN_NAME");

				comando.AdicionarParametroEntrada("tabela", DbType.String, 30, _tabela);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						colunas.Add(new ColunaOracle()
						{
							ColumnName = reader.GetValue<string>("COLUMN_NAME"),
							DataType = reader.GetValue<string>("DATA_TYPE"),
							DataLength = reader.GetValue<int>("DATA_LENGTH")
						});
					}
					reader.Close();
				}

				return colunas;
			}
		}

		public string ObterInsert()
		{
			List<ColunaOracle> lstColunas = ObterColunas();
			List<string> lstInsertCol = new List<string>();
			List<string> lstInsertVal = new List<string>();

			lstColunas.ForEach(x =>
			{
				lstInsertCol.Add(x.ColumnName);

				if (x.ColumnName == "ID")
				{
					lstInsertVal.Add(string.Format("{0}.nextval", _sequencia));
				}
				else
				{
					lstInsertVal.Add(string.Format(":{0}", x.ColumnName));
				}
			});


			return String.Format(@"insert into {0} ({1}) values ({2})",
				_tabela,
				String.Join(", ", lstInsertCol.ToArray()),
				String.Join(", ", lstInsertVal.ToArray()));
		}

		public void AddColuna(string tabela, string coluna, string tipo)
		{
			List<ColunaOracle> colunas = new List<ColunaOracle>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(_configSys.UsuarioRelatorio))
			{
				Comando comando = bancoDeDados.CriarComando(String.Format(@"alter table {0} add {1} {2}", tabela, coluna, tipo));

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		public void AddIndiceTxt(string tabela, string coluna, string idxName)
		{
			List<ColunaOracle> colunas = new List<ColunaOracle>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(_configSys.UsuarioRelatorio))
			{
				Comando comando = bancoDeDados.CriarComando(String.Format(@"create index {0} on {1}({2}) indextype is ctxsys.context parameters ('SYNC (ON COMMIT)')", idxName, tabela, coluna));

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}
	}
}