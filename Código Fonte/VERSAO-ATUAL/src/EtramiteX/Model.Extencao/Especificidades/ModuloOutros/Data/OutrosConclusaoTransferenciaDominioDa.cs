using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloOutros;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloOutros.Data
{
	public class OutrosConclusaoTransferenciaDominioDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public OutrosConclusaoTransferenciaDominioDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(OutrosConclusaoTransferenciaDominio outros, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro do Titulo

				eHistoricoAcao acao;
				object id;

				//Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_out_con_tra_dominio e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", outros.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_out_con_tra_dominio e set e.titulo = :titulo, e.protocolo = :protocolo, e.tid = :tid where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					outros.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_out_con_tra_dominio(id, titulo, protocolo, tid) values ({0}seq_esp_out_con_tra_dominio.nextval, :titulo, 
                        :protocolo, :tid) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", outros.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", outros.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					outros = outros ?? new OutrosConclusaoTransferenciaDominio();
					outros.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				List<int> ids = outros.Responsaveis.Concat(outros.Interessados.Concat(outros.Destinatarios)).Select(x => x.IdRelacionamento).ToList();

				comando = bancoDeDados.CriarComando("delete from {0}esp_out_con_tra_dom_pes t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, ids));
				comando.AdicionarParametroEntrada("especificidade", outros.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Destinatário
				foreach (var item in outros.Destinatarios)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_out_con_tra_dom_pes t set t.especificidade = :especificidade, t.pessoa_tipo = :tipo_pessoa, t.pessoa = :pessoa, t.tid = :tid where t.id = :id_rel", EsquemaBanco);
						comando.AdicionarParametroEntrada("id_rel", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_out_con_tra_dom_pes (id, especificidade, pessoa_tipo, pessoa, tid) values ({0}seq_esp_out_con_tra_dom_pes.nextval, 
							:especificidade, :tipo_pessoa, :pessoa, :tid)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("especificidade", outros.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("pessoa", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_pessoa", (int)ePessoaAssociacaoTipo.Destinatario, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Interessados

				foreach (var item in outros.Interessados)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_out_con_tra_dom_pes t set t.especificidade = :especificidade, t.pessoa_tipo = :tipo_pessoa, t.pessoa = :pessoa, t.tid = :tid where t.id = :id_rel", EsquemaBanco);
						comando.AdicionarParametroEntrada("id_rel", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_out_con_tra_dom_pes (id, especificidade, pessoa_tipo, pessoa, tid) values ({0}seq_esp_out_con_tra_dom_pes.nextval, 
							:especificidade, :tipo_pessoa, :pessoa, :tid)", EsquemaBanco);

					}

					comando.AdicionarParametroEntrada("especificidade", outros.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("pessoa", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_pessoa", (int)ePessoaAssociacaoTipo.Interessado, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Responsavel

				foreach (var item in outros.Responsaveis)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_out_con_tra_dom_pes t set t.especificidade = :especificidade, t.pessoa_tipo = :tipo_pessoa, t.pessoa = :pessoa, t.tid = :tid where t.id = :id_rel", EsquemaBanco);
						comando.AdicionarParametroEntrada("id_rel", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_out_con_tra_dom_pes (id, especificidade, pessoa_tipo, pessoa, tid) values ({0}seq_esp_out_con_tra_dom_pes.nextval, 
							:especificidade, :tipo_pessoa, :pessoa, :tid)", EsquemaBanco);

					}

					comando.AdicionarParametroEntrada("especificidade", outros.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("pessoa", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_pessoa", (int)ePessoaAssociacaoTipo.Responsavel, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Histórico

				Historico.Gerar(Convert.ToInt32(outros.Titulo.Id), eHistoricoArtefatoEspecificidade.outroscontransferenciadominio, acao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_out_con_tra_dominio c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.outroscontransferenciadominio, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = banco.CriarComandoPlSql(@"
                begin
                    delete from {0}esp_out_con_tra_dom_pes ep where ep.especificidade = (select e.id from {0}esp_out_con_tra_dominio e where e.titulo = :titulo);
                    delete from {0}esp_out_con_tra_dominio e where e.titulo = :titulo;
                end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal OutrosConclusaoTransferenciaDominio Obter(int titulo, BancoDeDados banco = null)
		{
			OutrosConclusaoTransferenciaDominio especificidade = new OutrosConclusaoTransferenciaDominio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.protocolo, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo from {0}esp_out_con_tra_dominio e, 
                    {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id(+) and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = Convert.ToInt32(reader["id"]);
						especificidade.Tid = reader["tid"].ToString();

						if (reader["protocolo"] != null && !Convert.IsDBNull(reader["protocolo"]))
						{
							especificidade.ProtocoloReq.IsProcesso = (reader["protocolo_tipo"] != null && Convert.ToInt32(reader["protocolo_tipo"]) == 1);
							especificidade.ProtocoloReq.RequerimentoId = Convert.ToInt32(reader["requerimento"]);
							especificidade.ProtocoloReq.Id = Convert.ToInt32(reader["protocolo"]);
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

				#region Pessoas Associadas

				comando = bancoDeDados.CriarComando(@"select ep.id IdRelacionamento, ep.pessoa Id, nvl(p.nome, p.razao_social) Nome, ep.pessoa_tipo Tipo from esp_out_con_tra_dom_pes ep, 
				esp_out_con_tra_dominio e, tab_titulo t, tab_pessoa p where p.id = ep.pessoa and ep.especificidade = e.id and e.titulo = t.id and t.id = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				List<PessoaEspecificidade> pessoas = bancoDeDados.ObterEntityList<PessoaEspecificidade>(comando);

				especificidade.Destinatarios = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Destinatario).ToList();
				especificidade.Interessados = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Interessado).ToList();
				especificidade.Responsaveis = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Responsavel).ToList();
				#endregion

				#endregion
			}

			return especificidade;
		}

		internal Outros ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Outros outros = new Outros();
			List<PessoaPDF> pessoas = new List<PessoaPDF>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);

				outros.Titulo = dados.Titulo;
				outros.Titulo.SetorEndereco = DaEsp.ObterEndSetor(outros.Titulo.SetorId);
				outros.Protocolo = dados.Protocolo;
				outros.Empreendimento = dados.Empreendimento;

				#region Pesssoas

				Comando comando = bancoDeDados.CriarComando(@"select ep.pessoa_tipo, nvl(p.nome, p.razao_social) nome_razao 
				from {0}esp_out_con_tra_dominio e, {0}esp_out_con_tra_dom_pes ep, {0}tab_pessoa p
				where e.id = ep.especificidade and ep.pessoa = p.id and e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						PessoaPDF pessoa = new PessoaPDF();
						pessoa.Tipo = reader.GetValue<int>("pessoa_tipo");
						pessoa.NomeRazaoSocial = reader.GetValue<string>("nome_razao");
						pessoas.Add(pessoa);
					}

					reader.Close();
				}

				outros.Destinatarios = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Destinatario).ToList();
				outros.Interessados = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Interessado).ToList();
				outros.ResponsaveisEmpreendimento = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Responsavel).ToList();

				#endregion Pesssoas
			}

			return outros;
		}

		internal OutrosConclusaoTransferenciaDominio ObterHistorico(int tituloId, string tituloTid, BancoDeDados banco = null)
		{
			OutrosConclusaoTransferenciaDominio especificidade = new OutrosConclusaoTransferenciaDominio();
			int hst = 0;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select h.id, h.especificidade_id, h.tid, h.protocolo_id, hn.numero, hn.ano, hp.requerimento_id, hp.protocolo_id protocolo_tipo
				from {0}hst_esp_out_con_tra_dominio h, {0}hst_titulo_numero hn, {0}hst_protocolo hp where h.titulo_id = hn.titulo_id(+) and h.titulo_tid = hn.tid(+) and h.protocolo_id = hp.id_protocolo(+) 
				and h.protocolo_tid = hp.tid(+) and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = h.acao_executada and l.acao = 3) 
				and h.titulo_id = :titulo and h.titulo_tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tituloTid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						especificidade.Titulo.Id = tituloId;
						especificidade.Id = reader.GetValue<int>("especificidade_id");
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento_id");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo_id");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
					}

					reader.Close();
				}

				#region Pessoas Associadas

				comando = bancoDeDados.CriarComando(@"select ep.transf_dominio_pessoa_id IdRelacionamento, ep.pessoa_id Id, nvl(p.nome, p.razao_social) Nome, ep.pessoa_tipo_id Tipo
				from {0}hst_esp_out_con_tra_dominio e, {0}hst_esp_out_con_tra_dom_pes ep, {0}hst_pessoa p where e.id = ep.id_hst and ep.pessoa_id = p.pessoa_id(+)
				and ep.pessoa_tid = p.tid(+) and p.data_execucao = (select max(x.data_execucao) from {0}hst_pessoa x where x.pessoa_id = p.pessoa_id and x.tid = p.tid) and e.id = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);
				List<PessoaEspecificidade> pessoas = bancoDeDados.ObterEntityList<PessoaEspecificidade>(comando);

				especificidade.Destinatarios = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Destinatario).ToList();
				especificidade.Interessados = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Interessado).ToList();
				especificidade.Responsaveis = pessoas.Where(x => x.Tipo == (int)ePessoaAssociacaoTipo.Responsavel).ToList();
				#endregion

				#endregion
			}

			return especificidade;
		}

		#endregion
	}
}