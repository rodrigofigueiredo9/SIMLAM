using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Business
{
	public class SilviculturaATVBus : ICaracterizacaoBus
	{
		#region Propriedades

		SilviculturaATVValidar _validar = new SilviculturaATVValidar();
		SilviculturaATVDa _da = new SilviculturaATVDa();
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
					Tipo = eCaracterizacao.SilviculturaATV
				};
			}
		}

		public SilviculturaATVBus()
		{
			_validar = new SilviculturaATVValidar();
		}

		public SilviculturaATVBus(SilviculturaATVValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(SilviculturaATV caracterizacao)
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
							Tipo = eCaracterizacao.SilviculturaATV,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						Validacao.Add(Mensagem.Silvicultura.Salvar);

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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.SilviculturaATV))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.SilviculturaATV);

					Validacao.Add(Mensagem.Silvicultura.Excluir);

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

		public SilviculturaATV ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			SilviculturaATV caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.SilviculturaATV, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public SilviculturaATV ObterDadosGeo(int EmpreendimentoId)
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

		public SilviculturaATV MergiarGeo(SilviculturaATV caracterizacaoAtual)
		{
			SilviculturaATV dadosGeo = ObterDadosGeo(caracterizacaoAtual.EmpreendimentoId);
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.SilviculturaATV, eCaracterizacaoDependenciaTipo.Caracterizacao);

			decimal totalFloresta = 0;

			caracterizacaoAtual.Areas.ForEach(x =>
			{
				if (x.Tipo != (int)eSilviculturaAreaATV.DECLIVIDADE &&
					x.Tipo != (int)eSilviculturaAreaATV.AA_FOMENTO &&
					x.Tipo != (int)eSilviculturaAreaATV.AA_PLANTIO)
				{
					x.Valor = dadosGeo.Areas.Find(z => z.Tipo == x.Tipo).Valor;
					x.ValorTexto = x.Valor.ToStringTrunc();

				}

				if (x.Tipo == (int)eSilviculturaAreaATV.AVN ||
					x.Tipo == (int)eSilviculturaAreaATV.AA_PLANTIO ||
					x.Tipo == (int)eSilviculturaAreaATV.AA_FLORESTA_PLANTADA)
				{
					totalFloresta += x.Valor;
				}
			});

			caracterizacaoAtual.Areas.Find(x => x.Tipo == (int)eSilviculturaAreaATV.AA_TOTAL_FLORESTA).Valor = totalFloresta;
			caracterizacaoAtual.Areas.Find(x => x.Tipo == (int)eSilviculturaAreaATV.AA_TOTAL_FLORESTA).ValorTexto = totalFloresta.ToStringTrunc();

			foreach (SilviculturaCaracteristicaATV silvicultura in dadosGeo.Caracteristicas)
			{
				if (!caracterizacaoAtual.Caracteristicas.Exists(x => x.Identificacao == silvicultura.Identificacao))
				{
					caracterizacaoAtual.Caracteristicas.Add(silvicultura);
				}
			}

			List<SilviculturaCaracteristicaATV> silviculturasRemover = new List<SilviculturaCaracteristicaATV>();
			foreach (SilviculturaCaracteristicaATV silvicultura in caracterizacaoAtual.Caracteristicas)
			{
				if (!dadosGeo.Caracteristicas.Exists(x => x.Identificacao == silvicultura.Identificacao))
				{
					silviculturasRemover.Add(silvicultura);
					continue;
				}
				else
				{
					SilviculturaCaracteristicaATV silviculturaAux = dadosGeo.Caracteristicas.SingleOrDefault(x => x.Identificacao == silvicultura.Identificacao) ?? new SilviculturaCaracteristicaATV();
					silvicultura.Identificacao = silviculturaAux.Identificacao;
					silvicultura.TotalCroqui = silviculturaAux.TotalCroqui;
				}
			}

			foreach (SilviculturaCaracteristicaATV silvicultura in silviculturasRemover)
			{
				caracterizacaoAtual.Caracteristicas.Remove(silvicultura);
			}

			return caracterizacaoAtual;

		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			/* Caso em que a caracterizacao só pode ser emitir titulos de uma atividade */
			if (_da.ExisteCaracterizacao(empreendimento))
			{
				return new List<int>() { ConfiguracaoAtividade.ObterId((int)eAtividadeCodigo.Silvicultura) };
			}

			return new List<int>();
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