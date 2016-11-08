using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class PericulosidadeAmbientalValidar
	{
		PericulosidadeAmbientalDa _da = new PericulosidadeAmbientalDa();

		internal bool Salvar(ConfiguracaoVegetalItem periculosidadeAmbiental)
		{
			if (String.IsNullOrEmpty(periculosidadeAmbiental.Texto))
			{
				Validacao.Add(Mensagem.PericulosidadeAmbiental.PericulosidadeAmbientalObrigatorio);
                return Validacao.EhValido;

			}

			if (Existe(periculosidadeAmbiental))
			{
				Validacao.Add(Mensagem.PericulosidadeAmbiental.Existente);
			}

			return Validacao.EhValido;
		}

		public bool Existe(ConfiguracaoVegetalItem periculosidadeAmbiental)
		{
			return _da.Existe(periculosidadeAmbiental);
		}
	}
}
