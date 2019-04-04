using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;

namespace Tecnomapas.EtramiteX.Interno.ViewModels.VMPTV
{
	public class PTVVM
	{
		public PTV PTV { get; set; }
		public List<TratamentoFitossanitario> LsTratamentoFitossanitario { get; set; }
		public List<LaudoLaboratorial> LsLaudoLaboratorial { get; set; }
		public bool IsVisualizar { get; set; }
		public List<SelectListItem> Empreendimentos { get; set; }
		public List<SelectListItem> ResponsavelList { get; set; }
		public List<SelectListItem> OrigemTipoList { get; set; }
		private List<SelectListItem> _estados = new List<SelectListItem>();
		public List<SelectListItem> Estados { get; set; }
		private List<SelectListItem> _municipios = new List<SelectListItem>();
		public List<SelectListItem> Municipios { get; set; }
		public List<SelectListItem> MunicipioLaboratorio { get; set; }
		public List<SelectListItem> Situacoes { get; set; }
		public List<SelectListItem> LstUnidades { get; set; }
		public List<SelectListItem> LsTipoTransporte { get; set; }
		public List<SelectListItem> LsResponsavelTecnico { get; set; }
		public List<SelectListItem> LstLaboratorio { get; set; }
		public List<SelectListItem> LstCultura { get; set; }
		public string IdsTela
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@tipoNumeroBloco = eDocumentoFitossanitarioTipoNumero.Bloco,
					@tipoNumeroDigital = eDocumentoFitossanitarioTipoNumero.Digital
				});
			}
		}
		public string IdsOrigem
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@origemCFO = eDocumentoFitossanitarioTipo.CFO,
					@origemCFOC = eDocumentoFitossanitarioTipo.CFOC,
					@origemPTV = eDocumentoFitossanitarioTipo.PTV,
					@origemPTVOutroEstado = eDocumentoFitossanitarioTipo.PTVOutroEstado,
					@origemCFCFR = eDocumentoFitossanitarioTipo.CFCFR,
					@origemTF = eDocumentoFitossanitarioTipo.TF
				});
			}
		}
		public string DataAtual { get { return DateTime.Today.ToShortDateString(); } }

		public List<SelectListItem> lsLocalEmissao { get; set; }

		public List<SelectListItem> lsLocalVistoria { get; set; }

		public List<SelectListItem> lsDiaHoraVistoria { get; set; }

		private List<Acao> _acoesAlterar = new List<Acao>();
		public List<Acao> AcoesAlterar
		{
			get { return _acoesAlterar; }
			set { _acoesAlterar = value; }
		}

		private List<SelectListItem> _estadosUF = new List<SelectListItem>();
		public List<SelectListItem> EstadosUF
		{
			get { return _estadosUF; }
			set { _estadosUF = value; }
		}

		private List<SelectListItem> _municipiosOT = new List<SelectListItem>();
		public List<SelectListItem> MunicipiosOT
		{
			get { return _municipiosOT; }
			set { _municipiosOT = value; }
		}

		public string IdsTelaAnalisar
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					Aprovado = eSolicitarPTVSituacao.Valido,
					Rejeitado = eSolicitarPTVSituacao.Rejeitado,
					AgendarFiscalizacao = eSolicitarPTVSituacao.AgendarFiscalizacao,
					Bloqueado = eSolicitarPTVSituacao.Bloqueado
				});
			}
		}

		public string Mensagens
		{
			get
			{
				return ViewModelHelper.Json(new
				{
					@NotaFiscalDeCaixaNumeroVazio = Mensagem.PTV.NotaFiscalDeCaixaNumeroVazio,
					@NumeroDeCaixasMaiorQueSaldoAtual = Mensagem.PTV.NumeroDeCaixasMaiorQueSaldoAtual,
					@SaldoInicialMaiorQueZero = Mensagem.PTV.SaldoInicialMaiorQueZero,
					@SaldoENumeroCaixasRequerid = Mensagem.PTV.SaldoENumeroCaixasRequerid,
					@InserirGridCaixaNumerosNFIguais = Mensagem.PTV.InserirGridCaixaNumerosNFIguais,
					@NumeroDocumentoDeOrigemObrigatório = Mensagem.PTV.NumeroDocumentoDeOrigemObrigatório,
					@NumeroDocumentoDeOrigemTipoNumerico = Mensagem.PTV.NumeroDocumentoDeOrigemTipoNumerico,
					@HoraInvalida = Mensagem.PTV.HoraInvalida,
				});
			}
		}

		public PTVVM() { }

		public PTVVM(PTV ptv, List<Lista> lstSituacoes, List<ListaValor> lsResponsavel, List<Lista> lsOrigem, List<TratamentoFitossanitario> lsFitossanitario,
			List<LaudoLaboratorial> lstLaboratorio, List<Lista> lstCultura, List<Lista> lsTipoTransporte, List<Municipio> lsLocalEmissao, List<ListaValor> LsSetor, bool isVisualizar = false)
		{
			this.PTV = ptv ?? new PTV();
			this.IsVisualizar = isVisualizar;

			if (lsResponsavel.Count == 1)
			{
				this.PTV.ResponsavelEmpreendimento = lsResponsavel.First().Id;
			}

			this.Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes, true, true, ptv.Situacao.ToString());

			ResponsavelList = ViewModelHelper.CriarSelectList(lsResponsavel, true, true, ptv.ResponsavelEmpreendimento.ToString());

			OrigemTipoList = ViewModelHelper.CriarSelectList(lsOrigem.OrderBy(x => x.Id).ToList(), true, true);

			LsTratamentoFitossanitario = lsFitossanitario ?? new List<TratamentoFitossanitario>();

			this.LstCultura = ViewModelHelper.CriarSelectList(lstCultura);

			this.LsTipoTransporte = ViewModelHelper.CriarSelectList(lsTipoTransporte, true, true,  ptv.TransporteTipo.ToString());

			LsLaudoLaboratorial = lstLaboratorio ?? new List<LaudoLaboratorial>();

			if (LsSetor.Count == 1 && ptv.Id >= 0)
			{
				LsSetor.ForEach(x =>
				{
					ptv.LocalEmissaoId = x.Id;
				});
			}

			this.lsLocalEmissao = ViewModelHelper.CriarSelectList(lsLocalEmissao, true, true, ptv.LocalEmissaoId.ToString());
		}

		public PTVVM(PTV ptv, List<Lista> lstSituacoes, List<ListaValor> lsResponsavel, List<Lista> lsOrigem, List<TratamentoFitossanitario> lsFitossanitario,
			List<LaudoLaboratorial> lstLaboratorio, List<Lista> lstCultura, List<Lista> lsTipoTransporte, List<Municipio> lsLocalEmissao, List<Setor> LsSetor, bool isVisualizar = false, List<ListaValor> lsDiaHoraVistoria = null)
		{
			this.PTV = ptv ?? new PTV();
			this.IsVisualizar = isVisualizar;

			if (lsResponsavel.Count == 1)
			{
				this.PTV.ResponsavelEmpreendimento = lsResponsavel.First().Id;
			}

			this.Situacoes = ViewModelHelper.CriarSelectList(lstSituacoes, true, true, ptv.Situacao.ToString());

			ResponsavelList = ViewModelHelper.CriarSelectList(lsResponsavel, true, true, ptv.ResponsavelEmpreendimento.ToString());

			OrigemTipoList = ViewModelHelper.CriarSelectList(lsOrigem.OrderBy(x => x.Id).ToList(), true, true);

			LsTratamentoFitossanitario = lsFitossanitario ?? new List<TratamentoFitossanitario>();

			this.LstCultura = ViewModelHelper.CriarSelectList(lstCultura);

			this.LsTipoTransporte = ViewModelHelper.CriarSelectList(lsTipoTransporte, true, true, ptv.TransporteTipo.ToString());

			LsLaudoLaboratorial = lstLaboratorio ?? new List<LaudoLaboratorial>();

			if (LsSetor.Count == 1 && ptv.Id >= 0)
			{
				LsSetor.ForEach(x =>
				{
					ptv.LocalEmissaoId = x.Id;
				});
			}

			this.lsLocalEmissao = ViewModelHelper.CriarSelectList(lsLocalEmissao, true, true, ptv.LocalEmissaoId.ToString());

			this.lsLocalVistoria = ViewModelHelper.CriarSelectList(LsSetor, true, true, ptv.LocalVistoriaId.ToString());

			this.lsDiaHoraVistoria = ViewModelHelper.CriarSelectList(lsDiaHoraVistoria, false, false, ptv.DataHoraVistoriaId.ToString());
		}

		public List<Acao> SetarAcoesTela(List<Acao> acoes)
		{
			//Habilitar Radio
			acoes.SingleOrDefault(x => x.Id == (int)eSolicitarPTVSituacao.Valido).Habilitado = true;// (PTV.Situacao == (int)eSolicitarPTVSituacao.AguardandoAnalise);
			acoes.SingleOrDefault(x => x.Id == (int)eSolicitarPTVSituacao.Rejeitado).Habilitado = true;// (PTV.Situacao == (int)eSolicitarPTVSituacao.AguardandoAnalise);
			acoes.SingleOrDefault(x => x.Id == (int)eSolicitarPTVSituacao.AgendarFiscalizacao).Habilitado = (PTV.Situacao != (int)eSolicitarPTVSituacao.Bloqueado); //true;
			acoes.SingleOrDefault(x => x.Id == (int)eSolicitarPTVSituacao.Bloqueado).Habilitado = (PTV.Situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao);// true;

			acoes.SingleOrDefault(x => x.Id == (int)eSolicitarPTVSituacao.Rejeitado).Texto = "Rejeitado";
			acoes.SingleOrDefault(x => x.Id == (int)eSolicitarPTVSituacao.Bloqueado).Texto = "Bloqueado";
			acoes.SingleOrDefault(x => x.Id == (int)eSolicitarPTVSituacao.AgendarFiscalizacao).Texto = "Agendar Fiscalização";

			return acoes;
		}
	}
}