using System;
using Tecnomapas.Blocos.Entities.Configuracao;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class Prateleira : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public String Tid { get; set; }
		public bool IsAtivo { get; set; }
		public int ModoId { get; set; }
		public String ModoTexto { get; set; }
		public int Arquivo { get; set; }
		public int Estante { get; set; }
	}
}