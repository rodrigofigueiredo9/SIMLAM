using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tecnomapas.Blocos.Etx.Criptografia.Business
{
	public class Encrypt
	{
		public static string Executar(string valor)
		{
			string valorEncriptado;

			try
			{
				byte[] key= Convert.FromBase64String("FNvAd0L0q7gRI96TGl/9AhAjP7OzqMT7H/vuTg8FN3Y=");
				byte[] IV = Convert.FromBase64String("ykGAFlmGSuNQdGGl423V4g==");

				byte[] dataToEncrypt = Encoding.UTF8.GetBytes(valor);

				RijndaelManaged metodo = new RijndaelManaged();
				metodo.Mode = CipherMode.CBC;
				ICryptoTransform encriptador = metodo.CreateEncryptor(key, IV);

				// criando as streams
				MemoryStream mStr = new MemoryStream();
				CryptoStream cStr = new CryptoStream(mStr, encriptador, CryptoStreamMode.Write);

				// carregando a cadeia na stream de encriptacao
				cStr.Write(dataToEncrypt, 0, dataToEncrypt.Length);
				cStr.FlushFinalBlock();

				byte[] encryptedData = mStr.ToArray();
				mStr.Close();
				cStr.Close();

				valorEncriptado = Convert.ToBase64String(encryptedData);
			}
			catch
			{
				throw new Exception("Falha ao encriptar o valor");
			}
			return valorEncriptado;
		}
	}
}
