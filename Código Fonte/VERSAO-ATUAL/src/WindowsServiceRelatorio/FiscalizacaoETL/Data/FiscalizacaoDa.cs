using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL.Entities;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;

namespace Tecnomapas.EtramiteX.WindowsService.FiscalizacaoETL.Data
{
	class FiscalizacaoDa
	{
		RelatorioDa _daRelatorio;
		ConfiguracaoSistema _configSys;

		public FiscalizacaoDa()
		{
			_configSys = new ConfiguracaoSistema();
			_daRelatorio = new RelatorioDa();
		}

		#region Ações de DML

		internal void Criar(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cria os dados da ETL de fiscalizacao

				Dictionary<string, object> fiscalizacao;

				Comando comando = bancoDeDados.CriarComando(@"insert into fat_fiscalizacao (id, numero, situacao_id, situacao_texto, cadastro_data, cadastro_ano, conclusao_data, conclusao_ano, agente_fiscal, 
					cad_setor_id, cad_setor_sigla, cad_setor_nome, resp_nome, resp_tipo_id, resp_tipo_texto, tid)values (:id, :numero, :situacao_id, :situacao_texto, :cadastro_data, :cadastro_ano, 
					:conclusao_data, :conclusao_ano, :agente_fiscal, :cad_setor_id, :cad_setor_sigla, :cad_setor_nome, :resp_nome, :resp_tipo_id, :resp_tipo_texto, :tid)");

				comando.AdicionarParametroEntrada("id", DbType.Int32);
				comando.AdicionarParametroEntrada("numero", DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_id", DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_texto", DbType.String);
				comando.AdicionarParametroEntrada("cadastro_data", DbType.DateTime);
				comando.AdicionarParametroEntrada("cadastro_ano", DbType.Int32);
				comando.AdicionarParametroEntrada("conclusao_data", DbType.DateTime);
				comando.AdicionarParametroEntrada("conclusao_ano", DbType.Int32);
				comando.AdicionarParametroEntrada("agente_fiscal", DbType.String);
				comando.AdicionarParametroEntrada("cad_setor_id", DbType.Int32);
				comando.AdicionarParametroEntrada("cad_setor_sigla", DbType.String);
				comando.AdicionarParametroEntrada("cad_setor_nome", DbType.String);
				comando.AdicionarParametroEntrada("resp_nome", DbType.String);
				comando.AdicionarParametroEntrada("resp_tipo_id", DbType.Int32);
				comando.AdicionarParametroEntrada("resp_tipo_texto", DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String);

				if (eleitos != null && eleitos.Count > 0)
				{
					bool flag = false;
					foreach (var pro in eleitos)
					{
						flag = Existe(Convert.ToInt32(pro["Id"]), bancoDeDados);
						if (!flag)
						{
							fiscalizacao = ObterFiscalizacao(Convert.ToInt32(pro["Id"]));
							if (fiscalizacao != null && fiscalizacao.Count > 0)
							{
								comando.SetarValorParametro("id", fiscalizacao["ID"]);
								comando.SetarValorParametro("numero", fiscalizacao["NUMERO"]);
								comando.SetarValorParametro("situacao_id", fiscalizacao["SITUACAO_ID"]);
								comando.SetarValorParametro("situacao_texto", fiscalizacao["SITUACAO_TEXTO"]);
								comando.SetarValorParametro("cadastro_data", fiscalizacao["CAD_SITUACAO_DATA"]);
								comando.SetarValorParametro("cadastro_ano", fiscalizacao["CAD_SITUACAO_ANO"]);
								comando.SetarValorParametro("conclusao_data", fiscalizacao["CON_SITUACAO_DATA"]);
								comando.SetarValorParametro("conclusao_ano", fiscalizacao["CON_SITUACAO_ANO"]);
								comando.SetarValorParametro("agente_fiscal", fiscalizacao["AGENTE_FISCAL"]);
								comando.SetarValorParametro("cad_setor_id", fiscalizacao["CAD_SETOR_ID"]);
								comando.SetarValorParametro("cad_setor_sigla", fiscalizacao["CAD_SETOR_SIGLA"]);
								comando.SetarValorParametro("cad_setor_nome", fiscalizacao["CAD_SETOR_TEXTO"]);
								comando.SetarValorParametro("resp_nome", fiscalizacao["RESP_NOME"]);
								comando.SetarValorParametro("resp_tipo_id", fiscalizacao["RESP_TIPO_ID"]);
								comando.SetarValorParametro("resp_tipo_texto", fiscalizacao["RESP_TIPO_TEXTO"]);
								comando.SetarValorParametro("tid", fiscalizacao["TID"]);

								bancoDeDados.ExecutarNonQuery(comando);

								Dimensoes(Convert.ToInt32(fiscalizacao["ID"]), fiscalizacao["TID"].ToString(), bancoDeDados);
							}
						}
					}
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Salvar(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cria os dados da ETL de fiscalizacao

				Dictionary<string, object> fiscalizacao;

				Comando comando = bancoDeDados.CriarComando(@"update fat_fiscalizacao f set  
					f.numero = :numero, 
					f.situacao_id = :situacao_id, 
					f.situacao_texto = :situacao_texto,
					f.cadastro_data = :cadastro_data, 
					f.cadastro_ano = :cadastro_ano,
					f.conclusao_data = :conclusao_data,
					f.conclusao_ano = :conclusao_ano,
					f.agente_fiscal = :agente_fiscal, 
					f.cad_setor_id = :cad_setor_id, 
					f.cad_setor_sigla = :cad_setor_sigla,
					f.cad_setor_nome = :cad_setor_nome,
					f.resp_nome = :resp_nome, 
					f.resp_tipo_id = :resp_tipo_id, 
					f.resp_tipo_texto = :resp_tipo_texto,
					f.tid = :tid
					where f.id = :id");

				comando.AdicionarParametroEntrada("id", DbType.Int32);
				comando.AdicionarParametroEntrada("numero", DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_id", DbType.Int32);
				comando.AdicionarParametroEntrada("situacao_texto", DbType.String);
				comando.AdicionarParametroEntrada("cadastro_data", DbType.DateTime);
				comando.AdicionarParametroEntrada("cadastro_ano", DbType.Int32);
				comando.AdicionarParametroEntrada("conclusao_data", DbType.DateTime);
				comando.AdicionarParametroEntrada("conclusao_ano", DbType.Int32);
				comando.AdicionarParametroEntrada("agente_fiscal", DbType.String);
				comando.AdicionarParametroEntrada("cad_setor_id", DbType.Int32);
				comando.AdicionarParametroEntrada("cad_setor_sigla", DbType.String);
				comando.AdicionarParametroEntrada("cad_setor_nome", DbType.String);
				comando.AdicionarParametroEntrada("resp_nome", DbType.String);
				comando.AdicionarParametroEntrada("resp_tipo_id", DbType.Int32);
				comando.AdicionarParametroEntrada("resp_tipo_texto", DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String);

				if (eleitos != null && eleitos.Count > 0)
				{
					bool flag = false;
					eleitos.ForEach(x =>
					{
						flag = Existe(Convert.ToInt32(x["Id"]), bancoDeDados);

						if (flag)
						{
							fiscalizacao = ObterFiscalizacao(Convert.ToInt32(x["Id"]));
							if (fiscalizacao != null && fiscalizacao.Count > 0)
							{
								comando.SetarValorParametro("id", fiscalizacao["ID"]);
								comando.SetarValorParametro("numero", fiscalizacao["NUMERO"]);
								comando.SetarValorParametro("situacao_id", fiscalizacao["SITUACAO_ID"]);
								comando.SetarValorParametro("situacao_texto", fiscalizacao["SITUACAO_TEXTO"]);
								comando.SetarValorParametro("cadastro_data", fiscalizacao["CAD_SITUACAO_DATA"]);
								comando.SetarValorParametro("cadastro_ano", fiscalizacao["CAD_SITUACAO_ANO"]);
								comando.SetarValorParametro("conclusao_data", fiscalizacao["CON_SITUACAO_DATA"]);
								comando.SetarValorParametro("conclusao_ano", fiscalizacao["CON_SITUACAO_ANO"]);
								comando.SetarValorParametro("agente_fiscal", fiscalizacao["AGENTE_FISCAL"]);
								comando.SetarValorParametro("cad_setor_id", fiscalizacao["CAD_SETOR_ID"]);
								comando.SetarValorParametro("cad_setor_sigla", fiscalizacao["CAD_SETOR_SIGLA"]);
								comando.SetarValorParametro("cad_setor_nome", fiscalizacao["CAD_SETOR_TEXTO"]);
								comando.SetarValorParametro("resp_nome", fiscalizacao["RESP_NOME"]);
								comando.SetarValorParametro("resp_tipo_id", fiscalizacao["RESP_TIPO_ID"]);
								comando.SetarValorParametro("resp_tipo_texto", fiscalizacao["RESP_TIPO_TEXTO"]);
								comando.SetarValorParametro("tid", fiscalizacao["TID"]);

								bancoDeDados.ExecutarNonQuery(comando);

								Dimensoes(Convert.ToInt32(fiscalizacao["ID"]), fiscalizacao["TID"].ToString(), bancoDeDados, true);

							}
						}
						else
						{
							Criar(new List<Dictionary<string, object>>() { x }, bancoDeDados);
						}
					});
				}

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Apaga os dados da ETL de processo

				List<String> lista = new List<string>();
				lista.Add("begin ");
				lista.Add("delete from dim_fisc_acompanhamento p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_auto_infracao p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_autuado p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_depositario p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_embargo_inter p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_empreendimento p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_enquadramento p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_infracao p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_material_apreen p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_posse p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_protocolo p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_pergunta p where p.fato = :fato_fisc;");
				lista.Add("delete from dim_fisc_campo p where p.fato = :fato_fisc;");
				lista.Add("delete from fat_fiscalizacao p where p.id = :fato_fisc;");
				lista.Add(" end;");

				Comando comando = bancoDeDados.CriarComando(String.Join(" ", lista));

				comando.AdicionarParametroEntrada("fato_fisc", DbType.Int32);

				eleitos.ForEach(x =>
				{
					comando.SetarValorParametro("fato_fisc", x["Id"]);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Dimensoes(int id, string tid, BancoDeDados banco = null, bool deletar = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;
				List<Dictionary<string, object>> aux;

				#region Apaga os dados da ETL das dimensões de fiscalizacao

				if (deletar)
				{
					List<String> lista = new List<string>();
					lista.Add("begin ");
					lista.Add("delete from dim_fisc_acompanhamento p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_auto_infracao p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_autuado p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_depositario p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_embargo_inter p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_empreendimento p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_enquadramento p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_infracao p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_material_apreen p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_posse p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_protocolo p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_pergunta p where p.fato = :fato_fisc;");
					lista.Add("delete from dim_fisc_campo p where p.fato = :fato_fisc;");
					lista.Add(" end;");

					comando = bancoDeDados.CriarComando(String.Join(" ", lista));
					comando.AdicionarParametroEntrada("fato_fisc", id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);

					bancoDeDados.Commit();
				}

				#endregion

				#region Cria os dados das dimensões de fiscalizacao

				#region Acompanhamento

				aux = ObterAcompanhamento(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_acompanhamento a(id, fato, numero_sufixo, vistoria_data, vistoria_ano, situacao_id, situacao_texto, agente_fiscal, tid)
					values (seq_dim_fisc_acompanhamento.nextval, :fato, :numero_sufixo, :vistoria_data, :vistoria_ano, :situacao_id, :situacao_texto, :agente_fiscal, :tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);
					comando.AdicionarParametroEntrada("numero_sufixo", DbType.String);
					comando.AdicionarParametroEntrada("vistoria_data", DbType.DateTime);
					comando.AdicionarParametroEntrada("vistoria_ano", DbType.Int32);
					comando.AdicionarParametroEntrada("situacao_id", DbType.Int32);
					comando.AdicionarParametroEntrada("situacao_texto", DbType.String);
					comando.AdicionarParametroEntrada("agente_fiscal", DbType.String);

					foreach (var acomp in aux)
					{
						comando.SetarValorParametro("numero_sufixo", acomp["NUMERO_SUFIXO"]);
						comando.SetarValorParametro("vistoria_data", acomp["VISTORIA_DATA"]);
						comando.SetarValorParametro("vistoria_ano", acomp["VISTORIA_ANO"]);
						comando.SetarValorParametro("situacao_id", acomp["SITUACAO_ID"]);
						comando.SetarValorParametro("situacao_texto", acomp["SITUACAO_TEXTO"]);
						comando.SetarValorParametro("agente_fiscal", acomp["AGENTE_FISCAL"]);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Auto Infracao

				aux = ObterAutoInfracao(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_auto_infracao(id, 
							fato, 
							numero, 
							serie_id, 
							serie_texto, 
							descricao_infracao, 
							codigo_receita_id, 
							codigo_receita_texto, 
							valor_multa, 
							lavratura_data, 
							lavratura_ano, 
							tid)
							values (
							seq_dim_fisc_auto_infracao.nextval, 
							:fato, 
							:numero, 
							:serie_id, 
							:serie_texto, 
							:descricao_infracao, 
							:codigo_receita_id, 
							:codigo_receita_texto, 
							:valor_multa, 
							:lavratura_data, 
							:lavratura_ano, 
							:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("numero", DbType.String);
					comando.AdicionarParametroEntrada("serie_id", DbType.Int32);
					comando.AdicionarParametroEntrada("serie_texto", DbType.String);
					comando.AdicionarParametroEntrada("descricao_infracao", DbType.String);
					comando.AdicionarParametroEntrada("codigo_receita_id", DbType.Int32);
					comando.AdicionarParametroEntrada("codigo_receita_texto", DbType.String);
					comando.AdicionarParametroEntrada("valor_multa", DbType.Decimal);
					comando.AdicionarParametroEntrada("lavratura_data", DbType.DateTime);
					comando.AdicionarParametroEntrada("lavratura_ano", DbType.Int32);

					foreach (var auto in aux)
					{
						comando.SetarValorParametro("numero", auto["NUMERO"]);
						comando.SetarValorParametro("serie_id", auto["SERIE_ID"]);
						comando.SetarValorParametro("serie_texto", auto["SERIE_TEXTO"]);
						comando.SetarValorParametro("descricao_infracao", auto["DESCRICAO_INFRACAO"]);
						comando.SetarValorParametro("codigo_receita_id", auto["CODIGO_RECEITA_ID"]);
						comando.SetarValorParametro("codigo_receita_texto", auto["CODIGO_RECEITA_TEXTO"]);
						comando.SetarValorParametro("valor_multa", auto["VALOR_MULTA"]);
						comando.SetarValorParametro("lavratura_data", auto["LAVRATURA_DATA"]);
						comando.SetarValorParametro("lavratura_ano", auto["LAVRATURA_ANO"]);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Autuado

				aux = ObterAutuado(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_autuado c(id, 
						fato, 
						autuado_id, 
						nome_razao, 
						cpf_cnpj, 
						vinc_prop_id, 
						vinc_prop_texto, 
						nivel_escola_id, 
						nivel_escola_texto, 
						municipio_id, 
						municipio_texto, 
						tid) values (seq_dim_fisc_autuado.nextval, 
						:fato, 
						:autuado_id, 
						:nome_razao, 
						:cpf_cnpj, 
						:vinc_prop_id, 
						:vinc_prop_texto, 
						:nivel_escola_id, 
						:nivel_escola_texto, 
						:municipio_id, 
						:municipio_texto, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("autuado_id", DbType.Int32);
					comando.AdicionarParametroEntrada("nome_razao", DbType.String);
					comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String);
					comando.AdicionarParametroEntrada("vinc_prop_id", DbType.Int32);
					comando.AdicionarParametroEntrada("vinc_prop_texto", DbType.String);
					comando.AdicionarParametroEntrada("nivel_escola_id", DbType.Int32);
					comando.AdicionarParametroEntrada("nivel_escola_texto", DbType.String);
					comando.AdicionarParametroEntrada("municipio_id", DbType.Int32);
					comando.AdicionarParametroEntrada("municipio_texto", DbType.String);

					foreach (var autuado in aux)
					{
						comando.SetarValorParametro("autuado_id", autuado["AUTUADO_ID"]);
						comando.SetarValorParametro("nome_razao", autuado["NOME_RAZAO"]);
						comando.SetarValorParametro("cpf_cnpj", autuado["CPF_CNPJ"]);
						comando.SetarValorParametro("vinc_prop_id", autuado["VINC_PROP_ID"]);
						comando.SetarValorParametro("vinc_prop_texto", autuado["VINC_PROP_TEXTO"]);
						comando.SetarValorParametro("nivel_escola_id", autuado["NIVEL_ESCOLA_ID"]);
						comando.SetarValorParametro("nivel_escola_texto", autuado["NIVEL_ESCOLA_TEXTO"]);
						comando.SetarValorParametro("municipio_id", autuado["MUNICIPIO_ID"]);
						comando.SetarValorParametro("municipio_texto", autuado["MUNICIPIO_TEXTO"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Depositario

				aux = ObterDepositario(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_depositario c(id, 
						fato, 
						nome_razao, 
						cpf_cnpj, 
						estado_id, 
						estado_texto, 
						municipio_id, 
						municipio_texto, 
						tid) values (seq_dim_fisc_depositario.nextval, 						
						:fato, 
						:nome_razao, 
						:cpf_cnpj, 
						:estado_id, 
						:estado_texto, 
						:municipio_id, 
						:municipio_texto, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("nome_razao", DbType.String);
					comando.AdicionarParametroEntrada("cpf_cnpj", DbType.String);
					comando.AdicionarParametroEntrada("estado_id", DbType.Int32);
					comando.AdicionarParametroEntrada("estado_texto", DbType.String);
					comando.AdicionarParametroEntrada("municipio_id", DbType.Int32);
					comando.AdicionarParametroEntrada("municipio_texto", DbType.String);

					foreach (var depositario in aux)
					{
						comando.SetarValorParametro("nome_razao", depositario["NOME_RAZAO"]);
						comando.SetarValorParametro("cpf_cnpj", depositario["CPF_CNPJ"]);
						comando.SetarValorParametro("estado_id", depositario["ESTADO_ID"]);
						comando.SetarValorParametro("estado_texto", depositario["ESTADO_TEXTO"]);
						comando.SetarValorParametro("municipio_id", depositario["MUNICIPIO_ID"]);
						comando.SetarValorParametro("municipio_texto", depositario["MUNICIPIO_TEXTO"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Embargo Interdicao

				aux = ObterEmbargoInterdicao(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_embargo_inter c(id, 
						fato, 
						numero, 
						serie_id, 
						serie_texto, 
						descricao_termo, 
						caracteristica_area_codigo, 
						caracteristica_area_texto, 
						declividade_media, 
						lavratura_data, 
						lavratura_ano, 
						tid) values (seq_dim_fisc_embargo_inter.nextval, 
						:fato, 
						:numero, 
						:serie_id, 
						:serie_texto, 
						:descricao_termo, 
						:caracteristica_area_codigo, 
						:caracteristica_area_texto, 
						:declividade_media, 
						:lavratura_data, 
						:lavratura_ano, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("numero", DbType.String);
					comando.AdicionarParametroEntrada("serie_id", DbType.Int32);
					comando.AdicionarParametroEntrada("serie_texto", DbType.String);
					comando.AdicionarParametroEntrada("descricao_termo", DbType.String);
					comando.AdicionarParametroEntrada("caracteristica_area_codigo", DbType.Int32);
					comando.AdicionarParametroEntrada("caracteristica_area_texto", DbType.String);
					comando.AdicionarParametroEntrada("declividade_media", DbType.Decimal);
					comando.AdicionarParametroEntrada("lavratura_data", DbType.DateTime);
					comando.AdicionarParametroEntrada("lavratura_ano", DbType.Int32);


					foreach (var embargo in aux)
					{
						comando.SetarValorParametro("numero", embargo["NUMERO"]);
						comando.SetarValorParametro("serie_id", embargo["SERIE_ID"]);
						comando.SetarValorParametro("serie_texto", embargo["SERIE_TEXTO"]);
						comando.SetarValorParametro("descricao_termo", embargo["DESCRICAO_TERMO"]);
						comando.SetarValorParametro("caracteristica_area_codigo", embargo["CARACTERISTICA_AREA_CODIGO"]);
						comando.SetarValorParametro("caracteristica_area_texto", embargo["CARACTERISTICA_AREA_TEXTO"]);
						comando.SetarValorParametro("declividade_media", embargo["DECLIVIDADE_MEDIA"]);
						comando.SetarValorParametro("lavratura_data", embargo["LAVRATURA_DATA"]);
						comando.SetarValorParametro("lavratura_ano", embargo["LAVRATURA_ANO"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Empreendimento

				aux = ObterEmpreendimento(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_empreendimento c(id, 
						fato, 
						denominador, 
						cnpj, 
						segmento_id, 
						segmento_texto, 
						zona_id, 
						zona_texto, 
						local_estado_id, 
						local_estado_texto, 
						local_municipio_id, 
						local_municipio_texto, 
						corresp_estado_id, 
						corresp_estado_texto, 
						corresp_municipio_id, 
						corresp_municipio_texto, 
						coordenadas, 
						area_total, 
						area_floresta_nativa, 
						tid
						) values (seq_dim_fisc_empreendimento.nextval, 
						:fato, 
						:denominador, 
						:cnpj, 
						:segmento_id, 
						:segmento_texto, 
						:zona_id, 
						:zona_texto, 
						:local_estado_id, 
						:local_estado_texto, 
						:local_municipio_id, 
						:local_municipio_texto, 
						:corresp_estado_id, 
						:corresp_estado_texto, 
						:corresp_municipio_id, 
						:corresp_municipio_texto, 
						:coordenadas, 
						:area_total, 
						:area_floresta_nativa, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("denominador", DbType.String);
					comando.AdicionarParametroEntrada("cnpj", DbType.String);
					comando.AdicionarParametroEntrada("segmento_id", DbType.Int32);
					comando.AdicionarParametroEntrada("segmento_texto", DbType.String);
					comando.AdicionarParametroEntrada("zona_id", DbType.Int32);
					comando.AdicionarParametroEntrada("zona_texto", DbType.String);
					comando.AdicionarParametroEntrada("local_estado_id", DbType.Int32);
					comando.AdicionarParametroEntrada("local_estado_texto", DbType.String);
					comando.AdicionarParametroEntrada("local_municipio_id", DbType.Int32);
					comando.AdicionarParametroEntrada("local_municipio_texto", DbType.String);
					comando.AdicionarParametroEntrada("corresp_estado_id", DbType.Int32);
					comando.AdicionarParametroEntrada("corresp_estado_texto", DbType.String);
					comando.AdicionarParametroEntrada("corresp_municipio_id", DbType.Int32);
					comando.AdicionarParametroEntrada("corresp_municipio_texto", DbType.String);
					comando.AdicionarParametroEntrada("coordenadas", DbType.String);
					comando.AdicionarParametroEntrada("area_total", DbType.Decimal);
					comando.AdicionarParametroEntrada("area_floresta_nativa", DbType.Decimal);


					foreach (var emp in aux)
					{
						comando.SetarValorParametro("denominador", emp["DENOMINADOR"]);
						comando.SetarValorParametro("cnpj", emp["CNPJ"]);
						comando.SetarValorParametro("segmento_id", emp["SEGMENTO_ID"]);
						comando.SetarValorParametro("segmento_texto", emp["SEGMENTO_TEXTO"]);
						comando.SetarValorParametro("zona_id", emp["ZONA_ID"]);
						comando.SetarValorParametro("zona_texto", emp["ZONA_TEXTO"]);
						comando.SetarValorParametro("local_estado_id", emp["LOCAL_ESTADO_ID"]);
						comando.SetarValorParametro("local_estado_texto", emp["LOCAL_ESTADO_TEXTO"]);
						comando.SetarValorParametro("local_municipio_id", emp["LOCAL_MUNICIPIO_ID"]);
						comando.SetarValorParametro("local_municipio_texto", emp["LOCAL_MUNICIPIO_TEXTO"]);
						comando.SetarValorParametro("corresp_estado_id", emp["CORRESP_ESTADO_ID"]);
						comando.SetarValorParametro("corresp_estado_texto", emp["CORRESP_ESTADO_TEXTO"]);
						comando.SetarValorParametro("corresp_municipio_id", emp["CORRESP_MUNICIPIO_ID"]);
						comando.SetarValorParametro("corresp_municipio_texto", emp["CORRESP_MUNICIPIO_TEXTO"]);
						comando.SetarValorParametro("coordenadas", emp["COORDENADAS"]);
						comando.SetarValorParametro("area_total", emp["AREA_TOTAL"]);
						comando.SetarValorParametro("area_floresta_nativa", emp["AREA_FLORESTA_NATIVA"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Enquadramento

				aux = ObterEnquadramento(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_enquadramento c(id, 
						fato, 
						artigo, 
						artigo_paragrafo, 
						combinado_artigo, 
						combinado_artigo_paragrafo, 
						norma_legal, 
						tid) values (seq_dim_fisc_enquadramento.nextval, 
						:fato, 
						:artigo, 
						:artigo_paragrafo, 
						:combinado_artigo, 
						:combinado_artigo_paragrafo, 
						:norma_legal, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);


					comando.AdicionarParametroEntrada("artigo", DbType.String);
					comando.AdicionarParametroEntrada("artigo_paragrafo", DbType.String);
					comando.AdicionarParametroEntrada("combinado_artigo", DbType.String);
					comando.AdicionarParametroEntrada("combinado_artigo_paragrafo", DbType.String);
					comando.AdicionarParametroEntrada("norma_legal", DbType.String);

					foreach (var enq in aux)
					{
						comando.SetarValorParametro("artigo", enq["ARTIGO"]);
						comando.SetarValorParametro("artigo_paragrafo", enq["ARTIGO_PARAGRAFO"]);
						comando.SetarValorParametro("combinado_artigo", enq["COMBINADO_ARTIGO"]);
						comando.SetarValorParametro("combinado_artigo_paragrafo", enq["COMBINADO_ARTIGO_PARAGRAFO"]);
						comando.SetarValorParametro("norma_legal", enq["NORMA_LEGAL"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Infracao

				aux = ObterInfracao(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_infracao c(id, 
						fato, 
						vistoria_data, 
						vistoria_ano, 
						local, 
						local_estado_id, 
						local_estado_texto, 
						local_municipio_id, 
						local_municipio_texto, 
						coordenadas, 
						classificacao_id, 
						classificacao_texto, 
						tipo_id, 
						tipo_texto, 
						item_id, 
						item_texto, 
						subitem_id, 
						subitem_texto, 
						tid) values (seq_dim_fisc_infracao.nextval, 
						:fato, 
						:vistoria_data, 
						:vistoria_ano, 
						:local, 
						:local_estado_id, 
						:local_estado_texto, 
						:local_municipio_id, 
						:local_municipio_texto, 
						:coordenadas, 
						:classificacao_id, 
						:classificacao_texto, 
						:tipo_id, 
						:tipo_texto, 
						:item_id, 
						:item_texto, 
						:subitem_id, 
						:subitem_texto, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("vistoria_data", DbType.DateTime);
					comando.AdicionarParametroEntrada("vistoria_ano", DbType.Int32);
					comando.AdicionarParametroEntrada("local", DbType.String);
					comando.AdicionarParametroEntrada("local_estado_id", DbType.Int32);
					comando.AdicionarParametroEntrada("local_estado_texto", DbType.String);
					comando.AdicionarParametroEntrada("local_municipio_id", DbType.Int32);
					comando.AdicionarParametroEntrada("local_municipio_texto", DbType.String);
					comando.AdicionarParametroEntrada("coordenadas", DbType.String);
					comando.AdicionarParametroEntrada("classificacao_id", DbType.Int32);
					comando.AdicionarParametroEntrada("classificacao_texto", DbType.String);
					comando.AdicionarParametroEntrada("tipo_id", DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_texto", DbType.String);
					comando.AdicionarParametroEntrada("item_id", DbType.Int32);
					comando.AdicionarParametroEntrada("item_texto", DbType.String);
					comando.AdicionarParametroEntrada("subitem_id", DbType.Int32);
					comando.AdicionarParametroEntrada("subitem_texto", DbType.String);

					foreach (var inf in aux)
					{
						comando.SetarValorParametro("vistoria_data", inf["VISTORIA_DATA"]);
						comando.SetarValorParametro("vistoria_ano", inf["VISTORIA_ANO"]);
						comando.SetarValorParametro("local", inf["LOCAL"]);
						comando.SetarValorParametro("local_estado_id", inf["LOCAL_ESTADO_ID"]);
						comando.SetarValorParametro("local_estado_texto", inf["LOCAL_ESTADO_TEXTO"]);
						comando.SetarValorParametro("local_municipio_id", inf["LOCAL_MUNICIPIO_ID"]);
						comando.SetarValorParametro("local_municipio_texto", inf["LOCAL_MUNICIPIO_TEXTO"]);
						comando.SetarValorParametro("coordenadas", inf["COORDENADAS"]);
						comando.SetarValorParametro("classificacao_id", inf["CLASSIFICACAO_ID"]);
						comando.SetarValorParametro("classificacao_texto", inf["CLASSIFICACAO_TEXTO"]);
						comando.SetarValorParametro("tipo_id", inf["TIPO_ID"]);
						comando.SetarValorParametro("tipo_texto", inf["TIPO_TEXTO"]);
						comando.SetarValorParametro("item_id", inf["ITEM_ID"]);
						comando.SetarValorParametro("item_texto", inf["ITEM_TEXTO"]);
						comando.SetarValorParametro("subitem_id", inf["SUBITEM_ID"]);
						comando.SetarValorParametro("subitem_texto", inf["SUBITEM_TEXTO"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Material Apreendido

				aux = ObterMaterialApreendido(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_material_apreen c(id, 
						fato, 
						numero, 
						serie_id, 
						serie_texto, 
						descricao_apreen, 
						valor_produto, 
						tipo_material_id, 
						tipo_material_texto, 
						especific_material, 
						lavratura_data, 
						lavratura_ano, 
						tid) values (seq_dim_fisc_material_apreen.nextval, 
						:fato, 
						:numero, 
						:serie_id, 
						:serie_texto, 
						:descricao_apreen, 
						:valor_produto, 
						:tipo_material_id, 
						:tipo_material_texto, 
						:especific_material, 
						:lavratura_data, 
						:lavratura_ano, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("numero", DbType.String);
					comando.AdicionarParametroEntrada("serie_id", DbType.Int32);
					comando.AdicionarParametroEntrada("serie_texto", DbType.String);
					comando.AdicionarParametroEntrada("descricao_apreen", DbType.String);
					comando.AdicionarParametroEntrada("valor_produto", DbType.Decimal);
					comando.AdicionarParametroEntrada("tipo_material_id", DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_material_texto", DbType.String);
					comando.AdicionarParametroEntrada("especific_material", DbType.String);
					comando.AdicionarParametroEntrada("lavratura_data", DbType.DateTime);
					comando.AdicionarParametroEntrada("lavratura_ano", DbType.Int32);

					foreach (var mat in aux)
					{
						comando.SetarValorParametro("numero", mat["NUMERO"]);
						comando.SetarValorParametro("serie_id", mat["SERIE_ID"]);
						comando.SetarValorParametro("serie_texto", mat["SERIE_TEXTO"]);
						comando.SetarValorParametro("descricao_apreen", mat["DESCRICAO_APREEN"]);
						comando.SetarValorParametro("valor_produto", mat["VALOR_PRODUTO"]);
						comando.SetarValorParametro("tipo_material_id", mat["TIPO_MATERIAL_ID"]);
						comando.SetarValorParametro("tipo_material_texto", mat["TIPO_MATERIAL_TEXTO"]);
						comando.SetarValorParametro("especific_material", mat["ESPECIFIC_MATERIAL"]);
						comando.SetarValorParametro("lavratura_data", mat["LAVRATURA_DATA"]);
						comando.SetarValorParametro("lavratura_ano", mat["LAVRATURA_ANO"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Posse

				aux = ObterPosse(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_posse c(id, 
						fato, 
						funcionario, 
						setor_id, 
						setor_sigla, 
						setor_nome, 
						tid) values (seq_dim_fisc_posse.nextval, 
						:fato, 
						:funcionario, 
						:setor_id, 
						:setor_sigla, 
						:setor_nome, 
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("funcionario", DbType.String);
					comando.AdicionarParametroEntrada("setor_id", DbType.Int32);
					comando.AdicionarParametroEntrada("setor_sigla", DbType.String);
					comando.AdicionarParametroEntrada("setor_nome", DbType.String);

					foreach (var posse in aux)
					{
						comando.SetarValorParametro("funcionario", posse["FUNCIONARIO"]);
						comando.SetarValorParametro("setor_id", posse["SETOR_ID"]);
						comando.SetarValorParametro("setor_sigla", posse["SETOR_SIGLA"]);
						comando.SetarValorParametro("setor_nome", posse["SETOR_NOME"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Protocolo

				aux = ObterProtocolo(id);

				if (aux != null && aux.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_fisc_protocolo c(id, 
						fato, 
						numero, 
						ano, 
						numero_completo, 
						tipo_id, 
						tipo_texto, 
						autuacao_sep_data, 
						autuacao_sep_ano, 
						autuacao_sep_numero, 
						cadastro_data, 
						cadastro_ano, 
						cadastro_setor_id,
						cadastro_setor_nome,
						cadastro_setor_sigla,
						tid) values (seq_dim_fisc_protocolo.nextval, 
						:fato, 
						:numero, 
						:ano, 
						:numero_completo, 
						:tipo_id, 
						:tipo_texto, 
						:autuacao_sep_data, 
						:autuacao_sep_ano, 
						:autuacao_sep_numero, 
						:cadastro_data, 
						:cadastro_ano, 
						:cadastro_setor_id,
						:cadastro_setor_nome,
						:cadastro_setor_sigla,
						:tid)");

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					comando.AdicionarParametroEntrada("numero", DbType.Int32);
					comando.AdicionarParametroEntrada("ano", DbType.Int32);
					comando.AdicionarParametroEntrada("numero_completo", DbType.String);
					comando.AdicionarParametroEntrada("tipo_id", DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_texto", DbType.String);
					comando.AdicionarParametroEntrada("autuacao_sep_data", DbType.DateTime);
					comando.AdicionarParametroEntrada("autuacao_sep_ano", DbType.Int32);
					comando.AdicionarParametroEntrada("autuacao_sep_numero", DbType.String);
					comando.AdicionarParametroEntrada("cadastro_data", DbType.DateTime);
					comando.AdicionarParametroEntrada("cadastro_ano", DbType.Int32);
					comando.AdicionarParametroEntrada("cadastro_setor_id", DbType.Int32);
					comando.AdicionarParametroEntrada("cadastro_setor_nome", DbType.String);
					comando.AdicionarParametroEntrada("cadastro_setor_sigla", DbType.String);


					foreach (var proto in aux)
					{
						comando.SetarValorParametro("numero", proto["NUMERO"]);
						comando.SetarValorParametro("ano", proto["ANO"]);
						comando.SetarValorParametro("numero_completo", proto["NUMERO_COMPLETO"]);
						comando.SetarValorParametro("tipo_id", proto["TIPO_ID"]);
						comando.SetarValorParametro("tipo_texto", proto["TIPO_TEXTO"]);
						comando.SetarValorParametro("autuacao_sep_data", proto["AUTUACAO_SEP_DATA"]);
						comando.SetarValorParametro("autuacao_sep_ano", proto["AUTUACAO_SEP_ANO"]);
						comando.SetarValorParametro("autuacao_sep_numero", proto["AUTUACAO_SEP_NUMERO"]);
						comando.SetarValorParametro("cadastro_data", proto["CADASTRO_DATA"]);
						comando.SetarValorParametro("cadastro_ano", proto["CADASTRO_ANO"]);
						comando.SetarValorParametro("cadastro_setor_id", proto["CADASTRO_SETOR_ID"]);
						comando.SetarValorParametro("cadastro_setor_nome", proto["CADASTRO_SETOR_NOME"]);
						comando.SetarValorParametro("cadastro_setor_sigla", proto["CADASTRO_SETOR_SIGLA"]);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Perguntas

				aux = ObterPergunta(id);

				if (aux != null && aux.Count > 0)
				{
					SqlBuilderDa sqlBuilder = new SqlBuilderDa("dim_fisc_pergunta", "seq_dim_fisc_pergunta");

					comando = bancoDeDados.CriarComando(sqlBuilder.ObterInsert());

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					List<ColunaOracle> lstColunas = sqlBuilder.ObterColunas();
					lstColunas.RemoveAll(x => x.ColumnName == "ID" || x.ColumnName == "FATO" || x.ColumnName == "TID");

					foreach (var col in lstColunas)
					{
						var pergunta = aux.FirstOrDefault(x => string.Format("P_TXT_{0}", x["PERGUNTA_ID"]) == col.ColumnName);
						if (pergunta != null)
						{
							comando.AdicionarParametroEntrada(col.ColumnName, DbType.String, 100, pergunta["PERGUNTA_TEXTO"]);
							comando.AdicionarParametroEntrada(string.Format("P_TID_{0}", pergunta["PERGUNTA_ID"]), DbType.String, 36, pergunta["PERGUNTA_TID"]);

							comando.AdicionarParametroEntrada(string.Format("P_RES_ID_{0}", pergunta["PERGUNTA_ID"]), pergunta["RESPOSTA_ID"], DbType.Int32);
							comando.AdicionarParametroEntrada(string.Format("P_RES_TXT_{0}", pergunta["PERGUNTA_ID"]), DbType.String, 100, pergunta["RESPOSTA_TEXTO"]);
							comando.AdicionarParametroEntrada(string.Format("P_RES_TID_{0}", pergunta["PERGUNTA_ID"]), DbType.String, 36, pergunta["RESPOSTA_TID"]);
						}
					}

					foreach (var col in lstColunas.Where(x => comando.DbCommand.Parameters.IndexOf(x.ColumnName) < 0))
					{
						comando.AdicionarParametroEntrada(col.ColumnName, DBNull.Value, DbType.String);
					}

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Campos

				aux = ObterCampo(id);

				if (aux != null && aux.Count > 0)
				{
					SqlBuilderDa sqlBuilder = new SqlBuilderDa("dim_fisc_campo", "seq_dim_fisc_campo");

					comando = bancoDeDados.CriarComando(sqlBuilder.ObterInsert());

					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", tid, DbType.String);

					List<ColunaOracle> lstColunas = sqlBuilder.ObterColunas();
					lstColunas.RemoveAll(x => x.ColumnName == "ID" || x.ColumnName == "FATO" || x.ColumnName == "TID");

					foreach (var col in lstColunas)
					{
						var campo = aux.FirstOrDefault(x => string.Format("C_VAL_{0}", x["CAMPO"]) == col.ColumnName);
						if (campo != null)
						{
							comando.AdicionarParametroEntrada(col.ColumnName, campo["CAMPO"], DbType.Int32);

							if (Convert.ToInt32(campo["TIPO"]) == 1)
							{
								comando.AdicionarParametroEntrada(string.Format("C_VAL_{0}", campo["CAMPO"]), DbType.String, 100, campo["TEXTO"]);
							}
							else
							{
								comando.AdicionarParametroEntrada(string.Format("C_VAL_{0}", campo["CAMPO"]), campo["VALOR_NUM"], DbType.Decimal);
							}
						}
					}

					foreach (var col in lstColunas.Where(x => comando.DbCommand.Parameters.IndexOf(x.ColumnName) < 0))
					{
						comando.AdicionarParametroEntrada(col.ColumnName, DBNull.Value, DbType.String);
					}

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		public List<Dictionary<string, object>> Eleitos(DateTime execucaoInicio, BancoDeDados banco = null)
		{
			List<Dictionary<string, object>> retorno = new List<Dictionary<string, object>>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select distinct t.id, (case when t.acao != 3 then null else 3 end) acao from (
				select h.fiscalizacao_id id, (select a.acao from lov_historico_artefatos_acoes a where a.id = h.acao_executada) acao 
				from hst_fiscalizacao h where h.data_execucao > :execucao_inicio and h.acao_executada in (279, 280, 281, 282, 301) order by h.data_execucao) t");

				comando.AdicionarParametroEntrada("execucao_inicio", execucaoInicio, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dictionary<string, object> eleito;
					while (reader.Read())
					{
						eleito = new Dictionary<string, object>();
						eleito.Add("Id", reader.GetValue<int>("id"));
						eleito.Add("Acao", reader.GetValue<int>("acao"));
						retorno.Add(eleito);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		public bool Existe(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from fat_fiscalizacao p where p.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public Dictionary<string, object> ObterFiscalizacao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select f.id, f.id numero, f.situacao situacao_id, fs.texto situacao_texto, 
					cad.cad_situacao_data, cad.cad_situacao_ano, cad.setor_id cad_setor_id, cad.setor_texto cad_setor_texto, cad.setor_sigla cad_setor_sigla,
					con.con_situacao_data, con.con_situacao_ano, 
					( select fun.nome from tab_funcionario fun where fun.id = f.autuante ) agente_fiscal, p.nome resp_nome, 
					( select tr.id from tab_empreendimento_responsavel r, lov_empreendimento_tipo_resp tr 
					where r.empreendimento = linf.empreendimento and r.responsavel = linf.resp_propriedade and r.tipo = tr.id )resp_tipo_id,
					( select tr.texto from tab_empreendimento_responsavel r, lov_empreendimento_tipo_resp tr 
					where r.empreendimento = linf.empreendimento and r.responsavel = linf.resp_propriedade and r.tipo = tr.id )resp_tipo_texto,
					f.tid
					from tab_fiscalizacao f, lov_fiscalizacao_situacao fs, tab_fisc_local_infracao linf, tab_pessoa p,
					( select h.fiscalizacao_id, min(h.situacao_data) cad_situacao_data, to_char(min(h.situacao_data), 'yyyy') cad_situacao_ano, 
							loc.setor_id, loc.setor_texto, loc.setor_sigla 
					from hst_fiscalizacao h, hst_fisc_local_infracao loc 
					where h.acao_executada = 280  
					and h.id = loc.id_hst 
					group by h.fiscalizacao_id, loc.setor_id, loc.setor_texto,loc.setor_sigla ) cad, 
					(  select h.fiscalizacao_id, max(h.situacao_data) con_situacao_data, to_char(max(h.situacao_data), 'yyyy') con_situacao_ano 
					from hst_fiscalizacao h where h.situacao_id = 2 group by h.fiscalizacao_id ) con 
					where f.situacao = fs.id
					and f.id = linf.fiscalizacao
					and linf.resp_propriedade = p.id(+)
					and f.id = cad.fiscalizacao_id
					and f.id = con.fiscalizacao_id
					and f.situacao != 1
					and f.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionary(comando);
			}
		}

		public List<Dictionary<string, object>> ObterAcompanhamento(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.numero_sufixo,  
					a.data_vistoria vistoria_data, 
					extract(YEAR from a.data_vistoria) vistoria_ano, 
					sf.id situacao_id, 
					sf.texto situacao_texto, 
					ag.nome agente_fiscal
					 from tab_acompanhamento_fisc a, tab_funcionario ag, lov_acomp_fisc_situacao sf where 
					a.agente_fiscal = ag.id
					and a.situacao = sf.id
					and a.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterAutoInfracao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select 
					case 
						when fi.infracao_autuada = 1 and fi.gerado_sistema = 0 then fi.numero_auto_infracao_bloco 
						when fi.infracao_autuada = 1 and fi.gerado_sistema = 1 then to_char(f.autos)
					else 
						null 
					end 
					numero,
					s.id  serie_id,
					s.texto serie_texto,
					c.id codigo_receita_id,
					c.texto codigo_receita_texto,
					fi.descricao_infracao,
					fi.valor_multa,
					case 
						when fi.infracao_autuada = 1 and fi.gerado_sistema = 0 then fi.data_lavratura_auto 
						when fi.infracao_autuada = 1 and fi.gerado_sistema = 1 then 
						(select h.situacao_data from hst_fiscalizacao h where h.situacao_id = 2/*Cadastro Concluido*/ and h.fiscalizacao_id = :id
							and h.data_execucao = (select max(data_execucao) from hst_fiscalizacao where situacao_id = 2/*Cadastro Concluido*/ and fiscalizacao_id = :id))
					else 
						null 
					end lavratura_data,
					case 
						when fi.infracao_autuada = 1 and fi.gerado_sistema = 0 then extract(YEAR from fi.data_lavratura_auto) 
						when fi.infracao_autuada = 1 and fi.gerado_sistema = 1 then 
						(select extract(YEAR from h.situacao_data) from hst_fiscalizacao h where h.situacao_id = 2/*Cadastro Concluido*/ and h.fiscalizacao_id = :id
							and h.data_execucao = (select max(data_execucao) from hst_fiscalizacao where situacao_id = 2/*Cadastro Concluido*/ and fiscalizacao_id = :id))
					 else 
						null 
					 end lavratura_ano
				from tab_fiscalizacao f , tab_fisc_infracao fi, lov_fiscalizacao_serie s, lov_fisc_infracao_codigo_rece c
				where fi.serie = s.id(+)
				and fi.codigo_receita = c.id(+)
				and f.id = fi.fiscalizacao
				and fi.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterAutuado(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select a.responsavel autuado_id, nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj, a.vinc_prop vinc_prop_id, 
					(case when a.vinc_prop <> 9 then ltr.texto else a.vinc_prop_especif_text end) vinc_prop_texto,
					n.id nivel_escola_id, n.texto nivel_escola_texto, pm.municipio_id, pm.municipio_texto
					from tab_fisc_compl_dados_aut a, tab_pessoa p, lov_empreendimento_tipo_resp ltr, lov_fisc_compl_dad_nivel_escol n, 
					(select pe.pessoa, lm.id municipio_id, lm.texto municipio_texto from tab_pessoa_endereco pe, lov_municipio lm where pe.municipio = lm.id) pm
					where a.responsavel = p.id 
					and a.vinc_prop = ltr.id
					and a.nivel_escolaridade = n.id
					and p.id = pm.pessoa(+)
					and a.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterDepositario(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select
					nvl(p.nome, p.razao_social) nome_razao, 
					nvl(p.cpf, p.cnpj) cpf_cnpj, 
					e.id estado_id, 
					e.sigla estado_texto, 
					m.id municipio_id, 
					m.texto municipio_texto
					from tab_fiscalizacao f, tab_fisc_material_apreendido fm, tab_pessoa p, lov_estado e, lov_municipio m 
					where f.id = fm.fiscalizacao 
					and fm.depositario = p.id
					and fm.endereco_estado = e.id
					and fm.endereco_municipio = m.id
					and f.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterEmbargoInterdicao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select 
					case 
						when foi.area_embargada_atv_intermed = 1 and foi.tei_gerado_pelo_sist = 0 then foi.num_tei_bloco 
						when foi.area_embargada_atv_intermed = 1 and foi.tei_gerado_pelo_sist = 1 then f.autos
					else 
						null 
					end 
					numero,
					s.id  serie_id,
					s.texto serie_texto,
					foi.desc_termo_embargo descricao_termo,
					foi.caract_solo_area_danif caracteristica_area_codigo,
					( select stragg(c.texto) from lov_fisc_obj_infra_carac_solo c where bitand( foi.caract_solo_area_danif, c.codigo ) <> 0) caracteristica_area_texto,
					foi.declividade_media_area declividade_media,
					case 
						when foi.area_embargada_atv_intermed = 1 and foi.tei_gerado_pelo_sist = 0 then foi.data_lavratura_termo 
						when foi.area_embargada_atv_intermed = 1 and foi.tei_gerado_pelo_sist = 1 then 
						(select h.situacao_data from hst_fiscalizacao h where h.situacao_id = 2/*Cadastro Concluido*/ and h.fiscalizacao_id = :id
							and h.data_execucao = (select max(data_execucao) from hst_fiscalizacao where situacao_id = 2/*Cadastro Concluido*/ and fiscalizacao_id = :id))
					 else 
						null 
					end lavratura_data,
					case 
						when foi.area_embargada_atv_intermed = 1 and foi.tei_gerado_pelo_sist = 0 then extract(YEAR from foi.data_lavratura_termo) 
						when foi.area_embargada_atv_intermed = 1 and foi.tei_gerado_pelo_sist = 1 then 
						(select extract(YEAR from h.situacao_data) from hst_fiscalizacao h where h.situacao_id = 2/*Cadastro Concluido*/ and h.fiscalizacao_id = :id
							and h.data_execucao = (select max(data_execucao) from hst_fiscalizacao where situacao_id = 2/*Cadastro Concluido*/ and fiscalizacao_id = :id))
					else 
						null 
					end lavratura_ano
				from tab_fiscalizacao f , tab_fisc_obj_infracao foi, lov_fiscalizacao_serie s
					where foi.tei_gerado_pelo_sist_serie = s.id(+)
					and f.id = foi.fiscalizacao
					and foi.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterEmpreendimento(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select 
					e.denominador, 
					e.cnpj, 
					s.id segmento_id, 
					s.texto segmento_texto, 
					endEmp.Zona zona_id, 
					(case when endEmp.Zona = 1 then 'Urbana' else 'Rural' end) zona_texto, 
					endEmp.Estado local_estado_id, 
					locEst.Sigla local_estado_texto, 
					endEmp.Municipio local_municipio_id, 
					locMun.Texto local_municipio_texto, 
					endCo.Estado corresp_estado_id, 
					endCo.Sigla corresp_estado_texto, 
					endco.mun_id corresp_municipio_id, 
					endCo.Mun_Texto corresp_municipio_texto, 
					(select  'Tipo Coord: '||(select lp.texto from 
									lov_coordenada_tipo lp where c.tipo_coordenada = lp.id)||', Easting: '||c.easting_utm||', Northing: '||c.northing_utm  from tab_empreendimento_coord c 
									where c.empreendimento = e.id) coordenadas , 
					fcom.prop_area_total area_total, 
					fcom.prop_area_cobert_flores_nativ area_floresta_nativa      
					from tab_fisc_local_infracao l, 
					tab_fisc_compl_dados_aut fcom,
					tab_empreendimento e, 
					lov_empreendimento_segmento s, 
					tab_empreendimento_endereco endEmp, 
					lov_estado locEst,
					lov_municipio locMun, 
					( select ec.empreendimento, ec.estado, le.sigla, lm.id mun_id, lm.texto mun_texto
					   from tab_empreendimento_endereco ec, lov_estado le, lov_municipio lm 
					   where ec.estado = le.id and ec.municipio = lm.id and ec.correspondencia = 1 ) endCo   
					where l.empreendimento = e.id
					and l.fiscalizacao = fcom.fiscalizacao
					and e.segmento = s.id
					and e.id = endEmp.Empreendimento
					and endEmp.estado = locEst.Id
					and endEmp.municipio = locMun.Id
					and e.id = endCo.Empreendimento(+)
					and l.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterEnquadramento(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select  
					ea.artigo,
					ea.artigo_paragrafo,
					ea.combinado_artigo,
					ea.combinado_artigo_paragrafo,
					ea.da_do norma_legal
					from tab_fisc_enquadramento e, tab_fisc_enquadr_artig ea 
					where e.id = ea.enquadramento_id
					and ea.artigo is not null
					and e.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterInfracao(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select 
					li.data vistoria_data,
					extract(YEAR from li.data) vistoria_ano,
					li.local local,
					le.id local_estado_id,
					le.sigla local_estado_texto,
					lm.id local_municipio_id,
					lm.texto local_municipio_texto,
					'Tipo Coord: '||(select lp.texto from lov_coordenada_tipo lp where lp.id = li.SIS_COORD)||', Easting: '||li.LON_EASTING||', Northing: '||li.LAT_NORTHING   coordenadas,
					cla.id classificacao_id,
					cla.texto classificacao_texto,
					ct.id tipo_id,
					ct.texto tipo_texto,
					citem.id item_id,
					citem.texto item_texto,
					csubitem.id subitem_id,
					csubitem.texto subitem_texto
					from tab_fisc_local_infracao li, lov_municipio lm, lov_estado le, tab_fisc_infracao fi, lov_cnf_fisc_infracao_classif cla, 
					cnf_fisc_infracao_tipo ct, cnf_fisc_infracao_item citem, cnf_fisc_infracao_subitem csubitem
					where li.municipio = lm.id
					and lm.estado = le.id
					and li.fiscalizacao = fi.fiscalizacao(+)
					and fi.classificacao = cla.id(+)
					and fi.tipo = ct.id(+)
					and fi.item = citem.id(+)
					and fi.subitem = csubitem.id(+)
					and li.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterMaterialApreendido(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select 
					case 
					 when m.houve_material = 1 and m.tad_gerado = 0 then m.tad_numero 
					 when m.houve_material = 1 and m.tad_gerado = 1 then to_char(f.autos)
					else 
					 null 
					end 
					 numero, 
					s.id serie_id, 
					s.texto serie_texto, 
					m.descricao descricao_apreen, 
					m.valor_produtos valor_produto, 
					mm.tipo tipo_material_id, 
					t.texto tipo_material_texto, 
					mm.especificacao especific_material, 
					case 
					 when m.houve_material = 1 and m.tad_gerado = 0 then m.tad_data
					 when m.houve_material = 1 and m.tad_gerado = 1 then fisc_con.situacao_data
					else 
					 null 
					end lavratura_data, 
					extract(YEAR from (
					case 
					 when m.houve_material = 1 and m.tad_gerado = 0 then m.tad_data
					 when m.houve_material = 1 and m.tad_gerado = 1 then fisc_con.situacao_data
					else 
					 null 
					end)) lavratura_ano
					from tab_fisc_material_apreendido m, lov_fiscalizacao_serie s, tab_fisc_mater_apree_material mm, lov_fisc_mate_apreendido_tipo t, 
					tab_fiscalizacao f, ( select h.fiscalizacao_id, max(h.situacao_data) situacao_data from hst_fiscalizacao h where h.situacao_id = 2 group by h.fiscalizacao_id ) fisc_con
					where 
					f.id = m.fiscalizacao
					and m.serie = s.id
					and m.id = mm.material_apreendido
					and mm.tipo = t.id(+)
					and f.id = fisc_con.fiscalizacao_id(+)
					and f.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando); ;
			}
		}

		public List<Dictionary<string, object>> ObterPosse(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select func.nome funcionario, 
					s.id setor_id, 
					s.sigla setor_sigla, 
					s.nome setor_nome
					from tab_protocolo p, tab_funcionario func, tab_setor s 
					where p.emposse = func.id 
					and p.setor = s.id
					and p.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterProtocolo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select
					p.numero, 
					p.ano, 
					p.numero||'/'||p.ano numero_completo, 
					t.id tipo_id, 
					t.texto tipo_texto, 
					p.data_autuacao autuacao_sep_data, 
					extract(YEAR from p.data_autuacao) autuacao_sep_ano, 
					p.numero_autuacao autuacao_sep_numero, 
					p.data_criacao cadastro_data, 
					extract(YEAR from p.data_criacao) cadastro_ano, 
					s.id cadastro_setor_id,
					s.nome cadastro_setor_nome,
					s.sigla cadastro_setor_sigla
					from tab_protocolo p, lov_protocolo_tipo t, tab_setor s 
					where p.tipo = t.id
					and p.setor_criacao = s.id 
					and p.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterPergunta(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select hcp.pergunta_id, hcp.texto pergunta_texto , hcp.tid pergunta_tid , hr.resposta_id, hr.texto resposta_texto, hr.tid resposta_tid
					from tab_fisc_infracao i, tab_fisc_infracao_pergunta p, hst_cnf_fisc_infracao_pergunta hcp, hst_cnf_fisc_infracao_resposta hr
					where i.id = p.infracao
					and p.pergunta = hcp.pergunta_id
					and p.pergunta_tid = hcp.tid
					and p.resposta = hr.resposta_id
					and p.resposta_tid = hr.tid
					and i.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterCampo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select c.id, c.campo, c.texto,  (case when cc.tipo = 2 then 
					ConverterParaNumero(c.texto)
					else null end) valor_num, cc.tipo 
					from tab_fisc_infracao i, tab_fisc_infracao_campo c , cnf_fisc_infracao_campo cc 
					where i.id = c.infracao
					and c.campo = cc.id 
					and i.fiscalizacao = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		#endregion

		#region Obter Ids para gerar Colunas

		public List<Dictionary<string, object>> ObterPerguntaIds()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select 
					hcp.pergunta_id, 
					hcp.texto pergunta_texto
					from (select max(hcp.id) id, hcp.pergunta_id from hst_cnf_fisc_infracao_pergunta hcp group by hcp.pergunta_id) p, 
					hst_cnf_fisc_infracao_pergunta hcp
					where p.id = hcp.id
					and p.pergunta_id = hcp.pergunta_id");

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		internal List<Dictionary<string, object>> ObterFiscCamposIds()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select ca.infracao_campo_id, ca.texto, ca.tipo, ca.unidade_texto
				from (select max(c.id) id, c.infracao_campo_id from hst_cnf_fisc_infracao_campo c group by c.infracao_campo_id) mc, 
				hst_cnf_fisc_infracao_campo ca
				where ca.id = mc.id
				and ca.infracao_campo_id = ca.infracao_campo_id");

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		#endregion
	}
}