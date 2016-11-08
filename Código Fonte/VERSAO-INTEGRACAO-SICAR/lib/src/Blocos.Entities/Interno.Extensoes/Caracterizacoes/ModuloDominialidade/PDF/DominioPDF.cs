using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF
{
	public class DominioPDF
	{
		public Int32 Id { get; set; }
		public string Tid { get; set; }
		public Int32 TipoId { get; set; }
		public String TipoTexto { get; set; }
		public String MatriculaComprovacao { get { return string.IsNullOrEmpty(Matricula) ? ComprovacaoTexto : Matricula; } }
		public String Matricula { get; set; }
		public String ComprovacaoTexto { get; set; }
		public Int64? NumeroCCIR { get; set; }
		public String AreaCroqui { get { return AreaCroquiDecimal.ToStringTrunc(); } }
		public Decimal AreaCroquiDecimal { get; set; }
		public String Registro { get; set; }
		public String AreaDocumento { get { return AreaDocumentoDecimal.ToStringTrunc(); } }
		public Decimal AreaDocumentoDecimal { get; set; }
		public String AreaCCIR { get { return AreaCCIRDecimal.ToStringTrunc(); } }
		public Decimal AreaCCIRDecimal { get; set; }
		public String APPCroqui { get { return APPCroquiDecimal.ToStringTrunc(); } }
		public Decimal APPCroquiDecimal { get; set; }
		public String Cartorio { get; set; }
		public String Folha { get; set; }
		public String Livro { get; set; }
		public String DataAtualizacao { set; get; }
		public String Perimetro { get; set; }
		public String ARLDocumento { get; set; }
		public String ConfrontacaoNorte { get; set; }
		public String ConfrontacaoSul { get; set; }
		public String ConfrontacaoLeste { get; set; }
		public String ConfrontacaoOeste { get; set; }
		public string Identificacao { get; set; }
		public String ReservasNumeroTermo
		{
			get
			{
				return Concatenar(ReservasLegais.Select(x => x.NumeroTermo).Distinct().ToList());
			}
		}

		private eDominioTipo _tipo = eDominioTipo.Matricula;
		public eDominioTipo Tipo
		{
			get { return _tipo; }
			set { _tipo = value; }
		}

		public String ReservasAreaRLPCroqui { get { return ReservasAreaRLPCroquiDecimal.ToStringTrunc(); } }
		public String ReservasAreaRLPCroquiHa
		{
			get
			{
				if (ReservasAreaRLPCroquiDecimal > 0)
				{
					return ReservasAreaRLPCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
				}

				return "0";
			}
		}
		public Decimal ReservasAreaRLPCroquiDecimal
		{
			get
			{
				return ReservasLegais
					.Where(x => x.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.Preservada)
					.Sum(y => y.ARLCroquiDecimal);
			}
		}

		public String ReservasAreaRLFCroqui { get { return ReservasAreaRLFCroquiDecimal.ToStringTrunc(); } }
		public String ReservasAreaRLFCroquiHa
		{
			get
			{
				if (ReservasAreaRLFCroquiDecimal == 0)
				{
					return "0";
				}
				return ReservasAreaRLFCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
			}
		}
		public Decimal ReservasAreaRLFCroquiDecimal
		{
			get
			{
				return ReservasLegais
					.Where(x => x.SituacaoVegetalId == (int)eReservaLegalSituacaoVegetal.EmRecuperacao)
					.Sum(y => y.ARLCroquiDecimal);
			}
		}

		public String ReservasAreaRLCroqui { get { return ReservasAreaRLCroquiDecimal.ToStringTrunc(); } }
		public String ReservasAreaRLCroquiHa
		{
			get
			{
				if (ReservasAreaRLCroquiDecimal == 0)
				{
					return "0";
				}
				return ReservasAreaRLCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
			}
		}
		public Decimal ReservasAreaRLCroquiDecimal
		{
			get
			{
				return ReservasLegais.Sum(y => y.ARLCroquiDecimal);
			}
		}

		#region Valores em Hectares

		public String AreaCroquiHa
		{
			get
			{
				if (AreaCroquiDecimal == 0)
				{
					return "0";
				}
				return AreaCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
			}
		}

		public String APPCroquiHa
		{
			get
			{
				if (APPCroquiDecimal == 0)
				{
					return "0";
				}
				return APPCroquiDecimal.Convert(eMetrica.M2ToHa).ToStringTrunc(4);
			}
		}

		#endregion

		private List<ReservaLegalPDF> _reservasLegais = new List<ReservaLegalPDF>();
		public List<ReservaLegalPDF> ReservasLegais
		{
			get { return _reservasLegais; }
			set { _reservasLegais = value; }
		}

		public DominioPDF() { }

		public string TipoCompensacao { get; set; }

		public DominioPDF(Dominio dominio)
		{
			Id = dominio.Id.GetValueOrDefault();

			Identificacao = dominio.Identificacao;
			TipoTexto = dominio.TipoTexto;
			ComprovacaoTexto = dominio.ComprovacaoTexto;
			Matricula = dominio.Matricula;
			NumeroCCIR = dominio.NumeroCCIR;
			Registro = dominio.DescricaoComprovacao; // Nome do campo alterado

			AreaCroquiDecimal = dominio.AreaCroqui;
			AreaDocumentoDecimal = dominio.AreaDocumento;
			AreaCCIRDecimal = dominio.AreaCCIR;
			APPCroquiDecimal = dominio.APPCroqui;

			Cartorio = dominio.Cartorio;
			Folha = dominio.Folha;
			Livro = dominio.Livro;
			Tipo = dominio.Tipo;
			DataAtualizacao = dominio.DataUltimaAtualizacao.DataTexto;
			ReservasLegais = dominio.ReservasLegais.Select(x => new ReservaLegalPDF(x)).ToList();
			ARLDocumento = dominio.ARLDocumento.ToStringTrunc();
			ConfrontacaoNorte = dominio.ConfrontacaoNorte;
			ConfrontacaoSul = dominio.ConfrontacaoSul;
			ConfrontacaoLeste = dominio.ConfrontacaoLeste;
			ConfrontacaoOeste = dominio.ConfrontacaoOeste;

			//Obtendo registro
			if (dominio.Tipo == eDominioTipo.Posse)
			{
				if (dominio.ComprovacaoId != (int)eDominioComprovacao.PosseiroPrimitivo)
				{
					Registro = dominio.ComprovacaoTexto + " - " + dominio.DescricaoComprovacao; // Nome do campo alterado
				}
				else
				{
					Registro = dominio.ComprovacaoTexto + " - CCIR Nº " + dominio.NumeroCCIR;
				}
			}
			else
			{
				Registro = "Folha -  " + dominio.Folha + ", livro - " + dominio.Livro + " do " + dominio.Cartorio;

			}

		}

		public static String Concatenar(List<String> lista)
		{
			if (lista.Count == 0)
				return String.Empty;

			if (lista.Count == 1)
				return lista[0];

			string lastItem = lista.Last();
			lista.RemoveAt(lista.Count - 1);
			return String.Join(", ", lista) + " e " + lastItem;

		}

		public static String Concatenar(string textoStragg, char separador = ';')
		{
			List<String> lista = textoStragg.Split(separador).ToList();

			if (lista.Count == 0)
				return String.Empty;

			if (lista.Count == 1)
				return lista[0];

			string lastItem = lista.Last();
			lista.RemoveAt(lista.Count - 1);
			return String.Join(", ", lista) + " e " + lastItem;
		}
	}
}
