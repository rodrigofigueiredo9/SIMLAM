using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloTramitacao
{
	public class TramitacaoHistoricoRelatorioPDF
	{
		#region Cabeçalho

		public String GovernoNome { get; set; }
		public String SecretariaNome { get; set; }
		public String OrgaoNome { get; set; }
		public String SetorNome { get; set; }
		public Byte[] LogoBrasao { get; set; }

		#endregion

		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }

		Protocolo _processoDocumento = new Protocolo();
		public Protocolo Protocolo
		{
			get { return _processoDocumento; }
			set { _processoDocumento = value; }
		}

		private List<TramitacaoHistoricoRelatorio> _tramitacoes = new List<TramitacaoHistoricoRelatorio>();
		public List<TramitacaoHistoricoRelatorio> Tramitacoes
		{
			get { return _tramitacoes; }
			set { _tramitacoes = value; }
		}

		public TramitacaoHistoricoRelatorioPDF(){}
	}
}
