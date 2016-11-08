using System;

namespace Tecnomapas.EtramiteX.WindowsService.SVCOperacoesGeo
{
	public class ItemFila
	{
		private int _tipo;
		private int _etapa;

		public ItemFila(string texto)
		{
			string[] valArray = texto.Replace('*', '0').Split('.');

			_tipo = Int32.Parse(valArray[0]);
			_etapa = Int32.Parse(valArray[1]);
		}

		public int tipo
		{
			get
			{
				return _tipo;
			}
		}

		public int etapa
		{
			get
			{
				return _etapa;
			}
		}
	}
}