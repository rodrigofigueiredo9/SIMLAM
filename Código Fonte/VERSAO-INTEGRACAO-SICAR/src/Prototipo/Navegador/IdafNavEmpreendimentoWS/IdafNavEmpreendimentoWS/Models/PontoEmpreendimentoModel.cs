using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using System.Data.Common;
using System.Data;

namespace IdafNavEmpreendimentoWS.Models
{
    public class PontoEmpreendimentoModel
    {
        public List<PontoEmpreendimento> listarEmpreendimentos(List<String> empreendimentos)
        {
            List<PontoEmpreendimento> listResult = new List<PontoEmpreendimento>();
            
            using (BancoDeDados bd = BancoDeDados.ObterInstancia())
            {
                string queryStr = @"select * from (select e.id, e.denominador," + 
                                  "(select ls.texto from lov_empreendimento_segmento ls where ls.id = e.segmento) segmento,"+
                                  "(select lm.texto from tab_empreendimento_endereco ed, lov_municipio lm where ed.empreendimento = e.id and ed.correspondencia = 0 and ed.municipio = lm.id) municipio,"+
                                  "(select ta.atividade from tab_empreendimento_atividade ta where ta.id = e.atividade) atividade,"+
                                  "(select stragg(l.numero_completo||' - '||l.interessado_nome_razao) from lst_protocolo l where l.empreendimento_id = e.id) processos "+
                                  "from idafgeo.geo_emp_localizacao g, tab_empreendimento e where e.id = g.empreendimento) q WHERE q.id in (";

                string ids = "";

                int m = empreendimentos.Count;
                for (int i = 0; i < m; i++)
                {
                    ids += "," + empreendimentos[i];
                }

                queryStr += ids.Substring(1) + ") ORDER BY q.id ASC";

                Comando comando = bd.CriarComando(queryStr);

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        PontoEmpreendimento result = new PontoEmpreendimento();
                        result.id = Convert.ToInt32(reader["id"]);
                        result.denominador = Convert.ToString(reader["denominador"]);
                        result.segmento = Convert.ToString(reader["segmento"]);
                        result.municipio = Convert.ToString(reader["municipio"]);
                        result.atividade = Convert.ToString(reader["atividade"]);
                        result.processos = Convert.ToString(reader["processos"]);
                        listResult.Add(result);
                    }
                    reader.Close();
                }
            }

            foreach (PontoEmpreendimento ponto in listResult)
            {
                getPontoEmpreendimentoXY(ponto);
            }

            return listResult;
        }
        public List<PontoEmpreendimento> listarEmpreendimentos(String empreendimento, String pessoa, String processo, String segmento, String municipio, String atividade)
        {
            List<PontoEmpreendimento> listResult = new List<PontoEmpreendimento>();

            using (BancoDeDados bd = BancoDeDados.ObterInstancia())
            {
                string queryStr = @"select * from (select e.id, e.denominador," +
                                  "(select ls.texto from lov_empreendimento_segmento ls where ls.id = e.segmento) segmento," +
                                  "(select lm.texto from tab_empreendimento_endereco ed, lov_municipio lm where ed.empreendimento = e.id and ed.correspondencia = 0 and ed.municipio = lm.id) municipio," +
                                  "(select ta.atividade from tab_empreendimento_atividade ta where ta.id = e.atividade) atividade," +
                                  "(select stragg(l.numero_completo||' - '||l.interessado_nome_razao) from lst_protocolo l where l.empreendimento_id = e.id) processos " +
                                  "from idafgeo.geo_emp_localizacao g, tab_empreendimento e where e.id = g.empreendimento) q ";

                List<string> conditions = new List<string>();

                if (empreendimento != "" || segmento != "" || municipio != "" || pessoa != "" || atividade != "" || processo != "")
                {
                    queryStr += "WHERE ";

                    if (empreendimento != "")
                    {
                        conditions.Add("UPPER(q.denominador) LIKE UPPER('%'||:empreendimento||'%') ");
                    }

                    if (segmento != "")
                    {
                        conditions.Add("q.segmento = :segmento ");
                    }

                    if (atividade != "")
                    {
                        conditions.Add("q.atividade = :atividade ");
                    }

                    if (municipio != "")
                    {
                        conditions.Add("q.municipio = :municipio ");
                    }

                    if (processo != "")
                    {
                        conditions.Add("UPPER(q.processos) LIKE UPPER('%'||:processo||'%') ");
                    }


                    if (pessoa != "")
                    {
                        conditions.Add("UPPER(q.processos) LIKE UPPER('%'||:pessoa||'%') ");
                    }

                    int m = conditions.Count;

                    if (m != 0)
                    {
                        queryStr += conditions[0];
                        for (int i = 1; i < m; i++)
                        {
                            queryStr += "AND " + conditions[i];
                        }
                    }
                }
                
                Comando comando = bd.CriarComando(queryStr);

                if (empreendimento != "")
                {
                    comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.String);
                }
                if (segmento != "")
                {
                    comando.AdicionarParametroEntrada("segmento", segmento, DbType.String);
                }
                if (municipio != "")
                {
                    comando.AdicionarParametroEntrada("municipio", municipio, DbType.String);
                }
                if (processo != "")
                {
                    comando.AdicionarParametroEntrada("processo", processo, DbType.String);
                }
                if (atividade != "")
                {
                    comando.AdicionarParametroEntrada("atividade", atividade, DbType.String);
                }
                if (pessoa != "")
                {
                    comando.AdicionarParametroEntrada("pessoa", pessoa, DbType.String);
                }

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        PontoEmpreendimento result = new PontoEmpreendimento();
                        result.id = Convert.ToInt32(reader["id"]);
                        result.denominador = Convert.ToString(reader["denominador"]);
                        result.segmento = Convert.ToString(reader["segmento"]);
                        result.municipio = Convert.ToString(reader["municipio"]);
                        result.atividade = Convert.ToString(reader["atividade"]);
                        result.processos = Convert.ToString(reader["processos"]);
                        listResult.Add(result);
                    }
                    reader.Close();
                }
            }

            foreach (PontoEmpreendimento ponto in listResult)
            {
                getPontoEmpreendimentoXY(ponto);
            }

            return listResult;
        }

        private void getPontoEmpreendimentoXY(PontoEmpreendimento ponto)
        {
            List<Decimal> ordernadas = new List<decimal>();
            
            using (BancoDeDados bd = BancoDeDados.ObterInstancia())
            {
                string queryStr = "select column_value ordenada from table (select sdo_geom.sdo_mbr(t.geometry).sdo_ordinates from idafgeo.geo_emp_localizacao t where t.empreendimento = :id and rownum =1)";

                Comando comando = bd.CriarComando(queryStr);

                comando.AdicionarParametroEntrada("id", ponto.id, DbType.Int32);

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        ordernadas.Add(Convert.ToDecimal(reader["ordenada"]));
                    }
                    reader.Close();
                }
            }

            ponto.x = ordernadas[0];
            ponto.y = ordernadas[1];
        }
    }
}