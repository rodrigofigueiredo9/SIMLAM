using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Business
{
	public class AtividadeCredenciadoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		AtividadeCredenciadoDa _da;

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public AtividadeCredenciadoBus() 
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new AtividadeCredenciadoDa();
		}

		public List<Lista> ObterAtividadesLista(int requerimentoId)
		{
			try
			{
				return _da.ObterAtividadesLista(requerimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}

		public List<AtividadeSolicitada> ObterAtividadesListaReq(int requerimentoId)
		{
			try
			{
				return _da.ObterAtividadesListaReq(requerimentoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}
			return null;
		}
	}
}