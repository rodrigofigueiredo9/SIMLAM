using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCredenciado.Data
{
	public class CredenciadoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String UsuarioInterno
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public CredenciadoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public void Salvar(CredenciadoPessoa credenciado, BancoDeDados banco = null, Executor executor = null)
		{
			if (credenciado == null)
			{
				throw new Exception("credenciado nulo.");
			}

			if (credenciado.Id <= 0)
			{
				Criar(credenciado, banco, executor);
			}
			else
			{
				Editar(credenciado, banco, executor);
			}
		}

		public void Criar(CredenciadoPessoa credenciado, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_credenciado (id, tipo, situacao, data_cadastro, chave, usuario, orgao_parc, orgao_parc_unid, tid)
				values  (seq_credenciado.nextval, :tipo, :situacao, sysdate, :chave, :usuario, :orgao_parc, :orgao_parc_unid, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tipo", credenciado.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", credenciado.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("chave", DbType.String, 150, credenciado.Chave);
				comando.AdicionarParametroEntrada("usuario", credenciado.IsUsuario?1:0, DbType.Int32);
				comando.AdicionarParametroEntrada("orgao_parc", (credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado && credenciado.OrgaoParceiroId > 0) ? credenciado.OrgaoParceiroId : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("orgao_parc_unid", (credenciado.Tipo == (int)eCredenciadoTipo.OrgaoParceiroConveniado && credenciado.OrgaoParceiroUnidadeId > 0) ? credenciado.OrgaoParceiroUnidadeId : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				credenciado.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				credenciado.Tid = GerenciadorTransacao.ObterIDAtual();

				if (executor != null)
				{
					executor.Id = credenciado.Id;
					executor.Tid = credenciado.Tid;
				}

				bancoDeDados.Commit();
			}
		}

		public void Editar(CredenciadoPessoa credenciado, BancoDeDados banco = null, Executor executor = null, bool gerarHistorico = true)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_credenciado t set t.pessoa = :pessoa, t.situacao = :situacao, t.chave = :chave, 
															t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("pessoa", credenciado.Pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", credenciado.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("chave", DbType.String, 150, credenciado.Chave);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				credenciado.Tid = GerenciadorTransacao.ObterIDAtual();

				if (executor != null)
				{
					executor.Id = credenciado.Id;
					executor.Tid = credenciado.Tid;
				}

				if (gerarHistorico)
				{
					Historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.atualizar, bancoDeDados, executor);
				}

				bancoDeDados.Commit();
			}
		}

		public void RegerarChave(CredenciadoPessoa credenciado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_credenciado t set t.situacao = :situacao, t.chave = :chave, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", credenciado.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("chave", DbType.String, 150, credenciado.Chave);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				credenciado.Tid = GerenciadorTransacao.ObterIDAtual();

				Historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.regerarchave, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		public void Ativar(CredenciadoPessoa credenciado, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_credenciado a set a.pessoa = :pessoa, a.usuario = :usuario, a.situacao = :situacao, 
				a.tentativa=0, a.tid = :tid where a.chave = :chave", banco.Conexao);

				comando.AdicionarParametroEntrada("chave", DbType.String, 150, credenciado.Chave);
				comando.AdicionarParametroEntrada("pessoa", credenciado.Pessoa.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("usuario", credenciado.Usuario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eCredenciadoSituacao.Ativo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#region Papel

				// Apaga os papeis antigos
				comando = bancoDeDados.CriarComando(@"delete tab_credenciado_papel c where c.credenciado = :id");
				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Insere os novos
				comando = bancoDeDados.CriarComando(@"insert into tab_credenciado_papel (id, credenciado, papel, tid)
				values (seq_credenciado_papel.nextval, :credenciado, (select min(p.id) from tab_autenticacao_papel p), :tid)");

				comando.AdicionarParametroEntrada("credenciado", credenciado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.ativar, bancoDeDados, executor);

				bancoDeDados.Commit();
			}

		}

		public void AlterarSituacao(CredenciadoPessoa credenciado, BancoDeDados banco = null)
		{
			if (banco == null)
			{
				GerenciadorTransacao.ObterIDAtual();
			}
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_credenciado a set a.situacao = :situacao, a.tid = :tid where a.id = :id");

				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", credenciado.Situacao, DbType.Int32);// Aguardando Ativação = 1, Ativo = 2, Bloqueado = 3, Senha Vencida = 4
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#region Papel

				// Apaga os papeis antigos
				comando = bancoDeDados.CriarComando(@"delete tab_credenciado_papel c where c.credenciado = :id");
				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				if (credenciado.Situacao == 2)//Ativo = 2
				{
					//Insere os novos
					comando = bancoDeDados.CriarComando(@"insert into tab_credenciado_papel (id, credenciado, papel, tid)
					(select seq_credenciado_papel.nextval, :credenciado, p.id, :tid from tab_autenticacao_papel p where p.credenciado_tipo = :tipo)");

					comando.AdicionarParametroEntrada("credenciado", credenciado.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("tipo", credenciado.Tipo, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}

		}

		public void EnviarEmail(CredenciadoPessoa credenciado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_credenciado a set a.data_cadastro = sysdate, a.chave = :chave, a.tid = :tid where a.id = :id");

				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("chave", DbType.String, 150, credenciado.Chave);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Autenticar(CredenciadoPessoa func, string sessionIdForcarLogoff, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_credenciado f set f.tentativa = 0, logado = 1, f.session_Id = :sessionId where f.id = :id");

				comando.AdicionarParametroEntrada("id", func.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("sessionId", func.SessionId, DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);

				if (!String.IsNullOrEmpty(sessionIdForcarLogoff))
				{
					comando = bancoDeDados.CriarComando(@"insert into tab_Credenciado_deslogar (id, login, session_Id, data_acao) 
					values (seq_Credenciado_deslogar.nextval, :login, :sessionForcaLogoff, sysdate)");

					comando.AdicionarParametroEntrada("login", func.Usuario.Login, DbType.String);
					comando.AdicionarParametroEntrada("sessionForcaLogoff", sessionIdForcarLogoff, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSenha(int usuarioId, CredenciadoPessoa executor = null, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_credenciado c set c.situacao = 2, c.tid = :tid where c.usuario = :usuario");

				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select c.id from tab_credenciado c where c.usuario = :usuario");
				comando.AdicionarParametroEntrada("usuario", usuarioId, DbType.Int32);
				int id = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				Historico.Gerar(id, eHistoricoArtefato.credenciado, eHistoricoAcao.alterarsenha, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Deslogar(string login, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_credenciado f set f.logado = 0 where f.usuario = (select u.id from tab_usuario u where u.login = :login)");
				comando.AdicionarParametroEntrada("login", login, DbType.String);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(CredenciadoPessoa credenciado, string motivo = "", BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_credenciado t set t.situacao = :situacao, t.situacao_motivo = :situacao_motivo, t.tid = :tid 
				where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", credenciado.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_motivo", DbType.String, 80, motivo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				credenciado.Tid = GerenciadorTransacao.ObterIDAtual();

				if (executor != null)
				{
					executor.Id = credenciado.Id;
					executor.Tid = credenciado.Tid;
				}

				Historico.Gerar(credenciado.Id, eHistoricoArtefato.credenciado, eHistoricoAcao.alterarsituacao, bancoDeDados, executor);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Buscas/Filtrar

		internal List<int> ObterIdsCredenciadosParceiros(int idOrgaoParceiro, int idUnidade, BancoDeDados banco = null)
		{
			List<int> retorno = new List<int>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Orgao Parceiro/ Conveniado

				Comando comando = bancoDeDados.CriarComando("");

				comando.AdicionarParametroEntrada("orgao_id", idOrgaoParceiro, DbType.Int32);

				string cmdTxt = comando.FiltroAnd("t.orgao_parc_unid", "unidade_id", idUnidade);

				comando.DbCommand.CommandText = String.Format("select id from {0}.tab_credenciado t where t.orgao_parc = :orgao_id" + cmdTxt, EsquemaBanco);
				retorno = bancoDeDados.ExecutarList<int>(comando);

				#endregion
			}

			return retorno;
		}

		public Resultados<Cred.ListarFiltro> Filtrar(Filtro<Cred.ListarFiltro> filtros)
		{
			Resultados<Cred.ListarFiltro> retorno = new Resultados<Cred.ListarFiltro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				string credenciado = String.IsNullOrEmpty(UsuarioCredenciado) ? "" : UsuarioCredenciado + ".";
				string interno = String.IsNullOrEmpty(UsuarioInterno) ? "" : UsuarioInterno + ".";
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("nvl(tp.nome, tp.razao_social)", "nome", filtros.Dados.NomeRazaoSocial, true, true);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.CpfCnpj))
				{
					if (ValidacoesGenericasBus.Cpf(filtros.Dados.CpfCnpj) || ValidacoesGenericasBus.Cnpj(filtros.Dados.CpfCnpj))
					{
						comandtxt += " and ((tp.cpf = :cpfcnpj) or (tp.cnpj = :cpfcnpj))";
					}
					else
					{
						comandtxt += "and ((tp.cpf like '%'|| :cpfcnpj ||'%') or (tp.cnpj = '%'|| :cpfcnpj ||'%'))";
					}

					comando.AdicionarParametroEntrada("cpfcnpj", filtros.Dados.CpfCnpj, DbType.String);
				}

				comandtxt += comando.FiltroAnd("tc.situacao", "situacao", filtros.Dados.Situacao);

				comandtxt += comando.FiltroAnd("tc.tipo", "tipo", filtros.Dados.Tipo);

				comandtxt += comando.FiltroAnd("to_char(tc.data_cadastro, 'dd/mm/yyyy')", "data_ativacao", filtros.Dados.DataAtivacao);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome", "cpfcnpj", "tipo", "ativacao", "situacao" };

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

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {1}tab_credenciado tc, {1}tab_pessoa tp,
					{1}lov_credenciado_tipo lct, {1}lov_credenciado_situacao lcs where tc.pessoa = tp.id 
					and tc.tipo = lct.id and tc.situacao = lcs.id " + comandtxt, interno, credenciado);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Funcionario.NaoEncontrouRegistros);
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select tc.id, tc.tid, nvl(tp.nome, tp.razao_social) nome, nvl(tp.cpf, tp.cnpj) cpfcnpj, lct.texto tipo, 
				to_char(tc.data_cadastro, 'dd/mm/yyyy') ativacao, lcs.id situacao, lcs.texto situacao_texto from {2}tab_credenciado tc, {2}tab_pessoa tp,
				{2}lov_credenciado_tipo lct, {2}lov_credenciado_situacao lcs where tc.pessoa = tp.id and tc.tipo = lct.id and tc.situacao = lcs.id {0} {1}", 
				comandtxt, DaHelper.Ordenar(colunas, ordenar), credenciado, interno); //1 - Aguardando Ativação

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Cred.ListarFiltro item;
					while (reader.Read())
					{
						item = new Cred.ListarFiltro();
						item.Id = reader.GetValue<string>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.NomeRazaoSocial = reader.GetValue<string>("nome");
						item.CpfCnpj = reader.GetValue<string>("cpfcnpj");
						item.DataAtivacao = reader.GetValue<string>("ativacao");
						item.Situacao = reader.GetValue<int>("situacao");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.TipoTexto = reader.GetValue<string>("tipo");

						retorno.Itens.Add(item);
					}

					reader.Close();
					#endregion
				}
			}

			return retorno;
		}


		internal void ObterCredenciadoInterno(Pessoa pessoa)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.interno, p.credenciado from {0}tab_pessoa p where p.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", DbType.Int32, pessoa.Id);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["interno"] != null && !Convert.IsDBNull(reader["interno"]))
						{
							pessoa.InternoId = Convert.ToInt32(reader["interno"]);
						}

						if (reader["credenciado"] != null && !Convert.IsDBNull(reader["credenciado"]))
						{
							pessoa.CredenciadoId = Convert.ToInt32(reader["credenciado"]);
						}
					}

					reader.Close();
				}
			}
		}

		internal CredenciadoPessoa ObterCredenciado(String chave)
		{
			int id = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}tab_credenciado c where c.chave = :chave", EsquemaBanco);
				comando.AdicionarParametroEntrada("chave", DbType.String, 150, chave);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						id = Convert.ToInt32(reader["id"]);
					}

					reader.Close();
				}

				return Obter(id, banco: bancoDeDados);
			}
		}

		internal CredenciadoPessoa Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			CredenciadoPessoa credenciado = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Credenciado

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.chave, c.usuario, p.id pessoa, p.interno, p.tipo pessoa_tipo, nvl(p.nome, p.razao_social) nome_razao, 
				nvl(p.cpf, p.cnpj) cpf_cnpj, p.interno, c.situacao, c.tipo, lt.texto tipo_texto, (ts.sigla || ' - ' || ts.nome_local) unidade_sigla_nome ,c.orgao_parc, 
				c.orgao_parc_unid, c.tid, u.login, (case when trunc(sysdate) > trunc(u.senha_data+(prazo.dias)) then 1 else 0 end) senha_vencida, (select valor from tab_pessoa_meio_contato 
				where meio_contato = 5 and pessoa = p.id) email from {0}tab_credenciado c, {0}tab_orgao_parc_conv_sigla_unid ts, {0}tab_usuario u, {0}tab_pessoa p, 
				{0}lov_credenciado_tipo lt, (select to_number(c.valor) dias from {0}cnf_sistema c where c.campo = 'validadesenha') prazo where c.usuario = u.id(+) and 
				c.pessoa = p.id and c.tipo = lt.id and c.orgao_parc_unid = ts.id(+) and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						credenciado = new CredenciadoPessoa();
						credenciado.Id = id;
						credenciado.Tid = reader.GetValue<string>("tid");
						credenciado.Tipo = reader.GetValue<int>("tipo");
						credenciado.TipoTexto = reader.GetValue<string>("tipo_texto");
						credenciado.Situacao = reader.GetValue<int>("situacao");
						credenciado.OrgaoParceiroId = reader.GetValue<int>("orgao_parc");
						credenciado.OrgaoParceiroUnidadeId = reader.GetValue<int>("orgao_parc_unid");
						credenciado.Chave = reader.GetValue<string>("chave");
						credenciado.OrgaoParceiroUnidadeSiglaNome = reader.GetValue<string>("unidade_sigla_nome");
						
						if (reader["pessoa"] != null && !Convert.IsDBNull(reader["pessoa"]))
						{
							credenciado.Pessoa.Id = Convert.ToInt32(reader["pessoa"]);
							credenciado.Pessoa.Tipo = Convert.ToInt32(reader["pessoa_tipo"]);
							credenciado.Pessoa.MeiosContatos.Add(new Contato { Valor = reader.GetValue<string>("email"), TipoContato = eTipoContato.Email });
							if (credenciado.Pessoa.IsFisica)
							{
								credenciado.Pessoa.Fisica.Nome = reader["nome_razao"].ToString();
								credenciado.Pessoa.Fisica.CPF = reader["cpf_cnpj"].ToString();
							}
							else
							{
								credenciado.Pessoa.Juridica.RazaoSocial = reader["nome_razao"].ToString();
								credenciado.Pessoa.Juridica.CNPJ = reader["cpf_cnpj"].ToString();
							}
						}

						if (reader["interno"] != null && !Convert.IsDBNull(reader["interno"]))
						{
							credenciado.Pessoa.InternoId = Convert.ToInt32(reader["interno"]);
						}

						if (reader["usuario"] != null && !Convert.IsDBNull(reader["usuario"]))
						{
							credenciado.Usuario.Id = Convert.ToInt32(reader["usuario"]);
							credenciado.Usuario.Login = reader["login"].ToString();
							credenciado.AlterarSenha = (reader["senha_vencida"].ToString() == "1");
						}
					}

					reader.Close();
				}

				#endregion

				if (credenciado == null || simplificado)
				{
					return credenciado;
				}

				#region Papel

				comando = bancoDeDados.CriarComando(@"select p.id, t.id idrelacao, p.nome, p.credenciado_tipo, p.tid from tab_credenciado_papel t, 
				tab_autenticacao_papel p where t.papel = p.id and t.credenciado = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						credenciado.Papeis.Add(new Papel()
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

				comando = bancoDeDados.CriarComando(@"select p.id, t.id idrelacao, p.nome, p.codigo, p.credenciado_tipo, p.descricao 
				from tab_credenciado_permissao t, lov_autenticacao_permissao p where t.permissao = p.id and t.credenciado = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						credenciado.Permissoes.Add(new Permissao()
						{
							Id = Convert.ToInt32(reader["id"]),
							IdRelacao = Convert.ToInt32(reader["idRelacao"]),
							Nome = reader["nome"].ToString(),
							Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString()),
							CredenciadoTipo = Convert.ToInt32(reader["Credenciado_tipo"]),
							Descricao = reader["descricao"].ToString()
						});
					}

					reader.Close();
				}

				#endregion
			}

			return credenciado;
		}

		internal CredenciadoPessoa Obter(string cpfCnpj, bool simplificado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
                Comando comando = bancoDeDados.CriarComando(@"select c.id
                                                              from {0}tab_pessoa p,
                                                                   {0}tab_credenciado c
                                                              where p.id = c.pessoa
                                                                    and nvl(p.cpf,p.cnpj) = :cpf_cnpj
                                                                    and (c.situacao >= 2 and c.situacao <= 5)", EsquemaBanco);
				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 25, cpfCnpj);

				int id = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (id <= 0)
				{
					return new CredenciadoPessoa();
				}

				return Obter(id, simplificado, bancoDeDados);
			}
		}

		internal CredenciadoPessoa ObterCredenciadoAutenticacao(string login)
		{
			CredenciadoPessoa credenciado = new CredenciadoPessoa();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				comando = bancoDeDados.CriarComando(@"select c.id, c.usuario, nvl(p.nome,p.razao_social) nome, c.tipo, u.login, u.logon_anterior_ip, u.logon_anterior_data, 
				lc.texto tipo_texto, c.tid, (select cm.valor from {0}tab_pessoa_meio_contato cm where cm.pessoa = p.id and cm.meio_contato = 5 and rownum =1 ) email 
				from {0}tab_credenciado c, {0}tab_pessoa p, {0}tab_usuario u, {0}lov_credenciado_tipo lc where u.login = :login and c.usuario = u.id 
				and c.situacao = :situacao and c.tipo = lc.id and c.pessoa = p.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				comando.AdicionarParametroEntrada("situacao", 2, DbType.String);//2 - Ativo

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						credenciado.Id = Convert.ToInt32(reader["id"]);
						credenciado.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						credenciado.Usuario.Login = reader["login"].ToString();
						credenciado.Usuario.IpUltimoLogon = reader["logon_anterior_ip"].ToString();
						credenciado.Usuario.DataUltimoLogon = (reader["logon_anterior_data"] != DBNull.Value) ? Convert.ToDateTime(reader["logon_anterior_data"]) : new DateTime?();
						credenciado.Nome = reader["nome"].ToString();
						credenciado.Tipo = Convert.ToInt32(reader["tipo"]);
						credenciado.TipoTexto = reader["tipo_texto"].ToString();
						credenciado.Tid = reader["tid"].ToString();

						credenciado.Pessoa.MeiosContatos.Add(new Contato() { Valor = reader["email"].ToString(), TipoContato = eTipoContato.Email });
					}

					reader.Close();
				}

				#region Papel / Permissao

				if (credenciado != null)
				{

					comando = bancoDeDados.CriarComando(@"select p.codigo from tab_credenciado_papel f, tab_autenticacao_papel_perm app, lov_autenticacao_permissao p 
					where f.credenciado = :id and f.papel = app.papel and app.permissao = p.id union select p.codigo from tab_credenciado_permissao t, lov_autenticacao_permissao p
					where t.permissao = p.id and t.credenciado = :id");

					comando.AdicionarParametroEntrada("id", credenciado.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							credenciado.Permissoes.Add(new Permissao()
							{
								Codigo = (ePermissao)Enum.Parse(typeof(ePermissao), reader["codigo"].ToString())
							});
						}

						reader.Close();
					}
				}


				#endregion
			}

			return credenciado;
		}

		internal CredenciadoPessoa ObterCredenciadoExecutor(string login)
		{
			CredenciadoPessoa credenciado = new CredenciadoPessoa();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.usuario, p.nome, c.tipo, u.login, u.logon_anterior_ip, u.logon_anterior_data, 
				lc.texto tipo_texto from {0}tab_credenciado c, {0}tab_pessoa p, {0}tab_usuario u, {0}lov_credenciado_tipo lc 
				where u.login = :login and c.usuario = u.id and c.situacao = 1 and c.tipo = c.id and c.pessoa = p.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					if (reader.Read())
					{
						credenciado.Id = Convert.ToInt32(reader["id"]);
						credenciado.Usuario.Id = Convert.ToInt32(reader["usuario"]);
						credenciado.Usuario.Login = reader["login"].ToString();
						credenciado.Usuario.IpUltimoLogon = reader["logon_anterior_ip"].ToString();
						credenciado.Usuario.DataUltimoLogon = (reader["logon_anterior_data"] != DBNull.Value) ? Convert.ToDateTime(reader["logon_anterior_data"]) : new DateTime?();
						credenciado.Nome = reader["nome"].ToString();
						credenciado.Tipo = Convert.ToInt32(reader["tipo"]);
						credenciado.TipoTexto = reader["tipo_texto"].ToString();
						credenciado.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}
			}

			return credenciado;
		}

		internal CredenciadoPessoa ObterCredenciadoLogin(int usuarioId, int timeout)
		{
			CredenciadoPessoa credenciado = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_credenciado f set f.tentativa = (nvl(f.tentativa,0)+1) where f.usuario = :usuarioId", EsquemaBanco);
				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select c.id, nvl(p.nome, p.razao_social) nome, c.tipo, c.situacao, c.tentativa, c.logado, c.session_id, 
				(case when (u.ultima_acao+numtodsinterval(:timeout, 'MINUTE'))  > sysdate then 1 else 0 end) forcar_logout 
				from {0}tab_credenciado c, {0}tab_usuario u, {0}tab_pessoa p where c.usuario = u.id and c.pessoa = p.id and u.id = :usuarioId", EsquemaBanco);

				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("timeout", timeout, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						credenciado = new CredenciadoPessoa();
						credenciado.Id = Convert.ToInt32(reader["id"]);
						credenciado.Usuario.Id = usuarioId;
						credenciado.Situacao = Convert.ToInt32(reader["situacao"]);
						credenciado.Tentativa = Convert.ToInt32((reader["tentativa"] != DBNull.Value) ? reader["tentativa"] : 0);
						credenciado.SessionId = ((reader["session_id"] != DBNull.Value) ? reader["session_id"].ToString() : string.Empty);
						credenciado.Logado = ((reader["logado"] != DBNull.Value) ? Convert.ToBoolean(reader["logado"]) : false);
						credenciado.ForcarLogout = ((reader["forcar_logout"] != DBNull.Value) ? Convert.ToBoolean(reader["forcar_logout"]) : false);
						credenciado.Nome = reader["nome"].ToString();
						credenciado.Tipo = Convert.ToInt32(reader["tipo"]);
					}

					reader.Close();
				}

				bancoDeDados.Commit();
			}

			return credenciado;
		}

		internal int ObterSituacao(string login)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.situacao from {0}tab_credenciado f, {0}tab_usuario u where f.usuario = u.id and u.login = :login", EsquemaBanco);
				comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public List<Papel> ObterPapeis(BancoDeDados banco = null)
		{
			List<Papel> lst = new List<Papel>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.nome from {0}tab_autenticacao_papel c where c.credenciado_tipo = 1", bancoDeDados.Conexao);
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

				comando = bancoDeDados.CriarComando(@"select p.id, p.nome, p.codigo, p.credenciado_tipo, p.descricao 
				from {0}lov_autenticacao_permissao p, {0}tab_autenticacao_papel_perm pp where p.id = pp.permissao and pp.papel = :papel and p.credenciado_tipo = 1", bancoDeDados.Conexao);

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
								CredenciadoTipo = Convert.ToInt32(reader["Credenciado_tipo"]),
								Descricao = reader["descricao"].ToString()
							});
						}

						reader.Close();
					}
				}
			}

			return lst;
		}

		internal List<Lista> ObterUnidadesLst(int orgao, BancoDeDados banco = null)
		{
			List<Lista> unidades = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Unidades

				Comando comando = bancoDeDados.CriarComando(@"select u.id, u.sigla, u.nome_local from tab_orgao_parc_conv_sigla_unid u where u.orgao_parc_conv = :orgao", EsquemaBanco);

				comando.AdicionarParametroEntrada("orgao", orgao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista unidade = null;

					while (reader.Read())
					{
						unidade = new Lista();
						unidade.Id = reader.GetValue<String>("id");
						unidade.Texto = reader.GetValue<String>("sigla") + " - " + reader.GetValue<String>("nome_local");
						unidade.IsAtivo = true;

						unidades.Add(unidade);
					}

					reader.Close();
				}

				#endregion
			}

			return unidades;
		}

		internal List<Lista> ObterOrgaosParceirosLst(BancoDeDados banco = null)
		{
			List<Lista> orgaosParceiros = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Orgaos Parceiros/ Conveniados

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.orgao_sigla, t.orgao_nome from tab_orgao_parc_conv t", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista orgao = null;

					while (reader.Read())
					{
						orgao = new Lista();
						orgao.Id = reader.GetValue<String>("id");
						orgao.Texto = reader.GetValue<String>("orgao_sigla") + " - " + reader.GetValue<String>("orgao_nome");
						orgao.IsAtivo = true;

						orgaosParceiros.Add(orgao);

					}

					reader.Close();

				}

				#endregion
			}

			return orgaosParceiros;
		}

		#endregion

		#region Validações

		internal bool VerificarSenhaVencida(int usuarioId)
		{
			bool vencido = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{

				bancoDeDados.IniciarTransacao();
				GerenciadorTransacao.ObterIDAtual();

				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_usuario u, tab_credenciado f, 
				(select to_number(c.valor) dias from cnf_sistema c where c.campo = 'validadesenha') prazo
				where u.id = f.usuario and f.situacao <> 4 and u.id = :usuarioId and trunc(sysdate) > trunc(u.senha_data+(prazo.dias))");

				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);

				vencido = bancoDeDados.ExecutarScalar(comando).ToString() != "0";

				if (vencido)
				{
					comando.DbCommand.CommandText = @"update tab_credenciado f set f.situacao = 4 where f.usuario = :usuarioId"; /*Senha Vencida*/
					bancoDeDados.ExecutarNonQuery(comando);

					comando.DbCommand.CommandText = @"select f.id from tab_credenciado f where f.usuario = :usuarioId";

					int idFunc = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

					Historico.Gerar(idFunc, eHistoricoArtefato.credenciado, eHistoricoAcao.atualizar, bancoDeDados);
				}

				bancoDeDados.Commit();
			}

			return vencido;
		}

		internal bool VerificarSeDeveDeslogar(String login, String sessionId, int timeout)
		{
			Comando comando = null;
			bool forcarLogoff = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				if (!String.IsNullOrEmpty(sessionId))
				{
					comando = bancoDeDados.CriarComando(@"select t.id from tab_credenciado_deslogar t where t.login = :login and t.session_Id = :session_Id");

					comando.AdicionarParametroEntrada("login", DbType.String, 30, login);
					comando.AdicionarParametroEntrada("session_Id", DbType.String, 36, sessionId);

					int idLogoff = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
					forcarLogoff = (idLogoff != 0);

					if (forcarLogoff)
					{
						comando = bancoDeDados.CriarComando(@"delete from tab_credenciado_deslogar t where t.id = :id or (t.data_acao+NUMTODSINTERVAL(:timeout, 'MINUTE')) < sysdate");
						comando.AdicionarParametroEntrada("id", idLogoff, DbType.Int32);
						comando.AdicionarParametroEntrada("timeout", timeout, DbType.Int32);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				bancoDeDados.Commit();
			}

			return forcarLogoff;
		}

		internal bool VerificarExisteChave(string chave)
		{
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				if (!String.IsNullOrEmpty(chave))
				{
					comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado c where c.chave = :chave");

					comando.AdicionarParametroEntrada("chave", DbType.String, 150, chave);

					return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
				}
			}

			return false;
		}

		internal bool VerificarChaveAtiva(string chave)
		{
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				if (!String.IsNullOrEmpty(chave))
				{
					comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado c where c.situacao = 2 and c.chave = :chave");//2 - Ativo

					comando.AdicionarParametroEntrada("chave", DbType.String, 150, chave);

					return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
				}
			}

			return false;
		}

		public bool ExisteInterno(String cpfCnpj)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) qtd from tab_pessoa p where nvl(p.cpf,p.cnpj) = :cpf_cnpj");

				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 25, cpfCnpj);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ExisteCredenciado(String cpfCnpj)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado c, (select p.id from tab_pessoa p where nvl(p.cpf,p.cnpj) = :cpf_cnpj) p 
				where c.pessoa = p.id");

				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 25, cpfCnpj);

				return bancoDeDados.ExecutarScalar(comando).ToString() != "0";
			}
		}

		public bool IsCredenciadoAtivo(String cpfCnpj)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado c, tab_pessoa p where  nvl(p.cpf,p.cnpj) = :cpf_cnpj
					  and c.pessoa = p.id  and c.situacao in (2, 4, 5)");// 2 = Ativo, 4 - Senha vencida, 5 - Aguardando chave

				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 25, cpfCnpj);

				return bancoDeDados.ExecutarScalar(comando).ToString() != "0";
			}
		}

		public bool IsCredenciadoAtivoAlgumaVez(int credenciadoID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from hst_credenciado c where c.credenciado_id = :credenciado_id and c.situacao_id = 2");// 2 = Ativo

				comando.AdicionarParametroEntrada("credenciado_id", credenciadoID, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool IsBloqueado(string cpfCnpj)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado c, tab_pessoa p where  nvl(p.cpf,p.cnpj) = :cpf_cnpj
					  and c.pessoa = p.id  and c.situacao = 3");// 3 = Bloqueado

				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 25, cpfCnpj);

				return bancoDeDados.ExecutarScalar(comando).ToString() != "0";
			}
		}

		internal bool ValidarConjugeAssociado(string pessoaCPF, string conjugeCPF, int credenciado)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select sum(qtd) qtd from (
				select count(*) qtd from tab_pessoa_conjuge c, tab_pessoa p, tab_pessoa pc
				where c.pessoa = p.id and c.conjuge = pc.id and p.credenciado = :credenciado and pc.credenciado = :credenciado and p.cpf <> :pessoa_cpf and pc.cpf = :conjuge_cpf 
				union all
				select count(*) qtd from tab_pessoa_conjuge c, tab_pessoa p, tab_pessoa pc
				where c.pessoa = p.id and c.conjuge = pc.id and p.credenciado = :credenciado and pc.credenciado = :credenciado and pc.cpf <> :pessoa_cpf and p.cpf = :conjuge_cpf)", EsquemaBanco);

				comando.AdicionarParametroEntrada("credenciado", credenciado, DbType.Int32);
				comando.AdicionarParametroEntrada("pessoa_cpf", DbType.String, 20, pessoaCPF);
				comando.AdicionarParametroEntrada("conjuge_cpf", DbType.String, 20, conjugeCPF);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool IsCredenciadoOrgaoParceiroPublico(string cpfCnpj)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado c, tab_pessoa p 
				where c.pessoa = p.id and c.tipo = 3 and c.situacao in (1, 6) and nvl(p.cpf,p.cnpj) = :cpf_cnpj");

				comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String, 25, cpfCnpj);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public List<Permissao> ObterPermissoesExtras(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(cfo.id) qtd from tab_hab_emi_cfo_cfoc cfo, tab_credenciado c
				where c.id = cfo.responsavel and c.id=:id");
				
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				var permissao = new Permissao();

				var permissoes = new List<Permissao>();
				if (Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0)
				{
					permissao = new Permissao();
					permissao.Codigo = ePermissao.ConsultarHabilitacaoPraga;
					permissoes.Add(permissao);
				}

				permissao = new Permissao();
				permissao.Codigo = ePermissao.ConsultarAgrotoxicos;
				permissoes.Add(permissao);

				return permissoes;
			}
		}

		#endregion
	}
}