using System;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class ArquivarItem
	{
		public Boolean IsSelecionado { get; set; }
		public DateTecno DataEnvio { get; set; }
		public DateTecno DataRecebido { get; set; }
		public Objetivo Objetivo { get; set; }
		public Setor SetorOrigem { get; set; }
		public Funcionario Remetente { get; set; }
		public Situacao Situacao { get; set; }
		public Protocolo Protocolo { get; set; }

		public ArquivarItem()
		{
			this.DataEnvio = new DateTecno();
			this.DataRecebido = new DateTecno();
			this.Objetivo = new Objetivo();
			this.SetorOrigem = new Setor();
			this.Situacao = new Situacao();
			this.Protocolo = new Protocolo();
		}
	}
}