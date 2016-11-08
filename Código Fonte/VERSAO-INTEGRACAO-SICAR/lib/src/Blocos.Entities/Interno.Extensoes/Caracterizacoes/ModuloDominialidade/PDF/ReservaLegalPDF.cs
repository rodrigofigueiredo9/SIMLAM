using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF
{
	public class ReservaLegalPDF
	{
		public int Id { get; set; }
		public string Tid { get; set; }
		public Boolean Compensada { get; set; }

		public String Compensacao { get; set; }
		public string Identificacao { get; set; }
		public string SituacaoVegetalTexto { get; set; }
		public String ARLCroqui { get; set; }
		public Decimal ARLCroquiDecimal { get; set; }

		public Decimal ARLCedida { get; set; }
		public Int32 LocalizacaoId { get; set; }
		public Int32? CedentePossuiEmpreendimento { get; set; }

		public int IdentificacaoARLCedente { get; set; }

		public String ARLDocumento { get; set; }
		public Decimal ARLDocumentoDecimal { get; set; }

		public String NumeroTermo { get; set; }
		public Int32 SituacaoId { get; set; }
		public Int32 SituacaoVegetalId { get; set; }
		public string SituacaoTexto { get; set; }

		public EmpreendimentoCaracterizacao EmpreendimentoCompensacao { get; set; }

		public eCompensacaoTipo CompensacaoTipo
		{
			get
			{
				if (LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoCedente || LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoMatriculaCedente)
				{
					return eCompensacaoTipo.Cedente;
				}

				if (LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora || LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoMatriculaReceptora)
				{
					return eCompensacaoTipo.Receptora;
				}

				return eCompensacaoTipo.Nulo;
			}
		}

		public Coordenada Coordenada { get; set; }

		public ReservaLegalPDF() { }

		public ReservaLegalPDF(ReservaLegal reserva)
		{
			ARLCroqui = reserva.AreaReservaLegal;
			ARLCroquiDecimal = reserva.ARLCroqui;

			Identificacao = reserva.Identificacao;
			NumeroTermo = reserva.NumeroTermo;
			SituacaoId = reserva.SituacaoId;
			SituacaoVegetalId = reserva.SituacaoVegetalId.GetValueOrDefault();
			SituacaoTexto = reserva.SituacaoTexto;
			SituacaoVegetalTexto = reserva.SituacaoVegetalTexto;
			Compensada = reserva.Compensada;
			Compensacao = reserva.Compensacao;
			LocalizacaoId = reserva.LocalizacaoId;
			CedentePossuiEmpreendimento = reserva.CedentePossuiEmpreendimento;
			ARLCedida = reserva.ARLCedida;
			EmpreendimentoCompensacao = reserva.EmpreendimentoCompensacao;
			IdentificacaoARLCedente = reserva.IdentificacaoARLCedente;
			Coordenada = reserva.Coordenada;
		}
	}
}