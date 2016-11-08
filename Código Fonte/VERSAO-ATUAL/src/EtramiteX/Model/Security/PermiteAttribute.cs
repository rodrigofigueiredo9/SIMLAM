using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.Security
{
	[AttributeUsage(AttributeTargets.Class | System.AttributeTargets.Struct | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class PermiteAttribute : FilterAttribute, IAuthorizationFilter
	{
		private string _roles;

		private string[] _rolesSplit = new string[0];

		public string Roles
		{
			get
			{
				return _roles ?? String.Empty;
			}
			set
			{
				_roles = value;
				_rolesSplit = SplitString(value);
			}
		}

		public Object[] RoleArray
		{
			get
			{
				return _rolesSplit;
			}
			set
			{
				_rolesSplit = value.Select(x => x.ToString()).ToArray();
				_roles = String.Join(",", _rolesSplit);
			}
		}

		public ePermissao[] RoleEnum
		{
			get
			{
				return null;
			}
			set
			{
				_rolesSplit = value.Select(x => x.ToString()).ToArray();
				_roles = String.Join(",", _rolesSplit);
			}
		}

		public string[] RoleArrayStr
		{
			get
			{
				return _rolesSplit;
			}
			set
			{
				_rolesSplit = value;
				_roles = String.Join(",", _rolesSplit);
			}
		}

		private ePermiteTipo _tipo = ePermiteTipo.PermissoesOu;
		public ePermiteTipo Tipo
		{
			get { return _tipo; }
			set { _tipo = value; }
		}

		// This method must be thread-safe since it is called by the thread-safe OnCacheAuthorization() method.
		protected virtual bool AuthorizeCore(HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}

			EtramitePrincipal user = httpContext.User as EtramitePrincipal;

			if (user == null)
			{
				return false;
			}

			if (!user.Identity.IsAuthenticated)
			{
				return false;
			}

			if (Tipo == ePermiteTipo.Logado)
			{
				return true;
			}

			bool permite = false;

			if (_rolesSplit.Length > 0 && _rolesSplit.Any(user.IsInRole))
			{
				permite |= true;
			}

			return permite;
		}

		private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
		{
			validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
		}

		public virtual void OnAuthorization(AuthorizationContext filterContext)
		{
			if (filterContext == null)
			{
				throw new ArgumentNullException("filterContext");
			}

			if (AuthorizeCore(filterContext.HttpContext))
			{
				// ** IMPORTANT **
				// Since we're performing authorization at the action level, the authorization code runs
				// after the output caching module. In the worst case this could allow an authorized user
				// to cause the page to be cached, then an unauthorized user would later be served the
				// cached page. We work around this by telling proxies not to cache the sensitive page,
				// then we hook our custom authorization code into the caching mechanism so that we have
				// the final say on whether a page should be served from the cache.

				HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
				cachePolicy.SetProxyMaxAge(new TimeSpan(0));
				cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
			}
			else
			{
				// auth failed, redirect to login page
				if (filterContext.HttpContext.User.Identity.IsAuthenticated &&
					_rolesSplit.Length > 0)
				{
					PermissaoValidar validar = new PermissaoValidar();
					validar.Verificar(_rolesSplit.Select(x => (ePermissao)Enum.Parse(typeof(ePermissao), x)).ToArray());
				}

				if (filterContext.HttpContext.User.Identity.IsAuthenticated && filterContext.HttpContext.Request.IsAjaxRequest())
				{
					filterContext.Result = new JsonResult
					{
						Data = new
						{
							MsgPermissoes = Validacao.Erros
						},
						JsonRequestBehavior = JsonRequestBehavior.AllowGet
					};

					filterContext.HttpContext.Response.StatusCode = 500;
					//filterContext.HttpContext.Response.SubStatusCode = 1;
					filterContext.HttpContext.Response.StatusDescription = Mensagem.Concatenar(Validacao.Erros.Select(x => x.Texto).ToList());
					filterContext.HttpContext.Response.SuppressContent = false;
					filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
				}
				else
				{
					filterContext.Result = new HttpUnauthorizedResult();
					//string url = Validacao.QueryParamSerializer(FormsAuthentication.LoginUrl);
					//filterContext.Result = new RedirectResult(url);
				}
			}
		}

		// This method must be thread-safe since it is called by the caching module.
		protected virtual HttpValidationStatus OnCacheAuthorization(HttpContextBase httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}

			bool isAuthorized = AuthorizeCore(httpContext);
			return (isAuthorized) ? HttpValidationStatus.Valid : HttpValidationStatus.IgnoreThisRequest;
		}

		internal static string[] SplitString(string original)
		{
			if (String.IsNullOrEmpty(original))
			{
				return new string[0];
			}

			var split = from piece in original.Split(',')
						let trimmed = piece.Trim()
						where !String.IsNullOrEmpty(trimmed)
						select trimmed;
			return split.ToArray();
		}
	}
}
