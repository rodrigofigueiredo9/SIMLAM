using System;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessOperacoesGeo.Business
{
	class CoordenadaBus
	{
		public static Decimal CalcularDistancia(Decimal x1, Decimal y1, Decimal x2, Decimal y2)
		{
			return (Decimal)Math.Sqrt(Math.Pow(Decimal.ToDouble(x2 - x1), 2) + Math.Pow(Decimal.ToDouble(y2 - y1), 2));
		}

		public static Decimal CalcularAzimute(Decimal x1, Decimal y1, Decimal x2, Decimal y2)
		{
			Decimal angle = 360 - ((360 - fromRadianToDegree(Math.Atan2(Decimal.ToDouble(x2 - x1), Decimal.ToDouble(y2 - y1)))) % 360);
			return (angle == 360) ? 0 : angle;
		}

		public static Decimal fromRadianToDegree(double rad)
		{
			return (Decimal)(180 * rad / Math.PI);
		}
	}
}