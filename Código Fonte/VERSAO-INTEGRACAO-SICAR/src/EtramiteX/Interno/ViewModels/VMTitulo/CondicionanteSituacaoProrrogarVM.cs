

using System;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class CondicionanteSituacaoProrrogarVM
	{
		public int? Dias { get; set; }
		public int CondicionanteId { get; set; }
		public int PeriodicidadeId { get; set; }

		public CondicionanteSituacaoProrrogarVM() 
		{

		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
				});
			}
		}
	}
}