using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloAvicultura
{
	public class Avicultura
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32 Atividade { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		private List<ConfinamentoAves> _confinamentos = new List<ConfinamentoAves>();
		public List<ConfinamentoAves> Confinamentos
		{
			get { return _confinamentos; }
			set { _confinamentos = value; }
		}

		private CoordenadaAtividade _coordenadaAtividade = new CoordenadaAtividade();
		public CoordenadaAtividade CoordenadaAtividade
		{
			get { return _coordenadaAtividade; }
			set { _coordenadaAtividade = value; }
		}
	}
}
