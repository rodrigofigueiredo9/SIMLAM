using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Configuracao.Data;

namespace Tecnomapas.EtramiteX.Configuracao.Interno.Data
{
	public class ListaValoresDa
	{
		public String UsuarioGeo
		{
			get { return new ConfiguracaoSistema().Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		public String UsuarioCredenciado
		{
			get { return new ConfiguracaoSistema().Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public String UsuarioPublicoGeo
		{
			get { return new ConfiguracaoSistema().Obter<String>(ConfiguracaoSistema.KeyUsuarioPublicoGeo); }
		}

		public Municipio ObterMunicipio(Int32 codigoIbge)
		{
			Municipio municipio = new Municipio();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select m.id, m.texto, m.estado, e.sigla estado_sigla from lov_municipio m, lov_estado e where m.estado = e.id and m.ibge = :valor order by m.texto", codigoIbge.ToString());

			foreach (var item in daReader)
			{
				municipio.Id = Convert.ToInt32(item["id"]);
				municipio.Texto = item["texto"].ToString();
				municipio.Ibge = codigoIbge;
				municipio.Estado = new Estado() { Id = Convert.ToInt32(item["estado"]), Sigla = item["estado_sigla"].ToString() };
			}

			return municipio;
		}

		public List<Cargo> ObterCargos()
		{
			List<Cargo> lst = new List<Cargo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.nome, c.codigo from tab_cargo c order by c.nome");
			foreach (var item in daReader)
			{
				lst.Add(new Cargo()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Setor> ObterSetores()
		{
			List<Setor> lst = new List<Setor>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.nome, c.sigla, c.responsavel, c.unidade_convenio from tab_setor c order by c.nome");
			foreach (var item in daReader)
			{
				lst.Add(new Setor()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					Sigla = item["sigla"].ToString(),
					Responsavel = Convert.IsDBNull(item["responsavel"]) ? 0 : Convert.ToInt32(item["responsavel"]),
					UnidadeConvenio = item["unidade_convenio"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterDocumentosFitossanitario()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_doc_fitossanitarios_tipo l order by l.texto ");

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

		internal List<Lista> ObterDocFitossanitarioTipoNumero()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_doc_fitossani_tipo_numero l order by l.texto ");

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

		internal List<Lista> ObterDocFitossanitarioSituacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_doc_fitossani_situacao l order by l.texto ");

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
		
		internal List<Lista> ObterDocFitossanitarioTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_doc_fitossanitarios_tipo l where rownum < 3 order by l.texto");

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

		internal List<Lista> ObterCFOProdutoEspecificacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto, l.codigo from lov_cfo_produto_especificacao l order by l.id ");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					Codigo = item["codigo"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterCFOCProdutoEspecificacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto, l.codigo from lov_cfoc_lote_especificacao l order by l.id ");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					Codigo = item["codigo"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterCFOCLoteEspecificacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto, l.codigo from lov_cfoc_lote_especificacao l order by l.id ");

			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					Codigo = item["codigo"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<FuncionarioLst> ObterFuncionarios()
		{
			List<FuncionarioLst> lst = new List<FuncionarioLst>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select f.id, f.nome from tab_funcionario f where f.tipo = 3 order by f.nome");
			foreach (var item in daReader)
			{
				lst.Add(new FuncionarioLst()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["nome"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
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

		internal List<CoordenadaTipo> ObterTiposCoordenada()
		{
			List<CoordenadaTipo> lst = new List<CoordenadaTipo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_coordenada_tipo c");
			foreach (var item in daReader)
			{
				lst.Add(new CoordenadaTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Estado> ObterEstados()
		{
			List<Estado> lst = new List<Estado>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.sigla from lov_estado c");
			foreach (var item in daReader)
			{
				lst.Add(new Estado()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["sigla"].ToString(),
					Sigla = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ContatoLst> ObterMeiosContato()
		{
			List<ContatoLst> lst = new List<ContatoLst>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.mascara, t.tid from tab_meio_contato t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new ContatoLst()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					Mascara = item["mascara"].ToString(),
					Tid = item["tid"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal Dictionary<int, List<Municipio>> ObterMunicipios(List<Estado> estados)
		{
			Dictionary<int, List<Municipio>> lst = new Dictionary<int, List<Municipio>>();
			List<Municipio> lstMun = null;

			foreach (var estado in estados)
			{
				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.estado, c.cep, c.ibge from lov_municipio c where c.estado = :valor order by c.texto", estado.Id.ToString());

				lstMun = new List<Municipio>();
				foreach (var item in daReader)
				{
					lstMun.Add(new Municipio()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["texto"].ToString(),
						Estado = new Estado() { Id = Convert.ToInt32(item["estado"]) },
						IsAtivo = true,
						Ibge = Convert.ToInt32(item["ibge"])
					});
				}

				lst.Add(estado.Id, lstMun);

			}

			return lst;
		}

		internal Dictionary<int, ListaValor> ObterModulosFiscais()
		{
			Dictionary<int, ListaValor> lista = new Dictionary<int, ListaValor>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.municipio, c.modulo_ha from cnf_municipio_mod_fiscal c");
			foreach (var item in daReader)
			{
				lista.Add(Convert.ToInt32(item["municipio"]), new ListaValor()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["modulo_ha"].ToString(),
					IsAtivo = true
				});
			}

			return lista;
		}

		internal List<Datum> ObterDatuns()
		{
			List<Datum> lst = new List<Datum>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.sigla, c.texto from lov_coordenada_datum c");
			foreach (var item in daReader)
			{
				lst.Add(new Datum()
				{
					Id = Convert.ToInt32(item["id"]),
					Sigla = item["sigla"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Situacao> ObterSituacaoFuncionario()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto nome from lov_funcionario_situacao s order by s.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Situacao> ObterSituacaoAdministrador()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto nome from lov_funcionario_situacao s order by s.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<CoordenadaHemisferio> ObterHemisferios()
		{
			List<CoordenadaHemisferio> lst = new List<CoordenadaHemisferio>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select h.id, h.texto from lov_coordenada_hemisferio h");
			foreach (var item in daReader)
			{
				lst.Add(new CoordenadaHemisferio()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<QuantPaginacao> ObterLstQuantPaginacao()
		{
			List<QuantPaginacao> lst = new List<QuantPaginacao>();
			lst.Add(new QuantPaginacao { Texto = "5", Id = 5 });
			lst.Add(new QuantPaginacao { Texto = "10", Id = 10 });
			lst.Add(new QuantPaginacao { Texto = "20", Id = 20 });
			lst.Add(new QuantPaginacao { Texto = "30", Id = 30 });
			lst.Add(new QuantPaginacao { Texto = "50", Id = 50 });

			return lst;
		}

		public Dictionary<Int32, String> ObterDiretorioArquivoTemp()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 1");
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}

		public Dictionary<Int32, String> ObterDiretorioArquivo()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 2");
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}

		internal List<EstadoCivil> ObterEstadosCivis()
		{
			List<EstadoCivil> lst = new List<EstadoCivil>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_pessoa_estado_civil c");
			foreach (var item in daReader)
			{
				lst.Add(new EstadoCivil()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ProfissaoLst> ObterProfissoes()
		{
			List<ProfissaoLst> lst = new List<ProfissaoLst>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from tab_profissao c");
			foreach (var item in daReader)
			{
				lst.Add(new ProfissaoLst()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<OrgaoClasse> ObterOrgaoClasses()
		{
			List<OrgaoClasse> lst = new List<OrgaoClasse>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.orgao_sigla from tab_orgao_classe c");
			foreach (var item in daReader)
			{
				lst.Add(new OrgaoClasse()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["orgao_sigla"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Sexo> ObterSexos()
		{
			List<Sexo> lst = new List<Sexo>();

			lst.Add(new Sexo()
			{
				Id = 1,
				Texto = "Masculino",
				IsAtivo = true
			});

			lst.Add(new Sexo()
			{
				Id = 2,
				Texto = "Feminino",
				IsAtivo = true
			});

			return lst;
		}

		internal List<Situacao> ObterSituacaoChecagemRoteiro()
		{
			List<Situacao> lst = new List<Situacao>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_checagem_situacao t order by t.texto");
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

		internal List<Segmento> ObterSegmentos()
		{
			List<Segmento> lst = new List<Segmento>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.denominador from lov_empreendimento_segmento t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Segmento()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					Denominador = item["denominador"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TipoResponsavel> ObterEmpTipoResponsavel()
		{
			List<TipoResponsavel> lst = new List<TipoResponsavel>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_empreendimento_tipo_resp t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new TipoResponsavel()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Finalidade> ObterFinalidades()
		{
			List<Finalidade> lst = new List<Finalidade>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_titulo_finalidade t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Finalidade()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Finalidade> ObterFinalidadesExploracao()
		{
			List<Finalidade> lst = new List<Finalidade>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_crt_exp_flores_finalidade t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new Finalidade()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<AgendamentoVistoria> AgendamentoVistoria()
		{
			List<AgendamentoVistoria> lista = new List<AgendamentoVistoria>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select lra.id, lra.texto from lov_requerimento_agendamento lra order by id");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(new AgendamentoVistoria() { Id = Convert.ToInt32(reader["id"]), Texto = reader["texto"].ToString(), IsAtivo = true });
					}

					reader.Close();
				}
			}
			return lista;
		}

		internal List<Situacao> ObterSituacoesRequerimento()
		{
			List<Situacao> lst = new List<Situacao>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_requerimento_situacao t order by t.texto");
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

		internal List<ResponsavelFuncoes> ObterResponsavelFuncoes()
		{
			List<ResponsavelFuncoes> lst = new List<ResponsavelFuncoes>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_protocolo_resp_funcao t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new ResponsavelFuncoes()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ProcessoAtividadeItem> ObterAtividadesProcesso()
		{
			List<ProcessoAtividadeItem> lst = new List<ProcessoAtividadeItem>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select tpa.id, tpa.codigo, tpa.atividade from tab_atividade tpa where tpa.situacao=1 order by tpa.atividade");
			foreach (var item in daReader)
			{
				lst.Add(new ProcessoAtividadeItem()
				{
					Id = Convert.ToInt32(item["id"]),
					Codigo = Convert.ToInt32(item["codigo"]),
					Atividade = item["atividade"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ProcessoAtividadeItem> ObterAtividadesSolicitadaTodas()
		{
			List<ProcessoAtividadeItem> lst = new List<ProcessoAtividadeItem>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select tpa.id, tpa.codigo, tpa.atividade from tab_atividade tpa order by tpa.atividade");
			foreach (var item in daReader)
			{
				lst.Add(new ProcessoAtividadeItem()
				{
					Id = Convert.ToInt32(item["id"]),
					Codigo = Convert.ToInt32(item["codigo"]),
					Atividade = item["atividade"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ListaValor> ObterAtividadesCategoria()
		{
			List<ListaValor> lst = new List<ListaValor>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@" select t.id, t.cod_categoria || ' - ' || t.atividade texto from tab_atividade t where t.situacao = 1 and t.cod_categoria is not null order by t.cod_categoria ");
			foreach (var item in daReader)
			{
				lst.Add(new ListaValor()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Situacao> ObterSituacoesProcessoAtividade()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.codigo from lov_atividade_situacao c");
			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TituloCondicionantePeriodTipo> ObterTituloCondicionantePeriodicidades()
		{
			List<TituloCondicionantePeriodTipo> lst = new List<TituloCondicionantePeriodTipo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_titulo_cond_periodo_tipo c");
			foreach (var item in daReader)
			{
				lst.Add(new TituloCondicionantePeriodTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TituloCondicionanteSituacao> ObterTituloCondicionanteSituacoes()
		{
			List<TituloCondicionanteSituacao> lst = new List<TituloCondicionanteSituacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_titulo_cond_situacao c");
			foreach (var item in daReader)
			{
				lst.Add(new TituloCondicionanteSituacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ProtocoloTipo> ObterTiposProcesso()
		{
			List<ProtocoloTipo> lst = new List<ProtocoloTipo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.processo, c.documento, c.checagem_pendencia, 
																	c.checagem_roteiro, c.requerimento, c.fiscalizacao, interessado
																	from lov_protocolo_tipo c where c.tipo = 1 order by c.texto");
			foreach (var item in daReader)
			{
				lst.Add(new ProtocoloTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true,

					PossuiDocumento = item["documento"].ToString() == "2",
					PossuiProcesso = item["processo"].ToString() == "2",
					PossuiChecagemPendencia = item["checagem_pendencia"].ToString() == "2",
					PossuiChecagemRoteiro = item["checagem_roteiro"].ToString() == "2",
					PossuiRequerimento = item["requerimento"].ToString() == "2",
					PossuiFiscalizacao = item["fiscalizacao"].ToString() == "2",
					PossuiInteressado = item["interessado"].ToString() == "2",

					DocumentoObrigatorio = item["documento"].ToString() == "1",
					ProcessoObrigatorio = item["processo"].ToString() == "1",
					ChecagemPendenciaObrigatorio = item["checagem_pendencia"].ToString() == "1",
					ChecagemRoteiroObrigatorio = item["checagem_roteiro"].ToString() == "1",
					RequerimentoObrigatorio = item["requerimento"].ToString() == "1",
					FiscalizacaoObrigatorio = item["fiscalizacao"].ToString() == "1",
					InteressadoObrigatorio = item["interessado"].ToString() == "1",
					LabelInteressado = (Convert.ToInt32(item["id"]) != 2) ? "Interessado" : "Autuado"
				});
			}

			return lst;
		}

		internal List<Situacao> ObterSituacoesItemAnalise()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_analise_item_situacao c order by c.texto");
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

		internal List<Situacao> ObterSituacoesAtividadeProcesso()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from  c order by c.texto");
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

		internal List<ProtocoloTipo> ObterTiposDocumento()
		{
			List<ProtocoloTipo> lst = new List<ProtocoloTipo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto, c.processo, c.documento, c.checagem_pendencia, c.checagem_roteiro, c.requerimento, fiscalizacao, interessado from lov_protocolo_tipo c where c.tipo = 2 order by c.texto");
			foreach (var item in daReader)
			{
				lst.Add(new ProtocoloTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true,

					PossuiDocumento = item["documento"].ToString() == "2",
					PossuiProcesso = item["processo"].ToString() == "2",
					PossuiChecagemPendencia = item["checagem_pendencia"].ToString() == "2",
					PossuiChecagemRoteiro = item["checagem_roteiro"].ToString() == "2",
					PossuiRequerimento = item["requerimento"].ToString() == "2",
					PossuiFiscalizacao = item["fiscalizacao"].ToString() == "2",
					PossuiInteressado = item["interessado"].ToString() == "2",

					DocumentoObrigatorio = item["documento"].ToString() == "1",
					ProcessoObrigatorio = item["processo"].ToString() == "1",
					ChecagemPendenciaObrigatorio = item["checagem_pendencia"].ToString() == "1",
					ChecagemRoteiroObrigatorio = item["checagem_roteiro"].ToString() == "1",
					RequerimentoObrigatorio = item["requerimento"].ToString() == "1",
					FiscalizacaoObrigatorio = item["fiscalizacao"].ToString() == "1",
					InteressadoObrigatorio = item["interessado"].ToString() == "1",
					LabelInteressado = (Convert.ToInt32(item["id"]) != 12) ? "Interessado" : "Autuado"
				});
			}

			return lst;
		}

		internal List<TipoItem> ObterItemTipos()
		{
			List<TipoItem> lst = new List<TipoItem>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_roteiro_item_tipo t order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new TipoItem()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Situacao> ObterSituacaoChecagemPendencia()
		{
			List<Situacao> lst = new List<Situacao>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_checagem_pend_situacao t order by t.texto");
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

		internal List<Situacao> ObterSituacaoChecagemPendenciaItem()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_checagem_pend_item_sit t order by t.texto");
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

		internal List<TramitacaoArquivoTipo> ObterTramitacaoArquivoTipos()
		{
			List<TramitacaoArquivoTipo> lst = new List<TramitacaoArquivoTipo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_tramitacao_arq_tipo c");
			foreach (var item in daReader)
			{
				lst.Add(new TramitacaoArquivoTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TramitacaoArquivoModo> ObterTramitacaoArquivoModo()
		{
			List<TramitacaoArquivoModo> lst = new List<TramitacaoArquivoModo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_tramitacao_arq_modo c");
			foreach (var item in daReader)
			{
				lst.Add(new TramitacaoArquivoModo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Situacao> ObterAtividadeSolicitadaSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_atividade_situacao c");
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

		internal List<Situacao> ObterTituloSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_titulo_situacao c where c.tipo = 1 order by c.texto");
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

		internal List<Situacao> ObterTituloDeclaratorioSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_titulo_situacao c where c.tipo = 2 order by c.texto");
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

		#region Modelos de Titulos e Titulos

		internal List<TituloModeloTipo> ObterTituloModeloTiposLst()
		{
			List<TituloModeloTipo> lst = new List<TituloModeloTipo>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_tipo t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new TituloModeloTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TituloModeloProtocoloTipo> ObterTituloModeloProtocoloTipos()
		{
			List<TituloModeloProtocoloTipo> lst = new List<TituloModeloProtocoloTipo>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_prot_tipo t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new TituloModeloProtocoloTipo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TituloModeloPeriodoRenovacao> ObterTituloModeloPeriodoRenovacao()
		{
			List<TituloModeloPeriodoRenovacao> lst = new List<TituloModeloPeriodoRenovacao>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_per_renova t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new TituloModeloPeriodoRenovacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TituloModeloInicioPrazo> ObterTituloModeloInicioPrazo()
		{
			List<TituloModeloInicioPrazo> lst = new List<TituloModeloInicioPrazo>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_inicio_prazo t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new TituloModeloInicioPrazo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TituloModeloTipoPrazo> ObterTituloModeloTipoPrazo()
		{
			List<TituloModeloTipoPrazo> lst = new List<TituloModeloTipoPrazo>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_tipo_prazo t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new TituloModeloTipoPrazo()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<TituloModeloAssinante> ObterTituloModeloAssinantes()
		{
			List<TituloModeloAssinante> lst = new List<TituloModeloAssinante>();

			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_assinante t order by t.texto");
			foreach (var item in daReader)
			{
				lst.Add(new TituloModeloAssinante()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Acao> ObterAlterarSituacaoAcoes()
		{
			List<Acao> lst = new List<Acao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_titulo_acao_situacao c");
			foreach (var item in daReader)
			{
				lst.Add(new Acao()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Motivo> ObterMotivosEncerramento()
		{
			List<Motivo> lst = new List<Motivo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_titulo_situacao_motivo c where c.tipo = 1 order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new Motivo()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Motivo> ObterDeclaratorioMotivosEncerramento()
		{
			List<Motivo> lst = new List<Motivo>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_titulo_situacao_motivo c where c.tipo = 2 and c.id != 14 order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new Motivo()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

        internal List<TituloModeloTipoDocumento> ObterTituloModeloTipoDocumento()
        {
            List<TituloModeloTipoDocumento> lst = new List<TituloModeloTipoDocumento>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_titulo_modelo_tipo_doc t order by t.texto");
            foreach (var item in daReader)
            {
                lst.Add(new TituloModeloTipoDocumento()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

		#endregion

		internal List<AtividadeAgrupador> ObterAgrupadoresAtividade()
		{
			List<AtividadeAgrupador> lst = new List<AtividadeAgrupador>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.nome from tab_atividade_agrupador t order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new AtividadeAgrupador()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<SetorAgrupador> ObterAgrupadoresSetor()
		{
			List<SetorAgrupador> lst = new List<SetorAgrupador>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.nome from tab_setor_agrupador t order by 2");
			foreach (var item in daReader)
			{
				lst.Add(new SetorAgrupador()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#region Atividade
		internal List<AtividadeCaracterizacao> ObterAtividadesCaracterizacoes()
		{
			List<AtividadeCaracterizacao> lst = new List<AtividadeCaracterizacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.atividade, t.caracterizacao, t.titulo_generico from cnf_atividade_caracterizacoes t");
			foreach (var item in daReader)
			{
				lst.Add(new AtividadeCaracterizacao()
				{
					AtividadeId = Convert.ToInt32(item["atividade"]),
					CaracterizacaoTipoId = Convert.ToInt32(item["caracterizacao"]),
					IsTituloGenerico = Convert.ToBoolean(item["titulo_generico"])
				});
			}

			return lst;
		}

		internal int ObterAtividadeId(int atividadeCodigo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id from tab_atividade a where a.codigo = :codigo");

				comando.AdicionarParametroEntrada("codigo", atividadeCodigo, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal int ObterAtividadeCodigo(int atividadeId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.codigo from tab_atividade a where a.id = :id");

				comando.AdicionarParametroEntrada("id", atividadeId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		internal List<NivelPrecisao> ObterAutuacaoNiveisPrecisao()
		{
			//Implementar Query
			List<NivelPrecisao> lst = new List<NivelPrecisao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_autuacao_projeto_geo_nivel c"); // Inserir Select
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


        internal List<Lista> ObterDiasSemana()
        {
            List<Lista> lst = new List<Lista>();

            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select d.id, d.texto  from lov_dia_semana d where d.dia_util = 1 order by d.id");
            foreach (var item in daReader)
            {
                lst.Add(new Lista()
                {
                    Id = Convert.ToString(item["id"]),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }



		#region Fiscalizacao

		internal List<Lista> ObterComplementoDadosRespostas()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_fisc_compl_dad_resp_padrao c order by c.id");

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

		internal List<Lista> ObterComplementoDadosRendaMensal()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_fisc_compl_dad_rend_mensal c order by c.id");

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

		internal List<Lista> ObterComplementoDadosNivelEscolaridade()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_fisc_compl_dad_nivel_escol c order by c.id");

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

		internal List<ReservaLegalLst> ObterComplementoDadosReservaLegalTipo()
		{
			List<ReservaLegalLst> lst = new List<ReservaLegalLst>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_fisc_compl_dad_reserva_leg t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new ReservaLegalLst()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterMaterialApreendidoTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_fisc_mate_apreendido_tipo t order by t.texto");

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

		internal List<Lista> ObterInfracaoClassificacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@" select l.id, l.texto from lov_cnf_fisc_infracao_classif l order by l.texto ");

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

		internal List<Lista> ObterInfracaoCodigoReceita()
		{
			List<Lista> lst = new List<Lista>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@" select l.id, (l.texto || ' - ' || l.descricao) texto from lov_fisc_infracao_codigo_rece l where l.ativo = 1 order by l.texto ");

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

        public List<Lista> ObterCodigoReceita(int idFiscalizacao)
        {
            List<Lista> lst = new List<Lista>();
            var bancoDeDados = BancoDeDados.ObterInstancia(); 
            var retorno = DaHelper.ObterLista(bancoDeDados.CriarComando(@"  select l.id, (l.texto || ' - ' || l.descricao) texto from lov_fisc_infracao_codigo_rece l 
                                                                where l.ativo = 1 OR l.id in 
                                                                (SELECT M.CODIGO_RECEITA FROM TAB_FISC_MULTA M WHERE M.FISCALIZACAO = " + idFiscalizacao + ")"));
            foreach (var item in retorno)
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

		internal List<Lista> ObterFiscalizacaoSituacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_fiscalizacao_situacao l order by l.texto ");

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

		internal List<Lista> ObterFiscalizacaoSerie()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_fiscalizacao_serie l order by l.texto ");

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

		#region Objeto Infracao

		internal List<CaracteristicaSoloAreaDanificada> ObterFiscalizacaoObjetoInfracaoCaracteristicaSolo()
		{
			List<CaracteristicaSoloAreaDanificada> lst = new List<CaracteristicaSoloAreaDanificada>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_fisc_obj_infra_carac_solo t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new CaracteristicaSoloAreaDanificada()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Lista> ObterFiscalizacaoObjetoInfracaoSerie()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_fisc_obj_infra_serie c order by c.id");

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

		internal List<Lista> ObterAcompanhamentoFiscalizacaoSituacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_acomp_fisc_situacao l order by l.texto ");

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

		internal List<CaracteristicaSoloAreaDanificada> ObterAcompanhamentoFiscalizacaoCaracteristicaSolo()
		{
			List<CaracteristicaSoloAreaDanificada> lst = new List<CaracteristicaSoloAreaDanificada>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto, l.codigo from lov_acomp_fisc_area_danif l order by l.id ");

			foreach (var item in daReader)
			{
				lst.Add(new CaracteristicaSoloAreaDanificada()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<ReservaLegalLst> ObterAcompanhamentoFiscalizacaoReservaLegalTipo()
		{
			List<ReservaLegalLst> lst = new List<ReservaLegalLst>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto, t.codigo from lov_acomp_fisc_reserva_leg t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new ReservaLegalLst()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					Codigo = Convert.ToInt32(item["codigo"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion

		#region Cadastro Ambiental Rural - CAR

		#region Solicitacao
		internal List<Lista> ObterCadastroAmbientalRuralSolicitacaoSituacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_car_solicitacao_situacao t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = Convert.ToString(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

        internal List<Lista> ObterSicarSituacao()
        {
            List<Lista> lst = new List<Lista>();
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_situacao_envio_sicar t order by t.id");
            foreach (var item in daReader)
            {
                lst.Add(new Lista()
                {
                    Id = Convert.ToString(item["id"]),
                    Texto = Convert.ToString(item["texto"]),
                    IsAtivo = true
                });
            }
            return lst;
        }

		internal List<Lista> ObterCadastroAmbientalRuralSolicitacaoOrigem()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.texto from lov_car_solicitacao_origem t order by t.id");
			foreach (var item in daReader)
			{
				lst.Add(new Lista()
				{
					Id = Convert.ToString(item["id"]),
					Texto = Convert.ToString(item["texto"]),
					IsAtivo = true
				});
			}
			return lst;
		}

		#endregion Recibo

		#endregion Cadastro Ambiental Rural - CAR

		#region Configuracoes Fiscalizacao

		internal List<Lista> ObterConfiguracaoFiscalizacaoCamposTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_cnf_fisc_infracao_camp_tip l order by l.texto ");

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

		internal List<Lista> ObterConfiguracaoFiscalizacaoCamposUnidade()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_cnf_fisc_infracao_camp_uni l order by l.texto ");

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

		internal List<Lista> OrgaoParceirosConveniadosSituacoes()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_orgao_parc_conv_situacao l order by l.texto ");

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

		#region Credenciado

		public Dictionary<Int32, String> ObterCredenciadoDiretorioArquivoTemp()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 1", schema: UsuarioCredenciado);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}

		public Dictionary<Int32, String> ObterCredenciadoDiretorioArquivo()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 2", schema: UsuarioCredenciado);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}

		internal List<Situacao> ObterCredenciadoSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto nome from lov_credenciado_situacao s order by s.texto", schema: UsuarioCredenciado);
			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<Situacao> ObterCredenciadoTipos()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto nome from lov_credenciado_tipo s order by s.texto", schema: UsuarioCredenciado);
			foreach (var item in daReader)
			{
				lst.Add(new Situacao()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}
			return lst;
		}

		internal List<ProcessoAtividadeItem> ObtertividadesProcessoCredenciado()
		{
			List<ProcessoAtividadeItem> lst = new List<ProcessoAtividadeItem>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select tpa.id, tpa.codigo, tpa.atividade from tab_atividade tpa 
																	where tpa.situacao=1 and tpa.id in (select id from tab_atividade where exibir_credenciado = 1) 
																	order by tpa.atividade");
			foreach (var item in daReader)
			{
				lst.Add(new ProcessoAtividadeItem()
				{
					Id = Convert.ToInt32(item["id"]),
					Codigo = Convert.ToInt32(item["codigo"]),
					Atividade = item["atividade"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<AtividadeAgrupador> ObterAgrupadoresAtividadeCredenciado()
		{
			List<AtividadeAgrupador> lst = new List<AtividadeAgrupador>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select t.id, t.nome from tab_atividade_agrupador t 
																	where t.id in (select id from tab_atividade where exibir_credenciado = 1) order by t.nome");
			foreach (var item in daReader)
			{
				lst.Add(new AtividadeAgrupador()
				{
					Id = Convert.ToInt32(item["id"]),
					Nome = item["nome"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Situacao> ObterHabilitacaoCFOSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto from lov_hab_emissao_cfo_situacao s order by s.texto");
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

		internal List<Lista> ObterHabilitacaoCFOMotivos()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto from lov_hab_emissao_cfo_motivo s order by s.texto");
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

		internal List<Lista> ObterLoteSituacoes()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto from lov_lote_situacao s order by s.texto");
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

		#region Publico

		public Dictionary<Int32, String> ObterDiretorioArquivoTempPublicoGeo()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 1", schema: UsuarioPublicoGeo);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}
			return lstDiretorios;
		}

		public Dictionary<Int32, String> ObterDiretorioArquivoPublicoGeo()
		{
			Dictionary<Int32, String> lstDiretorios = new Dictionary<int, string>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.raiz from cnf_arquivo c where c.ativo = 1 and tipo = 2", schema: UsuarioPublicoGeo);
			foreach (var item in daReader)
			{
				lstDiretorios.Add(Convert.ToInt32(item["id"]), item["raiz"].ToString());
			}

			return lstDiretorios;
		}

		#endregion

		#region Configuração Vegetal

		internal List<Situacao> ObterIngredienteAtivoSituacoes()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_ingrediente_ativo_situacao c order by c.texto");
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

		internal List<ListaValor> ObterMensagensAgrotoxicosDesativados()
		{
			List<ListaValor> lst = new List<ListaValor>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_agrotoxico_motivo c order by c.id");
			foreach (var item in daReader)
			{
				lst.Add(new ListaValor()
				{
					Id = Convert.ToInt32(item["id"]),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		internal List<Lista> ObterCultivarTipo()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_cultivar_tipo_producao c order by 1");
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

		internal List<Lista> ObterDeclaracaoAdicional()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select d.id, d.texto from lov_cultivar_declara_adicional d");
			foreach (var item in daReader)
			{
				lst.Add(new Lista() { 
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lst;
		}

		#endregion

		#region PTV

		internal List<Lista> ObterPTVSituacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_ptv_situacao l order by l.id");

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

		internal List<Lista> ObterPTVSolicitacaoSituacao()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_solicitacao_ptv_situacao l order by l.id", schema: UsuarioCredenciado);

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

		internal List<Lista> ObterPTVUnidadeMedida()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_crt_uni_prod_uni_medida l order by l.id");

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

		internal List<Lista> ObterTipoTransporte()
		{
			List<Lista> lst = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select l.id, l.texto from lov_tipo_transporte l order by l.id");

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

		#region Agrotóxico

		internal List<Lista> ObterAgrotoxicoIngredienteAtivoUnidadeMedida()
		{
			List<Lista> lista = new List<Lista>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select c.id, c.texto from lov_agrotoxico_uni_medida c order by c.texto");
			foreach (var item in daReader)
			{
				lista.Add(new Lista()
				{
					Id = item["id"].ToString(),
					Texto = item["texto"].ToString(),
					IsAtivo = true
				});
			}

			return lista;
		}

		#endregion

		#region Relatorios

		internal List<ListaValor> ObterTipoRelatorioMapa()
		{
			List<ListaValor> lst = new List<ListaValor>();
			lst.Add(new ListaValor { Texto = "CFO/CFOC", Id = 1, IsAtivo = true });
			lst.Add(new ListaValor { Texto = "PTV", Id = 2, IsAtivo = true });
			return lst;
		}

		#endregion
	}
}