using System;
using System.Web;
using System.Web.Caching;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class GerenciadorCache
	{
		internal static Cache Cache
		{
			get { return HttpContext.Current?.Cache; }
		}

		public static Object PapeisFuncionario
		{
			get { return Cache["PapeisFuncionario"]; }
			set { Cache["PapeisFuncionario"] = value; }
		}

		public static Object PapeisAdministrador
		{
			get { return Cache["PapeisAdministrador"]; }
			set { Cache["PapeisAdministrador"] = value; }
		}

		public static Object PermissaoGrupo
		{
			get { return Cache["PermissaoGrupo"]; }
			set { Cache["PermissaoGrupo"] = value; }
		}

		public static void SetCache(String key, Object valor)
		{
			Cache[key] = valor;
		}

		public static bool Get<T>(string key, out T result)
		{
			try
			{
				object item = Cache[key];
				if (item == null)
				{
					result = default(T);
					return false;
				}
				result = (T)item;
				return true;
			}
			catch
			{
				result = default(T);
				return false;
			}
		}
	}
}