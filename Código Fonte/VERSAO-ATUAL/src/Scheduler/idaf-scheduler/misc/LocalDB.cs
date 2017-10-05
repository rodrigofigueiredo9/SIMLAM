using log4net;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Reflection;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.misc
{
	internal static class LocalDB
	{
		static object lockObject = new object();

		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		internal static ItemFila PegarProximoItemFila(OracleConnection conn, string tipo)
		{
			lock (lockObject)
			{
				var schema = CarUtils.GetEsquemaInstitucional();
				ItemFila item = null;

				try
				{
					using (
						var cmd =
							new OracleCommand(@"update " + schema + @".tab_scheduler_fila set data_criacao = current_timestamp
								where id = (select min(id) from " + schema + @".tab_scheduler_fila where tipo = :tipo and (id = 34933 ) and data_criacao is null)
								returning id, requisitante, requisicao, empreendimento 
								into :id, :requisitante, :requisicao, :empreendimento", conn))  //OR ID = 34934  OR ID = 34932 OR ID = 34931
					{
						cmd.Parameters.Add(new OracleParameter("tipo", tipo));
						OracleParameter paramId = cmd.Parameters.Add(new OracleParameter("id", OracleDbType.Int32, System.Data.ParameterDirection.Output));
						OracleParameter paramRequisitante = cmd.Parameters.Add(new OracleParameter("requisitante", OracleDbType.Int32, System.Data.ParameterDirection.Output));
						OracleParameter paramRequisicao = cmd.Parameters.Add(new OracleParameter("requisicao", OracleDbType.Varchar2, 500) { Direction = System.Data.ParameterDirection.Output });
						OracleParameter paramEmpreendimento = cmd.Parameters.Add(new OracleParameter("empreendimento", OracleDbType.Int32, System.Data.ParameterDirection.Output));

						if (cmd.ExecuteNonQuery() > 0)
						{
							item = new ItemFila();

							item.Id = Convert.ToInt32(paramId.Value.ToString());
							item.Requisitante = Convert.ToInt32(paramRequisitante.Value.ToString());
							item.Empreendimento = Convert.ToInt32(paramEmpreendimento.Value.ToString());
							item.Requisicao = paramRequisicao.Value.ToString();
						}
					}
				}
				catch (Exception exception)
				{
					Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
				}

				return item;
			}
		}

		internal static ItemFila PegarItemFilaPorId(OracleConnection conn, int id)
		{
			var schema = CarUtils.GetEsquemaInstitucional();
			var item = new ItemFila();

			try
			{
				using (
					var cmd =
						new OracleCommand(
							"SELECT * FROM " + schema + ".TAB_SCHEDULER_FILA WHERE id = :id",
							conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", id));

					using (var dr = cmd.ExecuteReader())
					{
						while (dr.Read())
						{
							item = new ItemFila
							{
								Id = Convert.ToInt32(dr["id"]),
								Requisitante = Convert.ToInt32(dr["requisitante"]),
								Empreendimento = Convert.ToInt32(dr["empreendimento"]),
								Requisicao = Convert.ToString(dr["requisicao"])
							};
						}
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			return item;
		}

		internal static string PegarResultadoItemFilaPorRequisitante(OracleConnection conn, int id, string tipo)
		{
			var schema = CarUtils.GetEsquemaInstitucional();
			var item = new ItemFila();

			try
			{
				using (
					var cmd =
						new OracleCommand(
							"SELECT * FROM " + schema + ".TAB_SCHEDULER_FILA WHERE requisitante = :id and tipo = :tipo",
							conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", id));
					cmd.Parameters.Add(new OracleParameter("tipo", tipo));

					using (var dr = cmd.ExecuteReader())
					{
						if (dr.Read())
						{
							return Convert.ToString(dr["resultado"]);
						}
					}
				}
				
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			return string.Empty;
		}

		internal static void MarcarItemFilaIniciado(OracleConnection conn, int id)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			try
			{
				using (
					var cmd =
						new OracleCommand(
							"UPDATE " + schema + ".TAB_SCHEDULER_FILA SET data_criacao = CURRENT_TIMESTAMP WHERE id=:id",
							conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", id));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}
		}

		internal static void MarcarItemFilaTerminado(OracleConnection conn, int id, bool sucesso, string resultado)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			try
			{
				using (
					var cmd =
						new OracleCommand(
							"UPDATE " + schema +
							".TAB_SCHEDULER_FILA SET data_conclusao = CURRENT_TIMESTAMP, sucesso = :sucesso, resultado = :resultado WHERE id=:id",
							conn))
				{
					cmd.Parameters.Add(new OracleParameter("sucesso", ((sucesso) ? "verdadeiro" : "falso")));
					cmd.Parameters.Add(new OracleParameter("resultado", resultado));
					cmd.Parameters.Add(new OracleParameter("id", id));


					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}
		}

		internal static void AdicionarItemFila(OracleConnection conn, string tipo, int requisitante, string requisicao, int empreendimento)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			try
			{
				using (
					var cmd =
						new OracleCommand(
							"INSERT INTO " + schema + ".TAB_SCHEDULER_FILA (id, tipo, requisitante, requisicao, empreendimento) values (" +
							schema + ".SEQ_TAB_SCHEDULER_FILA.nextval, :tipo, :requisitante, :requisicao, :empreendimento)",
							conn))
				{
					cmd.Parameters.Add(new OracleParameter("tipo", tipo));
					cmd.Parameters.Add(new OracleParameter("requisitante", requisitante));
					cmd.Parameters.Add(new OracleParameter("requisicao", requisicao));
					cmd.Parameters.Add(new OracleParameter("empreendimento", empreendimento));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}
		}

		internal static void ResetarFila()
		{
			using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
			{
				conn.Open();

				var schema = CarUtils.GetEsquemaInstitucional();

				using (var cmd = new OracleCommand("UPDATE " + schema + ".TAB_SCHEDULER_FILA SET data_criacao = null WHERE data_conclusao is null", conn))
				{
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}