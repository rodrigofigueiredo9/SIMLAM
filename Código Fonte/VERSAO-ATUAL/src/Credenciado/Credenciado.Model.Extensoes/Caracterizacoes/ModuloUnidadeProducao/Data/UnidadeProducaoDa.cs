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

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data
{
	public class UnidadeProducaoDa
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

		internal void Salvar(UnidadeProducao caracterizacao, Blocos.Data.BancoDeDados banco = null)
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

		private int Criar(UnidadeProducao caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_unidade_producao
				(id, tid, empreendimento, possui_codigo_propriedade, propriedade_codigo, local_livro, interno_id, interno_tid) values
				(seq_crt_unidade_producao.nextval, :tid, :empreendimento_id, :possui_cod_propriedade, :propriedade_codigo, :local_livro, :interno_id, :interno_tid)
				returning id into :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento_id", caracterizacao.Empreendimento.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_cod_propriedade", caracterizacao.PossuiCodigoPropriedade, DbType.Int32);
                comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoID, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacao.InternoTID);
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
					(seq_crt_unida_prod_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
					:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
					returning id into :unidade_id", EsquemaCredenciadoBanco);

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
					(seq_crt_unida_prod_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaCredenciadoBanco);

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
					(seq_crt_unid_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaCredenciadoBanco);

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

		private void Editar(UnidadeProducao caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Unidade de Produção

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update crt_unidade_producao set propriedade_codigo = :propriedade_codigo, local_livro = :local_livro, tid = :tid where id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Deletando dados das tabelas filhas

				//Coordenada
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_unidade_producao_un_coord c where c.unidade_producao_unidade in
				(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaCredenciadoBanco);
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
				(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaCredenciadoBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.SelectMany(x => x.ResponsaveisTecnicos).Select(y => y.IdRelacionamento).ToList());
				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Unidades de Produção
				comando = bancoDeDados.CriarComando("delete from {0}crt_unidade_producao_unidade c where c.unidade_producao = :unidade_producao", EsquemaCredenciadoBanco);
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
						estimativa_quant_ano = :estimativa_quant_ano, estimativa_unid_medida = :estimativa_unid_medida, ano_abertura = :ano_abertura where id = :id", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into crt_unidade_producao_unidade (id, tid, unidade_producao, possui_cod_up, codigo_up, tipo_producao, renasem,
						renasem_data_validade, area, cultivar, data_plantio_ano_producao, estimativa_quant_ano, estimativa_unid_medida, ano_abertura, cultura) values
						(seq_crt_unida_prod_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
						:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
						returning id into :unidade_id", EsquemaCredenciadoBanco);

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
						where unidade_producao_unidade = :unidade_producao_unidade", EsquemaCredenciadoBanco);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_unidade_producao_un_coord
						(id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, municipio) values
						(seq_crt_unida_prod_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaCredenciadoBanco);
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
							numero_art =:numero_art, art_cargo_funcao =: art_cargo_funcao, data_validade_art =: data_validade_art where id = :id_rel", EsquemaCredenciadoBanco);

							comando.AdicionarParametroEntrada("id_rel", responsavel.IdRelacionamento, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_unidade_prod_un_resp_tec
							(id, tid, unidade_producao_unidade, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art) values
							(seq_crt_unid_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaCredenciadoBanco);

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

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_unidade_producao c where c.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
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
				end;", EsquemaCredenciadoBanco);

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
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade_producao", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void CopiarDadosInstitucional(UnidadeProducao caracterizacao, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				#region Unidade Produção

				if (caracterizacao.Id <= 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}crt_unidade_producao (id, tid, empreendimento, possui_codigo_propriedade, propriedade_codigo, local_livro,
					interno_id, interno_tid) values (seq_crt_unidade_producao.nextval, :tid, :empreendimento_id, :possui_cod_propriedade, :propriedade_codigo, :local_livro,
					:interno_id, :interno_tid) returning id into :id",
					EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("empreendimento_id", caracterizacao.Empreendimento.Id, DbType.Int32);
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update crt_unidade_producao set propriedade_codigo = :propriedade_codigo, possui_codigo_propriedade =:possui_cod_propriedade,
					interno_id =:interno_id , interno_tid =:interno_tid, local_livro = :local_livro, tid = :tid where id = :id", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoID, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacao.InternoTID);

				comando.AdicionarParametroEntrada("possui_cod_propriedade", caracterizacao.PossuiCodigoPropriedade, DbType.Int32);
                comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (caracterizacao.Id <= 0)
				{
					caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					delete from crt_unidade_prod_un_resp_tec r where r.unidade_producao_unidade in (select id from crt_unidade_producao_unidade cc where cc.unidade_producao = :unidade_producao);
					delete from crt_unidade_prod_un_produtor p where p.unidade_producao_unidade in (select id from crt_unidade_producao_unidade cc where cc.unidade_producao = :unidade_producao);
					delete from crt_unidade_producao_un_coord c where c.unidade_producao_unidade in (select id from crt_unidade_producao_unidade cc where cc.unidade_producao = :unidade_producao);
					delete from crt_unidade_producao_unidade cc where cc.unidade_producao = :unidade_producao;
				end; ", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Unidades de Produção

				foreach (UnidadeProducaoItem item in caracterizacao.UnidadesProducao)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into crt_unidade_producao_unidade (id, tid, unidade_producao, possui_cod_up, codigo_up, tipo_producao, renasem,
					renasem_data_validade, area, cultivar, data_plantio_ano_producao, estimativa_quant_ano, estimativa_unid_medida, ano_abertura, cultura) values
					(seq_crt_unida_prod_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
					:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
					returning id into :unidade_id", EsquemaCredenciadoBanco);

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
					(seq_crt_unida_prod_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaCredenciadoBanco);

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

					comando = bancoDeDados.CriarComando(@"select id from {0}tab_pessoa p where p.credenciado = :credenciado
					and p.id in (select er.responsavel from {0}tab_empreendimento_responsavel er where er.empreendimento = :empreendimento)", EsquemaCredenciadoBanco);
					comando.DbCommand.CommandText += String.Format(comando.AdicionarIn("and", "p.interno", DbType.Int32, item.Produtores.Select(x => x.Id).ToList()));

					comando.AdicionarParametroEntrada("credenciado", ((EtramiteIdentity)HttpContext.Current.User.Identity).FuncionarioId, DbType.Int32);
					comando.AdicionarParametroEntrada("empreendimento", caracterizacao.Empreendimento.Id, DbType.Int32);

					List<int> produtoresCrendenciadoIds = bancoDeDados.ExecutarList<int>(comando);

					comando = bancoDeDados.CriarComando(@"
					insert into crt_unidade_prod_un_produtor (id, tid, unidade_producao_unidade, produtor) values
					(seq_crt_unidade_prod_un_produt.nextval, :tid, :unidade_producao_unidade, :produtor)", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("produtor", DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (var produtorId in produtoresCrendenciadoIds)
					{
						comando.SetarValorParametro("produtor", produtorId);
						bancoDeDados.ExecutarNonQuery(comando);
					}

					#endregion

					#region Responsaveis Técnicos Habilitados CFO / CFOC

					comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_unidade_prod_un_resp_tec
					(id, tid, unidade_producao_unidade, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art) values
					(seq_crt_unid_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaCredenciadoBanco);

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

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.copiar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void AtualizarInternoIdTid(int unidadeProducaoId, int unidadeProducaoInternoId, string unidadeProducaoInternoTid, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualização do Tid

				Comando comando = bancoDeDados.CriarComandoPlSql(@"begin
				update crt_unidade_producao set tid = :tid, interno_id = :interno_id, interno_tid = :interno_tid where id = :unidade_producao_id;
				update crt_unidade_producao_unidade set tid = :tid where unidade_producao = :unidade_producao_id;
				update crt_unidade_producao_un_coord c set tid = :tid where c.unidade_producao_unidade in (select id from crt_unidade_producao_unidade where unidade_producao =:unidade_producao_id);
				update crt_unidade_prod_un_produtor r set tid = :tid where r.unidade_producao_unidade in (select id from crt_unidade_producao_unidade where unidade_producao =:unidade_producao_id);
				update crt_unidade_prod_un_resp_tec r set tid = :tid where r.unidade_producao_unidade in (select id from crt_unidade_producao_unidade where unidade_producao =:unidade_producao_id);
				end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade_producao_id", unidadeProducaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("interno_id", unidadeProducaoInternoId, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, unidadeProducaoInternoTid);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(unidadeProducaoId, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.atualizaridtid, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar
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

		internal UnidadeProducao ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeProducao caracterizacao = new UnidadeProducao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_unidade_producao t where t.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		public UnidadeProducao Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			UnidadeProducao caracterizacao = new UnidadeProducao();

            BancoDeDados novoBanco = null;

            BancoDeDados bancoDeDadosIDAF = BancoDeDados.ObterInstancia(novoBanco);

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.interno_id, c.interno_tid, c.empreendimento, c.possui_codigo_propriedade, c.propriedade_codigo, e.codigo empreendimento_codigo,
				e.interno emp_interno_id, c.local_livro from {0}crt_unidade_producao c, tab_empreendimento e where c.empreendimento = e.id and c.id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.CredenciadoID = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");

						caracterizacao.CodigoPropriedade = reader.GetValue<Int64>("propriedade_codigo");
						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						caracterizacao.Empreendimento.InternoID = reader.GetValue<int>("emp_interno_id");
						caracterizacao.Empreendimento.Codigo = reader.GetValue<int?>("empreendimento_codigo");
						caracterizacao.LocalLivroDisponivel = reader.GetValue<string>("local_livro");
						caracterizacao.PossuiCodigoPropriedade = reader.GetValue<bool>("possui_codigo_propriedade");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");
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
				select c.id, c.tid, c.unidade_producao, c.possui_cod_up, c.codigo_up, c.tipo_producao, c.renasem, c.renasem_data_validade, c.area, c.ano_abertura,
				c.cultura, c.cultivar, tc.texto cultura_texto, cc.cultivar cultivar_nome, c.data_plantio_ano_producao, c.estimativa_quant_ano, c.estimativa_unid_medida
				from {0}crt_unidade_producao_unidade c, {1}tab_cultura_cultivar cc, {1}tab_cultura tc
				where cc.id(+) = c.cultivar and tc.id = c.cultura and c.unidade_producao = :unidade_producao", EsquemaCredenciadoBanco, EsquemaBanco);

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
							CulturaTexto = reader.GetValue<string>("cultura_texto"),
							AnoAbertura = reader.GetValue<string>("ano_abertura")
						});
					}

					reader.Close();
				}

				foreach (var item in caracterizacao.UnidadesProducao)
				{
					#region Coordenadas

					comando = bancoDeDados.CriarComando(@"
					select id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm,
					hemisferio_utm, municipio from crt_unidade_producao_un_coord where unidade_producao_unidade = :id", EsquemaCredenciadoBanco);

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
					select c.id, c.tid, c.produtor, nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo, p.interno
					from crt_unidade_prod_un_produtor c, tab_pessoa p where p.id = c.produtor and unidade_producao_unidade = :unidade_producao_unidade", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);

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
								Tid = reader.GetValue<string>("tid"),
								InternoId = reader.GetValue<int>("interno")
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
					from {1}crt_unidade_prod_un_resp_tec c, {0}tab_credenciado tc, {1}tab_pessoa p, {1}tab_pessoa_profissao pp, {0}tab_profissao pf, {0}tab_orgao_classe oc
					where tc.id = c.responsavel_tecnico and p.id = tc.pessoa and pp.pessoa(+) = p.id and pf.id(+)  = pp.profissao and oc.id(+) = pp.orgao_classe
					and c.unidade_producao_unidade = :id", EsquemaBanco, EsquemaCredenciadoBanco);

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

                    EsquemaBanco = "idaf";

                    comando = bancoDeDados.CriarComando(@"select count(*) concluidos
                                                          from crt_unidade_producao_unidade u,
                                                               crt_unidade_producao c,
                                                               tab_titulo ti,
                                                               esp_abertura_livro_up esp,
                                                               esp_aber_livro_up_unid uni
                                                          where u.CODIGO_UP = :codUP
                                                                and u.unidade_producao = c.id
                                                                and c.empreendimento = ti.empreendimento
                                                                and ti.situacao = 3
                                                                and ti.id = esp.titulo
                                                                and esp.id = uni.especificidade
                                                                and uni.unidade = u.id
                                                        ", EsquemaBanco);

                    comando.AdicionarParametroEntrada("codUP", item.CodigoUP, DbType.String);

                    using (IDataReader reader = bancoDeDadosIDAF.ExecutarReader(comando))
                    {

                        if (reader.Read())
                        {
                            item.TitulosConcluidos = reader.GetValue<int>("concluidos");
                        }

                        reader.Close();
                    }

                    EsquemaBanco = null;

                    #endregion
				}

				#endregion
			}

			return caracterizacao;
		}

		public UnidadeProducao ObterHistorico(int id, string tid, BancoDeDados banco = null, bool simplificado = false)
		{
			int id_hst = 0;
			UnidadeProducao caracterizacao = new UnidadeProducao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id id_hst, c.tid, c.interno_id, c.interno_tid, c.empreendimento_id empreendimento, c.possui_cod_propriedade,
				c.propriedade_codigo, e.codigo empreendimento_codigo, e.interno_id  emp_interno_id, c.local_livro
				from {0}hst_crt_unidade_producao c, {0}hst_empreendimento e where  e.empreendimento_id = c.empreendimento_id
				and e.tid = c.empreendimento_tid and c.unidade_producao_id = :id and c.tid = :tid", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						id_hst = reader.GetValue<int>("id_hst");

						caracterizacao.Id = id;
						caracterizacao.CredenciadoID = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");

                        caracterizacao.CodigoPropriedade = reader.GetValue<Int64>("propriedade_codigo");
						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						caracterizacao.Empreendimento.InternoID = reader.GetValue<int>("emp_interno_id");
						caracterizacao.Empreendimento.Codigo = reader.GetValue<int?>("empreendimento_codigo");
						caracterizacao.LocalLivroDisponivel = reader.GetValue<string>("local_livro");
						caracterizacao.PossuiCodigoPropriedade = reader.GetValue<bool>("possui_cod_propriedade");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Unidades de produção

				comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.unidade_producao_id, c.unidade_producao_unidade_id, c.possui_cod_up, c.codigo_up, c.tipo_producao_id, c.renasem, c.renasem_data_validade,
				c.area, c.ano_abertura, c.cultura_id, tc.texto cultura_texto, c.cultivar_id, cc.cultivar cultivar_nome, c.data_plantio_ano_producao, c.estimativa_quant_ano,
				c.estimativa_unid_medida_id  from {0}hst_crt_unidade_prod_unidade c, {1}tab_cultura_cultivar cc, {1}hst_cultura tc where cc.id(+) = c.cultivar_id
				and  tc.cultura_id = c.cultura_id and tc.tid = c.cultura_tid and c.id_hst = :id_hst", EsquemaCredenciadoBanco, EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						id_hst = reader.GetValue<int>("id");
						UnidadeProducaoItem item = new UnidadeProducaoItem();

						item.Id = reader.GetValue<int>("unidade_producao_unidade_id");
						item.Tid = reader.GetValue<string>("tid");
						item.PossuiCodigoUP = reader.GetValue<bool>("possui_cod_up");
						item.CodigoUP = reader.GetValue<long>("codigo_up");
						item.TipoProducao = reader.GetValue<int>("tipo_producao_id");
						item.RenasemNumero = reader.GetValue<string>("renasem");
						item.DataValidadeRenasem = string.IsNullOrEmpty(reader.GetValue<string>("renasem_data_validade")) ? "" : Convert.ToDateTime(reader.GetValue<string>("renasem_data_validade")).ToShortDateString();
						item.AreaHA = reader.GetValue<decimal>("area");
						item.DataPlantioAnoProducao = reader.GetValue<string>("data_plantio_ano_producao");
						item.EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano");
						item.CultivarId = reader.GetValue<int>("cultivar_id");
						item.CultivarTexto = reader.GetValue<string>("cultivar_nome");
						item.CulturaId = reader.GetValue<int>("cultura_id");
						item.CulturaTexto = reader.GetValue<string>("cultura_texto");
						item.AnoAbertura = reader.GetValue<string>("ano_abertura");

						#region Coordenadas

						comando = bancoDeDados.CriarComando(@"
						select h.id, h.tid, h.unidade_producao_unidade_id unidade_producao_unidade, h.tipo_coordenada_id tipo_coordenada, h.datum_id datum,
						h.easting_utm easting_utm, h.northing_utm, h.fuso_utm, h.hemisferio_utm_id hemisferio_utm, h.municipio_id, h.municipio_texto
						from {0}hst_crt_unidade_prod_un_coord h where h.id_hst = :id_hst", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Coordenada coordenada = null;

							if (readerAux.Read())
							{
								coordenada = new Coordenada();
								coordenada.Id = readerAux.GetValue<int>("id");
								coordenada.Tipo.Id = readerAux.GetValue<int>("tipo_coordenada");
								coordenada.Datum.Id = readerAux.GetValue<int>("datum");
								coordenada.EastingUtmTexto = readerAux.GetValue<string>("easting_utm");
								coordenada.NorthingUtmTexto = readerAux.GetValue<string>("northing_utm");
								coordenada.FusoUtm = readerAux.GetValue<int>("fuso_utm");
								coordenada.HemisferioUtm = readerAux.GetValue<int>("hemisferio_utm");
								coordenada.HemisferioUtmTexto = readerAux.GetValue<string>("hemisferio_utm");

								item.Municipio.Id = readerAux.GetValue<int>("municipio_id");
								item.Municipio.Texto = readerAux.GetValue<string>("municipio_texto");
								item.Coordenada = coordenada;
							}

							readerAux.Close();
						}

						#endregion

						#region Produtores

						comando = bancoDeDados.CriarComando(@"
						select h.id, h.tid, h.unidade_prod_produtor_id id_rel, h.produtor_id, nvl(hp.nome, hp.razao_social) nome_razao, nvl(hp.cpf, hp.cnpj) cpf_cnpj, hp.tipo, hp.interno_id
						from {0}hst_crt_unid_prod_un_produtor h, {0}hst_pessoa hp where h.produtor_id = hp.pessoa_id and h.produtor_tid = hp.tid and h.id_hst = :id_hst", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								item.Produtores.Add(new Responsavel()
								{
									Id = readerAux.GetValue<int>("produtor_id"),
									NomeRazao = readerAux.GetValue<string>("nome_razao"),
									CpfCnpj = readerAux.GetValue<string>("cpf_cnpj"),
									IdRelacionamento = readerAux.GetValue<int>("id_rel"),
									Tipo = readerAux.GetValue<int>("tipo"),
									Tid = readerAux.GetValue<string>("tid"),
									InternoId = readerAux.GetValue<int>("interno_id")
								});
							}

							readerAux.Close();
						}

						#endregion

						#region Responsáveis Técnicos

						comando = bancoDeDados.CriarComando(@"
						select distinct c.id,
							c.unidade_prod_un_resp_tec_id,
							c.tid,
							c.unidade_producao_unidade_id,
							c.responsavel_tecnico_id,
							c.numero_hab_cfo_cfoc,
							c.numero_art,
							c.art_cargo_funcao,
							c.data_validade_art,
							nvl(p.nome, p.razao_social) nome_razao,
							nvl(p.cpf, p.cnpj) cpf_cnpj,
							pf.texto profissao,
							oc.orgao_sigla,
							pp.registro,
							(select h.extensao_habilitacao
								from {0}hst_hab_emi_cfo_cfoc h
								where h.responsavel_id = c.responsavel_tecnico_id
								and h.responsavel_tid = c.responsavel_tecnico_tid
								and h.data_execucao =
									(select max(hh.data_execucao)
										from {0}hst_hab_emi_cfo_cfoc hh
										where hh.responsavel_id = h.responsavel_id
										and hh.responsavel_tid = h.responsavel_tid
										and hh.data_execucao <= (select hc.data_execucao from hst_crt_unidade_producao hc
										where hc.id = (select hu.id_hst from hst_crt_unidade_prod_unidade hu where hu.id = c.id_hst)))) extensao_habilitacao
						from {1}hst_crt_un_prod_un_resp_tec c,
							{1}hst_credenciado              hc,
							{1}hst_pessoa                   p,
							{1}hst_pessoa_profissao         pp,
							{0}tab_profissao                pf,
							{0}tab_orgao_classe             oc
						where hc.credenciado_id = c.responsavel_tecnico_id
						and hc.tid = c.responsavel_tecnico_tid
						and p.pessoa_id = hc.pessoa_id
						and p.tid = hc.pessoa_tid
						and pp.id_hst(+) = p.id
						and pf.id(+) = pp.profissao_id
						and oc.id(+) = pp.orgao_classe_id
						and c.id_hst = :id_hst", EsquemaBanco, EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ResponsavelTecnico responsavel = null;

							while (readerAux.Read())
							{
								responsavel = new ResponsavelTecnico();
								responsavel.IdRelacionamento = readerAux.GetValue<int>("unidade_prod_un_resp_tec_id");
								responsavel.Id = readerAux.GetValue<int>("responsavel_tecnico_id");
								responsavel.NomeRazao = readerAux.GetValue<string>("nome_razao");
								responsavel.CpfCnpj = readerAux.GetValue<string>("cpf_cnpj");
								responsavel.CFONumero = readerAux.GetValue<string>("numero_hab_cfo_cfoc");
								responsavel.ArtCargoFuncao = readerAux.GetValue<bool>("art_cargo_funcao");
								responsavel.NumeroArt = readerAux.GetValue<string>("numero_art");

								responsavel.ProfissaoTexto = readerAux.GetValue<string>("profissao");
								responsavel.OrgaoClasseSigla = readerAux.GetValue<string>("orgao_sigla");
								responsavel.NumeroRegistro = readerAux.GetValue<string>("registro");

								responsavel.DataValidadeART = readerAux.GetValue<string>("data_validade_art");
								if (!string.IsNullOrEmpty(responsavel.DataValidadeART))
								{
									responsavel.DataValidadeART = Convert.ToDateTime(responsavel.DataValidadeART).ToShortDateString();
								}

                                if (Convert.ToBoolean(readerAux.GetValue<int>("extensao_habilitacao")))
								{
									responsavel.CFONumero += "-ES";
								}

								item.ResponsaveisTecnicos.Add(responsavel);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.UnidadesProducao.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select nvl(d.maior, 0) ultimo from (select max(to_number(substr(c.codigo_up, 14))) maior from crt_unidade_producao_unidade c
				where c.unidade_producao = (select u.id from crt_unidade_producao u where u.empreendimento = :empreendimento)) d", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion

		#region Validações

		internal bool CodigoPropriedadeExistente(UnidadeProducao caracterizacao, BancoDeDados banco = null)
		{
			bool existe = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}crt_unidade_producao where propriedade_codigo =: codigo and id <> :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("id", caracterizacao.InternoID, DbType.Int32);

				existe = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}

			if (existe)
			{
				return true;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(c.id) from {0}crt_unidade_producao c, {0}tab_empreendimento e
				where e.id = c.empreendimento and c.propriedade_codigo =: codigo and c.id <> :id and e.credenciado = :credenciado", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool VerificarResponsavelEmpreendimento(int empreendimentoId, int responsavelId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from {0}tab_empreendimento_responsavel where empreendimento =: empreendimento and responsavel =:responsavel", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel", responsavelId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool CodigoUPExistente(UnidadeProducaoItem unidade, int empreendimentoID)
		{
			bool existe = false;
			int empreendimentoInternoID = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select e.interno from tab_empreendimento e where e.id = :empreendimento), 0) from dual", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);

				empreendimentoInternoID = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(i.id) from {0}crt_unidade_producao_unidade i, {0}crt_unidade_producao c, {0}tab_empreendimento e
				where i.unidade_producao = c.id and c.empreendimento = e.id and i.codigo_up = :codigo and c.empreendimento <> :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo", unidade.CodigoUP, DbType.Int64);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoInternoID, DbType.Int32);

				existe = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}

			if (existe)
			{
				return true;
			}

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(i.id) from {0}crt_unidade_producao_unidade i, {0}crt_unidade_producao c, {0}tab_empreendimento e
				where i.unidade_producao = c.id and c.empreendimento = e.id and i.codigo_up = :codigo and i.id <> :id
				and e.credenciado = :credenciado", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("codigo", unidade.CodigoUP, DbType.Int64);
				comando.AdicionarParametroEntrada("id", unidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		internal bool FoiCopiada(int caracterizacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(1) from {0}hst_crt_unidade_producao h, {0}lov_historico_artefatos_acoes aa
				where h.acao_executada = aa.id and aa.acao = 38 and h.unidade_producao_id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) > 0;
			}
		}

		#endregion
	}
}