using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Business;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data
{
	public class TermoCPFARLDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();		
		GerenciadorConfiguracao<ConfiguracaoSistema>  _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		private string EsquemaBancoGeo { get; set; }

		#endregion

		public TermoCPFARLDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioGeo);

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TermoCPFARL termo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_termo_comp_pfarl e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@" update {0}esp_termo_comp_pfarl t set t.protocolo = :protocolo, t.tid = :tid where t.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					termo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@" insert into {0}esp_termo_comp_pfarl (id, titulo, protocolo, tid) 
														values ({0}seq_esp_termo_comp_pfarl.nextval, :titulo, :protocolo,
														:tid) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", termo.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					termo = termo ?? new TermoCPFARL();
					termo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Destinatário

				comando = bancoDeDados.CriarComando("delete from {0}esp_termo_comp_pfarl_destinat t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, termo.Destinatarios.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in termo.Destinatarios)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_termo_comp_pfarl_destinat t set t.destinatario = :destinatario, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_termo_comp_pfarl_destinat (id, especificidade, destinatario, tid) values ({0}seq_esp_termo_comp_pfarl_dest.nextval, 
							:especificidade, :destinatario, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("destinatario", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Histórico

				Historico.Gerar(termo.Titulo.Id, eHistoricoArtefatoEspecificidade.termocomppfarl, acao, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(
					"begin " +
					  "update {0}esp_termo_comp_pfarl c set c.tid = :tid where c.titulo = :titulo; " +
					  "update {0}esp_termo_comp_pfarl_destinat c set c.tid = :tid where c.especificidade = (select id from esp_termo_comp_pfarl where titulo = :titulo); " +
					"end; ", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.termocomppfarl, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(
					"begin " +
					  "delete from {0}esp_termo_comp_pfarl_destinat c where c.especificidade = (select id from esp_termo_comp_pfarl where titulo = :titulo); " +
					  "delete from {0}esp_termo_comp_pfarl e where e.titulo = :titulo; " +
					"end; ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal TermoCPFARL Obter(int titulo, BancoDeDados banco = null)
		{
			TermoCPFARL especificidade = new TermoCPFARL();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@" select e.id, e.titulo, e.protocolo, e.tid, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo
					from {0}esp_termo_comp_pfarl e, {0}tab_titulo_numero n, {0}tab_protocolo p where n.titulo(+) = e.titulo and e.protocolo = p.id and e.titulo = :titulo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("id");
						especificidade.Tid = reader.GetValue<string>("tid");
						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
					}

					reader.Close();
				}

				#region Destinatário

				comando = bancoDeDados.CriarComando(@" select p.id Id, t.id IdRelacionamento, nvl(p.nome, p.razao_social) Nome from 
													{0}esp_termo_comp_pfarl_destinat t, {0}esp_termo_comp_pfarl e, {0}tab_pessoa p where 
													t.especificidade = e.id and e.titulo = :titulo and p.id = t.destinatario", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.Destinatarios = bancoDeDados.ObterEntityList<TermoCPFARLDestinatario>(comando);

				#endregion

				#endregion
			}

			return especificidade;
		}

		internal TermoCPFARL ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			TermoCPFARL especificidade = new TermoCPFARL();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				if (situacao > 0)
				{
					comando = bancoDeDados.CriarComando(@"select e.id,
						e.especificidade_id,
						e.tid,
						p.id_protocolo,
						e.dominio_id,
						n.numero,
						n.ano,
						p.requerimento_id,
						p.protocolo_id protocolo_tipo
					from {0}hst_esp_termo_comp_pfarl e,
						{0}hst_titulo_numero           n,
						{0}hst_protocolo               p
					where e.titulo_id = n.titulo_id(+)
					and e.titulo_tid = n.titulo_tid(+)
					and e.protocolo_id = p.id_protocolo(+)
					and e.protocolo_tid = p.tid(+)
					and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
					and e.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
					(select max(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo_id and htt.situacao_id = :situacao))
					and e.titulo_id = :titulo", EsquemaBanco);

					comando.AdicionarParametroEntrada("situacao", situacao, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select e.id,
						e.especificidade_id,
						e.tid,
						p.id_protocolo,
						e.dominio_id,
						n.numero,
						n.ano,
						p.requerimento_id,
						p.protocolo_id protocolo_tipo
					from {0}hst_esp_termo_comp_pfarl e,
						{0}hst_titulo_numero           n,
						{0}hst_protocolo               p
					where e.titulo_id = n.titulo_id(+)
					and e.titulo_tid = n.titulo_tid(+)
					and e.protocolo_id = p.id_protocolo(+)
					and e.protocolo_tid = p.tid(+)
					and not exists(select 1 from {0}lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
					and e.titulo_tid = (select ht.tid from {0}hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao =
					(select min(htt.data_execucao) from {0}hst_titulo htt where htt.titulo_id = e.titulo_id and htt.data_execucao >
					(select max(httt.data_execucao) from {0}hst_titulo httt where httt.titulo_id = e.titulo_id and httt.situacao_id = 1)))
					and e.titulo_id = :titulo", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("especificidade_id");
						especificidade.Tid = reader.GetValue<string>("tid");

						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento_id");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("id_protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
					}

					reader.Close();
				}

				#endregion

				#region Destinatario

				comando = bancoDeDados.CriarComando(@"select distinct(p.pessoa_id), d.termo_destinatario_id id, nvl(p.nome, p.razao_social) nome_razao
				from {0}hst_esp_termo_comp_pfarl_dest d, {0}hst_pessoa p where d.destinatario_id = p.pessoa_id and d.destinatario_tid = p.tid
				and p.data_execucao = (select max(h.data_execucao) from {0}hst_pessoa h where h.pessoa_id = p.pessoa_id and h.tid = p.tid) and d.id_hst = :hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						especificidade.Destinatarios.Add(new TermoCPFARLDestinatario()
						{
							IdRelacionamento = reader.GetValue<int>("id"),
							Id = reader.GetValue<int>("pessoa_id"),
							Nome = reader.GetValue<string>("nome_razao")
						});
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal Termo ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Termo termo = new Termo();
			termo.Dominialidade = new DominialidadePDF();
			List<int> destinatarioIds = new List<int>();
			PessoaPDF pessoa = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				termo.Titulo = dados.Titulo;
				termo.Protocolo = dados.Protocolo;
				termo.Empreendimento = dados.Empreendimento;
				termo.Dominialidade = new DominialidadePDF(new DominialidadeBus().ObterPorEmpreendimento(dados.Empreendimento.Id.GetValueOrDefault()));

				#endregion

				#region Especificidade

				List<ReservaLegalPDF> reservas = new List<ReservaLegalPDF>();
				termo.Dominialidade.Dominios.ForEach(dominio => {dominio.ReservasLegais.ForEach(r => { reservas.Add(r); });});

				#endregion

				#region Interessado

				Comando comando = bancoDeDados.CriarComando(@" select t.destinatario from {0}esp_termo_comp_pfarl_destinat t, {0}esp_termo_comp_pfarl e where t.especificidade = e.id and e.titulo = :titulo ", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				destinatarioIds = bancoDeDados.ExecutarList<int>(comando);
				termo.Interessados = new List<PessoaPDF>();
				foreach (int destId in destinatarioIds)
				{
					pessoa = _daEsp.ObterDadosPessoa(destId, dados.Empreendimento.Id, banco);
					pessoa.VinculoTipoTexto = pessoa.VinculoTipoTexto == "Outro" ? "Representante" : pessoa.VinculoTipoTexto;
					termo.Interessados.Add(pessoa);
				}

				#endregion

				#region ARLs

				comando = bancoDeDados.CriarComando(@"select r.identificacao, r.arl_croqui, null coordenadaN, null coordenadaE,
													r.situacao, r.situacao_vegetal from {0}crt_dominialidade_reserva r, {0}esp_termo_comp_pfarl e
													where e.titulo = :titulo and r.dominio in (select id from {0}crt_dominialidade_dominio 
													where dominialidade = :dominialidade)", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidade", termo.Dominialidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					termo.RLFormacao = new List<AreaReservaLegalPDF>();
					termo.RLPreservada = new List<AreaReservaLegalPDF>();

					while (reader.Read())
					{
						int situacaoVegetal = reader.GetValue<int>("situacao_vegetal");
						int situacao = reader.GetValue<int>("situacao");

						switch (situacaoVegetal)
						{
							case (int)eReservaLegalSituacaoVegetal.Preservada:

								if (situacao == (int)eReservaLegalSituacao.Proposta)
								{
									termo.RLPreservada.Add(new AreaReservaLegalPDF
									{
										AreaCroqui = Convert.ToDecimal(reader.GetValue<string>("arl_croqui")).ToStringTrunc(),
										CoordenadaE = reader.GetValue<string>("coordenadaE"),
										CoordenadaN = reader.GetValue<string>("coordenadaN"),
										Identificacao = reader.GetValue<string>("identificacao")
									});
								}
								break;

							case (int)eReservaLegalSituacaoVegetal.EmRecuperacao:

								if (situacao == (int)eReservaLegalSituacao.Proposta)
								{
									termo.RLFormacao.Add(new AreaReservaLegalPDF
									{
										AreaCroqui = Convert.ToDecimal(reader.GetValue<string>("arl_croqui")).ToStringTrunc(),
										CoordenadaE = reader.GetValue<string>("coordenadaE"),
										CoordenadaN = reader.GetValue<string>("coordenadaN"),
										Identificacao = reader.GetValue<string>("identificacao")
									});
								}
								break;
						}
					}

					reader.Close();
				}

				termo.RLTotalPreservada = termo.RLPreservada.Sum(x => Convert.ToDecimal(x.AreaCroqui)).ToStringTrunc();
				termo.RLTotalFormacao = termo.RLFormacao.Sum(x => Convert.ToDecimal(x.AreaCroqui)).ToStringTrunc();

				#endregion

				#region ARLs - Coordenadas

				comando = bancoDeDados.CriarComando(@"select arl.codigo, c.empreendimento, c.id dominialidade, pr.id, trunc(arl_o.column_value, 2) coordenada
													from {0}crt_dominialidade c, {0}crt_projeto_geo pr, {0}crt_dominialidade_reserva cr, {1}geo_arl arl,
													table(geometria9i.pontoIdeal(arl.geometry).SDO_ORDINATES) arl_o where pr.empreendimento = c.empreendimento
													and c.id = :dominialidade and arl.codigo = :codigo and pr.caracterizacao = 1 and cr.identificacao = arl.codigo
													and cr.dominio in (select id from {0}crt_dominialidade_dominio dm where dm.dominialidade = c.id)
													and arl.projeto = pr.id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("dominialidade", termo.Dominialidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo", DbType.String);

				foreach (var item in termo.RLFormacao)
				{
					comando.SetarValorParametro("codigo", item.Identificacao);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							item.CoordenadaE = reader.GetValue<string>("coordenada");
						}

						if (reader.Read())
						{
							item.CoordenadaN = reader.GetValue<string>("coordenada");
						}

						reader.Close();
					}
				}

				foreach (var item in termo.RLPreservada)
				{
					comando.SetarValorParametro("codigo", item.Identificacao);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							item.CoordenadaE = reader.GetValue<string>("coordenada");
						}

						if (reader.Read())
						{
							item.CoordenadaN = reader.GetValue<string>("coordenada");
						}

						reader.Close();
					}
				}

				#endregion
			}

			return termo;
		}

		#endregion
	}
}