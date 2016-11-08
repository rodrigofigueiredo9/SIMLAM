using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloProtocolo.Data
{
	public class ProtocoloCredenciadoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public ProtocoloCredenciadoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal Resultados<Protocolo> Filtrar(Filtro<ListarProtocoloFiltro> filtros, BancoDeDados banco = null)
		{
			// Semelhante ao Filtrar, mas traz informações de Pendencias
			Resultados<Protocolo> retorno = new Resultados<Protocolo>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("e.protocolo", "protocolo", filtros.Dados.ProtocoloId);

				comandtxt += comando.FiltroAnd("e.numero", "numero", filtros.Dados.Protocolo.Numero);

				comandtxt += comando.FiltroAnd("e.ano", "ano", filtros.Dados.Protocolo.Ano);

				comandtxt += comando.FiltroAndLike("e.numero_autuacao", "numero_autuacao", filtros.Dados.NumeroAutuacao);

				comandtxt += comando.FiltroIn("e.setor_criacao_id", string.Format("select tse.setor from {0}tab_setor_endereco tse where tse.municipio = :municipio", (string.IsNullOrEmpty(EsquemaBanco) ? "" : ".")), "municipio", filtros.Dados.Municipio);

				if (!filtros.Dados.DataRegistro.IsEmpty && filtros.Dados.DataRegistro.IsValido)
				{
					comandtxt += comando.FiltroAnd("TO_DATE(e.data_criacao)", "data_criacao", filtros.Dados.DataRegistro.DataTexto);
				}

				if (!filtros.Dados.DataAutuacao.IsEmpty && filtros.Dados.DataAutuacao.IsValido)
				{
					comandtxt += comando.FiltroAnd("TO_DATE(e.data_autuacao)", "data_autuacao", filtros.Dados.DataAutuacao.DataTexto);
				}

				comandtxt += comando.FiltroAnd("e.tipo_id", "tipo", filtros.Dados.Tipo);

				comandtxt += comando.FiltroIn("e.protocolo_id", String.Format("select s.protocolo from {0}tab_protocolo_atividades s where s.situacao = :atividade_situacao_id", EsquemaBanco),
				"atividade_situacao_id", filtros.Dados.SituacaoAtividade);

				comandtxt += comando.FiltroIn("e.protocolo_id", String.Format("select a.protocolo from {0}tab_protocolo_atividades a where a.atividade = :atividade_id", EsquemaBanco),
				"atividade_id", filtros.Dados.AtividadeSolicitada);

				comandtxt += comando.FiltroAndLike("e.interessado_nome_razao", "interessado_nome_razao", filtros.Dados.InteressadoNomeRazao, true);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.InteressadoCpfCnpj))
				{
					if (ValidacoesGenericasBus.Cpf(filtros.Dados.InteressadoCpfCnpj) ||
						ValidacoesGenericasBus.Cnpj(filtros.Dados.InteressadoCpfCnpj))
					{
						comandtxt += comando.FiltroAnd("e.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);
					}
					else
					{
						comandtxt += comando.FiltroAndLike("e.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCpfCnpj);
					}
				}

				comandtxt += comando.FiltroAndLike("e.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoRazaoDenominacao, true);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.EmpreendimentoCnpj))
				{
					if (ValidacoesGenericasBus.Cnpj(filtros.Dados.EmpreendimentoCnpj))
					{
						comandtxt += comando.FiltroAnd("e.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);
					}
					else
					{
						comandtxt += comando.FiltroAndLike("e.empreendimento_cnpj", "empreendimento_cnpj", filtros.Dados.EmpreendimentoCnpj);
					}
				}

				comandtxt += @" and (:credenciado_id in
								(select re.responsavel
									from lst_protocolo p, tab_protocolo_responsavel re
									where re.protocolo(+) = p.protocolo_id
									and p.id = e.id) or e.interessado_id = :credenciado_id or e.empreendimento_id in 
										(select er.empreendimento from tab_empreendimento_responsavel er where er.responsavel = :credenciado_id) or 
										e.requerimento_id in (select tr.id from tab_requerimento tr where tr.autor = :autor_id))";
				comando.AdicionarParametroEntrada("credenciado_id", filtros.Dados.CredenciadoPessoaId, DbType.Int32);
				comando.AdicionarParametroEntrada("autor_id", filtros.Dados.AutorId, DbType.Int32);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero,ano", "interessado_nome_razao", "empreendimento_denominador" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero,ano");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}lst_protocolo e where e.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select e.id, e.protocolo_id, e.protocolo, e.numero, e.ano, e.tipo_id, e.tipo_texto, e.data_criacao, e.interessado_id,
										 e.interessado_tipo, e.interessado_nome_razao, e.interessado_cpf_cnpj, e.interessado_rg_ie, e.empreendimento_id,
										 e.empreendimento_denominador, e.empreendimento_cnpj, e.situacao_id, e.situacao_texto, (select count(*) from tab_titulo t, 
										 esp_oficio_notificacao esp where esp.titulo = t.id and t.situacao in (3,6) and e.protocolo_id = t.protocolo
										 and t.protocolo in ((select associado from tab_protocolo_associado pa where pa.protocolo = e.protocolo_id 
										 union all select e.protocolo_id from dual))) possui_oficio_pendencia from {0}lst_protocolo e where e.id > 0"

				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Protocolo item;

					while (reader.Read())
					{
						item = new Protocolo();
						item.Id = reader.GetValue<int?>("protocolo_id");
						item.IsProcesso = reader.GetValue<int>("protocolo") == 1;
						item.DataCadastro.Data = reader.GetValue<DateTime?>("data_criacao");
						item.Tipo.Id = reader.GetValue<int>("tipo_id");
						item.Tipo.Texto = reader.GetValue<string>("tipo_texto");
						item.NumeroProtocolo = reader.GetValue<int?>("numero");
						item.Ano = reader.GetValue<int?>("ano");
						item.SituacaoId = reader.GetValue<int>("situacao_id");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.Empreendimento.Id = reader.GetValue<int>("empreendimento_id");
						item.Empreendimento.Denominador = reader.GetValue<string>("empreendimento_denominador");
						item.Interessado.Id = reader.GetValue<int>("interessado_id");
						item.Interessado.Tipo = reader.GetValue<int>("interessado_tipo");
						item.PossuiPendencia = Convert.ToBoolean(reader.GetValue<int>("possui_oficio_pendencia"));

						if (item.Interessado.Tipo == PessoaTipo.FISICA)
						{
							item.Interessado.Fisica.Nome = reader.GetValue<string>("interessado_nome_razao");
						}
						else
						{
							item.Interessado.Juridica.RazaoSocial = reader.GetValue<string>("interessado_nome_razao");
						}

						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Resultados<NotificacaoPendencia> FiltrarNotificacaoPendencia(int protocolo, BancoDeDados banco = null)
		{
			Resultados<NotificacaoPendencia> retorno = new Resultados<NotificacaoPendencia>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.data_emissao, tm.nome, tn.numero, tn.ano, t.situacao situacao_id, 
															ls.texto situacao_texto, p.numero protocolo_numero, p.ano protocolo_ano
															from tab_titulo t, tab_titulo_modelo tm, tab_titulo_numero tn, esp_oficio_notificacao e, 
															lov_titulo_situacao ls, tab_protocolo p where t.id = e.titulo and ls.id = t.situacao and tm.id = t.modelo
															and tn.titulo = t.id and t.situacao in (3, 6) and t.protocolo = p.id
															and e.protocolo in (select associado from tab_protocolo_associado pa where pa.protocolo = :protocolo
															union all select :protocolo from dual)");


				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);


				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					NotificacaoPendencia item;

					while (reader.Read())
					{
						item = new NotificacaoPendencia();
						item.Id = reader.GetValue<int>("id");
						item.DataEmissao.Data = reader.GetValue<DateTime>("data_emissao");
						item.Nome = reader.GetValue<string>("nome");
						item.Numero = reader.GetValue<int>("numero");
						item.SituacaoId = reader.GetValue<int>("situacao_id");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.Ano = reader.GetValue<int>("ano");

						item.Protocolo.NumeroProtocolo = reader.GetValue<int>("protocolo_numero");
						item.Protocolo.Ano = reader.GetValue<int>("protocolo_ano");

						retorno.Itens.Add(item);
					}

					retorno.Quantidade = retorno.Itens.Count;

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}
		
		internal Resultados<HistoricoProtocolo> FiltrarHistoricoAssociados(Filtro<ListarProtocoloFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<HistoricoProtocolo> retorno = new Resultados<HistoricoProtocolo>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("associado_id", "associado_id", filtros.Dados.Id);

				comandtxt += comando.FiltroAnd("acao", "acao", filtros.Dados.Acao);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "tipo_id", "interessado_nome_razao", "empreendimento_denominador" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"
					select count(*)
					  from (select a.tipo_id, a.associado_id, la.id acao
							  from {0}hst_protocolo_associado       a,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where l.acao = la.id
							   and l.id = a.acao_executada
							union all
							select t.protocolo_id tipo_id, t.id_protocolo associado_id, la.id acao
							  from {0}hst_protocolo                 t,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where t.acao_executada = l.id(+)
							   and l.acao = la.id(+)
							   and la.id = 33/*Converter*/) where 0=0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.DbCommand.CommandText = String.Format(@"
					select *
					  from (select pa.numero || '/' || pa.ano numero,
								   to_char(a.data_execucao, 'dd/mm/yyyy hh24:mi:ss') data_execucao,
								   s.id setor_id,
								   s.sigla setor_sigla,
								   a.executor_id,
								   a.executor_nome executor_nome,
								   a.acao_executada,
								   la.id acao_id,
								   la.texto acao,
								   a.tipo_id,
								   a.tipo_texto protocolo_texto,
								   a.associado_id
							  from {0}hst_protocolo_associado       a,
								   {0}tab_protocolo                 p,
								   {0}tab_protocolo                 pa,
								   {0}tab_setor                     s,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where a.associado_id = p.id
							   and a.protocolo_id = pa.id
							   and p.setor = s.id(+)
							   and l.acao = la.id
							   and l.id = a.acao_executada
        
							union all
        
							select p.numero || '/' || p.ano numero,
								   to_char(a.data_execucao, 'dd/mm/yyyy hh24:mi:ss') data_execucao,
								   s.id setor_id,
								   s.sigla setor_sigla,
								   a.executor_id,
								   a.executor_nome executor_nome,
								   a.acao_executada,
								   la.id acao_id,
								   la.texto acao,
								   a.protocolo_id tipo_id,
								   a.protocolo_texto,
								   a.id_protocolo associado_id
							  from {0}hst_protocolo                 a,
								   {0}tab_protocolo                 p,
								   {0}tab_setor                     s,
								   {0}lov_historico_artefatos_acoes l,
								   {0}lov_historico_acao            la
							 where a.id_protocolo = p.id
							   and p.setor = s.id(+)
							   and a.acao_executada = l.id(+)
							   and l.acao = la.id(+)
							   and l.acao = 33/*Converter*/)
					 where 0 = 0"

				+ comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));


				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					HistoricoProtocolo historico;

					while (reader.Read())
					{
						historico = new HistoricoProtocolo();
						historico.Numero = reader["numero"].ToString();
						historico.AcaoId = Convert.ToInt32(reader["acao_id"]);
						historico.AcaoTexto = reader["acao"].ToString();
						historico.AcaoData.Data = Convert.ToDateTime(reader["data_execucao"]);

						if (reader["executor_id"] != null && !Convert.IsDBNull(reader["executor_id"]))
						{
							historico.Executor.Id = Convert.ToInt32(reader["executor_id"]);
							historico.Executor.Nome = reader["executor_nome"].ToString();
						}

						if (reader["setor_id"] != null && !Convert.IsDBNull(reader["setor_id"]))
						{
							historico.Setor.Id = Convert.ToInt32(reader["setor_id"]);
							historico.Setor.Sigla = reader["setor_sigla"].ToString();
						}

						historico.ProtocoloTipoId = reader.GetValue<Int32>("tipo_id");

						retorno.Itens.Add(historico);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}
	}
}