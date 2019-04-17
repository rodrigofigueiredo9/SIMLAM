using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
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

		public bool Salvar(IEnumerable<ExploracaoFlorestal> exploracoes)
		{
			try
			{
				if (_validar.Salvar(exploracoes))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						foreach (var caracterizacao in exploracoes)
						{
							_da.Salvar(caracterizacao, bancoDeDados);

							//Gerencia as dependências da caracterização
							_busCaracterizacao.Dependencias(new Caracterizacao()
							{
								Id = caracterizacao.Id,
								Tipo = eCaracterizacao.ExploracaoFlorestal,
								DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
								Dependencias = caracterizacao.Dependencias
							}, bancoDeDados);
						}

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
					return Validacao.EhValido;

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.ExploracaoFlorestal))
					return Validacao.EhValido;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					var exploracoes = _da.ObterPorEmpreendimentoList(empreendimento, true);
					var exploracoesEmAberto = exploracoes?.Where(x => x.DataConclusao.IsValido == false && x.PossuiVinculoComTitulo == false);
					foreach(var exploracao in exploracoesEmAberto)
						_da.Excluir(exploracao.Id, bancoDeDados);

					if (exploracoesEmAberto?.Count() == exploracoes?.Count())
					{
						var projetoId = _projetoGeoBus.ExisteProjetoGeografico(empreendimento, (int)eCaracterizacao.ExploracaoFlorestal, finalizado: false);
						var projeto = _projetoGeoBus.ObterProjeto(projetoId, simplificado: false);
						_projetoGeoBus.ExcluirRascunho(projeto, bancoDeDados);
					}

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

		public void FinalizarExploracao(int empreendimento, int titulo, BancoDeDados banco = null)
		{
			try
			{
				var idProjetoGeo = _projetoGeoBus.ExisteProjetoGeografico(empreendimento, (int)eCaracterizacao.ExploracaoFlorestal);
				if (idProjetoGeo == 0)
					throw new Exception("Projeto Geográfico não encontrado");

				var projeto = _projetoGeoBus.ObterProjeto(idProjetoGeo);
				_projetoGeoBus.FinalizarGeometrias(projeto, titulo, banco);

				if (!Validacao.EhValido) return;

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					var exploracoes = this.ObterPorEmpreendimentoList(empreendimento, simplificado: true, banco: bancoDeDados);
					foreach (var exploracao in exploracoes)
					{
						exploracao.Dependencias = _busCaracterizacao.ObterDependenciasAtual(exploracao.EmpreendimentoId, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao);
						_busCaracterizacao.Dependencias(new Caracterizacao()
						{
							Id = exploracao.Id,
							Tipo = eCaracterizacao.ExploracaoFlorestal,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = exploracao.Dependencias
						}, bancoDeDados);
						_busCaracterizacao.AtualizarDependentes(exploracao.EmpreendimentoId, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao, exploracao.Tid, bancoDeDados);
					}

					_da.FinalizarExploracao(empreendimento, bancoDeDados);

					if (!this.ExisteExploracaoEmAndamento(empreendimento, bancoDeDados))
					{
						if (projeto.SituacaoId != (int)eProjetoGeograficoSituacao.Finalizado)
						{
							projeto.Sobreposicoes = _projetoGeoBus.ObterGeoSobreposiacao(idProjetoGeo, eCaracterizacao.ExploracaoFlorestal);
							_projetoGeoBus.SalvarSobreposicoes(projeto);
							_projetoGeoBus.Finalizar(projeto, bancoDeDados);
							if (Validacao.EhValido)
								_projetoGeoBus.ExcluirRascunho(projeto, bancoDeDados);
						}
					}

					if (Validacao.EhValido)
						Validacao.Erros.Clear();
					else
					{
						bancoDeDados.Rollback();
						return;
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ReabrirExploracao(int empreendimento, int titulo, BancoDeDados banco = null)
		{
			try
			{
				var idProjetoGeo = _projetoGeoBus.ExisteProjetoGeografico(empreendimento, (int)eCaracterizacao.ExploracaoFlorestal);
				if (idProjetoGeo == 0)
					throw new Exception("Projeto Geográfico não encontrado");

				var projeto = _projetoGeoBus.ObterProjeto(idProjetoGeo);
				if (projeto.SituacaoId == (int)eProjetoGeograficoSituacao.Finalizado)
					_projetoGeoBus.Reabrir(projeto, titulo, banco);

				if (!Validacao.EhValido) return;

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.ReabrirExploracao(empreendimento, titulo, bancoDeDados);

					if (Validacao.EhValido)
						Validacao.Erros.Clear();
					else
					{
						bancoDeDados.Rollback();
						return;
					}

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		#endregion

		#region Obter

		public ExploracaoFlorestal ObterPorId(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			ExploracaoFlorestal caracterizacao = null;

			try
			{
				caracterizacao = _da.Obter(id, banco, simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.ExploracaoFlorestal, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

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

		public bool ExisteExploracaoEmAndamento(int empreendimento, BancoDeDados banco = null)
		{
			try
			{
				return _da.ExisteExploracaoEmAndamento(empreendimento, banco);
			}
			catch
			{
				return false;
			}
		}

		public bool ExisteExploracaoGeoNaoCadastrada(int projeto, BancoDeDados banco = null)
		{
			try
			{
				return _da.ExisteExploracaoGeoNaoCadastrada(projeto, banco);
			}
			catch
			{
				return false;
			}
		}

		public IEnumerable<ExploracaoFlorestal> ObterExploracoes(int tituloId, int modelo, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterExploracoes(tituloId, modelo, banco);
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
	}
}