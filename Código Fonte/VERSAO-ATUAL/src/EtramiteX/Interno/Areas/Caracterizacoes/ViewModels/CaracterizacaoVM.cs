using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Interno.Model.Security;
using Tecnomapas.EtramiteX.Interno.ViewModels;

namespace Tecnomapas.EtramiteX.Interno.Areas.Caracterizacoes.ViewModels
{
	public class CaracterizacaoVM
	{
		public string EmpreendimentoId { get; set; }
        public int? Codigo { get; set; }
		public string DenominadorTexto { get; set; }
		public string DenominadorValor { get; set; }
		public string CNPJ { get; set; }

		private List<CaracterizacaoVME> _caracterizacoesNaoCadastradas = new List<CaracterizacaoVME>();
		public List<CaracterizacaoVME> CaracterizacoesNaoCadastradas { get { return _caracterizacoesNaoCadastradas; } set { _caracterizacoesNaoCadastradas = value; } }

		private List<CaracterizacaoVME> _caracterizacoesCadastradas = new List<CaracterizacaoVME>();
		public List<CaracterizacaoVME> CaracterizacoesCadastradas { get { return _caracterizacoesCadastradas; } set { _caracterizacoesCadastradas = value; } }

		private List<SelectListItem> _uf = new List<SelectListItem>();
		public List<SelectListItem> Uf
		{
			get { return _uf; }
			set { _uf = value; }
		}

		private List<SelectListItem> _municipio = new List<SelectListItem>();
		public List<SelectListItem> Municipio
		{
			get { return _municipio; }
			set { _municipio = value; }
		}

		private List<SelectListItem> _zonaLocalizacao = new List<SelectListItem>();
		public List<SelectListItem> ZonaLocalizacao
		{
			get { return _zonaLocalizacao; }
			set { _zonaLocalizacao = value; }
		}

		public int SelecionarPrimeiroItem
		{
			get { return 1; }
		}

		public CaracterizacaoVM(EmpreendimentoCaracterizacao emp) 
		{
			if (emp != null)
			{
				EmpreendimentoId = emp.Id.ToString();
                Codigo = emp.Codigo;
				CNPJ = emp.CNPJ;
				DenominadorValor = emp.Denominador;
				DenominadorTexto = emp.DenominadorTipo;
				ListaValor municipio = new ListaValor() { Id = 1, Texto = emp.Municipio, IsAtivo = true };
				ListaValor uf = new ListaValor() { Id = 1, Texto = emp.Uf, IsAtivo = true };
				ListaValor zonalocalizacao = new ListaValor() { Id = 1, Texto = emp.ZonaLocalizacaoTexto, IsAtivo = true };

				Municipio = ViewModelHelper.CriarSelectList(new List<ListaValor>() { municipio }, true);
				Uf = ViewModelHelper.CriarSelectList(new List<ListaValor>() { uf }, true);
				ZonaLocalizacao = ViewModelHelper.CriarSelectList(new List<ListaValor>() { zonalocalizacao }, true);
			}
		}

		public static string GerarUrl(int empreendimentoId, bool isCadastrarCaracterizacao, eCaracterizacao tipo)
		{
			String url = "Visualizar";
			PermissaoValidar permissaoValidar = new PermissaoValidar();

			ePermissao ePermCriar = (ePermissao)Enum.Parse(typeof(ePermissao), String.Format("{0}Criar", tipo.ToString()));
			ePermissao ePermEditar = (ePermissao)Enum.Parse(typeof(ePermissao), String.Format("{0}Editar", tipo.ToString()));
			ePermissao ePermVisualizar = (ePermissao)Enum.Parse(typeof(ePermissao), String.Format("{0}Visualizar", tipo.ToString()));


			if (isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermCriar }, false))
			{
				url = "Criar";
			}

			if (!isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermEditar }, false))
			{
				url = "Editar";
			}

			if (!isCadastrarCaracterizacao && permissaoValidar.ValidarAny(new[] { ePermVisualizar }, false) && tipo == eCaracterizacao.ExploracaoFlorestal)
			{
				url = "Visualizar";
			}

			UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);

			return urlHelper.Action(url, tipo.ToString(), new { id = empreendimentoId, Msg = Validacao.QueryParam() });
		}
	}
}