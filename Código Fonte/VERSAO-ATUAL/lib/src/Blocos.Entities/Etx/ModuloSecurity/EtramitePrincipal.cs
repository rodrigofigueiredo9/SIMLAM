using System;
using System.Linq;
using System.Security.Principal;

namespace Tecnomapas.Blocos.Entities.Etx.ModuloSecurity
{
	public class EtramitePrincipal<T>: EtramitePrincipal
	{
		public EtramitePrincipal(IIdentity identity, T[] roles): base(identity, new String[]{})
		{
			_roles = roles.Select(x => x.ToString()).ToArray();
		}
	}

	public class EtramitePrincipal : IPrincipal
	{
		protected String[] _roles;

		public EtramitePrincipal(IIdentity identity, String[] roles)
		{
			Identity = identity;
			_roles = roles;
		}

		public IIdentity Identity
		{
			get;
			private set;
		}

		public EtramiteIdentity EtramiteIdentity
		{
			get { return Identity as EtramiteIdentity; }
		}

		public bool IsInRole(string role)
		{
			return _roles.Contains(role);
		}

		public bool IsInRole<TPermissao>(TPermissao role)
		{
			return IsInRole(role.ToString());
		}

		public bool IsInAnyRole(string roles)
		{
			string[] rolesArray = SplitString(roles);
			if (rolesArray.Length > 0 && !rolesArray.Any(IsInRole))
			{
				return false;
			}
			return true;
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
