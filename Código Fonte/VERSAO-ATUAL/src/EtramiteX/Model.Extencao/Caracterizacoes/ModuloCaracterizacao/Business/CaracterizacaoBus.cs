using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Business;
using CaractCredBus = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness;
using ProjetoDigitalCredBus = Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;
using CaracterizacaoCredenciadoBus = Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness.CaracterizacaoBus;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProjetoDigital.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Business
{
	public class CaracterizacaoBus
	{
		#region Propriedade

		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig;
		CaracterizacaoValidar _validar;
		CaracterizacaoDa _da;
		CaracterizacaoCredenciadoBus _busCaracterizacaoCredenciado;

		public GerenciadorConfiguracao<ConfiguracaoCaracterizacao> CaracterizacaoConfig
		{
			get { return _caracterizacaoConfig; }
		}

		public static EtramitePrincipal User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal); }
		}

		public CaracterizacaoValidar Validar { get { return _validar; } }

		public CaracterizacaoDa daTeste { get; set; }

		#endregion

		public CaracterizacaoBus()
		{
			_da = new CaracterizacaoDa();
			_caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
			_busCaracterizacaoCredenciado = new CaracterizacaoCredenciadoBus();
		}
		public CaracterizacaoBus(CaracterizacaoValidar validar)
		{
			_validar = validar;
			_da = new CaracterizacaoDa();
			_caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		}

		#region Ações DML

		public bool ExcluirCaracterizacoes(int empreendimento, BancoDeDados banco = null)
		{
			List<CaracterizacaoLst> caracterizacoes = CaracterizacaoConfig.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			ICaracterizacaoBus caracterizacao;

			try
			{
				if (!ValidarEmPosse(empreendimento))
				{
					return Validacao.EhValido;
				}

				if (caracterizacoes != null && caracterizacoes.Count > 0)
				{
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
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

		public bool CopiarDadosCredenciado(int projetoDigitalID, BancoDeDados banco, BancoDeDados bancoCredenciado)
		{
			CaractCredBus.CaracterizacaoBus caracCredBus = new CaractCredBus.CaracterizacaoBus();
			ProjetoDigitalCredenciadoBus projetoDigitalCredenciadoBus = new ProjetoDigitalCredenciadoBus();
			int empreendimentoID = projetoDigitalCredenciadoBus.Obter(projetoDigitalID, 0, null, null).EmpreendimentoId ?? 0;

			if (empreendimentoID < 1)
			{
				return false;
			}

			EmpreendimentoCaracterizacao empreendimento = caracCredBus.ObterEmpreendimentoSimplificado(empreendimentoID);
			ProjetoGeograficoBus projetoGeograficoBus = new ProjetoGeograficoBus();

			List<Dependencia> dependencias = projetoDigitalCredenciadoBus.ObterDependencias(projetoDigitalID);
			ICaracterizacaoBus caracterizacaoBus;
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				foreach (Dependencia item in dependencias.Where(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico).ToList())
				{
					projetoGeograficoBus.CopiarDadosCredenciado(item, empreendimento.InternoID, bancoDeDados, bancoCredenciado);

					if (!Validacao.EhValido)
					{
						break;
					}
				}

				if (!Validacao.EhValido)
				{
					bancoDeDados.Rollback();
					return false;
				}

				foreach (Dependencia item in dependencias.Where(x => x.DependenciaTipo == (int)eCaracterizacaoDependenciaTipo.Caracterizacao).ToList())
				{
					caracterizacaoBus = CaracterizacaoBusFactory.Criar((eCaracterizacao)item.DependenciaCaracterizacao);

					if (caracterizacaoBus == null)
					{
						Validacao.Add(eTipoMensagem.Erro, item.DependenciaCaracterizacaoTexto + " Bus não criada");
					}

					if (!caracterizacaoBus.CopiarDadosCredenciado(item, empreendimento.InternoID, bancoDeDados, bancoCredenciado))
					{
						break;
					}
				}

				if (!Validacao.EhValido)
				{
					bancoDeDados.Rollback();
					return false;
				}

				Validacao.Add(Mensagem.ProjetoDigital.CopiarCaracterizacao);
				bancoDeDados.Commit();
			}

			return true;
		}

		public void AtualizarDependentes(int dependenciaID, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo eCaracterizacaoDependenciaTipo, string dependenciaTID, BancoDeDados banco = null)
		{
			_da.AtualizarDependentes(dependenciaID, caracterizacaoTipo, eCaracterizacaoDependenciaTipo, dependenciaTID, banco);
		}

		#endregion

		#region Obter/Validações

        public bool ExisteCaracterizacaoPorEmpreendimento(Int64 empreendimentoCod, int empreendimentoId, BancoDeDados banco = null)
        {
            List<Caracterizacao> caracterizacoes = null;

            try
            {
                /*
                caracterizacoes = ObterCaracterizacoesEmpreendimento(empreendimentoId);

                foreach (var carac in caracterizacoes)
                {
                    if (carac.Tipo == eCaracterizacao.CadastroAmbientalRural)
                    {
                        return true;
                    }
                }*/

                caracterizacoes = ObterCaracterizacoesCAR(empreendimentoCod);

                foreach (var carac in caracterizacoes)
                {
                    if (carac.Tipo == eCaracterizacao.CadastroAmbientalRural)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return false;
        }

		public List<Caracterizacao> ObterCaracterizacoesEmpreendimento(int empreendimentoId)
		{
			try
			{
				return _da.ObterCaracterizacoes(empreendimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

        public List<Caracterizacao> ObterCaracterizacoesCAR(Int64 empreendimentoCod)
        {
            try
            {
                return _da.ObterCaracterizacoesCAR(empreendimentoCod);
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

		public List<Dependencia> ObterDependenciasAtual(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo)
		{
			try
			{
				return _da.ObterDependenciasAtual(empreendimentoId, caracterizacaoTipo, tipo);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<Dependencia> ObterDependencias(int dependenteId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterDependencias(dependenteId, caracterizacaoTipo, tipo, banco);
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

		public List<Caracterizacao> Dependentes(int empreendimentoId, eCaracterizacao caracterizacaoTipo, eCaracterizacaoDependenciaTipo tipo, BancoDeDados banco = null)
		{
			List<Caracterizacao> lista = new List<Caracterizacao>();
			List<DependenciaLst> dependentes = _caracterizacaoConfig.Obter<List<DependenciaLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias);
			dependentes = dependentes.Where(x => x.DependenciaTipo == (int)caracterizacaoTipo && x.TipoId == (int)tipo).ToList();

			List<Caracterizacao> caracterizacoes = _da.ObterCaracterizacoes(empreendimentoId, banco);
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

		public int Existe(int empreendimento, eCaracterizacao caracterizacaoTipo)
		{
			List<Caracterizacao> caracterizacoes = ObterCaracterizacoesEmpreendimento(empreendimento);

			return (caracterizacoes.OrderByDescending(x => x.Id).FirstOrDefault(x => x.Tipo == caracterizacaoTipo) ?? new Caracterizacao()).Id;
		}

		public List<CaracterizacaoLst> ObterCaracterizacoes(int projetoDigitalId)
		{
			try
			{
				return _busCaracterizacaoCredenciado.ObterCaracterizacoesPorProjetoDigital(projetoDigitalId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public int ObterPessoaID(String cpfCnpj, BancoDeDados banco = null)
		{
			try
			{
				return _da.ObterPessoaID(cpfCnpj, banco);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return 0;
		}

		#endregion
	}
}