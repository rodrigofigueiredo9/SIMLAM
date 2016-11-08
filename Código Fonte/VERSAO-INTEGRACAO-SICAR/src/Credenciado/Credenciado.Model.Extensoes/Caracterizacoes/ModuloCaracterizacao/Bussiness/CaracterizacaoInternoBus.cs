using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Bussiness
{
	public class CaracterizacaoInternoBus
	{
		#region Propriedade

		CaracterizacaoInternoDa _da;

		#endregion

		public CaracterizacaoInternoBus()
		{
			_da = new CaracterizacaoInternoDa();
		}

		#region Obter

		public List<Caracterizacao> ObterCaracterizacoesAtuais(int empreendimentoID, List<CaracterizacaoLst> caracterizacoes)
		{
			try
			{
				return _da.ObterCaracterizacoesAtuais(empreendimentoID, caracterizacoes);
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