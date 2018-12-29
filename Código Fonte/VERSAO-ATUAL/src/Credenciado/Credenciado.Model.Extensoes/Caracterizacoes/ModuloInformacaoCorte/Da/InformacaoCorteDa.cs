using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data
{
	public class InformacaoCorteDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }

		private String EsquemaBanco { get; set; }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		public EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region Ações DML

		internal void Salvar(InformacaoCorte caracterizacao, Blocos.Data.BancoDeDados banco = null)
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

		private int Criar(InformacaoCorte caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Informação Corte

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_informacao_corte
				(id, tid, empreendimento, data_informacao, area_flor_plantada, interno_id, interno_tid) values
				(seq_informacao_corte.nextval, :tid, :empreendimento_id, :data_informacao, :area_flor_plantada, :interno_id, :interno_tid)
				returning id into :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento_id", caracterizacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_informacao", caracterizacao.DataInformacao.Data, DbType.Date);
				comando.AdicionarParametroEntrada("area_flor_plantada", caracterizacao.AreaFlorestaPlantada, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoID > 0 ? caracterizacao.InternoID : null, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacao.InternoTID);
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Informação Corte Licença

				foreach (var item in caracterizacao.InformacaoCorteLicenca)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into crt_inf_corte_licenca (id, tid, corte_id, licenca, numero_licenca, atividade, area_licenca) values
					(seq_inf_corte_licenca.nextval, :tid, :corte_id, :licenca, :numero_licenca, :atividade, :area_licenca)", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("licenca", item.Licenca > 0 ? item.Licenca : null, DbType.Int32);
					comando.AdicionarParametroEntrada("numero_licenca", item.NumeroLicenca, DbType.Int64);
					comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.String);
					comando.AdicionarParametroEntrada("area_licenca", item.AreaLicenca, DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Informação Corte Tipo Corte

				foreach (var item in caracterizacao.InformacaoCorteTipo)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into crt_inf_corte_tipo (id, tid, corte_id, tipo_corte, especie, area_corte, idade_plantio) values
					(seq_inf_corte_tipo.nextval, :tid, :corte_id, :tipo_corte, :especie, :area_corte, :idade_plantio)
					returning id into :tipo_id", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_corte", item.TipoCorte, DbType.Int32);
					comando.AdicionarParametroEntrada("especie", item.EspecieInformada, DbType.Int64);
					comando.AdicionarParametroEntrada("area_corte", item.AreaCorte, DbType.Decimal);
					comando.AdicionarParametroEntrada("idade_plantio", item.IdadePlantio, DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("tipo_id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = comando.ObterValorParametro<int>("tipo_id");

					foreach (var destinacao in item.InformacaoCorteDestinacao)
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_inf_corte_dest_material (id, tid, tipo_corte_id, dest_material, produto, quantidade) values
						(seq_inf_corte_dest_material.nextval, :tid, :tipo_corte_id, :dest_material, :produto, :quantidade)", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("tipo_corte_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("dest_material", destinacao.DestinacaoMaterial, DbType.Int32);
						comando.AdicionarParametroEntrada("produto", destinacao.Produto, DbType.Int32);
						comando.AdicionarParametroEntrada("quantidade", destinacao.Quantidade, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

					}
				}

				#endregion

				#endregion Informação de Corte

				//Histórico
				//Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.criar, bancoDeDados, null);

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		private void Editar(InformacaoCorte caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Informação de Corte

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_informacao_corte set empreendimento = :empreendimento, data_informacao = :data_informacao, area_flor_plantada = :area_flor_plantada, tid = :tid where id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_informacao", caracterizacao.DataInformacao.Data, DbType.Date);
				comando.AdicionarParametroEntrada("area_flor_plantada", caracterizacao.AreaFlorestaPlantada, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Informação Corte Licença

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_inf_corte_licenca c where c.corte_id = :corte_id ", EsquemaCredenciadoBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.InformacaoCorteLicenca.Where(x => x.Id > 0).Select(y => y.Id).ToList());
				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in caracterizacao.InformacaoCorteLicenca)
				{
					if (item.Id == 0)
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_inf_corte_licenca (id, tid, corte_id, licenca, numero_licenca, atividade, area_licenca) values
						(seq_inf_corte_licenca.nextval, :tid, :corte_id, :licenca, :numero_licenca, :atividade, :area_licenca)", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("licenca", item.Licenca > 0 ? item.Licenca : null, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_licenca", item.NumeroLicenca, DbType.Int64);
						comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.Int32);
						comando.AdicionarParametroEntrada("area_licenca", item.AreaLicenca, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Informação Corte Licença

				#region Informação Corte Tipo

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_inf_corte_tipo c where c.corte_id = :corte_id ", EsquemaCredenciadoBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.InformacaoCorteTipo.Where(x => x.Id > 0).Select(y => y.Id).ToList());
				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in caracterizacao.InformacaoCorteTipo)
				{
					if (item.Id == 0)
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_inf_corte_tipo (id, tid, corte_id, tipo_corte, especie, area_corte, idade_plantio) values
						(seq_inf_corte_tipo.nextval, :tid, :corte_id, :tipo_corte, :especie, :area_corte, :idade_plantio)
						returning id into :tipo_id", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_corte", item.TipoCorte, DbType.Int32);
						comando.AdicionarParametroEntrada("especie", item.EspecieInformada, DbType.Int64);
						comando.AdicionarParametroEntrada("area_corte", item.AreaCorte, DbType.Decimal);
						comando.AdicionarParametroEntrada("idade_plantio", item.IdadePlantio, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("tipo_id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = comando.ObterValorParametro<int>("tipo_id");

						foreach (var destinacao in item.InformacaoCorteDestinacao)
						{
							comando = bancoDeDados.CriarComando(@"
						insert into crt_inf_corte_dest_material (id, tid, tipo_corte_id, dest_material, produto, quantidade) values
						(seq_inf_corte_dest_material.nextval, :tid, :tipo_corte_id, :dest_material, :produto, :quantidade)", EsquemaCredenciadoBanco);

							comando.AdicionarParametroEntrada("tipo_corte_id", item.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("dest_material", destinacao.DestinacaoMaterial, DbType.Int32);
							comando.AdicionarParametroEntrada("produto", destinacao.Produto, DbType.Int32);
							comando.AdicionarParametroEntrada("quantidade", destinacao.Quantidade, DbType.Decimal);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

							bancoDeDados.ExecutarNonQuery(comando);

						}
					}
				}

				#endregion Informação Corte Tipo

				//Histórico
				//Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.atualizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_informacao_corte c where c.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
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
				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					update {0}crt_informacao_corte c set c.tid = :tid where c.id = :id;
					update {0}crt_inf_corte_licenca c set c.tid = :tid where c.corte_id = :id;
					update {0}crt_inf_corte_tipo c set c.tid = :tid where c.corte_id = :id;
					update {0}crt_inf_corte_dest_material c set c.tid = :tid where c.tipo_corte_id in (select t.id from {0}crt_inf_corte_tipo t where t.tid = :tid and t.corte_id = :id);
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin
					delete from {0}crt_informacao_corte c where c.id = :id;
					delete from {0}crt_inf_corte_licenca c where c.corte_id = :id;
					delete from {0}crt_inf_corte_tipo c where c.corte_id = :id;
					delete from {0}crt_inf_corte_dest_material c where c.tipo_corte_id in (select t.id from {0}crt_inf_corte_tipo t where t.tid = :tid and t.corte_id = :id);
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter/Filtrar

		internal InformacaoCorte ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_informacao_corte t where t.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal InformacaoCorte Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informação de Corte

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento, c.data_informacao, c.area_flor_plantada, e.codigo empreendimento_codigo
				from {0}crt_informacao_corte c, tab_empreendimento e where c.empreendimento = e.id and c.id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						caracterizacao.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
						caracterizacao.DataInformacao = new DateTecno() { Data = reader.GetValue<DateTime>("data_informacao") };
						caracterizacao.AreaFlorestaPlantada = reader.GetValue<decimal>("area_flor_plantada");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");
						caracterizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Informação de Corte Licença

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.corte_id, c.licenca, c.numero_licenca, c.atividade, c.area_licenca
				from crt_inf_corte_licenca c
				where c.corte_id = :corte_id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.InformacaoCorteLicenca.Add(new InformacaoCorteLicenca()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							Corte = reader.GetValue<int>("corte_id"),
							Licenca = reader.GetValue<int>("licenca"),
							NumeroLicenca = reader.GetValue<string>("numero_licenca"),
							Atividade = reader.GetValue<string>("atividade"),
							AreaLicenca = reader.GetValue<decimal>("area_licenca")
						});
					}

					reader.Close();
				}

				#endregion

				#region Informação de Corte Tipo

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.corte_id, c.tipo_corte, c.especie, c.area_corte, c.idade_plantio
				from crt_inf_corte_tipo c
				where c.corte_id = :corte_id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var informacaoCorteTipo = new InformacaoCorteTipo()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							Corte = reader.GetValue<int>("corte_id"),
							TipoCorte = reader.GetValue<int>("tipo_corte"),
							EspecieInformada = reader.GetValue<int>("especie"),
							AreaCorte = reader.GetValue<decimal>("area_corte"),
							IdadePlantio = reader.GetValue<int>("idade_plantio")
						};

						#region Informação Corte Destinação

						comando = bancoDeDados.CriarComando(@"
						select c.id, c.tid, c.tipo_corte_id, c.dest_material, c.produto, c.quantidade
						from crt_inf_corte_dest_material c
						where c.tipo_corte_id = :tipo_corte_id", EsquemaBanco);

						comando.AdicionarParametroEntrada("tipo_corte_id", informacaoCorteTipo.Id, DbType.Int32);

						using (IDataReader readerDestinacao = bancoDeDados.ExecutarReader(comando))
						{
							while (readerDestinacao.Read())
							{
								informacaoCorteTipo.InformacaoCorteDestinacao.Add(new InformacaoCorteDestinacao()
								{
									Id = reader.GetValue<int>("id"),
									Tid = reader.GetValue<string>("tid"),
									TipoCorteId = reader.GetValue<int>("tipo_corte_id"),
									DestinacaoMaterial = reader.GetValue<int>("DestinacaoMaterial"),
									Produto = reader.GetValue<int>("produto"),
									Quantidade = reader.GetValue<int>("quantidade")
								});
							}

							readerDestinacao.Close();
						}

						#endregion

						caracterizacao.InformacaoCorteTipo.Add(informacaoCorteTipo);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal List<InformacaoCorte> FiltrarPorEmpreendimento(int id, BancoDeDados banco = null, string esquema = null)
		{
			var caracterizacao = new List<InformacaoCorte>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informação de Corte

				if (esquema == null) esquema = EsquemaBanco;

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.empreendimento, c.data_informacao, c.area_flor_plantada,
				(select sum(t.area_corte) from {0}crt_inf_corte_tipo t where t.corte_id = c.id) area_corte
				from {0}crt_informacao_corte c where c.empreendimento = :id", esquema);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.Add(new InformacaoCorte()
						{
							Id = reader.GetValue<int>("id"),
							DataInformacao = new DateTecno() { Data = reader.GetValue<DateTime>("data_informacao") },
							AreaFlorestaPlantada = reader.GetValue<decimal>("area_flor_plantada"),
							AreaCorteCalculada = reader.GetValue<decimal>("area_corte"),
							Tid = reader.GetValue<string>("tid")
						});
					}

					reader.Close();
				}

				#endregion

				if (esquema == EsquemaBanco)
				{
					var credenciado = this.FiltrarPorEmpreendimento(id, banco, EsquemaCredenciadoBanco);
					if (credenciado?.Count > 0)
						caracterizacao.AddRange(credenciado);
				}
			}

			return caracterizacao;
		}

		internal List<Lista> ObterListaInfCorteEmpreendimento(int empreendimento)
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				/*SELECT id, concat(concat(lpad(id, 4, '0'), '-'), data_informacao) informacaoCorte FROM crt_informacao_corte;*/
					SELECT ID, (LPAD(ID, 4, '0') || ' - ' || DATA_INFORMACAO) informacaoCorte
						FROM {0}CRT_INFORMACAO_CORTE CRT WHERE EMPREENDIMENTO = :empreendimento
					/*AND ID NOT IN (SELECT ID FROM TAB_EMPREENDIMENTO E WHERE CRT.ID = E.ID)*/", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Lista item = new Lista();
						item.Id = reader.GetValue<string>("ID");
						item.Texto = reader.GetValue<string>("informacaoCorte");

						retorno.Add(item);
					}

					reader.Close();
				}

				return retorno;
			}
		}
		
		internal List<Lista> ObterListaInfCorteTitulo(int titulo)
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				SELECT CRT.ID, LPAD(CRT.ID, 4, '0') || ' - ' || DATA_INFORMACAO informacaoCorte
					FROM {0}CRT_INFORMACAO_CORTE CRT 
					INNER JOIN ESP_OUT_INFORMACAO_CORTE INF ON CRT.id = INF.INFORMACAO_CORTE
				WHERE INF.TITULO = :titulo", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Lista item = new Lista();
						item.Id = reader.GetValue<string>("ID");
						item.Texto = reader.GetValue<string>("informacaoCorte");
						item.IsAtivo = true;

						retorno.Add(item);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		#endregion
	}
}