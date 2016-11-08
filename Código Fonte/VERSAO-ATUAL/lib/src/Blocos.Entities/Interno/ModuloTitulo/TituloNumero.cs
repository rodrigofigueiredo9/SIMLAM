using System;
using System.Text.RegularExpressions;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloNumero
	{
		public int? Inteiro { get; set; }
		public int? Ano { get; set; }

		public bool Automatico { get; set; }
		public bool ReiniciaPorAno { get; set; }
		public int? IniciaEm { get; set; }
		public int? IniciaEmAno { get; set; }

		private string _texto;
		public String Texto
		{
			get
			{
				if ((Inteiro ?? 0) > 0 && (Ano ?? 0) > 0)
				{
					return String.Format("{0}/{1}", Inteiro, Ano);
				}
				else if ((Inteiro ?? 0) > 0)
				{
					return Inteiro.ToString();
				}
				else
				{
					return String.Empty;
				}
			}
			set
			{
				_texto = value;
				int aux = 0;
				if (ValidarMaskNumeroBarraAno(_texto))
				{
					string[] strNumero = value.Split('/');

					int inteiro = 0;
					int ano = 0;

					Int32.TryParse(strNumero[0], out inteiro);
					Int32.TryParse(strNumero[1], out ano);

					Inteiro = (inteiro <= 0) ? (int?)null : inteiro;
					Ano = (ano <= 0) ? (int?)null : ano;
				}
				else
				{
					if (!String.IsNullOrEmpty(_texto) && Regex.IsMatch(_texto, @"^[0-9]{1,7}\/[0-9]*$"))
					{
						string[] strNumero = value.Split('/');

						int numero = 0;
						int ano = 0;

						Int32.TryParse(strNumero[0], out numero);
						Int32.TryParse(strNumero[1], out ano);

						Inteiro = numero;
						Ano = ano;

						return;
					}

					if (Int32.TryParse(_texto, out aux))
					{
						Inteiro = aux;
						Ano = null;
					}
					else
					{
						Inteiro = null;
						Ano = null;
					}
				}
			}
		}

		public static TituloNumero FromDb(object numero, object ano)
		{
			TituloNumero tn = new TituloNumero();
			if (numero != null && !Convert.IsDBNull(numero))
			{
				tn.Inteiro = Convert.ToInt32(numero);
			}
			if (ano != null && !Convert.IsDBNull(ano))
			{
				tn.Ano = Convert.ToInt32(ano);
			}
			return tn;
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

		public bool IsEmpty
		{
			get { return string.IsNullOrEmpty(_texto) && !Inteiro.HasValue && !Ano.HasValue; }
		}
	}
}