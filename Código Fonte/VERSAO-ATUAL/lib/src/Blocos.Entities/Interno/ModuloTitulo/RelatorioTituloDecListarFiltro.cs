using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class RelatorioTituloDecListarFiltro
	{
		public String NumeroTitulo { get; set; }
		public String Login { get; set; }
		public String NomeUsuario { get; set; }

		public String NomeInteressado { get; set; }
		public String InteressadoCpfCnpj { get; set; }

		private DateTecno _dataSituacaoAtual = new DateTecno();
		public DateTecno DataSituacaoAtual
		{
			get { return _dataSituacaoAtual; }
			set { _dataSituacaoAtual = value; }
		}

		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }

		public String IP { get; set; }

		public Int32 SituacaoTipo { get; set; }
		public String SituacaoTipoTexto { get; set; }

	}
}
