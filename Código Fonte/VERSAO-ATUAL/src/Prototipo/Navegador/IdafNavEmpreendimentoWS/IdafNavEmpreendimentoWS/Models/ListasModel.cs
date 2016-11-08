using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using System.Data.Common;

namespace IdafNavEmpreendimentoWS.Models
{
    public class ListasModel
    {
        public Listas GetListas()
        {
            Listas result = new Listas();

            result.atividades = GetAtividades();
            result.segmentos = GetSegmentos();
            result.municipios = GetMunicipios();
            
            return result;
        }

        private List<Atividade> GetAtividades()
        {
            List<Atividade> result = new List<Atividade>();

            using (BancoDeDados bd = BancoDeDados.ObterInstancia())
            {
                string queryStr = @"select t.id, t.atividade from tab_empreendimento_atividade t";

                Comando comando = bd.CriarComando(queryStr);

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        Atividade atividade = new Atividade();
                        atividade.id = Convert.ToInt32(reader["id"]);
                        atividade.atividade = Convert.ToString(reader["atividade"]);
                        result.Add(atividade);
                    }
                    reader.Close();
                }
            }

            return result;
        }

        private List<Segmento> GetSegmentos()
        {
            List<Segmento> result = new List<Segmento>();

            using (BancoDeDados bd = BancoDeDados.ObterInstancia())
            {
                string queryStr = @"select t.id, t.texto from lov_empreendimento_segmento t";

                Comando comando = bd.CriarComando(queryStr);

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        Segmento segmento = new Segmento();
                        segmento.id = Convert.ToInt32(reader["id"]);
                        segmento.texto = Convert.ToString(reader["texto"]);
                        result.Add(segmento);
                    }
                    reader.Close();
                }
            }

            return result;
        }

        private List<Municipio> GetMunicipios()
        {
            List<Municipio> result = new List<Municipio>();

            using (BancoDeDados bd = BancoDeDados.ObterInstancia())
            {
                string queryStr = @"select t.id, t.texto from lov_municipio t where t.estado = 8";

                Comando comando = bd.CriarComando(queryStr);

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        Municipio municipio = new Municipio();
                        municipio.id = Convert.ToInt32(reader["id"]);
                        municipio.texto = Convert.ToString(reader["texto"]);
                        result.Add(municipio);
                    }
                    reader.Close();
                }
            }

            return result;
        }
    }
}