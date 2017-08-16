using System;
using System.Collections.Generic;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes
{
	public class ConfigFiscalizacao
	{
		public Int32 Id { get; set; }
		public Int32 ClassificacaoId { get; set; }
		public String ClassificacaoTexto { get; set; }
		public Int32 TipoId { get; set; }
		public String TipoTexto { get; set; }
		public Int32 ItemId { get; set; }
		public String ItemTexto { get; set; }
		public String Tid { get; set; }

		private List<ConfigFiscalizacaoCampo> _campos = new List<ConfigFiscalizacaoCampo>();
		public List<ConfigFiscalizacaoCampo> Campos
		{
			get { return _campos; }
			set { _campos = value; }
		}

       
		private List<ConfigFiscalizacaoPergunta> _perguntas = new List<ConfigFiscalizacaoPergunta>();
		public List<ConfigFiscalizacaoPergunta> Perguntas
		{
			get { return _perguntas; }
			set { _perguntas = value; }
		}

		private List<ConfigFiscalizacaoSubItem> _subitens = new List<ConfigFiscalizacaoSubItem>();
		public List<ConfigFiscalizacaoSubItem> Subitens
		{
			get { return _subitens; }
			set { _subitens = value; }
		}

		public ConfigFiscalizacao()
		{
			this.ClassificacaoTexto =
			this.TipoTexto =
			this.ItemTexto = 
			this.Tid = String.Empty;
		}
	}
}
