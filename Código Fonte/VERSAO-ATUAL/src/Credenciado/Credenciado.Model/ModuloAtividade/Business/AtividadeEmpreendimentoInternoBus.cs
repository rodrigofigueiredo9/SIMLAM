using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business
{
	public class AtividadeEmpreendimentoInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		AtividadeEmpreendimentoInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public AtividadeEmpreendimentoInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new AtividadeEmpreendimentoInternoDa(UsuarioInterno);
		}

		public Resultados<EmpreendimentoAtividade> Filtrar(AtividadeListarFiltro FiltrosListar, Paginacao paginacao)
		{
			try
			{
				Dictionary<String, DadoFiltro> filtros = new Dictionary<String, DadoFiltro>();

				if (!String.IsNullOrEmpty(FiltrosListar.AtividadeNome))
				{
					filtros.Add("atividade", new DadoFiltro(DbType.String, FiltrosListar.AtividadeNome));
				}

				if (!String.IsNullOrEmpty(FiltrosListar.Secao))
				{
					filtros.Add("secao", new DadoFiltro(DbType.String, FiltrosListar.Secao));
				}

				if (FiltrosListar.Divisao > 0)
				{
					filtros.Add("divisao", new DadoFiltro(DbType.Int32, FiltrosListar.Divisao));
				}

				if (FiltrosListar.SetorId > 0)
				{
					filtros.Add("setor", new DadoFiltro(DbType.Int32, FiltrosListar.SetorId));
				}

				if (!String.IsNullOrEmpty(FiltrosListar.CNAE))
				{
					filtros.Add("cnae", new DadoFiltro(DbType.String, FiltrosListar.CNAE));
				}

				string ordenarValor = string.Empty;
				switch (paginacao.OrdenarPor)
				{
					case 1:
						ordenarValor = "cod_cnae";
						break;

					case 2:
						ordenarValor = "atividade";
						break;

					default:
						ordenarValor = "cod_cnae";
						break;
				}

				filtros.Add("menor", new DadoFiltro(DbType.Int32, ((paginacao.PaginaAtual) * paginacao.QuantPaginacao) - (paginacao.QuantPaginacao - 1)));
				filtros.Add("maior", new DadoFiltro(DbType.Int32, ((paginacao.PaginaAtual) * paginacao.QuantPaginacao)));
				filtros.Add("ordenar", new DadoFiltro(DbType.String, ordenarValor));

				return _da.Filtrar(filtros);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}