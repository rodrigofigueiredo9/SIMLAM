using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class CaracterizacaoPDF
	{
		public bool ExibirCampo { get; set; }
		public bool ExibirCampos1 { get; set; }
		public bool ExibirCampos2 { get; set; }

		public List<CaracterizacaoCampoPDF> Campos { get; set; }
		
		public void Organizar()
		{
			if (Campos != null && Campos.Count > 0)
			{
				ExibirCampo = (Campos.Count % 2) != 0;
				ExibirCampos1 = Campos.Count > 1;
				ExibirCampos2 = Campos.Count > 1;

				Campos1 = new List<CaracterizacaoCampoPDF>();
				Campos2 = new List<CaracterizacaoCampoPDF>();

				for (int i = 0; i < Campos.Count; i++)
				{
					if ((i == (Campos.Count - 1)) && (i % 2 == 0))
					{
						Campo = Campos[i];
						break;
					}

					if (i % 2 == 0)
					{
						Campos1.Add(Campos[i]);
					}
					else
					{
						Campos2.Add(Campos[i]);
					}
				}
			}
		}

		public CaracterizacaoPDF()
		{
			Campos = new List<CaracterizacaoCampoPDF>();
			Cultura = new CulturaFlorestalPDF();
		}

		public CaracterizacaoCampoPDF Campo { get; private set; }
		public List<CaracterizacaoCampoPDF> Campos1 { get; private set; }
		public List<CaracterizacaoCampoPDF> Campos2 { get; private set; }

		public CulturaFlorestalPDF Cultura { get; set; }

		public string NorthingLatitude { get; set; }
		public string EastingLongitude { get; set; }
	}
}