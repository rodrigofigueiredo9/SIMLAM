using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business
{
	public class UnidadeConsolidacaoInternoBus
	{
		#region Propriedades

		UnidadeConsolidacaoInternoDa _da = new UnidadeConsolidacaoInternoDa();

		#endregion

		public UnidadeConsolidacaoInternoBus() { }

		public UnidadeConsolidacao ObterPorEmpreendimento(int empreendimentoInternoId, bool simplificado = false)
		{
			UnidadeConsolidacao unidadeConsolidacao = null;

			try
			{
				unidadeConsolidacao = _da.ObterPorEmpreendimento(empreendimentoInternoId, simplificado);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}

			return unidadeConsolidacao;
		}

		public List<ListaValor> ObterEmpreendimentosResponsaveis()
		{
			List<ListaValor> retorno = null;

			try
			{
				return _da.ObterEmpreendimentosResponsaveis();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}
	}
}