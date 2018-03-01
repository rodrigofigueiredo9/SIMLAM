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

		public static int AtualizarControleSICAR(OracleConnection conn, MensagemRetorno resultado, RequisicaoJobCar requisicao, int situacaoEnvio, string tid, string arquivoCar = "", string tipo = "")
		{
			var schema = CarUtils.GetEsquemaInstitucional();

			var item = ObterItemControleCar(conn, requisicao);

			var pendencias = "";
			var condicao = "Aguardando Análise";
            var mensagensDeResposta = String.Empty;

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
			sqlBuilder.Append("url_recibo = :url_recibo,");
			sqlBuilder.Append("status_sicar = :status_sicar,");
			sqlBuilder.Append("condicao = :condicao,");

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
					cmd.Parameters.Add(new OracleParameter("pendencias", pendencias));
                    if (!String.IsNullOrWhiteSpace(resultado.codigoImovel))
					    cmd.Parameters.Add(new OracleParameter("codigo_imovel", resultado.codigoImovel));
					cmd.Parameters.Add(new OracleParameter("url_recibo", resultado.urlReciboInscricao));
					cmd.Parameters.Add(new OracleParameter("status_sicar", "IN"));
					cmd.Parameters.Add(new OracleParameter("condicao", condicao));
					
                    cmd.Parameters.Add(new OracleParameter("codigo_resposta", resultado.codigoResposta));
                    cmd.Parameters.Add(new OracleParameter("codigo_imovel_masc", resultado.codigoImovelComMascara));
                    
                    cmd.Parameters.Add(new OracleParameter("mensagem_resposta", mensagensDeResposta));
                    //cmd.Parameters.Add(new OracleParameter("mensagem_resposta", resultado.mensagensResposta));                    
                    cmd.Parameters.Add(new OracleParameter("id", item.id));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
			}

			//Inserir no Histórico
			InserirHistoricoControleCar(conn, requisicao, tid, resultado);
            if(item == null)
            {
                return 0;
            }
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
            sqlBuilder.Append("url_recibo = :url_recibo,");
            sqlBuilder.Append("status_sicar = :status_sicar,");
            sqlBuilder.Append("condicao = :condicao,");

            sqlBuilder.Append("CODIGO_RESPOSTA = :codigo_resposta, ");
            sqlBuilder.Append("CODIGO_IMOVEL_MASC = :codigo_imovel_masc, ");
            sqlBuilder.Append("MENSAGEM_RESPOSTA = :mensagem_resposta ");
            sqlBuilder.Append(" WHERE id = :id");

            try
            {
                using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
                {
                    cmd.Parameters.Add(new OracleParameter("tid", tid));
                    cmd.Parameters.Add(new OracleParameter("situacao_envio", SITUACAO_ENVIO_ARQUIVO_RETIFICADO));
                    cmd.Parameters.Add(new OracleParameter("chave_protocolo", resultado.protocoloImovel));
                    cmd.Parameters.Add(new OracleParameter("pendencias", pendencias));
                    if (!String.IsNullOrWhiteSpace(resultado.codigoImovel))
                        cmd.Parameters.Add(new OracleParameter("codigo_imovel", resultado.codigoImovel));
                    cmd.Parameters.Add(new OracleParameter("url_recibo", resultado.urlReciboInscricao));
                    cmd.Parameters.Add(new OracleParameter("status_sicar", "IN"));
                    cmd.Parameters.Add(new OracleParameter("condicao", condicao));

                    cmd.Parameters.Add(new OracleParameter("codigo_resposta", resultado.codigoResposta));
                    cmd.Parameters.Add(new OracleParameter("codigo_imovel_masc", resultado.codigoImovelComMascara));

                    cmd.Parameters.Add(new OracleParameter("mensagem_resposta", mensagensDeResposta));
                    //cmd.Parameters.Add(new OracleParameter("mensagem_resposta", resultado.mensagensResposta));                    
                    cmd.Parameters.Add(new OracleParameter("id", item.id));

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
            requisicao.solicitacao_car = item.solicitacao_car;
            requisicao.solicitacao_car_tid = item.solicitacao_car_tid;

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
			sqlBuilder.Append("url_recibo,status_sicar,condicao,solicitacao_car_esquema,data_execucao/*,");
            sqlBuilder.Append("CODIGO_RESPOSTA, CODIGO_IMOVEL_MASC, MENSAGEM_RESPOSTA*/)");
			sqlBuilder.Append(" values ");
			sqlBuilder.Append("(" + schema + ".SEQ_HST_CONTROLE_SICAR.nextval,:tid,:empreendimento,:empreendimento_tid,");
			sqlBuilder.Append(":solicitacao_car,:solicitacao_car_tid,:situacao_envio,:chave_protocolo,");
			sqlBuilder.Append(":data_gerado,:data_envio,:arquivo,:pendencias,:codigo_imovel,");
			sqlBuilder.Append(":url_recibo,:status_sicar,:condicao,:solicitacao_car_esquema,CURRENT_TIMESTAMP/*, :CODIGO_RESPOSTA, :CODIGO_IMOVEL_MASC, :MENSAGEM_RESPOSTA*/)");

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

                    /*cmd.Parameters.Add(new OracleParameter("codigo_resposta", resultado.codigoResposta));
                    cmd.Parameters.Add(new OracleParameter("codigo_imovel_masc", resultado.codigoImovelComMascara));
                    cmd.Parameters.Add(new OracleParameter("mensagem_resposta", mensagensDeResposta));
                    */ //cmd.Parameters.Add(new OracleParameter("mensagem_resposta", resultado.mensagensResposta));

					cmd.ExecuteNonQuery();
				}
			}
			catch (Exception exception)
			{
				Log.Error("Erro ao conectar ao Banco de dados:" + exception.Message, exception);
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
			sqlBuilder.Append("t.empreendimento = :empreendimento AND t.empreendimento_tid = :empreendimento_tid ");
			sqlBuilder.Append("AND t.solicitacao_car = :solicitacao_car AND t.solicitacao_car_tid = :solicitacao_car_tid ");
			sqlBuilder.Append("AND rownum = 1 ORDER BY id DESC");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("empreendimento", requisicao.empreendimento));
					cmd.Parameters.Add(new OracleParameter("empreendimento_tid", requisicao.empreendimento_tid));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car", requisicao.solicitacao_car));
					cmd.Parameters.Add(new OracleParameter("solicitacao_car_tid", requisicao.solicitacao_car_tid));

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
            //var schema = CarUtils.GetEsquemaInstitucional();
            var schema = (requisicao.origem == RequisicaoJobCar.INSTITUCIONAL)
               ? "IDAF"
               : "IDAFCREDENCIADO";

            var schemaN = (requisicao.origem == RequisicaoJobCar.INSTITUCIONAL) 
               ? 1 : 2;
            var item = new ItemControleCar();

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append("SELECT T.ID, T.TID, T.EMPREENDIMENTO, T.EMPREENDIMENTO_TID,T.SOLICITACAO_CAR, T.SOLICITACAO_CAR_TID,  ");
            sqlBuilder.Append("T.SITUACAO_ENVIO, T.CHAVE_PROTOCOLO,T.DATA_GERADO,T.DATA_ENVIO,T.PENDENCIAS,T.CODIGO_IMOVEL, ");
            sqlBuilder.Append("T.URL_RECIBO,T.STATUS_SICAR,T.CONDICAO,T.SOLICITACAO_CAR_ESQUEMA,NVL(T.SOLICITACAO_PASSIVO, 0)SOLICITACAO_PASSIVO, ");
            sqlBuilder.Append("NVL(T.SOLICITACAO_SITUACAO_APROVADO, 2)SOLICITACAO_SITUACAO_APROVADO FROM IDAF.TAB_CONTROLE_SICAR t ");
            sqlBuilder.Append("INNER JOIN "+ schema+ ".TAB_EMPREENDIMENTO E1 ON E1.ID = t.EMPREENDIMENTO  ");
            sqlBuilder.Append("INNER JOIN " + schema + ".TAB_EMPREENDIMENTO E2 ON E2.CODIGO = E1.CODIGO WHERE ");
            sqlBuilder.Append("E2.ID = : empreendimento AND t.SOLICITACAO_CAR != :car AND t.SITUACAO_ENVIO NOT IN (4,8) AND t.SOLICITACAO_CAR_ESQUEMA = :schema ");
            sqlBuilder.Append("AND rownum = 1 ORDER BY id DESC");

            try
            {
                using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
                {
                    cmd.Parameters.Add(new OracleParameter("empreendimento", requisicao.empreendimento));
                    cmd.Parameters.Add(new OracleParameter("car", requisicao.solicitacao_car));
                    cmd.Parameters.Add(new OracleParameter("schema", schemaN));

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

		internal static void AtualizarSolicitacaoCar(OracleConnection conn, RequisicaoJobCar requisicao,int situacao, string tid)
		{
			var controleCAR = ObterItemControleCar(conn, requisicao);

			if (situacao == ControleCarDB.SITUACAO_SOLICITACAO_VALIDO && controleCAR.solicitacao_passivo > 0)
			{
				situacao = controleCAR.solicitacao_situacao_aprovado;
			}	

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
			sqlBuilder.Append("situacao_anterior_data = situacao_data,");
			sqlBuilder.Append("motivo = :motivo");
			sqlBuilder.Append(" WHERE id = :id");

			try
			{
				using (var cmd = new OracleCommand(sqlBuilder.ToString(), conn))
				{
					cmd.Parameters.Add(new OracleParameter("tid", tid));
					cmd.Parameters.Add(new OracleParameter("situacao", situacao));
                    if (situacao == 2) cmd.Parameters.Add(new OracleParameter("motivo", " "));
                    if (situacao == 6 ||situacao == 3) cmd.Parameters.Add(new OracleParameter("motivo", "Arquivo CAR Reprovado"));
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

        public static void AtualizarSolicitacaoCarRetificacao(OracleConnection conn, string origem, int solicitacaoId, string tid)
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
	}
}