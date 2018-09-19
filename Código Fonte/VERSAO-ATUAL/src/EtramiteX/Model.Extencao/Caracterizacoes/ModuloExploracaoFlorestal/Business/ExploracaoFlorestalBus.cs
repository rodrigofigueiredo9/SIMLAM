using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal.Business
{
	public class ExploracaoFlorestalBus : ICaracterizacaoBus
	{
		#region Propriedades

		ExploracaoFlorestalValidar _validar = null;
		ExploracaoFlorestalDa _da = new ExploracaoFlorestalDa();
		ProjetoGeograficoBus _projetoGeoBus = new ProjetoGeograficoBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.ExploracaoFlorestal
				};
			}
		}

		#endregion

		public ExploracaoFlorestalBus()
		{
			_validar = new ExploracaoFlorestalValidar();
		}

		public ExploracaoFlorestalBus(ExploracaoFlorestalValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(ExploracaoFlorestal caracterizacao)
		{
			try
			{
				if (_validar.Salvar(caracterizacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(caracterizacao, bancoDeDados);

						//Gerencia as dependências da caracterização
						_busCaracterizacao.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.ExploracaoFlorestal,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.ExploracaoFlorestal.Salvar);

						bancoDeDados.Commit();
					}
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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.ExploracaoFlorestal))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.ExploracaoFlorestal);

					Validacao.Add(Mensagem.ExploracaoFlorestal.Excluir);

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

		public ExploracaoFlorestal ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			ExploracaoFlorestal caracterizacao = null;

			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimento, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public IEnumerable<ExploracaoFlorestal> ObterPorEmpreendimentoList(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			IEnumerable<ExploracaoFlorestal> exploracaoFlorestalList = null;

			try
			{
				exploracaoFlorestalList = _da.ObterPorEmpreendimentoList(empreendimento, simplificado: simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return exploracaoFlorestalList;
		}

		public IEnumerable<ExploracaoFlorestal> ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterDadosGeo(empreendimento);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public ExploracaoFlorestal MergiarGeo(ExploracaoFlorestal caracterizacaoAtual)
		{
			var dadosGeo = ObterDadosGeo(caracterizacaoAtual.EmpreendimentoId);

			foreach (var dado in dadosGeo)
			{
				foreach (ExploracaoFlorestalExploracao exploracao in dado.Exploracoes)
				{
					if (!caracterizacaoAtual.Exploracoes.Exists(x => x.Identificacao == exploracao.Identificacao))
						caracterizacaoAtual.Exploracoes.Add(exploracao);
				}
			}

			List<ExploracaoFlorestalExploracao> exploracoesRemover = new List<ExploracaoFlorestalExploracao>();
			foreach (ExploracaoFlorestalExploracao exploracao in caracterizacaoAtual.Exploracoes)
			{
				foreach (var dado in dadosGeo)
				{
					if (!dado.Exploracoes.Exists(x => x.Identificacao == exploracao.Identificacao))
					{
						exploracoesRemover.Add(exploracao);
						continue;
					}
					else
					{
						ExploracaoFlorestalExploracao exploracaoAux = dado.Exploracoes.SingleOrDefault(x => x.Identificacao == exploracao.Identificacao) ?? new ExploracaoFlorestalExploracao();
						exploracao.Identificacao = exploracaoAux.Identificacao;
						exploracao.GeometriaTipoId = exploracaoAux.GeometriaTipoId;
						exploracao.GeometriaTipoTexto = exploracaoAux.GeometriaTipoTexto;
						exploracao.AreaCroqui = exploracaoAux.AreaCroqui;
					}
				}
			}

			foreach (ExploracaoFlorestalExploracao exploracaoFlorestal in exploracoesRemover)
			{
				caracterizacaoAtual.Exploracoes.Remove(exploracaoFlorestal);
			}

			return caracterizacaoAtual;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Caso a coluna das atividades NÃO esteja na tabela Principal
			throw new NotImplementedException();
		}

		public Resultados<ExploracaoFlorestal> Filtrar(ListarExploracaoFlorestalFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarExploracaoFlorestalFiltro> filtro = new Filtro<ListarExploracaoFlorestalFiltro>(filtrosListar, paginacao);
				Resultados<ExploracaoFlorestal> resultados = _da.Filtrar(filtro);

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

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}

		public int ObterCodigoExploracao(int tipoExploracao, BancoDeDados bancoDeDados = null)
		{
			var codigoExploracao = 0;

			try
			{
				codigoExploracao = _da.ObterCodigoExploracao(tipoExploracao, bancoDeDados);
				codigoExploracao++;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return codigoExploracao;
		}
	}
}