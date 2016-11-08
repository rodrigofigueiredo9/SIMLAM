using System;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Model.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;

namespace Tecnomapas.Blocos.Entities.Model.Business
{
	static class EntitiesBus
	{
		public static int ObterAtividadeId(int codigo)
		{
			EntitiesDa da = new EntitiesDa();
			return da.ObterAtividadeId(codigo);
		}

		public static List<string> ObterFinalidades(int? finalidades)
		{
			List<string> finalidadesSelecionadas = new List<string>();
			EntitiesDa da = new EntitiesDa();
			List<Finalidade> lista = da.ObterFinalidadesExploracao();

			foreach (Finalidade finalidade in lista)
			{
				if ((finalidades & finalidade.Codigo) > 0)
				{
					finalidadesSelecionadas.Add(finalidade.Texto);
				}
			}

			return finalidadesSelecionadas;
		}

		public static String Concatenar(List<String> lista)
		{
			if (lista.Count == 0)
				return String.Empty;

			if (lista.Count == 1)
				return lista[0];

			string lastItem = lista.Last();
			lista.RemoveAt(lista.Count - 1);
			return String.Join(", ", lista) + " e " + lastItem;

		}

		public static String Concatenar(string textoStragg, char separador = ';')
		{
			List<String> lista = textoStragg.Split(separador).ToList();

			if (lista.Count == 0)
				return String.Empty;

			if (lista.Count == 1)
				return lista[0];

			string lastItem = lista.Last();
			lista.RemoveAt(lista.Count - 1);
			return String.Join(", ", lista) + " e " + lastItem;
		}

		public static List<Lista> ObterRegularizacaoFundiariaTipoLimite()
		{
			EntitiesDa da = new EntitiesDa();
			return da.ObterRegularizacaoFundiariaTipoLimite();
		}

		public static List<RelacaoTrabalho> ObterRegularizacaoFundiariaRelacaoTrabalho()
		{
			EntitiesDa da = new EntitiesDa();
			return da.ObterRegularizacaoFundiariaRelacaoTrabalho();
		}

		#region Escrever Decimal por Extenso

		internal static string NumeroExtenso(decimal numero)
		{
			int centavos;
			string retorno = string.Empty;

			if (numero == 0)
			{
				return "Zero Reais";
			}

			if (numero > 999999999999)
			{
				return "Valor máximo atingido";
			}

			centavos = (int)Decimal.Round((numero - (long)numero) * 100, MidpointRounding.ToEven);

			numero = (long)numero;

			if (centavos > 0)
			{
				string strCent;
				if (centavos == 1)
				{
					strCent = "centavo";
				}
				else
				{
					strCent = "centavos";
				}

				if (numero == 1)
				{
					retorno = ("Um Real e " + GetDecimal((byte)centavos) + strCent).Replace("  ", " ");
				}
				else if (numero == 0)
				{
					retorno = (GetDecimal((byte)centavos) + strCent).ToString().Replace("  ", " ");
				}
				else
				{
					retorno = (GetInteger(numero) + " Reais e " + GetDecimal((byte)centavos) + strCent).Replace("  ", " ");
				}

				retorno = retorno[0].ToString().ToUpper() + retorno.Substring(1, retorno.Length - 1).ToLower();
			}
			else
			{
				retorno = (numero == 1) ? "Um Real" : GetInteger(numero) + "Reais";
			}

			return retorno;
		}

