using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegularizacaoFundiaria.Data
{
	class RegularizacaoFundiariaDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		internal Historico Historico { get { return _historico; } }

		private String EsquemaBanco { get; set; }

		#endregion

		public RegularizacaoFundiariaDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(RegularizacaoFundiaria regularizacao, BancoDeDados banco)
		{
			if (regularizacao == null)
			{
				throw new Exception("A Caracterização de Regularização fundiária é nula.");
			}

			if (regularizacao.Id <= 0)
			{
				Criar(regularizacao, banco);
			}
			else
			{
				Editar(regularizacao, banco);
			}
		}

		internal int? Criar(RegularizacaoFundiaria regularizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Regularização fundiária



				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao c (id, empreendimento, tid) values
				({0}seq_crt_regularizacao.nextval, :empreendimento, :tid) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", regularizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				regularizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Domínios/Posse

				if (regularizacao.Posses != null && regularizacao.Posses.Count > 0)
				{
					foreach (Posse item in regularizacao.Posses)
					{
						#region Domínios/Posse
						//ver se vai retirar o dominio
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_regularizacao_dominio d (id, regularizacao, dominio, zona, identificacao, area_requerida, area_croqui, perimetro, regularizacao_tipo, 
						relacao_trabalho, benfeitorias, observacoes, possui_dominio_avulso, tid, comprovacao, area_documento, data_ultima_atualizacao, registro,
						numero_ccri, area_ccri, confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste) 
						values 
						({0}seq_crt_regularizacao_dominio.nextval, :regularizacao, :dominio, :zona, :identificacao, :area_requerida, :area_croqui, :perimetro, :regularizacao_tipo, 
						:relacao_trabalho, :benfeitorias, :observacoes, :possui_dominio_avulso, :tid, :comprovacao, :area_documento, :data_ultima_atualizacao, :registro, :numero_ccri,
						:area_ccri, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste) returning d.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("dominio", item.Dominio, DbType.Int32);
						comando.AdicionarParametroEntrada("zona", item.Zona, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("area_requerida", item.AreaRequerida, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("perimetro", item.Perimetro, DbType.Decimal);
						comando.AdicionarParametroEntrada("regularizacao_tipo", item.RegularizacaoTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("relacao_trabalho", item.RelacaoTrabalho, DbType.Int32);
						comando.AdicionarParametroEntrada("benfeitorias", DbType.String, 500, item.Benfeitorias);
						comando.AdicionarParametroEntrada("observacoes", DbType.String, 500, item.Observacoes);
						comando.AdicionarParametroEntrada("possui_dominio_avulso", item.PossuiDominioAvulso, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						comando.AdicionarParametroEntrada("comprovacao", item.ComprovacaoId > 0 ? item.ComprovacaoId : (object)DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("area_documento", item.AreaPosseDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("data_ultima_atualizacao", item.DataUltimaAtualizacaoCCIR.Data, DbType.DateTime);
						comando.AdicionarParametroEntrada("registro", DbType.String, 400, item.DescricaoComprovacao);
						comando.AdicionarParametroEntrada("numero_ccri", item.NumeroCCIR, DbType.Int64);
						comando.AdicionarParametroEntrada("area_ccri", item.AreaCCIR, DbType.Decimal);
						comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 400, item.ConfrontacoesNorte);
						comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 400, item.ConfrontacoesSul);
						comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 400, item.ConfrontacoesLeste);
						comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 400, item.ConfrontacoesOeste);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#endregion

						#region Dominios Avulsos

						if (item.DominiosAvulsos != null && item.DominiosAvulsos.Count > 0)
						{
							foreach (Dominio itemAux in item.DominiosAvulsos)
							{
								comando = bancoDeDados.CriarComando(@"
								insert into {0}crt_regula_dominio_avulso d (id, tid, dominio, matricula, folha, livro, cartorio, documento_area, ccir, area_ccir, data_ultima_atualizacao) 
								values ({0}seq_crt_regula_dominio_avulso.nextval, :tid, :dominio, :matricula, :folha, :livro, :cartorio, :documento_area, :ccir, :area_ccir, 
								:data_ultima_atualizacao)", EsquemaBanco);

								comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("matricula", DbType.String, 24, itemAux.Matricula);
								comando.AdicionarParametroEntrada("folha", DbType.String, 24, itemAux.Folha);
								comando.AdicionarParametroEntrada("livro", DbType.String, 24, itemAux.Livro);
								comando.AdicionarParametroEntrada("cartorio", DbType.String, 150, itemAux.Cartorio);
								comando.AdicionarParametroEntrada("documento_area", itemAux.AreaDocumento, DbType.Decimal);
								comando.AdicionarParametroEntrada("ccir", itemAux.NumeroCCIR, DbType.Int64);
								comando.AdicionarParametroEntrada("area_ccir", (itemAux.AreaCCIR <= 0 ? null : (object)itemAux.AreaCCIR), DbType.Decimal);
								comando.AdicionarParametroEntrada("data_ultima_atualizacao", itemAux.DataUltimaAtualizacao.Data, DbType.DateTime);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion

						#region Ocupações/Opções

						if (item.Opcoes != null && item.Opcoes.Count > 0)
						{
							foreach (Opcao itemAux in item.Opcoes)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_ocupacao d (id, dominio, tipo, valor, outro, tid) values 
								({0}seq_crt_regularizacao_ocupacao.nextval, :dominio, :tipo, :valor, :outro, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo", itemAux.Tipo, DbType.Int32);
								comando.AdicionarParametroEntrada("valor", DbType.String, 200, itemAux.Valor);
								comando.AdicionarParametroEntrada("outro", DbType.String, 200, itemAux.Outro);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion

						#region Transmitentes de Posse

						if (item.Transmitentes != null && item.Transmitentes.Count > 0)
						{
							foreach (TransmitentePosse itemAux in item.Transmitentes)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_transmite d (id, dominio, pessoa, tempo, tid) values 
								({0}seq_crt_regularizacao_transmit.nextval, :dominio, :pessoa, :tempo, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("pessoa", itemAux.Transmitente.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tempo", itemAux.TempoOcupacao, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion

						#region Uso atual do solo

						if (item.UsoAtualSolo != null && item.UsoAtualSolo.Count > 0)
						{
							foreach (UsoAtualSolo itemAux in item.UsoAtualSolo)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_uso_solo d (id, dominio, tipo, tipo_geo, area, tid) values 
								({0}seq_crt_regularizacao_uso_solo.nextval, :dominio, :tipo, :tipo_geo, :area, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo", itemAux.TipoDeUso, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo_geo", DbType.String, 100, itemAux.TipoDeUsoGeo);
								comando.AdicionarParametroEntrada("area", itemAux.AreaPorcentagem, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion

						#region Edificações

						if (item.Edificacoes != null && item.Edificacoes.Count > 0)
						{
							foreach (Edificacao itemAux in item.Edificacoes)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_edifica d (id, dominio, tipo, quantidade, tid) values 
								({0}seq_crt_regularizacao_edifica.nextval, :dominio, :tipo, :quantidade, :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo", DbType.String, 80, itemAux.Tipo);
								comando.AdicionarParametroEntrada("quantidade", DbType.String, 5, itemAux.Quantidade);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion
					}
				}

				#endregion

				Historico.Gerar(regularizacao.Id, eHistoricoArtefatoCaracterizacao.regularizacaofundiaria, eHistoricoAcao.criar, bancoDeDados, null);

				bancoDeDados.Commit();

				return regularizacao.Id;
			}
		}

		internal void Editar(RegularizacaoFundiaria regularizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Regularização fundiária

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_regularizacao c set c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", regularizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				#region Dominios Avulsos

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_regula_dominio_avulso c where c.dominio in 
				(select a.id from {0}crt_regularizacao_dominio a where a.regularizacao = :regularizacao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, regularizacao.Posses.SelectMany(x => x.DominiosAvulsos).Select(y => y.Id).ToList());

				comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Ocupações/Opções

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_regularizacao_ocupacao c where c.dominio in 
				(select a.id from {0}crt_regularizacao_dominio a where a.regularizacao = :regularizacao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, regularizacao.Posses.SelectMany(x => x.Opcoes).Select(y => y.Id).ToList());

				comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Transmitentes de Posse

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_regularizacao_transmite c where c.dominio in 
				(select a.id from {0}crt_regularizacao_dominio a where a.regularizacao = :regularizacao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, regularizacao.Posses.SelectMany(x => x.Transmitentes).Select(x => x.Id).ToList());

				comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Uso atual do solo

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_regularizacao_uso_solo c where c.dominio in 
				(select a.id from {0}crt_regularizacao_dominio a where a.regularizacao = :regularizacao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, regularizacao.Posses.SelectMany(x => x.UsoAtualSolo).Select(x => x.Id).ToList());

				comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Edificações

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_regularizacao_edifica c where c.dominio in 
				(select a.id from {0}crt_regularizacao_dominio a where a.regularizacao = :regularizacao)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, regularizacao.Posses.SelectMany(x => x.Edificacoes).Select(x => x.Id).ToList());

				comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Domínios/Posse

				comando = bancoDeDados.CriarComando("delete from {0}crt_regularizacao_dominio c where c.regularizacao = :regularizacao", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.id", DbType.Int32, regularizacao.Posses.Select(x => x.Id).ToList());

				comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#endregion

				#region Domínios/Posse

				if (regularizacao.Posses != null && regularizacao.Posses.Count > 0)
				{
					foreach (Posse item in regularizacao.Posses)
					{
						if (!string.IsNullOrWhiteSpace(item.Tid))
						{
							continue;
						}

						#region Domínios/Posse

						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_regularizacao_dominio d set d.dominio = :dominio, d.zona = :zona, d.identificacao = :identificacao, 
							d.area_requerida = :area_requerida, d.area_croqui = :area_croqui, d.perimetro = :perimetro, d.regularizacao_tipo = :regularizacao_tipo, d.relacao_trabalho= :relacao_trabalho, 
							d.benfeitorias = :benfeitorias, d.observacoes = :observacoes, d.possui_dominio_avulso = :possui_dominio_avulso, d.tid = :tid, d.comprovacao = :comprovacao, d.area_documento = :area_documento, d.data_ultima_atualizacao = :data_ultima_atualizacao,
							d.registro = :registro, d.numero_ccri = :numero_ccri, d.area_ccri = :area_ccri, d.confrontante_norte = :confrontante_norte,  d.confrontante_sul = :confrontante_sul, d.confrontante_leste = :confrontante_leste, d.confrontante_oeste = :confrontante_oeste where d.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_regularizacao_dominio d (id, regularizacao, dominio, zona, identificacao, area_requerida, area_croqui, perimetro, regularizacao_tipo, 
							relacao_trabalho, benfeitorias, observacoes, possui_dominio_avulso, tid, comprovacao, area_documento, data_ultima_atualizacao, registro,
							numero_ccri, area_ccri, confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste) 
							values 
							({0}seq_crt_regularizacao_dominio.nextval, :regularizacao, :dominio, :zona, :identificacao, :area_requerida, :area_croqui, :perimetro, :regularizacao_tipo, 
							:relacao_trabalho, :benfeitorias, :observacoes, :possui_dominio_avulso, :tid, :comprovacao, :area_documento, :data_ultima_atualizacao, :registro, :numero_ccri,
							:area_ccri, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste) returning d.id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("regularizacao", regularizacao.Id, DbType.Int32);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("dominio", item.Dominio, DbType.Int32);
						comando.AdicionarParametroEntrada("zona", item.Zona, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("area_requerida", item.AreaRequerida, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("perimetro", item.Perimetro, DbType.Decimal);
						comando.AdicionarParametroEntrada("regularizacao_tipo", item.RegularizacaoTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("relacao_trabalho", item.RelacaoTrabalho, DbType.Int32);
						comando.AdicionarParametroEntrada("benfeitorias", DbType.String, 500, item.Benfeitorias);
						comando.AdicionarParametroEntrada("observacoes", DbType.String, 500, item.Observacoes);
						comando.AdicionarParametroEntrada("possui_dominio_avulso", item.PossuiDominioAvulso, DbType.Int32);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						comando.AdicionarParametroEntrada("comprovacao", item.ComprovacaoId > 0 ? item.ComprovacaoId : (object)DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("area_documento", item.AreaPosseDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("data_ultima_atualizacao", item.DataUltimaAtualizacaoCCIR.Data, DbType.DateTime);
						comando.AdicionarParametroEntrada("registro", DbType.String, 400, item.DescricaoComprovacao);
						comando.AdicionarParametroEntrada("numero_ccri", item.NumeroCCIR, DbType.Int64);
						comando.AdicionarParametroEntrada("area_ccri", item.AreaCCIR, DbType.Decimal);
						comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 400, item.ConfrontacoesNorte);
						comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 400, item.ConfrontacoesSul);
						comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 400, item.ConfrontacoesLeste);
						comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 400, item.ConfrontacoesOeste);

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id.GetValueOrDefault() <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#endregion

						#region Dominios Avulsos

						if (item.DominiosAvulsos != null && item.DominiosAvulsos.Count > 0)
						{
							foreach (Dominio itemAux in item.DominiosAvulsos)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_regula_dominio_avulso d 
									set d.tid = :tid, d.matricula = :matricula, d.folha = :folha, d.livro = :livro, d.cartorio = :cartorio, d.documento_area = :documento_area, 
									d.ccir = :ccir, d.area_ccir = :area_ccir, d.data_ultima_atualizacao = :data_ultima_atualizacao where d.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"
									insert into {0}crt_regula_dominio_avulso d (id, tid, dominio, matricula, folha, livro, cartorio, documento_area, ccir, area_ccir, data_ultima_atualizacao) 
									values ({0}seq_crt_regula_dominio_avulso.nextval, :tid, :dominio, :matricula, :folha, :livro, :cartorio, :documento_area, :ccir, :area_ccir, 
									:data_ultima_atualizacao)", EsquemaBanco);

									comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								}

								comando.AdicionarParametroEntrada("matricula", DbType.String, 24, itemAux.Matricula);
								comando.AdicionarParametroEntrada("folha", DbType.String, 24, itemAux.Folha);
								comando.AdicionarParametroEntrada("livro", DbType.String, 24, itemAux.Livro);
								comando.AdicionarParametroEntrada("cartorio", DbType.String, 150, itemAux.Cartorio);
								comando.AdicionarParametroEntrada("documento_area", itemAux.AreaDocumento, DbType.Decimal);
								comando.AdicionarParametroEntrada("ccir", itemAux.NumeroCCIR, DbType.Int64);
								comando.AdicionarParametroEntrada("area_ccir", (itemAux.AreaCCIR <= 0 ? null : (object)itemAux.AreaCCIR), DbType.Decimal);
								comando.AdicionarParametroEntrada("data_ultima_atualizacao", itemAux.DataUltimaAtualizacao.Data, DbType.DateTime);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion

						#region Ocupações/Opções

						if (item.Opcoes != null && item.Opcoes.Count > 0)
						{
							foreach (Opcao itemAux in item.Opcoes)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_regularizacao_ocupacao d 
									set d.tipo = :tipo, d.valor = :valor, d.outro= :outro, d.tid = :tid where d.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_ocupacao d (id, dominio, tipo, valor, outro, tid) 
									values ({0}seq_crt_regularizacao_ocupacao.nextval, :dominio, :tipo, :valor, :outro, :tid)", EsquemaBanco);

									comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								}

								comando.AdicionarParametroEntrada("tipo", itemAux.Tipo, DbType.Int32);
								comando.AdicionarParametroEntrada("valor", DbType.String, 200, itemAux.Valor);
								comando.AdicionarParametroEntrada("outro", DbType.String, 200, itemAux.Outro);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion

						#region Transmitentes de Posse

						if (item.Transmitentes != null && item.Transmitentes.Count > 0)
						{
							foreach (TransmitentePosse itemAux in item.Transmitentes)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_regularizacao_transmite d 
									set d.pessoa = :pessoa, d.tempo = :tempo, d.tid = :tid where d.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_transmite d (id, dominio, pessoa, tempo, tid) 
									values ({0}seq_crt_regularizacao_transmit.nextval, :dominio, :pessoa, :tempo, :tid)", EsquemaBanco);

									comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								}

								comando.AdicionarParametroEntrada("pessoa", itemAux.Transmitente.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tempo", itemAux.TempoOcupacao, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}
						#endregion

						#region Uso atual do solo

						if (item.UsoAtualSolo != null && item.UsoAtualSolo.Count > 0)
						{
							foreach (UsoAtualSolo itemAux in item.UsoAtualSolo)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_regularizacao_uso_solo d 
									set d.tipo = :tipo, d.tipo_geo = :tipo_geo, d.area = :area, d.tid = :tid where d.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_uso_solo d (id, dominio, tipo, tipo_geo, area, tid) 
									values ({0}seq_crt_regularizacao_uso_solo.nextval, :dominio, :tipo, :tipo_geo, :area, :tid)", EsquemaBanco);

									comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								}

								comando.AdicionarParametroEntrada("tipo", itemAux.TipoDeUso, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo_geo", DbType.String, 100, itemAux.TipoDeUsoGeo);
								comando.AdicionarParametroEntrada("area", itemAux.AreaPorcentagem, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion

						#region Edificações

						if (item.Edificacoes != null && item.Edificacoes.Count > 0)
						{
							foreach (Edificacao itemAux in item.Edificacoes)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_regularizacao_edifica d 
									set d.tipo = :tipo, d.quantidade = :quantidade, d.tid = :tid where d.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_regularizacao_edifica d (id, dominio, tipo, quantidade, tid) 
									values ({0}seq_crt_regularizacao_edifica.nextval, :dominio, :tipo, :quantidade, :tid)", EsquemaBanco);

									comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								}

								comando.AdicionarParametroEntrada("tipo", DbType.String, 80, itemAux.Tipo);
								comando.AdicionarParametroEntrada("quantidade", DbType.String, 5, itemAux.Quantidade);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				Historico.Gerar(regularizacao.Id, eHistoricoArtefatoCaracterizacao.regularizacaofundiaria, eHistoricoAcao.atualizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				int id = 0;

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_regularizacao c where c.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno == null || Convert.IsDBNull(retorno))
				{
					return;
				}

				id = Convert.ToInt32(retorno); ;

				#endregion

				#region Histórico

				//Atualizar o tid para a nova ação
				comando = bancoDeDados.CriarComando(@"update {0}crt_regularizacao c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.regularizacaofundiaria, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;
					delete from {0}crt_regularizacao_edifica r where r.dominio in (select d.id from {0}crt_regularizacao_dominio d where d.regularizacao = :caracterizacao);
					delete from {0}crt_regularizacao_ocupacao r where r.dominio in (select d.id from {0}crt_regularizacao_dominio d where d.regularizacao = :caracterizacao);
					delete from {0}crt_regularizacao_transmite r where r.dominio in (select d.id from {0}crt_regularizacao_dominio d where d.regularizacao = :caracterizacao);
					delete from {0}crt_regularizacao_uso_solo r where r.dominio in (select d.id from {0}crt_regularizacao_dominio d where d.regularizacao = :caracterizacao);
					delete from {0}crt_regula_dominio_avulso r where r.dominio in (select d.id from {0}crt_regularizacao_dominio d where d.regularizacao = :caracterizacao);
					delete from {0}crt_regularizacao_dominio b where b.regularizacao = :caracterizacao;
					delete from {0}crt_regularizacao r where r.id = :caracterizacao;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.RegularizacaoFundiaria, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal RegularizacaoFundiaria ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			RegularizacaoFundiaria regularizacao = new RegularizacaoFundiaria();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_regularizacao s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					regularizacao = Obter(Convert.ToInt32(valor), simplificado, bancoDeDados);
				}
			}

			return regularizacao;
		}

		internal RegularizacaoFundiaria Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			RegularizacaoFundiaria regularizacao = new RegularizacaoFundiaria();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Regularização Fundiária

				Comando comando = bancoDeDados.CriarComando(@"select d.id, d.empreendimento, d.tid from {0}crt_regularizacao d where d.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						regularizacao.Id = id;
						regularizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						regularizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (regularizacao.Id <= 0 || simplificado)
				{
					return regularizacao;
				}

				#region Domínios/Posse

				comando = bancoDeDados.CriarComando(@"select p.id, p.regularizacao, p.dominio, p.zona, p.identificacao, p.area_requerida, p.area_croqui, p.regularizacao_tipo,
					p.relacao_trabalho, p.centro_comercial_km, p.br_km, p.es_km, p.benfeitorias, p.observacoes, p.tid, p.possui_dominio_avulso, p.perimetro, p.comprovacao, lc.texto as comprovacaotexto, p.area_documento,
					p.data_ultima_atualizacao, p.registro, p.numero_ccri, p.area_ccri, p.confrontante_norte, p.confrontante_sul, p.confrontante_leste, p.confrontante_oeste, p.registro, p.comprovacao
					from {0}crt_regularizacao_dominio p
					inner join crt_regularizacao r on r.id = p.regularizacao
					inner join lov_crt_domin_comprovacao lc on lc.id  = p.comprovacao
					where r.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", regularizacao.EmpreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Posse posse = null;

					if (((System.Data.Common.DbDataReader)reader).HasRows)
					{
						while (reader.Read())
						{
							#region Domínios/Posse

							posse = new Posse();

							posse.Id = Convert.ToInt32(reader["id"]);
							posse.Dominio = reader.GetValue<int>("dominio");
							posse.AreaCCIR = reader.GetValue<decimal>("area_ccri");
							posse.NumeroCCIR = reader.GetValue<long?>("numero_ccri");
							posse.AreaRequerida = reader.GetValue<decimal>("area_requerida");
							posse.ComprovacaoTexto = reader["comprovacaotexto"].ToString();
							posse.ComprovacaoId = Convert.ToInt32(reader["comprovacao"]);
							posse.AreaPosseDocumento = reader.GetValue<decimal>("area_documento");
							posse.AreaCroqui = reader.GetValue<decimal>("area_croqui");
							posse.Identificacao = reader["identificacao"].ToString();
							posse.DescricaoComprovacao = reader["registro"].ToString();
							posse.RegularizacaoTipo = Convert.ToInt32(reader["regularizacao_tipo"]);
							posse.RelacaoTrabalho = Convert.ToInt32(reader["relacao_trabalho"]);
							posse.Benfeitorias = reader["benfeitorias"].ToString();
							posse.Observacoes = reader["observacoes"].ToString();
							posse.Tid = reader["tid"].ToString();
							posse.PossuiDominioAvulso = reader.GetValue<int?>("possui_dominio_avulso");
							posse.Perimetro = reader.GetValue<decimal>("perimetro");
							posse.Distancia.BrAPosse = reader.GetValue<decimal?>("br_km");
							posse.Distancia.EsAPosse = reader.GetValue<decimal?>("es_km");
							posse.DataUltimaAtualizacaoCCIR.DataTexto = reader.GetValue<string>("data_ultima_atualizacao");
							posse.ConfrontacoesNorte = reader["confrontante_norte"].ToString();
							posse.ConfrontacoesSul = reader["confrontante_sul"].ToString();
							posse.ConfrontacoesLeste = reader["confrontante_leste"].ToString();
							posse.ConfrontacoesOeste = reader["confrontante_oeste"].ToString();

							#endregion

							ObterDadosDominio(posse, bancoDeDados);

							regularizacao.Posses.Add(posse);
						}
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"SELECT a.id, a.area_m2, a.codigo, sdo_geom.sdo_length(a.geometry, 0.00001) perimetro
							FROM IDAFGEO.geo_aativ a
							INNER JOIN crt_projeto_geo b ON a.projeto = b.id WHERE b.CARACTERIZACAO =
							" + (int)eCaracterizacao.RegularizacaoFundiaria + " AND b.EMPREENDIMENTO = :empreendimento", EsquemaBanco);

						comando.AdicionarParametroEntrada("empreendimento", regularizacao.EmpreendimentoId, DbType.Int32);

						while (reader.Read())
						{
							posse = new Posse();
							posse.Id = Convert.ToInt32(reader["id"]);
							posse.Identificacao = reader["codigo"].ToString();
							posse.AreaCroqui = reader.GetValue<decimal>("area_m2");
							posse.Perimetro = reader.GetValue<decimal>("perimetro");

							regularizacao.Posses.Add(posse);
						}
					}

					reader.Close();
				}
				#endregion
			}

			return regularizacao;
		}

		internal Posse ObterDadosDominio(Posse posse, BancoDeDados bancoDeDados)
		{
			Comando comando;

			#region Dominios Avulsos

			comando = bancoDeDados.CriarComando(@"
						select d.id, d.tid, d.matricula, d.folha, d.livro, d.cartorio, d.documento_area, d.ccir, d.area_ccir, 
						to_char(d.data_ultima_atualizacao, 'dd/MM/yyyy') data_ultima_atualizacao 
						from crt_regula_dominio_avulso d where d.dominio = :id", EsquemaBanco);

			comando.AdicionarParametroEntrada("id", posse.Id, DbType.Int32);

			using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
			{
				Dominio dominio = null;
				while (readerAux.Read())
				{
					dominio = new Dominio();
					dominio.Id = readerAux.GetValue<int>("id");
					dominio.Tid = readerAux.GetValue<string>("tid");
					dominio.Matricula = readerAux.GetValue<string>("matricula");
					dominio.Folha = readerAux.GetValue<string>("folha");
					dominio.Livro = readerAux.GetValue<string>("livro");
					dominio.Cartorio = readerAux.GetValue<string>("cartorio");
					dominio.AreaDocumento = readerAux.GetValue<decimal>("documento_area");
					dominio.NumeroCCIR = readerAux.GetValue<long?>("ccir");
					dominio.AreaCCIR = readerAux.GetValue<decimal>("area_ccir");
					dominio.DataUltimaAtualizacao.DataTexto = readerAux.GetValue<String>("data_ultima_atualizacao");

					posse.DominiosAvulsos.Add(dominio);
				}

				readerAux.Close();
			}

			#endregion

			#region Ocupações/Opções

			comando = bancoDeDados.CriarComando(@"select o.id, o.tipo, o.valor, o.outro, o.tid 
						from {0}crt_regularizacao_ocupacao o where o.dominio = :id", EsquemaBanco);

			comando.AdicionarParametroEntrada("id", posse.Id, DbType.Int32);

			using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
			{
				Opcao opcao = null;
				while (readerAux.Read())
				{
					opcao = new Opcao();
					opcao.Tid = readerAux.GetValue<string>("tid");
					opcao.Outro = readerAux.GetValue<string>("outro");
					opcao.Id = readerAux.GetValue<int>("id");
					opcao.Tipo = readerAux.GetValue<int>("tipo");
					opcao.Valor = readerAux.GetValue<int>("valor");

					posse.Opcoes.Add(opcao);
				}

				readerAux.Close();
			}

			#endregion Ocupações/Opções

			#region Transmitentes de Posse

			comando = bancoDeDados.CriarComando(@"select t.id, t.pessoa, t.tempo, nvl(p.nome,p.razao_social) nome , nvl( p.cpf, p.cnpj ) cpfCnpj , t.tid 
						from crt_regularizacao_transmite t, tab_pessoa p where t.pessoa = p.id AND t.dominio = :id", EsquemaBanco);

			comando.AdicionarParametroEntrada("id", posse.Id, DbType.Int32);

			using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
			{
				TransmitentePosse transmitente = null;
				while (readerAux.Read())
				{
					transmitente = new TransmitentePosse();
					transmitente.Tid = readerAux.GetValue<string>("tid");
					transmitente.Id = readerAux.GetValue<int>("id");
					transmitente.Transmitente.Id = readerAux.GetValue<int>("pessoa");
					transmitente.Transmitente.Fisica.Nome = readerAux.GetValue<string>("nome");
					transmitente.Transmitente.Fisica.CPF = readerAux.GetValue<string>("cpfCnpj");
					transmitente.TempoOcupacao = readerAux.GetValue<int>("tempo");

					posse.Transmitentes.Add(transmitente);
				}

				readerAux.Close();
			}

			#endregion

			#region Uso atual do solo

			comando = bancoDeDados.CriarComando(@"select u.id, u.tipo, l.tipo_geo, l.id, l.texto tipo_texto, u.area, u.tid 
						from {0}crt_regularizacao_uso_solo u, {0}lov_crt_regularizacao_tip_uso l where u.dominio = :id and u.tipo = l.id", EsquemaBanco);

			comando.AdicionarParametroEntrada("id", posse.Id, DbType.Int32);

			using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
			{
				UsoAtualSolo uso = null;
				while (readerAux.Read())
				{
					uso = new UsoAtualSolo();
					uso.Tid = readerAux.GetValue<string>("tid");
					uso.Id = readerAux.GetValue<int>("id");
					uso.TipoDeUso = readerAux.GetValue<int>("tipo");
					uso.TipoDeUsoTexto = readerAux.GetValue<string>("tipo_texto");
					uso.TipoDeUsoGeo = readerAux.GetValue<string>("tipo_geo");
					uso.AreaPorcentagem = readerAux.GetValue<int>("area");

					posse.UsoAtualSolo.Add(uso);
				}

				readerAux.Close();
			}

			#endregion

			#region Edificações

			comando = bancoDeDados.CriarComando(@"select e.id, e.tipo, e.quantidade, e.tid 
						from {0}crt_regularizacao_edifica e where e.dominio = :id", EsquemaBanco);

			comando.AdicionarParametroEntrada("id", posse.Id, DbType.Int32);

			using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
			{
				Edificacao edificacao = null;
				while (readerAux.Read())
				{
					edificacao = new Edificacao();
					edificacao.Tid = readerAux.GetValue<string>("tid");
					edificacao.Tipo = readerAux.GetValue<string>("tipo");
					edificacao.Id = readerAux.GetValue<int>("id");
					edificacao.Quantidade = readerAux.GetValue<int>("quantidade");

					posse.Edificacoes.Add(edificacao);
				}

				readerAux.Close();
			}

			#endregion

			return posse;
		}

		internal RegularizacaoFundiaria ObterHistorico(int id, string tid, bool simplificado = false, BancoDeDados banco = null)
		{
			RegularizacaoFundiaria regularizacao = new RegularizacaoFundiaria();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Regularização fundiária

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.empreendimento_id, c.empreendimento_tid 
				from {0}hst_crt_regularizacao c where c.regularizacao_id = :id and d.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						regularizacao.Id = id;
						regularizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						regularizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (regularizacao.Id <= 0 || simplificado)
				{
					return regularizacao;
				}

				#region Domínios/Posse

				comando = bancoDeDados.CriarComando(@"select d.id, d.dominio_id, d.regularizacao_dominio_id, d.zona, d.identificacao, d.area_requerida, d.area_croqui, d.perimetro, 
				d.regularizacao_tipo_id, d.relacao_trabalho, d.centro_comercial_km, d.br_km, d.es_km, d.benfeitorias, d.observacoes, d.possui_dominio_avulso, d.tid 
				from {0}hst_crt_regularizacao_dominio d where d.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Posse posse = null;

					while (reader.Read())
					{
						#region Domínios/Posse

						hst = reader.GetValue<int>("id");

						posse = new Posse();
						posse.Id = reader.GetValue<int>("regularizacao_dominio_id");
						posse.Dominio = reader.GetValue<int>("dominio_id");
						posse.Tid = reader.GetValue<string>("tid");
						posse.Identificacao = reader.GetValue<string>("identificacao");
						posse.Benfeitorias = reader.GetValue<string>("benfeitorias");
						posse.Observacoes = reader.GetValue<string>("observacoes");
						posse.AreaRequerida = reader.GetValue<decimal>("area_requerida");
						posse.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						posse.Perimetro = reader.GetValue<decimal>("perimetro");
						posse.Zona = reader.GetValue<int>("zona");
						posse.RegularizacaoTipo = reader.GetValue<int>("regularizacao_tipo_id");
						posse.RelacaoTrabalho = reader.GetValue<int>("relacao_trabalho");
						posse.PossuiDominioAvulso = reader.GetValue<int?>("possui_dominio_avulso");
						posse.Distancia.PosseCentroComercial = reader.GetValue<decimal>("centro_comercial_km");
						posse.Distancia.BrAPosse = reader.GetValue<decimal>("br_km");
						posse.Distancia.EsAPosse = reader.GetValue<decimal>("es_km");

						#endregion

						#region Dominios Avulsos

						comando = bancoDeDados.CriarComando(@"
						select d.reg_domi_avulso_id id, d.tid, d.matricula, d.folha, d.livro, d.cartorio, d.documento_area, d.ccir, d.area_ccir, 
						to_char(d.data_ultima_atualizacao, 'dd/MM/yyyy') data_ultima_atualizacao 
						from hst_crt_regula_domi_avulso d where d.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Dominio dominio = null;
							while (readerAux.Read())
							{
								dominio = new Dominio();
								dominio.Id = readerAux.GetValue<int>("id");
								dominio.Tid = readerAux.GetValue<string>("tid");
								dominio.Matricula = readerAux.GetValue<string>("matricula");
								dominio.Folha = readerAux.GetValue<string>("folha");
								dominio.Livro = readerAux.GetValue<string>("livro");
								dominio.Cartorio = readerAux.GetValue<string>("cartorio");
								dominio.AreaDocumento = readerAux.GetValue<decimal>("documento_area");
								dominio.NumeroCCIR = readerAux.GetValue<long?>("ccir");
								dominio.AreaCCIR = readerAux.GetValue<decimal>("area_ccir");
								dominio.DataUltimaAtualizacao.DataTexto = readerAux.GetValue<string>("data_ultima_atualizacao");

								posse.DominiosAvulsos.Add(dominio);
							}

							readerAux.Close();
						}

						#endregion

						#region Ocupações/Opções

						comando = bancoDeDados.CriarComando(@"select o.regularizacao_ocupacao_id, o.tipo_id, o.valor, o.outro, o.tid 
						from {0}hst_crt_regularizacao_ocupacao o where o.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Opcao opcao = null;
							while (readerAux.Read())
							{
								opcao = new Opcao();
								opcao.Id = readerAux.GetValue<int>("regularizacao_ocupacao_id");
								opcao.Tid = readerAux.GetValue<string>("tid");
								opcao.Outro = readerAux.GetValue<string>("outro");
								opcao.Tipo = readerAux.GetValue<int>("tipo_id");
								opcao.Valor = readerAux.GetValue<int>("valor");

								posse.Opcoes.Add(opcao);
							}

							readerAux.Close();
						}

						#endregion

						#region Transmitentes de Posse

						comando = bancoDeDados.CriarComando(@"select t.regularizacao_transmite_id, t.pessoa_id, t.tempo, t.tid 
						from {0}hst_crt_regularizacao_transmi t where t.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							TransmitentePosse transmitente = null;
							while (readerAux.Read())
							{
								transmitente = new TransmitentePosse();
								transmitente.Id = readerAux.GetValue<int>("regularizacao_transmite_id");
								transmitente.Tid = readerAux.GetValue<string>("tid");
								transmitente.Transmitente.Id = readerAux.GetValue<int>("pessoa_id");
								transmitente.TempoOcupacao = readerAux.GetValue<int>("tempo");

								posse.Transmitentes.Add(transmitente);
							}

							readerAux.Close();
						}

						#endregion

						#region Uso atual do solo

						comando = bancoDeDados.CriarComando(@"select u.regularizacao_uso_solo_id, u.tipo_id, u.tipo_texto, u.tipo_geo, u.area, u.tid 
						from {0}hst_crt_regularizacao_uso_solo u, {0}lov_crt_regularizacao_tip_uso l where u.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							UsoAtualSolo uso = null;
							while (readerAux.Read())
							{
								uso = new UsoAtualSolo();
								uso.Id = readerAux.GetValue<int>("regularizacao_uso_solo_id");
								uso.Tid = readerAux.GetValue<string>("tid");
								uso.TipoDeUso = readerAux.GetValue<int>("tipo_id");
								uso.TipoDeUsoTexto = readerAux.GetValue<string>("tipo_texto");
								uso.TipoDeUsoGeo = readerAux.GetValue<string>("tipo_geo");
								uso.AreaPorcentagem = readerAux.GetValue<int>("area");

								posse.UsoAtualSolo.Add(uso);
							}

							readerAux.Close();
						}

						#endregion

						#region Edificações

						comando = bancoDeDados.CriarComando(@"select e.regularizacao_edifica_id, e.tipo, e.quantidade, e.tid 
						from {0}hst_crt_regularizacao_edifica e where e.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Edificacao edificacao = null;
							while (readerAux.Read())
							{
								edificacao = new Edificacao();
								edificacao.Id = readerAux.GetValue<int>("regularizacao_edifica_id");
								edificacao.Tid = readerAux.GetValue<string>("tid");
								edificacao.Tipo = readerAux.GetValue<string>("tipo");
								edificacao.Quantidade = readerAux.GetValue<int>("quantidade");

								posse.Edificacoes.Add(edificacao);
							}

							readerAux.Close();
						}

						#endregion

						regularizacao.Posses.Add(posse);
					}

					reader.Close();
				}

				#endregion
			}

			return regularizacao;
		}

		internal List<AreaGeo> ObterUsoAtualSoloPosseDominialidade(int empreendimento, string identificacao, BancoDeDados banco = null)
		{
			List<AreaGeo> listArea = new List<AreaGeo>();
			object idProjetoGeoDominialidade = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select t.id from {0}crt_projeto_geo t where t.caracterizacao = :caracterizacao and t.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				idProjetoGeoDominialidade = bancoDeDados.ExecutarScalar(comando);

				if (!string.IsNullOrEmpty(Convert.ToString(idProjetoGeoDominialidade)))
				{
					#region Áreas Alteradas de Posse

					comando = bancoDeDados.CriarComando(@"
					select * from (
						select 'AA' classe, 'Área Total Posse' descricao, '' subtipo, trunc(ap.area_m2, 4) aream2
							from {0}geo_apmp ap where ap.tipo = 'P' and ap.projeto = :projetoGeo and ap.nome = :identificacao
						union all
						select 'AA' classe, 'Em Recuperação' descricao, '' subtipo, trunc(nvl(sum(aa.area_m2), 0), 4) aream2
							from {0}geo_aa aa, {0}geo_apmp ap where aa.cod_apmp = ap.id and ap.tipo = 'P' and aa.tipo = 'REC' and aa.projeto = :projetoGeo and ap.nome = :identificacao
						union all
						select * from (
							select classe, descricao, subtipo, sum(aream2) aream2 from (
								select 'AA' classe, 'Em Uso' descricao, vegetacao subtipo, trunc(nvl(sum(aa.area_m2), 0), 4) aream2
									from {0}geo_aa aa, {0}geo_apmp ap where aa.cod_apmp = ap.id and ap.tipo = 'P' and aa.tipo = 'USO' and aa.projeto = :projetoGeo 
										and ap.nome = :identificacao group by vegetacao
								union all
								select 'AA' classe, 'Em Uso' descricao, null subtipo, 0 aream2 from dual
							)
							group by classe, descricao, subtipo order by subtipo
						)
						union all
						select 'AA' classe, 'Não caracterizada' descricao, '' subtipo, trunc(nvl(sum(aa.area_m2), 0), 4) aream2
							from {0}geo_aa aa, {0}geo_apmp ap where aa.cod_apmp = ap.id and ap.tipo = 'P' and aa.tipo = 'D' and aa.projeto = :projetoGeo and ap.nome = :identificacao
					) tab where tab.aream2 > 0", EsquemaBancoGeo);

					comando.AdicionarParametroEntrada("projetoGeo", idProjetoGeoDominialidade, DbType.Int32);
					comando.AdicionarParametroEntrada("identificacao", identificacao, DbType.String);

					listArea = bancoDeDados.ObterEntityList<AreaGeo>(comando);

					#endregion
				}
			}

			return listArea;
		}

		internal List<Posse> ObterPosses(int empreendimento, int zona, BancoDeDados banco = null)
		{
			RegularizacaoFundiaria regularizacao = new RegularizacaoFundiaria();
			Posse posse;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select p.id, p.regularizacao, p.dominio, p.zona, p.identificacao, p.area_requerida, p.area_croqui, p.regularizacao_tipo,
					p.relacao_trabalho, p.centro_comercial_km, p.br_km, p.es_km, p.benfeitorias, p.observacoes, p.tid, p.possui_dominio_avulso, p.perimetro, p.comprovacao, lc.texto as comprovacaotexto, p.area_documento,
					p.data_ultima_atualizacao, p.registro, p.numero_ccri, p.area_ccri, p.confrontante_norte, p.confrontante_sul, p.confrontante_leste, p.confrontante_oeste, p.registro, p.comprovacao
					from {0}crt_regularizacao_dominio p
					inner join crt_regularizacao r on r.id = p.regularizacao
					inner join lov_crt_domin_comprovacao lc on lc.id  = p.comprovacao
					where r.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						posse = new Posse();

						posse.Id = Convert.ToInt32(reader["id"]);
						posse.Dominio = reader.GetValue<int>("dominio");
						posse.AreaCCIR = reader.GetValue<decimal>("area_ccri");
						posse.NumeroCCIR = reader.GetValue<long?>("numero_ccri");
						posse.AreaRequerida = reader.GetValue<decimal>("area_requerida");
						posse.ComprovacaoTexto = reader["comprovacaotexto"].ToString();
						posse.ComprovacaoId = Convert.ToInt32(reader["comprovacao"]);
						posse.AreaPosseDocumento = reader.GetValue<decimal>("area_documento");
						posse.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						posse.Identificacao = reader["identificacao"].ToString();
						posse.DescricaoComprovacao = reader["registro"].ToString();
						posse.RegularizacaoTipo = Convert.ToInt32(reader["regularizacao_tipo"]);
						posse.RelacaoTrabalho = Convert.ToInt32(reader["relacao_trabalho"]);
						posse.Benfeitorias = reader["benfeitorias"].ToString();
						posse.Observacoes = reader["observacoes"].ToString();
						posse.Tid = reader["tid"].ToString();
						posse.PossuiDominioAvulso = reader.GetValue<int?>("possui_dominio_avulso");
						posse.Perimetro = Convert.ToInt32(reader["perimetro"]);
						posse.Distancia.BrAPosse = reader.GetValue<decimal?>("br_km");
						posse.Distancia.EsAPosse = reader.GetValue<decimal?>("es_km");
						posse.DataUltimaAtualizacaoCCIR.DataTexto = reader.GetValue<string>("data_ultima_atualizacao");
						posse.ConfrontacoesNorte = reader["confrontante_norte"].ToString();
						posse.ConfrontacoesSul = reader["confrontante_sul"].ToString();
						posse.ConfrontacoesLeste = reader["confrontante_leste"].ToString();
						posse.ConfrontacoesOeste = reader["confrontante_oeste"].ToString();
						posse.Zona = zona;

						regularizacao.Posses.Add(posse);
					}
					reader.Close();

				}
			}

			if (regularizacao.Posses?.Count == 0)
				return ConsultaPosseObterPosseGeoAATIV(empreendimento, zona, banco);

			return regularizacao.Posses;
		}

		private List<Posse> ConsultaPosseObterPosseGeoAATIV(int empreendimento, int zona, BancoDeDados banco = null)
		{
			RegularizacaoFundiaria regularizacao = new RegularizacaoFundiaria();
			Posse posse;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				comando = bancoDeDados.CriarComando(@"SELECT a.id, a.area_m2, a.codigo, sdo_geom.sdo_length(a.geometry, 0.0001) perimetro FROM IDAFGEO.geo_aativ a INNER JOIN crt_projeto_geo b ON a.projeto = b.id WHERE b.CARACTERIZACAO = 2 AND b.EMPREENDIMENTO = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{

					while (reader.Read())
					{
						posse = new Posse();

						posse.Id = Convert.ToInt32(reader["id"]);
						posse.Identificacao = reader["codigo"].ToString();
						posse.AreaCroqui = reader.GetValue<decimal>("area_m2");
						posse.Perimetro = reader.GetValue<decimal>("perimetro");
						posse.Zona = zona;

						regularizacao.Posses.Add(posse);

					}
					reader.Close();
				}

			}

			return regularizacao.Posses;
		}

		#endregion

		#region Validações

		internal bool EmpreendimentoZonaAlterada(int empreendimentoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from dual 
				where (select distinct rd.zona from {0}crt_regularizacao_dominio rd where rd.regularizacao = 
					(select s.id from {0}crt_regularizacao s where s.empreendimento = :empreendimento)) <> 
				(select e.zona from {0}tab_empreendimento_endereco e where e.empreendimento = :empreendimento and e.correspondencia = 0)", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar<int>(comando));
			}
		}

		internal bool EmpreendimentoZonaAlterada(int empreendimentoId, int zona)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from dual 
				where (:zona) <> 
				(select e.zona from {0}tab_empreendimento_endereco e where e.empreendimento = :empreendimento and e.correspondencia = 0)", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("zona", zona, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar<int>(comando));
			}
		}

		#endregion
	}
}