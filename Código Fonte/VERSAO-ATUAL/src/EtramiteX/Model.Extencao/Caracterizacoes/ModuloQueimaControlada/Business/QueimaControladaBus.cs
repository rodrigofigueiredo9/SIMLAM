using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloQueimaControlada.Business
{
	public class QueimaControladaBus : ICaracterizacaoBus
	{
		#region Propriedades

		QueimaControladaValidar _validar = null;
		QueimaControladaDa _da = new QueimaControladaDa();
		ProjetoGeograficoBus _projetoGeoBus = new ProjetoGeograficoBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.QueimaControlada
				};
			}
		}

		public QueimaControladaBus()
		{
			_validar = new QueimaControladaValidar();
		}

		public QueimaControladaBus(QueimaControladaValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(QueimaControlada caracterizacao)
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
							Tipo = eCaracterizacao.QueimaControlada,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.QueimaControlada.Salvar);

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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.QueimaControlada))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.QueimaControlada);

					Validacao.Add(Mensagem.QueimaControlada.Excluir);

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

		public QueimaControlada ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			QueimaControlada caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.QueimaControlada, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public QueimaControlada ObterDadosGeo(int EmpreendimentoId)
		{

			try
			{
				return _da.ObterDadosGeo(EmpreendimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;

		}

		public QueimaControlada MergiarGeo(QueimaControlada caracterizacaoAtual)
		{
			QueimaControlada dadosGeo = ObterDadosGeo(caracterizacaoAtual.EmpreendimentoId);
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.QueimaControlada, eCaracterizacaoDependenciaTipo.Caracterizacao);

			foreach (QueimaControladaQueima queima in dadosGeo.QueimasControladas)
			{
				if (!caracterizacaoAtual.QueimasControladas.Exists(x => x.Identificacao == queima.Identificacao))
				{
					caracterizacaoAtual.QueimasControladas.Add(queima);
				}
			}

			List<QueimaControladaQueima> queimasRemover = new List<QueimaControladaQueima>();
			foreach (QueimaControladaQueima queima in caracterizacaoAtual.QueimasControladas)
			{
				if (!dadosGeo.QueimasControladas.Exists(x => x.Identificacao == queima.Identificacao))
				{
					queimasRemover.Add(queima);
					continue;
				}
				else
				{
					QueimaControladaQueima queimaAux = dadosGeo.QueimasControladas.SingleOrDefault(x => x.Identificacao == queima.Identificacao) ?? new QueimaControladaQueima();
					queima.Identificacao = queimaAux.Identificacao;
					queima.AreaCroqui = queimaAux.AreaCroqui;
				}
			}

			foreach (QueimaControladaQueima queimaControlada in queimasRemover)
			{
				caracterizacaoAtual.QueimasControladas.Remove(queimaControlada);
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

		#endregion

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}
	}
}