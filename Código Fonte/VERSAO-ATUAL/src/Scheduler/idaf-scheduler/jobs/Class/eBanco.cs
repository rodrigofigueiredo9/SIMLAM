namespace Tecnomapas.EtramiteX.Scheduler.jobs
{
	public enum eBanco
	{
		Institucional = 1,
		Credenciado = 2
	}

	public static class Enum {
		public static int ToInt(this eBanco values) => (int)values;
	}
}
