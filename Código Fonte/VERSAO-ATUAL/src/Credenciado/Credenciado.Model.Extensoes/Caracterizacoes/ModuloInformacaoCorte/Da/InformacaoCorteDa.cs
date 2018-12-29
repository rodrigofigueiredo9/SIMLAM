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
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;

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
					insert into crt_inf_corte_licenca (id, tid, corte_id, licenca, tipo_licenca, numero_licenca, atividade, area_licenca, data_vencimento) values
					(seq_inf_corte_licenca.nextval, :tid, :corte_id, :licenca, :tipo_licenca, :numero_licenca, :atividade, :area_licenca, :data_vencimento)", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("licenca", item.Licenca > 0 ? item.Licenca : null, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_licenca", item.TipoLicenca, DbType.String);
					comando.AdicionarParametroEntrada("numero_licenca", item.NumeroLicenca, DbType.Int64);
					comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.String);
					comando.AdicionarParametroEntrada("area_licenca", item.AreaLicenca, DbType.String);
					comando.AdicionarParametroEntrada("data_vencimento", item.DataVencimento.Data, DbType.Date);
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
						insert into crt_inf_corte_licenca (id, tid, corte_id, licenca, tipo_licenca, numero_licenca, atividade, area_licenca, data_vencimento) values
						(seq_inf_corte_licenca.nextval, :tid, :corte_id, :licenca, :tipo_licenca, :numero_licenca, :atividade, :area_licenca, :data_vencimento)", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("licenca", item.Licenca > 0 ? item.Licenca : null, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_licenca", item.TipoLicenca, DbType.String);
						comando.AdicionarParametroEntrada("numero_licenca", item.NumeroLicenca, DbType.Int64);
						comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.String);
						comando.AdicionarParametroEntrada("area_licenca", item.AreaLicenca, DbType.String);
						comando.AdicionarParametroEntrada("data_vencimento", item.DataVencimento.Data, DbType.Date);
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

		internal void ExcluirPorEmpreendimento(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_informacao_corte c where c.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					int id = reader.GetValue<int>("id");

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
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Histórico

				//Atualizar o tid para a nova ação
				var comando = bancoDeDados.CriarComandoPlSql(@"
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

		internal List<InformacaoCorte> ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			var caracterizacao = new List<InformacaoCorte>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_informacao_corte t where t.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var id = reader.GetValue<int>("id");
						if (id > 0)
							caracterizacao.Add(Obter(id, bancoDeDados, simplificado));
					}
					reader.Close();
				}
			}

			return caracterizacao;
		}

		internal InformacaoCorte Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Informação de Corte

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento, c.data_informacao, c.area_flor_plantada
				from {0}crt_informacao_corte c where c.id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId= reader.GetValue<int>("empreendimento");
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
				select c.id, c.tid, c.corte_id, c.licenca, c.tipo_licenca, c.numero_licenca, c.atividade, c.area_licenca, c.data_vencimento
				from {0}crt_inf_corte_licenca c
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
							TipoLicenca = reader.GetValue<string>("tipo_licenca"),
							NumeroLicenca = reader.GetValue<string>("numero_licenca"),
							Atividade = reader.GetValue<string>("atividade"),
							AreaLicenca = reader.GetValue<decimal>("area_licenca"),
							DataVencimento = new DateTecno() { Data = reader.GetValue<DateTime>("data_vencimento") }
						});
					}

					reader.Close();
				}

				#endregion

				#region Informação de Corte Tipo

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.corte_id, c.tipo_corte, c.especie, c.area_corte, c.idade_plantio
				from {0}crt_inf_corte_tipo c
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
							TipoCorteTexto = Enum.ToObject(typeof(eTipoCorte), reader.GetValue<int>("tipo_corte")).ToDescription(),
							EspecieInformada = reader.GetValue<int>("especie"),
							EspecieInformadaTexto = Enum.ToObject(typeof(eEspecieInformada), reader.GetValue<int>("especie")).ToDescription(),
							AreaCorte = reader.GetValue<decimal>("area_corte"),
							IdadePlantio = reader.GetValue<int>("idade_plantio")
						};

						#region Informação Corte Destinação

						comando = bancoDeDados.CriarComando(@"
						select c.id, c.tid, c.tipo_corte_id, c.dest_material, c.produto, c.quantidade, lv.texto as dest_material_texto,
							lvp.texto as produto_texto
						from {0}crt_inf_corte_dest_material c
						left join {1}lov_crt_inf_corte_inf_dest_mat lv
							on(c.dest_material = lv.id)
						left join {1}lov_crt_produto lvp
							on(c.produto = lvp.id)
						where c.tipo_corte_id = :tipo_corte_id", EsquemaCredenciadoBanco, "idaf");

						comando.AdicionarParametroEntrada("tipo_corte_id", informacaoCorteTipo.Id, DbType.Int32);

						using (IDataReader readerDestinacao = bancoDeDados.ExecutarReader(comando))
						{
							while (readerDestinacao.Read())
							{
								informacaoCorteTipo.InformacaoCorteDestinacao.Add(new InformacaoCorteDestinacao()
								{
									Id = readerDestinacao.GetValue<int>("id"),
									Tid = readerDestinacao.GetValue<string>("tid"),
									TipoCorteId = readerDestinacao.GetValue<int>("tipo_corte_id"),
									DestinacaoMaterial = readerDestinacao.GetValue<int>("dest_material"),
									DestinacaoMaterialTexto = readerDestinacao.GetValue<string>("dest_material_texto"),
									Produto = readerDestinacao.GetValue<int>("produto"),
									ProdutoTexto = readerDestinacao.GetValue<string>("produto_texto"),
									Quantidade = readerDestinacao.GetValue<int>("quantidade")
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
				select c.id, c.tid, c.empreendimento, c.data_informacao, c.area_flor_plantada, " +
				(esquema == EsquemaCredenciadoBanco ? "c.interno_id, c.interno_tid," : "c.id as interno_id, c.tid as interno_tid,") + @"
				(select sum(t.area_corte) from {0}crt_inf_corte_tipo t where t.corte_id = c.id) area_corte
				from {0}crt_informacao_corte c where c.empreendimento = :id" + (esquema == EsquemaCredenciadoBanco ? " and c.interno_id is null" : ""), esquema);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.Add(new InformacaoCorte()
						{
							Id = reader.GetValue<int>("id"),
							InternoID = reader.GetValue<int>("interno_id"),
							DataInformacao = new DateTecno() { Data = reader.GetValue<DateTime>("data_informacao") },
							AreaFlorestaPlantada = reader.GetValue<decimal>("area_flor_plantada"),
							AreaCorteCalculada = reader.GetValue<decimal>("area_corte"),
							InternoTID = reader.GetValue<string>("interno_tid"),
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


		#endregion
	}
}