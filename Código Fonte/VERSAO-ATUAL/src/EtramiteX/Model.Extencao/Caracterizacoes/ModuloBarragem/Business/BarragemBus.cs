using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Business
{
	public class BarragemBus : ICaracterizacaoBus
	{
		#region Propriedade

		BarragemValidar _validar = new BarragemValidar();
		BarragemDa _da = new BarragemDa();
		ProjetoGeograficoBus _projetoGeoBus = new ProjetoGeograficoBus();
		CaracterizacaoBus _busCaracterizacao = new CaracterizacaoBus();
		DominialidadeBus _busDominialidade = new DominialidadeBus();
		CaracterizacaoValidar _caracterizacaoValidar = new CaracterizacaoValidar();

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.Barragem
				};
			}
		}

		#endregion

		public BarragemBus() { }

		public BarragemBus(BarragemValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public int Salvar(Barragem caracterizacao)
		{
			int id = 0;
			try
			{
				if (_validar.Salvar(caracterizacao))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						id = _da.Salvar(caracterizacao, bancoDeDados);

						_busCaracterizacao.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.Barragem,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.BarragemMsg.Salvar);

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return id;
		}

		public bool Excluir(int empreendimento, BancoDeDados banco = null, bool validarDependencias = true)
		{
			try
			{
				if (!_caracterizacaoValidar.Basicas(empreendimento))
				{
					return Validacao.EhValido;
				}

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.Barragem, eCaracterizacaoDependenciaTipo.Caracterizacao))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.Barragem);

					Validacao.Add(Mensagem.BarragemMsg.Excluir);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ExcluirBarragemItem(int barragemItemId, BancoDeDados banco = null)
		{
			try
			{

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.ExcluirBarragemItem(barragemItemId, bancoDeDados);

					Validacao.Add(Mensagem.BarragemMsg.ExcluirBarragemItemSucesso);

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

		public Barragem ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Barragem caracterizacao = null;

			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimento, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.Barragem, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public Barragem Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			Barragem barragem = new Barragem();
			try
			{
				barragem = _da.Obter(id, banco, simplificado);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return barragem;
		}

        public List<BarragemItem> ObterListaFinalidade(int id, bool simplificado = false, BancoDeDados banco = null)
        {
            List<BarragemItem> barragem = new List<BarragemItem>();
            try
            {
                barragem = _da.ObterListaFinalidade(id, banco, simplificado);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return barragem;
        }

		public Barragem ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			return _da.ObterDadosPdfTitulo(Convert.ToInt32(especificidade.DadosEspAtivCaractObj), banco);
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Para o caso da coluna da atividade estar na tabela principal			
			return _busCaracterizacao.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}

		public List<Lista> ObterBarragens(int empreendimentoId, string tid = "", int barragemId = 0, BancoDeDados banco = null)
		{
			return _da.ObterBarragens(empreendimentoId, tid, barragemId, banco);
		}

		#endregion

		#region Validações

		public bool TemARL(int empreendimentoId, BancoDeDados banco = null)
		{
			return _da.TemARL(empreendimentoId, banco);
		}

		public bool TemARLDesconhecida(int empreendimentoId, BancoDeDados banco = null)
		{
			return _da.TemARLDesconhecida(empreendimentoId, banco);
		}

		#endregion

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}
	}
}