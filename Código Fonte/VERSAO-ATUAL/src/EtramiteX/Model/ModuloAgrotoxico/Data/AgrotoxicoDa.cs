using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAgrotoxico.Data
{
	class AgrotoxicoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		#region Ações DML

		internal void Salvar(Agrotoxico agrotoxico, BancoDeDados banco)
		{			
			if (agrotoxico == null)
			{
				throw new Exception("Agrotoxico é nulo.");
			}

			if (agrotoxico.Id <= 0)
			{
				Criar(agrotoxico, banco);
			}
			else
			{
				Editar(agrotoxico, banco);
			}
		}

		internal void Criar(Agrotoxico agrotoxico, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_agrotoxico (id, possui_cadastro, numero_cadastro, cadastro_ativo, motivo, nome_comercial, 
				numero_registro_ministerio, numero_processo_sep, titular_registro, classificacao_toxicologica, periculosidade_ambiental, forma_apresentacao, observacao_interna,
				observacao_geral, arquivo, tid) values (seq_tab_agrotoxico.nextval, :possui_cadastro, :numero_cadastro, :cadastro_ativo, :motivo, :nome_comercial, 
				:numero_registro_ministerio, :numero_processo_sep, :titular_registro, :classificacao_toxicologica, :periculosidade_ambiental, :forma_apresentacao, :observacao_interna,
				:observacao_geral, :arquivo, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("possui_cadastro", agrotoxico.PossuiCadastro, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_cadastro", agrotoxico.NumeroCadastro, DbType.Int32);
				comando.AdicionarParametroEntrada("cadastro_ativo", agrotoxico.CadastroAtivo, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", agrotoxico.MotivoId.GetValueOrDefault() < 1 ? null : agrotoxico.MotivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_comercial", agrotoxico.NomeComercial, DbType.String);
				comando.AdicionarParametroEntrada("numero_registro_ministerio", agrotoxico.NumeroRegistroMinisterio, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_processo_sep", agrotoxico.NumeroProcessoSep, DbType.Int64);
				comando.AdicionarParametroEntrada("titular_registro", agrotoxico.TitularRegistro.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("classificacao_toxicologica", agrotoxico.ClassificacaoToxicologica.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("periculosidade_ambiental", agrotoxico.PericulosidadeAmbiental.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("forma_apresentacao", agrotoxico.FormaApresentacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", agrotoxico.Bula.Id, DbType.Int32);
				comando.AdicionarParametroEntClob("observacao_interna", agrotoxico.ObservacaoInterna);
				comando.AdicionarParametroEntClob("observacao_geral", agrotoxico.ObservacaoGeral);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				agrotoxico.Id = comando.ObterValorParametro<int>("id");

				#region Ingredientes Ativos

				comando = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_ing_ativo (id, agrotoxico, ingrediente_ativo, concentracao, unidade_medida, unidade_medida_outro, tid) 
				values (seq_tab_agrotoxico_ing_ativo.nextval, :agrotoxico, :ingrediente_ativo, :concentracao, :unidade_medida, :unidade_medida_outro, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("ingrediente_ativo", DbType.Int32);
				comando.AdicionarParametroEntrada("concentracao", DbType.Decimal);
				comando.AdicionarParametroEntrada("unidade_medida", DbType.Int32);
				comando.AdicionarParametroEntrada("unidade_medida_outro", DbType.String, 30);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				agrotoxico.IngredientesAtivos.ForEach(ingrediente =>
				{
					comando.SetarValorParametro("ingrediente_ativo", ingrediente.Id);
					comando.SetarValorParametro("concentracao", ingrediente.Concentracao);
					comando.SetarValorParametro("unidade_medida", (ingrediente.UnidadeMedidaId > 0 ? ingrediente.UnidadeMedidaId: (object)DBNull.Value));
					comando.SetarValorParametro("unidade_medida_outro", ingrediente.UnidadeMedidaOutro);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Classes Uso

				comando = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_classe_uso (id, agrotoxico, classe_uso, tid) values (seq_tab_agrotoxico_classe_uso.nextval, :agrotoxico, 
				:classe_uso, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("classe_uso", DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				agrotoxico.ClassesUso.ForEach(classe =>
				{
					comando.SetarValorParametro("classe_uso", classe.Id);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Grupos Químicos

				comando = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_grupo_quimico (id, agrotoxico, grupo_quimico, tid) values (seq_tab_agrotox_grupo_quimico.nextval, 
				:agrotoxico, :grupo_quimico, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("grupo_quimico", DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				agrotoxico.GruposQuimicos.ForEach(grupo =>
				{
					comando.SetarValorParametro("grupo_quimico", grupo.Id);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Culturas

				comando = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_cultura (id, agrotoxico, cultura, intervalo_seguranca,tid) values (seq_tab_agrotoxico_cultura.nextval, 
				:agrotoxico, :cultura, :intervalo_seguranca,:tid) returning id into :cultura_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("cultura", DbType.Int32);
				comando.AdicionarParametroEntrada("intervalo_seguranca", DbType.String, 3);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("cultura_id", DbType.Int32);

				agrotoxico.Culturas.ForEach(cultura =>
				{
					comando.SetarValorParametro("cultura", cultura.Cultura.Id);
					comando.SetarValorParametro("intervalo_seguranca", cultura.IntervaloSeguranca);

					bancoDeDados.ExecutarNonQuery(comando);

					cultura.IdRelacionamento = comando.ObterValorParametro<int>("cultura_id");

					#region Praga

					Comando comandoAux = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_cultura_praga (id, agrotoxico_cultura, praga, tid) values 
					(seq_tab_agrotox_cultura_praga.nextval, :agrotoxico_cultura, :praga, :tid)", EsquemaBanco);

					comandoAux.AdicionarParametroEntrada("agrotoxico_cultura", cultura.IdRelacionamento, DbType.Int32);
					comandoAux.AdicionarParametroEntrada("praga", DbType.Int32);
					comandoAux.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					cultura.Pragas.ForEach(praga =>
					{
						comandoAux.SetarValorParametro("praga", praga.Id);
						bancoDeDados.ExecutarNonQuery(comandoAux);
					});

					#endregion

					#region Modalidades de aplicação

					comandoAux = bancoDeDados.CriarComando(@"insert into tab_agro_cult_moda_aplicacao (id, agrotoxico_cultura, modalidade_aplicacao, tid) values 
					(seq_tab_agro_cult_moda_aplic.nextval, :agrotoxico_cultura, :modalidade_aplicacao, :tid)", EsquemaBanco);

					comandoAux.AdicionarParametroEntrada("agrotoxico_cultura", cultura.IdRelacionamento, DbType.Int32);
					comandoAux.AdicionarParametroEntrada("modalidade_aplicacao", DbType.Int32);
					comandoAux.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					cultura.ModalidadesAplicacao.ForEach(modalidade =>
					{
						comandoAux.SetarValorParametro("modalidade_aplicacao", modalidade.Id);
						bancoDeDados.ExecutarNonQuery(comandoAux);
					});

					#endregion
				});

				#endregion

				Historico.Gerar(agrotoxico.Id, eHistoricoArtefato.agrotoxico, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Editar(Agrotoxico agrotoxico, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_agrotoxico set possui_cadastro = :possui_cadastro, numero_cadastro = :numero_cadastro, cadastro_ativo = 
				:cadastro_ativo, motivo =:motivo, nome_comercial =:nome_comercial, numero_registro_ministerio = :numero_registro_ministerio, numero_processo_sep =:numero_processo_sep,
				titular_registro =:titular_registro, classificacao_toxicologica =: classificacao_toxicologica, periculosidade_ambiental =:periculosidade_ambiental, forma_apresentacao = 
				:forma_apresentacao, observacao_interna =:observacao_interna, observacao_geral =:observacao_geral, arquivo =:arquivo, tid =:tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("possui_cadastro", agrotoxico.PossuiCadastro, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_cadastro", agrotoxico.NumeroCadastro, DbType.Int32);
				comando.AdicionarParametroEntrada("cadastro_ativo", agrotoxico.CadastroAtivo, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", agrotoxico.MotivoId.GetValueOrDefault() < 1 ? null : agrotoxico.MotivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_comercial", agrotoxico.NomeComercial, DbType.String);
				comando.AdicionarParametroEntrada("numero_registro_ministerio", agrotoxico.NumeroRegistroMinisterio, DbType.Int64);
				comando.AdicionarParametroEntrada("numero_processo_sep", agrotoxico.NumeroProcessoSep, DbType.Int64);
				comando.AdicionarParametroEntrada("titular_registro", agrotoxico.TitularRegistro.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("classificacao_toxicologica", agrotoxico.ClassificacaoToxicologica.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("periculosidade_ambiental", agrotoxico.PericulosidadeAmbiental.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("forma_apresentacao", agrotoxico.FormaApresentacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", agrotoxico.Bula.Id, DbType.Int32);
				comando.AdicionarParametroEntClob("observacao_interna", agrotoxico.ObservacaoInterna);
				comando.AdicionarParametroEntClob("observacao_geral", agrotoxico.ObservacaoGeral);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", agrotoxico.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Deletando dados

				//Modalidades de aplicação
				comando = bancoDeDados.CriarComando(@"delete from {0}tab_agro_cult_moda_aplicacao a where a.agrotoxico_cultura in 
				(select t.id from {0}tab_agrotoxico_cultura t where t.agrotoxico = :agrotoxico)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.id", DbType.Int32, agrotoxico.Culturas.SelectMany(x => x.ModalidadesAplicacao).Select(y => y.IdRelacionamento).ToList());
				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Praga

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_agrotoxico_cultura_praga a where a.agrotoxico_cultura in 
				(select t.id from {0}tab_agrotoxico_cultura t where t.agrotoxico = :agrotoxico)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.id", DbType.Int32, agrotoxico.Culturas.SelectMany(x => x.Pragas).Select(y => y.IdRelacionamento).ToList());
				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Cultura
				comando = bancoDeDados.CriarComando(@"delete from tab_agrotoxico_cultura ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where agrotoxico =:agrotoxico {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, agrotoxico.Culturas.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Grupo Químico
				comando = bancoDeDados.CriarComando(@"delete from tab_agrotoxico_grupo_quimico ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where agrotoxico =:agrotoxico {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, agrotoxico.GruposQuimicos.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Classe de uso
				comando = bancoDeDados.CriarComando(@"delete from tab_agrotoxico_classe_uso ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where agrotoxico =:agrotoxico {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, agrotoxico.ClassesUso.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Ingrediente ativo
				comando = bancoDeDados.CriarComando(@"delete from tab_agrotoxico_ing_ativo ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where agrotoxico =:agrotoxico {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, agrotoxico.IngredientesAtivos.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Ingredientes Ativos

				agrotoxico.IngredientesAtivos.ForEach(ingrediente =>
				{
					if (ingrediente.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_agrotoxico_ing_ativo set agrotoxico =: agrotoxico, ingrediente_ativo =: ingrediente_ativo, 
						concentracao = :concentracao, unidade_medida = :unidade_medida, unidade_medida_outro = :unidade_medida_outro, tid =:tid where id =:id_rel", EsquemaBanco);
						comando.AdicionarParametroEntrada("id_rel", ingrediente.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_agrotoxico_ing_ativo (id, agrotoxico, ingrediente_ativo, concentracao, unidade_medida, unidade_medida_outro, tid) 
						values (seq_tab_agrotoxico_ing_ativo.nextval, :agrotoxico, :ingrediente_ativo, :concentracao, :unidade_medida, :unidade_medida_outro, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ingrediente_ativo", ingrediente.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("concentracao", ingrediente.Concentracao, DbType.Decimal);
					comando.AdicionarParametroEntrada("unidade_medida", ingrediente.UnidadeMedidaId > 0 ? ingrediente.UnidadeMedidaId : (object)DBNull.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("unidade_medida_outro", DbType.String, 30, ingrediente.UnidadeMedidaOutro);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Classes Uso

				agrotoxico.ClassesUso.ForEach(classe =>
				{
					if (classe.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_agrotoxico_classe_uso set agrotoxico =:agrotoxico, classe_uso =:classe_uso, tid =:tid 
						where id = :id_rel", EsquemaBanco);
						comando.AdicionarParametroEntrada("id_rel", classe.IdRelacionamento, DbType.Int32);

					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_classe_uso (id, agrotoxico, classe_uso, tid) values (seq_tab_agrotoxico_classe_uso.nextval, 
						:agrotoxico, :classe_uso, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("classe_uso", classe.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Grupos Químicos

				agrotoxico.GruposQuimicos.ForEach(grupo =>
				{
					if (grupo.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_agrotoxico_grupo_quimico set agrotoxico =:agrotoxico , grupo_quimico =:grupo_quimico, tid =: tid where id =:id_rel", EsquemaBanco);
						comando.AdicionarParametroEntrada("id_rel", grupo.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_grupo_quimico (id, agrotoxico, grupo_quimico, tid) values 
						(seq_tab_agrotox_grupo_quimico.nextval, :agrotoxico, :grupo_quimico, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("grupo_quimico", grupo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Culturas

				agrotoxico.Culturas.ForEach(cultura =>
				{
					if (cultura.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_agrotoxico_cultura set agrotoxico =: agrotoxico, cultura =:cultura, intervalo_seguranca =:intervalo_seguranca, 
						tid =: tid where id =:id_rel", EsquemaBanco);
						comando.AdicionarParametroEntrada("id_rel", cultura.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_cultura (id, agrotoxico, cultura, intervalo_seguranca,tid) values (seq_tab_agrotoxico_cultura.nextval, 
						:agrotoxico, :cultura, :intervalo_seguranca,:tid) returning id into :cultura_id", EsquemaBanco);
						comando.AdicionarParametroSaida("cultura_id", DbType.Int32);

					}

					comando.AdicionarParametroEntrada("agrotoxico", agrotoxico.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("cultura", cultura.Cultura.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("intervalo_seguranca", DbType.String, 3, cultura.IntervaloSeguranca);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);

					cultura.IdRelacionamento = cultura.IdRelacionamento > 0 ? cultura.IdRelacionamento : comando.ObterValorParametro<int>("cultura_id");

					#region Praga

					Comando comandoAux = null;

					cultura.Pragas.ForEach(praga =>
					{
						if (praga.IdRelacionamento > 0)
						{
							comandoAux = bancoDeDados.CriarComando(@"update tab_agrotoxico_cultura_praga set agrotoxico_cultura =:agrotoxico_cultura, praga =:praga, tid =:tid
							where id =:id_rel", EsquemaBanco);

							comandoAux.AdicionarParametroEntrada("id_rel", praga.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comandoAux = bancoDeDados.CriarComando(@"insert into tab_agrotoxico_cultura_praga (id, agrotoxico_cultura, praga, tid) values 
							(seq_tab_agrotox_cultura_praga.nextval, :agrotoxico_cultura, :praga, :tid)", EsquemaBanco);
						}

						comandoAux.AdicionarParametroEntrada("agrotoxico_cultura", cultura.IdRelacionamento, DbType.Int32);
						comandoAux.AdicionarParametroEntrada("praga", praga.Id, DbType.Int32);
						comandoAux.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comandoAux);
					});

					#endregion

					#region Modalidades de aplicação

					cultura.ModalidadesAplicacao.ForEach(modalidade =>
					{
						if (modalidade.IdRelacionamento > 0)
						{
							comandoAux = bancoDeDados.CriarComando(@"update tab_agro_cult_moda_aplicacao set agrotoxico_cultura =:agrotoxico_cultura, modalidade_aplicacao 
							=:modalidade_aplicacao, tid =: tid where id =:id_rel", EsquemaBanco);
							comandoAux.AdicionarParametroEntrada("id_rel", modalidade.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comandoAux = bancoDeDados.CriarComando(@"insert into tab_agro_cult_moda_aplicacao (id, agrotoxico_cultura, modalidade_aplicacao, tid) values 
							(seq_tab_agro_cult_moda_aplic.nextval, :agrotoxico_cultura, :modalidade_aplicacao, :tid)", EsquemaBanco);
						}

						comandoAux.AdicionarParametroEntrada("agrotoxico_cultura", cultura.IdRelacionamento, DbType.Int32);
						comandoAux.AdicionarParametroEntrada("modalidade_aplicacao", modalidade.Id, DbType.Int32);
						comandoAux.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comandoAux);
					});

					#endregion
				});

				#endregion

				Historico.Gerar(agrotoxico.Id, eHistoricoArtefato.agrotoxico, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_agrotoxico c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.agrotoxico, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				comando = comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					delete from {0}tab_agro_cult_moda_aplicacao where agrotoxico_cultura in (select id from {0}tab_agrotoxico_cultura where agrotoxico = :id);
					delete from {0}tab_agrotoxico_cultura_praga where agrotoxico_cultura in (select id from {0}tab_agrotoxico_cultura where agrotoxico = :id);
					delete from {0}tab_agrotoxico_cultura where agrotoxico = :id;
					delete from {0}tab_agrotoxico_grupo_quimico where agrotoxico = :id;
					delete from {0}tab_agrotoxico_classe_uso where agrotoxico = :id;
					delete from {0}tab_agrotoxico_ing_ativo where agrotoxico = :id;
					delete from {0}tab_agrotoxico where id = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(Agrotoxico agrotoxico, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_agrotoxico a set a.cadastro_ativo = :situacao, a.motivo = :motivo, a.tid = :tid where a.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("situacao", agrotoxico.CadastroAtivo, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", agrotoxico.MotivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", agrotoxico.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(agrotoxico.Id, eHistoricoArtefato.agrotoxico, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal Agrotoxico Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			Agrotoxico agrotoxico = new Agrotoxico();

			using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDedados.CriarComando(@"select t.id, t.possui_cadastro, t.numero_cadastro, t.cadastro_ativo, t.motivo, lm.texto motivo_texto, t.nome_comercial, 
				t.numero_registro_ministerio, t.numero_processo_sep, t.titular_registro, p.razao_social titular_registro_nome, t.classificacao_toxicologica, 
				c.texto class_toxicologica_texto, t.periculosidade_ambiental, t.forma_apresentacao, t.observacao_interna, t.observacao_geral, t.arquivo, t.tid 
				from {0}tab_agrotoxico t, {0}lov_agrotoxico_motivo lm, {0}tab_pessoa p, {0}tab_class_toxicologica c 
				where lm.id(+) = t.motivo and c.id = classificacao_toxicologica and p.id = t.titular_registro and t.id = :id", EsquemaBanco);

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
						agrotoxico.MotivoTexto = reader.GetValue<string>("motivo_texto");
						agrotoxico.NomeComercial = reader.GetValue<string>("nome_comercial");
						agrotoxico.NumeroCadastro = reader.GetValue<int>("numero_cadastro");
						agrotoxico.NumeroRegistroMinisterio = reader.GetValue<long>("numero_registro_ministerio");
						agrotoxico.NumeroProcessoSep = reader.GetValue<long>("numero_processo_sep");
						agrotoxico.ObservacaoGeral = reader.GetValue<string>("observacao_geral");
						agrotoxico.ObservacaoInterna = reader.GetValue<string>("observacao_interna");
						agrotoxico.PericulosidadeAmbiental.Id = reader.GetValue<int>("periculosidade_ambiental");
						agrotoxico.PossuiCadastro = reader.GetValue<bool>("possui_cadastro");
						agrotoxico.Tid = reader.GetValue<string>("tid");
						agrotoxico.TitularRegistro.Id = reader.GetValue<int>("titular_registro");
						agrotoxico.TitularRegistro.NomeRazaoSocial = reader.GetValue<string>("titular_registro_nome");
					}

					reader.Close();
				}

				if (agrotoxico == null || agrotoxico.Id < 1 || simplificado)
				{
					return agrotoxico;
				}

				#region Ingredientes Ativos

				comando = bancoDedados.CriarComando(@"
				select t.id, t.tid, t.agrotoxico, t.ingrediente_ativo, i.texto ingrediente_ativo_texto, i.situacao, 
				s.texto situacao_texto, t.concentracao, t.unidade_medida, lu.texto unidade_medida_texto, t.unidade_medida_outro 
				from tab_agrotoxico_ing_ativo t, tab_ingrediente_ativo i, lov_ingrediente_ativo_situacao s, lov_agrotoxico_uni_medida lu 
				where t.ingrediente_ativo = i.id and i.situacao = s.id and t.unidade_medida = lu.id and t.agrotoxico = :agrotoxico", EsquemaBanco);

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
						ingrediente.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						ingrediente.UnidadeMedidaId = reader.GetValue<int>("unidade_medida");
						ingrediente.UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto");
						ingrediente.UnidadeMedidaOutro = reader.GetValue<string>("unidade_medida_outro");
						ingrediente.Tid = reader.GetValue<string>("tid");
						ingrediente.Concentracao = reader.GetValue<decimal>("concentracao");
						agrotoxico.IngredientesAtivos.Add(ingrediente);
					}

					reader.Close();
				}

				#endregion

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

				comando = bancoDedados.CriarComando(@"select t.id, t.agrotoxico, t.cultura, c.texto cultura_texto, t.intervalo_seguranca, t.tid
				from tab_agrotoxico_cultura t, tab_cultura c where t.cultura = c.id and t.agrotoxico = :agrotoxico order by c.texto", EsquemaBanco);

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

				if (agrotoxico.Culturas != null && agrotoxico.Culturas.Count > 0)
				{
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
							agrotoxico.Culturas.Single(x => x.IdRelacionamento == reader.GetValue<int>("agrotoxico_cultura")).Pragas.Add(praga);
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

							agrotoxico.Culturas.Single(x => x.IdRelacionamento == reader.GetValue<int>("agrotoxico_cultura")).ModalidadesAplicacao.Add(modalidade);
						}

						reader.Close();
					}

					#endregion
				}
			}

			return agrotoxico;
		}

		internal Resultados<AgrotoxicoFiltro> Filtrar(Filtro<AgrotoxicoFiltro> filtros)
		{
			Resultados<AgrotoxicoFiltro> retorno = new Resultados<AgrotoxicoFiltro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("a.nome_comercial", "nome_comercial", filtros.Dados.NomeComercial, true, true);

				comandtxt += comando.FiltroAnd("a.numero_cadastro", "numero_cadastro", filtros.Dados.NumeroCadastro);

				comandtxt += comando.FiltroAnd("a.numero_registro_ministerio", "numero_registro_ministerio", filtros.Dados.NumeroRegistroMinisterio);

				comandtxt += comando.FiltroAnd("a.numero_processo_sep", "numero_processo_sep", filtros.Dados.NumeroProcessoSep);

				comandtxt += comando.FiltroAnd("a.classificacao_toxicologica", "classificacao_toxicologica", filtros.Dados.ClassificacaoToxicologica);

				if (filtros.Dados.Situacao != "-1")
				{
					comandtxt += comando.FiltroAnd("a.cadastro_ativo", "cadastro_ativo", filtros.Dados.Situacao);
				}

				if (filtros.Dados.ClasseUso > 0)
				{
					comandtxt += comando.FiltroIn("a.id", "select t1.agrotoxico from tab_agrotoxico_classe_uso t1 where t1.classe_uso =:classe_uso", "classe_uso", filtros.Dados.ClasseUso);
				}

				if (filtros.Dados.ModalidadeAplicacao > 0)
				{
					comandtxt += comando.FiltroIn("a.id", "select t10.agrotoxico from tab_agro_cult_moda_aplicacao t9, tab_agrotoxico_cultura t10 where t9.agrotoxico_cultura = t10.id and t9.modalidade_aplicacao = :modalidade_aplicacao", "modalidade_aplicacao", filtros.Dados.ModalidadeAplicacao);
				}

				if (filtros.Dados.GrupoQuimico > 0)
				{
					comandtxt += comando.FiltroIn("a.id", "select t11.agrotoxico from tab_agrotoxico_grupo_quimico t11 where t11.grupo_quimico = :grupo_quimico", "grupo_quimico", filtros.Dados.GrupoQuimico);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.TitularRegistro))
				{
					comandtxt += comando.FiltroAndLike("nvl(p.nome, p.razao_social)", "titular_registro", filtros.Dados.TitularRegistro, true, true);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.IngredienteAtivo))
				{
					comandtxt += comando.FiltroIn("a.id", "select t3.agrotoxico from tab_ingrediente_ativo t2, tab_agrotoxico_ing_ativo t3 where t2.id = t3.ingrediente_ativo and upper(t2.texto) like upper('%'||:ingrediente||'%')", "ingrediente", filtros.Dados.IngredienteAtivo);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.Cultura))
				{
					comandtxt += comando.FiltroIn("a.id", "select t5.agrotoxico from tab_cultura t4, tab_agrotoxico_cultura t5 where t4.id = t5.cultura and upper(t4.texto) like upper('%'||:cultura||'%')", "cultura", filtros.Dados.Cultura);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.Praga))
				{
					comandtxt += comando.FiltroIn(@"a.id", "select t8.agrotoxico from tab_praga t6, tab_agrotoxico_cultura_praga t7, tab_agrotoxico_cultura t8 where t6.id = t7.praga and t7.agrotoxico_cultura = t8.id and upper(nvl(t6.nome_cientifico, t6.nome_comum)) like upper('%'||:praga||'%')", "praga", filtros.Dados.Praga);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero_cadastro", "nome_comercial", "titular_registro", "situacao" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero_cadastro");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format(@"select count(a.id) qtd from tab_agrotoxico a, tab_pessoa p where a.titular_registro = p.id " + comandtxt);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Funcionario.NaoEncontrouRegistros);
					return retorno;
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select a.id, (case when a.possui_cadastro>0 then 'Sim' else 'Não' end)  possui_cadastro,
				(case when a.cadastro_ativo>0 then 'Ativo' else 'Inativo' end) situacao, a.numero_cadastro, a.nome_comercial, 
				nvl(p.nome, p.razao_social) titular_registro, a.arquivo from tab_agrotoxico a, tab_pessoa p where a.titular_registro = p.id {0} {1}",
				comandtxt, DaHelper.Ordenar(colunas, ordenar)); //1 - Aguardando Ativação

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					AgrotoxicoFiltro agro;
					while (reader.Read())
					{
						agro = new AgrotoxicoFiltro();
						agro.Id = reader.GetValue<Int32>("id");
						agro.ArquivoId = reader.GetValue<Int32>("arquivo");
						agro.NumeroCadastro = reader.GetValue<String>("numero_cadastro");
						agro.NomeComercial = reader.GetValue<String>("nome_comercial");
						agro.TitularRegistro = reader.GetValue<String>("titular_registro");
						agro.Situacao = reader.GetValue<String>("situacao");

						retorno.Itens.Add(agro);
					}

					reader.Close();
					#endregion
				}
			}

			return retorno;
		}

		internal int ObterSequenciaNumeroCadastro()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select max(a.numero_cadastro) + 1 from tab_agrotoxico a where a.possui_cadastro = 0", EsquemaBanco);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal List<Agrotoxico> ObterLista(AgrotoxicoFiltro filtro, BancoDeDados banco = null)
		{
			List<Agrotoxico> retorno = new List<Agrotoxico>();

			using (BancoDeDados bancoDedados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDedados.CriarComando(@"
				select t.id, t.tid, t.cadastro_ativo, t.motivo, lm.texto motivo_texto 
				from {0}tab_agrotoxico t, {0}lov_agrotoxico_motivo lm where lm.id(+) = t.motivo ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.FiltroAnd("t.cadastro_ativo", "cadastro_ativo", filtro.Situacao);

				comando.DbCommand.CommandText += comando.FiltroAnd("t.motivo", "motivo", filtro.MotivoId);

				comando.DbCommand.CommandText += comando.FiltroIn("t.id", 
				"select ai.agrotoxico from tab_agrotoxico_ing_ativo ai where ai.ingrediente_ativo = :ingrediente", "ingrediente", filtro.IngredienteAtivoId);

				using (IDataReader reader = bancoDedados.ExecutarReader(comando))
				{
					Agrotoxico agrotoxico;

					while (reader.Read())
					{
						agrotoxico = new Agrotoxico();
						agrotoxico.Id = reader.GetValue<int>("id");
						agrotoxico.Tid = reader.GetValue<string>("tid");
						agrotoxico.CadastroAtivo = reader.GetValue<bool>("cadastro_ativo");
						agrotoxico.MotivoId = reader.GetValue<int?>("motivo");
						agrotoxico.MotivoTexto = reader.GetValue<string>("motivo_texto");

						retorno.Add(agrotoxico);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		internal bool Existe(int Id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select count(*) from tab_agrotoxico where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", Id, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool NumeroCadastroExiste(Agrotoxico agrotoxico, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_agrotoxico where numero_cadastro =:numero_cadastro and id <> :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", agrotoxico.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_cadastro", agrotoxico.NumeroCadastro, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool ProcessoSepEmOutroCadastro(Agrotoxico agrotoxico, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_agrotoxico where numero_processo_sep =:numero_processo_sep and id <> :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", agrotoxico.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_processo_sep", agrotoxico.NumeroProcessoSep, DbType.Int64);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal String PossuiTituloAssociado(int agrotoxicoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ls.texto situacao_texto from {0}tab_titulo t, {0}lov_titulo_situacao ls, 
															{0}esp_cert_produto_agro e where t.situacao <> 5 /*Encerrado*/ and e.titulo = t.id 
															and ls.id = t.situacao and e.agrotoxico = :agrotoxico", EsquemaBanco);

				comando.AdicionarParametroEntrada("agrotoxico", agrotoxicoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

		#endregion
	}
}