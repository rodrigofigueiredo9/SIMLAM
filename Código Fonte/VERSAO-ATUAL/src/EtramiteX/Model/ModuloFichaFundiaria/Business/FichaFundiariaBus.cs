using System;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloFichaFundiaria;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFichaFundiaria.Business
{
	public class FichaFundiariaBus
	{
		#region Propriedades

		FichaFundiariaDa _da = new FichaFundiariaDa();
		FichaFundiariaValidar _validar = new FichaFundiariaValidar();
		ListaBus _listaBus = new ListaBus();

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações de DML

		public bool Salvar(FichaFundiaria entidade)
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

						bancoDeDados.Commit();

						Validacao.Add(Mensagem.FichaFundiaria.Salvar);
					}
				}
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return Validacao.EhValido;
		}

		public void Excluir(int id)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();

				_da.Excluir(id, bancoDeDados);

				Validacao.Add(Mensagem.FichaFundiaria.Excluir);

				bancoDeDados.Commit();
			}
		}

		#endregion

		public FichaFundiaria Obter(int id, string tid = null)
		{
			FichaFundiaria ficha = null;
			try
			{
				ficha = _da.Obter(id);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return ficha;
		}

		public Resultados<FichaFundiaria> Filtrar(ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ListarFiltro> filtro = new Filtro<ListarFiltro>(filtrosListar, paginacao);
				Resultados<FichaFundiaria> resultados = _da.Filtrar(filtro);

				if (resultados.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				return resultados;
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}
