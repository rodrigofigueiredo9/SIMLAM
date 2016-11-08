using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CondicionantePDF
	{
		public String Descricao { get; set; }
		public String Prazo { get; set; }
		public String PrazoTipo { get; set; }
		public int Periodo { get; set; }
		public String PeriodoTipo { get; set; }
		public Boolean PossuiPeriodicidade { get; set; }

		public String PrazoPeriodicidadeTexto 
		{
			get 
			{
				if (!String.IsNullOrWhiteSpace(Prazo))
				{
					if (PossuiPeriodicidade)
					{
						String periodoTipoTexto = PeriodoTipo;

						if (Periodo > 1) 
						{
							switch (PeriodoTipo)
							{
								case "Dia":
									periodoTipoTexto = "Dias";
									break;
								case "Mês":
									periodoTipoTexto = "Meses";
									break;
								case "Ano":
									periodoTipoTexto = "Anos";
									break;
								default:
									periodoTipoTexto = PeriodoTipo;
									break;
							}
						}

						return String.Format("{0} {1} com periodicidade de {2} {3}", Prazo, PrazoTipo, Periodo, periodoTipoTexto);
					}

					return String.Format("{0} {1}", Prazo, PrazoTipo);
				}

				return "«remover»";
			}
		}
	}
}