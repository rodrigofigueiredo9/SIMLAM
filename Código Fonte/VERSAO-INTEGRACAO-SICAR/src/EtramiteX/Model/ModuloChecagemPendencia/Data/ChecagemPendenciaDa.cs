using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemPendencia;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloChecagemPendencia.Data
{	
	class ChecagemPendenciaDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public ChecagemPendenciaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public void Salvar(ChecagemPendencia pendencia, BancoDeDados banco = null)
		{
			if (pendencia == null)
			{
				throw new Exception("Checagem de itens de pendência é nula.");
			}

			if (pendencia.Id == 0)
			{
				Criar(pendencia, banco);
			}
			else
			{
				Editar(pendencia, banco);
			}
		}

		internal void Criar(ChecagemPendencia pendencia, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Checagem de Pendências

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem_pend a (id, numero, titulo, situacao, tid) values
				({0}seq_checagem_pend.nextval, seq_checagem_pend.currval, :titulo, :situacao, :tid) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", Convert.ToInt32(pendencia.TituloId), DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", pendencia.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				pendencia.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Itens

				if (pendencia.Itens != null && pendencia.Itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem_pend_itens(id, checagem, item_id, item_tid, nome, situacao, tid) values 
					({0}seq_checagem_pend_itens.nextval, :checagem, :item_id, :item_tid, (select tri.nome from {0}tab_roteiro_item tri where tri.id = :item_id), :situacao, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("checagem", pendencia.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("item_id", DbType.Int32);
					comando.AdicionarParametroEntrada("item_tid", DbType.String, 36);
					comando.AdicionarParametroEntrada("situacao", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (ChecagemPendenciaItem item in pendencia.Itens)
					{
						comando.SetarValorParametro("item_id", item.Id);
						comando.SetarValorParametro("item_tid", item.Tid);
						comando.SetarValorParametro("situacao", item.SituacaoId);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(pendencia.Id, eHistoricoArtefato.checagempendencia, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Editar(ChecagemPendencia pendencia, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Análise de itens de processo/documento

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_checagem_pend a set a.titulo = :titulo, a.situacao = :situacao, 
				a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", pendencia.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", Convert.ToInt32(pendencia.TituloId), DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", pendencia.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Itens da pendencia
				comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format("delete from {1}tab_checagem_pend_itens c where c.checagem = :checagem{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, pendencia.Itens.Select(x => x.IdRelacionamento).ToList()), EsquemaBanco);
				comando.AdicionarParametroEntrada("checagem", pendencia.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Itens

				if (pendencia.Itens != null && pendencia.Itens.Count > 0)
				{
					foreach (ChecagemPendenciaItem item in pendencia.Itens)
					{
						if (item.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_checagem_pend_itens c set c.checagem = :checagem, 
							c.item_id = :item_id, c.item_tid = :item_tid, c.situacao = :situacao, c.tid = :tid where c.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_checagem_pend_itens(id, checagem, item_id, item_tid, nome, situacao, tid) 
							values ({0}seq_checagem_pend_itens.nextval, :checagem, :item_id, :item_tid, (select ri.nome from {0}tab_roteiro_item ri where ri.id = :item_id), :situacao, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("checagem", pendencia.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("item_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("item_tid", DbType.String, 36, item.Tid);
						comando.AdicionarParametroEntrada("situacao", item.SituacaoId, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(pendencia.Id, eHistoricoArtefato.checagempendencia, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação
				
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_checagem_pend c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.checagempendencia, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da pendencia

				comando = bancoDeDados.CriarComando(@"begin " +
					"delete {0}tab_checagem_pend_itens a where a.checagem = :checagem;" +
					"delete {0}tab_checagem_pend a where a.id = :checagem;" +
					"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("checagem", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void AlterarSituacao(ChecagemPendencia checagem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Checagem

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_checagem_pend a set a.situacao = :situacao, a.tid = :tid where a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", checagem.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", checagem.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(checagem.Id, eHistoricoArtefato.checagempendencia, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal ChecagemPendencia Obter(int id, bool simplificado = false)
		{
			ChecagemPendencia pendencia = new ChecagemPendencia();
			
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Checagem de Pendências

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.numero, c.titulo, c.situacao situacao_id, 
				(select ls.texto from {0}lov_checagem_pend_situacao ls where ls.id = c.situacao) situacao_texto,
				tn.numero titulo_numero, tn.ano titulo_ano,
				(select p.numero || '/' || p.ano from {0}tab_protocolo p where p.id = t.protocolo) protocolo_numero,
				(select nvl(i.nome, i.razao_social) from {0}tab_protocolo p, tab_pessoa i where p.id = t.protocolo
				and p.interessado = i.id) protocolo_interessado      
				from {0}tab_checagem_pend c, {0}tab_titulo_numero tn, tab_titulo t where t.id = c.titulo
				and tn.titulo = c.titulo and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pendencia.Id = id;
						pendencia.Tid = reader["tid"].ToString();
						pendencia.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						pendencia.SituacaoTexto = reader["situacao_texto"].ToString();
						
						if (reader["titulo"] != null && !Convert.IsDBNull(reader["titulo"]))
						{
							pendencia.TituloId = Convert.ToInt32(reader["titulo"]);
						}
						pendencia.TituloNumero = TituloNumero.FromDb(reader["titulo_numero"], reader["titulo_ano"]).Texto;
						pendencia.ProtocoloNumero = reader["protocolo_numero"].ToString();
						pendencia.InteressadoNome = reader["protocolo_interessado"].ToString();
					}
					reader.Close();
				}

				if (simplificado)
				{
					return pendencia;
				}

				#endregion

				#region Itens
				comando = bancoDeDados.CriarComando(@"select c.id, c.item_id, c.item_tid, c.nome, c.checagem, ls.id situacao_id, ls.texto situacao_texto,
				c.tid from {0}tab_checagem_pend_itens c, {0}lov_checagem_pend_item_sit ls where c.situacao = ls.id and c.checagem = :checagem", EsquemaBanco);

				comando.AdicionarParametroEntrada("checagem", pendencia.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ChecagemPendenciaItem item = new ChecagemPendenciaItem();
						item.Id = Convert.ToInt32(reader["item_id"]);
						item.Tid = reader["item_tid"].ToString();
						item.IdRelacionamento = Convert.ToInt32(reader["id"]);
						item.Nome = reader["nome"].ToString();
						
						if (reader["checagem"] != null && !Convert.IsDBNull(reader["checagem"]))
						{
							item.ChecagemId = Convert.ToInt32(reader["checagem"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							item.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							item.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						pendencia.Itens.Add(item);
					}
					reader.Close();
				}
				#endregion
			}
			return pendencia;
		}

		internal ChecagemPendencia ObterSimplificado(int id)
		{
			return Obter(id, true);
		}

		internal Resultados<ChecagemPendencia> Filtrar(Filtro<ListarFiltroChecagemPendencia> filtros, BancoDeDados banco = null)
		{
			Resultados<ChecagemPendencia> retorno = new Resultados<ChecagemPendencia>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquema = (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("c.numero", "numero", filtros.Dados.Numero);

				comandtxt += comando.FiltroAnd("c.situacao", "situacao", filtros.Dados.SituacaoPendencia);

				comandtxt += comando.FiltroAndLike("tn.numero||'/'||tn.ano", "titulo_numero", filtros.Dados.TituloNumero);

				if (filtros.Dados.Protocolo.IsValido)
				{
					comandtxt += String.Format(@" and t.id in (select tt.id from {0}tab_protocolo tp, {0}tab_titulo tt where tp.id = tt.protocolo
					and tp.numero = :numero and tp.ano = :ano)", esquema);

					comando.AdicionarParametroEntrada("numero", filtros.Dados.Protocolo.Numero, DbType.Int32);
					comando.AdicionarParametroEntrada("ano", filtros.Dados.Protocolo.Ano, DbType.Int32);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.InteressadoNome))
				{
					comandtxt += " and ( ";
					comandtxt += String.Format("t.protocolo in (select p.id from {0}tab_protocolo p, {0}tab_pessoa i where p.interessado = i.id and ( upper(i.nome) like :interessado_nome or upper(i.razao_social) like :interessado_nome ) )", esquema);
					comandtxt += ") ";
					comando.AdicionarParametroEntrada("interessado_nome", filtros.Dados.InteressadoNome.ToUpper() + "%");
				}


				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "id", "titulo_numero", "protocolo_numero", "situacao_id", "interessado_nome" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("id");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from tab_checagem_pend c, lov_checagem_pend_situacao ls, tab_titulo t, tab_titulo_numero tn 
				where ls.id = c.situacao and t.id = c.titulo and t.id = tn.titulo" + comandtxt, esquema);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select c.id, c.tid, tn.numero || '/' || tn.ano titulo_numero, 
				(select p.numero || '/' || p.ano from {0}tab_protocolo p where p.id = t.protocolo) protocolo_numero,
				c.situacao situacao_id, (select ls.texto from {0}lov_checagem_pend_situacao ls where ls.id = c.situacao) situacao_texto,
				(select nvl(i.nome, i.razao_social) from {0}tab_protocolo p, {0}tab_pessoa i where p.id = t.protocolo and p.interessado = i.id) interessado_nome       
				from {0}tab_checagem_pend c, {0}tab_titulo t, {0}tab_titulo_numero tn where t.id = c.titulo and t.id = tn.titulo " + comandtxt +
				DaHelper.Ordenar(colunas, ordenar), esquema);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					ChecagemPendencia checagemPendencia;

					while (reader.Read())
					{
						checagemPendencia = new ChecagemPendencia();
						checagemPendencia.Id = Convert.ToInt32(reader["id"]);
						checagemPendencia.Tid = reader["tid"].ToString();
						checagemPendencia.TituloNumero = reader["titulo_numero"].ToString();
						checagemPendencia.ProtocoloNumero = reader["protocolo_numero"].ToString();
						checagemPendencia.InteressadoNome = reader["interessado_nome"].ToString();
						checagemPendencia.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
						checagemPendencia.SituacaoTexto = reader["situacao_texto"].ToString();
						retorno.Itens.Add(checagemPendencia);
					}
					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal TituloModeloLst TituloObterTipoModelo(int tituloId, BancoDeDados banco = null)
		{
			TituloModeloLst modelo = new TituloModeloLst();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select m.nome, m.codigo from tab_titulo t, tab_titulo_modelo m where m.id = t.modelo and t.id = :tituloId", EsquemaBanco);
				comando.AdicionarParametroEntrada("tituloId", tituloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						modelo.Codigo = Convert.ToInt32(reader["codigo"]);
						modelo.Texto = reader["nome"].ToString();
					}
					reader.Close();
				}
			}
			return modelo;
		}

		internal List<string> ObterNomeModeloTitulo(List<int> codigoModeloConfigurado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nome from {0}tab_titulo_modelo m where ", EsquemaBanco);
				
				//comando.AdicionarParametroEntrada("codigoModelo", codigoModeloConfigurado, DbType.Int32);
				comando.DbCommand.CommandText += comando.AdicionarIn("", "m.codigo", DbType.Int32, codigoModeloConfigurado);
				
				List<string> lstModelos = bancoDeDados.ExecutarList<string>(comando);

				return lstModelos;
			}
		}

		#endregion

		#region Validações

		internal bool TituloEstaAssociadoChecagemPendencia(int tituloId, int excetoChecagemPendenciaId = 0, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_checagem_pend where titulo = :tituloId and id != :checagemPendenciaId", EsquemaBanco);
				comando.AdicionarParametroEntrada("tituloId", tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("checagemPendenciaId", excetoChecagemPendenciaId, DbType.Int32);

				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					return Convert.ToInt32(obj) > 0;
				}
			}
			return false;
		}

		internal bool ExisteTitulo(int tituloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_titulo where id = :tituloId", EsquemaBanco);
				comando.AdicionarParametroEntrada("tituloId", tituloId, DbType.Int32);
				object obj = bancoDeDados.ExecutarScalar(comando);

				if (obj != null && !Convert.IsDBNull(obj))
				{
					return Convert.ToInt32(obj) > 0;
				}
			}
			return false;
		}

		internal List<string> DocumentosComChecagem(int checagemId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select d.numero||'/'||d.ano numero from {0}tab_protocolo d where d.checagem_pendencia = :checagemId", EsquemaBanco);
				comando.AdicionarParametroEntrada("checagemId", checagemId, DbType.Int32);
				return bancoDeDados.ExecutarList<string>(comando);
			}
		}

		#endregion
	}
}