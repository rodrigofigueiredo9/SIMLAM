using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Geobases.WebServices.Models.Entities;

namespace Tecnomapas.Geobases.WebServices.Models.Data
{
    public class AutenticacaoDa
    {
        public bool Autenticar(string chave)
        {
            bool autenticado = false;

            using (BancoDeDados bd = BancoDeDados.ObterInstancia("StringConexao"))
            {
                string queryStr = @"select count(*) autenticado from tab_sistema_externo a where a.chave_autenticacao = :chave";

                Comando comando = bd.CriarComando(queryStr);

                comando.AdicionarParametroEntrada("chave", chave, DbType.String);

                using (DbDataReader reader = bd.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        autenticado = (Convert.ToInt32(reader["autenticado"]) > 0);
                            
                    }
                    reader.Close();
                }
            }

            return autenticado;
        }
    }
}