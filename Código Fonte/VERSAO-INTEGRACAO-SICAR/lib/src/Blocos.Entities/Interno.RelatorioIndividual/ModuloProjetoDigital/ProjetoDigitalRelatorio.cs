using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloProjetoDigital
{
	public class ProjetoDigitalRelatorio
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public int Numero { get { return RequerimentoId; } }
		public int RequerimentoId { get; set; }
		public String RequerimentoTid { get; set; }
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public DateTime DataCadastro { get; set; }
		public string DataCadastroResumida { get { return DataCadastro.ToShortDateString(); } }
		public List<Dependencia> Dependencias { get; set; }

		public ProjetoDigitalRelatorio()
		{
			DataCadastro = new DateTime();
			Dependencias = new List<Dependencia>();
		}
	}
}