using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloProfissao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProfissao.Business
{
	public class ProfissaoValidar
	{
		ProfissaoDa _da = new ProfissaoDa();

		internal bool Salvar(Profissao profissao)
		{
			if (profissao.Id > 0)
			{
				if (!Editar(profissao))
				{
					return Validacao.EhValido;
				}
			}

			if (String.IsNullOrEmpty(profissao.Texto))
			{
				Validacao.Add(Mensagem.Profissao.ProfissaoObrigatorio);
			}

			if (Existe(profissao))
			{
				Validacao.Add(Mensagem.Profissao.Existente);
			}

			return Validacao.EhValido;
		}

		public bool Editar(Profissao profissao)
		{
			if (profissao.OrigemId == (int)eProfissaoOrigem.CBO)
			{
				Validacao.Add(Mensagem.Profissao.EditarSomenteAdministrador);
				return Validacao.EhValido;
			}

			return Validacao.EhValido;
		}

		public bool Existe(Profissao profissao)
		{
			return _da.Existe(profissao);
		}
	}
}