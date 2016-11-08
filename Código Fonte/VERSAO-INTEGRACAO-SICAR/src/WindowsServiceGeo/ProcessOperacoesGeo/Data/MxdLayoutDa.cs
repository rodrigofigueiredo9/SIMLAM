using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.ArcGIS;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Data
{
	class MxdLayoutDa
	{
		public int ObterQuantidade(LayerItem layer, params object[] parametros)
		{
			string strSQL = String.Format(@"select count(*) from " + layer.Source + " where " + layer.Query, parametros);

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(strSQL);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}
	}
}