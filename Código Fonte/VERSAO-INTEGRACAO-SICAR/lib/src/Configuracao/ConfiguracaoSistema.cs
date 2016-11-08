using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Data;

namespace Tecnomapas.EtramiteX.Configuracao
{
	public class ConfiguracaoSistema : ConfiguracaoBase
	{
		public const int NAO = 0;
		public const int SIM = 1;
		public const int Dispensado = 2;

		public const int EmitidoIDAF = 1;
		public const int EmitidoOutroOrgao = 2;

		private ListaValoresDa _daLista = new ListaValoresDa();

		public const String KeyLstQuantPaginacao = "LstQuantPaginacao";
		public List<QuantPaginacao> LstQuantPaginacao { get { return _daLista.ObterLstQuantPaginacao(); } }

		public const String KeyUsuarioInterno = "UsuarioInterno";
		public String UsuarioInterno { get { return _daLista.BuscarConfiguracaoSistema("UsuarioInterno"); } }

		public const String KeyUsuarioConsulta = "UsuarioConsulta";
		public String UsuarioConsulta { get { return _daLista.BuscarConfiguracaoSistema("UsuarioConsulta"); } }

		public const String KeyUsuarioCredenciado = "UsuarioCredenciado";
		public String UsuarioCredenciado { get { return _daLista.BuscarConfiguracaoSistema("UsuarioCredenciado"); } }

		public const String KeyUsuarioRelatorio = "UsuarioRelatorio";
		public String UsuarioRelatorio { get { return _daLista.BuscarConfiguracaoSistema("UsuarioRelatorio"); } }

		public const String KeyUsuarioGeo = "UsuarioGeo";
		public String UsuarioGeo { get { return _daLista.BuscarConfiguracaoSistema("UsuarioGeo"); } }

		public const String KeyUsuarioCredenciadoGeo = "UsuarioCredenciadoGeo";
		public String UsuarioCredenciadoGeo { get { return _daLista.BuscarConfiguracaoSistema("UsuarioCredenciadoGeo"); } }

		public const String KeyUsuarioPublicoGeo = "UsuarioPublicoGeo";
		public String UsuarioPublicoGeo { get { return _daLista.BuscarConfiguracaoSistema("UsuarioPublicoGeo"); } }

		public const String KeyCredenciadoLinkAcesso = "CredenciadoLinkAcesso";
		public String CredenciadoLinkAcesso { get { return _daLista.BuscarConfiguracaoSistema("CredenciadoLinkAcesso"); } }

		#region Localidade
		public const String KeyEstadoDefault = "EstadoDefault";
		public String EstadoDefault { get { return _daLista.BuscarConfiguracaoSistema("EstadoDefault"); } }

		public const String KeyMunicipioDefault = "MunicipioDefault";
		public String MunicipioDefault { get { return _daLista.BuscarConfiguracaoSistema("MunicipioDefault"); } }
		#endregion

		public const String KeyModeloTextoEmailDefault = "ModeloTextoEmailDefault";
		public String ModeloTextoEmailDefault { get { return _daLista.BuscarConfiguracaoSistema("ModeloTextoEmailDefault"); } }

		public const String KeyModeloTextoEmailComunicadorSolicitacaoPTV = "ModeloTextoEmailComunicadorSolicitacaoPTV";
		public String ModeloTextoEmailComunicadorSolicitacaoPTV { get { return _daLista.BuscarConfiguracaoSistema("ModeloTextoEmailComunicadorSolicitacaoPTV"); } }

		public const String KeyModeloTextoEmailAgendarFiscalizacaoSolicitacaoPTV = "ModeloTextoEmailAgendarFiscalizacaoSolicitacaoPTV";
		public String ModeloTextoEmailAgendarFiscalizacaoSolicitacaoPTV { get { return _daLista.BuscarConfiguracaoSistema("ModeloTextoEmailAgendarFiscalizacaoSolicitacaoPTV"); } }

		public const String KeyModeloTextoEmailAprovarSolicitacaoPTV = "ModeloTextoEmailAprovarSolicitacaoPTV";
		public String ModeloTextoEmailAprovarSolicitacaoPTV { get { return _daLista.BuscarConfiguracaoSistema("ModeloTextoEmailAprovarSolicitacaoPTV"); } }

		public const String KeyModeloTextoEmailRejeitarSolicitacaoPTV = "ModeloTextoEmailRejeitarSolicitacaoPTV";
		public String ModeloTextoEmailRejeitarSolicitacaoPTV { get { return _daLista.BuscarConfiguracaoSistema("ModeloTextoEmailRejeitarSolicitacaoPTV"); } }


		public const String KeyGovernoNome = "GovernoNome";
		public String GovernoNome { get { return _daLista.BuscarConfiguracaoSistema("orgaogoverno").ToUpper(); } }

		public const String KeySecretariaNome = "SecretariaNome";
		public String SecretariaNome { get { return _daLista.BuscarConfiguracaoSistema("orgaosecretaria").ToUpper(); } }

