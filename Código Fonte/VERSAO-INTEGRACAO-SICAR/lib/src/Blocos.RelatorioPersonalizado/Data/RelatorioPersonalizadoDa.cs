using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.RelatorioPersonalizado.Entities;

namespace Tecnomapas.Blocos.RelatorioPersonalizado.Data
{
	public class RelatorioPersonalizadoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public RelatorioPersonalizadoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações DML

		public void Salvar(Relatorio relatorio, BancoDeDados banco = null)
		{
			if (relatorio == null)
			{
				throw new Exception("Relatório personalizado é nulo.");
			}

			if (relatorio.Id <= 0)
			{
				Criar(relatorio, banco);
			}
			else
			{
				Editar(relatorio, banco);
			}
		}

		internal int? Criar(Relatorio relatorio, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Relatório

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_relatorio_personalizado r(id, nome, descricao, configuracao, data_criacao, proprietario, tipo_proprietario, fato, fato_tid, tid)
				values ({0}seq_relatorio_personalizado.nextval, :nome, :descricao, :configuracao, sysdate, :proprietario, :tipo_proprietario, :fato, (select f.tid from {0}tab_fato f where f.id = :fato),
				:tid) returning r.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", DbType.String, 200, relatorio.Nome);
				comando.AdicionarParametroEntrada("descricao", DbType.String, 4000, relatorio.Descricao);
				comando.AdicionarParametroEntrada("configuracao", relatorio.ConfiguracaoSerializada, DbType.StringFixedLength);
				comando.AdicionarParametroEntrada("proprietario", relatorio.UsuarioCriador.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_proprietario", relatorio.UsuarioCriador.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("fato", relatorio.FonteDados.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);
				
				bancoDeDados.ExecutarNonQuery(comando);

				relatorio.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Histórico

				Historico.Gerar(relatorio.Id, eHistoricoArtefato.relatorio, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return relatorio.Id;
			}
		}

		internal void Editar(Relatorio relatorio, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Relatorio

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_relatorio_personalizado r set r.nome =:nome, r.descricao =:descricao, 
				r.configuracao =:configuracao, r.fato =:fato, r.fato_tid = (select f.tid from {0}tab_fato f where f.id = :fato), r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", relatorio.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 200, relatorio.Nome);
				comando.AdicionarParametroEntrada("descricao", relatorio.Descricao, DbType.String);
				comando.AdicionarParametroEntrada("configuracao", relatorio.ConfiguracaoSerializada, DbType.StringFixedLength);
				comando.AdicionarParametroEntrada("fato", relatorio.FonteDados.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(relatorio.Id, eHistoricoArtefato.relatorio, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AtribuirExecutor(Relatorio relatorio, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Relatório Personalizado

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_relatorio_personalizado r set r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", relatorio.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#region Limpar os dados do banco

				//Usuarios
				comando = bancoDeDados.CriarComando("delete from {0}tab_relatorio_per_usuarios c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.relatorio = :relatorio{0}",
				comando.AdicionarNotIn("and", "c.usuario", DbType.Int32, relatorio.UsuariosPermitidos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("relatorio", relatorio.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Usuários

				if (relatorio.UsuariosPermitidos != null && relatorio.UsuariosPermitidos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_relatorio_per_usuarios (id, relatorio, usuario, tipo, tid) 
					(select {0}seq_relatorio_per_usuario.nextval, :relatorio, :usuario, :tipo, :tid from dual where not exists
					(select id from {0}tab_relatorio_per_usuarios r where r.usuario = :usuario and r.relatorio = :relatorio))", EsquemaBanco);

					comando.AdicionarParametroEntrada("relatorio", relatorio.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("usuario", DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (var item in relatorio.UsuariosPermitidos)
					{
						comando.SetarValorParametro("usuario", item.Id);
						comando.SetarValorParametro("tipo", item.Tipo);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"delete {0}tab_relatorio_per_usuarios p where p.relatorio = :relatorio", EsquemaBanco);
					comando.AdicionarParametroEntrada("relatorio", relatorio.Id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#endregion

				#region Histórico

				Historico.Gerar(relatorio.Id, eHistoricoArtefato.relatorio, eHistoricoAcao.atribuirusuario, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_relatorio_personalizado c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(id, eHistoricoArtefato.relatorio, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados do relatório

				List<String> listaDeletes = new List<String>();
				listaDeletes.Add("delete {0}tab_relatorio_per_usuarios p where p.relatorio = :relatorio;");
				listaDeletes.Add("delete {0}tab_relatorio_personalizado a where a.id = :relatorio;");
				
				comando = bancoDeDados.CriarComando("begin " + string.Join(" ", listaDeletes) + " end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("relatorio", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter/Filtrar

		internal Relatorio Obter(int id, bool simplificado = false)
		{
			Relatorio retorno = new Relatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.nome, t.descricao, t.configuracao, t.data_criacao, 
				t.proprietario, t.tipo_proprietario, t.fato, t.fato_tid, (case when (select count(id) from tab_fato f where f.id = t.fato and f.tid = t.fato_tid) > 0 then 1 else 0 end) atualizado,
				t.tid from {0}tab_relatorio_personalizado t where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.Id = reader.GetValue<int>("id");
						retorno.Atualizado = reader.GetValue<bool>("atualizado");
						retorno.Nome = reader.GetValue<string>("nome");
						retorno.Descricao = reader.GetValue<string>("descricao");
						retorno.ConfiguracaoSerializada = reader.GetValue<string>("configuracao");
						retorno.DataCriacao.Data = reader.GetValue<DateTime>("data_criacao");
						retorno.FonteDados.Id = reader.GetValue<int>("fato");
						retorno.FonteDados.Tid = reader.GetValue<string>("fato_tid");
						retorno.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				if (retorno.Id <= 0 || simplificado)
				{
					return retorno;
				}

				#region Usuários

				comando = bancoDeDados.CriarComando(@"select id, relatorio, usuario, tipo, tid from {0}tab_relatorio_per_usuarios u 
				where u.relatorio = :relatorio", EsquemaBanco);

				comando.AdicionarParametroEntrada("relatorio", retorno.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Usuario usuario;
					while (reader.Read())
					{
						usuario = new Usuario();
						usuario.Id = reader.GetValue<int>("usuario");
						usuario.Tipo = reader.GetValue<int>("tipo");
						usuario.Tid = reader.GetValue<string>("tid");
						retorno.UsuariosPermitidos.Add(usuario);
					}
					reader.Close();
				}
				
				#endregion
			}

			return retorno;
		}

		internal Resultados<Relatorio> Filtrar(Filtro<Relatorio> filtros)
		{
			Resultados<Relatorio> retorno = new Resultados<Relatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("t.fato", "fato", filtros.Dados.FonteDados.Id);

				comandtxt += comando.FiltroAndLike("t.nome", "nome", filtros.Dados.Nome, true, true);

				List<String> ordenar = new List<String>() { "nome" };
				List<String> colunas = new List<String>() { "nome" };

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_relatorio_personalizado t where t.id > 0" + comandtxt, esquema);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.DbCommand.CommandText = String.Format(@"select t.id Id, t.nome Nome, t.descricao Descricao from {0}tab_relatorio_personalizado t where t.id > 0 {1} {2}",
				esquema, comandtxt, DaHelper.Ordenar(colunas, ordenar));

				#endregion

				retorno.Itens = bancoDeDados.ObterEntityList<Relatorio>(comando);
			}

			return retorno;
		}

		#endregion
	}
}