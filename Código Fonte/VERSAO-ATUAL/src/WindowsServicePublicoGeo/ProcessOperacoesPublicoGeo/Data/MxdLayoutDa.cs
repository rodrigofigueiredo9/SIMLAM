using System;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.ArcGIS;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesPublicoGeo.Data
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