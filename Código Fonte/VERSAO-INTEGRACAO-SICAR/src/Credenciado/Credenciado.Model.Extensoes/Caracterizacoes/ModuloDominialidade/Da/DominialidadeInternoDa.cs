using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloDominialidade.Da
{
	class DominialidadeInternoDa
	{
		#region Propriedades

		private String EsquemaBanco { get; set; }

		#endregion

		public DominialidadeInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		internal Dominialidade ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select s.id from {0}crt_dominialidade s where s.empreendimento = :empreendimento", EsquemaBanco);
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
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste from {0}crt_dominialidade_dominio d, {0}lov_crt_domin_dominio_tipo ldt, 
				{0}lov_crt_domin_comprovacao ldc where d.tipo = ldt.id and d.comprovacao = ldc.id(+) and d.dominialidade = :id", EsquemaBanco);

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

								if (reserva.IdentificacaoARLCedente > 0)
								{
									ReservaLegal reservaAux = ObterARLPorId(reserva.IdentificacaoARLCedente);
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

		#endregion

		internal List<Lista> ObterDominiosLista(int empreendimentoInternoId, bool somenteMatriculas, BancoDeDados banco = null)
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

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoInternoId, DbType.Int32);
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

		internal List<ReservaLegal> ObterRLsCompensacao(int projetoGeoInternoId, BancoDeDados banco = null)
		{
			List<ReservaLegal> retorno = new List<ReservaLegal>(); ;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id, situacao, localizacao, identificacao, situacao_vegetal, arl_croqui, arl_documento, numero_termo, cartorio, 
				matricula, tid, nome_cartorio, numero_folha, numero_livro, matricula_numero, averbacao_numero, arl_recebida, emp_compensacao, cedente_possui_emp, arl_cedida, 
				arl_cedente, cedente_receptor   from crt_dominialidade_reserva r where dominio in (select id from crt_dominialidade_dominio where dominialidade 
				in(select id from crt_dominialidade where empreendimento = (select empreendimento from crt_projeto_geo where id = :id))) 
				and r.emp_compensacao is not null and r.compensada = 1", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", projetoGeoInternoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ReservaLegal reserva = null;
					while (reader.Read())
					{
						reserva = new ReservaLegal();
						reserva.Id = reader.GetValue<int>("id");
						reserva.SituacaoId = reader.GetValue<int>("situacao");
						reserva.LocalizacaoId = reader.GetValue<int>("localizacao");
						reserva.Identificacao = reader.GetValue<string>("identificacao");
						reserva.SituacaoVegetalGeo = reader.GetValue<string>("situacao_vegetal");
						reserva.ARLCroqui = reader.GetValue<decimal>("arl_croqui");
						reserva.NumeroTermo = reader.GetValue<string>("numero_termo");
						reserva.IdentificacaoARLCedente = reader.GetValue<int>("arl_cedente");
						retorno.Add(reserva);
					}
				}
			}

			return retorno;
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

	}
}