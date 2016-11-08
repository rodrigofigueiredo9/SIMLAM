using System;
using System.Collections.Generic;
using System.Web;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Publico.Model.ModuloHabilitarEmissaoCFOCFOC.Data;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloHabilitarEmissaoCFOCFOC.Business
{
	public class HabilitarEmissaoCFOCFOCBus
	{
		#region Propriedades

		HabilitarEmissaoCFOCFOCDa _da = new HabilitarEmissaoCFOCFOCDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações de DML

		public HabilitarEmissaoCFOCFOCBus(string Esquema = null)
		{
			_da = new HabilitarEmissaoCFOCFOCDa(Esquema);
		}

		#endregion

		#region Obter/Filtrar

		public HabilitarEmissaoCFOCFOC Obter(int id, Boolean isCredenciado = false)
		{
			HabilitarEmissaoCFOCFOC retorno = _da.Obter(id, isCredenciado: isCredenciado);

			if (retorno!=null)
			{
				return retorno;
			}
			else
			{
				Validacao.Add(Mensagem.HabilitarEmissaoCFOCFOC.CpfNaoCadastrado);
			}

			return retorno;
		}


		public Resultados<Cred.ListarFiltro> Filtrar(Cred.ListarFiltro filtrosListar, Paginacao paginacao)
		{
			try
			{
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

		#endregion

		
	}
}
