using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class FormaApresentacaoValidar
	{
		FormaApresentacaoDa _da = new FormaApresentacaoDa();

		internal bool Salvar(ConfiguracaoVegetalItem formaApresentacao)
		{
			if (String.IsNullOrEmpty(formaApresentacao.Texto))
			{
				Validacao.Add(Mensagem.FormaApresentacao.FormaApresentacaoObrigatorio);
				return Validacao.EhValido;
			}

			if (Existe(formaApresentacao))
			{
				Validacao.Add(Mensagem.FormaApresentacao.Existente);
			}

			return Validacao.EhValido;
		}

		public bool Existe(ConfiguracaoVegetalItem formaApresentacao)
		{
			return _da.Existe(formaApresentacao);
		}
	}
}
