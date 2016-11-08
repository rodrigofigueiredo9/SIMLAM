using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Business
{
	public class FichaFundiariaValidar
	{
		public bool Salvar(FichaFundiaria ficha)
		{
			if (String.IsNullOrWhiteSpace(ficha.Requerente.Nome))
			{
				Validacao.Add(Mensagem.FichaFundiaria.RequerenteNomeObrigatorio);
			}
			return Validacao.EhValido;
		}
	}
}