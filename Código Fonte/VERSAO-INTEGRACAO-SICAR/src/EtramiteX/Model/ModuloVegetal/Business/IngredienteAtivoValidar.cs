using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class IngredienteAtivoValidar
	{
		IngredienteAtivoDa _da = new IngredienteAtivoDa();

		internal bool Salvar(ConfiguracaoVegetalItem ingredienteAtivo)
		{
			if (String.IsNullOrEmpty(ingredienteAtivo.Texto))
			{
				Validacao.Add(Mensagem.IngredienteAtivo.IngredienteAtivoObrigatorio);
				return Validacao.EhValido;
			}

			if (Existe(ingredienteAtivo))
			{
				Validacao.Add(Mensagem.IngredienteAtivo.Existente);
			}

			return Validacao.EhValido;
		}

		internal bool AlterarSituacao(ConfiguracaoVegetalItem ingredienteAtivo)
		{
			if (ingredienteAtivo.SituacaoId < 1)
			{
				Validacao.Add(Mensagem.IngredienteAtivo.SituacaoObrigatoria);
			}

			if (string.IsNullOrWhiteSpace(ingredienteAtivo.Motivo))
			{
				Validacao.Add(Mensagem.IngredienteAtivo.MotivoObrigatorio);
			}

			return Validacao.EhValido;
		}

		public bool Existe(ConfiguracaoVegetalItem ingredienteAtivo)
		{
			return _da.Existe(ingredienteAtivo);
		}
	}
}