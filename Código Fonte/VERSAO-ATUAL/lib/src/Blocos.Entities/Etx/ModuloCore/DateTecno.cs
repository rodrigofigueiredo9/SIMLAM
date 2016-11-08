using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloCore
{
	public class DateTecno
	{
		private DateTime? _data;
		public DateTime? Data
		{
			get
			{
				return _data;
			}
			set
			{
				_data = value;
				if (_data == null || !_data.HasValue) // vazio
				{
					_isEmpty = true;
					_isValido = false;
					_dataTexto = string.Empty;
					_dataHoraTexto = string.Empty;
				}
				else // válido
				{
					_isEmpty = false;
					_isValido = true;
					_dataTexto = _data.Value.ToShortDateString();
					_dataHoraTexto = _data.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
				}
			}
		}

		private String _dataTexto = String.Empty;
		public String DataTexto
		{
			get { return _dataTexto; }
			set
			{
				_dataTexto = value;

				if (String.IsNullOrEmpty(_dataTexto)) // vazio
				{
					_data = null;
					_isEmpty = true;
					_isValido = false;
				}
				else
				{
					_isEmpty = false;

					try // válido
					{
						_dataTexto = _dataTexto.Length > 10 ? _dataTexto.Substring(0, 10) : _dataTexto;

						if (_regex.IsMatch(_dataTexto))
						{
							_data = DateTime.Parse(_dataTexto);
							_isValido = true;
							_dataTexto = _data.Value.ToShortDateString();
						}
					}
					catch // inválido
					{
						_isValido = false;
						_data = null;
					}
				}
			}
		}

		private String _dataHoraTexto = String.Empty;
		public String DataHoraTexto
		{
			get { return _dataHoraTexto; }
			set
			{
				_dataHoraTexto = value;

				if (String.IsNullOrEmpty(_dataHoraTexto)) // vazio
				{
					_data = null;
					_isEmpty = true;
					_isValido = false;
				}
				else
				{
					_isEmpty = false;

					try // válido
					{
						_data = DateTime.Parse(_dataTexto);
						_isValido = true;
						_dataHoraTexto = _data.Value.ToString("dd/MM/yyyy HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
					}
					catch // inválido
					{
						_isValido = false;
						_data = null;
					}
				}
			}
		}

		private bool _isValido;
		public bool IsValido { get { return _isValido; } set { _isValido = value; } }
		private bool _isEmpty;
		public bool IsEmpty { get { return _isEmpty; } set { _isEmpty = value; } }
		private Regex _regex;

		public DateTecno()
		{
			_data = null;
			_isEmpty = true;
			_isValido = false;
			_dataTexto = string.Empty;
			_dataHoraTexto = string.Empty;

			//Expressao: Valida integridade minima de _dataTexto. Ex: 02/03 nao eh valido e 02/03/2014 eh valido.
			_regex = new Regex(@"^(((0[1-9]|[12]\d|3[01])\/(0[13578]|1[02])\/((19|[2-9]\d)\d{2}))|((0[1-9]|[12]\d|30)\/(0[13456789]|1[012])\/((19|[2-9]\d)\d{2}))|((0[1-9]|1\d|2[0-8])\/02\/((19|[2-9]\d)\d{2}))|(29\/02\/((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))))$", RegexOptions.IgnoreCase);
		}
	}
}
