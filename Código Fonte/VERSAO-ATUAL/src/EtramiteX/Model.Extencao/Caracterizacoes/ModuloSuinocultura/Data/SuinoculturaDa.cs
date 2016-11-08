using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSuinocultura;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSuinocultura.Data
{
	public class SuinoculturaDa
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

		public SuinoculturaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Suinocultura caracterizacao, BancoDeDados banco)
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

		internal int? Criar(Suinocultura caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Suinocultura

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_suinocultura c (id, empreendimento, atividade, fase, existe_biodigestor, possui_fabrica_racao, numero_max_cabecas, numero_max_matrizes,  geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
															values(seq_crt_suinocultura.nextval, :empreendimento, :atividade, :fase, :existe_biodigestor, :possui_fabrica_racao, :numero_max_cabecas, :numero_max_matrizes, :geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) 
															returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);

				comando.AdicionarParametroEntrada("fase", caracterizacao.Fase, DbType.Int32);
				comando.AdicionarParametroEntrada("existe_biodigestor", caracterizacao.ExisteBiodigestor.GetValueOrDefault(0), DbType.Int32);
				comando.AdicionarParametroEntrada("possui_fabrica_racao", caracterizacao.PossuiFabricaRacao.GetValueOrDefault(0), DbType.Int32);
				comando.AdicionarParametroEntrada("numero_max_cabecas", (String.IsNullOrWhiteSpace(caracterizacao.NumeroCabecas)) ? (object)DBNull.Value : caracterizacao.NumeroCabecas, DbType.Decimal);
				comando.AdicionarParametroEntrada("numero_max_matrizes", (String.IsNullOrWhiteSpace(caracterizacao.NumeroMatrizes)) ? (object)DBNull.Value : caracterizacao.NumeroMatrizes, DbType.Decimal);

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

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.suinocultura, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(Suinocultura caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Suinocultura

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_suinocultura c set c.empreendimento = :empreendimento, c.atividade = :atividade, c.fase = :fase,
															c.geometria_coord_atv_x = :geometria_coord_atv_x, c.existe_biodigestor = :existe_biodigestor, c.possui_fabrica_racao = :possui_fabrica_racao,
															c.numero_max_cabecas = :numero_max_cabecas, c.numero_max_matrizes = :numero_max_matrizes, c.geometria_coord_atv_y = :geometria_coord_atv_y, c.geometria_id = :geometria_id, c.geometria_tipo = :geometria_tipo, c.tid = :tid
															where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("fase", caracterizacao.Fase, DbType.Int32);
				comando.AdicionarParametroEntrada("existe_biodigestor", caracterizacao.ExisteBiodigestor.GetValueOrDefault(0), DbType.Int32);
				comando.AdicionarParametroEntrada("possui_fabrica_racao", caracterizacao.PossuiFabricaRacao.GetValueOrDefault(0), DbType.Int32);
				comando.AdicionarParametroEntrada("numero_max_cabecas", (String.IsNullOrWhiteSpace(caracterizacao.NumeroCabecas)) ? (object)DBNull.Value : caracterizacao.NumeroCabecas, DbType.Decimal);
				comando.AdicionarParametroEntrada("numero_max_matrizes", (String.IsNullOrWhiteSpace(caracterizacao.NumeroMatrizes)) ? (object)DBNull.Value : caracterizacao.NumeroMatrizes, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_id", caracterizacao.CoordenadaAtividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_tipo", caracterizacao.CoordenadaAtividade.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_coord_atv_x", caracterizacao.CoordenadaAtividade.CoordX, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_coord_atv_y", caracterizacao.CoordenadaAtividade.CoordY, DbType.Decimal);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.suinocultura, eHistoricoAcao.atualizar, bancoDeDados, null);

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

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_suinocultura c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_suinocultura c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.suinocultura, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_suinocultura e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.Suinocultura, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal Suinocultura ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Suinocultura caracterizacao = new Suinocultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_suinocultura s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal Suinocultura Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Suinocultura caracterizacao = new Suinocultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_suinocultura s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal Suinocultura Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Suinocultura caracterizacao = new Suinocultura();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Suinocultura

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.atividade, c.fase ,lf.texto fase_texto, c.existe_biodigestor,
															c.possui_fabrica_racao, c.numero_max_cabecas, c.numero_max_matrizes, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															c.geometria_id, c.geometria_tipo, c.tid  from {0}crt_suinocultura c, lov_crt_suinocultura_fase lf 
															where c.id = :id and lf.id = c.fase", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);

						caracterizacao.Atividade = Convert.ToInt32(reader["atividade"]);
						caracterizacao.Fase = Convert.ToInt32(reader["fase"]);
						caracterizacao.ExisteBiodigestor = Convert.ToInt32(reader["existe_biodigestor"]);
						caracterizacao.PossuiFabricaRacao = Convert.ToInt32(reader["possui_fabrica_racao"]);
						caracterizacao.CoordenadaAtividade.Id = Convert.ToInt32(reader["geometria_id"]);
						caracterizacao.CoordenadaAtividade.Tipo = Convert.ToInt32(reader["geometria_tipo"]);

						if (reader["numero_max_cabecas"] != null && !Convert.IsDBNull(reader["numero_max_cabecas"]))
						{
							caracterizacao.NumeroCabecas = Convert.ToDecimal(reader["numero_max_cabecas"]).ToString("N0");
						}

						if (reader["numero_max_matrizes"] != null && !Convert.IsDBNull(reader["numero_max_matrizes"]))
						{
							caracterizacao.NumeroMatrizes = Convert.ToDecimal(reader["numero_max_matrizes"]).ToString("N0");
						}

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

		private Suinocultura ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Suinocultura caracterizacao = new Suinocultura();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Suinocultura

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.atividade_id, c.fase, lf.texto fase_texto, c.existe_biodigestor, c.possui_fabrica_racao,
															c.numero_max_cabecas, c.numero_max_matrizes, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															c.geometria_id, c.geometria_tipo, c.tid from {0}hst_crt_suinocultura c, lov_crt_suinocultura_fase lf
															where c.caracterizacao = :id and c.tid = :tid and lf.id = c.fase", EsquemaBanco);

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
						caracterizacao.Fase = Convert.ToInt32(reader["fase"]);
						caracterizacao.ExisteBiodigestor = Convert.ToInt32(reader["existe_biodigestor"]);
						caracterizacao.PossuiFabricaRacao = Convert.ToInt32(reader["possui_fabrica_racao"]);
						caracterizacao.NumeroCabecas = reader["numero_max_cabecas"].ToString();
						caracterizacao.NumeroMatrizes = reader["numero_max_matrizes"].ToString();
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

			}

			return caracterizacao;

		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			//Número máximo de cabeças
			//Número máximo de matrizes
			CaracterizacaoPDF caract = new CaracterizacaoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select c.numero_max_cabecas, c.numero_max_matrizes, c.geometria_coord_atv_x, c.geometria_coord_atv_y
															from {0}crt_suinocultura c where c.empreendimento = :empreendimento and c.atividade = :atividade", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						if (reader["numero_max_cabecas"] != null && !Convert.IsDBNull(reader["numero_max_cabecas"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Número máximo de cabeças", Valor = Convert.ToDecimal(reader["numero_max_cabecas"]).ToString("N2") });
						}

						if (reader["numero_max_matrizes"] != null && !Convert.IsDBNull(reader["numero_max_matrizes"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Número máximo de matrizes", Valor = Convert.ToDecimal(reader["numero_max_matrizes"]).ToString("N2") });
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