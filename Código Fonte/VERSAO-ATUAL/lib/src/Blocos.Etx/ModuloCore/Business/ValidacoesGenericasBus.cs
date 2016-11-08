using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Business
{
	public class ValidacoesGenericasBus
	{
		public static bool Email(string email)
		{
			try
			{
				var addr = new MailAddress(email);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool Cpf(string cpf)
		{
			if (String.IsNullOrEmpty(cpf) || !Regex.IsMatch(cpf, @"^\d{3}.\d{3}\.\d{3}\-\d{2}$", RegexOptions.ECMAScript))
			{
				return false;
			}

			cpf = cpf.Replace(".", string.Empty).Replace("-", string.Empty);
			string[] triviais = new string[]
				                    {
					                    "00000000000", "11111111111", "22222222222", "33333333333",
					                    "44444444444", "55555555555", "66666666666", "77777777777",
					                    "88888888888", "99999999999"
				                    };

			if ((cpf.Length != 11) || (Array.IndexOf(triviais, cpf) != -1))
			{
				return false;
			}

			int soma = 0;

			for (int i = 0; i < 9; i++)
			{
				soma += Int32.Parse(cpf[i].ToString())*(10 - i);
			}

			soma = soma - (Math.Abs(soma/11)*11);

			if ((soma < 2) && (((soma == 0) || (soma == 1)) && (0 == Int32.Parse(cpf[9].ToString()))))
			{
				soma = 0;
			}
			else if ((11 - soma) == Int32.Parse(cpf[9].ToString()))
			{
				soma = 0;
			}
			else
			{
				return false;
			}

			for (int i = 0; i < 9; i++)
			{
				soma += Int32.Parse(cpf[i].ToString())*(11 - i);
			}

			soma += Int32.Parse(cpf[9].ToString())*2;
			soma = soma - (Math.Abs(soma/11)*11);

			if ((soma < 2) && (((soma == 0) || (soma == 1)) && (0 == Int32.Parse(cpf[10].ToString()))))
			{
				return true;
			}
			else if ((11 - soma) == Int32.Parse(cpf[10].ToString()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static bool Cnpj(string cnpj)
		{

			if (String.IsNullOrEmpty(cnpj) ||
			    !Regex.IsMatch(cnpj, @"(^(\d{2}.\d{3}.\d{3}/\d{4}-\d{2})|(\d{14})$)", RegexOptions.ECMAScript))
			{
				return false;
			}
			else
			{
				cnpj = cnpj.Trim().Replace(".", string.Empty).Replace("-", string.Empty).Replace("/", string.Empty);

				if (cnpj.Length == 14)
				{
					if (cnpj == "00000000000000")
					{
						return false;
					}

					int[] multiplicador1 = new int[12] {5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};
					int[] multiplicador2 = new int[13] {6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2};
					int soma = 0;
					string digito;

					for (int i = 0; i < 12; i++)
					{
						soma += int.Parse(cnpj[i].ToString())*multiplicador1[i];
					}

					soma = (soma%11);

					if (soma < 2)
					{
						digito = "0";
					}
					else
					{
						digito = Convert.ToString(11 - soma);
					}

					string tempCnpj = cnpj.Substring(0, 12) + digito;
					soma = 0;

					for (int i = 0; i < 13; i++)
					{
						soma += int.Parse(tempCnpj[i].ToString())*multiplicador2[i];
					}

					soma = (soma%11);

					if (soma < 2)
					{
						digito += "0";
					}
					else
					{
						digito += Convert.ToString(11 - soma);
					}

					return cnpj.EndsWith(digito);
				}
				else
				{
					return false;
				}
			}
		}

		public static bool ValidarCep(String cep)
		{
			try
			{
				return new Regex("^[0-9]{2}\\.[0-9]{3}-[0-9]{3}$").IsMatch(cep);
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidarData(String data)
		{
			try
			{
				DateTime.Parse(data).ToString("dd/MM/yyyy");
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidarData(DateTime data)
		{
			return ValidarData(data.ToShortDateString());
		}

		public static DateTime? ParseData(String dataTexto)
		{
			if (String.IsNullOrEmpty(dataTexto))
			{
				return DateTime.MinValue;
			}
			else if (ValidarData(dataTexto) && dataTexto != DateTime.MinValue.ToShortDateString())
			{
				return DateTime.Parse(dataTexto);
			}
			else
			{
				return null;
			}
		}

		public static bool ValidarDouble(String texto)
		{
			try
			{
				if (String.IsNullOrEmpty(texto))
				{
					return false;
				}
				texto = texto.Trim().Replace(".", ",");

				Convert.ToDouble(texto);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidarMaskNumeroBarraAno(String numero)
		{
			try
			{
				if (String.IsNullOrWhiteSpace(numero))
				{
					return false;
				}

				return Regex.IsMatch(numero, @"^[0-9]{1,7}\/[0-9]{4}$");
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidarMaskHora(string hora)
		{
			if (String.IsNullOrEmpty(hora) ||
			    !Regex.IsMatch(hora, @"^([0-1][0-9]|2[0-3])(:[0-5][0-9]){2}$", RegexOptions.ECMAScript))
			{
				return false;
			}
			return true;
		}

		public static bool ValidarDecimal(String numero, Int32 numeroInteiro, Int32 casasDecimais)
		{
			try
			{
				if (String.IsNullOrWhiteSpace(numero))
				{
					return false;
				}

				return Regex.IsMatch(numero, @"^[+]?([0-9]{1," + numeroInteiro + @"}(\,[0-9]{1," + casasDecimais + "})?)$");
			}
			catch
			{
				return false;
			}
		}

		public static bool ValidarNumero(String numero, Int32 numeroInteiro)
		{
			try
			{
				if (String.IsNullOrWhiteSpace(numero))
				{
					return false;
				}

				return Regex.IsMatch(numero, @"^([0-9]{1," + numeroInteiro + @"})$");
			}
			catch
			{
				return false;
			}
		}

		public static bool DataMensagem(DateTecno data, string campo, string tipoData, bool superiorAtual = true)
		{
			if (data.IsEmpty)
			{
				Validacao.Add(Mensagem.Padrao.DataObrigatoria(campo, tipoData));
				return false;
			}
			else
			{
				if (!data.IsValido)
				{
					Validacao.Add(Mensagem.Padrao.DataInvalida(campo, tipoData));
					return false;
				}
				else if (superiorAtual && data.Data > DateTime.Today.AddDays(1).Subtract(TimeSpan.FromSeconds(1)))
				{
					Validacao.Add(Mensagem.Padrao.DataIgualMenorAtual(campo, tipoData));
					return false;
				}
				else
				{
					return true;
				}
			}
		}
		public static eUnidadeProducaoTipoProducao ObterTipoProducao(string unidadeMedida)
		{
			var unidadeMedidaId = 0;
			switch (unidadeMedida.ToUpper())
			{
				case "UND.":
					unidadeMedidaId = (int)eUnidadeProducaoUnidadeMedida.Und;
					break;
				case "T":
					unidadeMedidaId = (int)eUnidadeProducaoUnidadeMedida.T;
					break;
				case "M³":
					unidadeMedidaId = (int)eUnidadeProducaoUnidadeMedida.M3;
					break;

			}
			return ObterTipoProducao(unidadeMedidaId);
		}

		public static eUnidadeProducaoTipoProducao ObterTipoProducao(int unidadeMedidaId)
		{

			eUnidadeProducaoTipoProducao tipoProducao = eUnidadeProducaoTipoProducao.Nulo;
			switch ((eUnidadeProducaoUnidadeMedida)unidadeMedidaId)
			{
				case eUnidadeProducaoUnidadeMedida.M3:
					tipoProducao = eUnidadeProducaoTipoProducao.Madeira;
					break;
				case eUnidadeProducaoUnidadeMedida.T:
					tipoProducao = eUnidadeProducaoTipoProducao.Frutos;
					break;
				case eUnidadeProducaoUnidadeMedida.Und:
					tipoProducao = eUnidadeProducaoTipoProducao.MaterialPropagacao;
					break;
			}

			return tipoProducao;
		}
	}
}