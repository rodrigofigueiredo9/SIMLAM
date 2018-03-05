using System;

namespace Tecnomapas.EtramiteX.Scheduler.misc
{
	public class ItemControleCar
	{
		public int id { get; set; }
		public string tid { get; set; }
		public int empreendimento { get; set; }
		public string empreendimento_tid { get; set; }
		public int solicitacao_car { get; set; }
		public string solicitacao_car_tid { get; set; }
        public int solicitacao_car_anterior { get; set; }
        public string solicitacao_car_anterior_tid { get; set; }
        public int solicitacao_car_anterior_esquema { get; set; }
		public int situacao_envio { get; set; }
		public string chave_protocolo { get; set; }
		public DateTime data_gerado { get; set; }
		public DateTime data_envio { get; set; }
		public string pendencias { get; set; }
		public string codigo_imovel { get; set; }
		public string url_recibo { get; set; }
		public string status_sicar { get; set; }
		public string condicao { get; set; }
		public string arquivo { get; set; }
		public int solicitacao_car_esquema { get; set; }

		public int solicitacao_passivo { get; set; }

		public int solicitacao_situacao_aprovado { get; set; }
	}
}
