using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.WindowsService.Relatorio.Model.Data;

namespace Tecnomapas.EtramiteX.WindowsService.TituloETL.Data
{
	class TituloDa
	{
		ConfiguracaoSistema _configSys;
		RelatorioDa _daRelatorio;

		public TituloDa()
		{
			_configSys = new ConfiguracaoSistema();
			_daRelatorio = new RelatorioDa();
		}

		#region Ações de DML

		internal void Salvar(List<Dictionary<string, object>> eleitos, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				if (eleitos != null && eleitos.Count > 0)
				{
					eleitos.ForEach(item =>
					{
						bool existe = Existe(Convert.ToInt32(item["Id"]), bancoDeDados);

						if (existe)
						{
							#region Atualiza os dados da ETL

							Comando comando = bancoDeDados.CriarComando(@"
							update fat_titulo f set 
							f.numero = :numero,
							f.ano = :ano,
							f.numero_completo = :numero_completo,
							f.tipo_id = :tipo_id,
							f.tipo_texto = :tipo_texto,
							f.modelo_id = :modelo_id,
							f.modelo_nome = :modelo_nome,
							f.modelo_sigla = :modelo_sigla,
							f.cadastro_data = :cadastro_data,
							f.cadastro_ano = :cadastro_ano,
							f.vencimento_data = :vencimento_data,
							f.vencimento_ano = :vencimento_ano,
							f.autor = :autor,
							f.emissao_municipio_id = :emissao_municipio_id,
							f.emissao_municipio_texto = :emissao_municipio_texto,
							f.assinantes = :assinantes,
							f.destinatarios = :destinatarios,
							f.requerimento = :requerimento,
							f.tid = :tid
							where f.id = :id");

							Dictionary<string, object> fato = ObterTitulo(Convert.ToInt32(item["Id"]));

							if (fato != null && fato.Count > 0)
							{
								comando.DbCommand.Parameters.Clear();

								comando.AdicionarParametroEntrada("id", fato["ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("numero", fato["NUMERO"], DbType.Int32);
								comando.AdicionarParametroEntrada("ano", fato["ANO"], DbType.Int32);
								comando.AdicionarParametroEntrada("numero_completo", fato["NUMERO_COMPLETO"], DbType.String);
								comando.AdicionarParametroEntrada("tipo_id", fato["TIPO_ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("tipo_texto", fato["TIPO_TEXTO"], DbType.String);
								comando.AdicionarParametroEntrada("modelo_id", fato["MODELO_ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("modelo_nome", fato["MODELO_NOME"], DbType.String);
								comando.AdicionarParametroEntrada("modelo_sigla", fato["MODELO_SIGLA"], DbType.String);
								comando.AdicionarParametroEntrada("cadastro_data", fato["CADASTRO_DATA"], DbType.DateTime);
								comando.AdicionarParametroEntrada("cadastro_ano", fato["CADASTRO_ANO"], DbType.Int32);
								comando.AdicionarParametroEntrada("vencimento_data", fato["VENCIMENTO_DATA"], DbType.DateTime);
								comando.AdicionarParametroEntrada("vencimento_ano", fato["VENCIMENTO_ANO"], DbType.Int32);
								comando.AdicionarParametroEntrada("autor", fato["AUTOR"], DbType.String);
								comando.AdicionarParametroEntrada("emissao_municipio_id", fato["EMISSAO_MUNICIPIO_ID"], DbType.Int32);
								comando.AdicionarParametroEntrada("emissao_municipio_texto", fato["EMISSAO_MUNICIPIO_TEXTO"], DbType.String);
								comando.AdicionarParametroEntrada("assinantes", fato["ASSINANTES"], DbType.String);
								comando.AdicionarParametroEntrada("destinatarios", fato["DESTINATARIOS"], DbType.String);
								comando.AdicionarParametroEntrada("requerimento", fato["REQUERIMENTO"], DbType.Int32);
								comando.AdicionarParametroEntrada("tid", fato["TID"], DbType.String);

								bancoDeDados.ExecutarNonQuery(comando);
								Dimensoes(Convert.ToInt32(fato["ID"]), fato["TID"].ToString(), bancoDeDados, true);
							}

							#endregion
						}
						else
						{
							Criar(item, bancoDeDados);
						}
					});
				}

				bancoDeDados.Commit();
			}
		}

		internal void Criar(Dictionary<string, object> item, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Cria os dados da ETL

				Comando comando = bancoDeDados.CriarComando(@"insert into fat_titulo (id, numero, ano, numero_completo, tipo_id, tipo_texto, modelo_id, modelo_nome, modelo_sigla, cadastro_data, 
				cadastro_ano, vencimento_data, vencimento_ano, autor, emissao_municipio_id, emissao_municipio_texto, assinantes, destinatarios, requerimento, tid)
				values (:id, :numero, :ano, :numero_completo, :tipo_id, :tipo_texto, :modelo_id, :modelo_nome, :modelo_sigla, :cadastro_data, 
				:cadastro_ano, :vencimento_data, :vencimento_ano, :autor, :emissao_municipio_id, :emissao_municipio_texto, :assinantes, :destinatarios, :requerimento, :tid)");

				if (item != null && item.Count > 0)
				{
					Dictionary<string, object> fato = ObterTitulo(Convert.ToInt32(item["Id"]));

					if (fato != null && fato.Count > 0)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("id", fato["ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("numero", fato["NUMERO"], DbType.Int32);
						comando.AdicionarParametroEntrada("ano", fato["ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("numero_completo", fato["NUMERO_COMPLETO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_id", fato["TIPO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_texto", fato["TIPO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("modelo_id", fato["MODELO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("modelo_nome", fato["MODELO_NOME"], DbType.String);
						comando.AdicionarParametroEntrada("modelo_sigla", fato["MODELO_SIGLA"], DbType.String);
						comando.AdicionarParametroEntrada("cadastro_data", fato["CADASTRO_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("cadastro_ano", fato["CADASTRO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("vencimento_data", fato["VENCIMENTO_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("vencimento_ano", fato["VENCIMENTO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("autor", fato["AUTOR"], DbType.String);
						comando.AdicionarParametroEntrada("emissao_municipio_id", fato["EMISSAO_MUNICIPIO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("emissao_municipio_texto", fato["EMISSAO_MUNICIPIO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("assinantes", fato["ASSINANTES"], DbType.String);
						comando.AdicionarParametroEntrada("destinatarios", fato["DESTINATARIOS"], DbType.String);
						comando.AdicionarParametroEntrada("requerimento", fato["REQUERIMENTO"], DbType.Int32);
						comando.AdicionarParametroEntrada("tid", fato["TID"], DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
						Dimensoes(Convert.ToInt32(fato["ID"]), fato["TID"].ToString(), bancoDeDados);
					}
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

				#region Apaga os dados da ETL

				List<String> lista = new List<string>();
				lista.Add("begin ");
				lista.Add("delete from dim_titulo_reg_atividade p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_inf_corte p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_condicionante p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_atividade p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_resp_tecnico p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_setor p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_empreendimento p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_interessado p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_protocolo p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_situacao p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_silvicultura p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_queima_control p where p.fato = :fato;");
				lista.Add("delete from dim_titulo_explo_florestal p where p.fato = :fato;");
				lista.Add("delete from fat_titulo p where p.id = :fato;");
				lista.Add(" end;");

				Comando comando = bancoDeDados.CriarComando(String.Join(" ", lista));

				comando.AdicionarParametroEntrada("fato", DbType.Int32);

				eleitos.ForEach(item =>
				{
					comando.SetarValorParametro("fato", item["Id"]);
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
				List<Dictionary<string, object>> itens;

				#region Apaga os dados da ETL das dimensões

				if (deletar)
				{
					List<String> lista = new List<string>();
					lista.Add("begin ");
					lista.Add("delete from dim_titulo_reg_atividade p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_inf_corte p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_condicionante p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_atividade p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_resp_tecnico p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_setor p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_empreendimento p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_interessado p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_protocolo p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_situacao p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_silvicultura p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_queima_control p where p.fato = :fato;");
					lista.Add("delete from dim_titulo_explo_florestal p where p.fato = :fato;");
					lista.Add(" end;");

					comando = bancoDeDados.CriarComando(String.Join(" ", lista));
					comando.AdicionarParametroEntrada("fato", id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
					bancoDeDados.Commit();
				}

				#endregion

				#region Cria os dados das dimensões

				#region Situação

				itens = ObterSituacao(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_situacao c (id, fato, situacao_id, situacao_texto, situacao_data, situacao_ano, 
					situacao_motivo_id, situacao_motivo_texto, tid) values (seq_dim_titulo_situacao.nextval, :fato, :situacao_id, :situacao_texto, :situacao_data, :situacao_ano, 
					:situacao_motivo_id, :situacao_motivo_texto, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_id", item["SITUACAO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_texto", item["SITUACAO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("situacao_data", item["SITUACAO_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("situacao_ano", item["SITUACAO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_motivo_id", item["SITUACAO_MOTIVO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_motivo_texto", item["SITUACAO_MOTIVO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Situação

				#region Protocolo

				itens = ObterProtocolo(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_protocolo c (id, fato, numero, ano, numero_completo, protocolo_id, protocolo_texto, tipo_id, 
					tipo_texto, cadastro_data, cadastro_ano, cadastro_estado_id, cadastro_estado_texto, cadastro_municipio_id, cadastro_municipio_texto, autuacao_sep_data, 
					autuacao_sep_ano, autuacao_sep_numero, tid) values 
					(seq_dim_titulo_protocolo.nextval, :fato, :numero, :ano, :numero_completo, :protocolo_id, :protocolo_texto, :tipo_id, :tipo_texto, 
					:cadastro_data, :cadastro_ano, :cadastro_estado_id, :cadastro_estado_texto, :cadastro_municipio_id, :cadastro_municipio_texto, :autuacao_sep_data, 
					:autuacao_sep_ano, :autuacao_sep_numero, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("numero", item["NUMERO"], DbType.Int32);
						comando.AdicionarParametroEntrada("ano", item["ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("numero_completo", item["NUMERO_COMPLETO"], DbType.String);
						comando.AdicionarParametroEntrada("protocolo_id", item["PROTOCOLO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("protocolo_texto", item["PROTOCOLO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tipo_id", item["TIPO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_texto", item["TIPO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("cadastro_data", item["CADASTRO_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("cadastro_ano", item["CADASTRO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("cadastro_estado_id", item["CADASTRO_ESTADO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("cadastro_estado_texto", item["CADASTRO_ESTADO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("cadastro_municipio_id", item["CADASTRO_MUNICIPIO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("cadastro_municipio_texto", item["CADASTRO_MUNICIPIO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("autuacao_sep_data", item["AUTUACAO_SEP_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("autuacao_sep_ano", item["AUTUACAO_SEP_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("autuacao_sep_numero", item["AUTUACAO_SEP_NUMERO"], DbType.String);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Protocolo

				#region Interessado

				itens = ObterInteressado(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_interessado c (id, fato, nome_razao, cpf_cnpj, tid) 
					values (seq_dim_titulo_interessado.nextval, :fato, :nome_razao, :cpf_cnpj, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("nome_razao", item["NOME_RAZAO"], DbType.String);
						comando.AdicionarParametroEntrada("cpf_cnpj", item["CPF_CNPJ"], DbType.String);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Interessado

				#region Empreendimento

				itens = ObterEmpreendimento(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_empreendimento c (id, fato, denominador, cnpj, local_estado_id, 
					local_estado_texto, local_municipio_id, local_municipio_texto, coordenadas, tid) 
					values (seq_dim_titulo_empreendimento.nextval, :fato, :denominador, :cnpj, :local_estado_id, 
					:local_estado_texto, :local_municipio_id, :local_municipio_texto, :coordenadas, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("denominador", item["DENOMINADOR"], DbType.String);
						comando.AdicionarParametroEntrada("cnpj", item["CNPJ"], DbType.String);
						comando.AdicionarParametroEntrada("local_estado_id", item["LOCAL_ESTADO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("local_estado_texto", item["LOCAL_ESTADO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("local_municipio_id", item["LOCAL_MUNICIPIO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("local_municipio_texto", item["LOCAL_MUNICIPIO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("coordenadas", item["COORDENADAS"], DbType.String);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Empreendimento

				#region Setor

				itens = ObterSetor(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_setor c 
					(id, fato, cad_setor_id, cad_setor_sigla, cad_setor_nome, cad_setor_municipio_id, cad_setor_municipio_texto, 
					cad_setor_agrupador_id, cad_setor_agrupador_texto, con_setor_id, con_setor_sigla, con_setor_nome, con_setor_municipio_id, con_setor_municipio_texto, 
					con_setor_agrupador_id, con_setor_agrupador_texto, tid) values 
					(seq_dim_titulo_situacao.nextval, :fato, :cad_setor_id, :cad_setor_sigla, :cad_setor_nome, :cad_setor_municipio_id, :cad_setor_municipio_texto, 
					:cad_setor_agrupador_id, :cad_setor_agrupador_texto, :con_setor_id, :con_setor_sigla, :con_setor_nome, :con_setor_municipio_id, :con_setor_municipio_texto, 
					:con_setor_agrupador_id, :con_setor_agrupador_texto, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("cad_setor_id", item["CAD_SETOR_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("cad_setor_sigla", item["CAD_SETOR_SIGLA"], DbType.String);
						comando.AdicionarParametroEntrada("cad_setor_nome", item["CAD_SETOR_NOME"], DbType.String);
						comando.AdicionarParametroEntrada("cad_setor_municipio_id", item["CAD_SETOR_MUNICIPIO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("cad_setor_municipio_texto", item["CAD_SETOR_MUNICIPIO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("cad_setor_agrupador_id", item["CAD_SETOR_AGRUPADOR_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("cad_setor_agrupador_texto", item["CAD_SETOR_AGRUPADOR_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("con_setor_id", item["CON_SETOR_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("con_setor_sigla", item["CON_SETOR_SIGLA"], DbType.String);
						comando.AdicionarParametroEntrada("con_setor_nome", item["CON_SETOR_NOME"], DbType.String);
						comando.AdicionarParametroEntrada("con_setor_municipio_id", item["CON_SETOR_MUNICIPIO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("con_setor_municipio_texto", item["CON_SETOR_MUNICIPIO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("con_setor_agrupador_id", item["CON_SETOR_AGRUPADOR_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("con_setor_agrupador_texto", item["CON_SETOR_AGRUPADOR_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Setor

				#region Responsável Técnico

				itens = ObterResponsavelTecnico(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_resp_tecnico c (id, fato, nome_razao, cpf_cnpj, tid) 
					values (seq_dim_titulo_resp_tecnico.nextval, :fato, :nome_razao, :cpf_cnpj, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("nome_razao", item["NOME_RAZAO"], DbType.String);
						comando.AdicionarParametroEntrada("cpf_cnpj", item["CPF_CNPJ"], DbType.String);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Responsável Técnico

				#region Atividade

				itens = ObterAtividade(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_atividade c (id, fato, atividade_id, atividade_nome, situacao_id, situacao_texto, tid) 
					values (seq_dim_titulo_atividade.nextval, :fato, :atividade_id, :atividade_nome, :situacao_id, :situacao_texto, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade_id", item["ATIVIDADE_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("atividade_nome", item["ATIVIDADE_NOME"], DbType.String);
						comando.AdicionarParametroEntrada("situacao_id", item["SITUACAO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_texto", item["SITUACAO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Atividade

				#region Condicionante

				itens = ObterCondicionante(id, tid);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_condicionante c (id, fato, descricao, criacao_data, criacao_ano, situacao_id, situacao_texto, 
					possui_prazo_id, possui_prazo_texto, prazo, possui_periodicidade_id, possui_periodicidade_texto, periodicidade_periodo, periodicidade_tipo_id, 
					periodicidade_tipo_texto, vencimento_data, vencimento_ano, tid) 
					values (seq_dim_titulo_condicionante.nextval, :fato, :descricao, :criacao_data, :criacao_ano, :situacao_id, :situacao_texto, 
					:possui_prazo_id, :possui_prazo_texto, :prazo, :possui_periodicidade_id, :possui_periodicidade_texto, :periodicidade_periodo, :periodicidade_tipo_id, 
					:periodicidade_tipo_texto, :vencimento_data, :vencimento_ano, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("descricao", item["DESCRICAO"], DbType.StringFixedLength);
						comando.AdicionarParametroEntrada("criacao_data", item["CRIACAO_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("criacao_ano", item["CRIACAO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_id", item["SITUACAO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("situacao_texto", item["SITUACAO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("possui_prazo_id", item["POSSUI_PRAZO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("possui_prazo_texto", item["POSSUI_PRAZO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("prazo", item["PRAZO"], DbType.Int32);
						comando.AdicionarParametroEntrada("possui_periodicidade_id", item["POSSUI_PERIODICIDADE_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("possui_periodicidade_texto", item["POSSUI_PERIODICIDADE_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("periodicidade_periodo", item["PERIODICIDADE_PERIODO"], DbType.Int32);
						comando.AdicionarParametroEntrada("periodicidade_tipo_id", item["PERIODICIDADE_TIPO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("periodicidade_tipo_texto", item["PERIODICIDADE_TIPO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("vencimento_data", item["VENCIMENTO_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("vencimento_ano", item["VENCIMENTO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Condicionante

				#region Registro de Atividade Florestal

				itens = ObterRegistroAtividadeFlorestal(id);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_reg_atividade c (id, fato, exercicio_ano, numero_registro, categoria_id, categoria_texto, 
					materia_prima_id, materia_prima_texto, unidade_id, unidade_texto, quantidade_ano, quantidade_floresta_plantada, quantidade_floresta_nativa, 
					quantidade_outro_estado, possui_licenca, orgao_expedicao, titulo_id, titulo_numero, titulo_modelo_texto, data_validade, protocolo_numero, data_renovacao, protocolo_numero_ren, tid) 
					values (seq_dim_titulo_reg_atividade.nextval, :fato, :exercicio_ano, :numero_registro, :categoria_id, :categoria_texto, 
					:materia_prima_id, :materia_prima_texto, :unidade_id, :unidade_texto, :quantidade_ano, :quantidade_floresta_plantada, :quantidade_floresta_nativa, 
					:quantidade_outro_estado, :possui_licenca, :orgao_expedicao, :titulo_id, :titulo_numero, :titulo_modelo_texto, :data_validade, :protocolo_numero, :data_renovacao, :protocolo_numero_ren, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("exercicio_ano", item["EXERCICIO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("numero_registro", item["NUMERO_REGISTRO"], DbType.Int32);
						comando.AdicionarParametroEntrada("categoria_id", item["CATEGORIA_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("categoria_texto", item["CATEGORIA_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("materia_prima_id", item["MATERIA_PRIMA_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("materia_prima_texto", item["MATERIA_PRIMA_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("unidade_id", item["UNIDADE_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("unidade_texto", item["UNIDADE_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("quantidade_ano", item["QUANTIDADE_ANO"], DbType.Decimal);
						comando.AdicionarParametroEntrada("quantidade_floresta_plantada", item["QUANTIDADE_FLORESTA_PLANTADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("quantidade_floresta_nativa", item["QUANTIDADE_FLORESTA_NATIVA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("quantidade_outro_estado", item["QUANTIDADE_OUTRO_ESTADO"], DbType.Decimal);

						comando.AdicionarParametroEntrada("possui_licenca", item["POSSUI_LICENCA"], DbType.String);
						comando.AdicionarParametroEntrada("orgao_expedicao", item["ORGAO_EXPEDICAO"], DbType.String);
						comando.AdicionarParametroEntrada("titulo_id", item["TITULO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("titulo_numero", item["TITULO_NUMERO"], DbType.String);
						comando.AdicionarParametroEntrada("titulo_modelo_texto", item["TITULO_MODELO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("data_validade", item["DATA_VALIDADE"], DbType.DateTime);
						comando.AdicionarParametroEntrada("protocolo_numero", item["PROTOCOLO_NUMERO"], DbType.String);
						comando.AdicionarParametroEntrada("data_renovacao", item["DATA_RENOVACAO"], DbType.DateTime);
						comando.AdicionarParametroEntrada("protocolo_numero_ren", item["PROTOCOLO_NUMERO_REN"], DbType.String);

						comando.AdicionarParametroEntrada("tid", tid, DbType.String);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Registro de Atividade Florestal

				#region Informação de Corte

				itens = ObterInformacaoCorte(id);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_inf_corte c (id, fato, informacao_data, informacao_ano, especie_id, 
					especie_texto, especie_especificar_texto, arvores_isoladas, area_corte, idade_plantio, produto_id, produto_texto, destinacao_material_id, 
					destinacao_material_texto, quantidade_produto, arvores_isoladas_restante, area_corte_restante, tid) 
					values (seq_dim_titulo_inf_corte.nextval, :fato, :informacao_data, :informacao_ano, :especie_id, 
					:especie_texto, :especie_especificar_texto, :arvores_isoladas, :area_corte, :idade_plantio, :produto_id, :produto_texto, :destinacao_material_id, 
					:destinacao_material_texto, :quantidade_produto, :arvores_isoladas_restante, :area_corte_restante, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("informacao_data", item["INFORMACAO_DATA"], DbType.DateTime);
						comando.AdicionarParametroEntrada("informacao_ano", item["INFORMACAO_ANO"], DbType.Int32);
						comando.AdicionarParametroEntrada("especie_id", item["ESPECIE_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("especie_texto", item["ESPECIE_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("especie_especificar_texto", item["ESPECIE_ESPECIFICAR_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("arvores_isoladas", item["ARVORES_ISOLADAS"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_corte", item["AREA_CORTE"], DbType.Decimal);
						comando.AdicionarParametroEntrada("idade_plantio", item["IDADE_PLANTIO"], DbType.Int32);
						comando.AdicionarParametroEntrada("produto_id", item["PRODUTO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("produto_texto", item["PRODUTO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("destinacao_material_id", item["DESTINACAO_MATERIAL_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("destinacao_material_texto", item["DESTINACAO_MATERIAL_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("quantidade_produto", item["QUANTIDADE_PRODUTO"], DbType.Decimal);
						comando.AdicionarParametroEntrada("arvores_isoladas_restante", item["ARVORES_ISOLADAS_RESTANTE"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_corte_restante", item["AREA_CORTE_RESTANTE"], DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Informação de Corte

				#region Silvicultura

				itens = ObterSilvicultura(id);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_silvicultura(id, fato, identificacao, area_total_licenciada, cult_florest_licenciada, area_licenciada_cult, 
														conclusao_id, conclusao_texto, area_total_empreend, area_total_app, area_total_reserva_legal, avn_inicial, avn_media, avn_avancada, 
														avn_desconhecida, avn_total, area_floresta_plantada, area_total_floresta, app_com_veget_nativa, app_sem_veget_nativa, app_recuperacao, tid) 
														values (seq_dim_titulo_silvicultura.nextval, :fato, :identificacao, :area_total_licenciada, :cult_florest_licenciada, :area_licenciada_cult, 
														:conclusao_id, :conclusao_texto, :area_total_empreend, :area_total_app, :area_total_reserva_legal, :avn_inicial, :avn_media, :avn_avancada, 
														:avn_desconhecida, :avn_total, :area_floresta_plantada, :area_total_floresta, :app_com_veget_nativa, :app_sem_veget_nativa, :app_recuperacao, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", item["IDENTIFICACAO"], DbType.String);
						comando.AdicionarParametroEntrada("area_total_licenciada", item["AREA_TOTAL_LICENCIADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("cult_florest_licenciada", item["CULT_FLOREST_LICENCIADA"], DbType.String);
						comando.AdicionarParametroEntrada("area_licenciada_cult", item["AREA_LICENCIADA_CULT"], DbType.Decimal);
						comando.AdicionarParametroEntrada("conclusao_id", item["CONCLUSAO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("conclusao_texto", item["CONCLUSAO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("area_total_empreend", item["AREA_TOTAL_EMPREEND"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_app", item["AREA_TOTAL_APP"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_reserva_legal", item["AREA_TOTAL_RESERVA_LEGAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_inicial", item["AVN_INICIAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_media", item["AVN_MEDIA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_avancada", item["AVN_AVANCADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_desconhecida", item["AVN_DESCONHECIDA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_total", item["AVN_TOTAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_floresta_plantada", item["AREA_FLORESTA_PLANTADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_floresta", item["AREA_TOTAL_FLORESTA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_com_veget_nativa", item["APP_COM_VEGET_NATIVA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_sem_veget_nativa", item["APP_SEM_VEGET_NATIVA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_recuperacao", item["APP_RECUPERACAO"], DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Silvicultura

				#region Queima Controlada

				itens = ObterQueimaControlada(id);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_queima_control(id, fato, identificacao, caract_area_autorizada, area_queima_autorizada, total_area_autorizada, 
														conclusao_id, conclusao_texto, area_total_empreend, area_total_app, area_total_reserva_legal, avn_inicial, avn_media, avn_avancada, 
														avn_desconhecida, avn_total, area_floresta_plantada, area_total_floresta, app_com_veget_nativa, app_sem_veget_nativa, app_recuperacao, tid) 
														values (seq_dim_titulo_queima_control.nextval, :fato, :identificacao, :caract_area_autorizada, :area_queima_autorizada, :total_area_autorizada,
														:conclusao_id, :conclusao_texto, :area_total_empreend, :area_total_app, :area_total_reserva_legal, :avn_inicial, :avn_media, :avn_avancada, 
														:avn_desconhecida, :avn_total, :area_floresta_plantada, :area_total_floresta, :app_com_veget_nativa, :app_sem_veget_nativa, :app_recuperacao, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", item["IDENTIFICACAO"], DbType.String);
						comando.AdicionarParametroEntrada("caract_area_autorizada", item["CARACT_AREA_AUTORIZADA"], DbType.String);
						comando.AdicionarParametroEntrada("area_queima_autorizada", item["AREA_QUEIMA_AUTORIZADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("total_area_autorizada", item["TOTAL_AREA_AUTORIZADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("conclusao_id", item["CONCLUSAO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("conclusao_texto", item["CONCLUSAO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("area_total_empreend", item["AREA_TOTAL_EMPREEND"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_app", item["AREA_TOTAL_APP"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_reserva_legal", item["AREA_TOTAL_RESERVA_LEGAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_inicial", item["AVN_INICIAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_media", item["AVN_MEDIA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_avancada", item["AVN_AVANCADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_desconhecida", item["AVN_DESCONHECIDA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_total", item["AVN_TOTAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_floresta_plantada", item["AREA_FLORESTA_PLANTADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_floresta", item["AREA_TOTAL_FLORESTA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_com_veget_nativa", item["APP_COM_VEGET_NATIVA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_sem_veget_nativa", item["APP_SEM_VEGET_NATIVA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_recuperacao", item["APP_RECUPERACAO"], DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Queima Controlada
				
				#region Exploração Florestal

				itens = ObterExploracaoFlorestal(id);

				if (itens != null && itens.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into dim_titulo_explo_florestal 
					(id, fato, identificacao, finalidade_codigo, finalidade_texto, finalidade_outros, classificacao_vegetacao_id, 
					classificacao_vegetacao_texto, area_croqui, quantidade_arvores, produto_id, produto_texto, quantidade, exploracao_tipo_id, exploracao_tipo_texto, 
					area_total_croqui, quantidade_total_arvores, exploracao_requerida_id, exploracao_requerida_texto, conclusao_id, conclusao_texto, area_total_empreend, 
					area_total_app, area_total_reserva_legal, avn_inicial, avn_media, avn_avancada, avn_desconhecida, avn_total, area_floresta_plantada, area_total_floresta, 
					app_com_veget_nativa, app_sem_veget_nativa, app_recuperacao, tid) 
					values (seq_dim_titulo_explo_florestal.nextval, :fato, :identificacao, :finalidade_codigo, :finalidade_texto, :finalidade_outros, :classificacao_vegetacao_id, 
					:classificacao_vegetacao_texto, :area_croqui, :quantidade_arvores, :produto_id, :produto_texto, :quantidade, :exploracao_tipo_id, :exploracao_tipo_texto, 
					:area_total_croqui, :quantidade_total_arvores, :exploracao_requerida_id, :exploracao_requerida_texto, :conclusao_id, :conclusao_texto, :area_total_empreend, 
					:area_total_app, :area_total_reserva_legal, :avn_inicial, :avn_media, :avn_avancada, :avn_desconhecida, :avn_total, :area_floresta_plantada, :area_total_floresta, 
					:app_com_veget_nativa, :app_sem_veget_nativa, :app_recuperacao, :tid)");

					foreach (var item in itens)
					{
						comando.DbCommand.Parameters.Clear();

						comando.AdicionarParametroEntrada("fato", id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", item["IDENTIFICACAO"], DbType.String);
						comando.AdicionarParametroEntrada("finalidade_codigo", item["FINALIDADE_CODIGO"], DbType.Int32);
						comando.AdicionarParametroEntrada("finalidade_texto", item["FINALIDADE_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("finalidade_outros", item["FINALIDADE_OUTROS"], DbType.String);
						comando.AdicionarParametroEntrada("classificacao_vegetacao_id", item["CLASSIFICACAO_VEGETACAO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("classificacao_vegetacao_texto", item["CLASSIFICACAO_VEGETACAO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("area_croqui", item["AREA_CROQUI"], DbType.Decimal);
						comando.AdicionarParametroEntrada("quantidade_arvores", item["QUANTIDADE_ARVORES"], DbType.Int32);
						comando.AdicionarParametroEntrada("produto_id", item["PRODUTO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("produto_texto", item["PRODUTO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("quantidade", item["QUANTIDADE"], DbType.Decimal);
						comando.AdicionarParametroEntrada("exploracao_tipo_id", item["EXPLORACAO_TIPO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("exploracao_tipo_texto", item["EXPLORACAO_TIPO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("area_total_croqui", item["AREA_TOTAL_CROQUI"], DbType.Decimal);
						comando.AdicionarParametroEntrada("quantidade_total_arvores", item["QUANTIDADE_TOTAL_ARVORES"], DbType.Int32);
						comando.AdicionarParametroEntrada("exploracao_requerida_id", item["EXPLORACAO_REQUERIDA_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("exploracao_requerida_texto", item["EXPLORACAO_REQUERIDA_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("conclusao_id", item["CONCLUSAO_ID"], DbType.Int32);
						comando.AdicionarParametroEntrada("conclusao_texto", item["CONCLUSAO_TEXTO"], DbType.String);
						comando.AdicionarParametroEntrada("area_total_empreend", item["AREA_TOTAL_EMPREEND"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_app", item["AREA_TOTAL_APP"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_reserva_legal", item["AREA_TOTAL_RESERVA_LEGAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_inicial", item["AVN_INICIAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_media", item["AVN_MEDIA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_avancada", item["AVN_AVANCADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_desconhecida", item["AVN_DESCONHECIDA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("avn_total", item["AVN_TOTAL"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_floresta_plantada", item["AREA_FLORESTA_PLANTADA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("area_total_floresta", item["AREA_TOTAL_FLORESTA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_com_veget_nativa", item["APP_COM_VEGET_NATIVA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_sem_veget_nativa", item["APP_SEM_VEGET_NATIVA"], DbType.Decimal);
						comando.AdicionarParametroEntrada("app_recuperacao", item["APP_RECUPERACAO"], DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", tid, DbType.String);

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Exploração Florestal
				
				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion Ações de DML

		#region Obter

		public List<Dictionary<string, object>> Eleitos(DateTime execucaoInicio, BancoDeDados banco = null)
		{
			List<Dictionary<string, object>> retorno = new List<Dictionary<string, object>>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				//ação 69 - Alterar Situação (Somente para títulos declaratórios)
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct t.id, 
				(case when t.acao != 3 then null else 3 end) acao
				from (select h.titulo_id id, 
						(select a.acao from lov_historico_artefatos_acoes a where a.id = h.acao_executada) acao
						from hst_titulo h
						where h.data_execucao > :execucao_inicio
						and h.acao_executada in (70, 71, 72, 74, 75) 
						order by h.data_execucao) t");

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
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from fat_titulo p where p.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public Dictionary<string, object> ObterTitulo(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.tid from tab_titulo t where t.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				string tid = Convert.ToString(bancoDeDados.ExecutarScalar(comando));

				comando = bancoDeDados.CriarComando(@"
				select distinct t.titulo_id id,
					t.tid,
					tn.numero,
					tn.ano,
					tn.numero || '/' || tn.ano numero_completo,
					tm.tipo_id,
					tm.tipo_texto,
					tm.modelo_id,
					tm.nome modelo_nome,
					tm.sigla modelo_sigla,
					t.data_criacao cadastro_data,
					extract(year from t.data_criacao) cadastro_ano,
					t.data_vencimento vencimento_data,
					extract(year from t.data_vencimento) vencimento_ano,
					f.nome autor,
					t.local_emissao_id emissao_municipio_id,
					t.local_emissao_texto emissao_municipio_texto,
					(select stragg(fa.nome)
						from hst_titulo_assinantes ta, hst_funcionario fa
						where ta.funcionario_id = fa.funcionario_id 
						and ta.funcionario_tid = fa.tid
						and ta.id_hst = t.id) assinantes,
					(select stragg(nvl(pd.nome, pd.razao_social))
						from hst_titulo_pessoas tp, hst_pessoa pd
						where tp.pessoa_id = pd.pessoa_id 
						and tp.pessoa_tid = pd.tid
						and tp.id_hst = t.id) destinatarios,
					p.requerimento_id requerimento
				from hst_titulo            t,
					hst_titulo_numero      tn,
					hst_titulo_modelo      tm,
					hst_funcionario        f,
					hst_protocolo          p
				where t.id = tn.id_hst(+)
				and t.modelo_id = tm.modelo_id
				and t.modelo_tid = tm.tid
				and t.autor_id = f.funcionario_id
				and t.autor_tid = f.tid
				and t.protocolo_id = p.id_protocolo(+)
				and t.protocolo_tid = p.tid(+)
				and tm.documento_id = 1/*Titulo*/
				and t.situacao_id in (3, 5, 6) /*Concluido, Prorrogado, Encerrado*/
				and t.titulo_id = :id
				and t.tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionary(comando);
			}
		}

		public List<Dictionary<string, object>> ObterSituacao(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.situacao_id,
					t.situacao_texto,
					trunc(t.data_execucao) situacao_data,
					extract(year from t.data_execucao) situacao_ano,
					t.situacao_motivo_id,
					t.situacao_motivo_texto
				from hst_titulo t
				where t.titulo_id = :id
				and t.tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterProtocolo(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select p.numero,
					p.ano,
					p.numero || '/' || p.ano numero_completo,
					p.protocolo_id,
					p.protocolo_texto,
					p.tipo_id,
					p.tipo_texto,
					p.data_criacao cadastro_data,
					extract(year from p.data_criacao) cadastro_ano,
					se.estado_id cadastro_estado_id,
					se.estado_texto cadastro_estado_texto,
					se.municipio_id cadastro_municipio_id,
					se.municipio_texto cadastro_municipio_texto,
					p.data_autuacao autuacao_sep_data,
					extract(year from p.data_autuacao) autuacao_sep_ano,
					p.numero_autuacao autuacao_sep_numero
				from hst_titulo         t,
					 hst_protocolo      p,
					 hst_setor          s,
					 hst_setor_endereco se
				where p.id_protocolo = t.protocolo_id
				and p.tid = t.protocolo_tid
				and p.setor_criacao_id = s.setor_id
				and p.setor_tid = s.tid
				and se.id_hst = s.id
				and t.titulo_id = :id
				and t.tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterInteressado(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select nvl(p.nome, p.razao_social) nome_razao, 
					nvl(p.cpf, p.cnpj) cpf_cnpj
				from hst_titulo_pessoas tp, hst_pessoa p
				where p.pessoa_id = tp.pessoa_id
				and p.tid = tp.pessoa_tid
				and tp.titulo_id = :id
				and tp.titulo_tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterEmpreendimento(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select e.denominador,
					e.cnpj,
					e.segmento_id,
					e.segmento_texto,
					ee.estado_id local_estado_id,
					(select le.sigla from lov_estado le where le.id = ee.estado_id) local_estado_texto,
					ee.municipio_id local_municipio_id,
					ee.municipio_texto local_municipio_texto,
					(select 'Tipo Coord: ' ||c.tipo_coordenada_texto|| ', Easting: ' ||c.easting_utm|| ', Northing: ' ||c.northing_utm
						from hst_empreendimento_coord c
						where c.id_hst = e.id) coordenadas
				from hst_titulo                  t,
					 hst_empreendimento          e,
					 hst_empreendimento_endereco ee
				where t.empreendimento_id = e.empreendimento_id
				and t.empreendimento_tid = e.tid
				and ee.id_hst = e.id
				and ee.correspondencia = 0
				and t.titulo_id = :id
				and t.tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterSetor(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select s.setor_id                 cad_setor_id,
					s.sigla                       cad_setor_sigla,
					s.nome                        cad_setor_nome,
					se.municipio_id               cad_setor_municipio_id,
					se.municipio_texto            cad_setor_municipio_texto,
					sa.id                         cad_setor_agrupador_id,
					sa.nome                       cad_setor_agrupador_texto,
					con.con_setor_id,
					con.con_setor_sigla,
					con.con_setor_nome,
					con.con_setor_municipio_id,
					con.con_setor_municipio_texto,
					con.con_setor_agrupador_id,
					con.con_setor_agrupador_texto
				from hst_titulo t,
					hst_setor s,
					hst_setor_endereco se,
					tab_setor_grupo sg,
					tab_setor_agrupador sa,
					(select ht.titulo_id,
							hs.setor_id        con_setor_id,
							hs.sigla           con_setor_sigla,
							hs.nome            con_setor_nome,
							he.municipio_id    con_setor_municipio_id,
							he.municipio_texto con_setor_municipio_texto,
							a.id               con_setor_agrupador_id,
							a.nome             con_setor_agrupador_texto
						from hst_titulo         ht,
							hst_protocolo       hp,
							hst_setor           hs,
							hst_setor_endereco  he,
							tab_setor_grupo     g,
							tab_setor_agrupador a
						where ht.protocolo_id = hp.id_protocolo
						and ht.protocolo_tid = hp.tid
						and hp.setor_id = hs.setor_id
						and hp.setor_tid = hs.tid
						and hs.id = he.id_hst
						and hs.setor_id = g.setor
						and g.grupo = a.id
						and ht.id = (select min(h.id) from hst_titulo h where h.titulo_id = :id and h.situacao_id = 3)) con
				where t.setor_id = s.setor_id
				and t.setor_tid = s.tid
				and se.id_hst = s.id
				and sg.setor = s.setor_id
				and sa.id = sg.grupo
				and t.titulo_id = con.titulo_id
				and t.titulo_id = :id
				and t.tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterResponsavelTecnico(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select nvl(tp.nome, tp.razao_social) nome_razao,
					nvl(tp.cpf, tp.cnpj) cpf_cnpj
				from hst_titulo                t,
					hst_protocolo             p,
					hst_protocolo_responsavel pr,
					hst_pessoa                tp
				where p.id_protocolo = t.protocolo_id
				and p.tid = t.protocolo_tid
				and pr.id_hst = p.id
				and tp.pessoa_id = pr.responsavel_id
				and tp.tid = pr.responsavel_tid
				and t.titulo_id = :id
				and t.tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterAtividade(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select a.id     atividade_id,
					a.atividade atividade_nome,
					las.id      situacao_id,
					las.texto   situacao_texto
				from tab_titulo               t,
					tab_titulo_atividades    ta,
					tab_protocolo_atividades pa,
					tab_atividade            a,
					lov_atividade_situacao   las
				where t.id = ta.titulo
				and ta.protocolo = pa.protocolo
				and ta.atividade = pa.atividade
				and pa.atividade = a.id
				and pa.situacao = las.id
				and t.id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterCondicionante(int id, string tid)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select c.descricao,
					c.data_inicio criacao_data,
					extract(year from c.data_inicio) criacao_ano,
					c.situacao_id,
					c.situacao_texto,
					(case when c.possui_prazo = 0 then 1 else 2 end) possui_prazo_id,
					(case when c.possui_prazo = 0 then 'Não' else 'Sim' end) possui_prazo_texto,
					c.prazo,
					(case when c.periodicidade = 0 then 1 else 2 end) possui_periodicidade_id,
					(case when c.periodicidade = 0 then 'Não' else 'Sim' end) possui_periodicidade_texto,
					c.periodo periodicidade_periodo,
					c.periodo_tipo_id periodicidade_tipo_id,
					c.periodo_tipo_texto periodicidade_tipo_texto,
					c.data_vencimento vencimento_data,
					extract(year from c.data_inicio) vencimento_ano
				from hst_titulo_condicionantes      c
				where c.titulo_id = :id
				and c.titulo_tid = :tid");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterRegistroAtividadeFlorestal(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select e.ano_exercicio exercicio_ano,
					c.numero_registro,
					rc.atividade_id categoria_id,
					a.cod_categoria || ' - ' || a.atividade categoria_texto,
					rf.tipo_id materia_prima_id,
					rf.tipo_texto materia_prima_texto,
					rf.unidade_id,
					rf.unidade_texto,
					rf.qde quantidade_ano,
					rf.qde_floresta_plantada quantidade_floresta_plantada,
					rf.qde_floresta_nativa quantidade_floresta_nativa,
					rf.qde_outro_estado quantidade_outro_estado,
					(case when rc.possui_licenca = 1 then 'Sim' else 'Não' end) possui_licenca,
					rt.orgao_expedicao, 
					rt.titulo_id,
					rt.titulo_numero, 
					rt.titulo_modelo_texto, 
					rt.data_validade, 
					rt.protocolo_numero, 
					rt.data_renovacao, 
					rt.protocolo_numero_ren
				from esp_cer_reg_ati_florestal    e,
					tab_titulo_dependencia        td,
					tab_titulo_atividades         ta,
					hst_crt_reg_ativida_florestal c,
					hst_crt_reg_ati_flo_consumo   rc,
					hst_crt_reg_ati_flo_fonte     rf,
					hst_crt_reg_ati_flo_con_tit   rt,
					tab_atividade                 a
				where e.titulo = td.titulo
				and e.titulo = ta.titulo
				and td.dependencia_tid = c.tid
				and c.id = rc.id_hst
				and rc.id = rf.id_hst_consumo
				and rc.id = rt.id_hst_consumo
				and ta.atividade = rc.atividade_id
				and rc.atividade_id = a.id
				and e.titulo = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterInformacaoCorte(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct i.data_informacao informacao_data,
						extract(year from i.data_informacao) informacao_ano,
						ie.especie_id,
						ie.especie_texto,
						ie.especie_especificar_texto,
						ie.arvores_isoladas,
						ie.area_corte,
						ie.idade_plantio,
						null produto_id,
						null produto_texto,
						null destinacao_material_id,
						null destinacao_material_texto,
						null quantidade_produto,
						i.arvores_isoladas_restante,
						i.area_corte_restante
					from esp_out_informacao_corte e,
						(select he.titulo_id, he.informacao_corte_id, he.informacao_corte_tid
							from hst_esp_out_informacao_corte he, hst_titulo ht
							where he.titulo_id = ht.titulo_id
							and he.titulo_tid = ht.tid
							and ht.id = (select max(h.id) from hst_titulo h where h.titulo_id = ht.titulo_id and h.situacao_id = 3)) d,
						hst_crt_inf_corte_inf i,
						hst_crt_inf_corte_inf_especie ie
					where e.titulo = d.titulo_id
					and d.informacao_corte_id = i.inf_corte_inf
					and d.informacao_corte_tid = i.tid
					and i.id = ie.id_hst
					and e.titulo = :id
				union all
				select distinct i.data_informacao informacao_data,
						extract(year from i.data_informacao) informacao_ano,
						null especie_id,
						null especie_texto,
						null especie_especificar_texto,
						null arvores_isoladas,
						null area_corte,
						null idade_plantio,
						ip.produto_id,
						ip.produto_texto,
						ip.destinacao_material_id,
						ip.destinacao_material_texto,
						ip.quantidade quantidade_produto,
						i.arvores_isoladas_restante,
						i.area_corte_restante
					from esp_out_informacao_corte e,
						(select he.titulo_id, he.informacao_corte_id, he.informacao_corte_tid
							from hst_esp_out_informacao_corte he, hst_titulo ht
							where he.titulo_id = ht.titulo_id
							and he.titulo_tid = ht.tid
							and ht.id = (select max(h.id) from hst_titulo h where h.titulo_id = ht.titulo_id and h.situacao_id = 3)) d,
						hst_crt_inf_corte_inf i,
						hst_crt_inf_corte_inf_produto ip
					where e.titulo = d.titulo_id
					and d.informacao_corte_id = i.inf_corte_inf
					and d.informacao_corte_tid = i.tid
					and i.id = ip.id_hst
					and e.titulo = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarDictionaryList(comando);
			}
		}

		public List<Dictionary<string, object>> ObterSilvicultura(int id)
		{
			List<Dictionary<string, object>> Itens = new List<Dictionary<string, object>>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				#region Silvicultura

				comando.DbCommand.CommandText = @"select td.titulo, s.identificacao,
												  (select sum(area_croqui)
													from hst_crt_silvicultura_silv
													where id_hst = c.id) area_total_licenciada,
												  (select stragg(nvl(sc.especificar, sc.cultura_texto))
													from hst_crt_silvicultura_cult sc
													where sc.id_hst = s.id) cult_florest_licenciada,
												  (select sum(sc.area)
													from hst_crt_silvicultura_cult sc
													where sc.id_hst = s.id) area_licenciada_cult,
												  null conclusao_id,
												  null conclusao_texto,
												  null area_total_empreend,
												  null area_total_app,
												  null area_total_reserva_legal,
												  null avn_inicial,
												  null avn_media,
												  null avn_avancada,
												  null avn_desconhecida,
												  null avn_total,
												  null area_floresta_plantada,
												  null area_total_floresta,
												  null app_com_veget_nativa,
												  null app_sem_veget_nativa,
												  null app_recuperacao
												from tab_titulo_dependencia    td,
												  hst_crt_silvicultura      c,
												  hst_crt_silvicultura_silv s
												where td.dependencia_tipo = 1 /*Caracterização*/
												and td.dependencia_caracterizacao = 17 /*Silvicultura*/
												and td.dependencia_id = c.caracterizacao
												and td.dependencia_tid = c.tid
												and c.id = s.id_hst
												and td.titulo = :id";

				#endregion Silvicultura

				Itens.AddRange(bancoDeDados.ExecutarDictionaryList(comando));

				#region Laudo de Vistoria Florestal

				comando.DbCommand.CommandText = @"
												select 
												  null identificacao,
												  null area_total_licenciada,
												  null cult_florest_licenciada,
												  null area_licenciada_cult,
												  e.conclusao conclusao_id,
												  (select l.texto from lov_esp_conclusao l where l.id = e.conclusao) conclusao_texto,
												  (select sum(area_croqui) from hst_crt_dominialidade_dominio where id_hst = d.id) area_total_empreend,
												  (select sum(app_croqui) from hst_crt_dominialidade_dominio where id_hst = d.id) area_total_app,
												  (select sum(arl_croqui) from hst_crt_dominialidade_reserva
												  where id_hst in (select id from hst_crt_dominialidade_dominio where id_hst = d.id)) area_total_reserva_legal,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 1) avn_inicial,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 2) avn_media,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 3) avn_avancada,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 4) avn_desconhecida,
												  0 avn_total,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 10) area_floresta_plantada,
												  0 area_total_floresta,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 5) app_com_veget_nativa,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 8) app_sem_veget_nativa,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 7) app_recuperacao
												  from
												  tab_titulo                   t,
												  tab_titulo_modelo            tm,
												  esp_laudo_vistoria_florestal e,
												  tab_titulo_dependencia       td,
												  hst_crt_dominialidade        d
												  where t.modelo = tm.id
												  and tm.codigo = 4 /*Laudo de Vistoria Florestal*/
												  and td.dependencia_tipo = 1 /*Caracterização*/
												  and td.dependencia_caracterizacao = 1 /*Dominialidade*/
												  and d.dominialidade_id = td.dependencia_id
												  and d.tid = td.dependencia_tid      
												  and e.titulo = t.id
												  and td.titulo = t.id
												  and e.caracterizacao = 17/*Silvicultura*/
												  and t.id = :id";

				#endregion Laudo de Vistoria Florestal

				Itens.AddRange(bancoDeDados.ExecutarDictionaryList(comando));

				#region Configurar Áreas

				foreach (var item in Itens.Where(x => string.IsNullOrEmpty(x["IDENTIFICACAO"].ToString())))
				{
					Dictionary<string, object> auxiliar = new Dictionary<string, object>();
					Decimal avnInicial = 0;
					Decimal avnMedia = 0;
					Decimal avnAvancada = 0;
					Decimal avnDeconhecida = 0;
					Decimal areaFlorestaPlantada = 0;

					Decimal.TryParse(item["AVN_INICIAL"].ToString(), out avnInicial);
					Decimal.TryParse(item["AVN_MEDIA"].ToString(), out avnMedia);
					Decimal.TryParse(item["AVN_AVANCADA"].ToString(), out avnAvancada);
					Decimal.TryParse(item["AVN_DESCONHECIDA"].ToString(), out avnDeconhecida);
					Decimal.TryParse(item["AREA_FLORESTA_PLANTADA"].ToString(), out areaFlorestaPlantada);

					Decimal avnTotal = (avnInicial + avnMedia + avnAvancada + avnDeconhecida);

					item["AVN_TOTAL"] = avnTotal;
					item["AREA_TOTAL_FLORESTA"] = avnTotal + areaFlorestaPlantada;
				}

				#endregion Configurar Áreas

				return Itens;
			}
		}

		public List<Dictionary<string, object>> ObterQueimaControlada(int id)
		{
			List<Dictionary<string, object>> Itens = new List<Dictionary<string, object>>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				#region Queima Controlada

				comando.DbCommand.CommandText = @"select 
												  q.identificacao,
												 (select stragg(nvl(qc.nome_finalidade, qc.tipo_cultivo_texto))
												  from hst_crt_queima_contr_tipo_cult qc
												 where qc.id_hst = q.id) caract_area_autorizada,       
												 (select sum(qc.area_queima)
												  from hst_crt_queima_contr_tipo_cult qc
												 where qc.id_hst = q.id) area_queima_autorizada,       
												 (select sum(qc.area_queima)
												  from hst_crt_queima_contr_tipo_cult qc
												 where qc.id_hst in (select id from hst_crt_queima_contr_queima
												   where id_hst = c.id)) total_area_autorizada,        
												  null conclusao_id,
												  null conclusao_texto,
												  null area_total_empreend,
												  null area_total_app,
												  null area_total_reserva_legal,
												  null avn_inicial,
												  null avn_media,
												  null avn_avancada,
												  null avn_desconhecida,
												  null avn_total,
												  null area_floresta_plantada,
												  null area_total_floresta,
												  null app_com_veget_nativa,
												  null app_sem_veget_nativa,
												  null app_recuperacao
												from tab_titulo_dependencia    td,
												  hst_crt_queima_contr      c,
												  hst_crt_queima_contr_queima q
												where td.dependencia_tipo = 1 /*Caracterização*/
												and td.dependencia_caracterizacao = 5 /*Queima Controlada*/
												and td.dependencia_id = c.queima_controlada_id
												and td.dependencia_tid = c.tid
												and c.id = q.id_hst
												and td.titulo = :id";

				#endregion Queima Controlada

				Itens.AddRange(bancoDeDados.ExecutarDictionaryList(comando));

				#region Laudo de Vistoria Florestal

				comando.DbCommand.CommandText = @"
												select 
												  null identificacao,
												  null caract_area_autorizada,       
												  null area_queima_autorizada,       
												  null total_area_autorizada,
												  e.conclusao conclusao_id,
												  (select l.texto from lov_esp_conclusao l where l.id = e.conclusao) conclusao_texto,
												  (select sum(area_croqui) from hst_crt_dominialidade_dominio where id_hst = d.id) area_total_empreend,
												  (select sum(app_croqui) from hst_crt_dominialidade_dominio where id_hst = d.id) area_total_app,
												  (select sum(arl_croqui) from hst_crt_dominialidade_reserva
												  where id_hst in (select id from hst_crt_dominialidade_dominio where id_hst = d.id)) area_total_reserva_legal,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 1) avn_inicial,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 2) avn_media,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 3) avn_avancada,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 4) avn_desconhecida,
												  0 avn_total,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 10) area_floresta_plantada,
												  0 area_total_floresta,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 5) app_com_veget_nativa,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 8) app_sem_veget_nativa,
												  (select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 7) app_recuperacao
												  from
												  tab_titulo                   t,
												  tab_titulo_modelo            tm,
												  esp_laudo_vistoria_florestal e,
												  tab_titulo_dependencia       td,
												  hst_crt_dominialidade        d
												  where t.modelo = tm.id
												  and tm.codigo = 4 /*Laudo de Vistoria Florestal*/
												  and td.dependencia_tipo = 1 /*Caracterização*/
												  and td.dependencia_caracterizacao = 1 /*Dominialidade*/
												  and d.dominialidade_id = td.dependencia_id
												  and d.tid = td.dependencia_tid      
												  and e.titulo = t.id
												  and td.titulo = t.id
												  and e.caracterizacao = 5/*Queima Controlada*/
												  and t.id = :id";

				#endregion Laudo de Vistoria Florestal

				Itens.AddRange(bancoDeDados.ExecutarDictionaryList(comando));

				#region Configurar Áreas

				foreach (var item in Itens.Where(x => string.IsNullOrEmpty(x["IDENTIFICACAO"].ToString())))
				{
					Dictionary<string, object> auxiliar = new Dictionary<string, object>();
					Decimal avnInicial = 0;
					Decimal avnMedia = 0;
					Decimal avnAvancada = 0;
					Decimal avnDeconhecida = 0;
					Decimal areaFlorestaPlantada = 0;

					Decimal.TryParse(item["AVN_INICIAL"].ToString(), out avnInicial);
					Decimal.TryParse(item["AVN_MEDIA"].ToString(), out avnMedia);
					Decimal.TryParse(item["AVN_AVANCADA"].ToString(), out avnAvancada);
					Decimal.TryParse(item["AVN_DESCONHECIDA"].ToString(), out avnDeconhecida);
					Decimal.TryParse(item["AREA_FLORESTA_PLANTADA"].ToString(), out areaFlorestaPlantada);

					Decimal avnTotal = (avnInicial + avnMedia + avnAvancada + avnDeconhecida);

					item["AVN_TOTAL"] = avnTotal;
					item["AREA_TOTAL_FLORESTA"] = avnTotal + areaFlorestaPlantada;
				}

				#endregion Configurar Áreas

				return Itens;
			}
		}

		public List<Dictionary<string, object>> ObterExploracaoFlorestal(int id)
		{
			List<Dictionary<string, object>> Itens = new List<Dictionary<string, object>>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				#region Autorização de Exploração Florestal

				comando.DbCommand.CommandText = @"
				select 
					ce.identificacao,
					c.finalidade finalidade_codigo,
					(select stragg(lf.texto) from lov_crt_exp_flores_finalidade lf where bitand(c.finalidade, lf.codigo) <> 0) finalidade_texto,
					c.finalidade_outros,
					ce.classificacao_vegetacao_id,
					ce.classificacao_vegetacao_texto,
					ce.area_croqui,
					ce.quantidade_arvores,
					cp.produto_id,
					cp.produto_texto,
					cp.quantidade,
					ce.exploracao_tipo_id,
					ce.exploracao_tipo_texto,
					(select sum(h.area_croqui) from hst_crt_exp_florest_exploracao h where h.id_hst = c.id) area_total_croqui,
					(select sum(h.quantidade_arvores) from hst_crt_exp_florest_exploracao h where h.id_hst = c.id) quantidade_total_arvores,
					null exploracao_requerida_id,
					null exploracao_requerida_texto,
					null conclusao_id,
					null conclusao_texto,
					null area_total_empreend,
					null area_total_app,
					null area_total_reserva_legal,
					null avn_inicial,
					null avn_media,
					null avn_avancada,
					null avn_desconhecida,
					null avn_total,
					null area_floresta_plantada,
					null area_total_floresta,
					null app_com_veget_nativa,
					null app_sem_veget_nativa,
					null app_recuperacao
				from tab_titulo_dependencia        td,
					tab_titulo_modelo              tm,
					hst_crt_exploracao_florestal   c,
					hst_crt_exp_florest_exploracao ce,
					hst_crt_exp_florestal_produto  cp
				where td.dependencia_tipo = 1 /*Caracterização*/
					and td.dependencia_caracterizacao = 4 /*Exploração Florestal*/
					and tm.id = td.titulo_modelo
					and tm.codigo = 5 /*Autorização de Exploração Florestal*/
					and c.exploracao_florestal_id = td.dependencia_id
					and c.tid = td.dependencia_tid
					and ce.id_hst = c.id
					and cp.id_hst = ce.id
					and td.titulo = :id";

				#endregion Autorização de Exploração Florestal

				Itens.AddRange(bancoDeDados.ExecutarDictionaryList(comando));

				#region Laudo de Vistoria Florestal

				comando.DbCommand.CommandText = @"
				select 
					null identificacao,
					null finalidade_codigo,
					null finalidade_texto,
					null finalidade_outros,
					null classificacao_vegetacao_id,
					null classificacao_vegetacao_texto,
					null area_croqui,
					null quantidade_arvores,
					null produto_id,
					null produto_texto,
					null quantidade,
					null exploracao_tipo_id,
					null exploracao_tipo_texto,
					null area_total_croqui,
					null quantidade_total_arvores,
					null exploracao_requerida_id,
					null exploracao_requerida_texto,
					e.conclusao conclusao_id,
					(select l.texto from lov_esp_conclusao l where l.id = e.conclusao) conclusao_texto,
					(select sum(area_croqui) from hst_crt_dominialidade_dominio where id_hst = d.id) area_total_empreend,
					(select sum(app_croqui) from hst_crt_dominialidade_dominio where id_hst = d.id) area_total_app,
					(select sum(arl_croqui) from hst_crt_dominialidade_reserva
					where id_hst in (select id from hst_crt_dominialidade_dominio where id_hst = d.id)) area_total_reserva_legal,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 1) avn_inicial,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 2) avn_media,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 3) avn_avancada,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 4) avn_desconhecida,
					0 avn_total,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 10) area_floresta_plantada,
					0 area_total_floresta,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 5) app_com_veget_nativa,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 8) app_sem_veget_nativa,
					(select da.valor from hst_crt_dominialidade_areas da where da.id_hst = d.id and da.tipo_id = 7) app_recuperacao
				from esp_laudo_vistoria_florestal e,
					tab_titulo_dependencia        td,
					tab_titulo_modelo             tm,
					hst_crt_dominialidade         d
				where td.titulo = e.titulo
					and td.dependencia_tipo = 1 /*Caracterização*/
					and td.dependencia_caracterizacao = 1 /*Dominialidade*/
					and tm.id = td.titulo_modelo
					and tm.codigo = 4 /*Laudo de Vistoria Florestal*/
					and d.dominialidade_id = td.dependencia_id
					and d.tid = td.dependencia_tid
					and td.titulo = :id";

				#endregion Laudo de Vistoria Florestal

				Itens.AddRange(bancoDeDados.ExecutarDictionaryList(comando));

				#region Laudo de Vistoria Florestal

				comando.DbCommand.CommandText = @"
				select 
					ce.identificacao,
					null finalidade_codigo,
					null finalidade_texto,
					null finalidade_outros,
					null classificacao_vegetacao_id,
					null classificacao_vegetacao_texto,
					null area_croqui,
					null quantidade_arvores,
					null produto_id,
					null produto_texto,
					null quantidade,
					null exploracao_tipo_id,
					null exploracao_tipo_texto,
					null area_total_croqui,
					null quantidade_total_arvores,
					ce.exploracao_tipo_id exploracao_requerida_id,
					ce.exploracao_tipo_texto exploracao_requerida_texto,
					null conclusao_id,
					null conclusao_texto,
					null area_total_empreend,
					null area_total_app,
					null area_total_reserva_legal,
					null avn_inicial,
					null avn_media,
					null avn_avancada,
					null avn_desconhecida,
					null avn_total,
					null area_floresta_plantada,
					null area_total_floresta,
					null app_com_veget_nativa,
					null app_sem_veget_nativa,
					null app_recuperacao
				from tab_titulo_dependencia        td,
					tab_titulo_modelo              tm,
					hst_crt_exploracao_florestal   c,
					hst_crt_exp_florest_exploracao ce
				where td.dependencia_tipo = 1 /*Caracterização*/
					and td.dependencia_caracterizacao = 4 /*Exploração Florestal*/
					and tm.id = td.titulo_modelo
					and tm.codigo = 4 /*Laudo de Vistoria Florestal*/
					and c.exploracao_florestal_id = td.dependencia_id
					and c.tid = td.dependencia_tid
					and ce.id_hst = c.id
					and td.titulo = :id";

				#endregion Laudo de Vistoria Florestal

				Itens.AddRange(bancoDeDados.ExecutarDictionaryList(comando));

				#region Configurar Áreas

				foreach (var item in Itens.Where(x => string.IsNullOrEmpty(x["IDENTIFICACAO"].ToString())))
				{
					Dictionary<string, object> auxiliar = new Dictionary<string, object>();
					Decimal avnInicial = 0;
					Decimal avnMedia = 0;
					Decimal avnAvancada = 0;
					Decimal avnDeconhecida = 0;
					Decimal areaFlorestaPlantada = 0;

					Decimal.TryParse(item["AVN_INICIAL"].ToString(), out avnInicial);
					Decimal.TryParse(item["AVN_MEDIA"].ToString(), out avnMedia);
					Decimal.TryParse(item["AVN_AVANCADA"].ToString(), out avnAvancada);
					Decimal.TryParse(item["AVN_DESCONHECIDA"].ToString(), out avnDeconhecida);
					Decimal.TryParse(item["AREA_FLORESTA_PLANTADA"].ToString(), out areaFlorestaPlantada);

					Decimal avnTotal = (avnInicial + avnMedia + avnAvancada + avnDeconhecida);

					item["AVN_TOTAL"] = avnTotal;
					item["AREA_TOTAL_FLORESTA"] = avnTotal + areaFlorestaPlantada;
				}

				#endregion Configurar Áreas

				return Itens;
			}
		}

		#endregion Obter
	}
}