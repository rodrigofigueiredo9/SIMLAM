using System;
using System.ComponentModel;
using Newtonsoft.Json;
using Tecnomapas.EtramiteX.Scheduler.models.misc;

namespace Tecnomapas.EtramiteX.Scheduler.models
{
	public class Cadastrante
	{
		[DefaultValue("")]
		public string cpf { set; get; }

		[DefaultValue("")]
		public string nome { get; set; }

		[DefaultValue("")]
		public string nomeMae { get; set; }

		[JsonConverter(typeof(DateTimeDMY)), DefaultValue("")]
		public DateTime dataNascimento { get; set; }

		public Cadastrante()
		{
			dataNascimento = new DateTime(1900, 01, 01);
			nomeMae = "Não informado";
		}
	}
}