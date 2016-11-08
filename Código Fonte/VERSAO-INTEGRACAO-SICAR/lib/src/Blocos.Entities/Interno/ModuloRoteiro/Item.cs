using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro
{
	public class Item
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public string Tid { get; set; }
		public int Tipo { get; set; }
		public string TipoTexto { get; set; }
		public int Ordem { get; set; }
		public string Nome { get; set; }
		public int Situacao { get; set; }
		public Int32? Checagem { get; set; }
		public Int32? ChecagemId { get; set; }
		public Int32? AnaliseNumero { get; set; }		
		public string DataAnalise { get; set; }
		public string SituacaoTexto { get; set; }
		public string ProcedimentoAnalise { get; set; }
		public string Analista { get; set; }
		public bool IsEditavel { get; set; }
		public string StatusTela { get; set; }
		public int StatusId { get; set; }
		public bool Editado { get; set; }
		public bool Recebido { get; set; }
		public bool Avulso { get; set; }
		public Int32? SetorId { get; set; }
		public string SetorNome { get; set; }
		public string SetorSigla { get; set; }
		public string Descricao { get; set; }
		public string Motivo { get; set; }
		public int CaracterizacaoTipoId { get; set; }
		public String CaracterizacaoTipoTexto { get; set; }

		private string _condicionante = string.Empty;
		public string Condicionante
		{
			get { return _condicionante; }
			set { _condicionante = value; }
		}

		List<Int32> _roteiros = new List<Int32>();
		public List<Int32> Roteiros
		{
			get { return _roteiros; }
			set { _roteiros = value; }
		}

		public bool TemProjetoGeografico { get; set; }

		public bool IsAtualizado { get; set; }
	}
}