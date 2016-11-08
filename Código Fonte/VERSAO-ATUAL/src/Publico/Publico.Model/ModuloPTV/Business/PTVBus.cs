using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.EtramiteX.Publico.Model.ModuloPTV.Data;


namespace Tecnomapas.EtramiteX.Publico.Model.ModuloPTV.Business
{
    public class PTVBus
    {
        #region Propriedades

        PTVDa _da = new PTVDa();

        #endregion

		public Resultados<PTVListarResultado> Filtrar(PTVListarFiltro ptvListarFiltro, Paginacao paginacao)
		{
			try
			{
				Filtro<PTVListarFiltro> filtro = new Filtro<PTVListarFiltro>(ptvListarFiltro, paginacao);
				Resultados<PTVListarResultado> resultados = _da.Filtrar(filtro);

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
    }
}
