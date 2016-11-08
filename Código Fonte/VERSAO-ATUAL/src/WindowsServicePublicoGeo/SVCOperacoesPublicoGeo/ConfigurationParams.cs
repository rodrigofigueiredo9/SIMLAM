using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.WindowsService.SVCOperacoesPublicoGeo
{
	public class ConfigurationParams
	{
		private List<List<ItemFila>> _queues = new List<List<ItemFila>>();

		public ConfigurationParams(int interval, int maxDuration, string queueSettings)
		{
			timerInterval = interval;
			processMaxDuration = maxDuration;

			//queues separated by "@"
			string[] queues = queueSettings.Split('@');

			int count = queues.Length;
			for (int i = 0; i < count; i++)
			{
				List<ItemFila> itemList = new List<ItemFila>();

				//items separated by ";"
				string[] itemStringArray = queues[i].Split(';');

				int itemCount = itemStringArray.Length;
				for (int j = 0; j < itemCount; j++)
				{
					itemList.Add(new ItemFila(itemStringArray[j]));
				}

				_queues.Add(itemList);
			}
		}

		public int timerInterval { get; set; }

		public int processMaxDuration { get; set; }

		public List<List<ItemFila>> queuesInfo
		{
			get
			{
				return _queues;
			}
		}
	}
}