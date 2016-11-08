using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Tecnomapas.Blocos.Etx.Criptografia.Business
{
	public class Decrypt
	{
		public static string Executar(string valorEncriptado)
		{
			string valorDecriptado;

			try
			{
				byte[] key= Convert.FromBase64String("FNvAd0L0q7gRI96TGl/9AhAjP7OzqMT7H/vuTg8FN3Y=");
				byte[] IV = Convert.FromBase64String("ykGAFlmGSuNQdGGl423V4g==");

				byte[] dataToDecrypt = Convert.FromBase64String(valorEncriptado);

				RijndaelManaged metodo = new RijndaelManaged();
				metodo.Mode = CipherMode.CBC;
				ICryptoTransform encriptador = metodo.CreateDecryptor(key, IV);

				// criando as streams
				MemoryStream mStr = new MemoryStream();
				CryptoStream cStr = new CryptoStream(mStr, encriptador, CryptoStreamMode.Write);

				// carregando a cadeia na stream de decriptacao
				cStr.Write(dataToDecrypt, 0, dataToDecrypt.Length);
				cStr.FlushFinalBlock();

				byte[] decryptedData = mStr.ToArray();
				mStr.Close();
				cStr.Close();

				valorDecriptado = Encoding.UTF8.GetString(decryptedData);
			}
			catch
			{
				throw new Exception("Falha ao decriptar o valor");
			}
			return valorDecriptado;
		}
	}
}
