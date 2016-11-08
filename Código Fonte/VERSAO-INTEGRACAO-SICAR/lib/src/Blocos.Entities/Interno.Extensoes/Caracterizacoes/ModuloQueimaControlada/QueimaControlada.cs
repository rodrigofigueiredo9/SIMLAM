using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloQueimaControlada
{
	public class QueimaControlada
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 EmpreendimentoId { get; set; }

		public List<Dependencia> Dependencias { get; set; }

		private List<QueimaControladaQueima> _queimasControladas = new List<QueimaControladaQueima>();
		public List<QueimaControladaQueima> QueimasControladas
		{
			get { return _queimasControladas; }
			set { _queimasControladas = value; }
		}
	}
}
