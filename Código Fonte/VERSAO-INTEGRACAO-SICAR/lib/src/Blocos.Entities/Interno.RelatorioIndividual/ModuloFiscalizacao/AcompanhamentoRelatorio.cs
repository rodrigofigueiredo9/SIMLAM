using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;

namespace Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao
{
	public class AcompanhamentoRelatorio
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String NumeroSufixo { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public String Motivo { get; set; }
		public Int32 FiscalizacaoId { get; set; }

		public String Numero
		{
			get
			{
				if (Id > 0)
				{
					return FiscalizacaoId + "-" + NumeroSufixo;
				}

				return "Gerado automaticamente";
			}
		}

		public Int32 AgenteId { get; set; }
		public String AgenteNome { get; set; }
		public Int32 SetorId { get; set; }

		public String AreaTotal { get; set; }
		public String AreaFlorestalNativa { get; set; }
		public String ReservalegalTipo { get; set; }
		public String OpniaoAreaEmbargo { get; set; }

		public String AtividadeAreaEmbargada { get; set; }
		public String AtividadeAreaEmbargadaEspecificarTexto { get; set; }
		public String UsoAreaSoloDescricao { get; set; }
		public String CaracteristicaSoloAreaDanificada { get; set; }

		public String AreaDeclividadeMedia { get; set; }
		public String InfracaoResultouErosao { get; set; }
		public String InfracaoResultouErosaoEspecificar { get; set; }

		public String OpniaoDestMaterialApreend { get; set; }
		public String HouveDesrespeitoTAD { get; set; }
		public String HouveDesrespeitoTADEspecificar { get; set; }
		public String InformacoesRelevanteProcesso { get; set; }

		public String RepararDanoAmbiental { get; set; }
		public String RepararDanoAmbientalEspecificar { get; set; }
		public String FirmouTermoRepararDanoAmbiental { get; set; }
		public String FirmouTermoRepararDanoAmbientalEspecificar { get; set; }

		private DateTecno _dataVistoria = new DateTecno();
		public DateTecno DataVistoria
		{
			get { return _dataVistoria; }
			set { _dataVistoria = value; }
		}

		private DateTecno _dataSituacao = new DateTecno();
		public DateTecno DataSituacao
		{
			get { return _dataSituacao; }
			set { _dataSituacao = value; }
		}

		public String DataConclusao
		{
			get
			{
				if (SituacaoId == (int)eAcompanhamentoSituacaoRelatorio.CadastroConcluido)
				{
					return DataSituacao.DataTexto;
				}

				return String.Empty;
			}

		}

		private List<AssinanteDefault> _assinantes = new List<AssinanteDefault>();
		public List<AssinanteDefault> Assinantes
		{
			get { return _assinantes; }
			set { _assinantes = value; }
		}

		private List<ConsideracoesFinaisAnexoRelatorio> _anexos = new List<ConsideracoesFinaisAnexoRelatorio>();
		public List<ConsideracoesFinaisAnexoRelatorio> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		public AcompanhamentoRelatorio() { }

		public AcompanhamentoRelatorio(FiscalizacaoRelatorio fiscalizacao)
		{
			FiscalizacaoId = fiscalizacao.Id;
			AreaTotal = fiscalizacao.ComplementacaoDados.AreaInformada;
			AreaFlorestalNativa = fiscalizacao.ComplementacaoDados.AreaFlorestaNativa;
			OpniaoAreaEmbargo = fiscalizacao.ObjetoInfracao.OpinarEmbargo;
			AtividadeAreaEmbargada = fiscalizacao.ObjetoInfracao.AtividadeAreaDegradado;
			AtividadeAreaEmbargadaEspecificarTexto = fiscalizacao.ObjetoInfracao.AtividadeAreaDegradadoEspecif;
			CaracteristicaSoloAreaDanificada = fiscalizacao.ObjetoInfracao.CaracteristicaoAreaSoloDanifi;
			AreaDeclividadeMedia = fiscalizacao.ObjetoInfracao.DeclividadeMedia;
			InfracaoResultouErosao = fiscalizacao.ObjetoInfracao.IsInfracaoErosaoSolo;
			InfracaoResultouErosaoEspecificar = fiscalizacao.ObjetoInfracao.EspecificarIsInfracaoErosaoSolo;
			OpniaoDestMaterialApreend = fiscalizacao.MaterialApreendido.OpinarDestino;

			Assinantes = fiscalizacao.ConsideracoesFinais.Assinantes;
			Anexos = fiscalizacao.ConsideracoesFinais.Anexos;
		}

	}
}