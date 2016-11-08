using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class QueimaControladaPDF
	{
		private List<QueimaControladaQueimaPDF> _queimasControladas = new List<QueimaControladaQueimaPDF>();
		public List<QueimaControladaQueimaPDF> QueimasControladas
		{
			get { return _queimasControladas; }
			set { _queimasControladas = value; }
		}

		public Decimal TotalAreaQueima { get; set; }

		#region Valores em Hectares

		public String TotalAreaQueimaHa
		{
			get
			{
				if (TotalAreaQueima > 0)
				{
					return TotalAreaQueima.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return String.Empty;
			}
		}

		#endregion

		public QueimaControladaPDF() { }

		public QueimaControladaPDF(QueimaControlada queimaControlada)
		{
			#region Total de Area de Queima

			TotalAreaQueima = 0;

			if (queimaControlada.QueimasControladas != null && queimaControlada.QueimasControladas.Count > 0)
			{
				foreach (QueimaControladaQueima queima in queimaControlada.QueimasControladas)
				{
					if (queima.Cultivos != null && queima.Cultivos.Count > 0)
					{
						Decimal aux = 0;
						foreach (Cultivo cultivo in queima.Cultivos)
						{
							if (Decimal.TryParse(cultivo.AreaQueima, out aux))
							{
								TotalAreaQueima += aux;
							}
						}
					}
				}

			}

			#endregion

			#region Queimas Controladas


			List<Cultivo> cultivosSelecionados = new List<Cultivo>();
			List<Cultivo> lstTodosCultivos = queimaControlada.QueimasControladas.SelectMany(x => x.Cultivos).ToList();

			lstTodosCultivos.ForEach(cultivo =>
			{
				if (!cultivosSelecionados.Exists(y => y.CultivoTipoTexto.ToLower() == cultivo.CultivoTipoTexto.ToLower()))
				{
					cultivo.AreaQueima = lstTodosCultivos
						.Where(x => x.CultivoTipoTexto.ToLower() == cultivo.CultivoTipoTexto.ToLower())
						.Sum(x => Convert.ToDecimal(x.AreaQueima)).ToString();
					cultivosSelecionados.Add(cultivo);
				}
			});

			QueimasControladas = new List<QueimaControladaQueimaPDF>() { 
				new QueimaControladaQueimaPDF(){ Cultivos = cultivosSelecionados}
			};

			#endregion
		}
	}
}