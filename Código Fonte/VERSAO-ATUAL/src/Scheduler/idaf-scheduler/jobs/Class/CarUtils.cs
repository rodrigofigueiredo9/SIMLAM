using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;
using Oracle.ManagedDataAccess.Client;

namespace Tecnomapas.EtramiteX.Scheduler.jobs.Class
{
    public static class CarUtils
    {
        public static string GerarCodigoProtocolo(DateTime hora)
        {
            //Calcula o Número de protocolo baseado no timestamp (EPOCH) da hora atual convertido para MD5
            var agora = ConvertToTimestamp(hora);

            // Convert the input string to a byte array and compute the hash.
            var md5Hasher = MD5.Create();
            var data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(agora.ToString()));

            // Loop through each byte of the hashed data
            // and format each one as a hexadecimal string.
            var sBuilder = new StringBuilder();
            for (var i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }

            //Add a comma each four letters
            var charArray = sBuilder.ToString().ToCharArray();
            var final = "";
            var count = 0;
            foreach (var c in charArray)
            {
                final += c.ToString();
                count++;
                if (count == 4)
                {
                    final += ".";
                    count = 0;
                }
            }

            // And return it
            return final.Substring(0, final.Length - 1);
        }

        private static double ConvertToTimestamp(DateTime value)
        {
            //create Timespan by subtracting the value provided from
            //the Unix Epoch
            var span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());

            //return the total seconds (which is a UNIX timestamp), arredondado para baixo

            var random = new Random();

            return Math.Floor(span.TotalSeconds * random.NextDouble());
        }

        public static string ObterCodigoUF(int codigoMunicipio)
        {
            var codigo = codigoMunicipio / 100000;

            var codigoEstado = new Dictionary<int, string>
			{
				{11, "RO"},
				{12, "AC"},
				{13, "AM"},
				{14, "RR"},
				{15, "PA"},
				{16, "AP"},
				{17, "TO"},
				{21, "MA"},
				{22, "PI"},
				{23, "CE"},
				{24, "RN"},
				{25, "PB"},
				{26, "PE"},
				{27, "AL"},
				{28, "SE"},
				{29, "BA"},
				{31, "MG"},
				{32, "ES"},
				{33, "RJ"},
				{35, "SP"},
				{41, "PR"},
				{42, "SC"},
				{43, "RS"},
				{50, "MS"},
				{51, "MT"},
				{52, "GO"},
				{53, "DF"}
			};

            return (codigoEstado.ContainsKey(codigo) ? codigoEstado[codigo] : "XX");
        }

        public static string GetBancoInstitucional()
        {
            return ConfigurationManager.ConnectionStrings["bancoInstitucional"].ToString();
        }

        public static string GetBancoInstitucionalGeo()
        {
            return ConfigurationManager.ConnectionStrings["bancoInstitucionalGeo"].ToString();
        }

        public static string GetBancoCredenciado()
        {
            return ConfigurationManager.ConnectionStrings["bancoCredenciado"].ToString();
        }

        public static string GetBancoCredenciadoGeo()
        {
            return ConfigurationManager.ConnectionStrings["bancoCredenciadoGeo"].ToString();
        }

        public static string GetEsquemaInstitucional()
        {
            return ConfigurationManager.AppSettings["esquemaInstitucional"];
        }

        public static string GetEsquemaInstitucionalGeo()
        {
            return ConfigurationManager.AppSettings["esquemaInstitucionalGeo"];
        }

        public static string GetEsquemaCredenciado()
        {
            return ConfigurationManager.AppSettings["esquemaCredenciado"];
        }

        public static string GetEsquemaCredenciadoGeo()
        {
            return ConfigurationManager.AppSettings["esquemaCredenciadoGeo"];
        }

        public static T GetValue<T>(this OracleDataReader reader, string fieldName)
        {
            var result = default(T);
            var index = reader.GetOrdinal(fieldName);

            if (reader.IsDBNull(index))
            {
                return default(T);
            }

            if (typeof(T) == typeof(string))
            {
                result = (T)Convert.ChangeType(reader.GetString(index), typeof(T));

                if (result == null)
                {
                    result = (T)Convert.ChangeType(string.Empty, typeof(T));
                }
            }

            if (typeof(T) == typeof(int))
            {
                result = (T)Convert.ChangeType(reader.GetInt32(index), typeof(T));
            }

            if (typeof(T) == typeof(DateTime))
            {
                result = (T)Convert.ChangeType(reader.GetDateTime(index), typeof(T));
            }

            //if (typeof(T) == typeof(byte[]))
            //{
            //	OracleLob blob = reader.GetOracleLob(index);
            //	result = (T)Convert.ChangeType(blob.Value, typeof(T));
            //}

            return result;
        }
    }
}
