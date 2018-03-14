using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao.VMConfiguracoes
{
	public class ParametrizacaoVM
	{
		public Boolean IsVisualizar { get; set; }

		private Parametrizacao _entidade = new Parametrizacao();
		public Parametrizacao Entidade
		{
			get { return _entidade; }
			set { _entidade = value; }
		}

		private List<SelectListItem> _codigoReceita = new List<SelectListItem>();
		public List<SelectListItem> CodigoReceita
		{
			get { return _codigoReceita; }
			set { _codigoReceita = value; }
		}

		private List<SelectListItem> _decorrencia = new List<SelectListItem>();
		public List<SelectListItem> Decorrencia
		{
			get { return _decorrencia; }
			set { _decorrencia = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@CodigoReceitaObrigatorio = Mensagem.FiscalizacaoConfiguracao.ParCodigoReceitaObrigatorio,
					@InicioVigenciaObrigatorio = Mensagem.FiscalizacaoConfiguracao.InicioVigenciaObrigatorio,
					@MaximoParcelasObrigatorio = Mensagem.FiscalizacaoConfiguracao.MaximoParcelasObrigatorio,
					@ValorMinimoPFObrigatorio = Mensagem.FiscalizacaoConfiguracao.ValorMinimoPFObrigatorio,
					@ValorMinimoPJObrigatorio = Mensagem.FiscalizacaoConfiguracao.ValorMinimoPJObrigatorio

				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					/*@TipoParametrizacaoTexto = eTipoParametrizacao.Texto,
					@TipoParametrizacaoNumerico = eTipoParametrizacao.Numerico,*/
				});
			}
		}

		public ParametrizacaoVM(Parametrizacao entidade, List<Lista> codigoReceita, bool isVisualizar = false)
		{
			Entidade = entidade;
			IsVisualizar = isVisualizar;
			Decorrencia = new List<SelectListItem>()
			{
				new SelectListItem(){ Value = ((int)eDecorrencia.Dia).ToString() , Text = "Dia", Selected = entidade.PrazoDescontoDecorrencia == (int)eDecorrencia.Dia },
				new SelectListItem(){ Value = ((int)eDecorrencia.Mes).ToString(), Text = "Mês", Selected = entidade.PrazoDescontoDecorrencia == (int)eDecorrencia.Mes },
				new SelectListItem(){ Value = ((int)eDecorrencia.Ano).ToString(), Text = "Ano", Selected = entidade.PrazoDescontoDecorrencia == (int)eDecorrencia.Ano }
			};
			var listaCodigoReceita = ViewModelHelper.CriarSelectList(codigoReceita, true, true);
			var item = listaCodigoReceita.Find(x => x.Value == entidade.CodigoReceitaId.ToString());
			if(item != null)
				item.Selected = true;
			CodigoReceita = listaCodigoReceita;
		}
	}
}