using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Data;
using UnidadeConsolidacaoCredBus = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Business
{
	public class UnidadeConsolidacaoBus : ICaracterizacaoBus
	{
		#region Propriedades

		UnidadeConsolidacaoValidar _validar = null;
		UnidadeConsolidacaoDa _da = new UnidadeConsolidacaoDa();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.UnidadeConsolidacao
				};
			}
		}

		public UnidadeConsolidacaoBus()
		{
			_validar = new UnidadeConsolidacaoValidar();
		}

		#region Comandos DML

		public bool Salvar(UnidadeConsolidacao unidade)
		{
			try
			{
				if (!_validar.Salvar(unidade))
				{
					return Validacao.EhValido;
				}

				if (!unidade.PossuiCodigoUC)
				{
					if (unidade.Id < 1)
					{
						int codigo = _da.ObterSequenciaCodigoUC();
						EmpreendimentoCaracterizacao empreendimento = _busCaracterizacao.ObterEmpreendimentoSimplificado(unidade.Empreendimento.Id);

						unidade.CodigoUC = Convert.ToInt64(empreendimento.MunicipioIBGE.ToString() + codigo.ToString("D4"));
					}
					else
					{
						unidade.CodigoUC = ObterPorEmpreendimento(unidade.Empreendimento.Id, true).CodigoUC;
					}
				}

				foreach (var aux in unidade.ResponsaveisTecnicos)
				{
					aux.CFONumero = aux.CFONumero.Split('-').GetValue(0).ToString();
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Salvar(unidade, bancoDeDados);

					Validacao.Add(Mensagem.UnidadeConsolidacao.UnidadeConsolidacaoSalvaSucesso);

					bancoDeDados.Commit();
				}

			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		public bool Excluir(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				if (!_caracterizacaoValidar.Basicas(empreendimento))
				{
					return Validacao.EhValido;
				}

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.UnidadeConsolidacao))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.UnidadeConsolidacao.ExcluidoSucesso);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public UnidadeConsolidacao ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{

			UnidadeConsolidacao caracterizacao = new UnidadeConsolidacao();
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimentoId, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public List<ListaValor> ObterListaUnidadeMedida()
		{
			List<ListaValor> retorno = null;
			try
			{
				return _da.ObterListaUnidadeMedida();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return retorno;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			return _da.ObterDadosPdfTitulo(empreendimento, atividade, banco);
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Para o caso da coluna da atividade estar na tabela principal
			return _busCaracterizacao.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}

		#endregion

		public bool CopiarDadosCredenciado(Dependencia dependencia, int empreendimentoInternoId, BancoDeDados banco, BancoDeDados bancoCredenciado)
		{
			if (banco == null)
			{
				return false;
			}

			if (_validar == null)
			{
				_validar = new UnidadeConsolidacaoValidar();
			}

			#region Configurar Caracterização

			UnidadeConsolidacaoCredBus.UnidadeConsolidacaoBus credenciadoBus = new UnidadeConsolidacaoCredBus.UnidadeConsolidacaoBus();
			UnidadeConsolidacao caracterizacao = credenciadoBus.ObterHistorico(dependencia.DependenciaId, dependencia.DependenciaTid);

			caracterizacao.Empreendimento.Id = empreendimentoInternoId;
			caracterizacao.CredenciadoID = caracterizacao.Id;
			caracterizacao.Id = 0;
			caracterizacao.Tid = string.Empty;
			caracterizacao.Cultivares.ForEach(r => { r.IdRelacionamento = 0; });
			caracterizacao.ResponsaveisTecnicos.ForEach(r => { r.IdRelacionamento = 0; });

			#endregion

			if (_validar.CopiarDadosCredenciado(caracterizacao))
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					//Setar ID 
					caracterizacao.Id = ObterPorEmpreendimento(empreendimentoInternoId, simplificado: true, banco: bancoDeDados).Id;

					_da.CopiarDadosCredenciado(caracterizacao, bancoDeDados);

					credenciadoBus.AtualizarInternoIdTid(caracterizacao.CredenciadoID, caracterizacao.Id, GerenciadorTransacao.ObterIDAtual(), bancoCredenciado);

					bancoDeDados.Commit();
				}
			}

			return Validacao.EhValido;
		}
	}
}