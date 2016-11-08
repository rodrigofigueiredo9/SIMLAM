using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class PragaBus
	{

		#region Propriedades

		PragaValidar _validar = new PragaValidar();
		PragaDa _da = new PragaDa();
		#endregion

		public PragaBus()
		{

		}

		#region DML
		public void Salvar(Praga praga, BancoDeDados banco = null)
		{
			try
			{
				if (!_validar.Salvar(praga))
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(praga, bancoDeDados);
					bancoDeDados.Commit();

					Validacao.Add(Mensagem.Praga.PragaSalvaComSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void AssociarCulturas(Praga praga, BancoDeDados banco = null)
		{
			try
			{
				if (!_validar.AssociarCulturas(praga))
				{
					return;
				}

				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.AssociarCulturas(praga, bancoDeDados);
					bancoDeDados.Commit();

					Validacao.Add(Mensagem.Praga.CulturasAssociadasComSucesso);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter/Filtrar

		public Praga Obter(int id)
		{
			Praga praga = null;

			try
			{
				praga = _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return praga;
		}

		public List<Praga> ObterPragas(int idCultura)
		{
			try
			{
				return _da.ObterPragas(idCultura);
			}
			catch(Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new List<Praga>();
		}

		public Resultados<Praga> Filtrar(Praga filtro, Paginacao paginacao)
		{
			try
			{
				Filtro<Praga> filtros = new Filtro<Praga>(filtro, paginacao);
				Resultados<Praga> resultados = _da.Filtrar(filtros);

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

		public List<Cultura> ObterCulturas(int pragaId)
		{
			List<Cultura> culturas = null;
			try
			{
				culturas = _da.ObterCulturas(pragaId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return culturas;
		}

		#endregion
	}
}