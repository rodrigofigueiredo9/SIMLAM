using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class ConsideracoesFinaisRelatorio
	{
		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }
		public String JustificativaValorPenalidade { get; set; }
		public String DescricaoInfracao { get; set; }
		public String IsReparacao { get; set; }
		public String OpniaoFormaReparacao { get; set; }
		public String ReparacaoJustificativa { get; set; }
		public String IsTermoCompromisso { get; set; }
		public String TermoCompromissoJustificativa { get; set; }

		private List<AssinanteDefault> _assinantes = new List<AssinanteDefault>();
		public List<AssinanteDefault> Assinantes
		{
			get { return _assinantes; }
			set { _assinantes = value; }
		}

		private List<ConsideracoesFinaisAnexoRelatorio> _anexos = new List<ConsideracoesFinaisAnexoRelatorio>();
		public List<ConsideracoesFinaisAnexoRelatorio> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}
	}
}
