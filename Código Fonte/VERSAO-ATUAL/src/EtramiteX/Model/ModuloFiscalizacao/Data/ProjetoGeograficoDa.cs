using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	class ProjetoGeograficoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }

		private ConfiguracaoSistema _configuracaoSistema = new ConfiguracaoSistema();

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		private string EsquemaBanco { get; set; }

		#endregion

		public ProjetoGeograficoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			if (projeto == null)
			{
				throw new Exception("O Projeto geográfico é nulo.");
			}

			projeto.CorrigirMbr();

			if (projeto.Id <= 0)
			{
				Criar(projeto, banco);
			}
			else
			{
				Editar(projeto, banco);
			}
		}

		internal int? Criar(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fiscalizacao
                                    set possui_projeto_geo = :possui
                                    where id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("possui", projeto.PossuiProjetoGeo, DbType.Boolean);
                comando.AdicionarParametroEntrada("id", projeto.FiscalizacaoId, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                if (projeto.PossuiProjetoGeo == true)
                {
                    comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo p (id, fiscalizacao, situacao, nivel_precisao, mecanismo_elaboracao, 
				menor_x, menor_y, maior_x, maior_y, tid) values ({0}seq_tmp_projeto_geo.nextval, :fiscalizacao, 1, :nivel_precisao, :mecanismo_elaboracao, 
				:menor_x, :menor_y, :maior_x, :maior_y, :tid) returning p.id into :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("fiscalizacao", projeto.FiscalizacaoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("nivel_precisao", projeto.NivelPrecisaoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("mecanismo_elaboracao", projeto.MecanismoElaboracaoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("menor_x", projeto.MenorX, DbType.Decimal);
                    comando.AdicionarParametroEntrada("menor_y", projeto.MenorY, DbType.Decimal);
                    comando.AdicionarParametroEntrada("maior_x", projeto.MaiorX, DbType.Decimal);
                    comando.AdicionarParametroEntrada("maior_y", projeto.MaiorY, DbType.Decimal);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                    comando.AdicionarParametroSaida("id", DbType.Int32);

                    bancoDeDados.ExecutarNonQuery(comando);

                    projeto.Id = Convert.ToInt32(comando.ObterValorParametro("id"));


                #endregion

                    AlterarArquivosOrtofoto(projeto, bancoDeDados);

                    Historico.Gerar(projeto.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                    Consulta.Gerar(projeto.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);
                }

				bancoDeDados.Commit();

				return projeto.Id;
			}
		}

		internal void Editar(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				bancoDeDados.IniciarTransacao();

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fiscalizacao
                                    set possui_projeto_geo = :possui
                                    where id = :id", EsquemaBanco);
                comando.AdicionarParametroEntrada("possui", projeto.PossuiProjetoGeo, DbType.Boolean);
                comando.AdicionarParametroEntrada("id", projeto.FiscalizacaoId, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                if (projeto.PossuiProjetoGeo == true)
                {
                    comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo p set p.situacao = :situacao, p.nivel_precisao = :nivel_precisao, 
				                                          mecanismo_elaboracao = :mecanismo_elaboracao, p.menor_x = :menor_x, p.menor_y = :menor_y, p.maior_x = :maior_x, p.maior_y = :maior_y where p.id = :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", projeto.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("situacao", projeto.SituacaoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("mecanismo_elaboracao", projeto.MecanismoElaboracaoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("nivel_precisao", projeto.NivelPrecisaoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("menor_x", projeto.MenorX, DbType.Decimal);
                    comando.AdicionarParametroEntrada("menor_y", projeto.MenorY, DbType.Decimal);
                    comando.AdicionarParametroEntrada("maior_x", projeto.MaiorX, DbType.Decimal);
                    comando.AdicionarParametroEntrada("maior_y", projeto.MaiorY, DbType.Decimal);

                    bancoDeDados.ExecutarNonQuery(comando);

                    AlterarArquivosOrtofoto(projeto, bancoDeDados);

                    Historico.Gerar(projeto.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

                    Consulta.Gerar(projeto.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);
                }
                else if (projeto.PossuiProjetoGeo == false)
                {
                    Excluir(projeto.FiscalizacaoId, bancoDeDados);   
                }

				#endregion

				bancoDeDados.Commit();
			}
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			int? projetoId = 0;
			bool possuiGeo = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Delete da Coordenada
				Comando comando = bancoDeDados.CriarComando(@"delete from {0}geo_fisc_localizacao r where r.fiscalizacao = :fiscalizacao", EsquemaBancoGeo);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Obter dados do Projeto Geografico
				comando = bancoDeDados.CriarComando(@"select prj.*, ( select count(*) from {1}tab_fila f where f.projeto = prj.id ) temGeo from (
					select g.id from {0}tmp_projeto_geo g where g.fiscalizacao = :fiscalizacao    
					union 
					select p.id from {0}tab_fisc_prj_geo p where p.fiscalizacao = :fiscalizacao ) prj ", EsquemaBanco, EsquemaBancoGeo);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarReader(comando, (reader) =>
				{
					projetoId = reader.GetValue<int?>("id");
					possuiGeo = reader.GetValue<bool>("temGeo");
				});

				if (projetoId.GetValueOrDefault() == 0 && !possuiGeo)
				{
					bancoDeDados.Commit();
					return;
				}
				#endregion

				#region Historico/Delete Geo
                if (possuiGeo)
                {
                    try
                    {
                        Extensoes.Caracterizacoes.Data.Historico historicoCaract = new Extensoes.Caracterizacoes.Data.Historico();
                        historicoCaract.GerarGeo(projetoId.Value, (int)eHistoricoArtefato.fiscalizacao, eHistoricoAcao.excluir, bancoDeDados);
                    }
                    catch
                    {
                    }

                    comando = bancoDeDados.CriarComandoPlSql(
                    @"begin 
						
						{0}geo_operacoesprocessamentogeo.ApagarGeometriasTMP(:projeto, :fila_tipo);
						{0}geo_operacoesprocessamentogeo.ApagarGeometriasDES(:projeto, :fila_tipo);
						{0}geo_operacoesprocessamentogeo.ApagarGeometriasTrackmaker(:projeto, :fila_tipo);
						{0}geo_operacoesprocessamentogeo.ApagarGeometriasOficial(:projeto, :fila_tipo);

						delete from {1}tab_fila f where f.projeto = :projeto;

					end;", EsquemaBanco, EsquemaBancoGeo);

                    comando.AdicionarParametroEntrada("projeto", projetoId.Value, DbType.Int32);
                    comando.AdicionarParametroEntrada("fila_tipo", (int)eFilaTipoGeo.Fiscalizacao, DbType.Int32);

                    bancoDeDados.ExecutarNonQuery(comando);
                }
				#endregion

				#region Delete dos Dados Projeto
				if (projetoId.GetValueOrDefault() > 0)
				{
					comando = bancoDeDados.CriarComandoPlSql(
					@"begin 						
						delete from {0}tmp_projeto_geo_arquivos r where r.projeto = :projeto;
						delete from {0}tmp_projeto_geo_ortofoto r where r.projeto = :projeto;
						delete from {0}tmp_projeto_geo_sobrepos r where r.projeto = :projeto;
						delete from {0}tmp_projeto_geo r where r.id = :projeto;
						delete from {0}tab_fisc_prj_geo_arquivos r where r.projeto = :projeto;
						delete from {0}tab_fisc_prj_geo_ortofoto r where r.projeto = :projeto; 	
						delete from {0}tab_fisc_prj_geo t where t.id = :projeto; 
					end;", EsquemaBanco, EsquemaBancoGeo);

					comando.AdicionarParametroEntrada("projeto", projetoId.Value, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarArquivosOrtofoto(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Limpar os dados do banco

				//Ortofoto
				Comando comando = bancoDeDados.CriarComando("delete from {0}tmp_projeto_geo_ortofoto c where c.projeto = :projeto", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Arquivos Ortofoto

				comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) values 
				({0}seq_tmp_projeto_geo_ortofoto.nextval, :projeto, :caminho, :chave, :chave_data, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("caminho", DbType.String, 500);
				comando.AdicionarParametroEntrada("chave", DbType.String, 500);
				comando.AdicionarParametroEntrada("chave_data", DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				foreach (var item in projeto.ArquivosOrtofotos)
				{
					comando.SetarValorParametro("caminho", Path.GetFileName(item.Caminho));
					comando.SetarValorParametro("chave", item.Chave);
					comando.SetarValorParametro("chave_data", (item.ChaveData == DateTime.MinValue) ? (object)DBNull.Value : (object)item.ChaveData);
					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void ConcluirCadastro(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Finalizar/Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComandoPlSql(
				@"declare v_proj_id number:=0;
				begin	
					select g.id into v_proj_id from {0}tmp_projeto_geo g where g.fiscalizacao = :fiscalizacao;

					delete from {0}tab_fisc_prj_geo_ortofoto t where t.projeto = v_proj_id;
					delete from {0}tab_fisc_prj_geo_arquivos t where t.projeto = v_proj_id;
					delete from {0}tab_fisc_prj_geo t where t.id = v_proj_id;

					insert into {0}tab_fisc_prj_geo (id, fiscalizacao, situacao, nivel_precisao, mecanismo_elaboracao, menor_x, menor_y, maior_x, maior_y, tid) 
					(select t.id, t.fiscalizacao, t.situacao, t.nivel_precisao, t.mecanismo_elaboracao, t.menor_x, t.menor_y, t.maior_x, t.maior_y, :tid from {0}tmp_projeto_geo t where t.id=v_proj_id);

					insert into {0}tab_fisc_prj_geo_arquivos (id, projeto, tipo, arquivo, valido, arquivo_fila_tipo, tid)
					(select t.id, t.projeto, t.tipo, t.arquivo, t.valido, t.arquivo_fila_tipo, :tid from {0}tmp_projeto_geo_arquivos t where t.projeto = v_proj_id);
				 
					insert into {0}tab_fisc_prj_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) (select t.id, t.projeto, t.caminho, t.chave, t.chave_data, :tid 
					from {0}tmp_projeto_geo_ortofoto t where t.projeto = v_proj_id);

					delete from {1}geo_fiscal_ponto a where a.projeto = v_proj_id;
					delete from {1}geo_fiscal_linha a where a.projeto = v_proj_id;
					delete from {1}geo_fiscal_area a where a.projeto = v_proj_id;

					insert into {1}geo_fiscal_ponto (id, projeto, codigo, geometry) (select id, projeto, codigo, geometry from {1}tmp_fiscal_ponto a where a.projeto = v_proj_id);
					insert into {1}geo_fiscal_linha (id, projeto, codigo, comprimento, geometry) (select id, projeto, codigo, comprimento, geometry from {1}tmp_fiscal_linha a where a.projeto = v_proj_id);
					insert into {1}geo_fiscal_area (id, projeto, codigo, area_m2, geometry) (select id, projeto, codigo, area_m2, geometry from {1}tmp_fiscal_area a where a.projeto = v_proj_id);

					:projetoID := v_proj_id;
				end;", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.GerarNovoID());

				comando.AdicionarParametroSaida("projetoID", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				int projetoId = Convert.ToInt32(comando.ObterValorParametro("projetoID"));

				Extensoes.Caracterizacoes.Data.Historico historicoCaract = new Extensoes.Caracterizacoes.Data.Historico();
				historicoCaract.GerarGeo(projetoId, (int)eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Refazer(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Refazer/Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComandoPlSql(
				@"declare v_proj_id number:=0;
				begin	
					select g.id into v_proj_id from {0}tab_fisc_prj_geo g where g.fiscalizacao = :fiscalizacao;

					delete from {0}tmp_projeto_geo_arquivos t where t.projeto = v_proj_id;
					delete from {0}tmp_projeto_geo_ortofoto t where t.projeto = v_proj_id;					
					delete from {0}tmp_projeto_geo t where t.id = v_proj_id;

					insert into {0}tmp_projeto_geo (id, fiscalizacao, situacao, nivel_precisao, mecanismo_elaboracao, menor_x, menor_y, maior_x, maior_y, tid) 
					(select t.id, t.fiscalizacao, t.situacao, t.nivel_precisao, t.mecanismo_elaboracao, t.menor_x, t.menor_y, t.maior_x, t.maior_y, :tid from {0}tab_fisc_prj_geo t where t.id=v_proj_id);

					insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, arquivo_fila_tipo, tid)
					(select t.id, t.projeto, t.tipo, t.arquivo, t.valido, t.arquivo_fila_tipo, :tid from {0}tab_fisc_prj_geo_arquivos t where t.projeto = v_proj_id);
				 
					insert into {0}tmp_projeto_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) (select t.id, t.projeto, t.caminho, t.chave, t.chave_data, :tid 
					from {0}tab_fisc_prj_geo_ortofoto t where t.projeto = v_proj_id);

					delete from {1}tmp_fiscal_ponto a where a.projeto = v_proj_id;
					delete from {1}tmp_fiscal_linha a where a.projeto = v_proj_id;
					delete from {1}tmp_fiscal_area a where a.projeto = v_proj_id;

					insert into {1}tmp_fiscal_ponto (id, projeto, codigo, geometry) (select id, projeto, codigo, geometry from {1}geo_fiscal_ponto a where a.projeto = v_proj_id);
					insert into {1}tmp_fiscal_linha (id, projeto, codigo, comprimento, geometry) (select id, projeto, codigo, comprimento, geometry from {1}geo_fiscal_linha a where a.projeto = v_proj_id);
					insert into {1}tmp_fiscal_area (id, projeto, codigo, area_m2, geometry) (select id, projeto, codigo, area_m2, geometry from {1}geo_fiscal_area a where a.projeto = v_proj_id);
				end;", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.GerarNovoID());
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AtualizarArquivosImportar(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos a set a.valido = 0 where a.tipo > 3 and a.projeto = :projeto", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				if (arquivo.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos a set a.valido = 1, a.arquivo = :arquivo where a.projeto = :projeto and a.tipo = :tipo", EsquemaBanco);
					comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos a set a.valido = 1  where a.projeto = :projeto and a.tipo = :tipo", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.Tipo, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				if (comando.LinhasAfetadas == 0 && arquivo.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) values
						({0}seq_tmp_projeto_geo_arquivos.nextval, :projeto, 3, :arquivo, 1, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}
			}
		}

		internal void LimparArquivoEnviadoShape(int projetoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete {0}tmp_projeto_geo_arquivos a where a.tipo = 3 and a.projeto = :projeto", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void InvalidarArquivoProcessados(int projetoId, List<int> arquivos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos t set t.valido = 0 where t.projeto = :projeto", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "t.tipo", DbType.Int32, arquivos);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Ações de DML da base GEO

		internal void InserirFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {1}tab_fila f (id, projeto, tipo, mecanismo_elaboracao, etapa, situacao, data_fila)
				(select {1}seq_fila.nextval, t.id, :tipo, :mecanismo_elaboracao, :etapa, :situacao, sysdate from {0}tmp_projeto_geo t where t.id = :projeto)",
					EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.FilaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo_elaboracao", arquivo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select f.id from {0}tab_fila f where f.projeto = :projeto and f.tipo = :tipo", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.FilaTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					arquivo.IdRelacionamento = Convert.ToInt32(valor);
				}

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacaoFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("begin update {1}tmp_projeto_geo tt set tt.mecanismo_elaboracao = :mecanismo where tt.id = :projeto;" +
				"update {0}tab_fila t set t.etapa = :etapa, t.situacao = :situacao, t.data_fila = sysdate, t.data_inicio = null, t.data_fim = null, t.mecanismo_elaboracao = :mecanismo " +
				"where t.projeto = :projeto and t.tipo = :fila_tipo returning t.id into :id; end;", EsquemaBancoGeo, EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				//comando.AdicionarParametroEntrada("tipo", arquivo.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("fila_tipo", arquivo.FilaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo", arquivo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				arquivo.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));

				bancoDeDados.Commit();
			}
		}

		internal void InvalidarFila(int projetoId, List<int> arquivos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fila f set f.etapa = 1, f.situacao = 5 where f.projeto = :projeto", EsquemaBancoGeo);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "f.tipo", DbType.Int32, arquivos);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"update {0}tab_fila f set f.situacao = 5 where f.projeto = :projeto", EsquemaBancoGeo);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "f.tipo", DbType.Int32, arquivos);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal ProjetoGeografico ObterProjetoGeografico(int projetoId, BancoDeDados banco = null, bool finalizado = true)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();

			string tabela = finalizado ? "tab_fisc_prj_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.fiscalizacao, ls.id situacao_id, ls.texto situacao_texto, g.tid from {0}" + tabela + @" g, 
				{0}lov_crt_projeto_geo_situacao ls where g.situacao = ls.id and g.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projeto = new ProjetoGeografico();
						projeto.Id = projetoId;
						projeto.Tid = reader["tid"].ToString();
						projeto.FiscalizacaoId = Convert.ToInt32(reader["fiscalizacao"]);

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							projeto.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							projeto.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}

					reader.Close();
				}

				#endregion
			}

			return projeto;
		}

		internal ProjetoGeografico Obter(int projetoId, BancoDeDados banco = null, bool simplificado = false, bool finalizado = false)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();

			string tabela = finalizado ? "tab_fisc_prj_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.fiscalizacao, 
				ls.id situacao_id, ls.texto situacao_texto, ln.id nivel_precisao_id, ln.texto nivel_precisao_texto, lm.id mecanismo_elaboracao_id, 
				lm.texto mecanismo_elaboracao_texto, g.sobreposicoes_data, g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid from {0}" + tabela + @" g, 
				{0}lov_crt_projeto_geo_situacao ls, {0}lov_crt_projeto_geo_nivel ln, {0}lov_crt_projeto_geo_mecanismo lm where 
				 g.situacao = ls.id and g.nivel_precisao = ln.id(+) and g.mecanismo_elaboracao = lm.id(+) and g.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", projetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projeto = new ProjetoGeografico();
						projeto.Id = projetoId;
						projeto.Tid = reader["tid"].ToString();

						if (reader["fiscalizacao"] != null && !Convert.IsDBNull(reader["fiscalizacao"]))
						{
							projeto.FiscalizacaoId = Convert.ToInt32(reader["fiscalizacao"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							projeto.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							projeto.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["nivel_precisao_id"] != null && !Convert.IsDBNull(reader["nivel_precisao_id"]))
						{
							projeto.NivelPrecisaoId = Convert.ToInt32(reader["nivel_precisao_id"]);
							projeto.NivelPrecisaoTexto = reader["nivel_precisao_texto"].ToString();
						}

						if (reader["mecanismo_elaboracao_id"] != null && !Convert.IsDBNull(reader["mecanismo_elaboracao_id"]))
						{
							projeto.MecanismoElaboracaoId = Convert.ToInt32(reader["mecanismo_elaboracao_id"]);
							projeto.MecanismoElaboracaoTexto = reader["mecanismo_elaboracao_texto"].ToString();
						}

						if (reader["menor_x"] != null && !Convert.IsDBNull(reader["menor_x"]))
						{
							projeto.MenorX = Convert.ToDecimal(reader["menor_x"]);
						}

						if (reader["menor_y"] != null && !Convert.IsDBNull(reader["menor_y"]))
						{
							projeto.MenorY = Convert.ToDecimal(reader["menor_y"]);
						}

						if (reader["maior_x"] != null && !Convert.IsDBNull(reader["maior_x"]))
						{
							projeto.MaiorX = Convert.ToDecimal(reader["maior_x"]);
						}

						if (reader["maior_y"] != null && !Convert.IsDBNull(reader["maior_y"]))
						{
							projeto.MaiorY = Convert.ToDecimal(reader["maior_y"]);
						}

						projeto.CorrigirMbr();

						if (reader["sobreposicoes_data"] != null && !Convert.IsDBNull(reader["sobreposicoes_data"]))
						{
							projeto.Sobreposicoes.DataVerificacaoBanco = new DateTecno();
							projeto.Sobreposicoes.DataVerificacaoBanco.Data = Convert.ToDateTime(reader["sobreposicoes_data"]);
							projeto.Sobreposicoes.DataVerificacao = projeto.Sobreposicoes.DataVerificacaoBanco.Data.Value.ToString("dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
						}
					}

					reader.Close();
				}

				if (projeto.Id <= 0 || simplificado)
				{
					return projeto;
				}

				//Busca os arquivos
				if (projeto.Id > 0)
				{
					projeto.Arquivos = ObterArquivos(projeto.Id, banco: bancoDeDados, finalizado: finalizado);

					projeto.ArquivosOrtofotos = ObterOrtofotos(projeto.Id, banco: bancoDeDados, finalizado: finalizado);

					if (projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador)
					{
						projeto.ArquivoEnviadoDesenhador = ObterArquivoDesenhador(projeto, banco);
					}

					projeto.Sobreposicoes.Itens = ObterSobreposicoes(projeto.Id, bancoDeDados, finalizado);
				}

				#endregion
			}

			return projeto;
		}

		internal ProjetoGeografico Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false, bool finalizado = false)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();

			string tabela = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				projeto = Obter(id, bancoDeDados, simplificado, finalizado);

				#endregion
			}

			return projeto;
		}

		internal ProjetoGeografico ObterProjetoGeograficoPorFiscalizacao(int fiscalizacaoId, BancoDeDados banco = null, bool simplificado = false, bool finalizado = false)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();

			string tabela = finalizado ? "tab_fisc_prj_geo" : "tmp_projeto_geo";

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                #region Obter Id Projeto

                Comando comando = bancoDeDados.CriarComando(@"select g.id from {0}" + tabela + @" g where g.fiscalizacao = :fiscalizacao", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                object valor = bancoDeDados.ExecutarScalar(comando);

                projeto.Id = (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;

                #endregion

                if (projeto.Id <= 0)
                {
                    comando = bancoDeDados.CriarComando(@"select f.possui_projeto_geo from tab_fiscalizacao f where id = :fiscalizacao", EsquemaBanco);
                    comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        if (reader.Read())
                        {
                            projeto.PossuiProjetoGeo = reader.GetValue<bool?>("possui_projeto_geo");
                        }
                    }

                    return projeto;
                }

                projeto = Obter(projeto.Id, bancoDeDados, simplificado, finalizado);

                comando = bancoDeDados.CriarComando(@"
                                    select f.possui_projeto_geo
                                    from {0}tab_fiscalizacao f
                                    where f.id = :fiscalizacao", EsquemaBanco);
                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);
                projeto.PossuiProjetoGeo = bancoDeDados.ExecutarScalar<int>(comando) == 1 || projeto.Id > 0;
            }

			return projeto;
		}

		public ArquivoProjeto ObterArquivoDesenhador(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			ArquivoProjeto arquivo = new ArquivoProjeto();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select t.id, lc.id situacao_id, lc.texto situacao_texto from {1}tab_fila t, {0}lov_crt_projeto_geo_sit_proce lc
				where t.etapa = lc.etapa and t.situacao = lc.situacao and t.tipo = 5 and t.mecanismo_elaboracao = 2 and t.projeto = :projeto", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arquivo.IdRelacionamento = Convert.ToInt32(reader["id"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							arquivo.Situacao = Convert.ToInt32(reader["situacao_id"]);
							arquivo.SituacaoTexto = reader["situacao_texto"].ToString();
						}
					}
					reader.Close();
				}

				#endregion
			}

			return arquivo;
		}

		public List<ArquivoProjeto> ObterArquivos(int projetoId, BancoDeDados banco = null, bool? valido = true, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			string tabela = finalizado ? "tab_fisc_prj_geo_arquivos" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos

				Comando comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, t.arquivo_fila_tipo, t.arquivo, t.valido, 
				lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabela + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
				{0}lov_crt_projeto_geo_sit_proce lcp where t.tipo = lc.id and t.projeto = tf.projeto(+) and t.arquivo_fila_tipo = tf.tipo(+) 
				and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.projeto = :projeto and t.tipo <> 5 order by lc.id", EsquemaBanco, EsquemaBancoGeo);


				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arq.IdRelacionamento = Convert.ToInt32(reader["id"]);
						}

						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							arq.Nome = reader["tipo_texto"].ToString();
							arq.Tipo = Convert.ToInt32(reader["tipo"]);
						}

						if (reader["arquivo_fila_tipo"] != null && !Convert.IsDBNull(reader["arquivo_fila_tipo"]))
						{
							arq.FilaTipo = Convert.ToInt32(reader["arquivo_fila_tipo"]);
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							arq.Id = Convert.ToInt32(reader["arquivo"]);
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							arq.Situacao = Convert.ToInt32(reader["situacao_id"]);
							arq.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						if (reader["valido"] != null && !Convert.IsDBNull(reader["valido"]))
						{
							arq.isValido = Convert.ToBoolean(reader["valido"]);
						}

						if (valido == null || arq.isValido == valido)
						{
							arquivos.Add(arq);
						}
					}

					reader.Close();
				}

				#endregion
			}
			return arquivos;
		}

		public List<ArquivoProjeto> ObterOrtofotos(int projetoId, BancoDeDados banco = null, bool finalizado = false, bool todos = true)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			string tabela = finalizado ? "crt_projeto_geo_ortofoto" : "tmp_projeto_geo_ortofoto";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos Ortofoto

				Comando comando;

				if (todos)
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.caminho, t.chave, t.chave_data, 'application/zip' tipo from {0}" + tabela + " t where t.projeto = :projeto", EsquemaBanco);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.caminho, t.chave, t.chave_data, 'application/zip' tipo from {0}" + tabela + " t where t.id = :projeto", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					string diretorio = _configuracaoSistema.DiretorioOrtoFotoMosaico[1];
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();
						arq.Id = Convert.ToInt32(reader["id"]);
						arq.Situacao = 9;

						arq.Nome = reader["caminho"].ToString();

						arq.ContentType = reader["tipo"].ToString();
						if (reader["caminho"] != null && !Convert.IsDBNull(reader["caminho"]))
						{
							arq.Caminho = diretorio + "\\" + reader["caminho"].ToString();
						}

						if (reader["chave"] != null && !Convert.IsDBNull(reader["chave"]))
						{
							arq.Chave = reader["chave"].ToString();
						}

						if (reader["chave_data"] != null && !Convert.IsDBNull(reader["chave_data"]))
						{
							arq.ChaveData = Convert.ToDateTime(reader["chave_data"]);
						}

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}
			return arquivos;
		}

		internal List<Sobreposicao> ObterSobreposicoes(int projetoId, BancoDeDados banco = null, bool finalizado = false)
		{
			List<Sobreposicao> sobreposicoes = new List<Sobreposicao>();

			string tabela = finalizado ? "crt_projeto_geo_sobrepos" : "tmp_projeto_geo_sobrepos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Sobreposicao

				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.base, s.tipo, s.identificacao, s.tid 
					from {0}" + tabela + @" s where s.projeto = :projeto", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				Sobreposicao sobreposicao = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						sobreposicao = new Sobreposicao();

						sobreposicao.Base = Convert.ToInt32(reader["base"]);
						sobreposicao.Tipo = Convert.ToInt32(reader["tipo"]);

						if (reader["identificacao"] != null && !Convert.IsDBNull(reader["identificacao"]))
						{
							sobreposicao.Identificacao = reader["identificacao"].ToString();
						}

						sobreposicoes.Add(sobreposicao);
					}

					reader.Close();
				}

				#endregion
			}

			return sobreposicoes;
		}

		internal void ObterSituacaoFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select lc.id, lc.texto, cpg.arquivo from {1}tab_fila t, {0}lov_crt_projeto_geo_sit_proce lc, {0}tmp_projeto_geo_arquivos cpg
				where t.situacao = lc.situacao(+) and t.etapa = lc.etapa(+) and t.projeto = cpg.projeto(+) and t.tipo = cpg.arquivo_fila_tipo(+) and t.id = :arquivo_id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("arquivo_id", arquivo.IdRelacionamento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arquivo.Situacao = Convert.ToInt32(reader["id"]);
						}

						if (reader["texto"] != null && !Convert.IsDBNull(reader["texto"]))
						{
							arquivo.SituacaoTexto = reader["texto"].ToString();
						}

						if (reader["arquivo"] != null && !Convert.IsDBNull(reader["arquivo"]))
						{
							arquivo.Id = Convert.ToInt32(reader["arquivo"]);
						}
					}
					reader.Close();
				}
			}
		}

		internal void ObterSituacao(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto from {0}lov_crt_projeto_geo_sit_proce t where t.etapa = :etapa and t.situacao = :situacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
						{
							arquivo.Situacao = Convert.ToInt32(reader["id"]);
						}

						if (reader["texto"] != null && !Convert.IsDBNull(reader["texto"]))
						{
							arquivo.SituacaoTexto = reader["texto"].ToString();
						}
					}
					reader.Close();
				}
			}
		}

		internal int ObterSitacaoProjetoGeografico(int projetoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select t.situacao from {0}tmp_projeto_geo t where t.id = :projeto), 
				(select t.situacao from {0}crt_projeto_geo t where t.id = :projeto)) situacao from dual", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tmp_projeto_geo t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

		#endregion

		#region Obter Geo

		internal Sobreposicao ObterGeoSobreposicaoIdaf(int projetoId, BancoDeDados banco = null)
		{
			Sobreposicao sobreposicao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Sobreposicao Empreendimento [ATP]

				Comando comando = bancoDeDados.CriarComando(@"select e.id, seg.texto segmento, e.denominador 
					from {0}tab_empreendimento e, {0}crt_projeto_geo pg, {0}lov_empreendimento_segmento seg 
						where pg.empreendimento = e.id and pg.id in 
						(select atp.projeto 
						from {1}tmp_atp a, {1}geo_atp atp 
					where a.id = :projeto and sdo_relate(atp.geometry, a.geometry, 'MASK=ANYINTERACT') = 'TRUE')", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				List<String> lstEmp = new List<String>();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["denominador"] != null && !Convert.IsDBNull(reader["denominador"]))
						{
							lstEmp.Add(String.Format("{0} - {1}", reader["segmento"], reader["denominador"]));
						}
					}

					reader.Close();
				}

				if (lstEmp.Count > 0)
				{
					sobreposicao = new Sobreposicao();
					sobreposicao.Identificacao = String.Join("; ", lstEmp.ToArray());
					sobreposicao.Base = (int)eSobreposicaoBase.IDAF;
					sobreposicao.Tipo = (int)eSobreposicaoTipo.OutrosEmpreendimento;
				}

				#endregion
			}
			return sobreposicao;
		}

		#endregion

		#region Validações

		internal int ExisteProjetoGeografico(int fiscalizacao, bool finalizado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tmp_projeto_geo t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				/*if (finalizado)
				{
					comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo", EsquemaBanco);
				}*/

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal int ExisteArquivoFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_fila t where t.projeto = :projeto and t.tipo = :tipo", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.FilaTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal bool VerificarProjetoGeograficoProcessado(int projetoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_fila t where t.projeto = :projeto and t.etapa = 3 and t.situacao = 4", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}