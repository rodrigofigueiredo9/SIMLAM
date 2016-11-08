using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class Estante : IListaValor
	{
		public int Id { get; set; }
		public int? Arquivo { get; set; }
		public String Texto { get; set; }
		public String Tid { get; set; }
		public bool IsAtivo { get; set; }

		private List<Prateleira> _prateleiras = new List<Prateleira>();
		public List<Prateleira> Prateleiras
		{
			get { return _prateleiras; }
			set { _prateleiras = value; }
		}
		
		public Estante(string nome = null, int? idRelacionamento = null)
		{
			this.Texto = nome;
			this.Arquivo = idRelacionamento;
			this.Prateleiras = new List<Prateleira>();
		}

		public Estante() { }
	}
}