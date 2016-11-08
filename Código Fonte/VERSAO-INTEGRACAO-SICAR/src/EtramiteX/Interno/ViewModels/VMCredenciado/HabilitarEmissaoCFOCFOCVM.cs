using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMCredenciado
{
	public class HabilitarEmissaoCFOCFOCVM
	{
		public HabilitarEmissaoCFOCFOC HabilitarEmissao { get; set; }

		//Tela
		public Boolean IsVisualizar { get; set; }
		public Boolean IsEditar { get; set; }
		public Boolean IsAjaxRequest { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> SituacaoMotivos { get; set; }
		public List<SelectListItem> Estados { get; set; }
		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					CpfObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.CpfObrigatorio,
					CpfInvalido = Mensagem.HabilitarEmissaoCFOCFOC.CpfInvalido,
					CpfNaoCadastrado = Mensagem.HabilitarEmissaoCFOCFOC.CpfNaoCadastrado,
					PragaJaAdicionada = Mensagem.HabilitarEmissaoCFOCFOC.PragaJaAdicionada,
					PragaObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.PragaObrigatorio,
					PragaNomeObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.PragaNomeObrigatorio,
					ResponsavelObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.ResponsavelObrigatorio,
					ArquivoObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.ArquivoObrigatorio,
					ArquivoNaoImagem = Mensagem.HabilitarEmissaoCFOCFOC.ArquivoNaoImagem,
					NumeroHabilitacaoObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.NumeroHabilitacaoObrigatorio,
					NumeroDuaObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.NumeroDuaObrigatorio,
					ValidadeRegistroObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.ValidadeRegistroObrigatorio,
					NumeroHabilitacaoOrigemObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.NumeroHabilitacaoOrigemObrigatorio,
					UFObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.UFObrigatorio,
					RegistroCreaObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.RegistroCreaObrigatorio,
					DataInicialHabilitacaoObrigatoria = Mensagem.HabilitarEmissaoCFOCFOC.DataInicialHabilitacaoObrigatoria,
					DataFinalHabilitacaoObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.DataFinalHabilitacaoObrigatorio,
					NumeroVistoCreaObrigatorio = Mensagem.HabilitarEmissaoCFOCFOC.NumeroVistoCreaObrigatorio
				});
			}
		}

		public String ObterJSon(Object objeto)
		{
			return ViewModelHelper.JsSerializer.Serialize(objeto);
		}

		public HabilitarEmissaoCFOCFOCVM(List<Situacao> situacoes, List<Estado> estados, List<Lista> motivos)
		{
			SituacaoMotivos = ViewModelHelper.CriarSelectList(motivos, true);
			Situacoes = ViewModelHelper.CriarSelectList(situacoes, true);
			Estados = ViewModelHelper.CriarSelectList(estados, itemTextoPadrao: false);
			HabilitarEmissao = new HabilitarEmissaoCFOCFOC();
			IsVisualizar = false;
			IsEditar = false;
			IsAjaxRequest = false;
		}
		public HabilitarEmissaoCFOCFOCVM()
		{
			Situacoes = new List<SelectListItem>();
			Estados = new List<SelectListItem>();
			HabilitarEmissao = new HabilitarEmissaoCFOCFOC();
			IsVisualizar = false;
			IsEditar = false;
			IsAjaxRequest = false;
		}
	}
}