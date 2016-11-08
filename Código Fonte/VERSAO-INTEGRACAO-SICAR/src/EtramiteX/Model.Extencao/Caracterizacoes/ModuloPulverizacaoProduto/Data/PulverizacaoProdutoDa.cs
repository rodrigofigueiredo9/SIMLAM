using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloPulverizacaoProduto.Data
{
	public class PulverizacaoProdutoDa
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

		public PulverizacaoProdutoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(PulverizacaoProduto caracterizacao, BancoDeDados banco)
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

		internal int? Criar(PulverizacaoProduto caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Pulverização Aérea de Produtos Agrotóxicos

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_pulveriz_prod_agr c(id, empreendimento, atividade, empresa_prestadora, cnpj, geometria_coord_atv_x, geometria_coord_atv_y, geometria_id, geometria_tipo, tid)
															values(seq_crt_pulveriz_prod_agr.nextval, :empreendimento, :atividade, :empresa_prestadora, :cnpj, :geometria_coord_atv_x, :geometria_coord_atv_y, :geometria_id, :geometria_tipo, :tid ) 
															returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("empresa_prestadora", caracterizacao.EmpresaPrestadora, DbType.String);
				comando.AdicionarParametroEntrada("cnpj", caracterizacao.CNPJ, DbType.String);
				comando.AdicionarParametroEntrada("geometria_id", caracterizacao.CoordenadaAtividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_tipo", caracterizacao.CoordenadaAtividade.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_coord_atv_x", caracterizacao.CoordenadaAtividade.CoordX, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_coord_atv_y", caracterizacao.CoordenadaAtividade.CoordY, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Culturas

				if (caracterizacao.Culturas != null && caracterizacao.Culturas.Count > 0)
				{
					foreach (Cultura item in caracterizacao.Culturas)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_pulveriz_prod_cult c(id, caracterizacao, tipo, especificar, area, tid)
															values(seq_crt_pulveriz_prod_cult.nextval, :caracterizacao, :tipo, :especificar, :area, :tid)
															returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", item.TipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("especificar", String.IsNullOrWhiteSpace(item.EspecificarTexto) ? (Object)DBNull.Value : item.EspecificarTexto, DbType.String);
						comando.AdicionarParametroEntrada("area", item.Area, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.pulverizacaoproduto, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(PulverizacaoProduto caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Pulverização Aérea de Produtos Agrotóxicos

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_pulveriz_prod_agr c set c.empreendimento = :empreendimento, c.atividade = :atividade, 
															c.empresa_prestadora = :empresa_prestadora, c.cnpj = :cnpj, c.geometria_coord_atv_x = :geometria_coord_atv_x,
															c.geometria_coord_atv_y = :geometria_coord_atv_y, c.geometria_id = :geometria_id, 
															c.geometria_tipo = :geometria_tipo, c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", caracterizacao.Atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("empresa_prestadora", caracterizacao.EmpresaPrestadora, DbType.String);
				comando.AdicionarParametroEntrada("cnpj", caracterizacao.CNPJ, DbType.String);
				comando.AdicionarParametroEntrada("geometria_id", caracterizacao.CoordenadaAtividade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_tipo", caracterizacao.CoordenadaAtividade.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("geometria_coord_atv_x", caracterizacao.CoordenadaAtividade.CoordX, DbType.Decimal);
				comando.AdicionarParametroEntrada("geometria_coord_atv_y", caracterizacao.CoordenadaAtividade.CoordY, DbType.Decimal);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Culturas
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_pulveriz_prod_cult c where c.caracterizacao = :caracterizacao", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Culturas.Select(x => x.Id).ToList()));

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);


				#endregion

				#region Culturas

				if (caracterizacao.Culturas != null && caracterizacao.Culturas.Count > 0)
				{
					foreach (Cultura item in caracterizacao.Culturas)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_pulveriz_prod_cult c set c.caracterizacao = :caracterizacao,
																c.tipo = :tipo, c.especificar = :especificar, c.area= :area, c.tid = :tid 
																where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_pulveriz_prod_cult c (id, caracterizacao, tipo, especificar, area, tid)
																values (seq_crt_pulveriz_prod_cult.nextval, :caracterizacao, :tipo, :especificar, :area, :tid )
																returning c.id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", item.TipoId, DbType.Int32);
						comando.AdicionarParametroEntrada("especificar", String.IsNullOrWhiteSpace(item.EspecificarTexto) ? (Object)DBNull.Value : item.EspecificarTexto, DbType.String);
						comando.AdicionarParametroEntrada("area", item.Area, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.pulverizacaoproduto, eHistoricoAcao.atualizar, bancoDeDados, null);

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

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_pulveriz_prod_agr c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_pulveriz_prod_agr c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.pulverizacaoproduto, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_pulveriz_prod_cult r where r.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_pulveriz_prod_agr e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.PulverizacaoProduto, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal PulverizacaoProduto ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			PulverizacaoProduto caracterizacao = new PulverizacaoProduto();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_pulveriz_prod_agr s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal PulverizacaoProduto Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			PulverizacaoProduto caracterizacao = new PulverizacaoProduto();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_pulveriz_prod_agr s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal PulverizacaoProduto Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			PulverizacaoProduto caracterizacao = new PulverizacaoProduto();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Pulverização Aérea de Produtos Agrotóxicos

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.atividade, c.empresa_prestadora, c.cnpj, c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															c.geometria_id, c.geometria_tipo, c.tid  from {0}crt_pulveriz_prod_agr c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						caracterizacao.Atividade = Convert.ToInt32(reader["atividade"]);
						caracterizacao.EmpresaPrestadora = reader["empresa_prestadora"].ToString();
						caracterizacao.CNPJ = reader["cnpj"].ToString();
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

				#region Culturas

				comando = bancoDeDados.CriarComando(@"select s.id, s.tipo tipo_id, l.texto tipo_texto, s.especificar, s.area, s.tid 
													from {0}crt_pulveriz_prod_cult s, lov_crt_pulveriz_prod_agr_cult l
													where s.caracterizacao = :caracterizacao and l.id = s.tipo order by s.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Cultura cultura = null;

					while (reader.Read())
					{
						cultura = new Cultura();
						cultura.Id = reader.GetValue<Int32>("id");
						cultura.TipoId = reader.GetValue<Int32>("tipo_id");
						cultura.EspecificarTexto = reader.GetValue<String>("especificar");
						cultura.TipoTexto = (String.IsNullOrWhiteSpace(cultura.EspecificarTexto)) ? reader.GetValue<String>("tipo_texto") : cultura.EspecificarTexto;
						cultura.Area = reader.GetValue<Decimal>("area").ToStringTrunc(precisao: 4);
						cultura.Tid = reader.GetValue<String>("tid");

						caracterizacao.Culturas.Add(cultura);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		private PulverizacaoProduto ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			PulverizacaoProduto caracterizacao = new PulverizacaoProduto();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Pulverização Aérea de Produtos Agrotóxicos

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.atividade_id, c.empresa_prestadora, c.cnpj, 
															c.geometria_coord_atv_x, c.geometria_coord_atv_y, c.geometria_id, c.geometria_tipo, 
															c.tid from {0}hst_crt_pulveriz_prod_agr c  where c.caracterizacao = :id 
															and c.tid = :tid", EsquemaBanco);

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
						caracterizacao.EmpresaPrestadora = reader["empresa_prestadora"].ToString();
						caracterizacao.CNPJ = reader["cnpj"].ToString();
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

				#region Culturas

				comando = bancoDeDados.CriarComando(@"select s.cultura id, s.tipo_id, s.tipo_id, s.tipo_texto, s.especificar, 
													s.area, s.tid from {0}hst_crt_pulveriz_prod_cult s where s.id_hst = :id_hst 
													order by s.cultura", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Cultura cultura = null;

					if (reader.Read())
					{
						cultura = new Cultura();
						cultura.Id = reader.GetValue<Int32>("id");
						cultura.TipoId = reader.GetValue<Int32>("tipo_id");
						cultura.EspecificarTexto = reader.GetValue<String>("especificar");
						cultura.TipoTexto = (String.IsNullOrWhiteSpace(cultura.EspecificarTexto)) ? reader.GetValue<String>("tipo_texto") : cultura.EspecificarTexto;
						cultura.Area = reader.GetValue<Decimal>("area").ToStringTrunc(precisao: 4);
						cultura.Tid = reader.GetValue<String>("tid");

						caracterizacao.Culturas.Add(cultura);
					}

					reader.Close();
				}

				#endregion

			}

			return caracterizacao;

		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			//Coordenada da atividade
			CaracterizacaoPDF caract = new CaracterizacaoPDF();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select c.geometria_coord_atv_x, c.geometria_coord_atv_y,
															(select sum(p.area) from crt_pulveriz_prod_cult p where p.caracterizacao = c.id) area_total 
															from {0}crt_pulveriz_prod_agr c where c.empreendimento = :empreendimento 
															and c.atividade = :atividade", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["geometria_coord_atv_x"] != null && !Convert.IsDBNull(reader["geometria_coord_atv_x"]))
						{
							caract.EastingLongitude = Convert.ToDecimal(reader["geometria_coord_atv_x"]).ToString("F2");
							caract.NorthingLatitude = Convert.ToDecimal(reader["geometria_coord_atv_y"]).ToString("F2");
						}

						if (reader["area_total"] != null && !Convert.IsDBNull(reader["area_total"]))
						{
							caract.Campos.Add(new CaracterizacaoCampoPDF() { Nome = "Área total de pulverização (ha)", Valor = Convert.ToDecimal(reader["area_total"]).ToStringTrunc(4) });
						}
					}

					reader.Close();
				}
			}

			return caract;
		}

		#endregion

		#region Validacoes

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
