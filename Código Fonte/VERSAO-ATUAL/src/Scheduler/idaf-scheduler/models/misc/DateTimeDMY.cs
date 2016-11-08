using Newtonsoft.Json.Converters;

namespace Tecnomapas.EtramiteX.Scheduler.models.misc
{
	public class DateTimeDMY : IsoDateTimeConverter
	{
		public DateTimeDMY()
		{
			DateTimeFormat = "dd/MM/yyyy";
		}
	}
}
