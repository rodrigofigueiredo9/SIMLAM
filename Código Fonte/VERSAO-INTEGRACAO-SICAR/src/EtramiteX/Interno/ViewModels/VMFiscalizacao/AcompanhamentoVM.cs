using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMFiscalizacao
{
	public class AcompanhamentoVM
	{
		public Boolean IsVisualizar { get; set; }

		private Acompanhamento _acompanhamento = new Acompanhamento();
		public Acompanhamento Acompanhamento
		{
			get { return _acompanhamento; }
			set { _acompanhamento = value; }
		}

		private List<SelectListItem> _setoresLst = new List<SelectListItem>();
		public List<SelectListItem> SetoresLst
		{
			get { return _setoresLst; }
			set { _setoresLst = value; }
		}

		private List<ReservaLegalLst> _reservalegalTipoLst = new List<ReservaLegalLst>();
		public List<ReservaLegalLst> ReservaLegalTipoLst
		{
			get { return _reservalegalTipoLst; }
			set { _reservalegalTipoLst = value; }
		}

		private List<CaracteristicaSoloAreaDanificada> _caracteristicasSoloLst = new List<CaracteristicaSoloAreaDanificada>();
		public List<CaracteristicaSoloAreaDanificada> CaracteristicasSoloLst
		{
			get { return _caracteristicasSoloLst; }
			set { _caracteristicasSoloLst = value; }
		}

		private List<SelectListItem> _resultouErosao = new List<SelectListItem>();
		public List<SelectListItem> ResultouErosao
		{
			get { return _resultouErosao; }
			set { _resultouErosao = value; }
		}

		private ArquivoVM _arquivoVM = new ArquivoVM();
		public ArquivoVM ArquivoVM
		{
			get { return _arquivoVM; }
			set { _arquivoVM = value; }
		}

		private FiscalizacaoAssinanteVM _assinantesVM = new FiscalizacaoAssinanteVM();
		public FiscalizacaoAssinanteVM AssinantesVM
		{
			get { return _assinantesVM; }
			set { _assinantesVM = value; }
		}

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ArquivoObrigatorio = Mensagem.Acompanhamento.ArquivoObrigatorio,
					@ArquivoNaoEhPdf = Mensagem.Acompanhamento.ArquivoNaoEhPdf,
					@AssinanteJaAdicionado = Mensagem.Acompanhamento.AssinanteJaAdicionado,
					@AssinanteObrigatorio = Mensagem.Acompanhamento.AssinanteObrigatorio,
					@AssinanteSetorObrigatorio = Mensagem.Acompanhamento.AssinanteSetorObrigatorio,
					@AssinanteFuncionarioObrigatorio = Mensagem.Acompanhamento.AssinanteFuncionarioObrigatorio,
					@AssinanteCargoObrigatorio = Mensagem.Acompanhamento.AssinanteCargoObrigatorio
				});
			}
		}

		public String IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new{});
			}
		}

		public String ArquivoJSon { get; set; }
		public String TiposArquivoValido { get { return ViewModelHelper.Json(new ArrayList { ".pdf" }); } }

		public AcompanhamentoVM(){}

		public AcompanhamentoVM(Acompanhamento acompanhamento, List<Setor> setoresLst, List<ReservaLegalLst> reservalegalTipoLst, List<CaracteristicaSoloAreaDanificada> caracteristicasSolo, bool isVisualizar = false)
		{
			Acompanhamento = acompanhamento;
			IsVisualizar = isVisualizar;
			ReservaLegalTipoLst = reservalegalTipoLst;
			SetoresLst = ViewModelHelper.CriarSelectList(setoresLst, true, true, selecionado: acompanhamento.SetorId.ToString());
			CaracteristicasSoloLst = caracteristicasSolo;

			List<Lista> resultouErosaoLst = new List<Lista>();
			resultouErosaoLst.Add(new Lista() { Id = "1", Texto = "Sim", IsAtivo = true });
			resultouErosaoLst.Add(new Lista() { Id = "2", Texto = "Não", IsAtivo = true });

			ResultouErosao = ViewModelHelper.CriarSelectList(resultouErosaoLst, true, true, selecionado: acompanhamento.InfracaoResultouErosao.ToString());

			AssinantesVM = new FiscalizacaoAssinanteVM();
			AssinantesVM.IsVisualizar = isVisualizar;
			AssinantesVM.Setores = ViewModelHelper.CriarSelectList(setoresLst);
			AssinantesVM.Assinantes = Acompanhamento.Assinantes;
			ArquivoVM.Anexos = acompanhamento.Anexos;
			ArquivoVM.IsVisualizar = isVisualizar;
		}
	}
}