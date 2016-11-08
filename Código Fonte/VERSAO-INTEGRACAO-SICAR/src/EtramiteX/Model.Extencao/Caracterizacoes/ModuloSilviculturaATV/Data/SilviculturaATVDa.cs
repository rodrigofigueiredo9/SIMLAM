using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloSilviculturaATV;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloSilviculturaATV.Data
{
	public class SilviculturaATVDa
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

		public SilviculturaATVDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(SilviculturaATV caracterizacao, BancoDeDados banco)
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

		internal int? Criar(SilviculturaATV caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura - Implantação da Atividade de Silvicultura (Fomento)

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
					insert into {0}crt_silvicultura_atv c
					  (id, 
					   empreendimento, 
					   tid)
					values
					  ({0}seq_crt_silvicultura_atv.nextval, 
					   :empreendimento, 
					   :tid)
					returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Silvicultura/Áreas

				if (caracterizacao.Areas != null && caracterizacao.Areas.Count > 0)
				{
					foreach (var area in caracterizacao.Areas)
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_silvicultura_atv_areas
							  (id, 
							   caracterizacao, 
							   tipo, 
							   valor, 
							   tid)
							values
							  ({0}seq_crt_silvic_atv_areas.nextval,
							   :caracterizacao,
							   :tipo,
							   :valor,
							   :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("tipo", area.Tipo, DbType.Int32);
						comando.AdicionarParametroEntrada("valor", area.Valor, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				#region Silvicultura/Caracteristicas

				if (caracterizacao.Caracteristicas != null && caracterizacao.Caracteristicas.Count > 0)
				{
					foreach (var item in caracterizacao.Caracteristicas)
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_silvicultura_atv_carac c
							  (id,
							   caracterizacao,
							   identificacao,
							   geometria,
							   fomento,
							   declividade,
							   total_requerida,
							   total_croqui,
							   total_plant_eucalipto,
							   tid)
							values
							  ({0}seq_crt_silvic_atv_carac.nextval,
							   :caracterizacao,
							   :identificacao,
							   :geometria,
							   :fomento,
							   :declividade,
							   :total_requerida,
							   :total_croqui,
							   :total_plant_eucalipto,
							   :tid)
							returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("identificacao", item.Identificacao, DbType.String);
						comando.AdicionarParametroEntrada("geometria", item.GeometriaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("fomento", item.Fomento, DbType.Int32);
						comando.AdicionarParametroEntrada("declividade", item.Declividade, DbType.Decimal);
						comando.AdicionarParametroEntrada("total_requerida", item.TotalRequerida, DbType.Decimal);
						comando.AdicionarParametroEntrada("total_croqui", item.TotalCroqui, DbType.Decimal);
						comando.AdicionarParametroEntrada("total_plant_eucalipto", item.TotalPlantadaComEucalipto, DbType.Decimal);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						item.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Culturas Florestais

						if (item.Culturas != null && item.Culturas.Count > 0)
						{
							foreach (CulturaFlorestalATV itemAux in item.Culturas)
							{
								comando = bancoDeDados.CriarComando(@"
									insert into {0}crt_silvicultura_atv_cult c
									  (id, 
									   caracterizacao, 
									   silvicultura_id, 
									   cultura, 
									   area, 
									   especificar, 
									   tid)
									values
									  ({0}seq_crt_silvic_atv_cult.nextval,
									   :caracterizacao,
									   :silvicultura_id,
									   :cultura,
									   :area,
									   :especificar,
									   :tid)", EsquemaBanco);

								comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("silvicultura_id", item.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("cultura", itemAux.CulturaTipo, DbType.Int32);
								comando.AdicionarParametroEntrada("area", itemAux.AreaCultura, DbType.Decimal);
								comando.AdicionarParametroEntrada("especificar", String.Empty, DbType.String);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

								bancoDeDados.ExecutarNonQuery(comando);
							}
						}

						#endregion
					}
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.silviculturaatv, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(SilviculturaATV caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura - Implantação da Atividade de Silvicultura (Fomento)

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura c set c.tid = :tid where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando("delete from {0}crt_silvicultura_atv_cult t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.caracterizacao = :caracterizacao{0}",
					comando.AdicionarNotIn("and", "t.silvicultura_id", DbType.Int32, caracterizacao.Caracteristicas.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				foreach (var item in caracterizacao.Caracteristicas)
				{
					comando = bancoDeDados.CriarComando("delete from {0}crt_silvicultura_atv_cult t ", EsquemaBanco);
					comando.DbCommand.CommandText += String.Format("where t.silvicultura_id = :silvicultura_id{0}",
						comando.AdicionarNotIn("and", "t.id", DbType.Int32, item.Culturas.Select(x => x.Id).ToList()));
					comando.AdicionarParametroEntrada("silvicultura_id", item.Id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				comando = bancoDeDados.CriarComando("delete from {0}crt_silvicultura_atv_carac t ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where t.caracterizacao = :caracterizacao{0}",
					comando.AdicionarNotIn("and", "t.id", DbType.Int32, caracterizacao.Caracteristicas.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando("delete from {0}crt_silvicultura_atv_areas c ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where c.caracterizacao = :caracterizacao{0}",
					comando.AdicionarNotIn("and", "c.id", DbType.Int32, caracterizacao.Areas.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Áreas

				foreach (SilviculturaAreaATV area in caracterizacao.Areas)
				{
					if (area.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
							update {0}crt_silvicultura_atv_areas c
							   set c.tipo = :tipo, 
								   c.valor = :valor, 
								   c.tid = :tid
							 where c.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", area.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_silvicultura_atv_areas
							  (id, 
							   caracterizacao, 
							   tipo, 
							   valor, 
							   tid)
							values
							  ({0}seq_crt_silvic_atv_areas.nextval,
							   :caracterizacao,
							   :tipo,
							   :valor,
							   :tid)", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tipo", area.Tipo, DbType.Int32);
					comando.AdicionarParametroEntrada("valor", area.Valor, DbType.Decimal);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}				

				#endregion

				#region Caracteristicas

				foreach (var item in caracterizacao.Caracteristicas)
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
								update {0}crt_silvicultura_atv_carac c
								   set c.identificacao         = :identificacao,
									   c.geometria             = :geometria,
									   c.fomento               = :fomento,
									   c.declividade           = :declividade,
									   c.total_requerida       = :total_requerida,
									   c.total_croqui          = :total_croqui,
									   c.total_plant_eucalipto = :total_plant_eucalipto,
									   c.tid                   = :tid
								 where c.id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
								insert into {0}crt_silvicultura_atv_carac c
								  (id,
								   caracterizacao,
								   identificacao,
								   geometria,
								   fomento,
								   declividade,
								   total_requerida,
								   total_croqui,
								   total_plant_eucalipto,
								   tid)
								values
								  ({0}seq_crt_silvic_atv_carac.nextval,
								   :caracterizacao,
								   :identificacao,
								   :geometria,
								   :fomento,
								   :declividade,
								   :total_requerida,
								   :total_croqui,
								   :total_plant_eucalipto,
								   :tid)
								returning c.id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroSaida("id", DbType.Int32);
					}

					comando.AdicionarParametroEntrada("identificacao", item.Identificacao, DbType.String);
					comando.AdicionarParametroEntrada("geometria", item.GeometriaTipo, DbType.Int32);
					comando.AdicionarParametroEntrada("fomento", item.Fomento, DbType.Int32);
					comando.AdicionarParametroEntrada("declividade", item.Declividade, DbType.Decimal);
					comando.AdicionarParametroEntrada("total_requerida", item.TotalRequerida, DbType.Decimal);
					comando.AdicionarParametroEntrada("total_croqui", item.TotalCroqui, DbType.Decimal);
					comando.AdicionarParametroEntrada("total_plant_eucalipto", item.TotalPlantadaComEucalipto, DbType.Decimal);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);

					item.Id = item.Id > 0 ? item.Id : Convert.ToInt32(comando.ObterValorParametro("id"));

					#region Culturas Florestais

					foreach (CulturaFlorestalATV itemAux in item.Culturas)
					{
						if (itemAux.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"
								 update {0}crt_silvicultura_atv_cult c
									set c.caracterizacao  = :caracterizacao,
										c.silvicultura_id = :silvicultura_id,
										c.cultura         = :cultura,
										c.area            = :area,
										c.especificar     = :especificar,
										c.tid             = :tid
								  where c.id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", itemAux.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
								insert into {0}crt_silvicultura_atv_cult c
									(id, 
									caracterizacao, 
									silvicultura_id, 
									cultura, 
									area, 
									especificar, 
									tid)
								values
									({0}seq_crt_silvic_atv_cult.nextval,
									:caracterizacao,
									:silvicultura_id,
									:cultura,
									:area,
									:especificar,
									:tid) returning id into :id", EsquemaBanco);

							comando.AdicionarParametroSaida("id", DbType.Int32);
						}

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("silvicultura_id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("cultura", itemAux.CulturaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("area", itemAux.AreaCultura, DbType.Decimal);
						comando.AdicionarParametroEntrada("especificar", string.Empty, DbType.String);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);

						itemAux.Id = itemAux.Id > 0 ? itemAux.Id : Convert.ToInt32(comando.ObterValorParametro("id"));
					}

					#endregion
				}

				#endregion

				#region Histórico

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.silviculturaatv, eHistoricoAcao.atualizar, bancoDeDados);

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

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_silvicultura_atv c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_silvicultura_atv c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.silviculturaatv, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComando(@"begin " +
					@"delete from {0}crt_dependencia d where d.dependente_tipo = :dependente_tipo and d.dependente_id = :caracterizacao and d.dependente_caracterizacao = :dependente_caracterizacao;" +
					@"delete from {0}crt_silvicultura_atv_cult m where m.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_silvicultura_atv_areas b where b.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_silvicultura_atv_carac m where m.caracterizacao = :caracterizacao;" +
					@"delete from {0}crt_silvicultura_atv e where e.id = :caracterizacao;" +
				@" end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_tipo", (int)eCaracterizacaoDependenciaTipo.Caracterizacao, DbType.Int32);
				comando.AdicionarParametroEntrada("dependente_caracterizacao", (int)eCaracterizacao.SilviculturaATV, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal SilviculturaATV ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			SilviculturaATV caracterizacao = new SilviculturaATV();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_silvicultura_atv s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal SilviculturaATV Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			SilviculturaATV caracterizacao = new SilviculturaATV();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_silvicultura s where s.id = :id and s.tid = :tid", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? Obter(id, bancoDeDados, simplificado) : ObterHistorico(id, bancoDeDados, tid, simplificado);
				}
			}

			return caracterizacao;
		}

		internal SilviculturaATV Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			SilviculturaATV caracterizacao = new SilviculturaATV();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura - Implantação da Atividade de Silvicultura (Fomento)

				Comando comando = bancoDeDados.CriarComando(@"select c.id Id, c.empreendimento EmpreendimentoId, c.tid Tid from {0}crt_silvicultura_atv c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				caracterizacao = bancoDeDados.ObterEntity<SilviculturaATV>(comando);

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Áreas

				comando = bancoDeDados.CriarComando(@"
					select a.id         Id,
						   la.id       Tipo,
						   la.texto     TipoTexto,
						   a.valor      Valor,
						   a.tid        Tid,
						   la.descricao Descricao
					  from crt_silvicultura_atv_areas a, 
						   lov_crt_silvic_atv_area la
					 where la.id = a.tipo(+)
					   and :caracterizacao = a.caracterizacao(+)", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				caracterizacao.Areas = bancoDeDados.ObterEntityList<SilviculturaAreaATV>(comando, (IDataReader reader, SilviculturaAreaATV item) => 
				{ 
					item.Valor = reader.GetValue<Decimal>("Valor");
					item.ValorTexto = reader.GetValue<Decimal>("Valor").ToStringTrunc(); 
				});

				#endregion

				#region Caracteriscas

				comando = bancoDeDados.CriarComando(@"
					select c.id Id,
						   c.tid Tid,
						   c.identificacao Identificacao,
						   lv.id GeometriaTipo,
						   lv.texto GeometriaTipoTexto,
						   c.fomento FomentoId,
						   c.declividade Declividade,
						   c.total_requerida TotalRequerida,
						   c.total_croqui TotalCroqui,
						   c.total_plant_eucalipto TotalPlantadaComEucalipto
					  from {0}crt_silvicultura_atv_carac c, 
						   {0}lov_crt_geometria_tipo lv,
						   {0}lov_crt_silvic_atv_fomento lf
					 where c.caracterizacao = :id
					   and c.geometria = lv.id(+)
					   and c.fomento = lf.id(+)
					 order by c.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				caracterizacao.Caracteristicas = bancoDeDados.ObterEntityList<SilviculturaCaracteristicaATV>(comando);

				#region Culturas Florestais

				foreach (var item in caracterizacao.Caracteristicas)
				{
					comando = bancoDeDados.CriarComando(@"
						select c.id Id,
							   lc.id CulturaTipo,
							   lc.texto CulturaTipoTexto,
							   c.area AreaCultura,
							   c.especificar EspecificarTexto,
							   c.tid Tid
						  from {0}crt_silvicultura_atv_cult c, 
							   {0}lov_crt_silvic_atv_cobe lc
						 where c.cultura = lc.id
						   and c.silvicultura_id = :silvicultura", EsquemaBanco);

					comando.AdicionarParametroEntrada("silvicultura", item.Id, DbType.Int32);

					item.Culturas = bancoDeDados.ObterEntityList<CulturaFlorestalATV>(comando);
				}

				#endregion

				#endregion
			}

			return caracterizacao;
		}

		private SilviculturaATV ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			SilviculturaATV caracterizacao = new SilviculturaATV();
			int hst = 0;
			int hstCaracterizacao = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Silvicultura - Implantação da Atividade de Silvicultura (Fomento)

				Comando comando = bancoDeDados.CriarComando(@"
					select c.caracterizacao Id, 
						   c.empreendimento_id EmpreendimentoId, 
						   c.tid Tid,
						   c.id hstId
					  from {0}hst_crt_silvicultura_atv c
					 where c.caracterizacao = :id
					   and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				caracterizacao = bancoDeDados.ObterEntity<SilviculturaATV>(comando, (IDataReader reader, SilviculturaATV item) => { hst = reader.GetValue<int>("hstId"); });
				hstCaracterizacao = hst;

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Áreas

				comando = bancoDeDados.CriarComando(@"
					select a.silvicultura_area_id Id, 
						   a.tipo_id Tipo, 
						   a.tipo_texto TipoTexto, 
						   a.valor Valor, 
						   a.tid Tid,
						   la.descricao Descricao
					  from {0}hst_crt_silvic_atv_areas a,
					       {0}lov_crt_silvic_atv_area la 
					 where la.id = a.tipo_id(+)
					   and a.id_hst = :hst_caracterizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("hst_caracterizacao", hstCaracterizacao, DbType.Int32);

				caracterizacao.Areas = bancoDeDados.ObterEntityList<SilviculturaAreaATV>(comando, (IDataReader reader, SilviculturaAreaATV item) => 
				{ 
					item.Valor = reader.GetValue<Decimal>("Valor");
					item.ValorTexto = reader.GetValue<Decimal>("Valor").ToStringTrunc(); 
				});			

				#endregion

				#region Caracteristicas

				comando = bancoDeDados.CriarComando(@"
					select c.id hstId,
						   c.silvicultura_silv_id Id,
						   c.identificacao Identificacao,
						   c.geometria_id GeometriaTipo,
						   c.geometria_texto GeometriaTipoTexto,
						   c.fomento_id Fomento,
						   c.declividade Declividade,
						   c.total_requerida TotalRequerida,
						   c.total_croqui TotalCroqui,
						   c.total_plant_eucalipto TotalPlantadaComEucalipto,       
						   c.tid Tid       
					  from {0}hst_crt_silvic_atv_carac c
					 where c.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);

				caracterizacao.Caracteristicas = bancoDeDados.ObterEntityList<SilviculturaCaracteristicaATV>(comando, (IDataReader reader, SilviculturaCaracteristicaATV item) =>
				{
					#region Culturas Florestais
					hst = reader.GetValue<int>("hstId");
					comando = bancoDeDados.CriarComando(@"
							select c.silvicultura_cult_id Id,
								   c.cultura_id CulturaTipo,
								   c.cultura_texto CulturaTipoTexto,
								   c.area AreaCultura,
								   c.especificar EspecificarTexto,
								   c.tid Tid
							  from {0}hst_crt_silvic_atv_cult c
							 where c.id_hst = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", hst, DbType.Int32);
					item.Culturas = bancoDeDados.ObterEntityList<CulturaFlorestalATV>(comando);
					#endregion
				});			

				#endregion
			}

			return caracterizacao;
		}

		internal SilviculturaATV ObterDadosGeo(int empreendimento, BancoDeDados banco = null)
		{
			SilviculturaATV caracterizacao = new SilviculturaATV();
			caracterizacao.EmpreendimentoId = empreendimento;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dados Geo

				Comando comando = bancoDeDados.CriarComando(@"
					select a.atividade,
						   a.codigo    Identificacao,
						   3           GeometriaTipo,
						   a.area_m2   TotalCroqui
					  from {1}geo_aativ a, 
						   {0}crt_projeto_geo g, 
						   {0}lov_caracterizacao_tipo lc
					 where a.atividade = lc.texto
					   and a.projeto = g.id
					   and lc.id = :caracterizacao
					   and g.empreendimento = :empreendimento
					   and g.caracterizacao = :caracterizacao
					union all
					select a.atividade,
						   a.codigo    Identificacao,
						   2           GeometriaTipo,
						   null        TotalCroqui
					  from {1}geo_lativ a, 
						   {0}crt_projeto_geo g, 
						   {0}lov_caracterizacao_tipo lc
					 where a.atividade = lc.texto
					   and a.projeto = g.id
					   and lc.id = :caracterizacao
					   and g.empreendimento = :empreendimento
					   and g.caracterizacao = :caracterizacao
					union all
					select a.atividade,
						   a.codigo    Identificacao,
						   1           GeometriaTipo,
						   null        TotalCroqui
					  from {1}geo_pativ a, 
						   {0}crt_projeto_geo g, 
						   {0}lov_caracterizacao_tipo lc
					 where a.atividade = lc.texto
					   and a.projeto = g.id
					   and lc.id = :caracterizacao
					   and g.empreendimento = :empreendimento
					   and g.caracterizacao = :caracterizacao
					union all
					select a.atividade,
						   a.codigo    Identificacao,
						   3           GeometriaTipo,
						   a.area_m2   TotalCroqui
					  from {1}geo_aiativ a, 
						   {0}crt_projeto_geo g, 
						   {0}lov_caracterizacao_tipo lc
					 where a.atividade = lc.texto
					   and a.projeto = g.id
					   and lc.id = :caracterizacao
					   and g.empreendimento = :empreendimento
					   and g.caracterizacao = :caracterizacao", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao", (int)eCaracterizacao.SilviculturaATV, DbType.Int32);

				caracterizacao.Caracteristicas = bancoDeDados.ObterEntityList<SilviculturaCaracteristicaATV>(comando, (IDataReader reader, SilviculturaCaracteristicaATV item) => 
				{ 
					item.TotalCroqui = reader.GetValue<Decimal>("TotalCroqui");
				});

				#endregion

				#region Areas

				int projetoDomId = 0;

				comando = bancoDeDados.CriarComando(@"select g.id from {0}crt_projeto_geo g where g.empreendimento = :empreendimento and g.caracterizacao = :caracterizacao_dom", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao_dom", (int)eCaracterizacao.Dominialidade, DbType.Int32);

				projetoDomId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando = bancoDeDados.CriarComando(@"
					select 0              Id,
						   tab_areas.area Valor,
						   la.id          Tipo,
						   la.texto       TipoTexto,
						   la.descricao   Descricao
					  from (select sum(t.area_m2) area, 1 /*Área total - ATP (m²)*/ tipo
							  from {1}geo_atp t
							 where t.projeto = :projetoDominialidade
							union all
							select sum(t.area_m2) area, 2 /*Área de vegetação nativa (m²)*/ tipo
							  from {1}geo_avn t
							 where t.projeto = :projetoDominialidade
							union all
							select ta.valor area, la.id /*Declividade predominante (graus)*/
							  from {0}lov_crt_silvic_atv_area la, {0}crt_silvicultura_atv_areas ta
							 where la.id = ta.tipo(+)
							   and la.id = 3
							   and :projetoDominialidade = ta.caracterizacao(+)
							union all
							select sum(t.area_m2) area,
								   4 /*Área de floresta plantada (m²)*/ tipo
							  from {1}geo_aa t
							 where t.projeto = :projetoDominialidade
							   and t.vegetacao = 'FLORESTA-PLANTADA'
							union all
							select ta.valor area, la.id /*Área de outro fomento (m²)*/ tipo
							  from {0}lov_crt_silvic_atv_area la, {0}crt_silvicultura_atv_areas ta
							 where la.id = ta.tipo(+)
							   and la.id = 5
							   and :projetoDominialidade = ta.caracterizacao(+)
							union all
							select ta.valor area, la.id /*Área de plantio próprio (m²)*/ tipo
							  from {0}lov_crt_silvic_atv_area la, {0}crt_silvicultura_atv_areas ta
							 where la.id = ta.tipo(+)
							   and la.id = 6
							   and :projetoDominialidade = ta.caracterizacao(+)
							union all
							select (select sum(t.area_m2)
							  from {1}geo_areas_calculadas t
							 where t.projeto = :projetoDominialidade
							   and t.tipo = 'APP_APMP'
							 group by t.tipo) area, 7 /*APP (m²)*/tipo from dual
							union all
							select 0 area, 8 /*Total de floresta (m²)*/ tipo from dual
							union all
							select (select sum(t.area_m2)
									  from {1}geo_arl t
									 where t.projeto = :projetoDominialidade
									   and t.situacao in ('PRESERV')
									 group by t.situacao) area,
								   l.id /*Com vegetação nativa (m²)*/ tipo
							  from {0}lov_crt_silvic_atv_area l
							 where l.id = 9
							union all
							select (select sum(t.area_m2)
									  from {1}geo_arl t
									 where t.projeto = :projetoDominialidade
									   and t.situacao in ('USO')
									 group by t.situacao) area,
								   l.id /*Em uso (m²)*/ tipo
							  from {0}lov_crt_silvic_atv_area l
							 where l.id = 10
							union all
							select (select sum(t.area_m2)
									  from {1}geo_arl t
									 where t.projeto = :projetoDominialidade
									   and t.situacao in ('REC')
									 group by t.situacao) area,
								   l.id /*A recuperar (m²)*/ tipo
							  from {0}lov_crt_silvic_atv_area l
							 where l.id = 11
							union all
							select (select sum(t.area_m2)
									  from {1}geo_arl t
									 where t.projeto = :projetoDominialidade
									   and t.situacao in ('D')
									 group by t.situacao) area,
								   l.id /*Não caracterizada (m²)*/ tipo
							  from {0}lov_crt_silvic_atv_area l
							 where l.id = 12) tab_areas,
						   {0}lov_crt_silvic_atv_area la
					 where tab_areas.tipo = la.id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projetoDominialidade", projetoDomId, DbType.Int32);

				caracterizacao.Areas = bancoDeDados.ObterEntityList<SilviculturaAreaATV>(comando, (IDataReader reader, SilviculturaAreaATV item) => 
				{ 
					item.Valor = reader.GetValue<Decimal>("Valor");
					item.ValorTexto = reader.GetValue<Decimal>("Valor").ToStringTrunc(); 
				});
			}

			#endregion

			return caracterizacao;
		}

		#endregion

		#region Validações

		internal bool ExisteCaracterizacao(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(s.id) from {0}crt_silvicultura s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}
		
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