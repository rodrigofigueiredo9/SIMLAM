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
	public class CFOCDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private string EsquemaBanco { get; set; }

		#endregion

		public CFOCDa()
		{
			EsquemaBanco = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
		}

		public CFOCRelatorio Obter(int id, int credenciadoID, BancoDeDados banco = null)
		{
			CFOCRelatorio entidade = new CFOCRelatorio();

			#region Credenciado

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Dados

				Comando comando = bancoDeDados.CriarComando(@"
				select t.tid, t.situacao, t.numero, t.empreendimento, t.nome_laboratorio, t.numero_laudo_resultado_analise, le.sigla estado_sigla, lm.texto municipio, t.numero_lacre, t.numero_porao, t.numero_container, 
				t.produto_especificacao, t.partida_lacrada_origem, t.validade_certificado, nvl(t.informacoes_complement_html, t.informacoes_complementares) informacoes_complement_html, lee.sigla estado_emissao_sigla, lme.texto municipio_emissao, t.data_ativacao 
				from tab_cfoc t, lov_estado le, lov_municipio lm, lov_estado lee, lov_municipio lme 
				where le.id(+) = t.estado and lm.id(+) = t.municipio and lee.id(+) = t.estado_emissao and lme.id(+) = t.municipio_emissao and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Tid = reader.GetValue<string>("tid");
						entidade.Situacao = reader.GetValue<int>("situacao");
						entidade.Numero = reader.GetValue<string>("numero");
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

				comando = bancoDeDados.CriarComando(@"select d.id, d.tid, d.lote, d.codigo_lote, d.data_criacao, d.cultura, d.cultivar, sum(d.quantidade) quantidade, d.unidade_medida 
					from (select cp.id, cp.tid, cp.lote, l.codigo_uc || l.ano || lpad(l.numero, 4, '0') codigo_lote, l.data_criacao, c.texto cultura, 
					cc.cultivar, li.quantidade, (select lu.texto from lov_crt_uni_prod_uni_medida lu where lu.id = li.unidade_medida) unidade_medida 
					from tab_cfoc_produto cp, tab_lote l, tab_lote_item li, tab_cultura c, tab_cultura_cultivar cc where l.id = cp.lote and li.lote = 
					l.id and c.id = li.cultura and cc.id = li.cultivar and cp.cfoc = :id) d group by d.id, d.tid, d.lote, d.codigo_lote, 
					d.data_criacao, d.cultura, d.cultivar, d.unidade_medida", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						entidade.Produtos.Add(new IdentificacaoProdutoRelatorio()
						{
							Id = reader.GetValue<int>("id"),
							LoteCodigo = reader.GetValue<string>("codigo_lote"),
							CulturaTexto = reader.GetValue<string>("cultura"),
							CultivarTexto = reader.GetValue<string>("cultivar"),
							UnidadeMedida = reader.GetValue<string>("unidade_medida"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							DataConsolidacao = reader.GetValue<DateTime>("data_criacao").ToShortDateString()
						});
					}

					reader.Close();
				}

				#endregion

				#region Tratamentos Fitossanitarios

				comando = bancoDeDados.CriarComando(@"select c.id, c.produto_comercial, c.ingrediente_ativo, c.dose, c.praga_produto, c.modo_aplicacao from tab_cfoc_trata_fitossa c where c.cfoc = :cfoc", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);

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
				(select uc.codigo_uc from crt_unidade_consolidacao uc where uc.empreendimento = e.id) codigo_uc 
				from tab_empreendimento e, tab_empreendimento_endereco ee, lov_estado le, lov_municipio lm 
				where ee.empreendimento = e.id and le.id = ee.estado and lm.id = ee.municipio and ee.correspondencia = 0
				and e.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.UCCodigo = reader.GetValue<string>("codigo_uc");
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

		public CFOCRelatorio ObterHistorico(int id, string tid, int credenciadoID, BancoDeDados banco = null)
		{
			CFOCRelatorio entidade = new CFOCRelatorio();
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
					t.empreendimento_id,
					t.empreendimento_tid,
					t.nome_laboratorio,
					t.numero_laudo_resultado_analise,
					le.sigla estado_sigla,
					t.municipio_texto,
					t.numero_lacre,
					t.numero_porao,
					t.numero_container,
					t.produto_especificacao,
					t.partida_lacrada_origem,
					t.validade_certificado,
					nvl(t.informacoes_complement_html, t.informacoes_complementares) informacoes_complement_html,
					lee.sigla estado_emissao_sigla,
					t.municipio_emissao_texto,
					t.data_ativacao,
					t.data_execucao
				from hst_cfoc t, lov_estado le, lov_estado lee
				where le.id(+) = t.estado_id
				and lee.id(+) = t.estado_emissao_id
				and t.cfoc_id = :id
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
						entidade.Numero = reader.GetValue<string>("numero");
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
				select d.lote_id,
					d.codigo_lote,
					d.data_criacao,
					sum(d.quantidade) quantidade,
					d.unidade_medida_texto,
					d.cultura,
					d.cultivar_nome
				from (select cp.lote_id,
							l.codigo_uc || l.ano || lpad(l.numero, 4, '0') codigo_lote,
							l.data_criacao,
							li.quantidade,
							li.unidade_medida_texto,
							c.texto cultura,
							cc.cultivar_nome
						from hst_cfoc_produto     cp,
							hst_lote             l,
							hst_lote_item        li,
							hst_cultura          c,
							hst_cultura_cultivar cc
						where l.lote_id = cp.lote_id
						and l.tid = cp.lote_tid
						and li.id_hst = l.id
						and li.cultura_id = c.cultura_id
						and li.cultura_tid = c.tid
						and li.cultivar_id = cc.cultivar_id
						and li.cultivar_tid = cc.tid
						and cp.id_hst = :hst_id) d
				group by d.lote_id,
						d.codigo_lote,
						d.data_criacao,
						d.unidade_medida_texto,
						d.cultura,
						d.cultivar_nome", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst_id", hst_id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						entidade.Produtos.Add(new IdentificacaoProdutoRelatorio()
						{
							LoteCodigo = reader.GetValue<string>("codigo_lote"),
							CulturaTexto = reader.GetValue<string>("cultura"),
							CultivarTexto = reader.GetValue<string>("cultivar_nome"),
							UnidadeMedida = reader.GetValue<string>("unidade_medida_texto"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							DataConsolidacao = reader.GetValue<DateTime>("data_criacao").ToShortDateString()
						});
					}

					reader.Close();
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
				from hst_cfoc_trata_fitossa c
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
				#region Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"
				select e.denominador empreendimento_denominador,
					e.cnpj empreendimento_cnpj,
					ee.correspondencia emp_endereco_correspondencia,
					ee.cep empreendimento_cep,
					ee.logradouro empreendimento_logradouro,
					ee.bairro empreendimento_bairro,
					ee.municipio_texto empreendimento_municipio,
					ee.distrito empreendimento_distrito,
					le.sigla empreendimento_estado_sigla,
					(select c.codigo_uc from hst_crt_unidade_consolidacao c
					where c.tid = (select h.tid from hst_crt_unidade_consolidacao h where h.empreendimento_id = :id and h.data_execucao = 
					(select max(hu.data_execucao) from hst_crt_unidade_consolidacao hu where hu.empreendimento_id = :id and hu.data_execucao <= :data_execucao))) codigo_uc
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
						entidade.UCCodigo = reader.GetValue<string>("codigo_uc");
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