using System;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;
using Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Business
{
	public class CondicionanteBus
	{
		#region Propriedades
		CondicionanteValidar _validar = null;
		FuncionarioBus _busFuncionario = new FuncionarioBus();
		CondicionanteDa _da = new CondicionanteDa();

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());		
		ListaBus _listaBus = new ListaBus();

		public EtramiteIdentity User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}
		#endregion

		public CondicionanteBus() { _validar = new CondicionanteValidar(); }

		public CondicionanteBus(CondicionanteValidar validar)
		{
			_validar = validar;
		}

		public TituloCondicionante Obter(int id, bool validar = true)
		{
			try
			{
				TituloCondicionante cond = _da.Obter(id);
				if (validar && (cond == null || cond.Id <= 0))
				{
					Validacao.Add(Mensagem.Condicionante.Inexistente);
				}
				return cond;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<TituloCondicionante> Filtrar(TituloCondicionanteFiltro filtros, Paginacao paginacao)
		{
			try
			{
				Filtro<TituloCondicionanteFiltro> filtrosDa = new Filtro<TituloCondicionanteFiltro>(filtros, paginacao);
				return _da.Filtrar(filtrosDa);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public Resultados<TituloCondicionante> FiltrarDescricao(TituloCondicionanteFiltro filtros, Paginacao paginacao)
		{
			try
			{
				Filtro<TituloCondicionanteFiltro> filtrosDa = new Filtro<TituloCondicionanteFiltro>(filtros, paginacao);
				return _da.FiltrarDescricao(filtrosDa);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public bool DescricaoSalvar(TituloCondicionanteDescricao descricao)
		{
			try
			{
				if (_validar.DescricaoSalvar(descricao))
				{
					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{
						bancoDeDados.IniciarTransacao();
						_da.DescricaoSalvar(descricao);
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

		public bool SalvarValidar(TituloCondicionante condicionante)
		{
			try
			{
				_validar.Salvar(condicionante);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public bool Salvar(TituloCondicionante condicionante)
		{
			try
			{
				if (_validar.Salvar(condicionante))
				{
					GerenciadorTransacao.ObterIDAtual();
					using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
					{

						bancoDeDados.IniciarTransacao();
						_da.Salvar(condicionante, bancoDeDados);

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

		public TituloCondicionante Prorrogar(int condicionanteId, int periodicidadeId, int? diasProrrogados)
		{
			TituloCondicionante cond = null;
			TituloCondicionantePeriodicidade periodicidade = null;

			try
			{
				if (condicionanteId > 0)
				{
					cond = _da.Obter(condicionanteId);

					if (periodicidadeId > 0)
					{
						periodicidade = cond.Periodicidades.SingleOrDefault(x => x.Id == periodicidadeId);
					}
				}

				if (_validar.Prorrogar(cond, periodicidade, diasProrrogados))
				{
					if (periodicidadeId == 0 || periodicidade == null)
					{
						cond.DiasProrrogados = (cond.DiasProrrogados ?? 0) + diasProrrogados;
						if (cond.Situacao.Id == 2 || cond.Situacao.Id == 3)
						{
							cond.DataVencimento.Data = cond.DataInicioPrazo.Data.Value.AddDays(diasProrrogados.GetValueOrDefault());
						}
						//3 - Prorrogado
						cond.Situacao = _listaBus.TituloCondicionanteSituacoes.Single(x => x.Id == 3);
					}
					else
					{
						periodicidade.DiasProrrogados = (periodicidade.DiasProrrogados ?? 0) + diasProrrogados;
						if (periodicidade.Situacao.Id == 2 || periodicidade.Situacao.Id == 3)
						{
							periodicidade.DataVencimento.Data = periodicidade.DataVencimento.Data.Value.AddDays(diasProrrogados.GetValueOrDefault());
						}
						//3 - Prorrogado
						periodicidade.Situacao = _listaBus.TituloCondicionanteSituacoes.Single(x => x.Id == 3);

						cond.Periodicidades[cond.Periodicidades.FindIndex(x => x.Id == periodicidade.Id)] = periodicidade;
					}
					
					_da.Prorrogar(cond, periodicidade);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return cond;
		}

		public TituloCondicionante Atender(int condicionanteId, int periodicidadeId)
		{
			TituloCondicionante cond = null;
			TituloCondicionantePeriodicidade periodicidade = null;
			try
			{
				if (condicionanteId > 0)
				{
					cond = _da.Obter(condicionanteId);

					if (periodicidadeId > 0)
					{
						periodicidade = cond.Periodicidades.SingleOrDefault(x => x.Id == periodicidadeId);
					}
				}

				if (_validar.Atender(cond, periodicidade))
				{
					if (cond.Situacao.Id != 4)
					{
						if (periodicidadeId == 0)
						{
							//4 - Prorrogado
							cond.Situacao = _listaBus.TituloCondicionanteSituacoes.Single(x => x.Id == 4);
						}
						else
						{
							periodicidade.Situacao = _listaBus.TituloCondicionanteSituacoes.Single(x => x.Id == 4);
						}

						_da.AlterarSituacao(cond, periodicidade);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return cond;
		}

		public bool ValidarAssociarTitulo(TituloCondicionante condicionante)
		{
			_validar.AssociarTitulo(condicionante);
			return Validacao.EhValido;
		}

		public bool DescricaoValidarExcluir(TituloCondicionanteDescricao cond)
		{
			_validar.DescricaoExcluir(cond);
			return Validacao.EhValido;
		}

		public bool DescricaoValidarEditar(TituloCondicionanteDescricao condDesc)
		{
			_validar.DescricaoEditar(condDesc);
			return Validacao.EhValido;
		}

		public bool DescricaoValidarEditar(int id)
		{
			_validar.DescricaoEditar(id);
			return Validacao.EhValido;
		}

		public void DescricaoExcluir(int id)
		{
			try
			{
				if (_validar.DescricaoExcluir(new TituloCondicionanteDescricao() { Id = id }))
				{
					_da.DescricaoExcluir(id);
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
		}

		public TituloCondicionanteDescricao DescricaoObter(int id)
		{
			return _da.DescricaoObter(id);
		}
	}
}
