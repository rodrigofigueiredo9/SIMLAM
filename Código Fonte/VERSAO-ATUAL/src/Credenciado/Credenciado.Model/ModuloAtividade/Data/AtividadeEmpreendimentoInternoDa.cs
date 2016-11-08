using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloAtividade.Data
{
	public class AtividadeEmpreendimentoInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public AtividadeEmpreendimentoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal Resultados<EmpreendimentoAtividade> Filtrar(Dictionary<string, DadoFiltro> filtros)
		{
			Resultados<EmpreendimentoAtividade> retorno = new Resultados<EmpreendimentoAtividade>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if (filtros.ContainsKey("secao"))
				{
					comandtxt += " and upper(secao) like upper(:secao)";
					comando.AdicionarParametroEntrada("secao", filtros["secao"].Valor + "%", filtros["secao"].Tipo);
				}

				if (filtros.ContainsKey("divisao"))
				{
					comandtxt += " and divisao = :divisao ";
					comando.AdicionarParametroEntrada("divisao", filtros["divisao"].Valor, filtros["divisao"].Tipo);
				}

				if (filtros.ContainsKey("atividade"))
				{
					comandtxt += " and upper(atividade) like upper(:atividade)";
					comando.AdicionarParametroEntrada("atividade", filtros["atividade"].Valor + "%", filtros["atividade"].Tipo);
				}

				if (filtros.ContainsKey("cnae"))
				{
					comandtxt += " and upper(cod_cnae) like upper(:cnae)";
					comando.AdicionarParametroEntrada("cnae", filtros["cnae"].Valor + "%", filtros["cnae"].Tipo);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "cod_cnae", "atividade" };

				if (filtros.ContainsKey("ordenar"))
				{
					ordenar.Add(filtros["ordenar"].Valor.ToString());
				}
				else
				{
					ordenar.Add("cod_cnae");
				}
				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_empreendimento_atividade a where a.id > 0" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Mensagem msgSemRegistros = Mensagem.Funcionario.NaoEncontrouRegistros;
					Validacao.Add(msgSemRegistros);
				}

				if (filtros.ContainsKey("menor"))
				{
					comando.AdicionarParametroEntrada("menor", filtros["menor"].Valor, filtros["menor"].Tipo);
				}
				else
				{
					comando.AdicionarParametroEntrada("menor", 1, DbType.Int32);
				}

				if (filtros.ContainsKey("maior"))
				{
					comando.AdicionarParametroEntrada("maior", filtros["maior"].Valor, filtros["maior"].Tipo);
				}
				else
				{
					comando.AdicionarParametroEntrada("maior", 10, DbType.Int32);
				}

				comandtxt = String.Format(@"select a.id, a.secao, a.divisao, a.atividade, a.tid, a.cod_cnae from 
				{0}tab_empreendimento_atividade a where a.id > 0" + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno
					EmpreendimentoAtividade atividade;
					while (reader.Read())
					{
						atividade = new EmpreendimentoAtividade();
						atividade.Id = Convert.ToInt32(reader["id"]);
						atividade.Secao = reader["secao"].ToString();
						atividade.CNAE = reader["cod_cnae"].ToString();

						if (reader["divisao"] != null && !Convert.IsDBNull(reader["divisao"]))
						{
							atividade.Divisao = Convert.ToInt32(reader["divisao"]);
						}
						atividade.Atividade = reader["atividade"].ToString();
						atividade.Tid = reader["tid"].ToString();

						retorno.Itens.Add(atividade);
					}

					reader.Close();
					#endregion
				}
			}

			return retorno;
		}
	}
}