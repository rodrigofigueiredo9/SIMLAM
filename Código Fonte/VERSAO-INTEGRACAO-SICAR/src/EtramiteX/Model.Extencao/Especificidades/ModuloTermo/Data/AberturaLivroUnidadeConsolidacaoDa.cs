using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloTermo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloEspecificidade.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Especificidades.ModuloTermo.Data
{
	public class AberturaLivroUnidadeConsolidacaoDa
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

		public AberturaLivroUnidadeConsolidacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			EsquemaBancoGeo = _config.Obter<string>(ConfiguracaoSistema.KeyUsuarioGeo);

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(AberturaLivroUnidadeConsolidacao termo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cadastro da Especificidade

				eHistoricoAcao acao;
				object id;

				// Verifica a existencia da especificidade
				Comando comando = bancoDeDados.CriarComando(@"select e.id from {0}esp_abertura_livro_uc e where e.titulo = :titulo", EsquemaBanco);
				comando.AdicionarParametroEntrada("titulo", termo.Titulo.Id, DbType.Int32);
				id = bancoDeDados.ExecutarScalar(comando);

				if (id != null && !Convert.IsDBNull(id))
				{
					comando = bancoDeDados.CriarComando(@"update {0}esp_abertura_livro_uc e set e.tid = :tid, e.protocolo = :protocolo, e.total_paginas_livro = :total_paginas_livro, 
														e.pagina_inicial = :pagina_inicial, e.pagina_final = :pagina_final where e.titulo = :titulo", EsquemaBanco);

					acao = eHistoricoAcao.atualizar;
					termo.Id = Convert.ToInt32(id);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}esp_abertura_livro_uc (id, tid, titulo, protocolo, total_paginas_livro, 
														pagina_inicial, pagina_final) values ({0}seq_esp_abertura_livro_uc.nextval, :tid, :titulo, :protocolo,
														:total_paginas_livro, :pagina_inicial, :pagina_final) returning id into :id", EsquemaBanco);

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

				#region Culturas

				comando = bancoDeDados.CriarComando("delete from {0}esp_aber_livro_uc_cultura t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.especificidade = :especificidade {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, termo.Culturas.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in termo.Culturas)
				{
					if (item.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update {0}esp_aber_livro_uc_cultura t set t.cultura = :cultura, t.tid = :tid where t.id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", item.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}esp_aber_livro_uc_cultura (id, tid, especificidade, cultura) 
						values ({0}seq_esp_aber_livro_uc_cultura.nextval, :tid, :especificidade, :cultura)", EsquemaBanco);
						comando.AdicionarParametroEntrada("especificidade", termo.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("cultura", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(termo.Titulo.Id, eHistoricoArtefatoEspecificidade.aberturalivrounidadeconsolidac, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}esp_abertura_livro_uc c set c.tid = :tid where c.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(titulo, eHistoricoArtefatoEspecificidade.aberturalivrounidadeconsolidac, eHistoricoAcao.excluir, bancoDeDados);

				#region Apaga os dados da especificidade

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					delete from {0}esp_aber_livro_uc_cultura e where e.especificidade = (select e.id from {0}esp_abertura_livro_uc e where e.titulo = :titulo);
					delete from {0}esp_abertura_livro_uc e where e.titulo = :titulo;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion Ações de DML

		#region Obter

		internal AberturaLivroUnidadeConsolidacao Obter(int titulo, BancoDeDados banco = null)
		{
			AberturaLivroUnidadeConsolidacao especificidade = new AberturaLivroUnidadeConsolidacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				Comando comando = bancoDeDados.CriarComando(@"select e.id, e.tid, e.titulo, e.protocolo, e.total_paginas_livro, e.pagina_inicial, e.pagina_final, 
															n.numero, n.ano, p.requerimento, p.protocolo protocolo_tipo from {0}esp_abertura_livro_uc e, 
															{0}tab_protocolo p, {0}tab_titulo_numero n where n.titulo(+) = e.titulo and e.protocolo = p.id 
															and e.titulo = :titulo", EsquemaBanco);

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

				#region Culturas

				comando = bancoDeDados.CriarComando(@"
				select e.id IdRelacionamento, c.id Id, c.tid Tid, c.texto Nome
				from esp_abertura_livro_uc t, esp_aber_livro_uc_cultura e, tab_cultura c
				where e.especificidade = t.id and e.cultura = c.id and t.titulo = :titulo", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				especificidade.Culturas = bancoDeDados.ObterEntityList<Cultura>(comando);

				#endregion
			}

			return especificidade;
		}

		internal AberturaLivroUnidadeConsolidacao ObterHistorico(int titulo, int situacao, BancoDeDados banco = null)
		{
			AberturaLivroUnidadeConsolidacao especificidade = new AberturaLivroUnidadeConsolidacao();
			Comando comando = null;
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados da Especificidade

				comando = bancoDeDados.CriarComando(@"
				select e.id, e.especificidade_id, e.tid, p.id_protocolo, e.total_paginas_livro, e.pagina_inicial, e.pagina_final, n.numero, n.ano, p.requerimento_id, p.protocolo_id protocolo_tipo 
				from {0}hst_esp_abertura_livro_uc e, {0}hst_titulo_numero n, {0}hst_protocolo p
				where e.titulo_id = n.titulo_id(+) and e.titulo_tid = n.titulo_tid(+) 
				and e.protocolo_id = p.id_protocolo(+) and e.protocolo_tid = p.tid(+) 
				and not exists (select 1 from lov_historico_artefatos_acoes l where l.id = e.acao_executada and l.acao = 3) 
				and e.titulo_tid = (select ht.tid from hst_titulo ht where ht.titulo_id = e.titulo_id and ht.data_execucao = 
									(select max(htt.data_execucao) from hst_titulo htt where htt.titulo_id = e.titulo_id and htt.situacao_id = :situacao)) and e.titulo_id = :titulo", EsquemaBanco);

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

				#region Culturas

				comando = bancoDeDados.CriarComando(@"
				select e.aber_li_uc_cultura_id IdRelacionamento, c.cultura_id Id, c.tid Tid, c.texto Nome 
				from hst_esp_aber_li_uc_cultura e, hst_cultura c where c.cultura_id = e.cultura_id and c.tid = e.cultura_tid and e.hst_id = :id_hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

				especificidade.Culturas = bancoDeDados.ObterEntityList<Cultura>(comando);

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

				AberturaLivroUnidadeConsolidacao esp = ObterHistorico(titulo, dados.Titulo.SituacaoId, bancoDeDados);
				termo.TotalPaginasLivro = esp.TotalPaginasLivro;
				termo.PaginaInicial = esp.PaginaInicial;
				termo.PaginaFinal = esp.PaginaFinal;

				#endregion

				termo.Destinatario = DaEsp.ObterDadosPessoa(termo.Destinatario.Id, termo.Empreendimento.Id, bancoDeDados);
				
				termo.UnidadeConsolidacao = new UnidadeConsolidacaoPDF();
				termo.UnidadeConsolidacao.ResponsaveisEmpreendimento = DaEsp.ObterEmpreendimentoResponsaveis(termo.Empreendimento.Id.GetValueOrDefault());
			}

			return termo;
		}

		#endregion

		#region Auxiliares

		internal List<Lista> ObterCulturas(int protocoloId, BancoDeDados banco = null)
		{
			List<Lista> culturas = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select distinct(c.id) cultura_id, c.texto cultura_texto from tab_cultura c, 
															crt_unidade_cons_cultivar cc where cc.cultura = c.id and cc.unidade_consolidacao = 
															(select id from crt_unidade_consolidacao where empreendimento = (select empreendimento 
															from tab_protocolo where id = :protocolo))", EsquemaBanco);


				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);


				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Lista cultura;

					while (reader.Read())
					{
						cultura = new Lista();
						cultura.Id = reader.GetValue<String>("cultura_id");
						cultura.Texto = reader.GetValue<String>("cultura_texto");
						cultura.IsAtivo = true;

						culturas.Add(cultura);
					}

					reader.Close();
				}
			}

			return culturas;
		}

		internal List<Cultivar> ObterCultivares(List<int> culturas, int protocoloId, BancoDeDados banco = null)
		{
			List<Cultivar> cultivares = new List<Cultivar>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@" select c.texto cultura, cc.cultivar, crt_c.capacidade_mes, lu.texto unidade_medida_texto
															from {0}crt_unidade_cons_cultivar  crt_c, {0}tab_cultura c, {0}tab_cultura_cultivar cc,
															{0}lov_crt_un_conso_un_medida lu where crt_c.id in (select id from {0}crt_unidade_cons_cultivar
															@) and unidade_consolidacao in (select unidade_consolidacao
															from {0}crt_unidade_cons_cultivar where unidade_consolidacao = (select id 
															from {0}crt_unidade_consolidacao where empreendimento = (select empreendimento from {0}tab_protocolo 
															where id = :protocolo))) and crt_c.cultura = c.id and crt_c.cultivar = cc.id(+)
															and lu.id = crt_c.unidade_medida", EsquemaBanco);

			    comando.DbCommand.CommandText = comando.DbCommand.CommandText.Replace("@", "{0}");

				comando.DbCommand.CommandText = string.Format(comando.DbCommand.CommandText, comando.AdicionarIn("where", "cultura", DbType.Int32, culturas));

				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Cultivar cultivar;

					while (reader.Read())
					{
						cultivar = new Cultivar();
						cultivar.Nome = reader.GetValue<String>("cultivar");
						cultivar.CulturaTexto = reader.GetValue<String>("cultura");
						cultivar.CapacidadeMes = reader.GetValue<Decimal>("capacidade_mes");
						cultivar.UnidadeMedidaTexto = reader.GetValue<String>("unidade_medida_texto");

						cultivares.Add(cultivar);
					}

					reader.Close();
				}
			}

			return cultivares;
		}

		internal bool ExisteTituloParaCulturaMesmoRequerimento(int cultura, int protocolo, int titulo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from esp_abertura_livro_uc e, esp_aber_livro_uc_cultura ec 
				where ec.especificidade = e.id and ec.cultura = :cultura and e.protocolo = :protocolo and e.titulo != :titulo 
				and exists (select 1 from tab_titulo t where t.id = e.titulo and t.situacao != 5 and t.situacao_motivo not in (1, 4))", EsquemaBanco);

				comando.AdicionarParametroEntrada("cultura", cultura, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar<Int32>(comando));
			}
		}

		#endregion
	}
}