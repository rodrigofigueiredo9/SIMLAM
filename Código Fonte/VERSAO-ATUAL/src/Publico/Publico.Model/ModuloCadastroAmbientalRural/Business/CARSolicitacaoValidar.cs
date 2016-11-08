using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;

using Tecnomapas.EtramiteX.Publico.Model.ModuloCadastroAmbientalRural.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloCadastroAmbientalRural.Business
{
	public class CARSolicitacaoValidar
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys;
		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public CARSolicitacaoValidar()
		{
			_configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		}

		public bool Buscar(SolicitacaoListarFiltro filtrosListar)
		{
			if (filtrosListar.IsSolicitacaoNumero && string.IsNullOrEmpty(filtrosListar.SolicitacaoTituloNumero))
			{
				Validacao.Add(Mensagem.CARSolicitacao.SolicitacaoNumeroObrigatorio);
			}

			if (filtrosListar.IsTituloNumero && string.IsNullOrEmpty(filtrosListar.SolicitacaoTituloNumero))
			{
				Validacao.Add(Mensagem.CARSolicitacao.TituloNumeroObrigatorio);
			}

			if (filtrosListar.IsCPF && string.IsNullOrEmpty(filtrosListar.DeclaranteCPFCNPJ))
			{
				Validacao.Add(Mensagem.CARSolicitacao.CPFObrigatorio);
			}

			if (filtrosListar.IsCNPJ && string.IsNullOrEmpty(filtrosListar.DeclaranteCPFCNPJ))
			{
				Validacao.Add(Mensagem.CARSolicitacao.CNPJObrigatorio);
			}

			return Validacao.EhValido;
		}
	}
}