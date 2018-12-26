using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorteLicenca
	{
		public Int32 Id { get; set; }
		public Int32 Corte { get; set; }
		public Int32 Licenca { get; set; }
		public String TipoLicenca { get; set; }
		public String Atividade { get; set; }
		public Decimal AreaLicenca { get; set; }
		public String NumeroLicenca { get; set; }
		public DateTecno DataVencimento { get; set; }
	}
}
