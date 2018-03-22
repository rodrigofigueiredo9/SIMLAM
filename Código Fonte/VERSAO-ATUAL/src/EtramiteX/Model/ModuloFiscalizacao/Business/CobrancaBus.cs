using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloPessoa.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Business
{
	public class CobrancaBus
	{
		#region Propriedades

		CobrancaValidar _validar = null;
		CobrancaParcelamentoValidar _validarParcelamento = null;
		CobrancaDUAValidar _validarDUA = null;
		CobrancaDa _da = new CobrancaDa();
		CobrancaParcelamentoDa _daParcelamento = new CobrancaParcelamentoDa();
		CobrancaDUADa _daDUA = new CobrancaDUADa();
		ConfigFiscalizacaoBus _busConfiguracao = new ConfigFiscalizacaoBus();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Construtores
		public CobrancaBus()
		{
			_validar = new CobrancaValidar();
			_validarParcelamento = new CobrancaParcelamentoValidar();
			_validarDUA = new CobrancaDUAValidar();
		}

		public CobrancaBus(CobrancaValidar validar, CobrancaParcelamentoValidar cobrancaParcelamentoValidar, CobrancaDUAValidar cobrancaDUAValidar)
		{
			_validar = validar;
			_validarParcelamento = cobrancaParcelamentoValidar;
			_validarDUA = cobrancaDUAValidar;
		}
		#endregion

		#region Comandos DML

		public bool Salvar(Cobranca entidade)
		{
			try
			{
				if (_validar.Salvar(entidade))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();
						_da.Salvar(entidade, bancoDeDados);
						entidade.UltimoParcelamento.CobrancaId = entidade.Id;
						this.Salvar(entidade.UltimoParcelamento, bancoDeDados);
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

		public bool Salvar(CobrancaParcelamento parcelamento, BancoDeDados banco = null)
		{
			try
			{
				if (_validarParcelamento.Salvar(parcelamento))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();
						_daParcelamento.Salvar(parcelamento, bancoDeDados);
						foreach (var dua in parcelamento.DUAS)
						{
							dua.ParcelamentoId = parcelamento.Id;
							this.Salvar(dua, bancoDeDados);
						}
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

		public bool Salvar(CobrancaDUA cobrancaDUA, BancoDeDados banco = null)
		{
			try
			{
				if (_validarDUA.Salvar(cobrancaDUA))
				{
					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
					{
						bancoDeDados.IniciarTransacao();
						_daDUA.Salvar(cobrancaDUA, bancoDeDados);
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

		public bool Excluir(int id)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(id, bancoDeDados);

					Validacao.Add(Mensagem.CobrancaMsg.Excluir);

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool NovoParcelamento(Cobranca cobranca)
		{
			try
			{
				if (_validar.Salvar(cobranca))
				{
					var parcelas = cobranca.UltimoParcelamento.DUAS;
					var valorRestante = 0M;
					foreach (var parcela in parcelas)
					{
						if (!parcela.DataPagamento.IsValido && parcela.ValorPago == 0)
							parcela.DataCancelamento = new DateTecno() { Data = DateTime.Now };
						valorRestante += parcela.ValorDUA - parcela.ValorPago;
					}

					cobranca.UltimoParcelamento.DUAS = parcelas;
					this.Salvar(cobranca.UltimoParcelamento);

					var parcelamento = new CobrancaParcelamento()
					{
						CobrancaId = cobranca.Id,
						Data1Vencimento = new DateTecno() { Data = DateTime.Now.AddDays(15) },
						DataEmissao = new DateTecno() { Data = DateTime.Now },
						ValorMulta = valorRestante,
						DUAS = new List<CobrancaDUA>()
					};

					if(cobranca.AutuadoPessoa == null)
						cobranca.AutuadoPessoa = cobranca.AutuadoPessoaId > 0 ? new PessoaBus().Obter(cobranca.AutuadoPessoaId) : new Pessoa();
					parcelamento.QuantidadeParcelas = this.GetMaximoParcelas(cobranca, parcelamento);
					
					this.Salvar(parcelamento);
				}
			}
			catch (Exception e)
			{
				Validacao.AddErro(e);
			}

			return Validacao.EhValido;
		}

		#endregion

		#region Obter

		public Cobranca Obter(int fiscalizacao, BancoDeDados banco = null)
		{
			Cobranca entidade = new Cobranca();

			try
			{
				entidade = _da.Obter(fiscalizacao, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return entidade;
		}

		public List<CobrancaDUA> ObterCobrancaDUA(int parcelamentoId, BancoDeDados banco = null)
		{
			List<CobrancaDUA> list = new List<CobrancaDUA>();

			try
			{
				list = _daDUA.Obter(parcelamentoId, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return list;
		}

		#endregion

		#region Cálculo
		private int GetPrazoDesconto(int prazoDescontoDecorrencia)
		{
			var prazoDesconto = 0;
			if (prazoDescontoDecorrencia == (int)eDecorrencia.Dia)
				prazoDesconto = prazoDescontoDecorrencia * 1;
			else if (prazoDescontoDecorrencia == (int)eDecorrencia.Mes)
				prazoDesconto = prazoDescontoDecorrencia * 30;
			else if (prazoDescontoDecorrencia == (int)eDecorrencia.Ano)
				prazoDesconto = prazoDescontoDecorrencia * 365;

			return prazoDesconto;
		}

		private decimal GetValorTotalAtualizadoEmReais(Cobranca cobranca, CobrancaParcelamento parcelamento, Parametrizacao parametrizacao, decimal valorMulta)
		{
			var multaVRTE = valorMulta * (Convert.ToDecimal(parametrizacao.MultaPercentual) / 100);
			var jurosVRTE = new Decimal();
			var diasJuros = Convert.ToDecimal(parcelamento.Data1Vencimento.Data.Value.Date.Subtract(cobranca.DataIUF.Data.Value.Date).TotalDays);
			if (diasJuros > 0)
				jurosVRTE = (diasJuros / 30) * (Convert.ToDecimal(parametrizacao.JurosPercentual) / 100);

			return valorMulta + multaVRTE + (valorMulta * jurosVRTE);
		}

		public bool CalcularParcelas(Cobranca cobranca, CobrancaParcelamento parcelamento)
		{
			var retorno = false;

			var vrte = _busConfiguracao.ObterVrte(cobranca.DataIUF.Data.Value.Year) ?? new Vrte();
			var parametrizacao = _busConfiguracao.ObterParametrizacao(cobranca.CodigoReceitaId, cobranca.DataIUF.Data.Value);
			if (parametrizacao == null || (vrte?.Id ?? 0) == 0) return retorno;

			parcelamento.ValorMultaAtualizado = GetValorTotalAtualizadoEmReais(cobranca, parcelamento, parametrizacao, parcelamento.ValorMulta);
			var valorAtualizadoVRTE = parcelamento.ValorMultaAtualizado / vrte.VrteEmReais;
			var parcelas = parcelamento.DUAS;

			if (parcelas.Count == 1 && cobranca.Parcelamentos?.Count <= 1)
			{
				var parcela = parcelas[0];
				if (parcela.ValorDUA == 0)
				{
					var prazoDesconto = GetPrazoDesconto(parametrizacao.PrazoDescontoDecorrencia);

					if (parcela.DataVencimento.Data <= cobranca.DataIUF.Data.Value.AddDays(prazoDesconto))
					{
						parcela.VRTE = valorAtualizadoVRTE * (1 - (Convert.ToDecimal(parametrizacao.DescontoPercentual) / 100));
						var vrteParcela = _busConfiguracao.ObterVrte(parcela.DataVencimento.Data.Value.Year);
						if (vrteParcela.Id > 0)
							parcela.ValorDUA = Math.Round(parcela.VRTE * vrteParcela.VrteEmReais, 2);
					}
				}

				retorno = true;
			}
			else
			{
				var parcelaAnterior = new CobrancaDUA();
				var valorJuros = 1 + (Convert.ToDecimal(parametrizacao.JurosPercentual) / 100);

				foreach (var parcela in parcelas)
				{
					if (parcela.ValorDUA == 0)
					{
						if (parcela.VRTE == 0)
						{
							if (parcela.Parcela[0] == '1')
								parcela.VRTE = Math.Round(valorAtualizadoVRTE / parcelamento.QuantidadeParcelas, 4);
							else
								parcela.VRTE = Math.Round(parcelaAnterior.VRTE * valorJuros, 4);
						}

						var vrteParcela = _busConfiguracao.ObterVrte(parcela.DataVencimento.Data.Value.Year);
						if (vrteParcela.Id > 0)
							parcela.ValorDUA = Math.Round(parcela.VRTE * vrteParcela.VrteEmReais, 2);
					}
					parcelaAnterior = parcela;
					retorno = true;
				}
			}

			return retorno;
		}

		public List<CobrancaDUA> GerarParcelas(Cobranca cobranca, CobrancaParcelamento parcelamento)
		{
			var list = new List<CobrancaDUA>();
			var parametrizacao = _busConfiguracao.ObterParametrizacao(cobranca.CodigoReceitaId, cobranca.DataIUF.Data.Value);
			var parcelaAnterior = new CobrancaDUA();
			if (parametrizacao != null)
			{
				for (int i = 1; i <= parcelamento.QuantidadeParcelas; i++)
				{
					var parcela = new CobrancaDUA()
					{
						DataEmissao = parcelamento.DataEmissao,
						Parcela = string.Concat(i, '/', parcelamento.QuantidadeParcelas),
						ParcelamentoId = parcelamento.Id
					};

					if (i == 1)
						parcela.DataVencimento = new DateTecno() { Data = parcelamento.Data1Vencimento.Data };
					else
					{
						var dataVencimento = parcelaAnterior.DataVencimento.Data.Value.AddMonths(1);
						if (dataVencimento.DayOfWeek == DayOfWeek.Saturday)
							dataVencimento.AddDays(2);
						else if (dataVencimento.DayOfWeek == DayOfWeek.Monday)
							dataVencimento.AddDays(1);

						parcela.DataVencimento = new DateTecno() { Data = dataVencimento };
					}
					parcela.Situacao = "Em Aberto";

					parcelaAnterior = parcela;
					list.Add(parcela);
				}
			}

			return list;
		}

		public int GetMaximoParcelas(Cobranca cobranca, CobrancaParcelamento parcelamento)
		{
			if (cobranca == null || !Convert.ToBoolean(cobranca.DataIUF?.IsValido)) return 0;

			var parametrizacao = _busConfiguracao.ObterParametrizacao(cobranca.CodigoReceitaId, cobranca.DataIUF.Data.Value);
			if (parametrizacao == null) return 0;

			var vrte = _busConfiguracao.ObterVrte(cobranca.DataIUF.Data.Value.Year);
			if ((vrte?.Id ?? 0) == 0) return 0;

			var valorAtualizadoVRTE = parcelamento.ValorMultaAtualizado / vrte.VrteEmReais;
			var parcela = 0;

			if (cobranca.AutuadoPessoa.IsFisica)
				parcela = Decimal.ToInt32(valorAtualizadoVRTE / parametrizacao.ValorMinimoPF);
			else
				parcela = Decimal.ToInt32(valorAtualizadoVRTE / parametrizacao.ValorMinimoPJ);

			var detalhe = parametrizacao.ParametrizacaoDetalhes.FindLast(x => x.ValorInicial < parcelamento.ValorMultaAtualizado && (x.ValorFinal == 0 || x.ValorFinal >= parcelamento.ValorMultaAtualizado));
			if ((detalhe?.Id ?? 0) == 0) return 0;

			if (parcela > detalhe.MaximoParcelas)
				return detalhe.MaximoParcelas;

			return parcela;
		}

		#endregion Cálculo

		public bool ValidarAssociar(int fiscalizacaoId)
		{
			var cobranca = _da.Obter(fiscalizacaoId);
			if (cobranca?.Id > 0)
				Validacao.AddErro(new Exception("Já existe uma cobrança cadastrada para esta fiscalização."));

			return Validacao.EhValido;
		}
	}
}
