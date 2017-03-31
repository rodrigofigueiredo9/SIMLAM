using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade.PDF;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data
{
	public class UnidadeProducaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		internal Historico Historico { get { return _historico; } }
		private String EsquemaBanco { get; set; }
		private String EsquemaBancoCredenciado { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); ; } }


		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public UnidadeProducaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(UnidadeProducao caracterizacao, BancoDeDados banco)
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

		internal int? Criar(UnidadeProducao caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_unidade_producao
				(id, tid, empreendimento, possui_cod_propriedade, propriedade_codigo, local_livro) values
				(seq_crt_unidade_producao.nextval, :tid, :empreendimento_id, :possui_cod_propriedade, :propriedade_codigo, :local_livro)
				returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento_id", caracterizacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_cod_propriedade", caracterizacao.PossuiCodigoPropriedade, DbType.Int32);
				comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Unidades de Produção

				foreach (UnidadeProducaoItem item in caracterizacao.UnidadesProducao)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into crt_unidade_producao_unidade (id, tid, unidade_producao, possui_cod_up, codigo_up, tipo_producao, renasem,
					renasem_data_validade, area, cultivar, data_plantio_ano_producao, estimativa_quant_ano, estimativa_unid_medida, ano_abertura, cultura) values
					(seq_crt_un_producao_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
					:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
					returning id into :unidade_id", EsquemaBanco);

					comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("possui_cod_up", item.PossuiCodigoUP, DbType.Int32);
					comando.AdicionarParametroEntrada("codigo_up", item.CodigoUP, DbType.Int64);
					comando.AdicionarParametroEntrada("tipo_producao", item.TipoProducao, DbType.Int32);
					comando.AdicionarParametroEntrada("renasem", item.RenasemNumero, DbType.String);
					comando.AdicionarParametroEntrada("renasem_data_validade", item.DataValidadeRenasem, DbType.String);
					comando.AdicionarParametroEntrada("area", item.AreaHA, DbType.Decimal);
					comando.AdicionarParametroEntrada("cultivar", item.CultivarId, DbType.Int32);
					comando.AdicionarParametroEntrada("data_plantio_ano_producao", item.DataPlantioAnoProducao, DbType.String);
					comando.AdicionarParametroEntrada("estimativa_quant_ano", item.EstimativaProducaoQuantidadeAno, DbType.Decimal);
					comando.AdicionarParametroEntrada("estimativa_unid_medida", item.EstimativaProducaoUnidadeMedidaId, DbType.Int32);
					comando.AdicionarParametroEntrada("ano_abertura", item.AnoAbertura, DbType.Int32);
					comando.AdicionarParametroEntrada("cultura", item.CulturaId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("unidade_id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = comando.ObterValorParametro<int>("unidade_id");

					#region Coordenadas

					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_unidade_producao_un_coord
					(id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, municipio) values
					(seq_crt_un_producao_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaBanco);

					comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_coordenada", item.Coordenada.Tipo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum", item.Coordenada.Datum.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("easting_utm", item.Coordenada.EastingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("northing_utm", item.Coordenada.NorthingUtm, DbType.Double);
					comando.AdicionarParametroEntrada("fuso_utm", item.Coordenada.FusoUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("hemisferio_utm", item.Coordenada.HemisferioUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("municipio", item.Municipio.Id > 0 ? (object)item.Municipio.Id : DBNull.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);

					#endregion

					#region Produtores

					comando = bancoDeDados.CriarComando(@"
					insert into crt_unidade_prod_un_produtor (id, tid, unidade_producao_unidade, produtor) values
					(seq_crt_unidade_prod_un_produt.nextval, :tid, :unidade_producao_unidade, :produtor)");

					comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("produtor", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (var produtor in item.Produtores)
					{
						comando.SetarValorParametro("produtor", produtor.Id);
						bancoDeDados.ExecutarNonQuery(comando);
					}

					#endregion

					#region Responsaveis Técnicos Habilitados CFO / CFOC

					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_unidade_prod_un_resp_tec
					(id, tid, unidade_producao_unidade, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art) values
					(seq_crt_un_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaBanco);

					comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("responsavel_tecnico", DbType.Int32);
					comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", DbType.String);
					comando.AdicionarParametroEntrada("numero_art", DbType.String);
					comando.AdicionarParametroEntrada("art_cargo_funcao", DbType.Int32);
					comando.AdicionarParametroEntrada("data_validade_art", DbType.String);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (var responsavel in item.ResponsaveisTecnicos)
					{
						comando.SetarValorParametro("responsavel_tecnico", responsavel.Id);
						comando.SetarValorParametro("numero_hab_cfo_cfoc", responsavel.CFONumero);
						comando.SetarValorParametro("numero_art", responsavel.NumeroArt);
						comando.SetarValorParametro("art_cargo_funcao", responsavel.ArtCargoFuncao);
						comando.SetarValorParametro("data_validade_art", responsavel.DataValidadeART);
						bancoDeDados.ExecutarNonQuery(comando);
					}

					#endregion
				}

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.criar, bancoDeDados, null);

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(UnidadeProducao caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Unidade de Produção

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_unidade_producao set propriedade_codigo = :propriedade_codigo, local_livro = :local_livro, tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Deletando dados das tabelas filhas

				//Coordenada
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_unidade_producao_un_coord c where c.unidade_producao_unidade in
				(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.Select(x => x.Coordenada).Select(y => y.Id).ToList());
				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Produtores
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_unidade_prod_un_produtor c where c.unidade_producao_unidade in
				(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.SelectMany(x => x.Produtores).Select(y => y.IdRelacionamento).ToList());
				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Responsaveis Técnicos
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_unidade_prod_un_resp_tec c where c.unidade_producao_unidade in
				(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.SelectMany(x => x.ResponsaveisTecnicos).Select(y => y.IdRelacionamento).ToList());
				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Unidades de Produção
				comando = bancoDeDados.CriarComando("delete from {0}crt_unidade_producao_unidade c where c.unidade_producao = :unidade_producao", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.Select(x => x.Id).ToList());
				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Unidade de Produção

				foreach (UnidadeProducaoItem item in caracterizacao.UnidadesProducao)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update {0}crt_unidade_producao_unidade set cultura =:cultura,  tid = :tid, codigo_up = :codigo_up, tipo_producao = :tipo_producao, renasem =:renasem,
						renasem_data_validade = :renasem_data_validade, area = :area, cultivar = :cultivar, data_plantio_ano_producao = :data_plantio_ano_producao,
						estimativa_quant_ano = :estimativa_quant_ano, estimativa_unid_medida = :estimativa_unid_medida, ano_abertura = :ano_abertura where id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_unidade_producao_unidade (id, tid, unidade_producao, possui_cod_up, codigo_up, tipo_producao, renasem,
						renasem_data_validade, area, cultivar, data_plantio_ano_producao, estimativa_quant_ano, estimativa_unid_medida, ano_abertura, cultura) values
						(seq_crt_un_producao_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
						:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
						returning id into :unidade_id", EsquemaBanco);

						comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("possui_cod_up", item.PossuiCodigoUP, DbType.Int32);
						comando.AdicionarParametroSaida("unidade_id", DbType.Int32);
					}

					comando.AdicionarParametroEntrada("codigo_up", item.CodigoUP, DbType.Int64);
					comando.AdicionarParametroEntrada("tipo_producao", item.TipoProducao, DbType.Int32);
					comando.AdicionarParametroEntrada("renasem", item.RenasemNumero, DbType.String);
					comando.AdicionarParametroEntrada("renasem_data_validade", item.DataValidadeRenasem, DbType.String);
					comando.AdicionarParametroEntrada("area", item.AreaHA, DbType.Decimal);
					comando.AdicionarParametroEntrada("cultivar", item.CultivarId > 0 ? item.CultivarId : (object)DBNull.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("data_plantio_ano_producao", item.DataPlantioAnoProducao, DbType.String);
					comando.AdicionarParametroEntrada("estimativa_quant_ano", item.EstimativaProducaoQuantidadeAno, DbType.Decimal);
					comando.AdicionarParametroEntrada("estimativa_unid_medida", item.EstimativaProducaoUnidadeMedidaId, DbType.Int32);
					comando.AdicionarParametroEntrada("ano_abertura", item.AnoAbertura, DbType.Int32);
					comando.AdicionarParametroEntrada("cultura", item.CulturaId, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);

					if (item.Id < 1)
					{
						item.Id = comando.ObterValorParametro<int>("unidade_id");
					}

					#region Coordenadas

					if (item.Coordenada.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update {0}crt_unidade_producao_un_coord set tid = :tid, tipo_coordenada = :tipo_coordenada, datum = :datum, easting_utm = :easting_utm,
						northing_utm = :northing_utm, fuso_utm = :fuso_utm, hemisferio_utm = :hemisferio_utm, municipio = :municipio
						where unidade_producao_unidade = :unidade_producao_unidade", EsquemaBanco);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_unidade_producao_un_coord
						(id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, municipio) values
						(seq_crt_un_producao_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaBanco);
					}

					comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_coordenada", item.Coordenada.Tipo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("datum", item.Coordenada.Datum.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("easting_utm", item.Coordenada.EastingUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("northing_utm", item.Coordenada.NorthingUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("fuso_utm", item.Coordenada.FusoUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("hemisferio_utm", item.Coordenada.HemisferioUtm, DbType.Int32);
					comando.AdicionarParametroEntrada("municipio", item.Municipio.Id > 0 ? (object)item.Municipio.Id : DBNull.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);

					#endregion

					#region Produtores

					foreach (var produtor in item.Produtores)
					{
						if (produtor.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"update crt_unidade_prod_un_produtor set produtor = :produtor, tid = :tid where id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", produtor.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into crt_unidade_prod_un_produtor (id, tid, unidade_producao_unidade, produtor) values
							(seq_crt_unidade_prod_un_produt.nextval, :tid, :unidade_producao_unidade, :produtor)");
							comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("produtor", produtor.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}

					#endregion

					#region Responsáveis técnicos

					foreach (ResponsavelTecnico responsavel in item.ResponsaveisTecnicos)
					{
						if (responsavel.IdRelacionamento > 0)
						{
							comando = bancoDeDados.CriarComando(@"
							update crt_unidade_prod_un_resp_tec set tid =:tid, responsavel_tecnico =: responsavel_tecnico, numero_hab_cfo_cfoc =: numero_hab_cfo_cfoc,
							numero_art =:numero_art, art_cargo_funcao =: art_cargo_funcao, data_validade_art =: data_validade_art where id = :id_rel", EsquemaBanco);

							comando.AdicionarParametroEntrada("id_rel", responsavel.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_unidade_prod_un_resp_tec
							(id, tid, unidade_producao_unidade, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art) values
							(seq_crt_un_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaBanco);

							comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("responsavel_tecnico", responsavel.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", responsavel.CFONumero, DbType.String);
						comando.AdicionarParametroEntrada("art_cargo_funcao", responsavel.ArtCargoFuncao, DbType.Int32);
						comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt, DbType.String);
						comando.AdicionarParametroEntrada("data_validade_art", responsavel.DataValidadeART, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					}

					#endregion
				}

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.atualizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_unidade_producao c where c.empreendimento = :empreendimento", EsquemaBanco);
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
					update {0}crt_unidade_producao c set c.tid = :tid where c.id = :id;
					update {0}crt_unidade_producao_unidade c set c.tid = :tid where c.unidade_producao = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(
				@"begin
					delete from crt_unidade_prod_un_resp_tec where unidade_producao_unidade in((select id from crt_unidade_producao_unidade where unidade_producao = :unidade_producao));
					delete from crt_unidade_prod_un_produtor where unidade_producao_unidade in((select id from crt_unidade_producao_unidade where unidade_producao = :unidade_producao));
					delete from crt_unidade_producao_un_coord where unidade_producao_unidade in((select id from crt_unidade_producao_unidade where unidade_producao = :unidade_producao));
					delete from crt_unidade_producao_unidade where unidade_producao = :unidade_producao;
					delete from crt_unidade_producao where id = :unidade_producao;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("unidade_producao", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void CopiarDadosCredenciado(UnidadeProducao caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				#region Caracterização Principal

				if (caracterizacao.Id <= 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into crt_unidade_producao (id, tid, empreendimento, possui_cod_propriedade, propriedade_codigo, local_livro)
					values (seq_crt_unidade_producao.nextval, :tid, :empreendimento_id, :possui_cod_propriedade, :propriedade_codigo, :local_livro) returning id into :id");

					comando.AdicionarParametroSaida("id", DbType.Int32);
					comando.AdicionarParametroEntrada("possui_cod_propriedade", caracterizacao.PossuiCodigoPropriedade, DbType.Int32);
					comando.AdicionarParametroEntrada("empreendimento_id", caracterizacao.Empreendimento.Id, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update crt_unidade_producao set propriedade_codigo = :propriedade_codigo, local_livro = :local_livro, tid = :tid where id = :id");

					comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (caracterizacao.Id <= 0)
				{
					caracterizacao.Id = comando.ObterValorParametro<int>("id");
				}
				caracterizacao.Tid = GerenciadorTransacao.ObterIDAtual();

				#endregion

				#region Limpando listas

				comando = bancoDeDados.CriarComandoPlSql(string.Empty);

				comando.DbCommand.CommandText = string.Format(@"
				begin
					delete from crt_unidade_prod_un_resp_tec t where t.unidade_producao_unidade in (select u.id from crt_unidade_producao_unidade u where u.unidade_producao = :unidade {0});
					delete from crt_unidade_prod_un_produtor t where t.unidade_producao_unidade in (select u.id from crt_unidade_producao_unidade u where u.unidade_producao = :unidade {0});
					delete from crt_unidade_producao_un_coord c where c.unidade_producao_unidade in (select u.id from crt_unidade_producao_unidade u where u.unidade_producao = :unidade {0});
					delete from crt_unidade_producao_unidade u where u.unidade_producao = :unidade {0};
				end;", comando.AdicionarIn("and", "u.codigo_up", DbType.Int64, caracterizacao.UnidadesProducao.Select(x => x.CodigoUP).ToList())).Replace('\r', ' ');

				comando.AdicionarParametroEntrada("unidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Unidade de Produção Item

				comando = bancoDeDados.CriarComando(@"insert into crt_unidade_producao_unidade (id, tid, unidade_producao, possui_cod_up, codigo_up, tipo_producao, renasem,
						renasem_data_validade, area, cultivar, data_plantio_ano_producao, estimativa_quant_ano, estimativa_unid_medida, ano_abertura, cultura) values
						(seq_crt_un_producao_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
						:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
						returning id into :unidade_id");

				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("possui_cod_up", DbType.Int32);
				comando.AdicionarParametroEntrada("codigo_up", DbType.Int64);
				comando.AdicionarParametroEntrada("tipo_producao", DbType.Int32);
				comando.AdicionarParametroEntrada("renasem", DbType.String);
				comando.AdicionarParametroEntrada("renasem_data_validade", DbType.String);
				comando.AdicionarParametroEntrada("area", DbType.Decimal);
				comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
				comando.AdicionarParametroEntrada("data_plantio_ano_producao", DbType.String);
				comando.AdicionarParametroEntrada("estimativa_quant_ano", DbType.Decimal);
				comando.AdicionarParametroEntrada("estimativa_unid_medida", DbType.Int32);
				comando.AdicionarParametroEntrada("ano_abertura", DbType.Int32);
				comando.AdicionarParametroEntrada("cultura", DbType.Int32);
				comando.AdicionarParametroSaida("unidade_id", DbType.Int32);

				caracterizacao.UnidadesProducao.ForEach(item =>
				{
					comando.SetarValorParametro("possui_cod_up", item.PossuiCodigoUP);
					comando.SetarValorParametro("codigo_up", item.CodigoUP);
					comando.SetarValorParametro("tipo_producao", item.TipoProducao);
					comando.SetarValorParametro("renasem", item.RenasemNumero);
					comando.SetarValorParametro("renasem_data_validade", item.DataValidadeRenasem);
					comando.SetarValorParametro("area", item.AreaHA);
					comando.SetarValorParametro("cultivar", item.CultivarId);
					comando.SetarValorParametro("data_plantio_ano_producao", item.DataPlantioAnoProducao);
					comando.SetarValorParametro("estimativa_quant_ano", item.EstimativaProducaoQuantidadeAno);
					comando.SetarValorParametro("estimativa_unid_medida", item.EstimativaProducaoUnidadeMedidaId);
					comando.SetarValorParametro("ano_abertura", item.AnoAbertura);
					comando.SetarValorParametro("cultura", item.CulturaId);

					bancoDeDados.ExecutarNonQuery(comando);
					item.Id = comando.ObterValorParametro<int>("unidade_id");

					#region Coordenadas

					Comando cmdAux = bancoDeDados.CriarComando(@" insert into {0}crt_unidade_producao_un_coord  (id, tid, unidade_producao_unidade, tipo_coordenada, datum,
					easting_utm, northing_utm, fuso_utm, hemisferio_utm, municipio) values  (seq_crt_un_producao_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada,
					:datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaBanco);

					cmdAux.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("tipo_coordenada", item.Coordenada.Tipo.Id, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("datum", item.Coordenada.Datum.Id, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("easting_utm", item.Coordenada.EastingUtm, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("northing_utm", item.Coordenada.NorthingUtm, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("fuso_utm", item.Coordenada.FusoUtm, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("hemisferio_utm", item.Coordenada.HemisferioUtm, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("municipio", item.Municipio.Id > 0 ? (object)item.Municipio.Id : DBNull.Value, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(cmdAux);

					#endregion

					#region Produtores

					cmdAux = bancoDeDados.CriarComando(@"
					insert into crt_unidade_prod_un_produtor (id, tid, unidade_producao_unidade, produtor) values
					(seq_crt_unidade_prod_un_produt.nextval, :tid, :unidade_producao_unidade, :produtor)");

					cmdAux.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("produtor", DbType.Int32);
					cmdAux.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (var produtor in item.Produtores)
					{
						cmdAux.SetarValorParametro("produtor", produtor.Id);
						bancoDeDados.ExecutarNonQuery(cmdAux);
					}

					#endregion

					#region Responsáveis técnicos

					cmdAux = bancoDeDados.CriarComando(@"insert into {0}crt_unidade_prod_un_resp_tec (id, tid, unidade_producao_unidade, responsavel_tecnico, numero_hab_cfo_cfoc,
					numero_art, art_cargo_funcao, data_validade_art) values  (seq_crt_un_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico,
					:numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaBanco);

					cmdAux.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					cmdAux.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					cmdAux.AdicionarParametroEntrada("responsavel_tecnico", DbType.Int32);
					cmdAux.AdicionarParametroEntrada("numero_hab_cfo_cfoc", DbType.String);
					cmdAux.AdicionarParametroEntrada("art_cargo_funcao", DbType.Int32);
					cmdAux.AdicionarParametroEntrada("numero_art", DbType.String);
					cmdAux.AdicionarParametroEntrada("data_validade_art", DbType.String);

					item.ResponsaveisTecnicos.ForEach(responsavel =>
					{
						cmdAux.SetarValorParametro("responsavel_tecnico", responsavel.Id);
						cmdAux.SetarValorParametro("numero_hab_cfo_cfoc", responsavel.CFONumero);
						cmdAux.SetarValorParametro("art_cargo_funcao", responsavel.ArtCargoFuncao);
						cmdAux.SetarValorParametro("numero_art", responsavel.NumeroArt);
						cmdAux.SetarValorParametro("data_validade_art", responsavel.DataValidadeART);

						bancoDeDados.ExecutarNonQuery(cmdAux);
					});

					#endregion
				});

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.importar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal UnidadeProducao ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeProducao caracterizacao = new UnidadeProducao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_unidade_producao t where t.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), empreendimento, bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal UnidadeProducao Obter(int id, int idEmpreendimento, BancoDeDados banco = null, bool simplificado = false)
		{
			UnidadeProducao caracterizacao = new UnidadeProducao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.empreendimento, c.possui_cod_propriedade, c.propriedade_codigo, e.codigo empreendimento_codigo, c.local_livro
				from {0}crt_unidade_producao c, tab_empreendimento e where c.empreendimento = e.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
                        caracterizacao.CodigoPropriedade = reader.GetValue<Int64>("propriedade_codigo");
						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						caracterizacao.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
						caracterizacao.LocalLivroDisponivel = reader.GetValue<string>("local_livro");
						caracterizacao.PossuiCodigoPropriedade = reader.GetValue<bool>("possui_cod_propriedade");
						caracterizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Unidades de produção

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.unidade_producao, c.possui_cod_up, c.codigo_up, c.tipo_producao, c.renasem, c.renasem_data_validade, c.area,
				c.cultura, c.cultivar, tc.texto cultura_texto, cc.cultivar cultivar_nome, c.data_plantio_ano_producao, c.estimativa_quant_ano, c.estimativa_unid_medida
				from crt_unidade_producao_unidade c, tab_cultura_cultivar cc, tab_cultura tc
				where cc.id(+) = c.cultivar and tc.id = c.cultura and c.unidade_producao = :unidade_producao", EsquemaBanco);

				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.UnidadesProducao.Add(new UnidadeProducaoItem()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							PossuiCodigoUP = reader.GetValue<bool>("possui_cod_up"),
							CodigoUP = reader.GetValue<long>("codigo_up"),
							TipoProducao = reader.GetValue<int>("tipo_producao"),
							RenasemNumero = reader.GetValue<string>("renasem"),
							DataValidadeRenasem = string.IsNullOrEmpty(reader.GetValue<string>("renasem_data_validade")) ? "" : Convert.ToDateTime(reader.GetValue<string>("renasem_data_validade")).ToShortDateString(),
							AreaHA = reader.GetValue<decimal>("area"),
							DataPlantioAnoProducao = reader.GetValue<string>("data_plantio_ano_producao"),
							EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano"),
							CultivarId = reader.GetValue<int>("cultivar"),
							CultivarTexto = reader.GetValue<string>("cultivar_nome"),
							CulturaId = reader.GetValue<int>("cultura"),
							CulturaTexto = reader.GetValue<string>("cultura_texto")
						});
					}

					reader.Close();
				}

				foreach (var item in caracterizacao.UnidadesProducao)
				{
					#region Coordenadas

					comando = bancoDeDados.CriarComando(@"
					select id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm,
					hemisferio_utm, municipio from crt_unidade_producao_un_coord where unidade_producao_unidade = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Coordenada coordenada = null;

						if (reader.Read())
						{
							coordenada = new Coordenada();
							coordenada.Id = reader.GetValue<int>("id");
							coordenada.Tipo.Id = reader.GetValue<int>("tipo_coordenada");
							coordenada.Datum.Id = reader.GetValue<int>("datum");
							coordenada.EastingUtmTexto = reader.GetValue<string>("easting_utm");
							coordenada.NorthingUtmTexto = reader.GetValue<string>("northing_utm");
							coordenada.FusoUtm = reader.GetValue<int>("fuso_utm");
							coordenada.HemisferioUtm = reader.GetValue<int>("hemisferio_utm");
							coordenada.HemisferioUtmTexto = reader.GetValue<string>("hemisferio_utm");

							item.Municipio.Id = reader.GetValue<int>("municipio");
							item.Coordenada = coordenada;
						}

						reader.Close();
					}

					#endregion

					#region Produtores

					comando = bancoDeDados.CriarComando(@"
					select c.id, c.tid, c.produtor, nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo
					from crt_unidade_prod_un_produtor c, tab_pessoa p where p.id = c.produtor and c.unidade_producao_unidade = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							item.Produtores.Add(new Responsavel()
							{
								Id = reader.GetValue<int>("produtor"),
								NomeRazao = reader.GetValue<string>("nome_razao"),
								CpfCnpj = reader.GetValue<string>("cpf_cnpj"),
								IdRelacionamento = reader.GetValue<int>("id"),
								Tipo = reader.GetValue<int>("tipo"),
								Tid = reader.GetValue<string>("tid")
							});
						}

						reader.Close();
					}

					#endregion

					#region Responsáveis Técnicos

					comando = bancoDeDados.CriarComando(@"
					select c.id, c.tid, c.unidade_producao_unidade, c.responsavel_tecnico, nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj,
					pf.texto profissao, oc.orgao_sigla, pp.registro, c.numero_hab_cfo_cfoc, c.numero_art, c.art_cargo_funcao, c.data_validade_art,
					(select h.extensao_habilitacao from tab_hab_emi_cfo_cfoc h where h.responsavel = c.responsavel_tecnico) extensao_habilitacao
					from {0}crt_unidade_prod_un_resp_tec c, {0}tab_credenciado tc, {1}tab_pessoa p, {1}tab_pessoa_profissao pp, {0}tab_profissao pf, {0}tab_orgao_classe oc
					where tc.id = c.responsavel_tecnico and p.id = tc.pessoa and pp.pessoa(+) = p.id and pf.id(+)  = pp.profissao and oc.id(+) = pp.orgao_classe
					and c.unidade_producao_unidade = :id", EsquemaBanco, EsquemaBancoCredenciado);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						ResponsavelTecnico responsavel = null;

						while (reader.Read())
						{
							responsavel = new ResponsavelTecnico();
							responsavel.IdRelacionamento = reader.GetValue<int>("id");
							responsavel.Id = reader.GetValue<int>("responsavel_tecnico");
							responsavel.NomeRazao = reader.GetValue<string>("nome_razao");
							responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
							responsavel.CFONumero = reader.GetValue<string>("numero_hab_cfo_cfoc");
							responsavel.ArtCargoFuncao = reader.GetValue<bool>("art_cargo_funcao");
							responsavel.NumeroArt = reader.GetValue<string>("numero_art");

							responsavel.ProfissaoTexto = reader.GetValue<string>("profissao");
							responsavel.OrgaoClasseSigla = reader.GetValue<string>("orgao_sigla");
							responsavel.NumeroRegistro = reader.GetValue<string>("registro");

							responsavel.DataValidadeART = reader.GetValue<string>("data_validade_art");
							if (!string.IsNullOrEmpty(responsavel.DataValidadeART))
							{
								responsavel.DataValidadeART = Convert.ToDateTime(responsavel.DataValidadeART).ToShortDateString();
							}

							if (Convert.ToBoolean(reader.GetValue<int>("extensao_habilitacao")))
							{
								responsavel.CFONumero += "-ES";
							}

							item.ResponsaveisTecnicos.Add(responsavel);
						}

						reader.Close();
					}

					#endregion

                    #region Títulos Concluídos

                    comando = bancoDeDados.CriarComando(@"select count(*) concluidos
                                                          from crt_unidade_producao c,
                                                               crt_unidade_producao_unidade u,
                                                               tab_titulo ti,
                                                               esp_aber_livro_up_unid uni, 
                                                               esp_abertura_livro_up esp
                                                          where u.unidade_producao = c.id
                                                                and esp.titulo = ti.id 
                                                                and ti.empreendimento = c.empreendimento
                                                                and ti.situacao = 3
                                                                and uni.especificidade = esp.id 
                                                                and uni.unidade = u.id
                                                                and u.CODIGO_UP = :codUP
                                                        ", EsquemaBanco);

                    comando.AdicionarParametroEntrada("codUP", item.CodigoUP, DbType.String);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        Coordenada coordenada = null;

                        if (reader.Read())
                        {
                            item.TitulosConcluidos = reader.GetValue<int>("concluidos");
                        }

                        reader.Close();
                    }

                    #endregion
                }

				#endregion
			}

			return caracterizacao;
		}

		internal UnidadeProducaoItem ObterUnidadeProducaoItem(int id, BancoDeDados banco = null)
		{
			UnidadeProducaoItem item = new UnidadeProducaoItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Unidades de produção

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.unidade_producao, c.possui_cod_up, c.codigo_up, c.tipo_producao, c.renasem, c.renasem_data_validade, c.area,
				c.cultura, c.cultivar, tc.texto cultura_texto, cc.cultivar cultivar_nome, c.data_plantio_ano_producao, c.estimativa_quant_ano, c.estimativa_unid_medida
				from crt_unidade_producao_unidade c, tab_cultura_cultivar cc, tab_cultura tc
				where cc.id(+) = c.cultivar and tc.id = c.cultura and c.id = :item", EsquemaBanco);

				comando.AdicionarParametroEntrada("item", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.PossuiCodigoUP = reader.GetValue<bool>("possui_cod_up");
						item.CodigoUP = reader.GetValue<long>("codigo_up");
						item.TipoProducao = reader.GetValue<int>("tipo_producao");
						item.RenasemNumero = reader.GetValue<string>("renasem");
						item.DataValidadeRenasem = string.IsNullOrEmpty(reader.GetValue<string>("renasem_data_validade")) ? "" : Convert.ToDateTime(reader.GetValue<string>("renasem_data_validade")).ToShortDateString();
						item.AreaHA = reader.GetValue<decimal>("area");
						item.DataPlantioAnoProducao = reader.GetValue<string>("data_plantio_ano_producao");
						item.EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano");
						item.CultivarId = reader.GetValue<int>("cultivar");
						item.CultivarTexto = reader.GetValue<string>("cultivar_nome");
						item.CulturaId = reader.GetValue<int>("cultura");
						item.CulturaTexto = reader.GetValue<string>("cultura_texto");
					}

					reader.Close();
				}

				#endregion
			}

			return item;
		}

		internal CaracterizacaoPDF ObterDadosPdfTitulo(int empreendimento, int atividade, BancoDeDados banco)
		{
			throw new NotImplementedException();
		}

		internal int ObterSequenciaCodigoPropriedade()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select seq_crt_unidade_producao_cod.nextval from dual", EsquemaBanco);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal int ObterUltimoCodigoUP(int empreendimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select nvl(d.maior, 0) ultimo from (select max(to_number(substr(c.codigo_up, 14))) maior from crt_unidade_producao_unidade c
				where c.unidade_producao = (select u.id from crt_unidade_producao u where u.empreendimento = :empreendimento)) d", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

        public Municipio ObterMunicipioPropriedade(int EmprendimentoId, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Municipio municipio = new Municipio();
                Comando comando = bancoDeDados.CriarComando(@"
                    select m.id, m.texto, m.estado, m.cep, m.ibge 
                    from
                    TAB_EMPREENDIMENTO_ENDERECO en
                        inner join LOV_MUNICIPIO m
                          on m.ID = en.MUNICIPIO
                    where en.CORRESPONDENCIA = 0
                    and en.EMPREENDIMENTO = :empreendimento", EsquemaBanco);
                comando.AdicionarParametroEntrada("empreendimento", EmprendimentoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        municipio.Id = Convert.ToInt32(reader["id"]);
                        municipio.Estado.Id = Convert.ToInt32(reader["estado"]);
                        municipio.Ibge = Convert.IsDBNull(reader["ibge"]) ? 0 : Convert.ToInt32(reader["ibge"]);
                        municipio.Texto = reader["texto"].ToString();
                        municipio.Cep = reader["texto"].ToString();
                        municipio.IsAtivo = true;
                    }

                    reader.Close();
                }

                return municipio;
            }
        }

		#endregion

		#region Validações

		internal bool CodigoUPExistente(int unidadeProducaoItemId, long unidadeCodigo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(id) from {0}crt_unidade_producao_unidade where codigo_up = :codigo ", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo", unidadeCodigo, DbType.Int64);

				if (unidadeProducaoItemId > 0)
				{
					comando.DbCommand.CommandText += "and id <> :id";
					comando.AdicionarParametroEntrada("id", unidadeProducaoItemId, DbType.Int32);
				}

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool VerificarResponsavelEmpreendimento(int empreendimentoId, int responsavelId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				select count(*) from tab_empreendimento_responsavel where empreendimento =: empreendimento and responsavel =:responsavel", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel", responsavelId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

        internal bool CodigoPropriedadeExistente(int unidadeId, Int64 codigoPropriedade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@"select count(*) from crt_unidade_producao where propriedade_codigo =: codigo", EsquemaBanco);
				comando.AdicionarParametroEntrada("codigo", codigoPropriedade, DbType.Int64);

				if (unidadeId > 0)
				{
					comando.DbCommand.CommandText += " and id <> :id";
					comando.AdicionarParametroEntrada("id", unidadeId, DbType.Int32);
				}

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal string CodigoPropriedadeExistenteImportar(UnidadeProducao caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@"select e.codigo||' - '||e.denominador empreendimento from crt_unidade_producao c, tab_empreendimento e
				where e.id = c.empreendimento and c.propriedade_codigo = :codigo and c.empreendimento != :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("codigo", caracterizacao.CodigoPropriedade, DbType.Int64);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal EmpreendimentoCaracterizacao VerificarCodigoUPJaCadastrado(long codUp, int empreendimento, BancoDeDados banco = null)
		{
			EmpreendimentoCaracterizacao retorno = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.* from crt_unidade_producao_unidade c, crt_unidade_producao u, tab_empreendimento e where
				e.id = u.empreendimento and u.id = c.unidade_producao and c.codigo_up  = :cod_up and u.empreendimento != :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("cod_up", codUp, DbType.Int64);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno = new EmpreendimentoCaracterizacao();

						retorno.Id = reader.GetValue<int>("id");
						retorno.Codigo = reader.GetValue<int>("codigo");
						retorno.Denominador = reader.GetValue<string>("denominador");
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion
	}
}