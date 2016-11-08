using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using System.Collections.Generic;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloLaudo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloLaudo.Data
{
	public class LaudoVistoriaFundiariaDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public LaudoVistoriaFundiariaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(LaudoVistoriaFundiaria laudo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro do Titulo

				eHistoricoAcao acao;
				object id;

				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_laudo_vistoria_fundiaria e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", laudo.Titulo.Id, DbType.Int32);

				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_laudo_vistoria_fundiaria e set e.titulo = :titulo, e.protocolo = :protocolo,
														e.destinatario = :destinatario, e.data_vistoria = :data_vistoria, e.regularizacao_fundiaria = :regularizacao_fundiaria,
														e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					laudo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_laudo_vistoria_fundiaria(id, titulo, protocolo, destinatario, data_vistoria, regularizacao_fundiaria, tid)
														values ({0}seq_esp_laudo_visto_florestal.nextval, :titulo, :protocolo, :destinatario, :data_vistoria, 
														:regularizacao_fundiaria, :tid) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", laudo.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", laudo.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", laudo.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("data_vistoria", laudo.DataVistoria.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("regularizacao_fundiaria", laudo.RegularizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					laudo = laudo ?? new LaudoVistoriaFundiaria();
					laudo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}


				//Regularizacao Dominios
				comando = bancoDeDados.CriarComando("delete from {0}esp_laudo_vist_fund_domin d ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where d.especificidade = :especificidade{0}",
				comando.AdicionarNotIn("and", "d.id", DbType.Int32, laudo.RegularizacaoDominios.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("especificidade", laudo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Regularizacao Dominios

				if (laudo.RegularizacaoDominios != null && laudo.RegularizacaoDominios.Count > 0)
				{
					foreach (var item in laudo.RegularizacaoDominios)
					{

						object regularizacaoDominioTid;

						comando = bancoDeDados.CriarComando(@"select (case when t.situacao in (1, 2, 4) then 
															(select c.tid from crt_regularizacao_dominio c where c.id = :regularizacao_dominio)
															else (select dominio_tid from esp_laudo_vist_fund_domin where especificidade = 
															(select e.id from esp_laudo_vistoria_fundiaria e where e.titulo = :titulo) 
															and dominio = :regularizacao_dominio) end) tid 
															from tab_titulo t where t.id = :titulo", EsquemaBanco);

						comando.AdicionarParametroEntrada("titulo", laudo.Titulo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("regularizacao_dominio", item.DominioId, DbType.Int32);
						regularizacaoDominioTid = bancoDeDados.ExecutarScalar(comando);


						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update esp_laudo_vist_fund_domin d set d.especificidade = :especificidade, 
																d.dominio = :dominio, d.dominio_tid = :dominio_tid, d.tid = :tid where d.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into esp_laudo_vist_fund_domin (id, especificidade, dominio, dominio_tid, tid) 
																values(seq_esp_laudo_vist_fund_domin.nextval, :especificidade, :dominio, :dominio_tid, :tid)
																returning id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("especificidade", laudo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("dominio", item.DominioId, DbType.Int32);
						comando.AdicionarParametroEntrada("dominio_tid", regularizacaoDominioTid, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}

				}

				#endregion

				#endregion

				Historico.Gerar(Convert.ToInt32(laudo.Titulo.Id), eHistoricoArtefatoEspecificidade.laudovistoriafundiaria, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_laudo_vistoria_fundiaria c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.laudovistoriafundiaria, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"begin " +
					"delete from {0}esp_laudo_vist_fund_domin ed where ed.especificidade = (select id from esp_laudo_vistoria_fundiaria where titulo = :titulo);" +
					"delete from {0}esp_laudo_vistoria_fundiaria e where e.titulo = :titulo;" +
				"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal List<Lista> ObterPossesRegularizacao(int empreendimento, BancoDeDados banco = null)
		{
			List<Lista> lista = new List<Lista>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select rd.id, lc.texto comp_texto, dd.area_croqui 
				from {0}crt_regularizacao r, {0}crt_regularizacao_dominio rd, {0}crt_dominialidade_dominio dd, {0}lov_crt_domin_comprovacao lc
				where r.id = rd.regularizacao and rd.dominio = dd.id and dd.comprovacao = lc.id and r.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista item;
					while (reader.Read())
					{
						item = new Lista();
						item.Id = reader["id"].ToString();
						item.Texto = reader["comp_texto"].ToString() + " - " + reader.GetValue<decimal>("area_croqui").ToStringTrunc();
						item.IsAtivo = true;
						lista.Add(item);
					}

					reader.Close();
				}
			}

			return lista;
		}

		internal LaudoVistoriaFundiaria Obter(int titulo, BancoDeDados banco = null)
		{
			LaudoVistoriaFundiaria especificidade = new LaudoVistoriaFundiaria();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo,  e.data_vistoria, e.regularizacao_fundiaria regularizacao_fundiaria_id, 
															n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo, e.destinatario, (select distinct nvl(pe.nome, pe.razao_social) 
															from {0}hst_esp_laudo_visto_fundiaria he, {0}hst_pessoa pe where he.destinatario_id = pe.pessoa_id 
															and he.destinatario_tid = pe.tid and pe.data_execucao = (select max(h.data_execucao) 
															from {0}hst_pessoa h where h.pessoa_id = pe.pessoa_id and h.tid = pe.tid) and 
															he.especificidade_id = e.id and not exists(select 1 from {0}lov_historico_artefatos_acoes l 
															where l.id = he.acao_executada and l.acao = 3) and he.titulo_tid = (select ht.tid 
															from {0}hst_titulo ht where ht.titulo_id = e.titulo and ht.data_execucao = (select min(htt.data_execucao) 
															from {0}hst_titulo htt where htt.titulo_id = e.titulo and htt.data_execucao > (select max(httt.data_execucao) 
															from {0}hst_titulo httt where httt.titulo_id = e.titulo and httt.situacao_id = 1)))) destinatario_nome_razao 
															from {0}esp_laudo_vistoria_fundiaria e, {0}tab_titulo_numero n, {0}tab_protocolo p 
															where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.RegularizacaoId = reader.GetValue<Int32>("regularizacao_fundiaria_id");
						especificidade.Tid = reader["tid"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							especificidade.ProtocoloReq.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);
							especificidade.ProtocoloReq.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
							especificidade.ProtocoloReq.Id = Convert.ToInt32(reader["protocolo"]);
						}

						if (reader["destinatario"] != null && !Convert.IsDBNull(reader["destinatario"]))
						{
							especificidade.Destinatario = Convert.ToInt32(reader["destinatario"]);
							especificidade.DestinatarioNomeRazao = Convert.ToString(reader["destinatario_nome_razao"]);
						}

						if (reader["data_vistoria"] != null && !Convert.IsDBNull(reader["data_vistoria"]))
						{
							especificidade.DataVistoria.Data = Convert.ToDateTime(reader["data_vistoria"]);
						}

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							especificidade.Titulo.Numero.Inteiro = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							especificidade.Titulo.Numero.Ano = Convert.ToInt32(reader["ano"]);
						}
					}

					reader.Close();
				}

				#endregion

				#region Regularizacao Dominios

				comando = bancoDeDados.CriarComando(@"
				select ed.id, ed.tid, ed.dominio dominio_id, d.comprovacao_texto, d.area_croqui 
				from {0}hst_crt_dominialidade_dominio d, {0}hst_crt_regularizacao_dominio rd, {0}hst_esp_laudo_visto_fundiaria he, {0}esp_laudo_vist_fund_domin ed 
				where d.dominialidade_dominio_id = rd.dominio_id and d.tid = rd.dominio_tid and rd.regularizacao_dominio_id = ed.dominio and rd.tid = ed.dominio_tid 
				and rd.regularizacao_tid = he.regularizacao_fundiaria_tid and he.especificidade_id = ed.especificidade and he.tid = ed.tid  and ed.especificidade = :especificidade 
				order by ed.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("especificidade", especificidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					RegularizacaoDominio regularizacaoDominio = null;

					while (reader.Read())
					{
						regularizacaoDominio = new RegularizacaoDominio();
						regularizacaoDominio.Id = Convert.ToInt32(reader["id"]);
						regularizacaoDominio.DominioId = Convert.ToInt32(reader["dominio_id"]);
						regularizacaoDominio.Comprovacao = reader["comprovacao_texto"].ToString();
						regularizacaoDominio.AreaCroqui = Convert.ToDecimal(reader["area_croqui"]).ToStringTrunc();
						regularizacaoDominio.Tid = reader["tid"].ToString();

						especificidade.RegularizacaoDominios.Add(regularizacaoDominio);
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Laudo ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Laudo laudo = new Laudo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados do Titulo

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				laudo.Titulo = dados.Titulo;
				laudo.Protocolo = dados.Protocolo;
				laudo.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select destinatario, data_vistoria from {0}esp_laudo_vistoria_fundiaria e where e.titulo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						laudo.Destinatario.Id = Convert.ToInt32(reader["destinatario"]);

						if (reader["data_vistoria"] != null && !Convert.IsDBNull(reader["data_vistoria"]))
						{
							laudo.DataVistoria = Convert.ToDateTime(reader["data_vistoria"]).ToShortDateString();
						}
					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"select d.id, d.numero_ccri, d.data_ultima_atualizacao, d.area_ccri, d.confrontante_norte, d.confrontante_sul, 
				d.confrontante_leste, d.confrontante_oeste from {0}esp_laudo_vistoria_fundiaria e, {0}crt_regularizacao_dominio r, {0}crt_dominialidade_dominio d
				where e.regularizacao_dominio = r.id and r.dominio = d.id and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						laudo.Dominio = new DominioPDF
						{
							Id = reader.GetValue<int>("id"),
							NumeroCCIR = reader.GetValue<long>("numero_ccri"),
							DataAtualizacao = reader.GetValue<string>("data_ultima_atualizacao"),
							AreaCCIRDecimal = reader.GetValue<decimal>("area_ccri"),
							ConfrontacaoNorte = reader.GetValue<string>("confrontante_norte"),
							ConfrontacaoSul = reader.GetValue<string>("confrontante_sul"),
							ConfrontacaoLeste = reader.GetValue<string>("confrontante_leste"),
							ConfrontacaoOeste = reader.GetValue<string>("confrontante_oeste")
						};
					}

					reader.Close();
				}

				#endregion

				laudo.Destinatario = DaEsp.ObterDadosPessoa(laudo.Destinatario.Id, laudo.Empreendimento.Id, bancoDeDados);

				laudo.AnaliseItens = DaEsp.ObterAnaliseItem(laudo.Protocolo.Id.GetValueOrDefault(), bancoDeDados);
			}

			return laudo;
		}

		#endregion

		#region Validações

		public bool ValidarCaracterizacaoModificada(int titulo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}crt_regularizacao_dominio rd, {0}esp_laudo_vistoria_fundiaria e, 
															{0}esp_laudo_vist_fund_domin ed where ed.dominio_tid = rd.tid and ed.dominio = rd.id 
															and e.id = ed.especificidade and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				return !Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		#region Auxiliar

		internal int ExistePecaTecnica(int atividade, int protocolo, BancoDeDados banco = null)
		{
			int Id = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select * from tab_peca_tecnica tpt, tab_protocolo_atividades tpa where tpa.protocolo = tpt.protocolo and tpt.protocolo = :protocolo and tpa.atividade = :atividade", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				Id = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
			return Id;
		}

		#endregion
	}
}