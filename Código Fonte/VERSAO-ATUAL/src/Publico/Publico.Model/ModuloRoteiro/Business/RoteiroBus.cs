using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Data;
using Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Entities;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloRoteiro.Business
{
	public class RoteiroBus
	{
		RoteiroDa _da = new RoteiroDa();

		public Resultados<Roteiro> Filtrar(ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltro> filtro = new Filtro<ListarFiltro>(filtrosListar, paginacao);
				Resultados<Roteiro> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}
