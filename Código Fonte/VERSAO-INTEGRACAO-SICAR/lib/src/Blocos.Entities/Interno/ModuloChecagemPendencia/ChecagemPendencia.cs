using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia
{
	public class ChecagemPendencia
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public int Numero { get {return Id; } }
		public int TituloId { get; set; }
		public string TituloNumero { get; set; }
		public String TituloTipoSigla { get; set; }
		public int SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public string InteressadoNome { get; set; }
		public string ProtocoloNumero { get; set; }

		private DateTecno _tituloVencimento = new DateTecno();
		public DateTecno TituloVencimento { get { return _tituloVencimento; } set { _tituloVencimento = value; } }
		
		private List<ChecagemPendenciaItem> _itens = new List<ChecagemPendenciaItem>();
		public List<ChecagemPendenciaItem> Itens { get { return _itens; } set { _itens = value; } }
	}
}