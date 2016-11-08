using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloAgrotoxico.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Business
{
	public class IngredienteAtivoBus
	{
		#region Propriedades

		IngredienteAtivoDa _da = new IngredienteAtivoDa();
		IngredienteAtivoValidar _validar = new IngredienteAtivoValidar();

		#endregion

		#region Ações DML

		public bool Salvar(ConfiguracaoVegetalItem ingredienteAtivo)
		{
			try
			{
				if (_validar.Salvar(ingredienteAtivo))
				{
					ingredienteAtivo.Texto = ingredienteAtivo.Texto.DeixarApenasUmEspaco();

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(ingredienteAtivo, bancoDeDados);

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.IngredienteAtivo.Salvar);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool AlterarSituacao(ConfiguracaoVegetalItem ingredienteAtivo)
		{
			try
			{
				if (_validar.AlterarSituacao(ingredienteAtivo))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.AlterarSituacao(ingredienteAtivo, bancoDeDados);

						AgrotoxicoBus agrotoxicoBus = new AgrotoxicoBus();
						agrotoxicoBus.AlterarSituacao(ingredienteAtivo.Id, (eIngredienteAtivoSituacao)ingredienteAtivo.SituacaoId, bancoDeDados);

						Validacao.Add(Mensagem.IngredienteAtivo.AlterarSituacao);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter/Listar

		public ConfiguracaoVegetalItem Obter(int id)
		{
			try
			{
				return _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public void ObterValores(List<ConfiguracaoVegetalItem> itens)
		{
			try
			{
				_da.ObterValores(itens);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public Resultados<ConfiguracaoVegetalItem> Filtrar(ConfiguracaoVegetalItem filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ConfiguracaoVegetalItem> filtro = new Filtro<ConfiguracaoVegetalItem>(filtrosListar, paginacao);
				Resultados<ConfiguracaoVegetalItem> resultados = _da.Filtrar(filtro);

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