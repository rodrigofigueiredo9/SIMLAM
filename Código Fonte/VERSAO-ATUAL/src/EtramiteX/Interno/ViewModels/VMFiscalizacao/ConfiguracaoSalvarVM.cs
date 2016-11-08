using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class ConfiguracaoSalvarVM
	{
		public Boolean IsVisualizar { get; set; }

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

		private List<SelectListItem> _campos = new List<SelectListItem>();
		public List<SelectListItem> Campos
		{
			get { return _campos; }
			set { _campos = value; }
		}

		private List<SelectListItem> _perguntas = new List<SelectListItem>();
		public List<SelectListItem> Perguntas
		{
			get { return _perguntas; }
			set { _perguntas = value; }
		}

		private ConfigFiscalizacao _configuracao = new ConfigFiscalizacao();
		public ConfigFiscalizacao Configuracao
		{
			get { return _configuracao; }
			set { _configuracao = value; }
		}

		public String JSON(object obj)
		{
			return ViewModelHelper.Json(obj);
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
					{
						@SubitemObrigatorio = Mensagem.FiscalizacaoConfiguracao.SubitemObrigatorio,
						@PerguntaObrigatorio = Mensagem.FiscalizacaoConfiguracao.PerguntaObrigatorio,
						@CampoObrigatorio = Mensagem.FiscalizacaoConfiguracao.CampoObrigatorio,
						@SubitemJaAdd = Mensagem.FiscalizacaoConfiguracao.SubitemJaAdd,
						@PerguntaJaAdd = Mensagem.FiscalizacaoConfiguracao.PerguntaJaAdd,
						@CampoJaAdd = Mensagem.FiscalizacaoConfiguracao.CampoJaAdd
					});
			}
		}
	}
}