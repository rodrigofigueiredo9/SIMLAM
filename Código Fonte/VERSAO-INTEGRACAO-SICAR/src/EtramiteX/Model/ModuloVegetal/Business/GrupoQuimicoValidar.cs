using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class GrupoQuimicoValidar
	{
		GrupoQuimicoDa _da = new GrupoQuimicoDa();

		internal bool Salvar(ConfiguracaoVegetalItem grupoQuimico)
		{			
			if (String.IsNullOrEmpty(grupoQuimico.Texto))
			{
				Validacao.Add(Mensagem.GrupoQuimico.GrupoQuimicoObrigatorio);
				return Validacao.EhValido;
			}

			if (Existe(grupoQuimico))
			{
				Validacao.Add(Mensagem.GrupoQuimico.Existente);
			}

			return Validacao.EhValido;
		}

		public bool Existe(ConfiguracaoVegetalItem grupoQuimico)
		{
			return _da.Existe(grupoQuimico);
		}
	}
}
