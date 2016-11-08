using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao
{
	public class TramitacaoArquivo
	{
		public int? Id { get; set; }
		public String Tid { get; set; }
		public String Nome { get; set; }
		public int? SetorId { get; set; }
		public String SetorNome { get; set; }
		public int? TipoId { get; set; }
		public String TipoTexto { get; set; }
		public int? ProtocoloSituacao { get; set; }
		
		private List<Estante> _estantes = new List<Estante>();
		public List<Estante> Estantes
		{
			get { return _estantes; }
			set { _estantes = value; }
		}

		public TramitacaoArquivo()
		{
			this.Nome = string.Empty;
			this.Estantes = new List<Estante>();
		}
	}
}