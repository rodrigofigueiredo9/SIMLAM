using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCaracterizacoes;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCaracterizacoes.Data
{
	class DominialidadeDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public DominialidadeDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		internal DominialidadeRelatorio ObterPorEmpreendimento(int empreendimento, bool simplificado = false, BancoDeDados banco = null)
		{
			DominialidadeRelatorio caracterizacao = new DominialidadeRelatorio();

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

		internal DominialidadeRelatorio Obter(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			DominialidadeRelatorio caracterizacao = new DominialidadeRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
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

		internal DominialidadeRelatorio Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			DominialidadeRelatorio caracterizacao = new DominialidadeRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.tid from {0}crt_dominialidade d where d.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
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

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.tipo Tipo, la.texto TipoTexto, a.valor Valor, a.tid Tid 
				from {0}crt_dominialidade_areas a, {0}lov_crt_dominialidade_area la where a.tipo = la.id and a.dominialidade = :dominialidade", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominialidade", id, DbType.Int32);

				caracterizacao.Areas= bancoDeDados.ObterEntityList<DominialidadeAreaRelatorio>(comando);

				#endregion

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select d.id, d.tid, d.tipo, ldt.texto tipo_texto, d.area_croqui, 
				d.area_documento, d.app_croqui, d.numero_ccri, d.area_ccri, d.arl_documento  from {0}crt_dominialidade_dominio d, {0}lov_crt_domin_dominio_tipo ldt 
				where d.tipo = ldt.id and d.dominialidade = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					DominioRelatorio dominio = null;

					while (reader.Read())
					{
						dominio = new DominioRelatorio();
						dominio.Id = reader.GetValue<int>("id");
						dominio.Tid = reader.GetValue<string>("tid");
						dominio.Tipo = reader.GetValue<int>("tipo");
						dominio.TipoTexto = reader.GetValue<string>("tipo_texto");
						dominio.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						dominio.AreaDocumento = reader.GetValue<decimal>("area_documento");
						dominio.APPCroqui = reader.GetValue<decimal>("app_croqui");
						dominio.NumeroCCIR = reader.GetValue<long>("numero_ccri");
						dominio.AreaCCIR = reader.GetValue<decimal>("area_ccri");
						dominio.ARLDocumento = reader.GetValue<decimal?>("arl_documento");

						#region Reservas Legais

						comando = bancoDeDados.CriarComando(@"select r.id, r.tid, r.situacao_vegetal, lrsv.texto situacao_vegetal_texto, r.arl_croqui 
							from {0}crt_dominialidade_reserva r, {0}lov_crt_domin_reserva_sit_veg lrsv where r.situacao_vegetal = lrsv.id(+) and r.dominio = :dominio", EsquemaBanco);

						comando.AdicionarParametroEntrada("dominio", dominio.Id, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ReservaLegalRelatorio reserva = null;

							while (readerAux.Read())
							{
								reserva = new ReservaLegalRelatorio();
								reserva.Id = readerAux.GetValue<int>("id");
								reserva.Tid = readerAux.GetValue<string>("tid");
								reserva.SituacaoVegetalId = readerAux.GetValue<int>("situacao_vegetal");
								reserva.SituacaoVegetalTexto = readerAux.GetValue<string>("situacao_vegetal_texto");
								reserva.ARLCroqui = readerAux.GetValue<decimal>("arl_croqui");
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

		private DominialidadeRelatorio ObterHistorico(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			DominialidadeRelatorio caracterizacao = new DominialidadeRelatorio();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.id,  d.tid from {0}hst_crt_dominialidade d where d.dominialidade_id = :id and d.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						hst = reader.GetValue<int>("id");

						caracterizacao.Id = id;
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

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.tipo Tipo, la.texto TipoTexto, a.valor Valor, a.tid Tid
				from {0}crt_dominialidade_areas a, {0}lov_crt_dominialidade_area la where a.tipo = la.id and a.dominialidade = :dominialidade", EsquemaBanco);

				comando.AdicionarParametroEntrada("dominialidade", id, DbType.Int32);

				caracterizacao.Areas = bancoDeDados.ObterEntityList<DominialidadeAreaRelatorio>(comando);

				#endregion

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select d.id id_hst, d.dominialidade_dominio_id id, d.tid, d.tipo_id, d.area_croqui,
				d.area_documento, d.app_croqui, d.numero_ccri, d.area_ccri, d.arl_documento from {0}hst_crt_dominialidade_dominio d where d.id_hst = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", hst, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					DominioRelatorio dominio = null;

					while (reader.Read())
					{
						hst = reader.GetValue<int>("id_hst");

						dominio = new DominioRelatorio();
						dominio.Id = reader.GetValue<int>("id");
						dominio.Tid = reader.GetValue<string>("tid");
						dominio.Tipo = reader.GetValue<int>("tipo_id");
						dominio.AreaCroqui = reader.GetValue<decimal>("area_croqui");
						dominio.AreaDocumento = reader.GetValue<decimal>("area_documento");
						dominio.APPCroqui = reader.GetValue<decimal>("app_croqui");
						dominio.NumeroCCIR = reader.GetValue<long>("numero_ccri");
						dominio.AreaCCIR = reader.GetValue<decimal>("area_ccri");
						dominio.ARLDocumento = reader.GetValue<decimal>("arl_documento");

						#region Reservas Legais

						comando = bancoDeDados.CriarComando(@"select r.dominialidade_reserva_id id, r.tid, r.situacao_vegetal_id, r.situacao_vegetal_texto, r.arl_croqui 
						from {0}hst_crt_dominialidade_reserva r where r.id_hst = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							ReservaLegalRelatorio reserva = null;

							while (readerAux.Read())
							{
								reserva = new ReservaLegalRelatorio();
								reserva.Id = readerAux.GetValue<int>("id");
								reserva.Tid = readerAux.GetValue<string>("tid");
								reserva.ARLCroqui = readerAux.GetValue<decimal>("arl_croqui");
								reserva.SituacaoVegetalId = readerAux.GetValue<int>("situacao_vegetal_id");
								reserva.SituacaoVegetalTexto = readerAux.GetValue<string>("situacao_vegetal_texto");


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

		#endregion
	}
}