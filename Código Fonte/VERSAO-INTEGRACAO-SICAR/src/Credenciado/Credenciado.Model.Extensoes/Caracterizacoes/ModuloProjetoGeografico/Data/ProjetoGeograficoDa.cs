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
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data
{
	public class ProjetoGeograficoDa
	{
		#region Propriedades

		private GerenciadorConfiguracao _config;

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }

		private String EsquemaBancoInterno
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public string EsquemaBancoInternoGeo { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }


		private String EsquemaBancoCredenciado
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private String EsquemaBancoCredenciadoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciadoGeo); }
		}

		#endregion

		public ProjetoGeograficoDa()
		{
			_config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		}

		#region Ações de DML

		internal void Salvar(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			if (projetoGeo == null)
			{
				throw new Exception("O Projeto geográfico é nulo.");
			}

			projetoGeo.CorrigirMbr();

			if (projetoGeo.Id <= 0)
			{
				Criar(projetoGeo, banco);
			}
			else
			{
				Editar(projetoGeo, banco);
			}
		}

		internal int? Criar(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo p (id, empreendimento, caracterizacao, situacao, nivel_precisao, mecanismo_elaboracao, 
				menor_x, menor_y, maior_x, maior_y, tid) values ({0}seq_tmp_projeto_geo.nextval, :empreendimento, :caracterizacao, 1, :nivel_precisao, :mecanismo_elaboracao, 
				:menor_x, :menor_y, :maior_x, :maior_y, :tid) returning p.id into :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("empreendimento", projetoGeo.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", projetoGeo.CaracterizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nivel_precisao", projetoGeo.NivelPrecisaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo_elaboracao", projetoGeo.MecanismoElaboracaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("menor_x", projetoGeo.MenorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("menor_y", projetoGeo.MenorY, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_x", projetoGeo.MaiorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_y", projetoGeo.MaiorY, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				projetoGeo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				AlterarArquivosOrtofoto(projetoGeo, bancoDeDados);

				AlterarSobreposicoes(projetoGeo, bancoDeDados);

				bancoDeDados.Commit();

				return projetoGeo.Id;
			}
		}

		internal void Editar(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo p set p.situacao = :situacao, p.nivel_precisao = :nivel_precisao, mecanismo_elaboracao = :mecanismo_elaboracao, 
				p.menor_x = :menor_x, p.menor_y = :menor_y, p.maior_x = :maior_x, p.maior_y = :maior_y where p.id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", projetoGeo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", projetoGeo.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo_elaboracao", projetoGeo.MecanismoElaboracaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nivel_precisao", projetoGeo.NivelPrecisaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("menor_x", projetoGeo.MenorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("menor_y", projetoGeo.MenorY, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_x", projetoGeo.MaiorX, DbType.Decimal);
				comando.AdicionarParametroEntrada("maior_y", projetoGeo.MaiorY, DbType.Decimal);
				
				bancoDeDados.ExecutarNonQuery(comando);

				AlterarArquivosOrtofoto(projetoGeo, bancoDeDados);

				AlterarSobreposicoes(projetoGeo, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimentoID, int caracterizacaoTipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				#region Histórico - Atualizar TID

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_projeto_geo c set c.tid = :tid 
				where c.empreendimento = :empreendimento and c.caracterizacao = :caracterizacao returning c.id into :id", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				if (comando.LinhasAfetadas == 0)
				{
					return;
				}

				int projetoGeoID = Convert.ToInt32(comando.ObterValorParametro("id"));

				Historico.Gerar(projetoGeoID, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.excluir, bancoDeDados, null);

				Historico.GerarGeo(projetoGeoID, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.excluir, bancoDeDados, null);

				#region Apaga os dados do projeto

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					{1}operacoesprocessamentogeo.ApagarGeometriasTMP(:projeto, :fila_tipo);
					{1}operacoesprocessamentogeo.ApagarGeometriasDES(:projeto, :fila_tipo);
					{1}operacoesprocessamentogeo.ApagarGeometriasTrackmaker(:projeto, :fila_tipo);
					{1}operacoesprocessamentogeo.ApagarGeometriasOficial(:projeto, :fila_tipo);
					
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
				end;", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", caracterizacaoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.ProjetoGeografico, DbType.Int32);
				comando.AdicionarParametroEntrada("fila_tipo", (int)((caracterizacaoTipo == (int)eCaracterizacao.Dominialidade) ? eFilaTipoGeo.Dominialidade : eFilaTipoGeo.Atividade), DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void AlterarArquivosOrtofoto(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				#region Limpar os dados do banco

				Comando comando = bancoDeDados.CriarComando("delete from {0}tmp_projeto_geo_ortofoto c where c.projeto = :projeto", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("projeto", projetoGeo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Arquivos Ortofoto

				comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) values 
				({0}seq_tmp_projeto_geo_ortofoto.nextval, :projeto, :caminho, :chave, :chave_data, :tid)", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto", projetoGeo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("caminho", DbType.String, 500);
				comando.AdicionarParametroEntrada("chave", DbType.String, 500);
				comando.AdicionarParametroEntrada("chave_data", DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				foreach (var item in projetoGeo.ArquivosOrtofotos)
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

		internal void AlterarSobreposicoes(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				#region Limpar os dados do banco

				Comando comando = bancoDeDados.CriarComando("delete from {0}tmp_projeto_geo_sobrepos c where c.projeto = :projeto", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("projeto", projetoGeo.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				if (projetoGeo.Sobreposicoes.Itens != null && projetoGeo.Sobreposicoes.Itens.Count > 0)
				{
					#region Sobreposicoes data

					comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo c set c.sobreposicoes_data = :sobreposicoes_data where c.id = :projeto", EsquemaBancoCredenciado);
					comando.AdicionarParametroEntrada("projeto", projetoGeo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("sobreposicoes_data", projetoGeo.Sobreposicoes.DataVerificacaoBanco.Data, DbType.DateTime);

					bancoDeDados.ExecutarNonQuery(comando);

					#endregion

					#region Sobreposicoes

					foreach (var item in projetoGeo.Sobreposicoes.Itens)
					{
						if (string.IsNullOrEmpty(item.Identificacao) || item.Identificacao.Trim() == "-")
						{
							continue;
						}

						comando = bancoDeDados.CriarComando(@"
						insert into {0}tmp_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) values 
						({0}seq_tmp_projeto_geo_sobrepos.nextval, :projeto, :base, :tipo, :identificacao, :tid)", EsquemaBancoCredenciado);

						comando.AdicionarParametroEntrada("projeto", projetoGeo.Id, DbType.Int32);
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

		internal void Finalizar(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo c set c.tid = :tid where c.id = :id", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("id", projetoGeo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					for i in (select i.id, i.empreendimento, i.caracterizacao, i.situacao, i.nivel_precisao, i.mecanismo_elaboracao, i.sobreposicoes_data, 
						i.menor_x, i.menor_y, i.maior_x, i.maior_y, i.tid, i.interno_id, i.interno_tid from tmp_projeto_geo i where i.id = :id) 
					loop
						-- Apaga os dados antigos
						delete from {0}crt_projeto_geo_ortofoto t where t.projeto = i.id;
						delete from {0}crt_projeto_geo_sobrepos t where t.projeto = i.id;
						delete from {0}crt_projeto_geo_arquivos t where t.projeto = i.id;
						delete from {0}crt_projeto_geo t where t.id =  i.id;

						-- Inserindo na tabela oficial (2 - Finalizado)
						insert into {0}crt_projeto_geo (id, empreendimento, caracterizacao, situacao, nivel_precisao, mecanismo_elaboracao, 
						sobreposicoes_data, menor_x, menor_y, maior_x, maior_y, tid, interno_id, interno_tid, alterado_copiar) 
						values (i.id, i.empreendimento, i.caracterizacao, 2, i.nivel_precisao, i.mecanismo_elaboracao, 
						i.sobreposicoes_data, i.menor_x, i.menor_y, i.maior_x, i.maior_y, i.tid, i.interno_id, i.interno_tid, :alterado_copiar);

						-- Inserindo os arquivos
						insert into {0}crt_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid)
						(select id, projeto, tipo, arquivo, valido, i.tid from {0}tmp_projeto_geo_arquivos t where t.projeto = i.id);

						-- Inserindo os arquivos: ortofoto
						insert into {0}crt_projeto_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) 
						(select id, projeto, caminho, chave, chave_data, i.tid from {0}tmp_projeto_geo_ortofoto t where t.projeto = i.id);

						-- Inserindo Sobreposicoes
						insert into {0}crt_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
						(select id, projeto, base, tipo, identificacao, i.tid from {0}tmp_projeto_geo_sobrepos t where t.projeto = i.id);

						--Importa as tabelas do da temporária para as tabelas oficiais
						{1}operacoesprocessamentogeo.ExportarParaTabelasGEO(i.id, i.tid);

						--Apaga o rascunho
						delete {0}tmp_projeto_geo_arquivos g where g.projeto = i.id;
						delete {0}tmp_projeto_geo_ortofoto g where g.projeto = i.id;
						delete {0}tmp_projeto_geo_sobrepos g where g.projeto = i.id;
						delete {0}tmp_projeto_geo g where g.id = i.id;
					end loop;
					-----------------------------------------------------------------------------------------------
				end; ", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("id", projetoGeo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("alterado_copiar", projetoGeo.InternoID > 0 ? 1 : (Object)DBNull.Value, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(projetoGeo.Id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.finalizar, bancoDeDados, null);

				Historico.GerarGeo(projetoGeo.Id, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.finalizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void Refazer(int projetoGeoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare 
					v_fila_tipo number := 0;
					v_caract_tipo number := 0;
					v_mecanismo number := 0;
				begin 
					delete from {0}tmp_projeto_geo_ortofoto t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_sobrepos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo_arquivos t where t.projeto = :projeto; 
					delete from {0}tmp_projeto_geo t where t.id = :projeto; 

					insert into {0}tmp_projeto_geo (id, empreendimento, caracterizacao, situacao, nivel_precisao, 
					mecanismo_elaboracao, sobreposicoes_data, menor_x, menor_y, maior_x, maior_y, tid, interno_id, interno_tid) 
					(select g.id, g.empreendimento, g.caracterizacao, 4, g.nivel_precisao, g.mecanismo_elaboracao, g.sobreposicoes_data, 
					g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid, g.interno_id, g.interno_tid from {0}crt_projeto_geo g where g.id = :projeto); 

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

					{1}operacoesprocessamentogeo.ApagarGeometriasTMP(:projeto, v_fila_tipo);
					{1}operacoesprocessamentogeo.ApagarGeometriasDES(:projeto, v_fila_tipo);
					{1}operacoesprocessamentogeo.ApagarGeometriasTrackmaker(:projeto, v_fila_tipo);

					{1}operacoesprocessamentogeo.ImportarDoFinalizado(:projeto, v_fila_tipo);

					if v_mecanismo = :mecDesenhador then 
						{1}operacoesprocessamentogeo.ImportarParaDesenhFinalizada(:projeto, v_fila_tipo);
					end if;
				end; ", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("mecDesenhador", (int)eProjetoGeograficoMecanismo.Desenhador, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidadeTipo", (int)eCaracterizacao.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoDominialidade", (int)eFilaTipoGeo.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoAtividade", (int)eFilaTipoGeo.Atividade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void ExcluirRascunho(int projetoGeoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
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
					v_fila_tipo := :filaTipoAtividade;
					if v_caract_tipo = :dominialidadeTipo then 
						v_fila_tipo := :filaTipoDominialidade;
					end if;

					{1}operacoesprocessamentogeo.ApagarGeometriasTMP(:projeto, v_fila_tipo);
					{1}operacoesprocessamentogeo.ApagarGeometriasDES(:projeto, v_fila_tipo);
					{1}operacoesprocessamentogeo.ApagarGeometriasTrackmaker(:projeto, v_fila_tipo);
				end; ", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("dominialidadeTipo", (int)eCaracterizacao.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoDominialidade", (int)eFilaTipoGeo.Dominialidade, DbType.Int32);
				comando.AdicionarParametroEntrada("filaTipoAtividade", (int)eFilaTipoGeo.Atividade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void AtualizarArquivosEnviado(ArquivoProjeto arquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos a set a.valido = 0 where a.tipo > :tipo and a.projeto = :projeto", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("projeto", arquivo.Processamento.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos a set a.valido = 1 where a.projeto = :projeto and a.tipo = :tipo", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("projeto", arquivo.Processamento.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				if (comando.LinhasAfetadas == 0 && arquivo.Id > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tmp_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) values
					({0}seq_tmp_projeto_geo_arquivos.nextval, :projeto, :tipo, :arquivo, 1, :tid)", EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("projeto", arquivo.Processamento.ProjetoId, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", arquivo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo", (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}
			}
		}

		internal void InvalidarArquivoProcessados(int projetoGeoID, List<int> arquivos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tmp_projeto_geo_arquivos t set t.valido = 0 where t.projeto = :projeto", EsquemaBancoCredenciado);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "t.tipo", DbType.Int32, arquivos);
				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void ExcluirArquivos(int projetoGeoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete from {0}tmp_projeto_geo_arquivos a where a.projeto = :projeto", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		internal void ExcluirArquivoDuplicados(int projetoGeoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"delete from {0}tmp_projeto_geo_arquivos a where a.projeto = :projeto and a.id <> 
				(select max(b.id) from {0}tmp_projeto_geo_arquivos b where b.projeto = a.projeto and b.tipo = :tipo)", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", (int)eProjetoGeograficoArquivoTipo.ArquivoEnviado, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void CopiarDadosInstitucional(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
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
						from ins_projeto_geo i where i.id = :projetoGeoInternoID) loop

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
						insert into {0}crt_projeto_geo (id, tid, interno_id, interno_tid, empreendimento, caracterizacao, situacao, nivel_precisao, 
						mecanismo_elaboracao, sobreposicoes_data, menor_x, menor_y, maior_x, maior_y, alterado_copiar) 
						values (i.id, i.tid, :projetoGeoInternoID, :projetoGeoInternoTID, i.empreendimento, i.caracterizacao, i.situacao_finalizada, i.nivel_precisao, 
						i.mecanismo_elaboracao, i.sobreposicoes_data, i.menor_x, i.menor_y, i.maior_x, i.maior_y, 0);

						-- Inserindo os arquivos: ortofoto
						insert into {0}crt_projeto_geo_ortofoto (id, projeto, caminho, chave, chave_data, tid) 
						(select seq_tmp_projeto_geo_ortofoto.nextval, i.id, caminho, chave, chave_data, i.tid 
						from ins_projeto_geo_ortofoto t where t.projeto = :projetoGeoInternoID);

						-- Inserindo Sobreposicoes
						insert into {0}crt_projeto_geo_sobrepos (id, projeto, base, tipo, identificacao, tid) 
						(select seq_tmp_projeto_geo_sobrepos.nextval, i.id, base, tipo, identificacao, i.tid 
						from ins_projeto_geo_sobrepos t where t.projeto = :projetoGeoInternoID);

						--Importa as tabelas do da temporária para as tabelas oficiais
						{1}operacoesprocessamentogeo.copiarinstitucionalgeo(:projetoGeoInternoID, v_projetoGeoID, i.tid);

						select v_projetoGeoID into :projeto_geo_id from dual;
					end loop;
					-----------------------------------------------------------------------------------------------
				end; ", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("empreendimento", projetoGeo.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", projetoGeo.CaracterizacaoId, DbType.Int32);

				comando.AdicionarParametroEntrada("projetoGeoInternoID", projetoGeo.InternoID, DbType.Int32);
				comando.AdicionarParametroEntrada("projetoGeoInternoTID", DbType.String, 36, projetoGeo.InternoTID);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("projeto_geo_id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				projetoGeo.Id = Convert.ToInt32(comando.ObterValorParametro("projeto_geo_id"));

				bancoDeDados.Commit();
			}
		}

		internal void SalvarArquivosInstitucional(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}crt_projeto_geo_arquivos (id, projeto, tipo, arquivo, valido, tid) 
				values (seq_tmp_projeto_geo_arquivos.nextval, :projeto, :tipo, :arquivo, :valido, :tid)", EsquemaBancoCredenciado);

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

		#endregion

		#region Ações de DML da base GEO

		internal void InserirFila(ProcessamentoGeo processamentoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {1}tab_fila f (id, empreendimento, projeto, tipo, mecanismo_elaboracao, etapa, situacao, data_fila)
				(select {1}seq_fila.nextval, t.empreendimento, t.id, :tipo, :mecanismo_elaboracao, :etapa, :situacao, sysdate from {0}tmp_projeto_geo t where t.id = :projeto)",
				EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", processamentoGeo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", processamentoGeo.FilaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo_elaboracao", processamentoGeo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", processamentoGeo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", processamentoGeo.Situacao, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"select f.id from {0}tab_fila f where f.projeto = :projeto and f.tipo = :tipo", EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", processamentoGeo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", processamentoGeo.FilaTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					processamentoGeo.Id = Convert.ToInt32(valor);
				}

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacaoFila(ProcessamentoGeo processamentoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					update {1}tmp_projeto_geo tt set tt.mecanismo_elaboracao = :mecanismo where tt.id = :projeto;
					update {0}tab_fila t set t.etapa = :etapa, t.situacao = :situacao, t.data_fila = sysdate, t.data_inicio = null, t.data_fim = null, 
					t.mecanismo_elaboracao = :mecanismo where t.projeto = :projeto and t.tipo = :tipo returning t.id into :id; 
				end;", EsquemaBancoCredenciadoGeo, EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto", processamentoGeo.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", processamentoGeo.FilaTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("mecanismo", processamentoGeo.Mecanismo, DbType.Int32);
				comando.AdicionarParametroEntrada("etapa", processamentoGeo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", processamentoGeo.Situacao, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				processamentoGeo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				bancoDeDados.Commit();
			}
		}

		internal void InvalidarFila(int projetoGeoID, List<int> processamentosGeoTipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_fila f set f.etapa = 1, f.situacao = 5 where f.projeto = :projeto", EsquemaBancoCredenciadoGeo);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "f.tipo", DbType.Int32, processamentosGeoTipo);
				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"update {0}tab_fila f set f.situacao = 5 where f.projeto = :projeto", EsquemaBancoCredenciadoGeo);
				comando.DbCommand.CommandText += comando.AdicionarIn("and", "f.tipo", DbType.Int32, processamentosGeoTipo);
				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal ProjetoGeografico Obter(int projetoGeoID, BancoDeDados banco = null, bool simplificado = false, bool finalizado = false)
		{
			ProjetoGeografico projetoGeo = new ProjetoGeografico();
			
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
                projetoGeo = finalizado ? ObterFinalizado(projetoGeoID, bancoDeDados) : ObterTemporario(projetoGeoID, bancoDeDados);

				#region Arquivos

				if (projetoGeo.Id <= 0 || simplificado)
				{
					return projetoGeo;
				}

				if (projetoGeo.Id > 0)
				{
					projetoGeo.Arquivos = ObterArquivos(projetoGeo.Id, banco: bancoDeDados, finalizado: finalizado);

					projetoGeo.ArquivosOrtofotos = ObterOrtofotos(projetoGeo.Id, banco: bancoDeDados, finalizado: finalizado);

					if (projetoGeo.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador)
					{
						projetoGeo.ArquivoEnviadoDesenhador = ObterArquivoDesenhador(projetoGeo, banco);
					}

					projetoGeo.Sobreposicoes.Itens = ObterSobreposicoes(projetoGeo.Id, bancoDeDados, finalizado);
				}

				#endregion
			}

			return projetoGeo;
		}

        private ProjetoGeografico ObterTemporario(int projetoGeoId, BancoDeDados banco = null) 
        {
            ProjetoGeografico projetoGeo = new ProjetoGeografico();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.interno_id, g.interno_tid, g.empreendimento, lc.id caracterizacao_id, lc.texto caracterizacao_texto, 
				ls.id situacao_id, ls.texto situacao_texto, ln.id nivel_precisao_id, ln.texto nivel_precisao_texto, lm.id mecanismo_elaboracao_id, 
				lm.texto mecanismo_elaboracao_texto, g.sobreposicoes_data, g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid from {0}tmp_projeto_geo g, 
				{0}lov_caracterizacao_tipo lc, {0}lov_crt_projeto_geo_situacao ls, {0}lov_crt_projeto_geo_nivel ln, {0}lov_crt_projeto_geo_mecanismo lm 
				where g.caracterizacao = lc.id and g.situacao = ls.id and g.nivel_precisao = ln.id(+) and g.mecanismo_elaboracao = lm.id(+) and g.id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", projetoGeoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projetoGeo.Id = projetoGeoId;
						projetoGeo.Tid = reader.GetValue<string>("tid");
						projetoGeo.InternoID = reader.GetValue<int>("interno_id");
						projetoGeo.InternoTID = reader.GetValue<string>("interno_tid");
						projetoGeo.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						projetoGeo.CaracterizacaoId = reader.GetValue<int>("caracterizacao_id");
						projetoGeo.CaracterizacaoTexto = reader.GetValue<string>("caracterizacao_texto");
						projetoGeo.SituacaoId = reader.GetValue<int>("situacao_id");
						projetoGeo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						projetoGeo.NivelPrecisaoId = reader.GetValue<int>("nivel_precisao_id");
						projetoGeo.NivelPrecisaoTexto = reader.GetValue<string>("nivel_precisao_texto");
						projetoGeo.MecanismoElaboracaoId = reader.GetValue<int>("mecanismo_elaboracao_id");
						projetoGeo.MecanismoElaboracaoTexto = reader.GetValue<string>("mecanismo_elaboracao_texto");

						projetoGeo.MenorX = reader.GetValue<decimal>("menor_x");
						projetoGeo.MenorY = reader.GetValue<decimal>("menor_y");
						projetoGeo.MaiorX = reader.GetValue<decimal>("maior_x");
						projetoGeo.MaiorY = reader.GetValue<decimal>("maior_y");

						projetoGeo.CorrigirMbr();

						projetoGeo.Sobreposicoes.DataVerificacaoBanco.Data = reader.GetValue<DateTime>("sobreposicoes_data");
						projetoGeo.Sobreposicoes.DataVerificacao = projetoGeo.Sobreposicoes.DataVerificacaoBanco.Data.Value.ToString("dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
					    
                    }

					reader.Close();
				}
            }

            return projetoGeo;
        }

        private ProjetoGeografico ObterFinalizado(int projetoGeoId, BancoDeDados banco = null)
        {
            ProjetoGeografico projetoGeo = new ProjetoGeografico();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.interno_id, g.interno_tid, g.empreendimento, lc.id caracterizacao_id, lc.texto caracterizacao_texto, 
				ls.id situacao_id, ls.texto situacao_texto, ln.id nivel_precisao_id, ln.texto nivel_precisao_texto, lm.id mecanismo_elaboracao_id, 
				lm.texto mecanismo_elaboracao_texto, g.sobreposicoes_data, g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid, g.alterado_copiar from {0}crt_projeto_geo g, 
				{0}lov_caracterizacao_tipo lc, {0}lov_crt_projeto_geo_situacao ls, {0}lov_crt_projeto_geo_nivel ln, {0}lov_crt_projeto_geo_mecanismo lm 
				where g.caracterizacao = lc.id and g.situacao = ls.id and g.nivel_precisao = ln.id(+) and g.mecanismo_elaboracao = lm.id(+) and g.id = :id", EsquemaBancoCredenciado);

                comando.AdicionarParametroEntrada("id", projetoGeoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        projetoGeo.Id = projetoGeoId;
						projetoGeo.Tid = reader.GetValue<string>("tid");
						projetoGeo.InternoID = reader.GetValue<int>("interno_id");
						projetoGeo.InternoTID = reader.GetValue<string>("interno_tid");
                        projetoGeo.EmpreendimentoId = reader.GetValue<int>("empreendimento");
                        projetoGeo.CaracterizacaoId = reader.GetValue<int>("caracterizacao_id");
                        projetoGeo.CaracterizacaoTexto = reader.GetValue<string>("caracterizacao_texto");
                        projetoGeo.SituacaoId = reader.GetValue<int>("situacao_id");
                        projetoGeo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
                        projetoGeo.NivelPrecisaoId = reader.GetValue<int>("nivel_precisao_id");
                        projetoGeo.NivelPrecisaoTexto = reader.GetValue<string>("nivel_precisao_texto");
                        projetoGeo.MecanismoElaboracaoId = reader.GetValue<int>("mecanismo_elaboracao_id");
                        projetoGeo.MecanismoElaboracaoTexto = reader.GetValue<string>("mecanismo_elaboracao_texto");
                        projetoGeo.AlteradoCopiar = reader.GetValue<bool>("alterado_copiar");

                        projetoGeo.MenorX = reader.GetValue<decimal>("menor_x");
                        projetoGeo.MenorY = reader.GetValue<decimal>("menor_y");
                        projetoGeo.MaiorX = reader.GetValue<decimal>("maior_x");
                        projetoGeo.MaiorY = reader.GetValue<decimal>("maior_y");

                        projetoGeo.CorrigirMbr();

                        projetoGeo.Sobreposicoes.DataVerificacaoBanco.Data = reader.GetValue<DateTime>("sobreposicoes_data");
                        projetoGeo.Sobreposicoes.DataVerificacao = projetoGeo.Sobreposicoes.DataVerificacaoBanco.Data.Value.ToString("dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);

                    }

                    reader.Close();
                }
            }

            return projetoGeo;
        }
        
		internal ProjetoGeografico ObterHistorico(int projetoGeoID, string projetoGeoTID, BancoDeDados banco = null, bool simplificado = false)
		{
			ProjetoGeografico projetoGeo = new ProjetoGeografico();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"
				select g.id, g.interno_id, g.interno_tid, g.empreendimento_id, g.caracterizacao_id, g.caracterizacao_texto, g.situacao_id, g.situacao_texto, g.nivel_precisao_id, 
				g.nivel_precisao_texto, g.mecanismo_elaboracao_id, g.mecanismo_elaboracao_texto, g.sobreposicoes_data, g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid 
				from {0}hst_crt_projeto_geo g where g.projeto_geo_id = :id and g.tid = :tid", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, projetoGeoTID);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projetoGeo.Id = projetoGeoID;
						projetoGeo.Tid = reader.GetValue<string>("tid");
						projetoGeo.InternoID = reader.GetValue<int>("interno_id");
						projetoGeo.InternoTID = reader.GetValue<string>("interno_tid");
						projetoGeo.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						projetoGeo.CaracterizacaoId = reader.GetValue<int>("caracterizacao_id");
						projetoGeo.CaracterizacaoTexto = reader.GetValue<string>("caracterizacao_texto");
						projetoGeo.SituacaoId = reader.GetValue<int>("situacao_id");
						projetoGeo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						projetoGeo.NivelPrecisaoId = reader.GetValue<int>("nivel_precisao_id");
						projetoGeo.NivelPrecisaoTexto = reader.GetValue<string>("nivel_precisao_texto");
						projetoGeo.MecanismoElaboracaoId = reader.GetValue<int>("mecanismo_elaboracao_id");
						projetoGeo.MecanismoElaboracaoTexto = reader.GetValue<string>("mecanismo_elaboracao_texto");

						projetoGeo.MenorX = reader.GetValue<decimal>("menor_x");
						projetoGeo.MenorY = reader.GetValue<decimal>("menor_y");
						projetoGeo.MaiorX = reader.GetValue<decimal>("maior_x");
						projetoGeo.MaiorY = reader.GetValue<decimal>("maior_y");

						projetoGeo.CorrigirMbr();

						projetoGeo.Sobreposicoes.DataVerificacaoBanco.Data = reader.GetValue<DateTime>("sobreposicoes_data");
						projetoGeo.Sobreposicoes.DataVerificacao = projetoGeo.Sobreposicoes.DataVerificacaoBanco.Data.Value.ToString("dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
					}

					reader.Close();
				}

				#endregion

				#region Arquivos

				if (projetoGeo.Id <= 0 || simplificado)
				{
					return projetoGeo;
				}

				projetoGeo.Arquivos = ObterArquivosHistorico(projetoGeo.Id, projetoGeo.Tid, bancoDeDados);

				projetoGeo.ArquivosOrtofotos = ObterOrtofotosHistorico(projetoGeo.Id, projetoGeo.Tid, bancoDeDados);

				if (projetoGeo.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador)
				{
					//Sempre vai ser assim esse if, pois tab_fila nao possui historico
					projetoGeo.ArquivoEnviadoDesenhador.Id = 0;
					projetoGeo.ArquivoEnviadoDesenhador.Situacao = (int)eFilaSituacaoGeo.Concluido;
					projetoGeo.ArquivoEnviadoDesenhador.SituacaoTexto = eProjetoGeograficoSituacaoProcessamento.Processado.ToString();
				}

				projetoGeo.Sobreposicoes.Itens = ObterSobreposicoesHistorico(projetoGeo.Id, projetoGeo.Tid, bancoDeDados);

				#endregion
			}

			return projetoGeo;
		}


        /// <summary>
        /// Retorna apenas os campos  id, caracterizacao, situacao_id, situacao_texto e tid  do projeto geográfico
        /// </summary>
        /// <param name="projetoGeoID"></param>
        /// <param name="banco"></param>
        /// <param name="finalizado"></param>
        /// <returns></returns>
		internal ProjetoGeografico ObterProjetoGeografico(int projetoGeoID, BancoDeDados banco = null, bool finalizado = true)
		{
			ProjetoGeografico projetoGeo = new ProjetoGeografico();
			string tabela = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.caracterizacao, ls.id situacao_id, ls.texto situacao_texto, g.tid from {0}" + tabela + @" g, 
				{0}lov_crt_projeto_geo_situacao ls where g.situacao = ls.id and g.id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", projetoGeoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
                    if (reader.Read())
                    {
                        projetoGeo = new ProjetoGeografico();
                        projetoGeo.Id = projetoGeoID;
                        projetoGeo.Tid = reader.GetValue<string>("tid");
                        projetoGeo.CaracterizacaoId = reader.GetValue<int>("caracterizacao");
                        projetoGeo.SituacaoId = reader.GetValue<int>("situacao_id");
                        projetoGeo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
                    }

					reader.Close();
				}
			}

			return projetoGeo;
		}

		public List<ArquivoProjeto> ObterArquivos(int projetoGeoID, BancoDeDados banco = null, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();
			string tabela = finalizado ? "crt_projeto_geo_arquivos" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, tf.tipo fila_tipo, tf.etapa, t.arquivo, t.valido, 
				lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabela + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
				{0}lov_crt_projeto_geo_sit_proce lcp where t.tipo = lc.id and t.projeto = tf.projeto(+) and t.tipo = tf.tipo(+) 
				and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.projeto = :projeto and t.valido = 1 and t.tipo <> 5 order by lc.id", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						arq.Id = reader.GetValue<int>("arquivo");
						arq.Nome = reader.GetValue<string>("tipo_texto");
						arq.Tipo = reader.GetValue<int>("tipo");
						
						arq.Processamento.Id = reader.GetValue<int>("id");
						arq.Processamento.FilaTipo = reader.GetValue<int>("fila_tipo");
						arq.Processamento.Etapa = reader.GetValue<int>("etapa");

						arq.Processamento.Situacao = reader.GetValue<int>("situacao_id");
						arq.Processamento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						arq.Processamento.isValido = reader.GetValue<bool>("valido");

						arquivos.Add(arq);
					}

					reader.Close();
				}
			}

			return arquivos;
		}

		public List<ArquivoProjeto> ObterArquivos(int empreendimentoID, eCaracterizacao caracterizacaoTipo, BancoDeDados banco = null, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			string tabelaProjeto = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";
			string tabelaArquivo = finalizado ? "crt_projeto_geo_arquivos" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, tf.tipo fila_tipo, t.arquivo, t.valido, 
				lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabelaProjeto + " p, {0}" + tabelaArquivo + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
				{0}lov_crt_projeto_geo_sit_proce lcp where p.id = t.projeto and t.tipo = lc.id and t.projeto = tf.projeto(+) and t.tipo = tf.tipo(+) 
				and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.valido = 1 and t.tipo <> 5 and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao 
				order by lc.id", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)caracterizacaoTipo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						arq.Id = reader.GetValue<int>("arquivo");
						arq.Nome = reader.GetValue<string>("tipo_texto");
						arq.Tipo = reader.GetValue<int>("tipo");

						arq.Processamento.Id = reader.GetValue<int>("id");
						arq.Processamento.FilaTipo = reader.GetValue<int>("fila_tipo");
						arq.Processamento.Situacao = reader.GetValue<int>("situacao_id");
						arq.Processamento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						arq.Processamento.isValido = reader.GetValue<bool>("valido");

						arquivos.Add(arq);
					}

					reader.Close();
				}
			}

			return arquivos;
		}

		public List<ArquivoProjeto> ObterArquivosHistorico(int projetoGeoID, string projetoGeoTID, BancoDeDados banco = null)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tf.id, t.tipo_id, t.tipo_texto, tf.tipo fila_tipo, t.arquivo_id, t.valido, lcp.id situacao_id, 
				lcp.texto situacao_texto from {0}hst_crt_projeto_geo_arquivos t, {1}tab_fila tf, {0}lov_crt_projeto_geo_sit_proce lcp
				where t.projeto_id = tf.projeto(+) and t.tipo_id = tf.tipo(+) and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.valido = 1 and t.tipo_id <> 5 
				and t.projeto_id = :projeto and t.projeto_tid = :projeto_tid order by t.tipo_id", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_tid", DbType.String, 36, projetoGeoTID);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						arq.Id = reader.GetValue<int?>("arquivo_id");
						arq.Tipo = reader.GetValue<int>("tipo_id");
						arq.Nome = reader.GetValue<string>("tipo_texto");

						arq.Processamento.Id = reader.GetValue<int>("id");
						arq.Processamento.FilaTipo = reader.GetValue<int>("fila_tipo");
						arq.Processamento.Situacao = reader.GetValue<int>("situacao_id");
						arq.Processamento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						arq.Processamento.isValido = reader.GetValue<bool>("valido");

						/*Remover ao fazer refactor do institucional*/
						arq.FilaTipo = reader.GetValue<int>("fila_tipo");
						arq.Situacao = reader.GetValue<int>("situacao_id");
						arq.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						arq.isValido = reader.GetValue<bool>("valido");
						/*Remover ao fazer refactor do institucional*/

						arquivos.Add(arq);
					}

					reader.Close();
				}
			}

			return arquivos;
		}

		public List<ArquivoProjeto> ObterOrtofotos(int projetoGeoID, BancoDeDados banco = null, bool finalizado = false, bool todos = true)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();
			string tabela = finalizado ? "crt_projeto_geo_ortofoto" : "tmp_projeto_geo_ortofoto";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando;

				if (todos)
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.caminho, t.chave, t.chave_data, 'application/zip' tipo from {0}" + tabela + @" t 
					where t.projeto = :projeto", EsquemaBancoCredenciado);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.caminho, t.chave, t.chave_data, 'application/zip' tipo from {0}" + tabela + @" t 
					where t.id = :projeto", EsquemaBancoCredenciado);
				}

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					string diretorio = _config.Obter<Dictionary<int, string>>(ConfiguracaoSistema.KeyDiretorioOrtoFotoMosaico)[1];

					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						arq.Id = reader.GetValue<int>("id");
						arq.Nome = reader.GetValue<string>("caminho");
						arq.ContentType = reader.GetValue<string>("tipo");
						arq.Chave = reader.GetValue<string>("chave");
						arq.ChaveData = reader.GetValue<DateTime>("chave_data");
						arq.Caminho = diretorio + "\\" + reader.GetValue<string>("caminho");

						arq.Processamento.Situacao = (int)eProjetoGeograficoSituacaoProcessamento.Processado;

						arquivos.Add(arq);
					}

					reader.Close();
				}
			}

			return arquivos;
		}

		public List<ArquivoProjeto> ObterOrtofotosHistorico(int projetoGeoID, string projetoGeoTID, BancoDeDados banco = null)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.ortofoto_id id, t.caminho, t.chave, t.chave_data, 'application/zip' tipo 
				from {0}hst_crt_projeto_geo_ortofoto t where t.projeto_id = :projeto_id and t.projeto_tid = :projeto_tid", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto_id", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_tid", DbType.String, 36, projetoGeoTID);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					string diretorio = _config.Obter<Dictionary<int, string>>(ConfiguracaoSistema.KeyDiretorioOrtoFotoMosaico)[1];

					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						arq.Id = reader.GetValue<int>("id");
						arq.Nome = reader.GetValue<string>("caminho");
						arq.ContentType = reader.GetValue<string>("tipo");
						arq.Chave = reader.GetValue<string>("chave");
						arq.ChaveData = reader.GetValue<DateTime>("chave_data");
						arq.Caminho = diretorio + "\\" + reader.GetValue<string>("caminho");

						arq.Processamento.Situacao = (int)eProjetoGeograficoSituacaoProcessamento.Processado;

						arquivos.Add(arq);
					}

					reader.Close();
				}
			}

			return arquivos;
		}

		public ArquivoProjeto ObterArquivoDesenhador(ProjetoGeografico projetoGeo, BancoDeDados banco = null)
		{
			ArquivoProjeto arquivo = new ArquivoProjeto();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.id, lc.id situacao_id, lc.texto situacao_texto from {1}tab_fila t, {0}lov_crt_projeto_geo_sit_proce lc 
				where t.etapa = lc.etapa and t.situacao = lc.situacao and t.tipo in (3, 4) and t.mecanismo_elaboracao = 2 
				and t.projeto = :projeto", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						arquivo.Processamento.Id = reader.GetValue<int>("id");
						arquivo.Processamento.Situacao = reader.GetValue<int>("situacao_id");
						arquivo.Processamento.SituacaoTexto = reader.GetValue<string>("situacao_texto");
					}

					reader.Close();
				}
			}

			return arquivo;
		}

		internal List<Sobreposicao> ObterSobreposicoes(int projetoGeoID, BancoDeDados banco = null, bool finalizado = false)
		{
			List<Sobreposicao> sobreposicoes = new List<Sobreposicao>();
			string tabela = finalizado ? "crt_projeto_geo_sobrepos" : "tmp_projeto_geo_sobrepos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.base, s.tipo, s.identificacao, s.tid 
				from {0}" + tabela + @" s where s.projeto = :projeto", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);

				Sobreposicao sobreposicao = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						sobreposicao = new Sobreposicao();

						sobreposicao.Base = reader.GetValue<int>("base");
						sobreposicao.Tipo = reader.GetValue<int>("tipo");
						sobreposicao.Identificacao = reader.GetValue<string>("identificacao");

						sobreposicoes.Add(sobreposicao);
					}

					reader.Close();
				}
			}

			return sobreposicoes;
		}

		internal List<Sobreposicao> ObterSobreposicoesHistorico(int projetoGeoID, string projetoGeoTID, BancoDeDados banco = null)
		{
			List<Sobreposicao> sobreposicoes = new List<Sobreposicao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select s.sobreposicao_id id, s.base, s.tipo_id tipo, s.identificacao, s.tid 
				from {0}hst_crt_projeto_geo_sobrepos s where s.projeto_id = :projeto_id and s.projeto_tid = :projeto_tid", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto_id", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_tid", DbType.String, 36, projetoGeoTID);

				Sobreposicao sobreposicao = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						sobreposicao = new Sobreposicao();

						sobreposicao.Base = reader.GetValue<int>("base");
						sobreposicao.Tipo = reader.GetValue<int>("tipo");
						sobreposicao.Identificacao = reader.GetValue<string>("identificacao");

						sobreposicoes.Add(sobreposicao);
					}

					reader.Close();
				}
			}

			return sobreposicoes;
		}

		internal void ObterSituacaoFila(ProcessamentoGeo processamentoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select lc.id, lc.texto from {1}tab_fila t, {0}lov_crt_projeto_geo_sit_proce lc 
				where t.situacao = lc.situacao(+) and t.etapa = lc.etapa(+) and t.id = :fila_id", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("fila_id", processamentoGeo.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						processamentoGeo.Situacao = reader.GetValue<int>("id");
						processamentoGeo.SituacaoTexto = reader.GetValue<string>("texto");
					}

					reader.Close();
				}
			}
		}

		internal void ObterSituacao(ProcessamentoGeo processamentoGeo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.texto from {0}lov_crt_projeto_geo_sit_proce t where t.etapa = :etapa and t.situacao = :situacao", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("etapa", processamentoGeo.Etapa, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", processamentoGeo.Situacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						processamentoGeo.Situacao = reader.GetValue<int>("id");
						processamentoGeo.SituacaoTexto = reader.GetValue<string>("texto");
					}

					reader.Close();
				}
			}
		}

		internal int ObterSituacaoProjetoGeografico(int projetoGeoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select t.situacao from {0}tmp_projeto_geo t where t.id = :projeto), 
				(select t.situacao from {0}crt_projeto_geo t where t.id = :projeto)) situacao from dual", EsquemaBancoCredenciado);
				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		public Empreendimento ObterEmpreendimentoCoordenada(int empreendimentoID)
		{
			Empreendimento empreendimento = new Empreendimento();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
			{
				#region Coordenada

				Comando comando = bancoDeDados.CriarComando(@"select aec.id, aec.tid, aec.tipo_coordenada, aec.datum, aec.easting_utm,
				aec.northing_utm, aec.fuso_utm, aec.hemisferio_utm, aec.latitude_gms,aec.longitude_gms, aec.latitude_gdec, aec.longitude_gdec, 
				aec.forma_coleta, aec.local_coleta from {0}tab_empreendimento_coord aec where aec.empreendimento = :empreendimentoid", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("empreendimentoid", empreendimentoID, DbType.Int32);

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

		#endregion

		#region Obter Geo

		internal Sobreposicao ObterGeoSobreposicaoIdaf(int ProjetoGeoID, eCaracterizacao tipo, BancoDeDados banco = null)
		{
			Sobreposicao sobreposicao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				#region Sobreposicao Empreendimento [ATP]

				Comando comando = null;
				int projetoGeoDominialidadeID = ProjetoGeoID;

				if (tipo != eCaracterizacao.Dominialidade)
				{
					comando = bancoDeDados.CriarComando(@"select d.id projeto_dom from {0}crt_projeto_geo d 
					where d.caracterizacao = 1 and d.empreendimento = (select ent.empreendimento from {0}crt_projeto_geo ent where ent.id = :projeto_ativ )", EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("projeto_ativ", ProjetoGeoID, DbType.Int32);
					projetoGeoDominialidadeID = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				}

				comando = bancoDeDados.CriarComando(@"select e.id, seg.texto segmento, e.denominador from {0}tab_empreendimento e, {0}crt_projeto_geo pg, {0}lov_empreendimento_segmento seg 
				where pg.empreendimento = e.id and e.segmento = seg.id and pg.id <> :projeto and pg.id in (select atp.projeto from {1}tmp_atp a, {1}geo_atp atp 
					where a.projeto = :projeto and sdo_relate(atp.geometry, a.geometry, 'MASK=ANYINTERACT') = 'TRUE' )", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeoDominialidadeID, DbType.Int32);

				List<String> lstEmp = new List<String>();

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						string denominador = reader.GetValue<string>("denominador");

						if (!string.IsNullOrEmpty(denominador))
						{
							lstEmp.Add(String.Format("{0} - {1}", reader.GetValue<string>("segmento"), denominador));
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

		internal string ObterWktATP(int projetoGeoID, eCaracterizacao caracterizacaoTipo, BancoDeDados banco = null)
		{
			string wkt = string.Empty;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				if (caracterizacaoTipo == eCaracterizacao.Dominialidade)
				{
					comando = bancoDeDados.CriarComando(@"select mdsys.sdo_util.to_wktgeometry(a.geometry) wkt from {0}tmp_atp a where a.projeto = :projeto", EsquemaBancoCredenciadoGeo);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select mdsys.sdo_util.to_wktgeometry(a.geometry) wkt from {1}geo_atp a where a.projeto = 
					(select c.id from {0}crt_projeto_geo c where c.caracterizacao = 1 and c.empreendimento = (select g.empreendimento from {0}tmp_projeto_geo g where g.id = :projeto))", 
					EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);
				}

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						wkt = reader.GetValue<string>("wkt");
					}

					reader.Close();
				}
			}

			return wkt;
		}

		#endregion

		#region Validações

		internal int ExisteProjetoGeografico(int empreendimentoID, int caracterizacaoTipo, bool finalizado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select t.id from {0}tmp_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo),
				(select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo)) projeto from dual", EsquemaBancoCredenciado);

				if (finalizado)
				{
					comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo", EsquemaBancoCredenciado);
				}

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", caracterizacaoTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal int ExisteItemFila(ProcessamentoGeo processamento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}tab_fila t where t.projeto = :projeto and t.tipo = :tipo", EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", processamento.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", processamento.FilaTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		internal bool VerificarProjetoGeograficoProcessado(int projetoGeoID, eCaracterizacao caracterizacaoTipo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_fila t where t.projeto = :projeto and t.etapa = 3 
				and t.situacao = 4 and t.tipo = :tipo", EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", (int)((caracterizacaoTipo == eCaracterizacao.Dominialidade) ? eFilaTipoGeo.Dominialidade : eFilaTipoGeo.Atividade), DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ValidarProjetoGeograficoTemporario(int projetoGeoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tmp_projeto_geo t where t.id = :projeto", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto", projetoGeoID, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ArquivoAssociadoProjetoDigital(int arquivoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(1) qtd 
				from tab_proj_digital_dependencias pd, hst_crt_projeto_geo_arquivos ha 
				where pd.dependencia_tipo = 1 and pd.dependencia_id = ha.projeto_id and pd.dependencia_tid = ha.projeto_tid 
				and ha.arquivo_id = :arquivo_id", EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("arquivo_id", arquivoID, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal List<string> VerificarExcluirDominios(int empreendimentoID)
		{
			List<string> retorno = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select (case when c.tipo = 1 then 'Matricula' else 'Posse' end) || ' - ' || c.identificacao
				from {0}crt_dominialidade_dominio c
				where c.dominialidade = (select d.id from {0}crt_dominialidade d where d.empreendimento = :empreendimento)
				and exists (select 1 from {0}crt_dominialidade_reserva r where r.matricula = c.id)
				and c.identificacao not in (select a.nome from {1}tmp_apmp a 
				where a.projeto = (select p.id from {0}tmp_projeto_geo p where p.caracterizacao = 1 and p.empreendimento = :empreendimento))", EsquemaBancoCredenciado, EsquemaBancoCredenciadoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);

				retorno = bancoDeDados.ExecutarList<string>(comando);
			}

			return retorno;
		}

		#endregion

		#region Auxiliares

		internal void AtualizarInternoIdTid(int projetoId, int projetoInternoId, string projetoInternoTid, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualização do Tid

				Comando comando = bancoDeDados.CriarComandoPlSql(@"begin
				update {0}crt_projeto_geo set tid = :tid, interno_id = :interno_id, interno_tid = :interno_tid where id = :projeto_id;
				update {0}crt_projeto_geo_arquivos set tid = :tid where projeto = :projeto_id;
				update {0}crt_projeto_geo_sobrepos set tid = :tid where projeto = :projeto_id;
				update {0}crt_projeto_geo_ortofoto set tid = :tid where projeto = :projeto_id;
				end;", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("projeto_id", projetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());				
				comando.AdicionarParametroEntrada("interno_id", projetoInternoId, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, projetoInternoTid);
				
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion 

				#region Histórico

				Historico.Gerar(projetoId, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.atualizaridtid, bancoDeDados);
				Historico.GerarGeo(projetoId, eHistoricoArtefatoCaracterizacao.projetogeografico, eHistoricoAcao.atualizaridtid, bancoDeDados);

				#endregion

				bancoDeDados.Commit();				
			}
		}

		#endregion

	}
}