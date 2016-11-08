using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Especificidades.ModuloEspecificidade;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Extensoes.Caracterizacoes.ModuloRegistroAtividadeFlorestal.Data
{
	public class RegistroAtividadeFlorestalDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		internal Historico Historico { get { return _historico; } }
		private String EsquemaBanco { get; set; }

		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		#endregion

		public RegistroAtividadeFlorestalDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(RegistroAtividadeFlorestal caracterizacao, BancoDeDados banco)
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

		internal int? Criar(RegistroAtividadeFlorestal caracterizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Registro Atividade Florestal

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}crt_reg_atividade_florestal c (id, empreendimento,  numero_registro, possui_numero, 
				tid) values (seq_crt_reg_ativida_florestal.nextval, :empreendimento, " +
				(caracterizacao.PossuiNumero.Value ? @":numero_registro," : @"{0}reg_atv_florestal_numero.nextval,") + @":possui_numero,
				:tid) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_numero", caracterizacao.PossuiNumero, DbType.Int32);

				if (caracterizacao.PossuiNumero.Value)
				{
					comando.AdicionarParametroEntrada("numero_registro", caracterizacao.NumeroRegistro, DbType.String);
				}

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				caracterizacao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Consumos

				if (caracterizacao.Consumos != null && caracterizacao.Consumos.Count > 0)
				{
					foreach (Consumo consumo in caracterizacao.Consumos)
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}crt_reg_ati_flo_consumo (id, tid, caracterizacao, atividade, possui_licenca, dua_numero, dua_valor, dua_data_pagamento) 
						values (seq_crt_reg_ati_flo_consumo.nextval, :tid, :caracterizacao, :atividade, :possui_licenca, :dua_numero, :dua_valor, :dua_data_pagamento) 
						returning id into :id ", EsquemaBanco);

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", consumo.Atividade, DbType.Int32);
						comando.AdicionarParametroEntrada("possui_licenca", consumo.PossuiLicencaAmbiental, DbType.Int32);
						comando.AdicionarParametroEntrada("dua_numero", DbType.String, 40, consumo.DUANumero);
						comando.AdicionarParametroEntrada("dua_valor", consumo.DUAValor, DbType.Decimal);
						comando.AdicionarParametroEntrada("dua_data_pagamento", consumo.DUADataPagamento.Data, DbType.DateTime);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						consumo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Título

						if (consumo.Licenca != null)
						{
							comando = bancoDeDados.CriarComando(@"insert into {0}crt_reg_ati_flo_con_tit (
							id, caracterizacao, consumo, emitido_idaf, titulo, titulo_numero, titulo_modelo, titulo_modelo_texto, data_validade, 
							protocolo_numero, data_renovacao, protocolo_numero_ren, orgao_expedicao, tid) values 
							(seq_crt_reg_ati_flo_con_tit.nextval, :caracterizacao, :consumo, :emitido_idaf, :titulo, :titulo_numero, :titulo_modelo, :titulo_modelo_texto, :data_validade, 
							:protocolo_numero, :data_renovacao, :protocolo_numero_ren, :orgao_expedicao, :tid) returning id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("consumo", consumo.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("emitido_idaf", (consumo.Licenca.EmitidoPorInterno.Value ? 1 : 0), DbType.Int32);
							comando.AdicionarParametroEntrada("titulo", (consumo.Licenca.TituloId.HasValue && consumo.Licenca.TituloId > 0) ? consumo.Licenca.TituloId : null, DbType.Int32);
							comando.AdicionarParametroEntrada("titulo_numero", DbType.String, 20, consumo.Licenca.TituloNumero);
							comando.AdicionarParametroEntrada("titulo_modelo", consumo.Licenca.TituloModelo, DbType.Int32);
							comando.AdicionarParametroEntrada("titulo_modelo_texto", DbType.String, 100, consumo.Licenca.TituloModeloTexto);
							comando.AdicionarParametroEntrada("data_validade", consumo.Licenca.TituloValidadeData.Data, DbType.DateTime);
							comando.AdicionarParametroEntrada("protocolo_numero", DbType.String, 20, consumo.Licenca.ProtocoloNumero);
							comando.AdicionarParametroEntrada("data_renovacao", consumo.Licenca.ProtocoloRenovacaoData.Data, DbType.DateTime);
							comando.AdicionarParametroEntrada("protocolo_numero_ren", DbType.String, 20, consumo.Licenca.ProtocoloRenovacaoNumero);
							comando.AdicionarParametroEntrada("orgao_expedicao", DbType.String, 150, consumo.Licenca.OrgaoExpedidor);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							comando.AdicionarParametroSaida("id", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							consumo.Licenca.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
						}

						#endregion

						#region Fontes Energia

						if (consumo.FontesEnergia != null && consumo.FontesEnergia.Count > 0)
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_reg_ati_flo_fonte (id, caracterizacao, consumo, tipo, unidade, qde, qde_floresta_plantada, 
							qde_floresta_nativa, qde_outro_estado, tid) values 
							(seq_crt_reg_ati_flo_fonte.nextval, :caracterizacao, :consumo, :tipo, :unidade, :qde, :qde_floresta_plantada, 
							:qde_floresta_nativa, :qde_outro_estado, :tid) returning id into :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("consumo", consumo.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("tipo", DbType.Int32);
							comando.AdicionarParametroEntrada("unidade", DbType.Int32);
							comando.AdicionarParametroEntrada("qde", DbType.Decimal);
							comando.AdicionarParametroEntrada("qde_floresta_plantada", DbType.Decimal);
							comando.AdicionarParametroEntrada("qde_floresta_nativa", DbType.Decimal);
							comando.AdicionarParametroEntrada("qde_outro_estado", DbType.Decimal);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							comando.AdicionarParametroSaida("id", DbType.Int32);

							foreach (FonteEnergia fonteEnergia in consumo.FontesEnergia)
							{
								comando.SetarValorParametro("tipo", fonteEnergia.TipoId);
								comando.SetarValorParametro("unidade", fonteEnergia.UnidadeId);
								comando.SetarValorParametro("qde", fonteEnergia.Qde);
								comando.SetarValorParametro("qde_floresta_plantada", fonteEnergia.QdeFlorestaPlantada);
								comando.SetarValorParametro("qde_floresta_nativa", fonteEnergia.QdeFlorestaNativa);
								comando.SetarValorParametro("qde_outro_estado", fonteEnergia.QdeOutroEstado);

								bancoDeDados.ExecutarNonQuery(comando);

								fonteEnergia.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
							}
						}

						#endregion
					}
				}

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.registroatividadeflorestal, eHistoricoAcao.criar, bancoDeDados, null);

				bancoDeDados.Commit();

				return caracterizacao.Id;
			}
		}

		internal void Editar(RegistroAtividadeFlorestal caracterizacao, BancoDeDados banco = null)
		{
			int numero = 0;
			bool gerarNumero = !caracterizacao.PossuiNumero.Value && !int.TryParse(caracterizacao.NumeroRegistro, out numero);

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Registro Atividade Florestal

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update {0}crt_reg_atividade_florestal t
					set t.empreendimento  = :empreendimento,
						t.numero_registro = " + (gerarNumero ? @"{0}reg_atv_florestal_numero.nextval," : @":numero_registro,") + @"
						t.tid             = :tid,
						t.possui_numero   = :possui_numero
					where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("empreendimento", caracterizacao.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_numero", caracterizacao.PossuiNumero, DbType.Int32);

				if (!gerarNumero)
				{
					comando.AdicionarParametroEntrada("numero_registro", caracterizacao.NumeroRegistro, DbType.String);
				}

				comando.AdicionarParametroEntrada("id", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_reg_ati_flo_fonte t where t.caracterizacao = :caracterizacao ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "t.id", DbType.Int32, caracterizacao.Consumos.SelectMany(x => x.FontesEnergia).Select(y => y.Id).ToList());
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from {0}crt_reg_ati_flo_con_tit t where t.caracterizacao = :caracterizacao ", EsquemaBanco);
				comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "t.id", DbType.Int32, caracterizacao.Consumos.Select(x => (x.Licenca ?? new FinalidadeCaracterizacao()).Id).ToList());
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando("delete from {0}crt_reg_ati_flo_consumo t where t.caracterizacao = :caracterizacao ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "t.id", DbType.Int32, caracterizacao.Consumos.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Consumos

				if (caracterizacao.Consumos != null && caracterizacao.Consumos.Count > 0)
				{
					foreach (Consumo consumo in caracterizacao.Consumos)
					{
						if (consumo.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update {0}crt_reg_ati_flo_consumo t set 
							t.caracterizacao = :caracterizacao, t.atividade = :atividade, t.possui_licenca = :possui_licenca, t.dua_numero = :dua_numero, t.dua_valor = :dua_valor, 
							t.dua_data_pagamento = :dua_data_pagamento, t.tid = :tid where t.id = :consumo returning t.id into :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("consumo", consumo.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}crt_reg_ati_flo_consumo (id, tid, caracterizacao, atividade, possui_licenca, dua_numero, dua_valor, dua_data_pagamento) 
							values (seq_crt_reg_ati_flo_consumo.nextval, :tid, :caracterizacao, :atividade, :possui_licenca, :dua_numero, :dua_valor, :dua_data_pagamento) 
							returning id into :id ", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("atividade", consumo.Atividade, DbType.Int32);
						comando.AdicionarParametroEntrada("possui_licenca", consumo.PossuiLicencaAmbiental, DbType.Int32);
						comando.AdicionarParametroEntrada("dua_numero", DbType.String, 40, consumo.DUANumero);
						comando.AdicionarParametroEntrada("dua_valor", consumo.DUAValor, DbType.Decimal);
						comando.AdicionarParametroEntrada("dua_data_pagamento", consumo.DUADataPagamento.Data, DbType.DateTime);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarNonQuery(comando);

						consumo.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

						#region Título

						if (consumo.Licenca != null)
						{
							if (consumo.Licenca.Id > 0)
							{
								comando = bancoDeDados.CriarComando(@"update {0}crt_reg_ati_flo_con_tit t set 
								t.emitido_idaf = :emitido_idaf, t.titulo = :titulo, t.titulo_numero = :titulo_numero, t.titulo_modelo = :titulo_modelo, 
								t.titulo_modelo_texto = :titulo_modelo_texto, t.data_validade = :data_validade, t.protocolo_numero = :protocolo_numero, 
								t.data_renovacao = :data_renovacao, t.protocolo_numero_ren = :protocolo_numero_ren, t.orgao_expedicao = :orgao_expedicao, 
								t.tid = :tid where t.id = :id returning t.id into :idLicenca ", EsquemaBanco);

								comando.AdicionarParametroEntrada("id", consumo.Licenca.Id, DbType.Int32);
							}
							else
							{
								comando = bancoDeDados.CriarComando(@"insert into {0}crt_reg_ati_flo_con_tit (
								id, caracterizacao, consumo, emitido_idaf, titulo, titulo_numero, titulo_modelo, titulo_modelo_texto, data_validade, 
								protocolo_numero, data_renovacao, protocolo_numero_ren, orgao_expedicao, tid) values 
								(seq_crt_reg_ati_flo_con_tit.nextval, :caracterizacao, :consumo, :emitido_idaf, :titulo, :titulo_numero, :titulo_modelo, :titulo_modelo_texto, :data_validade, 
								:protocolo_numero, :data_renovacao, :protocolo_numero_ren, :orgao_expedicao, :tid) returning id into :idLicenca", EsquemaBanco);

								comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("consumo", consumo.Id, DbType.Int32);
							}

							comando.AdicionarParametroEntrada("emitido_idaf", (consumo.Licenca.EmitidoPorInterno.Value ? 1 : 0), DbType.Int32);
							comando.AdicionarParametroEntrada("titulo", (consumo.Licenca.TituloId.HasValue && consumo.Licenca.TituloId > 0) ? consumo.Licenca.TituloId : null, DbType.Int32);
							comando.AdicionarParametroEntrada("titulo_numero", DbType.String, 20, consumo.Licenca.TituloNumero);
							comando.AdicionarParametroEntrada("titulo_modelo", consumo.Licenca.TituloModelo, DbType.Int32);
							comando.AdicionarParametroEntrada("titulo_modelo_texto", DbType.String, 100, consumo.Licenca.TituloModeloTexto);
							comando.AdicionarParametroEntrada("data_validade", consumo.Licenca.TituloValidadeData.Data, DbType.DateTime);
							comando.AdicionarParametroEntrada("protocolo_numero", DbType.String, 20, consumo.Licenca.ProtocoloNumero);
							comando.AdicionarParametroEntrada("data_renovacao", consumo.Licenca.ProtocoloRenovacaoData.Data, DbType.DateTime);
							comando.AdicionarParametroEntrada("protocolo_numero_ren", DbType.String, 20, consumo.Licenca.ProtocoloRenovacaoNumero);
							comando.AdicionarParametroEntrada("orgao_expedicao", DbType.String, 150, consumo.Licenca.OrgaoExpedidor);
							comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
							comando.AdicionarParametroSaida("idLicenca", DbType.Int32);

							bancoDeDados.ExecutarNonQuery(comando);

							consumo.Licenca.Id = Convert.ToInt32(comando.ObterValorParametro("idLicenca"));
						}

						#endregion

						#region Fontes Energia

						if (consumo.FontesEnergia != null && consumo.FontesEnergia.Count > 0)
						{
							foreach (FonteEnergia fonteEnergia in consumo.FontesEnergia)
							{
								if (fonteEnergia.Id > 0)
								{
									comando = bancoDeDados.CriarComando(@"update {0}crt_reg_ati_flo_fonte t set t.caracterizacao = :caracterizacao, t.consumo = :consumo, t.tipo =
									:tipo, t.unidade = :unidade, t.qde = :qde, t.qde_floresta_plantada = :qde_floresta_plantada, t.qde_floresta_nativa = :qde_floresta_nativa, 
									t.qde_outro_estado = :qde_outro_estado, t.tid = :tid where t.id = :id returning id into :idFonte ", EsquemaBanco);

									comando.AdicionarParametroEntrada("id", fonteEnergia.Id, DbType.Int32);
								}
								else
								{
									comando = bancoDeDados.CriarComando(@" insert into {0}crt_reg_ati_flo_fonte (id, caracterizacao, consumo, tipo, unidade, qde, qde_floresta_plantada, 
									qde_floresta_nativa, qde_outro_estado, tid) values (seq_crt_reg_ati_flo_fonte.nextval, :caracterizacao, :consumo, :tipo, :unidade, :qde, 
									:qde_floresta_plantada, :qde_floresta_nativa, :qde_outro_estado, :tid) returning id into :idFonte ", EsquemaBanco);
								}

								comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("consumo", consumo.Id, DbType.Int32);
								comando.AdicionarParametroEntrada("tipo", fonteEnergia.TipoId, DbType.Int32);
								comando.AdicionarParametroEntrada("unidade", fonteEnergia.UnidadeId, DbType.Int32);
								comando.AdicionarParametroEntrada("qde", fonteEnergia.Qde, DbType.Decimal);
								comando.AdicionarParametroEntrada("qde_floresta_plantada", fonteEnergia.QdeFlorestaPlantada, DbType.Decimal);
								comando.AdicionarParametroEntrada("qde_floresta_nativa", fonteEnergia.QdeFlorestaNativa, DbType.Decimal);
								comando.AdicionarParametroEntrada("qde_outro_estado", fonteEnergia.QdeOutroEstado, DbType.Decimal);
								comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
								comando.AdicionarParametroSaida("idFonte", DbType.Int32);

								bancoDeDados.ExecutarNonQuery(comando);

								fonteEnergia.Id = Convert.ToInt32(comando.ObterValorParametro("idFonte"));
							}
						}

						#endregion
					}
				}

				#endregion

				Historico.Gerar(caracterizacao.Id, eHistoricoArtefatoCaracterizacao.registroatividadeflorestal, eHistoricoAcao.atualizar, bancoDeDados, null);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int empreendimento, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Obter id da caracterização

				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}crt_reg_atividade_florestal c where c.empreendimento = :empreendimento", EsquemaBanco);
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
				comando = bancoDeDados.CriarComando(@"update {0}crt_reg_atividade_florestal c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefatoCaracterizacao.registroatividadeflorestal, eHistoricoAcao.excluir, bancoDeDados, null);

				#endregion

				#region Apaga os dados da caracterização

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin 
					delete from {0}crt_reg_ati_flo_con_tit t where t.caracterizacao = :caracterizacao;
					delete from {0}crt_reg_ati_flo_fonte t where t.caracterizacao = :caracterizacao;
					delete from {0}crt_reg_ati_flo_consumo t where t.caracterizacao = :caracterizacao;
					delete from {0}crt_reg_atividade_florestal t where t.id = :caracterizacao;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter

		internal RegistroAtividadeFlorestal ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null, bool historico = false)
		{
			RegistroAtividadeFlorestal caracterizacao = new RegistroAtividadeFlorestal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.tid from {0}crt_reg_atividade_florestal s where s.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.Tid = reader.GetValue<string>("tid");
					}
				}

				if (caracterizacao.Id > 0)
				{
					caracterizacao = historico ? ObterHistorico(caracterizacao.Id, bancoDeDados, caracterizacao.Tid) : Obter(caracterizacao.Id, bancoDeDados, simplificado);
				}
			}

			return caracterizacao;

		}

		internal RegistroAtividadeFlorestal Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			RegistroAtividadeFlorestal caracterizacao = new RegistroAtividadeFlorestal();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Registro Atividade Florestal

				Comando comando = bancoDeDados.CriarComando(@"select c.empreendimento, c.numero_registro, c.tid, c.possui_numero 
				from {0}crt_reg_atividade_florestal c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						caracterizacao.NumeroRegistro = reader.GetValue<string>("numero_registro");
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.PossuiNumero = reader.GetValue<bool>("possui_numero");
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Consumos

				comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.atividade, a.atividade atividade_nome, a.cod_categoria, t.possui_licenca, t.dua_numero, t.dua_valor, t.dua_data_pagamento 
				from {0}crt_reg_ati_flo_consumo t, {0}tab_atividade a where t.atividade = a.id and t.caracterizacao = :caracterizacao order by t.id", EsquemaBanco);
				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Consumo consumo;
					while (reader.Read())
					{
						consumo = new Consumo();
						consumo.Id = reader.GetValue<int>("id");
						consumo.Tid = reader.GetValue<string>("tid");
						consumo.Atividade = reader.GetValue<int>("atividade");
						consumo.AtividadeNome = reader.GetValue<string>("atividade_nome");
						consumo.AtividadeCategoria = reader.GetValue<string>("cod_categoria");
						consumo.PossuiLicencaAmbiental = reader.GetValue<int?>("possui_licenca");
						consumo.DUANumero = reader.GetValue<string>("dua_numero");
						consumo.DUAValor = reader.GetValue<string>("dua_valor");
						consumo.DUADataPagamento.DataTexto = reader.GetValue<string>("dua_data_pagamento");

						#region Fonte energias

						comando = bancoDeDados.CriarComando(@" select t.id, t.caracterizacao, t.tipo, f.texto tipo_texto, t.unidade, u.texto unidade_texto, t.qde, t.qde_floresta_plantada, 
						t.qde_floresta_nativa, t.qde_outro_estado, t.tid from {0}crt_reg_ati_flo_fonte t, {0}lov_crt_reg_ati_flo_fonte f, {0}lov_crt_reg_ati_flo_unidade u where t.consumo = :consumo
						and t.tipo = f.id and t.unidade = u.id order by t.id ", EsquemaBanco);

						comando.AdicionarParametroEntrada("consumo", consumo.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							FonteEnergia fonteEnergia;
							while (readerAux.Read())
							{
								fonteEnergia = new FonteEnergia();

								fonteEnergia.Id = readerAux.GetValue<int>("id");
								fonteEnergia.TipoId = readerAux.GetValue<int>("tipo");
								fonteEnergia.TipoTexto = readerAux.GetValue<string>("tipo_texto");
								fonteEnergia.UnidadeId = readerAux.GetValue<int>("unidade");
								fonteEnergia.UnidadeTexto = readerAux.GetValue<string>("unidade_texto");
								fonteEnergia.Qde = readerAux.GetValue<string>("qde");
								fonteEnergia.QdeFlorestaPlantada = readerAux.GetValue<string>("qde_floresta_plantada");
								fonteEnergia.QdeFlorestaNativa = readerAux.GetValue<string>("qde_floresta_nativa");
								fonteEnergia.QdeOutroEstado = readerAux.GetValue<string>("qde_outro_estado");
								fonteEnergia.Tid = readerAux.GetValue<string>("tid");

								consumo.FontesEnergia.Add(fonteEnergia);
							}

							readerAux.Close();
						}

						#endregion

						#region Título

						comando = bancoDeDados.CriarComando(@"select ti.id, ti.emitido_idaf, ti.titulo, ti.titulo_numero, ti.titulo_modelo, 
						ti.titulo_modelo_texto, ti.data_validade, ti.protocolo_numero, ti.data_renovacao, ti.protocolo_numero_ren, ti.orgao_expedicao
						from crt_reg_ati_flo_con_tit ti where ti.consumo = :consumo", EsquemaBanco);

						comando.AdicionarParametroEntrada("consumo", consumo.Id, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								consumo.Licenca.Id = readerAux.GetValue<int>("id");
								consumo.Licenca.EmitidoPorInterno = readerAux.GetValue<int>("emitido_idaf") == 1;
								consumo.Licenca.TituloId = readerAux.GetValue<int?>("titulo");
								consumo.Licenca.TituloNumero = readerAux.GetValue<string>("titulo_numero");
								consumo.Licenca.TituloModelo = readerAux.GetValue<int?>("titulo_modelo");
								consumo.Licenca.TituloModeloTexto = readerAux.GetValue<string>("titulo_modelo_texto");
								consumo.Licenca.TituloValidadeData.DataTexto = readerAux.GetValue<string>("data_validade");
								consumo.Licenca.ProtocoloNumero = readerAux.GetValue<string>("protocolo_numero");
								consumo.Licenca.ProtocoloRenovacaoData.DataTexto = readerAux.GetValue<string>("data_renovacao");
								consumo.Licenca.ProtocoloRenovacaoNumero = readerAux.GetValue<string>("protocolo_numero_ren");
								consumo.Licenca.OrgaoExpedidor = readerAux.GetValue<string>("orgao_expedicao");
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.Consumos.Add(consumo);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		private RegistroAtividadeFlorestal ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			RegistroAtividadeFlorestal caracterizacao = new RegistroAtividadeFlorestal();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Registro Atividade Florestal

				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.empreendimento_id, c.numero_registro, c.tid, c.possui_numero 
				from {0}hst_crt_reg_ativida_florestal c where c.caracterizacao = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento_id");
						caracterizacao.NumeroRegistro = reader.GetValue<string>("numero_registro");
						caracterizacao.Tid = reader.GetValue<string>("tid");
						caracterizacao.PossuiNumero = reader.GetValue<bool>("possui_numero");
					}

					reader.Close();
				}

				#endregion

				if (caracterizacao.Id <= 0 || simplificado)
				{
					return caracterizacao;
				}

				#region Consumos

				comando = bancoDeDados.CriarComando(@"
				select t.id, t.consumo consumo_id, t.tid, t.atividade_id atividade, a.atividade atividade_nome, 
				a.cod_categoria, t.possui_licenca, t.possui_licenca, t.dua_numero, t.dua_valor, t.dua_data_pagamento 
				from {0}hst_crt_reg_ati_flo_consumo t, {0}hst_atividade a where t.atividade_id = a.atividade_id 
				and t.atividade_tid = a.tid and t.caracterizacao = :caracterizacao and t.tid = :tid 
				order by t.consumo", EsquemaBanco);

				comando.AdicionarParametroEntrada("caracterizacao", caracterizacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", caracterizacao.Tid, DbType.String);

				int hst_consumo;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Consumo consumo;
					while (reader.Read())
					{
						consumo = new Consumo();
						hst_consumo = reader.GetValue<int>("id");
						consumo.Id = reader.GetValue<int>("consumo_id");
						consumo.Tid = reader.GetValue<string>("tid");
						consumo.Atividade = reader.GetValue<int>("atividade");
						consumo.AtividadeNome = reader.GetValue<string>("atividade_nome");
						consumo.AtividadeCategoria = reader.GetValue<string>("cod_categoria");
						consumo.PossuiLicencaAmbiental = reader.GetValue<int?>("possui_licenca");
						consumo.DUANumero = reader.GetValue<string>("dua_numero");
						consumo.DUAValor = reader.GetValue<string>("dua_valor");
						consumo.DUADataPagamento.DataTexto = reader.GetValue<string>("dua_data_pagamento");

						#region Fonte energias

						comando = bancoDeDados.CriarComando(@"select t.id, t.fonte ,t.caracterizacao, t.tipo_id, t.tipo_texto, 
						t.unidade_id, t.unidade_texto, t.qde, t.qde_floresta_plantada, t.qde_floresta_nativa, t.qde_outro_estado, t.tid 
						from {0}hst_crt_reg_ati_flo_fonte t where t.id_hst_consumo = :id_hst", EsquemaBanco);

						comando.AdicionarParametroEntrada("id_hst", hst_consumo, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							FonteEnergia fonteEnergia;
							while (readerAux.Read())
							{
								fonteEnergia = new FonteEnergia();

								fonteEnergia.Id = readerAux.GetValue<int>("fonte");
								fonteEnergia.TipoId = readerAux.GetValue<int>("tipo_id");
								fonteEnergia.TipoTexto = readerAux.GetValue<string>("tipo_texto");
								fonteEnergia.UnidadeId = readerAux.GetValue<int>("unidade_id");
								fonteEnergia.UnidadeTexto = readerAux.GetValue<string>("unidade_texto");
								fonteEnergia.Qde = readerAux.GetValue<string>("qde");
								fonteEnergia.QdeFlorestaPlantada = readerAux.GetValue<string>("qde_floresta_plantada");
								fonteEnergia.QdeFlorestaNativa = readerAux.GetValue<string>("qde_floresta_nativa");
								fonteEnergia.QdeOutroEstado = readerAux.GetValue<string>("qde_outro_estado");
								fonteEnergia.Tid = readerAux.GetValue<string>("tid");

								consumo.FontesEnergia.Add(fonteEnergia);
							}

							readerAux.Close();
						}

						#endregion

						#region Título

						comando = bancoDeDados.CriarComando(@"select ti.registro_ati_con_tit id, ti.emitido_idaf, ti.titulo_id, ti.titulo_numero, ti.titulo_modelo_id, 
						ti.titulo_modelo_texto, ti.data_validade, ti.protocolo_numero, ti.data_renovacao, ti.protocolo_numero_ren, ti.orgao_expedicao
						from hst_crt_reg_ati_flo_con_tit ti where ti.id_hst_consumo = :id_hst", EsquemaBanco);

						comando.AdicionarParametroEntrada("id_hst", hst_consumo, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								consumo.Licenca.Id = readerAux.GetValue<int>("id");
								consumo.Licenca.EmitidoPorInterno = readerAux.GetValue<int>("emitido_idaf") == 1;
								consumo.Licenca.TituloId = readerAux.GetValue<int?>("titulo_id");
								consumo.Licenca.TituloNumero = readerAux.GetValue<string>("titulo_numero");
								consumo.Licenca.TituloModelo = readerAux.GetValue<int?>("titulo_modelo_id");
								consumo.Licenca.TituloModeloTexto = readerAux.GetValue<string>("titulo_modelo_texto");
								consumo.Licenca.TituloValidadeData.DataTexto = readerAux.GetValue<string>("data_validade");
								consumo.Licenca.ProtocoloNumero = readerAux.GetValue<string>("protocolo_numero");
								consumo.Licenca.ProtocoloRenovacaoData.DataTexto = readerAux.GetValue<string>("data_renovacao");
								consumo.Licenca.ProtocoloRenovacaoNumero = readerAux.GetValue<string>("protocolo_numero_ren");
								consumo.Licenca.OrgaoExpedidor = readerAux.GetValue<string>("orgao_expedicao");
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.Consumos.Add(consumo);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal object ObterDadosPdfTitulo(int empreendimento, int atividade, IEspecificidade especificidade, BancoDeDados banco)
		{
			return null;
		}

		internal List<int> ObterAtividadesCaracterizacao(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.atividade from crt_reg_ati_flo_consumo c, crt_reg_atividade_florestal r 
				where r.empreendimento = :empreendimento and c.caracterizacao = r.id", EsquemaBanco);//5-Encerrado

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return bancoDeDados.ExecutarList<Int32>(comando);
			}
		}

		internal AtividadeSolicitada ObterAtividadeSolicitada(int atividadeSolicitadaId, BancoDeDados banco = null)
		{
			AtividadeSolicitada atividade = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id, atividade, situacao, cod_categoria from tab_atividade ta where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", atividadeSolicitadaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						atividade = new AtividadeSolicitada();
						atividade.Id = reader.GetValue<int>("id");
						atividade.Texto = reader.GetValue<string>("atividade");
						atividade.Situacao = reader.GetValue<bool>("situacao");
						atividade.Categoria = reader.GetValue<string>("cod_categoria");
					}
				}

				return atividade;
			}
		}

		internal List<TituloModeloLst> ObterModelosLista(BancoDeDados banco = null)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.nome from tab_titulo_modelo t where t.situacao = 1 and t.codigo in (12, 15, 16) order by t.nome");

				IEnumerable<IDataReader> daReader = Tecnomapas.Blocos.Etx.ModuloCore.Data.DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new TituloModeloLst()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["nome"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		#endregion

		#region Validações

		internal bool IsNumeroUtilizado(string numero, int empId)
		{
			bool isUtilizado = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{

				Comando comando = bancoDeDados.CriarComando(@"select 1 from {0}crt_reg_atividade_florestal t 
				where to_char(t.numero_registro) = :numero_registro and t.empreendimento != :empreendimento ", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero_registro", numero, DbType.String);
				comando.AdicionarParametroEntrada("empreendimento", empId, DbType.Int32);

				object resultado = bancoDeDados.ExecutarScalar(comando);

				isUtilizado = (resultado != null && !Convert.IsDBNull(resultado) && !string.IsNullOrWhiteSpace(resultado.ToString()));
			}

			return isUtilizado;
		}

		#endregion
	}
}