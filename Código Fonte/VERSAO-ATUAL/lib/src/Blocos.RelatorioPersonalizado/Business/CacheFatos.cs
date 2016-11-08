using System;
using System.Web;
using System.Web.Caching;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Business
{
	class CacheFatos	
	{
		public static Cache Cache { get { return HttpContext.Current.Cache; } }

		public static TResult Obter<TResult>(string chave, Func<TResult> acao)
		{
			var objeto = Cache.Get(chave);
			if (objeto == null)
			{
				objeto = acao();
				Cache.Insert(chave, objeto);
			}
			return (TResult) objeto;
		}

		public static TResult Obter<TParam, TResult>(string chave, Func<TParam, TResult> acao, TParam param)
		{
			var objeto = Cache.Get(chave);
			if (objeto == null)
			{
				objeto = acao(param);
				Cache.Insert(chave, objeto);
			}
			return (TResult)objeto;
		}
	}
}
