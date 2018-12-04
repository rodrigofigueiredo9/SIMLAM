using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Areas.Caracterizacoes.ViewModels
{
	public class InformacaoCorteVM
	{

		#region Constructores

		public InformacaoCorteVM() { }

		#endregion

		#region Properties

		public long EmpreendimentoId { get; set; }
		public int? EmpreendimentoCodigo { get; set; }

		public string DenominadorTexto { get; set; }
		public string DenominadorValor { get; set; }
		public string EmpreendimentoCNPJ { get; set; }

		public long ProjetoDigitalId { get; set; }
		public bool IsVisualizar { get; set; }
		public bool IsPodeExcluir { get; set; }

		public string NumeroLicena { get; set; }
		public string Atividade { get; set; }
		public decimal AreaLicenciada { get; set; }
		public DateTime? DataVencimento { get; set; }
		public Paginacao Paginacao { get; set; } = new Paginacao();

		public List<Titulo> Resultados { get; set; } = new List<Titulo>();
		public List<SelectListItem> EmpreendimentoUf { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> EmpreendimentoZonaLocalizacao { get; set; } = new List<SelectListItem>();
		public List<SelectListItem> EmpreendimentoMunicipio { get; set; } = new List<SelectListItem>();

		#endregion

		#region Methods

		public void CarregarEmpreendimento(EmpreendimentoCaracterizacao empreendimento)
		{
			this.EmpreendimentoId = empreendimento.Id;
			this.EmpreendimentoCodigo = empreendimento.Codigo;
			this.EmpreendimentoCNPJ = empreendimento.CNPJ;

			this.DenominadorValor = empreendimento.Denominador;
			this.DenominadorTexto = empreendimento.DenominadorTipo;

			var municipio = new ListaValor() { Id = 1, Texto = empreendimento.Municipio, IsAtivo = true };
			var uf = new ListaValor() { Id = 1, Texto = empreendimento.Uf, IsAtivo = true };
			var zonalocalizacao = new ListaValor() { Id = 1, Texto = empreendimento.ZonaLocalizacaoTexto, IsAtivo = true };

			this.EmpreendimentoMunicipio = ViewModelHelper.CriarSelectList(new List<ListaValor>() { municipio }, true, selecionado: "1");
			this.EmpreendimentoUf = ViewModelHelper.CriarSelectList(new List<ListaValor>() { uf }, true, selecionado: "1");
			this.EmpreendimentoZonaLocalizacao = ViewModelHelper.CriarSelectList(new List<ListaValor>() { zonalocalizacao }, true, selecionado: "1");
		}

		public String ObterJSon(Titulo titulo)
		{
			object objeto = new
			{
				@Id = titulo.Id,
				@Tid = titulo.Tid,
				@Numero = titulo.Numero.Texto,
				@Modelo = titulo.Modelo.Nome,
				@ModeloTipo = titulo.Modelo.Tipo,
				@ModeloSigla = titulo.Modelo.Sigla
			};

			return HttpUtility.HtmlEncode(ViewModelHelper.JsSerializer.Serialize(objeto));
		}

		#endregion

	}
}