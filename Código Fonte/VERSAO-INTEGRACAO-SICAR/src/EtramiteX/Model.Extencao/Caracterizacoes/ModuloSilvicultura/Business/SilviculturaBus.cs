using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilvicultura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilvicultura.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilvicultura.Business
{
	public class SilviculturaBus : ICaracterizacaoBus
	{
		#region Propriedades

		SilviculturaValidar _validar = null;
		SilviculturaDa _da = new SilviculturaDa();
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
					Tipo = eCaracterizacao.Silvicultura
				};
			}
		}

		public SilviculturaBus()
		{
			_validar = new SilviculturaValidar();
		}

		public SilviculturaBus(SilviculturaValidar validar)
		{
			_validar = validar;
		}

		#region Comandos DML

		public bool Salvar(Silvicultura caracterizacao)
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
							Tipo = eCaracterizacao.Silvicultura,
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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.Silvicultura))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					_projetoGeoBus.Excluir(empreendimento, eCaracterizacao.Silvicultura);

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

		public Silvicultura ObterPorEmpreendimento(int EmpreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{

			Silvicultura caracterizacao = null;
			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(EmpreendimentoId, simplificado: simplificado);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.Silvicultura, eCaracterizacaoDependenciaTipo.Caracterizacao);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public Silvicultura ObterDadosGeo(int EmpreendimentoId)
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

		public Silvicultura MergiarGeo(Silvicultura caracterizacaoAtual)
		{
			Silvicultura dadosGeo = ObterDadosGeo(caracterizacaoAtual.EmpreendimentoId);
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.Silvicultura, eCaracterizacaoDependenciaTipo.Caracterizacao);
			caracterizacaoAtual.Areas = dadosGeo.Areas;

			foreach (SilviculturaSilvicult silvicultura in dadosGeo.Silviculturas)
			{
				if (!caracterizacaoAtual.Silviculturas.Exists(x => x.Identificacao == silvicultura.Identificacao))
				{
					caracterizacaoAtual.Silviculturas.Add(silvicultura);
				}
			}

			List<SilviculturaSilvicult> silviculturasRemover = new List<SilviculturaSilvicult>();
			foreach (SilviculturaSilvicult silvicultura in caracterizacaoAtual.Silviculturas)
			{
				if (!dadosGeo.Silviculturas.Exists(x => x.Identificacao == silvicultura.Identificacao))
				{
					silviculturasRemover.Add(silvicultura);
					continue;
				}
				else
				{
					SilviculturaSilvicult silviculturaAux = dadosGeo.Silviculturas.SingleOrDefault(x => x.Identificacao == silvicultura.Identificacao) ?? new SilviculturaSilvicult();
					silvicultura.Identificacao = silviculturaAux.Identificacao;
					silvicultura.AreaCroqui = silviculturaAux.AreaCroqui;
					silvicultura.AreaCroquiHa = silviculturaAux.AreaCroquiHa;
					silvicultura.GeometriaTipo = silviculturaAux.GeometriaTipo;
					silvicultura.GeometriaTipoTexto = silviculturaAux.GeometriaTipoTexto;
				}
			}

			foreach (SilviculturaSilvicult silvicultura in silviculturasRemover)
			{
				caracterizacaoAtual.Silviculturas.Remove(silvicultura);
			}

			return caracterizacaoAtual;
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			Silvicultura silvicultura = ObterPorEmpreendimento(empreendimento);
			CaracterizacaoPDF caractPdf = new CaracterizacaoPDF();

			#region Culturas

			caractPdf.Cultura.AreaTotalHa = silvicultura.Silviculturas.Sum(x => x.AreaCroquiHa).ToStringTrunc(4);

			//Agrupando Culturas Florestais
			List<CulturaFlorestalTipoPDF> culturas = new List<CulturaFlorestalTipoPDF>();

			silvicultura.Silviculturas.ForEach(x =>
			{
				x.Culturas.ForEach(y =>
				{
					culturas.Add(new CulturaFlorestalTipoPDF()
					{
						AreaCultura = y.AreaCultura.ToStringTrunc(4),
						AreaCulturaHa = y.AreaCulturaHa.ToStringTrunc(),
						AreaCulturaTexto = y.AreaCulturaTexto,
						CulturaTipo = y.CulturaTipo,
						CulturaTipoTexto = y.CulturaTipoTexto,
						EspecificarTexto = y.EspecificarTexto,
						Id = y.Id,
						Tid = y.Tid
					});
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