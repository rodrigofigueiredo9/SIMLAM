using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorteInformacao
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }

		public Int32 Empreendimento { get; set; }
		private DateTecno _dataInformacao = new DateTecno();
		public DateTecno DataInformacao { get { return _dataInformacao; } set { _dataInformacao = value; } }
		
		//public String AreaCorte
		//{
		//	get
		//	{
		//		Decimal retorno = 0;
		//		if (Especies != null && Especies.Count > 0)
		//		{
		//			foreach (Especie item in Especies)
		//			{
		//				Decimal aux = 0;
		//				if (Decimal.TryParse(item.AreaCorte, out aux))
		//				{
		//					retorno += aux;
		//				}
		//			}
		//		}

		//		return retorno.ToString("N4");
		//	}
		//}

		private List<InformacaoCorteTipo> _informacaoCorteTipo = new List<InformacaoCorteTipo>();
		public List<InformacaoCorteTipo> InformacaoCorteTipo
		{
			get { return _informacaoCorteTipo; }
			set { _informacaoCorteTipo = value; }
		}

		private List<InformacaoCorteDestinacao> _informacaoCorteDestinacao = new List<InformacaoCorteDestinacao>();
		public List<InformacaoCorteDestinacao> InformacaoCorteDestinacao
		{
			get { return _informacaoCorteDestinacao; }
			set { _informacaoCorteDestinacao = value; }
		}
	}
}
