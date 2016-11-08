using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAnaliseItens;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAnaliseItens.Data
{
	class PecaTecnicaDa
	{
		#region Propriedades

		Historico _historico = new Historico();

		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public PecaTecnicaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public int Salvar(PecaTecnica pecaTecnica, BancoDeDados banco = null)
		{
			if (pecaTecnica == null)
			{
				throw new Exception("Peça Técnica é nula.");
			}

			if (!pecaTecnica.Id.HasValue || pecaTecnica.Id <= 0)
			{
				return Criar(pecaTecnica, banco).GetValueOrDefault();
			}
			else
			{
				Editar(pecaTecnica, banco);
				return pecaTecnica.Id.GetValueOrDefault();
			}

		}

		internal int? Criar(PecaTecnica pecaTecnica, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Dados Principais

				Comando comando = null;

				if (pecaTecnica.ElaboradorTipo == (int)eElaboradorTipo.TecnicoIdaf)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_peca_tecnica t (id, protocolo, atividade, elaborador_tipo, elaborador_tecnico, setor_cadastro, tid) 
					values ({0}seq_peca_tecnica.nextval, :protocolo, :atividade, :elaborador_tipo, :elaborador, :setor_cadastro, :tid) returning t.id into :id", EsquemaBanco);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_peca_tecnica t (id, protocolo, atividade, elaborador_tipo, elaborador_pessoa, setor_cadastro, tid) 
					values ({0}seq_peca_tecnica.nextval, :protocolo, :atividade, :elaborador_tipo, :elaborador, :setor_cadastro, :tid) returning t.id into :id", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("protocolo", pecaTecnica.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", pecaTecnica.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("elaborador_tipo", pecaTecnica.ElaboradorTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("elaborador", pecaTecnica.Elaborador, DbType.Int32);
				comando.AdicionarParametroEntrada("setor_cadastro", pecaTecnica.SetorCadastro, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				pecaTecnica.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Responsaveis/Destinatarios

				if (pecaTecnica.ResponsaveisEmpreendimento != null && pecaTecnica.ResponsaveisEmpreendimento.Count > 0)
				{
					foreach (Responsavel item in pecaTecnica.ResponsaveisEmpreendimento)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_peca_tecnica_dest (id, peca_tecnica, destinatario, tid)
								values ({0}seq_peca_tecnica_dest.nextval, :peca_tecnica, :destinatario, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("peca_tecnica", pecaTecnica.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("destinatario", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Histórico/Consulta/Posse

				Historico.Gerar(pecaTecnica.Id.Value, eHistoricoArtefato.pecatecnica, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return pecaTecnica.Id;
			}
		}

		internal void Editar(PecaTecnica pecaTecnica, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = null;

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_peca_tecnica_dest p where p.peca_tecnica = :peca_tecnica ", EsquemaBanco);

				comando.AdicionarParametroEntrada("peca_tecnica", pecaTecnica.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Dados Principais

				if (pecaTecnica.ElaboradorTipo == (int)eElaboradorTipo.TecnicoIdaf)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_peca_tecnica t set t.protocolo = :protocolo, t.atividade = :atividade,
					t.elaborador_tipo = :elaborador_tipo, t.elaborador_pessoa = null, t.elaborador_tecnico = :elaborador, t.setor_cadastro = :setor_cadastro, t.tid = :tid  where t.id = :id", EsquemaBanco);
				}
				else 
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_peca_tecnica t set t.protocolo = :protocolo, t.atividade = :atividade,
					t.elaborador_tipo = :elaborador_tipo, t.elaborador_pessoa = :elaborador, t.elaborador_tecnico = null, t.setor_cadastro = :setor_cadastro, t.tid = :tid  where t.id = :id", EsquemaBanco);
				}

				

				comando.AdicionarParametroEntrada("id", pecaTecnica.Id, DbType.Int32);

				comando.AdicionarParametroEntrada("protocolo", pecaTecnica.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", pecaTecnica.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("elaborador_tipo", pecaTecnica.ElaboradorTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("elaborador", pecaTecnica.Elaborador, DbType.Int32);
				comando.AdicionarParametroEntrada("setor_cadastro", pecaTecnica.SetorCadastro, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				if (pecaTecnica.ResponsaveisEmpreendimento != null && pecaTecnica.ResponsaveisEmpreendimento.Count > 0)
				{
					foreach (Responsavel item in pecaTecnica.ResponsaveisEmpreendimento)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_peca_tecnica_dest (id, peca_tecnica, destinatario, tid)
								values ({0}seq_peca_tecnica_dest.nextval, :peca_tecnica, :destinatario, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("peca_tecnica", pecaTecnica.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("destinatario", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#region Histórico

				Historico.Gerar(pecaTecnica.Id.Value, eHistoricoArtefato.pecatecnica, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal PecaTecnica Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			PecaTecnica pecaTecnica = new PecaTecnica();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (String.IsNullOrWhiteSpace(tid))
				{
					pecaTecnica = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(r.id) existe from {0}tab_peca_tecnica r where r.id = :id and r.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					if (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando)))
					{
						pecaTecnica = Obter(id, bancoDeDados, simplificado);
					}
					else
					{
						pecaTecnica = ObterHistorico(id, bancoDeDados, tid, simplificado);
					}
				}
			}

			return pecaTecnica;
		}

		internal PecaTecnica Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			PecaTecnica pecaTecnica = new PecaTecnica();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados Principais

				Comando comando = bancoDeDados.CriarComando(@"select t.protocolo, t.atividade, t.elaborador_tipo, nvl(t.elaborador_pessoa, t.elaborador_tecnico) elaborador, t.setor_cadastro, t.tid 
				from {0}tab_peca_tecnica t where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pecaTecnica.Id = id;
						pecaTecnica.Tid = reader["tid"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							pecaTecnica.Protocolo.Id = Convert.ToInt32(reader["protocolo"]);
						}

						if (reader["atividade"] != null && !Convert.IsDBNull(reader["atividade"]))
						{
							pecaTecnica.Atividade = Convert.ToInt32(reader["atividade"]);
						}

						if (reader["elaborador_tipo"] != null && !Convert.IsDBNull(reader["elaborador_tipo"]))
						{
							pecaTecnica.ElaboradorTipo = Convert.ToInt32(reader["elaborador_tipo"]);
						}

						if (reader["elaborador"] != null && !Convert.IsDBNull(reader["elaborador"]))
						{
							pecaTecnica.Elaborador = Convert.ToInt32(reader["elaborador"]);
						}

						if (reader["setor_cadastro"] != null && !Convert.IsDBNull(reader["setor_cadastro"]))
						{
							pecaTecnica.SetorCadastro = Convert.ToInt32(reader["setor_cadastro"]);
						}
					}

					reader.Close();
				}

				if (pecaTecnica.Id <= 0 || simplificado)
				{
					return pecaTecnica;
				}

				#endregion

				#region Responsaveis/Destinatarios

				comando = bancoDeDados.CriarComando(@"select p.id destinatario_id, nvl(p.nome, p.razao_social) destinatario_nome_razao 
				from {0}tab_peca_tecnica_dest pr, {0}tab_pessoa p where pr.peca_tecnica = :peca_tecnica
				and pr.destinatario = p.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("peca_tecnica", pecaTecnica.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Responsavel responsavel = new Responsavel();
						responsavel.Id = reader.GetValue<Int32>("destinatario_id");
						responsavel.NomeRazao = reader.GetValue<String>("destinatario_nome_razao");

						pecaTecnica.ResponsaveisEmpreendimento.Add(responsavel);
					}

					reader.Close();
				}

				#endregion
			}

			return pecaTecnica;
		}

		internal PecaTecnica ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			PecaTecnica pecaTecnica = new PecaTecnica();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados Principais

				Comando comando = bancoDeDados.CriarComando(@"select t.protocolo_id, t.atividade_id, t.elaborador_tipo_id, t.elaborador_tipo_id, 
				t.setor_cadastro_id, t.tid  from {0}hst_peca_tecnica t where t.peca_tecnica_id = :id and t.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						pecaTecnica.Id = id;
						pecaTecnica.Tid = reader["tid"].ToString();

						if (reader["protocolo_id"] != null && !Convert.IsDBNull(reader["protocolo_id"]))
						{
							pecaTecnica.Protocolo.Id = Convert.ToInt32(reader["protocolo_id"]);
						}

						if (reader["atividade_id"] != null && !Convert.IsDBNull(reader["atividade_id"]))
						{
							pecaTecnica.Atividade = Convert.ToInt32(reader["atividade_id"]);
						}

						if (reader["elaborador_tipo_id"] != null && !Convert.IsDBNull(reader["elaborador_tipo_id"]))
						{
							pecaTecnica.ElaboradorTipo = Convert.ToInt32(reader["elaborador_tipo_id"]);
						}

						if (reader["elaborador_id"] != null && !Convert.IsDBNull(reader["elaborador_id"]))
						{
							pecaTecnica.Elaborador = Convert.ToInt32(reader["elaborador_id"]);
						}

						if (reader["setor_cadastro_id"] != null && !Convert.IsDBNull(reader["setor_cadastro_id"]))
						{
							pecaTecnica.SetorCadastro = Convert.ToInt32(reader["setor_cadastro_id"]);
						}
					}

					reader.Close();
				}

				if (pecaTecnica.Id <= 0 || simplificado)
				{
					return pecaTecnica;
				}

				#endregion

				#region Responsaveis/Destinatarios

				comando = bancoDeDados.CriarComando(@"select p.pessoa_id destinatario_id, nvl(p.nome, p.razao_social) destinatario_nome_razao 
													from {0}hst_peca_tecnica_dest pr, {0}hst_pessoa p where pr.peca_tecnica_id = :peca_tecnica
													and pr.id_hst = :id_hst and p.tid = pr.destinatario_tid and pr.destinatario_id = p.pessoa_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("peca_tecnica", pecaTecnica.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Responsavel responsavel = new Responsavel();
						responsavel.Id = reader.GetValue<Int32>("destinatario_id");
						responsavel.NomeRazao = reader.GetValue<String>("destinatario_nome_razao");

						pecaTecnica.ResponsaveisEmpreendimento.Add(responsavel);
					}

					reader.Close();
				}

				#endregion
			}

			return pecaTecnica;
		}

		#endregion

		#region Auxiliar

		internal int ExistePecaTecnica(int atividade, int protocolo, BancoDeDados banco = null)
		{
			int Id = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_peca_tecnica where protocolo = :protocolo and atividade = :atividade", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				Id = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}

			return Id;
		}

		#endregion
	}
}