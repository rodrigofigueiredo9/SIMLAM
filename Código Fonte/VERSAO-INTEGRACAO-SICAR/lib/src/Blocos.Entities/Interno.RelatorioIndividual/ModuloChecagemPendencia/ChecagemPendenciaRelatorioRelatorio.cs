using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloChecagemPendencia
{
	public class ChecagemPendenciaRelatorioRelatorio
	{
		public int Id { get; set; }
		public String Tid { get; set; }
		public int Numero { get { return Id; } }
		public int TituloId { get; set; }
		public string TituloNumero { get; set; }
		public String TituloTipoSigla { get; set; }
		public String TituloTipoTexto { get; set; }
		public int SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public string InteressadoNome { get; set; }
		public string ProtocoloNumero { get; set; }

		private DateTecno _tituloVencimento = new DateTecno();
		public DateTecno TituloVencimento { get { return _tituloVencimento; } set { _tituloVencimento = value; } }

		private List<ChecagemPendenciaItemRelatorio> _itens = new List<ChecagemPendenciaItemRelatorio>();
		public List<ChecagemPendenciaItemRelatorio> Itens { get { return _itens; } set { _itens = value; } }

		public ChecagemPendenciaRelatorioRelatorio() { }
	}
}