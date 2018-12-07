using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class AlterarSituacaoVM
	{
		public Int32 Id { get; set; }
		public String Numero { get; set; }
		public String Modelo { get; set; }
		public Int32? ModeloId { get; set; }
		public Int32? ModeloCodigo { get; set; }
		public String CodigoSicar { get; set; }
		public Int32 SituacaoId { get; set; }
		public String Situacao { get; set; }
		public String DataSituacaoAtual { get; set; }
		public String DataVencimento { get; set; }
		public String DataConclusao { get; set; }
		public String LabelPrazo { get; set; }
		public Boolean MostrarPrazo { get; set; }
		public Boolean PrazoAutomatico { get; set; }
		public String CodigoSinaflor { get; set; }

		private List<SelectListItem> _situacoes = new List<SelectListItem>();
		public List<SelectListItem> Situacoes
		{
			get { return _situacoes; }
			set { _situacoes = value; }
		}

		private List<SelectListItem> _motivosEncerramento = new List<SelectListItem>();
		public List<SelectListItem> MotivosEncerramento
		{
			get { return _motivosEncerramento; }
			set { _motivosEncerramento = value; }
		}
		
		private List<Acao> _acoesAlterar = new List<Acao>();
		public List<Acao> AcoesAlterar
		{
			get { return _acoesAlterar; }
			set { _acoesAlterar = value; }
		}

		public AlterarSituacaoVM() { }

		public AlterarSituacaoVM(List<Motivo> motivosEncerramento, Titulo titulo, List<Situacao> situacoes = null, string codigoSicar = null) 
		{
			MotivosEncerramento = ViewModelHelper.CriarSelectList(motivosEncerramento, true);

			if (situacoes != null)
			{
				Situacoes = ViewModelHelper.CriarSelectList(situacoes, true);
			}

			this.Id = titulo.Id;
			this.Numero = titulo.Numero.Texto;
			this.Modelo = titulo.Modelo.Nome;
			this.ModeloId = titulo.Modelo.Id;
			this.ModeloCodigo = titulo.Modelo.Codigo;
			this.SituacaoId = titulo.Situacao.Id;
			this.Situacao = titulo.Situacao.Texto;
			this.DataVencimento = titulo.DataVencimento.DataTexto;
			this.DataSituacaoAtual = titulo.DataSituacao.DataTexto;
			this.CodigoSinaflor = titulo.CodigoSinaflor;
			this.CodigoSicar = codigoSicar;
		}
	}
}