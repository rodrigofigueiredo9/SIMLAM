﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragemDispensaLicenca
{
	public class BarragemDispensaLicenca
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public int EmpreendimentoID { get; set; }
        public int EmpreendimentoCodigo { get; set; }
		public int AtividadeID { get; set; }
		public string Atividade { get; set; }

		public eBarragemTipo BarragemTipo { get; set; }
		public eRTElabocarao rtElaboracao  { get; set; }
		public eFase faseInstalacao { get; set; }
		public List<BarragemCoordenada> coordenadas { get; set; }

		public bool barragemContiguaMesmoNivel { get; set; }
		public decimal areaAlagada { get; set; }
		public decimal volumeArmazanado { get; set; }
		public decimal alturaBarramento { get; set; }
		public decimal comprimentoBarramento { get; set; }
		public decimal larguraBaseBarramento { get; set; }
		public decimal larguraCristaBarramento { get; set; }
		public List<int> Finalidade { get; set; }
		public string cursoHidrico { get; set; }
		public decimal areaBaciaContribuicao { get; set; }
		public decimal intensidadeMaxPrecipitacao { get; set; }
		public string fonteDadosIntensidadeMax { get; set; }
		public decimal periodoRetorno { get; set; }
		public decimal coeficienteEscoamento { get; set; }
		public string fonteDadosCoeficienteEscoamento { get; set; }
		public decimal tempoConcentracao { get; set; }
		public string tempoConcentracaoEquacaoUtilizada { get; set; }
		public decimal vazaoEnchente { get; set; }
		public string fonteDadosVazaoEnchente { get; set; }

		public decimal? Precipitacao { get; set; }
		public string EquacaoCalculo { get; set; }
		public int? Fase { get; set; }
		public int? PossuiMonge { get; set; }
		public int? MongeTipo { get; set; }
		public string EspecificacaoMonge { get; set; }
		public int? PossuiVertedouro { get; set; }
		public int? VertedouroTipo { get; set; }
		public string EspecificacaoVertedouro { get; set; }
		public int? PossuiEstruturaHidraulica { get; set; }
		public string AdequacoesRealizada { get; set; }
		public string DataInicioObra { get; set; }
		public string DataPrevisaoTerminoObra { get; set; }
		public Coordenada Coordenada { get; set; }
		public int FormacaoRT { get; set; }
		public string EspecificacaoRT { get; set; }
		public Arquivo.Arquivo Autorizacao { get; set; }
		public string NumeroARTElaboracao { get; set; }
		public string NumeroARTExecucao { get; set; }
        public int InternoID { get; set; }
        public string InternoTID { get; set; }
        public int CredenciadoID { get; set; }

		public BarragemConstruidaAConstruir construidaConstruir { get; set; }
		public List<BarragemRT> responsaveisTecnicos { get; set; }

		public BarragemDispensaLicenca()
		{
			Coordenada = new Coordenada();
			Autorizacao = new Arquivo.Arquivo();
		}
	}
}