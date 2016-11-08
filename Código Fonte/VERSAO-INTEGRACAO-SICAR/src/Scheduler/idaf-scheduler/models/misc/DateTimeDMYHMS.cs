using Newtonsoft.Json.Converters;

namespace Tecnomapas.EtramiteX.Scheduler.models.misc
{
	public class DateTimeDMYHMS : IsoDateTimeConverter
	{
		public DateTimeDMYHMS()
		{
			DateTimeFormat = "dd/MM/yyyy HH:mm:ss";
		}
	}
}
