

using System;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro
{
	public class ChecagemRoteiroListarFiltro
	{
		public Int32? Id { set; get; }
		public String NomeRoteiro { set; get; }
		public String Interessado { set; get; }
		public Int32 SituacaoId { set; get; }
		public String Situacao { set; get; }
	}
}