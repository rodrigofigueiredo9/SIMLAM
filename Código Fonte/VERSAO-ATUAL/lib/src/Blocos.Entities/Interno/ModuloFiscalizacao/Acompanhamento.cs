using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao
{
	public class Acompanhamento
	{
		public Int32 Id { get; set; }
		public String Tid { get; set; }
		public String NumeroSufixo { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public String Motivo { get; set; }
		public Int32 FiscalizacaoId { get; set; }

		public Boolean? PossuiAreaEmbargadaOuAtividadeInterditada { get; set; }
		public Boolean? HouveApreensaoMaterial { get; set; }

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
		public Int32? ReservalegalTipo { get; set; }
		public String OpniaoAreaEmbargo { get; set; }

		public Int32? AtividadeAreaEmbargada { get; set; }
		public String AtividadeAreaEmbargadaEspecificarTexto { get; set; }
		public String UsoAreaSoloDescricao { get; set; }
		public Int32? CaracteristicaSoloAreaDanificada { get; set; }

		public String AreaDeclividadeMedia { get; set; }
		public Int32 InfracaoResultouErosao { get; set; }
		public String InfracaoResultouErosaoEspecificar { get; set; }

		public String OpniaoDestMaterialApreend { get; set; }
		public Int32? HouveDesrespeitoTAD { get; set; }
		public String HouveDesrespeitoTADEspecificar { get; set; }
		public String InformacoesRelevanteProcesso { get; set; }

		public Int32? RepararDanoAmbiental { get; set; }
		public String RepararDanoAmbientalEspecificar { get; set; }
		public Int32? FirmouTermoRepararDanoAmbiental { get; set; }
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

		private List<FiscalizacaoAssinante> _assinantes = new List<FiscalizacaoAssinante>();
		public List<FiscalizacaoAssinante> Assinantes
		{
			get { return _assinantes; }
			set { _assinantes = value; }
		}

		private List<Anexo> _anexos = new List<Anexo>();
		public List<Anexo> Anexos
		{
			get { return _anexos; }
			set { _anexos = value; }
		}

		private Arquivo.Arquivo _pdfLaudo = new Arquivo.Arquivo();
		public Arquivo.Arquivo PdfLaudo
		{
			get { return _pdfLaudo; }
			set { _pdfLaudo = value; }
		}

		private Arquivo.Arquivo _arquivo = new Arquivo.Arquivo();
		public Arquivo.Arquivo Arquivo
		{
			get { return _arquivo; }
			set { _arquivo = value; }
		}

		public Acompanhamento() { }

		public Acompanhamento(Fiscalizacao fiscalizacao) 
		{
			FiscalizacaoId = fiscalizacao.Id;

			PossuiAreaEmbargadaOuAtividadeInterditada = (fiscalizacao.ObjetoInfracao.AreaEmbargadaAtvIntermed.HasValue ? (fiscalizacao.ObjetoInfracao.AreaEmbargadaAtvIntermed.Value == 1) : new Nullable<Boolean>());
            //HouveApreensaoMaterial = fiscalizacao.MaterialApreendido.IsApreendido;

			AreaTotal = fiscalizacao.ComplementacaoDados.AreaTotalInformada;
			AreaFlorestalNativa = fiscalizacao.ComplementacaoDados.AreaCoberturaFlorestalNativa;
			ReservalegalTipo = fiscalizacao.ComplementacaoDados.ReservalegalTipo;
			OpniaoAreaEmbargo = fiscalizacao.ObjetoInfracao.OpniaoAreaDanificada;
			AtividadeAreaEmbargada = fiscalizacao.ObjetoInfracao.ExisteAtvAreaDegrad;
			AtividadeAreaEmbargadaEspecificarTexto = fiscalizacao.ObjetoInfracao.ExisteAtvAreaDegradEspecificarTexto;
			UsoAreaSoloDescricao = fiscalizacao.ObjetoInfracao.UsoSoloAreaDanificada;
			CaracteristicaSoloAreaDanificada = fiscalizacao.ObjetoInfracao.CaracteristicaSoloAreaDanificada;
			AreaDeclividadeMedia = fiscalizacao.ObjetoInfracao.AreaDeclividadeMedia;
			InfracaoResultouErosao = fiscalizacao.ObjetoInfracao.InfracaoResultouErosaoTipo;
			InfracaoResultouErosaoEspecificar = fiscalizacao.ObjetoInfracao.InfracaoResultouErosaoTipoTexto;
			OpniaoDestMaterialApreend = fiscalizacao.MaterialApreendido.Opiniao;
		}
	}
}