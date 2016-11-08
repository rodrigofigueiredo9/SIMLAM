using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloTitulo
{
	public class Entrega
	{
		public int Id { get; set; }

		public string Tid { get; set; }
		public int? PessoaId { get; set; }
		public string EmpreendimentoDenominador { get; set; }
		public string Nome { get; set; }
		public string CPF { get; set; }
		public List<int> Titulos { get; set; }

		private List<Titulo> _titulosEntrega = new List<Titulo>();
		public List<Titulo> TitulosEntrega
		{
			get { return _titulosEntrega; }
			set { _titulosEntrega = value; }
		}
		
		private DateTecno _dataEntrega = new DateTecno();
		public DateTecno DataEntrega
		{
			get { return _dataEntrega; }
			set { _dataEntrega = value; }
		}

		private Protocolo _protocolo = new Protocolo();
		public Protocolo Protocolo
		{
			get { return _protocolo; }
			set { _protocolo = value; }
		}

		public Entrega()
		{
			DataEntrega.Data = DateTime.Now;

		}
	}
}
