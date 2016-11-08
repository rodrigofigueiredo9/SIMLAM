using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class ClasseUsoValidar
	{
		ClasseUsoDa _da = new ClasseUsoDa();

		internal bool Salvar(ConfiguracaoVegetalItem classeUso)
		{
			if (String.IsNullOrEmpty(classeUso.Texto))
			{
				Validacao.Add(Mensagem.ClasseUso.ClasseUsoObrigatorio);
				return Validacao.EhValido;
			}

			if (Existe(classeUso))
			{
				Validacao.Add(Mensagem.ClasseUso.Existente);
			}

			return Validacao.EhValido;
		}

		public bool Existe(ConfiguracaoVegetalItem classeUso)
		{
			return _da.Existe(classeUso);
		}
	}
}
