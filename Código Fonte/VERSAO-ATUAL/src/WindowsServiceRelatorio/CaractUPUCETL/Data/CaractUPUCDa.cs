using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;

namespace Tecnomapas.EtramiteX.WindowsService.CaractUPUCETL.Data
{
	class CaractUPUCDa
	{
		ConfiguracaoSistema _configSys;
		RelatorioDa _daRelatorio;

		public CaractUPUCDa()
		{
			_configSys = new ConfiguracaoSistema();
			_daRelatorio = new RelatorioDa();
		}

		public List<Dictionary<string, object>> Eleitos(DateTime execucaoInicio, BancoDeDados banco = null)
		{
			List<Dictionary<string, object>> retorno = new List<Dictionary<string, object>>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
                select distinct t.hst_id, t.cod_modelo, t.titulo_id,
                       (case when t.acao != 3 then null else 3 end) acao
                  from (select h.id hst_id,
                               hm.codigo cod_modelo,
                               h.titulo_id,
                               h.acao_executada,
                               (select a.acao
                                  from lov_historico_artefatos_acoes a
                                 where a.id = h.acao_executada) acao
                          from hst_titulo h,
                               hst_titulo_modelo hm
                         where (h.titulo_id, h.data_execucao) in
                               (select ht.titulo_id, max(ht.data_execucao)
                                  from hst_titulo ht
                                 where h.data_execucao > :execucao_inicio
                                   and ht.acao_executada in (70, 72, 74, 75, 342)
                                 group by ht.titulo_id)
                            and hm.modelo_id = h.modelo_id
                            and hm.tid = h.modelo_tid
                            and hm.codigo in (51,52)) t");

				comando.AdicionarParametroEntrada("execucao_inicio", execucaoInicio, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dictionary<string, object> eleito;
					while (reader.Read())
					{
						eleito = new Dictionary<string, object>();
						eleito.Add("hst_titulo_id", reader.GetValue<int>("hst_id"));
						eleito.Add("titulo_id", reader.GetValue<int>("titulo_id"));
						eleito.Add("cod_modelo", reader.GetValue<int>("cod_modelo"));
						eleito.Add("Acao", reader.GetValue<int>("acao"));
						retorno.Add(eleito);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		#region Metodos UP - Unidade de Produção
		internal void SalvarUP(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualiza os dados da ETL

				Dictionary<string, object> fato;

				Comando comando = bancoDeDados.CriarComando(@"update fat_propriedade f set f.codigo = :codigo, 
                    f.cnpj = :cnpj, f.municipio_id = :municipio_id, f.municipio_texto = :municipio_texto, f.nome = :nome, f.estado_id = :estado_id, 
                    f.estado_texto = :estado_texto, f.codigo_propriedade = :codigo_propriedade, f.local_livro = :local_livro where f.id = :id");

				if (eleitos != null && eleitos.Count > 0)
				{
					int idFatoExistente = 0;
					eleitos.ForEach(item =>
					{
						idFatoExistente = ExisteUP(Convert.ToInt32(item["titulo_id"]), bancoDeDados);

						if (idFatoExistente > 0)
						{
							fato = ObterUnidadeProducao(Convert.ToInt32(item["hst_titulo_id"]));

							if (fato != null && fato.Count > 0)
							{
								comando.DbCommand.Parameters.Clear();

								try
								{
									comando.AdicionarParametroEntrada("id", idFatoExistente, DbType.Int32);
									comando.AdicionarParametroEntrada("codigo", fato["CODIGO"], DbType.Int32);
									comando.AdicionarParametroEntrada("cnpj", fato["CNPJ"], DbType.String);
									comando.AdicionarParametroEntrada("municipio_id", fato["MUNICIPIO_ID"], DbType.Int32);
									comando.AdicionarParametroEntrada("municipio_texto", fato["MUNICIPIO_TEXTO"], DbType.String);
									comando.AdicionarParametroEntrada("nome", fato["NOME"], DbType.String);
									comando.AdicionarParametroEntrada("estado_id", fato["ESTADO_ID"], DbType.Int32);
									comando.AdicionarParametroEntrada("estado_texto", fato["ESTADO_TEXTO"], DbType.String);
									comando.AdicionarParametroEntrada("codigo_propriedade", fato["CODIGO_PROPRIEDADE"], DbType.Double);
									comando.AdicionarParametroEntrada("local_livro", fato["LOCAL_LIVRO"], DbType.String);
								}
								catch (Exception yy)
								{
									throw yy;
								}

								try
								{
									bancoDeDados.ExecutarNonQuery(comando);
								}
								catch (Exception ee)
								{
									throw ee;
								}

								try
								{
									DimensoesUp(idFatoExistente, Convert.ToInt32(item["hst_titulo_id"]), bancoDeDados, true);
								}
								catch (Exception xx)
								{
									throw xx;
								}
							}
						}
						else
						{
							CriarUP(new List<Dictionary<string, object>>() { item }, bancoDeDados);
						}
					});
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void CriarUP(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			try
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
				{
					bancoDeDados.IniciarTransacao();

					#region Cria os dados da ETL

					Dictionary<string, object> fato;

					Comando comando =
						bancoDeDados.CriarComando(
							@"insert into fat_propriedade f (id, codigo, cnpj, municipio_id, municipio_texto, nome, estado_id, estado_texto, codigo_propriedade, local_livro) values
                        (seq_fat_propriedade.nextval, :codigo, :cnpj, :municipio_id, :municipio_texto, :nome, :estado_id, :estado_texto, :codigo_propriedade, :local_livro) returning f.id into :id");

					if (eleitos != null && eleitos.Count > 0)
					{
						foreach (var item in eleitos)
						{
							fato = ObterUnidadeProducao(Convert.ToInt32(item["hst_titulo_id"]));

							if (fato != null && fato.Count > 0)
							{
								comando.DbCommand.Parameters.Clear();

								comando.AdicionarParametroEntrada("codigo", fato["CODIGO"], DbType.Int32);
								comando.AdicionarParametroEntrada("cnpj", fato["CNPJ"], DbType.String);
								comando.AdicionarParametroEntrada("municipio_id", fato["MUNICIPIO_ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("municipio_texto", fato["MUNICIPIO_TEXTO"], DbType.String);
								comando.AdicionarParametroEntrada("nome", fato["NOME"], DbType.String);
								comando.AdicionarParametroEntrada("estado_id", fato["ESTADO_ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("estado_texto", fato["ESTADO_TEXTO"], DbType.String);
								comando.AdicionarParametroEntrada("codigo_propriedade", fato["CODIGO_PROPRIEDADE"], DbType.Double);
								comando.AdicionarParametroEntrada("local_livro", fato["LOCAL_LIVRO"], DbType.String);
								comando.AdicionarParametroSaida("id", DbType.Int32);

								try
								{
									bancoDeDados.ExecutarNonQuery(comando);
								}
								catch (Exception e)
								{
									throw e;
								}

								int idFatoUP = Convert.ToInt32(comando.ObterValorParametro("id"));

								try
								{
									DimensoesUp(idFatoUP, Convert.ToInt32(item["hst_titulo_id"]), bancoDeDados, true);
								}
								catch (Exception x)
								{
									throw x;
								}
							}
						}
					}

					#endregion

					bancoDeDados.Commit();
				}
			}
			catch (Exception exc)
			{
				throw exc;
			}
		}

		internal void ExcluirUP(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Apaga os dados da ETL

				List<String> lista = new List<string>();
				lista.Add("begin ");
				lista.Add("delete from dim_up_produtor p where p.fato = :fato;");
				lista.Add("delete from dim_up_resp_tec p where p.fato = :fato;");
				lista.Add("delete from dim_up_unidades p where p.fato = :fato;");
				lista.Add("delete from dim_up_titulo p where p.fato = :fato;");
				lista.Add("delete from dim_up_protocolo p where p.fato = :fato;");
				lista.Add("delete from fat_propriedade p where p.id = :fato;");
				lista.Add(" end;");

				Comando comando = bancoDeDados.CriarComando(String.Join(" ", lista));

				comando.AdicionarParametroEntrada("fato", DbType.Int32);

				eleitos.ForEach(item =>
				{
					int idFatoExistente = ExisteUC(Convert.ToInt32(item["titulo_id"]), bancoDeDados);
					comando.SetarValorParametro("fato", idFatoExistente);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				bancoDeDados.Commit();
			}
		}
		internal void DimensoesUp(int fatoId, int hstTituloId, BancoDeDados banco = null, bool deletar = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;
				List<Dictionary<string, object>> itens;

				#region Apaga os dados da ETL das dimensões

				if (deletar)
				{
					List<String> lista = new List<string>();
					lista.Add("begin ");
					lista.Add("delete from dim_up_produtor p where p.fato = :fato;");
					lista.Add("delete from dim_up_resp_tec p where p.fato = :fato;");
					lista.Add("delete from dim_up_unidades p where p.fato = :fato;");
					lista.Add("delete from dim_up_titulo p where p.fato = :fato;");
					lista.Add("delete from dim_up_protocolo p where p.fato = :fato;");
					lista.Add(" end;");

					comando = bancoDeDados.CriarComando(String.Join(" ", lista));
					comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
					bancoDeDados.Commit();
				}

				#endregion

				#region Titulo

				itens = ObterTitulo(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_up_titulo
                        (id, fato, titulo_id, numero_ano, data_cadastro, numero_requerimento, local_emissao_id, local_emissao_texto, situacao_id, situacao_texto, motivo_id, motivo_texto, situacao_motivo)
                    values
                      (seq_dim_up_titulo.nextval, :fato, :titulo_id, :numero_ano, :data_cadastro, :numero_requerimento, :local_emissao_id, :local_emissao_texto, :situacao_id, :situacao_texto, :motivo_id,
                       :motivo_texto, :situacao_motivo)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_ano", item["NUMERO_ANO_TITULO"], DbType.String);
						comando.AdicionarParametroEntrada("data_cadastro", (Convert.IsDBNull(item["DATA_CADASTRO"]) ? DBNull.Value : (object)Convert.ToDateTime(item["DATA_CADASTRO"]).ToShortDateString()), DbType.Date);
						comando.AdicionarParametroEntrada("numero_requerimento", item["NUMERO_REQUERIMENTO"], DbType.Int32);
						comando.AdicionarParametroEntrada("local_emissao_id", item["LOCAL_EMISSAO_TITULO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("local_emissao_texto", item["LOCAL_EMISSAO_TITULO"], DbType.String);
						comando.AdicionarParametroEntrada("situacao_id", item["SITUACAO_TITULO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_texto", item["SITUACAO_TITULO"], DbType.String);
						comando.AdicionarParametroEntrada("motivo_id", item["MOTIVO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("motivo_texto", item["MOTIVO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("situacao_motivo", item["SITUACAO_MOTIVO"], DbType.String);
						comando.AdicionarParametroEntrada("titulo_id", item["TITULO_ID"], DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Titulo

				#region Responsável Técnico

				itens = ObterResponsaveisTecnicosUP(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_up_resp_tec (id, fato, responsavel_tecnico, cpf_cnpj, numero_habilitacao_cfo_cfoc, numero_art, data_validade_art) values
                               (seq_dim_up_resp_tec.nextval, :fato, :responsavel_tecnico, :cpf_cnpj, :cfo_cfoc, :numero_art, :data_validade_art)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("responsavel_tecnico", item["RESPONSAVEL_TECNICO"], DbType.String);
						comando.AdicionarParametroEntrada("cpf_cnpj", item["CPF_CNPJ"], DbType.String);
						comando.AdicionarParametroEntrada("cfo_cfoc", item["NUMERO_HABILITACAO_CFO_CFOC"], DbType.String);
						comando.AdicionarParametroEntrada("numero_art", item["NUMERO_ART"], DbType.String);
						comando.AdicionarParametroEntrada("data_validade_art", (Convert.IsDBNull(item["DATA_VALIDADE_ART"]) ? null : Convert.ToDateTime(item["DATA_VALIDADE_ART"]).ToShortDateString()), DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Responsável do empreendimento

				#region Protocolo

				itens = ObterProtocolo(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_up_protocolo (id, fato, numero, tipo_protocolo_id, tipo_protocolo_texto, tipo_documento_id, tipo_documento_texto,
                       tipo_protocolo_documento, data_recebimento, setor_posse_id, setor_posse_texto)  values
                      (seq_dim_up_protocolo.nextval, :fato, :numero, :tipo_protocolo_id, :tipo_protocolo_texto, 
                       :tipo_documento_id, :tipo_documento_texto, :tipo_protocolo_documento, :data_recebimento,
                       :setor_posse_id, :setor_posse_texto)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("numero", item["NUMERO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_protocolo_id", item["TIPO_PROTOCOLO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_protocolo_texto", item["TIPO_PROTOCOLO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_documento_id", item["TIPO_DOCUMENTO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_documento_texto", item["TIPO_DOCUMENTO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_protocolo_documento", item["TIPO_PROTOCOLO_DOCUMENTO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("data_recebimento", (Convert.IsDBNull(item["DATA_RECEBIMENTO"]) ? DBNull.Value : (object)Convert.ToDateTime(item["DATA_RECEBIMENTO"]).ToShortDateString()), DbType.Date);
						comando.AdicionarParametroEntrada("setor_posse_id", item["SETOR_POSSE_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("setor_posse_texto", item["SETOR_POSSE_TEXTO"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Protocolo

				#region Produtor

				itens = ObterProdutorUP(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"
                        insert into dim_up_produtor (id, fato, cpf_cnpj_produtor, nome_razao_social_produtor, endereco, municipio_texto) 
                            values (seq_dim_up_produtor.nextval, :fato, :cpf_cnpj_produtor, :nome_razao_social_produtor, :endereco, :municipio_texto)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("cpf_cnpj_produtor", item["CPF_CNPJ_PRODUTOR"], DbType.String);
						comando.AdicionarParametroEntrada("nome_razao_social_produtor", item["NOME_RAZAO_SOCIAL_PRODUTOR"], DbType.String);
						comando.AdicionarParametroEntrada("endereco", item["ENDERECO"], DbType.String);
						comando.AdicionarParametroEntrada("municipio_texto", item["MUNICIPIO_TEXTO"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Unidades Produção

				itens = ObterUnidadesUP(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"
                        insert into dim_up_unidades (id, fato, codigo_up, tipo_producao, numero_renasem, data_validad_renasem, area, cultura, 
                            cultivar, cultura_cultivar, data_plantio, quantidade_ano, unidade_medida_id, unidade_medida_texto, quantidade_ano_unid_medida, ano_abertura, coordenadas_up)
                        values (seq_dim_up_unidades.nextval, :fato, :codigo_up, :tipo_producao, :numero_renasem, :data_validad_renasem, :area, :cultura, 
                            :cultivar, :cultura_cultivar, :data_plantio, :quantidade_ano, :unidade_medida_id, :unidade_medida, :quantidade_ano_unid_medida, :ano_abertura, :coordenadas_up)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("CODIGO_UP", item["CODIGO_UP"], DbType.Decimal);
						comando.AdicionarParametroEntrada("TIPO_PRODUCAO", item["TIPO_PRODUCAO"], DbType.String);
						comando.AdicionarParametroEntrada("NUMERO_RENASEM", item["NUMERO_RENASEM"], DbType.String);
						comando.AdicionarParametroEntrada("DATA_VALIDAD_RENASEM", (Convert.IsDBNull(item["DATA_VALIDAD_RENASEM"]) ? DBNull.Value : (object)Convert.ToDateTime(item["DATA_VALIDAD_RENASEM"]).ToShortDateString()), DbType.Date);
						comando.AdicionarParametroEntrada("AREA", item["AREA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("CULTURA", item["CULTURA"], DbType.String);
						comando.AdicionarParametroEntrada("CULTIVAR", item["CULTIVAR"], DbType.String);
						comando.AdicionarParametroEntrada("CULTURA_CULTIVAR", item["CULTURA_CULTIVAR"], DbType.String);
						comando.AdicionarParametroEntrada("QUANTIDADE_ANO", item["QUANTIDADE_ANO"], DbType.Decimal);
						comando.AdicionarParametroEntrada("UNIDADE_MEDIDA_ID", item["UNIDADE_MEDIDA_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("UNIDADE_MEDIDA", item["UNIDADE_MEDIDA_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("QUANTIDADE_ANO_UNID_MEDIDA", item["QUANTIDADE_ANO_UNID_MEDIDA"], DbType.String);
						comando.AdicionarParametroEntrada("ANO_ABERTURA", item["ANO_ABERTURA"], DbType.Int32);
						comando.AdicionarParametroEntrada("DATA_PLANTIO", item["DATA_PLANTIO"], DbType.String);
						comando.AdicionarParametroEntrada("COORDENADAS_UP", item["COORDENADAS_UP"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Metodos UC - Unidade de Consolidação
		internal void SalvarUC(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualiza os dados da ETL

				Dictionary<string, object> fato;

				Comando comando = bancoDeDados.CriarComando(@"update fat_uc f set 
                   f.id = :id,
                   f.uc_id = :uc_id,
                   f.codigo_uc = :codigo_uc,
                   f.local_livro_disponivel = :local_livro_disponivel,
                   f.empreendimento_codigo = :empreendimento_codigo,
                   f.empreend_denominador = :empreend_denominador,
                   f.empreend_cnpj = :empreend_cnpj,
                   f.empreend_municipio_id = :empreend_municipio_id,
                   f.empreend_municipio_texto = :empreend_municipio_texto
                   where f.id = :id");

				if (eleitos != null && eleitos.Count > 0)
				{
					int idFatoExistente = 0;
					eleitos.ForEach(item =>
					{
						idFatoExistente = ExisteUC(Convert.ToInt32(item["titulo_id"]), bancoDeDados);

						if (idFatoExistente > 0)
						{
							fato = ObterUnidadeConsolidacao(Convert.ToInt32(item["hst_titulo_id"]));

							if (fato != null && fato.Count > 0)
							{
								comando.DbCommand.Parameters.Clear();

								comando.AdicionarParametroEntrada("id", idFatoExistente, DbType.Int32);
								comando.AdicionarParametroEntrada("uc_id", fato["UC_ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("codigo_uc", fato["CODIGO_UC"], DbType.Decimal);
								comando.AdicionarParametroEntrada("local_livro_disponivel", fato["LOCAL_LIVRO_DISPONIVEL"], DbType.String);
								comando.AdicionarParametroEntrada("empreendimento_codigo", fato["EMPREENDIMENTO_CODIGO"], DbType.Decimal);
								comando.AdicionarParametroEntrada("empreend_denominador", fato["EMPREEND_DENOMINADOR"], DbType.String);
								comando.AdicionarParametroEntrada("empreend_cnpj", fato["EMPREEND_CNPJ"], DbType.String);
								comando.AdicionarParametroEntrada("empreend_municipio_id", fato["EMPREEND_MUNICIPIO_ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("empreend_municipio_texto", fato["EMPREEND_MUNICIPIO_TEXTO"], DbType.String);

								bancoDeDados.ExecutarNonQuery(comando);
								DimensoesUC(idFatoExistente, Convert.ToInt32(item["hst_titulo_id"]), bancoDeDados, true);
							}
						}
						else
						{
							CriarUC(new List<Dictionary<string, object>>() { item }, bancoDeDados);
						}
					});
				}

				#endregion

				bancoDeDados.Commit();
			}
		}
		internal void CriarUC(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cria os dados da ETL

				Dictionary<string, object> fato;

				Comando comando = bancoDeDados.CriarComando(@"insert into fat_uc f
                    (id, uc_id, codigo_uc, local_livro_disponivel, empreendimento_codigo, empreend_denominador, empreend_cnpj,
                    empreend_municipio_id, empreend_municipio_texto)
                values
                    (seq_fat_uc.nextval, :uc_id, :codigo_uc, :local_livro_disponivel, :empreendimento_codigo, :empreend_denominador,
                    :empreend_cnpj, :empreend_municipio_id, :empreend_municipio_texto) returning f.id into :id");

				if (eleitos != null && eleitos.Count > 0)
				{
					foreach (var item in eleitos)
					{

						fato = ObterUnidadeConsolidacao(Convert.ToInt32(item["hst_titulo_id"]));
						if (fato != null && fato.Count > 0)
						{
							comando.DbCommand.Parameters.Clear();

							comando.AdicionarParametroEntrada("uc_id", fato["UC_ID"], DbType.Int32);
							comando.AdicionarParametroEntrada("codigo_uc", fato["CODIGO_UC"], DbType.Decimal);
							comando.AdicionarParametroEntrada("local_livro_disponivel", fato["LOCAL_LIVRO_DISPONIVEL"], DbType.String);
							comando.AdicionarParametroEntrada("empreendimento_codigo", fato["EMPREENDIMENTO_CODIGO"], DbType.Int32);
							comando.AdicionarParametroEntrada("empreend_denominador", fato["EMPREEND_DENOMINADOR"], DbType.String);
							comando.AdicionarParametroEntrada("empreend_cnpj", fato["EMPREEND_CNPJ"], DbType.String);
							comando.AdicionarParametroEntrada("empreend_municipio_id", fato["EMPREEND_MUNICIPIO_ID"], DbType.Int32);
							comando.AdicionarParametroEntrada("empreend_municipio_texto", fato["EMPREEND_MUNICIPIO_TEXTO"], DbType.String);
							comando.AdicionarParametroSaida("id", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							int idFatoUC = Convert.ToInt32(comando.ObterValorParametro("id"));
							DimensoesUC(idFatoUC, Convert.ToInt32(item["hst_titulo_id"]), bancoDeDados, true);
						}
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}
		internal void ExcluirUC(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Apaga os dados da ETL

				List<String> lista = new List<string>();
				lista.Add("begin ");
				lista.Add("delete from dim_uc_capacid_armaze_proces p where p.fato = :fato;");
				lista.Add("delete from dim_uc_resp_tecnico p where p.fato = :fato;");
				lista.Add("delete from dim_uc_titulo p where p.fato = :fato;");
				lista.Add("delete from dim_uc_responsavel_emp p where p.fato = :fato;");
				lista.Add("delete from dim_uc_protocolo p where p.fato = :fato;");
				lista.Add("delete from fat_uc p where p.id = :fato;");
				lista.Add(" end;");

				Comando comando = bancoDeDados.CriarComando(String.Join(" ", lista));

				comando.AdicionarParametroEntrada("fato", DbType.Int32);

				eleitos.ForEach(item =>
				{
					int idFatoExistente = ExisteUC(Convert.ToInt32(item["titulo_id"]), bancoDeDados);
					comando.SetarValorParametro("fato", idFatoExistente);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				bancoDeDados.Commit();
			}
		}
		internal void DimensoesUC(int fatoId, int hstTituloId, BancoDeDados banco = null, bool deletar = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;
				List<Dictionary<string, object>> itens;

				#region Apaga os dados da ETL das dimensões

				if (deletar)
				{
					List<String> lista = new List<string>();
					lista.Add("begin ");
					lista.Add("delete from dim_uc_capacid_armaze_proces p where p.fato = :fato;");
					lista.Add("delete from dim_uc_resp_tecnico p where p.fato = :fato;");
					lista.Add("delete from dim_uc_titulo p where p.fato = :fato;");
					lista.Add("delete from dim_uc_responsavel_emp p where p.fato = :fato;");
					lista.Add("delete from dim_uc_protocolo p where p.fato = :fato;");
					lista.Add(" end;");

					comando = bancoDeDados.CriarComando(String.Join(" ", lista));
					comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
					bancoDeDados.Commit();
				}

				#endregion

				#region Cria os dados das dimensões

				#region Capacidade de armazenamento/processamento

				itens = ObterCapacidadeArmazenamentoProcessamento(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_uc_capacid_armaze_proces
                      (id, fato, cultura_id, cultura_nome, cultivar_id, cultivar_nome, cultura_cultivar, capacidade_mes, 
                       unidade_medida_id, unidade_medida_texto, capaci_mes_uni_medida, tipo_apresentacao_produto)
                    values
                      (seq_dim_uc_cap_armaze_proces.nextval, :fato, :cultura_id, :cultura_nome, :cultivar_id, :cultivar_nome, 
                       :cultura_cultivar, :capacidade_mes, :unidade_medida_id, :unidade_medida_texto, :capaci_mes_uni_medida, :tipo_apresentacao_produto)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("cultura_id", item["CULTURA_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("cultura_nome", item["CULTURA_NOME"], DbType.String);
						comando.AdicionarParametroEntrada("cultivar_id", item["CULTIVAR_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("cultivar_nome", item["CULTIVAR_NOME"], DbType.String);
						comando.AdicionarParametroEntrada("cultura_cultivar", item["CULTURA_CULTIVAR"], DbType.String);
						comando.AdicionarParametroEntrada("capacidade_mes", item["CAPACIDADE_MES"], DbType.Decimal);
						comando.AdicionarParametroEntrada("unidade_medida_id", item["UNIDADE_MEDIDA_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("unidade_medida_texto", item["UNIDADE_MEDIDA_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("capaci_mes_uni_medida", item["CAPACI_MES_UNI_MEDIDA"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_apresentacao_produto", item["TIPO_APRESENTACAO_PRODUTO"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Capacidade de armazenamento/processamento

				#region Responsável Técnico

				itens = ObterResponsavelTecnicoUC(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_uc_resp_tecnico
                      (id, fato, nome_razao, cpf_cnpj, numero_hab_cfo_cfoc, numero_art, data_validade_art)
                    values
                      (seq_dim_uc_resp_tecnico.nextval, :fato, :nome_razao, :cpf_cnpj, :numero_hab_cfo_cfoc, :numero_art, :data_validade_art)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("nome_razao", item["NOME_RAZAO"], DbType.String);
						comando.AdicionarParametroEntrada("cpf_cnpj", item["CPF_CNPJ"], DbType.String);
						comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", item["NUMERO_HAB_CFO_CFOC"], DbType.String);
						comando.AdicionarParametroEntrada("numero_art", item["NUMERO_ART"], DbType.String);
						comando.AdicionarParametroEntrada("data_validade_art", (Convert.IsDBNull(item["DATA_VALIDADE_ART"]) ? null : Convert.ToDateTime(item["DATA_VALIDADE_ART"]).ToShortDateString()), DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Responsável Técnico

				#region Titulo

				itens = ObterTitulo(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_uc_titulo
                      (id, fato, titulo_id, numero_ano, data_cadastro, numero_requerimento, local_emissao_id, local_emissao_texto,
                       situacao_id, situacao_texto, motivo_id, motivo_texto, situacao_motivo)
                    values
                      (seq_dim_uc_titulo.nextval, :fato, :titulo_id, :numero_ano, :data_cadastro, :numero_requerimento,
                       :local_emissao_id, :local_emissao_texto, :situacao_id, :situacao_texto, :motivo_id,
                       :motivo_texto, :situacao_motivo)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_id", item["TITULO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("numero_ano", item["NUMERO_ANO_TITULO"], DbType.String);
						comando.AdicionarParametroEntrada("data_cadastro", (Convert.IsDBNull(item["DATA_CADASTRO"]) ? DBNull.Value : (object)Convert.ToDateTime(item["DATA_CADASTRO"]).ToShortDateString()), DbType.Date);
						comando.AdicionarParametroEntrada("numero_requerimento", item["NUMERO_REQUERIMENTO"], DbType.Int32);
						comando.AdicionarParametroEntrada("local_emissao_id", item["LOCAL_EMISSAO_TITULO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("local_emissao_texto", item["LOCAL_EMISSAO_TITULO"], DbType.String);
						comando.AdicionarParametroEntrada("situacao_id", item["SITUACAO_TITULO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_texto", item["SITUACAO_TITULO"], DbType.String);
						comando.AdicionarParametroEntrada("motivo_id", item["MOTIVO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("motivo_texto", item["MOTIVO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("situacao_motivo", item["SITUACAO_MOTIVO"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Titulo

				#region Responsável do empreendimento

				itens = ObterResponsavelEmpreendimentoUC(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_uc_responsavel_emp
                      (id, fato, nome_razao, cpf_cnpj, endereco, municipio_id, municipio_texto)
                    values
                      (seq_dim_uc_responsavel_emp.nextval, :fato, :nome_razao, :cpf_cnpj, :endereco, :municipio_id, :municipio_texto)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("nome_razao", item["NOME_RAZAO"], DbType.String);
						comando.AdicionarParametroEntrada("cpf_cnpj", item["CPF_CNPJ"], DbType.String);
						comando.AdicionarParametroEntrada("endereco", item["ENDERECO"], DbType.String);
						comando.AdicionarParametroEntrada("municipio_id", item["MUNICIPIO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("municipio_texto", item["MUNICIPIO_TEXTO"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Responsável do empreendimento

				#region Protocolo

				itens = ObterProtocolo(hstTituloId);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into DIM_UC_PROTOCOLO
                      (id, fato, numero, tipo_protocolo_id, tipo_protocolo_texto, tipo_documento_id, tipo_documento_texto,
                       tipo_protocolo_documento, data_recebimento, setor_posse_id, setor_posse_texto) 
                    values
                      (seq_dim_uc_protocolo.nextval, :fato, :numero, :tipo_protocolo_id, :tipo_protocolo_texto, 
                       :tipo_documento_id, :tipo_documento_texto, :tipo_protocolo_documento, :data_recebimento,
                       :setor_posse_id, :setor_posse_texto)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", fatoId, DbType.Int32);
						comando.AdicionarParametroEntrada("numero", item["NUMERO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_protocolo_id", item["TIPO_PROTOCOLO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_protocolo_texto", item["TIPO_PROTOCOLO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_documento_id", item["TIPO_DOCUMENTO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_documento_texto", item["TIPO_DOCUMENTO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_protocolo_documento", item["TIPO_PROTOCOLO_DOCUMENTO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("data_recebimento", (Convert.IsDBNull(item["DATA_RECEBIMENTO"]) ? DBNull.Value : (object)Convert.ToDateTime(item["DATA_RECEBIMENTO"]).ToShortDateString()), DbType.Date);
						comando.AdicionarParametroEntrada("setor_posse_id", item["SETOR_POSSE_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("setor_posse_texto", item["SETOR_POSSE_TEXTO"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Protocolo

				#endregion

				bancoDeDados.Commit();
			}
		}
		#endregion

		#region Obter Dados UP

		public int ExisteUP(int tituloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select p.id from fat_propriedade p, dim_up_titulo t where p.id = t.fato and t.titulo_id = :tituloId and rownum = 1), 0) id from dual");

				comando.AdicionarParametroEntrada("tituloId", tituloId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public Dictionary<string, object> ObterUnidadeProducao(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct he.codigo,
						he.cnpj,
						hee.municipio_id,
						hee.municipio_texto,
						he.denominador         nome,
						hee.estado_id,
						hee.estado_texto,
						hup.propriedade_codigo codigo_propriedade,
						hup.local_livro
				from hst_titulo                     ht,
						tab_titulo_dependencia      ttd,
						hst_crt_unidade_producao    hup,
						hst_empreendimento          he,
						hst_empreendimento_endereco hee
				where ttd.titulo = ht.titulo_id
					and ttd.dependencia_caracterizacao = 23
					and hup.unidade_producao_id = ttd.dependencia_id
					and hup.tid = ttd.dependencia_tid   
					and ht.empreendimento_id = he.empreendimento_id   
					and ht.empreendimento_tid = he.tid   
					and hee.id_hst = he.id
					and hee.correspondencia = 0
					and ht.id = :idHstTitulo");//UP

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionary(comando);
			}
		}

		public List<Dictionary<string, object>> ObterProdutorUP(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct nvl(hp.cpf, hp.cnpj) cpf_cnpj_produtor,
						nvl(hp.nome, hp.razao_social) nome_razao_social_produtor,
						hpe.logradouro || ' ; ' || hpe.bairro || ' ; ' || hpe.numero || ' ; ' || hpe.distrito endereco,
						hpe.municipio_texto
				from tab_titulo_dependencia           ttd,
						hst_titulo                    ht,
						hst_crt_unidade_producao      hup,
						hst_crt_unidade_prod_unidade  hupu,
						hst_crt_unid_prod_un_produtor hupp,
						hst_pessoa                    hp,
						hst_pessoa_endereco           hpe
				where ttd.titulo = ht.titulo_id
					and ttd.titulo_modelo = ht.modelo_id
					and ttd.dependencia_caracterizacao = 23 /*UP*/
					and hup.unidade_producao_id = ttd.dependencia_id
					and hup.tid = ttd.dependencia_tid   
					and hupu.id_hst = hup.id   
					and hupp.id_hst = hupu.id   
					and hp.pessoa_id = hupp.produtor_id
					and hp.tid = hupp.produtor_tid
					and hpe.id_hst = hp.id
					and ht.id = :idHstTitulo");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterUnidadesUP(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct ht.id,
					hupu.codigo_up,
					hupu.tipo_producao_texto tipo_producao,
					hupu.renasem numero_renasem,
					hupu.renasem_data_validade data_validad_renasem,
					hupu.area,
					hc.texto cultura,
					hcc.cultivar_nome cultivar,
					hc.texto ||' ' || hcc.cultivar_nome cultura_cultivar,
					hupu.estimativa_quant_ano quantidade_ano,
					hupu.estimativa_unid_medida_id unidade_medida_id,
					hupu.estimativa_unid_medida_texto unidade_medida_texto,
					hupu.estimativa_quant_ano || '/'|| hupu.estimativa_unid_medida_texto quantidade_ano_unid_medida,
					hupu.ano_abertura,
					hupu.data_plantio_ano_producao data_plantio,
					hcupuc.easting_utm || ' - ' || hcupuc.northing_utm coordenadas_up
				from hst_titulo                    ht,
					tab_titulo_dependencia         ttd,
					hst_crt_unidade_producao       hup,
					hst_crt_Unidade_Prod_Unidade   hupu,
					hst_cultura                    hc,
					hst_cultura_cultivar           hcc,
					hst_crt_unidade_prod_un_coord  hcupuc
				where ttd.titulo = ht.titulo_id
					and ttd.titulo_modelo = ht.modelo_id
					and hup.unidade_producao_id = ttd.dependencia_id
					and hup.tid = ttd.dependencia_tid
					and ht.id = :idHstTitulo
					and ttd.dependencia_caracterizacao = 23 /*UP*/
					and hupu.id_hst = hup.id
					and hupu.cultura_tid = hc.tid(+)
					and hupu.cultura_id =  hc.cultura_id(+)
					and hupu.cultivar_id = hcc.cultivar_id(+)
					and hupu.cultivar_tid = hcc.tid(+)
					and hupu.id = hcupuc.id_hst");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterResponsaveisTecnicosUP(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct nvl(hps.nome, hps.razao_social) responsavel_tecnico,
								nvl(hps.cpf, hps.cnpj) cpf_cnpj,
								huprt.numero_hab_cfo_cfoc numero_habilitacao_cfo_cfoc,
								huprt.numero_art numero_art,
								huprt.data_validade_art data_validade_art
				from tab_titulo_dependencia          ttd,
						hst_titulo                   ht,
						hst_crt_unidade_producao     hup,
						hst_crt_unidade_prod_unidade hupu,
						hst_crt_un_prod_un_resp_tec  huprt,
						hst_credenciado              hcred,
						cre_hst_pessoa               hps
				where ttd.titulo = ht.titulo_id
					and ttd.dependencia_caracterizacao = 23 /*UP*/
					and ttd.titulo_modelo = ht.modelo_id
					and hup.unidade_producao_id = ttd.dependencia_id
					and hup.tid = ttd.dependencia_tid   
					and hupu.id_hst = hup.id
					and huprt.id_hst = hupu.id   
					and hcred.credenciado_id = huprt.responsavel_tecnico_id
					and hcred.tid = huprt.responsavel_tecnico_tid
					and hcred.pessoa_id = hps.pessoa_id
					and hcred.pessoa_tid = hps.tid
					and ht.id = :idHstTitulo");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		#endregion Obter Dados UP

		#region Obter Dados UC

		public int ExisteUC(int tituloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select p.id from fat_uc p, dim_uc_titulo t where p.id = t.fato and t.titulo_id = :id and rownum = 1), 0) id from dual");

				comando.AdicionarParametroEntrada("id", tituloId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public Dictionary<string, object> ObterUnidadeConsolidacao(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct uc.unidade_consolidacao uc_id,
                       uc.codigo_uc,
                       uc.local_livro_disponivel,
                       he.codigo empreendimento_codigo,
                       he.denominador empreend_denominador,
                       he.cnpj empreend_cnpj,
                       hee.municipio_id empreend_municipio_id,
                       hee.municipio_texto empreend_municipio_texto
                  from hst_titulo                   ht,
                       tab_titulo_dependencia       ttd,
                       hst_crt_unidade_consolidacao uc,
                       hst_empreendimento he,
                       hst_empreendimento_endereco hee
                 where ttd.titulo = ht.titulo_id
                   and ttd.titulo_modelo = ht.modelo_id
                   and ttd.dependencia_caracterizacao = 24
                   and uc.unidade_consolidacao = ttd.dependencia_id
                   and uc.tid = ttd.dependencia_tid
                   and ht.empreendimento_id = he.empreendimento_id 
                   and ht.empreendimento_tid = he.tid 
                   and he.id = hee.id_hst
                   and hee.correspondencia = 0
                   and ht.id = :idHstTitulo");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionary(comando);
			}
		}

		public List<Dictionary<string, object>> ObterCapacidadeArmazenamentoProcessamento(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				    select distinct ht.id, 
                        hc.cultura_id cultura_id,
                        hc.texto cultura_nome,
                        hcc.cultivar_id cultivar_id,
                        hcc.cultivar_nome cultivar_nome,
                        hc.texto || ' - ' || hcc.cultivar_nome cultura_cultivar,
                        ucc.capacidade_mes capacidade_mes,
                        ucc.unidade_medida_id unidade_medida_id,
                        ucc.unidade_medida_texto unidade_medida_texto,
                        ucc.capacidade_mes || ' ' || ucc.unidade_medida_texto capaci_mes_uni_medida,
                        uc.tipo_apresentacao_produto
                    from hst_titulo                    ht,
                        tab_titulo_dependencia        ttd,
                        hst_crt_unidade_consolidacao  uc,
                        hst_crt_unidade_cons_cultivar ucc,
                        hst_cultura                   hc,
                        hst_cultura_cultivar          hcc 
                    where ttd.titulo = ht.titulo_id
                        and ttd.titulo_modelo = ht.modelo_id
                        and ttd.dependencia_caracterizacao = 24
                        and uc.unidade_consolidacao = ttd.dependencia_id
                        and uc.tid = ttd.dependencia_tid
                        and ucc.id_hst = uc.id
                        and ucc.cultura_id = hc.cultura_id(+)
                        and ucc.cultura_tid = hc.tid(+)
                        and ucc.cultivar_id = hcc.cultivar_id(+)
                        and ucc.cultivar_tid = hcc.tid(+)
                        and ht.id = :idHstTitulo");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterResponsavelTecnicoUC(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				    select distinct nvl(chp.nome, chp.razao_social) nome_razao,
                           nvl(chp.cpf, chp.cnpj) cpf_cnpj,
                           ucrp.numero_hab_cfo_cfoc numero_hab_cfo_cfoc,
                           ucrp.numero_art numero_art,
                           ucrp.data_validade_art data_validade_art
                      from hst_titulo                   ht,
                           tab_titulo_dependencia       ttd,
                           hst_crt_unidade_consolidacao uc,
                           hst_crt_unida_conso_resp_tec ucrp,
                           hst_credenciado              hc,
                           cre_hst_pessoa               chp
                     where ttd.titulo = ht.titulo_id
                       and ttd.titulo_modelo = ht.modelo_id
                       and ttd.dependencia_caracterizacao = 24
                       and uc.unidade_consolidacao = ttd.dependencia_id
                       and uc.tid = ttd.dependencia_tid
                       and uc.id = ucrp.id_hst
                       and ucrp.responsavel_tecnico_id = hc.credenciado_id
                       and ucrp.responsavel_tecnico_tid = hc.tid
                       and hc.pessoa_id = chp.pessoa_id
                       and hc.pessoa_tid = chp.tid
                       and ht.id = :idHstTitulo");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterResponsavelEmpreendimentoUC(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				    select distinct ht.id,
                        nvl(hp.nome, hp.razao_social) nome_razao,
                        nvl(hp.cpf, hp.cnpj) cpf_cnpj,
                        (select (case when t.logradouro is null then '' else t.logradouro || ', ' end) || 
                                        (case when t.bairro is null then '' else t.bairro || ', ' end) || 
                                        (case when t.numero is null then '' else t.numero || ', ' end) || t.distrito
                            from hst_pessoa_endereco t
                            where t.id_hst = hp.id) endereco,
                        hpe.municipio_texto,
                        hpe.municipio_id
                    from hst_titulo                     ht,
                        hst_empreendimento             he,
                        hst_empreendimento_responsavel her,
                        hst_pessoa                     hp,
                        hst_pessoa_endereco            hpe
                    where ht.empreendimento_id = he.empreendimento_id
                    and ht.empreendimento_tid = he.tid
                    and he.id = her.id_hst
                    and her.responsavel_id = hp.pessoa_id
                    and her.responsavel_tid = hp.tid
                    and hpe.id_hst = hp.id
                    and ht.id = :idHstTitulo");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		#endregion Obter Dados UC

		#region Obter geral
		public List<Dictionary<string, object>> ObterTitulo(int hstTituloId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				    select htn.numero||'/'|| htn.ano numero_ano_titulo, 
                           ht.requerimento_id, 
                           ht.requerimento_tid,
                           ht.data_criacao data_cadastro,
                           ht.requerimento_id numero_requerimento,
                           ht.local_emissao_id local_emissao_titulo_id,
                           ht.local_emissao_texto local_emissao_titulo,
                           ht.situacao_id situacao_titulo_id,
                           ht.situacao_texto situacao_titulo,
                           ht.situacao_motivo_id MOTIVO_ID,
                           ht.situacao_motivo_texto MOTIVO_TEXTO,
                           ht.situacao_texto ||' - '||ht.situacao_motivo_texto situacao_motivo,
                           ht.titulo_id
                      from hst_titulo ht, hst_titulo_numero htn
                     where ht.id = htn.id_hst
                      and  ht.id = :idHstTitulo");

				comando.AdicionarParametroEntrada("idHstTitulo", hstTituloId, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterProtocolo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				            select p.numero,
                                   lp.id tipo_protocolo_id,
                                   lp.texto tipo_protocolo_texto,
                                   lt.id tipo_documento_id,
                                   lt.texto tipo_documento_texto,
                                   lp.texto || ' / ' || lt.texto tipo_protocolo_documento_texto,
                                   te.id SETOR_POSSE_ID,
                                   te.sigla SETOR_POSSE_TEXTO,
                                   (select tt.data_execucao
                                      from hst_tramitacao tt
                                     where tt.id = p.tramitacao) data_recebimento
                              from tab_protocolo      p,
                                   lov_protocolo      lp,
                                   lov_protocolo_tipo lt,
                                   tab_setor_endereco se,
                                   lov_municipio      m,
                                   lov_estado         e,
                                   tab_setor          te
                             where p.protocolo = lp.id
                               and p.tipo = lt.id
                               and p.setor_criacao = se.setor
                               and se.municipio = m.id
                               and se.estado = e.id
                               and se.setor = te.id
                               and p.id = (select t.protocolo_id from hst_titulo t where t.id = :id)");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		#endregion Obter geral
	}
}