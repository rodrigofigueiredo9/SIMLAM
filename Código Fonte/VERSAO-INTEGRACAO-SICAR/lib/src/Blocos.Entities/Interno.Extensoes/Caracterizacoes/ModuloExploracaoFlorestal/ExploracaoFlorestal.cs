using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal
{
	public class ExploracaoFlorestal
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32? FinalidadeExploracao { get; set; }
		public String FinalidadeEspecificar { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		public List<ExploracaoFlorestalExploracao> Exploracoes { get; set; }
		
		public ExploracaoFlorestal()
		{
			Dependencias = new List<Dependencia>();
			Exploracoes = new List<ExploracaoFlorestalExploracao>();
		}
	}
}