using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data
{
	public class InformacaoCorteDa
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

		public InformacaoCorteDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(InformacaoCorte caracterizacao, BancoDeDados banco = null)
		{
			if (caracterizacao == null)
				throw new Exception("A Caracterização é nula.");

			if (caracterizacao.Id <= 0)
				Criar(caracterizacao, banco);
			else
				Editar(caracterizacao, banco);
		}

		private int Criar(InformacaoCorte caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Informação Corte

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_informacao_corte
				(id, tid, empreendimento, data_informacao, area_flor_plantada) values
				(seq_crt_informacao_corte.nextval, :tid, :empreendimento_id, :data_informacao, :area_flor_plantada)
				returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento_id", caracterizacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_informacao", caracterizacao.DataInformacao.Data, DbType.Date);
				comando.AdicionarParametroEntrada("area_flor_plantada", caracterizacao.AreaFlorestaPlantada, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Informação Corte Licença

				foreach (var item in caracterizacao.InformacaoCorteLicenca)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_inf_corte_licenca (id, tid, corte_id, licenca, tipo_licenca, numero_licenca, atividade, area_licenca, data_vencimento) values
					(seq_inf_corte_licenca.nextval, :tid, :corte_id, :licenca, :tipo_licenca, :numero_licenca, :atividade, :area_licenca, :data_vencimento)", EsquemaBanco);

					comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("licenca", item.Licenca > 0 ? item.Licenca : null, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_licenca", item.TipoLicenca, DbType.String);
					comando.AdicionarParametroEntrada("numero_licenca", item.NumeroLicenca, DbType.String);
					comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.String);
					comando.AdicionarParametroEntrada("area_licenca", item.AreaLicenca, DbType.Decimal);
					comando.AdicionarParametroEntrada("data_vencimento", item.DataVencimento.Data, DbType.Date);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				#region Informação Corte Tipo Corte

				foreach (var item in caracterizacao.InformacaoCorteTipo)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_inf_corte_tipo (id, tid, corte_id, tipo_corte, especie, area_corte, idade_plantio) values
					(seq_inf_corte_tipo.nextval, :tid, :corte_id, :tipo_corte, :especie, :area_corte, :idade_plantio)
					returning id into :tipo_id", EsquemaBanco);

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
						insert into {0}crt_inf_corte_dest_material (id, tid, tipo_corte_id, dest_material, produto, quantidade, inf_codigo_sefaz) values
						(seq_inf_corte_dest_material.nextval, :tid, :tipo_corte_id, :dest_material, :produto, :quantidade,
						(select s.id from lov_inf_codigo_sefaz s where s.inf_corte_produto = :produto and s.inf_destinacao_material = :dest_material and rownum = 1))", EsquemaBanco);

						comando.AdicionarParametroEntrada("tipo_corte_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("dest_material", destinacao.DestinacaoMaterial, DbType.Int32);
						comando.AdicionarParametroEntrada("produto", destinacao.Produto, DbType.Int32);
						comando.AdicionarParametroEntrada("quantidade", destinacao.Quantidade, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

					}
				}

				#endregion

				#endregion Informação de Corte

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		private void Editar(InformacaoCorte caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informação de Corte

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_informacao_corte set empreendimento = :empreendimento, data_informacao = :data_informacao, area_flor_plantada = :area_flor_plantada, tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_informacao", caracterizacao.DataInformacao.Data, DbType.Date);
				comando.AdicionarParametroEntrada("area_flor_plantada", caracterizacao.AreaFlorestaPlantada, DbType.Decimal);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Informação Corte Licença

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_inf_corte_licenca c where c.corte_id = :corte_id ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.InformacaoCorteLicenca.Where(x => x.Id > 0).Select(y => y.Id).ToList());
				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in caracterizacao.InformacaoCorteLicenca)
				{
					if (item.Id == 0)
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_inf_corte_licenca (id, tid, corte_id, licenca, tipo_licenca, numero_licenca, atividade, area_licenca, data_vencimento) values
						(seq_inf_corte_licenca.nextval, :tid, :corte_id, :licenca, :tipo_licenca, :numero_licenca, :atividade, :area_licenca, :data_vencimento)", EsquemaBanco);

						comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("licenca", item.Licenca > 0 ? item.Licenca : null, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo_licenca", item.TipoLicenca, DbType.String);
						comando.AdicionarParametroEntrada("numero_licenca", item.NumeroLicenca, DbType.String);
						comando.AdicionarParametroEntrada("atividade", item.Atividade, DbType.String);
						comando.AdicionarParametroEntrada("area_licenca", item.AreaLicenca, DbType.Decimal);
						comando.AdicionarParametroEntrada("data_vencimento", item.DataVencimento.Data, DbType.Date);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion Informação Corte Licença

				#region Informação Corte Tipo

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_inf_corte_tipo c where c.corte_id = :corte_id ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.InformacaoCorteTipo.Where(x => x.Id > 0).Select(y => y.Id).ToList());
				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in caracterizacao.InformacaoCorteTipo)
				{
					if (item.Id == 0)
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_inf_corte_tipo (id, tid, corte_id, tipo_corte, especie, area_corte, idade_plantio) values
						(seq_inf_corte_tipo.nextval, :tid, :corte_id, :tipo_corte, :especie, :area_corte, :idade_plantio)
						returning id into :tipo_id", EsquemaBanco);

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
							insert into {0}crt_inf_corte_dest_material (id, tid, tipo_corte_id, dest_material, produto, quantidade, inf_codigo_sefaz) values
							(seq_inf_corte_dest_material.nextval, :tid, :tipo_corte_id, :dest_material, :produto, :quantidade,
							(select s.id from lov_inf_codigo_sefaz s where s.inf_corte_produto = :produto and s.inf_destinacao_material = :dest_material and rownum = 1))", EsquemaBanco);

							comando.AdicionarParametroEntrada("tipo_corte_id", item.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("dest_material", destinacao.DestinacaoMaterial, DbType.Int32);
							comando.AdicionarParametroEntrada("produto", destinacao.Produto, DbType.Int32);
							comando.AdicionarParametroEntrada("quantidade", destinacao.Quantidade, DbType.Int32);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

							bancoDeDados.ExecutarNonQuery(comando);

						}
					}
				}

				#endregion Informação Corte Tipo

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void ExcluirPorEmpreendimento(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();


				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_informacao_corte c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				end;", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);

					Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.excluir, bancoDeDados, null);

					#endregion

					#region Apaga os dados da caracterização

					comando = bancoDeDados.CriarComandoPlSql(
					@"begin
					delete from {0}crt_informacao_corte c where c.id = :id;
					delete from {0}crt_inf_corte_licenca c where c.corte_id = :id;
					delete from {0}crt_inf_corte_tipo c where c.corte_id = :id;
					delete from {0}crt_inf_corte_dest_material c where c.tipo_corte_id in (select t.id from {0}crt_inf_corte_tipo t where t.tid = :tid and t.corte_id = :id);
				end;", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					bancoDeDados.ExecutarNonQuery(comando);
					bancoDeDados.Commit();

					#endregion
				}
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
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
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.informacaocorte, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin
					delete from {0}crt_informacao_corte c where c.id = :id;
					delete from {0}crt_inf_corte_licenca c where c.corte_id = :id;
					delete from {0}crt_inf_corte_tipo c where c.corte_id = :id;
					delete from {0}crt_inf_corte_dest_material c where c.tipo_corte_id in (select t.id from {0}crt_inf_corte_tipo t where t.tid = :tid and t.corte_id = :id);
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal List<InformacaoCorte> ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			var caracterizacao = new List<InformacaoCorte>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_informacao_corte t where t.empreendimento = :empreendimento", EsquemaBanco);
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

		internal InformacaoCorte Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_informacao_corte s where s.id = :id " + (tid == null ? "" : "and s.tid = :tid"), EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				if (tid != null)
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
			}

			return caracterizacao;
		}

		private InformacaoCorte Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informação de Corte

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.empreendimento, c.data_informacao, c.area_flor_plantada
				from {0}crt_informacao_corte c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						caracterizacao.DataInformacao = new DateTecno() { Data = reader.GetValue<DateTime>("data_informacao") };
						caracterizacao.AreaFlorestaPlantada = reader.GetValue<decimal>("area_flor_plantada");
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
				where c.corte_id = :corte_id", EsquemaBanco);

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
				where c.corte_id = :corte_id", EsquemaBanco);

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
						select c.id, c.tid, c.tipo_corte_id, c.dest_material, c.produto, c.quantidade, c.inf_codigo_sefaz,
							lv.texto as dest_material_texto, lvp.texto as produto_texto
						from {0}crt_inf_corte_dest_material c
						left join {0}lov_crt_inf_corte_inf_dest_mat lv
							on(c.dest_material = lv.id)
						left join {0}lov_crt_produto lvp
							on(c.produto = lvp.id)
						where c.tipo_corte_id = :tipo_corte_id", EsquemaBanco);

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
									Quantidade = readerDestinacao.GetValue<int>("quantidade"),
									CodigoSefazId = readerDestinacao.GetValue<int>("inf_codigo_sefaz")
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

		private InformacaoCorte ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informação de Corte

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.empreendimento, c.data_informacao, c.area_flor_plantada
				from {0}hst_crt_informacao_corte c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						caracterizacao.DataInformacao = new DateTecno() { Data = reader.GetValue<DateTime>("data_informacao") };
						caracterizacao.AreaFlorestaPlantada = reader.GetValue<decimal>("area_flor_plantada");
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
				from {0}hst_crt_inf_corte_licenca c
				where c.corte_id = :corte_id", EsquemaBanco);

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
				from {0}hst_crt_inf_corte_tipo c
				where c.corte_id = :corte_id", EsquemaBanco);

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
						select c.id, c.tid, c.tipo_corte_id, c.dest_material, c.produto, c.quantidade, c.inf_codigo_sefaz, c.dest_material_texto, c.produto_texto
						from {0}hst_crt_inf_corte_dest_material c
						where c.tipo_corte_id = :tipo_corte_id", EsquemaBanco);

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
									Quantidade = readerDestinacao.GetValue<int>("quantidade"),
									CodigoSefazId = readerDestinacao.GetValue<int>("inf_codigo_sefaz")
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

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.empreendimento, c.data_informacao, c.area_flor_plantada, 
				(select sum(t.area_corte) from {0}crt_inf_corte_tipo t where t.corte_id = c.id) area_corte
				from {0}crt_informacao_corte c where c.empreendimento = :id", EsquemaBanco);

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
					AND ID NOT IN (SELECT ID FROM TAB_EMPREENDIMENTO E WHERE CRT.ID = E.ID)", EsquemaBanco);

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
				WHERE INF.TITULO = :titulo", EsquemaBanco);

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

		internal List<Lista> ObterProdutos(int destinacaoId)
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select s.inf_corte_produto, p.texto from {0}lov_inf_codigo_sefaz s
				inner join lov_crt_produto_inf_corte p
				on (p.id = s.inf_corte_produto)
				where inf_destinacao_material = :inf_destinacao_material", EsquemaBanco);

				comando.AdicionarParametroEntrada("inf_destinacao_material", destinacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Lista item = new Lista();
						item.Id = reader.GetValue<string>("inf_corte_produto");
						item.Texto = reader.GetValue<string>("texto");
						item.IsAtivo = true;

						retorno.Add(item);
					}

					reader.Close();
				}

				return retorno;
			}
		}

		#endregion

		internal bool PossuiCaracterizacaoEmAberto(int empreendimentoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(t.id) from {0}crt_informacao_corte t where t.empreendimento = :empreendimento
					and not exists(select 1 from {0}esp_out_informacao_corte e where e.informacao_corte = t.id and e.validade is not null)", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}
	}
}