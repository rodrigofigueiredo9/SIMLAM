using System;
using System.Text.RegularExpressions;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{	
	public class TituloNumeroEsp
	{
		public int? Inteiro { get; set; }
		public int? Ano { get; set; }
		private string _texto;
		public String Texto
		{
			get
			{
				if ((Inteiro ?? 0) > 0 && (Ano ?? 0) > 0)
				{
					return String.Format("{0}/{1}", Inteiro, Ano);
				}
				else
				{
					return String.Empty;
				}
			}
			set
			{
				_texto = value;
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
					Inteiro = null;
					Ano = null;
				}
			}
		}

		public bool IsEmpty
		{
			get { return String.IsNullOrWhiteSpace(_texto) || !(Inteiro.HasValue && Ano.HasValue); }
		}

		public bool IsFormatoValido
		{
			get { return ValidarMaskNumeroBarraAno(_texto); }
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
	}
}