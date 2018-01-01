using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class InfracaoDa
	{
		#region Propriedade e Atributos

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		ConfigFiscalizacaoDa _configuracaoDa = new ConfigFiscalizacaoDa();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		public String EsquemaBancoGeo { get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); } }

		#endregion

		public InfracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public Infracao Salvar(Infracao infracao, BancoDeDados banco = null)
		{
			if (infracao == null)
			{
				throw new Exception("Infração do autuado é nulo.");
			}

			if (infracao.Id <= 0)
			{
				infracao = Criar(infracao, banco);
			}
			else
			{
				infracao = Editar(infracao, banco);
			}

			return infracao;
		}

		public Infracao Criar(Infracao infracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Infração

				Comando comando = bancoDeDados.CriarComando(@"
                                    insert into {0}tab_fisc_infracao (id,
                                                                      fiscalizacao,
                                                                      classificacao,
                                                                      tipo,
                                                                      item,
                                                                      subitem,
                                                                      descricao_infracao,
                                                                      configuracao,
                                                                      tid,
                                                                      configuracao_tid,
                                                                      possui_infracao,
                                                                      data_constatacao,
                                                                      hora_constatacao,
                                                                      classificacao_infracao)
                                    values ({0}seq_fisc_infracao.nextval,
                                            :fiscalizacao,
                                            :classificacao,
                                            :tipo,
                                            :item,
                                            :subitem,
                                            :descricao_infracao,
                                            :configuracao,
                                            :tid,
                                            :configuracao_tid,
                                            :possui_infracao,
                                            :data_constatacao,
                                            :hora_constatacao,
                                            :classificacao_infracao)
                                    returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("classificacao", infracao.ClassificacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", infracao.TipoId, DbType.Int32);
				comando.AdicionarParametroEntrada("item", infracao.ItemId, DbType.Int32);
				comando.AdicionarParametroEntrada("subitem", infracao.SubitemId, DbType.Int32);
				comando.AdicionarParametroEntrada("configuracao", infracao.ConfiguracaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("descricao_infracao", infracao.DescricaoInfracao, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("configuracao_tid", DbType.String, 36, infracao.ConfiguracaoTid);
                comando.AdicionarParametroEntrada("possui_infracao", infracao.ComInfracao, DbType.Boolean);
                comando.AdicionarParametroEntrada("data_constatacao", infracao.DataConstatacao.Data, DbType.Date);
                comando.AdicionarParametroEntrada("hora_constatacao", infracao.HoraConstatacao, DbType.String);
                comando.AdicionarParametroEntrada("classificacao_infracao", infracao.ClassificacaoInfracao, DbType.Int32);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				infracao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Campos

				comando = bancoDeDados.CriarComando(@" insert into {0}tab_fisc_infracao_campo (id, infracao, campo, texto, tid) values ({0}seq_fisc_inf_campo.nextval, :infracao, :campo, :texto, :tid) ", EsquemaBanco);

				comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("campo", DbType.Int32);
				comando.AdicionarParametroEntrada("texto", DbType.String);

				foreach (InfracaoCampo campo in infracao.Campos)
				{
					comando.SetarValorParametro("campo", campo.CampoId);
					comando.SetarValorParametro("texto", campo.Texto);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Questionário

				comando = bancoDeDados.CriarComando(@" insert into {0}tab_fisc_infracao_pergunta (id, infracao, pergunta, pergunta_tid, resposta, resposta_tid, especificacao, tid) values 
					({0}seq_fisc_inf_pergunta.nextval, :infracao, :pergunta, :pergunta_tid, :resposta, :resposta_tid, :especificacao, :tid) ", EsquemaBanco);

				comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("pergunta", DbType.Int32);
				comando.AdicionarParametroEntrada("pergunta_tid", DbType.String, 36);
				comando.AdicionarParametroEntrada("resposta", DbType.Int32);
				comando.AdicionarParametroEntrada("resposta_tid", DbType.String, 36);
				comando.AdicionarParametroEntrada("especificacao", DbType.String);

				foreach (InfracaoPergunta questionario in infracao.Perguntas)
				{
					comando.SetarValorParametro("pergunta", questionario.PerguntaId);
					comando.SetarValorParametro("pergunta_tid", questionario.PerguntaTid);
					comando.SetarValorParametro("resposta", questionario.RespostaId);
					comando.SetarValorParametro("resposta_tid", questionario.RespostaTid);
					comando.SetarValorParametro("especificacao", questionario.Especificacao);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

                #region Enquadramento

                infracao.EnquadramentoInfracao.FiscalizacaoId = infracao.FiscalizacaoId;

                EnquadramentoDa _enquadramentoDA = new EnquadramentoDa();

                infracao.EnquadramentoInfracao = _enquadramentoDA.Salvar(infracao.EnquadramentoInfracao, banco);

                #endregion

                #region Penalidades Fixas

                if (infracao.PossuiAdvertencia == true)
                {
                    comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Advertência%'),
                                        :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                if (infracao.PossuiMulta == true)
                {
                    comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Multa%'),
                                        :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                if (infracao.PossuiApreensao == true)
                {
                    comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Apreensão%'),
                                        :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                if (infracao.PossuiInterdicaoEmbargo == true)
                {
                    comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Interdição%'),
                                        :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion

                #region Outras Penalidades

                foreach (var id_penalidade in infracao.IdsOutrasPenalidades)
                {
                    if (id_penalidade > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_outras_penalidad_infr (id,
                                                                               infracao,
                                                                               penalidade_outra,
                                                                               tid)
                                values ({0}seq_fisc_outras_penalidad_infr.nextval,
                                        :id_infracao,
                                        :id_penalidade,
                                        :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("id_penalidade", id_penalidade, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                #endregion

                Historico.Gerar(infracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(infracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}

			return infracao;
		}

		public Infracao Editar(Infracao infracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

                #region infração

                Comando comando = bancoDeDados.CriarComando(@"
                                    update {0}tab_fisc_infracao t
                                    set fiscalizacao = :fiscalizacao,               
                                        classificacao = :classificacao,
                                        tipo = :tipo,
                                        item = :item,
                                        subitem = :subitem,
                                        descricao_infracao = :descricao_infracao,
                                        configuracao = :configuracao,
                                        tid = :tid,
                                        configuracao_tid = :configuracao_tid,
                                        possui_infracao = :possui_infracao,
                                        data_constatacao = :data_constatacao,
                                        hora_constatacao = :hora_constatacao,
                                        classificacao_infracao = :classificacao_infracao
                                    where t.id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", infracao.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("classificacao", infracao.ClassificacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("tipo", infracao.TipoId, DbType.Int32);
                comando.AdicionarParametroEntrada("item", infracao.ItemId, DbType.Int32);
                comando.AdicionarParametroEntrada("subitem", infracao.SubitemId, DbType.Int32);
                comando.AdicionarParametroEntrada("configuracao", infracao.ConfiguracaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("descricao_infracao", (infracao.ComInfracao == true ? infracao.DescricaoInfracao : null), DbType.String);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("configuracao_tid", DbType.String, 36, infracao.ConfiguracaoTid);
                comando.AdicionarParametroEntrada("possui_infracao", infracao.ComInfracao, DbType.Boolean);
                comando.AdicionarParametroEntrada("data_constatacao", infracao.DataConstatacao.Data, DbType.Date);
                comando.AdicionarParametroEntrada("hora_constatacao", infracao.HoraConstatacao, DbType.String);
                comando.AdicionarParametroEntrada("classificacao_infracao", (infracao.ComInfracao == true ? infracao.ClassificacaoInfracao : null), DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

                #endregion infração

                #region Campos

                comando = bancoDeDados.CriarComando(@"delete from {0}tab_fisc_infracao_campo c where c.infracao = :infracao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, infracao.Campos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (InfracaoCampo campo in infracao.Campos)
				{
					if (campo.Id == 0)
					{
						comando = bancoDeDados.CriarComando(@" insert into {0}tab_fisc_infracao_campo (id, infracao, campo, texto, tid) values ({0}seq_fisc_inf_campo.nextval, :infracao, :campo, :texto, 
							:tid) returning id into :id", EsquemaBanco);
						comando.AdicionarParametroSaida("id", DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@" update {0}tab_fisc_infracao_campo set infracao = :infracao, campo = :campo, texto = :texto, tid = :tid where id = :id ", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", campo.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("campo", campo.CampoId, DbType.Int32);
					comando.AdicionarParametroEntrada("texto", campo.Texto, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					if (campo.Id == 0)
					{
						campo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}
				}

				#endregion

				#region Questionário

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_fisc_infracao_pergunta c where c.infracao = :infracao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, infracao.Perguntas.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (InfracaoPergunta questionario in infracao.Perguntas)
				{
					if (questionario.Id == 0)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}tab_fisc_infracao_pergunta (id, infracao, pergunta, pergunta_tid, 
															resposta, resposta_tid, especificacao, tid) values ({0}seq_fisc_inf_pergunta.nextval, 
															:infracao, :pergunta, :pergunta_tid, :resposta, :resposta_tid, :especificacao, :tid) 
															returning id into :id", EsquemaBanco);
						comando.AdicionarParametroSaida("id", DbType.Int32);

					}
					else
					{
						comando = bancoDeDados.CriarComando(@"update {0}tab_fisc_infracao_pergunta set infracao = :infracao, pergunta = :pergunta, 
															pergunta_tid = :pergunta_tid, resposta = :resposta, resposta_tid = :resposta_tid, 
															especificacao = :especificacao, tid = :tid where id = :id", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", questionario.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("pergunta", questionario.PerguntaId, DbType.Int32);
					comando.AdicionarParametroEntrada("pergunta_tid", DbType.String, 36, questionario.PerguntaTid);
					comando.AdicionarParametroEntrada("resposta", questionario.RespostaId, DbType.Int32);
					comando.AdicionarParametroEntrada("resposta_tid", DbType.String, 36, questionario.RespostaTid);
					comando.AdicionarParametroEntrada("especificacao", questionario.Especificacao, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					if (questionario.Id == 0)
					{
						questionario.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}
				}

				#endregion

                if (infracao.ComInfracao == true)
                {
                    #region Enquadramento

                    infracao.EnquadramentoInfracao.FiscalizacaoId = infracao.FiscalizacaoId;

                    EnquadramentoDa _enquadramentoDA = new EnquadramentoDa();

                    infracao.EnquadramentoInfracao = _enquadramentoDA.Salvar(infracao.EnquadramentoInfracao, banco);

                    #endregion

                    #region Penalidades Fixas

                    comando = bancoDeDados.CriarComando(@"
                                delete from {0}tab_fisc_penalidades_infr
                                where infracao = :id_infracao", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);

                    bancoDeDados.ExecutarNonQuery(comando);

                    if (infracao.PossuiAdvertencia == true)
                    {
                        comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Advertência%'),
                                        :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                    else if (infracao.IdsOutrasPenalidades.Count(i => i > 0) == 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_outras_penalidades
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);

                        bancoDeDados.ExecutarNonQuery(comando);
                    }

                    if (infracao.PossuiMulta == true)
                    {
                        comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Multa%'),
                                        :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_multa
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);

                        bancoDeDados.ExecutarNonQuery(comando);
                    }

                    if (infracao.PossuiApreensao == true)
                    {
                        comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Apreensão%'),
                                        :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
                                    select id
                                    from {0}tab_fisc_apreensao 
                                    where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);

                        int apreensao = 0;
                        using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                        {
                            if (reader.Read())
                            {
                                apreensao = reader.GetValue<int>("id");
                            }

                            reader.Close();
                        }
                        if (apreensao > 0)
                        {
                            comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_apreensao_produto
                                        where apreensao = :id_apreensao", EsquemaBanco);
                            comando.AdicionarParametroEntrada("id_apreensao", apreensao, DbType.Int32);
                            bancoDeDados.ExecutarNonQuery(comando);

                            comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_apreensao
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                            comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);
                            bancoDeDados.ExecutarNonQuery(comando);
                        }
                    }

                    if (infracao.PossuiInterdicaoEmbargo == true)
                    {
                        comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_penalidades_infr (id,
                                                                          infracao,
                                                                          penalidade,
                                                                          tid)
                                values ({0}seq_fisc_penalidades_infr.nextval,
                                        :id_infracao,
                                        (select id from lov_fisc_penalidades_fixas where texto like '%Interdição%'),
                                        :tid)", EsquemaBanco);

                        comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_obj_infracao
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);

                        bancoDeDados.ExecutarNonQuery(comando);
                    }

                    #endregion

                    #region Outras Penalidades

                    comando = bancoDeDados.CriarComando(@"
                            delete from tab_fisc_outras_penalidad_infr
                            where infracao = :id_infracao", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);

                    foreach (var id_penalidade in infracao.IdsOutrasPenalidades)
                    {
                        if (id_penalidade > 0)
                        {
                            comando = bancoDeDados.CriarComando(@"
                                insert into {0}tab_fisc_outras_penalidad_infr (id,
                                                                               infracao,
                                                                               penalidade_outra,
                                                                               tid)
                                values ({0}seq_fisc_outras_penalidad_infr.nextval,
                                        :id_infracao,
                                        :id_penalidade,
                                        :tid)", EsquemaBanco);

                            comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                            comando.AdicionarParametroEntrada("id_penalidade", id_penalidade, DbType.Int32);
                            comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                            bancoDeDados.ExecutarNonQuery(comando);
                        }
                    }

                    #endregion
                }
                else if (infracao.ComInfracao == false)
                {
                    #region Enquadramento
                    EnquadramentoDa _enquadramentoDA = new EnquadramentoDa();

                    _enquadramentoDA.Excluir(infracao.FiscalizacaoId, banco);

                    #endregion

                    #region Penalidades Fixas

                    comando = bancoDeDados.CriarComando(@"
                                delete from {0}tab_fisc_penalidades_infr
                                where infracao = :id_infracao", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);

                    comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_outras_penalidades
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);

                    comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_multa
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);

                    comando = bancoDeDados.CriarComando(@"
                                    select id
                                    from {0}tab_fisc_apreensao 
                                    where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);

                    int apreensao = 0;
                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        if (reader.Read())
                        {
                            apreensao = reader.GetValue<int>("id");
                        }

                        reader.Close();
                    }
                    if (apreensao > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_apreensao_produto
                                        where apreensao = :id_apreensao", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id_apreensao", apreensao, DbType.Int32);
                        bancoDeDados.ExecutarNonQuery(comando);

                        comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_apreensao
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                        comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);
                        bancoDeDados.ExecutarNonQuery(comando);
                    }

                    comando = bancoDeDados.CriarComando(@"
                                        delete
                                        from {0}tab_fisc_obj_infracao
                                        where fiscalizacao = :id_fiscalizacao", EsquemaBanco);
                    comando.AdicionarParametroEntrada("id_fiscalizacao", infracao.FiscalizacaoId, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);

                    #endregion

                    #region Outras Penalidades

                    comando = bancoDeDados.CriarComando(@"
                            delete from tab_fisc_outras_penalidad_infr
                            where infracao = :id_infracao", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);
                    bancoDeDados.ExecutarNonQuery(comando);

                    #endregion
                }

				Historico.Gerar(infracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(infracao.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}
			return infracao;
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

                #region Enquadramento

                EnquadramentoDa _enquadramentoDA = new EnquadramentoDa();

                _enquadramentoDA.Excluir(fiscalizacaoId, banco);

                #endregion

				Comando comando = bancoDeDados.CriarComando("begin "
															+ "delete {0}tab_fisc_infracao_campo t where t.infracao = (select id from {0}tab_fisc_infracao where fiscalizacao = :fiscalizacao); "
															+ "delete {0}tab_fisc_infracao_pergunta t where t.infracao = (select id from {0}tab_fisc_infracao where fiscalizacao = :fiscalizacao); "
                                                            + "delete {0}tab_fisc_penalidades_infr t where t.infracao = (select id from {0}tab_fisc_infracao where fiscalizacao = :fiscalizacao); "
                                                            + "delete {0}tab_fisc_outras_penalidad_infr t where t.infracao = (select id from {0}tab_fisc_infracao where fiscalizacao = :fiscalizacao); "
															+ "delete {0}tab_fisc_infracao t where t.fiscalizacao = :fiscalizacao; "
														+ "end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public Infracao Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			Infracao infracao = new Infracao();
			InfracaoPergunta questionario = new InfracaoPergunta();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Infração

				Comando comando = bancoDeDados.CriarComando(@"select tfi.id, tfi.classificacao, lc.texto classificacao_texto, tfi.tipo, f.situacao situacao_id,
															lt.texto tipo_texto, tfi.item, cfi.texto item_texto, tfi.subitem, cfs.texto subitem_texto, tfi.infracao_autuada,
															tfi.gerado_sistema, tfi.valor_multa, tfi.codigo_receita, tfi.numero_auto_infracao_bloco, tfi.descricao_infracao,
															tfi.data_lavratura_auto, tfi.serie, tfi.configuracao, tfi.configuracao_tid,
                                                            tfi.possui_infracao, tfi.data_constatacao, tfi.hora_constatacao, tfi.classificacao_infracao
															from {0}tab_fisc_infracao tfi, {0}tab_fiscalizacao f, {0}tab_arquivo a, {0}lov_cnf_fisc_infracao_classif lc,
															{0}cnf_fisc_infracao_tipo lt, {0}cnf_fisc_infracao_item cfi, {0}cnf_fisc_infracao_subitem cfs where 
															tfi.arquivo = a.id(+) and tfi.classificacao = lc.id(+) and tfi.tipo = lt.id(+) and tfi.item = cfi.id(+)
															and tfi.subitem = cfs.id(+) and tfi.fiscalizacao = :fiscalizacao and f.id = tfi.fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						infracao = new Infracao
						{
							Id = reader.GetValue<int>("id"),
							ClassificacaoId = reader.GetValue<int>("classificacao"),
							ClassificacaoTexto = reader.GetValue<string>("classificacao_texto"),							
							TipoId = reader.GetValue<int>("tipo"),
							TipoTexto = reader.GetValue<string>("tipo_texto"),							
							ItemId = reader.GetValue<int>("item"),
							ItemTexto = reader.GetValue<string>("item_texto"),							
							SubitemId = reader.GetValue<int>("subitem"),
							SubitemTexto = reader.GetValue<string>("subitem_texto"),							
							SerieId = reader.GetValue<int>("serie"),
							ConfiguracaoId = reader.GetValue<int>("configuracao"),
							IsAutuada = reader.GetValue<bool>("infracao_autuada"),
							IsGeradaSistema = reader.GetValue<bool?>("gerado_sistema"),
							ValorMulta = reader.GetValue<string>("valor_multa"),
							CodigoReceitaId = reader.GetValue<int>("codigo_receita"),
							NumeroAutoInfracaoBloco = reader.GetValue<string>("numero_auto_infracao_bloco"),
							DescricaoInfracao = reader.GetValue<string>("descricao_infracao"),
							ConfiguracaoTid = reader.GetValue<string>("configuracao_tid"),
							FiscalizacaoSituacaoId = reader.GetValue<int>("situacao_id"),
                            ComInfracao = reader.GetValue<bool>("possui_infracao"),
                            HoraConstatacao = reader.GetValue<string>("hora_constatacao"),
                            ClassificacaoInfracao = reader.GetValue<int>("classificacao_infracao")
						};

                        infracao.DataConstatacao.Data = reader.GetValue<DateTime>("data_constatacao");

						if (!string.IsNullOrWhiteSpace(reader.GetValue<string>("data_lavratura_auto")))
						{
							infracao.DataLavraturaAuto.DataTexto = reader.GetValue<string>("data_lavratura_auto");
						}
					}
					reader.Close();
				}

				#endregion

				#region Campos

				comando = bancoDeDados.CriarComando(@"
					select tfic.id      Id,
						   tfic.campo   CampoId,
						   tfic.texto   Texto,
						   cfic.texto   CampoIdentificacao,
						   cfic.unidade CampoUnidade,
						   lu.texto     CampoUnidadeTexto,
						   cfic.Tipo    CampoTipo,
						   lt.texto     CampoTipoTexto
					  from {0}tab_fisc_infracao_campo        tfic,
						   {0}lov_cnf_fisc_infracao_camp_tip lt,
						   {0}lov_cnf_fisc_infracao_camp_uni lu,
						   {0}cnf_fisc_infracao_campo        cfic
					 where tfic.campo = cfic.id
					   and cfic.tipo = lt.id(+)
					   and cfic.unidade = lu.id(+)
					   and tfic.infracao = :infracao", EsquemaBanco);

				comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					infracao.Campos = new List<InfracaoCampo>();
					while (reader.Read())
					{
						infracao.Campos.Add(new InfracaoCampo
						{
							Id = reader.GetValue<int>("Id"),
							CampoId = reader.GetValue<int>("CampoId"),
							Identificacao = reader.GetValue<string>("CampoIdentificacao"),
							Tipo = reader.GetValue<int>("CampoTipo"),
							TipoTexto = reader.GetValue<string>("CampoTipoTexto"),
							Unidade = reader.GetValue<int>("CampoUnidade"),
							UnidadeTexto = reader.GetValue<string>("CampoUnidadeTexto"),
							Texto = reader.GetValue<string>("Texto")
						});
					}
					reader.Close();
				}

				#endregion

				#region Questionário

				comando = bancoDeDados.CriarComando(@"select tfiq.id Id,
													tfiq.pergunta      PerguntaId,
													tfiq.pergunta_tid  PerguntaTid,
													cfip.texto         PerguntaIdentificacao,
													tfiq.resposta      RespostaId,
													tfiq.resposta_tid  RespostaTid,
													tfiq.Especificacao
													from {0}tab_fisc_infracao_pergunta tfiq,
													{0}cnf_fisc_infracao_pergunta cfip
													where tfiq.pergunta = cfip.id
													and tfiq.infracao = :infracao order by tfiq.pergunta", EsquemaBanco);

				comando.AdicionarParametroEntrada("infracao", infracao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					infracao.Perguntas = new List<InfracaoPergunta>();
					while (reader.Read())
					{
						infracao.Perguntas.Add(new InfracaoPergunta
						{
							Id = reader.GetValue<int>("Id"),
							PerguntaId = reader.GetValue<int>("PerguntaId"),
							PerguntaTid = reader.GetValue<string>("PerguntaTid"),
							RespostaId = reader.GetValue<int>("RespostaId"),
							RespostaTid = reader.GetValue<string>("RespostaTid"),
							Identificacao = reader.GetValue<string>("PerguntaIdentificacao"),
							Especificacao = reader.GetValue<string>("Especificacao"),
							Respostas = _configuracaoDa.ObterRespostas(reader.GetValue<int>("PerguntaId"))
						});
					}
					reader.Close();
				}

				#endregion

                #region Enquadramento

                EnquadramentoDa _enquadramentoDA = new EnquadramentoDa();

                infracao.EnquadramentoInfracao = _enquadramentoDA.Obter(fiscalizacaoId, banco);

                #endregion

                #region Penalidades Fixas

                infracao.PossuiAdvertencia = infracao.PossuiMulta = infracao.PossuiApreensao = infracao.PossuiInterdicaoEmbargo = false;

                comando = bancoDeDados.CriarComando(@"
                            select (case when lfpf.texto like '%Advertência%' then 1 else 0 end) advertencia,
                                   (case when lfpf.texto like '%Multa%' then 1 else 0 end) multa,
                                   (case when lfpf.texto like '%Apreensão%' then 1 else 0 end) apreensao,
                                   (case when lfpf.texto like '%Interdição%' then 1 else 0 end) interdicao
                            from tab_fisc_penalidades_infr tfpi,
                                 lov_fisc_penalidades_fixas lfpf
                            where tfpi.penalidade = lfpf.id
                                  and tfpi.infracao = :id_infracao", EsquemaBanco);
                
                comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        var advertencia = reader.GetValue<bool>("advertencia");
                        var multa = reader.GetValue<bool>("multa");
                        var apreensao = reader.GetValue<bool>("apreensao");
                        var interdicao = reader.GetValue<bool>("interdicao");
                        
                        if (advertencia) infracao.PossuiAdvertencia = true;
                        if (multa) infracao.PossuiMulta = true;
                        if (apreensao) infracao.PossuiApreensao = true;
                        if (interdicao) infracao.PossuiInterdicaoEmbargo = true;
                    }
                }

                #endregion

                #region Outras Penalidades

                infracao.IdsOutrasPenalidades = new List<int>();

                comando = bancoDeDados.CriarComando(@"
                            select penalidade_outra idPenalidade
                            from tab_fisc_outras_penalidad_infr
                            where infracao = :id_infracao", EsquemaBanco);

                comando.AdicionarParametroEntrada("id_infracao", infracao.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        infracao.IdsOutrasPenalidades.Add(reader.GetValue<int>("idPenalidade"));
                    }
                }

                #endregion
			}

			return infracao;
		}

		public Infracao ObterHistoricoPorFiscalizacao(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select f.id from tab_fisc_infracao f where f.fiscalizacao = :fiscalizacaoId");
				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				int infracaoId = 0;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						infracaoId = reader.GetValue<int>("id");
					}
					reader.Close();
				}

				return ObterHistorico(infracaoId, bancoDeDados);
			}
		}

		public Infracao ObterHistorico(int id, BancoDeDados banco = null)
		{
			Infracao infracao = new Infracao();
			InfracaoPergunta questionario = new InfracaoPergunta();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Infração

				Comando comando = bancoDeDados.CriarComando(@"select tfi.infracao_id id, f.situacao_id, tfi.classificacao_id classificacao, tfi.classificacao_texto,
															tfi.tipo_id tipo, lt.texto tipo_texto, tfi.item_id item, cfi.texto item_texto, tfi.subitem_id subitem,
															cfs.texto subitem_texto, tfi.infracao_autuada, tfi.gerado_sistema, tfi.valor_multa, tfi.codigo_receita_id codigo_receita,
															tfi.numero_auto_infracao_bloco, tfi.descricao_infracao, tfi.data_lavratura_auto, tfi.serie_id serie, tfi.configuracao_id configuracao,
															tfi.arquivo_id arquivo, a.nome arquivo_nome, tfi.configuracao_tid from hst_fisc_infracao tfi, hst_fiscalizacao f,
															tab_arquivo a, lov_cnf_fisc_infracao_classif lc, cnf_fisc_infracao_tipo lt, cnf_fisc_infracao_item cfi,
															cnf_fisc_infracao_subitem cfs where tfi.arquivo_id = a.id(+) and tfi.classificacao_id = lc.id(+) and tfi.tipo_id = lt.id(+)
															and tfi.item_id = cfi.id(+) and tfi.subitem_id = cfs.id(+) and tfi.fiscalizacao_id_hst = f.id 
															and tfi.id = (select max(t.id) id from hst_fisc_infracao t where t.infracao_id = :id)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						infracao = new Infracao
						{
							Id = reader.GetValue<int>("id"),
							ClassificacaoId = reader.GetValue<int>("classificacao"),
							ClassificacaoTexto = reader.GetValue<string>("classificacao_texto"),
							TipoId = reader.GetValue<int>("tipo"),
							TipoTexto = reader.GetValue<string>("tipo_texto"),
							ItemId = reader.GetValue<int>("item"),
							ItemTexto = reader.GetValue<string>("item_texto"),
							SubitemId = reader.GetValue<int>("subitem"),
							SubitemTexto = reader.GetValue<string>("subitem_texto"),
							SerieId = reader.GetValue<int>("serie"),
							ConfiguracaoId = reader.GetValue<int>("configuracao"),
							IsAutuada = reader.GetValue<bool>("infracao_autuada"),
							IsGeradaSistema = reader.GetValue<bool?>("gerado_sistema"),
							ValorMulta = reader.GetValue<string>("valor_multa"),
							CodigoReceitaId = reader.GetValue<int>("codigo_receita"),
							NumeroAutoInfracaoBloco = reader.GetValue<string>("numero_auto_infracao_bloco"),
							DescricaoInfracao = reader.GetValue<string>("descricao_infracao"),
							ConfiguracaoTid = reader.GetValue<string>("configuracao_tid"),
							FiscalizacaoSituacaoId = reader.GetValue<int>("situacao_id")
						};

						infracao.Arquivo = new Arquivo
						{
							Id = reader.GetValue<int>("arquivo"),
							Nome = reader.GetValue<string>("arquivo_nome")
						};

						if (!string.IsNullOrWhiteSpace(reader.GetValue<string>("data_lavratura_auto")))
						{
							infracao.DataLavraturaAuto.DataTexto = reader.GetValue<string>("data_lavratura_auto");
						}
					}
					reader.Close();
				}

				#endregion

				#region Campos

				comando = bancoDeDados.CriarComando(@"
					select tfic.infracao_campo_id Id,
						   tfic.campo_id          CampoId,
						   tfic.texto             Texto,
						   cfic.texto             CampoIdentificacao,
						   cfic.unidade           CampoUnidade,
						   lu.texto               CampoUnidadeTexto,
						   cfic.Tipo              CampoTipo,
						   lt.texto               CampoTipoTexto
					  from {0}hst_fisc_infracao_campo        tfic,
						   {0}lov_cnf_fisc_infracao_camp_tip lt,
						   {0}lov_cnf_fisc_infracao_camp_uni lu,
						   {0}cnf_fisc_infracao_campo        cfic
					 where tfic.campo_id = cfic.id
					   and cfic.tipo = lt.id(+)
					   and cfic.unidade = lu.id(+)
					   and tfic.infracao_id_hst = (select max(t.id) id from {0}hst_fisc_infracao t where t.infracao_id = :id)", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					infracao.Campos = new List<InfracaoCampo>();
					while (reader.Read())
					{
						infracao.Campos.Add(new InfracaoCampo
						{
							Id = reader.GetValue<int>("Id"),
							CampoId = reader.GetValue<int>("CampoId"),
							Tipo = reader.GetValue<int>("CampoTipo"),
							Identificacao = reader.GetValue<string>("CampoIdentificacao"),
							TipoTexto = reader.GetValue<string>("CampoTipoTexto"),
							Unidade = reader.GetValue<int>("CampoUnidade"),
							UnidadeTexto = reader.GetValue<string>("CampoUnidadeTexto"),
							Texto = reader.GetValue<string>("Texto")
						});
					}
					reader.Close();
				}

				#endregion

				#region Questionário

				comando = bancoDeDados.CriarComando(@"
					select tfiq.id            Id,
						   tfiq.pergunta_id   PerguntaId,
						   tfiq.pergunta_tid  PerguntaTid,
						   hp.texto           PerguntaIdentificacao,
						   tfiq.resposta_id   RespostaId,
						   tfiq.especificacao
					  from {0}hst_fisc_infracao_pergunta tfiq, 
						   {0}cnf_fisc_infracao_pergunta cfip,
						   {0}hst_cnf_fisc_infracao_pergunta hp
					 where tfiq.pergunta_id = cfip.id
					   and hp.tid = tfiq.pergunta_tid
					   and hp.pergunta_id = cfip.id
					   and tfiq.infracao_id_hst = (select max(t.id) id from {0}hst_fisc_infracao t where t.infracao_id = :id) order by tfiq.pergunta_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					infracao.Perguntas = new List<InfracaoPergunta>();
					while (reader.Read())
					{
						int perguntaId = reader.GetValue<int>("PerguntaId");
						string perguntaTid = reader.GetValue<string>("PerguntaTid");

						infracao.Perguntas.Add(new InfracaoPergunta
						{
							Id = reader.GetValue<int>("Id"),
							PerguntaId = reader.GetValue<int>("PerguntaId"),
							PerguntaTid = perguntaTid,
							RespostaId = reader.GetValue<int>("RespostaId"),
							Identificacao = reader.GetValue<string>("PerguntaIdentificacao"),
							Especificacao = reader.GetValue<string>("Especificacao"),
							Respostas = _configuracaoDa.ObterRespostasHistorico(perguntaId, perguntaTid)
						});
					}
					reader.Close();
				}

				#endregion
			}

			return infracao;
		}

		public Infracao ObterConfig(int configuracaoId, BancoDeDados banco = null)
		{
			Infracao infracao = new Infracao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Infração

				Comando comando = bancoDeDados.CriarComando(@"
					select t.id            ConfiguracaoId,
						   t.classificacao ClassificacaoId,
						   l.texto         ClassificacaoTexto,
						   t.tipo          TipoId,
						   cit.texto       TipoTexto,
						   t.item          ItemId,
						   cii.texto       ItemTexto,
						   t.tid           ConfiguracaoTid
					  from {0}cnf_fisc_infracao             t,
						   {0}cnf_fisc_infracao_tipo        cit,
						   {0}cnf_fisc_infracao_item        cii,
						   {0}lov_cnf_fisc_infracao_classif l
					 where t.tipo = cit.id(+)
					   and t.item = cii.id(+)
					   and t.classificacao = l.id(+)
					   and t.id = :configuracaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);

				infracao = bancoDeDados.ObterEntity<Infracao>(comando);

				#endregion

				#region Campos

				comando = bancoDeDados.CriarComando(@"
					select tfic.campo CampoId,
						   cfic.texto Identificacao,
						   lt.texto   TipoTexto,
						   lt.id      Tipo,
						   lu.id      Unidade,
						   lu.texto   UnidadeTexto,
						   ''         Texto
					  from {0}cnf_fisc_infr_cnf_campo       tfic,
						   {0}lov_cnf_fisc_infracao_camp_tip lt,
						   {0}lov_cnf_fisc_infracao_camp_uni lu,
						   {0}cnf_fisc_infracao_campo        cfic
					 where tfic.campo = cfic.id
					   and cfic.tipo = lt.id(+)
					   and cfic.unidade = lu.id(+)
					   and tfic.configuracao = :configuracaoId", EsquemaBanco);
				comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);

				infracao.Campos = bancoDeDados.ObterEntityList<InfracaoCampo>(comando);

				#endregion

				#region Questionário

				comando = bancoDeDados.CriarComando(@"
					 select tfiq.pergunta PerguntaId,
							cfip.tid      PerguntaTid,
							cfip.texto    Identificacao,
							0             RespostaId,
							''            Especificacao
					   from {0}cnf_fisc_infr_cnf_pergunta tfiq,
							{0}cnf_fisc_infracao_pergunta cfip
					  where tfiq.pergunta = cfip.id
						and tfiq.configuracao = :configuracaoId order by tfiq.pergunta", EsquemaBanco);

				comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);

				infracao.Perguntas = bancoDeDados.ObterEntityList<InfracaoPergunta>(comando, (IDataReader reader, InfracaoPergunta item) => 
				{ 
					item.Respostas = _configuracaoDa.ObterRespostas(item.PerguntaId);
				});

				#endregion
			}

			return infracao;
		}

        public bool PossuiIUFBloco(int fiscalizacaoId, BancoDeDados banco = null)
        {
            bool retorno = false;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
                Comando comando = bancoDeDados.CriarComando(@"
                                    select count(1) valor
                                    from tab_fisc_apreensao tfa,
                                         tab_fisc_multa tfm,
                                         tab_fisc_obj_infracao tfoi,
                                         tab_fisc_outras_penalidades tfop
                                    where (tfa.iuf_digital = 0 and tfa.fiscalizacao = :fiscalizacao)
                                          or (tfm.iuf_digital = 0 and tfm.fiscalizacao = :fiscalizacao)
                                          or (tfoi.iuf_digital = 0 and tfoi.fiscalizacao = :fiscalizacao)
                                          or (tfop.iuf_digital = 0 and tfop.fiscalizacao = :fiscalizacao)", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        int existe = reader.GetValue<int>("valor");

                        retorno = existe > 0;
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_infracao t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

		#endregion

		#region Validação

		internal bool ConfigAlterada(int configuracaoId, string tid, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) count from cnf_fisc_infracao t where t.id = :configuracaoId and t.tid <> :tid", EsquemaBanco);
				comando.AdicionarParametroEntrada("configuracaoId", configuracaoId, DbType.Int32);				
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);
				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool PerguntaRespostaAlterada(int infracaoId, BancoDeDados banco = null)
		{
			Boolean alterou = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select i.pergunta, i.resposta, i.pergunta_tid, i.resposta_tid 
															from tab_fisc_infracao_pergunta i where i.infracao = :infracao", EsquemaBanco);

				comando.AdicionarParametroEntrada("infracao", infracaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Int32 perguntaId = reader.GetValue<Int32>("pergunta");
						String perguntaTid = reader.GetValue<String>("pergunta_tid");

						Int32 respostaId = reader.GetValue<Int32>("resposta");
						String respostaTid = reader.GetValue<String>("resposta_tid");


						#region Pergunta

						comando = bancoDeDados.CriarComando(@"select p.tid pergunta_tid 
															from cnf_fisc_infracao_pergunta p 
															where p.id = :pergunta", EsquemaBanco);

						comando.AdicionarParametroEntrada("pergunta", perguntaId, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read()) 
							{
								String perguntaNovaTid = readerAux.GetValue<String>("pergunta_tid");
								alterou = perguntaNovaTid != perguntaTid;
								
								if (alterou) 
								{
									return true;
								}
							}

							readerAux.Close();
						}

						#endregion

						#region Resposta

						comando = bancoDeDados.CriarComando(@"select p.tid resposta_tid from cnf_fisc_infracao_resposta p 
															where p.id = :resposta", EsquemaBanco);

						comando.AdicionarParametroEntrada("resposta", respostaId, DbType.Int32);

						using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
						{
							if (readerAux2.Read())
							{
								String respostaNovaTid = readerAux2.GetValue<String>("resposta_tid");
								alterou = respostaNovaTid != respostaTid;

								if (alterou)
								{
									return true;
								}
							}

							readerAux2.Close();
						}

						#endregion
					}

					reader.Close();
				}

				return alterou;
			}
		}

		internal bool PerguntaRespostaAlterada(Infracao infracao, BancoDeDados banco = null)
		{
			Boolean alterou = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				if (infracao.Perguntas == null || infracao.Perguntas.Count == 0)
				{
					return alterou;
				}

				foreach (var pergunta in infracao.Perguntas)
				{
					#region Pergunta

					comando = bancoDeDados.CriarComando(@"select p.tid pergunta_tid 
															from cnf_fisc_infracao_pergunta p 
															where p.id = :pergunta", EsquemaBanco);

					comando.AdicionarParametroEntrada("pergunta", pergunta.PerguntaId, DbType.Int32);

					using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
					{
						while (readerAux.Read())
						{
							String perguntaNovaTid = readerAux.GetValue<String>("pergunta_tid");
							alterou = perguntaNovaTid != pergunta.PerguntaTid;

							if (alterou)
							{
								return true;
							}
						}

						readerAux.Close();
					}

					#endregion


					#region Resposta

					comando = bancoDeDados.CriarComando(@"select p.tid resposta_tid from cnf_fisc_infracao_resposta p 
															where p.id = :resposta", EsquemaBanco);

					comando.AdicionarParametroEntrada("resposta", pergunta.RespostaId, DbType.Int32);

					using (IDataReader readerAux2 = bancoDeDados.ExecutarReader(comando))
					{
						if (readerAux2.Read())
						{
							String respostaNovaTid = readerAux2.GetValue<String>("resposta_tid");
							alterou = respostaNovaTid != pergunta.RespostaTid;

							if (alterou)
							{
								return true;
							}
						}

						readerAux2.Close();
					}

					#endregion

				}

				return alterou;
			}
		}

		#endregion
	}
}