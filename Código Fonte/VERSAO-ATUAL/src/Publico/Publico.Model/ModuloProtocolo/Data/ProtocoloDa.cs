using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloProtocolo.Data
{
	class ProtocoloDa
	{
		#region Protocolo

		private string EsquemaBanco { get; set; }

		#endregion

		#region Obter / Filtrar

		internal Resultados<Protocolo> Filtrar(Filtro<ListarProtocoloFiltro> filtros, BancoDeDados banco = null)
		{
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

				comandtxt = String.Format(@"select id, protocolo_id, numero, ano, tipo_id, tipo_texto, data_criacao, interessado_id, interessado_tipo, 
					interessado_nome_razao, interessado_cpf_cnpj, interessado_rg_ie, empreendimento_id, 
					empreendimento_denominador, empreendimento_cnpj, situacao_id, situacao_texto from {0}lst_protocolo e where e.id > 0"
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
						item.Id = Convert.ToInt32(reader["protocolo_id"]);
						item.DataCadastro.Data = Convert.ToDateTime(reader["data_criacao"]);
						item.Tipo.Id = Convert.ToInt32(reader["tipo_id"]);

						if (reader["numero"] != null && !Convert.IsDBNull(reader["numero"]))
						{
							item.NumeroProtocolo = Convert.ToInt32(reader["numero"]);
						}

						if (reader["ano"] != null && !Convert.IsDBNull(reader["ano"]))
						{
							item.Ano = Convert.ToInt32(reader["ano"]);
						}

						item.Tipo.Texto = reader["tipo_texto"].ToString();

						if (reader["interessado_id"] != null && !Convert.IsDBNull(reader["interessado_id"]))
						{
							item.Interessado.Id = Convert.ToInt32(reader["interessado_id"]);
							item.Interessado.Tipo = Convert.ToInt32(reader["interessado_tipo"]);

							if (reader["interessado_tipo"].ToString() == "1")
							{
								item.Interessado.Fisica.Nome = reader["interessado_nome_razao"].ToString();
							}
							else
							{
								item.Interessado.Juridica.RazaoSocial = reader["interessado_nome_razao"].ToString();
							}
						}

						if (reader["empreendimento_id"] != null && !Convert.IsDBNull(reader["empreendimento_id"]))
						{
							item.Empreendimento.Id = Convert.ToInt32(reader["empreendimento_id"]);
							item.Empreendimento.Denominador = reader["empreendimento_denominador"].ToString();
						}

						if (reader["situacao_id"] != null && !Convert.IsDBNull(reader["situacao_id"]))
						{
							item.SituacaoId = Convert.ToInt32(reader["situacao_id"]);
							item.SituacaoTexto = reader["situacao_texto"].ToString();
						}

						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
		}

		#endregion
	}
}
