using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo
{
	public class DocumentoAtividade
	{
		public int Id { get; set; }
		public int IdRelacionamento { get; set; }
		public string NomeAtividade { get; set; }
		public Int32 Finalidade { get; set; }

		public int SituacaoId { get; set; }
		public string SituacaoTexto { get; set; }

		public String Motivo { get; set; }

		private List<Finalidade> _finalidades = new List<Finalidade>();
		public List<Finalidade> Finalidades
		{
			get { return _finalidades; }
			set { _finalidades = value; }
		}

		private List<TipoDocumento> _tiposDocumento = new List<TipoDocumento>();
		public List<TipoDocumento> TiposDocumento
		{
			get { return _tiposDocumento; }
			set { _tiposDocumento = value; }
		}

		public string Tid { get; set; }

		public DocumentoAtividade() { }

		public DocumentoAtividade(List<Finalidade> _finalidades, List<TipoDocumento> _tiposDocumento)
		{
			Finalidades = _finalidades;
			TiposDocumento = _tiposDocumento;
		}
	}
}