using System;
using System.Web;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Business
{
	public class TramitacaoCredenciadoBus
	{
		#region Propriedades

		TramitacaoCredenciadoDa _da;

		public EtramiteIdentity User
		{
			get
			{
				try
				{
					return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity;
				}
				catch (Exception exc)
				{
					Validacao.AddErro(exc);
					return null;
				}
			}
		}

		#endregion

		public TramitacaoCredenciadoBus()
		{
			_da = new TramitacaoCredenciadoDa();
		}

		public Resultados<Tramitacao> FiltrarHistorico(ListarTramitacaoFiltro filtrosListar)
		{
			try
			{
				return _da.FiltrarHistorico(new Filtro<ListarTramitacaoFiltro>(filtrosListar));
			}
			catch (Exception exc)
			{
				Validacao.AddErro(exc);
			}

			return new Resultados<Tramitacao>();
		}
	}
}