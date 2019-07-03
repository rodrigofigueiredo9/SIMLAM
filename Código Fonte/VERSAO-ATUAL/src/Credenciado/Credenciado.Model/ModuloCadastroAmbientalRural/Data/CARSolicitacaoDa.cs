using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data
{
	public class CARSolicitacaoDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public string UsuarioCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}
		public string UsuarioInterno
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}
		public string UsuarioConsulta
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioConsulta); }
		}

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }


		#endregion

		#region Ações DML

		internal int Salvar(CARSolicitacao solicitacao, BancoDeDados banco)
		{
			if (solicitacao == null)
			{
				throw new Exception("Solicitacao é nula.");
			}

			return Criar(solicitacao, banco);
		}

		internal int Criar(CARSolicitacao solicitacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}tab_car_solicitacao(id, numero, data_emissao, situacao, situacao_data, requerimento,  atividade, empreendimento, declarante, tid, credenciado, projeto_digital, passivo_enviado) 
				values(:id, :id, :data_emissao, :situacao, :situacao_data, :requerimento, :atividade, :empreendimento, :declarante, :tid, :credenciado, :projeto_digital, 1)",
				UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", solicitacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_emissao", DateTime.Now, DbType.Date);
				comando.AdicionarParametroEntrada("situacao", (int)eCARSolicitacaoSituacao.EmCadastro, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_data", DateTime.Now, DbType.Date);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("requerimento", solicitacao.Requerimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", solicitacao.Atividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", solicitacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("declarante", solicitacao.Declarante.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("projeto_digital", solicitacao.ProjetoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(solicitacao.Id, eHistoricoArtefato.carsolicitacao, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();

				//Inserir na fila para gerar o .CAR para envio para o SICAR
				InserirFilaArquivoCarSicar(solicitacao, eCARSolicitacaoOrigem.Credenciado, banco);

				return solicitacao.Id;

			}
		}

		internal void AlterarSituacao(CARSolicitacao entidade, BancoDeDados banco, eHistoricoAcao acao = eHistoricoAcao.alterarsituacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update tab_car_solicitacao t
				set t.situacao               = :situacao,
					t.situacao_data          = sysdate,
					t.situacao_anterior      = t.situacao,
					t.situacao_anterior_data = t.situacao_data,
					t.motivo                 = :motivo,
					t.tid                    = :tid
				where t.id = :id");

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", entidade.Motivo, DbType.String);
				comando.AdicionarParametroEntrada("situacao", entidade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(entidade.Id, eHistoricoArtefato.carsolicitacao, acao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void InserirFilaArquivoCarSicar(CARSolicitacao solicitacao, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados banco = null)
		{
			string requisicao_fila = string.Empty;
            var selectNULO = true;
            #region Requisição Credenciado
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando("select s.solicitacao_id solic_id, s.tid solic_tid, s.empreendimento_id emp_id, s.empreendimento_tid emp_tid from hst_car_solicitacao s where s.solicitacao_id = :idSolicitacao       "+
					"and s.tid = (select ss.tid from tab_car_solicitacao ss where ss.id= :idSolicitacao) order by id desc", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("idSolicitacao", solicitacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						requisicao_fila = "{"
											+ "\"origem\": \"" + solicitacaoOrigem.ToString().ToLower() + "\", "
											+ "\"empreendimento\":" + reader.GetValue<int>("emp_id") + ", "
											+ "\"empreendimento_tid\": \"" + reader.GetValue<string>("emp_tid") + "\","
											+ "\"solicitacao_car\": " + reader.GetValue<int>("solic_id") + ","
											+ "\"solicitacao_car_tid\": \"" + reader.GetValue<string>("solic_tid") + "\"" +
										  "}";
					}

					reader.Close();
				}
                
				if (requisicao_fila != string.Empty)
				{
                    selectNULO = false;
					comando = bancoDeDados.CriarComando(@"
				    insert into tab_scheduler_fila (id, tipo, requisitante, requisicao, empreendimento, data_criacao, data_conclusao, resultado, sucesso) 
                    values (seq_tab_scheduler_fila.nextval, 'gerar-car', 0, :requisicao, 0, NULL, NULL, '', '')", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("requisicao", requisicao_fila, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					SalvarControleArquivoCarSicar(solicitacao, eStatusArquivoSICAR.AguardandoEnvio, solicitacaoOrigem, banco);

					bancoDeDados.Commit();
				}
			}
#endregion
            //Se a consulta no credenciado não retornar, fará a consulta no institucional
            #region Requisição Institucional
            if (selectNULO)
            {
                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
                {
                    bancoDeDados.IniciarTransacao();

                    Comando comando = bancoDeDados.CriarComando("select s.solicitacao_id solic_id, s.tid solic_tid, s.empreendimento_id emp_id, s.empreendimento_tid emp_tid from hst_car_solicitacao s where s.solicitacao_id = :idSolicitacao    "+
					" and s.tid = (select ss.tid from tab_car_solicitacao ss where ss.id= :idSolicitacao) order by id desc");

                    comando.AdicionarParametroEntrada("idSolicitacao", solicitacao.Id, DbType.Int32);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        while (reader.Read())
                        {
                            requisicao_fila = "{"
                                                + "\"origem\": \"" + solicitacaoOrigem.ToString().ToLower() + "\", "
                                                + "\"empreendimento\":" + reader.GetValue<int>("emp_id") + ", "
                                                + "\"empreendimento_tid\": \"" + reader.GetValue<string>("emp_tid") + "\","
                                                + "\"solicitacao_car\": " + reader.GetValue<int>("solic_id") + ","
                                                + "\"solicitacao_car_tid\": \"" + reader.GetValue<string>("solic_tid") + "\"" +
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
            #endregion
        }

		internal void SalvarControleArquivoCarSicar(CARSolicitacao solicitacao, eStatusArquivoSICAR statusArquivoSICAR, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados banco = null)
		{
			ControleArquivoSICAR controleArquivoSICAR = new ControleArquivoSICAR();
			controleArquivoSICAR.SolicitacaoCarId = solicitacao.Id;
            CARSolicitacao retificado = new CARSolicitacao();
			String codigoImovelTxt = String.Empty;
            String codigoRetificacao = String.Empty;

            retificado = ObterPorEmpreendimento(solicitacao.Empreendimento.Codigo ?? 0, false);

            if (retificado != null)
            {
                using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			    {
				    bancoDeDados.IniciarTransacao();

				    #region Coleta de dados
                    Comando comando = bancoDeDados.CriarComando(@"select tcs.codigo_imovel from tab_controle_sicar tcs 
                                                                    where tcs.solicitacao_car = :idSolicitacao and solicitacao_car_esquema = :schema");
                    comando.AdicionarParametroEntrada("idSolicitacao", retificado.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("schema", retificado.Esquema, DbType.Int32);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        if (reader.Read())
                        {
                            codigoRetificacao = reader.GetValue<String>("codigo_imovel");
                        }
                        reader.Close();
                    }
                    #endregion
                }
            }

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				#region Coleta de dados
				Comando comando = bancoDeDados.CriarComando(@"select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid, tcrls.id controle_id
                    from tab_car_solicitacao tcs, tab_empreendimento te, (select tcsicar.id, tcsicar.solicitacao_car from tab_controle_sicar tcsicar 
                    where tcsicar.solicitacao_car_esquema = :esquema) tcrls where tcs.empreendimento = te.id and tcs.id = tcrls.solicitacao_car(+)
                    and tcs.id = :idSolicitacao", UsuarioCredenciado);

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

				#endregion

				if (controleArquivoSICAR.Id == 0)
				{
					#region Criar controle arquivo SICAR
					comando = bancoDeDados.CriarComando(@"
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
					if(retificado == null)
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
                    
                    comando.AdicionarParametroEntrada("codigo_imovel", codigoRetificacao, DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					controleArquivoSICAR.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					#endregion
				}
				else
				{
					#region Editar controle arquivo SICAR
					if (!String.IsNullOrWhiteSpace(codigoRetificacao))
						codigoImovelTxt = " codigo_imovel = :codigo_imovel, ";

					comando = bancoDeDados.CriarComando(@"
				    update tab_controle_sicar r set r.empreendimento_tid = :empreendimento_tid, r.solicitacao_car_tid = :solicitacao_car_tid, r.situacao_envio = :situacao_envio,
					solicitacao_car_anterior = :solicitacao_car_anterior, solicitacao_car_anterior_tid = :solicitacao_car_anterior_tid, solicitacao_car_ant_esquema = :solicitacao_car_ant_esquema,
					" + codigoImovelTxt + @"
                    r.tid = :tid, r.arquivo = null where r.id = :id", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("empreendimento_tid", controleArquivoSICAR.EmpreendimentoTid, DbType.String);
					comando.AdicionarParametroEntrada("solicitacao_car_tid", controleArquivoSICAR.SolicitacaoCarTid, DbType.String);
					comando.AdicionarParametroEntrada("situacao_envio", (int)statusArquivoSICAR, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("id", controleArquivoSICAR.Id, DbType.Int32);
					if (retificado == null)
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
					if(!String.IsNullOrWhiteSpace(codigoRetificacao))
						comando.AdicionarParametroEntrada("codigo_imovel", codigoRetificacao, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					#endregion
				}

				GerarHistoricoControleArquivoCarSicar(controleArquivoSICAR.Id, banco);

				bancoDeDados.Commit();
			}
		}

		internal void GerarHistoricoControleArquivoCarSicar(int controleArquivoId, BancoDeDados banco = null)
		{
			if (controleArquivoId > 0)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
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
					end;", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("id", controleArquivoId, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
					bancoDeDados.Commit();

					#endregion
				}
			}
		}

		#endregion

		#region Obter/Filtrar

		internal List<PessoaLst> ObterDeclarantes(int requerimentoId)
		{
			List<PessoaLst> retorno = new List<PessoaLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl(r.interessado,0) id, 'Interessado' || ' - ' || nvl(tp.nome, tp.razao_social) texto from {0}tab_requerimento r, 
					{0}tab_pessoa tp where r.interessado = tp.id and r.id = :id union all select nvl(er.responsavel,0) id, ler.texto || ' - ' || nvl(tp.nome, tp.razao_social) texto from 
					{0}tab_requerimento r, {0}tab_empreendimento_responsavel er, {0}tab_pessoa tp, {0}lov_empreendimento_tipo_resp ler where er.responsavel = tp.id and er.tipo = ler.id and r.id = :id
					and r.empreendimento = er.empreendimento union all select nvl(rr.responsavel,0) id, 'Responsável técnico' || ' - ' || nvl(tp.nome, tp.razao_social) texto from 
					{0}tab_requerimento_responsavel rr, {0}tab_pessoa tp where rr.responsavel = tp.id and rr.requerimento = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst pes;
					while (reader.Read())
					{
						pes = new PessoaLst();
						pes.Id = reader.GetValue<int>("id");
						pes.Texto = reader.GetValue<string>("texto");
						pes.IsAtivo = true;
						retorno.Add(pes);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		public CARSolicitacao Obter(int id, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando("select s.tid," + 
					  " s.numero,"+
					  " s.data_emissao,"+
					  " s.situacao_data,"+
					  " l.id situacao,"+
					  " l.texto situacao_texto,"+
					  " s.situacao_anterior,"+
					  " la.texto situacao_anterior_texto,"+
					  " s.situacao_anterior_data,"+
					  " nvl(pes.nome, pes.razao_social) declarante_nome_razao,"+
					  " s.requerimento,"+
					  " s.atividade,"+
					  " e.id empreendimento_id,"+
					  " e.denominador empreendimento_nome,"+
					  " e.codigo empreendimento_codigo,"+
					  " s.declarante,"+

					  " p.id protocolo_id,"+
					  " p.protocolo,"+
                      " p.numero protocolo_numero,"+
                      " p.ano protocolo_ano,"+

					  " s.credenciado autor_id,"+
					  " nvl(f.nome, f.razao_social) autor_nome,"+
					  " lct.texto  autor_tipo,"+
					  " 'Credenciado' autor_modulo,"+

					  " s.motivo,"+
					  " tr.data_criacao requerimento_data_cadastro,"+
					  " s.projeto_digital"+
					" from "+
					  " tab_car_solicitacao          s,"+
					  " lov_car_solicitacao_situacao l,"+
					  " lov_car_solicitacao_situacao la,"+
					  " tab_empreendimento           e,"+
					  " tab_pessoa                   pes,"+
					  " tab_requerimento             tr,"+
					  " tab_credenciado              tc,"+
					  " tab_pessoa                   f,"+
					  " lov_credenciado_tipo         lct,"+
					  " ins_protocolo                p"+
					" where s.situacao = l.id"+
					" and s.situacao_anterior = la.id(+)"+
					" and s.empreendimento = e.id"+
					" and s.declarante = pes.id"+
					" and s.requerimento = tr.id"+
					" and s.empreendimento = e.id"+
					" and tc.id = s.credenciado"+
					" and f.id = tc.pessoa"+
					" and lct.id = tc.tipo"+
					" and s.requerimento=p.requerimento(+)"+
					" and s.id = :id", UsuarioCredenciado);

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
						solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
						solicitacao.Requerimento.DataCadastro = reader.GetValue<DateTime>("requerimento_data_cadastro");
						solicitacao.Atividade.Id = reader.GetValue<Int32>("atividade");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
						solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante");
						solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nome_razao");

						solicitacao.Protocolo.Id = reader.GetValue<Int32>("protocolo_id");
						solicitacao.Protocolo.IsProcesso = reader.GetValue<Int32>("protocolo") == 1;
						solicitacao.Protocolo.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_numero");
						solicitacao.Protocolo.Ano = reader.GetValue<Int32>("protocolo_ano");

						solicitacao.AutorId = reader.GetValue<Int32>("autor_id");
						solicitacao.AutorNome = reader.GetValue<String>("autor_nome");
						solicitacao.AutorTipoTexto = reader.GetValue<String>("autor_tipo");
						solicitacao.AutorModuloTexto = reader.GetValue<String>("autor_modulo");

						solicitacao.Motivo = reader.GetValue<String>("motivo");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_digital");
					}

					reader.Close();
				}

				#endregion
			}

			return solicitacao;
		}

        internal CARSolicitacao ObterOnInstitucional(int id, BancoDeDados banco = null)
        {
            CARSolicitacao solicitacao = new CARSolicitacao();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                #region Solicitação

                Comando comando = bancoDeDados.CriarComando(@"select s.tid,
                    s.numero,
					   s.data_emissao,
					   s.situacao_data,
					   l.id situacao,
					   l.texto situacao_texto,
					   s.situacao_anterior,
					   la.texto situacao_anterior_texto,
					   s.situacao_anterior_data,
					   nvl(pes.nome, pes.razao_social) declarante_nome_razao,
					   s.requerimento,
					   s.atividade,
					   e.id empreendimento_id,
					   e.denominador empreendimento_nome,
					   e.codigo empreendimento_codigo,
					   s.declarante,

					   p.id protocolo_id,
					   p.protocolo,
                       p.numero protocolo_numero,
                       p.ano protocolo_ano,

					   s.autor autor_id,
					   f.nome autor_nome,
					   tf.texto  autor_tipo,
					   'Credenciado' autor_modulo,

					   s.motivo,
					   tr.data_criacao requerimento_data_cadastro,
					   pg.ID projeto_digital,
					   s.arquivo

                    from tab_car_solicitacao s 
                        INNER JOIN          lov_car_solicitacao_situacao l      ON  s.SITUACAO =  l.ID
					    LEFT JOIN           lov_car_solicitacao_situacao la     ON  s.SITUACAO_ANTERIOR = la.ID
					    INNER JOIN          tab_empreendimento e                ON  s.EMPREENDIMENTO = e.ID
					    INNER JOIN          tab_pessoa  pes                     ON  s.DECLARANTE = pes.ID
					    INNER JOIN          tab_requerimento  tr                ON  s.REQUERIMENTO = tr.ID
					    INNER JOIN          tab_protocolo  p                    ON  s.REQUERIMENTO = p.REQUERIMENTO
                        INNER JOIN          tab_funcionario f                   ON  s.AUTOR = f.ID
                        INNER JOIN          lov_funcionario_tipo tf             ON  f.TIPO = tf.ID
                        INNER JOIN          crt_projeto_geo pg                  ON  s.EMPREENDIMENTO = pg.EMPREENDIMENTO
                    where 
					  s.id = :id
                    ");

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
                        solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
                        solicitacao.Requerimento.DataCadastro = reader.GetValue<DateTime>("requerimento_data_cadastro");
                        solicitacao.Atividade.Id = reader.GetValue<Int32>("atividade");
                        solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
                        solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
                        solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
                        solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante");
                        solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nome_razao");

                        solicitacao.Protocolo.Id = reader.GetValue<Int32>("protocolo_id");
                        solicitacao.Protocolo.IsProcesso = reader.GetValue<Int32>("protocolo") == 1;
                        solicitacao.Protocolo.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_numero");
                        solicitacao.Protocolo.Ano = reader.GetValue<Int32>("protocolo_ano");

                        solicitacao.AutorId = reader.GetValue<Int32>("autor_id");
                        solicitacao.AutorNome = reader.GetValue<String>("autor_nome");
                        solicitacao.AutorTipoTexto = reader.GetValue<String>("autor_tipo");
                        solicitacao.AutorModuloTexto = reader.GetValue<String>("autor_modulo");

                        solicitacao.Motivo = reader.GetValue<String>("motivo");
                        solicitacao.Arquivo = reader.GetValue<int>("arquivo");
                        //solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_digital");
                    }

                    reader.Close();
                }

                #endregion
            }

            return solicitacao;
        }

		internal CARSolicitacao ObterHistorico(int id, string tid, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
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
							r.requerimento_id,
							r.data_criacao            requerimento_data_criacao,
							a.id                      atividade_id,
							a.atividade               atividade_texto,
							e.empreendimento_id,
							e.denominador             empreendimento_nome,
							e.codigo                  empreendimento_codigo,
							s.declarante_id,
							s.credenciado_id autor_id,
							nvl(f.nome, f.razao_social) autor_nome,
							tc.tipo_texto  autor_tipo,
							'Credenciado' autor_modulo,
							nvl(d.nome, d.razao_social) declarante_nomerazao,
							s.motivo,
							pg.projeto_geo_id,
							s.executor_tipo_id
						from hst_car_solicitacao s,
							hst_requerimento    r,
							hst_empreendimento  e,
							hst_pessoa          d,
							tab_atividade       a,
							hst_crt_projeto_geo pg,
							hst_credenciado     tc,
							hst_pessoa          f
						where s.empreendimento_id = e.empreendimento_id
						and s.empreendimento_tid = e.tid
						and s.declarante_id = d.pessoa_id
						and s.declarante_tid = d.tid
						and s.atividade_id = a.id
						and s.projeto_geo_id = pg.projeto_geo_id
						and s.projeto_geo_tid = pg.tid
						and r.requerimento_id = s.requerimento_id
						and r.tid = s.requerimento_tid
						and f.pessoa_id = tc.pessoa_id
						and f.tid = tc.pessoa_tid
						and tc.credenciado_id = s.credenciado_id
						and tc.tid = s.credenciado_tid
						and s.solicitacao_id = :id
						and s.tid = :tid", UsuarioCredenciado);

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
						solicitacao.AutorTipoTexto = reader.GetValue<String>("autor_tipo");
						solicitacao.AutorModuloTexto = reader.GetValue<String>("autor_modulo");

						solicitacao.Motivo = reader.GetValue<String>("motivo");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_geo_id");

						solicitacao.ExecutorTipoID = reader.GetValue<Int32>("executor_tipo_id");
					}

					reader.Close();
				}

				#endregion
			}

			return solicitacao;
		}

		internal CARSolicitacao ObterUltimoHistoricoSituacao(int id, eCARSolicitacaoSituacao situacao, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select 
					s.tid,
					s.situacao_id,
					s.situacao_texto,
					s.situacao_data,
					s.situacao_anterior_id,
					s.situacao_anterior_texto,
					s.situacao_anterior_data,
					s.motivo
				from hst_car_solicitacao s
				where s.data_execucao = (select max(h.data_execucao) from hst_car_solicitacao h where h.solicitacao_id = s.solicitacao_id and h.situacao_id = :situacao)
				and s.solicitacao_id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)situacao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = id;
						solicitacao.Tid = reader.GetValue<String>("tid");

						solicitacao.SituacaoId = reader.GetValue<Int32>("situacao_id");
						solicitacao.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						solicitacao.DataSituacao.DataTexto = reader.GetValue<String>("situacao_data");
						solicitacao.SituacaoAnteriorId = reader.GetValue<Int32>("situacao_anterior_id");
						solicitacao.SituacaoAnteriorTexto = reader.GetValue<String>("situacao_anterior_texto");
						solicitacao.DataSituacaoAnterior.DataTexto = reader.GetValue<String>("situacao_anterior_data");
						solicitacao.Motivo = reader.GetValue<String>("motivo");
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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select hcs.tid, hcs.projeto_geo_id, hcs.projeto_geo_tid, hcs.empreendimento_id 
				from hst_car_solicitacao hcs where hcs.id = (select min(h.id) from hst_car_solicitacao h where h.solicitacao_id = :id and h.situacao_id = :situacao)", UsuarioCredenciado);

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

		internal CARSolicitacao ObterPorRequerimento(int requerimentoID, bool simplificado = false, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select c.id solicitacao from tab_car_solicitacao c where requerimento = :id and (situacao <> 3 or situacao <> 4) /*Invalido*/", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", requerimentoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = solicitacao.ProjetoId = reader.GetValue<Int32>("solicitacao");
					}
					reader.Close();
				}

				if (solicitacao.Id > 0)
				{
					solicitacao = Obter(solicitacao.Id, banco: bancoDeDados);
				}

				#endregion
			}

			return solicitacao;
		}

		internal CARSolicitacao ObterPorProjetoDigital(int projetoDigitalId, BancoDeDados banco)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select c.id solicitacao from tab_car_solicitacao c where projeto_digital = :id and (situacao <> 3 or situacao <> 4) /*Invalido*/", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", projetoDigitalId, DbType.Int32);

				int solicitacaoId = 0;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacaoId = solicitacao.ProjetoId = reader.GetValue<Int32>("solicitacao");
					}
					reader.Close();
				}

				solicitacao = solicitacaoId <= 0 ? null : Obter(solicitacaoId, banco: bancoDeDados);

				#endregion
			}

			return solicitacao;
		}

        internal CARSolicitacao ObterPorProjetoDigitalSituacao(int projetoDigitalId, BancoDeDados banco = null)
        {
            CARSolicitacao solicitacao = new CARSolicitacao();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            {
                #region Solicitação

                Comando comando = bancoDeDados.CriarComando(@"select c.id solicitacao from tab_car_solicitacao c where situacao != 3 and projeto_digital = :id ", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("id", projetoDigitalId, DbType.Int32);

                int solicitacaoId = 0;

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        solicitacaoId = solicitacao.ProjetoId = reader.GetValue<Int32>("solicitacao");
                    }
                    reader.Close();
                }

                solicitacao = solicitacaoId <= 0 ? null : Obter(solicitacaoId, banco: bancoDeDados);

                #endregion
            }

            return solicitacao;
        }

        internal CARSolicitacao ObterPorEmpreendimento(Int64 empreendimentoCod, bool comPendente, BancoDeDados banco = null)
        {
            CARSolicitacao solicitacao = new CARSolicitacao();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
				#region Solicitação não válida
				if (comPendente)
				{
					//CREDENCIADO
					Comando comando = bancoDeDados.CriarComando(@"select * from	(  select * from	(
																	select c.id solicitacao, c.SITUACAO, 1 esquema from tab_car_solicitacao c 
																		  inner join tab_empreendimento ec on ec.id = c.empreendimento 
																	  where c.situacao != 3 and ec.codigo = :codigo 
																	  union all
																	  select c.id solicitacao, c.SITUACAO, 2 esquema from {0}tab_car_solicitacao c 
																		  inner join {0}tab_empreendimento ec on ec.id = c.empreendimento 
																	  where c.situacao != 3 and ec.codigo = :codigo 
																  ) order by 
																	  case situacao 
																	  when 1 then 7
																	  else situacao end
																	  desc) where rownum = 1", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("codigo", empreendimentoCod, DbType.Int32);

					int solicitacaoId = 0;

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							solicitacaoId = solicitacao.ProjetoId = reader.GetValue<Int32>("solicitacao");
							solicitacao.Esquema = reader.GetValue<Int32>("esquema");
						}
						reader.Close();
					}

					if (solicitacaoId > 0)
					{
						if (solicitacao.Esquema == 2)
						{
							BancoDeDados bd = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado);

							solicitacao = Obter(solicitacaoId, banco: bd);
							return solicitacao;
						}
						else if (solicitacao.Esquema == 1)
						{
							CARSolicitacaoInternoDa _da = new CARSolicitacaoInternoDa();

							solicitacao = _da.Obter(solicitacaoId, banco: bancoDeDados);
							return solicitacao;
						}

					}
				}
				else
				{
					//CREDENCIADO
					Comando comando = bancoDeDados.CriarComando(@"select * from	(
																	select c.id solicitacao, c.SITUACAO, 1 esquema from tab_car_solicitacao c 
																		  inner join tab_empreendimento ec on ec.id = c.empreendimento 
																	  where c.situacao not in (1, 3, 6) and ec.codigo = :codigo 
																	  union all
																	  select c.id solicitacao, c.SITUACAO, 2 esquema from {0}tab_car_solicitacao c 
																		  inner join {0}tab_empreendimento ec on ec.id = c.empreendimento 
																	  where c.situacao not in (1, 3, 6) and ec.codigo = :codigo 
																  )  where rownum = 1", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("codigo", empreendimentoCod, DbType.Int32);

					int solicitacaoId = 0;
					int esquema = 0;

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							solicitacaoId = solicitacao.ProjetoId = reader.GetValue<Int32>("solicitacao");
							solicitacao.Esquema = esquema = reader.GetValue<Int32>("esquema");
						}
						reader.Close();
					}

					if (solicitacaoId > 0)
					{
						if (solicitacao.Esquema == 2)
						{
							BancoDeDados bd = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado);

							solicitacao = Obter(solicitacaoId, banco: bd);
							solicitacao.Esquema = esquema;
							return solicitacao;
						}
						else if (solicitacao.Esquema == 1)
						{
							CARSolicitacaoInternoDa _da = new CARSolicitacaoInternoDa();

							solicitacao = _da.Obter(solicitacaoId, banco: bancoDeDados);
							solicitacao.Esquema = esquema;
							return solicitacao;
						}

					}
				}
				
                
                #endregion
            }

            return null;
        }

        internal ControleArquivoSICAR ObterControleArquivoSicar(int empreendimentoId, BancoDeDados banco = null)
        {
            ControleArquivoSICAR controleSicar = new ControleArquivoSICAR();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            {
                #region Solicitação

                Comando comando = bancoDeDados.CriarComando(@"SELECT CODIGO_IMOVEL, STATUS_SICAR, SOLICITACAO_CAR  FROM {0}TAB_CONTROLE_SICAR WHERE SOLICITACAO_CAR_ESQUEMA = 2 AND EMPREENDIMENTO = :id ", UsuarioInterno);

                comando.AdicionarParametroEntrada("id", empreendimentoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        controleSicar.CodigoImovel = reader.GetValue<String>("CODIGO_IMOVEL");
                        controleSicar.InscricaoSicar = reader.GetValue<String>("STATUS_SICAR");
                        controleSicar.SolicitacaoCarId = reader.GetValue<Int32>("SOLICITACAO_CAR");
                    }
                    reader.Close();
                }              
                #endregion
            }

            return controleSicar;
        }
		internal Resultados<SolicitacaoListarResultados> Filtrar(Filtro<SolicitacaoListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<SolicitacaoListarResultados> retorno = new Resultados<SolicitacaoListarResultados>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioConsulta))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = string.IsNullOrEmpty(UsuarioConsulta) ? "" : UsuarioConsulta + ".";
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

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(1) from (
                    select s.id, s.solic_tit_id interno_id, s.solic_tit_id, s.solicitacao_numero, null titulo_numero, null titulo_ano, 
                    s.protocolo_id, s.protocolo_numero, s.protocolo_ano, s.protocolo_numero_completo, null projeto_digital, null 
                    credenciado, s.declarante_id, s.declarante_nome_razao, s.declarante_cpf_cnpj, s.empreendimento_id, e.codigo empreendimento_codigo,
                    s.empreendimento_denominador, s.municipio_id, s.municipio_texto, s.situacao_id, s.situacao_texto, s.situacao_motivo, s.requerimento, 1 origem, 1 tipo,
                    tcs.situacao_envio situacao_envio_id, lses.texto situacao_envio_texto, tcs.url_recibo, tcs.arquivo, tcs.codigo_imovel 
                    from lst_car_solic_tit s, tab_controle_sicar tcs, lov_situacao_envio_sicar lses, tab_empreendimento e where s.tipo = 1 and nvl(tcs.solicitacao_car_esquema, 1) = 1 
                    and s.solic_tit_id = tcs.solicitacao_car(+) and tcs.situacao_envio = lses.id(+) and e.id = s.empreendimento_id
                    and (s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp where r.interessado = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp, tab_requerimento_responsavel trr
                    where r.id = trr.requerimento and trr.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id
                    from tab_requerimento r, tab_pessoa tp, tab_empreendimento_responsavel ter where r.empreendimento = ter.empreendimento and ter.responsavel = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp 
                    where r.autor = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj))
                    union all 
                    /*Solicitacao Titulo*/
                    select s.id, 0 interno_id, s.solic_tit_id, null solicitacao_numero, s.titulo_numero, 
                    s.titulo_ano, s.protocolo_id, s.protocolo_numero, s.protocolo_ano, s.protocolo_numero_completo, null projeto_digital, null credenciado, 
                    s.declarante_id, s.declarante_nome_razao, s.declarante_cpf_cnpj, s.empreendimento_id, s.empreendimento_codigo, s.empreendimento_denominador, 
                    s.municipio_id, s.municipio_texto, null situacao_id, s.situacao_texto, s.situacao_motivo, s.requerimento, 1 origem, 2 tipo,
                    null situacao_envio_id, null situacao_envio_texto, null url_recibo, null arquivo, null codigo_imovel
                    from lst_car_solic_tit s where s.tipo = 2 
                    and (s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp where r.interessado = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp, tab_requerimento_responsavel trr
                    where r.id = trr.requerimento and trr.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id
                    from tab_requerimento r, tab_pessoa tp, tab_empreendimento_responsavel ter where r.empreendimento = ter.empreendimento and ter.responsavel = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp 
                    where r.autor = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj))
                    union all 
                    /*Solicitacao Credenciado*/
                    select c.id, 0 interno_id, c.solicitacao_id solic_tit_id, c.numero solicitacao_numero, null titulo_numero, 
                    null titulo_ano, null protocolo_id, null protocolo_numero, null protocolo_ano, null protocolo_numero_completo, c.projeto_digital, 
                    c.credenciado, c.declarante_id, c.declarante_nome_razao, c.declarante_cpf_cnpj, c.empreendimento_id, e.codigo empreendimento_codigo, 
                    c.empreendimento_denominador, c.municipio_id, c.municipio_texto, c.situacao_id, c.situacao_texto, c.situacao_motivo, c.requerimento, 2 origem, 1 tipo,
                    tcs.situacao_envio situacao_envio_id, lses.texto situacao_envio_texto, tcs.url_recibo, tcs.arquivo, tcs.codigo_imovel  
                    from lst_car_solicitacao_cred c, tab_controle_sicar tcs, lov_situacao_envio_sicar lses, idafcredenciado.tab_empreendimento e  
                    where nvl(tcs.solicitacao_car_esquema, 2) = 2 and c.solicitacao_id = tcs.solicitacao_car(+) and tcs.situacao_envio = lses.id(+) and c.empreendimento_id = e.id
                    and (c.credenciado = :credenciado or c.requerimento in (select r.id from tab_requerimento_cred r, tab_pessoa_cred tp where r.interessado = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or c.requerimento in (select r.id from tab_requerimento_cred r, tab_pessoa tp, tab_requerimento_resp_cred trr
                    where r.id = trr.requerimento and trr.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or c.requerimento in (select r.id
                    from tab_requerimento_cred r, tab_pessoa_cred tp, tab_empreendimento_resp_cred ter where r.empreendimento = ter.empreendimento and ter.responsavel = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj))
                    ) l where 1 = 1" + comandtxt, esquemaBanco);

				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("cpfCnpj", DbType.String, 18, filtros.Dados.AutorCPFCNPJ);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select l.solic_tit_id, l.interno_id, nvl(l.solicitacao_numero, l.titulo_numero) numero, l.titulo_ano ano, l.empreendimento_denominador, l.municipio_texto, 
                    l.situacao_id, l.situacao_texto, l.situacao_motivo, l.credenciado, l.origem, l.tipo,  l.situacao_envio_id, l.situacao_envio_texto, l.url_recibo, l.arquivo, l.empreendimento_codigo from (
                    /*Solicitacao Interno*/
                    select s.id, s.solic_tit_id interno_id, s.solic_tit_id, s.solicitacao_numero, null titulo_numero, null titulo_ano, 
                    s.protocolo_id, s.protocolo_numero, s.protocolo_ano, s.protocolo_numero_completo, null projeto_digital, null 
                    credenciado, s.declarante_id, s.declarante_nome_razao, s.declarante_cpf_cnpj, s.empreendimento_id, e.codigo empreendimento_codigo,
                    s.empreendimento_denominador, s.municipio_id, s.municipio_texto, s.situacao_id, s.situacao_texto, s.situacao_motivo, s.requerimento, 1 origem, 1 tipo,
                    tcs.situacao_envio situacao_envio_id, lses.texto situacao_envio_texto, tcs.url_recibo, tcs.arquivo, tcs.codigo_imovel 
                    from lst_car_solic_tit s, tab_controle_sicar tcs, lov_situacao_envio_sicar lses, tab_empreendimento e where s.tipo = 1 and nvl(tcs.solicitacao_car_esquema, 1) = 1 
                    and s.solic_tit_id = tcs.solicitacao_car(+) and tcs.situacao_envio = lses.id(+) and e.id = s.empreendimento_id
                    and (s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp where r.interessado = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp, tab_requerimento_responsavel trr
                    where r.id = trr.requerimento and trr.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id
                    from tab_requerimento r, tab_pessoa tp, tab_empreendimento_responsavel ter where r.empreendimento = ter.empreendimento and ter.responsavel = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp 
                    where r.autor = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj))
                    union all 
                    /*Solicitacao Titulo*/
                    select s.id, 0 interno_id, s.solic_tit_id, null solicitacao_numero, s.titulo_numero, 
                    s.titulo_ano, s.protocolo_id, s.protocolo_numero, s.protocolo_ano, s.protocolo_numero_completo, null projeto_digital, null credenciado, 
                    s.declarante_id, s.declarante_nome_razao, s.declarante_cpf_cnpj, s.empreendimento_id, s.empreendimento_codigo, s.empreendimento_denominador, 
                    s.municipio_id, s.municipio_texto, null situacao_id, s.situacao_texto, s.situacao_motivo, s.requerimento, 1 origem, 2 tipo,
                    null situacao_envio_id, null situacao_envio_texto, null url_recibo, null arquivo, null codigo_imovel
                    from lst_car_solic_tit s where s.tipo = 2 
                    and (s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp where r.interessado = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp, tab_requerimento_responsavel trr
                    where r.id = trr.requerimento and trr.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id
                    from tab_requerimento r, tab_pessoa tp, tab_empreendimento_responsavel ter where r.empreendimento = ter.empreendimento and ter.responsavel = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or s.requerimento in (select r.id from tab_requerimento r, tab_pessoa tp 
                    where r.autor = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj))
                    union all 
                    /*Solicitacao Credenciado*/
                    select c.id, 0 interno_id, c.solicitacao_id solic_tit_id, c.numero solicitacao_numero, null titulo_numero, 
                    null titulo_ano, null protocolo_id, null protocolo_numero, null protocolo_ano, null protocolo_numero_completo, c.projeto_digital, 
                    c.credenciado, c.declarante_id, c.declarante_nome_razao, c.declarante_cpf_cnpj, c.empreendimento_id, e.codigo empreendimento_codigo, 
                    c.empreendimento_denominador, c.municipio_id, c.municipio_texto, c.situacao_id, c.situacao_texto, c.situacao_motivo, c.requerimento, 2 origem, 1 tipo,
                    tcs.situacao_envio situacao_envio_id, lses.texto situacao_envio_texto, tcs.url_recibo, tcs.arquivo, tcs.codigo_imovel  
                    from lst_car_solicitacao_cred c, tab_controle_sicar tcs, lov_situacao_envio_sicar lses, idafcredenciado.tab_empreendimento e  
                    where nvl(tcs.solicitacao_car_esquema, 2) = 2 and c.solicitacao_id = tcs.solicitacao_car(+) and tcs.situacao_envio = lses.id(+) and c.empreendimento_id = e.id
                    and (c.credenciado = :credenciado or c.requerimento in (select r.id from tab_requerimento_cred r, tab_pessoa_cred tp where r.interessado = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or c.requerimento in (select r.id from tab_requerimento_cred r, tab_pessoa tp, tab_requerimento_resp_cred trr
                    where r.id = trr.requerimento and trr.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or c.requerimento in (select r.id
                    from tab_requerimento_cred r, tab_pessoa_cred tp, tab_empreendimento_resp_cred ter where r.empreendimento = ter.empreendimento and ter.responsavel = tp.id
                    and nvl(tp.cpf, tp.cnpj) = :cpfCnpj))
                    ) l where 1 = 1" + comandtxt;

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
						item.InternoId = reader.GetValue<int>("interno_id");
						item.Numero = reader.GetValue<string>("numero");
						item.Ano = reader.GetValue<string>("ano");
						item.EmpreendimentoDenominador = reader.GetValue<string>("empreendimento_denominador");
						item.EmpreendimentoCodigo = reader.GetValue<Int64>("empreendimento_codigo");
						item.MunicipioTexto = reader.GetValue<string>("municipio_texto");
						item.SituacaoID = reader.GetValue<int>("situacao_id");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.IsTitulo = reader.GetValue<int>("tipo") == 2;
						item.SituacaoMotivo = reader.GetValue<string>("situacao_motivo");
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

		internal List<CARSolicitacao> ObterCARSolicitacoes(CARSolicitacao filtro, BancoDeDados banco = null)
		{
			List<CARSolicitacao> retorno = new List<CARSolicitacao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				comandtxt += comando.FiltroAnd("t.id", "id", filtro.Id);

				comandtxt += comando.FiltroAnd("t.empreendimento", "empreendimento", filtro.Empreendimento.Id);

				comandtxt += comando.FiltroAnd("t.requerimento", "requerimento", filtro.Requerimento.Id);

				comandtxt += comando.FiltroAnd("t.situacao", "situacao", filtro.SituacaoId);

				if (filtro.Empreendimento.Codigo.GetValueOrDefault() > 0)
				{
					comandtxt += comando.FiltroIn("t.empreendimento", "select e.id from tab_empreendimento e where e.codigo = :codigo", "codigo", filtro.Empreendimento.Codigo);
				}

				comando.DbCommand.CommandText = @"select t.id, t.tid, t.situacao from tab_car_solicitacao t where t.id > 0 " + comandtxt;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					CARSolicitacao item;

					while (reader.Read())
					{
						item = new CARSolicitacao();
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.SituacaoId = reader.GetValue<int>("situacao");
						retorno.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}


		public Tuple<eStatusArquivoSICAR, string> BuscaSituacaoAtualArquivoSICAR(int solicitacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				//Busca na Base do institucional
				Comando comando = bancoDeDados.CriarComando(@"select e.situacao_envio, (select s.texto from lov_situacao_envio_sicar s where s.id = e.situacao_envio) situacao_envio_texto from tab_controle_sicar e where e.solicitacao_car = :solicitacaoId and e.solicitacao_car_esquema = 2");

				comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
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

		#endregion

		#region Validação

		internal string EmpreendimentoPossuiSolicitacao(int empreendimentoInternoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ls.texto situacao from tab_car_solicitacao t, tab_empreendimento e, lov_car_solicitacao_situacao ls 
				where t.empreendimento = e.id and t.situacao = ls.id and e.interno = :empreendimentoID and t.situacao in (1, 2, 4) /*Em cadastro, Válido, Suspenso*/", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoInternoID, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

        internal CARSolicitacao EmpreendimentoPossuiSolicitacaoProjetoDigital(int empreendimentoInternoID, BancoDeDados banco = null)
        {
            CARSolicitacao car = new CARSolicitacao();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.situacao, t.projeto_digital from tab_car_solicitacao t, tab_empreendimento e, lov_car_solicitacao_situacao ls 
				where t.empreendimento = e.id and t.situacao = ls.id and e.interno = :empreendimentoID and t.situacao in (1, 2, 4) /*Em cadastro, Válido, Suspenso*/", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoInternoID, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        car.Id = reader.GetValue<Int32>("id");
                        car.SituacaoId = reader.GetValue<Int32>("situacao");
                        car.ProjetoId = reader.GetValue<Int32>("projeto_digital");
                    }
                    reader.Close();
                }

                return car; //Convert.ToString(bancoDeDados.ExecutarScalar(comando));
            }
        }
		internal string EmpreendimentoCredenciadoPossuiSolicitacao(int empreendimentoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ls.texto situacao from tab_car_solicitacao t, lov_car_solicitacao_situacao ls
				where t.situacao = ls.id and t.empreendimento = :empreendimentoID and t.situacao in (1, 2, 4) /*Em cadastro, Válido, Suspenso*/", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}
        
        internal CARSolicitacao EmpreendimentoCredenciadoPossuiSolicitacaoProjetoDigital(int empreendimentoID, BancoDeDados banco = null)
        {
            CARSolicitacao car = new CARSolicitacao();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.situacao, t.projeto_digital from tab_car_solicitacao t, lov_car_solicitacao_situacao ls
				where t.situacao = ls.id and t.empreendimento = :empreendimentoID and t.situacao in (1, 2, 4) /*Em cadastro, Válido, Suspenso*/", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        car.Id = reader.GetValue<Int32>("id");
                        car.SituacaoId = reader.GetValue<Int32>("situacao");
                        car.ProjetoId = reader.GetValue<Int32>("projeto_digital");
                    }
                    reader.Close();
                }

                return car;
            }
        }

		internal string EmpreendimentoPossuiSolicitacao(string cnpj, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ls.texto from tab_empreendimento t, tab_car_solicitacao c, lov_car_solicitacao_situacao ls 
				where t.id = c.empreendimento and c.situacao = ls.id and c.situacao in (2,4) and t.cnpj =:cnpj and rownum = 1", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("cnpj", cnpj, DbType.String);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool AlteradoSituacaoInterno(int solicitacaoCARID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from {0}hst_car_solicitacao h 
				where h.solicitacao_id = :solicitacaoCARID and h.acao_executada = 41/*Alterar situação*/ and h.executor_tipo_id = 1/*Funcionário*/ 
				and h.data_execucao = (select max(h2.data_execucao) from {0}hst_car_solicitacao h2 where h2.solicitacao_id = h.solicitacao_id)", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("solicitacaoCARID", solicitacaoCARID, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		public void FazerVirarPassivo(int solicitacaoID, BancoDeDados banco = null)
		{
			//TODO:Validacao de Solicitacao de Inscricao para Salvar Titulo CAR
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"update tab_controle_sicar s set s.solicitacao_passivo = 1, s.solicitacao_situacao_aprovado = 5 where s.solicitacao_car=:solicitacao_car and s.solicitacao_car_esquema = 2");

				comando.AdicionarParametroEntrada("solicitacao_car", solicitacaoID, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			}
		}
	}
}