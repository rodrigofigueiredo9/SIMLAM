using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloBarragem;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloBarragem.Data
{
	public class BarragemDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		internal Historico Historico { get { return _historico; } }

		private String EsquemaBanco { get; set; }

		#endregion

		public BarragemDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal int Salvar(Barragem barragem, BancoDeDados banco)
		{
			if (barragem == null)
			{
				throw new Exception("A Caracterização de Barragem é nula.");
			}

			if (barragem.Id <= 0)
			{
				return Criar(barragem, banco);
			}
			else
			{
				Editar(barragem, banco);
				return barragem.Id;
			}
		}

		internal int Criar(Barragem barragem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Barragem

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_barragem c
					  (id, 
					   empreendimento, 
					   atividade, 
					   tid)
					values
					  ({0}seq_crt_barragem.nextval, 
					   :empreendimento, 
					   :atividade, 
					   :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", barragem.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", barragem.AtividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				barragem.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Barragem - Barragens

				foreach (var itemBarragem in barragem.Barragens)
				{
					comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_barragem_barragens
							  (id,
							   barragem,
							   quantidade,
							   finalidade,
							   especificar,
							   geometria_id,
							   geometria_tipo,
							   geometria_coord_atv_x,
							   geometria_coord_atv_y,
							   tid)
							values
							  ({0}seq_crt_barragem_barragens.nextval,
							   :barragem,
							   :quantidade,
							   :finalidade,
							   :especificar,
							   :geometria_id,
							   :geometria_tipo,
							   :geometria_coord_atv_x,
							   :geometria_coord_atv_y,
							   :tid) returning id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("barragem", barragem.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("quantidade", itemBarragem.Quantidade, DbType.Int32);
					comando.AdicionarParametroEntrada("finalidade", itemBarragem.FinalidadeId, DbType.Int32);
					comando.AdicionarParametroEntrada("especificar", DbType.String, 30, itemBarragem.Especificar);
					comando.AdicionarParametroEntrada("geometria_id", itemBarragem.CoordenadaAtividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_tipo", itemBarragem.CoordenadaAtividade.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_coord_atv_x", itemBarragem.CoordenadaAtividade.CoordX, DbType.Decimal);
					comando.AdicionarParametroEntrada("geometria_coord_atv_y", itemBarragem.CoordenadaAtividade.CoordY, DbType.Decimal);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					itemBarragem.Id = Convert.ToInt32(comando.ObterValorParametro("id"));


                    //Comando para inserir dados na tabela CRT_BARRAGENS_FINALIDADES
                    //Alteração na legislação: 1 Barragem pode possuir uma ou várias finalidades.
                    comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_barragens_finalidades
							  (id,
							   crt_barragem,
							   barragem,
							   finalidade,
							   finalidadeTexto)
							values
							  ({0}seq_crt_barragens_finalidades.nextval,
							   :itemBarragem.Id,
							   :barragem,
							   :finalidade,
							   :finalidadeTexto) returning id into :id", EsquemaBanco);

                    comando.AdicionarParametroEntrada("barragem", barragem.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("quantidade", itemBarragem.Quantidade, DbType.Int32);
                    comando.AdicionarParametroEntrada("finalidade", itemBarragem.FinalidadeId, DbType.Int32);
                    comando.AdicionarParametroEntrada("finalidadetexto", DbType.String, 36, itemBarragem.FinalidadeTexto);                    
                    comando.AdicionarParametroSaida("id", DbType.Int32);

                    bancoDeDados.ExecutarNonQuery(comando);

                    //itemBarragem.Id = Convert.ToInt32(comando.ObterValorParametro("id"));


					#region Barragem - Barragens Dados

					foreach (var itemBarragemDados in itemBarragem.BarragensDados)
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_barragem_brgns_dados
							  (id,
							   barragens,
							   identificador,
							   lamina_agua,
							   volume_armazenamento,
							   outorga,
							   numero,
							   tid)
							values
							  ({0}seq_crt_barragem_brgnsdados.nextval,
							   :barragens,
							   :identificador,
							   :lamina_agua,
							   :volume_armazenamento,
							   :outorga,
							   :numero,
							   :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("barragens", itemBarragem.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificador", itemBarragemDados.Identificador, DbType.Int32);
						comando.AdicionarParametroEntrada("lamina_agua", itemBarragemDados.LaminaAgua, DbType.Decimal);
						comando.AdicionarParametroEntrada("volume_armazenamento", itemBarragemDados.VolumeArmazenamento, DbType.Decimal);
						comando.AdicionarParametroEntrada("outorga", itemBarragemDados.OutorgaId, DbType.Int32);
						comando.AdicionarParametroEntrada("numero", DbType.String, 15, itemBarragemDados.Numero);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}

					#endregion
				}

				#endregion

				#region Histórico

				Historico.Gerar(barragem.Id, eHistoricoArtefatoCaracterizacao.barragem, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return barragem.Id;
			}
		}

		internal void Editar(Barragem barragem, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Barragem

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_barragem c set c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", barragem.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Barragem - Barragens

				foreach (var item in barragem.Barragens)
				{
                    //Deleta registro na tabela agregada CRT_BARRAGENS_FINALIDADES
                    comando = bancoDeDados.CriarComando("delete from {0}crt_barragens_finalidades t ", EsquemaBanco);
                    comando.DbCommand.CommandText += String.Format("where t.barragem = :barragem{0} ",
                    comando.AdicionarNotIn("and", "t.id_barragem_dados", DbType.Int32, item.BarragensDados.Select(x => x.IdRelacionamento).ToList()));
                    comando.AdicionarParametroEntrada("barragem", item.Id, DbType.Int32);

                    bancoDeDados.ExecutarNonQuery(comando);

                    //Deleta registro na tabela agregada CRT_BARRAGEM_BRGNS_DADOS
					comando = bancoDeDados.CriarComando("delete from {0}crt_barragem_brgns_dados t ", EsquemaBanco);
					comando.DbCommand.CommandText += String.Format("where t.barragens = :barragem{0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, item.BarragensDados.Select(x => x.IdRelacionamento).ToList()));
					comando.AdicionarParametroEntrada("barragem", item.Id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				comando = bancoDeDados.CriarComando("delete from {0}crt_barragem_barragens t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.barragem = :barragem{0}",
				comando.AdicionarNotIn("and", "t.id", DbType.Int32, barragem.Barragens.Select(x => x.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("barragem", barragem.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

                                
				foreach (var itemBarragem in barragem.Barragens)
				{
					if (itemBarragem.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}crt_barragem_barragens t
							   set t.quantidade            = :quantidade,
								   t.geometria_id          = :geometria_id,
								   t.geometria_tipo        = :geometria_tipo,
								   t.geometria_coord_atv_x = :geometria_coord_atv_x,
								   t.geometria_coord_atv_y = :geometria_coord_atv_y,
								   t.tid				   = :tid
							 where t.id = :id ", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", itemBarragem.Id, DbType.Int32);


//                        comando2 = bancoDeDados.CriarComando(@"update {0}crt_barragens_finalidades c set c.finalidade = :finalidade, 
//                                                                                                           c.finalidadetexto = :finalidadetexto, 
//                                                                                                           c.barragem = :barragem{0}
//                                                                       where c.id = :id and c.finalidade = :finalidade", EsquemaBanco);

                        
                        //comando2.AdicionarParametroEntrada("id", itemBarragem.Id, DbType.Int32);
                      //  comando2.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                        


					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_barragem_barragens
							  (id,
							   barragem,
							   quantidade,
							   geometria_id,
							   geometria_tipo,
							   geometria_coord_atv_x,
							   geometria_coord_atv_y,
							   tid)
							values
							  ({0}seq_crt_barragem_barragens.nextval,
							   :barragem,
							   :quantidade,
							   :geometria_id,
							   :geometria_tipo,
							   :geometria_coord_atv_x,
							   :geometria_coord_atv_y,
							   :tid) returning id into :id", EsquemaBanco);
						
						comando.AdicionarParametroEntrada("barragem", barragem.Id, DbType.Int32);
						comando.AdicionarParametroSaida("id", DbType.Int32);

                       
                       
					}

					comando.AdicionarParametroEntrada("quantidade", itemBarragem.Quantidade, DbType.Int32);
                    //comando.AdicionarParametroEntrada("finalidade", itemBarragem.FinalidadeId, DbType.Int32);
                    //comando.AdicionarParametroEntrada("especificar", DbType.String, 30, itemBarragem.Especificar);
					comando.AdicionarParametroEntrada("geometria_id", itemBarragem.CoordenadaAtividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_tipo", itemBarragem.CoordenadaAtividade.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("geometria_coord_atv_x", itemBarragem.CoordenadaAtividade.CoordX, DbType.Decimal);
					comando.AdicionarParametroEntrada("geometria_coord_atv_y", itemBarragem.CoordenadaAtividade.CoordY, DbType.Decimal);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());					

					bancoDeDados.ExecutarNonQuery(comando);

                    // Commando 2: Operação na tabela CRT_BARRAGENS_FINALIDADES

                    //comando2.AdicionarParametroEntrada("finalidade", itemBarragem.FinalidadeId, DbType.Int32);
                    //comando2.AdicionarParametroEntrada("finalidadetexto", DbType.String, 2500, itemBarragem.FinalidadeTexto);
                    
                    //bancoDeDados.ExecutarNonQuery(comando2);



					itemBarragem.Id = itemBarragem.Id > 0 ? itemBarragem.Id : Convert.ToInt32(comando.ObterValorParametro("id"));

					#region Barragem - Barragens Dados

					foreach (var itemBarragemDados in itemBarragem.BarragensDados)
					{

						if (itemBarragemDados.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"
							update {0}crt_barragem_brgns_dados t
							   set t.identificador         = :identificador,
								   t.lamina_agua           = :lamina_agua,
								   t.volume_armazenamento  = :volume_armazenamento,
								   t.outorga               = :outorga,
								   t.numero                = :numero,
								   t.tid				   = :tid
							 where t.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", itemBarragemDados.Id, DbType.Int32);
						}
						else
						{							
							comando = bancoDeDados.CriarComando(@"
								insert into {0}crt_barragem_brgns_dados
								  (id,
								   barragens,
								   identificador,
								   lamina_agua,
								   volume_armazenamento,
								   outorga,
								   numero,
								   tid)
								values
								  ({0}seq_crt_barragem_brgnsdados.nextval,
								   :barragens,
								   :identificador,
								   :lamina_agua,
								   :volume_armazenamento,
								   :outorga,
								   :numero,
								   :tid) returning id into :idBarragemDados", EsquemaBanco);

							comando.AdicionarParametroEntrada("barragens", itemBarragem.Id, DbType.Int32);
                            comando.AdicionarParametroSaida("idBarragemDados", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("identificador", itemBarragemDados.Identificador, DbType.Int32);
						comando.AdicionarParametroEntrada("lamina_agua", itemBarragemDados.LaminaAguaToDecimal, DbType.Decimal);
						comando.AdicionarParametroEntrada("volume_armazenamento", itemBarragemDados.VolumeArmazenamentoToDecimal, DbType.Decimal);
						comando.AdicionarParametroEntrada("outorga", itemBarragemDados.OutorgaId, DbType.Int32);
						comando.AdicionarParametroEntrada("numero", DbType.String, 15, itemBarragemDados.Numero);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

                        if (itemBarragemDados.Id == 0)
                        {

                            // Commando 2: Operação na tabela CRT_BARRAGENS_FINALIDADES
                            foreach (var idFinalidade in itemBarragemDados.ListaIdsFinalidades)
                            {
                                Comando comando2 = bancoDeDados.CriarComando(@"
                                insert into {0}crt_barragens_finalidades 
                                (id, 
                                id_barragem_dados, 
                                barragem, 
                                finalidade, 
                                finalidade_texto)
                                values    
                                ({0}seq_crt_barragens_finalidades.nextval, 
                                    :barragemDados, 
                                    :barragem, 
                                    :finalidade, 
                                    (select f.texto from lov_crt_barragem_finalidade f where f.id = :finalidade))", EsquemaBanco);

                                comando2.AdicionarParametroEntrada("barragemDados", Convert.ToInt32(comando.ObterValorParametro("idBarragemDados")), DbType.Int32);
                                comando2.AdicionarParametroEntrada("barragem", itemBarragem.Id, DbType.Int32);
                                comando2.AdicionarParametroEntrada("finalidade", idFinalidade, DbType.Int32);

                                bancoDeDados.ExecutarNonQuery(comando2);
                            }
                        }
                        }
					
					#endregion
                    }

				#endregion

				#region Histórico

				Historico.Gerar(barragem.Id, eHistoricoArtefatoCaracterizacao.barragem, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select cb.id from {0}crt_barragem cb where cb.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_barragem c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.barragem, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					"delete from {0}crt_barragem_brgns_dados b where b.barragens in (select id from {0}crt_barragem_barragens where barragem = :caracterizacao);" +
					"delete from {0}crt_barragem_barragens b where b.barragem = :caracterizacao;" +
					"delete from {0}crt_barragem r where r.id = :caracterizacao;" +
                    "delete from {0}crt_barragens_finalidades f where f.barragem = :caracterizacao;" +
				"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.Barragem, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void ExcluirBarragemItem(int barragemItemId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select t.barragem from crt_barragem_barragens t where t.id = :barragemItemId", EsquemaBanco);
				comando.AdicionarParametroEntrada("barragemItemId", barragemItemId, DbType.Int32);

				int id = 0;
				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					id = Convert.ToInt32(retorno);
				}

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}crt_barragem c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.barragem, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				#region Apaga Barragem Item

				comando = bancoDeDados.CriarComando(@"begin " +
					"delete from {0}crt_barragem_brgns_dados b where b.barragens in (select id from {0}crt_barragem_barragens where id = :barragemItemId);" +
					"delete from {0}crt_barragem_barragens b where b.id = :barragemItemId;" +
                    "delete from {0}crt_barragens_finalidades f where f.crt_barragem = :barragemItemId;" +           
				"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragemItemId", barragemItemId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal Barragem ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Barragem regularizacao = new Barragem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_barragem s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					regularizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return regularizacao;
		}


        internal List<BarragemItem> ObterListaFinalidade(int id, BancoDeDados banco = null, bool simplificado = false)
        {
            List<BarragemItem> barragemFinalidade = new List<BarragemItem>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                #region ListaFinalidadeTexto
                
                //Seleciona todas as finalidades de uma caracterização

                Comando comando = bancoDeDados.CriarComando(@"
					select b.finalidade FinalidadeId, b.finalidade_texto FinalidadeTexto
                    from {0}crt_barragens_finalidades b    
                    where b.id_barragem_dados = :id", EsquemaBanco); 

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                barragemFinalidade = bancoDeDados.ObterEntityList<BarragemItem>(comando);

                #endregion
            }
            return barragemFinalidade;
        }

        internal void SalvarEdicaoFinalidades(int idBarragem, int idBarragemGeral, List<int> idsFinalidades, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                #region ListaFinalidadeTexto

                //Seleciona todas as finalidades de uma caracterização

                 //Deleta registro na tabela agregada CRT_BARRAGENS_FINALIDADES
                Comando comando = bancoDeDados.CriarComando("delete from {0}crt_barragens_finalidades t ", EsquemaBanco);
                comando.DbCommand.CommandText += String.Format("where t.barragem = :idBarragemGeral{0} and t.ID_BARRAGEM_DADOS = :idBarragem{0}", EsquemaBanco);
                //comando.AdicionarNotIn("and", "t.ID_BARRAGEM_DADOS = :", DbType.Int32, barragem.Barragens.Select(x => x.IdRelacionamento).ToList()));
                comando.AdicionarParametroEntrada("idBarragem", idBarragem, DbType.Int32);
                comando.AdicionarParametroEntrada("idBarragemGeral", idBarragemGeral, DbType.Int32);

                bancoDeDados.ExecutarNonQuery(comando);

                foreach (int idFinalidade in idsFinalidades)
                {
                    comando = bancoDeDados.CriarComando(@"
								insert into {0}crt_barragens_finalidades
								  (id,
								   barragem,
								   finalidade,
								   finalidade_texto,
								   id_barragem_dados)
								values
								  ({0}seq_crt_barragens_finalidades.nextval,
								   :idBarragemGeral,
                                   :idFinalidade,
								   (select f.texto from lov_crt_barragem_finalidade f where f.id = :idFinalidade),
								   :idBarragem)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("idBarragem", idBarragem, DbType.Int32);
                    comando.AdicionarParametroEntrada("idBarragemGeral", idBarragemGeral, DbType.Int32);
                    comando.AdicionarParametroEntrada("idFinalidade", idFinalidade, DbType.Int32);

                    //                barragemFinalidade = bancoDeDados.ObterEntityList<BarragemItem>(comando);

                    bancoDeDados.ExecutarNonQuery(comando);
                }

                #endregion
            }
        }

		internal Barragem Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Barragem barragem = new Barragem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Barragem

				Comando comando = bancoDeDados.CriarComando(@"
					select cb.id Id, 
						   cb.empreendimento EmpreendimentoId,
						   cb.tid Tid
					  from {0}crt_barragem cb
					 where cb.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				barragem = bancoDeDados.ObterEntity<Barragem>(comando);

				#endregion

				if (simplificado)
				{
					return barragem;
				}

				#region Barragem - Barragens

                 //lcbf.texto FinalidadeTexto,
                //{0}lov_crt_barragem_finalidade lcbf,
                //cbb.finalidade = lcbf.id(+)
				comando = bancoDeDados.CriarComando(@"
					select cbb.id Id,
						   cbb.id IdRelacionamento,
						   cbb.quantidade Quantidade,
                           cbb.finalidade FinalidadeId, 						   
                           f_get_lista_finalidades(cbb.id) FinalidadeTexto,   					  
						   cbb.especificar Especificar,
						   cbb.geometria_id,
						   cbb.geometria_tipo geometria_tipo_id,
						   lcgt.texto geometria_tipo_texto,
						   cbb.geometria_coord_atv_x,
						   cbb.geometria_coord_atv_y,
						   cbb.tid
					  from {0}crt_barragem_barragens      cbb,						   
						   {0}lov_crt_geometria_tipo      lcgt
					 where 
					       cbb.geometria_tipo = lcgt.id(+)
					   and cbb.barragem = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				barragem.Barragens = bancoDeDados.ObterEntityList<BarragemItem>(comando, (IDataReader reader, BarragemItem item) =>
				{
					item.CoordenadaAtividade.Id = reader.GetValue<int>("geometria_id");
					item.CoordenadaAtividade.Tipo = reader.GetValue<int>("geometria_tipo_id");
					item.CoordenadaAtividade.TipoTexto = reader.GetValue<string>("geometria_tipo_texto");
					item.CoordenadaAtividade.CoordX = reader.GetValue<decimal>("geometria_coord_atv_x");
					item.CoordenadaAtividade.CoordY = reader.GetValue<decimal>("geometria_coord_atv_y");
				});

				#region Barragem - Barragens Dados

				foreach (var item in barragem.Barragens)
				{
					comando = bancoDeDados.CriarComando(@"
					select cbb.id Id,
						   cbb.id IdRelacionamento,
						   cbb.identificador Identificador,
                           f_get_lista_finalidades_caract(cbb.id) FinalidadeTexto,  
						   cbb.lamina_agua LaminaAgua,
						   cbb.volume_armazenamento VolumeArmazenamento,
						   cbb.outorga OutorgaId,
						   lcbo.texto OutorgaTexto,
						   cbb.numero Numero,
						   cbb.tid Tid
					  from {0}crt_barragem_brgns_dados cbb,
						   {0}lov_crt_barragem_outorga lcbo
					 where cbb.outorga = lcbo.id(+)
					   and cbb.barragens = :id order by cbb.identificador", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);

					item.BarragensDados = bancoDeDados.ObterEntityList<BarragemDadosItem>(comando);
				}			

				#endregion

				#endregion
			}
			return barragem;
		}

		private Barragem ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Barragem barragem = new Barragem();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Barragem

				Comando comando = bancoDeDados.CriarComando(@"
					select hcb.id Id, 
						   hcb.empreendimento EmpreendimentoId, 
						   hcb.tid Tid
					  from {0}hst_crt_barragem hcb
					 where hcb.barragem_id = :id
					   and hcb.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				barragem = bancoDeDados.ObterEntity<Barragem>(comando);

				#endregion

				if (simplificado)
				{
					return barragem;
				}

				#region Barragem - Barragens 

                
				comando = bancoDeDados.CriarComando(@"
					select hcbb.barragem_barragens_id Id,
						   hcbb.quantidade Quantidade,
						   hcbb.finalidade_id FinalidadeId,
						   hcbb.finalidade_texto FinalidadeTexto,
						   hcbb.especificar Especificar,
						   hcbb.identificador Identificador,
						   hcbb.lamina_agua LaminaAgua,
						   hcbb.volume_armazenamento VolumeArmazenamento,
						   hcbb.outorga_id OutorgaId,
						   hcbb.outorga_texto OutorgaTexto,
						   hcbb.numero Numero,
						   hcbb.geometria_id,
						   hcbb.geometria_tipo_id,
						   hcbb.geometria_tipo_texto,
						   hcbb.geometria_coord_atv_x,
						   hcbb.geometria_coord_atv_y,
						   hcbb.tid
					  from {0}hst_crt_barragem_barragens hcbb
					 where hcbb.barragem_id = :id
					   and hcbb.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				barragem.Barragens = bancoDeDados.ObterEntityList<BarragemItem>(comando, (IDataReader reader, BarragemItem item) =>
				{
					item.CoordenadaAtividade.Id = reader.GetValue<int>("geometria_id");
					item.CoordenadaAtividade.Tipo = reader.GetValue<int>("geometria_tipo_id");
					item.CoordenadaAtividade.TipoTexto = reader.GetValue<string>("geometria_tipo_texto");
					item.CoordenadaAtividade.CoordX = reader.GetValue<decimal>("geometria_coord_atv_x");
					item.CoordenadaAtividade.CoordX = reader.GetValue<decimal>("geometria_coord_atv_y");
				});

				#region Barragem - Barragens Dados

				foreach (var item in barragem.Barragens)
				{
					comando = bancoDeDados.CriarComando(@"
					select hcbb.barragens_dados_id Id,						   
						   hcbb.identificador Identificador,                           
						   hcbb.lamina_agua LaminaAgua,
						   hcbb.volume_armazenamento VolumeArmazenamento,
						   hcbb.outorga_id OutorgaId,
						   hcbb.outorga_texto OutorgaTexto,
						   hcbb.numero Numero,
						   hcbb.tid
					  from {0}hst_crt_brgm_brgns_dados hcbb
					 where hcbb.barragens_id = :id
					   and hcbb.tid = :tid order by hcbb.identificador", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);

					item.BarragensDados = bancoDeDados.ObterEntityList<BarragemDadosItem>(comando);
				}

				#endregion

				#endregion
			}

			return barragem;
		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int barragemId, BancoDeDados banco = null)
		{
			CaracterizacaoPDF caract = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select ta.quantidade, 
						   lf.texto finalidade, 
						   tb.lamina_agua, 
						   tb.volume_armazenamento,
						   ta.geometria_coord_atv_x,
						   ta.geometria_coord_atv_y
					  from {0}crt_barragem_barragens ta,
						   {0}lov_crt_barragem_finalidade lf,
						   (select sum(cbbd.lamina_agua) lamina_agua,
								   sum(cbbd.volume_armazenamento) volume_armazenamento,
								   cbbd.barragens
							  from {0}crt_barragem_barragens cbb, {0}crt_barragem_brgns_dados cbbd
							 where cbb.id = cbbd.barragens
							 group by cbbd.barragens) tb
					 where ta.finalidade = lf.id(+)
					   and ta.id = tb.barragens
					   and ta.id = :barragemId", EsquemaBanco);

				comando.AdicionarParametroEntrada("barragemId", barragemId, DbType.Int32);				

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caract = new CaracterizacaoPDF();
						caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Quantidade de Barragens", Valor = reader.GetValue<string>("quantidade") });
						caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Finalidade", Valor = reader.GetValue<string>("finalidade") });
						caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Área total de lâmina (ha)", Valor = reader.GetValue<string>("lamina_agua") });
						caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Volume Total armazenado (m³)", Valor = reader.GetValue<string>("volume_armazenamento") });
						caract.EastingLongitude = reader.GetValue<decimal>("geometria_coord_atv_x").ToString("F2");
						caract.NorthingLatitude = reader.GetValue<decimal>("geometria_coord_atv_y").ToString("F2");
					}
					reader.Close();
				}
			}

			return caract;
		}

		internal List<Lista> ObterBarragens(int empreendimentoId, string tid = "", int barragemId = 0, BancoDeDados banco = null)
		{
			var lst = new List<Lista>();
			var item = new Lista();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select ta.id,
						   ta.quantidade,
						   lf.texto,
						   tb.lamina_agua,
						   tb.volume_armazenamento
					  from {0}crt_barragem_barragens ta,
						   {0}crt_barragem cb,
						   {0}lov_crt_barragem_finalidade lf,
						   (select sum(cbbd.lamina_agua) lamina_agua,
								   sum(cbbd.volume_armazenamento) volume_armazenamento,
								   cbbd.barragens
							  from {0}crt_barragem_barragens cbb, 
								   {0}crt_barragem_brgns_dados cbbd
							 where cbb.id = cbbd.barragens
							 group by cbbd.barragens) tb
					 where ta.finalidade = lf.id(+)
					   and ta.id = tb.barragens
					   and ta.barragem = cb.id
					   and cb.empreendimento = :empreendimentoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lst.Add(new Lista 
						{ 
							Id = reader.GetValue<string>("id"),
							Texto = string.Format("Quantidade: {0}; Finalidade: {1}; Área total de lâmina (ha): {2}; Volume total armazenado (m³): {3};", 
							reader.GetValue<string>("quantidade"), 
							reader.GetValue<string>("texto"), 
							reader.GetValue<string>("lamina_agua"), 
							reader.GetValue<string>("volume_armazenamento")),
							IsAtivo = true
						});
					}
					reader.Close();
				}

				if (!string.IsNullOrEmpty(tid) && barragemId > 0)
				{
					comando = bancoDeDados.CriarComando(@"
						select t.barragem_barragens_id id,
							   t.quantidade,
							   t.finalidade_texto,
							   tb.lamina_agua,
							   tb.volume_armazenamento
						  from {0}hst_crt_barragem_barragens t,
							   (select sum(lamina_agua) lamina_agua,
									   sum(volume_armazenamento) volume_armazenamento
								  from (select t.barragens_id, 
											   t.lamina_agua, 
											   t.volume_armazenamento
										  from {0}hst_crt_brgm_brgns_dados t
										 where t.barragens_id = :barragemId
										   and t.tid = :tid)) tb
						 where t.barragem_barragens_id = :barragemId
						   and t.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("barragemId", barragemId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							item = new Lista
							{
								Id = reader.GetValue<string>("id"),
								Texto = string.Format("Quantidade: {0}; Finalidade: {1}; Área total de lâmina (ha): {2}; Volume total armazenado (m³): {3};",
								reader.GetValue<string>("quantidade"),
								reader.GetValue<string>("finalidade_texto"),
								reader.GetValue<string>("lamina_agua"),
								reader.GetValue<string>("volume_armazenamento")),
								IsAtivo = true
							};
						}
						reader.Close();
					}
					if (!lst.Exists(x => x.Id == item.Id))
					{
						lst.Add(item);
					}
				}
			}
			return lst;
		}

		#endregion

		#region Validações

		internal bool TemARL(int empreendimentoId, BancoDeDados banco = null)
		{
			bool temARL = false;
			object valor = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_empreendimento_endereco t, {0}crt_projeto_geo cpg, {1}geo_arl ga 
				where t.correspondencia = 0 and t.empreendimento = cpg.empreendimento and cpg.caracterizacao = 1 /*Dominialidade*/
				and cpg.id = ga.projeto and t.zona = 2 /*Rural*/ and t.empreendimento = :empreendimentoId", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				valor = bancoDeDados.ExecutarScalar(comando);

				temARL = valor != null && !Convert.IsDBNull(valor) && Convert.ToInt32(valor) > 0;
			}

			return temARL;
		}

		internal bool TemARLDesconhecida(int empreendimentoId, BancoDeDados banco = null)
		{
			bool temARLDesconhecida = false;
			object valor = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_empreendimento_endereco t, {0}crt_projeto_geo cpg, {1}geo_arl ga 
				where t.correspondencia = 0 and t.empreendimento = cpg.empreendimento and cpg.caracterizacao = 1 /*Dominialidade*/ 
				and cpg.id = ga.projeto and ga.situacao = 'D'/*NÃO CARACTERIZADA*/ and t.zona = 2 /*Rural*/ and t.empreendimento = :empreendimentoId", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimentoId", empreendimentoId, DbType.Int32);

				valor = bancoDeDados.ExecutarScalar(comando);

				temARLDesconhecida = valor != null && !Convert.IsDBNull(valor) && Convert.ToInt32(valor) > 0;
			}

			return temARLDesconhecida;
		}

		#endregion
	}
}
