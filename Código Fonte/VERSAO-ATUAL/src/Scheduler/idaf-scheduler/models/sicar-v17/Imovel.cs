using System;
using Newtonsoft.Json;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class Imovel
	{
		//Domínio do campo TIPO
		public const string TipoImovelRural = "IRU";
		public const string TipoImovelRuralPovosComunidTradicionais = "PCT";
		public const string TipoImovelRuralAssentReformaAgraria = "AST";

		//Domínio do campo ZONALOCALIZACAO
		public const string ZonaRural = "RURAL";
		public const string ZonaUrbana = "URBANA";
		
		public string idPai { get; set; }
		public string tipo { get; set; }
		public int? fracaoIdeal { get; set; }
		public string nome { get; set; }
		public int codigoMunicipio { get; set; }
		public string cep { get; set; }
		public string descricaoAcesso { get; set; }
		public string zonaLocalizacao { get; set; }
		public string email { get; set; }
		public string telefone { get; set; }
		public double modulosFiscais { get; set; }
		public EnderecoCorrespondencia enderecoCorrespondencia { get; set; }
		public string codigoProjetoAssentamento { get; set; }
		
		[JsonConverter(typeof(DateTimeDMY))]
		public DateTime? dataCriacaoAssentamento { get; set; }

		public Imovel()
		{
			tipo = TipoImovelRural;
			enderecoCorrespondencia = new EnderecoCorrespondencia();
		}
	}
}