using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Business
{
	class ArquivarInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		ArquivarInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public ArquivarInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new ArquivarInternoDa(UsuarioInterno);
		}

		public Arquivar ObterArquivamento(int tramitacaoId = 0)
		{
			try
			{
				return _da.ObterArquivamento(tramitacaoId);
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return null;
		}
	}
}
