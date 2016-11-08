using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo
{
	public class NotificacaoPendencia
	{
		public Int32 Id { get; set; }
		public String Nome { get; set; }
		public Int32 Ano { get; set; }
		public Int32 Numero { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }

		public String NumeroTexto
		{
			get
			{
				return Numero + "/" + Ano;
			}
		}

		private Protocolo _protocolo = new Protocolo();
		public Protocolo Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		private DateTecno _dataEmissao = new DateTecno();
		public DateTecno DataEmissao
		{
			get { return _dataEmissao; }
			set { _dataEmissao = value; }
		}
	}
}
