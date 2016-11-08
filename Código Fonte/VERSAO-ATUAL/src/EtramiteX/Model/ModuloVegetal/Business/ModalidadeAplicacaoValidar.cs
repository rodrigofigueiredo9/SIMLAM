using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class ModalidadeAplicacaoValidar
	{
		ModalidadeAplicacaoDa _da = new ModalidadeAplicacaoDa();

		internal bool Salvar(ConfiguracaoVegetalItem modalidadeAplicacao)
		{
			if (String.IsNullOrEmpty(modalidadeAplicacao.Texto))
			{
				Validacao.Add(Mensagem.ModalidadeAplicacao.ModalidadeAplicacaoObrigatorio);
				return Validacao.EhValido;
			}

			if (Existe(modalidadeAplicacao))
			{
				Validacao.Add(Mensagem.ModalidadeAplicacao.Existente);
			}

			return Validacao.EhValido;
		}

		public bool Existe(ConfiguracaoVegetalItem modalidadeAplicacao)
		{
			return _da.Existe(modalidadeAplicacao);
		}
	}
}
