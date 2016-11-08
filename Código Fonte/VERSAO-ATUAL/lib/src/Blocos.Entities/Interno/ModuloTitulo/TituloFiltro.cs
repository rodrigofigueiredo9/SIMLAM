using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class TituloFiltro
	{
		public ProtocoloNumero Protocolo{ get; set; }
		public bool IsDeclaratorio { get; set; }
		public bool IsProcessoNumero { get; set; }
		public string Numero { get; set; }
		public int Modelo { get; set; }
		public string ModeloTexto { get; set; }
		public List<Int32> ModeloFiltrar { get; set; }
		public List<Int32> SituacoesFiltrar { get; set; }
		public int Situacao { get; set; }
		public int Setor { get; set; }
		public string Empreendimento { get; set; }
		public long? EmpreendimentoCodigo { get; set; }
		public int Atividade { get; set; }
		public int CredenciadoId { get; set; }
		public int CredenciadoPessoaId { get; set; }
		public string DataEmisssao { get; set; }
		public int? RequerimentoID { get; set; }
		public string InteressadoNomeRazao { get; set; }
		public string InteressadoCPFCNPJ { get; set; }
		public int OrigemID { get; set; }

		public TituloFiltro()
		{
			Protocolo = new ProtocoloNumero();
			ModeloFiltrar = new List<Int32>();
			SituacoesFiltrar = new List<Int32>();
		}
	}
}