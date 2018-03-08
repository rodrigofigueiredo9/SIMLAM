using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class ConsideracaoFinalDa
	{
		#region Propriedade e Atributos

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public String EsquemaBancoGeo { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }

		#endregion

		public ConsideracaoFinalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public int Salvar(ConsideracaoFinal consideracaoFinal, BancoDeDados banco = null)
		{
			if (consideracaoFinal == null)
			{
				throw new Exception("Consideração Final é nulo.");
			}

			if (consideracaoFinal.Id <= 0)
			{
				consideracaoFinal.Id = Criar(consideracaoFinal, banco);
			}
			else
			{
				Editar(consideracaoFinal, banco);
			}

			return consideracaoFinal.Id;
		}

		public int Criar(ConsideracaoFinal consideracaoFinal, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
					insert into {0}tab_fisc_consid_final
					  (id,
					   fiscalizacao,
					   descrever,
					   tem_reparacao,
					   reparacao,
					   tem_termo_comp,
					   tem_termo_comp_justificar,
					   tid,
					   arquivo_termo)
					values
					  ({0}seq_tab_fisc_consid_final.nextval,
					   :fiscalizacao,
					   :descrever,
					   :tem_reparacao,
					   :reparacao,
					   :tem_termo_comp,
					   :tem_termo_comp_justificar,
					   :tid,
					   :arquivo_termo)
					returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", consideracaoFinal.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntClob("descrever", consideracaoFinal.Descrever);
				comando.AdicionarParametroEntrada("tem_reparacao", consideracaoFinal.HaReparacao, DbType.Int32);
				comando.AdicionarParametroEntrada("reparacao", DbType.String, 2000, consideracaoFinal.Reparacao);
				comando.AdicionarParametroEntrada("tem_termo_comp", consideracaoFinal.HaTermoCompromisso, DbType.Int32);
				comando.AdicionarParametroEntrada("tem_termo_comp_justificar", DbType.String, 250, consideracaoFinal.TermoCompromissoJustificar);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("arquivo_termo", (consideracaoFinal.Arquivo ?? new Arquivo()).Id, DbType.Int32);

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				consideracaoFinal.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Testemunhas

				foreach (var item in consideracaoFinal.Testemunhas)
				{
					comando = bancoDeDados.CriarComando(@"
						insert into {0}tab_fisc_consid_final_test
						  (id, 
						   consid_final, 
						   idaf, 
						   testemunha, 
						   nome, 
						   endereco, 
						   tid,
						   testemunha_setor,
                           cpf)
						values
						  ({0}seq_tab_fiscconsidfinaltest.nextval, 
						   :consid_final, 
						   :idaf, 
						   :testemunha, 
						   :nome, 
						   :endereco, 
						   :tid,
						   :testemunha_setor,
                           :cpf)
						returning id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("idaf", item.TestemunhaIDAF, DbType.Int32);
					comando.AdicionarParametroEntrada("testemunha", item.TestemunhaId, DbType.Int32);
					comando.AdicionarParametroEntrada("nome", DbType.String, 80, item.TestemunhaNome);
					comando.AdicionarParametroEntrada("endereco", DbType.String, 200, item.TestemunhaEndereco);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("testemunha_setor", item.TestemunhaSetorId, DbType.Int32);
                    comando.AdicionarParametroEntrada("cpf", item.TestemunhaCPF, DbType.String);
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Assinantes

				foreach (var item in consideracaoFinal.Assinantes)
				{
					comando = bancoDeDados.CriarComando(@"
						insert into {0}tab_fisc_consid_final_ass(id, consid_final, funcionario, cargo, tid)
						values ({0}seq_fisc_consid_final_ass.nextval, :consid_final, :funcionario, :cargo, :tid)
						returning id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
					comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Anexos

				foreach (var item in consideracaoFinal.Anexos)
				{
					comando = bancoDeDados.CriarComando(@"
					 insert into {0}tab_fisc_consid_final_arq a
					   (id, 
						consid_final, 
						arquivo, 
						ordem, 
						descricao, 
						tid)
					 values
					   ({0}seq_fisc_consid_final_arq.nextval,
						:consid_final,
						:arquivo,
						:ordem,
						:descricao,
						:tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

                #region Anexos IUF

                foreach (var item in consideracaoFinal.AnexosIUF)
                {
                    comando = bancoDeDados.CriarComando(@"
					 insert into {0}tab_fisc_consid_final_iuf a
					   (id, 
						consid_final, 
						arquivo, 
						ordem, 
						descricao, 
						tid)
					 values
					   ({0}seq_fisc_consid_final_iuf.nextval,
						:consid_final,
						:arquivo,
						:ordem,
						:descricao,
						:tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
                    comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

				Historico.Gerar(consideracaoFinal.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(consideracaoFinal.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}
			return consideracaoFinal.Id;
		}

		public void Editar(ConsideracaoFinal consideracaoFinal, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				  update {0}tab_fisc_consid_final t
					 set t.descrever                 = :descrever,
						 t.tem_reparacao             = :tem_reparacao,
						 t.reparacao                 = :reparacao,
						 t.tem_termo_comp            = :tem_termo_comp,
						 t.tem_termo_comp_justificar = :tem_termo_comp_justificar,
						 t.tid                       = :tid,
						 t.arquivo_termo             = :arquivo_termo
				   where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntClob("descrever", consideracaoFinal.Descrever);
				comando.AdicionarParametroEntrada("tem_reparacao", consideracaoFinal.HaReparacao, DbType.Int32);
				comando.AdicionarParametroEntrada("reparacao", DbType.String, 2000, consideracaoFinal.Reparacao);
				comando.AdicionarParametroEntrada("tem_termo_comp", consideracaoFinal.HaTermoCompromisso, DbType.Int32);
				comando.AdicionarParametroEntrada("tem_termo_comp_justificar", DbType.String, 250, consideracaoFinal.TermoCompromissoJustificar);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("arquivo_termo", (consideracaoFinal.Arquivo ?? new Arquivo()).Id, DbType.Int32);
				comando.AdicionarParametroEntrada("id", consideracaoFinal.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Testemunhas

				comando = bancoDeDados.CriarComando("delete from {0}tab_fisc_consid_final_test ra ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ra.consid_final = :consid_final{0}",
					comando.AdicionarNotIn("and", "ra.id", DbType.Int32, consideracaoFinal.Testemunhas.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in consideracaoFinal.Testemunhas)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}tab_fisc_consid_final_test t
							   set t.idaf				= :idaf, 
								   t.testemunha			= :testemunha, 
								   t.nome				= :nome, 
								   t.endereco			= :endereco, 
								   t.tid				= :tid,
								   t.testemunha_setor	= :testemunha_setor,
                                   t.cpf                = :cpf
							 where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}tab_fisc_consid_final_test
							  (id, 
							   consid_final, 
							   idaf, 
							   testemunha, 
							   nome, 
							   endereco, 
							   tid,
						       testemunha_setor,
                               cpf)
							values
							  ({0}seq_tab_fiscconsidfinaltest.nextval, 
							   :consid_final, 
							   :idaf, 
							   :testemunha, 
							   :nome, 
							   :endereco, 
							   :tid,
						       :testemunha_setor,
                               :cpf)
							returning id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
						comando.AdicionarParametroSaida("id", DbType.Int32);
					}

					comando.AdicionarParametroEntrada("idaf", item.TestemunhaIDAF, DbType.Int32);
					comando.AdicionarParametroEntrada("testemunha", item.TestemunhaId, DbType.Int32);
					comando.AdicionarParametroEntrada("nome", DbType.String, 80, item.TestemunhaNome);
					comando.AdicionarParametroEntrada("endereco", DbType.String, 80, item.TestemunhaEndereco);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("testemunha_setor", item.TestemunhaSetorId, DbType.Int32);
                    comando.AdicionarParametroEntrada("cpf", item.TestemunhaCPF, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = item.Id > 0 ? item.Id : Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Assinantes

				comando = bancoDeDados.CriarComando("delete from {0}tab_fisc_consid_final_ass ass ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ass.consid_final = :consid_final{0}",
					comando.AdicionarNotIn("and", "ass.id", DbType.Int32, consideracaoFinal.Assinantes.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in consideracaoFinal.Assinantes)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}tab_fisc_consid_final_ass t
							   set t.funcionario	= :funcionario, 
								   t.cargo			= :cargo, 
								   t.tid			= :tid
							 where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}tab_fisc_consid_final_ass(id, consid_final, funcionario, cargo, tid)
						values ({0}seq_fisc_consid_final_ass.nextval, :consid_final, :funcionario, :cargo, :tid)
						returning id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
					}


					comando.AdicionarParametroEntrada("funcionario", item.FuncionarioId, DbType.Int32);
					comando.AdicionarParametroEntrada("cargo", item.FuncionarioCargoId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Anexos

				comando = bancoDeDados.CriarComando("delete from {0}tab_fisc_consid_final_arq ra ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ra.consid_final = :consid_final{0}",
					comando.AdicionarNotIn("and", "ra.id", DbType.Int32, consideracaoFinal.Anexos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in consideracaoFinal.Anexos)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}tab_fisc_consid_final_arq t
							   set t.arquivo   = :arquivo,
								   t.ordem     = :ordem,
								   t.descricao = :descricao,
								   t.tid       = :tid
							 where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}tab_fisc_consid_final_arq a
							  (id, 
							   consid_final, 
							   arquivo, 
							   ordem, 
							   descricao, 
							   tid)
							values
							  ({0}seq_fisc_consid_final_arq.nextval,
							   :consid_final,
							   :arquivo,
							   :ordem,
							   :descricao,
							   :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

                #region Anexos IUF

                comando = bancoDeDados.CriarComando("delete from {0}tab_fisc_consid_final_iuf ra ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where ra.consid_final = :consid_final{0}",
                    comando.AdicionarNotIn("and", "ra.id", DbType.Int32, consideracaoFinal.AnexosIUF.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                foreach (var item in consideracaoFinal.AnexosIUF)
                {
                    if (item.Id > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
							update {0}tab_fisc_consid_final_iuf t
							   set t.arquivo   = :arquivo,
								   t.ordem     = :ordem,
								   t.descricao = :descricao,
								   t.tid       = :tid
							 where t.id = :id", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
							insert into {0}tab_fisc_consid_final_iuf a
							  (id, 
							   consid_final, 
							   arquivo, 
							   ordem, 
							   descricao, 
							   tid)
							values
							  ({0}seq_fisc_consid_final_iuf.nextval,
							   :consid_final,
							   :arquivo,
							   :ordem,
							   :descricao,
							   :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);
                    }

                    comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
                    comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

				Historico.Gerar(consideracaoFinal.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(consideracaoFinal.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(
					"begin " +
					  "delete {0}tab_fisc_consid_final_arq t where t.consid_final = (select id from {0}tab_fisc_consid_final where fiscalizacao = :fiscalizacao); " +
					  "delete {0}tab_fisc_consid_final_test t where t.consid_final = (select id from {0}tab_fisc_consid_final where fiscalizacao = :fiscalizacao); " +
					  "delete {0}tab_fisc_consid_final_ass t where t.consid_final = (select id from {0}tab_fisc_consid_final where fiscalizacao = :fiscalizacao); " +
					  "delete {0}tab_fisc_consid_final t where t.fiscalizacao = :fiscalizacao; " +
					"end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public ConsideracaoFinal Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			ConsideracaoFinal consideracaoFinal = new ConsideracaoFinal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select t.id                        Id,
						   t.fiscalizacao              FiscalizacaoId,
						   t.justificar                Justificar,
						   t.descrever                 Descrever,
						   t.tem_reparacao             HaReparacao,
						   t.reparacao                 Reparacao,
						   t.tem_termo_comp            HaTermoCompromisso,
						   t.tem_termo_comp_justificar TermoCompromissoJustificar,
						   t.tid                       Tid,
						   t.arquivo_termo             arquivo_id,
						   ta.nome                     arquivo_nome
					  from {0}tab_fisc_consid_final t,
						   {0}tab_arquivo           ta
					 where t.arquivo_termo = ta.id(+)
					   and t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				consideracaoFinal = bancoDeDados.ObterEntity<ConsideracaoFinal>(comando, (IDataReader reader, ConsideracaoFinal item) =>
				{
					item.Arquivo = new Arquivo();
					item.Arquivo.Id = reader.GetValue<int>("arquivo_id");
					item.Arquivo.Nome = reader.GetValue<string>("arquivo_nome");
				});

				#region Testemunhas

				comando = bancoDeDados.CriarComando(@"
					select t.id               Id,
						   t.consid_final     ConsideracaoFinalId,
						   t.idaf             TestemunhaIDAF,
						   t.testemunha       TestemunhaId,
						   t.nome             TestemunhaNome,
						   t.endereco         TestemunhaEndereco,
						   t.tid              Tid,
						   t.testemunha_setor TestemunhaSetorId,
                           t.cpf              TestemunhaCPF
					  from {0}tab_fisc_consid_final_test t
					 where t.consid_final = :consid_final", EsquemaBanco);

				comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

				consideracaoFinal.Testemunhas = bancoDeDados.ObterEntityList<ConsideracaoFinalTestemunha>(comando);

				#endregion

				#region Assinantes
				comando = bancoDeDados.CriarComando(@"select ta.id, ta.consid_final, f.id func_id, f.nome func_nome, ta.cargo, c.nome cargo_nome, ta.tid 
					from {0}tab_fisc_consid_final_ass ta, {0}tab_funcionario f, {0}tab_cargo c 
					where ta.funcionario = f.id and ta.cargo = c.id and ta.consid_final = :consid_final", EsquemaBanco);
				comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					FiscalizacaoAssinante item;
					while (reader.Read())
					{
						item = new FiscalizacaoAssinante();
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<String>("tid");
						item.FuncionarioId = reader.GetValue<int>("func_id");
						item.FuncionarioNome = reader.GetValue<String>("func_nome");
						item.FuncionarioCargoId = reader.GetValue<int>("cargo");
						item.FuncionarioCargoNome = reader.GetValue<String>("cargo_nome");
						item.Selecionado = true;

						consideracaoFinal.Assinantes.Add(item);
					}
					reader.Close();
				}
				#endregion

				#region Anexos

				comando = bancoDeDados.CriarComando(@"
				select a.id Id,
					   a.ordem Ordem,
					   a.descricao Descricao,
					   b.nome,
					   b.extensao,
					   b.id arquivo_id,
					   b.caminho,
					   a.tid Tid
				  from {0}tab_fisc_consid_final_arq a, 
					   {0}tab_arquivo b
				 where a.arquivo = b.id
				   and a.consid_final = :consid_final
				 order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

				consideracaoFinal.Anexos = bancoDeDados.ObterEntityList<Anexo>(comando, (IDataReader reader, Anexo item) =>
				{
					item.Arquivo.Id = reader.GetValue<int>("arquivo_id");
					item.Arquivo.Caminho = reader.GetValue<string>("caminho");
					item.Arquivo.Nome = reader.GetValue<string>("nome");
					item.Arquivo.Extensao = reader.GetValue<string>("extensao");
				});

				#endregion

                #region Anexos IUF

                comando = bancoDeDados.CriarComando(@"
				select a.id Id,
					   a.ordem Ordem,
					   a.descricao Descricao,
					   b.nome,
					   b.extensao,
					   b.id arquivo_id,
					   b.caminho,
					   a.tid Tid
				  from {0}tab_fisc_consid_final_iuf a, 
					   {0}tab_arquivo b
				 where a.arquivo = b.id
				   and a.consid_final = :consid_final
				 order by a.ordem", EsquemaBanco);

                comando.AdicionarParametroEntrada("consid_final", consideracaoFinal.Id, DbType.Int32);

                consideracaoFinal.AnexosIUF = bancoDeDados.ObterEntityList<Anexo>(comando, (IDataReader reader, Anexo item) =>
                {
                    item.Arquivo.Id = reader.GetValue<int>("arquivo_id");
                    item.Arquivo.Caminho = reader.GetValue<string>("caminho");
                    item.Arquivo.Nome = reader.GetValue<string>("nome");
                    item.Arquivo.Extensao = reader.GetValue<string>("extensao");
                });

                #endregion
			}
			return consideracaoFinal;
		}

		internal List<PessoaLst> ObterFuncionarios()
		{
			List<PessoaLst> lst = new List<PessoaLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select t.id   Id, 
						   t.nome Texto,
						   1      IsAtivo
					  from {0}tab_funcionario t
					 where t.situacao not in (2 /*Bloqueado*/, 4 /*Ausente*/)
					   and t.tipo = 3 /*Funcionário*/
					 order by t.nome", EsquemaBanco);

				lst = bancoDeDados.ObterEntityList<PessoaLst>(comando);
			}

			return lst;
		}

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_consid_final t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

		#endregion
	}
}