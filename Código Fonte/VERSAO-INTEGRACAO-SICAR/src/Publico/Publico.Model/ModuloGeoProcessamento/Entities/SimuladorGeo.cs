using System;
using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloGeoProcessamento.Entities
{
	public class SimuladorGeo
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String Cpf { get; set; }

		public Int32 MecanismoElaboracaoId { get; set; }
		public String MecanismoElaboracaoTexto { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public String SistemaCoordenada { get; set; }

		public String Easting { get; set; }
		public String Northing { get; set; }

		public Decimal EastingDecimal { get { return (String.IsNullOrWhiteSpace(Easting)) ? Decimal.Zero : Decimal.Parse(Easting); } }
		public Decimal NorthingDecimal { get { return (String.IsNullOrWhiteSpace(Northing)) ? Decimal.Zero : Decimal.Parse(Northing); } }

		public Decimal MenorX { get; set; }
		public Decimal MenorY { get; set; }
		public Decimal MaiorX { get; set; }
		public Decimal MaiorY { get; set; }

		private List<SimuladorGeoArquivo> _arquivos = new List<SimuladorGeoArquivo>();
		public List<SimuladorGeoArquivo> Arquivos
		{
			get { return _arquivos; }
			set { _arquivos = value; }
		}

		private SimuladorGeoArquivo _arquivosEnviado = new SimuladorGeoArquivo();
		public SimuladorGeoArquivo ArquivoEnviado
		{
			get { return _arquivosEnviado; }
			set { _arquivosEnviado = value; }
		}

		public SimuladorGeo()
		{
			ArquivoEnviado = new SimuladorGeoArquivo();
		}
	}
}
