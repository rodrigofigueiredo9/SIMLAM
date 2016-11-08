using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Business
{
	public class ItemValidar
	{
		RoteiroDa _da = new RoteiroDa();

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

		public bool Salvar(Item item)
		{
			if (item.Nome != null)
			{
				item.Nome = item.Nome.Trim();
			}

			if (_da.ExisteItem(item.Id, item.Nome))
			{
				Validacao.Add(Mensagem.Item.ItemExistente);
			}

			if (item.Tipo <= 0)
			{
				Validacao.Add(Mensagem.Item.TipoObrigatorio);
			}

			if (string.IsNullOrWhiteSpace(item.Nome))
			{
				Validacao.Add(Mensagem.Item.NomeObrigatorio);
			}

			if (item.Tipo == (int)eRoteiroItemTipo.ProjetoDigital)
			{
				Validacao.Add(Mensagem.Item.TipoNaoPermitido);
			}

			if (item.Id > 0)
			{
				Editar(item.Id);
			}

			return Validacao.EhValido;
		}

		public bool Editar(int id)
		{
			if (!_da.TipoPermitido(id))
			{
				Validacao.Add(Mensagem.Item.NaoPermiteManipular);
				return Validacao.EhValido;
			}
	
			if (User.IsInRole(ePermissao.ItemRoteiroAssociadoEditar.ToString()))
			{
				return Validacao.EhValido;
			}

			if (_da.ItemUtilizado(id))
			{
				string roteiros = string.Join(", ", _da.ObterRoteiros(new Item() { Id = id }).Select(x => x.Numero).ToList());
				Validacao.Add(Mensagem.Item.ItemAssociado("editado", roteiros));
			}

			List<String> checagens = _da.ObterChecagens(id);
			if (checagens.Count > 0)
			{
				Validacao.Add(Mensagem.Item.ItemAssociadoChecagem("editado", checagens));
			}

			if (_da.ItemUtilizadoAnalise(id))
			{
				Validacao.Add(Mensagem.Item.ItemAssociadoAnalise("editado", _da.ObterAnalises(id).ToList()));
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int id)
		{
			if (!_da.TipoPermitido(id))
			{
				Validacao.Add(Mensagem.Item.NaoPermiteManipular);
				return Validacao.EhValido;
			}
			
			if (_da.ItemUtilizado(id))
			{
				string roteiros = string.Join(", ", _da.ObterRoteiros(new Item() { Id = id }).Select(x => x.Numero).ToList());
				Validacao.Add(Mensagem.Item.ItemAssociado("excluído", roteiros));
			}

			List<String> checagens = _da.ObterChecagens(id);
			if (checagens.Count > 0)
			{
				Validacao.Add(Mensagem.Item.ItemAssociadoChecagem("excluído", checagens));
			}

			if (_da.ItemUtilizadoAnalise(id))
			{
				Validacao.Add(Mensagem.Item.ItemAssociadoAnalise("excluído", _da.ObterAnalises(id).ToList()));
			}

			return Validacao.EhValido;
		}

		public bool Visualizar(int id)
		{
			if (!_da.TipoPermitido(id))
			{
				Validacao.Add(Mensagem.Item.NaoPermiteVisualizar);
				return Validacao.EhValido;
			}

			return true;
		}
	}
}