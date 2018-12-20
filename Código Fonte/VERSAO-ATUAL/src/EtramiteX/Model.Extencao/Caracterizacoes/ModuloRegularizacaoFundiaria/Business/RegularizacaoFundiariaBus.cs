using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Business
{
	public class RegularizacaoFundiariaBus : ICaracterizacaoBus
	{
		#region Propriedade

		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao;
		RegularizacaoFundiariaValidar _validar;
		RegularizacaoFundiariaDa _da;
		CaracterizacaoBus _busCaracterizacao;
		DominialidadeBus _busDominialidade;
		CaracterizacaoValidar _caracterizacaoValidar;

		public List<UsoAtualSoloLst> TiposUsos
		{
			get { return _configCaracterizacao.Obter<List<UsoAtualSoloLst>>(ConfiguracaoCaracterizacao.KeyRegularizacaoFundiariaTipoUso).MoverItemFinal(ConfiguracaoExtensoes.OUTRO); }
		}

		public Caracterizacao Caracterizacao
		{
			get
			{
				return new Caracterizacao()
				{
					Tipo = eCaracterizacao.RegularizacaoFundiaria
				};
			}
		}

		#endregion

		public RegularizacaoFundiariaBus() : this (new RegularizacaoFundiariaValidar()) { }

		public RegularizacaoFundiariaBus(RegularizacaoFundiariaValidar validar)
		{
			_configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

			_validar = validar;
			_da = new RegularizacaoFundiariaDa();
			_busCaracterizacao = new CaracterizacaoBus();
			_busDominialidade = new DominialidadeBus();
			_caracterizacaoValidar = new CaracterizacaoValidar();
		}

		#region Comandos DML

		public bool Salvar(RegularizacaoFundiaria caracterizacao)
		{
			try
			{
				if (_validar.Salvar(caracterizacao))
				{
					caracterizacao.Posses.ForEach(x =>
					{
						x.UsoAtualSolo.ForEach(y =>
						{
							if (TiposUsos.Exists(uso => uso.Id == y.TipoDeUso))
							{
								y.TipoDeUsoGeo = TiposUsos.First(tiposusos => tiposusos.Id == y.TipoDeUso).TipoGeo;
							}
						});
					});

					GerenciadorTransacao.ObterIDAtual();

					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();

						_da.Salvar(caracterizacao, bancoDeDados);

						#region Gerencia as dependências da caracterização

						_busCaracterizacao.Dependencias(new Caracterizacao()
						{
							Id = caracterizacao.Id,
							Tipo = eCaracterizacao.RegularizacaoFundiaria,
							DependenteTipo = eCaracterizacaoDependenciaTipo.Caracterizacao,
							Dependencias = caracterizacao.Dependencias
						}, bancoDeDados);

						#endregion

						Validacao.Add(Mensagem.RegularizacaoFundiaria.Salvar);

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

				if (validarDependencias && !_caracterizacaoValidar.DependenciasExcluir(empreendimento, eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao))
				{
					return Validacao.EhValido;
				}

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					_da.Excluir(empreendimento, bancoDeDados);

					Validacao.Add(Mensagem.RegularizacaoFundiaria.Excluir);

					bancoDeDados.Commit();
				}

			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool CopiarDadosCredenciado(Dependencia caracterizacao, int empreendimentoInternoId, BancoDeDados bancoDeDados, BancoDeDados bancoCredenciado = null)
		{
			throw new NotImplementedException();
		}

		#endregion

		#region Obter

		public RegularizacaoFundiaria ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			RegularizacaoFundiaria caracterizacao = null;

			try
			{
				caracterizacao = _da.ObterPorEmpreendimento(empreendimento, simplificado, banco);
				caracterizacao.Dependencias = _busCaracterizacao.ObterDependencias(caracterizacao.Id, eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao, banco);

				int zona = (int)_busCaracterizacao.ObterEmpreendimentoSimplificado(empreendimento).ZonaLocalizacao;
				caracterizacao.Posses.ForEach(x =>
				{
					x.Zona = zona;
				});

				caracterizacao.Matriculas = _busDominialidade.ObterPorEmpreendimento(empreendimento).Dominios.Where(x => x.Tipo == eDominioTipo.Matricula).ToList();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacao;
		}

		public RegularizacaoFundiaria Obter(int id, string tid = null, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				RegularizacaoFundiaria caracterizacao;

				if (tid == null)
				{
					caracterizacao = _da.Obter(id, simplificado, banco);
				}
				else
				{
					caracterizacao = _da.ObterHistorico(id, tid, simplificado, banco);
				}

				caracterizacao.Matriculas = _busDominialidade.ObterPorEmpreendimento(caracterizacao.EmpreendimentoId).Dominios.Where(x => x.Tipo == eDominioTipo.Matricula).ToList();

				return caracterizacao;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public RegularizacaoFundiaria ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{
			RegularizacaoFundiaria caracterizacao = new RegularizacaoFundiaria();

			try
			{
				Dominialidade dominialidade = _busDominialidade.ObterPorEmpreendimento(empreendimento);
				int zona = (int)_busCaracterizacao.ObterEmpreendimentoSimplificado(empreendimento).ZonaLocalizacao;

				//dominialidade.Dominios.Where(x => x.Tipo == eDominioTipo.Posse).ToList().ForEach(x =>
				//{
				//	caracterizacao.Posses.Add(new Posse(x, zona));
				//});
				caracterizacao.Posses = ObterPosses(empreendimento, zona);
				caracterizacao.Matriculas = dominialidade.Dominios.Where(x => x.Tipo == eDominioTipo.Matricula).ToList();
				return caracterizacao;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		//public RegularizacaoFundiaria ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		//{
		//	RegularizacaoFundiaria caracterizacao = new RegularizacaoFundiaria();

		//	try
		//	{
		//		Dominialidade dominialidade = _busDominialidade.ObterPorEmpreendimento(empreendimento);
		//		int zona = (int)_busCaracterizacao.ObterEmpreendimentoSimplificado(empreendimento).ZonaLocalizacao;

		//		dominialidade.Dominios.Where(x => x.Tipo == eDominioTipo.Posse).ToList().ForEach(x =>
		//		{
		//			caracterizacao.Posses.Add(new Posse(x, zona));
		//		});

		//		caracterizacao.Matriculas = dominialidade.Dominios.Where(x => x.Tipo == eDominioTipo.Matricula).ToList();
		//		return caracterizacao;
		//	}
		//	catch (Exception exc)
		//	{
		//		Validacao.AddErro(exc);
		//	}

		//	return null;
		//}

		public List<Posse> ObterPosses(int empreendimento, int zona, BancoDeDados banco = null)
		{
			List<Posse> posses = new List<Posse>();

			try
			{
				posses = _da.ObterPosses(empreendimento,zona,banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return posses;
		}

		public RegularizacaoFundiaria MergiarGeo(RegularizacaoFundiaria caracterizacaoAtual)
		{
			RegularizacaoFundiaria regularizacaoGeo = ObterDadosGeo(caracterizacaoAtual.EmpreendimentoId);
			caracterizacaoAtual.Dependencias = _busCaracterizacao.ObterDependenciasAtual(caracterizacaoAtual.EmpreendimentoId, eCaracterizacao.RegularizacaoFundiaria, eCaracterizacaoDependenciaTipo.Caracterizacao);

			List<Posse> removerPosse = new List<Posse>();

			foreach (Posse posseAtual in caracterizacaoAtual.Posses)
			{
				if (!regularizacaoGeo.Posses.Exists(x => posseAtual.Identificacao == x.Identificacao))
				{
					removerPosse.Add(posseAtual);
				}
			}
			caracterizacaoAtual.Posses.RemoveAll(x => removerPosse.Exists(y => y.Identificacao == x.Identificacao)); ;

			foreach (Posse posse in regularizacaoGeo.Posses)
			{
				if (!caracterizacaoAtual.Posses.Exists(x => x.Identificacao == posse.Identificacao))
				{
					caracterizacaoAtual.Posses.Add(posse);
				}
				else
				{
					Posse posseAtual = caracterizacaoAtual.Posses.Single(x => x.Identificacao == posse.Identificacao);
					posseAtual.ComprovacaoTexto = posse.ComprovacaoTexto;
					posseAtual.AreaCroqui = posse.AreaCroqui;
					posseAtual.Zona = posse.Zona;
				}
			}

			return caracterizacaoAtual;
		}

		public List<UsoAtualSolo> ObterUsoAtualSoloPosseDominialidade(int empreendimento, string strItendificacao, BancoDeDados banco = null)
		{
			List<UsoAtualSolo> list = new List<UsoAtualSolo>();
			List<AreaGeo> listGeo = new List<AreaGeo>();
			AreaGeo areaTotal = null;

			listGeo = _da.ObterUsoAtualSoloPosseDominialidade(empreendimento, strItendificacao, banco);

			areaTotal = listGeo.FirstOrDefault(x => x.Descricao == "Área Total Posse");

			listGeo.Remove(areaTotal);

			listGeo.ForEach(x =>
			{
				UsoAtualSolo usoAtualSolo = new UsoAtualSolo();
				usoAtualSolo.TipoDeUsoGeo = String.IsNullOrEmpty(x.SubTipo) ? x.Descricao : x.SubTipo;
				usoAtualSolo.AreaPorcentagem = Convert.ToInt32((x.AreaM2 * 100) / areaTotal.AreaM2);

				usoAtualSolo.TipoDeUso = TiposUsos.FirstOrDefault(y => String.Equals(y.TipoGeo, usoAtualSolo.TipoDeUsoGeo, StringComparison.InvariantCultureIgnoreCase)).Id;

				list.Add(usoAtualSolo);
			});

			return list;
		}

		public List<int> ObterAtividadesCaracterizacao(int empreendimento, BancoDeDados banco = null)
		{
			//Caso a coluna das atividades NÃO esteja na tabela Principal
			throw new NotImplementedException();
		}

		public object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco = null)
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}