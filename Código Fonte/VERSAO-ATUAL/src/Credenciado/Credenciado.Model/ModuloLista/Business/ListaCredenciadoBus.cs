using System;
using System.Collections.Generic;
using System.Linq;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloLista.Business
{
	public static class ListaCredenciadoBus
	{
		public static List<Lista> SistemaOrigem
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoSistema> configuracao = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				return configuracao.Obter<List<Lista>>(ConfiguracaoSistema.KeySistemaOrigem);
			}
		}

		public static List<QuantPaginacao> QuantPaginacao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoSistema> configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				return configSys.Obter<List<QuantPaginacao>>(ConfiguracaoSistema.KeyLstQuantPaginacao);
			}
		}

		public static List<Setor> Setores
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoFuncionario> configFuncionario = new GerenciadorConfiguracao<ConfiguracaoFuncionario>(new ConfiguracaoFuncionario());
				return configFuncionario.Obter<List<Setor>>(ConfiguracaoFuncionario.KeySetores);
			}
		}

		public static List<Finalidade> Finalidades
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoRequerimento> configRequerimento = new GerenciadorConfiguracao<ConfiguracaoRequerimento>(new ConfiguracaoRequerimento());
				return configRequerimento.Obter<List<Finalidade>>(ConfiguracaoRequerimento.KeyFinalidades);
			}
		}

		public static String EstadoDefault
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoSistema> configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				return configSys.Obter<String>(ConfiguracaoSistema.KeyEstadoDefault);
			}
		}

		public static List<Estado> Estados
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoEndereco> configEndereco = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());
				return configEndereco.Obter<List<Estado>>(ConfiguracaoEndereco.KeyEstados);
			}
		}

		public static List<Municipio> Municipios(String uf)
		{
			Estado estado = Estados.SingleOrDefault(x => String.Equals(x.Texto, uf, StringComparison.InvariantCultureIgnoreCase));
			int idEstado = 1;

			if (estado != null)
			{
				idEstado = estado.Id;
			}

			GerenciadorConfiguracao<ConfiguracaoEndereco> configEndereco = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());
			Dictionary<int, List<Municipio>> lstMun = configEndereco.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios);

			return lstMun[idEstado];
		}

		public static List<Municipio> Municipios(int idEstado)
		{
			if (idEstado <= 0)
			{
				return new List<Municipio>();
			}

			GerenciadorConfiguracao<ConfiguracaoEndereco> configEndereco = new GerenciadorConfiguracao<ConfiguracaoEndereco>(new ConfiguracaoEndereco());
			Dictionary<int, List<Municipio>> lstMun = configEndereco.Obter<Dictionary<int, List<Municipio>>>(ConfiguracaoEndereco.KeyMunicipios);

			return lstMun[idEstado];
		}

		public static List<Roteiro> RoteiroPadrao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoSistema> configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

				List<String> valores = configSys.Obter<List<String>>(ConfiguracaoSistema.KeyRoteiroPadrao);
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

		public static List<ContatoLst> ListarMeiosContato
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoSistema> configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				return configSys.Obter<List<ContatoLst>>(ConfiguracaoSistema.KeyMeiosContato);
			}
		}

		public static List<OrgaoClasse> OrgaosClasse
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPessoa> configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
				return configPessoa.Obter<List<OrgaoClasse>>(ConfiguracaoPessoa.KeyOrgaoClasses);
			}
		}

		public static List<EstadoCivil> EstadosCivil
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPessoa> configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
				return configPessoa.Obter<List<EstadoCivil>>(ConfiguracaoPessoa.KeyEstadosCivis);
			}
		}

		public static List<Sexo> Sexos
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPessoa> configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
				return configPessoa.Obter<List<Sexo>>(ConfiguracaoPessoa.KeySexos);
			}
		}

		public static List<ProfissaoLst> Profissoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPessoa> configPessoa = new GerenciadorConfiguracao<ConfiguracaoPessoa>(new ConfiguracaoPessoa());
				return configPessoa.Obter<List<ProfissaoLst>>(ConfiguracaoPessoa.KeyProfissoes);
			}
		}

		public static List<Situacao> CredenciadoSituacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCredenciado> configCredenciado = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());
				return configCredenciado.Obter<List<Situacao>>(ConfiguracaoCredenciado.KeyCredenciadoSituacoes);
			}
		}

		public static List<Motivo> MotivosEncerramento
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoTitulo> configTitulo = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
				return configTitulo.Obter<List<Motivo>>(ConfiguracaoTitulo.KeyMotivosEncerramento);
			}
		}

		public static List<Situacao> SituacoesRequerimento
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoRequerimento> configRequerimento = new GerenciadorConfiguracao<ConfiguracaoRequerimento>(new ConfiguracaoRequerimento());
				return configRequerimento.Obter<List<Situacao>>(ConfiguracaoRequerimento.KeySituacoesRequerimento);
			}
		}

		public static List<Segmento> Segmentos
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoEmpreendimento> configEmpreendimento = new GerenciadorConfiguracao<ConfiguracaoEmpreendimento>(new ConfiguracaoEmpreendimento());
				return configEmpreendimento.Obter<List<Segmento>>(ConfiguracaoEmpreendimento.KeySegmentos);
			}
		}

		public static List<TipoResponsavel> TiposResponsavel
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoEmpreendimento> configEmpreendimento = new GerenciadorConfiguracao<ConfiguracaoEmpreendimento>(new ConfiguracaoEmpreendimento());
				return configEmpreendimento.Obter<List<TipoResponsavel>>(ConfiguracaoEmpreendimento.KeyTiposResponsavel);
			}
		}

		public static List<CoordenadaTipo> TiposCoordenada
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCoordenada> configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
				return configCoordenada.Obter<List<CoordenadaTipo>>(ConfiguracaoCoordenada.KeyTiposCoordenada);
			}
		}

		public static List<Lista> LocalColetaPonto
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCoordenada> configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
				return configCoordenada.Obter<List<Lista>>(ConfiguracaoCoordenada.KeyLocalColetaPonto);
			}
		}

		public static List<Lista> FormaColetaPonto
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCoordenada> configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
				return configCoordenada.Obter<List<Lista>>(ConfiguracaoCoordenada.KeyFormaColetaPonto);
			}
		}

		public static List<Fuso> Fusos
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCoordenada> configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
				return configCoordenada.Obter<List<Fuso>>(ConfiguracaoCoordenada.KeyFusos);
			}
		}

		public static List<CoordenadaHemisferio> Hemisferios
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCoordenada> configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
				return configCoordenada.Obter<List<CoordenadaHemisferio>>(ConfiguracaoCoordenada.KeyHemisferios);
			}
		}

		public static List<Datum> Datuns
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCoordenada> configCoordenada = new GerenciadorConfiguracao<ConfiguracaoCoordenada>(new ConfiguracaoCoordenada());
				return configCoordenada.Obter<List<Datum>>(ConfiguracaoCoordenada.KeyDatuns);
			}
		}

		public static List<ProcessoAtividadeItem> AtividadesSolicitada
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoProcesso> configProcesso = new GerenciadorConfiguracao<ConfiguracaoProcesso>(new ConfiguracaoProcesso());
				return configProcesso.Obter<List<ProcessoAtividadeItem>>(ConfiguracaoProcesso.KeyAtividadesProcessoCredenciado);
			}
		}

		public static List<AtividadeAgrupador> AgrupadoresAtividade
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoAtividade> configAtividade = new GerenciadorConfiguracao<ConfiguracaoAtividade>(new ConfiguracaoAtividade());
				return configAtividade.Obter<List<AtividadeAgrupador>>(ConfiguracaoAtividade.KeyAgrupadoresAtividadeCredenciado);
			}
		}

		public static String OrgaoSigla
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoSistema> configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				return configSys.Obter<String>(ConfiguracaoSistema.KeyOrgaoSigla);
			}
		}

		public static List<ResponsavelFuncoes> ResponsavelFuncoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoResponsavel> configResponsavel = new GerenciadorConfiguracao<ConfiguracaoResponsavel>(new ConfiguracaoResponsavel());
				return configResponsavel.Obter<List<ResponsavelFuncoes>>(ConfiguracaoResponsavel.KeyResponsavelFuncoes);
			}
		}

		public static List<AgendamentoVistoria> AgendamentoVistoria
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoRequerimento> configRequerimento = new GerenciadorConfiguracao<ConfiguracaoRequerimento>(new ConfiguracaoRequerimento());
				return configRequerimento.Obter<List<AgendamentoVistoria>>(ConfiguracaoRequerimento.KeyAgendamentoVistoria);
			}
		}

		public static List<Situacao> SituacoesProcessoAtividade
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoProcesso> configProcesso = new GerenciadorConfiguracao<ConfiguracaoProcesso>(new ConfiguracaoProcesso());
				return configProcesso.Obter<List<Situacao>>(ConfiguracaoProcesso.KeySituacoesProcessoAtividade);
			}
		}

		public static List<ProtocoloTipo> TiposProcesso
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoProcesso> configProcesso = new GerenciadorConfiguracao<ConfiguracaoProcesso>(new ConfiguracaoProcesso());
				return configProcesso.Obter<List<ProtocoloTipo>>(ConfiguracaoProcesso.KeyProcessoTipos);
			}
		}

		public static List<ProtocoloTipo> TiposDocumento
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoDocumento> configDocumento = new GerenciadorConfiguracao<ConfiguracaoDocumento>(new ConfiguracaoDocumento());
				return configDocumento.Obter<List<ProtocoloTipo>>(ConfiguracaoDocumento.KeyDocumentoTipos);
			}
		}

		public static List<CaracterizacaoLst> Caracterizacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configSys = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return configSys.Obter<List<CaracterizacaoLst>>(ConfiguracaoCaracterizacao.KeyCaracterizacoes);
			}
		}

		public static List<Lista> BooleanLista
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoSistema> configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
				return configSys.Obter<List<Lista>>(ConfiguracaoSistema.KeyBooleanLista);
			}
		}

		public static List<Lista> DominialidadeComprovacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configSys = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return configSys.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeComprovacoes);
			}
		}

		public static List<Lista> DominialidadeReservaSituacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configSys = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return configSys.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaSituacoes);
			}
		}

		public static List<Lista> DominialidadeReservaSituacaoVegetacao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configSys = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return configSys.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaSituacaoVegetacao);
			}
		}

		public static List<Lista> DominialidadeReservaLocalizacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configSys = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return configSys.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaLocalizacoes);
			}
		}

		public static List<Lista> DominialidadeReservaCartorios
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> configSys = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return configSys.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaCartorios);
			}
		}

		public static List<Lista> CARSolicitacaoSituacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCadastroAmbientalRural> configSys = new GerenciadorConfiguracao<ConfiguracaoCadastroAmbientalRural>(new ConfiguracaoCadastroAmbientalRural());
				return configSys.Obter<List<Lista>>(ConfiguracaoCadastroAmbientalRural.KeyCadastroAmbientalRuralSolicitacaoSituacao);
			}
		}

        public static List<Lista> SicarSituacoes
        {
            get
            {
                GerenciadorConfiguracao<ConfiguracaoCadastroAmbientalRural> configSys = new GerenciadorConfiguracao<ConfiguracaoCadastroAmbientalRural>(new ConfiguracaoCadastroAmbientalRural());
                return configSys.Obter<List<Lista>>(ConfiguracaoCadastroAmbientalRural.KeySicarSituacao);
            }
        }

		public static List<Situacao> CredenciadoTipos
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCredenciado> _configCredenciado = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());
				return _configCredenciado.Obter<List<Situacao>>(ConfiguracaoCredenciado.KeyCredenciadoTipos);
			}
		}

		public static List<Lista> CFOProdutoEspecificacao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario> _configCredenciado = new GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario>(new ConfiguracaoDocumentoFitossanitario());
				return _configCredenciado.Obter<List<Lista>>(ConfiguracaoDocumentoFitossanitario.KeyCFOProdutoEspecificacao);
			}
		}

		public static List<Lista> CFOCLoteEspecificacao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario> _configCredenciado = new GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario>(new ConfiguracaoDocumentoFitossanitario());
				return _configCredenciado.Obter<List<Lista>>(ConfiguracaoDocumentoFitossanitario.KeyCFOCLoteEspecificacao);
			}
		}

		public static List<Lista> DocFitossanitarioSituacao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario> _configCredenciado = new GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario>(new ConfiguracaoDocumentoFitossanitario());
				return _configCredenciado.Obter<List<Lista>>(ConfiguracaoDocumentoFitossanitario.KeyDocFitossanitarioSituacao);
			}
		}

		public static List<Lista> DocumentosFitossanitario
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario> configSys = new GerenciadorConfiguracao<ConfiguracaoDocumentoFitossanitario>(new ConfiguracaoDocumentoFitossanitario());
				return configSys.Obter<List<Lista>>(ConfiguracaoDocumentoFitossanitario.KeyDocumentosFitossanitario);
			}
		}

		public static List<Lista> LoteSituacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCredenciado> _config = new GerenciadorConfiguracao<ConfiguracaoCredenciado>(new ConfiguracaoCredenciado());
				return _config.Obter<List<Lista>>(ConfiguracaoCredenciado.KeyLoteSituacoes);
			}
		}

		public static List<Lista> PTVSituacao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPTV> _configPTV = new GerenciadorConfiguracao<ConfiguracaoPTV>(new ConfiguracaoPTV());
				return _configPTV.Obter<List<Lista>>(ConfiguracaoPTV.KeyPTVSituacao);
			}
		}

		public static List<Lista> PTVUnidadeMedida
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPTV> _configPTV = new GerenciadorConfiguracao<ConfiguracaoPTV>(new ConfiguracaoPTV());
				return _configPTV.Obter<List<Lista>>(ConfiguracaoPTV.KeyPTVUnidadeMedida);
			}
		}

		public static List<Situacao> TituloDeclaratorioSituacoes
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoTitulo> configuracao = new GerenciadorConfiguracao<ConfiguracaoTitulo>(new ConfiguracaoTitulo());
				return configuracao.Obter<List<Situacao>>(ConfiguracaoTitulo.KeyTituloDeclaratorioSituacoes);
			}
		}

		#region Barragem para Dispensa de Licença Ambiental

		public static List<Lista> BarragemDispensaLicencaBarragemTipo
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaBarragemTipo);
			}
		}

		public static List<Lista> BarragemDispensaLicencaFinalidadeAtividade
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaFinalidadeAtividade);
			}
		}

		public static List<Lista> BarragemDispensaLicencaFase
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaFase);
			}
		}

		public static List<Lista> BarragemDispensaLicencaMongeTipo
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaMongeTipo);
			}
		}

		public static List<Lista> BarragemDispensaLicencaVertedouroTipo
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaVertedouroTipo);
			}
		}

		public static List<Lista> BarragemDispensaLicencaFormacaoRT
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _configCaracterizacao = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());
				return _configCaracterizacao.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyBarragemDispensaLicencaFormacaoRT);
			}
		}

		#endregion

		#region Certidão

		public static List<Lista> ObterVinculoPropriedade
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoEspecificidade> configEspecificidade = new GerenciadorConfiguracao<ConfiguracaoEspecificidade>(new ConfiguracaoEspecificidade());
				return configEspecificidade.Obter<List<Lista>>(ConfiguracaoEspecificidade.KeyVinculoPropriedade);
			}
		}

		#endregion

		public static List<Lista> PTVSolicitacaoSituacao
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPTV> configuracao = new GerenciadorConfiguracao<ConfiguracaoPTV>(new ConfiguracaoPTV());
				return configuracao.Obter<List<Lista>>(ConfiguracaoPTV.KeyPTVSolicitacaoSituacao);
			}
		}

		public static List<Lista> TipoTransporte
		{
			get
			{
				GerenciadorConfiguracao<ConfiguracaoPTV> configuracao = new GerenciadorConfiguracao<ConfiguracaoPTV>(new ConfiguracaoPTV());
				return configuracao.Obter<List<Lista>>(ConfiguracaoPTV.KeyTipoTransporte);
			}
		}
	}
}