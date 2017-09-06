using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Data
{
	public class UnidadeConsolidacaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		private String EsquemaBanco { get; set; }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public UnidadeConsolidacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações DML

		internal void Salvar(UnidadeConsolidacao unidadeConsolidacao, BancoDeDados banco = null)
		{
			if (unidadeConsolidacao == null)
			{
				throw new Exception("A Caracterização é nula.");
			}

			if (unidadeConsolidacao.Id <= 0)
			{
				Criar(unidadeConsolidacao, banco);
			}
			else
			{
				Editar(unidadeConsolidacao, banco);
			}
		}

		private void Criar(UnidadeConsolidacao unidadeConsolidacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}crt_unidade_consolidacao (id, empreendimento, possui_codigo_uc, codigo_uc, local_livro_disponivel, tipo_apresentacao_produto, tid) values 
				(seq_crt_unidade_consolidacao.nextval, :empreendimento,:possui_codigo_uc, :codigo_uc, :local_livro_disponivel, :tipo_apresentacao_produto, :tid) returning id into :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("possui_codigo_uc", unidadeConsolidacao.PossuiCodigoUC, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo_uc", unidadeConsolidacao.CodigoUC, DbType.Int64);
				comando.AdicionarParametroEntrada("empreendimento", unidadeConsolidacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("local_livro_disponivel", DbType.String, 100, unidadeConsolidacao.LocalLivroDisponivel);
				comando.AdicionarParametroEntrada("tipo_apresentacao_produto", DbType.String, 250, unidadeConsolidacao.TipoApresentacaoProducaoFormaIdentificacao);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				unidadeConsolidacao.Id = comando.ObterValorParametro<int>("id");

				#region Cultivares

				comando = bancoDeDados.CriarComando(@"
				insert into crt_unidade_cons_cultivar (id, unidade_consolidacao, cultivar, capacidade_mes, unidade_medida, tid, cultura) values 
				(seq_crt_unidade_cons_cultivar.nextval, :unidade_consolidacao, :cultivar, :capacidade_mes, :unidade_medida, :tid, :cultura)", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade_consolidacao", unidadeConsolidacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
				comando.AdicionarParametroEntrada("capacidade_mes", DbType.Decimal);
				comando.AdicionarParametroEntrada("unidade_medida", DbType.Int32);
				comando.AdicionarParametroEntrada("cultura", DbType.Int32);

				unidadeConsolidacao.Cultivares.ForEach(x =>
				{
					comando.SetarValorParametro("cultivar", x.Id < 1 ? (object)DBNull.Value : x.Id);
					comando.SetarValorParametro("capacidade_mes", x.CapacidadeMes);
					comando.SetarValorParametro("unidade_medida", x.UnidadeMedida);
					comando.SetarValorParametro("cultura", x.CulturaId);

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Responsáveis Técnicos

				comando = bancoDeDados.CriarComando(@"
				insert into crt_unida_conso_resp_tec (id, unidade_consolidacao, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art, tid) values 
				(seq_crt_unida_conso_resp_tec.nextval, :unidade_consolidacao, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art, :tid)", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade_consolidacao", unidadeConsolidacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("responsavel_tecnico", DbType.Int32);
				comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", DbType.String);
				comando.AdicionarParametroEntrada("numero_art", DbType.String);
				comando.AdicionarParametroEntrada("art_cargo_funcao", DbType.Int32);
				comando.AdicionarParametroEntrada("data_validade_art", DbType.String);

				unidadeConsolidacao.ResponsaveisTecnicos.ForEach(x =>
				{
					comando.SetarValorParametro("responsavel_tecnico", x.Id);
					comando.SetarValorParametro("numero_hab_cfo_cfoc", x.CFONumero);
					comando.SetarValorParametro("numero_art", x.NumeroArt);
					comando.SetarValorParametro("art_cargo_funcao", x.ArtCargoFuncao);
					comando.SetarValorParametro("data_validade_art", x.DataValidadeART);

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				Historico.Gerar(unidadeConsolidacao.Id, eHistoricoArtefatoCaracterizacao.unidadeconsolidacao, eHistoricoAcao.criar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		private void Editar(UnidadeConsolidacao unidadeConsolidacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_unidade_consolidacao set codigo_uc = :codigo_uc, 
				local_livro_disponivel = :local_livro_disponivel, tipo_apresentacao_produto = :tipo_apresentacao_produto, tid = :tid 
				where id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", unidadeConsolidacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo_uc", unidadeConsolidacao.CodigoUC, DbType.Int64);
				comando.AdicionarParametroEntrada("local_livro_disponivel", DbType.String, 100, unidadeConsolidacao.LocalLivroDisponivel);
				comando.AdicionarParametroEntrada("tipo_apresentacao_produto", DbType.String, 250, unidadeConsolidacao.TipoApresentacaoProducaoFormaIdentificacao);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#region Apagando dados

				//Responsáveis técnicos
				comando = bancoDeDados.CriarComando(@"delete from crt_unida_conso_resp_tec ", EsquemaCredenciadoBanco);
				comando.DbCommand.CommandText += String.Format("where unidade_consolidacao = :unidade_consolidacao {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, unidadeConsolidacao.ResponsaveisTecnicos.Select(x => x.IdRelacionamento).ToList()));

				comando.AdicionarParametroEntrada("unidade_consolidacao", unidadeConsolidacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Cultivares
				comando = bancoDeDados.CriarComando(@"delete from crt_unidade_cons_cultivar c", EsquemaCredenciadoBanco);
				comando.DbCommand.CommandText += String.Format(" where c.unidade_consolidacao = :unidade_consolidacao {0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, unidadeConsolidacao.Cultivares.Select(x => x.IdRelacionamento).ToList()));

				comando.AdicionarParametroEntrada("unidade_consolidacao", unidadeConsolidacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Cultivares

				unidadeConsolidacao.Cultivares.ForEach(x =>
				{
					if (x.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update crt_unidade_cons_cultivar set cultivar = :cultivar, capacidade_mes = :capacidade_mes, cultura =:cultura,
						unidade_medida = :unidade_medida, tid = :tid where id = :id_rel", EsquemaCredenciadoBanco);
						comando.AdicionarParametroEntrada("id_rel", x.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_unidade_cons_cultivar (id, unidade_consolidacao, cultivar, capacidade_mes, unidade_medida, tid, cultura) values 
						(seq_crt_unidade_cons_cultivar.nextval, :unidade_consolidacao, :cultivar, :capacidade_mes, :unidade_medida, :tid, :cultura)", EsquemaCredenciadoBanco);
						comando.AdicionarParametroEntrada("unidade_consolidacao", unidadeConsolidacao.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("cultivar", x.Id < 1 ? (object)DBNull.Value : x.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("capacidade_mes", x.CapacidadeMes, DbType.Decimal);
					comando.AdicionarParametroEntrada("unidade_medida", x.UnidadeMedida, DbType.Int32);
					comando.AdicionarParametroEntrada("cultura", x.CulturaId, DbType.Int32);

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Responsáveis Técnicos

				unidadeConsolidacao.ResponsaveisTecnicos.ForEach(x =>
				{
					if (x.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update crt_unida_conso_resp_tec set responsavel_tecnico =:responsavel_tecnico, numero_hab_cfo_cfoc =:numero_hab_cfo_cfoc, 
						numero_art = :numero_art, art_cargo_funcao = :art_cargo_funcao, data_validade_art = :data_validade_art, tid = :tid
						where id = :id_rel", EsquemaCredenciadoBanco);
						comando.AdicionarParametroEntrada("id_rel", x.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_unida_conso_resp_tec 
						(id, unidade_consolidacao, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art, tid) values 
						(seq_crt_unida_conso_resp_tec.nextval, :unidade_consolidacao, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art, :tid)", EsquemaCredenciadoBanco);
						comando.AdicionarParametroEntrada("unidade_consolidacao", unidadeConsolidacao.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("responsavel_tecnico", x.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", x.CFONumero, DbType.String);
					comando.AdicionarParametroEntrada("numero_art", x.NumeroArt, DbType.String);
					comando.AdicionarParametroEntrada("art_cargo_funcao", x.ArtCargoFuncao, DbType.Int32);
					comando.AdicionarParametroEntrada("data_validade_art", x.DataValidadeART, DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				Historico.Gerar(unidadeConsolidacao.Id, eHistoricoArtefatoCaracterizacao.unidadeconsolidacao, eHistoricoAcao.atualizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_unidade_consolidacao c where c.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				int id = 0;
				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					id = Convert.ToInt32(retorno);
				}

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}crt_unidade_consolidacao c set c.tid = :tid where c.id = :id", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.unidadeconsolidacao, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin 
					delete from {0}crt_unida_conso_resp_tec where unidade_consolidacao = :id;
					delete from {0}crt_unidade_cons_cultivar where unidade_consolidacao = :id;
					delete from {0}crt_unidade_consolidacao where id = :id;
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void CopiarDadosInstitucional(UnidadeConsolidacao caracterizacao, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				if (caracterizacao.Id <= 0)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_unidade_consolidacao (id, empreendimento, possui_codigo_uc, codigo_uc, local_livro_disponivel, 
					tipo_apresentacao_produto, tid, interno_id, interno_tid) values (seq_crt_unidade_consolidacao.nextval, :empreendimento, 
					:possui_codigo_uc, :codigo_uc, :local_livro_disponivel, :tipo_apresentacao_produto, :tid, :interno_id, :interno_tid) 
					returning id into :id", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("empreendimento", caracterizacao.Empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
					update {0}crt_unidade_consolidacao set possui_codigo_uc = :possui_codigo_uc, codigo_uc = :codigo_uc, 
					local_livro_disponivel = :local_livro_disponivel, tipo_apresentacao_produto = :tipo_apresentacao_produto, 
					tid = :tid, interno_id = :interno_id, interno_tid = :interno_tid where id = :id", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("codigo_uc", caracterizacao.CodigoUC, DbType.Int64);
				comando.AdicionarParametroEntrada("possui_codigo_uc", caracterizacao.PossuiCodigoUC, DbType.Int32);
				comando.AdicionarParametroEntrada("local_livro_disponivel", DbType.String, 100, caracterizacao.LocalLivroDisponivel);
				comando.AdicionarParametroEntrada("tipo_apresentacao_produto", DbType.String, 250, caracterizacao.TipoApresentacaoProducaoFormaIdentificacao);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoId, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", caracterizacao.InternoTid, DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);

				if (caracterizacao.Id <= 0)
				{
					caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#region Apagando dados

				//Responsáveis técnicos
				comando = bancoDeDados.CriarComando(@"delete from crt_unida_conso_resp_tec where unidade_consolidacao = :unidade_consolidacao", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("unidade_consolidacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Cultivares
				comando = bancoDeDados.CriarComando(@"delete from crt_unidade_cons_cultivar c where c.unidade_consolidacao = :unidade_consolidacao", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("unidade_consolidacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Cultivares

				caracterizacao.Cultivares.ForEach(x =>
				{
					comando = bancoDeDados.CriarComando(@"
					insert into crt_unidade_cons_cultivar (id, unidade_consolidacao, cultivar, capacidade_mes, unidade_medida, tid, cultura) values 
					(seq_crt_unidade_cons_cultivar.nextval, :unidade_consolidacao, :cultivar, :capacidade_mes, :unidade_medida, :tid, :cultura)", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("unidade_consolidacao", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("cultivar", x.Id < 1 ? (object)DBNull.Value : x.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("capacidade_mes", x.CapacidadeMes, DbType.Decimal);
					comando.AdicionarParametroEntrada("unidade_medida", x.UnidadeMedida, DbType.Int32);
					comando.AdicionarParametroEntrada("cultura", x.CulturaId, DbType.Int32);

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Responsáveis Técnicos

				caracterizacao.ResponsaveisTecnicos.ForEach(x =>
				{
					comando = bancoDeDados.CriarComando(@"
					insert into crt_unida_conso_resp_tec 
					(id, unidade_consolidacao, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art, tid) values 
					(seq_crt_unida_conso_resp_tec.nextval, :unidade_consolidacao, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art, :tid)", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("unidade_consolidacao", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("responsavel_tecnico", x.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", x.CFONumero, DbType.String);
					comando.AdicionarParametroEntrada("numero_art", x.NumeroArt, DbType.String);
					comando.AdicionarParametroEntrada("art_cargo_funcao", x.ArtCargoFuncao, DbType.Int32);
					comando.AdicionarParametroEntrada("data_validade_art", x.DataValidadeART, DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeconsolidacao, eHistoricoAcao.copiar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void AtualizarInternoIdTid(int caracterizacaoID, int caracterizacaoInternoID, string caracterizacaoInternoTID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualização do TID

				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					update crt_unidade_consolidacao c set c.tid = :tid, c.interno_id = :interno_id, c.interno_tid = :interno_tid where c.id = :caracterizacao_id;
					update crt_unidade_cons_cultivar c set c.tid = :tid where c.unidade_consolidacao = :caracterizacao_id;
					update crt_unida_conso_resp_tec c set c.tid = :tid where c.unidade_consolidacao = :caracterizacao_id;
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("caracterizacao_id", caracterizacaoID, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("interno_id", caracterizacaoInternoID, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacaoInternoTID);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(caracterizacaoID, eHistoricoArtefatoCaracterizacao.unidadeconsolidacao, eHistoricoAcao.atualizaridtid, bancoDeDados);

				bancoDeDados.Commit();

			}
		}

		#endregion

		#region Obter / Filtrar

		internal UnidadeConsolidacao ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeConsolidacao unidade = new UnidadeConsolidacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}crt_unidade_consolidacao where empreendimento = :empreendimento", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						unidade = new UnidadeConsolidacao();
						unidade.Id = reader.GetValue<int>("id");
					}

					reader.Close();
				}

				if (unidade != null)
				{
					return Obter(unidade.Id, simplificado, bancoDeDados);
				}
			}

			return unidade;
		}

		public UnidadeConsolidacao Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeConsolidacao unidade = new UnidadeConsolidacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select id, tid, empreendimento, possui_codigo_uc, codigo_uc, local_livro_disponivel, tipo_apresentacao_produto, 
				interno_id, interno_tid from {0}crt_unidade_consolidacao where id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						unidade.Id = id;
						unidade.CredenciadoID = id;
						unidade.Tid = reader.GetValue<string>("tid");
						unidade.InternoId = reader.GetValue<int>("interno_id");
						unidade.InternoTid = reader.GetValue<string>("interno_tid");

						unidade.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						unidade.PossuiCodigoUC = reader.GetValue<bool>("possui_codigo_uc");
						unidade.CodigoUC = reader.GetValue<long>("codigo_uc");
						unidade.LocalLivroDisponivel = reader.GetValue<string>("local_livro_disponivel");
						unidade.TipoApresentacaoProducaoFormaIdentificacao = reader.GetValue<string>("tipo_apresentacao_produto");
						unidade.InternoId = reader.GetValue<int>("interno_id");
						unidade.InternoTid = reader.GetValue<string>("interno_tid");
					}

					reader.Close();
				}

				if (unidade.Id < 1 || simplificado)
				{
					return unidade;
				}

				#region Cultivares

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.unidade_consolidacao, c.capacidade_mes, c.unidade_medida, lu.texto unidade_medida_texto, c.cultivar, cc.cultivar cultivar_nome, c.cultura,
				tc.texto cultura_texto from crt_unidade_cons_cultivar c, tab_cultura tc, tab_cultura_cultivar cc, lov_crt_un_conso_un_medida lu where tc.id = c.cultura 
				and cc.id(+) = c.cultivar and lu.id = c.unidade_medida and c.unidade_consolidacao = :unidade", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade", id, DbType.Int32);

				Cultivar cultivar = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						cultivar = new Cultivar();
						cultivar.CapacidadeMes = reader.GetValue<decimal>("capacidade_mes");
						cultivar.Id = reader.GetValue<int>("cultivar");
						cultivar.IdRelacionamento = reader.GetValue<int>("id");
						cultivar.Nome = reader.GetValue<string>("cultivar_nome");
						cultivar.Tid = reader.GetValue<string>("tid");
						cultivar.UnidadeMedida = reader.GetValue<int>("unidade_medida");
						cultivar.UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto");
						cultivar.CulturaId = reader.GetValue<int>("cultura");
						cultivar.CulturaTexto = reader.GetValue<string>("cultura_texto");

						unidade.Cultivares.Add(cultivar);
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis Técnicos

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.unidade_consolidacao, c.responsavel_tecnico, c.numero_hab_cfo_cfoc, c.numero_art, c.art_cargo_funcao, c.data_validade_art, c.tid, 
				nvl(tp.nome, tp.razao_social) nome_razao, nvl(tp.cpf, tp.cnpj) cpf_cnpj, pf.texto profissao, oc.orgao_sigla, pp.registro, 
				(select h.extensao_habilitacao from tab_hab_emi_cfo_cfoc h where h.responsavel = c.responsavel_tecnico) extensao_habilitacao 
				from {0}crt_unida_conso_resp_tec c, {0}tab_credenciado tc, {1}tab_pessoa tp, {1}tab_pessoa_profissao pp, {0}tab_profissao pf, {0}tab_orgao_classe oc where tc.id = c.responsavel_tecnico 
				and tp.id = tc.pessoa and pp.pessoa(+) = tp.id and pf.id(+) = pp.profissao and oc.id(+) = pp.orgao_classe 
				and c.unidade_consolidacao = :unidade", EsquemaBanco, EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade", id, DbType.Int32);

				ResponsavelTecnico responsavel = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnico();
						responsavel.IdRelacionamento = reader.GetValue<int>("id");
						responsavel.Id = reader.GetValue<int>("responsavel_tecnico");
						responsavel.NomeRazao = reader.GetValue<string>("nome_razao");
						responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
						responsavel.CFONumero = reader.GetValue<string>("numero_hab_cfo_cfoc");
						responsavel.NumeroArt = reader.GetValue<string>("numero_art");
						responsavel.ArtCargoFuncao = reader.GetValue<bool>("art_cargo_funcao");

						responsavel.ProfissaoTexto = reader.GetValue<string>("profissao");
						responsavel.OrgaoClasseSigla = reader.GetValue<string>("orgao_sigla");
						responsavel.NumeroRegistro = reader.GetValue<string>("registro");

						responsavel.DataValidadeART = reader.GetValue<string>("data_validade_art");
						if (!string.IsNullOrEmpty(responsavel.DataValidadeART))
						{
							responsavel.DataValidadeART = Convert.ToDateTime(responsavel.DataValidadeART).ToShortDateString();
						}

						if (Convert.ToBoolean(reader.GetValue<int>("extensao_habilitacao")))
						{
							responsavel.CFONumero += "-ES";
						}

						unidade.ResponsaveisTecnicos.Add(responsavel);
					}

					reader.Close();
				}

				#endregion
			}

			return unidade;
		}

		public UnidadeConsolidacao ObterHistorico(int id, string tid, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeConsolidacao caracterizacao = new UnidadeConsolidacao();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento_id, c.possui_codigo_uc, c.codigo_uc, c.local_livro_disponivel, 
				c.tipo_apresentacao_produto from {0}hst_crt_unidade_consolidacao c where c.unidade_consolidacao = :id and c.tid = :tid", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						caracterizacao.Id = id;
						caracterizacao.CredenciadoID = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.InternoId = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTid = reader.GetValue<string>("interno_tid");

						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento_id");
						caracterizacao.PossuiCodigoUC = reader.GetValue<bool>("possui_codigo_uc");
						caracterizacao.CodigoUC = reader.GetValue<long>("codigo_uc");
						caracterizacao.LocalLivroDisponivel = reader.GetValue<string>("local_livro_disponivel");
						caracterizacao.TipoApresentacaoProducaoFormaIdentificacao = reader.GetValue<string>("tipo_apresentacao_produto");
					}

					reader.Close();
				}

				if (caracterizacao.Id < 1 || simplificado)
				{
					return caracterizacao;
				}

				#region Cultivares

				comando = bancoDeDados.CriarComando(@"
				select c.unid_consoli_cultivar_id id, c.tid, c.cultura_id, cu.texto cultura_texto, c.cultivar_id, cc.cultivar_nome, c.capacidade_mes, c.unidade_medida_id, c.unidade_medida_texto 
				from hst_crt_unidade_cons_cultivar c, hst_cultura cu, hst_cultura_cultivar cc where cu.cultura_id = c.cultura_id and cu.tid = c.cultura_tid and cc.cultivar_id(+) = c.cultivar_id 
				and cc.tid(+) = c.cultivar_tid and c.id_hst = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				Cultivar cultivar = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						cultivar = new Cultivar();

						cultivar.IdRelacionamento = reader.GetValue<int>("id");
						cultivar.Tid = reader.GetValue<string>("tid");
						cultivar.CulturaId = reader.GetValue<int>("cultura_id");
						cultivar.CulturaTexto = reader.GetValue<string>("cultura_texto");
						cultivar.Id = reader.GetValue<int>("cultivar_id");
						cultivar.Nome = reader.GetValue<string>("cultivar_nome");
						cultivar.CapacidadeMes = reader.GetValue<decimal>("capacidade_mes");
						cultivar.UnidadeMedida = reader.GetValue<int>("unidade_medida_id");
						cultivar.UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto");

						caracterizacao.Cultivares.Add(cultivar);
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis Técnicos

				comando = bancoDeDados.CriarComando(@"
				select distinct 
					c.unid_consoli_resp_tec_id id,
					c.responsavel_tecnico_id,
					c.numero_hab_cfo_cfoc,
					c.numero_art,
					c.art_cargo_funcao,
					c.data_validade_art,
					c.tid,
					nvl(tp.nome, tp.razao_social) nome_razao,
					nvl(tp.cpf, tp.cnpj) cpf_cnpj,
					pf.texto profissao,
					oc.orgao_sigla,
					pp.registro,
					(select h.extensao_habilitacao
						from hst_hab_emi_cfo_cfoc h
						where h.responsavel_id = c.responsavel_tecnico_id
						and h.responsavel_tid = c.responsavel_tecnico_tid
						and h.data_execucao =
							(select max(hh.data_execucao)
							from hst_hab_emi_cfo_cfoc hh
							where hh.responsavel_id = h.responsavel_id
							and hh.responsavel_tid = h.responsavel_tid
							and hh.data_execucao <=
								(select hc.data_execucao
									from hst_crt_unidade_consolidacao hc
									where hc.id = c.id_hst))) extensao_habilitacao
				from hst_crt_unida_conso_resp_tec c,
					hst_credenciado               tc,
					hst_pessoa                    tp,
					hst_pessoa_profissao          pp,
					hst_profissao                 pf,
					tab_orgao_classe              oc
				where tc.credenciado_id = c.responsavel_tecnico_id
				and tc.tid = c.responsavel_tecnico_tid
				and tp.pessoa_id = tc.pessoa_id
				and tp.tid = tc.pessoa_tid
				and pp.id_hst(+) = tp.id
				and pf.profissao_id(+) = pp.profissao_id
				and pf.tid(+) = pp.profissao_tid
				and oc.id(+) = pp.orgao_classe_id
				and c.id_hst = :id", EsquemaBanco, EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				ResponsavelTecnico responsavel = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnico();
						responsavel.IdRelacionamento = reader.GetValue<int>("id");
						responsavel.Id = reader.GetValue<int>("responsavel_tecnico_id");
						responsavel.NomeRazao = reader.GetValue<string>("nome_razao");
						responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
						responsavel.CFONumero = reader.GetValue<string>("numero_hab_cfo_cfoc");
						responsavel.NumeroArt = reader.GetValue<string>("numero_art");
						responsavel.ArtCargoFuncao = reader.GetValue<bool>("art_cargo_funcao");

						responsavel.ProfissaoTexto = reader.GetValue<string>("profissao");
						responsavel.OrgaoClasseSigla = reader.GetValue<string>("orgao_sigla");
						responsavel.NumeroRegistro = reader.GetValue<string>("registro");

						responsavel.DataValidadeART = reader.GetValue<string>("data_validade_art");
						if (!string.IsNullOrEmpty(responsavel.DataValidadeART))
						{
							responsavel.DataValidadeART = Convert.ToDateTime(responsavel.DataValidadeART).ToShortDateString();
						}

						if (Convert.ToBoolean(reader.GetValue<int>("extensao_habilitacao")))
						{
							responsavel.CFONumero += "-ES";
						}

						caracterizacao.ResponsaveisTecnicos.Add(responsavel);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal List<ListaValor> ObterListaUnidadeMedida()
		{
			List<ListaValor> retorno = new List<ListaValor>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select id, texto from lov_crt_un_conso_un_medida", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new ListaValor()
						{
							Id = reader.GetValue<int>("id"),
							Texto = reader.GetValue<string>("texto")
						});
					}

					reader.Close();
				}

				return retorno;
			}
		}

		internal int ObterSequenciaCodigoUC()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select seq_crt_un_cons_codigo.nextval from dual", EsquemaBanco);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		#region Validações

		internal bool CodigoUCExiste(UnidadeConsolidacao unidade)
		{
//            bool existe = false;
//            int empreendimentoInternoID = 0;

//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
//            {
//                Comando comando = bancoDeDados.CriarComando(@"select nvl((select e.interno from tab_empreendimento e where e.id = :empreendimento), 0) from dual", EsquemaCredenciadoBanco);
//                comando.AdicionarParametroEntrada("empreendimento", unidade.Empreendimento.Id, DbType.Int32);

//                empreendimentoInternoID = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
//            }

//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
//            {
//                Comando comando = bancoDeDados.CriarComando(@"select count(c.id) from {0}crt_unidade_consolidacao c, {0}tab_empreendimento e 
//				where c.empreendimento = e.id and c.codigo_uc = :codigo and c.empreendimento <> :empreendimento", EsquemaBanco);

//                comando.AdicionarParametroEntrada("codigo", unidade.CodigoUC, DbType.Int64);
//                comando.AdicionarParametroEntrada("empreendimento", empreendimentoInternoID, DbType.Int32);

//                existe = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
//            }

//            if (existe)
//            {
//                return true;
//            }

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(c.id) from {0}crt_unidade_consolidacao c, {0}tab_empreendimento e 
				where e.id = c.empreendimento and c.codigo_uc = :codigo and c.id <> :id and e.credenciado = :credenciado", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("codigo", unidade.CodigoUC, DbType.Int64);
				comando.AdicionarParametroEntrada("id", unidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool FoiCopiada(int caracterizacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(1) from {0}hst_crt_unidade_consolidacao h, {0}lov_historico_artefatos_acoes aa 
				where h.acao_executada = aa.id and aa.acao = 38 and h.unidade_consolidacao = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		#endregion
	}
}