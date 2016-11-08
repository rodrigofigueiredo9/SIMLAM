using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloVegetal.Business
{
	public class CulturaInternoBus
	{
		#region Propriedades

		CulturaInternoDa _da = new CulturaInternoDa();

		#endregion

		public CulturaInternoBus() { }

		#region Obter/Filtrar

		public List<Lista> ObterLstCultivar(int culturaId)
		{
			List<Lista> retorno = new List<Lista>();
			try
			{
				List<Cultivar> cultivares = _da.ObterCultivares(culturaId);

				cultivares.ForEach(cultivar => { retorno.Add(new Lista() { Id = cultivar.Id.ToString(), Texto = cultivar.Nome }); });
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return retorno;
		}

		public List<Cultivar> ObterCultivares(int culturaId)
		{
			try
			{
				return _da.ObterCultivares(culturaId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Cultivar> ObterCultivares(List<int> culturas)
		{
			try
			{
				return _da.ObterCultivares(culturas);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<CulturaListarResultado> Filtrar(CulturaListarFiltro culturaListarFiltro, Paginacao paginacao)
		{
			try
			{
				Filtro<CulturaListarFiltro> filtro = new Filtro<CulturaListarFiltro>(culturaListarFiltro, paginacao);
				Resultados<CulturaListarResultado> resultados = _da.Filtrar(filtro);

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

		#endregion
	}
}