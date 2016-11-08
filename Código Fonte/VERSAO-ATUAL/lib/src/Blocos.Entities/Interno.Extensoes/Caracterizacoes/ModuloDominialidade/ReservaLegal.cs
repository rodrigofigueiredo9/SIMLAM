

using System;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade
{
	public class ReservaLegal
	{
		public Int32 Id { get; set; }
		public Int32? IDRelacionamento { get; set; }
		public String Tid { get; set; }
		public Boolean Compensada { get; set; }
		public Int32 SituacaoId { get; set; }
		public String SituacaoTexto { get; set; }
		public Int32 LocalizacaoId { get; set; }
		public String LocalizacaoTexto { get; set; }
		public String Identificacao { get; set; }

		public Decimal ARLCroqui { get; set; }
		public String ARLCroquiTexto { get { return ARLCroqui.ToStringTrunc(); } }

		public String NumeroTermo { get; set; }
		public String NumeroCartorio { get; set; }
		public Int32 TipoCartorioId { get; set; }
		public String TipoCartorioTexto { get; set; }
		public String NomeCartorio { get; set; }

		//Matricula da compensação
		public Int32? MatriculaId { get; set; }
		public String MatriculaIdentificacao { get; set; }
		public String MatriculaTexto { get; set; }
		//Matricula da compensação
		public String NumeroFolha { get; set; }
		public String NumeroLivro { get; set; }

		public Int32? SituacaoVegetalId { get; set; }
		public String SituacaoVegetalTexto { get; set; }
		public String SituacaoVegetalGeo
		{
			get
			{
				switch ((eReservaLegalSituacaoVegetal)SituacaoVegetalId.GetValueOrDefault())
				{
					case eReservaLegalSituacaoVegetal.Preservada:
						return "PRESERV";

					case eReservaLegalSituacaoVegetal.EmUso:
						return "USO";

					case eReservaLegalSituacaoVegetal.EmRecuperacao:
						return "REC";

					case eReservaLegalSituacaoVegetal.NaoCaracterizada:
						return "D";

					default:
						return string.Empty;
				}
			}
			set
			{
				if (!String.IsNullOrEmpty(value))
				{
					value = value.ToUpper();
				}

				switch (value)
				{
					case "PRESERV":
						SituacaoVegetalId = (int)eReservaLegalSituacaoVegetal.Preservada;
						break;

					case "USO":
						SituacaoVegetalId = (int)eReservaLegalSituacaoVegetal.EmUso;
						break;

					case "REC":
						SituacaoVegetalId = (int)eReservaLegalSituacaoVegetal.EmRecuperacao;
						break;

					case "D":
						SituacaoVegetalId = (int)eReservaLegalSituacaoVegetal.NaoCaracterizada;
						break;

					default:
						SituacaoVegetalId = null;
						break;
				}
			}
		}

		public String MatriculaNumero { get; set; }
		public String AverbacaoNumero { get; set; }
		public Decimal ARLRecebida { get; set; }
		public Int32? CedentePossuiEmpreendimento { get; set; }
		public Decimal ARLCedida { get; set; }
		public Int32 IdentificacaoARLCedente { get; set; }
		public String ARLCedenteTID { get; set; }
		public Int32 EmpreendimentoId { get; set; }


		public String Compensacao
		{
			get
			{
				if (Compensada)
				{
					return "Cedente";
				}

				if (LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora || LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoMatriculaReceptora)
				{
					return "Receptora";
				}

				return "";
			}
		}

		public eCompensacaoTipo CompensacaoTipo
		{
			get
			{
				if (Compensada)
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

		public string AreaReservaLegal
		{
			get
			{
				if (ARLCroqui > 0)
				{
					return ARLCroquiTexto;
				}

				if (LocalizacaoId == (int)eReservaLegalLocalizacao.CompensacaoEmpreendimentoReceptora)
				{
					if (Convert.ToBoolean(CedentePossuiEmpreendimento))
					{
						return ARLCedida.ToStringTrunc();
					}

					if (SituacaoId == (int)eReservaLegalSituacao.Registrada && !Convert.ToBoolean(CedentePossuiEmpreendimento))
					{
						return ARLCedida.ToStringTrunc();
					}
				}

				if (SituacaoId == (int)eReservaLegalSituacao.NaoInformada)
				{
					return "---";
				}

				return "";
			}
		}
		public bool Excluir { get; set; }
		public EmpreendimentoCaracterizacao EmpreendimentoCompensacao { get; set; }

		public Coordenada Coordenada { get; set; }

		public ReservaLegal()
		{
			EmpreendimentoCompensacao = new EmpreendimentoCaracterizacao();
			if (Coordenada == null)
			{
				Coordenada = new Coordenada();

			}
		}
	}
}