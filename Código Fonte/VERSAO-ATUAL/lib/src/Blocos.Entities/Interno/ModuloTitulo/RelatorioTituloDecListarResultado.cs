using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class RelatorioTituloDecListarResultado
	{
		public int ID { get; set; }
		public string NumeroTitulo { get; set; }
		public string Login { get; set; }
		public string NomeUsuario { get; set; }
		public string NomeInteressado { get; set; }
		public string CPFCNPJInteressado { get; set; }
		public DateTime DataSituacao { get; set; }
		public string DataSituacaoTexto
		{
			get
			{
				return DataSituacao.ToShortDateString();
			}
		}
		public int Situacao { get; set; }
		public string SituacaoTexto { get; set; }
		public string IP { get; set; }
	}
}