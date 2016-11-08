using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Security
{
	public class PermissaoValidar
	{
		private PermissaoDa _da = new PermissaoDa();

		public EtramitePrincipal User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal);
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		public bool Validar(ePermissao permissao)
		{
			if (!User.IsInRole(permissao.ToString()))
			{
				Validacao.Add(Mensagem.Sistema.SemPermissao(_da.ObterGrupoNome(new List<ePermissao>() { permissao })));
			}
			return Validacao.EhValido;
		}

		public bool ValidarAny(ePermissao[] permissao, bool msg = true)
		{
			if (permissao.Length == 1)
			{
				return Validar(permissao.First());
			}
			
			String strPermissao = String.Join(",", permissao);
			if (!User.IsInAnyRole(strPermissao))
			{
				string nomes = String.Join(",", permissao.Select(x => _da.ObterNome(x)).ToArray());

				if (msg)
				{
					Validacao.Add(Mensagem.Sistema.SemPermissoes(nomes));
				}
				return false;
			}

			return true;
		}

		public void Verificar(ePermissao[] permissoes)
		{
			List<ePermissao> lstPermissao = permissoes.Where(x => !User.IsInRole(x.ToString())).ToList();
			List<String> lstPermissaoNome = _da.ObterGrupoNome(lstPermissao);

			if (lstPermissaoNome == null || lstPermissaoNome.Count == 0)
			{
				Validacao.Add(Mensagem.Sistema.PermissaoNaoEncontrada(permissoes.Select(x => x.ToString()).ToList()));
				return;
			}
			Validacao.Add(Mensagem.Sistema.SemPermissao(lstPermissaoNome));
		}
	}
}
