using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class PerguntaInfracao
	{
		public Int32 Id { get; set; }
		public Int32 SituacaoId { get; set; }
		public String Texto { get; set; }
		public String Tid { get; set; }

		private List<Resposta> _respostas = new List<Resposta>();
		public List<Resposta> Respostas
		{
			get { return _respostas; }
			set { _respostas = value; }
		}

		private List<Item> _itens = new List<Item>();
		public List<Item> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}
	}
}