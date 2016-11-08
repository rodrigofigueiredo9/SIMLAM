using System.Globalization;
using System.Text;

namespace System
{
	public static class StringExt
	{
		public static string DeixarApenasUmEspaco(this string texto)
		{
			bool isEspaco = false;

			for (int i = 0; i < texto.Length; i++)
			{
				isEspaco = (isEspaco && texto[i] == ' ');

				if (isEspaco)
				{
					texto = texto.Remove(i, 1);
					i--;
				}

				isEspaco = (texto[i] == ' ');
			}

			return texto.Trim();
		}

		public static string RemoverAcentos(this string texto)
		{
			if(string.IsNullOrEmpty(texto))
			{
				return texto;
			}

			string s = texto.Normalize(NormalizationForm.FormD);

			StringBuilder sb = new StringBuilder();

			for (int k = 0; k < s.Length; k++)
			{
				UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(s[k]);
				if (uc != UnicodeCategory.NonSpacingMark)
				{
					sb.Append(s[k]);
				}
			}

			return sb.ToString();
		}
	}
}