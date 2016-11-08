using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class InfracaoRelatorio
	{
		public Int32 Id { get; set; }
		public Int32 HistoricoId { get; set; }
		public int? IsGeradoSistema { get; set; }
		public String NumeroAI { get; set; }
		public String DataLavraturaAI { get; set; }
		public String DescricaoInfracao { get; set; }
		public String Classificacao { get; set; }
		public String Tipo { get; set; }
		public String Item { get; set; }
		public String Subitem { get; set; }
		public String SerieTexto { get; set; }

		private List<InfracaoCampoRelatorio> _campos = new List<InfracaoCampoRelatorio>();
		public List<InfracaoCampoRelatorio> Campos
		{
			get { return _campos; }
			set { _campos = value; }
		}

		private List<InfracaoPerguntaRelatorio> _perguntas = new List<InfracaoPerguntaRelatorio>();
		public List<InfracaoPerguntaRelatorio> Perguntas
		{
			get { return _perguntas; }
			set { _perguntas = value; }
		}
	}
}
