using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class Arquivar
	{
		public int ObjetivoId { get; set; }

		public String SetorNome { get; set; }
		public int SetorId { get; set; }

		public String ArquivoNome { get; set; }
		public int ArquivoId { get; set; }

		public String EstanteNome { get; set; }
		public int EstanteId { get; set; }

		public String PrateleiraNome { get; set; }
		public int PrateleiraId { get; set; }

		public int PrateleiraModoId { get; set; }
		public String Despacho { get; set; }

		private DateTecno _dataArquivamento = new DateTecno();
		public DateTecno DataArquivamento
		{
			get { return _dataArquivamento; }
			set { _dataArquivamento = value; }
		}

		private Funcionario _funcionario = new Funcionario();
		public Funcionario Funcionario
		{
			get { return _funcionario; }
			set { _funcionario = value; }
		}
	}
}