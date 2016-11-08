using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class ConsideracaoFinal
	{
		public int Id { get; set; }
		public int FiscalizacaoId { get; set; }
		public string Justificar { get; set; }
		public string Descrever { get; set; }
		public bool? HaReparacao { get; set; }
		public string Reparacao { get; set; }
		public bool? HaTermoCompromisso { get; set; }
		public string TermoCompromissoJustificar { get; set; }
		
		public List<ConsideracaoFinalTestemunha> Testemunhas { get; set; }
		public List<FiscalizacaoAssinante> Assinantes { get; set; }
		public List<Anexo> Anexos { get; set; }
		public string Tid { get; set; }
		public Arquivo.Arquivo Arquivo { get; set; }

		public ConsideracaoFinal()
		{
			Justificar =
			Descrever =
			Reparacao =
			TermoCompromissoJustificar =
			Tid = string.Empty;
			Assinantes = new List<FiscalizacaoAssinante>();
			Testemunhas = new List<ConsideracaoFinalTestemunha>();
			Anexos = new List<Anexo>();
			Arquivo = new Arquivo.Arquivo();
		}
	}
}
