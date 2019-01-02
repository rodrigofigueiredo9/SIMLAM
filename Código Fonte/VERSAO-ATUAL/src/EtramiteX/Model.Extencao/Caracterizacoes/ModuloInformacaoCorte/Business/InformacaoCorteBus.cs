using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Business
{
	public class InformacaoCorteBus : ICaracterizacaoBus
	{
		#region Propriedades

		InformacaoCorteValidar _validar = null;
		InformacaoCorteDa _da = new InformacaoCorteDa();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.InformacaoCorte
				};
			}
		}

		public InformacaoCorteBus()
		{
			_validar = new InformacaoCorteValidar();
		}

		public InformacaoCorteBus(InformacaoCorteValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(InformacaoCorte caracterizacao)
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
						if (caracterizacao.Dependencias != null && caracterizacao.Dependencias.Count > 0)
						{
							_busCaracterizacao.Dependencias(new Caracterizacao()
							{
								Id = caracterizacao.Id,
								Tipo = eCaracterizacao.InformacaoCorte,
								DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
								Dependencias = caracterizacao.Dependencias
							}, bancoDeDados);
						}

						Validacao.Add(Mensagem.InformacaoCorte.Salvar);

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

		public bool Excluir(int id, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				var caracterizacao = this.Obter(id, simplificado: true);

				if (!_caracterizacaoValidar.Basicas(caracterizacao.EmpreendimentoId))
					return Validacao.EhValido;

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(caracterizacao.EmpreendimentoId, eCaracterizacao.InformacaoCorte))
					return Validacao.EhValido;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.InformacaoCorte.Excluir);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirPorEmpreendimento(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				if (!_caracterizacaoValidar.Basicas(empreendimento))
					return Validacao.EhValido;

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.InformacaoCorte))
					return Validacao.EhValido;

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.InformacaoCorte.Excluir);

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

		#region Obter/Filtrar

		public InformacaoCorte Obter(int id, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = null;
			try
			{
				caracterizacao = _da.Obter(id, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}


		public List<InformacaoCorte> ObterPorEmpreendimento(int empreendimentoInternoId, bool simplificado = false, BancoDeDados banco = null)
		{
			List<InformacaoCorte> caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimentoInternoId, simplificado: simplificado, banco: banco);
				foreach (var informacao in caracterizacao)
					informacao.Dependencias = _busCaracterizacao.ObterDependencias(informacao.Id, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public List<InformacaoCorte> FiltrarPorEmpreendimento(int empreendimentoInternoId)
		{
			List<InformacaoCorte> caracterizacao = null;
			try
			{
				caracterizacao = _da.FiltrarPorEmpreendimento(empreendimentoInternoId);
				foreach(var informacao in caracterizacao)
					informacao.Dependencias = _busCaracterizacao.ObterDependencias(informacao.Id, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		public List<Lista> ObterListaInfCorteEmpreendimento(int empreendimento)
		{
			List<Lista> retorno = new List<Lista>();
			try
			{
				retorno = _da.ObterListaInfCorteEmpreendimento(empreendimento);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return retorno;
		}

		public List<Lista> ObterListaInfCorteTitulo(int titulo)
		{
			List<Lista> retorno = new List<Lista>();
			try
			{
				retorno = _da.ObterListaInfCorteTitulo(titulo);
			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
			return retorno;
		}

		#endregion

		public InformacaoCorte MergiarGeo(InformacaoCorte caracterizacaoAtual)
		{
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.InformacaoCorte, eCaracterizacaoDependenciaTipo.Caracterizacao);
			return caracterizacaoAtual;
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Caso a coluna das atividades NÃO esteja na tabela Principal
			throw new NotImplementedException();
		}

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}
	}
}