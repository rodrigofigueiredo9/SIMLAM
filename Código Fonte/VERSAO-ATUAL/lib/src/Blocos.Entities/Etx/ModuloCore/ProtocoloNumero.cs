using System;
using System.Text.RegularExpressions;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class ProtocoloNumero
	{
		public int Id { get; set; }

		public Int32? Ano
		{
			get
			{
				if (string.IsNullOrWhiteSpace(NumeroTexto)) 
					return 0;
				try
				{
					return Convert.ToInt32(Regex.Replace(NumeroTexto.Split('/').GetValue(1).ToString().Trim(), "\\s", ""));
				}
				catch
				{
					return 0;
				}
			}
		}

		public Int32? Numero
		{
			get
			{
				if (string.IsNullOrWhiteSpace(NumeroTexto)) 
					return 0;
				try
				{
					return Convert.ToInt32(Regex.Replace(NumeroTexto.Split('/').GetValue(0).ToString().Trim(), "\\s", ""));
				}
				catch
				{
					return 0;
				}
			}
		}

		public String NumeroTexto { get; set; }

		public Boolean IsProcesso { get; set; }

		public Int32? Tipo { get; set; }

		public String TipoTexto { get; set; }

		public ProtocoloNumero()
		{
			IsProcesso = true;
		}

		public ProtocoloNumero(String numero)
		{
			IsProcesso = true;
			NumeroTexto = numero;
		}

		public ProtocoloNumero(String numero, int id, bool isProcesso)
		{
			IsProcesso = true;
			NumeroTexto = numero;
			IsProcesso = isProcesso;
		}

		public bool IsValido { get { return Numero > 0 && Ano > 0; } }
	}
}
