using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAdministrador;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAdministrador.Data
{
	class AdministradorDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		public AdministradorDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Administrador obj, BancoDeDados banco = null)
		{
			if (obj == null)
			{
				throw new Exception("Administrador nulo.");
			}

			if (obj.Id <= 0)
			{
				Criar(obj, banco);
			}
			else
			{
				Editar(obj, banco);
			}
		}

		private void Criar(Administrador obj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_funcionario (id, usuario, nome, cpf, situacao, email, tipo, tid) values 
				({0}seq_funcionario.nextval, :usuario, :nome, :cpf, 3, :email, 2, :tid) returning id into :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("usuario", obj.Usuario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 80, obj.Nome);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 14, obj.Cpf);
				comando.AdicionarParametroEntrada("email", DbType.String, 250, obj.Email);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				obj.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Papel

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_funcionario_papel (id, funcionario, papel, tid) values 
				({0}seq_funcionario_papel.nextval, :funcionario, :papel, :tid)", EsquemaBanco);

				foreach (Papel item in obj.Papeis)
				{
					comando.AdicionarParametroEntrada("funcionario", obj.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("papel", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Permissao

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_funcionario_Permissao (id, funcionario, permissao, tid) values 
				({0}seq_funcionario_Permissao.nextval, :funcionario, :permissao, :tid)", EsquemaBanco);

				foreach (Permissao item in obj.Permissoes)
				{
					comando.AdicionarParametroEntrada("funcionario", obj.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("permissao", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(obj.Id, eHistoricoArtefato.funcionario, eHistoricoAcao.criar, bancoDeDados);
				bancoDeDados.Commit();
			}

		}

		private void Editar(Administrador obj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_funcionario set nome = :nome, cpf = :cpf, situacao = :situacao, email = :email, tid= :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", DbType.String, 80, obj.Nome);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 14, obj.Cpf);
				comando.AdicionarParametroEntrada("situacao", obj.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("email", DbType.String, 250, obj.Email);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", obj.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Papel

				//Limpar Papel
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from {1}tab_funcionario_papel c where c.funcionario = :funcionario{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, obj.Papeis.Select(x => x.IdRelacao).ToList()), EsquemaBanco);

				comando.AdicionarParametroEntrada("funcionario", obj.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Papel item in obj.Papeis)
				{
					if (item.IdRelacao > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_funcionario_papel set tid = :tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_funcionario_papel (id, funcionario, papel, tid) values 
						({0}seq_funcionario_papel.nextval, :funcionario, :papel, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("funcionario", obj.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("papel", item.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Permissao

				//Limpar Permissao
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from {1}tab_funcionario_permissao c where c.funcionario = :funcionario{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, obj.Permissoes.Select(x => x.IdRelacao).ToList()), EsquemaBanco);

				comando.AdicionarParametroEntrada("funcionario", obj.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Permissao item in obj.Permissoes)
				{
					if (item.IdRelacao > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_funcionario_permissao set tid = :tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacao, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_funcionario_Permissao (id, funcionario, permissao, tid) values 
						(seq_funcionario_Permissao.nextval, :funcionario, :permissao, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("funcionario", obj.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("permissao", item.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(obj.Id, eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados);
				bancoDeDados.Commit();
			}
		}

		internal void TransferirSistema(int id, string motivo = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				GerenciadorTransacao.ObterIDAtual();
				bancoDeDados.IniciarTransacao();

				#region Apaga as permissões do antigo sistema e passa para o novo adm

				Comando comando = bancoDeDados.CriarComando("declare id_ant integer; begin" +
				" select tf.id into id_ant from {0}tab_funcionario tf where tf.sistema = 1;" +
				" update {0}tab_funcionario f set f.sistema = 0, f.tid = :tid where f.id = id_ant;" +
				" update {0}tab_funcionario f set f.sistema = 1, f.sistema_motivo = :motivo, f.tid = :tid where f.id = :funcionario;" +
				" delete {0}tab_funcionario_papel fp where fp.funcionario = id_ant" +
				" and fp.papel in (select ap.id from {0}tab_autenticacao_papel ap where ap.funcionario_tipo = 1);" +
				" insert into {0}tab_funcionario_papel(id,funcionario,papel,tid)" +
				" (select {0}seq_funcionario_papel.nextval, :funcionario, ap.id, :tid from {0}tab_autenticacao_papel ap where ap.funcionario_tipo = 1); " +
				" :idPerdeu := id_ant; end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("funcionario", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("motivo", DbType.String, 100, motivo);
				comando.AdicionarParametroSaida("idPerdeu", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(comando.ObterValorParametro("idPerdeu")), eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados);
				Historico.Gerar(id, eHistoricoArtefato.funcionario, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(int id, int situacao, string motivo = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				GerenciadorTransacao.ObterIDAtual();
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_funcionario f set f.situacao = :situacao, f.situacao_motivo = :situacao_motivo, 
				f.tentativa = 0, f.tid = :tid where f.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_motivo", DbType.String, 100, motivo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				Historico.Gerar(id, eHistoricoArtefato.funcionario, eHistoricoAcao.alterarsituacao, bancoDeDados);
				bancoDeDados.Commit();
			}
		}

		internal void AlterarSenha(int usuarioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_funcionario f set f.situacao = 1, f.tid = :tid where f.usuario = :usuario", EsquemaBanco);
				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando.DbCommand.Parameters.Clear();
				comando.DbCommand.CommandText = String.Format(@"select f.id from {0}tab_funcionario f where f.usuario = :usuario", EsquemaBanco);
				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				int idFunc = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				Historico.Gerar(idFunc, eHistoricoArtefato.funcionario, eHistoricoAcao.alterarsenha, bancoDeDados);
				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Administrador Obter(int id)
		{
			Administrador obj = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id, f.usuario, f.nome, f.cpf, f.situacao, f.situacao_motivo, f.email, f.tipo, f.tid, u.login 
					from {0}tab_funcionario f, {0}tab_usuario u where f.tipo = 2 and f.id = :id and f.usuario = u.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						obj = new Administrador();
						obj.Id = id;
						obj.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						obj.Usuario.Login = reader["login"].ToString();
						obj.Nome = reader["nome"].ToString();
						obj.Cpf = reader["cpf"].ToString();
						obj.Email = (reader["email"] ?? String.Empty).ToString();
						obj.Tipo = Convert.ToInt32(reader["tipo"]);
						obj.Situacao = Convert.ToInt32(reader["situacao"]);
						obj.SituacaoMotivo = Convert.IsDBNull(reader["situacao_motivo"]) ? String.Empty : reader["situacao_motivo"].ToString();
						obj.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				if (obj == null)
				{
					return obj;
				}

				#region Cargo
				comando = bancoDeDados.CriarComando(@"select c.id, f.id idRelacao, c.nome, c.tid  from {0}tab_funcionario_cargo f, {0}tab_cargo c 
					where f.funcionario = :id and f.cargo = c.id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						obj.Cargos.Add(new Cargo()
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
				comando = bancoDeDados.CriarComando(@"select s.id, f.id idRelacao, s.nome, s.sigla, s.responsavel, s.tid from {0}tab_funcionario_setor f, {0}tab_setor s 
					where f.setor = s.id and f.funcionario = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						obj.Setores.Add(new Setor()
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
				comando = bancoDeDados.CriarComando(@"select p.id, t.id idRelacao, p.nome, p.funcionario_tipo, p.tid from {0}tab_funcionario_papel t, 
					{0}tab_autenticacao_papel p where t.papel = p.id and t.funcionario = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						obj.Papeis.Add(new Papel()
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
					from {0}tab_funcionario_permissao t, {0}lov_autenticacao_permissao p where t.permissao = p.id and t.funcionario = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						obj.Permissoes.Add(new Permissao()
						{
							Id = Convert.ToInt32(reader["id"]),
							IdRelacao = Convert.ToInt32(reader["idRelacao"]),
							Nome = reader["nome"].ToString(),
							Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString()),
							AdministradorTipo = Convert.ToInt32(reader["funcionario_tipo"]),
							Descricao = reader["descricao"].ToString()
						});
					}

					reader.Close();
				}
				#endregion
			}

			return obj;
		}

		internal Administrador Obter(string Cpf)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_funcionario where tipo = 2 and cpf = :cpf", EsquemaBanco);
				comando.AdicionarParametroEntrada("cpf", DbType.String, 14, Cpf);

				int id = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (id <= 0)
				{
					return null;
				}

				return Obter(id);
			}

		}

		internal Administrador ObterAdministradorAutenticacao(string login)
		{
			Administrador obj = new Administrador();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select f.id, f.usuario, f.nome, f.tipo, u.login, f.email, u.logon_anterior_ip, u.logon_anterior_data, 
					t.texto tipo_texto from {0}tab_funcionario f, {0}tab_usuario u, {0}lov_funcionario_tipo t where u.login = :login and f.usuario = u.id 
					and f.situacao = 1 and f.tipo = t.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						obj.Id = Convert.ToInt32(reader["id"]);
						obj.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						obj.Usuario.Login = reader["login"].ToString();
						obj.Usuario.IpUltimoLogon = reader["logon_anterior_ip"].ToString();
						obj.Usuario.DataUltimoLogon = (reader["logon_anterior_data"] != DBNull.Value) ? Convert.ToDateTime(reader["logon_anterior_data"]) : new DateTime?();
						obj.Nome = reader["nome"].ToString();
						obj.Email = reader["email"].ToString();
						obj.Tipo = Convert.ToInt32(reader["tipo"]);
						obj.TipoTexto = reader["tipo_texto"].ToString();
					}

					reader.Close();
				}

				#region Papel / Permissao
				comando = bancoDeDados.CriarComando(@"select p.codigo from {0}tab_funcionario_papel f, {0}tab_autenticacao_papel_perm app, {0}lov_autenticacao_permissao p 
					where f.funcionario = :id and f.papel = app.papel and app.permissao = p.id union select p.codigo from {0}tab_funcionario_permissao t, {0}lov_autenticacao_permissao p 
					where t.permissao = p.id and t.funcionario = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", obj.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						obj.Permissoes.Add(new Permissao()
						{
							Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString())
						});
					}

					reader.Close();
				}
				#endregion
			}

			return obj;
		}

		internal Administrador ObterAdministradorLogin(int usuarioId, int timeout)
		{
			Administrador obj = new Administrador();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_funcionario f set f.tentativa = (nvl(f.tentativa,0)+1) where f.usuario = :usuarioId", EsquemaBanco);
				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select f.id, f.tid, f.nome, f.tipo, f.situacao, f.tentativa, f.logado, f.session_id, 
					(case when (u.ultima_acao+NUMTODSINTERVAL(:timeout, 'MINUTE'))  > sysdate then 1 else 0 end) forcar_logout 
					from {0}tab_funcionario f, {0}tab_usuario u where f.usuario = u.id and u.id = :usuarioId", EsquemaBanco);
				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("timeout", timeout, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						obj.Id = Convert.ToInt32(reader["id"]);
						obj.Usuario.Id = usuarioId;
						obj.Situacao = Convert.ToInt32(reader["situacao"]);
						obj.Tentativa = Convert.ToInt32((reader["tentativa"] != DBNull.Value) ? reader["tentativa"] : 0);
						obj.SessionId = ((reader["session_id"] != DBNull.Value) ? reader["session_id"].ToString() : string.Empty);
						obj.Logado = ((reader["logado"] != DBNull.Value) ? Convert.ToBoolean(reader["logado"]) : false);
						obj.ForcarLogout = ((reader["forcar_logout"] != DBNull.Value) ? Convert.ToBoolean(reader["forcar_logout"]) : false);
						obj.Nome = reader["nome"].ToString();
						obj.Tipo = Convert.ToInt32(reader["tipo"]);
						obj.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}
			}

			return obj;
		}

		internal List<Situacao> ObterSituacaoAdministrador()
		{
			List<Situacao> lst = new List<Situacao>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.texto nome from {0}lov_funcionario_situacao s order by texto", EsquemaBanco);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lst.Add(new Situacao()
						{
							Id = Convert.ToInt32(reader["id"]),
							Nome = reader["nome"].ToString(),
							IsAtivo = true
						});
					}
					reader.Close();
				}
			}
			return lst;
		}

		public List<Papel> ObterPapeis()
		{
			List<Papel> lst = new List<Papel>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.nome from {0}tab_autenticacao_papel c where c.funcionario_tipo = 2", EsquemaBanco);
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
					{0}lov_autenticacao_permissao p, {0}tab_autenticacao_papel_perm pp where p.id = pp.permissao and pp.papel = :papel and p.funcionario_tipo = 2", EsquemaBanco);

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
								AdministradorTipo = Convert.ToInt32(reader["funcionario_tipo"]),
								Descricao = reader["descricao"].ToString()
							});
						}

						reader.Close();
					}
				}
			}

			return lst;
		}

		public Resultados<Administrador> Filtrar(Filtro<AdministradorListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Administrador> retorno = new Resultados<Administrador>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("f.cpf", "cpf", filtros.Dados.Cpf);

				comandtxt += comando.FiltroAndLike("f.nome", "nome", filtros.Dados.Nome, true);

				comandtxt += comando.FiltroAndLike("u.login", "login", filtros.Dados.Login, true);

				comandtxt += comando.FiltroAnd("f.situacao", "situacao", filtros.Dados.Situacao);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome", "cpf", "login", "situacao" };

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

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_funcionario f, tab_usuario u where f.tipo = 2 and f.usuario = u.id" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select f.id, f.tid, f.sistema, f.usuario, f.nome, u.login, f.cpf, f.situacao, lfs.texto situacao_texto, f.tipo, u.logon_data 
					from {2}tab_funcionario f, {2}tab_usuario u, {2}lov_funcionario_situacao lfs where f.tipo = 2 and f.situacao = lfs.id and f.usuario = u.id {0} {1}", comandtxt, DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Resultados

					Administrador item;

					while (reader.Read())
					{
						item = new Administrador();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						item.Usuario.Login = reader["login"].ToString();
						item.IsSistema = !Convert.IsDBNull(reader["sistema"]) && Convert.ToInt32(reader["sistema"]) > 0;

						if (reader["logon_data"] != null && !Convert.IsDBNull(reader["logon_data"]))
						{
							item.Usuario.DataUltimoLogon = Convert.ToDateTime(reader["logon_data"]);
						}

						item.Nome = reader["nome"].ToString();
						item.Cpf = reader["cpf"].ToString();
						item.Situacao = Convert.ToInt32(reader["situacao"]);
						item.SituacaoTexto = reader["situacao_texto"].ToString();
						item.Tipo = Convert.ToInt32(reader["tipo"]);

						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		#endregion
	}
}