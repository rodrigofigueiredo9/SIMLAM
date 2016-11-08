

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class InfracaoCampo
	{
		public Int32 Id { get; set; }
		public Int32 ConfiguracaoId { get; set; }
		public Int32 CampoId { get; set; }
		public String Identificacao { get; set; }
		public String Texto { get; set; }
		public String UnidadeTexto { get; set; }
		public Int32 Unidade { get; set; }
		public String TipoTexto { get; set; }
		public Int32 Tipo { get; set; }

		public String Mascara
		{
			get
			{
				if (Tipo == (int)eCampoTipo.Numerico)
				{
					switch (Unidade)
					{
						case (int)eCampoUnidadeMedida.M2:
							return "maskDecimalPonto";

						case (int)eCampoUnidadeMedida.M3:
							return "maskDecimalPonto";

						case (int)eCampoUnidadeMedida.Ha:
							return "maskDecimalPonto";

						case (int)eCampoUnidadeMedida.Kg:
							return "maskDecimalPonto";

						case (int)eCampoUnidadeMedida.Qtd:
							return "maskInteger";

						case (int)eCampoUnidadeMedida.St:
							return "maskInteger";

						case (int)eCampoUnidadeMedida.MDC:
							return "maskInteger";

						default:
							return String.Empty;
					}
				}

				return String.Empty;
			}
		}

		public String Tamanho 
		{
			get 
			{
				if (Tipo == (int)eCampoTipo.Numerico) 
				{
					switch (Unidade)
					{
						case (int)eCampoUnidadeMedida.M2:
						case (int)eCampoUnidadeMedida.M3:
						case (int)eCampoUnidadeMedida.Ha:
						case (int)eCampoUnidadeMedida.Kg:
							return "12";

						case (int)eCampoUnidadeMedida.Qtd:
						case (int)eCampoUnidadeMedida.St:
						case (int)eCampoUnidadeMedida.MDC:
							return "13";
					}
				}

				return "50";
			}
		}
	}
}
