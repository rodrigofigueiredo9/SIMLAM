using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Data;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Business
{
	public class HabilitarEmissaoCFOCFOCBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		HabilitarEmissaoCFOCFOCDa _da;

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public List<QuantPaginacao> LstQuantPaginacao
		{
			get { return _configSys.Obter<List<QuantPaginacao>>(ConfiguracaoSistema.KeyLstQuantPaginacao); }
		}

		#endregion

		public HabilitarEmissaoCFOCFOCBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new HabilitarEmissaoCFOCFOCDa(UsuarioCredenciado);
		}

		#region Obter/Listar

		public Resultados<Cred.PragaHabilitarEmissao> Filtrar(Cred.ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
				//Seta o id do usuário logado
				filtrosListar.Id = User.UsuarioId.ToString();
				var filtros = new Filtro<Cred.ListarFiltro>(filtrosListar, paginacao);
				var resultados = _da.Filtrar(filtros);

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

		public HabilitarEmissaoCFOCFOC ObterPorCredenciado(int credenciadoId, bool simplificado = false)
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

		#endregion

		public bool VerificarCredenciadoHabilitado()
		{
			try
			{
				return _da.VerificarCredenciadoHabilitado();
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return false;
		}
	}
}