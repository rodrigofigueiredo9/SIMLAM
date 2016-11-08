using System;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade
{
	public class Dominio
	{
		public Int32? Id { get; set; }
		public String Tid { get; set; }

		public Int32 EmpreendimentoLocalizacao { get; set; }
		public String Identificacao { get; set; }
		public String Matricula { get; set; }
		public String Folha { get; set; }
		public String Livro { get; set; }
		public String Cartorio { get; set; }
		public Int32 ComprovacaoId { get; set; }
		public String ComprovacaoTexto { get; set; }
		public Decimal Perimetro { get; set; }
		public Decimal AreaCroqui { get; set; }
		public String AreaCroquiTexto { get { return AreaCroqui.ToStringTrunc(); } }
		
		public Decimal AreaDocumento { get; set; }
		public String AreaDocumentoTexto{ get; set; }

		public String DescricaoComprovacao { get; set; }
		public Int64? NumeroCCIR { get; set; }
		
		public Decimal AreaCCIR { get; set; }
		public String AreaCCIRTexto { get; set; }

		public Decimal APPCroqui { get; set; }
		public String APPCroquiTexto { get { return APPCroqui.ToStringTrunc(); } }
		
		public String TipoTexto { get; set; }
		public String ConfrontacaoNorte { get; set; }
		public String ConfrontacaoSul { get; set; }
		public String ConfrontacaoLeste { get; set; }
		public String ConfrontacaoOeste { get; set; }

		public char TipoGeo
		{
			get
			{
				if (_tipo == eDominioTipo.Matricula)
				{
					return 'M';
				}
				else
				{
					return 'P';
				}
			}
			set
			{
				if (value == 'M')
				{
					_tipo = eDominioTipo.Matricula;
				}
				else
				{
					_tipo = eDominioTipo.Posse;
				}
			}
		}

		private eDominioTipo _tipo = eDominioTipo.Matricula;
		public eDominioTipo Tipo
		{
			get { return _tipo; }
			set { _tipo = value; }
		}

		private DateTecno _dataUltimaAtualizacao = new DateTecno();
		public DateTecno DataUltimaAtualizacao
		{
			get { return _dataUltimaAtualizacao; }
			set { _dataUltimaAtualizacao = value; }
		}

		private List<ReservaLegal> _reservasLegais = new List<ReservaLegal>();
		public List<ReservaLegal> ReservasLegais
		{
			get { return _reservasLegais; }
			set { _reservasLegais = value; }
		}

		public Decimal? ARLDocumento { get; set; }
		public String ARLDocumentoTexto{ get; set; }
	}
}