		//Função auxiliar - Parte decimal a converter
		private static string GetDecimal(byte number)
		{

			if (number == 0)
			{
				return "Zero Reais";
			}
			else if (number >= 1 && number <= 19)
			{
				string[] strArray = {"Um", "Dois", "Três", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove", "Dez",
                                    "Onze", "Doze","Treze", "Quatorze", "Quinze", "Dezesseis", "Dezessete", "Dezoito", "Dezenove"};
				return strArray[number - 1] + " ";
			}
			else if (number >= 20 && number <= 99)
			{
				string[] strArray = { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" };

				if (number % 10 == 0)
				{
					return strArray[number / 10 - 2] + " ";
				}
				else
				{
					return strArray[number / 10 - 2] + " e " + GetDecimal((byte)(number % 10)) + " ";
				}
			}
			else
			{
				return string.Empty;
			}

		}

		private static string GetInteger(Decimal decnumber)
		{

			long number = (long)decnumber;
			if (number < 0)
			{
				return "-" + GetInteger((long)-number);
			}
			else if (number == 0)
			{
				return string.Empty;
			}
			else if (number >= 1 && number <= 19)
			{
				string[] strArray = {"Um", "Dois", "Três", "Quatro", "Cinco", "Seis", "Sete", "Oito", "Nove", "Dez",
                                    "Onze", "Doze", "Treze", "Quatorze", "Quinze", "Dezesseis", "Dezessete", "Dezoito","Dezenove"};
				return strArray[(long)number - 1] + " ";
			}
			else if (number >= 20 && number <= 99)
			{
				string[] strArray = { "Vinte", "Trinta", "Quarenta", "Cinquenta", "Sessenta", "Setenta", "Oitenta", "Noventa" };

				if (number % 10 == 0)
				{
					return strArray[(long)number / 10 - 2];
				}
				else
				{
					return strArray[(long)number / 10 - 2] + " e " + GetInteger(number % 10);
				}
			}
			else if (number == 100)
			{
				return "Cem ";
			}
			else if (number >= 101 && number <= 999)
			{
				string[] strArray = { "Cento", "Duzentos", "Trezentos", "Quatrocentos", "Quinhentos", "Seiscentos", "Setecentos", "Oitocentos", "Novecentos" };
				if (number % 100 == 0)
					return strArray[(long)number / 100 - 1] + " ";
				else
					return strArray[(long)number / 100 - 1] + " e " + GetInteger(number % 100);
			}
			else if (number >= 1000 && number <= 1999)
			{
				if (number % 1000 == 0)
				{
					return "Mil ";
				}
				else if (number % 1000 <= 100)
				{
					return "Mil e " + GetInteger(number % 1000);
				}
				else
				{
					return "Mil, " + GetInteger(number % 1000);
				}
			}
			else if (number >= 2000 && number <= 999999)
			{
				if (number % 1000 == 0)
				{
					return GetInteger((decimal)number / 1000) + " Mil ";
				}
				else if (number % 1000 <= 100)
				{
					return GetInteger((decimal)number / 1000) + " Mil e " + GetInteger(number % 1000);
				}
				else
				{
					return GetInteger((decimal)number / 1000) + " Mil, " + GetInteger(number % 1000);
				}
			}
			else if (number >= 1000000 && number <= 1999999)
			{
				if (number % 1000000 == 0)
				{
					return "Um Milhão ";
				}
				else if (number % 1000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000) + " Milhão e " + GetInteger(number % 1000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000) + " Milhão, " + GetInteger(number % 1000000);
				}
			}
			else if (number >= 2000000 && number <= 999999999)
			{
				if (number % 1000000 == 0)
				{
					return GetInteger((decimal)number / 1000000) + " Milhões ";
				}
				else if (number % 1000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000) + " Milhões e " + GetInteger(number % 1000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000) + " Milhões, " + GetInteger(number % 1000000);
				}
			}
			else if (number >= 1000000000 && number <= 1999999999)
			{
				if (number % 1000000000 == 0)
					return "Um Bilhão ";
				else if (number % 1000000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhão e " + GetInteger(number % 1000000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhão, " + GetInteger(number % 1000000000);
				}
			}
			else
			{
				if (number % 1000000000 == 0)
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhões ";
				}
				else if (number % 1000000000 <= 100)
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhões e " + GetInteger(number % 1000000000);
				}
				else
				{
					return GetInteger((decimal)number / 1000000000) + " Bilhões, " + GetInteger(number % 1000000000);
				}
			}
		}

		#endregion

	}
}