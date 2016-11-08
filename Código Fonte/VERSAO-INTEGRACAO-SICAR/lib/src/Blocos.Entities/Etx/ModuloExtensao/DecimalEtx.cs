namespace System
{
	public enum eMetrica
	{
		HaToM2,
		M2ToHa
	}

	public static class DecimalEtx
	{
		public const decimal TOLERANCIA = 99.99M;

		public static decimal? ToDecimalMask(string strValor)
		{
			decimal valor;
			if (string.IsNullOrWhiteSpace(strValor) || !decimal.TryParse(ClearMask(strValor), out valor))
			{
				return null;
			}

			return valor;
		}

		public static string ToStringTrunc(this decimal? valor, int precisao = 2, bool format = true)
		{
			return valor.HasValue ? valor.Value.ToStringTrunc(precisao, format) : string.Empty;
		}

		public static string ToStringTrunc(this decimal valor, int precisao = 2, bool format = true)
		{
			return valor.ToString((format ? "N" : "F") + precisao);
		}

		public static decimal Convert(this decimal valor, eMetrica tipo)
		{
			if (tipo == eMetrica.M2ToHa)
			{
				return valor / 10000M;
			}
			
			return valor * 10000M;
		}

		public static string ClearMask(string strValor)
		{
			return string.IsNullOrWhiteSpace(strValor) ? string.Empty : strValor.Replace(".", "");
		}

		public static bool MaiorToleranciaM2(this decimal valor)
		{
			return Decimal.Parse(valor.ToStringTrunc()) > TOLERANCIA;
		}
	}
}