using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloModelo
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public string Tid { get; set; }
		public int? Codigo { get; set; }
		public int Tipo { get; set; }
		public string TipoTexto { get; set; }
		public int? SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }
		public string SubTipo { get; set; }

		private DateTecno _dataCriacao = new DateTecno();
		public DateTecno DataCriacao 
		{ 
			get { return _dataCriacao; } 
			set { _dataCriacao = value; } 
		}
		
		public string Nome { get; set; }
		public string Sigla { get; set; }

		public int TipoProtocolo { get; set; }

		public eTipoProtocolo TipoProtocoloEnum
		{
			get { return (eTipoProtocolo)TipoProtocolo; }
		}

		private List<TituloModeloRegra> _listaRegras = new List<TituloModeloRegra>();
		public List<TituloModeloRegra> Regras { get { return _listaRegras; } set { _listaRegras = value; } }

		private List<Setor> _setores = new List<Setor>();
		public List<Setor> Setores { get { return _setores; }  set { _setores = value; } }

		private List<Assinante> _assinantes = new List<Assinante>();
		public List<Assinante> Assinantes { get { return _assinantes; } set { _assinantes = value; } }

		private List<TituloModelo> _modelos = new List<TituloModelo>();
		public List<TituloModelo> Modelos { get { return _modelos; } set { _modelos = value; } }

		private Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
		public Arquivo.Arquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

        public int TipoDocumento { get; set; }

        public eTituloModeloTipoDocumento TipoDocumentoEnum
        {
            get { return (eTituloModeloTipoDocumento)this.TipoDocumento;  }
        }
        public string TipoDocumentoTexto { get; set; }
	}
}