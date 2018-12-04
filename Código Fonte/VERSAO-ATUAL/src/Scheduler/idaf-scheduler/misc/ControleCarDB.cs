using log4net;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Tecnomapas.EtramiteX.Scheduler.models.misc;
using Tecnomapas.EtramiteX.Scheduler.jobs.Class;

namespace Tecnomapas.EtramiteX.Scheduler.misc
{
	internal static class ControleCarDB
	{
		public const int SITUACAO_ENVIO_AGUARDANDO_ENVIO = 1;
		public const int SITUACAO_ENVIO_GERANDO_ARQUIVO = 2;
		public const int SITUACAO_ENVIO_ARQUIVO_GERADO = 3;
		public const int SITUACAO_ENVIO_ARQUIVO_REPROVADO = 4;
		public const int SITUACAO_ENVIO_ENVIANDO = 5;
		public const int SITUACAO_ENVIO_ARQUIVO_ENTREGUE = 6;
		public const int SITUACAO_ENVIO_ERRO = 7;
		public const int SITUACAO_ENVIO_ARQUIVO_RETIFICADO = 8;

		public const int SITUACAO_SOLICITACAO_VALIDO = 2;
		public const int SITUACAO_SOLICITACAO_INVALIDO = 3;
		public const int SITUACAO_SOLICITACAO_SUBSTITUIDO_TITULO_CAR = 5;
		public const int SITUACAO_SOLICITACAO_PENDENTE = 6;

		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public static int InserirControleSICAR(OracleConnection conn, ItemFila itemFila, string arquivoCar)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var requisicao = JsonConvert.DeserializeObject<RequisicaoJobCar>(itemFila.Requisicao);

			var tid = Blocos.Data.GerenciadorTransacao.ObterIDAtual();

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("INSERT INTO " + schema + ".TAB_CONTROLE_SICAR ");
			sqlBuilder.Append("(id,tid,empreendimento,empreendimento_tid,solicitacao_car,solicitacao_car_tid,situacao_envio,");
			sqlBuilder.Append("data_gerado,arquivo,solicitacao_car_esquema)");
			sqlBuilder.Append(" VALUES ");
			sqlBuilder.Append("(" + schema + ".SEQ_TAB_CONTROLE_SICAR.nextval,:tid,:empreendimento,:empreendimento_tid,");
			sqlBuilder.Append(
				":solicitacao_car,:solicitacao_car_tid,:situacao_envio,CURRENT_TIMESTAMP,:arquivo,:solicitacao_car_esquema) RETURNING id INTO :novo_id");

