using GeoJSON.Net.Geometry;
using Ionic.Zip;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Data;
using System.Threading.Tasks;
using Tecnomapas.EtramiteX.Scheduler.misc;
using Tecnomapas.EtramiteX.Scheduler.misc.WKT;
using Tecnomapas.EtramiteX.Scheduler.models;
using Tecnomapas.EtramiteX.Scheduler.models.misc;
using Tecnomapas.EtramiteX.Scheduler.models.simlam;
//using Tecnomapas.EtramiteX.Interno.Controllers;
//using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
    [DisallowConcurrentExecution]
    public class IntegracaoCarJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            //INSTITUCIONAL
            int origem = 1;
            using (var connInst = new OracleConnection(CarUtils.GetBancoInstitucional()))
            {
                connInst.Open();
                ChamadaEnviarCarInterno(origem, connInst);
                connInst.Close();
            }
            
            //CREDENCIADO
            origem = 2;            
            using (var connCre = new OracleConnection(CarUtils.GetBancoCredenciado()))
            {
                connCre.Open();
                ChamadaEnviarCarInterno(origem, connCre);
                connCre.Close();
            }
        }

        private void ChamadaEnviarCarInterno(int origem, OracleConnection conn)
      {
            var listIDCar = GetIdCar(origem, conn);
            

          foreach(int solicitacaoID in listIDCar)
          {
            // Percorre todos os registros da tab_car_solicitacao (passivo) e monta *.car de cada empreendimento e insere tab_schedule_fila
              try
              {
                  CARSolicitacaoFunc cr = new CARSolicitacaoFunc();
                  // Monta o json na coluna REQUISÃO da tab_schedule_fila 
                  cr.EnviarReenviarArquivoSICAR(solicitacaoID, origem, true, conn);
              }
              catch (Exception ex)
              {
                  using (OracleCommand command = new OracleCommand("UPDATE TAB_CAR_SOLICITACAO SET PASSIVO_ENVIADO = 0 WHERE PASSIVO_ENVIADO = 1 AND ID = :id", conn))
                  {
                      command.Parameters.Add(new OracleParameter("id", solicitacaoID));
                      command.ExecuteNonQuery();
                  }
                  String i = ex.Message;
              }              
          }
        }

        public List<int> GetIdCar(int origem, OracleConnection conn)
        {
            //Busca os IDs para fazer o loop nos cadastros CAR passivo
            string BuildSQl = "SELECT ID FROM TAB_CAR_SOLICITACAO WHERE  ID IN (40774,45210,65651)"; // C 2300 // I 25300 // C 2100  // I 25100 // I 25000 // I 24500 // C 2000
            
            //string BuildSQlUp = "UPDATE TAB_CAR_SOLICITACAO SET PASSIVO_ENVIADO = 1 WHERE PASSIVO_ENVIADO IS NULL AND ID < 2300";    
            /*string BuildSQl = @"SELECT CAR.ID
                                FROM TAB_CAR_SOLICITACAO CAR
                                    INNER JOIN IDAFGEO.GEO_CAR_APP_CALCULADAS GEOAPP
                                        ON GEOAPP.EMPREENDIMENTO = CAR.EMPREENDIMENTO
                                  WHERE ROWNUM < 1000";
            
            */
            /*string BuildSQl = @"SELECT CAR.ID
                FROM TAB_CAR_SOLICITACAO CAR
                    INNER JOIN IDAFGEO.GEO_CAR_ESCADINHA_CALCULADAS GEOAPP
                        ON GEOAPP.EMPREENDIMENTO = CAR.EMPREENDIMENTO
                  WHERE ROWNUM < 1000
                ";
              */
            var arrayIDS = new List<int>();
            try
            {
                using (OracleCommand command = new OracleCommand(BuildSQl, conn))
                {
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            arrayIDS.Add(dr.GetValue<int>("ID"));
                        }
                    }                    
                }
               /*using (OracleCommand cmdUp = new OracleCommand(BuildSQlUp, conn))
                {
                    cmdUp.ExecuteNonQuery();                    
                }*/                              
            }catch(Exception e)
            {
                string v = e.Message;
            }
            return arrayIDS;
        }

        public List<int> GetEmpreendimento(int origem, OracleConnection conn)
        {
            string BuildSQl = "SELECT EMPREENDIMENTO FROM TAB_CAR_SOLICITACAO WHERE SITUACAO NOT IN (1,3) " ;
            var arrayIDS = new List<int>();

            using (OracleCommand command = new OracleCommand(BuildSQl, conn))
            {
                using (var dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        arrayIDS.Add(dr.GetValue<int>("EMPREENDIMENTO"));
                    }
                }
            }
            return arrayIDS;
        }

        public void callProcedures(int origem, OracleConnection conn)
        {
            //var listEmpreendimento = GetEmpreendimento(origem, conn);

            /*
            foreach (int solicitacaoEmp in listEmpreendimento)
            {
                try
                {
                    #region Carga das tabelas APP Caculada e APP Escadinha
                    var qtdModuloFiscal = 0.0;

                    string buildSQL = "SELECT ATP_QTD_MODULO_FISCAL FROM CRT_CAD_AMBIENTAL_RURAL WHERE EMPREENDIMENTO = :empreendimentoID";

                    using (OracleCommand command = new OracleCommand(buildSQL, conn))
                    {
                        command.Parameters.Add(new OracleParameter("empreendimentoID", solicitacaoEmp));
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                qtdModuloFiscal = Convert.ToDouble(reader["ATP_QTD_MODULO_FISCAL"]);
                            }

                            reader.Close();
                        }
                    }
                    if (origem == 1)
                    {
                        #region Chamada Procedure
                        using (var connInst = new OracleConnection(CarUtils.GetBancoInstitucionalGeo()))
                        {
                            connInst.Open();
                            OracleCommand command = connInst.CreateCommand();
                            OracleTransaction transaction;

                            // Start a local transaction
                            transaction = connInst.BeginTransaction(IsolationLevel.Serializable);
                            // Assign transaction object for a pending local transaction
                            command.Transaction = transaction;
                            command.CommandText = "BEGIN OPERACOESPROCESSAMENTOGEO.CalcularAppClassificadaCAR(:emp); END;";
                            
                            command.Parameters.Add(new OracleParameter("emp", solicitacaoEmp));

                            command.ExecuteNonQuery();

                            transaction.Commit();
                        
                            connInst.Close();
                        }

                        using (var connInst = new OracleConnection(CarUtils.GetBancoInstitucionalGeo()))
                        {
                            connInst.Open();
                            OracleCommand command = connInst.CreateCommand();
                            OracleTransaction transaction;

                            // Start a local transaction
                            transaction = connInst.BeginTransaction(IsolationLevel.Serializable);
                            // Assign transaction object for a pending local transaction
                            command.Transaction = transaction;
                            command.CommandText = "BEGIN OPERACOESPROCESSAMENTOGEO.CalcularEscadinhaCAR(:emp, :moduloFiscal); END;";

                            command.Parameters.Add(new OracleParameter("emp", solicitacaoEmp));
                            command.Parameters.Add(new OracleParameter("moduloFiscal", qtdModuloFiscal));

                            command.ExecuteNonQuery();
                            transaction.Commit();

                            connInst.Close();
                        }
                        #endregion
                    }
                    else
                    {
                        #region Chamada Procedure
                        using (var connCred = new OracleConnection(CarUtils.GetEsquemaCredenciadoGeo()))
                        {
                            connCred.Open();
                            OracleCommand command = connCred.CreateCommand();
                            OracleTransaction transaction;

                            // Start a local transaction
                            transaction = connCred.BeginTransaction(IsolationLevel.Serializable);
                            // Assign transaction object for a pending local transaction
                            command.Transaction = transaction;
                            //connInst.BeginTransaction();
                            command.CommandText = "BEGIN OPERACOESPROCESSAMENTOGEO.CalcularAppClassificadaCAR(:emp); END;";
                            //using (OracleCommand command = new OracleCommand(sql, conn))
                            //{
                            command.Parameters.Add(new OracleParameter("emp", solicitacaoEmp));

                            command.ExecuteNonQuery();
                            transaction.Commit();

                            //bancoDeDados.Commit();
                            //}
                            command.Transaction = transaction;
                            //connInst.BeginTransaction();
                            command.CommandText = "BEGIN OPERACOESPROCESSAMENTOGEO.CalcularEscadinhaCAR(:emp, :moduloFiscal); END;";

                            //using (OracleCommand command = new OracleCommand(sql, conn))
                            //{
                            command.Parameters.Add(new OracleParameter("emp", solicitacaoEmp));
                            command.Parameters.Add(new OracleParameter("moduloFiscal", qtdModuloFiscal));

                            command.ExecuteNonQuery();
                            transaction.Commit();

                            //}

                            connCred.Close();
                        }
                        #endregion
                    }
                    #endregion
                }catch(Exception ex)
                {
                    string excecao = ex.Message;
                }
            }
            */

        }
    }
}
