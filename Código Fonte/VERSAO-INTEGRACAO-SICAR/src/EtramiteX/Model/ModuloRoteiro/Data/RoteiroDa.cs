using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.ModuloLista.Business;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloRoteiro.Data
{
	public class RoteiroDa
	{
		#region Propriedades
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		ListaBus _busLista = new ListaBus();

		#endregion

		public RoteiroDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter / Filtrar

		internal int ObterNovoID(BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select {0}seq_requerimento.nextval from dual", EsquemaBanco);
				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		internal Item ObterItem(int id)
		{
			Item item;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Itens do roteiro
				Comando comando = bancoDeDados.CriarComando(@"select tri.id, tri.nome, tri.condicionante, tri.procedimento, tri.tipo, lrip.texto tipo_texto, tid
				from {0}tab_roteiro_item tri, {0}lov_roteiro_item_tipo lrip where tri.tipo = lrip.id and tri.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					item = new Item();
					if (reader.Read())
					{
						item = new Item();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.ProcedimentoAnalise = reader["procedimento"].ToString();
						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							item.Tipo = Convert.ToInt32(reader["tipo"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
						}
						item.Tid = reader["tid"].ToString();
					}
					reader.Close();
				}
				#endregion
			}
			return item;
		}

		internal Roteiro Obter(int id)
		{
			Roteiro roteiro = new Roteiro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Roteiro
				Comando comando = bancoDeDados.CriarComando(@"select a.numero, a.versao versao_atual, a.nome, a.setor setor_id, s.nome setor_texto, a.situacao, a.observacoes, a.finalidade,
				a.data_criacao, a.tid, (select r.versao from hst_roteiro r where r.tid = a.tid and r.roteiro_id = a.id) versao
				from {0}tab_roteiro a, {0}tab_setor s where a.setor = s.id and a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						roteiro.Id = id;
						roteiro.Tid = reader["tid"].ToString();
						roteiro.Versao = Convert.ToInt32(reader["versao_atual"]);
						roteiro.VersaoAtual = Convert.ToInt32(reader["versao"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.Padrao = _busLista.RoteiroPadrao.Exists(x => x.Id == roteiro.Id);

						if (reader["finalidade"] != null && !Convert.IsDBNull(reader["finalidade"]))
						{
							roteiro.Finalidade = Convert.ToInt32(reader["finalidade"]);
						}

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
							roteiro.SetorNome = reader["setor_texto"].ToString();
						}
						roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
						roteiro.Observacoes = reader["observacoes"].ToString();
						roteiro.DataCriacao = Convert.ToDateTime(reader["data_criacao"]);
					}
					reader.Close();
				}
				#endregion

				#region Itens do roteiro
				comando = bancoDeDados.CriarComando(@"select tri.id, i.ordem, tri.nome, tri.condicionante, tri.procedimento, tri.tipo, lrip.texto tipo_texto, tri.tid
				from {0}tab_roteiro_itens i, {0}tab_roteiro_item tri, {0}lov_roteiro_item_tipo lrip where 
				i.item = tri.id and tri.tipo = lrip.id and i.roteiro = :roteiro order by i.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Item item;
					while (reader.Read())
					{
						item = new Item();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.Ordem = Convert.ToInt32(reader["ordem"]);
						item.ProcedimentoAnalise = reader["procedimento"].ToString();
						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							item.Tipo = Convert.ToInt32(reader["tipo"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
						}
						item.Tid = reader["tid"].ToString();
						roteiro.Itens.Add(item);
					}
					reader.Close();
				}
				#endregion

				#region Modelos de Títulos
				comando = bancoDeDados.CriarComando(@"select rm.id, rm.roteiro, rm.modelo, m.nome modelo_nome, rm.tid from {0}tab_roteiro_modelos rm, 
				{0}tab_titulo_modelo m where rm.roteiro = :roteiro and rm.modelo = m.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloModeloLst item;
					while (reader.Read())
					{
						item = new TituloModeloLst();
						item.Id = Convert.ToInt32(reader["modelo"]);
						item.IdRelacionamento = Convert.ToInt32(reader["id"]);
						item.Texto = reader["modelo_nome"].ToString();
						roteiro.Modelos.Add(item);
					}
					reader.Close();
				}
				#endregion

				#region Atividades
				comando = bancoDeDados.CriarComando(@"select rm.id, rm.roteiro, rm.atividade, a.atividade atividade_nome, 
				rm.tid from {0}tab_roteiro_atividades rm, {0}tab_atividade a where rm.roteiro = :roteiro and rm.atividade = a.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AtividadeSolicitada item;
					while (reader.Read())
					{
						item = new AtividadeSolicitada();
						item.Id = Convert.ToInt32(reader["atividade"]);
						item.IdRelacionamento = Convert.ToInt32(reader["id"]);
						item.Texto = reader["atividade_nome"].ToString();
						roteiro.Atividades.Add(item);
					}
					reader.Close();
				}
				#endregion

				#region Arquivos do roteiro
				comando = bancoDeDados.CriarComando(@"select a.id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho,
				a.tid from {0}tab_roteiro_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.roteiro = :roteiro order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Anexo item;
					while (reader.Read())
					{
						item = new Anexo();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Ordem = Convert.ToInt32(reader["ordem"]);
						item.Descricao = reader["descricao"].ToString();

						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();

						item.Tid = reader["tid"].ToString();

						roteiro.Anexos.Add(item);
					}
					reader.Close();
				}
				#endregion

				#region Palavra chave do roteiro
				comando = bancoDeDados.CriarComando(@"select a.id, a.chave from {0}tab_roteiro_chave a where a.roteiro = :roteiro", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PalavraChave palavra = null;

					while (reader.Read())
					{
						palavra = new PalavraChave();
						palavra.Id = Convert.ToInt32(reader["id"]);
						palavra.Nome = reader["chave"].ToString();
						roteiro.PalavraChaves.Add(palavra);
					}
					reader.Close();
				}
				#endregion
			}
			return roteiro;
		}

		internal Roteiro ObterSimplificado(int id)
		{
			Roteiro roteiro = new Roteiro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Roteiro
				Comando comando = bancoDeDados.CriarComando(@"select a.numero, a.versao versao_atual, a.nome, a.setor setor_id, s.nome setor_texto, a.situacao, a.observacoes, a.finalidade,
				a.data_criacao, a.tid, (select r.versao from hst_roteiro r where r.tid = a.tid and r.roteiro_id = a.id) versao
				from {0}tab_roteiro a, {0}tab_setor s where a.setor = s.id and a.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						roteiro.Id = id;
						roteiro.Tid = reader["tid"].ToString();
						roteiro.Versao = Convert.ToInt32(reader["versao_atual"]);
						roteiro.VersaoAtual = Convert.ToInt32(reader["versao"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.Padrao = _busLista.RoteiroPadrao.Exists(x => x.Id == roteiro.Id);

						if (reader["finalidade"] != null && !Convert.IsDBNull(reader["finalidade"]))
						{
							roteiro.Finalidade = Convert.ToInt32(reader["finalidade"]);
						}

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
							roteiro.SetorNome = reader["setor_texto"].ToString();
						}
						roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
						roteiro.Observacoes = reader["observacoes"].ToString();
						roteiro.DataCriacao = Convert.ToDateTime(reader["data_criacao"]);
					}
					reader.Close();
				}
				#endregion

				#region Itens do roteiro
				comando = bancoDeDados.CriarComando(@"select tri.id, i.ordem, tri.nome, tri.condicionante, tri.procedimento, tri.tipo, lrip.texto tipo_texto, tri.tid
				from {0}tab_roteiro_itens i, {0}tab_roteiro_item tri, {0}lov_roteiro_item_tipo lrip where 
				i.item = tri.id and tri.tipo = lrip.id and i.roteiro = :roteiro order by i.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Item item;
					while (reader.Read())
					{
						item = new Item();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.Ordem = Convert.ToInt32(reader["ordem"]);
						item.ProcedimentoAnalise = reader["procedimento"].ToString();
						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							item.Tipo = Convert.ToInt32(reader["tipo"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
						}
						item.Tid = reader["tid"].ToString();
						roteiro.Itens.Add(item);
					}
					reader.Close();
				}
				#endregion
			}
			return roteiro;
		}

		internal Roteiro ObterHistorico(int id, string tid)
		{
			Roteiro roteiro = new Roteiro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Roteiro
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.numero, a.versao, (select tr.versao from tab_roteiro tr where tr.id = a.roteiro_id) versao_atual, 
				a.nome, a.setor_id, s.nome setor_texto, a.situacao_id situacao, a.observacoes, a.finalidade, a.data_criacao, a.tid from {0}hst_roteiro a, {0}hst_setor s 
				where a.roteiro_id = :id and a.tid = :tidRoteiro and a.setor_id = s.setor_id and a.setor_tid = s.tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						roteiro.Id = id;
						roteiro.Tid = reader["tid"].ToString();
						roteiro.IdRelacionamento = Convert.ToInt32(reader["id"]);
						roteiro.Versao = Convert.ToInt32(reader["versao"]);
						if (reader["versao_atual"] != null && !Convert.IsDBNull(reader["versao_atual"]))
						{
							roteiro.VersaoAtual = Convert.ToInt32(reader["versao_atual"]);
						}

						roteiro.Nome = reader["nome"].ToString();

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
							roteiro.SetorNome = reader["setor_texto"].ToString();
						}

						if (reader["finalidade"] != null && !Convert.IsDBNull(reader["finalidade"]))
						{
							roteiro.Finalidade = Convert.ToInt32(reader["finalidade"]);
						}

						roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
						roteiro.Observacoes = reader["observacoes"].ToString();
						roteiro.DataCriacao = Convert.ToDateTime(reader["data_criacao"]);
					}
					reader.Close();
				}
				#endregion

				//Valida se houve retorno
				if (roteiro.Id > 0)
				{
					#region Itens do roteiro
					comando = bancoDeDados.CriarComando(@"select trhi.item_id id, trhi.ordem, tri.nome, tri.condicionante, tri.procedimento, tri.tipo_id tipo, tri.tipo_texto, trhi.item_tid tid
					from {0}hst_roteiro_itens trhi, {0}hst_roteiro_item tri where trhi.roteiro_id = :roteiro and trhi.tid = :tidRoteiro and trhi.item_id = tri.item_id 
					and trhi.item_tid = tri.tid order by trhi.ordem", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Item item;
						while (reader.Read())
						{
							item = new Item();
							item.Id = Convert.ToInt32(reader["id"]);
							item.Nome = reader["nome"].ToString();
							item.Condicionante = reader["condicionante"].ToString();
							item.Ordem = Convert.ToInt32(reader["ordem"]);
							item.ProcedimentoAnalise = reader["procedimento"].ToString();
							item.Tipo = Convert.ToInt32(reader["tipo"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
							item.Tid = reader["tid"].ToString();
							roteiro.Itens.Add(item);
						}
						reader.Close();
					}
					#endregion

					#region Modelos de Títulos
					comando = bancoDeDados.CriarComando(@"select ra.id, ra.id_hst, ra.roteiro_id, ra.roteiro_tid, m.modelo_id, m.nome modelo_nome from {0}hst_roteiro_modelos ra, 
					{0}hst_titulo_modelo m where ra.modelo_tid = m.tid and  ra.roteiro_id = :roteiro and ra.tid = :tidRoteiro", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						TituloModeloLst item;
						while (reader.Read())
						{
							item = new TituloModeloLst();
							item.Id = Convert.ToInt32(reader["modelo_id"]);
							item.IdRelacionamento = Convert.ToInt32(reader["id"]);
							item.Texto = reader["modelo_nome"].ToString();
							roteiro.Modelos.Add(item);
						}
						reader.Close();
					}
					#endregion

					#region Atividades
					comando = bancoDeDados.CriarComando(@"select ra.id, ra.id_hst, ra.roteiro_id, ra.roteiro_tid, ra.atividade_id, a.atividade atividade_nome, ra.tid 
					from {0}hst_roteiro_atividades ra, {0}hst_atividade a where ra.atividade_tid = a.tid and a.atividade_id = ra.atividade_id 
					and ra.id_hst = :id_hst and ra.tid = :tidRoteiro", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", roteiro.IdRelacionamento, DbType.Int32);
					comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						AtividadeSolicitada item;
						while (reader.Read())
						{
							item = new AtividadeSolicitada();
							item.Id = Convert.ToInt32(reader["atividade_id"]);
							item.IdRelacionamento = Convert.ToInt32(reader["id"]);
							item.Texto = reader["atividade_nome"].ToString();
							roteiro.Atividades.Add(item);
						}
						reader.Close();
					}
					#endregion

					#region Arquivos do roteiro
					comando = bancoDeDados.CriarComando(@"select a.arquivo_id id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho,
					a.tid from {0}hst_roteiro_arquivo a, {0}hst_arquivo b where a.arquivo_id = b.arquivo_id and a.arquivo_tid = b.tid and a.roteiro_id = :roteiro 
					and a.roteiro_tid = :tidRoteiro order by a.ordem", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Anexo item;
						while (reader.Read())
						{
							item = new Anexo();
							item.Id = Convert.ToInt32(reader["id"]);
							item.Ordem = Convert.ToInt32(reader["ordem"]);
							item.Descricao = reader["descricao"].ToString();

							item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
							item.Arquivo.Caminho = reader["caminho"].ToString();
							item.Arquivo.Nome = reader["nome"].ToString();
							item.Arquivo.Extensao = reader["extensao"].ToString();

							item.Tid = reader["tid"].ToString();

							roteiro.Anexos.Add(item);
						}
						reader.Close();
					}
					#endregion

					#region Palavra chave do roteiro
					comando = bancoDeDados.CriarComando(@"select a.chave_id, a.chave from {0}hst_roteiro_chave a where a.roteiro_id = :roteiro and a.roteiro_tid = :tidRoteiro", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						PalavraChave palavra = null;

						while (reader.Read())
						{
							palavra = new PalavraChave();
							palavra.Id = Convert.ToInt32(reader["chave_id"]);
							palavra.Nome = reader["chave"].ToString();
							roteiro.PalavraChaves.Add(palavra);
						}
						reader.Close();
					}
					#endregion
				}
			}
			return roteiro;
		}

		internal Roteiro ObterHistoricoSimplificado(int id, string tdi)
		{
			Roteiro roteiro = new Roteiro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Roteiro
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.numero, a.versao, (select tr.versao from tab_roteiro tr where tr.id = a.roteiro_id) versao_atual, 
				a.nome, a.setor_id, s.nome setor_texto, a.situacao_id situacao, a.observacoes, a.finalidade, a.data_criacao, a.tid from {0}hst_roteiro a, {0}hst_setor s 
				where a.roteiro_id = :id and a.tid = :tidRoteiro and a.setor_id = s.setor_id and a.setor_tid = s.tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tdi);


				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						roteiro.Id = id;
						roteiro.Tid = reader["tid"].ToString();
						roteiro.IdRelacionamento = Convert.ToInt32(reader["id"]);
						roteiro.Versao = Convert.ToInt32(reader["versao"]);
						if (reader["versao_atual"] != null && !Convert.IsDBNull(reader["versao_atual"]))
						{
							roteiro.VersaoAtual = Convert.ToInt32(reader["versao_atual"]);
						}

						roteiro.Nome = reader["nome"].ToString();

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
							roteiro.SetorNome = reader["setor_texto"].ToString();
						}

						if (reader["finalidade"] != null && !Convert.IsDBNull(reader["finalidade"]))
						{
							roteiro.Finalidade = Convert.ToInt32(reader["finalidade"]);
						}

						roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
						roteiro.Observacoes = reader["observacoes"].ToString();
						roteiro.DataCriacao = Convert.ToDateTime(reader["data_criacao"]);
					}
					reader.Close();
				}
				#endregion

				//Valida se houve retorno
				if (roteiro.Id > 0)
				{
					#region Itens do roteiro
					comando = bancoDeDados.CriarComando(@"select trhi.id_hst id, trhi.ordem, tri.nome, tri.condicionante, tri.procedimento, tri.tipo_id tipo, tri.tipo_texto, trhi.item_tid tid
					from {0}hst_roteiro_itens trhi, {0}hst_roteiro_item tri where trhi.roteiro_id = :roteiro and trhi.tid = :tidRoteiro and trhi.item_id = tri.item_id 
					and trhi.item_tid = tri.tid order by trhi.ordem", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tidRoteiro", DbType.String, 36, tdi);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Item item;
						while (reader.Read())
						{
							item = new Item();
							item.Id = Convert.ToInt32(reader["id"]);
							item.Nome = reader["nome"].ToString();
							item.Condicionante = reader["condicionante"].ToString();
							item.Ordem = Convert.ToInt32(reader["ordem"]);
							item.ProcedimentoAnalise = reader["procedimento"].ToString();
							item.Tipo = Convert.ToInt32(reader["tipo"]);
							item.TipoTexto = reader["tipo_texto"].ToString();
							item.Tid = reader["tid"].ToString();
							roteiro.Itens.Add(item);
						}
						reader.Close();
					}
					#endregion
				}
			}
			return roteiro;
		}

		internal List<Roteiro> ObterNomeRoteiro(string nomeRoteiro)
		{
			List<Roteiro> lista = new List<Roteiro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id, a.nome, a.numero, a.versao, a.setor from {0}tab_roteiro a where upper(a.nome) like upper(:nome_roteiro)", EsquemaBanco);
				comando.AdicionarParametroEntrada("nome_roteiro", nomeRoteiro, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na lista
					Roteiro roteiro;
					while (reader.Read())
					{
						roteiro = new Roteiro();
						roteiro.Id = Convert.ToInt32(reader["id"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.Versao = Convert.ToInt32(reader["versao"]);
						roteiro.Setor = Convert.ToInt32(reader["setor"]);
						lista.Add(roteiro);
					}

					reader.Close();
					#endregion
				}
			}
			return lista;
		}

		internal Resultados<Roteiro> Filtrar(Filtro<ListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<Roteiro> retorno = new Resultados<Roteiro>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("r.numero", "numero", filtros.Dados.Numero);

				comandtxt += comando.FiltroAnd("r.setor", "setor", filtros.Dados.Setor);

				comandtxt += comando.FiltroAnd("r.situacao", "situacao", filtros.Dados.Situacao);

				comandtxt += comando.FiltroAndLike("r.nome", "nome", filtros.Dados.Nome, true);

				comandtxt += comando.FiltroIn("r.id", String.Format("select a.roteiro from {0}tab_roteiro_chave a where upper(a.chave) like upper(:palavra_chave)",
					EsquemaBanco), "palavra_chave", filtros.Dados.PalavaChave);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "versao", "sigla", "nome", "situacao_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_roteiro r where r.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select r.id, r.numero, r.versao, r.tid, r.nome, s.id setor_id, s.nome setor_texto, r.situacao, lrs.texto situacao_texto, s.sigla
				from {0}tab_roteiro r, {0}tab_setor s, {0}lov_roteiro_situacao lrs where r.setor = s.id and r.situacao = lrs.id " + comandtxt + DaHelper.Ordenar(colunas, ordenar);

				comando.DbCommand.CommandText = String.Format(@"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Roteiro roteiro;

					while (reader.Read())
					{
						roteiro = new Roteiro();
						roteiro.Id = Convert.ToInt32(reader["id"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.Versao = Convert.ToInt32(reader["versao"]);
						roteiro.Tid = reader["tid"].ToString();
						roteiro.Setor = Convert.ToInt32(reader["setor_id"]);
						roteiro.SetorNome = reader["setor_texto"].ToString();
						roteiro.SetorSigla = reader["sigla"].ToString();
						roteiro.SituacaoTexto = reader["situacao_texto"].ToString();
						roteiro.Situacao = Convert.ToInt32(reader["situacao"]);
						retorno.Itens.Add(roteiro);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Resultados<Item> FiltrarItem(Filtro<ListarFiltroItem> filtros, BancoDeDados banco = null)
		{
			Resultados<Item> retorno = new Resultados<Item>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("r.tipo", "tipo", filtros.Dados.TipoId);

				if (filtros.Dados.TiposPermitidos != null && filtros.Dados.TiposPermitidos.Count > 0)
				{
					comandtxt += comando.AdicionarIn("and", "r.tipo", DbType.Int32, filtros.Dados.TiposPermitidos);
				}

				comandtxt += comando.FiltroAndLike("r.nome", "nome", filtros.Dados.Nome, true);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "tipo_texto", "nome", "condicionante" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_roteiro_item r where r.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select r.id, r.nome, r.tipo, l.texto tipo_texto, r.condicionante, r.procedimento, r.tid from {0}tab_roteiro_item r, {0}lov_roteiro_item_tipo l where r.tipo = l.id " + comandtxt + DaHelper.Ordenar(colunas, ordenar);

				comando.DbCommand.CommandText = String.Format(@"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Item item;

					while (reader.Read())
					{
						item = new Item();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Nome = reader["nome"].ToString();
						item.Condicionante = reader["condicionante"].ToString();
						item.Tipo = Convert.ToInt32(reader["tipo"]);
						item.TipoTexto = reader["tipo_texto"].ToString();
						item.ProcedimentoAnalise = reader["procedimento"].ToString();
						item.Tid = reader["tid"].ToString();
						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal List<Roteiro> ObterRoteiros(Item item, BancoDeDados banco = null)
		{
			List<Roteiro> roteiros = new List<Roteiro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiros

				Comando comando = bancoDeDados.CriarComando(@"select e.roteiro, r.numero from {0}tab_roteiro r, {0}tab_roteiro_itens e where r.id = e.roteiro and e.item = :item", EsquemaBanco);

				comando.AdicionarParametroEntrada("item", item.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Roteiro rot;
					while (reader.Read())
					{
						rot = new Roteiro();
						rot.Id = Convert.ToInt32(reader["roteiro"]);

						roteiros.Add(rot);
					}
					reader.Close();
				}

				#endregion
			}
			return roteiros;
		}

		internal Roteiro ObterRoteirosPorAtividades(Finalidade finalidade, BancoDeDados banco = null)
		{
			Roteiro roteiro = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
											select c.id, c.nome, c.versao, a.atividade atividade_nome, c.tid
												from {0}tab_roteiro            c,
													{0}tab_roteiro_atividades d,
													{0}tab_atividade          a,
													{0}tab_roteiro_modelos    g
												where c.id = d.roteiro
												and c.id = g.roteiro
												and d.atividade = a.id
												and c.situacao = 1
												and d.atividade = :atividade
												and bitand(c.finalidade, :finalidade) > 0
												and g.modelo = :modelo", EsquemaBanco);

				if (finalidade.AtividadeSetorId > 0)
				{
					comando.DbCommand.CommandText += " and a.setor = :setor";
					comando.AdicionarParametroEntrada("setor", finalidade.AtividadeSetorId, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("atividade", finalidade.AtividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("finalidade", finalidade.Codigo, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", finalidade.TituloModelo, DbType.Int32);

				comando.DbCommand.CommandText += " order by c.id";

				#region busca todos os roteiros relacionados com a atividade/modelo/finalidade

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						roteiro = new Roteiro();
						roteiro.Tid = reader["tid"].ToString();
						roteiro.Id = Convert.ToInt32(reader["id"]);
						roteiro.Nome = reader["nome"].ToString();
						roteiro.VersaoAtual = Convert.ToInt32(reader["versao"]);
						roteiro.AtividadeTexto = reader["atividade_nome"].ToString();
					}
					reader.Close();
				}
				#endregion
			}
			return roteiro;
		}

		internal List<String> ObterAnalises(int id, BancoDeDados banco = null)
		{
			List<String> analises = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiros

				Comando comando = bancoDeDados.CriarComando(@"select case when t.protocolo is not null then (select 'processo: '||p.numero||'/'||p.ano from {0}tab_protocolo p where p.id = t.protocolo ) 
				else (select 'documento: '||p.numero||'/'||p.ano from {0}tab_protocolo p where p.id = t.protocolo ) end protocolo from {0}tab_analise_itens tt, {0}tab_analise t  where tt.analise = t.id 
				and tt.item_id = :id order by t.protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						analises.Add(reader["protocolo"].ToString());
					}
					reader.Close();
				}

				#endregion
			}
			return analises;
		}

		internal List<String> ObterChecagens(int id, BancoDeDados banco = null)
		{
			List<String> checagens = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiros

				Comando comando = bancoDeDados.CriarComando(@"select ci.checagem from {0}tab_checagem_itens ci where ci.item_id = :item", EsquemaBanco);

				comando.AdicionarParametroEntrada("item", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						checagens.Add(reader["checagem"].ToString());
					}
					reader.Close();
				}

				#endregion
			}
			return checagens;
		}

		internal int ObterVersaoAtual(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select r.versao from {0}tab_roteiro r where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				object objVersaoAtual = bancoDeDados.ExecutarScalar(comando);

				return (objVersaoAtual != null) ? Convert.ToInt32(objVersaoAtual) : 0;
			}
		}

		internal List<string> ObterFinalidades(int finalidade)
		{
			List<string> finalidades = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Finalidades

				Comando comando = bancoDeDados.CriarComando(@"select lr.texto from {0}lov_titulo_finalidade lr where bitand(:finalidade, lr.codigo) <> 0", EsquemaBanco);

				comando.AdicionarParametroEntrada("finalidade", finalidade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						finalidades.Add(reader["texto"].ToString());
					}
					reader.Close();
				}

				#endregion
			}
			return finalidades;
		}

		internal int ObterFinalidadeCodigo(int finalidadeId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select lr.codigo from {0}lov_titulo_finalidade lr where lr.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", finalidadeId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal Finalidade ObterFinalidade(Finalidade finalidade)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select lr.id Id, lr.codigo Codigo, lr.texto Texto, tm.nome TituloModeloTexto
				from {0}lov_titulo_finalidade lr, {0}tab_titulo_modelo tm where lr.id = :id and tm.id = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", finalidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", finalidade.TituloModelo, DbType.Int32);

				finalidade = bancoDeDados.ObterEntity<Finalidade>(comando);
				return finalidade;
			}
		}

		internal List<int> ObterChecagensFinalizadasContendoRoteiros(List<int> lstRoteiros)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select cr.checagem from {0}tab_checagem c, {0}tab_checagem_roteiro cr 
				where c.id = cr.checagem and c.situacao = 1 ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "cr.roteiro", DbType.Int32, lstRoteiros);
				return bancoDeDados.ExecutarList<int>(comando);
			}
		}

		internal List<int> ObterRoteirosDeItem(int idItem, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ri.roteiro from {0}tab_roteiro_itens ri where ri.item = :itemid ", EsquemaBanco);
				comando.AdicionarParametroEntrada("itemid", idItem, DbType.Int32);
				return bancoDeDados.ExecutarList<int>(comando);
			}
		}

		internal int ObterSituacao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.situacao from {0}tab_roteiro t where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public Atividade ObterAtividade(Atividade atividade)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id Id, a.setor SetorId, a.atividade NomeAtividade from {0}tab_atividade a where a.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", atividade.Id, DbType.Int32);
				atividade = bancoDeDados.ObterEntity<Atividade>(comando);
				return atividade;
			}
		}

		#endregion

		#region Ações de DML

		internal Roteiro Criar(Roteiro roteiro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiro

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro a (id, numero, nome, setor, situacao, observacoes, data_criacao, finalidade, tid) 
				values ({0}seq_roteiro.nextval, {0}seq_roteiro.currval, :nome, :setor, 1, :observacoes, sysdate, :finalidade, :tid) returning a.id, a.numero into :id, :numero", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", DbType.String, 100, roteiro.Nome);
				comando.AdicionarParametroEntrada("setor", roteiro.Setor, DbType.Int32);
				comando.AdicionarParametroEntrada("observacoes", roteiro.Observacoes, DbType.AnsiString);
				comando.AdicionarParametroEntrada("finalidade", (roteiro.Finalidade.HasValue) ? roteiro.Finalidade : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);
				comando.AdicionarParametroSaida("numero", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				roteiro.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Modelos de Títulos

				if (roteiro.Modelos != null && roteiro.Modelos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_modelos a (id, roteiro, modelo, tid) 
						values ({0}seq_roteiro_modelos.nextval, :roteiro, :modelo, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("modelo", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (TituloModeloLst item in roteiro.Modelos)
					{
						comando.SetarValorParametro("modelo", item.Id);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Atividades

				if (roteiro.Atividades != null && roteiro.Atividades.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_atividades a (id, roteiro, atividade, tid) 
						values ({0}seq_roteiro_atividades.nextval, :roteiro, :atividade, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("atividade", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (AtividadeSolicitada item in roteiro.Atividades)
					{
						comando.SetarValorParametro("atividade", item.Id);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Itens do roteiro

				if (roteiro.Itens != null && roteiro.Itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_itens a (id, roteiro, item, ordem, tid) 
						values ({0}seq_roteiro_itens.nextval, :roteiro, :item, :ordem, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("item", DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Item item in roteiro.Itens)
					{
						comando.SetarValorParametro("item", item.Id);
						comando.SetarValorParametro("ordem", item.Ordem);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Arquivos do roteiro

				if (roteiro.Anexos != null && roteiro.Anexos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_arquivo a (id, roteiro, arquivo, ordem, descricao, tid) 
					values ({0}seq_roteiro_arquivo.nextval, :roteiro, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Anexo item in roteiro.Anexos)
					{
						comando.SetarValorParametro("arquivo", item.Arquivo.Id);
						comando.SetarValorParametro("ordem", item.Ordem);
						comando.SetarValorParametro("descricao", item.Descricao);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Palavra chave do roteiro

				if (roteiro.PalavraChaves != null && roteiro.PalavraChaves.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_chave a (id, roteiro, chave, tid) 
					values ({0}seq_roteiro_chave.nextval, :roteiro, :chave, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("chave", DbType.String, 100);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (PalavraChave item in roteiro.PalavraChaves)
					{
						comando.SetarValorParametro("chave", item.Nome);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(roteiro.Id, eHistoricoArtefato.roteiro, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
			return roteiro;
		}

		internal Roteiro Editar(Roteiro roteiro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiro

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
										update {0}tab_roteiro r
											set r.versao = (select nvl(max(a.versao), 0) + 1 from {0}tab_roteiro a where a.numero = r.numero),
												r.nome = :nome,
												r.finalidade = :finalidade,
												r.situacao = :situacao,
												r.observacoes = :observacoes,
												r.tid = :tid
											where r.id = :id returning r.versao into :versao", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", roteiro.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", DbType.String, 100, roteiro.Nome);
				comando.AdicionarParametroEntrada("situacao", roteiro.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("observacoes", roteiro.Observacoes, DbType.AnsiString);
				comando.AdicionarParametroEntrada("finalidade", (roteiro.Finalidade.HasValue) ? roteiro.Finalidade : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("versao", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				roteiro.Versao = Convert.ToInt32(comando.ObterValorParametro("versao"));

				#endregion

				#region Limpar os dados do banco

				//Roteiro item
				comando = bancoDeDados.CriarComando("delete from {0}tab_roteiro_itens ri ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ri.roteiro = :roteiro{0}",

					comando.AdicionarNotIn("and", "ri.item", DbType.Int32, roteiro.Itens.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				//Roteiro Modelos
				comando = bancoDeDados.CriarComando("delete from {0}tab_roteiro_modelos ri ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ri.roteiro = :roteiro{0}",
					comando.AdicionarNotIn("and", "ri.id", DbType.Int32, roteiro.Modelos.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				//Roteiro Atividades
				comando = bancoDeDados.CriarComando("delete from {0}tab_roteiro_atividades ri ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ri.roteiro = :roteiro{0}",
					comando.AdicionarNotIn("and", "ri.id", DbType.Int32, roteiro.Atividades.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				//Arquivos do roteiro
				comando = bancoDeDados.CriarComando("delete from {0}tab_roteiro_arquivo ra ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ra.roteiro = :roteiro{0}",
					comando.AdicionarNotIn("and", "ra.id", DbType.Int32, roteiro.Anexos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				//Palavra chave do roteiro
				comando = bancoDeDados.CriarComando("delete from {0}tab_roteiro_chave rc ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where rc.roteiro = :roteiro{0}",
					comando.AdicionarNotIn("and", "rc.id", DbType.Int32, roteiro.PalavraChaves.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Modelos de Títulos

				foreach (TituloModeloLst item in roteiro.Modelos)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro_modelos set roteiro = :roteiro, modelo = :modelo, 
						tid = :tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_modelos(id, roteiro, modelo, tid) values 
						({0}seq_roteiro_modelos.nextval, :roteiro, :modelo, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("modelo", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Atividades

				foreach (AtividadeSolicitada item in roteiro.Atividades)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro_atividades set roteiro = :roteiro, atividade = :atividade, 
						tid = :tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_atividades(id, roteiro, atividade, tid) values 
						({0}seq_roteiro_atividades.nextval, :roteiro, :atividade, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("atividade", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Itens do roteiro

				foreach (Item item in roteiro.Itens)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro_itens set roteiro = :roteiro, item = :item, 
						ordem = :ordem, tid = :tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_itens(id, roteiro, item, ordem, tid) values 
						({0}seq_roteiro_itens.nextval, :roteiro, :item, :ordem, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("item", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Arquivos do roteiro

				if (roteiro.Anexos != null && roteiro.Anexos.Count > 0)
				{
					foreach (Anexo item in roteiro.Anexos)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro_arquivo a set a.roteiro = :roteiro, a.arquivo = :arquivo, 
							a.ordem = :ordem, a.descricao = :descricao, a.tid = :tid where a.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_arquivo a (id, roteiro, arquivo, ordem, descricao, tid) 
							values ({0}seq_roteiro_arquivo.nextval, :roteiro, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
						comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Palavra chave do roteiro

				foreach (PalavraChave item in roteiro.PalavraChaves)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro_chave set roteiro = :roteiro, chave = :chave, 
						tid = :tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_chave a (id, roteiro, chave, tid) 
					values ({0}seq_roteiro_chave.nextval, :roteiro, :chave, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("roteiro", roteiro.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("chave", DbType.String, 100, item.Nome);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Histórico

				Historico.Gerar(roteiro.Id, eHistoricoArtefato.roteiro, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
			return roteiro;
		}

		public Roteiro Salvar(Roteiro roteiro, BancoDeDados banco = null)
		{
			if (roteiro == null)
			{
				throw new Exception("Roteiro é nulo.");
			}

			if (roteiro.Id == 0)
			{
				roteiro = Criar(roteiro, banco);
			}
			else
			{
				roteiro = Editar(roteiro, banco);
			}
			return roteiro;
		}

		public Item SalvarItem(Item item, BancoDeDados banco = null)
		{
			if (item == null)
			{
				throw new Exception("Item de roteiro nulo.");
			}

			if (item.Id == 0)
			{
				item = CriarItem(item, banco);
			}
			else
			{
				item = EditarItem(item, banco);
			}
			return item;
		}

		internal Roteiro AlterarVersao(Roteiro roteiro, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiro

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro r set 
					r.versao = (select nvl(max(a.versao),0)+1 from {0}tab_roteiro a where a.numero = r.numero), r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", roteiro.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(roteiro.Id, eHistoricoArtefato.roteiro, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
			return roteiro;
		}

		public void AlterarSituacao(int id, int situacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Roteiro

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro r set r.situacao = :situacao, tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);// 1 Ativo e 2 Desativo
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.roteiro, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal Item CriarItem(Item item, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Item

				string tid = GerenciadorTransacao.ObterIDAtual();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_roteiro_item a (id, nome, procedimento, tipo, tid, condicionante) 
					values ({0}seq_roteiro_item.nextval, :nome, :procedimento, :tipo, :tid, :condicionante) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", DbType.String, 200, item.Nome);
				comando.AdicionarParametroEntrada("procedimento", DbType.String, 500, item.ProcedimentoAnalise);
				comando.AdicionarParametroEntrada("condicionante", DbType.String, 200, item.Condicionante);
				comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				item.Tid = tid;
				#endregion

				#region Histórico

				Historico.Gerar(item.Id, eHistoricoArtefato.itemroteiro, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
			return item;
		}

		internal Item EditarItem(Item item, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Item

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_roteiro_item r set r.nome = :nome, r.procedimento = :procedimento, 
					condicionante = :condicionante, r.tipo = :tipo, r.tid = :tid
				where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome", DbType.String, 200, item.Nome);
				comando.AdicionarParametroEntrada("procedimento", DbType.String, 500, item.ProcedimentoAnalise);
				comando.AdicionarParametroEntrada("condicionante", DbType.String, 200, item.Condicionante);
				comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Roteiros

				List<Roteiro> roteiros = ObterRoteiros(item, bancoDeDados);

				foreach (Roteiro roteiro in roteiros)
				{
					AlterarVersao(roteiro, bancoDeDados);
				}

				#endregion

				#region Histórico

				Historico.Gerar(item.Id, eHistoricoArtefato.itemroteiro, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
			return item;
		}

		public void ExcluirItem(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_requerimento c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.itemroteiro, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Itens

				comando = bancoDeDados.CriarComando(@"delete {0}tab_roteiro_item r where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Validações

		internal bool ValidarAtividadesAtivadas(int roteiroId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(ta.id) from tab_roteiro tr, tab_roteiro_atividades tra, tab_atividade ta where tr.id = tra.roteiro
				and tra.atividade = ta.id and ta.situacao = 0 and tr.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", roteiroId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) < 1;
			}
		}
		internal bool VerificarTidAtual(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select r.tid from {0}tab_roteiro r where r.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				string tidAtual = bancoDeDados.ExecutarScalar<string>(comando);

				return tidAtual.Equals(tid, StringComparison.InvariantCultureIgnoreCase);
			}
		}

		internal string VerificarAssociacao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string numero = string.Empty;
				Comando comando = bancoDeDados.CriarComando(@"select r.tid from {0}tab_roteiro r where r.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				numero = bancoDeDados.ExecutarScalar(comando).ToString();

				return numero;
			}
		}

		internal bool ExisteItem(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Itens do roteiro

				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_roteiro_item tri where tri.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				#endregion
			}
		}

		internal bool ExisteItem(int id, string nome)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Itens do roteiro

				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_roteiro_item tri where tri.id <> :id and lower(tri.nome) = lower(:nome)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("nome", nome, DbType.String);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				#endregion
			}
		}

		internal bool ItemUtilizado(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_roteiro_itens t where t.item = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ItemUtilizadoAnalise(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_analise_itens tt where tt.item_id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ValidarRoteiroConfigurado(Roteiro roteiro, out List<string> atividades, out List<string> modelos, out List<string> finalidades)
		{
			bool temConfigurado = false;
			List<string> lstAtividades = new List<string>();
			List<string> lstModelos = new List<string>();
			List<string> lstFinalidades = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) qtd from {0}tab_roteiro ro, {0}tab_roteiro_modelos m, {0}tab_roteiro_atividades atv where 
					bitand(ro.finalidade, :finalidade) <> 0 and ro.id = m.roteiro and ro.id = atv.roteiro and ro.id <> :roteiro_id and ro.situacao = 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("finalidade", roteiro.Finalidade.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("roteiro_id", roteiro.Id, DbType.Int32);

				//Modelo de Título
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "m.modelo", DbType.Int32, roteiro.Modelos.Select(x => x.Id).ToList());
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "atv.atividade", DbType.Int32, roteiro.Atividades.Select(x => x.Id).ToList());

				temConfigurado = Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

				if (temConfigurado)
				{
					#region Atividades

					comando = bancoDeDados.CriarComando(@"
						select distinct ta.atividade
						  from {0}tab_roteiro ro, 
							   {0}tab_roteiro_modelos m, 
							   {0}tab_roteiro_atividades atv, 
							   {0}tab_atividade ta    
						 where bitand(ro.finalidade, :finalidade) <> 0
						   and ro.id = m.roteiro
						   and ro.id = atv.roteiro
						   and ro.id <> :roteiro_id
						   and ro.situacao = 1   
						   and atv.atividade = ta.id(+)", EsquemaBanco);

					comando.AdicionarParametroEntrada("finalidade", roteiro.Finalidade.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("roteiro_id", roteiro.Id, DbType.Int32);
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "m.modelo", DbType.Int32, roteiro.Modelos.Select(x => x.Id).ToList());
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "atv.atividade", DbType.Int32, roteiro.Atividades.Select(x => x.Id).ToList());

					lstAtividades = bancoDeDados.ObterEntityList<string>(comando);

					#endregion

					#region Modelos

					comando = bancoDeDados.CriarComando(@"
						select distinct ttm.nome
						  from {0}tab_roteiro ro, 
							   {0}tab_roteiro_modelos m, 
							   {0}tab_roteiro_atividades atv,
							   {0}tab_titulo_modelo ttm
						 where bitand(ro.finalidade, :finalidade) <> 0
						   and ro.id = m.roteiro
						   and ro.id = atv.roteiro
						   and ro.id <> :roteiro_id
						   and ro.situacao = 1
						   and m.modelo = ttm.id(+)", EsquemaBanco);

					comando.AdicionarParametroEntrada("finalidade", roteiro.Finalidade.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("roteiro_id", roteiro.Id, DbType.Int32);
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "m.modelo", DbType.Int32, roteiro.Modelos.Select(x => x.Id).ToList());
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "atv.atividade", DbType.Int32, roteiro.Atividades.Select(x => x.Id).ToList());

					lstModelos = bancoDeDados.ObterEntityList<string>(comando);

					#endregion

					#region Finalidades

					comando = bancoDeDados.CriarComando(@"
						select distinct ltf.texto
						  from {0}tab_roteiro ro, 
							   {0}tab_roteiro_modelos m, 
							   {0}tab_roteiro_atividades atv,
							   {0}lov_titulo_finalidade ltf     
						 where bitand(ro.finalidade, :finalidade) <> 0
						   and ro.id = m.roteiro
						   and ro.id = atv.roteiro
						   and ro.id <> :roteiro_id
						   and ro.situacao = 1
						   and bitand(ltf.codigo, :finalidade) <> 0", EsquemaBanco);

					comando.AdicionarParametroEntrada("finalidade", roteiro.Finalidade.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("roteiro_id", roteiro.Id, DbType.Int32);
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "m.modelo", DbType.Int32, roteiro.Modelos.Select(x => x.Id).ToList());
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "atv.atividade", DbType.Int32, roteiro.Atividades.Select(x => x.Id).ToList());

					lstFinalidades = bancoDeDados.ObterEntityList<string>(comando);

					#endregion
				}
			}

			atividades = lstAtividades;
			modelos = lstModelos;
			finalidades = lstFinalidades;

			return temConfigurado;
		}

		internal bool ValidarSituacao(int id, int situacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_roteiro t where t.id = :id and t.situacao = :situacao", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool RoteiroDesativado(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select * from {0}tab_roteiro where situacao = 2 and id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		// retorna true case uma lista de roteiros informados seja mdo mesmo setor
		internal bool RoteirosMesmoSetor(List<int> roteiros)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{

				if (roteiros.Count <= 0)
				{
					return true;
				}

				Comando comando = bancoDeDados.CriarComando(String.Empty);
				comando.DbCommand.CommandText = String.Format(@"select count(*) from (select t.setor from tab_roteiro t where t.id = :roteiro) t,
				(select t.setor from tab_roteiro t where {1}) x where t.setor <> x.setor", EsquemaBanco, comando.AdicionarIn("", "t.id", DbType.Int32, roteiros)
				);
				comando.AdicionarParametroEntrada("roteiro", roteiros[0], DbType.Int32);

				return !Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));

			}
		}

		internal bool RoteirosMesmoSetor(List<Roteiro> roteiros)
		{
			return RoteirosMesmoSetor((roteiros ?? new List<Roteiro>()).Select(x => x.Id).ToList<int>());
		}

		internal bool AtividadeEmSetor(int atividadeId, int roteiroSetor)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_atividade a where a.id = :atividade and a.setor = :setor", EsquemaBanco);
				comando.AdicionarParametroEntrada("atividade", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor", roteiroSetor, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) >= 1;
			}
		}

		internal bool AtividadesEmSetor(List<int> lstAtividadeIds, int roteiroSetor)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("select count(*) from {0}tab_atividade a where a.setor != :setor ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "a.id", DbType.Int32, lstAtividadeIds);
				comando.AdicionarParametroEntrada("setor", roteiroSetor, DbType.Int32);
				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) <= 0;
			}
		}

		internal bool TipoPermitido(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tipo from {0}tab_roteiro_item where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) != (int)eRoteiroItemTipo.ProjetoDigital;
			}
		}

		internal String ModeloTituloNaoAdicionadoRoteiro(Finalidade finalidade)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tm.nome from cnf_atividade_atividades caa, cnf_atividade_modelos cam, 
															tab_titulo_modelo tm where caa.atividade = :atividade and cam.configuracao = caa.configuracao and 
															cam.modelo = :modelo and tm.id = cam.modelo and cam.modelo not in (select rm.modelo from 
															tab_roteiro_modelos rm, tab_roteiro_atividades ra where rm.roteiro = ra.roteiro and 
															ra.atividade = :atividade)", EsquemaBanco);

				comando.AdicionarParametroEntrada("atividade", finalidade.AtividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", finalidade.TituloModelo, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

		internal bool IsAtividadeAtiva(int atividade)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select situacao from {0}tab_atividade where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", atividade, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}