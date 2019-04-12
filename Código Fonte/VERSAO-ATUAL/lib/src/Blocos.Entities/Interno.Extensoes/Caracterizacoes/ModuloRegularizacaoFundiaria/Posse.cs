using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;

namespace Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria
{
	public class Posse
	{
		public Int32? Id { get; set; }
		public Int32? Dominio { get; set; }
		public eDominioTipo Tipo { get { return eDominioTipo.Posse; } }
		public String Tid { get; set; }
		public String Identificacao { get; set; }
		public Int32 ComprovacaoId { get; set; }
		public String ComprovacaoTexto { get; set; }
		public Decimal AreaCroqui { get; set; }
		public Decimal? Perimetro { get; set; }
		public Decimal AreaRequerida { set; get; }
		public Int32 Zona { set; get; }
		public Int32 RegularizacaoTipo { set; get; }
		public Int32 RelacaoTrabalho { set; get; }
		public String Benfeitorias { get; set; }
		public String Observacoes { get; set; }
	
		public Decimal AreaPosseDocumento { get; set; }
		public String DescricaoComprovacao { get; set; }
		public long? NumeroCCIR { get; set; }
		public Decimal AreaCCIR { get; set; }

		private DateTecno _dataUltimaAtualizacaoCCIR = new DateTecno();
		public DateTecno DataUltimaAtualizacaoCCIR
		{
			get { return _dataUltimaAtualizacaoCCIR; }
			set { _dataUltimaAtualizacaoCCIR = value; }
		}

		public string ConfrontacoesNorte { get; set; }
		public string ConfrontacoesSul { get; set; }
		public string ConfrontacoesLeste { get; set; }
		public string ConfrontacoesOeste { get; set; }

		public Distancia Distancia { set; get; }
		public List<Opcao> Opcoes { set; get; }
		public List<TransmitentePosse> Transmitentes { set; get; }
		public List<Edificacao> Edificacoes { set; get; }
		public List<UsoAtualSolo> UsoAtualSolo { set; get; }

		public int? PossuiDominioAvulso { get; set; }
		public bool PossuiDominioAvulsoBool { get { return Convert.ToBoolean(PossuiDominioAvulso); } }
		public List<Dominio> DominiosAvulsos { set; get; }

		private Dominio _dominioPosse = new Dominio();
		public Dominio DominioPosse
		{
			get { return _dominioPosse; }
			set { _dominioPosse = value; }
		}

		private List<SelectListItem> _comprovacoes = new List<SelectListItem>();
		public List<SelectListItem> Comprovacoes
		{
			get { return _comprovacoes; }
			set { _comprovacoes = value; }
		}

		public Posse(Dominio dominio, int zona)
		{
			Dominio = dominio.Id;
			Identificacao = dominio.Identificacao;
			ComprovacaoTexto = dominio.ComprovacaoTexto;
			AreaCroqui = dominio.AreaCroqui;
			Perimetro = dominio.Perimetro;
			Zona = zona;
			DominioPosse = dominio;

			DominiosAvulsos = new List<Dominio>();
			Opcoes = new List<Opcao>();
			Distancia = new Distancia();
			Transmitentes = new List<TransmitentePosse>();
			UsoAtualSolo = new List<UsoAtualSolo>();
			Edificacoes = new List<Edificacao>();
		}

		public Posse()
		{
			DominiosAvulsos = new List<Dominio>();
			Opcoes = new List<Opcao>();
			Distancia = new Distancia();
			Transmitentes = new List<TransmitentePosse>();
			UsoAtualSolo = new List<UsoAtualSolo>();
			Edificacoes = new List<Edificacao>();
		}
	}
}