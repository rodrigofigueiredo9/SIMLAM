using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tecnomapas.EtramiteX.Scheduler.models.misc
{
	public class ConsultaSicarResposta
	{
		public string status { get; set; }
		public string mensagem { get; set; }
		public ConsultaSicarDadosResposta dados { get; set; }
		public string erro { get; set; }
		public string saidaLista { get; set; }
	}

	public class ConsultaSicarDadosResposta
	{
		public int id { get; set; }
		public string codigoImovel { get; set; }
		public string protocolo { get; set; }
		public string nomeCadastrante { get; set; }
		public string nomeImovel { get; set; }
		public ConsultaSicarMunicipioResposta municipio { get; set; }
		public decimal areaImovel { get; set; }
		public decimal modulosFiscais { get; set; }
		public string dataCriacao { get; set; }
		public bool isAtivo { get; set; }
		public string statusImovel { get; set; }
		public string condicaoAnalise { get; set; }
		public string idImovelPosterior { get; set; }
		public string passivelAdesaoPra { get; set; }
		public string possuiRestricoes { get; set; }
		public string dataConsulta { get; set; }
		public string horaConsulta { get; set; }
		public string bbox { get; set; }
		public bool substituiImovelPorDuplicidade { get; set; }
	}

	public class ConsultaSicarMunicipioResposta
	{
		public int id { get; set; }
		public string nome { get; set; }
		public ConsultaSicarEstadoResposta estado { get; set; }
		public decimal moduloFiscal { get; set; }
		public decimal area { get; set; }
	}

	public class ConsultaSicarEstadoResposta
	{
		public string id { get; set; }
		public string nome { get; set; }
		public int codigoIbge { get; set; }
		public decimal area { get; set; }
	}
}
