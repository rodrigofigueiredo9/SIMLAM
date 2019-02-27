using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF
{
	public class BarragemDispensaLicencaPDF
	{
		public int Id { get; set; }
		public string NorthingLatitude { get; set; }
		public string EastingLongitude { get; set; }
		public string BarragemTipo { get; set; }
		public string NumeroARTElaboracao { get; set; }
		public string NumeroARTExecucao { get; set; }
		public decimal? AreaAlagada { get; set; }
		public decimal? VolumeArmazanado { get; set; }
		public int FinalidadeAtividade { get; set; }
		public BarragemDispensaLicenca barragemEntity { get; set; }
		public List<Lista> finalidades { get; set; }
		public string CampoNome { get; set; }
		public string CampoValor { get; set; }
		public bool? HaSupresVegetApp { get; set; }

		public BarragemDispensaLicencaPDF(BarragemDispensaLicenca caracterizacao)
		{
			barragemEntity = caracterizacao;
			Id = caracterizacao.Id;
			NorthingLatitude = caracterizacao.Coordenada.NorthingUtmTexto;
			EastingLongitude = caracterizacao.Coordenada.EastingUtmTexto;
			NumeroARTElaboracao = caracterizacao.NumeroARTElaboracao;
			NumeroARTExecucao = caracterizacao.NumeroARTExecucao;
			AreaAlagada = caracterizacao.areaAlagada;
			VolumeArmazanado = caracterizacao.volumeArmazanado;
			HaSupresVegetApp = caracterizacao.haSupresVegetApp;
		}
	}
}