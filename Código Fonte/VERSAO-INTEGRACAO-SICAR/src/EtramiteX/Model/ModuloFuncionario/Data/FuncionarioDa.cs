using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFuncionario;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFuncionario.Data
{
	class FuncionarioDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		#region DMLs

		internal void Salvar(Funcionario funcionario, BancoDeDados banco = null)
		{
			if (funcionario == null)
			{
				throw new Exception("Funcionario nulo.");
			}

			if (funcionario.Id <= 0)
			{
				Criar(funcionario, banco);
			}
			else
			{
				Editar(funcionario, banco);
			}
		}

		private void Criar(Funcionario funcionario, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Funcionario

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_funcionario (id, usuario, arquivo, nome, cpf, situacao, email, tipo, tid) values 
				(seq_funcionario.nextval, :usuario, :arquivo, :nome, :cpf, 3, :email, 3, :tid) returning id into :id ");

				comando.AdicionarParametroEntrada("usuario", funcionario.Usuario.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("arquivo", (funcionario.Arquivo != null && funcionario.Arquivo.Id > 0) ? funcionario.Arquivo.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, funcionario.Nome);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 14, funcionario.Cpf);
				comando.AdicionarParametroEntrada("email", DbType.String, 250, funcionario.Email);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				funcionario.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Cargos

				comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_cargo (id, funcionario, cargo, tid) values 
				(seq_funcionario_cargo.nextval, :funcionario, :cargo, :tid)");

				foreach (Cargo item in funcionario.Cargos)
				{
					comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("cargo", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Setor

				comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_setor (id, funcionario, setor, tid) values 
				(seq_funcionario_setor.nextval, :funcionario, :setor, :tid)");

				foreach (Setor item in funcionario.Setores)
				{
					comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				comando = bancoDeDados.CriarComando(@"update tab_setor s set s.responsavel = :funcionario, s.tid = :tid where s.id = :setor");

				foreach (Setor item in funcionario.Setores)
				{
					if (!item.EhResponsavel)
					{
						continue;
					}

					comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);

					Historico.Gerar(item.Id, eHistoricoArtefato.setor, eHistoricoAcao.atualizar, bancoDeDados);
				}

				#endregion

				#region Papel

				comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_papel (id, funcionario, papel, tid) values 
				(seq_funcionario_papel.nextval, :funcionario, :papel, :tid)");

				foreach (Papel item in funcionario.Papeis)
				{
					comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("papel", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Permissao

				comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_Permissao (id, funcionario, permissao, tid) values 
				(seq_funcionario_Permissao.nextval, :funcionario, :permissao, :tid)");

				foreach (Permissao item in funcionario.Permissoes)
				{
					comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("permissao", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(funcionario.Id, eHistoricoArtefato.funcionario, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		private void Editar(Funcionario funcionario, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Funcionario

                Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario set nome = :nome, arquivo = :arquivo, cpf = :cpf, situacao = :situacao, email = :email, tid= :tid where id = :id");

				comando.AdicionarParametroEntrada("nome", DbType.String, 80, funcionario.Nome);
                comando.AdicionarParametroEntrada("arquivo", (funcionario.Arquivo != null && funcionario.Arquivo.Id > 0) ? funcionario.Arquivo.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 14, funcionario.Cpf);
				comando.AdicionarParametroEntrada("situacao", funcionario.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("email", DbType.String, 250, funcionario.Email);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", funcionario.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Cargos

				//Limpar Cargos
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from tab_funcionario_cargo c where c.funcionario = :funcionario{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, funcionario.Cargos.Select(x => x.IdRelacao).ToList()));

				comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Cargo item in funcionario.Cargos)
				{
					if (item.IdRelacao > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_funcionario_cargo set tid = :tid where id = :id");
						comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_cargo (id, funcionario, cargo, tid) values 
						(seq_funcionario_cargo.nextval, :funcionario, :cargo, :tid)");

						comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("cargo", item.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Setor

				List<Setor> setoresAtuais = ObterSetoresFuncionario(funcionario.Id, bancoDeDados);

				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from tab_funcionario_setor c where c.funcionario = :funcionario{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, funcionario.Setores.Select(x => x.IdRelacao).ToList()));

				comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Setor item in funcionario.Setores)
				{
					if (item.IdRelacao > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_funcionario_setor set tid = :tid where id = :id");
						comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_setor (id, funcionario, setor, tid) values 
						(seq_funcionario_setor.nextval, :funcionario, :setor, :tid)");

						comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				//Tabela de Setor
				comando = bancoDeDados.CriarComando(@"update tab_setor set responsavel = null, tid = :tid where responsavel = :funcionario");
				comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Setor item in setoresAtuais)
				{
					if (item.EhResponsavel && (!funcionario.Setores.Exists(x => x.Id == item.Id) || !funcionario.Setores.Single(x => x.Id == item.Id).EhResponsavel))
					{
						Historico.Gerar(item.Id, eHistoricoArtefato.setor, eHistoricoAcao.atualizar, bancoDeDados);
					}
				}

				comando = bancoDeDados.CriarComando(@"update tab_setor set responsavel = :funcionario, tid = :tid where id = :setor");
				foreach (Setor item in funcionario.Setores)
				{
					if (!item.EhResponsavel)
					{
						continue;
					}

					comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("setor", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);

					Historico.Gerar(item.Id, eHistoricoArtefato.setor, eHistoricoAcao.atualizar, bancoDeDados);
				}

				#endregion

				#region Papel

				//Limpar Papel 
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from tab_funcionario_papel c where c.funcionario = :funcionario {0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, funcionario.Papeis.Select(x => x.IdRelacao).ToList()));

				comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Papel item in funcionario.Papeis)
				{
					if (item.IdRelacao > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_funcionario_papel set tid = :tid where id = :id");
						comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_papel (id, funcionario, papel, tid) values 
						(seq_funcionario_papel.nextval, :funcionario, :papel, :tid)");

						comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("papel", item.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Permissao

				//Limpar Permissao
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from tab_funcionario_permissao c where c.funcionario = :funcionario{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, funcionario.Permissoes.Select(x => x.IdRelacao).ToList()));

				comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Permissao item in funcionario.Permissoes)
				{
					if (item.IdRelacao > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_funcionario_permissao set tid = :tid where id = :id");
						comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_permissao (id, funcionario, permissao, tid) values 
						(seq_funcionario_Permissao.nextval, :funcionario, :permissao, :tid)");

						comando.AdicionarParametroEntrada("funcionario", funcionario.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("permissao", item.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(funcionario.Id, eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta consulta = new Consulta();
				consulta.Gerenciar(funcionario.Id, eHistoricoArtefato.funcionario, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(int id, int situacao, string motivo = null, Funcionario executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				GerenciadorTransacao.ObterIDAtual();
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.situacao = :situacao, f.situacao_motivo = :situacao_motivo, 
				f.tentativa = 0, f.tid = :tid where f.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_motivo", DbType.String, 100, motivo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				Historico.Gerar(id, eHistoricoArtefato.funcionario, eHistoricoAcao.alterarsituacao, bancoDeDados, executor != null ? executor.Executor() : null);
				bancoDeDados.Commit();
			}
		}

		internal void AlterarSenha(int usuarioId, Funcionario executor = null, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.situacao = 1, f.tid = :tid where f.usuario = :usuario");
				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando.DbCommand.Parameters.Clear();
				comando.DbCommand.CommandText = @"select f.id from tab_funcionario f where f.usuario = :usuario";
				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				int idFunc = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				Historico.Gerar(idFunc, eHistoricoArtefato.funcionario, eHistoricoAcao.alterarsenha, bancoDeDados, executor != null ? executor.Executor() : null);
				bancoDeDados.Commit();
			}
		}

		internal void Autenticar(Funcionario func, string sessionIdForcarLogoff)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();
				try
				{
					Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.tentativa = 0, logado = 1, f.session_Id = :sessionId where f.id = :id");

					comando.AdicionarParametroEntrada("id", func.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("sessionId", func.SessionId, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					if (!String.IsNullOrEmpty(sessionIdForcarLogoff))
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_funcionario_deslogar (id, login, session_Id, data_acao) 
							values (seq_funcionario_deslogar.nextval, :login, :sessionForcaLogoff, sysdate)");

						comando.AdicionarParametroEntrada("login", func.Usuario.Login, DbType.String);
						comando.AdicionarParametroEntrada("sessionForcaLogoff", sessionIdForcarLogoff, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
					bancoDeDados.Commit();
				}
				catch
				{
					bancoDeDados.Rollback();
					throw;
				}
			}
		}

		internal void Deslogar(string login, BancoDeDados banco = null)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.logado = 0 where f.usuario = (select u.id from tab_usuario u where u.login = :login)");
				comando.AdicionarParametroEntrada("login", login, DbType.String);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		internal void AdicionarPermissaoTramitarRegistro(List<int> funcionarios, List<string> permissaoCodigos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				IDataReader reader;
				Comando comando;

				#region Remove

				comando = bancoDeDados.CriarComando("select distinct fp.funcionario from {0}tab_funcionario_permissao fp, {0}lov_autenticacao_permissao ap where fp.permissao = ap.id ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "ap.codigo", DbType.String, permissaoCodigos);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "fp.funcionario", DbType.Int32, funcionarios);

				reader = bancoDeDados.ExecutarReader(comando);

				comando = bancoDeDados.CriarComando("delete from {0}tab_funcionario_permissao fp where ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("fp.permissao in (select ap.id from {0}lov_autenticacao_permissao ap where ap.codigo in ('{1}'))",
				(string.IsNullOrEmpty(EsquemaBanco) ? "" : "."), String.Join("','", permissaoCodigos));
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "fp.funcionario", DbType.Int32, funcionarios);

				bancoDeDados.ExecutarNonQuery(comando);

				while (reader.Read())
				{
					Historico.Gerar(Convert.ToInt32(reader["funcionario"]), eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados);
				}

				reader.Close();

				#endregion

				#region Atualiza

				comando = bancoDeDados.CriarComando("select distinct fp.funcionario from {0}tab_funcionario_permissao fp, {0}lov_autenticacao_permissao ap where fp.permissao = ap.id ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "ap.codigo", DbType.String, permissaoCodigos);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "fp.funcionario", DbType.Int32, funcionarios);

				reader = bancoDeDados.ExecutarReader(comando);

				comando = bancoDeDados.CriarComando("update {0}tab_funcionario_permissao fp set fp.tid = :tid where ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("fp.permissao in (select ap.id from {0}lov_autenticacao_permissao ap where ap.codigo in ('{1}'))",
				(string.IsNullOrEmpty(EsquemaBanco) ? "" : "."), String.Join("','", permissaoCodigos));
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "fp.funcionario", DbType.Int32, funcionarios);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				while (reader.Read())
				{
					funcionarios.Remove(Convert.ToInt32(reader["funcionario"]));
					Historico.Gerar(Convert.ToInt32(reader["funcionario"]), eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados);
				}

				reader.Close();

				#endregion

				#region Insere

				if (funcionarios != null && funcionarios.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_funcionario_permissao (id, funcionario, permissao, tid)
					values ({0}seq_funcionario_permissao.nextval, :funcionario, (select ap.id from {0}lov_autenticacao_permissao ap where ap.codigo = :codigo), :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("funcionario", DbType.Int32);
					comando.AdicionarParametroEntrada("codigo", DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (int funcionario in funcionarios)
					{
						comando.SetarValorParametro("funcionario", funcionario);

						foreach (string codigo in permissaoCodigos)
						{
							comando.SetarValorParametro("codigo", codigo);
							bancoDeDados.ExecutarNonQuery(comando);
						}

						Historico.Gerar(funcionario, eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados);
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion 

		#region Buscar / Listar

		internal Funcionario Obter(int id)
		{
			Funcionario funcionario = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
                Comando comando = bancoDeDados.CriarComando(@"select f.id, f.usuario, f.nome, f.arquivo, aa.nome arquivo_nome, f.cpf, f.situacao, f.situacao_motivo, f.email, f.tipo, f.tid, u.login 
					from tab_funcionario f, tab_usuario u, tab_arquivo aa where f.id = :id  and f.arquivo = aa.id(+) and f.usuario = u.id");


				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						funcionario = new Funcionario();
						funcionario.Id = id;
						funcionario.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						funcionario.Usuario.Login = reader["login"].ToString();
						funcionario.Nome = reader["nome"].ToString();
                        funcionario.Arquivo.Id = reader.GetValue<Int32>("arquivo");
                        funcionario.Arquivo.Nome = reader.GetValue<String>("arquivo_nome");
						funcionario.Cpf = reader["cpf"].ToString();
						funcionario.Email = (reader["email"] ?? String.Empty).ToString();
						funcionario.Tipo = Convert.ToInt32(reader["tipo"]);
						funcionario.Situacao = Convert.ToInt32(reader["situacao"]);
						funcionario.SituacaoMotivo = Convert.IsDBNull(reader["situacao_motivo"]) ? String.Empty : reader["situacao_motivo"].ToString();
						funcionario.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				if (funcionario == null)
				{
					return funcionario;
				}

				#region Cargo
				comando = bancoDeDados.CriarComando(@"select c.id, f.id idRelacao, c.nome, c.tid  from tab_funcionario_cargo f, tab_cargo c 
					where f.funcionario = :id and f.cargo = c.id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						funcionario.Cargos.Add(new Cargo()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString(),
							IdRelacao = Convert.ToInt32(reader["idRelacao"]),
							IsAtivo = true
						});
					}

					reader.Close();
				}
				#endregion

				#region Setor
				comando = bancoDeDados.CriarComando(@"select s.id, f.id idRelacao, s.nome, s.sigla, s.responsavel, s.tid from tab_funcionario_setor f, tab_setor s 
					where f.setor = s.id and f.funcionario = :id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						funcionario.Setores.Add(new Setor()
						{
							Id = Convert.ToInt32(reader["id"]),
							IdRelacao = Convert.ToInt32(reader["idRelacao"]),
							Nome = reader["nome"].ToString(),
							Sigla = reader["sigla"].ToString(),
							Responsavel = Convert.ToInt32((reader["responsavel"] == DBNull.Value) ? 0 : reader["responsavel"]),
							EhResponsavel = (id == Convert.ToInt32((reader["responsavel"] == DBNull.Value) ? 0 : reader["responsavel"]))
						});
					}

					reader.Close();
				}
				#endregion

				#region Papel
				comando = bancoDeDados.CriarComando(@"select p.id, t.id idRelacao, p.nome, p.funcionario_tipo, p.tid from tab_funcionario_papel t, 
					tab_autenticacao_papel p where t.papel = p.id and t.funcionario = :id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						funcionario.Papeis.Add(new Papel()
						{
							Id = Convert.ToInt32(reader["id"]),
							IdRelacao = Convert.ToInt32(reader["idRelacao"]),
							Nome = reader["nome"].ToString()
						});
					}

					reader.Close();
				}
				#endregion

				#region Permissao
				comando = bancoDeDados.CriarComando(@"select p.id, t.id idRelacao, p.nome, p.codigo, p.funcionario_tipo, p.descricao 
					from tab_funcionario_permissao t, lov_autenticacao_permissao p where t.permissao = p.id and t.funcionario = :id");
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						funcionario.Permissoes.Add(new Permissao()
						{
							Id = Convert.ToInt32(reader["id"]),
							IdRelacao = Convert.ToInt32(reader["idRelacao"]),
							Nome = reader["nome"].ToString(),
							Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString()),
							FuncionarioTipo = Convert.ToInt32(reader["funcionario_tipo"]),
							Descricao = reader["descricao"].ToString()
						});
					}

					reader.Close();
				}
				#endregion
			}

			return funcionario;
		}

		internal Funcionario Obter(string Cpf)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_funcionario f where f.cpf = :cpf and f.tipo = 3");
				comando.AdicionarParametroEntrada("cpf", DbType.String, 14, Cpf);
				int id = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				if (id <= 0)
				{
					return null;
				}
				return Obter(id);
			}
		}

		internal Funcionario ObterFuncionarioAutenticacao(string login)
		{
			Funcionario funcionario = new Funcionario();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select f.id, f.usuario, f.nome, f.tipo, u.login, f.email, u.logon_anterior_ip, u.logon_anterior_data, 
					t.texto tipo_texto, f.tid from tab_funcionario f, tab_usuario u, lov_funcionario_tipo t where u.login = :login and f.usuario = u.id 
					and f.situacao = 1 and f.tipo = t.id");

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						funcionario.Id = Convert.ToInt32(reader["id"]);
						funcionario.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						funcionario.Usuario.Login = reader["login"].ToString();
						funcionario.Usuario.IpUltimoLogon = reader["logon_anterior_ip"].ToString();
						funcionario.Usuario.DataUltimoLogon = (reader["logon_anterior_data"] != DBNull.Value) ? Convert.ToDateTime(reader["logon_anterior_data"]) : new DateTime?();
						funcionario.Nome = reader["nome"].ToString();
						funcionario.Email = reader["email"].ToString();
						funcionario.Tipo = Convert.ToInt32(reader["tipo"]);
						funcionario.TipoTexto = reader["tipo_texto"].ToString();
						funcionario.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#region Papel / Permissao
				comando = bancoDeDados.CriarComando(@"
					select p.codigo 
					  from tab_funcionario_papel f, tab_autenticacao_papel_perm app, lov_autenticacao_permissao p 
					 where f.funcionario = :id and f.papel = app.papel and app.permissao = p.id 
					union
					select p.codigo 
					from tab_funcionario_permissao t, lov_autenticacao_permissao p 
					where t.permissao = p.id and t.funcionario = :id");
				comando.AdicionarParametroEntrada("id", funcionario.Id, DbType.Int32);
				
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (!Enum.IsDefined(typeof(ePermissao), reader["codigo"].ToString()))
						{
							Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = String.Format("Permissão \"{0}\" não definida! [Verifique Enumerado e commite !]", reader["codigo"].ToString()) });
							continue;
						}

						funcionario.Permissoes.Add(new Permissao()
						{
							Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString())
						});
					}

					reader.Close();
				}
				#endregion
			}

			return funcionario;
		}

		internal Funcionario ObterFuncionarioExecutor(string login)
		{
			Funcionario funcionario = new Funcionario();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select f.id, f.tid, f.usuario, f.nome, f.tipo, u.login, f.email, u.logon_anterior_ip, u.logon_anterior_data, 
					t.texto tipo_texto from tab_funcionario f, tab_usuario u, lov_funcionario_tipo t where u.login = :login and f.usuario = u.id 
					and f.tipo = t.id");

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						funcionario.Id = Convert.ToInt32(reader["id"]);
						funcionario.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						funcionario.Usuario.Login = reader["login"].ToString();
						funcionario.Usuario.IpUltimoLogon = reader["logon_anterior_ip"].ToString();
						funcionario.Usuario.DataUltimoLogon = (reader["logon_anterior_data"] != DBNull.Value) ? Convert.ToDateTime(reader["logon_anterior_data"]) : new DateTime?();
						funcionario.Nome = reader["nome"].ToString();						
						funcionario.Tipo = Convert.ToInt32(reader["tipo"]);
						funcionario.TipoTexto = reader["tipo_texto"].ToString();
						funcionario.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}
			}

			return funcionario;
		}

		internal int ObterSituacao(string login)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.situacao from tab_funcionario f, tab_usuario u where f.usuario = u.id and u.login = :login");
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public List<Papel> ObterPapeis()
		{
			List<Papel> lst = new List<Papel>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.nome from tab_autenticacao_papel c where c.funcionario_tipo = 3 and c.tipo = 1");
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						lst.Add(new Papel()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString()
						});
					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"select p.id, p.nome, p.codigo, p.funcionario_tipo, p.descricao from 
					lov_autenticacao_permissao p, tab_autenticacao_papel_perm pp where p.id = pp.permissao and pp.papel = :papel and p.funcionario_tipo = 3");

				foreach (Papel item in lst)
				{
					comando.DbCommand.Parameters.Clear();
					comando.AdicionarParametroEntrada("papel", item.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{

						while (reader.Read())
						{
							item.Permissoes.Add(new Permissao()
							{
								Id = Convert.ToInt32(reader["id"]),
								Nome = reader["nome"].ToString(),
								Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString()),
								FuncionarioTipo = Convert.ToInt32(reader["funcionario_tipo"]),
								Descricao = reader["descricao"].ToString()
							});
						}

						reader.Close();
					}
				}
			}

			return lst;
		}

		public Resultados<Funcionario> Filtrar(Filtro<FuncionarioListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Funcionario> retorno = new Resultados<Funcionario>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("f.cpf", "cpf", filtros.Dados.Cpf);

				comandtxt += comando.FiltroAndLike("f.nome", "nome", filtros.Dados.Nome, true);

				comandtxt += comando.FiltroAndLike("u.login", "login", filtros.Dados.Login, true);

				comandtxt += comando.FiltroAnd("f.situacao", "situacao", filtros.Dados.Situacao);

				comandtxt += comando.FiltroIn("f.id", "select b.funcionario from tab_funcionario_cargo b where b.cargo = :cargo", "cargo", filtros.Dados.Cargo);

				comandtxt += comando.FiltroIn("f.id", "select c.funcionario from tab_funcionario_setor c where c.setor = :setor", "setor", filtros.Dados.Setor);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome", "login", "cpf", "situacao", "cargo", "setor" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome");
				}

				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = "select count(*) from tab_funcionario f, tab_usuario u where f.usuario = u.id and f.tipo = 3" + comandtxt;

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select	f.id, f.tid, f.usuario, f.nome, u.login, f.cpf, f.situacao, lfs.texto situacao_texto, f.tipo, u.logon_data,
					(select stragg(ca.nome) from tab_cargo ca, tab_funcionario_cargo fc where ca.id = fc.cargo and fc.funcionario = f.id) cargo,
					(select stragg(nvl(s.sigla, s.nome)) from tab_setor s, tab_funcionario_setor fs where s.id = fs.setor and fs.funcionario = f.id) setor
					from tab_funcionario f, tab_usuario u, lov_funcionario_situacao lfs where f.usuario = u.id and f.situacao = lfs.id and f.tipo = 3 {0} {1}", comandtxt, DaHelper.Ordenar(colunas, ordenar));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Resultados

					Funcionario funcinario;
					while (reader.Read())
					{
						funcinario = new Funcionario();
						funcinario.Id = Convert.ToInt32(reader["id"]);
						funcinario.Tid = reader["tid"].ToString();
						funcinario.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						funcinario.Usuario.Login = reader["login"].ToString();

						if (reader["logon_data"] != null && !Convert.IsDBNull(reader["logon_data"]))
						{
							funcinario.Usuario.DataUltimoLogon = Convert.ToDateTime(reader["logon_data"]);
						}

						funcinario.Nome = reader["nome"].ToString();
						funcinario.Cpf = reader["cpf"].ToString();
						funcinario.SituacaoTexto = reader["situacao_texto"].ToString();
						funcinario.Situacao = Convert.ToInt32(reader["situacao"]);
						funcinario.Tipo = Convert.ToInt32(reader["tipo"]);
						funcinario.CargosStragg = !Convert.IsDBNull(reader["cargo"]) ? reader["cargo"].ToString() : String.Empty;
						funcinario.SetoresStragg = !Convert.IsDBNull(reader["setor"]) ? reader["setor"].ToString() : String.Empty;
						
						retorno.Itens.Add(funcinario);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Funcionario ObterFuncionarioLogin(int usuarioId, int timeout)
		{
			Funcionario funcionario = new Funcionario();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"update tab_funcionario f set f.tentativa = (nvl(f.tentativa,0)+1) where f.usuario = :usuarioId");
				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select f.id, f.nome, f.tipo, f.situacao, f.tentativa, f.logado, f.session_id, 
					(case when (u.ultima_acao+NUMTODSINTERVAL(:timeout, 'MINUTE'))  > sysdate then 1 else 0 end) forcar_logout 
					from tab_funcionario f, tab_usuario u where f.usuario = u.id and u.id = :usuarioId");
				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("timeout", timeout, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						funcionario.Id = Convert.ToInt32(reader["id"]);
						funcionario.Usuario.Id = usuarioId;
						funcionario.Situacao = Convert.ToInt32(reader["situacao"]);
						funcionario.Tentativa = Convert.ToInt32((reader["tentativa"] != DBNull.Value) ? reader["tentativa"] : 0);
						funcionario.SessionId = ((reader["session_id"] != DBNull.Value) ? reader["session_id"].ToString() : string.Empty);
						funcionario.Logado = ((reader["logado"] != DBNull.Value) ? Convert.ToBoolean(reader["logado"]) : false);
						funcionario.ForcarLogout = ((reader["forcar_logout"] != DBNull.Value) ? Convert.ToBoolean(reader["forcar_logout"]) : false);
						funcionario.Nome = reader["nome"].ToString();
						funcionario.Tipo = Convert.ToInt32(reader["tipo"]);
					}

					reader.Close();
				}
			}

			return funcionario;
		}

		internal FuncionarioLst ObterResponsavelSetor(int setorId, BancoDeDados banco = null)
		{
			FuncionarioLst funcionario = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select f.id, f.nome 
					from tab_setor s, tab_funcionario f 
					where f.id = s.responsavel and s.id = :setorId");

				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						funcionario = new FuncionarioLst()
						{
							Id = Convert.ToInt32(reader["id"]),
							Texto = reader["nome"].ToString(),
							IsAtivo = true
						};
					}

					reader.Close();
				}
			}
			return funcionario;
		}

		internal List<FuncionarioLst> ObterFuncionariosSetor(int setorId, BancoDeDados banco = null)
		{
			List<FuncionarioLst> lstFuncionario = new List<FuncionarioLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tf.id, tf.nome from tab_funcionario_setor tfs, tab_funcionario tf where
					tfs.funcionario = tf.id and tfs.setor = :id order by tf.nome");

				comando.AdicionarParametroEntrada("id", setorId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstFuncionario.Add(new FuncionarioLst()
						{
							Id = Convert.ToInt32(reader["id"]),
							Texto = reader["nome"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return lstFuncionario;
		}

		internal List<PessoaLst> ObterFuncionariosPorSetorFuncao(int setorId, int funcaoCod, BancoDeDados banco = null)
		{
			List<PessoaLst> lista = new List<PessoaLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.nome from tab_funcionario_setor fs, tab_funcionario f,  
															tab_funcionario_cargo fc, tab_cargo c where fs.funcionario = f.id and fc.funcionario = f.id 
															and fs.setor = :setor and fc.cargo = c.id and c.codigo = :cargo order by f.nome");

				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);
				comando.AdicionarParametroEntrada("cargo", funcaoCod, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(new PessoaLst()
						{
							Id = Convert.ToInt32(reader["id"]),
							Texto = reader["nome"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return lista;
		}

		internal List<Funcionario> ObterFuncionarios(List<int> ids, BancoDeDados banco = null)
		{
			List<Funcionario> lstFuncionario = new List<Funcionario>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tf.id, tf.nome from tab_funcionario tf ");
				comando.DbCommand.CommandText += comando.AdicionarIn("where", "tf.id", DbType.Int32, ids);
				comando.DbCommand.CommandText += " order by tf.nome";

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstFuncionario.Add(new Funcionario()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString()
						});
					}

					reader.Close();
				}
			}

			return lstFuncionario;
		}

		internal List<Setor> ObterSetoresFuncionario(int funcionarioId, BancoDeDados banco = null)
		{
			List<Setor> lstSetor = new List<Setor>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ts.id, ts.nome, tts.tipo tipo_tramitacao, ts.responsavel
								from tab_funcionario_setor tfs, tab_setor ts, tab_tramitacao_setor tts
								where tts.setor(+) = ts.id and tfs.setor = ts.id and tfs.funcionario = :id order by ts.nome");

				comando.AdicionarParametroEntrada("id", funcionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstSetor.Add(new Setor()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString(),
							IsAtivo = true,
							EhResponsavel = Convert.IsDBNull(reader["responsavel"]) ? false : Convert.ToInt32(reader["responsavel"]) == funcionarioId,
							TramitacaoTipoId = Convert.IsDBNull(reader["tipo_tramitacao"]) ? 1 : Convert.ToInt32(reader["tipo_tramitacao"])//1 - Normal
						});
					}

					reader.Close();
				}
			}
			return lstSetor;
		}

		internal List<Setor> ObterSetoresProtocolosEmPosse(int funcionarioId, BancoDeDados banco = null)
		{
			List<Setor> lstSetor = new List<Setor>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select po.setor id, ts.nome from {0}tab_protocolo po, {0}tab_setor ts where po.setor = ts.id and po.emposse = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", funcionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstSetor.Add(new Setor()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return lstSetor;
		}

		internal List<Setor> ObterSetoresPorTipo(int funcionarioId, int tipoTramitacaoSetor, BancoDeDados banco = null)
		{
			List<Setor> lstSetor = new List<Setor>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.nome from tab_setor s, tab_funcionario_setor fs, tab_tramitacao_setor ts where fs.funcionario = :id and ts.setor = fs.setor and ts.tipo = :tipo and s.id = ts.setor");
				comando.AdicionarParametroEntrada("id", funcionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", tipoTramitacaoSetor, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstSetor.Add(new Setor()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return lstSetor;
		}

		internal List<Setor> ObterSetoresRegistrador(int funcionarioId, BancoDeDados banco = null)
		{
			List<Setor> lstSetor = new List<Setor>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.nome from tab_tramitacao_setor_func fs, tab_setor s where fs.setor = s.id and fs.funcionario = :id");

				comando.AdicionarParametroEntrada("id", funcionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lstSetor.Add(new Setor()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString(),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}
			return lstSetor;
		}

		internal List<Cargo> ObterFuncionarioCargos(int funcionarioId)
		{
			List<Cargo> cargos = new List<Cargo>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select c.id, c.nome, fc.id id_relacao from tab_cargo c, tab_funcionario_cargo fc where fc.funcionario = :funcId and c.id = fc.cargo");
				comando.AdicionarParametroEntrada("funcId", funcionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						cargos.Add(new Cargo()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString(),
							IsAtivo = true,
							IdRelacao = Convert.ToInt32(reader["id_relacao"])
						});
					}

					reader.Close();
				}
			}
			return cargos;
		}

		internal List<Situacao> ObterSituacaoFuncionario()
		{
			List<Situacao> lst = new List<Situacao>();
			IEnumerable<IDataReader> daReader = DaHelper.ObterLista(@"select s.id, s.texto nome from lov_funcionario_situacao s");
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

		#endregion

		#region Verificar / Validar

		internal bool VerificarResponsavelSetor(int funcId, int setorId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.responsavel from tab_setor where id =:setorId");
				comando.AdicionarParametroEntrada("setorId", setorId, DbType.Int32);
				int? responsavelId = bancoDeDados.ExecutarScalar<int?>(comando);
				return responsavelId != null && responsavelId.HasValue && responsavelId.Value == funcId;
			}
		}

		internal List<string> VerificarResponsavelSetor(Funcionario funcionario)
		{
			List<string> retorno = new List<string>();
			if (funcionario.Setores.Where(x => x.EhResponsavel && (x.Responsavel??-1) != funcionario.Id).Count() > 0)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					Comando comando = bancoDeDados.CriarComando(@"select s.nome from tab_setor s 
						where s.id in (" + String.Join(",", (funcionario.Setores.Where(x => x.EhResponsavel && (x.Responsavel??-1) != funcionario.Id).Select(x => x.Id.ToString())).ToArray()) + @") and 
						s.responsavel is not null");

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							retorno.Add(reader["nome"].ToString());
						}
						reader.Close();
					}
				}
			}
			return retorno;
		}

		internal bool VerificarResponsavelSetor(int idSetor)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_setor s 
						where s.id = :idSetor and 
						s.responsavel is not null");
				comando.AdicionarParametroEntrada("idSetor", idSetor, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<Decimal>(comando) > 0);
			}
		}

		internal bool VerificarSenhaVencida(int usuarioId, Funcionario executor = null)
		{
			bool vencido = false;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				bancoDeDados.IniciarTransacao();
				GerenciadorTransacao.ObterIDAtual();

				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_usuario u, tab_funcionario f, 
				(select to_number(c.valor) dias from cnf_sistema c where c.campo = 'validadesenha') prazo
				where u.id = f.usuario and f.situacao <> 6 and u.id = :usuarioId and trunc(sysdate) > trunc(u.senha_data+(prazo.dias))");

				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);

				vencido = bancoDeDados.ExecutarScalar(comando).ToString() != "0";

				if (vencido)
				{
					comando.DbCommand.CommandText = @"update tab_funcionario f set f.situacao = 6 where f.usuario = :usuarioId"; //Vencida
					bancoDeDados.ExecutarNonQuery(comando);

					comando.DbCommand.CommandText = @"select f.id from tab_funcionario f where f.usuario = :usuarioId";

					int idFunc = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

					_historico.Gerar(idFunc, eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados, executor != null? executor.Executor():null);
				}

				bancoDeDados.Commit();

			}

			return vencido;
		}

		internal bool VerificarFuncionarioContidoSetor(int funcionarioId, int setorId, BancoDeDados banco = null)
		{
			bool contido = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_funcionario_setor fs where fs.funcionario = :funcionario and fs.setor = :setor");

				comando.AdicionarParametroEntrada("funcionario", funcionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);

				contido = Convert.ToString(bancoDeDados.ExecutarScalar(comando)) != "0";
			}

			return contido;
		}

		internal bool VerificarSeDeveDeslogar(String login, String sessionId, int timeout)
		{
			Comando comando = null;
			bool forcarLogoff = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				if (!String.IsNullOrEmpty(sessionId))
				{
					comando = bancoDeDados.CriarComando(@"select t.id from tab_funcionario_deslogar t where t.login = :login and t.session_Id = :session_Id
					union
					select f.id from tab_funcionario f, tab_usuario u where f.usuario = u.id and u.login = :login and f.session_id <> :session_Id");

					comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
					comando.AdicionarParametroEntrada("session_Id", DbType.String, 36, sessionId);

					int idLogoff = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
					forcarLogoff = (idLogoff != 0);

					if (forcarLogoff)
					{
						comando = bancoDeDados.CriarComando(@"delete from tab_funcionario_deslogar t where t.id = :id or (t.data_acao+NUMTODSINTERVAL(:timeout, 'MINUTE')) < sysdate");
						comando.AdicionarParametroEntrada("id", idLogoff, DbType.Int32);
						comando.AdicionarParametroEntrada("timeout", timeout, DbType.Int32);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
			}

			return forcarLogoff;
		}		

		#endregion
	}
}