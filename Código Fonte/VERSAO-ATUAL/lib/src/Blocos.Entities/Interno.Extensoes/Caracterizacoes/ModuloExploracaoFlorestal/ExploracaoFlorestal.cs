using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloExploracaoFlorestal
{
	public class ExploracaoFlorestal
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }
		public Int32 CodigoExploracao { get; set; }
		public String CodigoExploracaoTexto { get; set; }
		public Int32 TipoExploracao { get; set; }
		public String TipoExploracaoTexto { get; set; }
		public String Localizador { get; set; }
		public DateTecno DataCadastro { get; set; } = new DateTecno() { Data = DateTime.Now };
		public DateTecno DataConclusao { get; set; } = new DateTecno() { Data = DateTime.Now };
		public String GeometriaPredominanteTexto { get; set; }

		public List<ExploracaoFlorestalExploracao> Exploracoes { get; set; }
		public List<Dependencia> Dependencias { get; set; }

		public ExploracaoFlorestal()
		{
			Exploracoes = new List<ExploracaoFlorestalExploracao>();
			Dependencias = new List<Dependencia>();
		}
	}
}