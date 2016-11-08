using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria
{
	public class RegularizacaoFundiaria
	{
		public Int32 Id { set; get; }
		public String Tid { set; get; }
		public Int32 EmpreendimentoId { set; get; }

		public List<Posse> Posses { set; get; }
		public List<Dominio> Matriculas { set; get; }
		public Posse Posse { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		public RegularizacaoFundiaria()
		{
			Dependencias = new List<Dependencia>();
			Posses = new List<Posse>();
			Matriculas = new List<Dominio>();
			Posse = new Posse();
		}
	}
}