		public const String KeySecretariaSigla = "SecretariaSigla";
		public String SecretariaSigla { get { return _daLista.BuscarConfiguracaoSistema("orgaosecretariasigla").ToUpper(); } }

		#region EnderecoOrgao
		public const String KeyOrgaoNome = "OrgaoNome";
		public String OrgaoNome { get { return _daLista.BuscarConfiguracaoSistema("OrgaoNome"); } }

		public const String KeyOrgaoSigla = "OrgaoSigla";
		public String OrgaoSigla { get { return _daLista.BuscarConfiguracaoSistema("OrgaoSigla"); } }

		public const String KeyOrgaoEndereco = "OrgaoEndereco";
		public String OrgaoEndereco { get { return _daLista.BuscarConfiguracaoSistema("OrgaoEndereco"); } }

		public const String KeyOrgaoMunicipio = "OrgaoMunicipio";
		public String OrgaoMunicipio { get { return _daLista.BuscarConfiguracaoSistema("orgaomunicipio"); } }

		public const String KeyOrgaoUf = "OrgaoUf";
		public String OrgaoUf { get { return _daLista.BuscarConfiguracaoSistema("orgaouf"); } }

		public const String KeyOrgaoCep = "OrgaoCep";
		public String OrgaoCep { get { return _daLista.BuscarConfiguracaoSistema("orgaocep"); } }

		public const String KeyOrgaoTelefone = "OrgaoTelefone";
		public String OrgaoTelefone { get { return _daLista.BuscarConfiguracaoSistema("OrgaoTelefone"); } }

		public const String KeyOrgaoTelefoneFax = "OrgaoTelefoneFax";
		public String OrgaoTelefoneFax { get { return _daLista.BuscarConfiguracaoSistema("orgaotelefonefax"); } }

		public const String KeyOrgaoSite = "OrgaoSite";
		public String OrgaoSite { get { return _daLista.BuscarConfiguracaoSistema("orgaosite"); } }
		#endregion

		public const string KeyMeses = "Meses";
		public List<string> Meses
		{
			get
			{
				return CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Select(x => x.ToLower()).ToList();
			}
		}

		public const String KeyRoteiroPadrao = "RoteiroPadrao";
		public List<String> RoteiroPadrao
		{
			get
			{
				return _daLista.BuscarConfiguracaoSistema("roteiropadrao").Split(',').ToList() ?? new List<String>();
			}
		}

		public const String KeyDiretorioOrtoFotoMosaico = "DiretorioOrtoFotoMosaico";
		public Dictionary<Int32, String> DiretorioOrtoFotoMosaico
		{
			get
			{
				Dictionary<Int32, String> diretorios = new Dictionary<int, string>();
				diretorios.Add(1, _daLista.BuscarConfiguracaoSistema("diretorioortofoto"));
				return diretorios;
			}
		}

		public const string KeyFinalidades = "Finalidades";
		public List<Finalidade> Finalidades { get { return _daLista.ObterFinalidades(); } }

		public const string KeyFinalidadesExploracao = "FinalidadesExploracao";
		public List<Finalidade> FinalidadesExploracao { get { return _daLista.ObterFinalidadesExploracao(); } }

		public const String KeyUrlGeoBasesWebServices = "UrlGeoBasesWebServices";
		public String UrlGeoBasesWebServices { get { return _daLista.BuscarConfiguracaoSistema("geobaseswebservices"); } }

		public const string KeyMeiosContato = "MeiosContato";
		public List<ContatoLst> MeiosContato { get { return _daLista.ObterMeiosContato(); } }

		public const String KeyGeoBasesWebServicesAutencicacaoChave = "GeoBasesWebServicesAutencicacaoChave";
		public String GeoBasesWebServicesAutencicacaoChave { get { return _daLista.BuscarConfiguracaoSistema("geobaseswebservicesautenticacaocache"); } }

		public const String KeyMotosserraNumeroRegistro = "MotosserraNumeroRegistro";
		public String MotosserraNumeroRegistro { get { return _daLista.BuscarConfiguracaoSistema("motosserranumeroregistro"); } }

		public const String KeySolicitacaoNumeroMaxDiasValido = "SolicitacaoNumeroMaxDiasValido";
		public String SolicitacaoNumeroMaxDiasValido { get { return _daLista.BuscarConfiguracaoSistema("solicitacaonumeromaxdiasvalido"); } }

		public const String KeySolicitacaoMotivoSuspensaoPeriodoProtocolo = "SolicitacaoMotivoSuspensaoPeriodoProtocolo";
		public String SolicitacaoMotivoSuspensaoPeriodoProtocolo { get { return _daLista.BuscarConfiguracaoSistema("solicitacaomotivosuspensaoperiodoprotocolo"); } }

