using System;
using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class GerenciadorConfiguracao
	{
		IConfiguracao _config;
		public GerenciadorConfiguracao(IConfiguracao confg) => _config = confg;

		public TRet Obter<TRet>(String idx) => (TRet)_config[idx];

		public Object Obter(String idx) => _config[idx];
	}

	public class GerenciadorConfiguracao<T> : GerenciadorConfiguracao
	{
		public GerenciadorConfiguracao(IConfiguracao confg) : base(confg) { }

		public T Instancia { get; set; }
	}
}