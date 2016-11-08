using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloProfissao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProfissao.Business
{
	public class ProfissaoCredenciadoBus
	{
		ProfissaoCredenciadoDa _da = new ProfissaoCredenciadoDa();

		#region Filtrar
		public Resultados<Profissao> Filtrar(ProfissaoListarFiltros filtrosListar, Paginacao paginacao)
		{
			try
			{
				Filtro<ProfissaoListarFiltros> filtro = new Filtro<ProfissaoListarFiltros>(filtrosListar, paginacao);
				Resultados<Profissao> resultados = _da.Filtrar(filtro);

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
