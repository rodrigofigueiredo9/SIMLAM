using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloTitulo.Business
{
	public class TituloBus
	{
		TituloDa _da = new TituloDa();

		public Resultados<Titulo> Filtrar(TituloFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<TituloFiltro> filtro = new Filtro<TituloFiltro>(filtrosListar, paginacao);
				Resultados<Titulo> resultados = _da.Filtrar(filtro);

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

		public List<Situacao> ObterSituacoes()
		{
			return _da.ObterSituacoes();
		}
	}
}
