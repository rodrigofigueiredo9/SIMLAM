using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCFOCFOC.Data
{
	public class CFODa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private string EsquemaBanco { get; set; }

		#endregion

		public CFODa()
		{
			EsquemaBanco = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
		}

		public CFORelatorio Obter(int id, int credenciadoID, BancoDeDados banco = null)
		{
			CFORelatorio entidade = new CFORelatorio();

			#region Credenciado

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Dados

				Comando comando = bancoDeDados.CriarComando(@"select t.tid, t.situacao, t.numero, p.tipo produtor_tipo, nvl(p.nome, p.razao_social) produtor_nome_razao, nvl(p.cpf, p.cnpj) produtor_cpf_cnpj, 
				t.empreendimento, t.nome_laboratorio, t.numero_laudo_resultado_analise, le.sigla estado_sigla, lm.texto municipio, t.numero_lacre, t.numero_porao, t.numero_container, 
				t.produto_especificacao, t.partida_lacrada_origem, t.validade_certificado, t.informacoes_complement_html, lee.sigla estado_emissao_sigla, lme.texto municipio_emissao, t.data_ativacao , t.serie
				from tab_cfo t, ins_pessoa p, lov_estado le, lov_municipio lm, lov_estado lee, lov_municipio lme 
				where t.produtor = p.id and le.id(+) = t.estado and lm.id(+) = t.municipio and lee.id(+) = t.estado_emissao and lme.id(+) = t.municipio_emissao and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Tid = reader.GetValue<string>("tid");
						entidade.Situacao = reader.GetValue<int>("situacao");
                        entidade.Numero = reader.GetValue<string>("numero") + (string.IsNullOrEmpty(reader.GetValue<string>("serie")) ? "" : "/" + reader.GetValue<string>("serie")); 
						entidade.Produtor.Tipo = reader.GetValue<int>("produtor_tipo");
						entidade.Produtor.NomeRazaoSocial = reader.GetValue<string>("produtor_nome_razao");
						entidade.Produtor.CPFCNPJ = reader.GetValue<string>("produtor_cpf_cnpj");
						entidade.Empreendimento.Id = reader.GetValue<int>("empreendimento");

						entidade.NomeLaboratorio = reader.GetValue<string>("nome_laboratorio");
						entidade.NumeroLaudoResultadoAnalise = reader.GetValue<string>("numero_laudo_resultado_analise");
						entidade.EstadoSigla = reader.GetValue<string>("estado_sigla");
						entidade.MunicipioTexto = reader.GetValue<string>("municipio");

						entidade.NumeroLacre = reader.GetValue<string>("numero_lacre");
						entidade.NumeroPorao = reader.GetValue<string>("numero_porao");
						entidade.NumeroContainer = reader.GetValue<string>("numero_container");

						entidade.ProdutoEspecificacao = reader.GetValue<int>("produto_especificacao");
						entidade.PartidaLacradaOrigem = reader.GetValue<int>("partida_lacrada_origem") == ConfiguracaoSistema.SIM;
						entidade.ValidadeCertificado = reader.GetValue<int>("validade_certificado");
						entidade.DeclaracaoAdicionalHtml = reader.GetValue<string>("informacoes_complement_html");

						entidade.EstadoEmissaoSigla = reader.GetValue<string>("estado_emissao_sigla");
						entidade.MunicipioEmissaoTexto = reader.GetValue<string>("municipio_emissao");

						entidade.DataAtivacao = reader.GetValue<DateTime>("data_ativacao").ToShortDateString();
					}

					reader.Close();
				}

				#endregion Dados

				#region Produtos

				comando = bancoDeDados.CriarComando(@"
				select cp.id, cp.tid, cp.unidade_producao, i.codigo_up, c.texto cultura, cc.cultivar, lu.texto unidade_medida, cp.quantidade, cp.inicio_colheita, cp.fim_colheita, cp.exibe_kilos
				from tab_cfo_produto cp, ins_crt_unidade_prod_unidade i, tab_cultura c, tab_cultura_cultivar cc, lov_crt_uni_prod_uni_medida lu 
				where i.id = cp.unidade_producao and c.id = i.cultura and cc.id = i.cultivar and i.estimativa_unid_medida = lu.id and cp.cfo = :cfo", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfo", entidade.Id, DbType.Int32);

				using (IDataReader dr = bancoDeDados.ExecutarReader(comando))
				{
					while (dr.Read())
					{
						entidade.Produtos.Add(new IdentificacaoProdutoRelatorio()
						{
							Id = dr.GetValue<int>("id"),
							CodigoUP = dr.GetValue<string>("codigo_up"),
							CulturaTexto = dr.GetValue<string>("cultura"),
							CultivarTexto = dr.GetValue<string>("cultivar"),
							UnidadeMedida = dr.GetValue<string>("unidade_medida"),
							Quantidade = dr.GetValue<decimal>("quantidade"),
							DataInicioColheita = dr.GetValue<DateTime>("inicio_colheita").ToShortDateString(),
                            ExibeQtdKg = dr.GetValue<string>("exibe_kilos") == "1" ? true : false,
							DataFimColheita = dr.GetValue<DateTime>("fim_colheita").ToShortDateString()
						});
					}

					dr.Close();
				}

				#endregion

				#region Tratamentos Fitossanitarios

				comando = bancoDeDados.CriarComando(@"select c.id, c.produto_comercial, c.ingrediente_ativo, c.dose, c.praga_produto, c.modo_aplicacao from tab_cfo_trata_fitossa c where c.cfo = :cfo", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfo", entidade.Id, DbType.Int32);

				using (IDataReader dr = bancoDeDados.ExecutarReader(comando))
				{
					while (dr.Read())
					{
						entidade.TratamentosFitossanitarios.Add(new TratamentoFitossanitarioRelatorio()
						{
							Id = dr.GetValue<int>("id"),
							ProdutoComercial = dr.GetValue<string>("produto_comercial"),
							IngredienteAtivo = dr.GetValue<string>("ingrediente_ativo"),
							Dose = dr.GetValue<decimal>("dose"),
							PragaProduto = dr.GetValue<string>("praga_produto"),
							ModoAplicacao = dr.GetValue<string>("modo_aplicacao")
						});
					}

					dr.Close();
				}

				#endregion

				#region Responsavel Tecnico

				comando = bancoDeDados.CriarComando(@"
				select nvl(p.nome, p.razao_social) nome_razao, (select pp.registro from tab_pessoa_profissao pp where pp.pessoa = p.id) registro, 
				(select (case when h.extensao_habilitacao = 1 then h.numero_habilitacao||'-ES' else h.numero_habilitacao end) from tab_hab_emi_cfo_cfoc h where h.responsavel = c.id) numero_habilitacao 
				from tab_credenciado c, tab_pessoa p where p.id = c.pessoa and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", credenciadoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.ResponsavelTecnico.NomeRazao = reader.GetValue<string>("nome_razao");
						entidade.ResponsavelTecnico.NumeroHabilitacao = reader.GetValue<string>("numero_habilitacao");
						entidade.ResponsavelTecnico.Registro = reader.GetValue<string>("registro");
					}

					reader.Close();
				}

				#endregion Responsavel Tecnico
			}

			#endregion Credenciado

			#region Interno

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"
				select e.denominador empreendimento_denominador, e.cnpj empreendimento_cnpj, ee.correspondencia emp_endereco_correspondencia, ee.cep empreendimento_cep, 
				ee.logradouro empreendimento_logradouro, ee.bairro empreendimento_bairro, lm.texto empreendimento_municipio, ee.distrito empreendimento_distrito, le.sigla empreendimento_estado_sigla, 
				(select up.propriedade_codigo from crt_unidade_producao up where up.empreendimento = e.id) propriedade_codigo 
				from tab_empreendimento e, tab_empreendimento_endereco ee, lov_estado le, lov_municipio lm 
				where ee.empreendimento = e.id and le.id = ee.estado and lm.id = ee.municipio and ee.correspondencia = 0
				and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.PropriedadeCodigo = reader.GetValue<string>("propriedade_codigo");
						entidade.Empreendimento.NomeRazao = reader.GetValue<string>("empreendimento_denominador");
						entidade.Empreendimento.CNPJ = reader.GetValue<string>("empreendimento_cnpj");

						entidade.Empreendimento.Enderecos.Add(new EnderecoRelatorio()
						{
							Correspondencia = reader.GetValue<int?>("emp_endereco_correspondencia"),
							Cep = reader.GetValue<string>("empreendimento_cep"),
							Logradouro = reader.GetValue<string>("empreendimento_logradouro"),
							Bairro = reader.GetValue<string>("empreendimento_bairro"),
							Distrito = reader.GetValue<string>("empreendimento_distrito"),
							MunicipioTexto = reader.GetValue<string>("empreendimento_municipio"),
							EstadoSigla = reader.GetValue<string>("empreendimento_estado_sigla")
						});
					}

					reader.Close();
				}

				#endregion Empreendimento
			}

			#endregion Interno

			return entidade;
		}

		public CFORelatorio ObterHistorico(int id, string tid, int credenciadoID, BancoDeDados banco = null)
		{
			CFORelatorio entidade = new CFORelatorio();
			string credenciadoTID = string.Empty;
			int hst_id = 0;

			#region Credenciado

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Dados

				Comando comando = bancoDeDados.CriarComando(@"
				select t.id,
					t.situacao_id,
					t.numero,
					t.produtor_id,
					t.produtor_tid,
					t.empreendimento_id,
					t.empreendimento_tid,
					t.nome_laboratorio,
					t.numero_laudo_resultado_analise,
					le.sigla estado_sigla,
				    lee.sigla as estado_emissao_sigla,
				    t.estado_emissao_texto,
					t.municipio_texto,
					t.municipio_emissao_texto,
					t.numero_lacre,
					t.numero_porao,
					t.numero_container,
					t.produto_especificacao,
					t.partida_lacrada_origem,
					t.validade_certificado,
					t.informacoes_complement_html,
					t.data_ativacao,
					t.data_execucao,
                    t.serie
				from hst_cfo t, lov_estado le, lov_estado lee
				where le.id(+) = t.estado_id
				and lee.id(+) = t.estado_emissao_id
				and t.cfo_id = :id
				and t.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst_id = reader.GetValue<int>("id");

						entidade.Id = id;
						entidade.Situacao = reader.GetValue<int>("situacao_id");
                        entidade.Numero = reader.GetValue<string>("numero") + (string.IsNullOrEmpty(reader.GetValue<string>("serie")) ? "" : "/" + reader.GetValue<string>("serie")); 
						entidade.Produtor.Id = reader.GetValue<int>("produtor_id");
						entidade.Produtor.Tid = reader.GetValue<string>("produtor_tid");
						entidade.Empreendimento.Id = reader.GetValue<int>("empreendimento_id");
						entidade.Empreendimento.Tid = reader.GetValue<string>("empreendimento_tid");

						entidade.NomeLaboratorio = reader.GetValue<string>("nome_laboratorio");
						entidade.NumeroLaudoResultadoAnalise = reader.GetValue<string>("numero_laudo_resultado_analise");
						entidade.EstadoSigla = reader.GetValue<string>("estado_sigla");
						entidade.MunicipioTexto = reader.GetValue<string>("municipio_texto");

						entidade.NumeroLacre = reader.GetValue<string>("numero_lacre");
						entidade.NumeroPorao = reader.GetValue<string>("numero_porao");
						entidade.NumeroContainer = reader.GetValue<string>("numero_container");

						entidade.ProdutoEspecificacao = reader.GetValue<int>("produto_especificacao");
						entidade.PartidaLacradaOrigem = reader.GetValue<int>("partida_lacrada_origem") == ConfiguracaoSistema.SIM;
						entidade.ValidadeCertificado = reader.GetValue<int>("validade_certificado");
						entidade.DeclaracaoAdicionalHtml = reader.GetValue<string>("informacoes_complement_html");

						entidade.EstadoEmissaoSigla = reader.GetValue<string>("estado_emissao_sigla");
						entidade.MunicipioEmissaoTexto = reader.GetValue<string>("municipio_emissao_texto");

						entidade.DataAtivacao = reader.GetValue<DateTime>("data_ativacao").ToShortDateString();
						entidade.DataExecucao = reader.GetValue<DateTime>("data_execucao");
					}

					reader.Close();
				}

				#endregion Dados

				#region Produtos

				comando = bancoDeDados.CriarComando(@"
				select cp.id,
					cp.tid,
					cp.unidade_producao_id,
					cp.unidade_producao_tid,
					cp.quantidade,
					cp.inicio_colheita,
					cp.fim_colheita,
                    cp.exibe_kilos
				from hst_cfo_produto cp
				where cp.id_hst = :hst_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst_id", hst_id, DbType.Int32);

				using (IDataReader dr = bancoDeDados.ExecutarReader(comando))
				{
					while (dr.Read())
					{
						entidade.Produtos.Add(new IdentificacaoProdutoRelatorio()
						{
							Id = dr.GetValue<int>("id"),
							UnidadeProducaoID = dr.GetValue<int>("unidade_producao_id"),
							UnidadeProducaoTID = dr.GetValue<string>("unidade_producao_tid"),
							Quantidade = dr.GetValue<decimal>("quantidade"),
							DataInicioColheita = dr.GetValue<DateTime>("inicio_colheita").ToShortDateString(),
                            ExibeQtdKg = dr.GetValue<string>("exibe_kilos") == "1" ? true : false,
							DataFimColheita = dr.GetValue<DateTime>("fim_colheita").ToShortDateString()
						});
					}

					dr.Close();
				}

				#endregion

				#region Tratamentos Fitossanitarios

				comando = bancoDeDados.CriarComando(@"
				select c.id,
					c.produto_comercial,
					c.ingrediente_ativo,
					c.dose,
					c.praga_produto,
					c.modo_aplicacao
				from hst_cfo_trata_fitossa c
				where c.id_hst = :hst_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst_id", hst_id, DbType.Int32);

				using (IDataReader dr = bancoDeDados.ExecutarReader(comando))
				{
					while (dr.Read())
					{
						entidade.TratamentosFitossanitarios.Add(new TratamentoFitossanitarioRelatorio()
						{
							Id = dr.GetValue<int>("id"),
							ProdutoComercial = dr.GetValue<string>("produto_comercial"),
							IngredienteAtivo = dr.GetValue<string>("ingrediente_ativo"),
							Dose = dr.GetValue<decimal>("dose"),
							PragaProduto = dr.GetValue<string>("praga_produto"),
							ModoAplicacao = dr.GetValue<string>("modo_aplicacao")
						});
					}

					dr.Close();
				}

				#endregion

				#region Responsavel Tecnico

				comando = bancoDeDados.CriarComando(@"
				select c.tid credenciado_tid,
					nvl(p.nome, p.razao_social) nome_razao,
					(select pp.registro from hst_pessoa_profissao pp where pp.id_hst = p.id) registro
				from hst_credenciado c, hst_pessoa p
				where p.pessoa_id = c.pessoa_id
				and p.tid = c.pessoa_tid
				and c.credenciado_id = :id
				and c.tid = (select hc.tid from hst_credenciado hc where hc.credenciado_id = :id and hc.data_execucao = 
				(select max(h.data_execucao) from hst_credenciado h where h.credenciado_id = :id and h.data_execucao <= :data_execucao))", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", credenciadoID, DbType.Int32);
				comando.AdicionarParametroEntrada("data_execucao", entidade.DataExecucao, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						credenciadoTID = reader.GetValue<string>("credenciado_tid");
						entidade.ResponsavelTecnico.NomeRazao = reader.GetValue<string>("nome_razao");
						entidade.ResponsavelTecnico.Registro = reader.GetValue<string>("registro");
					}

					reader.Close();
				}

				#endregion Responsavel Tecnico
			}

			#endregion Credenciado

			#region Interno

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Produtor

				Comando comando = bancoDeDados.CriarComando(@"
				select p.tipo produtor_tipo,
					nvl(p.nome, p.razao_social) produtor_nome_razao,
					nvl(p.cpf, p.cnpj) produtor_cpf_cnpj
				from hst_pessoa p
				where p.pessoa_id = :id
				and p.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Produtor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, entidade.Produtor.Tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Produtor.Tipo = reader.GetValue<int>("produtor_tipo");
						entidade.Produtor.NomeRazaoSocial = reader.GetValue<string>("produtor_nome_razao");
						entidade.Produtor.CPFCNPJ = reader.GetValue<string>("produtor_cpf_cnpj");
					}

					reader.Close();
				}

				#endregion Produtor

				#region Empreendimento

				comando = bancoDeDados.CriarComando(@"
				select e.denominador empreendimento_denominador,
					e.cnpj empreendimento_cnpj,
					ee.correspondencia emp_endereco_correspondencia,
					ee.cep empreendimento_cep,
					ee.logradouro empreendimento_logradouro,
					ee.bairro empreendimento_bairro,
					ee.municipio_texto empreendimento_municipio,
					ee.distrito empreendimento_distrito,
					le.sigla empreendimento_estado_sigla,
					(select up.propriedade_codigo from hst_crt_unidade_producao up
						where up.tid = (select h.tid from hst_crt_unidade_producao h where h.empreendimento_id = :id and h.data_execucao = 
						(select max(hu.data_execucao) from hst_crt_unidade_producao hu where hu.empreendimento_id = :id and hu.data_execucao <= :data_execucao))) propriedade_codigo
				from hst_empreendimento e, hst_empreendimento_endereco ee, lov_estado le
				where ee.id_hst = e.id
				and le.id = ee.estado_id
				and ee.correspondencia = 0
				and e.empreendimento_id = :id
				and e.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, entidade.Empreendimento.Tid);
				comando.AdicionarParametroEntrada("data_execucao", entidade.DataExecucao, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.PropriedadeCodigo = reader.GetValue<string>("propriedade_codigo");
						entidade.Empreendimento.NomeRazao = reader.GetValue<string>("empreendimento_denominador");
						entidade.Empreendimento.CNPJ = reader.GetValue<string>("empreendimento_cnpj");

						entidade.Empreendimento.Enderecos.Add(new EnderecoRelatorio()
						{
							Correspondencia = reader.GetValue<int?>("emp_endereco_correspondencia"),
							Cep = reader.GetValue<string>("empreendimento_cep"),
							Logradouro = reader.GetValue<string>("empreendimento_logradouro"),
							Bairro = reader.GetValue<string>("empreendimento_bairro"),
							Distrito = reader.GetValue<string>("empreendimento_distrito"),
							MunicipioTexto = reader.GetValue<string>("empreendimento_municipio"),
							EstadoSigla = reader.GetValue<string>("empreendimento_estado_sigla")
						});
					}

					reader.Close();
				}

				#endregion Empreendimento

				#region Produtos

				entidade.Produtos.ForEach(produto =>
				{
					comando = bancoDeDados.CriarComando(@"
					select u.codigo_up,
						c.texto                        cultura,
						cc.cultivar_nome               cultivar,
						u.estimativa_unid_medida_texto unidade_medida
					from hst_crt_unidade_prod_unidade u,
						hst_cultura                   c,
						hst_cultura_cultivar          cc
					where c.cultura_id = u.cultura_id
					and c.tid = u.cultura_tid
					and cc.cultivar_id(+) = u.cultivar_id
					and cc.tid(+) = u.cultivar_tid
					and u.unidade_producao_unidade_id = :id
					and u.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", produto.UnidadeProducaoID, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, produto.UnidadeProducaoTID);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							produto.CodigoUP = reader.GetValue<string>("codigo_up");
							produto.CulturaTexto = reader.GetValue<string>("cultura");
							produto.CultivarTexto = reader.GetValue<string>("cultivar");
							produto.UnidadeMedida = reader.GetValue<string>("unidade_medida");
						}

						reader.Close();
					}
				});

				#endregion Produtos

				#region Responsavel Tecnico

				comando = bancoDeDados.CriarComando(@"
				select (case when ha.extensao_habilitacao = 1 then ha.numero_habilitacao||'-ES' else ha.numero_habilitacao end) numero_habilitacao 
				from hst_hab_emi_cfo_cfoc ha where ha.tid = (select h.tid from hst_hab_emi_cfo_cfoc h where h.responsavel_id = :id and h.data_execucao =
				(select max(hh.data_execucao) from hst_hab_emi_cfo_cfoc hh where hh.responsavel_id = :id and hh.data_execucao <= :data_execucao))", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", credenciadoID, DbType.Int32);
				comando.AdicionarParametroEntrada("data_execucao", entidade.DataExecucao, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.ResponsavelTecnico.NumeroHabilitacao = reader.GetValue<string>("numero_habilitacao");
					}

					reader.Close();
				}

				#endregion Responsavel Tecnico
			}

			#endregion Interno

			return entidade;
		}
	}
}