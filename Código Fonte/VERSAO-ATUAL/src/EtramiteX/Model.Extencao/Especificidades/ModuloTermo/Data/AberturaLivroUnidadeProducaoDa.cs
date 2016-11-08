using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;
using System.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Business;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data
{
	public class AberturaLivroUnidadeProducaoDa
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

		public AberturaLivroUnidadeProducaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioGeo);

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(AberturaLivroUnidadeProducao termo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_abertura_livro_up e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"
					update {0}esp_abertura_livro_up e set e.tid = :tid, e.protocolo = :protocolo, 
					e.total_paginas_livro = :total_paginas_livro, e.pagina_inicial = :pagina_inicial, e.pagina_final = :pagina_final 
					where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					termo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}esp_abertura_livro_up (id, tid, titulo, protocolo, total_paginas_livro, pagina_inicial, pagina_final) 
					values ({0}seq_esp_abertura_livro_up.nextval, :tid, :titulo, :protocolo, :total_paginas_livro, :pagina_inicial, :pagina_final) 
					returning id into :id", EsquemaBanco);

					acao = eHistoricoAcao.criar;
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}

				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", termo.Titulo.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("total_paginas_livro", termo.TotalPaginasLivro, DbType.Int32);
				comando.AdicionarParametroEntrada("pagina_inicial", termo.PaginaInicial, DbType.Int32);
				comando.AdicionarParametroEntrada("pagina_final", termo.PaginaFinal, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (id == null || Convert.IsDBNull(id))
				{
					termo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Unidades

				comando = bancoDeDados.CriarComando("delete from {0}esp_aber_livro_up_unid t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, termo.Unidades.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in termo.Unidades)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_aber_livro_up_unid t set t.unidade = :unidade, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}esp_aber_livro_up_unid (id, especificidade, unidade, tid) 
						values ({0}seq_esp_aber_livro_up_unid.nextval, :especificidade, :unidade, :tid)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("unidade", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(termo.Titulo.Id, eHistoricoArtefatoEspecificidade.aberturalivrounidadeproducao, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComandoPlSql(
				@"begin 
					update {0}esp_abertura_livro_up e set e.tid = :tid where e.titulo = :titulo;
					update {0}esp_aber_livro_up_unid u set u.tid = :tid where u.especificidade = (select e.id from esp_abertura_livro_up e where e.titulo = :titulo);
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.aberturalivrounidadeproducao, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					delete from {0}esp_aber_livro_up_unid u where u.especificidade = (select e.id from {0}esp_abertura_livro_up e where e.titulo = :titulo);
					delete from {0}esp_abertura_livro_up e where e.titulo = :titulo;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal AberturaLivroUnidadeProducao Obter(int titulo, BancoDeDados banco = null)
		{
			AberturaLivroUnidadeProducao especificidade = new AberturaLivroUnidadeProducao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.titulo, e.protocolo, e.total_paginas_livro, e.pagina_inicial, e.pagina_final, 
															up.id unidade_producao_unid_id, up.tid unidade_producao_unid_tid, up.codigo_up unidade_producao_unid_codigo, 
															n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo from {0}esp_abertura_livro_up e, {0}tab_protocolo p, 
															{0}tab_titulo_numero n, {0}tab_pessoa dest, {0}crt_unidade_producao_unidade up where n.titulo(+) = e.titulo 
															and e.protocolo = p.id and up.id(+) = e.unidade_producao_unid and e.titulo = :titulo", EsquemaBanco);

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
						especificidade.TotalPaginasLivro = reader.GetValue<string>("total_paginas_livro");
						especificidade.PaginaInicial = reader.GetValue<string>("pagina_inicial");
						especificidade.PaginaFinal = reader.GetValue<string>("pagina_final");
					}

					reader.Close();
				}

				#endregion

				#region Unidades

				comando = bancoDeDados.CriarComando(@"
				select u.id Id, e.id IdRelacionamento, u.codigo_up CodigoUP, c.texto CulturaTexto, cc.cultivar CultivarTexto, u.estimativa_quant_ano 
				from esp_abertura_livro_up t, esp_aber_livro_up_unid e, crt_unidade_producao_unidade u, tab_cultura c, tab_cultura_cultivar cc 
				where e.especificidade = t.id and e.unidade = u.id and u.cultura = c.id and u.cultivar = cc.id(+) and t.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.Unidades = bancoDeDados.ObterEntityList<UnidadeProducaoItem>(comando, (IDataReader reader, UnidadeProducaoItem item) =>
				{
					item.EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano");
				});
				#endregion
			}

			return especificidade;
		}

		internal AberturaLivroUnidadeProducao ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			AberturaLivroUnidadeProducao especificidade = new AberturaLivroUnidadeProducao();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				comando = bancoDeDados.CriarComando(@"
				select e.id, e.especificidade_id, e.tid, p.id_protocolo, e.total_paginas_livro,
				e.pagina_inicial, e.pagina_final, n.numero, n.ano, p.requerimento_id, p.protocolo_id protocolo_tipo 
				from {0}hst_esp_abertura_livro_up e, {0}hst_titulo_numero n, {0}hst_protocolo p
				where e.titulo_id = n.titulo_id(+) and e.titulo_tid = n.titulo_tid(+)
				and e.protocolo_id = p.id_protocolo(+) and e.protocolo_tid = p.tid(+) and not exists (select 1 from lov_historico_artefatos_acoes l 
				where l.id = e.acao_executada and l.acao = 3) and e.titulo_tid = (select ht.tid from hst_titulo ht 
				where ht.titulo_id = e.titulo_id and ht.data_execucao = (select max(htt.data_execucao) from hst_titulo htt 
				where htt.titulo_id = e.titulo_id and htt.situacao_id = :situacao)) and e.titulo_id = :titulo", EsquemaBanco);

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

						especificidade.TotalPaginasLivro = reader.GetValue<string>("total_paginas_livro");
						especificidade.PaginaInicial = reader.GetValue<string>("pagina_inicial");
						especificidade.PaginaFinal = reader.GetValue<string>("pagina_final");

						especificidade.ProtocoloReq.IsProcesso = reader.GetValue<int>("protocolo_tipo") == 1;
						especificidade.ProtocoloReq.RequerimentoId = reader.GetValue<int>("requerimento_id");
						especificidade.ProtocoloReq.Id = reader.GetValue<int>("id_protocolo");
						especificidade.Titulo.Numero.Inteiro = reader.GetValue<int>("numero");
						especificidade.Titulo.Numero.Ano = reader.GetValue<int>("ano");
					}

					reader.Close();
				}

				#endregion

				#region Unidades

				comando = bancoDeDados.CriarComando(@"
				select u.unidade_producao_unidade_id Id, e.aber_livro_up_unid_id IdRelacionamento, u.codigo_up CodigoUP, 
				c.texto CulturaTexto, cc.cultivar_nome CultivarTexto, u.estimativa_quant_ano 
				from hst_esp_aber_livro_up_unid e, hst_crt_unidade_prod_unidade u, hst_cultura c, hst_cultura_cultivar cc 
				where e.unidade_id = u.unidade_producao_unidade_id and e.unidade_tid = u.tid and u.cultura_id = c.cultura_id 
				and u.cultura_tid = c.tid and u.cultivar_id = cc.cultivar_id(+) and u.cultivar_tid = cc.tid(+) and e.id_hst = :id_hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

				especificidade.Unidades = bancoDeDados.ObterEntityList<UnidadeProducaoItem>(comando, (IDataReader reader, UnidadeProducaoItem item) =>
				{
					item.EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano");
				});
				#endregion
			}

			return especificidade;
		}

		internal Termo ObterDadosPDF(int titulo, BancoDeDados banco = null)
		{
			Termo termo = new Termo();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Título

				DadosPDF dados = DaEsp.ObterDadosTitulo(titulo, bancoDeDados);
				termo.Titulo = dados.Titulo;
				termo.Titulo.SetorEndereco = DaEsp.ObterEndSetor(termo.Titulo.SetorId);
				termo.Protocolo = dados.Protocolo;
				termo.Empreendimento = dados.Empreendimento;

				#endregion

				#region Dados da Especificidade

				AberturaLivroUnidadeProducao esp = ObterHistorico(titulo, dados.Titulo.SituacaoId, bancoDeDados);
				termo.TotalPaginasLivro = esp.TotalPaginasLivro;
				termo.PaginaInicial = esp.PaginaInicial;
				termo.PaginaFinal = esp.PaginaFinal;

				foreach (var item in esp.Unidades)
				{
					termo.UnidadeProducao.Unidades.Add(new UnidadeProducaoItemPDF(item));
				}

				#endregion

				termo.Destinatario = DaEsp.ObterDadosPessoa(termo.Destinatario.Id, termo.Empreendimento.Id, bancoDeDados);
			}

			return termo;
		}

		#endregion

		#region Auxiliares

		internal List<Lista> ObterUnidadesProducoes(int protocoloId, BancoDeDados banco = null)
		{
			List<Lista> unidades = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id, codigo_up nome from {0}crt_unidade_producao_unidade 
															where unidade_producao = (select id from {0}crt_unidade_producao 
															where empreendimento = (select empreendimento from {0}tab_protocolo 
															where id = :protocolo))", EsquemaBanco);


				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);


				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista unidade;

					while (reader.Read())
					{
						unidade = new Lista();
						unidade.Id = reader.GetValue<String>("id");
						unidade.Texto = reader.GetValue<String>("nome");
						unidade.IsAtivo = true;

						unidades.Add(unidade);
					}

					reader.Close();
				}
			}

			return unidades;
		}

		internal bool ExisteUnidadeProducao(int unidadeId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}crt_unidade_producao_unidade t where t.id = :unidade", EsquemaBanco);
				comando.AdicionarParametroEntrada("unidade", unidadeId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar<Int32>(comando));
			}
		}

		internal String ObterTituloAssociado(int unidadeID, int tituloID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select tn.numero || '/' || tn.ano titulo_numero
				from tab_titulo t, tab_titulo_numero tn, esp_abertura_livro_up e
				where t.situacao != 5
				and e.titulo = t.id
				and tn.titulo = t.id
				and e.id in (select u.especificidade from esp_aber_livro_up_unid u where u.unidade = :unidade)
				and t.id != :titulo
				and rownum = 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("unidade", unidadeID, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", tituloID, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);

			}
		}

		#endregion
	}
}