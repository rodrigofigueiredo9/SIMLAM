using System;
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
		public int? BarragemTipo { get; set; }
		public string BarragemTipoTexto { get; set; }
		public int FinalidadeAtividade { get; set; }
		public string CursoHidrico { get; set; }
		public decimal? VazaoEnchente { get; set; }
		public decimal? AreaBaciaContribuicao { get; set; }
		public decimal? Precipitacao { get; set; }
		public int? PeriodoRetorno { get; set; }
		public string CoeficienteEscoamento { get; set; }
		public string TempoConcentracao { get; set; }
		public string EquacaoCalculo { get; set; }
		public decimal? AreaAlagada { get; set; }
		public decimal? VolumeArmazanado { get; set; }
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
		public bool PossuiAssociacaoExterna { get; set; }

		public BarragemDispensaLicenca()
		{
			Coordenada = new Coordenada();
			Autorizacao = new Arquivo.Arquivo();
		}
	}
}