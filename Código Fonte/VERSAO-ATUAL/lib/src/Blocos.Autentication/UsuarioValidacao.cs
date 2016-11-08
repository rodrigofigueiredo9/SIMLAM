using System.Text.RegularExpressions;

namespace Tecnomapas.Blocos.Autenticacao
{
	public class UsuarioValidacao
	{
		public static bool FormatoLogin(string login)
		{
			return Regex.IsMatch(login, @"^([a-zA-Z]+(.*))$", RegexOptions.ECMAScript);
		}
	}
}