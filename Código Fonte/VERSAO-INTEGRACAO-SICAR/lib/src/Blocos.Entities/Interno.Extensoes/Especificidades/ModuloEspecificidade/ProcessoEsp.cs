using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade
{
	public class ProcessoEsp
	{
		public int? Id { get; set; }
		public String Tid { get; set; }
		public String Numero { get; set; }
		public int Volume { get; set; }
		public int SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public String DataCadastroTexto { get; set; }
		public DateTime DataCadastro { get; set; }
		public Boolean IsArquivado { get; set; }
		public int SetorId { get; set; }

		private List<ProcessoEsp> _processos = new List<ProcessoEsp>();
		public List<ProcessoEsp> Processos
		{
			get { return _processos; }
			set { _processos = value; }
		}
	
		public ProcessoEsp() { }
	}
}