			var novoId = 0;

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("tid", tid));
					cmd.Parameters.Add(new OracleParameter("empreendimento", requisicao.empreendimento));
					cmd.Parameters.Add(new OracleParameter("empreendimento_tid", requisicao.empreendimento_tid));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", requisicao.solicitacao_car));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car_tid", requisicao.solicitacao_car_tid));
					cmd.Parameters.Add(new OracleParameter("situacao_envio", SITUACAO_ENVIO_ARQUIVO_GERADO));
					cmd.Parameters.Add(new OracleParameter("arquivo", arquivoCar));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car_esquema",
						((requisicao.origem == RequisicaoJobCar.INSTITUCIONAL ? 1 : 2))));
					cmd.Parameters.Add(new OracleParameter("novo_id", OracleDbType.Decimal, ParameterDirection.ReturnValue));

					cmd.ExecuteNonQuery();

					novoId = Convert.ToInt32(cmd.Parameters["novo_id"].Value.ToString());
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			//Inserir no Histórico
			InserirHistoricoControleCar(conn, requisicao, tid, null);

			return novoId;
		}

		public static int AtualizarControleSICAR(OracleConnection conn, MensagemRetorno resultado, RequisicaoJobCar requisicao, int situacaoEnvio, string tid, string arquivoCar = "", string tipo = "", bool catchEnviar = false, string codigoProtocolo = "")
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var item = ObterItemControleCar(conn, requisicao);

			var pendencias = "";
			var condicao = "Aguardando Análise";
			var mensagensDeResposta = String.Empty;
			var mensagemErro = "";

			if (resultado == null)
			{
				resultado = new MensagemRetorno { mensagensResposta = new List<string>() };
			}
			else
			{
				if (resultado.mensagensResposta == null)
				{
					mensagensDeResposta = "Erro de conexão com o SICAR, será feita uma nova tentativa ; ";
					situacaoEnvio = SITUACAO_ENVIO_ARQUIVO_REPROVADO;
				}
				else
				{
					if (resultado.codigoResposta != MensagemRetorno.CodigoRespostaSucesso)
					{
						if (resultado.mensagensResposta.Count > 1 ||
							(tipo.Equals("gerar-car") && resultado.mensagensResposta.Count >= 1) ||
							 resultado.codigoResposta == 400 ||
							 resultado.codigoResposta == 500)
						{
							resultado.mensagensResposta = TratandoMensagens(conn, resultado.mensagensResposta);
							pendencias = resultado.mensagensResposta.Aggregate("", (current, resposta) => current + (resposta + " ; "));
							situacaoEnvio = SITUACAO_ENVIO_ARQUIVO_REPROVADO;
						}
						else
						{
							situacaoEnvio = SITUACAO_ENVIO_ARQUIVO_ENTREGUE;
						}
						//pendencias = pendencias.Replace("O arquivo especificado contém informações inválidas.;", "");
					}
					foreach (var men in resultado.mensagensResposta)
					{
						mensagensDeResposta = String.Concat(mensagensDeResposta, men);
						mensagensDeResposta = String.Concat(mensagensDeResposta, "  ;  ");

						if (men.Contains("sobreposição"))
						{
							mensagemErro = men;
						}
					}
				}
			}

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("UPDATE " + schema + ".TAB_CONTROLE_SICAR SET ");
			sqlBuilder.Append("tid = :tid,");
			sqlBuilder.Append("situacao_envio = :situacao_envio,");
			sqlBuilder.Append("chave_protocolo = :chave_protocolo,");

			if (tipo.Equals("gerar-car"))
				sqlBuilder.Append("data_gerado = CURRENT_TIMESTAMP,");
			//if (situacaoEnvio == SITUACAO_ENVIO_ARQUIVO_ENTREGUE)
			else
				sqlBuilder.Append("data_envio = CURRENT_TIMESTAMP,");
			if (arquivoCar != "")
				sqlBuilder.Append("arquivo = '" + arquivoCar + "',");

			sqlBuilder.Append("pendencias = :pendencias,");
			if (!String.IsNullOrWhiteSpace(resultado.codigoImovel))
				sqlBuilder.Append("codigo_imovel = :codigo_imovel,");
			if (!String.IsNullOrWhiteSpace(resultado.urlReciboInscricao))
				sqlBuilder.Append("url_recibo = :url_recibo,");
			sqlBuilder.Append("status_sicar = :status_sicar,");
			sqlBuilder.Append("condicao = :condicao,");
			if(!String.IsNullOrWhiteSpace(codigoProtocolo))
				sqlBuilder.Append("chave_protocolo_enviado = :chave_protocolo_enviado,");

			sqlBuilder.Append("CODIGO_RESPOSTA = :codigo_resposta, ");
			sqlBuilder.Append("CODIGO_IMOVEL_MASC = :codigo_imovel_masc, ");
			sqlBuilder.Append("MENSAGEM_RESPOSTA = :mensagem_resposta ");
			sqlBuilder.Append(" WHERE id = :id");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("tid", tid));
					cmd.Parameters.Add(new OracleParameter("situacao_envio", situacaoEnvio));
					cmd.Parameters.Add(new OracleParameter("chave_protocolo", resultado.protocoloImovel));
					if (catchEnviar)
					{
						string pendencia = "Falha na integração, comunique o administrador do sistema";
						cmd.Parameters.Add(new OracleParameter("pendencias", pendencia));
					}
					else
						cmd.Parameters.Add(new OracleParameter("pendencias", mensagensDeResposta));

					if (!String.IsNullOrWhiteSpace(resultado.codigoImovel))
						cmd.Parameters.Add(new OracleParameter("codigo_imovel", resultado.codigoImovel));
					if (!String.IsNullOrWhiteSpace(resultado.urlReciboInscricao))
						cmd.Parameters.Add(new OracleParameter("url_recibo", resultado.urlReciboInscricao));
					cmd.Parameters.Add(new OracleParameter("status_sicar", "IN"));
					cmd.Parameters.Add(new OracleParameter("condicao", condicao));
					if (!String.IsNullOrWhiteSpace(codigoProtocolo))
						cmd.Parameters.Add(new OracleParameter("chave_protocolo_enviado", codigoProtocolo));

					cmd.Parameters.Add(new OracleParameter("codigo_resposta", resultado.codigoResposta));
					cmd.Parameters.Add(new OracleParameter("codigo_imovel_masc", resultado.codigoImovelComMascara));

					cmd.Parameters.Add(new OracleParameter("mensagem_resposta", mensagensDeResposta));
					cmd.Parameters.Add(new OracleParameter("id", item?.id));

					cmd.ExecuteNonQuery();
				}
				//Inserir no Histórico
				InserirHistoricoControleCar(conn, requisicao, tid, resultado);

				if(!String.IsNullOrWhiteSpace(mensagemErro) && item != null)
					VerificarListaCodigoImovel(conn, schema, mensagemErro, item.solicitacao_car, item.empreendimento, requisicao.origem, requisicao, tid);
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			if (item == null) return 0;

			return item.id;
		}

		public static int AtualizarControleSICarRetificacao(OracleConnection conn, MensagemRetorno resultado, ItemControleCar item, int situacaoEnvio, int solicitacaoRetificadora, string tid, string arquivoCar = "", string tipo = "")
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var pendencias = " Solicitação retificada pela solicitação : " + solicitacaoRetificadora;
			var condicao = "Aguardando Análise";
			var mensagensDeResposta = pendencias;


			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("UPDATE " + schema + ".TAB_CONTROLE_SICAR SET ");
			//sqlBuilder.Append("tid = :tid,");
			sqlBuilder.Append("situacao_envio = :situacao_envio,");
			sqlBuilder.Append("chave_protocolo = :chave_protocolo,");

			if (tipo.Equals("gerar-car"))
				sqlBuilder.Append("data_gerado = CURRENT_TIMESTAMP,");
			//if (situacaoEnvio == SITUACAO_ENVIO_ARQUIVO_ENTREGUE)
			else
				sqlBuilder.Append("data_envio = CURRENT_TIMESTAMP,");
			if (arquivoCar != "")
				sqlBuilder.Append("arquivo = '" + arquivoCar + "',");

			sqlBuilder.Append("pendencias = :pendencias,");
			if (!String.IsNullOrWhiteSpace(resultado.codigoImovel))
				sqlBuilder.Append("codigo_imovel = :codigo_imovel,");
			if (!String.IsNullOrWhiteSpace(resultado.urlReciboInscricao))
				sqlBuilder.Append("url_recibo = :url_recibo,");
			sqlBuilder.Append("status_sicar = :status_sicar,");
			sqlBuilder.Append("condicao = :condicao,");

			sqlBuilder.Append("CODIGO_RESPOSTA = :codigo_resposta, ");
			sqlBuilder.Append("CODIGO_IMOVEL_MASC = :codigo_imovel_masc, ");
			sqlBuilder.Append("MENSAGEM_RESPOSTA = :mensagem_resposta ");
			sqlBuilder.Append(" WHERE solicitacao_car = :id");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					//cmd.Parameters.Add(new OracleParameter("tid", item.solicitacao_car_anterior_tid));
					cmd.Parameters.Add(new OracleParameter("situacao_envio", SITUACAO_ENVIO_ARQUIVO_RETIFICADO));
					cmd.Parameters.Add(new OracleParameter("chave_protocolo", resultado.protocoloImovel));
					cmd.Parameters.Add(new OracleParameter("pendencias", pendencias));
					if (!String.IsNullOrWhiteSpace(resultado.codigoImovel))
						cmd.Parameters.Add(new OracleParameter("codigo_imovel", resultado.codigoImovel));
					if (!String.IsNullOrWhiteSpace(resultado.urlReciboInscricao))
						cmd.Parameters.Add(new OracleParameter("url_recibo", resultado.urlReciboInscricao));
					cmd.Parameters.Add(new OracleParameter("status_sicar", "IN"));
					cmd.Parameters.Add(new OracleParameter("condicao", condicao));

					cmd.Parameters.Add(new OracleParameter("codigo_resposta", resultado.codigoResposta));
					cmd.Parameters.Add(new OracleParameter("codigo_imovel_masc", resultado.codigoImovelComMascara));

					cmd.Parameters.Add(new OracleParameter("mensagem_resposta", mensagensDeResposta));
					//cmd.Parameters.Add(new OracleParameter("mensagem_resposta", resultado.mensagensResposta));                    
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", item.solicitacao_car_anterior));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			RequisicaoJobCar requisicao = new RequisicaoJobCar();
			requisicao.empreendimento = item.empreendimento;
			requisicao.empreendimento_tid = item.empreendimento_tid;
			requisicao.solicitacao_car = item.solicitacao_car_anterior;
			requisicao.solicitacao_car_tid = item.solicitacao_car_anterior_tid;

			//Inserir no Histórico
			InserirHistoricoControleCar(conn, requisicao, tid, resultado);
			if (item == null)
			{
				return 0;
			}
			return item.id;

		}

		private static void InserirHistoricoControleCar(OracleConnection conn, RequisicaoJobCar requisicao, string tid, MensagemRetorno resultado)
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var item = ObterItemControleCar(conn, requisicao);

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("INSERT INTO " + schema + ".HST_CONTROLE_SICAR ");
			sqlBuilder.Append("(id,tid,empreendimento,empreendimento_tid,solicitacao_car,solicitacao_car_tid,situacao_envio,");
			sqlBuilder.Append("chave_protocolo,data_gerado,data_envio,arquivo,pendencias,codigo_imovel,");
			sqlBuilder.Append("url_recibo,status_sicar,condicao,solicitacao_car_esquema,data_execucao,");
			sqlBuilder.Append("CODIGO_RESPOSTA, CODIGO_IMOVEL_MASC, MENSAGEM_RESPOSTA)");
			sqlBuilder.Append(" values ");
			sqlBuilder.Append("(" + schema + ".SEQ_HST_CONTROLE_SICAR.nextval,:tid,:empreendimento,:empreendimento_tid,");
			sqlBuilder.Append(":solicitacao_car,:solicitacao_car_tid,:situacao_envio,:chave_protocolo,");
			sqlBuilder.Append(":data_gerado,:data_envio,:arquivo,:pendencias,:codigo_imovel,");
			sqlBuilder.Append(":url_recibo,:status_sicar,:condicao,:solicitacao_car_esquema,CURRENT_TIMESTAMP, :CODIGO_RESPOSTA, :CODIGO_IMOVEL_MASC, :MENSAGEM_RESPOSTA)");

			var mensagensDeResposta = String.Empty;

			if (resultado.mensagensResposta == null)
			{
				mensagensDeResposta = "Erro de conexão com o SICAR, será feita uma nova tentativa ; ";
				//situacaoEnvio = SITUACAO_ENVIO_ARQUIVO_REPROVADO;
			}
			else
			{
				foreach (var men in resultado.mensagensResposta)
				{
					mensagensDeResposta = String.Concat(mensagensDeResposta, men);
					mensagensDeResposta = String.Concat(mensagensDeResposta, "  ;  ");
				}
			}
			try
			{
				if (item == null) throw new Exception("ITEM NULO !!!");

				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("tid", tid));
					cmd.Parameters.Add(new OracleParameter("empreendimento", item.empreendimento));
					cmd.Parameters.Add(new OracleParameter("empreendimento_tid", item.empreendimento_tid));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", item.solicitacao_car));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car_tid", tid));//item.solicitacao_car_tid));
					cmd.Parameters.Add(new OracleParameter("situacao_envio", item.situacao_envio));
					cmd.Parameters.Add(new OracleParameter("chave_protocolo", item.chave_protocolo));
					cmd.Parameters.Add(new OracleParameter("data_gerado", item.data_gerado));
					cmd.Parameters.Add(new OracleParameter("data_envio", item.data_envio));
					cmd.Parameters.Add(new OracleParameter("arquivo", item.arquivo));
					cmd.Parameters.Add(new OracleParameter("pendencias", item.pendencias));
					cmd.Parameters.Add(new OracleParameter("codigo_imovel", item.codigo_imovel));
					cmd.Parameters.Add(new OracleParameter("url_recibo", item.url_recibo));
					cmd.Parameters.Add(new OracleParameter("status_sicar", item.status_sicar));
					cmd.Parameters.Add(new OracleParameter("condicao", item.condicao));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car_esquema", item.solicitacao_car_esquema));

					cmd.Parameters.Add(new OracleParameter("codigo_resposta", resultado.codigoResposta));
					cmd.Parameters.Add(new OracleParameter("codigo_imovel_masc", resultado.codigoImovelComMascara));
					cmd.Parameters.Add(new OracleParameter("mensagem_resposta", mensagensDeResposta));
					//cmd.Parameters.Add(new OracleParameter("mensagem_resposta", resultado.mensagensResposta));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Requisicao: " + (requisicao != null ? JsonConvert.SerializeObject(requisicao) : " IS NULL"), exception);
			}
		}

		internal static string ObterDataSolicitacao(OracleConnection conn, int solicitacaoCar, string origem)
		{
			var tabela = (origem == RequisicaoJobCar.INSTITUCIONAL)
				? "HST_CAR_SOLICITACAO"
				: "HST_CAR_SOLICITACAO_CRED";

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("select to_char(min(c.data_execucao), 'dd/mm/yyyy hh24:mm') data_execucao from ");
			sqlBuilder.Append(tabela);
			sqlBuilder.Append(" c where c.solicitacao_id = :solicitacao_car");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", solicitacaoCar));

					using (var dr = cmd.ExecuteReader())
					{
						if (dr.Read())
							return dr.GetValue<string>("data_execucao");
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			return string.Empty;
		}

		public static ItemControleCar ObterItemControleCar(OracleConnection conn, RequisicaoJobCar requisicao)
		{
			var schema = CarUtils.GetEsquemaInstitucional();
			var item = new ItemControleCar();

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("SELECT T.ID, T.TID, T.EMPREENDIMENTO, T.EMPREENDIMENTO_TID,T.SOLICITACAO_CAR, T.SOLICITACAO_CAR_TID,  ");
			sqlBuilder.Append("T.SITUACAO_ENVIO, T.CHAVE_PROTOCOLO,T.DATA_GERADO,T.DATA_ENVIO,T.PENDENCIAS,T.CODIGO_IMOVEL, ");
			sqlBuilder.Append("T.URL_RECIBO,T.STATUS_SICAR,T.CONDICAO,T.SOLICITACAO_CAR_ESQUEMA,NVL(T.SOLICITACAO_PASSIVO, 0)SOLICITACAO_PASSIVO, ");
			sqlBuilder.Append("NVL(T.SOLICITACAO_SITUACAO_APROVADO, 2)SOLICITACAO_SITUACAO_APROVADO FROM " + schema + ".TAB_CONTROLE_SICAR t WHERE ");
			sqlBuilder.Append("t.empreendimento = :empreendimento /*AND t.empreendimento_tid = :empreendimento_tid */");
			sqlBuilder.Append("AND t.solicitacao_car = :solicitacao_car /*AND t.solicitacao_car_tid = :solicitacao_car_tid */");
			sqlBuilder.Append("AND rownum = 1 ORDER BY id DESC");

			try
			{
				if (conn.State == ConnectionState.Broken || conn.State == ConnectionState.Closed)
					Log.Error("ObterItemControleCar: Conexão fechada ou quebrada.");

				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("empreendimento", requisicao.empreendimento));
					//cmd.Parameters.Add(new OracleParameter("empreendimento_tid", requisicao.empreendimento_tid));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", requisicao.solicitacao_car));
					//cmd.Parameters.Add(new OracleParameter("solicitacao_car_tid", requisicao.solicitacao_car_tid));

					using (var dr = cmd.ExecuteReader())
					{
						if (!dr.Read())
						{
							if(dr.IsClosed)
								Log.Error(String.Concat("ObterItemControleCar: dr is closed. ", " - Estado da conexão: ", conn.State.ToString()));
							else if(!dr.HasRows)
								Log.Error(String.Concat("ObterItemControleCar: consulta não retornou resultado. empreendimento: ", requisicao.empreendimento,
									" - solicitacao_car: ", requisicao.solicitacao_car, " - Estado da conexão: ", conn.State.ToString()));
							else
								Log.Error(String.Concat("ObterItemControleCar: não foi possível ler a consulta. ", " - Estado da conexão: ", conn.State.ToString()));

							return null;
						}
						

						item = new ItemControleCar()
						{
							id = dr.GetValue<Int32>("id"),
							tid = dr.GetValue<string>("tid"),
							empreendimento = dr.GetValue<Int32>("empreendimento"),
							empreendimento_tid = dr.GetValue<string>("empreendimento_tid"),
							solicitacao_car = dr.GetValue<Int32>("solicitacao_car"),
							solicitacao_car_tid = dr.GetValue<string>("solicitacao_car_tid"),
							situacao_envio = dr.GetValue<int>("situacao_envio"),
							chave_protocolo = dr.GetValue<string>("chave_protocolo"),
							data_gerado = dr.GetValue<DateTime>("data_gerado"),
							data_envio = dr.GetValue<DateTime>("data_envio"),
							pendencias = dr.GetValue<string>("pendencias"),
							codigo_imovel = dr.GetValue<string>("codigo_imovel"),
							url_recibo = dr.GetValue<string>("url_recibo"),
							status_sicar = dr.GetValue<string>("status_sicar"),
							condicao = dr.GetValue<string>("condicao"),
							solicitacao_car_esquema = dr.GetValue<int>("solicitacao_car_esquema"),
							solicitacao_passivo = Convert.ToInt32(dr["solicitacao_passivo"]),
							solicitacao_situacao_aprovado = Convert.ToInt32(dr["solicitacao_situacao_aprovado"]),
						};
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			return item;
		}

		public static ItemControleCar ObterItemControleCarRetificacao(OracleConnection conn, RequisicaoJobCar requisicao)
		{
			var item = new ItemControleCar();
			var sqlBuilder = new StringBuilder();

			try
			{
				using (var cmd = new OracleCommand(@"SELECT ID, TID, EMPREENDIMENTO, EMPREENDIMENTO_TID, SOLICITACAO_CAR, SOLICITACAO_CAR_TID, 
                                                    SOLICITACAO_CAR_ANTERIOR, SITUACAO_ENVIO, CHAVE_PROTOCOLO, DATA_GERADO, DATA_ENVIO, PENDENCIAS, 
                                                    CODIGO_IMOVEL, SOLICITACAO_CAR_ESQUEMA, SOLICITACAO_CAR_ANT_ESQUEMA, SOLICITACAO_CAR_ANTERIOR_TID 
                                                    FROM TAB_CONTROLE_SICAR WHERE SOLICITACAO_CAR = :solicitacao", conn))
				{
					cmd.Parameters.Add(new OracleParameter("solicitacao", requisicao.solicitacao_car));

					using (var dr = cmd.ExecuteReader())
					{
						if (!dr.Read())
							return null;

						item = new ItemControleCar()
						{
							id = dr.GetValue<Int32>("id"),
							tid = dr.GetValue<string>("tid"),
							empreendimento = dr.GetValue<Int32>("empreendimento"),
							empreendimento_tid = dr.GetValue<string>("empreendimento_tid"),
							solicitacao_car = dr.GetValue<Int32>("solicitacao_car"),
							solicitacao_car_tid = dr.GetValue<string>("solicitacao_car_tid"),
							solicitacao_car_anterior = dr.GetValue<Int32>("solicitacao_car_anterior"),
							situacao_envio = dr.GetValue<int>("situacao_envio"),
							chave_protocolo = dr.GetValue<string>("chave_protocolo"),
							data_gerado = dr.GetValue<DateTime>("data_gerado"),
							data_envio = dr.GetValue<DateTime>("data_envio"),
							pendencias = dr.GetValue<string>("pendencias"),
							codigo_imovel = dr.GetValue<string>("codigo_imovel"),
							solicitacao_car_esquema = dr.GetValue<int>("solicitacao_car_esquema"),
							solicitacao_car_anterior_esquema = dr.GetValue<Int32>("solicitacao_car_ant_esquema"),
							solicitacao_car_anterior_tid = dr.GetValue<string>("solicitacao_car_anterior_tid"),
						};
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			return item;
		}

		public static  CARSolicita ObterCarSolicitacaoInst(OracleConnection conn, int id)
		{
			var schema = CarUtils.GetEsquemaInstitucional();
			var item = new CARSolicita();
			var tabela = ("a" == RequisicaoJobCar.INSTITUCIONAL)
				? "HST_CAR_SOLICITACAO"
				: "HST_CAR_SOLICITACAO_CRED";

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append(@"SELECT S.ID, S.TID, s.SITUACAO_DATA, s.DATA_EMISSAO, F.ID AUTOR_ID, F.TID AUTOR_TID, S.SITUACAO, LV.TEXTO SITUACAO_TEXTO, s.MOTIVO
								FROM IDAF.TAB_CAR_SOLICITACAO					s
								INNER JOIN IDAF.TAB_FUNCIONARIO					F	ON s.AUTOR = F.ID
								INNER JOIN LOV_CAR_SOLICITACAO_SITUACAO		LV  ON s.SITUACAO = LV.ID   
								WHERE S.ID = :id ");
			
			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", id));

					using (var dr = cmd.ExecuteReader())
					{
						if (dr.Read())
						{
							item.Id = dr.GetValue<Int32>("id");
							item.Tid = dr.GetValue<string>("tid");
							item.DataEmissao.Data = dr.GetValue<DateTime>("DATA_EMISSAO");
							item.DataSituacao.Data = dr.GetValue<DateTime>("SITUACAO_DATA");
							item.AutorId = dr.GetValue<Int32>("AUTOR_ID");
							item.AutorTid = dr.GetValue<string>("AUTOR_TID");
							item.SituacaoId = dr.GetValue<int>("SITUACAO");
							item.SituacaoTexto = dr.GetValue<string>("SITUACAO_TEXTO");
							item.Motivo = dr.GetValue<string>("MOTIVO");
						} else
							return null;
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			return item;
		}

		public static  CARSolicita ObterCarSolicitacaoCred(OracleConnection conn, int id)
		{
			var item = new CARSolicita();

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append(@"SELECT S.ID, S.TID, S.DATA_EMISSAO, S.SITUACAO_DATA, C.ID CREDENCIADO, C.TID CREDENCIADO_TID, S.MOTIVO, S.SITUACAO, LV.TEXTO SITUACAO_TEXTO
								FROM TAB_CAR_SOLICITACAO S
								INNER JOIN TAB_CREDENCIADO C  ON S.CREDENCIADO = C.ID
								INNER JOIN IDAF.LOV_CAR_SOLICITACAO_SITUACAO LV ON S.SITUACAO = LV.ID
								WHERE S.ID = :id");
			
			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", id));

					using (var dr = cmd.ExecuteReader())
					{
						if (dr.Read())
						{
							item.Id = dr.GetValue<Int32>("id");
							item.Tid = dr.GetValue<string>("tid");
							item.DataEmissao.Data = dr.GetValue<DateTime>("DATA_EMISSAO");
							item.DataSituacao.Data = dr.GetValue<DateTime>("SITUACAO_DATA");
							item.AutorId = dr.GetValue<Int32>("CREDENCIADO");
							item.AutorTid = dr.GetValue<string>("CREDENCIADO_TID");
							item.Motivo = dr.GetValue<string>("MOTIVO");
							item.SituacaoId = dr.GetValue<int>("SITUACAO");
							item.SituacaoTexto = dr.GetValue<string>("SITUACAO_TEXTO");
						} else
							return null;
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			return item;
		}

		internal static void AtualizarSolicitacaoCar(OracleConnection conn, RequisicaoJobCar requisicao, int situacao, string tid)
		{
			//var controleCAR = ObterItemControleCar(conn, requisicao);

			//if (situacao == ControleCarDB.SITUACAO_SOLICITACAO_VALIDO && controleCAR.solicitacao_passivo > 0)
			//{
			//	situacao = controleCAR.solicitacao_situacao_aprovado;
			//}	

			AtualizarSolicitacaoCar(conn, requisicao.origem, requisicao.solicitacao_car, situacao, tid);
		}

		public static void AtualizarSolicitacaoCar(OracleConnection conn, string origem, int solicitacaoId, int situacao, string tid)
		{
			var tabela = (origem == RequisicaoJobCar.INSTITUCIONAL)
				? "TAB_CAR_SOLICITACAO"
				: "TAB_CAR_SOLICITACAO_CRED";

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("UPDATE " + tabela + " SET ");
			sqlBuilder.Append("tid = :tid,");
			sqlBuilder.Append("situacao = :situacao,");
			sqlBuilder.Append("situacao_data = SYSDATE,");
			sqlBuilder.Append("situacao_anterior = situacao,");
			sqlBuilder.Append("situacao_anterior_data = situacao_data");
			sqlBuilder.Append(" WHERE id = :id");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("tid", tid));
					cmd.Parameters.Add(new OracleParameter("situacao", situacao));
					cmd.Parameters.Add(new OracleParameter("id", solicitacaoId));

					cmd.ExecuteNonQuery();
				}

				var strQuery = string.Empty;
				if (origem == RequisicaoJobCar.INSTITUCIONAL)
					strQuery = "begin LST_CONSULTA.carSolicitacaoTitulo(:id); end;";
				else
					strQuery = "begin LST_CONSULTA_CRED.carSolicitacao(:id); end;";

				using (var cmd = new OracleCommand(strQuery, conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", solicitacaoId));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			//Inserir no Histórico
			InserirHistoricoSolicitacaoCar(conn, origem, solicitacaoId);
		}

		public static void AtualizarSolicitacaoCarRetificacao(OracleConnection conn, int origem, int solicitacaoId, string tid)
		{
			var tabela = (origem == 1)
				? "TAB_CAR_SOLICITACAO"
				: "TAB_CAR_SOLICITACAO_CRED";

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("UPDATE " + tabela + " SET ");
			sqlBuilder.Append("tid = :tid,");
			sqlBuilder.Append("situacao = :situacao,");
			sqlBuilder.Append("situacao_data = SYSDATE,");
			sqlBuilder.Append("situacao_anterior = situacao,");
			sqlBuilder.Append("situacao_anterior_data = situacao_data,");
			sqlBuilder.Append("motivo = :motivo");
			sqlBuilder.Append(" WHERE id = :id");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("tid", tid));
					cmd.Parameters.Add(new OracleParameter("situacao", ControleCarDB.SITUACAO_SOLICITACAO_INVALIDO));
					cmd.Parameters.Add(new OracleParameter("motivo", "Arquivo CAR Retificado"));
					cmd.Parameters.Add(new OracleParameter("id", solicitacaoId));

					cmd.ExecuteNonQuery();
				}

				var strQuery = string.Empty;
				if (origem == 1)
					strQuery = "begin LST_CONSULTA.carSolicitacaoTitulo(:id); end;";
				else
					strQuery = "begin LST_CONSULTA_CRED.carSolicitacao(:id); end;";

				using (var cmd = new OracleCommand(strQuery, conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", solicitacaoId));

					cmd.ExecuteNonQuery();
				}


			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			if (origem == 1)
				InserirHistoricoSolicitacaoCarRetificacaoInst(conn, solicitacaoId);
			else
				InserirHistoricoSolicitacaoCarRetificacaoCred(conn, solicitacaoId);
		}

		private static void InserirHistoricoSolicitacaoCar(OracleConnection conn, string origem, int solicitacaoId)
		{
			var pacote = (origem == RequisicaoJobCar.INSTITUCIONAL) ? "HISTORICO" : "HISTORICO_CRED";

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append("BEGIN " + pacote + ".CARSOLICITACAO(");
			sqlBuilder.Append(":id,:acao, :executor_id, :executor_nome, :executor_login, :executor_tipo_id, :executor_tid);");
			sqlBuilder.Append("END;");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					var assembly = Assembly.GetExecutingAssembly();

					cmd.Parameters.Add(new OracleParameter("id", solicitacaoId));
					cmd.Parameters.Add(new OracleParameter("acao", 17));//alterarsituacao
					cmd.Parameters.Add(new OracleParameter("executor_id", 1/*sistema*/));
					cmd.Parameters.Add(new OracleParameter("executor_nome", assembly.ManifestModule.Name));
					cmd.Parameters.Add(new OracleParameter("executor_login", "IDAF_Scheduler"));
					cmd.Parameters.Add(new OracleParameter("executor_tipo_id", 1/*interno*/));
					cmd.Parameters.Add(new OracleParameter("executor_tid", assembly.ManifestModule.ModuleVersionId.ToString()));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}
		}

		private static void InserirHistoricoSolicitacaoCarRetificacaoInst(OracleConnection conn, int solicitacaoId)
		{
			CARSolicita car = ObterCarSolicitacaoInst(conn, solicitacaoId);

			var sqlBuilder = new StringBuilder();
			sqlBuilder.Append(@"insert into hst_car_solicitacao
			( id, solicitacao_id, tid, numero, data_emissao, situacao_id,	situacao_texto, protocolo_id,	protocolo_tid,	requerimento_id, requerimento_tid,	
			protocolo_selecionado_id, protocolo_selecionado_tid,	atividade_id,	atividade_tid, empreendimento_id,	empreendimento_tid, declarante_id, declarante_tid, 
			autor_id, autor_tid, situacao_data, situacao_anterior_data, situacao_anterior_id, situacao_anterior_texto,	motivo,	dominialidade_id, dominialidade_tid,	projeto_geo_id, projeto_geo_tid,
			executor_id, executor_tid,	executor_nome, executor_login,	executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao)
			(
			SELECT seq_hst_car_solicitacao.nextval, SOLICITACAO_ID, TID, NUMERO, :data_emissao, :situacao, :situacao_texto, PROTOCOLO_ID, PROTOCOLO_TID, REQUERIMENTO_ID, REQUERIMENTO_TID,
			PROTOCOLO_SELECIONADO_ID, PROTOCOLO_SELECIONADO_TID, ATIVIDADE_ID, ATIVIDADE_TID, EMPREENDIMENTO_ID, EMPREENDIMENTO_TID, DECLARANTE_ID, DECLARANTE_TID, 
			:autor_id, :autor_tid, :situacao_data, SITUACAO_DATA, SITUACAO_ID, SITUACAO_TEXTO, :motivo, DOMINIALIDADE_ID, DOMINIALIDADE_TID, PROJETO_GEO_ID, PROJETO_GEO_TID, 
			:executor_id, :executor_tid, :executor_nome, :executor_login, :executor_tipo_id, executor_tipo_texto, :acao_executada, systimestamp
			FROM HST_CAR_SOLICITACAO WHERE ID IN (
			SELECT MAX(ID) FROM HST_CAR_SOLICITACAO WHERE SOLICITACAO_ID = :id_solicitacao AND SITUACAO_ID = 2 ))");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					var assembly = Assembly.GetExecutingAssembly();

					cmd.Parameters.Add(new OracleParameter("data_emissao", car.DataEmissao.Data));
					cmd.Parameters.Add(new OracleParameter("situacao", car.SituacaoId));
					cmd.Parameters.Add(new OracleParameter("situacao_texto", car.SituacaoTexto));
					cmd.Parameters.Add(new OracleParameter("autor_id", car.AutorId));
					cmd.Parameters.Add(new OracleParameter("autor_tid", car.AutorTid));
					cmd.Parameters.Add(new OracleParameter("situacao_data", car.DataSituacao.Data));
					cmd.Parameters.Add(new OracleParameter("motivo", car.Motivo));
					cmd.Parameters.Add(new OracleParameter("executor_id", 1/*sistema*/));
					cmd.Parameters.Add(new OracleParameter("executor_tid", assembly.ManifestModule.ModuleVersionId.ToString()));
					cmd.Parameters.Add(new OracleParameter("executor_nome", assembly.ManifestModule.Name));
					cmd.Parameters.Add(new OracleParameter("executor_login", "IDAF_Scheduler"));
					cmd.Parameters.Add(new OracleParameter("executor_tipo_id", 1/*interno*/));
					cmd.Parameters.Add(new OracleParameter("acao_executada", 17));//alterarsituacao

					cmd.Parameters.Add(new OracleParameter("id_solicitacao", solicitacaoId));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}
		}

		private static void InserirHistoricoSolicitacaoCarRetificacaoCred(OracleConnection conn, int solicitacaoId)
		{
			try
			{
				using (var connection = new OracleConnection(CarUtils.GetBancoCredenciado()))
				{
					connection.Open();

					CARSolicita car = ObterCarSolicitacaoCred(connection, solicitacaoId);

					var sqlBuilder = new StringBuilder();
					sqlBuilder.Append(@"insert into hst_car_solicitacao r
							  (id, solicitacao_id, tid, credenciado_id, credenciado_tid, numero, data_emissao, situacao_id, situacao_texto,
							   projeto_digital_id, projeto_digital_tid, requerimento_id, requerimento_tid, atividade_id, atividade_tid,
							   empreendimento_id, empreendimento_tid, declarante_id, declarante_tid, situacao_data, situacao_anterior_data,
							   situacao_anterior_id, situacao_anterior_texto, motivo, dominialidade_id, dominialidade_tid,
							   projeto_geo_id, projeto_geo_tid, executor_id, executor_tid, executor_nome, executor_login,
							   executor_tipo_id, executor_tipo_texto, acao_executada, data_execucao)

							SELECT seq_hst_car_solicitacao.nextval, SOLICITACAO_ID, TID, :credenciado, :credenciado_tid, NUMERO, :data_emissao, :situacao_id, :situacao_texto,
							PROJETO_DIGITAL_ID, PROJETO_DIGITAL_TID, REQUERIMENTO_ID, REQUERIMENTO_TID, ATIVIDADE_ID, ATIVIDADE_TID, 
							EMPREENDIMENTO_ID, EMPREENDIMENTO_TID, DECLARANTE_ID, DECLARANTE_TID, :situacao_data, SITUACAO_DATA,
							SITUACAO_ID, SITUACAO_TEXTO, :motivo, DOMINIALIDADE_ID, DOMINIALIDADE_TID,
							PROJETO_GEO_ID, PROJETO_GEO_TID, :executor_id, :executor_tid, :executor_nome, :executor_login,
							:executor_tipo_id, executor_tipo_texto, :acao_executada, systimestamp

							FROM HST_CAR_SOLICITACAO WHERE ID IN (
								SELECT MAX(ID) FROM HST_CAR_SOLICITACAO WHERE SOLICITACAO_ID = :id_solicitacao AND SITUACAO_ID = 2 )");

					using (var cmd = new OracleCommand(sqlBuilder.ToString(), connection))
					{
						var assembly = Assembly.GetExecutingAssembly();

						cmd.Parameters.Add(new OracleParameter("credenciado", car.AutorId));
						cmd.Parameters.Add(new OracleParameter("credenciado_tid", car.AutorTid));
						cmd.Parameters.Add(new OracleParameter("data_emissao", car.DataEmissao.Data));
						cmd.Parameters.Add(new OracleParameter("situacao_id", car.SituacaoId));
						cmd.Parameters.Add(new OracleParameter("situacao_texto", car.SituacaoTexto));
						cmd.Parameters.Add(new OracleParameter("situacao_data", car.DataSituacao.Data));
						cmd.Parameters.Add(new OracleParameter("motivo", car.Motivo));
						cmd.Parameters.Add(new OracleParameter("executor_id", 1/*sistema*/));
						cmd.Parameters.Add(new OracleParameter("executor_tid", assembly.ManifestModule.ModuleVersionId.ToString()));
						cmd.Parameters.Add(new OracleParameter("executor_nome", assembly.ManifestModule.Name));
						cmd.Parameters.Add(new OracleParameter("executor_login", "IDAF_Scheduler"));
						cmd.Parameters.Add(new OracleParameter("executor_tipo_id", 1/*interno*/));
						cmd.Parameters.Add(new OracleParameter("acao_executada", 17));//alterarsituacao

						cmd.Parameters.Add(new OracleParameter("id_solicitacao", solicitacaoId));

						cmd.ExecuteNonQuery();
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}
		}

		private static void VerificarListaCodigoImovel(OracleConnection conn, string schema, string codigoImovelString, int solicitacaoNumero, Int32 empreendimento, string origem, RequisicaoJobCar requisicao, string tid)
		{
			try
			{
				var listaCodigos = codigoImovelString.Split('\n', ':', ',', '.')?.Where(x => x.Length == 43)?.Select(x => x.Replace(",", "")).Select(x => x.Replace(".", ""))?.ToList();

				if (listaCodigos.Count() == 1)
				{
					if(!VerificarCodigo(conn, listaCodigos[0], solicitacaoNumero, origem))
					{
						AtualizaInformacoesCAR(conn, listaCodigos[0], solicitacaoNumero, origem, requisicao, tid);
						InserirTabelaTransacional(conn, listaCodigos[0], solicitacaoNumero, empreendimento, 1, origem);
					}else
					{
						InserirTabelaTransacional(conn, listaCodigos[0], solicitacaoNumero, empreendimento, 0, origem);
					}
				}
				else
				{
					var naoExisteNoSimlam = new List<string>();

					foreach (var codigo in listaCodigos)
					{
						if (!VerificarCodigo(conn, listaCodigos[0], solicitacaoNumero, origem))
						{
							naoExisteNoSimlam.Add(codigo);
						}
					}

					if(naoExisteNoSimlam.Count() > 1)
					{
						foreach(var codigo in naoExisteNoSimlam)
						{
							InserirTabelaTransacional(conn, codigo, solicitacaoNumero, empreendimento, 0, origem);
						}
					} else if (naoExisteNoSimlam.Count() == 1)
					{
						AtualizaInformacoesCAR(conn, naoExisteNoSimlam[0], solicitacaoNumero, origem, requisicao, tid);
						InserirTabelaTransacional(conn, naoExisteNoSimlam[0], solicitacaoNumero, empreendimento, 1, origem);
					}
				}
			} catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados  / VerificarListaCodigoImovel:  /" + exception.Message, exception);
			}
		}

		private static bool VerificarCodigo(OracleConnection conn, string codigo, int solicitacao, string origem)
		{
			try
			{
				var esquema = origem == RequisicaoJobCar.INSTITUCIONAL ? 1 : 2;

				//using (var cmd = new OracleCommand("SELECT COUNT(ID) FROM TAB_CONTROLE_SICAR WHERE CODIGO_IMOVEL LIKE '%:codigo%'", conn))
				using (var cmd = new OracleCommand(@"SELECT SUM(CONTADOR) FROM (
														  SELECT COUNT(1) CONTADOR FROM IDAF.TAB_CONTROLE_SICAR S	
																	  INNER JOIN IDAF.TAB_CAR_SOLICITACAO C ON C.ID = S.SOLICITACAO_CAR
																WHERE S.SOLICITACAO_CAR_ANTERIOR IN
																			(SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR WHERE CODIGO_IMOVEL = :codigo)
														  UNION ALL
														  SELECT COUNT(1) CONTADOR FROM IDAF.TAB_CONTROLE_SICAR S
																	  INNER JOIN IDAFCREDENCIADO.TAB_CAR_SOLICITACAO C ON C.ID = S.SOLICITACAO_CAR
																WHERE S.SOLICITACAO_CAR_ANTERIOR IN
																			(SELECT SOLICITACAO_CAR FROM TAB_CONTROLE_SICAR WHERE CODIGO_IMOVEL = :codigo)
															  )", conn))
				{
					cmd.Parameters.Add(new OracleParameter("codigo", codigo));
					if (Convert.ToBoolean(cmd.ExecuteScalar()))
					{
						return false;
					}
					else
					{
						using (var cd = new OracleCommand("SELECT COUNT(1) FROM TAB_CONTROLE_SICAR WHERE CODIGO_IMOVEL = :codigo AND SOLICITACAO_CAR != :solicitacao", conn))
						{
							cd.Parameters.Add(new OracleParameter("codigo", codigo));
							cd.Parameters.Add(new OracleParameter("solicitacao", solicitacao));

							return (Convert.ToBoolean(cd.ExecuteScalar()));
						}
					}
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
				return false;
			}
		}

		private static void AtualizaInformacoesCAR(OracleConnection conn, string codigo, int solicitacaoNumero, string origem, RequisicaoJobCar requisicao, string tid)
		{
			var retorno = new MensagemRetorno();
			retorno.mensagensResposta = new List<string>();
			retorno.codigoImovelComMascara = codigo;
			retorno.codigoResposta = 200;
			var resultado = "Imóvel inserido com sucesso no banco de dados do Sicar - Sistema Nacional de Cadastro Ambiental Rural;";
			retorno.mensagensResposta.Add(resultado);
			var schema = (origem == RequisicaoJobCar.INSTITUCIONAL) ? "IDAF" : "IDAFCREDENCIADO";

			try
			{
				// ATUALIZA TAB_CONTROLE_SICAR
				using (var cmd = new OracleCommand(@"UPDATE TAB_CONTROLE_SICAR SET CODIGO_IMOVEL = :codigo_imovel, 
													SITUACAO_ENVIO = 6, CODIGO_RESPOSTA = 200, PENDENCIAS = NULL, MENSAGEM_RESPOSTA = :mensagem 
	 												WHERE SOLICITACAO_CAR = :solicitacao_car", conn))
				{
					cmd.Parameters.Add(new OracleParameter("codigo_imovel", codigo));
					cmd.Parameters.Add(new OracleParameter("mensagem", resultado));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", solicitacaoNumero));

					cmd.ExecuteNonQuery();
				}

				// ATUALIZA TAB_CAR_SOLICITACAO
				using (var cmd = new OracleCommand("UPDATE "+ schema + @".TAB_CAR_SOLICITACAO SET SITUACAO_ANTERIOR = SITUACAO,
													SITUACAO_ANTERIOR_DATA = SITUACAO_DATA, SITUACAO = 2, SITUACAO_DATA = SYSDATE
													WHERE ID = :solicitacao_car", conn))
				{
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", solicitacaoNumero));

					cmd.ExecuteNonQuery();
				}

				// ATUALIZA O IDAF_CONSULTA
				var strQuery = String.Empty;
				if (origem == RequisicaoJobCar.INSTITUCIONAL)
					strQuery = "begin LST_CONSULTA.carSolicitacaoTitulo(:id); end;";
				else
					strQuery = "begin LST_CONSULTA_CRED.carSolicitacao(:id); end;";

				using (var cmd = new OracleCommand(strQuery, conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", solicitacaoNumero));

					cmd.ExecuteNonQuery();
				}

				// ATUALIZA O HISTORICO
				InserirHistoricoSolicitacaoCar(conn, origem, solicitacaoNumero);
				InserirHistoricoControleCar(conn, requisicao, tid, retorno);
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados - UPDATES:" + exception.Message, exception);
			}
		}

		private static void InserirTabelaTransacional(OracleConnection conn, string codigo, int solicitacaoNumero, Int32 empreendimento, int integrado_atualizado, string origem)
		{		
			try
			{
				var esquema = origem == RequisicaoJobCar.INSTITUCIONAL ? 1 : 2;

				using (var cmd = new OracleCommand(@"INSERT INTO TAB_CONTROLE_COD_IMOVEL (ID, EMPREENDIMENTO, SOLICITACAO_CAR, CODIGO_IMOVEL,
													SOLICITACAO_CAR_ESQUEMA, INTEGRADO_ATUALIZADO)
													VALUES (SEQ_CONTROLE_COD_IMOVEL.nextval, :empreendimento, :solicitacao, :codigo,
													:esquema, :integrado_atualizado)", conn))
				{
					cmd.Parameters.Add(new OracleParameter("empreendimento", empreendimento));
					cmd.Parameters.Add(new OracleParameter("solicitacao", solicitacaoNumero));
					cmd.Parameters.Add(new OracleParameter("codigo", codigo));
					cmd.Parameters.Add(new OracleParameter("esquema", esquema));
					cmd.Parameters.Add(new OracleParameter("integrado_atualizado", integrado_atualizado));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados - INSERIR:" + exception.Message, exception);
			}
		}

		private static List<string> TratandoMensagens(OracleConnection conn, List<string> mensagens)
		{
			for (int i =0; i < mensagens.Count; i++)
			{
				if(mensagens[i].Contains("Ocorreu um erro ao processar"))
				{
					mensagens[i] = "Mensagem retornada do SICAR: " + mensagens[i];
				}
			}

			return mensagens;
			//Ocorreu um erro ao processar 
		}

		public static bool VerificarCarValido(OracleConnection conn, int id)
		{
			try
			{
				using (var cmd = new OracleCommand(@"SELECT count(1) FROM TAB_CONTROLE_SICAR WHERE SOLICITACAO_CAR = :id AND SITUACAO_ENVIO = 6", conn))
				{
					cmd.Parameters.Add(new OracleParameter("id", id));
					return Convert.ToBoolean(cmd.ExecuteScalar());
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro VerificarCarValido:  - " + id + " ID -- " + exception + " + exception.Message, exception");
				return false;
			}
		}
	}
}