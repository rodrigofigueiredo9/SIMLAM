using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCImportarTituloDeclaratorio.Business
{
	public class CredenciadoBus
	{
		CredenciadoDa _da = new CredenciadoDa();
		ConfiguracaoSistema _configSys = new ConfiguracaoSistema();

		public String UsuarioCredenciado
		{
			get { return _configSys.UsuarioCredenciado; }
		}

	    public CredenciadoPessoa Obter(int id, bool simplificado = false, BancoDeDados banco = null)
	    {

	        using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
	        {
	            return _da.Obter(id, simplificado, bancoDeDados);
	        }
	    }
	}
}