using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte
{
	public class InformacaoCorte
	{
		public Int32 Id { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public String Tid { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		private InformacaoCorteInformacao _informacaoCorteInformacao = new InformacaoCorteInformacao();
		public InformacaoCorteInformacao InformacaoCorteInformacao
		{
			get { return _informacaoCorteInformacao; }
			set { _informacaoCorteInformacao = value; }
		}

		private List<InformacaoCorteInformacao> _informacoesCortes = new List<InformacaoCorteInformacao>();
		public List<InformacaoCorteInformacao> InformacoesCortes
		{
			get { return _informacoesCortes; }
			set { _informacoesCortes = value; }
		}

		public String ArvoresIsoladasTotal 
		{
			get 
			{
				Decimal retorno = 0;
				if (InformacoesCortes != null && InformacoesCortes.Count > 0)
				{
					foreach (InformacaoCorteInformacao item in InformacoesCortes)
					{
						Decimal aux = 0;
						if (Decimal.TryParse(item.ArvoresIsoladas, out aux))
						{
							retorno += aux;
						}
					}
				}
				return retorno.ToString("N0");
			} 
		}

		public String AreaCorteTotal 
		{
			get 
			{
				Decimal retorno = 0;
				if (InformacoesCortes != null && InformacoesCortes.Count > 0)
				{
					foreach (InformacaoCorteInformacao item in InformacoesCortes)
					{
						Decimal aux = 0;
						if (Decimal.TryParse(item.AreaCorte, out aux))
						{
							retorno += aux;
						}
					}
				}
				return retorno.ToString("N4");
			}
		}
	}
}
