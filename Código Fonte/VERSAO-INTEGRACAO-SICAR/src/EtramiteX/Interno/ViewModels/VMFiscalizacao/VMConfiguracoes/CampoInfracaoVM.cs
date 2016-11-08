using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao.Configuracoes;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class CampoInfracaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private CampoInfracao _entidade = new CampoInfracao();
		public CampoInfracao Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}

		private List<SelectListItem> _tipos = new List<SelectListItem>();
		public List<SelectListItem> Tipos
		{
			get { return _tipos; }
			set { _tipos = value; }
		}

		private List<SelectListItem> _unidades = new List<SelectListItem>();
		public List<SelectListItem> Unidades
		{
			get { return _unidades; }
			set { _unidades = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@NomeItemObrigatorio = Mensagem.FiscalizacaoConfiguracao.CampoNomeObrigatorio,
					@ItemDuplicado = Mensagem.FiscalizacaoConfiguracao.CampoJaAdicionado,
					@UnidadeCampoObrigatorio = Mensagem.FiscalizacaoConfiguracao.CampoUnidadeObrigatorio,
					@TipoCampoObrigatorio = Mensagem.FiscalizacaoConfiguracao.CampoTipoObrigatorio,
					@EditarItemDesativado = Mensagem.FiscalizacaoConfiguracao.EditarCampoInfracaoDesativado
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@TipoCampoTexto = eTipoCampo.Texto,
					@TipoCampoNumerico = eTipoCampo.Numerico,
				});
			}
		}

		public CampoInfracaoVM(List<Item> itens, List<Lista> tipos, List<Lista> unidades)
		{
			Entidade.Itens = itens;
			Tipos = ViewModelHelper.CriarSelectList(tipos, true, true);
			Unidades = ViewModelHelper.CriarSelectList(unidades, true, true);
		}

	}
}