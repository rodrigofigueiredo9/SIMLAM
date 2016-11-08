using System;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Business
{
	public class TramitacaoInternoBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		TramitacaoInternoDa _da;

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public TramitacaoInternoBus()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
			_da = new TramitacaoInternoDa(UsuarioInterno);
		}

		public Tramitacao Obter(int tramitacaoId)
		{
			return _da.Obter(tramitacaoId);
		}

		public int ObterTramitacaoProtocolo(int protocolo)
		{
			return _da.ExisteTramitacao(protocolo);
		}

		internal TramitacaoPosse ObterProtocoloPosse(int processoId)
		{
			return _da.ObterProtocoloPosse(processoId);
		}
	}
}
