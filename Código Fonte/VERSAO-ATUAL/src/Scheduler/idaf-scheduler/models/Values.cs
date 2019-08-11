using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public static class Values
	{
		public struct UsuarioScheduler
		{
			public static int Id => 1;
			public static string Tid => "3148f886-5794-43fb-a99e-9685365dd0df";
			public static string Nome => "Tecnomapas.EtramiteX.Scheduler.exe";
			public static string Login => "IDAF_Scheduler";
		}

		public enum Origem
		{
			Institucional = 1,
			Credenciado = 2
		}
	}
}
