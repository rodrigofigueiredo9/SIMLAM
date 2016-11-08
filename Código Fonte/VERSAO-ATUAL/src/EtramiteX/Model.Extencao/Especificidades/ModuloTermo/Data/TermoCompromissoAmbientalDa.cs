using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using System.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Entities.Interno.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data
{
	public class TermoCompromissoAmbientalDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		EspecificidadeDa _daEsp = new EspecificidadeDa();
		GerenciadorConfiguracao<ConfiguracaoSistema> _config = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		internal EspecificidadeDa DaEsp { get { return _daEsp; } }

		private string EsquemaBanco { get; set; }

		private string EsquemaBancoGeo { get; set; }

		#endregion

		public TermoCompromissoAmbientalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioGeo);

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TermoCompromissoAmbiental termo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_termo_compr_amb e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_termo_compr_amb e set e.tid = :tid, e.protocolo = :protocolo, e.licenca = :licenca, 
														e.destinatario = :destinatario, e.representante = :representante, e.descricao = :descricao 
														where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					termo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_termo_compr_amb (id, tid, titulo, protocolo, licenca, destinatario, 
														representante, descricao) values ({0}seq_esp_termo_compr_amb.nextval, :tid, :titulo, :protocolo, 
														:licenca, :destinatario, :representante, :descricao) returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", termo.ProtocoloReq.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("licenca", termo.Licenca, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", termo.Destinatario, DbType.Int32);
				comando.AdicionarParametroEntrada("representante", termo.Representante > 0 ? termo.Representante : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntClob("descricao", termo.Descricao);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					termo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				Historico.Gerar(termo.Titulo.Id, eHistoricoArtefatoEspecificidade.termocompromissoambiental, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_termo_compr_amb c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.termocompromissoambiental, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComando(@"delete from {0}esp_termo_compr_amb e where e.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal TermoCompromissoAmbiental Obter(int titulo, BancoDeDados banco = null)
		{
			TermoCompromissoAmbiental especificidade = new TermoCompromissoAmbiental();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.titulo, e.protocolo, e.licenca licenca_id, tm.sigla licenca_sigla, 
															lic_num.numero licenca_numero, lic_num.ano licenca_ano, e.destinatario, dest.tipo destinatario_tipo,
															nvl(dest.nome, dest.razao_social) destinatario_nome_razao, e.representante, repr.nome representante_nome_razao,
															e.descricao, e.tid, n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo
															from esp_termo_compr_amb e, tab_titulo_numero n, tab_protocolo p, tab_pessoa dest, 
															tab_pessoa repr, tab_titulo_numero lic_num, tab_titulo_modelo tm where n.titulo(+) = e.titulo
															and e.protocolo = p.id and dest.id = e.destinatario and repr.id(+) = e.representante
															and lic_num.titulo = e.licenca and tm.id = lic_num.modelo and e.titulo = :titulo", EsquemaBanco);

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

						if (reader["destinatario"] != null && !Convert.IsDBNull(reader["destinatario"]))
						{
							especificidade.Destinatario = Convert.ToInt32(reader["destinatario"]);
							especificidade.DestinatarioTipo = Convert.ToInt32(reader["destinatario_tipo"]);
							especificidade.DestinatarioNomeRazao = reader["destinatario_nome_razao"].ToString();
						}

						if (reader["representante"] != null && !Convert.IsDBNull(reader["representante"]))
						{
							especificidade.Representante = Convert.ToInt32(reader["representante"]);
							especificidade.RepresentanteNomeRazao = reader["representante_nome_razao"].ToString();
						}

						especificidade.Licenca = reader.GetValue<int>("licenca_id");
						especificidade.LicencaNumero = reader.GetValue<int>("licenca_numero") + "/" + reader.GetValue<int>("licenca_ano") + " - " + reader.GetValue<string>("licenca_sigla");

						especificidade.Descricao = reader.GetValue<string>("descricao");
					}

					reader.Close();
				}

				#endregion
			}

			return especificidade;
		}

		internal TermoCompromissoAmbiental ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			TermoCompromissoAmbiental especificidade = new TermoCompromissoAmbiental();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade


				comando = bancoDeDados.CriarComando(@"select e.id, e.especificidade_id, e.tid, p.id_protocolo, e.licenca_id, e.licenca_tid, lic_n.numero licenca_numero, 
													lic_n.ano licenca_ano, tm.sigla licenca_sigla, e.destinatario_id, e.destinatario_tid, dest.tipo destinatario_tipo, nvl(dest.nome, dest.razao_social) destinatario_nome_razao, 
													e.representante_id, e.representante_tid, repr.nome representante_nome_razao, e.descricao, n.numero, 
													n.ano, p.requerimento_id, p.protocolo_id protocolo_tipo from {0}hst_esp_termo_compr_amb e,
													{0}hst_titulo_numero n, {0}hst_titulo_numero lic_n, {0}hst_titulo_modelo tm, {0}hst_protocolo p, {0}hst_pessoa dest, 
													{0}hst_pessoa repr where e.titulo_id = n.titulo_id(+) and e.licenca_id = lic_n.titulo_id and tm.modelo_id = lic_n.modelo_id and
													tm.tid = lic_n.modelo_tid and e.titulo_tid = n.titulo_tid(+) and e.protocolo_id = p.id_protocolo(+) and e.protocolo_tid = p.tid(+) 
													and dest.pessoa_id = e.destinatario_id and dest.tid = e.destinatario_tid and repr.pessoa_id(+) = e.representante_id 
													and repr.tid(+) = e.representante_tid and not exists (select 1 from lov_historico_artefatos_acoes l where l.id = e.acao_executada 
													and l.acao = 3) and e.titulo_tid = (select ht.tid from hst_titulo ht where ht.titulo_id = e.titulo_id 
													and ht.data_execucao = (select max(htt.data_execucao) from hst_titulo htt where htt.titulo_id = e.titulo_id 
													and htt.situacao_id = :situacao))and e.titulo_id = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", situacao > 0 ? situacao : 1, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						especificidade.Titulo.Id = titulo;
						especificidade.Id = reader.GetValue<int>("especificidade_id");
						especificidade.Tid = reader.GetValue<string>("tid");

						if (reader["destinatario_id"] != null && !Convert.IsDBNull(reader["destinatario_id"]))
						{
							especificidade.Destinatario = Convert.ToInt32(reader["destinatario_id"]);
							especificidade.DestinatarioTipo = Convert.ToInt32(reader["destinatario_tipo"]);
							especificidade.DestinatarioTid = reader.GetValue<string>("destinatario_tid");
							especificidade.DestinatarioNomeRazao = reader["destinatario_nome_razao"].ToString();
						}

						if (reader["representante_id"] != null && !Convert.IsDBNull(reader["representante_id"]))
						{
							especificidade.Representante = Convert.ToInt32(reader["representante_id"]);
							especificidade.RepresentanteNomeRazao = reader["representante_nome_razao"].ToString();
						}

						especificidade.Licenca = reader.GetValue<int>("licenca_id");
						especificidade.LicencaNumero = reader.GetValue<int>("licenca_numero") + "/" + reader.GetValue<int>("licenca_ano") + " - " + reader.GetValue<string>("licenca_sigla");

						especificidade.Descricao = reader.GetValue<string>("descricao");

						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento_id");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("id_protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
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
			string compromitente = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				termo.Titulo = dados.Titulo;
				termo.Protocolo = dados.Protocolo;
				termo.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"
				select e.destinatario, e.descricao, num_lar.numero||'/'||num_lar.ano numero_lar,
				(case when e.representante is not null then (select p.nome from tab_pessoa p where p.id = e.representante)
				else (select p.nome from tab_pessoa p where p.id = e.destinatario) end) compromitente 
				from esp_termo_compr_amb e, tab_titulo_numero num_lar  where e.titulo = :titulo and num_lar.titulo = e.licenca", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						termo.Destinatario.Id = reader.GetValue<int>("destinatario");
						termo.NumeroLAR = reader.GetValue<string>("numero_lar");
						termo.Descricao = reader.GetValue<string>("descricao");
						compromitente = reader.GetValue<string>("compromitente");
					}

					reader.Close();
				}

				#endregion

				termo.Destinatario = DaEsp.ObterDadosPessoa(termo.Destinatario.Id, termo.Empreendimento.Id, bancoDeDados);
				termo.Titulo.AssinanteSource.Add(new AssinanteDefault { Cargo = "Compromitente", Nome = compromitente });
			}

			return termo;
		}

		#region Auxiliares

		internal Titulo ObterTitulo(int tituloId, BancoDeDados banco = null)
		{
			Titulo titulo = new Titulo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tm.codigo modelo_codigo, t.situacao situacao_id from {0}tab_titulo t, 
															{0}tab_titulo_modelo tm where tm.id = t.modelo and t.id = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						titulo.Id = tituloId;
						titulo.Modelo.Codigo = reader.GetValue<Int32>("modelo_codigo");
						titulo.Situacao.Id = reader.GetValue<Int32>("situacao_id");
					}

					reader.Close();
				}
			}

			return titulo;
		}

		internal Pessoa ObterPessoa(int pessoaId, BancoDeDados banco = null)
		{
			Pessoa pessoa = new Pessoa();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select tipo from tab_pessoa where id = :pessoa", EsquemaBanco);
				comando.AdicionarParametroEntrada("pessoa", pessoaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						pessoa.Id = pessoaId;
						pessoa.Tipo = reader.GetValue<int>("tipo");
					}

					reader.Close();
				}
			}

			return pessoa;
		}

		internal List<PessoaLst> ObterRepresentantes(int destinatarioId, string destinatarioTid = null, BancoDeDados banco = null)
		{
			List<PessoaLst> representantes = new List<PessoaLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				if (destinatarioTid != null)
				{
					comando = bancoDeDados.CriarComando(@"select pessoa_id id, nome from hst_pessoa where pessoa_id in (select representante_id
														from hst_pessoa_representante where pessoa_id = :destinatario_id and pessoa_tid = :destinatario_tid)
														and tid in (select representante_tid from hst_pessoa_representante where pessoa_id = :destinatario_id 
														and pessoa_tid = :destinatario_tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("destinatario_tid", destinatarioTid, DbType.String);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select id, nome from {0}tab_pessoa where id in (select representante from {0}tab_pessoa_representante 
														where pessoa = :destinatario_id)", EsquemaBanco);

				}

				comando.AdicionarParametroEntrada("destinatario_id", destinatarioId, DbType.Int32);


				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst representante;

					while (reader.Read())
					{
						representante = new PessoaLst();
						representante.Id = reader.GetValue<Int32>("id");
						representante.Texto = reader.GetValue<String>("nome");
						representante.IsAtivo = true;

						representantes.Add(representante);
					}

					reader.Close();
				}
			}

			return representantes;
		}

		internal int ObterProtocolo(int tituloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.protocolo from {0}tab_titulo t where t.id = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", tituloId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<Int32>(comando);
			}
		}

		internal int ObterDestinatarioTipo(int destinatarioId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.tipo from {0}tab_pessoa t where t.id = :destinatario", EsquemaBanco);
				comando.AdicionarParametroEntrada("destinatario", destinatarioId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<Int32>(comando);
			}
		}

		internal int ObterEmpreendimentoPorProtocolo(int protocoloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl(empreendimento, 0) from tab_protocolo where id = :protocolo", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<Int32>(comando);
			}
		}

		#endregion

		#endregion
	}
}