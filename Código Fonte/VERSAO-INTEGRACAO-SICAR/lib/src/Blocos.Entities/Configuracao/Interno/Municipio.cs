

using System;

namespace Tecnomapas.Blocos.Entities.Configuracao.Interno
{
	public class Municipio : IListaValor
	{
		public int Id { get; set; }
		public String Texto { get; set; }
		public String Cep { get; set; }
		public int Ibge { get; set; }
		public bool IsAtivo { get; set; }

		private Estado _estado = new Estado();
		public Estado Estado
		{
			get { return _estado; }
			set { _estado = value; }
		}
	}
}