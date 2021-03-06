﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		public string CampoNome { get; set; }
		public string CampoValor { get; set; }

		public BarragemDispensaLicencaPDF(BarragemDispensaLicenca caracterizacao)
		{
			Id = caracterizacao.Id;
			NorthingLatitude = caracterizacao.Coordenada.NorthingUtmTexto;
			EastingLongitude = caracterizacao.Coordenada.EastingUtmTexto;
			BarragemTipo = caracterizacao.BarragemTipoTexto;
			NumeroARTElaboracao = caracterizacao.NumeroARTElaboracao;
			NumeroARTExecucao = caracterizacao.NumeroARTExecucao;
			AreaAlagada = caracterizacao.AreaAlagada;
			VolumeArmazanado = caracterizacao.VolumeArmazanado;
			FinalidadeAtividade = caracterizacao.FinalidadeAtividade;
		}
	}
}