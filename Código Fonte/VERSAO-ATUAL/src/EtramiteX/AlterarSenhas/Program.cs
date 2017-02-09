using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Data;

namespace AlterarSenhas
{
    class Program
    {
        static void Main(string[] args)
        {
            // Instância: ATENTAR no config! 
            // "default"=Institucional, "idafcredenciado"=Credenciado, etc.
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia("default")) 
            {

                Comando comando = bancoDeDados.CriarComando(@"Select * from tab_usuario where login = 'livia.almeida'");
                string senhaHash = "", id = "";
                ArrayList alterar = new ArrayList();

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        string strTexto = reader["login"].ToString().ToLower() + "*123456";
                        UTF8Encoding encoder = new UTF8Encoding();
                        SHA512 sha512 = SHA512.Create();
                        byte[] byteHash = sha512.ComputeHash(encoder.GetBytes(strTexto));


                        senhaHash = string.Join("", byteHash.Select(bin => bin.ToString("X2")).ToArray());
                        id = reader["ID"].ToString();
                        alterar.Add("UPDATE tab_usuario set  SENHA_DATA = SYSDATE, SENHA = '" + senhaHash + "' WHERE ID = " + id);
                    }
                    reader.Close();

                }
                bancoDeDados.IniciarTransacao();
                foreach (string comandalterar in alterar)
                {
                    Comando comandoUpdate = bancoDeDados.CriarComando(comandalterar);

                    bancoDeDados.ExecutarNonQuery(comandoUpdate);
                }
                bancoDeDados.Commit();


            }
        }
    }
}
