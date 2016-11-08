using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAquicultura
{
	public class AquiculturaAquicult
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Atividade { get; set; }

		public String AreaInundadaTotal { get; set; }
		public String AreaCultivo { get; set; }
		public String NumViveiros { get; set; }
		public String NumUnidadeCultivos { get; set; }

		public String Identificador { get; set; }

		private List<Cultivo> _cultivos = new List<Cultivo>();
		public List<Cultivo> Cultivos
		{
			get { return _cultivos; }
			set { _cultivos = value; }
		}

		private CoordenadaAtividade _coordenadaAtividade = new CoordenadaAtividade();
		public CoordenadaAtividade CoordenadaAtividade
		{
			get { return _coordenadaAtividade; }
			set { _coordenadaAtividade = value; }
		}
	}
}
