

using System;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao
{
	public class Dependencia
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 DependenciaTipo { get; set; }
		public Int32 DependenciaCaracterizacao { get; set; }
		public Int32 DependenciaId { get; set; }
		public String DependenciaTid { get; set; }
		public String DependenciaCaracterizacaoTexto { get; set; }
	}
}