using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDespolpamentoCafe;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDespolpamentoCafe.Data
{
	public class DespolpamentoCafeDa
	{

		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		internal Historico Historico { get { return _historico; } }
		private String EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public DespolpamentoCafeDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(DespolpamentoCafe caracterizacao, BancoDeDados banco)
		{
			if (caracterizacao == null)
			{
				throw new Exception("A Caracterização é nula.");
			}

			if (caracterizacao.Id <= 0)
			{
				Criar(caracterizacao, banco);
			}
			else
			{
				Editar(caracterizacao, banco);
			}
		}

		internal int? Criar(DespolpamentoCafe caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Despolpamento/Descascamento de Café

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_despolpamento_cafe c(id, empreendimento, atividade, capacidade_total_instalada, agua_residuaria_cafe, geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
															values(seq_crt_despolpamento_cafe.nextval, :empreendimento, :atividade, :capacidade_total_instalada, :agua_residuaria_cafe, :geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) 
															returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("capacidade_total_instalada", caracterizacao.CapacidadeTotalInstalada, DbType.Decimal);
				comando.AdicionarParametroEntrada("agua_residuaria_cafe", (String.IsNullOrWhiteSpace(caracterizacao.AguaResiduariaCafe)) ? (object)DBNull.Value : caracterizacao.AguaResiduariaCafe, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_id", caracterizacao.CoordenadaAtividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_tipo", caracterizacao.CoordenadaAtividade.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_coord_atv_x", caracterizacao.CoordenadaAtividade.CoordX, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_coord_atv_y", caracterizacao.CoordenadaAtividade.CoordY, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.despolpamentocafe, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(DespolpamentoCafe caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Despolpamento/Descascamento de Café

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_despolpamento_cafe c set c.empreendimento = :empreendimento, c.atividade = :atividade, c.capacidade_total_instalada = :capacidade_total_instalada,
															c.agua_residuaria_cafe = :agua_residuaria_cafe ,c.geometria_coord_atv_x = :geometria_coord_atv_x, c.geometria_coord_atv_y = :geometria_coord_atv_y, c.geometria_id = :geometria_id, c.geometria_tipo = :geometria_tipo, c.tid = :tid
															where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("capacidade_total_instalada", caracterizacao.CapacidadeTotalInstalada, DbType.Decimal);
				comando.AdicionarParametroEntrada("agua_residuaria_cafe", (String.IsNullOrWhiteSpace(caracterizacao.AguaResiduariaCafe)) ? (object)DBNull.Value : caracterizacao.AguaResiduariaCafe, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_id", caracterizacao.CoordenadaAtividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_tipo", caracterizacao.CoordenadaAtividade.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_coord_atv_x", caracterizacao.CoordenadaAtividade.CoordX, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_coord_atv_y", caracterizacao.CoordenadaAtividade.CoordY, DbType.Decimal);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.despolpamentocafe, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_despolpamento_cafe c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_despolpamento_cafe c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.despolpamentocafe, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_despolpamento_cafe e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.DespolpamentoCafe, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal DespolpamentoCafe ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			DespolpamentoCafe caracterizacao = new DespolpamentoCafe();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_despolpamento_cafe s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal DespolpamentoCafe Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			DespolpamentoCafe caracterizacao = new DespolpamentoCafe();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_despolpamento_cafe s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal DespolpamentoCafe Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			DespolpamentoCafe caracterizacao = new DespolpamentoCafe();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Despolpamento/Descascamento de Café

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.atividade, c.capacidade_total_instalada, c.agua_residuaria_cafe, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															c.geometria_id, c.geometria_tipo, c.tid  from {0}crt_despolpamento_cafe c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						caracterizacao.Atividade = Convert.ToInt32(reader["atividade"]);
						caracterizacao.CapacidadeTotalInstalada = reader["capacidade_total_instalada"].ToString();
						caracterizacao.AguaResiduariaCafe = reader["agua_residuaria_cafe"].ToString();
						caracterizacao.CoordenadaAtividade.Id = Convert.ToInt32(reader["geometria_id"]);

						caracterizacao.CoordenadaAtividade.Tipo = Convert.ToInt32(reader["geometria_tipo"]);

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caracterizacao.CoordenadaAtividade.CoordX = Convert.ToDecimal(reader["geometria_coord_atv_x"]);
						}

						if (reader["geometria_coord_atv_y"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_y"]))
						{
							caracterizacao.CoordenadaAtividade.CoordY = Convert.ToDecimal(reader["geometria_coord_atv_y"]);
						}

						caracterizacao.Tid = reader["tid"].ToString();
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

			}

			return caracterizacao;
		}

		private DespolpamentoCafe ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			DespolpamentoCafe caracterizacao = new DespolpamentoCafe();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Despolpamento/Descascamento de Café

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.atividade_id, c.capacidade_total_instalada, c.agua_residuaria_cafe, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															c.geometria_id, c.geometria_tipo, c.tid from {0}hst_crt_despolpamento_cafe c 
															where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = Convert.ToInt32(reader["id"]);

						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento_id"]);
						caracterizacao.Atividade = Convert.ToInt32(reader["atividade_id"]);
						caracterizacao.CapacidadeTotalInstalada = reader["capacidade_total_instalada"].ToString();
						caracterizacao.AguaResiduariaCafe = reader["agua_residuaria_cafe"].ToString();
						caracterizacao.CoordenadaAtividade.Id = Convert.ToInt32(reader["geometria_id"]);
						caracterizacao.CoordenadaAtividade.Tipo = Convert.ToInt32(reader["geometria_tipo"]);
						caracterizacao.Tid = reader["tid"].ToString();

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caracterizacao.CoordenadaAtividade.CoordX = Convert.ToDecimal(reader["geometria_coord_atv_x"]);
						}

						if (reader["geometria_coord_atv_y"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_y"]))
						{
							caracterizacao.CoordenadaAtividade.CoordY = Convert.ToDecimal(reader["geometria_coord_atv_y"]);
						}
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

			}

			return caracterizacao;

		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			CaracterizacaoPDF caract = new CaracterizacaoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select c.capacidade_total_instalada, c.geometria_coord_atv_x, 
					c.geometria_coord_atv_y from {0}crt_despolpamento_cafe c where c.empreendimento = :empreendimento and c.atividade = :atividade", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["capacidade_total_instalada"] != null && !Convert.IsDBNull(reader["capacidade_total_instalada"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Capacidade instalada total (L/hora)", Valor = reader["capacidade_total_instalada"].ToString() });
						}

						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caract.EastingLongitude = Convert.ToDecimal(reader["geometria_coord_atv_x"]).ToString("F2");
							caract.NorthingLatitude = Convert.ToDecimal(reader["geometria_coord_atv_y"]).ToString("F2");
						}
					}

					reader.Close();
				}
			}

			return caract;
		}

		#endregion
	}
}