		public const string KeyBooleanLista = "BooleanLista";
		public List<Lista> BooleanLista
		{
			get
			{
				List<Lista> lista = new List<Lista>();
				lista.Add(new Lista() { Id = "-1", Texto = "*** Selecione ***", IsAtivo = true });
				lista.Add(new Lista() { Id = NAO.ToString(), Texto = "Não", IsAtivo = true });
				lista.Add(new Lista() { Id = SIM.ToString(), Texto = "Sim", IsAtivo = true });

				return lista;
			}
		}

		public const string KeyAtivoLista = "AtivoLista";
		public List<Lista> AtivoLista
		{
			get
			{
				List<Lista> lista = new List<Lista>();
				lista.Add(new Lista() { Id = "-1", Texto = "*** Selecione ***", IsAtivo = true });
				lista.Add(new Lista() { Id = SIM.ToString(), Texto = "Ativo", IsAtivo = true });
				lista.Add(new Lista() { Id = NAO.ToString(), Texto = "Inativo", IsAtivo = true });

				return lista;
			}
		}

		public const string KeySistemaOrigem = "SistemaOrigem";
		public List<Lista> SistemaOrigem
		{
			get
			{
				List<Lista> lista = new List<Lista>();
				lista.Add(new Lista() { Id = "1", Texto = "Institucional", IsAtivo = true });
				lista.Add(new Lista() { Id = "2", Texto = "Credenciado", IsAtivo = true });

				return lista;
			}
		}

		public const string LimiteSimilaridade = "0,8";

		public const String KeyAgrotoxicoMaxNumeroCadastro = "AgrotoxicoMaxNumeroCadastro";
		public String AgrotoxicoMaxNumeroCadastro { get { return _daLista.BuscarConfiguracaoSistema("agrotoxico_max_numcadastro"); } }

		public const string KeyDiasSemana = "DiasSemana";
		public List<Lista> DiasSemana { get { return _daLista.ObterDiasSemana(); } }


		public const String KeyDUASenhaCertificado = "DUASenhaCertificado";
		public String DUASenhaCertificado { get { return _daLista.BuscarConfiguracaoSistema("duaSenhaCertificado"); } }


        public const String KeyUrlPDFPublico = "UrlPDFPublico";
        public String UrlPDFPublico { get { return _daLista.BuscarConfiguracaoSistema("url_pfd_publico"); } }

		public const String KeyUrlSICAR = "UrlSICAR";
		public String UrlSICAR { get { return _daLista.BuscarConfiguracaoSistema("url_sicar"); } }

		#region Caracterização

		public const String KeyUnidadeConsolidacaoMaxCodigoUC = "UnidadeConsolidacaoMaxCodigoUC";
		public String UnidadeConsolidacaoMaxCodigoUC { get { return _daLista.BuscarConfiguracaoSistema("crt_unidade_consolidacao_maxcoduc"); } }

		public const String KeyUnidadeProducaoMaxCodigoPropriedade = "UnidadeProducaoMaxCodigoPropriedade";
		public String UnidadeProducaoMaxCodigoPropriedade { get { return _daLista.BuscarConfiguracaoSistema("crt_unidade_producao_maxcodproprie"); } }

		#endregion

        public const String KeyDesenhadorWebserviceURL = "DesenhadorWebserviceURL";
        public String DesenhadorWebserviceURL
        {
            get
            { return _daLista.BuscarConfiguracaoSistema("desenhador_webservice_url"); }
        }

        public const String KeyMunicipioWebserviceURL = "MunicipioWebserviceURL";
        public String MunicipioWebserviceURL
        {
            get { return _daLista.BuscarConfiguracaoSistema("municipio_webservice_url"); }
        }

        public const String KeyGeoprocessamentoWebserviceURL = "GeoprocessamentoWebserviceURL";
        public String GeoprocessamentoWebserviceURL
        {
            get { return _daLista.BuscarConfiguracaoSistema("geoprocessamento_webservice_url"); }
        }

        public const String KeyMapaTematicoURL = "MapaTematicoURL";
        public String MapaTematicoURL
        {
            get { return _daLista.BuscarConfiguracaoSistema("mapa_tematico_url"); }
        }

        public const String KeyAeroLevantamentoMapaImagemURL = "AeroLevantamentoMapaImagemURL";
        public String AeroLevantamentoMapaImagemURL
        {
            get { return _daLista.BuscarConfiguracaoSistema("aero_levantamento_mapa_imagem_url"); }
        }

        public const String KeyDevEmpreendimentoMapaLoteURL = "DevEmpreendimentoMapaLoteURL";
        public String DevEmpreendimentoMapaLoteURL
        {
            get { return _daLista.BuscarConfiguracaoSistema("dev_empreendimento_mapa_lote_url"); }
        }

        public const String KeyFiscalMapaLoteURL = "FiscalMapaLoteURL";
        public String FiscalMapaLoteURL
        {
            get { return _daLista.BuscarConfiguracaoSistema("fiscal_mapa_lote_url");}
        }
    }
}