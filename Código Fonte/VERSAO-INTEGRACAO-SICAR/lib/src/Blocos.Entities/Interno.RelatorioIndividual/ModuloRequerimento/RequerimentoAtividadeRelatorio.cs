using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento
{
	public class RequerimentoAtividadeRelatorio
	{
		public int Id { get; set; }
		public int HistoricoId { get; set; }
		public int IdRelacionamento { get; set; }
		public String Tid { get; set; }
		public string NomeAtividade { get; set; }
		public Int32 Finalidade { get; set; }
		public string Conclusao { get; set; }

		private List<Finalidade> _finalidades = new List<Finalidade>();
		public List<Finalidade> Finalidades
		{
			get { return _finalidades; }
			set { _finalidades = value; }
		}

		private List<TipoDocumento> _tiposDocumento = new List<TipoDocumento>();
		public List<TipoDocumento> TiposDocumento
		{
			get { return _tiposDocumento; }
			set { _tiposDocumento = value; }
		}
	}
}