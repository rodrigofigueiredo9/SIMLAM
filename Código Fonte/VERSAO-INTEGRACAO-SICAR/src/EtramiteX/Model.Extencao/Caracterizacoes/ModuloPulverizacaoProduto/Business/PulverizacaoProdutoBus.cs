using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto.Business
{
	public class PulverizacaoProdutoBus : ICaracterizacaoBus
	{
		#region Propriedades

		PulverizacaoProdutoValidar _validar = null;
		PulverizacaoProdutoDa _da = new PulverizacaoProdutoDa();
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
					Tipo = eCaracterizacao.PulverizacaoProduto
				};
			}
		}

		public PulverizacaoProdutoBus()
		{
			_validar = new PulverizacaoProdutoValidar();
		}

		public PulverizacaoProdutoBus(PulverizacaoProdutoValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(PulverizacaoProduto caracterizacao)
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
							Tipo = eCaracterizacao.PulverizacaoProduto,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.PulverizacaoProduto.Salvar);

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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.PulverizacaoProduto))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.PulverizacaoProduto);

					Validacao.Add(Mensagem.PulverizacaoProduto.Excluir);

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

		public PulverizacaoProduto ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{

			PulverizacaoProduto caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.PulverizacaoProduto, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public PulverizacaoProduto ObterDadosGeo(int EmpreendimentoId)
		{
			throw new NotImplementedException();
		}

		public PulverizacaoProduto MergiarGeo(PulverizacaoProduto caracterizacaoAtual)
		{
			caracterizacaoAtual.CoordenadaAtividade.Tipo = 0; //limpando dados selecionados
			caracterizacaoAtual.CoordenadaAtividade.Id = 0; //limpando dados selecionados
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.PulverizacaoProduto, eCaracterizacaoDependenciaTipo.Caracterizacao);
			return caracterizacaoAtual;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{

			PulverizacaoProduto pulverizacao = ObterPorEmpreendimento(empreendimento);
			CaracterizacaoPDF caractPdf = new CaracterizacaoPDF();

			#region Culturas

			Decimal totalHa = 0;
			foreach (Cultura cultura in pulverizacao.Culturas)
			{
				Decimal auxHa = 0;
				if (Decimal.TryParse(cultura.Area, out auxHa))
				{
					totalHa += auxHa;
				}
			}

			caractPdf.Cultura.AreaTotalHa = totalHa.ToStringTrunc(4);

			//Agrupando Culturas Florestais
			List<CulturaFlorestalTipoPDF> culturas = new List<CulturaFlorestalTipoPDF>();

			pulverizacao.Culturas.ForEach(y =>
				{
					culturas.Add(new CulturaFlorestalTipoPDF()
					{
						AreaCultura = y.Area,
						AreaCulturaHa = y.Area,
						AreaCulturaTexto = y.Area,
						CulturaTipo = y.TipoId,
						CulturaTipoTexto = y.TipoTexto,
						EspecificarTexto = y.EspecificarTexto,
						Id = y.Id,
						Tid = y.Tid
					});
				});

			culturas.ForEach(cultura =>
			{
				if (!caractPdf.Cultura.Tipos.Exists(y => y.CulturaTipoTexto.ToLower() == cultura.CulturaTipoTexto.ToLower()))
				{
					cultura.AreaCultura = culturas
						.Where(x => x.CulturaTipoTexto.ToLower() == cultura.CulturaTipoTexto.ToLower())
						.Sum(x => Convert.ToDecimal(x.AreaCulturaHa)).ToStringTrunc(4);
					caractPdf.Cultura.Tipos.Add(cultura);
				}
			});

			#endregion

			return caractPdf;
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Para o caso da coluna da atividade estar na tabela principal			
			return _busCaracterizacao.ObterAtividades(empreendimento, Caracterizacao.Tipo);
		}

		#endregion

		#region Validacoes

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