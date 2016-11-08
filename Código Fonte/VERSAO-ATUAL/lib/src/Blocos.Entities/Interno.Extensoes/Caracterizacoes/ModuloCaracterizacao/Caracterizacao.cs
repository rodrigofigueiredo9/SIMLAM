

using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao
{
	public class Caracterizacao
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public string Nome { get; set; }

		#region Projeto Geográfico

		public int ProjetoId { get; set; }
		public int ProjetoRascunhoId { get; set; }
		public string ProjetoTid { get; set; }
		public int ProjetoSituacao { get; set; }

		#endregion

		#region Descrição de Licenciamento de Atividade

		public int DscLicAtividadeId { get; set; }
		public string DscLicAtividadeTid { get; set; }

		#endregion

		public eCaracterizacao Tipo { get; set; }
		public eCaracterizacaoDependenciaTipo DependenteTipo { get; set; }
		public List<Dependencia> Dependencias { get; set; }

		public Caracterizacao()
		{
			Dependencias = new List<Dependencia>();
		}
	}
}