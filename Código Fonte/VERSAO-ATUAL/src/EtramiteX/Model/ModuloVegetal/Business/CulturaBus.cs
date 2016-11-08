using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class CulturaBus
	{
		#region Propriedades

		CulturaValidar _validar = new CulturaValidar();
		CulturaDa _da = new CulturaDa();

		#endregion

		public CulturaBus() { }

		public void Salvar(Cultura cultura, BancoDeDados banco = null)
		{
			try
			{
				if (!_validar.Salvar(cultura))
				{
					return;
				}

				cultura.Nome = cultura.Nome.DeixarApenasUmEspaco();
				cultura.LstCultivar.ForEach(x => x.Nome.DeixarApenasUmEspaco());

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(cultura, bancoDeDados);
					bancoDeDados.Commit();

					Validacao.Add(Mensagem.Cultura.CulturaSalvaSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public Cultura Obter(int id)
		{
			Cultura cultura = null;

			try
			{
				cultura = _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return cultura;
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

		public List<Lista> ObterLstCultivar(int culturaId)
		{
			try
			{
				return _da.ObterLstCultivar(culturaId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}