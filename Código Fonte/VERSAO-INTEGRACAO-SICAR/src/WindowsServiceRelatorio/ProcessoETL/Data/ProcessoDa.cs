using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;

namespace Tecnomapas.EtramiteX.WindowsService.ProcessoETL.Data
{
	class ProcessoDa
	{
		RelatorioDa _daRelatorio;
		ConfiguracaoSistema _configSys;

		public ProcessoDa()
		{
			_configSys = new ConfiguracaoSistema();
			_daRelatorio = new RelatorioDa();
		}

		#region Ações de DML

		internal void Salvar(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cria os dados da ETL de processo

				Dictionary<string, object> processo;

				Comando comando = bancoDeDados.CriarComando(@"update fat_processo p set p.numero=:numero, p.numero_autuacao=:numero_autuacao, p.data_autuacao=:data_autuacao, 
				p.ano=:ano, p.tipo_id=:tipo_id, p.tipo_texto=:tipo_texto, p.estado_id=:estado_id, p.estado_texto=:estado_texto, p.municipio_id=:municipio_id, p.municipio_texto=:municipio_texto, 
				p.atividades=:atividades, p.titulos=:titulos, p.tid=:tid where p.id = :id");

				comando.AdicionarParametroEntrada("id", DbType.Int32);
				comando.AdicionarParametroEntrada("numero", DbType.Int32);
				comando.AdicionarParametroEntrada("ano", DbType.Int32);
				comando.AdicionarParametroEntrada("numero_autuacao", DbType.String);
				comando.AdicionarParametroEntrada("data_autuacao", DbType.DateTime);
				comando.AdicionarParametroEntrada("tipo_id", DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_texto", DbType.String);
				comando.AdicionarParametroEntrada("estado_id", DbType.Int32);
				comando.AdicionarParametroEntrada("estado_texto", DbType.String);
				comando.AdicionarParametroEntrada("municipio_id", DbType.Int32);
				comando.AdicionarParametroEntrada("municipio_texto", DbType.String);
				comando.AdicionarParametroEntrada("atividades", DbType.StringFixedLength);
				comando.AdicionarParametroEntrada("titulos", DbType.StringFixedLength);
				comando.AdicionarParametroEntrada("tid", DbType.String);

				if (eleitos != null && eleitos.Count > 0)
				{
					bool flag = false;
					eleitos.ForEach(x =>
					{
						flag = Existe(Convert.ToInt32(x["Id"]), bancoDeDados);
						if (flag)
						{
							processo = ObterProcesso(Convert.ToInt32(x["Id"]));
							if (processo != null && processo.Count > 0)
							{
								comando.SetarValorParametro("id", processo["ID"]);
								comando.SetarValorParametro("numero", processo["NUMERO"]);
								comando.SetarValorParametro("ano", processo["ANO"]);
								comando.SetarValorParametro("numero_autuacao", processo["NUMERO_AUTUACAO"]);
								comando.SetarValorParametro("data_autuacao", processo["DATA_AUTUACAO"]);
								comando.SetarValorParametro("tipo_id", processo["TIPO_ID"]);
								comando.SetarValorParametro("tipo_texto", processo["TIPO_TEXTO"]);
								comando.SetarValorParametro("estado_id", processo["ESTADO_ID"]);
								comando.SetarValorParametro("estado_texto", processo["ESTADO_TEXTO"]);
								comando.SetarValorParametro("municipio_id", processo["MUNICIPIO_ID"]);
								comando.SetarValorParametro("municipio_texto", processo["MUNICIPIO_TEXTO"]);
								comando.SetarValorParametroClob("atividades", processo["ATIVIDADES"]);
								comando.SetarValorParametroClob("titulos", processo["TITULOS"]);
								comando.SetarValorParametro("tid", processo["TID"]);
								bancoDeDados.ExecutarNonQuery(comando);
								Dimensoes(Convert.ToInt32(processo["ID"]), processo["TID"].ToString(), bancoDeDados, true);
							}
						}
						else
						{
							Criar(new List<Dictionary<string, object>>() { x }, bancoDeDados);
						}
					});
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Criar(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cria os dados da ETL de processo

				Dictionary<string, object> processo;

				Comando comando = bancoDeDados.CriarComando(@"insert into fat_processo p (id, numero, numero_autuacao, data_autuacao, ano, tipo_id, tipo_texto, estado_id, estado_texto, 
				municipio_id, municipio_texto, atividades, titulos, tid) values (:id, :numero, :numero_autuacao, :data_autuacao, :ano, :tipo_id, :tipo_texto, 
				:estado_id, :estado_texto, :municipio_id, :municipio_texto, :atividades, :titulos, :tid)");

				comando.AdicionarParametroEntrada("id", DbType.Int32);
				comando.AdicionarParametroEntrada("numero", DbType.Int32);
				comando.AdicionarParametroEntrada("ano", DbType.Int32);
				comando.AdicionarParametroEntrada("numero_autuacao", DbType.String);
				comando.AdicionarParametroEntrada("data_autuacao", DbType.DateTime);
				comando.AdicionarParametroEntrada("tipo_id", DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_texto", DbType.String);
				comando.AdicionarParametroEntrada("estado_id", DbType.Int32);
				comando.AdicionarParametroEntrada("estado_texto", DbType.String);
				comando.AdicionarParametroEntrada("municipio_id", DbType.Int32);
				comando.AdicionarParametroEntrada("municipio_texto", DbType.String);
				comando.AdicionarParametroEntrada("atividades", DbType.StringFixedLength);
				comando.AdicionarParametroEntrada("titulos", DbType.StringFixedLength);
				comando.AdicionarParametroEntrada("tid", DbType.String);

				if (eleitos != null && eleitos.Count > 0)
				{
					bool flag = false;
					foreach (var pro in eleitos)
					{
						flag = Existe(Convert.ToInt32(pro["Id"]), bancoDeDados);
						if (!flag)
						{
							processo = ObterProcesso(Convert.ToInt32(pro["Id"]));
							if (processo != null && processo.Count > 0)
							{
								comando.SetarValorParametro("id", processo["ID"]);
								comando.SetarValorParametro("numero", processo["NUMERO"]);
								comando.SetarValorParametro("ano", processo["ANO"]);
								comando.SetarValorParametro("numero_autuacao", processo["NUMERO_AUTUACAO"]);
								comando.SetarValorParametro("data_autuacao", processo["DATA_AUTUACAO"]);
								comando.SetarValorParametro("tipo_id", processo["TIPO_ID"]);
								comando.SetarValorParametro("tipo_texto", processo["TIPO_TEXTO"]);
								comando.SetarValorParametro("estado_id", processo["ESTADO_ID"]);
								comando.SetarValorParametro("estado_texto", processo["ESTADO_TEXTO"]);
								comando.SetarValorParametro("municipio_id", processo["MUNICIPIO_ID"]);
								comando.SetarValorParametro("municipio_texto", processo["MUNICIPIO_TEXTO"]);
								comando.SetarValorParametroClob("atividades", processo["ATIVIDADES"]);
								comando.SetarValorParametroClob("titulos", processo["TITULOS"]);
								comando.SetarValorParametro("tid", processo["TID"]);

								bancoDeDados.ExecutarNonQuery(comando);

								Dimensoes(Convert.ToInt32(processo["ID"]), processo["TID"].ToString(), bancoDeDados);
							}
						}
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Apaga os dados da ETL de processo

				List<String> lista = new List<string>();
				lista.Add("delete from dim_processo_atividade p where p.fato = :processo;");
				lista.Add("delete from dim_processo_cadastro p where p.fato = :processo;");
				lista.Add("delete from dim_processo_empreendimento p where p.fato = :processo;");
				lista.Add("delete from dim_processo_interessado p where p.fato = :processo;");
				lista.Add("delete from dim_processo_posse p where p.fato = :processo;");
				lista.Add("delete from dim_processo_titulo p where p.fato = :processo;");
				lista.Add("delete from fat_processo p where p.id = :processo;");

				Comando comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;");

				comando.AdicionarParametroEntrada("processo", DbType.Int32);

				eleitos.ForEach(x =>
				{
					comando.SetarValorParametro("processo", x["Id"]);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void Dimensoes(int id, string tid, BancoDeDados banco = null, bool deletar = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;
				List<Dictionary<string, object>> aux;
				Dictionary<string, object> aux2;

				#region Apaga os dados da ETL das dimensões de processo

				if (deletar)
				{
					List<String> lista = new List<string>();
					lista.Add("delete from dim_processo_atividade p where p.fato = :processo;");
					lista.Add("delete from dim_processo_cadastro p where p.fato = :processo;");
					lista.Add("delete from dim_processo_empreendimento p where p.fato = :processo;");
					lista.Add("delete from dim_processo_interessado p where p.fato = :processo;");
					lista.Add("delete from dim_processo_posse p where p.fato = :processo;");
					lista.Add("delete from dim_processo_titulo p where p.fato = :processo;");

					comando = bancoDeDados.CriarComando(@"begin " + string.Join(" ", lista) + "end;");
					comando.AdicionarParametroEntrada("processo", id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);

					bancoDeDados.Commit();
				}

				#endregion

				#region Cria os dados das dimensões de processo

				#region Atividades

				aux = ObterAtividades(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_processo_atividade a(id, fato, atividade_id, atividade, situacao_id, situacao_texto, setor_id, setor_sigla, tid)
					values (seq_dim_processo_atividade.nextval, :fato, :atividade_id, :atividade, :situacao_id, :situacao_texto, :setor_id, :setor_sigla, :tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);
					comando.AdicionarParametroEntrada("atividade_id", DbType.Int32);
					comando.AdicionarParametroEntrada("atividade", DbType.String);
					comando.AdicionarParametroEntrada("situacao_id", DbType.Int32);
					comando.AdicionarParametroEntrada("situacao_texto", DbType.String);
					comando.AdicionarParametroEntrada("setor_id", DbType.Int32);
					comando.AdicionarParametroEntrada("setor_sigla", DbType.String);

					foreach (var ativ in aux)
					{
						comando.SetarValorParametro("atividade_id", ativ["ATIVIDADE_ID"]);
						comando.SetarValorParametro("atividade", ativ["ATIVIDADE"]);
						comando.SetarValorParametro("situacao_id", ativ["SITUACAO_ID"]);
						comando.SetarValorParametro("situacao_texto", ativ["SITUACAO_TEXTO"]);
						comando.SetarValorParametro("setor_id", ativ["SETOR_ID"]);
						comando.SetarValorParametro("setor_sigla", ativ["SETOR_SIGLA"]);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Títulos

				aux = ObterTitulos(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_processo_titulo t(id, fato, numero, numero_ano, modelo_id, modelo_nome, modelo_tipo_id, modelo_tipo_texto, situacao_id, situacao_texto, 
					setor_id, setor_sigla, data_emissao, data_vencimento, solicitado_externo, tid) values (seq_dim_processo_titulo.nextval, :fato, :numero, :numero_ano, 
					:modelo_id, :modelo_nome, :modelo_tipo_id, :modelo_tipo_texto, :situacao_id, :situacao_texto, :setor_id, :setor_sigla, :data_emissao, :data_vencimento, :solicitado_externo, :tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);
					comando.AdicionarParametroEntrada("numero", DbType.Int32);
					comando.AdicionarParametroEntrada("numero_ano", DbType.Int32);
					comando.AdicionarParametroEntrada("modelo_id", DbType.Int32);
					comando.AdicionarParametroEntrada("modelo_nome", DbType.String);
					comando.AdicionarParametroEntrada("modelo_tipo_id", DbType.Int32);
					comando.AdicionarParametroEntrada("modelo_tipo_texto", DbType.String);
					comando.AdicionarParametroEntrada("situacao_id", DbType.Int32);
					comando.AdicionarParametroEntrada("situacao_texto", DbType.String);
					comando.AdicionarParametroEntrada("setor_id", DbType.Int32);
					comando.AdicionarParametroEntrada("setor_sigla", DbType.String);
					comando.AdicionarParametroEntrada("data_emissao", DbType.DateTime);
					comando.AdicionarParametroEntrada("data_vencimento", DbType.DateTime);
					comando.AdicionarParametroEntrada("solicitado_externo", DbType.String);

					foreach (var tit in aux)
					{
						comando.SetarValorParametro("numero", tit["NUMERO"]);
						comando.SetarValorParametro("numero_ano", tit["ANO"]);
						comando.SetarValorParametro("modelo_id", tit["MODELO_ID"]);
						comando.SetarValorParametro("modelo_nome", tit["MODELO_NOME"]);
						comando.SetarValorParametro("modelo_tipo_id", tit["MODELO_TIPO_ID"]);
						comando.SetarValorParametro("modelo_tipo_texto", tit["MODELO_TIPO_TEXTO"]);
						comando.SetarValorParametro("situacao_id", tit["SITUACAO_ID"]);
						comando.SetarValorParametro("situacao_texto", tit["SITUACAO_TEXTO"]);
						comando.SetarValorParametro("setor_id", tit["SETOR_ID"]);
						comando.SetarValorParametro("setor_sigla", tit["SETOR_SIGLA"]);
						comando.SetarValorParametro("data_emissao", tit["DATA_EMISSAO"]);
						comando.SetarValorParametro("data_vencimento", tit["DATA_VENCIMENTO"]);
						comando.SetarValorParametro("solicitado_externo", tit["SOLICITADO_EXTERNO"]);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Cadastro

				aux2 = ObterCadastro(id);

				if (aux2 != null && aux2.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_processo_cadastro c(id, fato, nome, setor_id, setor_nome, data_cadastro, data_cadastro_dia, data_cadastro_mes, 
					data_cadastro_ano, tid) values (seq_dim_processo_cadastro.nextval, :fato, :nome, :setor_id, :setor_nome, :data_cadastro, :data_cadastro_dia, :data_cadastro_mes, 
					:data_cadastro_ano, :tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);
					comando.AdicionarParametroEntrada("nome", aux2["NOME"], DbType.String);
					comando.AdicionarParametroEntrada("setor_id", aux2["SETOR_ID"], DbType.Int32);
					comando.AdicionarParametroEntrada("setor_nome", aux2["SETOR_NOME"], DbType.String);
					comando.AdicionarParametroEntrada("data_cadastro", aux2["DATA_CRIACAO"], DbType.DateTime);
					comando.AdicionarParametroEntrada("data_cadastro_dia", aux2["DATA_CADASTRO_DIA"], DbType.Int32);
					comando.AdicionarParametroEntrada("data_cadastro_mes", aux2["DATA_CADASTRO_MES"], DbType.Int32);
					comando.AdicionarParametroEntrada("data_cadastro_ano", aux2["DATA_CADASTRO_ANO"], DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);

				}

				#endregion

				#region Empreendimento

				aux2 = ObterEmpreendimento(id);

				if (aux2 != null && aux2.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_processo_empreendimento d (id, fato, denominador, cnpj, segmento_id, segmento_texto, zona_id, zona_texto, estado_id, 
					estado_texto, municipio_id, municipio_texto, coordenadas, tid) values (seq_dim_processo_empreend.nextval, :fato, :denominador, :cnpj, 
					:segmento_id, :segmento_texto, :zona_id, :zona_texto, :estado_id, :estado_texto, :municipio_id, :municipio_texto, :coordenadas, :tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);
					comando.AdicionarParametroEntrada("denominador", aux2["DENOMINADOR"], DbType.String);
					comando.AdicionarParametroEntrada("cnpj", aux2["CNPJ"], DbType.String);
					comando.AdicionarParametroEntrada("segmento_id", aux2["SEGMENTO_ID"], DbType.Int32);
					comando.AdicionarParametroEntrada("segmento_texto", aux2["SEGMENTO_TEXTO"], DbType.String);
					comando.AdicionarParametroEntrada("zona_id", aux2["ZONA_ID"], DbType.Int32);
					comando.AdicionarParametroEntrada("zona_texto", aux2["ZONA_TEXTO"], DbType.String);
					comando.AdicionarParametroEntrada("estado_id", aux2["ESTADO_ID"], DbType.Int32);
					comando.AdicionarParametroEntrada("estado_texto", aux2["ESTADO_TEXTO"], DbType.String);
					comando.AdicionarParametroEntrada("municipio_id", aux2["MUNICIPIO_ID"], DbType.Int32);
					comando.AdicionarParametroEntrada("municipio_texto", aux2["MUNICIPIO_TEXTO"], DbType.String);
					comando.AdicionarParametroEntrada("coordenadas", aux2["COORDENADAS"], DbType.String);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Interessado

				aux2 = ObterInteressado(id);

				if (aux2 != null && aux2.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_processo_interessado i (id, fato, nome_razao, cpf_cnpj, tid) values
					(seq_dim_processo_interessado.nextval, :fato, :nome_razao, :cpf_cnpj, :tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);
					comando.AdicionarParametroEntrada("nome_razao", aux2["NOME_RAZAO"], DbType.String);
					comando.AdicionarParametroEntrada("cpf_cnpj", aux2["CPF_CNPJ"], DbType.String);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Posse

				aux2 = ObterPosse(id);

				if (aux2 != null && aux2.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_processo_posse p (id, fato, nome, setor_id, setor_nome, data_recebimento, data_recebimento_dia, data_recebimento_mes, 
					data_recebimento_ano, tid) values (seq_dim_processo_posse.nextval, :fato, :nome, :setor_id, :setor_nome, :data_recebimento, :data_recebimento_dia, :data_recebimento_mes, 
					:data_recebimento_ano, :tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);
					comando.AdicionarParametroEntrada("nome", aux2["NOME"], DbType.String);
					comando.AdicionarParametroEntrada("setor_id", aux2["SETOR_ID"], DbType.Int32);
					comando.AdicionarParametroEntrada("setor_nome", aux2["SETOR_NOME"], DbType.String);
					comando.AdicionarParametroEntrada("data_recebimento", aux2["DATA_RECEBIMENTO"], DbType.DateTime);
					comando.AdicionarParametroEntrada("data_recebimento_dia", aux2["DATA_RECEBIMENTO_DIA"], DbType.Int32);
					comando.AdicionarParametroEntrada("data_recebimento_mes", aux2["DATA_RECEBIMENTO_MES"], DbType.Int32);
					comando.AdicionarParametroEntrada("data_recebimento_ano", aux2["DATA_RECEBIMENTO_ANO"], DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		public List<Dictionary<string, object>> Eleitos(DateTime execucaoInicio, BancoDeDados banco = null)
		{
			List<Dictionary<string, object>> retorno = new List<Dictionary<string, object>>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct t.id, (case when t.acao != 3 then null else 3 end) acao from (
					select h.id_protocolo id, (select a.acao from lov_historico_artefatos_acoes a where a.id = h.acao_executada) acao
						from hst_protocolo h
						where h.protocolo_id = 1
						and h.data_execucao > :execucao_inicio
						and h.acao_executada in (33, 34, 35, 204)
					union all
					select p.id_protocolo id, 0 acao
						from hst_tramitacao h, hst_protocolo p
						where h.protocolo_id = p.id_protocolo
						and h.protocolo_tid = p.tid 
						and p.protocolo_id = 1
						and h.data_execucao > :execucao_inicio
				) t");

				comando.AdicionarParametroEntrada("execucao_inicio", execucaoInicio, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dictionary<string, object> eleito;
					while (reader.Read())
					{
						eleito = new Dictionary<string, object>();
						eleito.Add("Id", reader.GetValue<int>("id"));
						eleito.Add("Acao", reader.GetValue<int>("acao"));
						retorno.Add(eleito);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		public bool Existe(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from  fat_processo p where p.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public Dictionary<string, object> ObterProcesso(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.numero, p.ano, p.numero_autuacao, p.data_autuacao, lp.id tipo_id, lp.texto tipo_texto,
				m.municipio_id, m.municipio_texto, m.estado_id, m.estado_texto, (select stragg_barra(a.atividade) from tab_atividade a, tab_protocolo_atividades ta 
				where ta.protocolo = p.id and a.id = ta.atividade) atividades, 
				(select stragg(tn.numero||'/'||tn.ano||' - '||m.sigla) from tab_titulo t, tab_titulo_numero tn, tab_titulo_modelo m 
				where t.situacao = 3 and t.id = tn.titulo and t.protocolo = p.id and t.modelo = m.id) titulos 
				from tab_protocolo p, lov_protocolo_tipo lp,
				(select s.municipio municipio_id, lm.texto municipio_texto, lm.estado estado_id, le.sigla estado_texto, s.setor from tab_setor_endereco s, 
				lov_municipio lm, lov_estado le where lm.id = s.municipio and lm.estado = le.id ) m where p.id = :id and p.tipo = lp.id and m.setor = p.setor_criacao order by p.id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionary(comando);
			}
		}

		public List<Dictionary<string, object>> ObterAtividades(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.id atividade_id, a.atividade, pa.situacao situacao_id,
				a.setor setor_id, (select s.sigla from tab_setor s where s.id = a.setor) setor_sigla,
				(select la.texto from lov_atividade_situacao la where la.id = pa.situacao) situacao_texto,
				pa.tid from tab_protocolo_atividades pa, tab_atividade a where pa.atividade = a.id
				and pa.protocolo = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterTitulos(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, (select tn.numero from tab_titulo_numero tn where tn.titulo = t.id) numero,
				(select tn.ano from tab_titulo_numero tn where tn.titulo = t.id) ano, t.modelo modelo_id,
				(select m.nome from tab_titulo_modelo m where m.id = t.modelo) modelo_nome, (select m.tipo from tab_titulo_modelo m where m.id = t.modelo) modelo_tipo_id,
				(select (select lm.texto from lov_titulo_modelo_tipo lm where lm.id = m.tipo) from tab_titulo_modelo m where m.id = t.modelo) modelo_tipo_texto,
				t.situacao situacao_id, (select ls.texto from lov_titulo_situacao ls where ls.id = t.situacao) situacao_texto, t.setor setor_id,
				(select s.sigla from tab_setor s where s.id = t.setor) setor_sigla, t.data_emissao, t.data_vencimento,
				case when (select count(*) from tab_titulo_modelo_regras mr, lov_titulo_modelo_regras l where l.id = 8 and l.id = mr.regra and mr.id = t.modelo) = 1 then 'Solicitado' else 'Não solicitado' end solicitado_externo
				from tab_titulo t where t.protocolo = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public Dictionary<string, object> ObterCadastro(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.executor_id id, p.executor_nome nome, p.data_criacao, to_char(p.data_criacao, 'DD') data_cadastro_dia,
				to_char(p.data_criacao, 'MM') data_cadastro_mes, to_char(p.data_criacao, 'YYYY') data_cadastro_ano, p.setor_criacao_id setor_id, s.nome setor_nome from hst_protocolo p, hst_setor s
				where p.id in(select min(id) from hst_protocolo p where p.id_protocolo = :id) and p.setor_id = s.setor_id and p.setor_criacao_tid = s.tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionary(comando);
			}
		}

		public Dictionary<string, object> ObterEmpreendimento(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.denominador, e.cnpj, e.segmento segmento_id, 
				(select ls.texto from lov_empreendimento_segmento ls where ls.id = e.segmento) segmento_texto,
				ee.zona zona_id, (case when ee.zona = 1 then 'Urbana' when ee.zona = 2 then 'Rural' end) zona_texto, ee.estado estado_id,
				(select le.sigla from lov_estado le where le.id = ee.estado) estado_texto, ee.municipio municipio_id,
				(select lm.texto from lov_municipio lm where lm.id = ee.municipio) municipio_texto, (select  'Tipo Coord: '||(select lp.texto from 
				lov_coordenada_tipo lp where c.tipo_coordenada = lp.id)||', Easting: '||c.easting_utm||', Northing: '||c.northing_utm  from tab_empreendimento_coord c 
				where c.empreendimento = e.id) coordenadas from tab_protocolo p, tab_empreendimento e, tab_empreendimento_endereco ee where  e.id = p.empreendimento
				and ee.empreendimento(+) = e.id and ee.correspondencia = 0 and p.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				Dictionary<string, object> aux = bancoDeDados.ExecutarDictionary(comando);


				return aux;
			}
		}

		public Dictionary<string, object> ObterInteressado(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.interessado id, (select nvl(pe.nome, pe.razao_social) from tab_pessoa pe where pe.id = p.interessado) nome_razao,
				(select nvl(pe.cpf, pe.cnpj) from tab_pessoa pe where pe.id = p.interessado) cpf_cnpj, p.tid from tab_protocolo p where p.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionary(comando);
			}
		}

		public Dictionary<string, object> ObterPosse(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id,
				nvl(p.emposse,(select pa.emposse from tab_protocolo pa where pa.id = (select pa.protocolo from tab_protocolo_associado pa where pa.associado = p.id))) emposse,
				(select f.nome from tab_funcionario f where f.id = nvl(p.emposse, (select po.emposse from tab_protocolo_associado pa, tab_protocolo po where pa.protocolo = po.id 
				and pa.associado = p.id)) ) nome, nvl(p.setor, (select po.setor from tab_protocolo_associado pa, tab_protocolo po where pa.protocolo = po.id and pa.associado = p.id)
				) setor_id,  (select s.nome from tab_setor s where s.id =  nvl(p.setor, (select po.setor from tab_protocolo_associado pa, tab_protocolo po where pa.protocolo = po.id 
				and pa.associado = p.id) )) setor_nome, (select tt.data_execucao from hst_tramitacao tt where tt.id = p.tramitacao) data_recebimento
				from tab_protocolo p where p.protocolo = 1 and p.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				Dictionary<string, object> dic = bancoDeDados.ExecutarDictionary(comando);

				dic.Add("DATA_RECEBIMENTO_DIA", null);
				dic.Add("DATA_RECEBIMENTO_MES", null);
				dic.Add("DATA_RECEBIMENTO_ANO", null);

				if (dic["DATA_RECEBIMENTO"] != null && !Convert.IsDBNull(dic["DATA_RECEBIMENTO"]))
				{
					dic["DATA_RECEBIMENTO_DIA"] = Convert.ToDateTime(dic["DATA_RECEBIMENTO"]).Day;
					dic["DATA_RECEBIMENTO_MES"] = Convert.ToDateTime(dic["DATA_RECEBIMENTO"]).Month;
					dic["DATA_RECEBIMENTO_ANO"] = Convert.ToDateTime(dic["DATA_RECEBIMENTO"]).Year;
				}

				return dic;
			}
		}

		#endregion
	}
}