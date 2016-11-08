using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMTitulo
{
	public class TituloModeloVM
	{
		public int ModeloId { get; set; }

		private List<SelectListItem> _tipos = new List<SelectListItem>();
		public List<SelectListItem> Tipos { get { return _tipos; } set { _tipos = value; } }

		private List<SelectListItem> _tiposProtocolos = new List<SelectListItem>();
		public List<SelectListItem> TiposProtocolos { get { return _tiposProtocolos; } set { _tiposProtocolos = value; } }

		private List<SelectListItem> _periodosRenovacao = new List<SelectListItem>();
		public List<SelectListItem> PeriodosRenovacao { get { return _periodosRenovacao; } set { _periodosRenovacao = value; } }

		private List<SelectListItem> _iniciosPrazo = new List<SelectListItem>();
		public List<SelectListItem> IniciosPrazo { get { return _iniciosPrazo; } set { _iniciosPrazo = value; } }

		private List<SelectListItem> _tiposPrazo = new List<SelectListItem>();
		public List<SelectListItem> TiposPrazo { get { return _tiposPrazo; } set { _tiposPrazo = value; } }

		private List<SelectListItem> _setores = new List<SelectListItem>();
		public List<SelectListItem> Setores { get { return _setores; } set { _setores = value; } }

		private List<SelectListItem> _assinantes = new List<SelectListItem>();
		public List<SelectListItem> Assinantes { get { return _assinantes; } set { _assinantes = value; } }

		private List<SelectListItem> _modelos = new List<SelectListItem>();
		public List<SelectListItem> Modelos { get { return _modelos; } set { _modelos = value; } }
        public List<SelectListItem> TiposDocumento { get; set; }

		private TituloModelo _tituloModelo = new TituloModelo();
		public TituloModelo Modelo { get { return _tituloModelo; } set { _tituloModelo = value; } }

		public string ArquivoJSon { get; set; }

		public CampoVM NumeracaoAutomatica { get { return CarregarCampo(eRegra.NumeracaoAutomatica); } }
		public CampoVM NumeracaoReiniciada { get { return CarregarCampo(eRegra.NumeracaoReiniciada); } }
		public CampoVM ProtocoloObrigatorio { get { return CarregarCampo(eRegra.ProtocoloObrigatorio); } }
		public CampoVM Prazo { get { return CarregarCampo(eRegra.Prazo); } }
		public CampoVM Condicionantes { get { return CarregarCampo(eRegra.Condicionantes); } }
		public CampoVM Renovacao { get { return CarregarCampo(eRegra.Renovacao); } }
		public CampoVM EnviarEmail { get { return CarregarCampo(eRegra.EnviarEmail); } }
		public CampoVM PublicoExterno { get { return CarregarCampo(eRegra.PublicoExterno); } }
		public CampoVM AnexarPDFTitulo { get { return CarregarCampo(eRegra.AnexarPDFTitulo); } }
		public CampoVM FaseAnterior { get { return CarregarCampo(eRegra.FaseAnterior); } }
		public CampoVM PdfGeradoSistema { get { return CarregarCampo(eRegra.PdfGeradoSistema); } }
        public CampoVM CredenciadoEmite { get { return CarregarCampo(eRegra.EmitidoCredenciado); } }
        public CampoVM NumeroSincronizado { get { return CarregarCampo(eRegra.SincronizaInstitucional); } }
		public CampoVM InicioNumeracao { get { return CarregarCampo(eRegra.NumeracaoAutomatica, eResposta.InicioNumeracao); } }
		public CampoVM PeriodoRenovacao { get { return CarregarCampo(eRegra.Renovacao, eResposta.InicioPeriodoRenovacao); } }
		public CampoVM Dias { get { return CarregarCampo(eRegra.Renovacao, eResposta.Dias); } }
		public CampoVM InicioPrazo { get { return CarregarCampo(eRegra.Prazo, eResposta.InicioPrazo); } }
		public CampoVM TipoPrazo { get { return CarregarCampo(eRegra.Prazo, eResposta.TipoPrazo); } }
		public CampoVM TextoEmail { get { return CarregarCampo(eRegra.EnviarEmail, eResposta.TextoEmail); } }
		public CampoVM TituloAnteriroObrigatorio { get { return CarregarCampo(eRegra.FaseAnterior, eResposta.TituloAnteriorObrigatorio); } }

		public String TiposArquivoValido = ViewModelHelper.Json(new ArrayList { ".doc", "docx" });

		public String Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@ExisteSetor = Mensagem.TituloModelo.SetorExistente,
					@ExisteAssinante = Mensagem.TituloModelo.AssinanteExiste,
					@ExisteModeloTitulo = Mensagem.TituloModelo.ModeloTituloExiste,
					@SelecioneModelo = Mensagem.TituloModelo.SelecioneModelo,
					@SelecioneSetor = Mensagem.TituloModelo.SelecioneSetor,
					@SelecioneSetorAssinante = Mensagem.TituloModelo.SelecioneSetorAssinante,
					@ArquivoNaoEhDoc = Mensagem.TituloModelo.TipoArquivoDoc,
					@ArquivoObrigatorio = Mensagem.TituloModelo.ArquivoObrigatorio
				});
			}
		}

		public TituloModeloVM() { }

        public void SetListItens(List<TituloModeloTipo> tipos = null, List<TituloModeloProtocoloTipo> protocoloTipos = null, List<TituloModeloPeriodoRenovacao> periodoRenovacao = null, List<TituloModeloInicioPrazo> inicioPrazo = null, List<TituloModeloTipoPrazo> tipoPrazo = null, List<TituloModeloLst> modelos = null, List<TituloModeloAssinante> assinantes = null, List<TituloModeloTipoDocumento> tipoDocumento = null)
		{
			SetModelos(modelos);
			SetTipoPrazo(tipoPrazo);
			SetInicioPrazo(inicioPrazo);
			SetPeriodoRenovacao(periodoRenovacao);
			SetProtocoloTipo(protocoloTipos);
			SetAssinantes(assinantes);
			SetTipos(tipos);
            SetTiposDocumento(tipoDocumento);
		}

		public void SetSetores(List<Setor> setores)
		{
			if (setores != null)
			{
				Setores = ViewModelHelper.CriarSelectList(setores, true);
			}
		}

		public void SetModelos(List<TituloModeloLst> modelos)
		{
			if (modelos != null)
			{
				Modelos = ViewModelHelper.CriarSelectList(modelos, true);
			}
		}

		public void SetAssinantes(List<TituloModeloAssinante> assinantes)
		{
			if (assinantes != null)
			{
				Assinantes = ViewModelHelper.CriarSelectList(assinantes, true);
			}
		}

		public void SetTipos(List<TituloModeloTipo> tipos)
		{
			if (tipos != null)
			{
				Tipos = ViewModelHelper.CriarSelectList(tipos, true);
			}
		}

		public void SetProtocoloTipo(List<TituloModeloProtocoloTipo> protocoloTipos)
		{
			if (protocoloTipos != null)
			{
				TiposProtocolos = ViewModelHelper.CriarSelectList(protocoloTipos, true);
			}
		}

		public void SetPeriodoRenovacao(List<TituloModeloPeriodoRenovacao> periodoRenovacao)
		{
			if (periodoRenovacao != null)
			{
				PeriodosRenovacao = ViewModelHelper.CriarSelectList(periodoRenovacao, true);
			}
		}

		public void SetInicioPrazo(List<TituloModeloInicioPrazo> inicioPrazo)
		{
			if (inicioPrazo != null)
			{
				IniciosPrazo = ViewModelHelper.CriarSelectList(inicioPrazo, true);
			}
		}

		public void SetTipoPrazo(List<TituloModeloTipoPrazo> tipoPrazo)
		{
			if (tipoPrazo != null)
			{
				TiposPrazo = ViewModelHelper.CriarSelectList(tipoPrazo, true);
			}
		}

        public void SetTiposDocumento(List<TituloModeloTipoDocumento> tipoDocumento)
        {
            if (tipoDocumento != null)
            {
                TiposDocumento = ViewModelHelper.CriarSelectList(tipoDocumento, true);
            }
        }

		private CampoVM CarregarCampo(eRegra tipo)
		{
			CampoVM campo = new CampoVM();

			campo.Valor = (Modelo.Regras.SingleOrDefault(x => x.TipoEnum == tipo) ?? new TituloModeloRegra()).Valor;
			campo.Id = (Modelo.Regras.SingleOrDefault(x => x.TipoEnum == tipo) ?? new TituloModeloRegra()).Id;

			return campo;
		}

		private CampoVM CarregarCampo(eRegra regra, eResposta resposta)
		{
			CampoVM campo = new CampoVM();

			TituloModeloRegra tituloRegra = Modelo.Regras.SingleOrDefault(x => x.TipoEnum == regra);

			if (tituloRegra == null)
				return campo;

			campo.Valor = (tituloRegra.Respostas.SingleOrDefault(x => x.TipoEnum == resposta) ?? new TituloModeloResposta()).Valor;
			campo.Id = (tituloRegra.Respostas.SingleOrDefault(x => x.TipoEnum == resposta) ?? new TituloModeloResposta()).Id;

			return campo;
		}
	}
}