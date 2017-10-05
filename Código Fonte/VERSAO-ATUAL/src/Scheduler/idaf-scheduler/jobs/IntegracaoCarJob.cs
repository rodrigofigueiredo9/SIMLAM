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
            var listID = GetIdCar(origem, conn);              

            foreach(int solicitacaoID in listID)
            {
                try
                {

                    CARSolicitacaoFunc cr = new CARSolicitacaoFunc();
                    cr.EnviarReenviarArquivoSICAR(solicitacaoID, origem, true, conn);

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
            //Busca os IDs para fazer o loop nos cadastros CAR passivo
            string BuildSQl = "SELECT ID FROM TAB_CAR_SOLICITACAO WHERE ID = 39242  OR" +
                //" ID = 60885 OR"+
                                                                        " ID = 43080 OR" +
                //" ID = 48836 OR"+
                //" ID = 54266 OR"+
                                                                        " ID = 36565 OR" +
                                                                        " ID = 40597";
                                                                        //" ID = 767  ";
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
