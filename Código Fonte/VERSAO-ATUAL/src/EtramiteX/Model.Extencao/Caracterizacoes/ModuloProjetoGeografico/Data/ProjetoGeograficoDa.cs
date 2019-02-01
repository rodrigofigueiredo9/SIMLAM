using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data
{
	class ProjetoGeograficoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

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

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo p (id, empreendimento, caracterizacao, situacao, nivel_precisao, mecanismo_elaboracao, 
				menor_x, menor_y, maior_x, maior_y, tid) values ({0}seq_tmp_projeto_geo.nextval, :empreendimento, :caracterizacao, 1, :nivel_precisao, :mecanismo_elaboracao, 
				:menor_x, :menor_y, :maior_x, :maior_y, :tid) returning p.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", projeto.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", projeto.CaracterizacaoId, DbType.Int32);
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

				#region Arquivos ortofoto

				AlterarArquivosOrtofoto(projeto, bancoDeDados);

				#endregion

				#region Sobreposicoes
				AlterarSobreposicoes(projeto, bancoDeDados);
				#endregion

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

				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo p set p.situacao = :situacao, p.nivel_precisao = :nivel_precisao, 
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

				#endregion

				#region Arquivos ortofoto

				AlterarArquivosOrtofoto(projeto, bancoDeDados);

				#endregion

				#region Sobreposicoes
				AlterarSobreposicoes(projeto, bancoDeDados);
				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, int tipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_projeto_geo c set c.tid = :tid 
				where c.empreendimento = :empreendimento and c.caracterizacao = :caracterizacao returning c.id into :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				if (comando.LinhasAfetadas == 0)
				{
					return;
				}

				int id = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Gerar Historico Geo

				Historico.GerarGeo(id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados do projeto

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComandoPlSql(
				@"begin 
                    
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasTMP(:projeto, :fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasDES(:projeto, :fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasTrackmaker(:projeto, :fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasOficial(:projeto, :fila_tipo);
					
					delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :projeto and d.dependente_caracterizacao = :dependente_caracterizacao;
                    delete from {1}tab_fila f where f.projeto = :projeto;
                    delete from {0}tmp_projeto_geo_arquivos r where r.projeto = :projeto;
                    delete from {0}tmp_projeto_geo_ortofoto r where r.projeto = :projeto;
                    delete from {0}tmp_projeto_geo_sobrepos r where r.projeto = :projeto;
                    delete from {0}tmp_projeto_geo r where r.id = :projeto;
                    delete from {0}crt_projeto_geo_arquivos r where r.projeto = :projeto;
                    delete from {0}crt_projeto_geo_ortofoto r where r.projeto = :projeto;
                    delete from {0}crt_projeto_geo_sobrepos r where r.projeto = :projeto;
                    delete from {0}crt_projeto_geo r where r.id = :projeto;

				end;", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico, DbType.Int32);
				comando.AdicionarParametroEntrada("fila_tipo", (int)((tipo == (int)eCaracterizacao.Dominialidade) ? eFilaTipoGeo.Dominialidade : eFilaTipoGeo.Atividade), DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
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

		internal void AlterarSobreposicoes(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Limpar os dados do banco

				Comando comando = bancoDeDados.CriarComando("delete from {0}tmp_projeto_geo_sobrepos c where c.projeto = :projeto", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				if (projeto.Sobreposicoes.Itens != null && projeto.Sobreposicoes.Itens.Count > 0)
				{
					#region Sobreposicoes data
					//Atualizar o tid para a nova ação
					comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo c set c.sobreposicoes_data = :sobreposicoes_data where c.id = :projeto", EsquemaBanco);
					comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("sobreposicoes_data", projeto.Sobreposicoes.DataVerificacaoBanco.Data, DbType.DateTime);

					bancoDeDados.ExecutarNonQuery(comando);
					#endregion

					#region Sobreposicoes

					foreach (var item in projeto.Sobreposicoes.Itens)
					{
						if (string.IsNullOrEmpty(item.Identificacao) || item.Identificacao.Trim() == "-")
						{
							continue;
						}

						comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
							values ({0}seq_tmp_projeto_geo_sobrepos.nextval, :projeto, :base, :tipo, :identificacao, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						comando.AdicionarParametroEntrada("base", item.Base, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);

						comando.AdicionarParametroEntrada("identificacao", (item.Identificacao.Length > 4000) ? item.Identificacao.Remove(4000) : item.Identificacao, DbType.AnsiString);

						bancoDeDados.ExecutarNonQuery(comando);
					}

					#endregion
				}

				bancoDeDados.Commit();
			}
		}

		internal void Finalizar(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Finalizar/Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					-----------------------------------------------------------------------------------------------
					-- Projeto Geográfico
					-----------------------------------------------------------------------------------------------
					for i in (select i.id, i.empreendimento, i.caracterizacao, i.situacao, i.nivel_precisao, i.mecanismo_elaboracao, i.sobreposicoes_data, 
						i.menor_x, i.menor_y, i.maior_x, i.maior_y, i.tid from tmp_projeto_geo i where i.id = :id) loop
                   
						-- Apaga os dados antigos
						delete from {0}crt_projeto_geo_ortofoto t where t.projeto = i.id;
						delete from {0}crt_projeto_geo_sobrepos t where t.projeto = i.id;
						delete from {0}crt_projeto_geo_arquivos t where t.projeto = i.id;
						delete from {0}crt_projeto_geo t where t.id =  i.id;
            
						-- Inserindo na tabela oficial
						insert into {0}crt_projeto_geo (id, empreendimento, caracterizacao, situacao, nivel_precisao, mecanismo_elaboracao, sobreposicoes_data, menor_x, menor_y,
						maior_x, maior_y, tid) values (i.id, i.empreendimento, i.caracterizacao, 2/*Finalizado*/, i.nivel_precisao, i.mecanismo_elaboracao, i.sobreposicoes_data,
						i.menor_x, i.menor_y, i.maior_x, i.maior_y, i.tid);

						-- Inserindo os arquivos         
						insert into {0}crt_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid)
						(select id, projeto, tipo, arquivo, valido, i.tid from {0}tmp_projeto_geo_arquivos t where t.projeto = i.id);
            
						-- Inserindo os arquivos: ortofoto         
						insert into {0}crt_projeto_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) (select id, projeto, caminho, chave, chave_data, i.tid 
						from {0}tmp_projeto_geo_ortofoto t where t.projeto = i.id);
         
						-- Inserindo Sobreposicoes         
						insert into {0}crt_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
						(select id, projeto, base, tipo, identificacao, i.tid from {0}tmp_projeto_geo_sobrepos t where t.projeto = i.id);

						--Importa as tabelas do da temporária para as tabelas oficiais
						{0}geo_operacoesprocessamentogeo.ExportarParaTabelasGEO(i.id, i.tid);

						--Atualiza relacionamento com exploracoes
						update {0}crt_exp_florestal_geo c set c.geo_pativ_id = c.tmp_pativ_id
							where exists(select 1 from {1}geo_pativ g where g.id = c.tmp_pativ_id and g.projeto = i.id);
						update {0}crt_exp_florestal_geo c set c.geo_aativ_id = c.tmp_aativ_id
							where exists(select 1 from {1}geo_aativ g where g.id = c.tmp_aativ_id and g.projeto = i.id);
	
						--Apaga o rascunho
						delete {0}tmp_projeto_geo_arquivos g where g.projeto = i.id;
						delete {0}tmp_projeto_geo_ortofoto g where g.projeto = i.id;
						delete {0}tmp_projeto_geo_sobrepos g where g.projeto = i.id;
						delete {0}tmp_projeto_geo g where g.id = i.id;         
                            
					end loop;
					-----------------------------------------------------------------------------------------------
				end; ", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.finalizar, bancoDeDados, null);

				#endregion

				#region Gerar Historico Geo

				Historico.GerarGeo(id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.finalizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Refazer(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Refazer o projeto geográfico

				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare 
					v_fila_tipo number:=0;
					v_caract_tipo number:=0;
					v_mecanismo number:=0;
				begin 
					delete from {0}tmp_projeto_geo_ortofoto t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_sobrepos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_arquivos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo t where t.id = :projeto; 

					insert into {0}tmp_projeto_geo (id, empreendimento, caracterizacao, situacao, nivel_precisao, mecanismo_elaboracao, sobreposicoes_data, menor_x, menor_y, 
					maior_x, maior_y, tid) (select g.id, g.empreendimento, g.caracterizacao, 4, g.nivel_precisao, g.mecanismo_elaboracao, g.sobreposicoes_data, g.menor_x, 
					g.menor_y, g.maior_x, g.maior_y, g.tid from {0}crt_projeto_geo g where g.id = :projeto); 

					insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) 
					(select g.id, g.projeto, g.tipo, g.arquivo, g.valido, g.tid from {0}crt_projeto_geo_arquivos g where g.projeto = :projeto); 

					insert into {0}tmp_projeto_geo_ortofoto (id, projeto, caminho, tid) 
					(select g.id, g.projeto, g.caminho, g.tid from {0}crt_projeto_geo_ortofoto g where g.projeto = :projeto); 

					insert into {0}tmp_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
					(select g.id, g.projeto, g.base, g.tipo, g.identificacao, g.tid from {0}crt_projeto_geo_sobrepos g where g.projeto = :projeto); 

					select c.mecanismo_elaboracao, c.caracterizacao into v_mecanismo, v_caract_tipo from {0}crt_projeto_geo c where c.id = :projeto;

					v_fila_tipo := :filaTipoAtividade;
					if v_caract_tipo = :dominialidadeTipo then 
						v_fila_tipo := :filaTipoDominialidade;
					end if;

					{0}geo_operacoesprocessamentogeo.ApagarGeometriasTMP(:projeto, v_fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasDES(:projeto, v_fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasTrackmaker(:projeto, v_fila_tipo);

					{0}geo_operacoesprocessamentogeo.ImportarDoFinalizado(:projeto, v_fila_tipo);

					if v_mecanismo = :mecDesenhador then 
						{0}geo_operacoesprocessamentogeo.ImportarParaDesenhFinalizada(:projeto, v_fila_tipo);
					end if;

				end; ", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);
				comando.AdicionarParametroEntrada("mecDesenhador", (int)eProjetoGeograficoMecanismo.Desenhador, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidadeTipo", (int)eCaracterizacao.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoDominialidade", (int)eFilaTipoGeo.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoAtividade", (int)eFilaTipoGeo.Atividade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void Atualizar(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Atualizar o projeto geográfico

				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare 
					v_fila_tipo number:=0;
					v_caract_tipo number:=0;
					v_mecanismo number:=0;
				begin 
					delete from {0}tmp_projeto_geo_ortofoto t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_sobrepos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_arquivos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo t where t.id = :projeto; 

					insert into {0}tmp_projeto_geo (id, empreendimento, caracterizacao, situacao, nivel_precisao, mecanismo_elaboracao, sobreposicoes_data, menor_x, menor_y, 
					maior_x, maior_y, tid) (select g.id, g.empreendimento, g.caracterizacao, 4, g.nivel_precisao, g.mecanismo_elaboracao, g.sobreposicoes_data, g.menor_x, 
					g.menor_y, g.maior_x, g.maior_y, g.tid from {0}crt_projeto_geo g where g.id = :projeto); 

					insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) 
					(select g.id, g.projeto, g.tipo, g.arquivo, g.valido, g.tid from {0}crt_projeto_geo_arquivos g where g.projeto = :projeto); 

					insert into {0}tmp_projeto_geo_ortofoto (id, projeto, caminho, tid) 
					(select g.id, g.projeto, g.caminho, g.tid from {0}crt_projeto_geo_ortofoto g where g.projeto = :projeto); 

					insert into {0}tmp_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
					(select g.id, g.projeto, g.base, g.tipo, g.identificacao, g.tid from {0}crt_projeto_geo_sobrepos g where g.projeto = :projeto); 

					select c.mecanismo_elaboracao, c.caracterizacao into v_mecanismo, v_caract_tipo from {0}crt_projeto_geo c where c.id = :projeto;

					v_fila_tipo := :filaTipoAtividade;
					if v_caract_tipo = :dominialidadeTipo then 
						v_fila_tipo := :filaTipoDominialidade;
					end if;

				end; ", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidadeTipo", (int)eCaracterizacao.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoDominialidade", (int)eFilaTipoGeo.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoAtividade", (int)eFilaTipoGeo.Atividade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void Reabrir(int id, int titulo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Atualizar o projeto geográfico

				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare 
					v_fila_tipo number:=0;
					v_caract_tipo number:=0;
					v_mecanismo number:=0;
				begin 
					delete from {0}tmp_projeto_geo_ortofoto t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_sobrepos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_arquivos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo t where t.id = :projeto; 

					insert into {0}tmp_projeto_geo (id, empreendimento, caracterizacao, situacao, nivel_precisao, mecanismo_elaboracao, sobreposicoes_data, menor_x, menor_y, 
					maior_x, maior_y, tid) (select g.id, g.empreendimento, g.caracterizacao, 4, g.nivel_precisao, g.mecanismo_elaboracao, g.sobreposicoes_data, g.menor_x, 
					g.menor_y, g.maior_x, g.maior_y, g.tid from {0}crt_projeto_geo g where g.id = :projeto); 

					insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) 
					(select g.id, g.projeto, g.tipo, g.arquivo, g.valido, g.tid from {0}crt_projeto_geo_arquivos g where g.projeto = :projeto); 

					insert into {0}tmp_projeto_geo_ortofoto (id, projeto, caminho, tid) 
					(select g.id, g.projeto, g.caminho, g.tid from {0}crt_projeto_geo_ortofoto g where g.projeto = :projeto); 

					insert into {0}tmp_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
					(select g.id, g.projeto, g.base, g.tipo, g.identificacao, g.tid from {0}crt_projeto_geo_sobrepos g where g.projeto = :projeto); 

					select c.mecanismo_elaboracao, c.caracterizacao into v_mecanismo, v_caract_tipo from {0}crt_projeto_geo c where c.id = :projeto;

					v_fila_tipo := :filaTipoAtividade;
					if v_caract_tipo = :dominialidadeTipo then 
						v_fila_tipo := :filaTipoDominialidade;
					end if;

					insert into {1}DES_PATIV (id, projeto, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, rest_declividade, arl, rppn, app, tipo_exploracao, geometry)
						(select a.id, a.projeto, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.rest_declividade, a.arl, a.rppn, a.app, a.tipo_exploracao, a.geometry
							from {1}GEO_PATIV a where a.projeto = :projeto
							and exists
							(select 1 from {0}crt_exp_florestal_geo g
								where g.geo_pativ_id = a.id
								and exists(select 1 from {0}crt_exp_florestal_exploracao ep
								where ep.id = g.exp_florestal_exploracao
								and exists(select 1 from {0}tab_titulo_exp_florestal t
								where t.titulo = :titulo
								and t.exploracao_florestal = ep.exploracao_florestal)))
							and not exists(select 1 from {1}DES_PATIV d where d.id = a.id));
					insert into {1}DES_AATIV (id, projeto, area_m2, cod_apmp, codigo, atividade, rocha, massa_dagua, avn, aa, afs, rest_declividade, arl, rppn, app, tipo_exploracao, geometry)
						(select a.id, a.projeto, a.area_m2, a.cod_apmp, a.codigo, a.atividade, a.rocha, a.massa_dagua, a.avn, a.aa, a.afs, a.rest_declividade, a.arl, a.rppn, a.app, a.tipo_exploracao, a.geometry
							from {1}GEO_AATIV a where a.projeto = :projeto
							and exists
							(select 1 from {0}crt_exp_florestal_geo g
								where g.geo_aativ_id = a.id
								and exists(select 1 from {0}crt_exp_florestal_exploracao ep
								where ep.id = g.exp_florestal_exploracao
								and exists(select 1 from {0}tab_titulo_exp_florestal t
								where t.titulo = :titulo
								and t.exploracao_florestal = ep.exploracao_florestal)))
							and not exists(select 1 from {1}DES_AATIV d where d.id = a.id));

					delete from {1}GEO_PATIV a where a.projeto = :projeto
						and exists
						(select 1 from {0}crt_exp_florestal_geo g
							where g.geo_pativ_id = a.id
							and exists(select 1 from {0}crt_exp_florestal_exploracao ep
							where ep.id = g.exp_florestal_exploracao
							and exists(select 1 from {0}tab_titulo_exp_florestal t
							where t.titulo = :titulo
							and t.exploracao_florestal = ep.exploracao_florestal)));

					delete from {1}GEO_AATIV a where a.projeto = :projeto
						and exists
						(select 1 from {0}crt_exp_florestal_geo g
							where g.geo_aativ_id = a.id
							and exists(select 1 from {0}crt_exp_florestal_exploracao ep
							where ep.id = g.exp_florestal_exploracao
							and exists(select 1 from {0}tab_titulo_exp_florestal t
							where t.titulo = :titulo
							and t.exploracao_florestal = ep.exploracao_florestal)));
				end; ", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidadeTipo", (int)eCaracterizacao.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoDominialidade", (int)eFilaTipoGeo.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoAtividade", (int)eFilaTipoGeo.Atividade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void ExcluirRascunho(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Excluir o projeto geográfico

				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComandoPlSql(
				@"declare 
					v_fila_tipo number:=0;
					v_caract_tipo number:=0;
				begin 
					delete from {0}tmp_projeto_geo_sobrepos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_ortofoto t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_arquivos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo t where t.id = :projeto; 
					update {1}tab_fila f set f.etapa = (case when f.tipo in (1,2) then 2 else 3 end), f.situacao = 4 where f.projeto = :projeto; 
					
					select c.caracterizacao into v_caract_tipo from {0}crt_projeto_geo c where c.id = :projeto;					
					
					if v_caract_tipo = :dominialidadeTipo then 
						v_fila_tipo := :filaTipoDominialidade;
					elsif v_caract_tipo = :regularizacaoTipo then
						v_fila_tipo := :filaTipoRegularizacao;
					else
						v_fila_tipo := :filaTipoAtividade;
					end if;					
					
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasTMP(:projeto, v_fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasDES(:projeto, v_fila_tipo);
					{0}geo_operacoesprocessamentogeo.ApagarGeometriasTrackmaker(:projeto, v_fila_tipo);

				end; ", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidadeTipo", (int)eCaracterizacao.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoDominialidade", (int)eFilaTipoGeo.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoAtividade", (int)eFilaTipoGeo.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("regularizacaoTipo", (int)eCaracterizacao.RegularizacaoFundiaria, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoRegularizacao", (int)eFilaTipoGeo.RegularizacaoFundiaria, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void AtualizarArquivosEnviado(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos a set a.valido = 0 where a.tipo > 3 and a.projeto = :projeto", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos a set a.valido = 1 where a.projeto = :projeto and a.tipo = :tipo", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				if (comando.LinhasAfetadas == 0 && arquivo.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) values
						({0}seq_tmp_projeto_geo_arquivos.nextval, :projeto, :tipo, :arquivo, 1, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}
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

		internal void ExcluirArquivoDuplicados(int projetoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete from {0}tmp_projeto_geo_arquivos a where a.projeto = :projeto and a.tipo = :tipo and a.id <> 
					(select max(b.id) from {0}tmp_projeto_geo_arquivos b where b.projeto = a.projeto and b.tipo = a.tipo )", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void AnexarCroqui(int titulo, int arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Alterar situação do Título

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_arquivo a (id, titulo, arquivo, ordem, descricao, croqui, tid) 
							select {0}seq_titulo_arquivo.nextval, :titulo, :arquivo, nvl((select count(*) from {0}tab_titulo_arquivo t where t.titulo = :titulo), 0), 'Croqui', :croqui, :tid from dual
							where not exists (select 1 from {0}tab_titulo_arquivo t where t.titulo = :titulo and t.croqui = :croqui)", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", arquivo, DbType.Int32);
				comando.AdicionarParametroEntrada("croqui", true, DbType.Boolean);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

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

				Comando comando = bancoDeDados.CriarComando(@"insert into {1}tab_fila f (id, empreendimento, projeto, tipo, mecanismo_elaboracao, etapa, situacao, data_fila, titulo)
				(select {1}seq_fila.nextval, t.empreendimento, t.id, :tipo, :mecanismo_elaboracao, :etapa, :situacao, sysdate, :titulo from {0}" +
				(arquivo.TituloId > 0 ? "crt_projeto_geo" : "tmp_projeto_geo") + @" t where t.id = :projeto)",
					EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.FilaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo_elaboracao", arquivo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", arquivo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", arquivo.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("titulo", arquivo.TituloId, DbType.Int32);

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

				Comando comando = bancoDeDados.CriarComando("begin " + (arquivo.TituloId > 0 ? "" : "update {1}tmp_projeto_geo tt set tt.mecanismo_elaboracao = :mecanismo where tt.id = :projeto;") +
				"update {0}tab_fila t set t.etapa = :etapa, t.situacao = :situacao, t.data_fila = sysdate, t.data_inicio = null, t.data_fim = null, t.mecanismo_elaboracao = :mecanismo " +
				"where t.projeto = :projeto and t.tipo = :tipo returning t.id into :id; end;", EsquemaBancoGeo, EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.FilaTipo, DbType.Int32);
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

		internal ProjetoGeografico ObterProjetoGeografico(int id, BancoDeDados banco = null, bool finalizado = true)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();

			string tabela = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.caracterizacao, ls.id situacao_id, ls.texto situacao_texto, g.tid from {0}" + tabela + @" g, 
				{0}lov_crt_projeto_geo_situacao ls where g.situacao = ls.id and g.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projeto = new ProjetoGeografico();
						projeto.Id = id;
						projeto.Tid = reader["tid"].ToString();
						projeto.CaracterizacaoId = Convert.ToInt32(reader["caracterizacao"]);

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

		internal ProjetoGeografico Obter(int id, int? tipo = null, BancoDeDados banco = null, bool simplificado = false, bool finalizado = false)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();

			string tabela = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.empreendimento, lc.id caracterizacao_id, lc.texto caracterizacao_texto, 
				ls.id situacao_id, ls.texto situacao_texto, ln.id nivel_precisao_id, ln.texto nivel_precisao_texto, lm.id mecanismo_elaboracao_id, 
				lm.texto mecanismo_elaboracao_texto, g.sobreposicoes_data, g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid from {0}" + tabela + @" g, {0}lov_caracterizacao_tipo lc, 
				{0}lov_crt_projeto_geo_situacao ls, {0}lov_crt_projeto_geo_nivel ln, {0}lov_crt_projeto_geo_mecanismo lm where g.caracterizacao = lc.id 
				and g.situacao = ls.id and g.nivel_precisao = ln.id(+) and g.mecanismo_elaboracao = lm.id(+) and g.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projeto = new ProjetoGeografico();
						projeto.Id = id;
						projeto.Tid = reader["tid"].ToString();

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							projeto.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						}

						if (reader["caracterizacao_id"] != null && !Convert.IsDBNull(reader["caracterizacao_id"]))
						{
							projeto.CaracterizacaoId = Convert.ToInt32(reader["caracterizacao_id"]);
							projeto.CaracterizacaoTexto = reader["caracterizacao_texto"].ToString();
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
					projeto.Arquivos = ObterArquivos(projeto.Id, tipo, banco: bancoDeDados, finalizado: finalizado);

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

				if (tid == null)
				{
					projeto = Obter(id, null, bancoDeDados, simplificado, finalizado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}" + tabela + " s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					projeto = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, null, bancoDeDados, simplificado, finalizado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}

				#endregion
			}

			return projeto;
		}

		private ProjetoGeografico ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.empreendimento_id, g.caracterizacao_id, g.caracterizacao_texto, 
				g.situacao_id, g.situacao_texto, g.nivel_precisao_id, g.nivel_precisao_texto, g.mecanismo_elaboracao_id, 
				g.mecanismo_elaboracao_texto, g.sobreposicoes_data, g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid from {0}hst_crt_projeto_geo g
				where g.projeto_geo_id = :id and g.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);
						projeto = new ProjetoGeografico();
						projeto.Id = id;
						projeto.Tid = reader["tid"].ToString();

						if (reader["empreendimento"] != null && !Convert.IsDBNull(reader["empreendimento"]))
						{
							projeto.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						}

						if (reader["caracterizacao_id"] != null && !Convert.IsDBNull(reader["caracterizacao_id"]))
						{
							projeto.CaracterizacaoId = Convert.ToInt32(reader["caracterizacao_id"]);
							projeto.CaracterizacaoTexto = reader["caracterizacao_texto"].ToString();
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
							projeto.Sobreposicoes = new Sobreposicoes();
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

				#endregion

				#region Projeto Geográfico/Arquivos

				if (hst > 0)
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.projeto_geo_arquivo_id, t.tipo_id, t.tipo_texto, t.arquivo_id, t.arquivo_tid, 
					t.valido from {0}hst_crt_projeto_geo_arquivos t where t.id_hst = :id_hst and t.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							ArquivoProjeto arq = new ArquivoProjeto();

							if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
							{
								arq.IdRelacionamento = Convert.ToInt32(reader["id"]);
							}

							if (reader["tipo_id"] != null && !Convert.IsDBNull(reader["tipo_id"]))
							{
								arq.Nome = reader["tipo_texto"].ToString();
								arq.Tipo = Convert.ToInt32(reader["tipo_id"]);
							}

							if (reader["arquivo_id"] != null && !Convert.IsDBNull(reader["arquivo_id"]))
							{
								arq.Id = Convert.ToInt32(reader["arquivo_id"]);
							}

							if (reader["valido"] != null && !Convert.IsDBNull(reader["valido"]))
							{
								arq.isValido = Convert.ToBoolean(reader["valido"]);
							}

							projeto.Arquivos.Add(arq);
						}

						reader.Close();
					}
				}

				#endregion

				#region Projeto Geográfico/Arquivos Ortofoto

				if (hst > 0)
				{
					comando = bancoDeDados.CriarComando(@"select po.ortofoto_id id, po.caminho, po.tid from {0}hst_crt_projeto_geo_ortofoto po 
					where po.id_hst = :id_hst and po.tid =:tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							ArquivoProjeto arq = new ArquivoProjeto();

							if (reader["id"] != null && !Convert.IsDBNull(reader["id"]))
							{
								arq.IdRelacionamento = Convert.ToInt32(reader["id"]);
							}

							arq.Caminho = reader["caminho"].ToString();
							arq.Tid = reader["tid"].ToString();

							projeto.ArquivosOrtofotos.Add(arq);
						}

						reader.Close();
					}
				}

				#endregion

				#region Projeto Geográfico/Sobreposicoes

				if (hst > 0)
				{
					comando = bancoDeDados.CriarComando(@"select so.id, so.id_hst, so.tid, so.sobreposicao_id, so.projeto_id, so.projeto_tid, so.base, 
					so.tipo_id, so.tipo_texto, so.identificacao from {0}hst_crt_projeto_geo_sobrepos so where so.id_hst = :id_hst and so.tid =:tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Sobreposicao sobreposicao = null;
						while (reader.Read())
						{
							sobreposicao = new Sobreposicao();

							if (reader["base"] != null && !Convert.IsDBNull(reader["base"]))
							{
								sobreposicao.Base = Convert.ToInt32(reader["base"]);
							}

							if (reader["tipo_id"] != null && !Convert.IsDBNull(reader["tipo_id"]))
							{
								sobreposicao.Tipo = Convert.ToInt32(reader["tipo_id"]);
							}

							if (reader["tid"] != null && !Convert.IsDBNull(reader["tid"]))
							{
								sobreposicao.Tid = reader["tid"].ToString();
							}

							if (reader["identificacao"] != null && !Convert.IsDBNull(reader["identificacao"]))
							{
								sobreposicao.Identificacao = reader["identificacao"].ToString();
							}

							projeto.Sobreposicoes.Itens.Add(sobreposicao);
						}

						reader.Close();
					}
				}

				#endregion
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
				where t.etapa = lc.etapa and t.situacao = lc.situacao and t.tipo in (3, 4) and t.mecanismo_elaboracao = 2 and t.projeto = :projeto", EsquemaBanco, EsquemaBancoGeo);

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

		public List<ArquivoProjeto> ObterArquivos(int projetoId, int? tipo = null, BancoDeDados banco = null, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			string tabela = finalizado ? "crt_projeto_geo_arquivos" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos
				Comando comando = null;

				if (tipo == (int)eCaracterizacao.RegularizacaoFundiaria)
				{
					comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, tf.tipo fila_tipo, t.arquivo, t.valido, 
						lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabela + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
						{0}lov_crt_projeto_geo_sit_proce lcp where t.tipo = lc.id and t.projeto = tf.projeto(+) and tf.tipo(+) = " + (int)eFilaTipoGeo.RegularizacaoFundiaria +
						"and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.projeto = :projeto and t.valido = 1 and t.tipo <> 5 order by lc.id", EsquemaBanco, EsquemaBancoGeo);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, tf.tipo fila_tipo, t.arquivo, t.valido, 
						lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabela + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
						{0}lov_crt_projeto_geo_sit_proce lcp where t.tipo = lc.id and t.projeto = tf.projeto(+) and t.tipo = tf.tipo(+) 
						and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.projeto = :projeto and t.valido = 1 and t.tipo <> 5 order by lc.id", EsquemaBanco, EsquemaBancoGeo);
				}

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

						if (reader["fila_tipo"] != null && !Convert.IsDBNull(reader["fila_tipo"]))
						{
							arq.FilaTipo = Convert.ToInt32(reader["fila_tipo"]);
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

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}
			return arquivos;
		}

		public List<ArquivoProjeto> ObterArquivos(int empreendimento, eCaracterizacao caracterizacao, BancoDeDados banco = null, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			string tabelaProjeto = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";
			string tabelaArquivo = finalizado ? "crt_projeto_geo_arquivos" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos

				Comando comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, tf.tipo fila_tipo, t.arquivo, t.valido, 
				lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabelaProjeto + " p, {0}" + tabelaArquivo + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
				{0}lov_crt_projeto_geo_sit_proce lcp where p.id = t.projeto and t.tipo = lc.id and t.projeto = tf.projeto(+) and t.tipo = tf.tipo(+) 
				and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.valido = 1 and t.tipo <> 5 and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao 
				order by lc.id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)caracterizacao, DbType.Int32);

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

						if (reader["fila_tipo"] != null && !Convert.IsDBNull(reader["fila_tipo"]))
						{
							arq.FilaTipo = Convert.ToInt32(reader["fila_tipo"]);
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

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}
			return arquivos;
		}

		public List<ArquivoProjeto> ObterArquivosHistorico(int projetoId, string projetoTid, BancoDeDados banco = null)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos

				Comando comando = bancoDeDados.CriarComando(@"select tf.id, t.tipo_id, t.tipo_texto, tf.tipo fila_tipo, t.arquivo_id, t.valido, lcp.id situacao_id, 
				lcp.texto situacao_texto from {0}hst_crt_projeto_geo_arquivos t, {1}tab_fila tf, {0}lov_crt_projeto_geo_sit_proce lcp
				where t.projeto_id = tf.projeto(+) and t.tipo_id = tf.tipo(+) and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.valido = 1 and t.tipo_id <> 5 
				and t.projeto_id = :projeto and t.projeto_tid = :projeto_tid order by t.tipo_id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_tid", DbType.String, 36, projetoTid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();
						arq.IdRelacionamento = reader.GetValue<int>("id");
						arq.Tipo = reader.GetValue<int>("tipo_id");
						arq.Nome = reader.GetValue<string>("tipo_texto");
						arq.FilaTipo = reader.GetValue<int>("fila_tipo");
						arq.Id = reader.GetValue<int?>("arquivo_id");
						arq.Situacao = reader.GetValue<int>("situacao_id");
						arq.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						arq.isValido = reader.GetValue<bool>("valido");
						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}

			return arquivos;
		}

		public List<ArquivoProjeto> ObterOrtofotos(int id, BancoDeDados banco = null, bool finalizado = false, bool todos = true)
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

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);

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

		internal List<Sobreposicao> ObterSobreposicoes(int id, BancoDeDados banco = null, bool finalizado = false)
		{
			List<Sobreposicao> sobreposicoes = new List<Sobreposicao>();

			string tabela = finalizado ? "crt_projeto_geo_sobrepos" : "tmp_projeto_geo_sobrepos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Sobreposicao

				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.base, s.tipo, s.identificacao, s.tid 
					from {0}" + tabela + @" s where s.projeto = :projeto", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);

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
				where t.situacao = lc.situacao(+) and t.etapa = lc.etapa(+) and t.projeto = cpg.projeto(+) and t.tipo = cpg.tipo(+) and t.id = :arquivo_id", EsquemaBanco, EsquemaBancoGeo);

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

		internal int ObterSituacaoProjetoGeografico(int projetoId, BancoDeDados banco = null)
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

		#endregion

		#region Obter Geo

		internal Sobreposicao ObterGeoSobreposicaoIdaf(int id, eCaracterizacao tipo, BancoDeDados banco = null)
		{
			Sobreposicao sobreposicao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Sobreposicao Empreendimento [ATP]

				Comando comando = null;
				int projetoId = id;

				if (tipo != eCaracterizacao.Dominialidade)
				{
					comando = bancoDeDados.CriarComando(@"select d.id projeto_dom 
						from {0}crt_projeto_geo d  
						where d.caracterizacao = 1 
						and d.empreendimento = (select ent.empreendimento 
												from {0}crt_projeto_geo ent 
												where ent.id = :projeto_ativ )", EsquemaBanco);
					comando.AdicionarParametroEntrada("projeto_ativ", id, DbType.Int32);
					projetoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				}

				comando = bancoDeDados.CriarComando(@"select e.id, seg.texto segmento, e.denominador 
				from {0}tab_empreendimento e, {0}crt_projeto_geo pg, {0}lov_empreendimento_segmento seg 
					where pg.empreendimento = e.id and 
					e.segmento = seg.id and
					pg.id <> :projeto and 
					pg.id in (select atp.projeto 
								from {1}tmp_atp a, {1}geo_atp atp 
							where a.projeto = :projeto and sdo_relate(atp.geometry, a.geometry, 'MASK=ANYINTERACT') = 'TRUE' )", EsquemaBanco, EsquemaBancoGeo);


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

		internal string ObterWktATP(int id, eCaracterizacao tipo, BancoDeDados banco = null)
		{
			string wkt = string.Empty;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region ObterWkt

				if (tipo == eCaracterizacao.Dominialidade)
				{
					comando = bancoDeDados.CriarComando(@"select mdsys.sdo_util.to_wktgeometry(a.geometry) wkt from {0}tmp_atp a where a.projeto = :projeto", EsquemaBancoGeo);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select mdsys.sdo_util.to_wktgeometry(a.geometry) wkt from {1}geo_atp a where a.projeto = (
						select c.id from {0}crt_projeto_geo c 
						where c.caracterizacao = 1 and c.empreendimento = 
						(select g.empreendimento from {0}tmp_projeto_geo g where g.id = :projeto))", EsquemaBanco, EsquemaBancoGeo);
				}

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["wkt"] != null && !Convert.IsDBNull(reader["wkt"]))
						{
							wkt = reader["wkt"].ToString();
						}
					}

					reader.Close();
				}
				#endregion
			}
			return wkt;
		}

		#endregion

		#region Validações

		internal int ExisteProjetoGeografico(int empreendimentoId, int caracterizacaoTipo, bool finalizado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select t.id from {0}tmp_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo),
				(select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo)) projeto from dual", EsquemaBanco);

				if (finalizado)
				{
					comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", caracterizacaoTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal int ExisteArquivoFila(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_fila t where t.projeto = :projeto and t.tipo = :tipo and rownum = 1" + (arquivo.TituloId > 0 ? " and t.titulo = :titulo" : ""),
					EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", arquivo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", arquivo.FilaTipo, DbType.Int32);
				if (arquivo.TituloId > 0)
					comando.AdicionarParametroEntrada("titulo", arquivo.TituloId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal bool VerificarProjetoGeograficoProcessado(int projetoId, eCaracterizacao tipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_fila t where t.projeto = :projeto and t.etapa = 3 and t.situacao = 4 and t.tipo = :tipo", EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				if (tipo == eCaracterizacao.Dominialidade)
					comando.AdicionarParametroEntrada("tipo", (int)eFilaTipoGeo.Dominialidade, DbType.Int32);
				else if (tipo == eCaracterizacao.RegularizacaoFundiaria)
					comando.AdicionarParametroEntrada("tipo", (int)eFilaTipoGeo.RegularizacaoFundiaria, DbType.Int32);
				else
					comando.AdicionarParametroEntrada("tipo", (int)eFilaTipoGeo.Atividade, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ValidarProjetoGeograficoTemporario(int projetoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tmp_projeto_geo t where t.id = :projeto", EsquemaBanco);
				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal List<string> VerificarExcluirDominios(int empreendimentoID)
		{
			List<string> retorno = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select (case when c.tipo = 1 then 'Matricula' else 'Posse' end) || ' - ' || c.identificacao
				from {0}crt_dominialidade_dominio c
				where c.dominialidade = (select d.id from {0}crt_dominialidade d where d.empreendimento = :empreendimento)
				and exists (select 1 from {0}crt_dominialidade_reserva r where r.matricula = c.id)
				and c.identificacao not in (select a.nome from {1}tmp_apmp a 
				where a.projeto = (select p.id from {0}tmp_projeto_geo p where p.caracterizacao = 1 and p.empreendimento = :empreendimento))", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);

				retorno = bancoDeDados.ExecutarList<string>(comando);
			}

			return retorno;
		}

		#endregion

		internal Empreendimento ObterEmpreendimentoCoordenada(int empreendimentoId)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Coordenada

				Comando comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, aec.datum, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec, 
				aec.forma_coleta, aec.local_coleta from {0}tab_empreendimento_coord aec where aec.empreendimento = :empreendimentoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoid", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Coordenada.Id = Convert.ToInt32(reader["id"]);
						empreendimento.Coordenada.Tid = reader["tid"].ToString();

						if (!Convert.IsDBNull(reader["easting_utm"]))
						{
							empreendimento.Coordenada.EastingUtm = Convert.ToDouble(reader["easting_utm"]);
							empreendimento.Coordenada.EastingUtmTexto = empreendimento.Coordenada.EastingUtm.ToString();
						}

						if (!Convert.IsDBNull(reader["northing_utm"]))
						{
							empreendimento.Coordenada.NorthingUtm = Convert.ToDouble(reader["northing_utm"]);
							empreendimento.Coordenada.NorthingUtmTexto = empreendimento.Coordenada.NorthingUtm.ToString();
						}

						empreendimento.Coordenada.FusoUtm = Convert.IsDBNull(reader["fuso_utm"]) ? 0 : Convert.ToInt32(reader["fuso_utm"]);
						empreendimento.Coordenada.HemisferioUtm = Convert.IsDBNull(reader["hemisferio_utm"]) ? 0 : Convert.ToInt32(reader["hemisferio_utm"]);
						empreendimento.Coordenada.LatitudeGms = reader["latitude_gms"].ToString();
						empreendimento.Coordenada.LongitudeGms = reader["longitude_gms"].ToString();

						if (!Convert.IsDBNull(reader["latitude_gdec"]))
						{
							empreendimento.Coordenada.LatitudeGdec = Convert.ToDouble(reader["latitude_gdec"]);
							empreendimento.Coordenada.LatitudeGdecTexto = empreendimento.Coordenada.LatitudeGdec.ToString();
						}

						if (!Convert.IsDBNull(reader["longitude_gdec"]))
						{
							empreendimento.Coordenada.LongitudeGdec = Convert.ToDouble(reader["longitude_gdec"]);
							empreendimento.Coordenada.LongitudeGdecTexto = empreendimento.Coordenada.LongitudeGdec.ToString();
						}

						empreendimento.Coordenada.Datum.Id = Convert.ToInt32(reader["datum"]);
						empreendimento.Coordenada.Tipo.Id = Convert.ToInt32(reader["tipo_coordenada"]);

						if (!Convert.IsDBNull(reader["forma_coleta"]))
						{
							empreendimento.Coordenada.FormaColeta = Convert.ToInt32(reader["forma_coleta"]);
						}

						if (!Convert.IsDBNull(reader["local_coleta"]))
						{
							empreendimento.Coordenada.LocalColeta = Convert.ToInt32(reader["local_coleta"]);
						}

					}

					reader.Close();
				}

				#endregion
			}

			return empreendimento;
		}

		internal void CopiarDadosCredenciado(ProjetoGeografico projetoGeo, int empreendimentoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare 
					v_projetoGeoID number(38) := 0;
				begin 
					select nvl(nvl((select t.id from {0}tmp_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo),
					(select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo)), {0}seq_tmp_projeto_geo.nextval) into v_projetoGeoID from dual;

					for i in (select v_projetoGeoID id, :tid tid, :empreendimento empreendimento, i.caracterizacao, 2 situacao_finalizada, 
						i.nivel_precisao, i.mecanismo_elaboracao, i.sobreposicoes_data, i.menor_x, i.menor_y, i.maior_x, i.maior_y 
						from cre_projeto_geo i where i.id = :projetoGeoID) loop

						-- Apaga os dados - Oficial
						delete from {0}crt_projeto_geo_ortofoto t where t.projeto = i.id;
						delete from {0}crt_projeto_geo_sobrepos t where t.projeto = i.id;
						delete from {0}crt_projeto_geo_arquivos t where t.projeto = i.id;
						delete from {0}crt_projeto_geo t where t.id =  i.id;

						-- Apaga os dados - Rascunho
						delete {0}tmp_projeto_geo_arquivos g where g.projeto = i.id;
						delete {0}tmp_projeto_geo_ortofoto g where g.projeto = i.id;
						delete {0}tmp_projeto_geo_sobrepos g where g.projeto = i.id;
						delete {0}tmp_projeto_geo g where g.id = i.id;

						-- Inserindo na tabela oficial
						insert into {0}crt_projeto_geo (id, tid, empreendimento, caracterizacao, situacao, nivel_precisao, 
						mecanismo_elaboracao, sobreposicoes_data, menor_x, menor_y, maior_x, maior_y) 
						values (i.id, i.tid, i.empreendimento, i.caracterizacao, i.situacao_finalizada, i.nivel_precisao, 
						i.mecanismo_elaboracao, i.sobreposicoes_data, i.menor_x, i.menor_y, i.maior_x, i.maior_y);

						-- Inserindo os arquivos - ortofoto
						insert into {0}crt_projeto_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) 
						(select {0}seq_tmp_projeto_geo_ortofoto.nextval, i.id, caminho, chave, chave_data, i.tid 
						from cre_projeto_geo_ortofoto t where t.projeto = :projetoGeoID);

						-- Inserindo Sobreposicoes
						insert into {0}crt_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
						(select {0}seq_tmp_projeto_geo_sobrepos.nextval, i.id, base, tipo, identificacao, i.tid 
						from cre_projeto_geo_sobrepos t where t.projeto = :projetoGeoID);

						--Importa as tabelas do da temporária para as tabelas oficiais
						{1}operacoesprocessamentogeo.copiarcredenciadogeo(:projetoGeoID, v_projetoGeoID, i.tid);

						select v_projetoGeoID into :projeto_geo_id from dual;
					end loop;
					-----------------------------------------------------------------------------------------------
				end; ", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", projetoGeo.CaracterizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("projetoGeoID", projetoGeo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("projeto_geo_id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				projetoGeo.Id = Convert.ToInt32(comando.ObterValorParametro("projeto_geo_id"));
				projetoGeo.Tid = GerenciadorTransacao.ObterIDAtual();

				bancoDeDados.Commit();
			}
		}

		internal void SalvarArquivosCredenciado(ProjetoGeografico projetoGeo, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}crt_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) 
				values ({0}seq_tmp_projeto_geo_arquivos.nextval, :projeto, :tipo, :arquivo, :valido, :tid)", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", projetoGeo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", DbType.Int32);
				comando.AdicionarParametroEntrada("valido", DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				foreach (var item in projetoGeo.Arquivos)
				{
					comando.SetarValorParametro("tipo", item.Tipo);
					comando.SetarValorParametro("arquivo", item.Id);
					comando.SetarValorParametro("valido", 1);//Válido
					bancoDeDados.ExecutarNonQuery(comando);
				}

				bancoDeDados.Commit();
			}
		}
	}
}