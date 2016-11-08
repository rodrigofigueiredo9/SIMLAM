using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSuinocultura
{
	public class Suinocultura
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32 Atividade { get; set; }
		public Int32 Fase { get; set; }
		public String NumeroCabecas { get; set; }
		public String NumeroMatrizes { get; set; }
		public Int32? ExisteBiodigestor { get; set; }
		public Int32? PossuiFabricaRacao { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		private CoordenadaAtividade _coordenadaAtividade = new CoordenadaAtividade();
		public CoordenadaAtividade CoordenadaAtividade
		{
			get { return _coordenadaAtividade; }
			set { _coordenadaAtividade = value; }
		}
	}
}
