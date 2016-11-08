using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Bussiness;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness
{
	public class CaracterizacaoBus
	{
		public delegate void DesassociarDependencias(ProjetoDigital projetoDigital, BancoDeDados banco);

		#region Propriedade

		ConfiguracaoSistema _config;
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig;
		CaracterizacaoValidar _validar;
		CaracterizacaoDa _da;

		public GerenciadorConfiguracao<ConfiguracaoCaracterizacao> CaracterizacaoConfig
		{
			get { return _caracterizacaoConfig; }
		}

		public static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		private string UsuarioCredenciado
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public CaracterizacaoValidar Validar { get { return _validar; } }

		#endregion

		public CaracterizacaoBus()
		{
			_da = new CaracterizacaoDa();
			_config = new ConfiguracaoSistema();
			_caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		}

		public CaracterizacaoBus(CaracterizacaoValidar validar)
		{
			_validar = validar;
			_da = new CaracterizacaoDa();
			_config = new ConfiguracaoSistema();
			_caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		}

		#region Ações DML

		public bool ExcluirCaracterizacoes(int empreendimento, BancoDeDados banco = null)
		{
			List<CaracterizacaoLst> caracterizacoes = CaracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			List<DependenciaLst> dependencias = CaracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);
			caracterizacoes = caracterizacoes.Where(x => dependencias.Exists(y => y.TipoId == x.Id && y.ExibirCredenciado)).ToList();
			ICaracterizacaoBus caracterizacao;

			try
			{
				if (!ValidarEmPosse(empreendimento))
				{
					return Validacao.EhValido;
				}

				if (caracterizacoes != null && caracterizacoes.Count > 0)
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
					{
						bancoDeDados.IniciarTransacao();

						GerenciadorTransacao.ObterIDAtual();

						foreach (CaracterizacaoLst car in caracterizacoes)
						{
							caracterizacao = CaracterizacaoBusFactory.Criar((eCaracterizacao)car.Id);

							if (caracterizacao == null)
							{
								Validacao.Add(eTipoMensagem.Erro, String.Format("{0} Bus não criada", car.Texto));
								continue;
							}

							caracterizacao.Excluir(empreendimento, bancoDeDados, false);
						}

						bancoDeDados.Commit();
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void Dependencias(Caracterizacao caracterizacao, BancoDeDados banco = null)
		{
			_da.Dependencias(caracterizacao, banco);
		}

		public void CopiarDadosInstitucional(int empreendimentoID, eCaracterizacao caracterizacaoTipo, int projetoDigitalID, DesassociarDependencias desassociarDependencias)
		{
			try
			{
				EmpreendimentoCaracterizacao empreendimento = ObterEmpreendimentoSimplificado(empreendimentoID);
				ProjetoGeograficoBus projetoGeograficoBus = new ProjetoGeograficoBus();

				GerenciadorTransacao.ObterIDAtual();

				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					bancoDeDados.IniciarTransacao();

                    if (caracterizacaoTipo != eCaracterizacao.UnidadeProducao && caracterizacaoTipo != eCaracterizacao.UnidadeConsolidacao && caracterizacaoTipo != eCaracterizacao.BarragemDispensaLicenca)
					{
						projetoGeograficoBus.CopiarDadosInstitucional(empreendimentoID, empreendimento.InternoID, (eCaracterizacao)caracterizacaoTipo, bancoDeDados);
					}

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return;
					}

					ICaracterizacaoBus caracterizacaoBus = CaracterizacaoBusFactory.Criar(caracterizacaoTipo);
					if (caracterizacaoBus == null)
					{
						Validacao.Add(eTipoMensagem.Erro, "Caracterizacao Bus não criada");
					}

					caracterizacaoBus.CopiarDadosInstitucional(empreendimentoID, empreendimento.InternoID, bancoDeDados);

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
						return;
					}

					#region Alterar Projeto Digital

					ProjetoDigital projetoDigital = new ProjetoDigital();
					projetoDigital.Id = projetoDigitalID;
					projetoDigital.EmpreendimentoId = empreendimentoID;
					projetoDigital.Dependencias.Add(new Dependencia() { DependenciaCaracterizacao = (int)caracterizacaoTipo });
					desassociarDependencias(projetoDigital, bancoDeDados);

					#endregion

					if (!Validacao.EhValido)
					{
						bancoDeDados.Rollback();
					}

					Validacao.Erros.Clear();
					Validacao.Add(Mensagem.ProjetoDigital.CopiarCaracterizacao);
					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public void ConfigurarEtapaExcluirCaracterizacao(int empreendimento, BancoDeDados banco)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				List<ProjetoDigital> lista = _da.ObterProjetosDigitaisExcluirCaracterizacao(empreendimento, bancoDeDados);

				_da.ExcluirTemporario(lista.Where(x => x.Situacao == (int)eProjetoDigitalSituacao.AguardandoImportacao).ToList());

				foreach (var item in lista.Where(x => (x.Situacao == (int)eProjetoDigitalSituacao.EmElaboracao || x.Situacao == (int)eProjetoDigitalSituacao.EmCorrecao) && x.Etapa != (int)eProjetoDigitalEtapa.Requerimento))
				{
					item.Etapa = (int)eProjetoDigitalEtapa.Caracterizacao;

					_da.Editar(item, bancoDeDados);
				}

				foreach (var item in lista.Where(x => x.Situacao == (int)eProjetoDigitalSituacao.AguardandoImportacao))
				{
					item.Etapa = (int)eProjetoDigitalEtapa.Caracterizacao;
					item.Situacao = (int)eProjetoDigitalSituacao.EmElaboracao;

					_da.CancelarEnvio(item, bancoDeDados);
				}

				foreach (var item in lista.Where(x => x.Situacao == (int)eProjetoDigitalSituacao.AguardandoCorrecao))
				{
					item.Etapa = (int)eProjetoDigitalEtapa.Requerimento;
					item.Situacao = (int)eProjetoDigitalSituacao.EmElaboracao;

					_da.Editar(item, bancoDeDados);
				}

				_da.DesassociarDependencias(lista, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Validações

		public List<Caracterizacao> ObterCaracterizacoesEmpreendimento(int empreendimentoId, int projetoDigitalId)
		{
			try
			{
				return _da.ObterCaracterizacoes(empreendimentoId, projetoDigitalId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public EmpreendimentoCaracterizacao ObterEmpreendimentoSimplificado(int id)
		{
			try
			{
				return _da.ObterEmpreendimentoSimplificado(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<ProjetoGeografico> ObterProjetosEmpreendimento(int id)
		{
			try
			{
				return _da.ObterProjetosEmpreendimento(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<ProjetoGeografico> ObterProjetosPorProjetoDigital(int projetoId, int empreendimentoId)
		{
			try
			{
				return _da.ObterProjetosPorProjetoDigital(projetoId, empreendimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Dependencia> ObterDependenciasAtual(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterDependenciasAtual(empreendimentoId, caracterizacaoTipo, tipo, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Dependencia> ObterDependencias(int dependenteId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, string tid = "", BancoDeDados banco = null)
		{
			try
			{
				return string.IsNullOrEmpty(tid) ? _da.ObterDependencias(dependenteId, caracterizacaoTipo, tipo, banco) : _da.ObterDependenciasHistorico(dependenteId, caracterizacaoTipo, tipo, tid, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Dependencia> ObterDependenciasAlteradas(int empreendimentoId, int dependenteId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo)
		{
			try
			{
				List<Dependencia> dependenciasBanco = _da.ObterDependenciasAtual(empreendimentoId, caracterizacaoTipo, tipo);

				Dependencia dependenciaBanco = null;
				List<Dependencia> dependencias = ObterDependencias(dependenteId, caracterizacaoTipo, tipo);

				List<Dependencia> dependenciasAteradas = new List<Dependencia>();

				foreach (Dependencia item in dependencias)
				{
					dependenciaBanco = dependenciasBanco.SingleOrDefault(x => x.DependenciaId == item.DependenciaId && x.DependenciaCaracterizacao == item.DependenciaCaracterizacao
						&& x.DependenciaTipo == item.DependenciaTipo

						) ?? new Dependencia();

					if (item.DependenciaTid != dependenciaBanco.DependenciaTid)
					{
						dependenciasAteradas.Add(dependenciaBanco);
					}
				}

				return dependenciasAteradas;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Caracterizacao> Dependentes(int empreendimentoId, int projetoDigitalId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();
			try
			{
				List<DependenciaLst> dependentes = _caracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);
				dependentes = dependentes.Where(x => x.DependenciaTipo == (int)caracterizacaoTipo && x.TipoId == (int)tipo).ToList();

				List<Caracterizacao> caracterizacoes = _da.ObterCaracterizacoes(empreendimentoId, projetoDigitalId, banco);
				caracterizacoes = caracterizacoes.Where(x => x.Id > 0 || x.ProjetoId > 0 || x.ProjetoRascunhoId > 0 || x.DscLicAtividadeId > 0).ToList();

				Caracterizacao caracterizacao = null;

				foreach (DependenciaLst dependente in dependentes)
				{
					caracterizacao = caracterizacoes.SingleOrDefault(y => (int)y.Tipo == dependente.DependenteTipo) ?? new Caracterizacao();

					if (caracterizacao.Id > 0 || (caracterizacao.ProjetoId > 0 || caracterizacao.ProjetoRascunhoId > 0 || caracterizacao.DscLicAtividadeId > 0))
					{
						lista.Add(caracterizacao);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return lista.Distinct().ToList();
		}

		public List<CoordenadaAtividade> ObterCoordenadaAtividade(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eTipoGeometria tipoGeo)
		{
			return _da.ObterCoordenadas(empreendimentoId, caracterizacaoTipo, tipoGeo);
		}

		public List<Lista> ObterCoordenadaAtividadeLst(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eTipoGeometria tipoGeo)
		{

			List<Lista> coordenadas = new List<Lista>();
			foreach (CoordenadaAtividade item in _da.ObterCoordenadas(empreendimentoId, caracterizacaoTipo, tipoGeo))
			{
				Lista itemLista = new Lista();
				itemLista.Id = item.Id.ToString() + "|" + item.CoordX + "|" + item.CoordY;
				itemLista.Texto = item.CoordenadasAtividade;
				itemLista.IsAtivo = true;
				coordenadas.Add(itemLista);
			}

			return coordenadas;
		}

		public List<int> ObterAtividades(int empreendimentoId, eCaracterizacao caracterizacaoTipo)
		{
			List<CaracterizacaoLst> caracterizacoes = CaracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			string tabela = caracterizacoes.Find(x => x.Id == (int)caracterizacaoTipo).Tabela;
			return _da.ObterAtividadesCaracterizacao(tabela, empreendimentoId);
		}

		public List<Lista> ObterTipoGeometria(int empreendimentoId, int caracterizacaoTipo)
		{
			return _da.ObterTipoDeGeometriaAtividade(empreendimentoId, caracterizacaoTipo);
		}

		public List<CaracterizacaoLst> ObterCaracterizacoesPorProjetoDigital(int projetoDigitalID, BancoDeDados banco = null)
		{
			List<CaracterizacaoLst> caracterizacoes = null;

			try
			{
				caracterizacoes = _da.ObterCaracterizacoesPorProjetoDigital(projetoDigitalID, banco);

				caracterizacoes = caracterizacoes.Where(x =>
					x.IsExibirCredenciado ||
					x.Id == (int)eCaracterizacao.UnidadeProducao ||
					x.Id == (int)eCaracterizacao.UnidadeConsolidacao ||
                    x.Id == (int)eCaracterizacao.BarragemDispensaLicenca).ToList();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacoes;
		}

		public List<Caracterizacao> ObterCaracterizacoesAssociadasProjetoDigital(int projetoDigitalID, BancoDeDados banco = null)
		{
			List<Caracterizacao> caracterizacoes = null;

			try
			{
				caracterizacoes = _da.ObterCaracterizacoesAssociadasProjetoDigital(projetoDigitalID, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return caracterizacoes;
		}

		public List<Caracterizacao> ObterCaracterizacoesAtuais(int empreendimentoID, List<Caracterizacao> caracterizacoes)
		{
			try
			{
				return _da.ObterCaracterizacoesAtuais(empreendimentoID, caracterizacoes);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<int> ObterPossivelCopiar(List<Caracterizacao> cadastradas)
		{
			List<int> retorno = new List<int>();

			try
			{
				retorno = _da.ObterPossivelCopiar(cadastradas);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public CredenciadoPessoa ObterCredenciado(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
				{
					return _da.ObterCredenciado(id, simplificado, bancoDeDados);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		#endregion

		#region Validar

		public bool ValidarEmPosse(int id)
		{
			try
			{
				if (id <= 0)
				{
					Validacao.Add(Mensagem.Caracterizacao.EmpreendimentoNaoEncontrado);
				}

				if (!_da.EmPosse(id))
				{
					Validacao.Add(Mensagem.Caracterizacao.Posse);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool ValidarAcessarTela(List<CaracterizacaoLst> caracterizacoes = null, bool mostrarMensagem = true)
		{
			if (caracterizacoes == null)
			{
				caracterizacoes = CaracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			}

			bool podeAcessar = false;
			eCaracterizacao tipo;

			caracterizacoes.ForEach(x =>
			{
				tipo = ((eCaracterizacao)x.Id);

				if (User.IsInAnyRole(String.Join(",", new[]{
				String.Format("{0}Criar", tipo.ToString()),
				String.Format("{0}Editar", tipo.ToString()),
				String.Format("{0}Visualizar", tipo.ToString()),
				String.Format("{0}Excluir", tipo.ToString())})))
				{
					podeAcessar = true;
					return;
				}
			});

			if (!podeAcessar && mostrarMensagem)
			{
				Validacao.Add(Mensagem.Padrao.SemPermissao);
			}

			return podeAcessar;
		}

		public bool ExisteEmpreendimento(int id)
		{
			return _da.ExisteEmpreendimento(id);
		}

		public int Existe(int empreendimento, int projetoDigitalId, eCaracterizacao caracterizacaoTipo)
		{
			List<Caracterizacao> caracterizacoes = ObterCaracterizacoesEmpreendimento(empreendimento, projetoDigitalId);

			return (caracterizacoes.SingleOrDefault(x => x.Tipo == caracterizacaoTipo) ?? new Caracterizacao()).Id;
		}

		#endregion

		#region Auxiliares

		public HabilitarEmissaoCFOCFOC ObterHabilitacaoPorCredenciado(int credenciadoId, bool simplificado = false)
		{
			HabilitarEmissaoCFOCFOC retorno = null;

			try
			{
				retorno = _da.ObterPorCredenciado(credenciadoId, simplificado);

				if (retorno == null || retorno.Id < 1)
				{
					Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.NaoExisteHabilitacaoParaEsteCredenciado);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return retorno;
		}

		public List<Cultura> ObterCulturas(int pragaId)
		{
			List<Cultura> culturas = null;
			try
			{
				culturas = _da.ObterCulturas(pragaId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return culturas;
		}

		#endregion Auxiliares
	}
}