using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloCertificado;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloCertificado.Data
{
	class CertificadoCadastroProdutoAgrotoxicoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		private string EsquemaBancoGeo { get; set; }

		#endregion

		public CertificadoCadastroProdutoAgrotoxicoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioGeo);

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(CertificadoCadastroProdutoAgrotoxico termo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_cert_produto_agro e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_cert_produto_agro e set e.tid = :tid, e.protocolo = :protocolo, e.destinatario = :destinatario, 
														e.agrotoxico = :agrotoxico where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					termo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_cert_produto_agro (id, tid, titulo, protocolo, destinatario, agrotoxico) 
														values ({0}seq_esp_cert_produto_agro.nextval, :tid, :titulo, :protocolo, :destinatario, :agrotoxico)
														returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", termo.Titulo.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", termo.DestinatarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("agrotoxico", termo.AgrotoxicoId, DbType.Int32);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					termo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				Historico.Gerar(termo.Titulo.Id, eHistoricoArtefatoEspecificidade.certificadoprodutoagrotoxico, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_cert_produto_agro c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.certificadoprodutoagrotoxico, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_cert_produto_agro e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal CertificadoCadastroProdutoAgrotoxico Obter(int titulo, BancoDeDados banco = null)
		{
			CertificadoCadastroProdutoAgrotoxico especificidade = new CertificadoCadastroProdutoAgrotoxico();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.titulo, e.protocolo, e.destinatario, nvl(dest.nome, dest.razao_social) destinatario_nome_razao,
															e.agrotoxico agrotoxico_id, a.tid agrotoxico_tid, a.nome_comercial agrotoxico_nome, n.numero, n.ano, p.requerimento,
															p.protocolo protocolo_tipo from {0}esp_cert_produto_agro e, {0}tab_protocolo p, {0}tab_titulo_numero n, {0}tab_pessoa dest,
															{0}tab_agrotoxico a where n.titulo(+) = e.titulo and e.protocolo = p.id and dest.id = e.destinatario and a.id = e.agrotoxico
															and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("id");
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");

						especificidade.AgrotoxicoId = reader.GetValue<int>("agrotoxico_id");
						especificidade.AgrotoxicoTid = reader.GetValue<string>("agrotoxico_tid");
						especificidade.AgrotoxicoNome = reader.GetValue<string>("agrotoxico_nome");

						if (reader["destinatario"] != null && !Convert.IsDBNull(reader["destinatario"]))
						{
							especificidade.DestinatarioId = Convert.ToInt32(reader["destinatario"]);
							especificidade.DestinatarioNomeRazao = reader["destinatario_nome_razao"].ToString();
						}
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal CertificadoCadastroProdutoAgrotoxico ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			CertificadoCadastroProdutoAgrotoxico especificidade = new CertificadoCadastroProdutoAgrotoxico();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				comando = bancoDeDados.CriarComando(@"select e.id, e.especificidade_id, e.tid, p.id_protocolo, e.destinatario_id, e.destinatario_tid, dest.tipo destinatario_tipo,
													nvl(dest.nome, dest.razao_social) destinatario_nome_razao, a.agrotoxico_id, a.nome_comercial agrotoxico_nome, a.tid agrotoxico_tid,
													n.numero, n.ano, p.requerimento_id, p.protocolo_id protocolo_tipo from {0}hst_esp_cert_produto_agro e, {0}hst_titulo_numero n,
													{0}hst_protocolo p, {0}hst_pessoa dest, {0}hst_agrotoxico a where e.titulo_id = n.titulo_id(+) and e.titulo_tid = n.titulo_tid(+)
													and e.protocolo_id = p.id_protocolo(+) and e.protocolo_tid = p.tid(+) and dest.pessoa_id = e.destinatario_id and dest.tid = e.destinatario_tid
													and e.agrotoxico_id = a.agrotoxico_id and e.agrotoxico_tid = a.tid and not exists (select 1 from lov_historico_artefatos_acoes l
													where l.id = e.acao_executada and l.acao = 3) and e.titulo_tid = (select ht.tid from hst_titulo ht where ht.titulo_id = e.titulo_id
													and ht.data_execucao = (select max(htt.data_execucao) from hst_titulo htt where htt.titulo_id = e.titulo_id and htt.situacao_id = :situacao))
													and e.titulo_id = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", situacao > 0 ? situacao : 1, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("especificidade_id");
						especificidade.Tid = reader.GetValue<string>("tid");

						especificidade.AgrotoxicoId = reader.GetValue<int>("agrotoxico_id");
						especificidade.AgrotoxicoTid = reader.GetValue<string>("agrotoxico_tid");
						especificidade.AgrotoxicoNome = reader.GetValue<string>("agrotoxico_nome");

						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							especificidade.DestinatarioId = Convert.ToInt32(reader["destinatario_id"]);
							especificidade.DestinatarioNomeRazao = reader["destinatario_nome_razao"].ToString();
						}

						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento_id");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("id_protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Certificado ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Certificado certificado = new Certificado();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				certificado.Titulo = dados.Titulo;
				certificado.Titulo.SetorEndereco = DaEsp.ObterEndSetor(certificado.Titulo.SetorId);
				certificado.Protocolo = dados.Protocolo;
				certificado.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				CertificadoCadastroProdutoAgrotoxico esp = ObterHistorico(titulo, dados.Titulo.SituacaoId, bancoDeDados);

				#endregion

				certificado.Destinatario = DaEsp.ObterDadosPessoa(esp.DestinatarioId, certificado.Empreendimento.Id, bancoDeDados);
				certificado.Agrotoxico = new AgrotoxicoPDF(ObterAgrotoxico(esp.AgrotoxicoId, banco: bancoDeDados));
			}

			return certificado;
		}

		internal Agrotoxico ObterAgrotoxicoPorProtocolo(int protocoloId, BancoDeDados banco = null)
		{
			Agrotoxico agrotoxico = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.nome_comercial, t.tid from tab_agrotoxico t where t.numero_processo_sep =  
															(select numero_autuacao from tab_protocolo where id = :protocolo)", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						agrotoxico = new Agrotoxico();
						agrotoxico.Id = reader.GetValue<int>("id");
						agrotoxico.NomeComercial = reader.GetValue<string>("nome_comercial");
						agrotoxico.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return agrotoxico;
		}

		internal Agrotoxico ObterAgrotoxico(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			Agrotoxico agrotoxico = new Agrotoxico();

			using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDedados.CriarComando(@"select t.id, t.possui_cadastro, t.numero_cadastro, t.cadastro_ativo, t.motivo, t.nome_comercial,
															t.numero_registro_ministerio, t.numero_processo_sep, t.titular_registro, nvl(p.nome, p.razao_social) 
															titular_registro_nome_razao, nvl(p.cpf, p.cnpj) titular_registro_cpf_cnpj, t.classificacao_toxicologica, 
															c.texto class_toxicologica_texto, t.periculosidade_ambiental, pa.texto periculosidade_ambiental_texto, 
															t.forma_apresentacao, t.observacao_interna, t.observacao_geral, t.arquivo, t.tid from tab_agrotoxico t,
															tab_pessoa p, tab_class_toxicologica c, tab_peric_ambiental pa where c.id = classificacao_toxicologica 
															and p.id = t.titular_registro and pa.id = t.periculosidade_ambiental and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						agrotoxico.Id = id;
						agrotoxico.Bula.Id = reader.GetValue<int>("arquivo");
						agrotoxico.CadastroAtivo = reader.GetValue<bool>("cadastro_ativo");
						agrotoxico.ClassificacaoToxicologica.Id = reader.GetValue<int>("classificacao_toxicologica");
						agrotoxico.ClassificacaoToxicologica.Texto = reader.GetValue<string>("class_toxicologica_texto");
						agrotoxico.FormaApresentacao.Id = reader.GetValue<int>("forma_apresentacao");
						agrotoxico.MotivoId = reader.GetValue<int?>("motivo");
						agrotoxico.NomeComercial = reader.GetValue<string>("nome_comercial");
						agrotoxico.NumeroCadastro = reader.GetValue<int>("numero_cadastro");
						agrotoxico.NumeroRegistroMinisterio = reader.GetValue<long>("numero_registro_ministerio");
						agrotoxico.NumeroProcessoSep = reader.GetValue<long>("numero_processo_sep");
						agrotoxico.ObservacaoGeral = reader.GetValue<string>("observacao_geral");
						agrotoxico.ObservacaoInterna = reader.GetValue<string>("observacao_interna");
						agrotoxico.PericulosidadeAmbiental.Id = reader.GetValue<int>("periculosidade_ambiental");
						agrotoxico.PericulosidadeAmbiental.Texto = reader.GetValue<string>("periculosidade_ambiental_texto");
						agrotoxico.PossuiCadastro = reader.GetValue<bool>("possui_cadastro");
						agrotoxico.Tid = reader.GetValue<string>("tid");
						agrotoxico.TitularRegistro.Id = reader.GetValue<int>("titular_registro");
						agrotoxico.TitularRegistro.NomeRazaoSocial = reader.GetValue<string>("titular_registro_nome_razao");
						agrotoxico.TitularRegistro.CPFCNPJ = reader.GetValue<string>("titular_registro_cpf_cnpj");
					}
					reader.Close();
				}

				if (simplificado)
				{
					return agrotoxico;
				}

				#region Classes de Uso

				comando = bancoDedados.CriarComando(@"select t.id, t.agrotoxico, t.classe_uso, c.texto classe_uso_texto, t.tid from tab_agrotoxico_classe_uso t, tab_classe_uso c 
                where t.classe_uso = c.id and t.agrotoxico =:agrotoxico", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", id, DbType.Int32);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					ConfiguracaoVegetalItem classeUso = null;

					while (reader.Read())
					{
						classeUso = new ConfiguracaoVegetalItem();
						classeUso.IdRelacionamento = reader.GetValue<int>("id");
						classeUso.Id = reader.GetValue<int>("classe_uso");
						classeUso.Texto = reader.GetValue<string>("classe_uso_texto");
						classeUso.Tid = reader.GetValue<string>("tid");
						agrotoxico.ClassesUso.Add(classeUso);
					}

					reader.Close();
				}

				#endregion

				#region Ingredientes Ativos

				comando = bancoDedados.CriarComando(@"select t.id, t.agrotoxico, t.ingrediente_ativo, i.texto ingrediente_ativo_texto, i.situacao, t.concentracao, t.tid from tab_agrotoxico_ing_ativo 
                t, tab_ingrediente_ativo i where t.ingrediente_ativo = i.id and t.agrotoxico = :agrotoxico", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", id, DbType.Int32);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					ConfiguracaoVegetalItem ingrediente = null;

					while (reader.Read())
					{
						ingrediente = new ConfiguracaoVegetalItem();
						ingrediente.IdRelacionamento = reader.GetValue<int>("id");
						ingrediente.Id = reader.GetValue<int>("ingrediente_ativo");
						ingrediente.Texto = reader.GetValue<string>("ingrediente_ativo_texto");
						ingrediente.SituacaoId = reader.GetValue<int>("situacao");
						ingrediente.Tid = reader.GetValue<string>("tid");
						ingrediente.Concentracao = reader.GetValue<decimal>("concentracao");
						agrotoxico.IngredientesAtivos.Add(ingrediente);
					}

					reader.Close();
				}

				#endregion

				#region Grupos químicos

				comando = bancoDedados.CriarComando(@"select t.id, t.agrotoxico, t.grupo_quimico, g.texto grupo_quimico_texto, t.tid from tab_agrotoxico_grupo_quimico t, 
                tab_grupo_quimico g where t.grupo_quimico = g.id and t.agrotoxico = :agrotoxico", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", id, DbType.Int32);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					ConfiguracaoVegetalItem grupo = null;

					while (reader.Read())
					{
						grupo = new ConfiguracaoVegetalItem();
						grupo.IdRelacionamento = reader.GetValue<int>("id");
						grupo.Id = reader.GetValue<int>("grupo_quimico");
						grupo.Texto = reader.GetValue<string>("grupo_quimico_texto");
						grupo.Tid = reader.GetValue<string>("tid");
						agrotoxico.GruposQuimicos.Add(grupo);
					}

					reader.Close();
				}

				#endregion

				#region Culturas

				comando = bancoDedados.CriarComando(@"select t.id, t.agrotoxico, t.cultura, c.texto cultura_texto, t.intervalo_seguranca, 
													t.tid from tab_agrotoxico_cultura t, tab_cultura c where t.cultura = c.id 
													and t.agrotoxico = :agrotoxico", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", id, DbType.Int32);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					AgrotoxicoCultura cultura = null;

					while (reader.Read())
					{
						cultura = new AgrotoxicoCultura();
						cultura.IdRelacionamento = reader.GetValue<int>("id");
						cultura.Cultura.Id = reader.GetValue<int>("cultura");
						cultura.Cultura.Nome = reader.GetValue<string>("cultura_texto");
						cultura.IntervaloSeguranca = reader.GetValue<string>("intervalo_seguranca");
						cultura.Tid = reader.GetValue<string>("tid");
						agrotoxico.Culturas.Add(cultura);
					}

					reader.Close();
				}

				#endregion

				#region Pragas das culturas

				comando = bancoDedados.CriarComando(@"select t.id, t.agrotoxico_cultura, t.praga, p.nome_cientifico, p.nome_comum, t.tid from tab_agrotoxico_cultura_praga t, tab_praga p ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" where t.praga = p.id {0}",
					comando.AdicionarIn("and", "t.agrotoxico_cultura", DbType.Int32, agrotoxico.Culturas.Select(x => x.IdRelacionamento).ToList()));

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					Praga praga = null;

					while (reader.Read())
					{
						praga = new Praga();
						praga.IdRelacionamento = reader.GetValue<int>("id");
						praga.Id = reader.GetValue<int>("praga");
						praga.NomeCientifico = reader.GetValue<string>("nome_cientifico");
						praga.NomeComum = reader.GetValue<string>("nome_comum");
						praga.Tid = reader.GetValue<string>("tid");

						if (agrotoxico.Culturas != null && agrotoxico.Culturas.Count > 0)
						{
							agrotoxico.Culturas.Single(x => x.IdRelacionamento == reader.GetValue<int>("agrotoxico_cultura")).Pragas.Add(praga);
						}
					}

					reader.Close();
				}

				#endregion

				#region Modalidades de aplicacao das culturas

				comando = bancoDedados.CriarComando(@"select t.id, t.agrotoxico_cultura, t.modalidade_aplicacao, m.texto modalidade_texto, t.tid from tab_agro_cult_moda_aplicacao t, 
                tab_modalidade_aplicacao m ", EsquemaBanco);

				comando.DbCommand.CommandText += String.Format(" where t.modalidade_aplicacao = m.id {0}",
					comando.AdicionarIn("and", "t.agrotoxico_cultura", DbType.Int32, agrotoxico.Culturas.Select(x => x.IdRelacionamento).ToList()));


				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					ConfiguracaoVegetalItem modalidade = null;

					while (reader.Read())
					{
						modalidade = new ConfiguracaoVegetalItem();
						modalidade.Id = reader.GetValue<int>("modalidade_aplicacao");
						modalidade.IdRelacionamento = reader.GetValue<int>("id");
						modalidade.Texto = reader.GetValue<string>("modalidade_texto");

						if (agrotoxico.Culturas != null && agrotoxico.Culturas.Count > 0)
						{
							agrotoxico.Culturas.Single(x => x.IdRelacionamento == reader.GetValue<int>("agrotoxico_cultura")).ModalidadesAplicacao.Add(modalidade);
						}
					}

					reader.Close();
				}

				#endregion
			}

			return agrotoxico;
		}

		#endregion

		#region Auxiliares

		internal Boolean ProcessoPossuiSEP(int protocoloId, long numeroSEP, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from tab_protocolo t where t.id = :protocolo 
															and t.numero_autuacao = :numero_sep", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_sep", numeroSEP, DbType.Int64);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar<Int32>(comando));

			}
		}

		#endregion
	}
}