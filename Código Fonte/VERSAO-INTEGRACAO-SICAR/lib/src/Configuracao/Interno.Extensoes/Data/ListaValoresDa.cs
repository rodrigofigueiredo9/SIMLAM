using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Configuracao.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes.Data
{
	class ListaValoresDa
	{
		public String UsuarioGeo
		{
			get { return new ConfiguracaoSistema().Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		internal String BuscarConfiguracaoSistema(string valor)
		{
			String retorno = String.Empty;

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.valor from cnf_sistema c where lower(c.campo) = :valor", valor.ToLower());

			foreach (var item in daReader)
			{
				retorno = item["valor"].ToString();
				break;
			}

			return retorno;
		}

		internal List<Lista> ObterLocalColetaPonto()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_empreendimento_local_colet c order by c.id");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterFormaColetaPonto()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_empreendimento_forma_colet c order by c.id");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<NivelPrecisao> ObterNiveisPrecisao()
		{
			//Implementar Query
			List<NivelPrecisao> lst = new List<NivelPrecisao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_projeto_geo_nivel c"); // Inserir Select
			foreach (var item in daReader)
			{
				lst.Add(new NivelPrecisao()
				{
					Id = Convert.ToInt32(item["id"]).ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<SistemaCoordenada> ObterSistemaCoordenada()
		{
			//Implementar Query
			List<SistemaCoordenada> lst = new List<SistemaCoordenada>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_coordenada_tipo c"); // Inserir Select
			foreach (var item in daReader)
			{
				lst.Add(new SistemaCoordenada()
				{
					Id = Convert.ToInt32(item["id"]).ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#region Caracterizacoes

		internal List<CaracterizacaoLst> ObterCaracterizacoes()
		{
			List<CaracterizacaoLst> lst = new List<CaracterizacaoLst>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.tabela from lov_caracterizacao_tipo c order by c.id");
			foreach (var item in daReader)
			{
				lst.Add(new CaracterizacaoLst()
				{
					Id = Convert.ToInt32(item["id"].ToString()),
					Texto = item["texto"].ToString(),
					Tabela = item["tabela"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterCaracterizacaoGeometriaTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_geometria_tipo c order by c.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterCaracterizacaoUnidadeMedida()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_unidade_medida c order by c.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterCaracterizacaoProdutosExploracao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_produto t");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<DependenciaLst> ObterCaracterizacoesDependencias()
		{
			List<DependenciaLst> lst = new List<DependenciaLst>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.caracterizacao, c.dependencia, c.tipo_detentor, c.tipo, l.texto tipo_texto, c.exibir_credenciado 
			from cnf_caracterizacao c, lov_crt_dependencia_tipo l where c.tipo = l.id");

			foreach (var item in daReader)
			{
				lst.Add(new DependenciaLst()
				{
					Id = Convert.ToInt32(item["id"]),
					DependenteTipo = Convert.ToInt32(item["caracterizacao"]),
					DependenciaTipo = Convert.ToInt32(item["dependencia"]),
					TipoId = Convert.ToInt32(item["tipo"]),
					TipoTexto = item["tipo_texto"].ToString(),
					TipoDetentorId = Convert.ToInt32(item["tipo_detentor"]),
					ExibirCredenciado = Convert.ToBoolean(item["exibir_credenciado"])
				});
			}

			return lst;
		}

		#region Projeto Geo

		internal List<SobreposicaoTipo> ObterSobreposicaoTipo()
		{
			//Implementar Query
			List<SobreposicaoTipo> lst = new List<SobreposicaoTipo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_projeto_geo_sobrepo c"); // Inserir Select
			foreach (var item in daReader)
			{
				lst.Add(new SobreposicaoTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Situacao> ObterSituacoesProcessamento()
		{
			//Implementar Query
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_projeto_geo_sit_proce c");
			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#endregion

		#region Dominialidade

		internal List<Lista> ObterDominialidadeDominioTipos()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_domin_dominio_tipo c order by c.texto");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterDominialidadeComprovacoes()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_domin_comprovacao c order by c.id");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> DominialidadeReservaSituacoes()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_domin_reserva_situacao c order by c.texto");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> DominialidadeReservaLocalizacoes()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_domin_reserva_local c order by c.texto");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> DominialidadeReservaCartorios()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_domin_reserva_cartorio c order by c.texto");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> DominialidadeReservaSituacaoVegetacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_domin_reserva_sit_veg c order by c.texto");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#endregion

		#region RegularizacaoFundiaria

		internal List<Lista> ObterRegularizacaoFundiariaHomologacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_regularizacao_homologa c order by c.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<RelacaoTrabalho> ObterRegularizacaoFundiariaRelacaoTrabalho()
		{
			List<RelacaoTrabalho> lst = new List<RelacaoTrabalho>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.codigo from lov_crt_regularizacao_rel_tab c order by c.texto");
			foreach (var item in daReader)
			{
				lst.Add(new RelacaoTrabalho()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true,
					Codigo = Convert.ToInt32(item["codigo"])
				});
			}

			return lst;
		}

		internal List<Lista> ObterRegularizacaoFundiariaTipoRegularizacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_regularizacao_tipo c order by c.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterRegularizacaoFundiariaTipoLimite()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_crt_regularizacao_limite c order by c.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<UsoAtualSoloLst> ObterRegularizacaoFundiariaTipoUso()
		{
			List<UsoAtualSoloLst> lst = new List<UsoAtualSoloLst>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.tipo_geo from lov_crt_regularizacao_tip_uso c order by c.texto");
			foreach (var item in daReader)
			{
				lst.Add(new UsoAtualSoloLst()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					TipoGeo = item["tipo_geo"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#endregion

		#region ExploracaoFlorestal

		internal List<FinalidadeExploracao> ObterExploracaoFlorestalFinalidadeExploracao()
		{
			List<FinalidadeExploracao> lst = new List<FinalidadeExploracao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_crt_exp_flores_finalidade t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new FinalidadeExploracao()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterExploracaoFlorestalClassificacaoVegetal()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_exp_flores_classif t");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterExploracaoFlorestalExploracao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_exp_flores_exploracao t");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Queima Controlada

		internal List<Lista> ObterCultivoTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_queima_c_cultivo_tipo t");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Secagem Mecânica de Grãos

		internal List<Lista> ObterSecagemMateriaPrimaConsumida()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_sec_mec_graos_mat_pr_c t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Descrição de Licenciamento de Atividade

		internal List<Lista> ObterDscLicAtvFontesAbastecimentoAguaTipo()
		{
			return ObterDadosLov("lov_crt_dsc_lc_atv_foabagua");
		}

		internal List<Lista> ObterDscLicAtvPontosLancamentoEfluenteTipo()
		{
			return ObterDadosLov("lov_crt_dsc_lc_atv_ptlanefl");
		}

		internal List<Lista> ObterDscLicAtvOutorgaAguaTipo()
		{
			return ObterDadosLov("lov_crt_dsc_lc_atv_out_agua");
		}

		internal List<Lista> ObterDscLicAtvFontesGeracaoTipo()
		{
			return ObterDadosLov("lov_crt_dsc_lc_atv_font_ger");
		}

		internal List<Lista> ObterDscLicAtvUnidadeTipo()
		{
			return ObterDadosLov("lov_crt_dsc_lc_atv_unidade");
		}

		internal List<Lista> ObterDscLicAtvCombustivelTipo()
		{
			return ObterDadosLov("lov_crt_dsc_lc_atv_combust");
		}

		internal List<Lista> ObterDscLicAtvVegetacaoAreaUtil()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista("select t.id, t.texto, t.codigo from lov_crt_dsc_lc_atv_vgarutil t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true,
					Codigo = item["codigo"].ToString()
				});
			}
			return lst;
		}

		internal List<Lista> ObterDscLicAtvAcondicionamento()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista("select t.id, t.texto, t.codigo from lov_crt_dsc_lc_atv_acondici t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true,
					Codigo = item["codigo"].ToString()
				});
			}
			return lst;
		}

		internal List<Lista> ObterDscLicAtvEstocagem()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista("select t.id, t.texto, t.codigo from lov_crt_dsc_lc_atv_estocage t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true,
					Codigo = item["codigo"].ToString()
				});
			}
			return lst;
		}

		internal List<Lista> ObterDscLicAtvTratamento()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista("select t.id, t.texto, t.codigo from lov_crt_dsc_lc_atv_tratamen t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true,
					Codigo = item["codigo"].ToString()
				});
			}
			return lst;
		}

		internal List<Lista> ObterDscLicAtvDestinoFinal()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista("select t.id, t.texto, t.codigo from lov_crt_dsc_lc_atv_dt_final t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true,
					Codigo = item["codigo"].ToString()
				});
			}
			return lst;
		}

		#endregion

		#region Producao Carvao Vegetal

		internal List<Lista> ObterProducaoCarvaoVegetalMateriaPrimaConsumida()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_prod_carv_veg_mat_pr_c t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Avicultura

		internal List<Lista> ObterAviculturaConfinamentoFinalidades()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_avicultura_finalid t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Suinocultura

		internal List<Lista> ObterSuinoculturaFases()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_suinocultura_fase t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Beneficiamento e tratamento de madeira

		internal List<Lista> ObterBeneficiamentoMadeiraMateriaPrimaConsumida()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_benef_madeira_mat_pr_c t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Registro de atividade florestal

		internal List<ListaValor> ObterRegistroAtividadeFlorestalFonte()
		{
			List<ListaValor> lst = new List<ListaValor>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@" select l.id, l.texto from lov_crt_reg_ati_flo_fonte l order by l.texto ");
			foreach (var item in daReader)
			{
				lst.Add(new ListaValor()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<ListaValor> ObterRegistroAtividadeFlorestalUnidade()
		{
			List<ListaValor> lst = new List<ListaValor>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@" select l.id, l.sigla texto from lov_crt_reg_ati_flo_unidade l order by l.sigla");
			foreach (var item in daReader)
			{
				lst.Add(new ListaValor()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Barragem

		internal List<Lista> ObterBarragemFinalidades()
		{
			return ObterDadosLov("lov_crt_barragem_finalidade");
		}

		internal List<Lista> ObterBarragemOutorgas()
		{
			return ObterDadosLov("lov_crt_barragem_outorga");
		}

		#endregion

		#region Silvicultura

		internal List<Lista> ObterSilviculturaCulturasFlorestais()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_silvicultura_cult_fl t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Silvicultura - Implantação da Atividade de Silvicultura (Fomento)

		internal List<Lista> ObterSilviculturaAtvCaracteristicaFomento()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_silvic_atv_fomento t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterSilviculturaAtvCoberturaExitente()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_silvic_atv_cobe t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Informacao de Corte

		internal List<Lista> ObterDestinacaoMaterial()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_inf_corte_inf_dest_mat t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Pulverização Aérea de Produtos Agrotóxicos

		internal List<Lista> ObterPulverizacaoProdutoCulturas()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_pulveriz_prod_agr_cult t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Barragem para Dispensa de Licença Ambiental

		internal List<Lista> ObterBarragemDispensaLicencaBarragemTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_bdla_barragem_tipo t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterBarragemDispensaLicencaFinalidadeAtividade()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_crt_bdla_finalidade_atv t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					Codigo = item["codigo"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterBarragemDispensaLicencaFase()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_bdla_fase t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterBarragemDispensaLicencaMongeTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_bdla_monge_tipo t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterBarragemDispensaLicencaVertedouroTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_crt_bdla_vertedouro_tipo t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterBarragemDispensaLicencaFormacaoRT()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_crt_bdla_formacao_rt t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					Codigo = item["codigo"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#endregion

		#region Especificidade

		internal List<Lista> ObterEspecificidadeConclusoes()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_esp_conclusao c order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterEspecificidadeResultados()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_esp_laudo_aud_foment_resul c order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Legislacao> ParecerManifestacaoLegislacoes()
		{
			List<Legislacao> lst = new List<Legislacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_esp_parecer_man_legislacao c order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new Legislacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterVinculoPropriedade()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_esp_cert_disp_amb c order by c.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#endregion

		internal List<Lista> ObterDadosLov(string strLov)
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista("select t.id, t.texto from " + strLov + " t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}
	}
}