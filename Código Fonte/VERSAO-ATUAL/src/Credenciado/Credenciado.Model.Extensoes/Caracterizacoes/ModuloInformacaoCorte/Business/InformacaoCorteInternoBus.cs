using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Entities;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business
{
	public class InformacaoCorteInternoBus
	{
		#region Propriedades

		InformacaoCorteInternoDa _da = new InformacaoCorteInternoDa();

		#endregion

		public InformacaoCorte Obter(int id, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = null;
			try
			{
				caracterizacao = _da.Obter(id, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public InformacaoCorte ObterPorEmpreendimento(int empreendimentoInternoId, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimentoInternoId, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.UnidadeProducao
				};
			}
		}
	}
}