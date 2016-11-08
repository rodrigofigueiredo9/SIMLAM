using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class InfracaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private Infracao _infracao = new Infracao();
		public Infracao Infracao
		{
			get { return _infracao; }
			set { _infracao = value; }
		}

		private DateTecno _dataConclusaoFiscalizacao = new DateTecno();
		public DateTecno DataConclusaoFiscalizacao
		{
			get { return _dataConclusaoFiscalizacao; }
			set { _dataConclusaoFiscalizacao = value; }
		}

		private List<SelectListItem> _classificacoes = new List<SelectListItem>();
		public List<SelectListItem> Classificacoes
		{
			get { return _classificacoes; }
			set { _classificacoes = value; }
		}

		private List<SelectListItem> _tipos = new List<SelectListItem>();
		public List<SelectListItem> Tipos
		{
			get { return _tipos; }
			set { _tipos = value; }
		}

		private List<SelectListItem> _itens = new List<SelectListItem>();
		public List<SelectListItem> Itens
		{
			get { return _itens; }
			set { _itens = value; }
		}

		private List<SelectListItem> _subitens = new List<SelectListItem>();
		public List<SelectListItem> Subitens
		{
			get { return _subitens; }
			set { _subitens = value; }
		}

		private List<InfracaoCampo> _campos = new List<InfracaoCampo>();
		public List<InfracaoCampo> Campos
		{
			get { return _campos; }
			set { _campos = value; }
		}

		private List<InfracaoPergunta> _perguntas = new List<InfracaoPergunta>();
		public List<InfracaoPergunta> Perguntas
		{
			get { return _perguntas; }
			set { _perguntas = value; }
		}

		private List<SelectListItem> _series = new List<SelectListItem>();
		public List<SelectListItem> Series
		{
			get { return _series; }
			set { _series = value; }
		}

		private List<SelectListItem> _codigoReceitas = new List<SelectListItem>();
		public List<SelectListItem> CodigoReceitas
		{
			get { return _codigoReceitas; }
			set { _codigoReceitas = value; }
		}

		public String TiposArquivoValido = ViewModelHelper.Json(new ArrayList { ".pdf" });

		public string ArquivoJSon { get; set; }

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@Salvar = Mensagem.InfracaoMsg.Salvar,
					@ArquivoNaoEhDoc = Mensagem.TituloModelo.TipoArquivoDoc,
					@ArquivoObrigatorio = Mensagem.TituloModelo.ArquivoObrigatorio,
					@ConfigAlteradaConfirme = Mensagem.InfracaoMsg.ConfigAlteradaConfirme
				});
			}
		}	
	}
}