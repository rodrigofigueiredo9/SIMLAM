using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business
{
	public class ListaBus
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoAdministrador> _configAdministrador = new GerenciadorConfiguracao<ConfiguracaoAdministrador>(new ConfiguracaoAdministrador());
		GerenciadorConfiguracao<ConfiguracaoEndereco> _configEndereco = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());
		GerenciadorConfiguracao<ConfiguracaoCoordenada> _configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
		GerenciadorConfiguracao<ConfiguracaoEmpreendimento> _configEmpreendimento = new GerenciadorConfiguracao<ConfiguracaoEmpreendimento>(new ConfiguracaoEmpreendimento());
		GerenciadorConfiguracao<ConfiguracaoCredenciado> _configCredenciado = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());
		GerenciadorConfiguracao<ConfiguracaoPessoa> _configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
		GerenciadorConfiguracao<ConfiguracaoFuncionario> _configFuncionario = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());
		GerenciadorConfiguracao<ConfiguracaoChecagemRoteiro> _configChecagemRoteiro = new GerenciadorConfiguracao<ConfiguracaoChecagemRoteiro>(new ConfiguracaoChecagemRoteiro());
		GerenciadorConfiguracao<ConfiguracaoRequerimento> _configRequerimento = new GerenciadorConfiguracao<ConfiguracaoRequerimento>(new ConfiguracaoRequerimento());
		GerenciadorConfiguracao<ConfiguracaoResponsavel> _configResponsavel = new GerenciadorConfiguracao<ConfiguracaoResponsavel>(new ConfiguracaoResponsavel());
		GerenciadorConfiguracao<ConfiguracaoProcesso> _configProcesso = new GerenciadorConfiguracao<ConfiguracaoProcesso>(new ConfiguracaoProcesso());
		GerenciadorConfiguracao<ConfiguracaoDocumento> _configDocumento = new GerenciadorConfiguracao<ConfiguracaoDocumento>(new ConfiguracaoDocumento());
		GerenciadorConfiguracao<ConfiguracaoRoteiro> _configRoteiro = new GerenciadorConfiguracao<ConfiguracaoRoteiro>(new ConfiguracaoRoteiro());
		GerenciadorConfiguracao<ConfiguracaoAnalise> _configAnalise = new GerenciadorConfiguracao<ConfiguracaoAnalise>(new ConfiguracaoAnalise());
		GerenciadorConfiguracao<ConfiguracaoChecagemPendencia> _configChecagemPendencia = new GerenciadorConfiguracao<ConfiguracaoChecagemPendencia>(new ConfiguracaoChecagemPendencia());
		GerenciadorConfiguracao<ConfiguracaoTramitacao> _configTramitacao = new GerenciadorConfiguracao<ConfiguracaoTramitacao>(new ConfiguracaoTramitacao());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
		GerenciadorConfiguracao<ConfiguracaoTitulo> _configTitulo = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
		GerenciadorConfiguracao<ConfiguracaoEspecificidade> _configEspecificidade = new GerenciadorConfiguracao<ConfiguracaoEspecificidade>(new ConfiguracaoEspecificidade());
		GerenciadorConfiguracao<ConfiguracaoAtividade> _configAtividade = new GerenciadorConfiguracao<ConfiguracaoAtividade>(new ConfiguracaoAtividade());
		GerenciadorConfiguracao<ConfiguracaoSetor> _configSetor = new GerenciadorConfiguracao<ConfiguracaoSetor>(new ConfiguracaoSetor());
		GerenciadorConfiguracao<ConfiguracaoFiscalizacao> _configFiscalizacao = new GerenciadorConfiguracao<ConfiguracaoFiscalizacao>(new ConfiguracaoFiscalizacao());
		GerenciadorConfiguracao<ConfiguracaoCadastroAmbientalRural> _configCadastroAmbientalRural = new GerenciadorConfiguracao<ConfiguracaoCadastroAmbientalRural>(new ConfiguracaoCadastroAmbientalRural());
		GerenciadorConfiguracao<ConfiguracaoOrgaoParceiroConveniado> _configOrgaoParceiroConveniado = new GerenciadorConfiguracao<ConfiguracaoOrgaoParceiroConveniado>(new ConfiguracaoOrgaoParceiroConveniado());
		GerenciadorConfiguracao<ConfiguracaoVegetal> _configVegetal = new GerenciadorConfiguracao<ConfiguracaoVegetal>(new ConfiguracaoVegetal());
		GerenciadorConfiguracao<ConfiguracaoAgrotoxico> _configAgrotoxico = new GerenciadorConfiguracao<ConfiguracaoAgrotoxico>(new ConfiguracaoAgrotoxico());
		GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario> _configDocFitossanitario = new GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario>(new ConfiguracaoDocumentoFitossanitario());
		GerenciadorConfiguracao<ConfiguracaoPTV> _configPTV = new GerenciadorConfiguracao<ConfiguracaoPTV>(new ConfiguracaoPTV());
		GerenciadorConfiguracao<ConfiguracaoRelatorioEspecifico> _configRelatorio = new GerenciadorConfiguracao<ConfiguracaoRelatorioEspecifico>(new ConfiguracaoRelatorioEspecifico());

		#endregion

		public List<Lista> SistemaOrigem
		{
			get { return _configSys.Obter<List<Lista>>(ConfiguracaoSistema.KeySistemaOrigem); }
		}

		public List<Lista> BooleanLista
		{
			get { return _configSys.Obter<List<Lista>>(ConfiguracaoSistema.KeyBooleanLista); }
		}

		public List<Lista> AtivoLista
		{
			get { return _configSys.Obter<List<Lista>>(ConfiguracaoSistema.KeyAtivoLista); }
		}

		public List<Estado> Estados
		{
			get { return _configEndereco.Obter<List<Estado>>(ConfiguracaoEndereco.KeyEstados); }
		}

		public List<Municipio> Municipios(String uf)
		{
			Estado estado = Estados.SingleOrDefault(x => String.Equals(x.Texto, uf, StringComparison.InvariantCultureIgnoreCase));
			int idEstado = 1;

			if (estado != null)
			{
				idEstado = estado.Id;
			}

			Dictionary<int, List<Municipio>> lstMun = _configEndereco.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios);

			return lstMun[idEstado];
		}

		public List<Municipio> Municipios(int idEstado)
		{
			if (idEstado <= 0)
			{
				return new List<Municipio>();
			}

			Dictionary<int, List<Municipio>> lstMun = _configEndereco.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios);

			return lstMun[idEstado];
		}

		public ListaValor ModuloFiscal(int municipioID)
		{
			Dictionary<int, ListaValor> lista = _configEndereco.Obter<Dictionary<int, ListaValor>>(ConfiguracaoEndereco.KeyModulosFiscais);

			return lista[municipioID];
		}

		public List<Segmento> Segmentos
		{
			get { return _configEmpreendimento.Obter<List<Segmento>>(ConfiguracaoEmpreendimento.KeySegmentos); }
		}

		public List<Lista> LocalColetaPonto
		{
			get { return _configCoordenada.Obter<List<Lista>>(ConfiguracaoCoordenada.KeyLocalColetaPonto); }
		}

		public List<Lista> FormaColetaPonto
		{
			get { return _configCoordenada.Obter<List<Lista>>(ConfiguracaoCoordenada.KeyFormaColetaPonto); }
		}

		public List<TipoResponsavel> TiposResponsavel
		{
			get { return _configEmpreendimento.Obter<List<TipoResponsavel>>(ConfiguracaoEmpreendimento.KeyTiposResponsavel); }
		}

		public List<CoordenadaTipo> TiposCoordenada
		{
			get { return _configCoordenada.Obter<List<CoordenadaTipo>>(ConfiguracaoCoordenada.KeyTiposCoordenada); }
		}

		public List<Datum> Datuns
		{
			get { return _configCoordenada.Obter<List<Datum>>(ConfiguracaoCoordenada.KeyDatuns); }
		}

		public List<Fuso> Fusos
		{
			get { return _configCoordenada.Obter<List<Fuso>>(ConfiguracaoCoordenada.KeyFusos); }
		}

		public List<CoordenadaHemisferio> Hemisferios
		{
			get { return _configCoordenada.Obter<List<CoordenadaHemisferio>>(ConfiguracaoCoordenada.KeyHemisferios); }
		}

		#region Configuração do Sistema

		public String EstadoDefault
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault); }
		}

		public String ModeloTextoEmailDefault
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyModeloTextoEmailDefault); }
		}

		public String OrgaoSigla
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla); }
		}

		public List<Roteiro> RoteiroPadrao
		{
			get
			{
				List<String> valores = _configSys.Obter<List<String>>(ConfiguracaoSistema.KeyRoteiroPadrao);
				List<Roteiro> roteiros = new List<Roteiro>();
				if (valores != null && valores.Count > 0)
				{
					foreach (String valor in valores)
					{
						roteiros.Add(new Roteiro() { Id = Convert.ToInt32(valor.Split('@')[0]), Setor = Convert.ToInt32(valor.Split('@')[1]) });
					}
				}
				return roteiros;
			}
		}

		public List<QuantPaginacao> QuantPaginacao
		{
			get { return _configSys.Obter<List<QuantPaginacao>>(ConfiguracaoSistema.KeyLstQuantPaginacao); }
		}

		#endregion

		public List<Situacao> CredenciadoSituacoes
		{
			get { return _configCredenciado.Obter<List<Situacao>>(ConfiguracaoCredenciado.KeyCredenciadoSituacoes); }
		}

		public List<Situacao> CredenciadoTipos
		{
			get { return _configCredenciado.Obter<List<Situacao>>(ConfiguracaoCredenciado.KeyCredenciadoTipos); }
		}

		public List<OrgaoClasse> OrgaosClasse
		{
			get { return _configPessoa.Obter<List<OrgaoClasse>>(ConfiguracaoPessoa.KeyOrgaoClasses); }
		}

		public List<EstadoCivil> EstadosCivil
		{
			get { return _configPessoa.Obter<List<EstadoCivil>>(ConfiguracaoPessoa.KeyEstadosCivis); }
		}

		public List<Sexo> Sexos
		{
			get { return _configPessoa.Obter<List<Sexo>>(ConfiguracaoPessoa.KeySexos); }
		}

		public List<ProfissaoLst> Profissoes
		{
			get { return _configPessoa.Obter<List<ProfissaoLst>>(ConfiguracaoPessoa.KeyProfissoes); }
		}

		public List<Cargo> Cargos
		{
			get { return _configFuncionario.Obter<List<Cargo>>(ConfiguracaoFuncionario.KeyCargos); }
		}

		public List<Setor> Setores
		{
			get { return _configFuncionario.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores); }
		}

		public List<Setor> SetoresComSigla
		{
			get
			{
				List<Setor> lstSetores = (new ConfiguracaoFuncionario()).Setores;
				lstSetores.ForEach(x => x.Nome = x.SiglaComNome);
				return lstSetores;
			}
		}

		public List<Setor> SetoresAtuais
		{
			get { return (new ConfiguracaoFuncionario()).Setores; }
		}

		public List<Setor> SetoresComSiglaAtuais
		{
			get
			{
				List<Setor> lstSetores = (new ConfiguracaoFuncionario()).Setores;
				lstSetores.ForEach(x => x.Nome = x.SiglaComNome);
				return lstSetores;
			}
		}

		public List<Situacao> SituacaoChecagemRoteiro
		{
			get { return _configChecagemRoteiro.Obter<List<Situacao>>(ConfiguracaoChecagemRoteiro.KeySituacoesChecagemRoteiro); }
		}

		public List<Finalidade> Finalidades
		{
			get { return _configRequerimento.Obter<List<Finalidade>>(ConfiguracaoRequerimento.KeyFinalidades); }
		}

		public List<Situacao> SituacoesRequerimento
		{
			get { return _configRequerimento.Obter<List<Situacao>>(ConfiguracaoRequerimento.KeySituacoesRequerimento); }
		}

		public List<ResponsavelFuncoes> ResponsavelFuncoes
		{
			get { return _configResponsavel.Obter<List<ResponsavelFuncoes>>(ConfiguracaoResponsavel.KeyResponsavelFuncoes); }
		}

		public List<ProcessoAtividadeItem> AtividadesSolicitada
		{
			get { return _configProcesso.Obter<List<ProcessoAtividadeItem>>(ConfiguracaoProcesso.KeyAtividadesProcesso); }
		}

		public List<ProcessoAtividadeItem> AtividadesSolicitadaAtivasDesativas
		{
			get { return _configProcesso.Obter<List<ProcessoAtividadeItem>>(ConfiguracaoProcesso.KeyAtividadesSolicitadaTodas); }
		}

		public List<Situacao> SituacoesProcessoAtividade
		{
			get { return _configProcesso.Obter<List<Situacao>>(ConfiguracaoProcesso.KeySituacoesProcessoAtividade); }
		}

		public List<ProtocoloTipo> TiposProcesso
		{
			get { return _configProcesso.Obter<List<ProtocoloTipo>>(ConfiguracaoProcesso.KeyProcessoTipos); }
		}

		public List<Situacao> SituacoesItemAnalise
		{
			get { return _configAnalise.Obter<List<Situacao>>(ConfiguracaoAnalise.KeySituacoesItemAnalise); }
		}

		public List<ProtocoloTipo> TiposDocumento
		{
			get { return _configDocumento.Obter<List<ProtocoloTipo>>(ConfiguracaoDocumento.KeyDocumentoTipos); }
		}

		public List<TipoItem> ItemTipos
		{
			get { return _configRoteiro.Obter<List<TipoItem>>(ConfiguracaoRoteiro.KeyItemTipos); }
		}

		public List<Situacao> SituacaoChecagemPendencia
		{
			get { return _configChecagemPendencia.Obter<List<Situacao>>(ConfiguracaoChecagemPendencia.KeySituacoesChecagemPendencia); }
		}

		public List<Situacao> SituacaoChecagemPendenciaItem
		{
			get { return _configChecagemPendencia.Obter<List<Situacao>>(ConfiguracaoChecagemPendencia.KeySituacoesChecagemPendenciaItem); }
		}

		public List<AgendamentoVistoria> AgendamentoVistoria
		{
			get { return _configRequerimento.Obter<List<AgendamentoVistoria>>(ConfiguracaoRequerimento.KeyAgendamentoVistoria); }
		}

		public List<TramitacaoArquivoTipo> TiposTramitacaoArquivo
		{
			get { return _configTramitacao.Obter<List<TramitacaoArquivoTipo>>(ConfiguracaoTramitacao.KeyTramitacaoArquivoTipo); }
		}

		public List<TramitacaoArquivoModo> TiposTramitacaoArquivoModo
		{
			get { return _configTramitacao.Obter<List<TramitacaoArquivoModo>>(ConfiguracaoTramitacao.KeyTramitacaoArquivoModo); }
		}

		public List<Situacao> AtividadeSolicitadaSituacoes
		{
			get { return _configTramitacao.Obter<List<Situacao>>(ConfiguracaoTramitacao.KeyAtividadeSolicitadaSituacoes); }
		}

		#region Caracterização

		public List<Dependencia> CaracterizacoesDependencias
		{
			get { return _configCaracterizacao.Obter<List<Dependencia>>(ConfiguracaoCaracterizacao.KeyCaracterizacoesDependencias); }
		}

		public List<CaracterizacaoLst> Caracterizacoes
		{
			get { return _configCaracterizacao.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes); }
		}

		public List<Lista> CaracterizacaoGeometriaTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyCaracterizacaoGeometriaTipo); }
		}

		public List<Lista> CaracterizacaoUnidadeMedida
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyCaracterizacaoUnidadeMedida); }
		}

		public List<Lista> CaracterizacaoProdutosExploracao
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyCaracterizacaoProdutosExploracao); }
		}

		#region Dominialidades

		public List<Lista> DominialidadeComprovacoes
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeComprovacoes); }
		}

		public List<Lista> DominialidadeReservaSituacaoVegetacao
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaSituacaoVegetacao); }
		}

		public List<Lista> DominialidadeReservaSituacoes
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaSituacoes); }
		}

		public List<Lista> DominialidadeReservaLocalizacoes
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaLocalizacoes); }
		}

		public List<Lista> DominialidadeReservaCartorios
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaCartorios); }
		}

		#endregion

		#region RegularizacaoFundiaria

		public List<Lista> RegularizacaoFundiariaHomologacao
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyRegularizacaoFundiariaHomologacao); }
		}

		public List<RelacaoTrabalho> RegularizacaoFundiariaRelacaoTrabalho
		{
			get { return _configCaracterizacao.Obter<List<RelacaoTrabalho>>(ConfiguracaoCaracterizacao.KeyRegularizacaoFundiariaRelacaoTrabalho); }
		}

		public List<Lista> RegularizacaoFundiariaTipoRegularizacao
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyRegularizacaoFundiariaTipoRegularizacao); }
		}

		public List<Lista> RegularizacaoFundiariaTipoLimite
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyRegularizacaoFundiariaTipoLimite); }
		}

		public List<UsoAtualSoloLst> RegularizacaoFundiariaTipoUso
		{
			get { return _configCaracterizacao.Obter<List<UsoAtualSoloLst>>(ConfiguracaoCaracterizacao.KeyRegularizacaoFundiariaTipoUso).MoverItemFinal(ConfiguracaoExtensoes.OUTRO); }
		}

		#endregion

		#region ExploracaoFlorestal

		public List<FinalidadeExploracao> ExploracaoFlorestalFinalidadesExploracoes
		{
			get { return _configCaracterizacao.Obter<List<FinalidadeExploracao>>(ConfiguracaoCaracterizacao.KeyExploracaoFlorestalFinalidadeExploracao); }
		}

		public List<Lista> ExploracaoFlorestalClassificacoesVegetais
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyExploracaoFlorestalClassificacaoVegetal); }
		}

		public List<Lista> ExploracaoFlorestalExploracoesTipos
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyExploracaoFlorestalExploracao); }
		}

		#endregion

		#region Queima Controlada

		public List<Lista> QueimaControladaCultivoTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyQueimaControladaCultivoTipo); }
		}

		#endregion

		#region Secagem Mecânica de Grãos

		public List<Lista> SecagemMecanicaGraosMateriaPrimaConsumida
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeySecagemMecanicaGraosMateriaPrimaConsumida); }
		}

		#endregion

		#region Descrição de Licenciamento de Atividade

		public List<Lista> DscLicAtvFontesAbastecimentoAguaTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvFontesAbastecimentoAguaTipo); }
		}

		public List<Lista> DscLicAtvPontosLancamentoEfluenteTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvPontosLancamentoEfluenteTipo); }
		}

		public List<Lista> DscLicAtvOutorgaAguaTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvOutorgaAguaTipo); }
		}

		public List<Lista> DscLicAtvFontesGeracaoTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvFontesGeracaoTipo); }
		}

		public List<Lista> DscLicAtvUnidadeTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvUnidadeTipo); }
		}

		public List<Lista> DscLicAtvCombustivelTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvCombustivelTipo); }
		}

		public List<Lista> DscLicAtvVegetacaoAreaUtil
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvVegetacaoAreaUtil); }
		}

		public List<Lista> DscLicAtvAcondicionamento
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvAcondicionamento); }
		}

		public List<Lista> DscLicAtvEstocagem
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvEstocagem); }
		}

		public List<Lista> DscLicAtvTratamento
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvTratamento); }
		}

		public List<Lista> DscLicAtvDestinoFinal
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDscLicAtvDestinoFinal); }
		}

		#endregion

		#region Producao Carvao Vegetal

		public List<Lista> ProducaoCarvaoVegetalMateriaPrimaConsumida
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyProducaoCarvaoVegetalMateriaPrimaConsumida); }
		}

		#endregion

		#region Avicultura

		public List<Lista> AviculturaConfinamentoFinalidades
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyAviculturaConfinamentoFinalidades); }
		}

		#endregion

		#region Suinocultura

		public List<Lista> SuinoculturaFases
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeySuinoculturaFases); }
		}

		#endregion

		#region Beneficiamento e tratamento de madeira

		public List<Lista> BeneficiamentoMadeiraMateriaPrimaConsumida
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBeneficiamentoMadeiraMateriaPrimaConsumida); }
		}

		#endregion

		#region Barragem

		public List<Lista> BarragemFinalidades
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemFinalidades); }
		}

		public List<Lista> BarragemOutorgas
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemOutorgas); }
		}

		#region Registro Atividade Florestal

		public List<ListaValor> RegistroAtividadeFlorestalFonte
		{
			get { return _configCaracterizacao.Obter<List<ListaValor>>(ConfiguracaoCaracterizacao.KeyRegistroAtividadeFlorestalFonte); }
		}

		public List<ListaValor> RegistroAtividadeFlorestalUnidade
		{
			get { return _configCaracterizacao.Obter<List<ListaValor>>(ConfiguracaoCaracterizacao.KeyRegistroAtividadeFlorestalUnidade); }
		}


		#endregion

		#endregion

		#region Silvicultura

		public List<Lista> SilviculturaCulturasFlorestais
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeySilviculturaCulturasFlorestais); }
		}

		#endregion

		#region Silvicultura - Implantação da Atividade de Silvicultura (Fomento)

		public List<Lista> SilviculturaAtvCaracteristicaFomento
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeySilviculturaAtvCaracteristicaFomento); }
		}

		public List<Lista> SilviculturaAtvCoberturaExitente
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeySilviculturaAtvCoberturaExitente); }
		}

		#endregion

		#region Informacao de Corte

		public List<Lista> DestinacaoMaterial
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDestinacaoMaterial); }
		}

		#endregion

		#region Pulverização Aérea de Produtos Agrotóxicos

		public List<Lista> PulverizacaoProdutoCulturas
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyPulverizacaoProdutoCulturas); }
		}

		#endregion

		#region Barragem para Dispensa de Licença Ambiental

		public List<Lista> BarragemDispensaLicencaBarragemTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaBarragemTipo); }
		}

		public List<Lista> BarragemDispensaLicencaFinalidadeAtividade
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaFinalidadeAtividade); }
		}

		public List<Lista> BarragemDispensaLicencaFase
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaFase); }
		}

		public List<Lista> BarragemDispensaLicencaMongeTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaMongeTipo); }
		}

		public List<Lista> BarragemDispensaLicencaVertedouroTipo
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaVertedouroTipo); }
		}

		public List<Lista> BarragemDispensaLicencaFormacaoRT
		{
			get { return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaFormacaoRT); }
		}

		#endregion

		#endregion

		#region Titulo

		public List<Situacao> TituloSituacoes
		{
			get { return _configTitulo.Obter<List<Situacao>>(ConfiguracaoTitulo.KeySituacoes); }
		}

		public List<Situacao> TituloDeclaratorioSituacoes
		{
			get { return _configTitulo.Obter<List<Situacao>>(ConfiguracaoTitulo.KeyTituloDeclaratorioSituacoes); }
		}

		public List<TituloCondicionantePeriodTipo> TituloCondicionantePeriodicidades
		{
			get { return _configTitulo.Obter<List<TituloCondicionantePeriodTipo>>(ConfiguracaoTitulo.KeyTituloCondicionantePeriodicidades); }
		}

		public List<TituloCondicionanteSituacao> TituloCondicionanteSituacoes
		{
			get { return _configTitulo.Obter<List<TituloCondicionanteSituacao>>(ConfiguracaoTitulo.KeyTituloCondicionanteSituacoes); }
		}

		public List<TituloModeloTipo> TituloModeloTipos
		{
			get { return _configTitulo.Obter<List<TituloModeloTipo>>(ConfiguracaoTitulo.KeyTituloModeloTipos); }
		}

		public List<TituloModeloProtocoloTipo> TituloModeloProtocoloTipos
		{
			get { return _configTitulo.Obter<List<TituloModeloProtocoloTipo>>(ConfiguracaoTitulo.KeyTituloModeloProtocoloTipos); }
		}

		public List<TituloModeloPeriodoRenovacao> TituloModeloPeriodosRenovacoes
		{
			get { return _configTitulo.Obter<List<TituloModeloPeriodoRenovacao>>(ConfiguracaoTitulo.KeyTituloModeloPeriodosRenovacoes); }
		}

		public List<TituloModeloInicioPrazo> TituloModeloIniciosPrazos
		{
			get { return _configTitulo.Obter<List<TituloModeloInicioPrazo>>(ConfiguracaoTitulo.KeyTituloModeloIniciosPrazos); }
		}

		public List<TituloModeloTipoPrazo> TituloModeloTiposPrazos
		{
			get { return _configTitulo.Obter<List<TituloModeloTipoPrazo>>(ConfiguracaoTitulo.KeyTituloModeloTiposPrazos); }
		}

		public List<TituloModeloAssinante> TituloModeloAssinantes
		{
			get { return _configTitulo.Obter<List<TituloModeloAssinante>>(ConfiguracaoTitulo.KeyTituloModeloAssinantes); }
		}

		public List<Acao> TituloAlterarSituacaoAcoes
		{
			get { return _configTitulo.Obter<List<Acao>>(ConfiguracaoTitulo.KeyAlterarSituacaoAcoes); }
		}

		public List<Motivo> MotivosEncerramento
		{
			get { return _configTitulo.Obter<List<Motivo>>(ConfiguracaoTitulo.KeyMotivosEncerramento); }
		}

		public List<Motivo> DeclaratorioMotivosEncerramento
		{
			get { return _configTitulo.Obter<List<Motivo>>(ConfiguracaoTitulo.KeyDeclaratorioMotivosEncerramento); }
		}

		public List<TituloModeloTipoDocumento> TituloModeloTipoDocumento
		{
			get { return _configTitulo.Obter<List<TituloModeloTipoDocumento>>(ConfiguracaoTitulo.KeyTituloModeloTipoDocumento); }
		}

		#endregion

		#region Especificidade

		public List<Legislacao> ParecerManifestacaoLegislacoes
		{
			get { return _configEspecificidade.Obter<List<Legislacao>>(ConfiguracaoEspecificidade.KeyParecerManifestacaoLegislacoes); }
		}

		public List<Lista> ObterEspecificidadeConclusoes
		{
			get { return _configEspecificidade.Obter<List<Lista>>(ConfiguracaoEspecificidade.KeyEspecificidadeConclusoes); }
		}

		#region Laudo

		public List<Lista> ObterEspecificidadeResultados
		{
			get { return _configEspecificidade.Obter<List<Lista>>(ConfiguracaoEspecificidade.KeyEspecificidadeResultados); }
		}


		#endregion

		#region Certidão

		public List<Lista> ObterVinculoPropriedade
		{
			get { return _configEspecificidade.Obter<List<Lista>>(ConfiguracaoEspecificidade.KeyVinculoPropriedade); }
		}

		#endregion

		#endregion

		#region Fiscalizacao

		public List<Lista> FiscalizacaoComplementoDadosRespostas
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyComplementoDadosRespostas); }
		}

		public List<Lista> FiscalizacaoComplementoDadosRendaMensal
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyComplementoDadosRendaMensal); }
		}

		public List<Lista> FiscalizacaoComplementoDadosNivelEscolaridade
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyComplementoDadosNivelEscolaridade); }
		}

		public List<ReservaLegalLst> FiscalizacaoComplementoDadosReservaLegalTipo
		{
			get { return _configFiscalizacao.Obter<List<ReservaLegalLst>>(ConfiguracaoFiscalizacao.KeyComplementoDadosReservaLegalTipo); }
		}

		public List<Lista> MaterialApreendidoTipo
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyMaterialApreendidoTipo); }
		}

		public List<CaracteristicaSoloAreaDanificada> FiscalizacaoObjetoInfracaoCaracteristicaSolo
		{
			get { return _configFiscalizacao.Obter<List<CaracteristicaSoloAreaDanificada>>(ConfiguracaoFiscalizacao.KeyFiscalizacaoObjetoInfracaoCaracteristicaSolo); }
		}

		public List<Lista> FiscalizacaoObjetoInfracaoSerie
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyFiscalizacaoObjetoInfracaoSerie); }
		}

		public List<Lista> FiscalizacaoSerie
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyFiscalizacaoSerie); }
		}

		public List<Lista> InfracaoClassificacao
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyInfracaoClassificacao); }
		}

		public List<Lista> InfracaoCodigoReceita
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyObterInfracaoCodigoReceita); }
		}

		public List<Lista> FiscalizacaoSituacao
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyFiscalizacaoSituacao); }
		}

		public List<Lista> AcompanhamentoFiscalizacaoSituacao
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyAcompanhamentoFiscalizacaoSituacao); }
		}

		#endregion

		#region Configuracao Fiscalizacao

		public List<Lista> ConfiguracaoFiscalizacaoCamposTipo
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyConfiguracaoFiscalizacaoCamposTipo); }
		}

		public List<Lista> ConfiguracaoFiscalizacaoCamposUnidade
		{
			get { return _configFiscalizacao.Obter<List<Lista>>(ConfiguracaoFiscalizacao.KeyConfiguracaoFiscalizacaoCamposUnidade); }
		}

		#endregion

		#region Acompanhamento Fiscalizacao

		public List<CaracteristicaSoloAreaDanificada> AcompanhamentoFiscalizacaoCaracteristicaSolo
		{
			get { return _configFiscalizacao.Obter<List<CaracteristicaSoloAreaDanificada>>(ConfiguracaoFiscalizacao.KeyAcompanhamentoFiscalizacaoCaracteristicaSolo); }
		}

		public List<ReservaLegalLst> AcompanhamentoFiscalizacaoReservaLegalTipo
		{
			get { return _configFiscalizacao.Obter<List<ReservaLegalLst>>(ConfiguracaoFiscalizacao.KeyAcompanhamentoFiscalizacaoReservaLegalTipo); }
		}

		#endregion

		#region Cadastro Ambiental Rural - Solicitacao CAR

		public List<Lista> CadastroAmbientalRuralSolicitacaoSituacao
		{
			get { return _configCadastroAmbientalRural.Obter<List<Lista>>(ConfiguracaoCadastroAmbientalRural.KeyCadastroAmbientalRuralSolicitacaoSituacao); }
		}

		public List<Lista> CadastroAmbientalRuralSolicitacaoOrigem
		{
			get { return _configCadastroAmbientalRural.Obter<List<Lista>>(ConfiguracaoCadastroAmbientalRural.KeyCadastroAmbientalRuralSolicitacaoOrigem); }
		}

		#endregion Cadastro Ambiental Rural - CAR

		public List<AtividadeAgrupador> AgrupadoresAtividade
		{
			get { return _configAtividade.Obter<List<AtividadeAgrupador>>(ConfiguracaoAtividade.KeyAgrupadoresAtividade); }
		}

		public List<SetorAgrupador> AgrupadoresSetor
		{
			get { return _configSetor.Obter<List<SetorAgrupador>>(ConfiguracaoSetor.KeyAgrupadoresSetor); }
		}

		public List<ContatoLst> ListarMeiosContato
		{
			get { return _configSys.Obter<List<ContatoLst>>(ConfiguracaoSistema.KeyMeiosContato); }
		}

		public List<ListaValor> AtividadesCategoria
		{
			get { return _configAtividade.Obter<List<ListaValor>>(ConfiguracaoAtividade.KeyAtividadesCategoria); }
		}

		public List<Lista> OrgaoParceirosConveniadosSituacoes
		{
			get { return _configOrgaoParceiroConveniado.Obter<List<Lista>>(ConfiguracaoOrgaoParceiroConveniado.KeyOrgaoParceirosConveniadosSituacoes); }
		}

		#region Configuração Vegetal

		public List<Situacao> IngredienteAtivoSituacoes
		{
			get { return _configVegetal.Obter<List<Situacao>>(ConfiguracaoVegetal.KeyIngredienteAtivoSituacoes); }
		}

		public List<ListaValor> AgrotoxicoDesativadosMensagens
		{
			get { return _configVegetal.Obter<List<ListaValor>>(ConfiguracaoVegetal.KeyMensagensAgrotoxicoDesativados); }

		}

		public List<Lista> CultivarTipos
		{
			get { return _configVegetal.Obter<List<Lista>>(ConfiguracaoVegetal.KeyCultivarTipos); }
		}

		public List<Lista> DeclaracaoAdicional
		{
			get { return _configVegetal.Obter<List<Lista>>(ConfiguracaoVegetal.KeyDeclaracaoAdicional); }
		}
		#endregion

		#region Habilitar Emissão de CFO e CFOC

		public List<Situacao> HabilitacaoCFOSituacoes
		{
			get { return _configCredenciado.Obter<List<Situacao>>(ConfiguracaoCredenciado.KeyHabilitacaoCFOSituacoes); }
		}

		public List<Lista> HabilitacaoCFOMotivos
		{
			get { return _configCredenciado.Obter<List<Lista>>(ConfiguracaoCredenciado.KeyHabilitacaoCFOMotivos); }
		}

		#endregion

		#region Agrotóxico

		public List<Lista> AgrotoxicoIngredienteAtivoUnidadeMedida
		{
			get { return _configAgrotoxico.Obter<List<Lista>>(ConfiguracaoAgrotoxico.KeyAgrotoxicoIngredienteAtivoUnidadeMedida); }
		}

		#endregion

		public List<Lista> DocumentosFitossanitario
		{
			get { return _configDocFitossanitario.Obter<List<Lista>>(ConfiguracaoDocumentoFitossanitario.KeyDocumentosFitossanitario); }
		}

        public List<Lista> DocumentosFitossanitarioTipoNumero
        {
            get { return _configDocFitossanitario.Obter<List<Lista>>(ConfiguracaoDocumentoFitossanitario.KeyDocFitossanitarioTipoNumero); }
        }

		public List<Lista> DocFitossanitarioSituacao
		{
			get { return _configDocFitossanitario.Obter<List<Lista>>(ConfiguracaoDocumentoFitossanitario.KeyDocFitossanitarioSituacao); }
		}

		public List<Lista> DiasSemana
		{
			get { return _configSys.Obter<List<Lista>>(ConfiguracaoSistema.KeyDiasSemana); }
		}

		#region PTV

		public List<Lista> PTVSituacao
		{
			get { return _configPTV.Obter<List<Lista>>(ConfiguracaoPTV.KeyPTVSituacao); }
		}

		public List<Lista> PTVSolicitacaoSituacao
		{
			get { return _configPTV.Obter<List<Lista>>(ConfiguracaoPTV.KeyPTVSolicitacaoSituacao); }
		}

		public List<Lista> PTVUnidadeMedida
		{
			get { return _configPTV.Obter<List<Lista>>(ConfiguracaoPTV.KeyPTVUnidadeMedida); }
		}

		public List<Lista> TipoTransporte
		{
			get { return _configPTV.Obter<List<Lista>>(ConfiguracaoPTV.KeyTipoTransporte); }
		}

		#endregion

		public List<ListaValor> TipoRelatorioMapa
		{
			get { return _configRelatorio.Obter<List<ListaValor>>(ConfiguracaoRelatorioEspecifico.KeyTipoRelatorioMapa); }
		}
	}
}