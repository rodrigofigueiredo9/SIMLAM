

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes
{
	public class DependenciaLst : IListaValor
	{
		public Int32 Id { get; set; }
		public Int32 DependenteTipo { get; set; }
		public Int32 DependenciaTipo { get; set; }
		public Int32 TipoId { get; set; }
		public String TipoTexto { get; set; }
		public String Texto { get; set; }
		public String DependenciaTid { get; set; }
		public Boolean IsAtivo { get; set; }
		public Int32 TipoDetentorId { get; set; }
		public Boolean ExibirCredenciado { get; set; }
	}
}