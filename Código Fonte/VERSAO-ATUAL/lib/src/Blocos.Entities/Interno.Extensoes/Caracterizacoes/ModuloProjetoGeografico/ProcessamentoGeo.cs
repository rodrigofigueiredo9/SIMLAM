using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico
{
	public class ProcessamentoGeo
	{
		public Int32 Id { get; set; }
		public Int32 ProjetoId { get; set; }

		public Int32 Mecanismo { get; set; }
		public Int32 Etapa { get; set; }
		public Int32 FilaTipo { get; set; }

		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }

		public Boolean isValido { get; set; }

		public ProcessamentoGeo(){}

	}
}
