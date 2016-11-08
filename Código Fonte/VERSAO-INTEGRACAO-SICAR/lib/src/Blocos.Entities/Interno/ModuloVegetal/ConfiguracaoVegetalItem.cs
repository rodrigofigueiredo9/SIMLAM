using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloVegetal
{
	public class ConfiguracaoVegetalItem
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public string Tid { get; set; }

		private string _texto;
		public string Texto
		{
			get { return _texto; }
			set { _texto = (value ?? "").Trim(); }
		}
		public int SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }
		public string Motivo { get; set; }

		public decimal Concentracao { get; set; }
		public int UnidadeMedidaId { get; set; }
		public string UnidadeMedidaTexto { get; set; }
		public string UnidadeMedidaOutro { get; set; }

		public ConfiguracaoVegetalItem()
		{
			_texto = String.Empty;
		}
	}
}