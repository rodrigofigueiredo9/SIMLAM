using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens
{	
	public class AnaliseItem
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public Int32 Situacao { get; set; }
		public String SituacaoTexto { get; set; }
		public Int32 ChecagemId { get; set; }
		public bool Atualizado { get; set; }
		public bool IsProcesso { get; set; }
		public String ProtocoloPai { get; set; }

		private List<Roteiro> _roteiros = new List<Roteiro>();
		public List<Roteiro> Roteiros
		{
			get { return _roteiros; }
			set { _roteiros = value; }
		}

		private Protocolo _protocolo = new Protocolo();
		public Protocolo Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		private List<Item> _itens = new List<Item>();
		public List<Item> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		private ChecagemRoteiro _checagem = new ChecagemRoteiro();
		public ChecagemRoteiro Checagem
		{
			get { return _checagem; }
			set { _checagem = value; }
		}

		private List<Requerimento> _requerimentos = new List<Requerimento>();
		public List<Requerimento> Requerimentos
		{
			get { return _requerimentos; }
			set { _requerimentos = value; }
		}

		public AnaliseItem() { }
	}
}