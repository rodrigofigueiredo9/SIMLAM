using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPatioLavagem;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDescricaoLicenciamentoAtividade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPatioLavagem.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPatioLavagem.Business
{
	public class PatioLavagemBus : ICaracterizacaoBus
	{
		#region Propriedades

		PatioLavagemValidar _validar = null;
		PatioLavagemDa _da = new PatioLavagemDa();
		ProjetoGeograficoBus _projetoGeoBus = new ProjetoGeograficoBus();
		DescricaoLicenciamentoAtividadeBus _descricaoAtividade = new DescricaoLicenciamentoAtividadeBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		#endregion

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.PatioLavagem
				};
			}
		}

		public PatioLavagemBus()
		{
			_validar = new PatioLavagemValidar();
		}

		public PatioLavagemBus(PatioLavagemValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(PatioLavagem caracterizacao)
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
							Tipo = eCaracterizacao.PatioLavagem,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.PatioLavagem.Salvar);

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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.PatioLavagem))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.PatioLavagem);

					_descricaoAtividade.Excluir(empreendimento, eCaracterizacao.PatioLavagem);

					Validacao.Add(Mensagem.PatioLavagem.Excluir);

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

		public PatioLavagem ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{

			PatioLavagem caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.PatioLavagem, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public PatioLavagem ObterDadosGeo(int EmpreendimentoId)
		{
			throw new NotImplementedException();
		}

		public PatioLavagem MergiarGeo(PatioLavagem caracterizacaoAtual)
		{
			caracterizacaoAtual.CoordenadaAtividade.Tipo = 0; //limpando dados selecionados
			caracterizacaoAtual.CoordenadaAtividade.Id = 0; //limpando dados selecionados
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.PatioLavagem, eCaracterizacaoDependenciaTipo.Caracterizacao);
			return caracterizacaoAtual;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			CaracterizacaoPDF caractPdf = _da.ObterDadosPdfTitulo(empreendimento, atividade, banco);

			return caractPdf;
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Para o caso da coluna da atividade estar na tabela principal			
			return _busCaracterizacao.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}

		#endregion

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}
	}
}