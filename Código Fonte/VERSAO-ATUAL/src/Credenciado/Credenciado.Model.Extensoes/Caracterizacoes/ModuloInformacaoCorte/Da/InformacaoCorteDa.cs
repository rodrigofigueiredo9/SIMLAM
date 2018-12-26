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
				//bancoDeDados.IniciarTransacao();

				//#region Unidade de Produção

				//Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_unidade_producao
				//(id, tid, empreendimento, possui_codigo_propriedade, propriedade_codigo, local_livro, interno_id, interno_tid) values
				//(seq_crt_unidade_producao.nextval, :tid, :empreendimento_id, :possui_cod_propriedade, :propriedade_codigo, :local_livro, :interno_id, :interno_tid)
				//returning id into :id", EsquemaCredenciadoBanco);

				//comando.AdicionarParametroEntrada("empreendimento_id", caracterizacao.Empreendimento.Id, DbType.Int32);
				//comando.AdicionarParametroEntrada("possui_cod_propriedade", caracterizacao.PossuiCodigoPropriedade, DbType.Int32);
    //            comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				//comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				//comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoID, DbType.Int32);
				//comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacao.InternoTID);
				//comando.AdicionarParametroSaida("id", DbType.Int32);

				//bancoDeDados.ExecutarNonQuery(comando);

				//caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				//#endregion

				//#region Unidades de Produção

				//foreach (UnidadeProducaoItem item in caracterizacao.UnidadesProducao)
				//{
				//	comando = bancoDeDados.CriarComando(@"
				//	insert into crt_unidade_producao_unidade (id, tid, unidade_producao, possui_cod_up, codigo_up, tipo_producao, renasem,
				//	renasem_data_validade, area, cultivar, data_plantio_ano_producao, estimativa_quant_ano, estimativa_unid_medida, ano_abertura, cultura) values
				//	(seq_crt_unida_prod_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
				//	:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
				//	returning id into :unidade_id", EsquemaCredenciadoBanco);

				//	comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("possui_cod_up", item.PossuiCodigoUP, DbType.Int32);
				//	comando.AdicionarParametroEntrada("codigo_up", item.CodigoUP, DbType.Int64);
				//	comando.AdicionarParametroEntrada("tipo_producao", item.TipoProducao, DbType.Int32);
				//	comando.AdicionarParametroEntrada("renasem", item.RenasemNumero, DbType.String);
				//	comando.AdicionarParametroEntrada("renasem_data_validade", item.DataValidadeRenasem, DbType.String);
				//	comando.AdicionarParametroEntrada("area", item.AreaHA, DbType.Decimal);
				//	comando.AdicionarParametroEntrada("cultivar", item.CultivarId, DbType.Int32);
				//	comando.AdicionarParametroEntrada("data_plantio_ano_producao", item.DataPlantioAnoProducao, DbType.String);
				//	comando.AdicionarParametroEntrada("estimativa_quant_ano", item.EstimativaProducaoQuantidadeAno, DbType.Decimal);
				//	comando.AdicionarParametroEntrada("estimativa_unid_medida", item.EstimativaProducaoUnidadeMedidaId, DbType.Int32);
				//	comando.AdicionarParametroEntrada("ano_abertura", item.AnoAbertura, DbType.Int32);
				//	comando.AdicionarParametroEntrada("cultura", item.CulturaId, DbType.Int32);
				//	comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//	comando.AdicionarParametroSaida("unidade_id", DbType.Int32);

				//	bancoDeDados.ExecutarNonQuery(comando);

				//	item.Id = comando.ObterValorParametro<int>("unidade_id");

				//	#region Coordenadas

				//	comando = bancoDeDados.CriarComando(@"
				//	insert into {0}crt_unidade_producao_un_coord
				//	(id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, municipio) values
				//	(seq_crt_unida_prod_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaCredenciadoBanco);

				//	comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("tipo_coordenada", item.Coordenada.Tipo.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("datum", item.Coordenada.Datum.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("easting_utm", item.Coordenada.EastingUtm, DbType.Double);
				//	comando.AdicionarParametroEntrada("northing_utm", item.Coordenada.NorthingUtm, DbType.Double);
				//	comando.AdicionarParametroEntrada("fuso_utm", item.Coordenada.FusoUtm, DbType.Int32);
				//	comando.AdicionarParametroEntrada("hemisferio_utm", item.Coordenada.HemisferioUtm, DbType.Int32);
				//	comando.AdicionarParametroEntrada("municipio", item.Municipio.Id > 0 ? (object)item.Municipio.Id : DBNull.Value, DbType.Int32);
				//	comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//	bancoDeDados.ExecutarNonQuery(comando);

				//	#endregion

				//	#region Produtores

				//	comando = bancoDeDados.CriarComando(@"
				//	insert into crt_unidade_prod_un_produtor (id, tid, unidade_producao_unidade, produtor) values
				//	(seq_crt_unidade_prod_un_produt.nextval, :tid, :unidade_producao_unidade, :produtor)");

				//	comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("produtor", DbType.Int32);
				//	comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				//	foreach (var produtor in item.Produtores)
				//	{
				//		comando.SetarValorParametro("produtor", produtor.Id);
				//		bancoDeDados.ExecutarNonQuery(comando);
				//	}

				//	#endregion

				//	#region Responsaveis Técnicos Habilitados CFO / CFOC

				//	comando = bancoDeDados.CriarComando(@"
				//	insert into {0}crt_unidade_prod_un_resp_tec
				//	(id, tid, unidade_producao_unidade, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art) values
				//	(seq_crt_unid_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaCredenciadoBanco);

				//	comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("responsavel_tecnico", DbType.Int32);
				//	comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", DbType.String);
				//	comando.AdicionarParametroEntrada("numero_art", DbType.String);
				//	comando.AdicionarParametroEntrada("art_cargo_funcao", DbType.Int32);
				//	comando.AdicionarParametroEntrada("data_validade_art", DbType.String);
				//	comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				//	foreach (var responsavel in item.ResponsaveisTecnicos)
				//	{
				//		comando.SetarValorParametro("responsavel_tecnico", responsavel.Id);
				//		comando.SetarValorParametro("numero_hab_cfo_cfoc", responsavel.CFONumero);
				//		comando.SetarValorParametro("numero_art", responsavel.NumeroArt);
				//		comando.SetarValorParametro("art_cargo_funcao", responsavel.ArtCargoFuncao);
				//		comando.SetarValorParametro("data_validade_art", responsavel.DataValidadeART);
				//		bancoDeDados.ExecutarNonQuery(comando);
				//	}

				//	#endregion
				//}

				//#endregion

				//Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.criar, bancoDeDados, null);

				//bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		private void Editar(InformacaoCorte caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				//#region Unidade de Produção

				//bancoDeDados.IniciarTransacao();

				//Comando comando = bancoDeDados.CriarComando(@"update crt_unidade_producao set propriedade_codigo = :propriedade_codigo, local_livro = :local_livro, tid = :tid where id = :id", EsquemaCredenciadoBanco);

				//comando.AdicionarParametroEntrada("propriedade_codigo", caracterizacao.CodigoPropriedade, DbType.Int64);
				//comando.AdicionarParametroEntrada("local_livro", caracterizacao.LocalLivroDisponivel, DbType.String);
				//comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				//bancoDeDados.ExecutarNonQuery(comando);

				//#endregion

				//#region Deletando dados das tabelas filhas

				////Coordenada
				//comando = bancoDeDados.CriarComando(@"delete from {0}crt_unidade_producao_un_coord c where c.unidade_producao_unidade in
				//(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaCredenciadoBanco);
				//comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.Select(x => x.Coordenada).Select(y => y.Id).ToList());
				//comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				//bancoDeDados.ExecutarNonQuery(comando);

				////Produtores
				//comando = bancoDeDados.CriarComando(@"delete from {0}crt_unidade_prod_un_produtor c where c.unidade_producao_unidade in
				//(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaBanco);
				//comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.SelectMany(x => x.Produtores).Select(y => y.IdRelacionamento).ToList());
				//comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				//bancoDeDados.ExecutarNonQuery(comando);

				////Responsaveis Técnicos
				//comando = bancoDeDados.CriarComando(@"delete from {0}crt_unidade_prod_un_resp_tec c where c.unidade_producao_unidade in
				//(select t.id from {0}crt_unidade_producao_unidade t where t.unidade_producao = :unidade_producao)", EsquemaCredenciadoBanco);
				//comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.SelectMany(x => x.ResponsaveisTecnicos).Select(y => y.IdRelacionamento).ToList());
				//comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				//bancoDeDados.ExecutarNonQuery(comando);

				////Unidades de Produção
				//comando = bancoDeDados.CriarComando("delete from {0}crt_unidade_producao_unidade c where c.unidade_producao = :unidade_producao", EsquemaCredenciadoBanco);
				//comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.UnidadesProducao.Select(x => x.Id).ToList());
				//comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				//bancoDeDados.ExecutarNonQuery(comando);

				//#endregion

				//#region Unidade de Produção

				//foreach (UnidadeProducaoItem item in caracterizacao.UnidadesProducao)
				//{
				//	if (item.Id > 0)
				//	{
				//		comando = bancoDeDados.CriarComando(@"
				//		update {0}crt_unidade_producao_unidade set cultura =:cultura,  tid = :tid, codigo_up = :codigo_up, tipo_producao = :tipo_producao, renasem =:renasem,
				//		renasem_data_validade = :renasem_data_validade, area = :area, cultivar = :cultivar, data_plantio_ano_producao = :data_plantio_ano_producao,
				//		estimativa_quant_ano = :estimativa_quant_ano, estimativa_unid_medida = :estimativa_unid_medida, ano_abertura = :ano_abertura where id = :id", EsquemaCredenciadoBanco);

				//		comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
				//	}
				//	else
				//	{
				//		comando = bancoDeDados.CriarComando(@"
				//		insert into crt_unidade_producao_unidade (id, tid, unidade_producao, possui_cod_up, codigo_up, tipo_producao, renasem,
				//		renasem_data_validade, area, cultivar, data_plantio_ano_producao, estimativa_quant_ano, estimativa_unid_medida, ano_abertura, cultura) values
				//		(seq_crt_unida_prod_unidade.nextval, :tid, :unidade_producao, :possui_cod_up, :codigo_up, :tipo_producao, :renasem,
				//		:renasem_data_validade, :area, :cultivar, :data_plantio_ano_producao, :estimativa_quant_ano, :estimativa_unid_medida, :ano_abertura, :cultura)
				//		returning id into :unidade_id", EsquemaCredenciadoBanco);

				//		comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);
				//		comando.AdicionarParametroEntrada("possui_cod_up", item.PossuiCodigoUP, DbType.Int32);
				//		comando.AdicionarParametroSaida("unidade_id", DbType.Int32);
				//	}

				//	comando.AdicionarParametroEntrada("codigo_up", item.CodigoUP, DbType.Int64);
				//	comando.AdicionarParametroEntrada("tipo_producao", item.TipoProducao, DbType.Int32);
				//	comando.AdicionarParametroEntrada("renasem", item.RenasemNumero, DbType.String);
				//	comando.AdicionarParametroEntrada("renasem_data_validade", item.DataValidadeRenasem, DbType.String);
				//	comando.AdicionarParametroEntrada("area", item.AreaHA, DbType.Decimal);
				//	comando.AdicionarParametroEntrada("cultivar", item.CultivarId > 0 ? item.CultivarId : (object)DBNull.Value, DbType.Int32);
				//	comando.AdicionarParametroEntrada("data_plantio_ano_producao", item.DataPlantioAnoProducao, DbType.String);
				//	comando.AdicionarParametroEntrada("estimativa_quant_ano", item.EstimativaProducaoQuantidadeAno, DbType.Decimal);
				//	comando.AdicionarParametroEntrada("estimativa_unid_medida", item.EstimativaProducaoUnidadeMedidaId, DbType.Int32);
				//	comando.AdicionarParametroEntrada("ano_abertura", item.AnoAbertura, DbType.Int32);
				//	comando.AdicionarParametroEntrada("cultura", item.CulturaId, DbType.Int32);
				//	comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//	bancoDeDados.ExecutarNonQuery(comando);

				//	if (item.Id < 1)
				//	{
				//		item.Id = comando.ObterValorParametro<int>("unidade_id");
				//	}

				//	#region Coordenadas

				//	if (item.Coordenada.Id > 0)
				//	{
				//		comando = bancoDeDados.CriarComando(@"
				//		update {0}crt_unidade_producao_un_coord set tid = :tid, tipo_coordenada = :tipo_coordenada, datum = :datum, easting_utm = :easting_utm,
				//		northing_utm = :northing_utm, fuso_utm = :fuso_utm, hemisferio_utm = :hemisferio_utm, municipio = :municipio
				//		where unidade_producao_unidade = :unidade_producao_unidade", EsquemaCredenciadoBanco);
				//	}
				//	else
				//	{
				//		comando = bancoDeDados.CriarComando(@"
				//		insert into {0}crt_unidade_producao_un_coord
				//		(id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm, hemisferio_utm, municipio) values
				//		(seq_crt_unida_prod_un_coord.nextval, :tid, :unidade_producao_unidade, :tipo_coordenada, :datum, :easting_utm, :northing_utm, :fuso_utm, :hemisferio_utm, :municipio)", EsquemaCredenciadoBanco);
				//	}

				//	comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("tipo_coordenada", item.Coordenada.Tipo.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("datum", item.Coordenada.Datum.Id, DbType.Int32);
				//	comando.AdicionarParametroEntrada("easting_utm", item.Coordenada.EastingUtm, DbType.Int32);
				//	comando.AdicionarParametroEntrada("northing_utm", item.Coordenada.NorthingUtm, DbType.Int32);
				//	comando.AdicionarParametroEntrada("fuso_utm", item.Coordenada.FusoUtm, DbType.Int32);
				//	comando.AdicionarParametroEntrada("hemisferio_utm", item.Coordenada.HemisferioUtm, DbType.Int32);
				//	comando.AdicionarParametroEntrada("municipio", item.Municipio.Id > 0 ? (object)item.Municipio.Id : DBNull.Value, DbType.Int32);
				//	comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				//	bancoDeDados.ExecutarNonQuery(comando);

				//	#endregion

				//	#region Produtores

				//	foreach (var produtor in item.Produtores)
				//	{
				//		if (produtor.IdRelacionamento > 0)
				//		{
				//			comando = bancoDeDados.CriarComando(@"update crt_unidade_prod_un_produtor set produtor = :produtor, tid = :tid where id = :id", EsquemaBanco);
				//			comando.AdicionarParametroEntrada("id", produtor.IdRelacionamento, DbType.Int32);
				//		}
				//		else
				//		{
				//			comando = bancoDeDados.CriarComando(@"
				//			insert into crt_unidade_prod_un_produtor (id, tid, unidade_producao_unidade, produtor) values
				//			(seq_crt_unidade_prod_un_produt.nextval, :tid, :unidade_producao_unidade, :produtor)");
				//			comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
				//		}

				//		comando.AdicionarParametroEntrada("produtor", produtor.Id, DbType.Int32);
				//		comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//		bancoDeDados.ExecutarNonQuery(comando);
				//	}

				//	#endregion

				//	#region Responsáveis técnicos

				//	foreach (ResponsavelTecnico responsavel in item.ResponsaveisTecnicos)
				//	{
				//		if (responsavel.IdRelacionamento > 0)
				//		{
				//			comando = bancoDeDados.CriarComando(@"
				//			update crt_unidade_prod_un_resp_tec set tid =:tid, responsavel_tecnico =: responsavel_tecnico, numero_hab_cfo_cfoc =: numero_hab_cfo_cfoc,
				//			numero_art =:numero_art, art_cargo_funcao =: art_cargo_funcao, data_validade_art =: data_validade_art where id = :id_rel", EsquemaCredenciadoBanco);

				//			comando.AdicionarParametroEntrada("id_rel", responsavel.IdRelacionamento, DbType.Int32);
				//		}
				//		else
				//		{
				//			comando = bancoDeDados.CriarComando(@"
				//			insert into {0}crt_unidade_prod_un_resp_tec
				//			(id, tid, unidade_producao_unidade, responsavel_tecnico, numero_hab_cfo_cfoc, numero_art, art_cargo_funcao, data_validade_art) values
				//			(seq_crt_unid_prod_un_resp_tec.nextval, :tid, :unidade_producao_unidade, :responsavel_tecnico, :numero_hab_cfo_cfoc, :numero_art, :art_cargo_funcao, :data_validade_art)", EsquemaCredenciadoBanco);

				//			comando.AdicionarParametroEntrada("unidade_producao_unidade", item.Id, DbType.Int32);
				//		}

				//		comando.AdicionarParametroEntrada("responsavel_tecnico", responsavel.Id, DbType.Int32);
				//		comando.AdicionarParametroEntrada("numero_hab_cfo_cfoc", responsavel.CFONumero, DbType.String);
				//		comando.AdicionarParametroEntrada("art_cargo_funcao", responsavel.ArtCargoFuncao, DbType.Int32);
				//		comando.AdicionarParametroEntrada("numero_art", responsavel.NumeroArt, DbType.String);
				//		comando.AdicionarParametroEntrada("data_validade_art", responsavel.DataValidadeART, DbType.String);
				//		comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				//		bancoDeDados.ExecutarNonQuery(comando);
				//	}

				//	#endregion
				//}

				//#endregion

				//Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.unidadeproducao, eHistoricoAcao.atualizar, bancoDeDados, null);

				//bancoDeDados.Commit();
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

		#endregion
	}
}