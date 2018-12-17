using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Data
{
	class DominialidadeDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		internal Historico Historico { get { return _historico; } }

		private String EsquemaBanco { get; set; }

		#endregion

		public DominialidadeDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(Dominialidade caracterizacao, BancoDeDados banco)
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

		internal int? Criar(Dominialidade caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade c (id, empreendimento, croqui_area, documento_area, 
				ccri_area, arl_croqui, arl_documento, app_area, possui_area_exced_matri, confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, tid) 
				values ({0}seq_crt_dominialidade.nextval, :empreendimento, :croqui_area, :documento_area, :ccri_area, :arl_croqui, 
				:arl_documento, :app_area, :possui_area_exced_matri, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("croqui_area", caracterizacao.AreaCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("documento_area", caracterizacao.AreaDocumento, DbType.Decimal);
				comando.AdicionarParametroEntrada("ccri_area", caracterizacao.AreaCCRI, DbType.Decimal);
				comando.AdicionarParametroEntrada("arl_croqui", caracterizacao.ARLCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("arl_documento", caracterizacao.ARLDocumento, DbType.Decimal);
				comando.AdicionarParametroEntrada("app_area", caracterizacao.APPCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("possui_area_exced_matri", caracterizacao.PossuiAreaExcedenteMatricula, DbType.Int32);
				comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 250, caracterizacao.ConfrontacaoNorte);
				comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 250, caracterizacao.ConfrontacaoSul);
				comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 250, caracterizacao.ConfrontacaoLeste);
				comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 250, caracterizacao.ConfrontacaoOeste);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (DominialidadeArea item in caracterizacao.Areas)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_areas(id, dominialidade, tipo, valor, tid)
						values ({0}seq_crt_dominialidade_areas.nextval, :dominialidade, :tipo, :valor, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", item.Valor, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Domínios

				if (caracterizacao.Dominios != null && caracterizacao.Dominios.Count > 0)
				{
					#region Insert Dominios

					foreach (Dominio item in caracterizacao.Dominios)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_dominio d (id, dominialidade, identificacao, tipo, matricula, folha, livro, 
						cartorio, area_croqui, area_documento, app_croqui, comprovacao, registro, numero_ccri, area_ccri, data_ultima_atualizacao, arl_documento, 
						confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, tid) values({0}seq_crt_dominialidade_dominio.nextval, :dominialidade, 
						:identificacao, :tipo, :matricula, :folha, :livro, :cartorio, :area_croqui, :area_documento, :app_croqui, :comprovacao, :registro, :numero_ccri, :area_ccri, 
						:data_ultima_atualizacao, :arl_documento, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning d.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("tipo", (int)item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("matricula", DbType.String, 24, item.Matricula);
						comando.AdicionarParametroEntrada("folha", DbType.String, 24, item.Folha);
						comando.AdicionarParametroEntrada("livro", DbType.String, 24, item.Livro);
						comando.AdicionarParametroEntrada("cartorio", DbType.String, 80, item.Cartorio);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_documento", item.AreaDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("app_croqui", item.APPCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("comprovacao", item.ComprovacaoId > 0 ? item.ComprovacaoId : (object)DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("registro", DbType.String, 80, item.DescricaoComprovacao); //campo alterado
						comando.AdicionarParametroEntrada("numero_ccri", item.NumeroCCIR, DbType.Int64);
						comando.AdicionarParametroEntrada("area_ccri", item.AreaCCIR, DbType.Decimal);
						comando.AdicionarParametroEntrada("data_ultima_atualizacao", item.DataUltimaAtualizacao.Data, DbType.DateTime);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroEntrada("arl_documento", item.ARLDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 250, item.ConfrontacaoNorte);
						comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 250, item.ConfrontacaoSul);
						comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 250, item.ConfrontacaoLeste);
						comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 250, item.ConfrontacaoOeste);
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}

					#endregion

					foreach (Dominio item in caracterizacao.Dominios)
					{
						#region Reservas Legais

						if (item.ReservasLegais != null && item.ReservasLegais.Count > 0)
						{
							foreach (ReservaLegal itemAux in item.ReservasLegais)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_reserva r
								(id, tid, dominio, situacao, localizacao, identificacao, situacao_vegetal, arl_croqui, numero_termo, cartorio, matricula, compensada, numero_cartorio, nome_cartorio, 
								numero_livro, numero_folha, matricula_numero, averbacao_numero, arl_recebida, emp_compensacao, cedente_possui_emp, arl_cedida, arl_cedente, cedente_receptor) 
								values ({0}seq_crt_dominialidade_reserva.nextval, :tid, :dominio, :situacao, :localizacao, :identificacao, :situacao_vegetal, :arl_croqui, :numero_termo, :cartorio, :matricula, 
								:compensada, :numero_cartorio, :nome_cartorio, :numero_livro, :numero_folha, :matricula_numero, :averbacao_numero, :arl_recebida, :emp_compensacao, :cedente_possui_emp, 
								:arl_cedida, :identificacao_arl_cedente, :cedente_receptor) returning r.id into :id", EsquemaBanco);

								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("situacao", itemAux.SituacaoId, DbType.Int32);
								comando.AdicionarParametroEntrada("localizacao", itemAux.LocalizacaoId > 0 ? itemAux.LocalizacaoId : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, itemAux.Identificacao);
								comando.AdicionarParametroEntrada("situacao_vegetal", itemAux.SituacaoVegetalId <= 0 ? (object)DBNull.Value : itemAux.SituacaoVegetalId, DbType.Int32);
								comando.AdicionarParametroEntrada("arl_croqui", itemAux.ARLCroqui, DbType.Decimal);
								comando.AdicionarParametroEntrada("numero_termo", DbType.String, 20, itemAux.NumeroTermo);
								comando.AdicionarParametroEntrada("cartorio", itemAux.TipoCartorioId > 0 ? itemAux.TipoCartorioId : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("matricula", itemAux.MatriculaId > 0 ? itemAux.MatriculaId : (caracterizacao.Dominios.SingleOrDefault(x => x.Identificacao == itemAux.MatriculaIdentificacao) ?? new Dominio()).Id, DbType.Int32);
								comando.AdicionarParametroEntrada("compensada", itemAux.Compensada ? 1 : 0, DbType.Int32);
								comando.AdicionarParametroEntrada("numero_cartorio", DbType.String, 20, itemAux.NumeroCartorio);
								comando.AdicionarParametroEntrada("nome_cartorio", DbType.String, 80, itemAux.NomeCartorio);
								comando.AdicionarParametroEntrada("numero_livro", DbType.String, 7, itemAux.NumeroLivro);
								comando.AdicionarParametroEntrada("numero_folha", DbType.String, 7, itemAux.NumeroFolha);
								comando.AdicionarParametroEntrada("matricula_numero", DbType.String, 50, itemAux.MatriculaNumero);
								comando.AdicionarParametroEntrada("averbacao_numero", DbType.String, 50, itemAux.AverbacaoNumero);
								comando.AdicionarParametroEntrada("arl_recebida", itemAux.ARLRecebida, DbType.Decimal);
								comando.AdicionarParametroEntrada("emp_compensacao", itemAux.EmpreendimentoCompensacao.Id > 0 ? itemAux.EmpreendimentoCompensacao.Id : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("cedente_possui_emp", itemAux.CedentePossuiEmpreendimento < 0 ? (object)DBNull.Value : itemAux.CedentePossuiEmpreendimento, DbType.Int32);
								comando.AdicionarParametroEntrada("arl_cedida", itemAux.ARLCedida, DbType.Decimal);
								comando.AdicionarParametroEntrada("identificacao_arl_cedente", DbType.Int32, 100, itemAux.IdentificacaoARLCedente);
								comando.AdicionarParametroEntrada("cedente_receptor", DbType.Int32, 1, itemAux.CompensacaoTipo == eCompensacaoTipo.Nulo ? (object)DBNull.Value : (int)itemAux.CompensacaoTipo);

								comando.AdicionarParametroSaida("id", DbType.Int32);


								bancoDeDados.ExecutarNonQuery(comando);

								itemAux.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

								if (itemAux.Coordenada != null && itemAux.Coordenada.EastingUtm.GetValueOrDefault() > 0)
								{
									comando = bancoDeDados.CriarComando(@"
									insert into crt_dominia_reserva_coord (id, tid, reserva, coordenada_tipo, datum, easting_utm, northing_utm)
									values (seq_crt_dominia_reserva_coord.nextval, :tid, :reserva, :coordenada_tipo, :datum, :easting_utm, :northing_utm)");

									comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
									comando.AdicionarParametroEntrada("reserva", itemAux.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("coordenada_tipo", itemAux.Coordenada.Tipo.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("datum", itemAux.Coordenada.Datum.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("easting_utm", itemAux.Coordenada.EastingUtm, DbType.Double);
									comando.AdicionarParametroEntrada("northing_utm", itemAux.Coordenada.NorthingUtm, DbType.Double);

									bancoDeDados.ExecutarNonQuery(comando);
								}
							}
						}

						#endregion
					}
				}

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.criar, bancoDeDados, null);

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(Dominialidade caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade c set c.croqui_area = :croqui_area, c.documento_area = :documento_area, 
				c.ccri_area =:ccri_area, c.arl_croqui = :arl_croqui, c.arl_documento = :arl_documento, c.app_area =:app_area, c.possui_area_exced_matri = :possui_area_exced_matri, 
				c.confrontante_norte = :confrontante_norte, c.confrontante_sul = :confrontante_sul, c.confrontante_leste = :confrontante_leste, 
				c.confrontante_oeste = :confrontante_oeste, c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("croqui_area", caracterizacao.AreaCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("documento_area", caracterizacao.AreaDocumento, DbType.Decimal);
				comando.AdicionarParametroEntrada("ccri_area", caracterizacao.AreaCCRI, DbType.Decimal);
				comando.AdicionarParametroEntrada("arl_croqui", caracterizacao.ARLCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("arl_documento", caracterizacao.ARLDocumento, DbType.Decimal);
				comando.AdicionarParametroEntrada("app_area", caracterizacao.APPCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("possui_area_exced_matri", caracterizacao.PossuiAreaExcedenteMatricula, DbType.Int32);
				comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 250, caracterizacao.ConfrontacaoNorte);
				comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 250, caracterizacao.ConfrontacaoSul);
				comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 250, caracterizacao.ConfrontacaoLeste);
				comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 250, caracterizacao.ConfrontacaoOeste);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Coordenada reservas legais
				comando = bancoDeDados.CriarComando("delete from {0}crt_dominia_reserva_coord c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(@"where c.reserva in (select r.id from crt_dominialidade_reserva r 
				where r.dominio in (select d.id from crt_dominialidade_dominio d where d.dominialidade = :dominialidade)) {0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Dominios.SelectMany(x => x.ReservasLegais).Where(x => x.Coordenada != null).Select(x => x.Coordenada.Id).ToList()));
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Reservas Legais
				comando = bancoDeDados.CriarComando("delete from {0}crt_dominialidade_reserva r ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where r.dominio in (select d.id from crt_dominialidade_dominio d where d.dominialidade = :dominialidade){0}",
				comando.AdicionarNotIn("and", "r.id", DbType.Int32, caracterizacao.Dominios.SelectMany(x => x.ReservasLegais).Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Areas
				comando = bancoDeDados.CriarComando("delete from {0}crt_dominialidade_areas c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.dominialidade = :dominialidade{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Areas.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (DominialidadeArea item in caracterizacao.Areas)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade_areas c set c.tipo = :tipo, c.valor = :valor, 
							c.tid = :tid where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_areas(id, dominialidade, tipo, valor, tid)
							values ({0}seq_crt_dominialidade_areas.nextval, :dominialidade, :tipo, :valor, :tid)", EsquemaBanco);

							comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", item.Valor, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Domínios

				if (caracterizacao.Dominios != null && caracterizacao.Dominios.Count > 0)
				{
					#region Insert Dominios

					foreach (Dominio item in caracterizacao.Dominios)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade_dominio d set d.identificacao = :identificacao, d.tipo = :tipo, d.matricula = :matricula, 
							d.folha = :folha, d.livro = :livro, d.cartorio = :cartorio, d.area_croqui = :area_croqui, d.area_documento = :area_documento, d.app_croqui = :app_croqui, 
							d.comprovacao = :comprovacao, d.registro = :registro, d.numero_ccri = :numero_ccri, d.area_ccri = :area_ccri, d.arl_documento = :arl_documento, 
							d.data_ultima_atualizacao = :data_ultima_atualizacao, d.confrontante_norte = :confrontante_norte, d.confrontante_sul = :confrontante_sul, 
							d.confrontante_leste = :confrontante_leste, d.confrontante_oeste = :confrontante_oeste, d.tid = :tid where d.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_dominio d (id, dominialidade, identificacao, tipo, matricula, folha, livro, 
							cartorio, area_croqui, area_documento, app_croqui, comprovacao, registro, numero_ccri, area_ccri, data_ultima_atualizacao, arl_documento, 
							confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, tid) values({0}seq_crt_dominialidade_dominio.nextval, :dominialidade, 
							:identificacao, :tipo, :matricula, :folha, :livro, :cartorio, :area_croqui, :area_documento, :app_croqui, :comprovacao, :registro, :numero_ccri, :area_ccri, 
							:data_ultima_atualizacao, :arl_documento, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning d.id into :id",
							EsquemaBanco);

							comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("tipo", (int)item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("matricula", DbType.String, 24, item.Matricula);
						comando.AdicionarParametroEntrada("folha", DbType.String, 24, item.Folha);
						comando.AdicionarParametroEntrada("livro", DbType.String, 24, item.Livro);
						comando.AdicionarParametroEntrada("cartorio", DbType.String, 80, item.Cartorio);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_documento", item.AreaDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("app_croqui", item.APPCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("comprovacao", item.ComprovacaoId > 0 ? item.ComprovacaoId : (object)DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("registro", DbType.String, 80, item.DescricaoComprovacao); //campo alterado
						comando.AdicionarParametroEntrada("numero_ccri", item.NumeroCCIR, DbType.Int64);
						comando.AdicionarParametroEntrada("area_ccri", item.AreaCCIR, DbType.Decimal);
						comando.AdicionarParametroEntrada("data_ultima_atualizacao", item.DataUltimaAtualizacao.Data, DbType.DateTime);
						comando.AdicionarParametroEntrada("arl_documento", item.ARLDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 250, item.ConfrontacaoNorte);
						comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 250, item.ConfrontacaoSul);
						comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 250, item.ConfrontacaoLeste);
						comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 250, item.ConfrontacaoOeste);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id.GetValueOrDefault() <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}

					#endregion

					foreach (Dominio item in caracterizacao.Dominios)
					{
						#region Reservas Legais

						if (item.ReservasLegais != null && item.ReservasLegais.Count > 0)
						{
							foreach (ReservaLegal itemAux in item.ReservasLegais)
							{
								if (itemAux.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"
									update {0}crt_dominialidade_reserva r set r.situacao = :situacao, r.localizacao = :localizacao, r.identificacao = :identificacao, 
									r.situacao_vegetal= :situacao_vegetal, r.arl_croqui = :arl_croqui, r.numero_termo = :numero_termo, r.cartorio = :cartorio, r.matricula = :matricula, 
									r.compensada = :compensada, r.numero_cartorio = :numero_cartorio, r.nome_cartorio = :nome_cartorio, r.numero_livro = :numero_livro, r.numero_folha = :numero_folha, 
									r.matricula_numero = :matricula_numero, r.averbacao_numero = :averbacao_numero, r.arl_recebida = :arl_recebida, r.emp_compensacao = :emp_compensacao, 
									r.cedente_possui_emp = :cedente_possui_emp, r.arl_cedida = :arl_cedida, r.arl_cedente = :identificacao_arl_cedente, r.tid = :tid, r.cedente_receptor = :cedente_receptor where r.id = :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_reserva r
									(id, tid, dominio, situacao, localizacao, identificacao, situacao_vegetal, arl_croqui, numero_termo, cartorio, matricula, compensada, numero_cartorio, nome_cartorio, 
									numero_livro, numero_folha, matricula_numero, averbacao_numero, arl_recebida, emp_compensacao, cedente_possui_emp, arl_cedida, arl_cedente, cedente_receptor) 
									values ({0}seq_crt_dominialidade_reserva.nextval, :tid, :dominio, :situacao, :localizacao, :identificacao, :situacao_vegetal, :arl_croqui, :numero_termo, :cartorio, 
									:matricula, :compensada, :numero_cartorio, :nome_cartorio, :numero_livro, :numero_folha, :matricula_numero, :averbacao_numero, :arl_recebida, :emp_compensacao, 
									:cedente_possui_emp, :arl_cedida, :identificacao_arl_cedente, :cedente_receptor) returning r.id into :id", EsquemaBanco);

									comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
									comando.AdicionarParametroSaida("id", DbType.Int32);
								}

								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								comando.AdicionarParametroEntrada("situacao", itemAux.SituacaoId, DbType.Int32);
								comando.AdicionarParametroEntrada("localizacao", itemAux.LocalizacaoId > 0 ? itemAux.LocalizacaoId : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, itemAux.Identificacao);
								comando.AdicionarParametroEntrada("situacao_vegetal", itemAux.SituacaoVegetalId <= 0 ? (object)DBNull.Value : itemAux.SituacaoVegetalId, DbType.Int32);
								comando.AdicionarParametroEntrada("arl_croqui", itemAux.ARLCroqui, DbType.Decimal);
								comando.AdicionarParametroEntrada("numero_termo", DbType.String, 20, itemAux.NumeroTermo);
								comando.AdicionarParametroEntrada("cartorio", itemAux.TipoCartorioId > 0 ? itemAux.TipoCartorioId : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("matricula", itemAux.MatriculaId > 0 ? itemAux.MatriculaId : (caracterizacao.Dominios.SingleOrDefault(x => x.Identificacao == itemAux.MatriculaIdentificacao) ?? new Dominio()).Id, DbType.Int32);
								comando.AdicionarParametroEntrada("compensada", itemAux.Compensada ? 1 : 0, DbType.Int32);
								comando.AdicionarParametroEntrada("numero_cartorio", DbType.String, 20, itemAux.NumeroCartorio);
								comando.AdicionarParametroEntrada("nome_cartorio", DbType.String, 80, itemAux.NomeCartorio);
								comando.AdicionarParametroEntrada("numero_livro", DbType.String, 7, itemAux.NumeroLivro);
								comando.AdicionarParametroEntrada("numero_folha", DbType.String, 7, itemAux.NumeroFolha);
								comando.AdicionarParametroEntrada("matricula_numero", DbType.String, 50, itemAux.MatriculaNumero);
								comando.AdicionarParametroEntrada("averbacao_numero", DbType.String, 50, itemAux.AverbacaoNumero);
								comando.AdicionarParametroEntrada("arl_recebida", itemAux.ARLRecebida, DbType.Decimal);
								comando.AdicionarParametroEntrada("emp_compensacao", itemAux.EmpreendimentoCompensacao.Id > 0 ? itemAux.EmpreendimentoCompensacao.Id : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("cedente_possui_emp", itemAux.CedentePossuiEmpreendimento < 0 ? (object)DBNull.Value : itemAux.CedentePossuiEmpreendimento, DbType.Int32);
								comando.AdicionarParametroEntrada("arl_cedida", itemAux.ARLCedida, DbType.Decimal);
								comando.AdicionarParametroEntrada("identificacao_arl_cedente", DbType.Int32, 100, itemAux.IdentificacaoARLCedente);
								comando.AdicionarParametroEntrada("cedente_receptor", DbType.Int32, 1, itemAux.CompensacaoTipo == eCompensacaoTipo.Nulo ? (object)DBNull.Value : (int)itemAux.CompensacaoTipo);

								bancoDeDados.ExecutarNonQuery(comando);

								if (itemAux.Id < 1)
								{
									itemAux.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
								}

								if (itemAux.Coordenada != null && itemAux.Coordenada.EastingUtm.GetValueOrDefault() > 0)
								{
									if (itemAux.Coordenada.Id > 0)
									{
										comando = bancoDeDados.CriarComando(@"
										update crt_dominia_reserva_coord set tid = :tid, coordenada_tipo = :coordenada_tipo, 
										datum = :datum, easting_utm = :easting_utm, northing_utm = :northing_utm where id = :id");

										comando.AdicionarParametroEntrada("id", itemAux.Coordenada.Id, DbType.Int32);
									}
									else
									{
										comando = bancoDeDados.CriarComando(@"
										insert into crt_dominia_reserva_coord (id, tid, reserva, coordenada_tipo, datum, easting_utm, northing_utm)
										values (seq_crt_dominia_reserva_coord.nextval, :tid, :reserva, :coordenada_tipo, :datum, :easting_utm, :northing_utm)");

										comando.AdicionarParametroEntrada("reserva", itemAux.Id, DbType.Int32);
									}

									comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
									comando.AdicionarParametroEntrada("coordenada_tipo", itemAux.Coordenada.Tipo.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("datum", itemAux.Coordenada.Datum.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("easting_utm", itemAux.Coordenada.EastingUtm, DbType.Double);
									comando.AdicionarParametroEntrada("northing_utm", itemAux.Coordenada.NorthingUtm, DbType.Double);

									bancoDeDados.ExecutarNonQuery(comando);
								}
							}
						}

						#endregion
					}
				}

				#endregion

				#region limpar Dominios

				//Domínios [Deve ser feito depois de atualizar as Reservas Legais] por causa das matriculas atualizadas
				comando = bancoDeDados.CriarComando("delete from {0}crt_dominialidade_dominio c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.dominialidade = :dominialidade{0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Dominios.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.atualizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_dominialidade c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					"delete from {0}crt_dominia_reserva_coord c where c.reserva in (select r.id from {0}crt_dominialidade_reserva r where r.dominio in (select d.id from {0}crt_dominialidade_dominio d where d.dominialidade = :caracterizacao));" +
					"delete from {0}crt_dominialidade_reserva r where r.dominio in (select d.id from {0}crt_dominialidade_dominio d where d.dominialidade = :caracterizacao);" +
					"delete from {0}crt_dominialidade_dominio b where b.dominialidade = :caracterizacao;" +
					"delete from {0}crt_dominialidade_areas b where b.dominialidade = :caracterizacao;" +
					"delete from {0}crt_dominialidade e where e.id = :caracterizacao;" +
				"end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal List<ReservaLegal> ObterReservasLegais(int dominioId)
		{
			ReservaLegal item = null;
			List<ReservaLegal> colecao = null;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select r.id, r.situacao, r.localizacao, r.situacao_vegetal from {0}crt_dominialidade_reserva r where r.dominio = :dominio", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominio", dominioId, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					colecao = new List<ReservaLegal>();
					while (reader.Read())
					{
						item = new ReservaLegal();
						item.Id = Convert.ToInt32(reader["id"]);
						item.SituacaoId = Convert.ToInt32(reader["situacao"]);
						item.LocalizacaoId = reader.GetValue<int>("localizacao");
						item.SituacaoVegetalId = reader.GetValue<int?>("situacao_vegetal");

						colecao.Add(item);
					}

					reader.Close();
				}
			}

			return colecao;
		}

		internal Dominialidade ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_dominialidade s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal Dominialidade Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.empreendimento, ee.zona empreendimento_localizacao, d.possui_area_exced_matri, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, d.tid 
				from {0}crt_dominialidade d, {0}tab_empreendimento_endereco ee 
				where ee.correspondencia = 0 and d.empreendimento = ee.empreendimento and d.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						caracterizacao.EmpreendimentoLocalizacao = reader.GetValue<int>("empreendimento_localizacao");
						caracterizacao.PossuiAreaExcedenteMatricula = reader.GetValue<int?>("possui_area_exced_matri");
						caracterizacao.ConfrontacaoNorte = reader.GetValue<string>("confrontante_norte");
						caracterizacao.ConfrontacaoSul = reader.GetValue<string>("confrontante_sul");
						caracterizacao.ConfrontacaoLeste = reader.GetValue<string>("confrontante_leste");
						caracterizacao.ConfrontacaoOeste = reader.GetValue<string>("confrontante_oeste");
						caracterizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id, a.tipo, la.texto tipo_texto, a.valor, a.tid from {0}crt_dominialidade_areas a, {0}lov_crt_dominialidade_area la 
				where a.tipo = la.id and a.dominialidade = :dominialidade", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominialidade", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					DominialidadeArea item;
					while (reader.Read())
					{
						item = new DominialidadeArea();
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.Tipo = reader.GetValue<int>("tipo");
						item.TipoTexto = reader.GetValue<string>("tipo_texto");
						item.Valor = reader.GetValue<decimal>("valor");

						caracterizacao.Areas.Add(item);
					}

					reader.Close();
				}

				#endregion

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select d.id, d.identificacao, d.tipo, ldt.texto tipo_texto, d.matricula, d.folha, d.livro, d.cartorio, d.area_croqui, 
				d.area_documento, d.app_croqui, d.comprovacao, ldc.texto comprovacao_texto, d.registro, d.numero_ccri, d.area_ccri, d.data_ultima_atualizacao, d.tid, d.arl_documento, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, 
				(select sdo_geom.sdo_length(a.geometry, 0.0001) from {1}geo_apmp a, crt_projeto_geo p where p.id = a.projeto and upper(a.tipo) in ('P', 'M') 
				and p.empreendimento = (select dd.empreendimento from crt_dominialidade dd where dd.id = d.dominialidade) and p.caracterizacao = 1 and a.nome = d.identificacao) perimetro
				from {0}crt_dominialidade_dominio d, {0}lov_crt_domin_dominio_tipo ldt, 
				{0}lov_crt_domin_comprovacao ldc where d.tipo = ldt.id and d.comprovacao = ldc.id(+) and d.dominialidade = :id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dominio dominio = null;

					while (reader.Read())
					{
						dominio = new Dominio();
						dominio.Id = reader.GetValue<int>("id");
						dominio.Tid = reader.GetValue<string>("tid");
						dominio.Identificacao = reader.GetValue<string>("identificacao");
						dominio.Tipo = (eDominioTipo)reader.GetValue<int>("tipo");
						dominio.TipoTexto = reader.GetValue<string>("tipo_texto");
						dominio.Matricula = reader.GetValue<string>("matricula");
						dominio.Folha = reader.GetValue<string>("folha");
						dominio.Livro = reader.GetValue<string>("livro");
						dominio.Cartorio = reader.GetValue<string>("cartorio");
						dominio.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						dominio.AreaDocumento = reader.GetValue<decimal>("area_documento");
						dominio.AreaDocumentoTexto = reader.GetValue<decimal>("area_documento").ToStringTrunc();
						dominio.EmpreendimentoLocalizacao = caracterizacao.EmpreendimentoLocalizacao;
						dominio.APPCroqui = reader.GetValue<decimal>("app_croqui");
						dominio.DescricaoComprovacao = reader.GetValue<string>("registro"); //campo alterado
						dominio.AreaCCIR = reader.GetValue<decimal>("area_ccri");
						dominio.AreaCCIRTexto = reader.GetValue<decimal>("area_ccri").ToStringTrunc();
						dominio.DataUltimaAtualizacao.DataTexto = reader.GetValue<string>("data_ultima_atualizacao");
						dominio.ARLDocumento = reader.GetValue<decimal?>("arl_documento");
						dominio.ARLDocumentoTexto = reader.GetValue<decimal?>("arl_documento").ToStringTrunc();
						dominio.ConfrontacaoNorte = reader.GetValue<string>("confrontante_norte");
						dominio.ConfrontacaoSul = reader.GetValue<string>("confrontante_sul");
						dominio.ConfrontacaoLeste = reader.GetValue<string>("confrontante_leste");
						dominio.ConfrontacaoOeste = reader.GetValue<string>("confrontante_oeste");
						dominio.ComprovacaoId = reader.GetValue<int>("comprovacao");
						dominio.ComprovacaoTexto = reader.GetValue<string>("comprovacao_texto");
						dominio.NumeroCCIR = reader.GetValue<long?>("numero_ccri");
						dominio.Perimetro = reader.GetValue<decimal>("perimetro");

						#region Reservas Legais

						comando = bancoDeDados.CriarComando(@"select r.id, r.tid, r.situacao, lrs.texto situacao_texto, r.localizacao, lrl.texto localizacao_texto, r.identificacao, r.situacao_vegetal, 
						lrsv.texto situacao_vegetal_texto, r.arl_croqui, r.arl_documento, r.numero_termo, r.cartorio, lrc.texto cartorio_texto, r.matricula, d.identificacao matricula_identificacao, 
						r.compensada, r.numero_cartorio, r.nome_cartorio, r.numero_livro, r.numero_folha, r.matricula_numero, r.averbacao_numero, r.arl_recebida, r.emp_compensacao, r.cedente_possui_emp, 
						r.arl_cedida, r.arl_cedente, c.id coordenada_id, c.coordenada_tipo, ct.texto coordenada_tipo_texto, c.datum, cd.texto datum_texto, c.easting_utm, c.northing_utm 
						from {0}crt_dominialidade_reserva r, {0}crt_dominialidade_dominio d, {0}lov_crt_domin_reserva_situacao lrs, {0}lov_crt_domin_reserva_local lrl, 
						{0}lov_crt_domin_reserva_sit_veg lrsv, {0}lov_crt_domin_reserva_cartorio lrc, {0}crt_dominia_reserva_coord c, {0}lov_coordenada_tipo ct, {0}lov_coordenada_datum cd 
						where r.matricula = d.id(+) and r.situacao = lrs.id and r.localizacao = lrl.id(+) and r.situacao_vegetal = lrsv.id(+) and r.cartorio = lrc.id(+) 
						and ct.id(+) = c.coordenada_tipo and cd.id(+) = c.datum and c.reserva(+) = r.id and r.dominio = :dominio", EsquemaBanco);

						comando.AdicionarParametroEntrada("dominio", dominio.Id, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ReservaLegal reserva = null;

							while (readerAux.Read())
							{
								reserva = new ReservaLegal();
								reserva.Id = readerAux.GetValue<int>("id");
								reserva.SituacaoId = readerAux.GetValue<int>("situacao");
								reserva.SituacaoTexto = readerAux.GetValue<string>("situacao_texto");
								reserva.Identificacao = readerAux.GetValue<string>("identificacao");
								reserva.Compensada = readerAux.GetValue<bool>("compensada");
								reserva.LocalizacaoId = readerAux.GetValue<int>("localizacao");
								reserva.LocalizacaoTexto = readerAux.GetValue<string>("localizacao_texto");
								reserva.SituacaoVegetalId = readerAux.GetValue<int>("situacao_vegetal");
								reserva.SituacaoVegetalTexto = readerAux.GetValue<string>("situacao_vegetal_texto");
								reserva.ARLCroqui = readerAux.GetValue<decimal>("arl_croqui");
								reserva.NumeroTermo = readerAux.GetValue<string>("numero_termo");
								reserva.Tid = readerAux.GetValue<string>("tid");
								reserva.TipoCartorioId = readerAux.GetValue<int>("cartorio");
								reserva.TipoCartorioTexto = readerAux.GetValue<string>("cartorio_texto");
								reserva.MatriculaId = readerAux.GetValue<int>("matricula");
								reserva.MatriculaIdentificacao = readerAux.GetValue<string>("matricula_identificacao");
								reserva.NumeroCartorio = readerAux.GetValue<string>("numero_cartorio");
								reserva.NomeCartorio = readerAux.GetValue<string>("nome_cartorio");
								reserva.NumeroFolha = readerAux.GetValue<string>("numero_folha");
								reserva.NumeroLivro = readerAux.GetValue<string>("numero_livro");

								//Compensação
								reserva.MatriculaNumero = readerAux.GetValue<string>("matricula_numero");
								reserva.AverbacaoNumero = readerAux.GetValue<string>("averbacao_numero");
								reserva.ARLRecebida = readerAux.GetValue<decimal>("arl_recebida");
								reserva.EmpreendimentoCompensacao.Id = readerAux.GetValue<int>("emp_compensacao");
								reserva.CedentePossuiEmpreendimento = readerAux.GetValue<int>("cedente_possui_emp");
								reserva.ARLCedida = readerAux.GetValue<decimal>("arl_cedida");
								reserva.IdentificacaoARLCedente = readerAux.GetValue<int>("arl_cedente");

								//Coordenada
								reserva.Coordenada.Id = readerAux.GetValue<int>("coordenada_id");
								reserva.Coordenada.Tipo.Id = readerAux.GetValue<int>("coordenada_tipo");
								reserva.Coordenada.Tipo.Texto = readerAux.GetValue<string>("coordenada_tipo_texto");
								reserva.Coordenada.Datum.Id = readerAux.GetValue<int>("datum");
								reserva.Coordenada.Datum.Texto = readerAux.GetValue<string>("datum_texto");
								reserva.Coordenada.EastingUtm = readerAux.GetValue<double?>("easting_utm");
								reserva.Coordenada.NorthingUtm = readerAux.GetValue<double?>("northing_utm");

								if (reserva.EmpreendimentoCompensacao.Id > 0)
								{
									reserva.EmpreendimentoCompensacao = _caracterizacaoDa.ObterEmpreendimentoSimplificado(reserva.EmpreendimentoCompensacao.Id, bancoDeDados);
								}

								if (reserva.IdentificacaoARLCedente > 0)
								{
									ReservaLegal reservaAux = ObterARLPorId(reserva.IdentificacaoARLCedente);

									//reserva.ARLCroqui = reservaAux.ARLCroqui;
									reserva.SituacaoVegetalId = reservaAux.SituacaoVegetalId;
									reserva.SituacaoVegetalTexto = reservaAux.SituacaoVegetalTexto;
								}

								dominio.ReservasLegais.Add(reserva);
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.Dominios.Add(dominio);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal Dominialidade ObterDadosGeo(int empreendimento, BancoDeDados banco = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"
				select a.id, a.area_m2 from {1}geo_atp a, {0}crt_projeto_geo p 
				where p.id = a.projeto and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao",
				EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.ATPCroqui = reader.GetValue<decimal>("area_m2");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select a.id, upper(a.tipo) tipo, a.nome identificacao, a.area_m2, 
				(select sum(ac.area_m2) from {1}geo_areas_calculadas ac where ac.tipo = 'APP_APMP' and ac.projeto = p.id and ac.cod_apmp = a.id) app, 
				(select te.zona from {0}tab_empreendimento_endereco te where te.empreendimento = p.empreendimento and te.correspondencia = 0) zona 
				from {1}geo_apmp a, {0}crt_projeto_geo p where p.id = a.projeto and upper(a.tipo) in('M', 'P') and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao",
				EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dominio dominio = null;

					while (reader.Read())
					{
						dominio = new Dominio();
						dominio.Id = Convert.ToInt32(reader["id"]);
						dominio.TipoGeo = Convert.ToChar(reader["tipo"]);
						dominio.Identificacao = reader["identificacao"].ToString();
						dominio.EmpreendimentoLocalizacao = reader.GetValue<int>("zona");
						dominio.AreaCroqui = reader.GetValue<decimal>("area_m2");
						dominio.APPCroqui = reader.GetValue<decimal>("app");

						dominio.TipoTexto = _caracterizacaoConfig.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeDominioTipos).
									SingleOrDefault(x => x.Id == ((int)dominio.Tipo).ToString()).Texto;

						#region Reservas Legais

						comando = bancoDeDados.CriarComando(@"select a.codigo identificacao, a.situacao situacao, a.compensada, a.area_m2, trunc(arl.column_value, 2) coordenada
						from {0}geo_arl a, table(geometria9i.pontoIdeal(a.geometry).SDO_ORDINATES) arl where a.cod_apmp = :apmp", EsquemaBancoGeo);

						comando.AdicionarParametroEntrada("apmp", dominio.Id, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ReservaLegal reserva = null;
							int idx = 0;

							while (readerAux.Read())
							{
								idx++;
								if ((idx % 2) == 0)
								{
									reserva.Coordenada.NorthingUtm = readerAux.GetValue<double>("coordenada");
									dominio.ReservasLegais.Add(reserva);
									continue;
								}

								reserva = new ReservaLegal();
								reserva.Identificacao = readerAux["identificacao"].ToString();
								reserva.SituacaoVegetalGeo = readerAux.GetValue<string>("situacao");
								reserva.Compensada = readerAux["compensada"].ToString() == "S";
								reserva.ARLCroqui = readerAux.GetValue<decimal>("area_m2");

								reserva.SituacaoVegetalTexto = _caracterizacaoConfig.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaSituacaoVegetacao).
									SingleOrDefault(x => x.Id == reserva.SituacaoVegetalId.GetValueOrDefault(0).ToString()).Texto;

								reserva.Coordenada.EastingUtm = readerAux.GetValue<double>("coordenada");
							}

							readerAux.Close();
						}

						#endregion

						dominio.Id = null;
						caracterizacao.Dominios.Add(dominio);
					}

					reader.Close();
				}

				#endregion

				#region Areas

				comando = bancoDeDados.CriarComando(@"select a.estagio, sum(a.area_m2)area_m2
					  from {1}geo_avn a, {0}crt_projeto_geo p
					 where p.id = a.projeto
					   and p.empreendimento = :empreendimento
					   and p.caracterizacao = :caracterizacao
					group by a.estagio",
						EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				DominialidadeArea area = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						area = new DominialidadeArea();
						area.Tipo = (int)ObterEnumeDominialidadeArea(reader["estagio"].ToString());
						area.Valor = reader.GetValue<decimal>("area_m2");

						caracterizacao.Areas.Add(area);
					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"select a.tipo, sum(a.area_m2)area_m2
					from {1}geo_areas_calculadas a, {0}crt_projeto_geo p
				   where p.id = a.projeto
					 and p.empreendimento = :empreendimento
					 and p.caracterizacao = :caracterizacao
					 and a.tipo in ('APP_APMP', 'APP_AA_USO','APP_AA_REC', 'APP_AVN', 'APP_ARL')
				  group by a.tipo", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				area = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						area = new DominialidadeArea();
						area.Tipo = (int)ObterEnumeDominialidadeArea(reader["tipo"].ToString());
						area.Valor = reader.GetValue<decimal>("area_m2");

						caracterizacao.Areas.Add(area);
					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"select a.tipo, sum(a.area_m2)area_m2
						from {1}geo_aa a, {0}crt_projeto_geo p
					   where p.id = a.projeto
						 and p.empreendimento = :empreendimento
						 and p.caracterizacao = :caracterizacao
						 and a.tipo = 'USO'
						 and a.vegetacao = 'FLORESTA-PLANTADA'
					  group by a.tipo, a.vegetacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				area = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						area = new DominialidadeArea();
						area.Tipo = (int)eDominialidadeArea.AA_USO_FLORES_PLANT;
						area.Valor = reader.GetValue<decimal>("area_m2");

						caracterizacao.Areas.Add(area);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal Dominialidade ObterDadosGeoTMP(int empreendimento, BancoDeDados banco = null)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"
				select a.id, a.area_m2 from {1}tmp_atp a, {0}tmp_projeto_geo p 
				where p.id = a.projeto and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao",
				EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.ATPCroqui = reader.GetValue<decimal>("area_m2");
					}

					reader.Close();
				}

				#endregion

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select a.id, upper(a.tipo) tipo, a.nome identificacao, a.area_m2, 
				(select sum(ac.area_m2) from {1}tmp_areas_calculadas ac where ac.tipo = 'APP_APMP' and ac.projeto = p.id and ac.cod_apmp = a.id) app, 
				(select te.zona from {0}tab_empreendimento_endereco te where te.empreendimento = p.empreendimento and te.correspondencia = 0) zona 
				from {1}tmp_apmp a, {0}tmp_projeto_geo p where p.id = a.projeto and upper(a.tipo) in('M', 'P') and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao",
				EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dominio dominio = null;

					while (reader.Read())
					{
						dominio = new Dominio();
						dominio.Id = Convert.ToInt32(reader["id"]);
						dominio.TipoGeo = Convert.ToChar(reader["tipo"]);
						dominio.Identificacao = reader["identificacao"].ToString();
						dominio.EmpreendimentoLocalizacao = reader.GetValue<int>("zona");
						dominio.AreaCroqui = reader.GetValue<decimal>("area_m2");
						dominio.APPCroqui = reader.GetValue<decimal>("app");

						dominio.TipoTexto = _caracterizacaoConfig.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeDominioTipos).
									SingleOrDefault(x => x.Id == ((int)dominio.Tipo).ToString()).Texto;

						#region Reservas Legais

						comando = bancoDeDados.CriarComando(@"select a.codigo identificacao, a.situacao situacao, a.compensada, a.area_m2, trunc(arl.column_value, 2) coordenada
						from {0}tmp_arl a, table(geometria9i.pontoIdeal(a.geometry).SDO_ORDINATES) arl where a.cod_apmp = :apmp", EsquemaBancoGeo);

						comando.AdicionarParametroEntrada("apmp", dominio.Id, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ReservaLegal reserva = null;
							int idx = 0;

							while (readerAux.Read())
							{
								idx++;
								if ((idx % 2) == 0)
								{
									reserva.Coordenada.NorthingUtm = readerAux.GetValue<double>("coordenada");
									dominio.ReservasLegais.Add(reserva);
									continue;
								}

								reserva = new ReservaLegal();
								reserva.Identificacao = readerAux["identificacao"].ToString();
								reserva.SituacaoVegetalGeo = readerAux.GetValue<string>("situacao");
								reserva.Compensada = readerAux["compensada"].ToString() == "S";
								reserva.ARLCroqui = readerAux.GetValue<decimal>("area_m2");

								reserva.SituacaoVegetalTexto = _caracterizacaoConfig.Obter<List<Lista>>(ConfiguracaoCaracterizacao.KeyDominialidadeReservaSituacaoVegetacao).
									SingleOrDefault(x => x.Id == reserva.SituacaoVegetalId.GetValueOrDefault(0).ToString()).Texto;

								reserva.Coordenada.EastingUtm = readerAux.GetValue<double>("coordenada");
							}

							readerAux.Close();
						}

						#endregion

						dominio.Id = null;
						caracterizacao.Dominios.Add(dominio);
					}

					reader.Close();
				}

				#endregion

				#region Areas

				comando = bancoDeDados.CriarComando(@"select a.estagio, sum(a.area_m2)area_m2
					  from {1}tmp_avn a, {0}tmp_projeto_geo p
					 where p.id = a.projeto
					   and p.empreendimento = :empreendimento
					   and p.caracterizacao = :caracterizacao
					group by a.estagio",
						EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				DominialidadeArea area = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["estagio"] != null && !Convert.IsDBNull(reader["estagio"]))
						{
							area = new DominialidadeArea();
							area.Tipo = (int)ObterEnumeDominialidadeArea(reader["estagio"].ToString());
							area.Valor = reader.GetValue<decimal>("area_m2");

							caracterizacao.Areas.Add(area);
						}
					}

					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"select a.tipo, sum(a.area_m2)area_m2
					from {1}tmp_areas_calculadas a, {0}tmp_projeto_geo p
				   where p.id = a.projeto
					 and p.empreendimento = :empreendimento
					 and p.caracterizacao = :caracterizacao
					 and a.tipo in ('APP_APMP', 'APP_AA_USO','APP_AA_REC', 'APP_AVN', 'APP_ARL')
				  group by a.tipo", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				area = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						if (reader["tipo"] != null && !Convert.IsDBNull(reader["tipo"]))
						{
							area = new DominialidadeArea();
							area.Tipo = (int)ObterEnumeDominialidadeArea(reader["tipo"].ToString());
							area.Valor = reader.GetValue<decimal>("area_m2");

							caracterizacao.Areas.Add(area);
						}
					}

					reader.Close();

				}

				comando = bancoDeDados.CriarComando(@"select a.tipo, sum(a.area_m2)area_m2
						from {1}tmp_aa a, {0}tmp_projeto_geo p
					   where p.id = a.projeto
						 and p.empreendimento = :empreendimento
						 and p.caracterizacao = :caracterizacao
						 and a.tipo = 'USO'
						 and a.vegetacao = 'FLORESTA-PLANTADA'
					  group by a.tipo, a.vegetacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				area = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						area = new DominialidadeArea();
						area.Tipo = (int)eDominialidadeArea.AA_USO_FLORES_PLANT;
						area.Valor = reader.GetValue<decimal>("area_m2");

						caracterizacao.Areas.Add(area);
					}

					reader.Close();

				}

				#endregion
			}

			return caracterizacao;
		}

		public eDominialidadeArea ObterEnumeDominialidadeArea(string tipoArea)
		{
			switch (tipoArea)
			{
				case "A":
					return eDominialidadeArea.AVN_A;
				case "D":
					return eDominialidadeArea.AVN_D;
				case "I":
					return eDominialidadeArea.AVN_I;
				case "M":
					return eDominialidadeArea.AVN_M;
				case "APP_APMP":
					return eDominialidadeArea.APP_APMP;
				case "APP_AA_USO":
					return eDominialidadeArea.APP_AA_USO;
				case "APP_AA_REC":
					return eDominialidadeArea.APP_AA_REC;
				case "APP_AVN":
					return eDominialidadeArea.APP_AVN;
				case "APP_ARL":
					return eDominialidadeArea.APP_ARL;
			}
			return eDominialidadeArea.NULL;
			//throw new Exception("Tipo de area de AVN nao encontrada.");
		}

		internal List<Lista> ObterDominiosLista(int empreendimento, bool somenteMatriculas = false, BancoDeDados banco = null)
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select d.id, (case when d.tipo = 1 then d.identificacao|| ' - ' ||d.matricula|| ' - ' ||d.folha|| ' - ' ||d.livro else 
				d.identificacao|| ' - ' ||(select l.texto from lov_crt_domin_comprovacao l where l.id = d.comprovacao)|| ' - ' ||d.registro end) texto 
				from crt_dominialidade_dominio  d where d.dominialidade = (select c.id from crt_dominialidade c where c.empreendimento = :empreendimento)", EsquemaBanco);

				if (somenteMatriculas)
				{
					comando.DbCommand.CommandText += " and d.matricula is not null";
				}

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = reader.GetValue<string>("id"),
							Texto = reader.GetValue<string>("texto"),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal List<Lista> ObterARLCompensacaoLista(int empreendimentoReceptor, int dominio, BancoDeDados banco = null)
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select r.id, 'ARL - '||r.identificacao||' - '||round(r.arl_croqui, 2) texto from crt_dominialidade_reserva r 
				where r.dominio = :dominio and r.emp_compensacao = :empreendimentoReceptor", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimentoReceptor", empreendimentoReceptor, DbType.Int32);
				comando.AdicionarParametroEntrada("dominio", dominio, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = reader.GetValue<string>("id"),
							Texto = reader.GetValue<string>("texto"),
							IsAtivo = true
						});
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal List<Lista> ObterEmpreendimentoDominio(int dominio)
		{
			List<Lista> retorno = new List<Lista>();

			using (var bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select l.empreendimento_id id, l.denominador texto from lst_empreendimento l 
				where  l.empreendimento_id in(select r.emp_compensacao from crt_dominialidade_reserva r where r.dominio = :dominio)", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominio", dominio, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = reader.GetValue<string>("id"),
							Texto = reader.GetValue<string>("texto"),
							IsAtivo = true
						});
					}
				}
			}

			return retorno;
		}

		internal EmpreendimentoCaracterizacao ObterEmpreendimentoReceptor(int reservaLegal, BancoDeDados banco)
		{
			EmpreendimentoCaracterizacao empreendimento = new EmpreendimentoCaracterizacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"
				select e.id, e.tid, e.denominador from crt_dominialidade_reserva r, tab_empreendimento e where r.emp_compensacao = e.id and r.id = :reservaLegal", EsquemaBanco);

				comando.AdicionarParametroEntrada("reservaLegal", reservaLegal, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						empreendimento.Id = reader.GetValue<int>("id"); ;
						empreendimento.Denominador = reader.GetValue<string>("denominador");
						empreendimento.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return empreendimento;
		}

		internal ReservaLegal ObterARLPorId(int reservaLegalID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select r.id, r.tid, r.situacao, lrs.texto situacao_texto, r.localizacao, lrl.texto localizacao_texto, r.identificacao, r.situacao_vegetal, 
						lrsv.texto situacao_vegetal_texto, r.arl_croqui, r.arl_documento, r.numero_termo, r.cartorio, lrc.texto cartorio_texto, r.matricula, d.identificacao matricula_identificacao, 
						r.compensada, r.numero_cartorio, r.nome_cartorio, r.numero_livro, r.numero_folha, r.matricula_numero, r.averbacao_numero, r.arl_recebida, r.emp_compensacao, r.cedente_possui_emp, 
						r.arl_cedida, r.arl_cedente, c.id coordenada_id, c.coordenada_tipo, ct.texto coordenada_tipo_texto, c.datum, cd.texto datum_texto, c.easting_utm, c.northing_utm 
						from {0}crt_dominialidade_reserva r, {0}crt_dominialidade_dominio d, {0}lov_crt_domin_reserva_situacao lrs, {0}lov_crt_domin_reserva_local lrl, 
						{0}lov_crt_domin_reserva_sit_veg lrsv, {0}lov_crt_domin_reserva_cartorio lrc, {0}crt_dominia_reserva_coord c, {0}lov_coordenada_tipo ct, {0}lov_coordenada_datum cd 
						where r.matricula = d.id(+) and r.situacao = lrs.id and r.localizacao = lrl.id(+) and r.situacao_vegetal = lrsv.id(+) and r.cartorio = lrc.id(+) 
						and ct.id(+) = c.coordenada_tipo and cd.id(+) = c.datum and c.reserva(+) = r.id and r.id = :reserva", EsquemaBanco);

				comando.AdicionarParametroEntrada("reserva", reservaLegalID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ReservaLegal reserva = new ReservaLegal();

					if (reader.Read())
					{
						reserva = new ReservaLegal();
						reserva.Id = reader.GetValue<int>("id");
						reserva.SituacaoId = reader.GetValue<int>("situacao");
						reserva.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						reserva.Identificacao = reader.GetValue<string>("identificacao");
						reserva.Compensada = reader.GetValue<bool>("compensada");
						reserva.LocalizacaoId = reader.GetValue<int>("localizacao");
						reserva.LocalizacaoTexto = reader.GetValue<string>("localizacao_texto");
						reserva.SituacaoVegetalId = reader.GetValue<int>("situacao_vegetal");
						reserva.SituacaoVegetalTexto = reader.GetValue<string>("situacao_vegetal_texto");
						reserva.ARLCroqui = reader.GetValue<decimal>("arl_croqui");
						reserva.NumeroTermo = reader.GetValue<string>("numero_termo");
						reserva.Tid = reader.GetValue<string>("tid");
						reserva.TipoCartorioId = reader.GetValue<int>("cartorio");
						reserva.TipoCartorioTexto = reader.GetValue<string>("cartorio_texto");
						reserva.MatriculaId = reader.GetValue<int>("matricula");
						reserva.MatriculaIdentificacao = reader.GetValue<string>("matricula_identificacao");
						reserva.NumeroCartorio = reader.GetValue<string>("numero_cartorio");
						reserva.NomeCartorio = reader.GetValue<string>("nome_cartorio");
						reserva.NumeroFolha = reader.GetValue<string>("numero_folha");
						reserva.NumeroLivro = reader.GetValue<string>("numero_livro");

						//Compensação
						reserva.MatriculaNumero = reader.GetValue<string>("matricula_numero");
						reserva.AverbacaoNumero = reader.GetValue<string>("averbacao_numero");
						reserva.ARLRecebida = reader.GetValue<decimal>("arl_recebida");
						reserva.EmpreendimentoCompensacao.Id = reader.GetValue<int>("emp_compensacao");
						reserva.CedentePossuiEmpreendimento = reader.GetValue<int>("cedente_possui_emp");
						reserva.ARLCedida = reader.GetValue<decimal>("arl_cedida");
						reserva.IdentificacaoARLCedente = reader.GetValue<int>("arl_cedente");

						//Coordenada
						reserva.Coordenada.Id = reader.GetValue<int>("coordenada_id");
						reserva.Coordenada.Tipo.Id = reader.GetValue<int>("coordenada_tipo");
						reserva.Coordenada.Tipo.Texto = reader.GetValue<string>("coordenada_tipo_texto");
						reserva.Coordenada.Datum.Id = reader.GetValue<int>("datum");
						reserva.Coordenada.Datum.Texto = reader.GetValue<string>("datum_texto");
						reserva.Coordenada.EastingUtm = reader.GetValue<double?>("easting_utm");
						reserva.Coordenada.NorthingUtm = reader.GetValue<double?>("northing_utm");
					}

					return reserva;
				}
			}
		}

		internal Dominio ObterDominio(int id, BancoDeDados banco = null)
		{
			Dominio retorno = new Dominio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Domínios

				Comando comando = bancoDeDados.CriarComando(@"select d.id, d.identificacao, d.tipo, ldt.texto tipo_texto, d.matricula, d.folha, d.livro, d.cartorio, d.area_croqui, 
				d.area_documento, d.app_croqui, d.comprovacao, ldc.texto comprovacao_texto, d.registro, d.numero_ccri, d.area_ccri, d.data_ultima_atualizacao, d.tid, d.arl_documento, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, 
				(select sdo_geom.sdo_length(a.geometry, 0.0001) from {1}geo_apmp a, crt_projeto_geo p where p.id = a.projeto and upper(a.tipo) in ('P', 'M') 
				and p.empreendimento = (select dd.empreendimento from crt_dominialidade dd where dd.id = d.dominialidade) and p.caracterizacao = 1 and a.nome = d.identificacao) perimetro
				from {0}crt_dominialidade_dominio d, {0}lov_crt_domin_dominio_tipo ldt, 
				{0}lov_crt_domin_comprovacao ldc where d.tipo = ldt.id and d.comprovacao = ldc.id(+) and d.id = :id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = new Dominio();
						retorno.Id = reader.GetValue<int>("id");
						retorno.Tid = reader.GetValue<string>("tid");
						retorno.Identificacao = reader.GetValue<string>("identificacao");
						retorno.Tipo = (eDominioTipo)reader.GetValue<int>("tipo");
						retorno.TipoTexto = reader.GetValue<string>("tipo_texto");
						retorno.Matricula = reader.GetValue<string>("matricula");
						retorno.Folha = reader.GetValue<string>("folha");
						retorno.Livro = reader.GetValue<string>("livro");
						retorno.Cartorio = reader.GetValue<string>("cartorio");
						retorno.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						retorno.AreaDocumento = reader.GetValue<decimal>("area_documento");
						retorno.AreaDocumentoTexto = reader.GetValue<decimal>("area_documento").ToStringTrunc();
						retorno.APPCroqui = reader.GetValue<decimal>("app_croqui");
						retorno.DescricaoComprovacao = reader.GetValue<string>("registro"); //campo alterado
						retorno.AreaCCIR = reader.GetValue<decimal>("area_ccri");
						retorno.AreaCCIRTexto = reader.GetValue<decimal>("area_ccri").ToStringTrunc();
						retorno.DataUltimaAtualizacao.DataTexto = reader.GetValue<string>("data_ultima_atualizacao");
						retorno.ARLDocumento = reader.GetValue<decimal?>("arl_documento");
						retorno.ARLDocumentoTexto = reader.GetValue<decimal?>("arl_documento").ToStringTrunc();
						retorno.ConfrontacaoNorte = reader.GetValue<string>("confrontante_norte");
						retorno.ConfrontacaoSul = reader.GetValue<string>("confrontante_sul");
						retorno.ConfrontacaoLeste = reader.GetValue<string>("confrontante_leste");
						retorno.ConfrontacaoOeste = reader.GetValue<string>("confrontante_oeste");
						retorno.ComprovacaoId = reader.GetValue<int>("comprovacao");
						retorno.ComprovacaoTexto = reader.GetValue<string>("comprovacao_texto");
						retorno.NumeroCCIR = reader.GetValue<long?>("numero_ccri");
						retorno.Perimetro = reader.GetValue<decimal>("perimetro");
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

		#endregion

		#region Validações

		public bool EmPosse(int empreendimento)
		{
			return _caracterizacaoDa.EmPosse(empreendimento);
		}

		internal EmpreendimentoCaracterizacao VerificarRLAssociadaEmOutroEmpreendimentoCedente(int reservaLegalId, int empreendimentCedenteId, int identificacaoARLCedente)
		{
			EmpreendimentoCaracterizacao retorno = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select e.codigo, e.denominador from crt_dominialidade_reserva c, tab_empreendimento e 
                where c.emp_compensacao = e.id  and c.emp_compensacao = :emp_comp_id and c.arl_cedente = :ident_arl_cedente ", EsquemaBanco);

				comando.AdicionarParametroEntrada("emp_comp_id", empreendimentCedenteId, DbType.Int32);
				comando.AdicionarParametroEntrada("ident_arl_cedente", identificacaoARLCedente, DbType.Int32);

				if (reservaLegalId > 0)
				{
					comando.DbCommand.CommandText += "and c.id <> :id";
					comando.AdicionarParametroEntrada("id", reservaLegalId, DbType.Int32);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = new EmpreendimentoCaracterizacao();
						retorno.Codigo = reader.GetValue<int?>("codigo");
						retorno.Denominador = reader.GetValue<string>("denominador");
					}
				}
			}

			return retorno;
		}

		internal bool VerificarEmpreendimentoPossuiDominialidade(int empreendimentoId, BancoDeDados banco = null)
		{
			bool retorno = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select count(*) total from {0}crt_dominialidade d where d.empreendimento = :empreendimento", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = (reader.GetValue<int>("total") > 0);
					}
				}
			}
			return retorno;
		}

		internal List<string> VerificarExcluirDominios(Dominialidade caracterizacao)
		{
			List<string> retorno = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select (case when c.tipo = 1 then 'Matricula' else 'Posse' end)|| ' - ' ||c.identificacao 
				from {0}crt_dominialidade_dominio c where c.dominialidade = (select d.id from {0}crt_dominialidade d where d.empreendimento = :empreendimento) 
				and exists(select 1 from {0}crt_dominialidade_reserva r where r.matricula = c.id)", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.identificacao", DbType.String, caracterizacao.Dominios.Select(x => x.Identificacao).ToList());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);

				retorno = bancoDeDados.ExecutarList<string>(comando);
			}

			return retorno;
		}

		#endregion

		internal void CopiarDadosCredenciado(Dominialidade caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				#region Dominialidade

				if (caracterizacao.Id <= 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade c (id, empreendimento, croqui_area, documento_area, 
					ccri_area, arl_croqui, arl_documento, app_area, possui_area_exced_matri, confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, tid) 
					values ({0}seq_crt_dominialidade.nextval,  :empreendimento, :croqui_area, :documento_area, :ccri_area, :arl_croqui, 
					:arl_documento, :app_area, :possui_area_exced_matri, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning c.id into :id",
					EsquemaBanco);

					comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade c set c.croqui_area = :croqui_area, 
					c.documento_area = :documento_area, c.ccri_area =:ccri_area, c.arl_croqui = :arl_croqui, c.arl_documento = :arl_documento, c.app_area =:app_area, 
					c.possui_area_exced_matri = :possui_area_exced_matri, c.confrontante_norte = :confrontante_norte, c.confrontante_sul = :confrontante_sul, 
					c.confrontante_leste = :confrontante_leste, c.confrontante_oeste = :confrontante_oeste, c.tid = :tid where c.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("croqui_area", caracterizacao.AreaCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("documento_area", caracterizacao.AreaDocumento, DbType.Decimal);
				comando.AdicionarParametroEntrada("ccri_area", caracterizacao.AreaCCRI, DbType.Decimal);
				comando.AdicionarParametroEntrada("arl_croqui", caracterizacao.ARLCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("arl_documento", caracterizacao.ARLDocumento, DbType.Decimal);
				comando.AdicionarParametroEntrada("app_area", caracterizacao.APPCroqui, DbType.Decimal);
				comando.AdicionarParametroEntrada("possui_area_exced_matri", caracterizacao.PossuiAreaExcedenteMatricula, DbType.Int32);
				comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 250, caracterizacao.ConfrontacaoNorte);
				comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 250, caracterizacao.ConfrontacaoSul);
				comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 250, caracterizacao.ConfrontacaoLeste);
				comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 250, caracterizacao.ConfrontacaoOeste);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if (caracterizacao.Id <= 0)
				{
					caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				}
				caracterizacao.Tid = GerenciadorTransacao.ObterIDAtual();

				#endregion

				#region Limpar os dados do banco

				//Coordenada reservas legais
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_dominia_reserva_coord c where c.reserva in (select r.id from crt_dominialidade_reserva r 
				where r.dominio in (select d.id from crt_dominialidade_dominio d where d.dominialidade = :dominialidade))", EsquemaBanco);
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Reservas
				comando = bancoDeDados.CriarComando(@"delete from crt_dominialidade_reserva r
				where r.dominio in (select d.id from crt_dominialidade_dominio d ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where d.dominialidade = :dominialidade{0})",
				comando.AdicionarNotIn("and", "d.identificacao", DbType.String, caracterizacao.Dominios.Select(x => x.Identificacao).ToList()));
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);


				//Dominios
				comando = bancoDeDados.CriarComando(@"delete from {0}crt_dominialidade_reserva r where
				exists (select 1 from {0}crt_dominialidade_dominio d where d.id = r.dominio ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("and d.dominialidade = :dominialidade{0})",
				comando.AdicionarNotIn("and", "d.identificacao", DbType.String, caracterizacao.Dominios.Select(x => x.Identificacao).ToList()));
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				//Areas
				comando = bancoDeDados.CriarComando("delete from {0}crt_dominialidade_areas a where a.dominialidade = :dominialidade", EsquemaBanco);
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (DominialidadeArea item in caracterizacao.Areas)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_areas(id, dominialidade, tipo, valor, tid)
						values ({0}seq_crt_dominialidade_areas.nextval, :dominialidade, :tipo, :valor, :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", item.Valor, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Domínios/Reservas Legais

				if (caracterizacao.Dominios != null && caracterizacao.Dominios.Count > 0)
				{
					#region Dominios

					foreach (Dominio item in caracterizacao.Dominios)
					{
						#region Obter Dominio Por Identificacao

						comando = bancoDeDados.CriarComando(@"select d.id from crt_dominialidade_dominio d where d.identificacao = :identificacao and d.dominialidade = :dominialidade", EsquemaBanco);

						comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", item.Identificacao, DbType.String);

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							if (reader.Read())
							{
								item.Id = reader.GetValue<Int32>("id");
							}

							reader.Close();
						}

						#endregion

						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade_dominio d set d.dominialidade = :dominialidade, d.identificacao = :identificacao, d.tipo = :tipo, 
																d.matricula = :matricula, d.folha = :folha, d.livro = :livro, d.cartorio = :cartorio, d.area_croqui = :area_croqui, 
																d.area_documento = :area_documento, d.app_croqui = :app_croqui, d.comprovacao = :comprovacao, d.registro = :registro,
																d.numero_ccri = :numero_ccri, d.area_ccri = :area_ccri, d.data_ultima_atualizacao = :data_ultima_atualizacao, 
																d.arl_documento = :arl_documento, d.confrontante_norte = :confrontante_norte, d.confrontante_sul = :confrontante_sul, 
																d.confrontante_leste = :confrontante_leste, d.confrontante_oeste = :confrontante_oeste, d.tid = :tid where d.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_dominio d (id, dominialidade, identificacao, tipo, matricula, folha, livro, 
																cartorio, area_croqui, area_documento, app_croqui, comprovacao, registro, numero_ccri, area_ccri, data_ultima_atualizacao, arl_documento, 
																confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, tid) values({0}seq_crt_dominialidade_dominio.nextval, 
																:dominialidade, :identificacao, :tipo, :matricula, :folha, :livro, :cartorio, :area_croqui, :area_documento, 
																:app_croqui, :comprovacao, :registro, :numero_ccri, :area_ccri, :data_ultima_atualizacao, :arl_documento, 
																:confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning d.id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, item.Identificacao);
						comando.AdicionarParametroEntrada("tipo", (int)item.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("matricula", DbType.String, 24, item.Matricula);
						comando.AdicionarParametroEntrada("folha", DbType.String, 24, item.Folha);
						comando.AdicionarParametroEntrada("livro", DbType.String, 24, item.Livro);
						comando.AdicionarParametroEntrada("cartorio", DbType.String, 80, item.Cartorio);
						comando.AdicionarParametroEntrada("area_croqui", item.AreaCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("area_documento", item.AreaDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("app_croqui", item.APPCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("comprovacao", item.ComprovacaoId > 0 ? item.ComprovacaoId : (object)DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("registro", DbType.String, 80, item.DescricaoComprovacao); //campo alterado
						comando.AdicionarParametroEntrada("numero_ccri", item.NumeroCCIR, DbType.Int64);
						comando.AdicionarParametroEntrada("area_ccri", item.AreaCCIR, DbType.Decimal);
						comando.AdicionarParametroEntrada("data_ultima_atualizacao", item.DataUltimaAtualizacao.Data, DbType.DateTime);
						comando.AdicionarParametroEntrada("arl_documento", item.ARLDocumento, DbType.Decimal);
						comando.AdicionarParametroEntrada("confrontante_norte", DbType.String, 250, item.ConfrontacaoNorte);
						comando.AdicionarParametroEntrada("confrontante_sul", DbType.String, 250, item.ConfrontacaoSul);
						comando.AdicionarParametroEntrada("confrontante_leste", DbType.String, 250, item.ConfrontacaoLeste);
						comando.AdicionarParametroEntrada("confrontante_oeste", DbType.String, 250, item.ConfrontacaoOeste);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						if (item.Id <= 0)
						{
							item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}
					}

					#endregion

					foreach (Dominio item in caracterizacao.Dominios)
					{
						#region Reservas Legais

						if (item.ReservasLegais != null && item.ReservasLegais.Count > 0)
						{
							foreach (ReservaLegal itemAux in item.ReservasLegais)
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_reserva r
								(id, tid, dominio, situacao, localizacao, identificacao, situacao_vegetal, arl_croqui, numero_termo, cartorio, matricula, compensada, numero_cartorio, nome_cartorio, 
								numero_livro, numero_folha, matricula_numero, averbacao_numero, arl_recebida, emp_compensacao, cedente_possui_emp, arl_cedida, arl_cedente, cedente_receptor) 
								values ({0}seq_crt_dominialidade_reserva.nextval, :tid, :dominio, :situacao, :localizacao, :identificacao, :situacao_vegetal, :arl_croqui, :numero_termo, :cartorio, :matricula, 
								:compensada, :numero_cartorio, :nome_cartorio, :numero_livro, :numero_folha, :matricula_numero, :averbacao_numero, :arl_recebida, :emp_compensacao, :cedente_possui_emp, 
								:arl_cedida, :identificacao_arl_cedente, :cedente_receptor) returning r.id into :id", EsquemaBanco);

								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("situacao", itemAux.SituacaoId, DbType.Int32);
								comando.AdicionarParametroEntrada("localizacao", itemAux.LocalizacaoId > 0 ? itemAux.LocalizacaoId : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("identificacao", DbType.String, 100, itemAux.Identificacao);
								comando.AdicionarParametroEntrada("situacao_vegetal", itemAux.SituacaoVegetalId <= 0 ? (object)DBNull.Value : itemAux.SituacaoVegetalId, DbType.Int32);
								comando.AdicionarParametroEntrada("arl_croqui", itemAux.ARLCroqui, DbType.Decimal);
								comando.AdicionarParametroEntrada("numero_termo", DbType.String, 20, itemAux.NumeroTermo);
								comando.AdicionarParametroEntrada("cartorio", itemAux.TipoCartorioId > 0 ? itemAux.TipoCartorioId : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("matricula", itemAux.MatriculaId > 0 ? itemAux.MatriculaId : (caracterizacao.Dominios.SingleOrDefault(x => x.Identificacao == itemAux.MatriculaIdentificacao) ?? new Dominio()).Id, DbType.Int32);
								comando.AdicionarParametroEntrada("compensada", itemAux.Compensada ? 1 : 0, DbType.Int32);
								comando.AdicionarParametroEntrada("numero_cartorio", DbType.String, 20, itemAux.NumeroCartorio);
								comando.AdicionarParametroEntrada("nome_cartorio", DbType.String, 80, itemAux.NomeCartorio);
								comando.AdicionarParametroEntrada("numero_livro", DbType.String, 7, itemAux.NumeroLivro);
								comando.AdicionarParametroEntrada("numero_folha", DbType.String, 7, itemAux.NumeroFolha);
								comando.AdicionarParametroEntrada("matricula_numero", DbType.String, 50, itemAux.MatriculaNumero);
								comando.AdicionarParametroEntrada("averbacao_numero", DbType.String, 50, itemAux.AverbacaoNumero);
								comando.AdicionarParametroEntrada("arl_recebida", itemAux.ARLRecebida, DbType.Decimal);
								comando.AdicionarParametroEntrada("emp_compensacao", itemAux.EmpreendimentoCompensacao.Id > 0 ? itemAux.EmpreendimentoCompensacao.Id : (object)DBNull.Value, DbType.Int32);
								comando.AdicionarParametroEntrada("cedente_possui_emp", itemAux.CedentePossuiEmpreendimento < 0 ? (object)DBNull.Value : itemAux.CedentePossuiEmpreendimento, DbType.Int32);
								comando.AdicionarParametroEntrada("arl_cedida", itemAux.ARLCedida, DbType.Decimal);
								comando.AdicionarParametroEntrada("identificacao_arl_cedente", DbType.Int32, 100, itemAux.IdentificacaoARLCedente);
								comando.AdicionarParametroEntrada("cedente_receptor", DbType.Int32, 1, itemAux.CompensacaoTipo == eCompensacaoTipo.Nulo ? (object)DBNull.Value : (int)itemAux.CompensacaoTipo);

								comando.AdicionarParametroSaida("id", DbType.Int32);
								bancoDeDados.ExecutarNonQuery(comando);
								itemAux.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

								if (itemAux.Coordenada.EastingUtm.GetValueOrDefault() > 0)
								{
									comando = bancoDeDados.CriarComando(@"
									insert into crt_dominia_reserva_coord (id, tid, reserva, coordenada_tipo, datum, easting_utm, northing_utm)
									values (seq_crt_dominia_reserva_coord.nextval, :tid, :reserva, :coordenada_tipo, :datum, :easting_utm, :northing_utm)");

									comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
									comando.AdicionarParametroEntrada("reserva", itemAux.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("coordenada_tipo", itemAux.Coordenada.Tipo.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("datum", itemAux.Coordenada.Datum.Id, DbType.Int32);
									comando.AdicionarParametroEntrada("easting_utm", itemAux.Coordenada.EastingUtm, DbType.Double);
									comando.AdicionarParametroEntrada("northing_utm", itemAux.Coordenada.NorthingUtm, DbType.Double);

									bancoDeDados.ExecutarNonQuery(comando);
								}
							}
						}

						#endregion
					}
				}

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.importar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal List<Lista> ObterARLCompensacaoDominio(int dominio, BancoDeDados banco = null)
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select r.id, r.identificacao||' - '||round(r.arl_croqui, 2) texto from crt_dominialidade_reserva r 
				where r.localizacao in (1, 3) and r.dominio = :dominio", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominio", dominio, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = reader.GetValue<string>("id"),
							Texto = reader.GetValue<string>("texto"),
							IsAtivo = true
						});
					}
				}
			}

			return retorno;
		}

		internal List<PessoaLst> ObterResponsaveis(int id, BancoDeDados banco = null)
		{
			List<PessoaLst> responsaveis = new List<PessoaLst>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.id, nvl(p.nome, p.razao_social) nome_razao_social, r.tipo
				from {0}tab_empreendimento_responsavel r, {0}tab_pessoa p where r.responsavel = p.id and r.empreendimento = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst pessoa = null;

					while (reader.Read())
					{
						pessoa = new PessoaLst();
						pessoa.Id = Convert.ToInt32(reader["id"]);
						pessoa.Texto = reader["nome_razao_social"].ToString();
						pessoa.VinculoTipo = Convert.ToInt32(reader["tipo"]);
						responsaveis.Add(pessoa);
					}

					reader.Close();
				}
			}

			return responsaveis;
		}

		internal ReservaLegal ObterReservaLegal(int reservaId, BancoDeDados banco = null)
		{
			ReservaLegal reserva = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select r.id, r.situacao_vegetal, l.texto situacao_vegetal_texto , r.arl_croqui
															from crt_dominialidade_reserva r, lov_crt_domin_reserva_sit_veg l where l.id = r.situacao_vegetal and r.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", reservaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						reserva = new ReservaLegal();
						reserva.Id = reader.GetValue<int>("id");
						reserva.ARLCroqui = reader.GetValue<decimal>("arl_croqui");
						reserva.SituacaoVegetalId = reader.GetValue<int>("situacao_vegetal");
						reserva.SituacaoVegetalTexto = reader.GetValue<string>("situacao_vegetal_texto");
					}
					reader.Close();
				}
			}
			return reserva;
		}

		internal bool VerificarEmpreendimentoCompensacaoAssociado(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select count(*) from crt_dominialidade_reserva where emp_compensacao = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) <= 0;
			}
		}

		internal bool ValidarSituacaoVegetal(int reservaLegalId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select count(*) from crt_dominialidade_reserva where id = :id and situacao_vegetal in (1, 3)", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", reservaLegalId, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal Coordenada ObterCoordenada(int arlCedenteId, BancoDeDados banco = null)
		{
			Coordenada retorno = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.coordenada_tipo, lc.texto coordenada_tipo_texto, c.datum, lc.texto datum_texto, c.easting_utm, 
				c.northing_utm from crt_dominia_reserva_coord c, lov_coordenada_datum ld, lov_coordenada_tipo lc where ld.id = c.datum and lc.id = c.coordenada_tipo and 
				c.reserva = :reserva ", EsquemaBanco);
				comando.AdicionarParametroEntrada("reserva", arlCedenteId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno = new Coordenada();
						retorno.Tipo.Id = reader.GetValue<int>("coordenada_tipo");
						retorno.Tipo.Texto = reader.GetValue<string>("coordenada_tipo_texto");
						retorno.Datum.Id = reader.GetValue<int>("datum");
						retorno.Datum.Texto = reader.GetValue<string>("datum_texto");
						retorno.EastingUtm = reader.GetValue<double>("easting_utm");
						retorno.NorthingUtm = reader.GetValue<double>("northing_utm");
					}
					reader.Close();
				}


				if (retorno != null)
				{
					return retorno;
				}

				comando = bancoDeDados.CriarComando(@"select trunc(arl.column_value, 2) coordenada from {0}geo_arl a, table(geometria9i.pontoIdeal(a.geometry).SDO_ORDINATES) arl, {0}geo_apmp ga, {1}crt_dominialidade c, {1}crt_projeto_geo g where a.cod_apmp =  ga.id and ga.projeto = g.id and 
				c.empreendimento = g.empreendimento and g.caracterizacao = 1 and  c.id in (select dominialidade from {1}crt_dominialidade_dominio where id in (select dominio 
				from {1}crt_dominialidade_reserva where id = :reserva_id))", EsquemaBancoGeo, EsquemaBanco);

				comando.AdicionarParametroEntrada("reserva_id", arlCedenteId, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					int idx = 0;

					while (reader.Read())
					{
						idx++;
						if ((idx % 2) == 0)
						{
							retorno.NorthingUtm = reader.GetValue<double>("coordenada");
							continue;
						}
						retorno = new Coordenada();
						retorno.EastingUtm = reader.GetValue<double>("coordenada");
						retorno.Datum.Id = 1;
						retorno.Tipo.Id = 3;
					}

					reader.Close();
				}

			}
			return retorno;
		}
	}
}