using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business
{
	public class TituloDeclaratorioConfiguracaoBus
	{
		#region Properties

		TituloDeclaratorioConfiguracaoDa _da = new TituloDeclaratorioConfiguracaoDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		TituloDeclaratorioConfiguracaoValidar _validar = new TituloDeclaratorioConfiguracaoValidar();

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion Properties

		public TituloDeclaratorioConfiguracaoBus()
		{
		}

		public void Salvar(TituloDeclaratorioConfiguracao configuracao)
		{
			try
			{
				GerenciadorTransacao.ObterIDAtual();
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					bancoDeDados.IniciarTransacao();

					#region Arquivo

					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);
					ArquivoDa _arquivoDa = new ArquivoDa();

					if (configuracao.BarragemSemAPP.Id != null && configuracao.BarragemSemAPP.Id == 0)
						configuracao.BarragemSemAPP = _busArquivo.Copiar(configuracao.BarragemSemAPP);

					if (configuracao.BarragemSemAPP.Id == 0)
						_arquivoDa.Salvar(configuracao.BarragemSemAPP, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);

					if (configuracao.BarragemComAPP.Id != null && configuracao.BarragemComAPP.Id == 0)
						configuracao.BarragemComAPP = _busArquivo.Copiar(configuracao.BarragemComAPP);

					if (configuracao.BarragemComAPP.Id == 0)
						_arquivoDa.Salvar(configuracao.BarragemComAPP, User.FuncionarioId, User.Name, User.Login, (int)eExecutorTipo.Interno, User.FuncionarioTid, bancoDeDados);

					#endregion

					_da.Salvar(configuracao, bancoDeDados);

					bancoDeDados.Commit();
					Validacao.Add(Mensagem.TituloDeclaratorioConfiguracao.SalvoSucesso);
				}

			}
			catch (Exception ex)
			{
				Validacao.AddErro(ex);
			}
		}

		#region Obter / Filtrar

		public TituloDeclaratorioConfiguracao Obter(int id = 0) => _da.Obter(id);

		public Resultados<RelatorioTituloDecListarResultado> Filtrar(RelatorioTituloDecListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				if (!_validar.Filtrar(filtrosListar))
				{
					return new Resultados<RelatorioTituloDecListarResultado>();
				}
				
				Filtro<RelatorioTituloDecListarFiltro> filtro = new Filtro<RelatorioTituloDecListarFiltro>(filtrosListar, paginacao);
				Resultados<RelatorioTituloDecListarResultado> resultados = _da.Filtrar(filtro);

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

		#endregion  Obter / Filtrar
	}
}
