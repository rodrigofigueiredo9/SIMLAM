using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloIndicador.Data
{
	class RelatorioIndicadorDa
	{
		private string EsquemaBanco { get; set; }

		public RelatorioIndicadorDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal IndicadorPeriodoRelatorio RelatorioTituloIndicadores()
		{
			IndicadorPeriodoRelatorio titulos = new IndicadorPeriodoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Executa a querry de resultado

				comandtxt = String.Format(@"select 
				(select count(*) from {0}tab_titulo ta where ta.situacao in (3,6) and trunc(ta.data_vencimento) = trunc(sysdate)) hoje,
				(select count(*) from {0}tab_titulo ta where ta.situacao in (3,6) and trunc(ta.data_vencimento) 
				between trunc(sysdate - to_number(to_char(sysdate, 'D')) + 1) and trunc(sysdate + (7 - to_number(to_char(sysdate, 'D'))))) essasemana,
				(select count(*) from {0}tab_titulo ta where ta.situacao in (3,6) and trunc(ta.data_vencimento) 
				between to_date('01/' || to_char(sysdate, 'mm/yyyy')) and trunc(last_day(sysdate))) essemes,
				(select count(*) from {0}tab_titulo ta where ta.situacao in (3,6) and trunc(ta.data_vencimento) 
				between to_date('01/' || to_char(Add_months(sysdate, 1), 'mm/yyyy')) and trunc(last_day(add_months(sysdate, 1)))) proximomes from dual", EsquemaBanco);

				comando.DbCommand.CommandText = comandtxt;

				#endregion

				#region Adicionando os dados na classe de retorno

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						titulos = new IndicadorPeriodoRelatorio();
						titulos.Hoje = Convert.ToInt32(reader["hoje"]);
						titulos.EssaSemana = Convert.ToInt32(reader["essasemana"]);
						titulos.EsseMes = Convert.ToInt32(reader["essemes"]);
						titulos.ProximoMes = Convert.ToInt32(reader["proximomes"]);
					}

					reader.Close();
				}

				#endregion
			}

			return titulos;
		}

		internal IndicadorPeriodoRelatorio RelatorioTituloCondicionantesIndicadores()
		{
			IndicadorPeriodoRelatorio titulos = new IndicadorPeriodoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Executa a querry de resultado

				comandtxt = String.Format(@"select (select count(*) from {0}tab_titulo tat, {0}tab_titulo_condicionantes tc where tc.titulo = tat.id
				and tc.situacao = 2 and trunc(tc.data_vencimento) = trunc(sysdate)) hoje,
				(select count(*) from {0}tab_titulo tat, {0}tab_titulo_condicionantes tc where tc.titulo = tat.id
				and tc.situacao = 2 and trunc(tc.data_vencimento) between trunc(sysdate - to_number(to_char(sysdate, 'D')) + 1) and
				trunc(sysdate + (7 - to_number(to_char(sysdate, 'D'))))) essasemana,
				(select count(*) from {0}tab_titulo tat, {0}tab_titulo_condicionantes tc where tc.titulo = tat.id and tc.situacao = 2
				and trunc(tc.data_vencimento) between to_date('01/' || to_char(sysdate, 'mm/yyyy')) and trunc(last_day(sysdate))) essemes,
				(select count(*) from {0}tab_titulo tat, {0}tab_titulo_condicionantes tc where tc.titulo = tat.id and tc.situacao = 2
				and trunc(tc.data_vencimento) between to_date('01/' || to_char(Add_months(sysdate, 1), 'mm/yyyy')) and
				trunc(last_day(add_months(sysdate, 1)))) proximomes from dual", EsquemaBanco);

				comando.DbCommand.CommandText = comandtxt;

				#endregion

				#region Adicionando os dados na classe de retorno

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						titulos = new IndicadorPeriodoRelatorio();
						titulos.Hoje = Convert.ToInt32(reader["hoje"]);
						titulos.EssaSemana = Convert.ToInt32(reader["essasemana"]);
						titulos.EsseMes = Convert.ToInt32(reader["essemes"]);
						titulos.ProximoMes = Convert.ToInt32(reader["proximomes"]);
					}

					reader.Close();
				}

				#endregion
			}

			return titulos;
		}
	}
}