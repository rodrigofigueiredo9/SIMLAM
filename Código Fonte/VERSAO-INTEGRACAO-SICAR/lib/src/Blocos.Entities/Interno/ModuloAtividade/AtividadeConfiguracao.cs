using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAtividade
{
	public class AtividadeConfiguracao
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public string NomeGrupo { get; set; }	
		private List<AtividadeSolicitada> _atividades = new List<AtividadeSolicitada>();
		public List<AtividadeSolicitada>  Atividades
		{
			get { return _atividades; }
			set { _atividades = value; }
		}

		private List<Modelo> _modelos = new List<Modelo>();
		public List<Modelo> Modelos
		{
			get { return _modelos; }
			set { _modelos = value; }
		}
	}
}