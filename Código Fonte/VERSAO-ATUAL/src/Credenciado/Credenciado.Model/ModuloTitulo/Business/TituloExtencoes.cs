using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Especificidades.ModuloEspecificidade.Business;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Business
{
	public static class TituloExtencoes
	{
		public static bool PossuiEspecificidade(this TituloModelo modelo)
		{
			int codigoDoModelo = (modelo.Codigo != null && modelo.Codigo.HasValue ? modelo.Codigo.Value : 0);
			return EspecificiadadeBusFactory.Possui(codigoDoModelo);
		}
	}
}