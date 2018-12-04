namespace Tecnomapas.EtramiteX.Scheduler.misc
{
	public class RequisicaoJobCar
	{
		public const string INSTITUCIONAL = "institucional";
		public const string CREDENCIADO = "credenciado";
		
		public int empreendimento { get; set; }
		public string empreendimento_tid { get; set; }
		public int solicitacao_car { get; set; }
		public string solicitacao_car_tid { get; set; }
		public string origem { get; set; }

		public bool tem_titulo { get; set; }


		public int caracterizacao_id { get; set; }

		public string caracterizacao_tid { get; set; }

		public int projeto_geografico_id { get; set; }

		public string projeto_geografico_tid { get; set; }

		public int carac_origem { get; set; }
	}
}
