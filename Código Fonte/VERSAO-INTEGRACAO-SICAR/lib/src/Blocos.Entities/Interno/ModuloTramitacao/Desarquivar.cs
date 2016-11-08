using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class Desarquivar
	{
		public int FuncionarioId { get; set; }
		public int SetorId { get; set; }
		public int ObjetivoId { get; set; }
		public int ArquivoId { get; set; }
		public int EstanteId { get; set; }
		public int PrateleiraId { get; set; }
		public DateTecno DataArquivamento { get; set; }
		public String Despacho { get; set; }
		public List<ArquivarItem> Itens { get; set; }
		public Funcionario Funcionario { get; set; }

		public Desarquivar()
		{
			this.Funcionario = new Funcionario();
			this.DataArquivamento = new DateTecno();
			this.Despacho = string.Empty;
			this.Itens = new List<ArquivarItem>();
		}

	}
}