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
            using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
            {
                conn.Open();
                ChamadaEnviarCarInterno(origem, conn);
                conn.Close();
            }
            
            //CREDENCIADO
            //origem = 2;
            //ChamadaEnviarCarInterno(origem);
        }

        private void ChamadaEnviarCarInterno(int origem, OracleConnection conn)
        {
            var listID = GetIdCar(origem, conn);              

            foreach(int solicitacaoID in listID)
            {
                try
                {

                    CARSolicitacaoFunc cr = new CARSolicitacaoFunc();
                    cr.EnviarReenviarArquivoSICAR(solicitacaoID, origem, false, conn);

                    //CARSolicitacaoController variavel = new CARSolicitacaoController();
                    //ar.EnviarReenviarArquivoSICAR(solicitacaoID, origem, false);
                }
                catch (Exception ex)
                {

                    String i = ex.Message;
                }
                
                
            }

            
        }

        public List<int> GetIdCar(int origem, OracleConnection conn)
        {
            string BuildSQl = "SELECT ID FROM BKP_CAR_SOLICITACAO";
            var arrayIDS = new List<int>();

            //using (var conn = new OracleConnection(CarUtils.GetBancoInstitucional()))
            //{
                using (OracleCommand command = new OracleCommand(BuildSQl, conn))
                {
                    //conn.Open();
                    using (var dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            arrayIDS.Add(dr.GetValue<int>("ID"));
                        }
                    }
                    //conn.Close();
                }
            //}
            
            return arrayIDS;
        }
    }
}
