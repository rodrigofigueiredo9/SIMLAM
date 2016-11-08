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
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Da
{
	public class DominialidadeDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		CaracterizacaoDa _caracterizacaoDa = new CaracterizacaoDa();
		CaracterizacaoInternoDa _caracterizacaoInternoDa = new CaracterizacaoInternoDa();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		GerenciadorConfiguracao<ConfiguracaoCaracterizacao> _caracterizacaoConfig = new GerenciadorConfiguracao<ConfiguracaoCaracterizacao>(new ConfiguracaoCaracterizacao());

		public String EsquemaCredenciadoBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciadoGeo); }
		}

		internal Historico Historico { get { return _historico; } }

		private String EsquemaBanco { get; set; }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

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
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade c (id, empreendimento, croqui_area, documento_area, 
				ccri_area, arl_croqui, arl_documento, app_area, possui_area_exced_matri, confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, tid) 
				values ({0}seq_crt_dominialidade.nextval, :empreendimento, :croqui_area, :documento_area, :ccri_area, :arl_croqui, 
				:arl_documento, :app_area, :possui_area_exced_matri, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning c.id into :id", EsquemaCredenciadoBanco);

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
						values ({0}seq_crt_dominialidade_areas.nextval, :dominialidade, :tipo, :valor, :tid)", EsquemaCredenciadoBanco);

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
						:data_ultima_atualizacao, :arl_documento, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning d.id into :id", EsquemaCredenciadoBanco);

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
								:arl_cedida, :identificacao_arl_cedente, :cedente_receptor) returning r.id into :id", EsquemaCredenciadoBanco);

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
									if (itemAux.Coordenada.Id > 0)
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
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.criar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(Dominialidade caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade c set c.croqui_area = :croqui_area, c.documento_area = :documento_area, 
				c.ccri_area =:ccri_area, c.arl_croqui = :arl_croqui, c.arl_documento = :arl_documento, c.app_area =:app_area, c.possui_area_exced_matri = :possui_area_exced_matri, 
				c.confrontante_norte = :confrontante_norte, c.confrontante_sul = :confrontante_sul, c.confrontante_leste = :confrontante_leste, 
				c.confrontante_oeste = :confrontante_oeste, c.alterado_copiar = :alterado_copiar, c.tid = :tid where c.id = :id", EsquemaCredenciadoBanco);

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
				comando.AdicionarParametroEntrada("alterado_copiar", caracterizacao.InternoID > 0 ? 1 : (Object)DBNull.Value, DbType.Int16);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				#region Domínios/Reserva Legais

				//Coordenada reservas legais
				comando = bancoDeDados.CriarComando("delete from {0}crt_dominia_reserva_coord c ", EsquemaCredenciadoBanco);
				comando.DbCommand.CommandText += String.Format(@"where c.reserva in (select r.id from crt_dominialidade_reserva r 
				where r.dominio in (select d.id from crt_dominialidade_dominio d where d.dominialidade = :dominialidade)) {0}",
				comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Dominios.SelectMany(x => x.ReservasLegais).Where(x => x.Coordenada != null).Select(x => x.Coordenada.Id).ToList()));
				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_dominialidade_reserva c where c.dominio in 
				(select a.id from {0}crt_dominialidade_dominio a where a.dominialidade = :dominialidade ", EsquemaCredenciadoBanco);

				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "a.id", DbType.Int32, caracterizacao.Dominios.Select(x => x.Id).ToList());

				comando.DbCommand.CommandText += ")";

				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (Dominio item in caracterizacao.Dominios)
				{
					comando = bancoDeDados.CriarComando(@"delete from {0}crt_dominialidade_reserva c 
						where c.dominio in (select a.id from {0}crt_dominialidade_dominio a where a.dominialidade = :dominialidade and a.id = :dominio)", EsquemaCredenciadoBanco);
					comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, item.ReservasLegais.Select(x => x.Id).ToList()));

					comando.AdicionarParametroEntrada("dominio", item.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				//Areas
				comando = bancoDeDados.CriarComando("delete from {0}crt_dominialidade_areas c ", EsquemaCredenciadoBanco);
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
							values ({0}seq_crt_dominialidade_areas.nextval, :dominialidade, :tipo, :valor, :tid)", EsquemaCredenciadoBanco);

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
							d.confrontante_leste = :confrontante_leste, d.confrontante_oeste = :confrontante_oeste, d.tid = :tid where d.id = :id", EsquemaCredenciadoBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_dominio d (id, dominialidade, identificacao, tipo, matricula, folha, livro, 
							cartorio, area_croqui, area_documento, app_croqui, comprovacao, registro, numero_ccri, area_ccri, data_ultima_atualizacao, arl_documento, 
							confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, tid) values({0}seq_crt_dominialidade_dominio.nextval, :dominialidade, 
							:identificacao, :tipo, :matricula, :folha, :livro, :cartorio, :area_croqui, :area_documento, :app_croqui, :comprovacao, :registro, :numero_ccri, :area_ccri, 
							:data_ultima_atualizacao, :arl_documento, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, :tid) returning d.id into :id",
							EsquemaCredenciadoBanco);

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

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.atualizar, bancoDeDados, null);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_dominialidade c where c.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade c set c.tid = :tid where c.id = :id", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					"delete from {0}crt_dominialidade_reserva r where r.dominio in (select d.id from {0}crt_dominialidade_dominio d where d.dominialidade = :caracterizacao);" +
					"delete from {0}crt_dominialidade_dominio b where b.dominialidade = :caracterizacao;" +
					"delete from {0}crt_dominialidade_areas b where b.dominialidade = :caracterizacao;" +
					"delete from {0}crt_dominialidade e where e.id = :caracterizacao;" +
				"end;", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void CopiarDadosInstitucional(Dominialidade caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando;

				#region Dominialidade

				if (caracterizacao.Id <= 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade c (id, interno_id, interno_tid, empreendimento, croqui_area, documento_area, 
					ccri_area, arl_croqui, arl_documento, app_area, possui_area_exced_matri, confrontante_norte, confrontante_sul, confrontante_leste, confrontante_oeste, alterado_copiar, tid) 
					values ({0}seq_crt_dominialidade.nextval, :interno_id, :interno_tid, :empreendimento, :croqui_area, :documento_area, :ccri_area, :arl_croqui, 
					:arl_documento, :app_area, :possui_area_exced_matri, :confrontante_norte, :confrontante_sul, :confrontante_leste, :confrontante_oeste, 0, :tid) returning c.id into :id",
					EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}crt_dominialidade c set c.interno_id = :interno_id, c.interno_tid = :interno_tid, c.croqui_area = :croqui_area, 
					c.documento_area = :documento_area, c.ccri_area =:ccri_area, c.arl_croqui = :arl_croqui, c.arl_documento = :arl_documento, c.app_area =:app_area, 
					c.possui_area_exced_matri = :possui_area_exced_matri, c.confrontante_norte = :confrontante_norte, c.confrontante_sul = :confrontante_sul, 
					c.confrontante_leste = :confrontante_leste, c.confrontante_oeste = :confrontante_oeste, alterado_copiar = 0, c.tid = :tid where c.id = :id", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("interno_id", caracterizacao.InternoID, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, caracterizacao.InternoTID);

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

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					delete from crt_dominia_reserva_coord c where c.reserva in (select r.id from crt_dominialidade_reserva r where r.dominio in (select d.id from crt_dominialidade_dominio d where d.dominialidade = :dominialidade));
					delete from crt_dominialidade_reserva r where r.dominio in (select d.id from crt_dominialidade_dominio d where d.dominialidade = :dominialidade);
					delete from crt_dominialidade_dominio d where d.dominialidade = :dominialidade;
					delete from crt_dominialidade_areas a where a.dominialidade = :dominialidade;
				end; ", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominialidade", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (DominialidadeArea item in caracterizacao.Areas)
					{
						comando = bancoDeDados.CriarComando(@"insert into {0}crt_dominialidade_areas(id, dominialidade, tipo, valor, tid)
						values ({0}seq_crt_dominialidade_areas.nextval, :dominialidade, :tipo, :valor, :tid)", EsquemaCredenciadoBanco);

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

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.copiar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter

		internal List<ReservaLegal> ObterReservasLegais(int dominioId)
		{
			ReservaLegal item = null;
			List<ReservaLegal> colecao = null;
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				comando = bancoDeDados.CriarComando(@"select r.id, r.situacao, r.localizacao, r.situacao_vegetal from {0}crt_dominialidade_reserva r where r.dominio = :dominio", EsquemaCredenciadoBanco);

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_dominialidade s where s.empreendimento = :empreendimento", EsquemaCredenciadoBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}

				#endregion
			}

			return caracterizacao;
		}

		internal Dominialidade Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_dominialidade s where s.id = :id and s.tid = :tid", EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}

				#endregion
			}

			return caracterizacao;
		}

		internal Dominialidade Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.empreendimento, ee.zona empreendimento_localizacao, d.possui_area_exced_matri, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, d.tid, d.interno_id, d.interno_tid, d.alterado_copiar  
				from {0}crt_dominialidade d, {0}tab_empreendimento_endereco ee 
				where ee.correspondencia = 0 and d.empreendimento = ee.empreendimento and d.id = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						caracterizacao.EmpreendimentoLocalizacao = Convert.ToInt32(reader["empreendimento_localizacao"]);
						caracterizacao.PossuiAreaExcedenteMatricula = reader.GetValue<int?>("possui_area_exced_matri");
						caracterizacao.ConfrontacaoNorte = reader["confrontante_norte"].ToString();
						caracterizacao.ConfrontacaoSul = reader["confrontante_sul"].ToString();
						caracterizacao.ConfrontacaoLeste = reader["confrontante_leste"].ToString();
						caracterizacao.ConfrontacaoOeste = reader["confrontante_oeste"].ToString();
						caracterizacao.Tid = reader["tid"].ToString();
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");
						caracterizacao.AlteradoCopiar = reader.GetValue<bool>("alterado_copiar");
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
				where a.tipo = la.id and a.dominialidade = :dominialidade", EsquemaCredenciadoBanco);

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
				{0}lov_crt_domin_comprovacao ldc where d.tipo = ldt.id and d.comprovacao = ldc.id(+) and d.dominialidade = :id", EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
									reserva.EmpreendimentoCompensacao = _caracterizacaoInternoDa.ObterEmpreendimentoSimplificado(reserva.EmpreendimentoCompensacao.Id);
								}

								if (reserva.IdentificacaoARLCedente > 0)
								{
									DominialidadeInternoDa internoDA = new DominialidadeInternoDa();
									ReservaLegal reservaAux = internoDA.ObterARLPorId(reserva.IdentificacaoARLCedente);

									reserva.ARLCroqui = reservaAux.ARLCroqui;
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

		private Dominialidade ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.id, d.empreendimento_id, d.possui_area_exced_matri, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, d.tid, d.interno_id, d.interno_tid, d.alterado_copiar
				from {0}hst_crt_dominialidade d where d.dominialidade_id = :id and d.tid = :tid", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						caracterizacao.PossuiAreaExcedenteMatricula = reader.GetValue<int?>("possui_area_exced_matri");
						caracterizacao.ConfrontacaoNorte = reader.GetValue<string>("confrontante_norte");
						caracterizacao.ConfrontacaoSul = reader.GetValue<string>("confrontante_sul");
						caracterizacao.ConfrontacaoLeste = reader.GetValue<string>("confrontante_leste");
						caracterizacao.ConfrontacaoOeste = reader.GetValue<string>("confrontante_oeste");
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.InternoID = reader.GetValue<int>("interno_id");
						caracterizacao.InternoTID = reader.GetValue<string>("interno_tid");
						caracterizacao.AlteradoCopiar = reader.GetValue<bool>("alterado_copiar");
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id, a.tid, a.tipo_id, a.tipo_texto, a.valor
				from {0}hst_crt_dominialidade_areas a where a.id_hst = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					DominialidadeArea item;
					while (reader.Read())
					{
						item = new DominialidadeArea();
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.Tipo = reader.GetValue<int>("tipo_id");
						item.TipoTexto = reader.GetValue<string>("tipo_texto");
						item.Valor = reader.GetValue<decimal>("valor");

						caracterizacao.Areas.Add(item);
					}

					reader.Close();
				}

				#endregion

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select d.id, d.dominialidade_dominio_id, d.identificacao, d.tipo_id, d.tipo_texto, d.matricula, d.folha, d.livro, d.cartorio, d.area_croqui,
				d.area_documento, d.app_croqui, d.comprovacao_id, d.comprovacao_texto, d.registro, d.numero_ccri, d.area_ccri, d.data_ultima_atualizacao, d.tid, d.arl_documento, d.confrontante_norte, 
				d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste from {0}hst_crt_dominialidade_dominio d where d.id_hst = :id", EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Dominio dominio = null;

					while (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						dominio = new Dominio();
						dominio.Id = reader.GetValue<int>("dominialidade_dominio_id");
						dominio.Tid = reader.GetValue<string>("tid");
						dominio.Identificacao = reader.GetValue<string>("identificacao");
						dominio.Tipo = (eDominioTipo)reader.GetValue<int>("tipo_id");
						dominio.TipoTexto = reader.GetValue<string>("tipo_texto");
						dominio.Matricula = reader.GetValue<string>("matricula");
						dominio.Folha = reader.GetValue<string>("folha");
						dominio.Livro = reader.GetValue<string>("livro");
						dominio.Cartorio = reader.GetValue<string>("cartorio");
						dominio.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						dominio.AreaDocumento = reader.GetValue<decimal>("area_documento");
						dominio.AreaDocumentoTexto = reader.GetValue<decimal>("area_documento").ToStringTrunc();
						dominio.APPCroqui = reader.GetValue<decimal>("app_croqui");
						dominio.DescricaoComprovacao = reader.GetValue<string>("registro"); //Campo alterado
						dominio.AreaCCIR = reader.GetValue<decimal>("area_ccri");
						dominio.AreaCCIRTexto = reader.GetValue<decimal>("area_ccri").ToStringTrunc();
						dominio.DataUltimaAtualizacao.DataTexto = reader.GetValue<string>("data_ultima_atualizacao");
						dominio.ARLDocumento = reader.GetValue<decimal>("arl_documento");
						dominio.ARLDocumentoTexto = reader.GetValue<decimal>("arl_documento").ToStringTrunc();
						dominio.ConfrontacaoNorte = reader.GetValue<string>("confrontante_norte");
						dominio.ConfrontacaoSul = reader.GetValue<string>("confrontante_sul");
						dominio.ConfrontacaoLeste = reader.GetValue<string>("confrontante_leste");
						dominio.ConfrontacaoOeste = reader.GetValue<string>("confrontante_oeste");
						dominio.NumeroCCIR = reader.GetValue<long>("numero_ccri");
						dominio.ComprovacaoId = reader.GetValue<int>("comprovacao_id");
						dominio.ComprovacaoTexto = reader.GetValue<string>("comprovacao_texto");

						#region Reservas Legais

						comando = bancoDeDados.CriarComando(@"select r.dominialidade_reserva_id, r.tid, r.situacao_id, r.situacao_texto, r.localizacao_id, r.localizacao_texto, 
						r.identificacao, r.situacao_vegetal_id, r.situacao_vegetal_texto, r.arl_croqui, r.numero_termo, r.cartorio_id, r.cartorio_texto, r.matricula_id,  r.compensada, 
						r.numero_cartorio, r.nome_cartorio, r.numero_folha, r.numero_livro, r.matricula_numero, r.averbacao_numero, r.arl_recebida, r.emp_compensacao_id, r.cedente_possui_emp, 
						r.arl_cedida, r.arl_cedente_id, r.arl_cedente_tid, c.dominia_reserva_coord_id, c.coordenada_tipo_id, c.coordenada_tipo_texto, c.datum_id, c.datum_texto, c.easting_utm, c.northing_utm, 
						(select d.identificacao from hst_crt_dominialidade_dominio d where d.dominialidade_dominio_id = r.matricula_id and d.tid = r.matricula_tid) matricula_identificacao 
						from {0}hst_crt_dominialidade_reserva r, {0}hst_crt_dominia_reserva_coord c where c.id_hst(+) = r.id and r.id_hst = :id", EsquemaCredenciadoBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ReservaLegal reserva = null;

							while (readerAux.Read())
							{
								reserva = new ReservaLegal();
								reserva.Id = readerAux.GetValue<int>("dominialidade_reserva_id");
								reserva.Tid = readerAux.GetValue<string>("tid");
								reserva.SituacaoId = readerAux.GetValue<int>("situacao_id");
								reserva.SituacaoTexto = readerAux.GetValue<string>("situacao_texto");
								reserva.Identificacao = readerAux.GetValue<string>("identificacao");
								reserva.Compensada = readerAux.GetValue<bool>("compensada");
								reserva.LocalizacaoId = readerAux.GetValue<int>("localizacao_id");
								reserva.LocalizacaoTexto = readerAux.GetValue<string>("localizacao_texto");
								reserva.SituacaoVegetalId = readerAux.GetValue<int>("situacao_vegetal_id");
								reserva.SituacaoVegetalTexto = readerAux.GetValue<string>("situacao_vegetal_texto");
								reserva.ARLCroqui = readerAux.GetValue<decimal>("arl_croqui");
								reserva.NumeroTermo = readerAux.GetValue<string>("numero_termo");
								reserva.TipoCartorioId = readerAux.GetValue<int>("cartorio_id");
								reserva.TipoCartorioTexto = readerAux["cartorio_texto"].ToString();
								reserva.MatriculaId = readerAux.GetValue<int>("matricula_id");
								reserva.MatriculaIdentificacao = readerAux.GetValue<string>("matricula_identificacao");
								reserva.NumeroCartorio = readerAux.GetValue<string>("numero_cartorio");
								reserva.NomeCartorio = readerAux.GetValue<string>("nome_cartorio");
								reserva.NumeroFolha = readerAux.GetValue<string>("numero_folha");
								reserva.NumeroLivro = readerAux.GetValue<string>("numero_livro");

								//Compensação
								reserva.MatriculaNumero = readerAux.GetValue<string>("matricula_numero");
								reserva.AverbacaoNumero = readerAux.GetValue<string>("averbacao_numero");
								reserva.ARLRecebida = readerAux.GetValue<decimal>("arl_recebida");
								reserva.EmpreendimentoCompensacao.Id = readerAux.GetValue<int>("emp_compensacao_id");
								reserva.CedentePossuiEmpreendimento = readerAux.GetValue<int>("cedente_possui_emp");
								reserva.ARLCedida = readerAux.GetValue<decimal>("arl_cedida");
								reserva.IdentificacaoARLCedente = readerAux.GetValue<int>("arl_cedente_id");
								reserva.ARLCedenteTID = readerAux.GetValue<string>("arl_cedente_tid");

								//Coordenada
								reserva.Coordenada.Id = readerAux.GetValue<int>("dominia_reserva_coord_id");
								reserva.Coordenada.Tipo.Id = readerAux.GetValue<int>("coordenada_tipo_id");
								reserva.Coordenada.Tipo.Texto = readerAux.GetValue<string>("coordenada_tipo_texto");
								reserva.Coordenada.Datum.Id = readerAux.GetValue<int>("datum_id");
								reserva.Coordenada.Datum.Texto = readerAux.GetValue<string>("datum_texto");
								reserva.Coordenada.EastingUtm = readerAux.GetValue<double?>("easting_utm");
								reserva.Coordenada.NorthingUtm = readerAux.GetValue<double?>("northing_utm");

								if (reserva.EmpreendimentoCompensacao.Id > 0)
								{
									reserva.EmpreendimentoCompensacao = _caracterizacaoInternoDa.ObterEmpreendimentoSimplificado(reserva.EmpreendimentoCompensacao.Id);
								}

								dominio.ReservasLegais.Add(reserva);
							}

							readerAux.Close();
						}

						dominio.ReservasLegais.ForEach(reserva =>
						{
							if (reserva.IdentificacaoARLCedente > 0)
							{
								comando = bancoDeDados.CriarComando(@"select r.situacao_vegetal_id, r.situacao_vegetal_texto, r.arl_croqui from hst_crt_dominialidade_reserva r 
								where r.dominialidade_reserva_id = :id and r.tid = :tid", EsquemaBanco);

								comando.AdicionarParametroEntrada("id", reserva.IdentificacaoARLCedente, DbType.Int32);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, reserva.ARLCedenteTID);

								using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
								{
									if (readerAux.Read())
									{
										reserva.ARLCroqui = readerAux.GetValue<decimal>("arl_croqui");
										reserva.SituacaoVegetalId = readerAux.GetValue<int>("situacao_vegetal_id");
										reserva.SituacaoVegetalTexto = readerAux.GetValue<string>("situacao_vegetal_texto");
									}
								}
							}
						});

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"
				select a.id, a.area_m2 from {1}geo_atp a, {0}crt_projeto_geo p 
				where p.id = a.projeto and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao",
				EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
				EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
						from {0}geo_arl a, table(geometria9i.pontoIdeal(a.geometry).SDO_ORDINATES) arl where a.cod_apmp = :apmp", EsquemaCredenciadoBancoGeo);

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
						EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
				  group by a.tipo", EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
					  group by a.tipo, a.vegetacao", EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciadoBanco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"
				select a.id, a.area_m2 from {1}tmp_atp a, {0}tmp_projeto_geo p 
				where p.id = a.projeto and p.empreendimento = :empreendimento and p.caracterizacao = :caracterizacao",
				EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
				EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
						from {0}tmp_arl a, table(geometria9i.pontoIdeal(a.geometry).SDO_ORDINATES) arl where a.cod_apmp = :apmp", EsquemaCredenciadoBancoGeo);

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
						EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
				  group by a.tipo", EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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
					  group by a.tipo, a.vegetacao", EsquemaCredenciadoBanco, EsquemaCredenciadoBancoGeo);

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

		#endregion

		#region Validações

		public bool EmPosse(int empreendimento)
		{
			return _caracterizacaoDa.EmPosse(empreendimento);
		}

		internal List<string> VerificarExcluirDominios(Dominialidade caracterizacao)
		{
			List<string> retorno = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select (case when c.tipo = 1 then 'Matricula' else 'Posse' end)|| ' - ' ||c.identificacao 
				from {0}crt_dominialidade_dominio c where c.dominialidade = (select d.id from {0}crt_dominialidade d where d.empreendimento = :empreendimento) 
				and exists(select 1 from {0}crt_dominialidade_reserva r where r.matricula = c.id)", EsquemaCredenciadoBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "c.identificacao", DbType.String, caracterizacao.Dominios.Select(x => x.Identificacao).ToList());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);

				retorno = bancoDeDados.ExecutarList<string>(comando);
			}

			return retorno;
		}

		#endregion

		#region Auxiliares

		internal void AtualizarInternoIdTid(int dominialidadeId, int dominialidadeInternoId, string dominialidadeInternoTid, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualização do Tid

				Comando comando = bancoDeDados.CriarComandoPlSql(@"begin
				update crt_dominialidade set tid = :tid, interno_id = :interno_id, interno_tid = :interno_tid where id = :dominialidade_id;
				update crt_dominialidade_areas set tid = :tid where dominialidade = :dominialidade_id;
				update crt_dominialidade_dominio set tid = :tid where dominialidade = :dominialidade_id;
				update crt_dominialidade_reserva set tid = :tid where dominio in (select id from crt_dominialidade_dominio where dominialidade = :dominialidade_id);
				end;");

				comando.AdicionarParametroEntrada("dominialidade_id", dominialidadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("interno_id", dominialidadeInternoId, DbType.Int32);
				comando.AdicionarParametroEntrada("interno_tid", DbType.String, 36, dominialidadeInternoTid);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(dominialidadeId, eHistoricoArtefatoCaracterizacao.dominialidade, eHistoricoAcao.atualizaridtid, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

			}
		}

		#endregion

		internal List<ReservaLegal> ObterRLGeoProcessadas(int projetoGeoId, BancoDeDados banco = null)
		{
			List<ReservaLegal> retorno = new List<ReservaLegal>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select codigo identificacao, (case when compensada = 'S' then 1 else 0 end)compensada, area_m2, situacao  from 
				{0}tmp_arl where projeto = :id", EsquemaCredenciadoBancoGeo);

				comando.AdicionarParametroEntrada("id", projetoGeoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ReservaLegal reserva = null;

					while (reader.Read())
					{
						reserva = new ReservaLegal();
						reserva.Identificacao = reader.GetValue<string>("identificacao");
						reserva.ARLCroqui = reader.GetValue<decimal>("area_m2");
						reserva.SituacaoVegetalGeo = reader.GetValue<string>("situacao");
						reserva.Compensada = reader.GetValue<bool>("compensada");
						retorno.Add(reserva);
					}
				}
			}
			return retorno;
		}

		internal Coordenada ObterCoordenada(int arlCedenteId, BancoDeDados banco = null)
		{
			Coordenada retorno = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciadoBanco))
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
				from {1}crt_dominialidade_reserva where id = :reserva_id))", EsquemaCredenciadoBancoGeo, EsquemaCredenciadoBanco);

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