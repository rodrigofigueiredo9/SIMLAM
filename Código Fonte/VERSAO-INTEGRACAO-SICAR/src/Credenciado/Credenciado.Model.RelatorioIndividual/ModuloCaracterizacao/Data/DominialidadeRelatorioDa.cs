using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data
{
	public class DominialidadeRelatorioDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		private string EsquemaBanco { get; set; }

		#endregion

		public DominialidadeRelatorioDa()
		{
			EsquemaBanco = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
		}

		internal Dominialidade Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Dominialidade

				if (tid == null)
				{
					caracterizacao = Obter(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_dominialidade s where s.id = :id and s.tid = :tid", EsquemaBanco);

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
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
						caracterizacao.EmpreendimentoId = Convert.ToInt32(reader["empreendimento"]);
						caracterizacao.EmpreendimentoLocalizacao = Convert.ToInt32(reader["empreendimento_localizacao"]);
						caracterizacao.PossuiAreaExcedenteMatricula = reader.GetValue<int?>("possui_area_exced_matri");
						caracterizacao.ConfrontacaoNorte = reader["confrontante_norte"].ToString();
						caracterizacao.ConfrontacaoSul = reader["confrontante_sul"].ToString();
						caracterizacao.ConfrontacaoLeste = reader["confrontante_leste"].ToString();
						caracterizacao.ConfrontacaoOeste = reader["confrontante_oeste"].ToString();
						caracterizacao.Tid = reader["tid"].ToString();
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
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();
						item.Tipo = Convert.ToInt32(reader["tipo"]);
						item.TipoTexto = reader["tipo_texto"].ToString();
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
						dominio.Id = Convert.ToInt32(reader["id"]);
						dominio.Tid = reader["tid"].ToString();
						dominio.Identificacao = reader["identificacao"].ToString();
						dominio.Tipo = (eDominioTipo)Convert.ToInt32(reader["tipo"]);
						dominio.TipoTexto = reader["tipo_texto"].ToString();
						dominio.Matricula = reader["matricula"].ToString();
						dominio.Folha = reader["folha"].ToString();
						dominio.Livro = reader["livro"].ToString();
						dominio.Cartorio = reader["cartorio"].ToString();
						dominio.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						dominio.AreaDocumento = reader.GetValue<decimal>("area_documento");
						dominio.AreaDocumentoTexto = reader.GetValue<decimal>("area_documento").ToStringTrunc();
						dominio.EmpreendimentoLocalizacao = caracterizacao.EmpreendimentoLocalizacao;
						dominio.APPCroqui = reader.GetValue<decimal>("app_croqui");
						dominio.DescricaoComprovacao = reader["registro"].ToString(); //campo alterado
						dominio.AreaCCIR = reader.GetValue<decimal>("area_ccri");
						dominio.AreaCCIRTexto = reader.GetValue<decimal>("area_ccri").ToStringTrunc();
						dominio.DataUltimaAtualizacao.DataTexto = reader["data_ultima_atualizacao"].ToString();
						dominio.ARLDocumento = reader.GetValue<decimal?>("arl_documento");
						dominio.ARLDocumentoTexto = reader.GetValue<decimal?>("arl_documento").ToStringTrunc();
						dominio.ConfrontacaoNorte = reader["confrontante_norte"].ToString();
						dominio.ConfrontacaoSul = reader["confrontante_sul"].ToString();
						dominio.ConfrontacaoLeste = reader["confrontante_leste"].ToString();
						dominio.ConfrontacaoOeste = reader["confrontante_oeste"].ToString();

						if (reader["comprovacao"] != null && !Convert.IsDBNull(reader["comprovacao"]))
						{
							dominio.ComprovacaoId = Convert.ToInt32(reader["comprovacao"]);
							dominio.ComprovacaoTexto = reader["comprovacao_texto"].ToString();
						}

						if (reader["numero_ccri"] != null && !Convert.IsDBNull(reader["numero_ccri"]))
						{
							dominio.NumeroCCIR = Convert.ToInt64(reader["numero_ccri"]);
						}

						#region Reservas Legais

						comando = bancoDeDados.CriarComando(@"select r.id, r.situacao, lrs.texto situacao_texto, r.arl_cedida, r.arl_recebida, r.localizacao, lrl.texto localizacao_texto, r.identificacao, r.situacao_vegetal, 
							lrsv.texto situacao_vegetal_texto, r.arl_croqui, r.arl_documento, r.numero_termo, r.cartorio, lrc.texto cartorio_texto, r.matricula, d.identificacao matricula_identificacao, 
							r.compensada, r.numero_cartorio, r.nome_cartorio, r.numero_livro, r.numero_folha, r.tid from {0}crt_dominialidade_reserva r, {0}crt_dominialidade_dominio d, {0}lov_crt_domin_reserva_situacao lrs,
							{0}lov_crt_domin_reserva_local lrl, {0}lov_crt_domin_reserva_sit_veg lrsv, {0}lov_crt_domin_reserva_cartorio lrc where r.matricula = d.id(+) and r.situacao = lrs.id and r.localizacao 
							= lrl.id(+) and r.situacao_vegetal = lrsv.id(+) and r.cartorio = lrc.id(+) and r.dominio = :dominio", EsquemaBanco);

						comando.AdicionarParametroEntrada("dominio", dominio.Id, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ReservaLegal reserva = null;

							while (readerAux.Read())
							{
								reserva = new ReservaLegal();
								reserva.Id = Convert.ToInt32(readerAux["id"]);
								reserva.SituacaoId = Convert.ToInt32(readerAux["situacao"]);
								reserva.SituacaoTexto = readerAux["situacao_texto"].ToString();
								reserva.Identificacao = readerAux["identificacao"].ToString();
								reserva.Compensada = Convert.ToBoolean(readerAux["compensada"]);
								reserva.ARLCedida = readerAux.GetValue<decimal>("arl_cedida");
								reserva.ARLRecebida = readerAux.GetValue<decimal>("arl_recebida");

								if (readerAux["localizacao"] != null && !Convert.IsDBNull(readerAux["localizacao"]))
								{
									reserva.LocalizacaoId = Convert.ToInt32(readerAux["localizacao"]);
									reserva.LocalizacaoTexto = readerAux["localizacao_texto"].ToString();
								}

								if (readerAux["situacao_vegetal"] != null && !Convert.IsDBNull(readerAux["situacao_vegetal"]))
								{
									reserva.SituacaoVegetalId = Convert.ToInt32(readerAux["situacao_vegetal"]);
									reserva.SituacaoVegetalTexto = readerAux["situacao_vegetal_texto"].ToString();
								}

								reserva.ARLCroqui = readerAux.GetValue<decimal>("arl_croqui");
								reserva.NumeroTermo = readerAux["numero_termo"].ToString();
								reserva.Tid = readerAux["tid"].ToString();

								if (readerAux["cartorio"] != null && !Convert.IsDBNull(readerAux["cartorio"]))
								{
									reserva.TipoCartorioId = Convert.ToInt32(readerAux["cartorio"]);
									reserva.TipoCartorioTexto = readerAux["cartorio_texto"].ToString();
								}

								if (readerAux["matricula"] != null && !Convert.IsDBNull(readerAux["matricula"]))
								{
									reserva.MatriculaId = Convert.ToInt32(readerAux["matricula"]);
									reserva.MatriculaIdentificacao = readerAux["matricula_identificacao"].ToString();
								}

								reserva.NumeroCartorio = readerAux["numero_cartorio"].ToString();
								reserva.NomeCartorio = readerAux["nome_cartorio"].ToString();
								reserva.NumeroFolha = readerAux["numero_folha"].ToString();
								reserva.NumeroLivro = readerAux["numero_livro"].ToString();

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.id, d.empreendimento_id, d.possui_area_exced_matri, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, d.tid 
				from {0}hst_crt_dominialidade d where d.dominialidade_id = :id and d.tid = :tid", EsquemaBanco);

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
				from {0}hst_crt_dominialidade_areas a where a.id_hst = :id", EsquemaBanco);

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
				d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste from {0}hst_crt_dominialidade_dominio d where d.id_hst = :id", EsquemaBanco);

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

						comando = bancoDeDados.CriarComando(@"select r.dominialidade_reserva_id, r.situacao_id, r.situacao_texto, r.localizacao_id, r.localizacao_texto, 
						r.identificacao, r.situacao_vegetal_id, r.situacao_vegetal_texto, r.arl_croqui, r.numero_termo, r.cartorio_id, r.cartorio_texto, r.matricula_id, 
						r.compensada, r.numero_cartorio, r.nome_cartorio, r.numero_folha, r.numero_livro, r.tid 
						from {0}hst_crt_dominialidade_reserva r where r.id_hst = :id", EsquemaBanco);

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
								reserva.MatriculaIdentificacao = dominio.Identificacao;
								reserva.NumeroCartorio = readerAux.GetValue<string>("numero_cartorio");
								reserva.NomeCartorio = readerAux.GetValue<string>("nome_cartorio");
								reserva.NumeroFolha = readerAux.GetValue<string>("numero_folha");
								reserva.NumeroLivro = readerAux.GetValue<string>("numero_livro");

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
	}
}