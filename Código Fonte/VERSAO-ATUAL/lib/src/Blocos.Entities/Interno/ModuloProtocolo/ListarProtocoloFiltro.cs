using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo
{
	public class ListarProtocoloFiltro
	{
		public int Id { get; set; }
		public ProtocoloNumero Protocolo { get; set; }
		public String Numero { get; set; } // registro
		public String Nome { get; set; }
		public string NumeroAutuacao { get; set; }
		public int Tipo { get; set; }
		public int Acao { get; set; }
		public int ProtocoloId { get; set; }
		public string TipoTexto { get; set; }
		public int AtividadeSolicitada { get; set; }
		public int SituacaoAtividade { get; set; }
		public string EmpreendimentoCnpj { get; set; }
		public long? EmpreendimentoCodigo { get; set; }
		public string EmpreendimentoRazaoDenominacao{ get; set; }
		public string InteressadoNomeRazao { get; set; }
		public string InteressadoCpfCnpj { get; set; }
		public bool InteressadoIsCnpj { get; set; }
		public int Municipio { get; set; }
		public int AutorId { get; set; }
		public int CredenciadoPessoaId { get; set; }
		public string Assunto { get; set; }

		private DateTecno _dataRegistro = new DateTecno();
		public DateTecno DataRegistro
		{
			get { return _dataRegistro; }
			set { _dataRegistro = value; }
		}
		
		private DateTecno _dataAutuacao = new DateTecno();
		public DateTecno DataAutuacao
		{
			get { return _dataAutuacao; }
			set { _dataAutuacao = value; }
		}

		public ListarProtocoloFiltro()
		{
			Protocolo = new ProtocoloNumero();
		}
	}
}