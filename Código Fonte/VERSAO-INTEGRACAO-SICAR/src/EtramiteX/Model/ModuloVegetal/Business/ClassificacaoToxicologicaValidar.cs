using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class ClassificacaoToxicologicaValidar
	{
		ClassificacaoToxicologicaDa _da = new ClassificacaoToxicologicaDa();

		internal bool Salvar(ConfiguracaoVegetalItem classificacaoToxicologica)
		{
			if (String.IsNullOrEmpty(classificacaoToxicologica.Texto))
			{
				Validacao.Add(Mensagem.ClassificacaoToxicologica.ClassificacaoToxicologicaObrigatorio);
				return Validacao.EhValido;
			}

			if (Existe(classificacaoToxicologica))
			{
				Validacao.Add(Mensagem.ClassificacaoToxicologica.Existente);
			}

			return Validacao.EhValido;
		}

		public bool Existe(ConfiguracaoVegetalItem classificacaoToxicologica)
		{
			return _da.Existe(classificacaoToxicologica);
		}
	}
}
