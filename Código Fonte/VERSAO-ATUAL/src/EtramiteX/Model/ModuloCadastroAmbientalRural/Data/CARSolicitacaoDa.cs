using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Data
{
	public class CARSolicitacaoDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
        Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data.CARSolicitacaoDa _daCred = new Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data.CARSolicitacaoDa();

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private String EsquemaBanco { get; set; }

		#endregion

		public string UsuarioCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public CARSolicitacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal int Salvar(CARSolicitacao solicitacao, BancoDeDados banco)
		{
			if (solicitacao == null)
			{
				throw new Exception("Solicitacao é nula.");
			}

			if (solicitacao.Id <= 0)
			{
                return Criar(solicitacao, banco);
			}
			else
			{
				return Editar(solicitacao, banco);
			}
		}

        internal int Criar(CARSolicitacao solicitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}tab_car_solicitacao(id, numero, data_emissao, situacao, situacao_data, protocolo, requerimento, protocolo_selecionado, atividade, 
				 empreendimento, declarante, autor, tid, passivo_enviado) values({0}seq_car_solicitacao.nextval, {0}seq_car_solicitacao.currval, sysdate, :situacao, 
				sysdate, :protocolo, :requerimento, :protocolo_selecionado, :atividade, :empreendimento, :declarante, :autor, :tid, 1) returning id into :id", EsquemaBanco);

                comando.AdicionarParametroEntrada("situacao", (int)eCARSolicitacaoSituacao.EmCadastro, DbType.Int32);
                comando.AdicionarParametroEntrada("protocolo", solicitacao.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", solicitacao.Requerimento.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("protocolo_selecionado", solicitacao.ProtocoloSelecionado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", solicitacao.Atividade.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("empreendimento", solicitacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("declarante", solicitacao.Declarante.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("autor", solicitacao.AutorId, DbType.Int32);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				solicitacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion Solicitação

				Historico.Gerar(solicitacao.Id, eHistoricoArtefato.carsolicitacao, eHistoricoAcao.criar, bancoDeDados);

				//Consulta.Gerar(solicitacao.Id, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);
				comando = bancoDeDados.CriarComando(@"begin lst_consulta.carSolicitacaoTitulo(:id); end;");
				comando.AdicionarParametroEntrada("id", solicitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);


				//Inserir na fila para gerar o .CAR para envio para o SICAR
				InserirFilaArquivoCarSicar(solicitacao, eCARSolicitacaoOrigem.Institucional, bancoDeDados);
				
				bancoDeDados.Commit();

				return solicitacao.Id;
			}
		}

		internal int Editar(CARSolicitacao solicitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update {0}tab_car_solicitacao r set r.protocolo = :protocolo, r.protocolo_selecionado = :protocolo_selecionado, r.requerimento = :requerimento, 
				r.atividade = :atividade, r.empreendimento = :empreendimento, r.declarante = :declarante, r.tid = :tid where r.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", solicitacao.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", solicitacao.Requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("protocolo_selecionado", solicitacao.ProtocoloSelecionado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", solicitacao.Atividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", solicitacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("declarante", solicitacao.Declarante.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", solicitacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Solicitação

				Historico.Gerar(solicitacao.Id, eHistoricoArtefato.carsolicitacao, eHistoricoAcao.atualizar, bancoDeDados, null);

				//Consulta.Gerar(solicitacao.Id, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);
				comando = bancoDeDados.CriarComando(@"begin lst_consulta.carSolicitacaoTitulo(:id); end;");
				comando.AdicionarParametroEntrada("id", solicitacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				//Inserir na fila para gerar o .CAR para envio para o SICAR
				InserirFilaArquivoCarSicar(solicitacao, eCARSolicitacaoOrigem.Institucional, banco);

				return solicitacao.Id;
			}
		}

		internal void AlterarSituacao(CARSolicitacao entidade, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_car_solicitacao t
				set t.situacao               = :situacao,
					t.situacao_data          = :situacao_data,
					t.situacao_anterior      = :situacao_anterior,
					t.situacao_anterior_data = :situacao_anterior_data,
					t.motivo                 = :motivo,
					t.tid                    = :tid
				where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", entidade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_data", entidade.DataSituacao.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("situacao_anterior", entidade.SituacaoAnteriorId, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_anterior_data", entidade.DataSituacaoAnterior.DataTexto, DbType.Date);
				comando.AdicionarParametroEntrada("motivo", DbType.String, 300, entidade.Motivo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(entidade.Id, eHistoricoArtefato.carsolicitacao, eHistoricoAcao.alterarsituacao, bancoDeDados);

				Consulta.Gerar(entidade.Id, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacaoArquivoSicar(CARSolicitacao entidade, int situacaoArquivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update tab_controle_sicar t
				set t.situacao_envio         = :situacao
				where t.solicitacao_car = :solicitacao
				returning t.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("solicitacao", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", situacaoArquivo, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				int idControleSicar = Convert.ToInt32(comando.ObterValorParametro("id"));
				GerarHistoricoControleArquivoCarSicar(idControleSicar, banco);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Histórico

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_car_solicitacao r set r.tid = :tid where r.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.carsolicitacao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				Consulta.Deletar(id, eHistoricoArtefato.carsolicitacaotitulo, bancoDeDados);

				#region Apaga os dados da solicitacao

				comando = bancoDeDados.CriarComandoPlSql("begin delete from {0}tab_car_solicitacao r where r.id = :id; end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void InserirFilaArquivoCarSicar(CARSolicitacao solicitacao, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados banco = null)
		{
			string requisicao_fila = string.Empty;
            
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid from tab_car_solicitacao tcs, tab_empreendimento te 
                    where tcs.empreendimento = te.id and tcs.id = :idSolicitacao");

				comando.AdicionarParametroEntrada("idSolicitacao", solicitacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						requisicao_fila = "{ \"origem\": \"" + solicitacaoOrigem.ToString().ToLower() + "\"" +
											", \"empreendimento\":" + reader.GetValue<int>("emp_id") +
											", \"empreendimento_tid\": \"" + reader.GetValue<string>("emp_tid") + "\"" +
											", \"solicitacao_car\":" + reader.GetValue<int>("solic_id") +
											", \"solicitacao_car_tid\": \"" + reader.GetValue<string>("solic_tid") + "\"" +
											 "}";
					}

					reader.Close();
				}

				if (requisicao_fila != string.Empty)
				{
					comando = bancoDeDados.CriarComando(@"
				    insert into tab_scheduler_fila (id, tipo, requisitante, requisicao, empreendimento, data_criacao, data_conclusao, resultado, sucesso) 
                    values (seq_tab_scheduler_fila.nextval, 'gerar-car', 0, :requisicao, 0, NULL, NULL, '', '')");

					comando.AdicionarParametroEntrada("requisicao", requisicao_fila, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					SalvarControleArquivoCarSicar(solicitacao, eStatusArquivoSICAR.AguardandoEnvio, solicitacaoOrigem, banco);

					bancoDeDados.Commit();
				}
			}
		}

		internal void SalvarControleArquivoCarSicar(CARSolicitacao solicitacao, eStatusArquivoSICAR statusArquivoSICAR, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados banco = null)
		{
			ControleArquivoSICAR controleArquivoSICAR = new ControleArquivoSICAR();
			controleArquivoSICAR.SolicitacaoCarId = solicitacao.Id;
            CARSolicitacao retificado = new CARSolicitacao();
            String codigoImovelTxt = String.Empty;

			// Se o campo para retificar já está preenchido, não preenche novamente
			if (!ValidarRetificado(solicitacao))
				retificado = ObterPorEmpreendimentoCod(solicitacao.Empreendimento.Codigo ?? 0, solicitacao.Id);

            if (retificado.Id > 0)
				controleArquivoSICAR.CodigoImovel = ObterCodigoImovel(retificado);


			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				controleArquivoSICAR = ObterDadosControleSicar(controleArquivoSICAR, solicitacaoOrigem, bancoDeDados);

				if (controleArquivoSICAR.Id == 0)
					controleArquivoSICAR.Id = CriarControleSicar(controleArquivoSICAR, retificado, statusArquivoSICAR, solicitacaoOrigem, bancoDeDados);
				else
					EditarControleSicar(controleArquivoSICAR, retificado, statusArquivoSICAR, bancoDeDados);
				

				GerarHistoricoControleArquivoCarSicar(controleArquivoSICAR.Id, banco);

				bancoDeDados.Commit();
			}
		}

		internal void GerarHistoricoControleArquivoCarSicar(int controleArquivoId, BancoDeDados banco = null)
		{
			if (controleArquivoId > 0)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					#region Histórico do controle de arquivo SICAR

					bancoDeDados.IniciarTransacao();

					Comando comando = bancoDeDados.CriarComandoPlSql(@"
					begin
                      for j in (select tcs.id, tcs.tid, tcs.empreendimento, tcs.empreendimento_tid, tcs.solicitacao_car, tcs.solicitacao_car_tid,
                                       tcs.situacao_envio, tcs.chave_protocolo, tcs.data_gerado, tcs.data_envio, tcs.arquivo, tcs.pendencias,
                                       tcs.codigo_imovel, tcs.url_recibo, tcs.status_sicar, tcs.condicao, tcs.solicitacao_car_esquema,
                                       solicitacao_car_anterior, solicitacao_car_anterior_tid, solicitacao_car_ant_esquema 
                                from tab_controle_sicar tcs
                                where tcs.id = :id) loop  
                         INSERT INTO HST_CONTROLE_SICAR
                           (id, controle_sicar_id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio,
                            chave_protocolo, data_gerado, data_envio, arquivo, pendencias, codigo_imovel, url_recibo, status_sicar, condicao,
                            solicitacao_car_esquema, data_execucao, solicitacao_car_anterior, solicitacao_car_anterior_tid, solicitacao_car_ant_esquema)
                         values 
                           (SEQ_HST_CONTROLE_SICAR.nextval, j.id, j.tid, j.empreendimento, j.empreendimento_tid, j.solicitacao_car, j.solicitacao_car_tid,
                            j.situacao_envio, j.chave_protocolo, j.data_gerado, j.data_envio, j.arquivo, j.pendencias, j.codigo_imovel, j.url_recibo,
                            j.status_sicar, j.condicao, j.solicitacao_car_esquema, CURRENT_TIMESTAMP,
                            j.solicitacao_car_anterior, j.solicitacao_car_anterior_tid, j.solicitacao_car_ant_esquema);
                      end loop;
                    end;");

					comando.AdicionarParametroEntrada("id", controleArquivoId, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
					bancoDeDados.Commit();

					#endregion
				}
			}
		}

		internal int CriarControleSicar(ControleArquivoSICAR controleArquivoSICAR, CARSolicitacao retificado, eStatusArquivoSICAR statusArquivoSICAR, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados bancoDeDados = null)
		{
			Comando comando = bancoDeDados.CriarComando(@"
				    insert into tab_controle_sicar (id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio, solicitacao_car_esquema, solicitacao_car_anterior, solicitacao_car_anterior_tid, solicitacao_car_ant_esquema, codigo_imovel)
                    values
                    (seq_tab_controle_sicar.nextval, :tid, :empreendimento, :empreendimento_tid, :solicitacao_car, :solicitacao_car_tid, :situacao_envio, :solicitacao_car_esquema, :solicitacao_car_anterior, :solicitacao_car_anterior_tid, :solicitacao_car_ant_esquema, :codigo_imovel)
                     returning id into :id", UsuarioCredenciado);

			comando.AdicionarParametroEntrada("empreendimento", controleArquivoSICAR.EmpreendimentoId, DbType.Int32);
			comando.AdicionarParametroEntrada("empreendimento_tid", controleArquivoSICAR.EmpreendimentoTid, DbType.String);
			comando.AdicionarParametroEntrada("solicitacao_car", controleArquivoSICAR.SolicitacaoCarId, DbType.Int32);
			comando.AdicionarParametroEntrada("solicitacao_car_tid", controleArquivoSICAR.SolicitacaoCarTid, DbType.String);
			comando.AdicionarParametroEntrada("situacao_envio", (int)statusArquivoSICAR, DbType.Int32);
			comando.AdicionarParametroEntrada("solicitacao_car_esquema", (int)solicitacaoOrigem, DbType.Int32);
			if (retificado.Id == 0)
			{
				comando.AdicionarParametroEntrada("solicitacao_car_anterior", null, DbType.Int32);
				comando.AdicionarParametroEntrada("solicitacao_car_anterior_tid", null, DbType.String);
				comando.AdicionarParametroEntrada("solicitacao_car_ant_esquema", null, DbType.Int32);
			}
			else
			{
				comando.AdicionarParametroEntrada("solicitacao_car_anterior", retificado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("solicitacao_car_anterior_tid", retificado.Tid, DbType.String);
				comando.AdicionarParametroEntrada("solicitacao_car_ant_esquema", retificado.Esquema, DbType.Int32);
			}
			comando.AdicionarParametroEntrada("codigo_imovel", controleArquivoSICAR.CodigoImovel, DbType.String);
			comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
			comando.AdicionarParametroSaida("id", DbType.Int32);

			bancoDeDados.ExecutarNonQuery(comando);

			return Convert.ToInt32(comando.ObterValorParametro("id"));
			
		}

		internal void EditarControleSicar(ControleArquivoSICAR controleArquivoSICAR, CARSolicitacao retificado, eStatusArquivoSICAR statusArquivoSICAR, BancoDeDados bancoDeDados = null)
		{
			String codigoImovelTxt = String.Empty;

			if (!String.IsNullOrWhiteSpace(controleArquivoSICAR.CodigoImovel))
				codigoImovelTxt = @"
						solicitacao_car_anterior = :solicitacao_car_anterior, solicitacao_car_anterior_tid = :solicitacao_car_anterior_tid,
						solicitacao_car_ant_esquema = :solicitacao_car_ant_esquema,codigo_imovel = :codigo_imovel, ";

			Comando comando = bancoDeDados.CriarComando(@"
				    update tab_controle_sicar r set r.empreendimento_tid = :empreendimento_tid, r.solicitacao_car_tid = :solicitacao_car_tid, r.situacao_envio = :situacao_envio,
					" + codigoImovelTxt + @"
                    r.tid = :tid, r.arquivo = null where r.id = :id");

			comando.AdicionarParametroEntrada("empreendimento_tid", controleArquivoSICAR.EmpreendimentoTid, DbType.String);
			comando.AdicionarParametroEntrada("solicitacao_car_tid", controleArquivoSICAR.SolicitacaoCarTid, DbType.String);
			comando.AdicionarParametroEntrada("situacao_envio", (int)statusArquivoSICAR, DbType.Int32);
			comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
			comando.AdicionarParametroEntrada("id", controleArquivoSICAR.Id, DbType.Int32);

			if (!String.IsNullOrWhiteSpace(controleArquivoSICAR.CodigoImovel))
			{
				comando.AdicionarParametroEntrada("solicitacao_car_anterior", retificado.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("solicitacao_car_anterior_tid", retificado.Tid, DbType.String);
				comando.AdicionarParametroEntrada("solicitacao_car_ant_esquema", retificado.Esquema, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo_imovel", controleArquivoSICAR.CodigoImovel, DbType.String);
			}

			bancoDeDados.ExecutarNonQuery(comando);
		}

		#endregion

		#region Obter/ Filtrar

		internal CARSolicitacao Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select s.tid,
					s.numero,
					s.data_emissao,
					s.situacao_data,
					l.id situacao,
					l.texto situacao_texto,
					s.situacao_anterior,
					la.texto situacao_anterior_texto,
					s.situacao_anterior_data,
					p.id protocolo_id,
					p.protocolo,
					p.numero protocolo_numero,
					p.ano protocolo_ano,
					nvl(pes.nome, pes.razao_social) declarante_nome_razao,
					ps.id protocolo_selecionado_id,
					ps.protocolo protocolo_selecionado,
					ps.numero protocolo_selecionado_numero,
					ps.ano protocolo_selecionado_ano,
					p.requerimento,
					s.atividade,
					e.id empreendimento_id,
					e.denominador empreendimento_nome,
					e.codigo empreendimento_codigo,
					s.declarante,
					f.funcionario_id autor_id,
					f.nome autor_nome,
					(select stragg_barra(sigla) from hst_setor where 
					setor_id in (select fs.setor_id from hst_funcionario_setor fs where fs.id_hst = f.id)
					and tid in (select fs.setor_tid from hst_funcionario_setor fs where fs.id_hst = f.id )) autor_setor,
					'Institucional' autor_modulo,
					s.autor,
					s.motivo,
					tr.data_criacao requerimento_data_cadastro,
					pg.id projeto_geo_id,
					s.arquivo,
					cs.situacao_envio
				from tab_car_solicitacao         s,
					tab_controle_sicar			 cs,
					lov_car_solicitacao_situacao l,
					lov_car_solicitacao_situacao la,
					tab_protocolo                p,
					tab_protocolo                ps,
					tab_empreendimento           e,
					crt_projeto_geo              pg,
					tab_pessoa                   pes,
					tab_requerimento             tr,
					hst_funcionario              f
				where s.id = cs.solicitacao_car
				and s.situacao = l.id
				and s.situacao_anterior = la.id(+)
				and s.protocolo_selecionado = p.id
				and s.protocolo_selecionado = ps.id(+)
				and s.empreendimento = e.id
				and s.empreendimento = pg.empreendimento
				and s.declarante = pes.id
				and s.requerimento = tr.id
				and pg.caracterizacao = 1
				and f.funcionario_id = s.autor
				and f.tid = (select autor_tid from hst_car_solicitacao where acao_executada = 342 and solicitacao_id = s.id)
				and s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = id;
						solicitacao.Tid = reader.GetValue<String>("tid");
						solicitacao.Numero = reader.GetValue<String>("numero");
						solicitacao.DataEmissao.DataTexto = reader.GetValue<String>("data_emissao");
						solicitacao.SituacaoId = reader.GetValue<Int32>("situacao");
						solicitacao.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						solicitacao.DataSituacao.DataTexto = reader.GetValue<String>("situacao_data");
						solicitacao.SituacaoAnteriorId = reader.GetValue<Int32>("situacao_anterior");
						solicitacao.SituacaoAnteriorTexto = reader.GetValue<String>("situacao_anterior_texto");
						solicitacao.DataSituacaoAnterior.DataTexto = reader.GetValue<String>("situacao_anterior_data");

						solicitacao.Protocolo.Id = reader.GetValue<Int32>("protocolo_id");
						solicitacao.Protocolo.IsProcesso = reader.GetValue<Int32>("protocolo") == 1;
						solicitacao.Protocolo.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_numero");
						solicitacao.Protocolo.Ano = reader.GetValue<Int32>("protocolo_ano");

						solicitacao.ProtocoloSelecionado.Id = reader.GetValue<Int32>("protocolo_selecionado_id");
						solicitacao.ProtocoloSelecionado.IsProcesso = reader.GetValue<Int32>("protocolo_selecionado") == 1;
						solicitacao.ProtocoloSelecionado.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_selecionado_numero");
						solicitacao.ProtocoloSelecionado.Ano = reader.GetValue<Int32>("protocolo_selecionado_ano");
						solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
						solicitacao.Requerimento.DataCadastro = reader.GetValue<DateTime>("requerimento_data_cadastro");
						solicitacao.Atividade.Id = reader.GetValue<Int32>("atividade");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
						solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante");
						solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nome_razao");

						solicitacao.AutorId = reader.GetValue<Int32>("autor_id");
						solicitacao.AutorNome = reader.GetValue<String>("autor_nome");
						solicitacao.AutorSetorTexto = reader.GetValue<String>("autor_setor");
						solicitacao.AutorModuloTexto = reader.GetValue<String>("autor_modulo");

						solicitacao.Motivo = reader.GetValue<String>("motivo");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_geo_id");
						solicitacao.Arquivo = reader.GetValue<Int32>("arquivo");
						solicitacao.SICAR.SituacaoEnvio = (eStatusArquivoSICAR)reader.GetValue<Int32>("situacao_envio");
					}

					reader.Close();
				}

				#endregion
			}

			return solicitacao;
		}

		internal CARSolicitacao ObterPorEmpreendimento(int empreendimentoId, List<int> situacoes = null, bool simplificado = false, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select c.id solicitacao from tab_car_solicitacao c where empreendimento = :empreendimento ", EsquemaBanco);

				if (situacoes != null && situacoes.Count > 0)
				{
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "c.situacao", DbType.Int32, situacoes);
				}

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				int solicitacaoId = 0;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacaoId = solicitacao.ProjetoId = reader.GetValue<Int32>("solicitacao");
					}
					reader.Close();
				}

				solicitacao = solicitacaoId <= 0 ? null : Obter(solicitacaoId, simplificado: simplificado, banco: bancoDeDados);

				#endregion
			}

			return solicitacao;
		}

        internal CARSolicitacao ObterPorEmpreendimentoCod(Int64 empreendimentoCod, int solicitacaoAtual = 0, BancoDeDados banco = null)
        {
            CARSolicitacao solicitacao = new CARSolicitacao();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {

				#region Solicitação Não válida
				
                //CREDENCIADO
                Comando comando = bancoDeDados.CriarComando(@"select * from	(  select * from	(
																	select c.id solicitacao, c.SITUACAO, 1 esquema from tab_car_solicitacao c 
																		  inner join tab_empreendimento ec on ec.id = c.empreendimento 
																	  where c.situacao != 3 and ec.codigo = :codigo and c.id != :solicitacao
																	  union all
																	  select c.id solicitacao, c.SITUACAO, 2 esquema from {0}tab_car_solicitacao c 
																		  inner join {0}tab_empreendimento ec on ec.id = c.empreendimento 
																	  where c.situacao != 3 and ec.codigo = :codigo and c.id != :solicitacao
																  ) order by 
																	  case situacao 
																	  when 1 then 7
																	  else situacao end
																	  desc) where rownum = 1", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("codigo", empreendimentoCod, DbType.Int32);
                comando.AdicionarParametroEntrada("solicitacao", solicitacaoAtual, DbType.Int32);

                int solicitacaoId = 0;
				int esquema = 0;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        solicitacaoId = reader.GetValue<Int32>("solicitacao");					
						solicitacao.Esquema = esquema =  reader.GetValue<Int32>("esquema");
					}
                    reader.Close();
                }

                if (solicitacaoId > 0)
                {
					if(solicitacao.Esquema == 2)
					{
						BancoDeDados bd = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado);

						solicitacao = _daCred.Obter(solicitacaoId, banco: bd);
						solicitacao.Esquema = esquema;
						return solicitacao;
					}
					else if (solicitacao.Esquema == 1)
					{
						solicitacao = Obter(solicitacaoId, banco: bancoDeDados);
						solicitacao.Esquema = esquema;
						return solicitacao;
					}
                }
				/*
                //INSTITUCIONAL
                using (BancoDeDados bd = BancoDeDados.ObterInstancia(banco))
                {
                    comando = bd.CriarComando(@"select * from (
													select c.id solicitacao from tab_car_solicitacao c 
														inner join tab_empreendimento ei on ei.id = c.empreendimento 
													where c.situacao != 3 and ei.codigo = :codigo order by 1 desc
												) where rownum = 1");

                    comando.AdicionarParametroEntrada("codigo", empreendimentoCod, DbType.Int32);

                    solicitacaoId = 0;

                    using (IDataReader reader = bd.ExecutarReader(comando))
                    {
                        if (reader.Read())
                        {
                            solicitacaoId = solicitacao.ProjetoId = reader.GetValue<Int32>("solicitacao");
                        }
                        reader.Close();
                    }

                    if (solicitacaoId > 0)
                    {
                        solicitacao = Obter(solicitacaoId, banco: bd);
                        solicitacao.Esquema = 1;
                        return solicitacao;
                    }
                }*/
                #endregion
            }

            return solicitacao;
        }

		internal CARSolicitacao ObterPorRequerimento(CARSolicitacao car, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			//CREDENCIADO
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.requerimento from tab_car_solicitacao c
																where c.requerimento = :requerimento and c.situacao != 3 and rownum <= 1");

				comando.AdicionarParametroEntrada("requerimento", car.Requerimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = reader.GetValue<Int32>("id");
						solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
					}
					reader.Close();
				}
			}

			if(solicitacao.Id < 1)
			{
				//INSTITUCIONAL
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{

					Comando comando = bancoDeDados.CriarComando(@"select c.id, p.requerimento from tab_car_solicitacao c
																inner join tab_protocolo p on c.protocolo_selecionado = p.id
															where p.requerimento = :requerimento and c.situacao != 3 ", EsquemaBanco);

					comando.AdicionarParametroEntrada("requerimento", car.Requerimento.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							solicitacao.Id = reader.GetValue<Int32>("id");
							solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
						}
						reader.Close();
					}
				}
			}

			if(solicitacao.Id < 1)
			{
				return null;
			}

			return solicitacao;
		}

		internal CARSolicitacao ObterHistorico(int id, string tid, bool simplificado = false, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select s.tid,
				   s.numero,
				   s.data_emissao,
				   s.situacao_id,
				   s.situacao_texto,
				   s.situacao_data,
				   s.situacao_anterior_id,
				   s.situacao_anterior_texto,
				   s.situacao_anterior_data,
				   p.id_protocolo,
				   p.protocolo_id,
				   p.numero protocolo_numero,
				   p.ano protocolo_ano,
				   ps.id_protocolo id_protocolo_selecionado,
				   ps.protocolo_id protocolo_selecionado_id,
				   ps.numero protocolo_selecionado_numero,
				   ps.ano protocolo_selecionado_ano,
				   ps.requerimento_id,
				   r.data_criacao requerimento_data_criacao,
				   a.id atividade_id,
				   a.atividade atividade_texto,
				   e.empreendimento_id,
				   e.denominador empreendimento_nome,
				   e.codigo empreendimento_codigo,
				   s.declarante_id,
       

				   f.funcionario_id autor_id,
				   f.tid autor_tid,
				   f.nome autor_nome,
                    
				   (select stragg_barra(sigla) from hst_setor where 
				   setor_id in (select fs.setor_id from hst_funcionario_setor fs where fs.id_hst = f.id)
				   and tid in (select fs.setor_tid from hst_funcionario_setor fs where fs.id_hst = f.id )) autor_setor,
				   'Institucional' autor_modulo,
                 
				   nvl(d.nome, d.razao_social) declarante_nomerazao,
				   s.motivo,
				   pg.projeto_geo_id
			  from hst_car_solicitacao s,
				   hst_protocolo       p,
				   hst_protocolo       ps,
				   hst_requerimento    r,
				   hst_empreendimento  e,
				   hst_pessoa          d,
				   tab_atividade       a,
				   hst_crt_projeto_geo pg,
				   hst_funcionario     f
			 where s.protocolo_id = p.id_protocolo
			   and s.protocolo_tid = p.tid
			   and s.protocolo_selecionado_id = ps.id_protocolo
			   and s.protocolo_selecionado_tid = ps.tid
			   and ps.requerimento_id = r.requerimento_id
			   and ps.requerimento_tid = r.tid
			   and s.empreendimento_id = e.empreendimento_id
			   and s.empreendimento_tid = e.tid
			   and s.declarante_id = d.pessoa_id
			   and s.declarante_tid = d.tid
			   and s.atividade_id = a.id
			   and s.projeto_geo_id = pg.projeto_geo_id
			   and s.projeto_geo_tid = pg.tid
			   and f.funcionario_id = s.autor_id
			   and f.tid = s.autor_tid
			   and s.solicitacao_id = :id
			   and s.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = id;
						solicitacao.Tid = reader.GetValue<String>("tid");
						solicitacao.Numero = reader.GetValue<String>("numero");
						solicitacao.DataEmissao.DataTexto = reader.GetValue<String>("data_emissao");
						solicitacao.SituacaoId = reader.GetValue<Int32>("situacao_id");
						solicitacao.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						solicitacao.DataSituacao.DataTexto = reader.GetValue<String>("situacao_data");
						solicitacao.SituacaoAnteriorId = reader.GetValue<Int32>("situacao_anterior_id");
						solicitacao.SituacaoAnteriorTexto = reader.GetValue<String>("situacao_anterior_texto");
						solicitacao.DataSituacaoAnterior.DataTexto = reader.GetValue<String>("situacao_anterior_data");
						solicitacao.Protocolo.Id = reader.GetValue<Int32>("id_protocolo");
						solicitacao.Protocolo.IsProcesso = reader.GetValue<Int32>("protocolo_id") == 1;
						solicitacao.Protocolo.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_numero");
						solicitacao.Protocolo.Ano = reader.GetValue<Int32>("protocolo_ano");
						solicitacao.ProtocoloSelecionado.Id = reader.GetValue<Int32>("id_protocolo_selecionado");
						solicitacao.ProtocoloSelecionado.IsProcesso = reader.GetValue<Int32>("protocolo_selecionado_id") == 1;
						solicitacao.ProtocoloSelecionado.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_selecionado_numero");
						solicitacao.Protocolo.Ano = reader.GetValue<Int32>("protocolo_selecionado_ano");
						solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento_id");
						solicitacao.Requerimento.DataCadastro = reader.GetValue<DateTime>("requerimento_data_criacao");
						solicitacao.Atividade.Id = reader.GetValue<Int32>("atividade_id");
						solicitacao.Atividade.NomeAtividade = reader.GetValue<String>("atividade_texto");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
						solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante_id");
						solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nomerazao");

						solicitacao.AutorId = reader.GetValue<Int32>("autor_id");                        
						solicitacao.AutorNome = reader.GetValue<String>("autor_nome");
						solicitacao.AutorSetorTexto = reader.GetValue<String>("autor_setor");
						solicitacao.AutorModuloTexto = reader.GetValue<String>("autor_modulo");

						solicitacao.Motivo = reader.GetValue<String>("motivo");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_geo_id");
					}

					reader.Close();
                }
               
                #endregion
            }

			return solicitacao;
		}

		internal CARSolicitacao ObterHistoricoUltimaSituacao(int id, eCARSolicitacaoSituacao situacao, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select hcs.tid, hcs.projeto_geo_id, hcs.projeto_geo_tid, hcs.empreendimento_id 
				from hst_car_solicitacao hcs where hcs.id = (select max(h.id) from hst_car_solicitacao h where h.solicitacao_id = :id and h.situacao_id = :situacao)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)situacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = id;
						solicitacao.Tid = reader.GetValue<String>("tid");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_geo_id");
						solicitacao.ProjetoTid = reader.GetValue<String>("projeto_geo_tid");
					}

					reader.Close();
				}

				#endregion
			}

			return solicitacao;
		}

		internal CARSolicitacao ObterHistoricoPrimeiraSituacao(int id, eCARSolicitacaoSituacao situacao, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select hcs.tid, hcs.projeto_geo_id, hcs.projeto_geo_tid, hcs.empreendimento_id 
				from hst_car_solicitacao hcs where hcs.id = (select min(h.id) from hst_car_solicitacao h where h.solicitacao_id = :id and h.situacao_id = :situacao)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)situacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = id;
						solicitacao.Tid = reader.GetValue<String>("tid");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_geo_id");
						solicitacao.ProjetoTid = reader.GetValue<String>("projeto_geo_tid");
					}

					reader.Close();
				}

				#endregion

				solicitacao = ObterHistorico(solicitacao.Id, solicitacao.Tid, banco: bancoDeDados);
			}

			return solicitacao;
		}

		internal string ObterCNPJEmpreendimento(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.cnpj from {0}tab_empreendimento e where e.id = :empreendimentoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<string>(comando);
			}
		}

		internal Resultados<SolicitacaoListarResultados> Filtrar(Filtro<SolicitacaoListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<SolicitacaoListarResultados> retorno = new Resultados<SolicitacaoListarResultados>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = string.IsNullOrEmpty(EsquemaBanco) ? "" : ".";
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("l.solicitacao_numero", "solicitacao_numero", filtros.Dados.SolicitacaoNumero);

				comandtxt += comando.FiltroAnd("l.empreendimento_codigo", "empreendimento_codigo", filtros.Dados.EmpreendimentoCodigo);

				comandtxt += comando.FiltroAndLike("l.protocolo_numero_completo", "protocolo_numero_completo", filtros.Dados.Protocolo.NumeroTexto);

				comandtxt += comando.FiltroAndLike("l.declarante_nome_razao", "declarante_nome_razao", filtros.Dados.DeclaranteNomeRazao, true);

				comandtxt += comando.FiltroAnd("l.declarante_cpf_cnpj", "declarante_cpf_cnpj", filtros.Dados.DeclaranteCPFCNPJ);

				comandtxt += comando.FiltroAndLike("l.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoDenominador, true);

				comandtxt += comando.FiltroAnd("l.municipio_id", "municipio_id", filtros.Dados.Municipio);

				comandtxt += comando.FiltroAnd("l.requerimento", "requerimento", filtros.Dados.Requerimento);

				comandtxt += comando.FiltroAnd("l.titulo_numero", "titulo_numero", filtros.Dados.Titulo.Inteiro);

				comandtxt += comando.FiltroAnd("l.titulo_ano", "titulo_ano", filtros.Dados.Titulo.Ano);

				comandtxt += comando.FiltroAnd("l.origem", "origem", filtros.Dados.Origem);

                comandtxt += comando.FiltroAnd("l.situacao_envio_id", "situacao_envio", filtros.Dados.SituacaoSicar);

                comandtxt += comando.FiltroAnd("l.codigo_imovel", "codigo_imovel", filtros.Dados.codigoImovelSicar); 

				if (!String.IsNullOrWhiteSpace(filtros.Dados.Situacao))
				{
					comandtxt += comando.FiltroAnd("l.situacao_id", "situacao", filtros.Dados.Situacao);
					comandtxt += comando.FiltroAnd("l.tipo", "tipo", 1);//Solicitacao
				}

				if (filtros.Dados.Situacoes.Count > 0)
				{
					comandtxt += String.Format(" and l.situacao_texto in ({0})", String.Join(",", filtros.Dados.Situacoes));
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "solicitacao_numero", "empreendimento_codigo", "empreendimento_denominador", "municipio_texto", "situacao_texto", "situacao_envio_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("solicitacao_numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(1)
							from (select tcs.id,
                                   tcs.id solic_tit_id,
                                   tcs.numero solicitacao_numero,
                                   null titulo_numero,
                                   null titulo_ano,
                                   
                                   pt.id protocolo_id,
                                   pt.numero protocolo_numero,
                                   pt.ano protocolo_ano,
                                   pt.numero || '/' || pt.ano protocolo_numero_completo,
                                   
                                   null projeto_digital,
                                   null credenciado,
                                   
                                   pe.id declarante_id,
                                   coalesce(pe.nome,pe.razao_social) declarante_nome_razao,
                                   coalesce(pe.cpf, pe.cnpj) declarante_cpf_cnpj,
                                   
                                   e.id empreendimento_id,
                                   e.codigo empreendimento_codigo,
                                   e.denominador empreendimento_denominador,
                                   lme.id municipio_id,
                                   lme.texto municipio_texto,
                                   
                                   lcss.id situacao_id,
                                   lcss.texto situacao_texto,
                                   tcs.requerimento requerimento,
                                   
                                   1 origem,
                                   1 tipo,
                                   
                                   lses.id situacao_envio_id,
								   lses.texto situacao_envio_texto,
								   tsicar.url_recibo,
                                   tsicar.codigo_imovel
                              from tab_car_solicitacao tcs, tab_protocolo pt, tab_pessoa pe, tab_empreendimento e, tab_empreendimento_endereco ee,
                                   lov_municipio lme, lov_car_solicitacao_situacao lcss, tab_controle_sicar tsicar,lov_situacao_envio_sicar lses
                             where not exists (select lst.solic_tit_id from lst_car_solic_tit lst where lst.tipo=1 and lst.solic_tit_id=tcs.id)
                               and tcs.protocolo=pt.id
                               and tcs.declarante=pe.id
                               and tcs.empreendimento=e.id
                               and e.id=ee.empreendimento(+)
                               and ee.municipio=lme.id(+)
                               and ee.correspondencia(+)=0
                               and tcs.situacao=lcss.id
                               and tcs.id=tsicar.solicitacao_car(+)
                               and tsicar.solicitacao_car_esquema(+)=1
                               and tsicar.situacao_envio=lses.id(+)
                                 union all
								
								select s.id,
									   s.solic_tit_id,
									   s.solicitacao_numero,
									   null                         titulo_numero,
									   null                         titulo_ano,
									   s.protocolo_id,
									   s.protocolo_numero,
									   s.protocolo_ano,
									   s.protocolo_numero_completo,
									   null                         projeto_digital,
									   null                         credenciado,
									   s.declarante_id,
									   s.declarante_nome_razao,
									   s.declarante_cpf_cnpj,
									   s.empreendimento_id,
									   s.empreendimento_codigo,
									   s.empreendimento_denominador,
									   s.municipio_id,
									   s.municipio_texto,
									   s.situacao_id,
									   s.situacao_texto,
									   s.requerimento,
									   1                            origem,
									   1                            tipo,
									   tcs.situacao_envio           situacao_envio_id,
									   lses.texto                   situacao_envio_texto,
									   tcs.url_recibo,
                                       tcs.codigo_imovel
								  from lst_car_solic_tit        s,
									   tab_controle_sicar       tcs,
									   lov_situacao_envio_sicar lses
								 where s.tipo = 1
								   and nvl(tcs.solicitacao_car_esquema, 1) = 1
								   and s.solic_tit_id = tcs.solicitacao_car(+)
								   and tcs.situacao_envio = lses.id(+)
								union all
								select s.id,
									   s.solic_tit_id,
									   null                         solicitacao_numero,
									   s.titulo_numero,
									   s.titulo_ano,
									   s.protocolo_id,
									   s.protocolo_numero,
									   s.protocolo_ano,
									   s.protocolo_numero_completo,
									   null                         projeto_digital,
									   null                         credenciado,
									   s.declarante_id,
									   s.declarante_nome_razao,
									   s.declarante_cpf_cnpj,
									   s.empreendimento_id,
									   s.empreendimento_codigo,
									   s.empreendimento_denominador,
									   s.municipio_id,
									   s.municipio_texto,
									   null                         situacao_id,
									   s.situacao_texto,
									   s.requerimento,
									   1                            origem,
									   2                            tipo,
									   null                         situacao_envio_id,
									   null                         situacao_envio_texto,
									   null                         url_recibo,
                                       null                         codigo_imovel
								  from lst_car_solic_tit s
								 where s.tipo = 2
								union all
								select c.id,
									   c.solicitacao_id solic_tit_id,
									   c.numero solicitacao_numero,
									   null titulo_numero,
									   null titulo_ano,
									   tp.id protocolo_id,
									   tp.numero protocolo_numero,
									   tp.ano protocolo_ano,
									   tp.numero || '/' || tp.ano protocolo_numero_completo,
									   c.projeto_digital,
									   c.credenciado,
									   c.declarante_id,
									   c.declarante_nome_razao,
									   c.declarante_cpf_cnpj,
									   c.empreendimento_id,
									   (select ce.codigo from cre_empreendimento ce where ce.id = c.empreendimento_id) empreendimento_codigo,
									   c.empreendimento_denominador,
									   c.municipio_id,
									   c.municipio_texto,
									   c.situacao_id,
									   c.situacao_texto,
									   c.requerimento,
									   2 origem,
									   1 tipo,
									   tcs.situacao_envio situacao_envio_id,
									   lses.texto situacao_envio_texto,
									   tcs.url_recibo,
                                       tcs.codigo_imovel
								  from lst_car_solicitacao_cred c,
									   tab_controle_sicar       tcs,
									   lov_situacao_envio_sicar lses,
									   tab_protocolo            tp
								 where nvl(tcs.solicitacao_car_esquema, 2) = 2
								   and c.solicitacao_id = tcs.solicitacao_car(+)
								   and tcs.situacao_envio = lses.id(+)
								   AND c.requerimento = tp.requerimento(+)) l
						 where 1 = 1 " + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select l.solic_tit_id,
					   nvl(l.solicitacao_numero, l.titulo_numero) numero,
					   l.titulo_ano ano,
					   l.empreendimento_denominador,
					   l.municipio_texto,
					   l.situacao_id,
					   l.situacao_texto,
					   l.situacao_motivo,
					   l.credenciado,
					   l.origem,
					   l.tipo,
					   l.situacao_envio_id,
					   l.situacao_envio_texto,
					   l.url_recibo,
					   l.arquivo,
                       l.codigo_imovel,
					   l.empreendimento_codigo
				  from (select tcs.id,
                               tcs.id solic_tit_id,
                               tcs.numero solicitacao_numero,
                               null titulo_numero,
                               null titulo_ano,
                               
                               pt.id protocolo_id,
                               pt.numero protocolo_numero,
                               pt.ano protocolo_ano,
                               pt.numero || '/' || pt.ano protocolo_numero_completo,
                                   
                               null projeto_digital,
                               null credenciado,
                                   
                               pe.id declarante_id,
                               coalesce(pe.nome,pe.razao_social) declarante_nome_razao,
                               coalesce(pe.cpf, pe.cnpj) declarante_cpf_cnpj,
                                   
                               e.id empreendimento_id,
                               e.codigo empreendimento_codigo,
                               e.denominador empreendimento_denominador,
                               lme.id municipio_id,
                               lme.texto municipio_texto,
                                   
                               lcss.id situacao_id,
                               lcss.texto situacao_texto,
                               tcs.motivo situacao_motivo,
                               tcs.requerimento requerimento,
                                   
                               1 origem,
                               1 tipo,
                                   
                               lses.id situacao_envio_id,
                               lses.texto situacao_envio_texto,
                               tsicar.url_recibo,
                               tsicar.arquivo,
                               tsicar.codigo_imovel
                              from tab_car_solicitacao tcs, tab_protocolo pt, tab_pessoa pe, tab_empreendimento e, tab_empreendimento_endereco ee,
                                   lov_municipio lme, lov_car_solicitacao_situacao lcss, tab_controle_sicar tsicar,lov_situacao_envio_sicar lses
                             where not exists (select lst.solic_tit_id from lst_car_solic_tit lst where lst.tipo=1 and lst.solic_tit_id=tcs.id)
                               and tcs.protocolo=pt.id
                               and tcs.declarante=pe.id
                               and tcs.empreendimento=e.id
                               and e.id=ee.empreendimento(+)
                               and ee.municipio=lme.id(+)
                               and ee.correspondencia(+)=0
                               and tcs.situacao=lcss.id
                               and tcs.id=tsicar.solicitacao_car(+)
                               and tsicar.solicitacao_car_esquema(+)=1
                               and tsicar.situacao_envio=lses.id(+)
                                 union all

							select s.id,
							   s.solic_tit_id,
							   s.solicitacao_numero,
							   null                         titulo_numero,
							   null                         titulo_ano,
							   s.protocolo_id,
							   s.protocolo_numero,
							   s.protocolo_ano,
							   s.protocolo_numero_completo,
							   null                         projeto_digital,
							   null                         credenciado,
							   s.declarante_id,
							   s.declarante_nome_razao,
							   s.declarante_cpf_cnpj,
							   s.empreendimento_id,
							   s.empreendimento_codigo,
							   s.empreendimento_denominador,
							   s.municipio_id,
							   s.municipio_texto,
							   s.situacao_id,
							   s.situacao_texto,
							   s.situacao_motivo,
							   s.requerimento,
							   1                            origem,
							   1                            tipo,
							   tcs.situacao_envio           situacao_envio_id,
							   lses.texto                   situacao_envio_texto,
							   tcs.url_recibo,
							   tcs.arquivo,
                               tcs.codigo_imovel
						  from lst_car_solic_tit        s,
							   tab_controle_sicar       tcs,
							   lov_situacao_envio_sicar lses
						 where s.tipo = 1
						   and nvl(tcs.solicitacao_car_esquema, 1) = 1
						   and s.solic_tit_id = tcs.solicitacao_car(+)
						   and tcs.situacao_envio = lses.id(+)
						union all
						select s.id,
							   s.solic_tit_id,
							   null                         solicitacao_numero,
							   s.titulo_numero,
							   s.titulo_ano,
							   s.protocolo_id,
							   s.protocolo_numero,
							   s.protocolo_ano,
							   s.protocolo_numero_completo,
							   null                         projeto_digital,
							   null                         credenciado,
							   s.declarante_id,
							   s.declarante_nome_razao,
							   s.declarante_cpf_cnpj,
							   s.empreendimento_id,
							   s.empreendimento_codigo,
							   s.empreendimento_denominador,
							   s.municipio_id,
							   s.municipio_texto,
							   null                         situacao_id,
							   s.situacao_texto,
							   s.situacao_motivo,
							   s.requerimento,
							   1                            origem,
							   2                            tipo,
							   null                         situacao_envio_id,
							   null                         situacao_envio_texto,
							   null                         url_recibo,
							   null                         arquivo,
                               null                         codigo_imovel
						  from lst_car_solic_tit s
						 where s.tipo = 2
						union all
						select c.id,
							   c.solicitacao_id solic_tit_id,
							   c.numero solicitacao_numero,
							   null titulo_numero,
							   null titulo_ano,
							   tp.id protocolo_id,
							   tp.numero protocolo_numero,
							   tp.ano protocolo_ano,
							   tp.numero || '/' || tp.ano protocolo_numero_completo,
							   c.projeto_digital,
							   c.credenciado,
							   c.declarante_id,
							   c.declarante_nome_razao,
							   c.declarante_cpf_cnpj,
							   c.empreendimento_id,
							   (select ce.codigo from cre_empreendimento ce where ce.id = c.empreendimento_id) empreendimento_codigo,
							   c.empreendimento_denominador,
							   c.municipio_id,
							   c.municipio_texto,
							   c.situacao_id,
							   c.situacao_texto,
							   c.situacao_texto,
							   c.requerimento,
							   2 origem,
							   1 tipo,
							   tcs.situacao_envio situacao_envio_id,
							   lses.texto situacao_envio_texto,
							   tcs.url_recibo,
							   tcs.arquivo,
                               tcs.codigo_imovel
						  from lst_car_solicitacao_cred c,
							   tab_controle_sicar       tcs,
							   lov_situacao_envio_sicar lses,
							   tab_protocolo            tp
						 where nvl(tcs.solicitacao_car_esquema, 2) = 2
						   and c.solicitacao_id = tcs.solicitacao_car(+)
						   and tcs.situacao_envio = lses.id(+)
						   AND c.requerimento = TP.Requerimento(+)) l
				 where 1 = 1" + comandtxt + DaHelper.Ordenar(colunas, ordenar);

				comando.DbCommand.CommandText = String.Format(@"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", esquemaBanco);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando o retorno

					SolicitacaoListarResultados item;

					while (reader.Read())
					{
						item = new SolicitacaoListarResultados();
						item.Id = reader.GetValue<int>("solic_tit_id");
						item.Numero = reader.GetValue<string>("numero");
						item.Ano = reader.GetValue<string>("ano");
						item.IsTitulo = (item.Ano != null) ? true : false;
						item.EmpreendimentoDenominador = reader.GetValue<string>("empreendimento_denominador");
						item.EmpreendimentoCodigo = reader.GetValue<Int64>("empreendimento_codigo");
						item.MunicipioTexto = reader.GetValue<string>("municipio_texto");
						item.SituacaoID = reader.GetValue<int>("situacao_id");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.IsTitulo = reader.GetValue<int>("tipo") == 2;
						item.CredenciadoId = reader.GetValue<int>("credenciado");
						item.Origem = (eCARSolicitacaoOrigem)(reader.GetValue<int>("origem"));

						item.SituacaoArquivoCarID = reader.GetValue<int>("situacao_envio_id");
						item.SituacaoArquivoCarTexto = reader.GetValue<string>("situacao_envio_texto");
						item.UrlPdfReciboSICAR = reader.GetValue<string>("url_recibo");
						item.ArquivoSICAR = reader.GetValue<string>("arquivo");

						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion Adicionando o retorno
				}
			}

			return retorno;
		}

		public Tuple<eStatusArquivoSICAR, string> BuscaSituacaoAtualArquivoSICAR(int solicitacaoId, int origem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.situacao_envio, (select s.texto from {0}lov_situacao_envio_sicar s where s.id = e.situacao_envio) situacao_envio_texto from {0}tab_controle_sicar e where e.solicitacao_car = :solicitacaoId and e.solicitacao_car_esquema = :origem", EsquemaBanco);

				comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("origem", origem, DbType.Int32);
				using (var reader = bancoDeDados.ExecutarReader(comando))
				{
					if (!reader.Read())
						return new Tuple<eStatusArquivoSICAR, string>(eStatusArquivoSICAR.Nulo, "vazio");

					var SituacaoArquivoid = Convert.ToInt32(reader[0]);
					var SituacaoArquivoTexto = Convert.ToString(reader[1]);
					return new Tuple<eStatusArquivoSICAR, string>((eStatusArquivoSICAR)SituacaoArquivoid, SituacaoArquivoTexto);
				}
			}
		}

		public String ObterCodigoImovel(CARSolicitacao solicitacao, BancoDeDados banco = null)
		{
			String codigoRetificacao = String.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select tcs.codigo_imovel from tab_controle_sicar tcs 
                                                                    where tcs.solicitacao_car = :idSolicitacao and solicitacao_car_esquema = :schema");
				comando.AdicionarParametroEntrada("idSolicitacao", solicitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("schema", solicitacao.Esquema, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
						codigoRetificacao = reader.GetValue<String>("codigo_imovel");

					reader.Close();
				}
			}

			return codigoRetificacao;
		}

		public ControleArquivoSICAR ObterDadosControleSicar(ControleArquivoSICAR controleArquivoSICAR, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados bancoDeDados = null)
		{
			Comando comando = bancoDeDados.CriarComando(@"select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid, tcrls.id controle_id
                    from tab_car_solicitacao tcs, tab_empreendimento te, (select tcsicar.id, tcsicar.solicitacao_car from tab_controle_sicar tcsicar 
                    where tcsicar.solicitacao_car_esquema = :esquema) tcrls where tcs.empreendimento = te.id and tcs.id = tcrls.solicitacao_car(+)
                    and tcs.id = :idSolicitacao");

			comando.AdicionarParametroEntrada("esquema", (int)solicitacaoOrigem, DbType.Int32);
			comando.AdicionarParametroEntrada("idSolicitacao", controleArquivoSICAR.SolicitacaoCarId, DbType.Int32);

			using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
			{
				if (reader.Read())
				{
					controleArquivoSICAR.SolicitacaoCarTid = reader.GetValue<String>("solic_tid");
					controleArquivoSICAR.EmpreendimentoId = reader.GetValue<Int32>("emp_id");
					controleArquivoSICAR.EmpreendimentoTid = reader.GetValue<String>("emp_tid");
					controleArquivoSICAR.Id = Convert.ToInt32(reader.GetValue<String>("controle_id"));
				}
				reader.Close();
			}

			return controleArquivoSICAR;
		}

		internal string ObterCodigoSicarPorEmpreendimento(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select
                       l.codigo_imovel                      
				  from (select tsicar.codigo_imovel
                              from tab_car_solicitacao tcs, tab_protocolo pt, tab_pessoa pe, tab_empreendimento e, tab_empreendimento_endereco ee,
                                   lov_municipio lme, lov_car_solicitacao_situacao lcss, tab_controle_sicar tsicar,lov_situacao_envio_sicar lses
                             where not exists (select lst.solic_tit_id from lst_car_solic_tit lst where lst.tipo=1 and lst.solic_tit_id=tcs.id)
                               and tcs.protocolo=pt.id
                               and tcs.declarante=pe.id
                               and tcs.empreendimento=e.id
                               and e.id=ee.empreendimento(+)
                               and ee.municipio=lme.id(+)
                               and ee.correspondencia(+)=0
                               and tcs.situacao=lcss.id
                               and tcs.id=tsicar.solicitacao_car(+)
                               and tsicar.solicitacao_car_esquema(+)=1
                               and tsicar.situacao_envio=lses.id(+)
                               and e.id = :empreendimento
                                 union all
							select tcs.codigo_imovel
						  from lst_car_solic_tit        s,
							   tab_controle_sicar       tcs,
							   lov_situacao_envio_sicar lses
						 where s.tipo = 1
						   and nvl(tcs.solicitacao_car_esquema, 1) = 1
						   and s.solic_tit_id = tcs.solicitacao_car(+)
						   and tcs.situacao_envio = lses.id(+)
						   and s.empreendimento_id = :empreendimento
						union all
						select tcs.codigo_imovel
						  from lst_car_solicitacao_cred c,
							   tab_controle_sicar       tcs,
							   lov_situacao_envio_sicar lses,
							   tab_protocolo            tp
						 where nvl(tcs.solicitacao_car_esquema, 2) = 2
						   and c.solicitacao_id = tcs.solicitacao_car(+)
						   and tcs.situacao_envio = lses.id(+)
						   AND c.requerimento = TP.Requerimento(+)
						   and c.empreendimento_id = :empreendimento) l
				 where l.codigo_imovel is not null and rownum = 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<string>(comando);
			}
		}

		#endregion

		#region Auxiliares

		internal List<PessoaLst> ObterResponsaveis(int empreendimentoId)
		{
			List<PessoaLst> lst = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Responsaveis do Empreendimento

				Comando comando = bancoDeDados.CriarComando(@"
				select tp.id, (case when lv.id != 9/*Outro*/ then lv.texto else ter.especificar end) || ' - ' || nvl(tp.nome, tp.razao_social) nome_razao 
				from tab_empreendimento_responsavel ter, tab_pessoa tp, lov_empreendimento_tipo_resp lv 
				where ter.tipo = lv.id(+) and ter.responsavel = tp.id and ter.empreendimento = :empreendimento order by nome_razao", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst item;

					while (reader.Read())
					{
						item = new PessoaLst();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Texto = reader["nome_razao"].ToString();
						item.IsAtivo = true;
						lst.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return lst;
		}

		#endregion

		#region Validacao

		internal string EmpreendimentoPossuiSolicitacao(int solicitacaoId, int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				if (solicitacaoId <= 0)
				{
					comando = bancoDeDados.CriarComando(@"select ls.texto count from tab_car_solicitacao t, lov_car_solicitacao_situacao ls 
					where t.situacao = ls.id and t.empreendimento = :empreendimentoId and situacao in (1, 2, 4) /*Em cadastro, Válido, Suspenso*/ and rownum = 1", EsquemaBanco);
					comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);
					return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select ls.texto count from tab_car_solicitacao t, lov_car_solicitacao_situacao ls 
					where t.situacao = ls.id and t.empreendimento = :empreendimentoId and situacao in (1, 2, 4) /*Em cadastro, Válido, Suspenso*/ and t.id <> :solicitacaoId and rownum = 1", EsquemaBanco);
					comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);
					comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
					return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
				}
			}
		}

		internal bool EmpreendimentoProjetoGeograficoDominialidadeNaoFinalizado(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select sum(contador) from (select count(1) contador from tmp_projeto_geo p 
				where p.caracterizacao = 1 and p.empreendimento = :empreendimentoId and p.situacao <> 2/*Finalizado*/
				union all
				select count(1) contador from tmp_projeto_geo p 
				where p.caracterizacao = 1 and p.empreendimento = :empreendimentoId and p.situacao <> 2/*Finalizado*/)", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool EmpreendimentoProjetoGeograficoDominialidadeEmRascunho(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from tmp_projeto_geo p 
				where p.caracterizacao = 1 and p.empreendimento = :empreendimentoId and p.situacao <> 2/*Finalizado*/", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool ExisteProtocoloAssociado(int protocoloId, int associadoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_protocolo_associado 
				where protocolo = :protocolo and associado = :associado", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);
				comando.AdicionarParametroEntrada("associado", associadoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal String ObterNumeroProtocoloPai(int protocoloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.numero||'/'||p.ano numero from {0}tab_protocolo p 
				where p.id = (select protocolo from {0}tab_protocolo_associado where associado = :protocolo)", EsquemaBanco);
				comando.AdicionarParametroEntrada("protocolo", protocoloId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

		internal String ObterSituacaoTituloCARExistente(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select ls.texto situacao_texto from tab_titulo t, lov_titulo_situacao ls
				where t.situacao <> 5 /*Encerrado*/ and t.modelo = (select id from tab_titulo_modelo where codigo = 49 /*Cadastro Ambiental Rural*/)
				and t.empreendimento = :empreendimento and ls.id = t.situacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

		internal string ObterSituacaoTituloCARExistenteCredenciado(int empreendimentoCredenciado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ls.texto situacao_texto from tab_titulo t, lov_titulo_situacao ls, tab_empreendimento te
				where t.situacao <> 5 /*Encerrado*/ and t.modelo = (select id from tab_titulo_modelo where codigo = 49 /*Cadastro Ambiental Rural*/) and ls.id = t.situacao 
				and t.empreendimento = te.id and te.codigo = (select ce.codigo from cre_empreendimento ce where ce.id= :empreendimento) ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoCredenciado, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

		internal bool ExisteCredenciado(int solicitacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_car_solicitacao where id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", solicitacaoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool ValidarFuncionarioPermissao(int funcionarioId, int permissao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from tab_funcionario_papel fp 
																inner join tab_autenticacao_papel_perm pp on fp.papel = pp.papel
																where pp.permissao = :permissao and fp.funcionario = :funcionario");

				comando.AdicionarParametroEntrada("funcionario", funcionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("permissao", permissao, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		public bool ValidarRetificado(CARSolicitacao solicitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"SELECT SUM(SOLICITACAO_CAR_ANTERIOR) FROM TAB_CONTROLE_SICAR WHERE SOLICITACAO_CAR = :solicitacao");
				comando.AdicionarParametroEntrada("solicitacao", solicitacao.Id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion Validacao

		internal string ObterUrlGeracaoRecibo(int solicitacaoId, int schemaSolicitacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tcs.url_recibo from tab_controle_sicar tcs where tcs.solicitacao_car = :solicitacaoId and tcs.solicitacao_car_esquema = :schemaSolicitacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("schemaSolicitacao", schemaSolicitacao, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

        internal string ObterUrlGeracaoDemonstrativo(int id, int schemaSolicitacao, bool isTitulo)
        {
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
				Comando comando;
				if(!isTitulo)
				{
					comando = bancoDeDados.CriarComando(@"select tcs.codigo_imovel from tab_controle_sicar tcs where tcs.solicitacao_car = :id and tcs.solicitacao_car_esquema = :schemaSolicitacao");
					comando.AdicionarParametroEntrada("schemaSolicitacao", schemaSolicitacao, DbType.Int32);

				}
				else
				{
					comando = bancoDeDados.CriarComando(@"SELECT CODIGO_IMOVEL FROM (SELECT  CS.CODIGO_IMOVEL, TT.ID TITULO FROM TAB_TITULO TT 
															INNER JOIN TAB_CONTROLE_SICAR CS ON TT.EMPREENDIMENTO = CS.EMPREENDIMENTO
														  WHERE TT.SITUACAO = 3 /*Concluído*/ AND CS.SOLICITACAO_CAR_ESQUEMA = 1 AND TT.ID = :id
														UNION ALL
														SELECT CS.CODIGO_IMOVEL, TT.ID TITULO FROM TAB_TITULO TT 																
															INNER JOIN TAB_CONTROLE_SICAR CS ON TT.EMPREENDIMENTO = (select e.id from IDAF.TAB_EMPREENDIMENTO e
															where e.codigo = (select ec.codigo from IDAFCREDENCIADO.TAB_EMPREENDIMENTO ec where ec.id = CS.EMPREENDIMENTO)) 
														  WHERE TT.SITUACAO = 3 /*Concluído*/ AND CS.SOLICITACAO_CAR_ESQUEMA = 2 AND TT.ID = :id)
														  WHERE ROWNUM = 1 ORDER BY TITULO DESC");
				}

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                return bancoDeDados.ExecutarScalar<String>(comando);
            }
        }

		internal bool VerificarSeEmpreendimentoPossuiSolicitacaoEmCadastro(int empreendimentoID)
		{
			//TODO:Validacao Verifica se há solicitação CAR em cadastro
			var sql = @"select sum(valor) resultado
			  from (select count(c.id) valor
					  from  tab_car_solicitacao     c,
                            tab_empreendimento      e,
                            tab_controle_sicar      cs  
					 where c.empreendimento = :empreendimento
                        and cs.solicitacao_car = c.id

                        and c.situacao = 1 /*Em Cadastro*/
					union all
					select count(cc.id) valor
					  from tab_car_solicitacao_cred cc,
						   cre_empreendimento       ce,
						   tab_empreendimento       e,
                           tab_controle_sicar       cs
					             where cc.empreendimento = ce.id
					                and ce.codigo = e.codigo
					                and e.id = :empreendimento
                                    and cs.solicitacao_car = cc.id
             
                                    and cc.situacao = 1 /*Em Cadastro*/)";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(sql);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);

				try
				{
					return !Convert.ToBoolean(bancoDeDados.ExecutarScalar<int>(comando));
				}
				catch
				{
					return false;
				}
			}
		}

		internal bool VerificarSeEmpreendimentoPossuiSolicitacaoValidaEEnviada(int empreendimentoID)
		{
			//TODO:Validacao de Solicitacao de Inscricao para Salvar Titulo CAR
			var sql = @"select sum(valor) from (select count(c.id) valor from tab_car_solicitacao c, tab_controle_sicar s
					where c.id=s.solicitacao_car and s.solicitacao_car_esquema=1 and c.situacao=2 and s.situacao_envio=6 and c.empreendimento=:empreendimento
					union all select count(cc.id) valor from tab_car_solicitacao_cred cc, tab_controle_sicar ss, cre_empreendimento ce, tab_empreendimento e
					where cc.empreendimento=ce.id and ce.codigo=e.codigo and cc.id=ss.solicitacao_car and ss.solicitacao_car_esquema=2 and cc.situacao=2
					and ss.situacao_envio=6 and e.id=:empreendimento)";

			//TODO:Validacao Verifica se há solicitação CAR válida e arquivo entregue
			sql = @"select sum(valor)
			  from (select count(c.id) valor
					  from  tab_car_solicitacao     c,
                            tab_empreendimento      e,
                            tab_controle_sicar      cs  
					 where c.empreendimento = :empreendimento
                        and cs.solicitacao_car = c.id
                        and c.situacao = 2 /*Válido*/
                        and cs.situacao_envio = 6 /*Arquivo Entregue*/
					union all
					select count(cc.id) valor
					  from tab_car_solicitacao_cred cc,
						   cre_empreendimento       ce,
						   tab_empreendimento       e,
                           tab_controle_sicar       cs
					             where cc.empreendimento = ce.id
					                and ce.codigo = e.codigo
					                and e.id = :empreendimento
                                    and cs.solicitacao_car = cc.id
             
                                    and cc.situacao = 2 /*Válido*/
                                    and cs.situacao_envio = 6 /*Arquivo Entregue*/)";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(sql);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);

				try
				{
					return Convert.ToBoolean(bancoDeDados.ExecutarScalar<int>(comando));
				}
				catch {
					return false;
				}
			}
		}

        internal void FazerVirarPassivo(int solicitacaoID, BancoDeDados banco)
		{
			//TODO:Validacao de Solicitacao de Inscricao para Salvar Titulo CAR
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"update tab_controle_sicar s set s.solicitacao_passivo = 1, s.solicitacao_situacao_aprovado = 5 where s.solicitacao_car=:solicitacao_car and s.solicitacao_car_esquema = 1");

				comando.AdicionarParametroEntrada("solicitacao_car", solicitacaoID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		internal void SubstituirPorTituloCARCredenciado(int empreendimentoInstitucionalID, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(
					" begin " +
					"   for i in ( select tcs.id, tcs.situacao, tcs.situacao_data " +
					"         from tab_car_solicitacao_cred tcs " +
					"          where tcs.empreendimento in " +
					"            (select ce.id " +
					"             from cre_empreendimento ce " +
					"              where ce.codigo in " +
					"                (select te.codigo " +
					"                 from tab_empreendimento te " +
					"                  where te.id = :empreendimento))) loop " +
					"   update tab_car_solicitacao_cred c  " +
					"      set c.situacao = 5/*Substituído pelo título CAR*/, " +
					"        c.situacao_data = sysdate, " +
					"        c.situacao_anterior = i.situacao, " +
					"        c.situacao_anterior_data = i.situacao_data " +
					"    where c.id = i.id and c.situacao = 2 /*Válido*/; " +
					"   end loop; " +
					" end; ");

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoInstitucionalID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		internal void FazerVirarPassivoCredenciado(int empreendimentoInstitucionalID, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(
					" begin "+
					"   for i in ( select tcs.id, tcs.situacao, tcs.situacao_data "+
					"         from tab_car_solicitacao_cred tcs "+
					"          where tcs.empreendimento in "+
					"            (select ce.id "+
					"             from cre_empreendimento ce "+
					"              where ce.codigo in "+
					"                (select te.codigo "+
					"                 from tab_empreendimento te "+
					"                  where te.id = :empreendimento))) loop "+
					"     update tab_controle_sicar ts "+
					"        set ts.solicitacao_passivo = 1, "+
					"            ts.solicitacao_situacao_aprovado = 5/*Substituído pelo título CAR*/ "+
					"      where ts.solicitacao_car = i.id; "+
					"   end loop;  "+
					" end; ");

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoInstitucionalID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

		internal void AtualizarLstConsultaCredenciado(int empreendimentoInstitucionalID, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(
					" begin  " +
					"   for i in ( select tcs.id, tcs.situacao, tcs.situacao_data  "+
					"         from tab_car_solicitacao_cred tcs  "+
					"          where tcs.empreendimento in  "+
					"            (select ce.id  "+
					"             from cre_empreendimento ce  "+
					"              where ce.codigo in  "+
					"                (select te.codigo  "+
					"                 from tab_empreendimento te  "+
					"                  where te.id = :empreendimento))) loop  "+
					"       lst_consulta_cred.carsolicitacao(i.id); "+
					"   end loop; "+
					" end;");

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoInstitucionalID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}

	}
}