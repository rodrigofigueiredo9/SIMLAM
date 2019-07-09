using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCaracterizacoes;
using Tecnomapas.Blocos.Entities.Etx.ModuloRelatorio.AsposeEtx;
using System.IO;
using Tecnomapas.Blocos.Arquivo.Data;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloRelatorio.AsposeEtx;
using Tecnomapas.EtramiteX.Scheduler.models;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade;

namespace Tecnomapas.EtramiteX.Scheduler.misc.Relatorio
{
	class RelatorioSolicitacao : PdfPadraoRelatorio
	{
		public RelatorioSolicitacao () { }

		public CARSolicitacaoRelatorio ObterHistoricoInstitucional(int id, BancoDeDados banco = null)
		{
			CARSolicitacaoRelatorio entidade = new CARSolicitacaoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select hcs.tid,
						   hcs.dominialidade_id,
						   hcs.dominialidade_tid,
						   hcs.numero,
						   hcs.data_emissao,
						   hcs.situacao_texto,
						   (select count(*)
							  from hst_empreendimento_responsavel her
							 where her.id_hst = he.id
							   and her.responsavel_id != hcs.declarante_id) +
						   (select count(*)
							  from hst_protocolo hp
							 where hp.id_protocolo = hcs.protocolo_id
							   and hp.tid = hcs.protocolo_tid
							   and hp.interessado_id != hcs.declarante_id) possui_outros,
						   hp.tipo declarante_tipo,
						   (select her.tipo_texto
							  from hst_empreendimento_responsavel her
							 where her.id_hst = he.id
							   and her.responsavel_id = hcs.declarante_id
							   and her.responsavel_tid = hcs.declarante_tid) declarante_tipo_texto,
						   nvl(hp.nome, hp.razao_social) declarante_nome_razao,
						   nvl(hp.cpf, hp.cnpj) declarante_cpf_cnpj,
						   hpe.cep declarante_cep,
						   hpe.logradouro declarante_logradouro,
						   hpe.bairro declarante_bairro,
						   hpe.municipio_texto declarante_municipio,
						   hpe.distrito declarante_distrito,
						   lem.sigla declarante_estado_sigla,
						   he.denominador empreendimento_denominador,
						   he.codigo empreendimento_codigo,
						   he.cnpj empreendimento_cnpj,
						   hee.correspondencia emp_endereco_correspondencia,
						   hee.cep empreendimento_cep,
						   hee.logradouro empreendimento_logradouro,
						   hee.bairro empreendimento_bairro,
						   hee.municipio_texto empreendimento_municipio,
						   hee.distrito empreendimento_distrito,
						   lee.sigla empreendimento_estado_sigla,
						   hec.tipo_coordenada_texto emp_coordenada_texto,
						   hec.datum_texto emp_datum_texto,
						   hec.northing_utm emp_northing,
						   hec.easting_utm emp_easting,
						   hec.fuso_utm emp_fuso,
						   hec.local_coleta_texto emp_local_coleta,
						   hec.forma_coleta_texto emp_forma_coleta,
						   hpt.requerimento_id,
						   (select sicar.codigo_imovel
							  from tab_controle_sicar sicar
							 where sicar.solicitacao_car = hcs.solicitacao_id
							   and sicar.solicitacao_car_esquema = 1) numero_sicar,
						   (select sicar.pendencias
							  from tab_controle_sicar sicar
							 where sicar.solicitacao_car = hcs.solicitacao_id
							   and sicar.solicitacao_car_esquema = 1) pendencias_sicar
					  from hst_car_solicitacao         hcs,
						   hst_pessoa                  hp,
						   hst_pessoa_endereco         hpe,
						   lov_estado                  lem,
						   hst_empreendimento          he,
						   hst_empreendimento_endereco hee,
						   lov_estado                  lee,
						   hst_empreendimento_coord    hec,
						   hst_protocolo			   hpt
					 where hp.pessoa_id = hcs.declarante_id
					   and hp.tid = hcs.declarante_tid
					   and hp.id = hpe.id_hst(+)
                       and hpe.estado_id = lem.id(+)
					   and he.empreendimento_id = hcs.empreendimento_id
					   and he.tid = hcs.empreendimento_tid
					   and he.id = hee.id_hst
					   and hee.correspondencia = 0
					   and lee.id = hee.estado_id
					   and hec.id_hst = he.id
					   and hcs.protocolo_selecionado_id = hpt.id_protocolo
					   and hcs.id = (select max(id)
									   from hst_car_solicitacao hcs1
									  where hcs1.solicitacao_id = hcs.solicitacao_id)
					   and hcs.solicitacao_id = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Numero = reader.GetValue<int>("numero");
						entidade.DataEmissao = reader.GetValue<string>("data_emissao");
						entidade.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						entidade.DominialidadeId = reader.GetValue<int>("dominialidade_id");
						entidade.DominialidadeTid = reader.GetValue<string>("dominialidade_tid");

						entidade.DeclarantePossuiOutros = reader.GetValue<int>("possui_outros") > 0;
						entidade.Declarante.Tipo = reader.GetValue<int>("declarante_tipo");
						entidade.Declarante.TipoTexto = reader.GetValue<string>("declarante_tipo_texto");
						entidade.Declarante.NomeRazaoSocial = reader.GetValue<string>("declarante_nome_razao");
						entidade.Declarante.CPFCNPJ = reader.GetValue<string>("declarante_cpf_cnpj");
						entidade.Declarante.Endereco.Cep = reader.GetValue<string>("declarante_cep");
						entidade.Declarante.Endereco.Logradouro = reader.GetValue<string>("declarante_logradouro");
						entidade.Declarante.Endereco.Bairro = reader.GetValue<string>("declarante_bairro");
						entidade.Declarante.Endereco.Distrito = reader.GetValue<string>("declarante_distrito");
						entidade.Declarante.Endereco.MunicipioTexto = reader.GetValue<string>("declarante_municipio");
						entidade.Declarante.Endereco.EstadoSigla = reader.GetValue<string>("declarante_estado_sigla");

						entidade.Empreendimento.NomeRazao = reader.GetValue<string>("empreendimento_denominador");
						entidade.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
						entidade.Empreendimento.CNPJ = reader.GetValue<string>("empreendimento_cnpj");

						entidade.Empreendimento.Enderecos.Add(new EnderecoRelatorio()
						{
							Correspondencia = reader.GetValue<int?>("emp_endereco_correspondencia"),
							Cep = reader.GetValue<string>("empreendimento_cep"),
							Logradouro = reader.GetValue<string>("empreendimento_logradouro"),
							Bairro = reader.GetValue<string>("empreendimento_bairro"),
							Distrito = reader.GetValue<string>("empreendimento_distrito"),
							MunicipioTexto = reader.GetValue<string>("empreendimento_municipio"),
							EstadoSigla = reader.GetValue<string>("empreendimento_estado_sigla")
						});

						entidade.Empreendimento.Coordenada.Tipo.Texto = reader.GetValue<string>("emp_coordenada_texto");
						entidade.Empreendimento.Coordenada.NorthingUtm = reader.GetValue<double?>("emp_northing");
						entidade.Empreendimento.Coordenada.EastingUtm = reader.GetValue<double?>("emp_easting");
						entidade.Empreendimento.Coordenada.FusoUtm = reader.GetValue<int?>("emp_fuso");
						entidade.Empreendimento.Coordenada.LocalColetaTexto = reader.GetValue<string>("emp_local_coleta");
						entidade.Empreendimento.Coordenada.FormaColetaTexto = reader.GetValue<string>("emp_forma_coleta");
						entidade.Empreendimento.Coordenada.DatumTexto = reader.GetValue<string>("emp_datum_texto");

						entidade.Sicar.NumeroSICAR = reader.GetValue<string>("numero_sicar");
						entidade.Sicar.Pendencias = reader.GetValue<string>("pendencias_sicar");
						entidade.RequerimentoNumero = reader.GetValue<Int32>("requerimento_id");
					}

					reader.Close();
				}

				#endregion Solicitação

				return entidade;
			}
		}

		internal DominialidadeRelatorio ObterCaracterizacaoInstitucional(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			DominialidadeRelatorio caracterizacao = new DominialidadeRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				if (tid == null)
				{
					caracterizacao = ObterCaracterizacaoInstitucional(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from crt_dominialidade s where s.id = :id and s.tid = :tid");

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? ObterCaracterizacaoInstitucional(id, bancoDeDados, simplificado) : ObterHistoricoCaracterizacaoInstitucional(id, bancoDeDados, tid, simplificado);
				}

				#endregion
			}

			return caracterizacao;
		}

		private DominialidadeRelatorio ObterCaracterizacaoInstitucional(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			DominialidadeRelatorio caracterizacao = new DominialidadeRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.tid from crt_dominialidade d where d.id = :id");
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
				#region ATP
				comando = bancoDeDados.CriarComando(@"SELECT (ATP.AREA_M2) ATP FROM CRT_PROJETO_GEO CRP
														  INNER JOIN  GEO_ATP   ATP ON ATP.PROJETO = CRP.ID  
														  INNER JOIN CRT_DOMINIALIDADE  CRD ON CRD.EMPREENDIMENTO = CRP.EMPREENDIMENTO
														WHERE CRD.ID  = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.ATPCroquiHa = reader.GetValue<Decimal>("ATP").Convert(eMetrica.M2ToHa).ToStringTrunc(4); ;
					}
				}
				#endregion

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.tipo Tipo, la.texto TipoTexto, a.valor Valor, a.tid Tid 
				from crt_dominialidade_areas a, lov_crt_dominialidade_area la where a.tipo = la.id and a.dominialidade = :dominialidade");

				comando.AdicionarParametroEntrada("dominialidade", id, DbType.Int32);

				caracterizacao.Areas = bancoDeDados.ObterEntityList<DominialidadeAreaRelatorio>(comando);

				#endregion

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select d.id, d.tid, d.tipo, ldt.texto tipo_texto, d.area_croqui, 
				d.area_documento, d.app_croqui, d.numero_ccri, d.area_ccri, d.arl_documento  from crt_dominialidade_dominio d, lov_crt_domin_dominio_tipo ldt 
				where d.tipo = ldt.id and d.dominialidade = :id");

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
							from crt_dominialidade_reserva r, lov_crt_domin_reserva_sit_veg lrsv where r.situacao_vegetal = lrsv.id(+) and r.dominio = :dominio");

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

		private DominialidadeRelatorio ObterHistoricoCaracterizacaoInstitucional(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			DominialidadeRelatorio caracterizacao = new DominialidadeRelatorio();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.id,  d.tid from hst_crt_dominialidade d where d.dominialidade_id = :id and d.tid = :tid");

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
				#region ATP
				comando = bancoDeDados.CriarComando(@"SELECT (ATP.AREA_M2) ATP FROM CRT_PROJETO_GEO CRP
														  INNER JOIN  IDAFGEO.GEO_ATP   ATP ON ATP.PROJETO = CRP.ID  
														  INNER JOIN CRT_DOMINIALIDADE  CRD ON CRD.EMPREENDIMENTO = CRP.EMPREENDIMENTO
														WHERE CRD.ID  = :id");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.ATPCroquiHa = reader.GetValue<Decimal>("ATP").Convert(eMetrica.M2ToHa).ToStringTrunc(4); ;
					}
				}
				#endregion
				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id Id, a.tipo Tipo, la.texto TipoTexto, a.valor Valor, a.tid Tid
				from crt_dominialidade_areas a, lov_crt_dominialidade_area la where a.tipo = la.id and a.dominialidade = :dominialidade");

				comando.AdicionarParametroEntrada("dominialidade", id, DbType.Int32);

				caracterizacao.Areas = bancoDeDados.ObterEntityList<DominialidadeAreaRelatorio>(comando);

				#endregion

				#region Domínios

				comando = bancoDeDados.CriarComando(@"select d.id id_hst, d.dominialidade_dominio_id id, d.tid, d.tipo_id, d.area_croqui,
				d.area_documento, d.app_croqui, d.numero_ccri, d.area_ccri, d.arl_documento from hst_crt_dominialidade_dominio d where d.id_hst = :id");

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
						from hst_crt_dominialidade_reserva r where r.id_hst = :id");

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

		public CARSolicitacaoRelatorio ObterHistoricoCredenciado(int id, BancoDeDados banco = null)
		{
			CARSolicitacaoRelatorio entidade = new CARSolicitacaoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, "IDAFCREDENCIADO"))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select hcs.tid,
						hcs.dominialidade_id,
						hcs.dominialidade_tid,
						hcs.numero,
						hcs.data_emissao,
						hcs.situacao_texto,
						hcs.motivo,
						(select count(*)
							from hst_empreendimento_responsavel her
							where her.id_hst = he.id
							and her.responsavel_id != hcs.declarante_id) possui_outros,
						hp.tipo declarante_tipo,
						(select lrt.texto
							from hst_empreendimento_responsavel her,
								lov_empreendimento_tipo_resp   lrt
							where her.tipo_id = lrt.id
							and her.id_hst = he.id
							and her.responsavel_id = hcs.declarante_id
							and her.responsavel_tid = hcs.declarante_tid) declarante_tipo_texto,
						nvl(hp.nome, hp.razao_social) declarante_nome_razao,
						nvl(hp.cpf, hp.cnpj) declarante_cpf_cnpj,
						hpe.cep declarante_cep,
						hpe.logradouro declarante_logradouro,
						hpe.bairro declarante_bairro,
						lmp.texto declarante_municipio,
						hpe.distrito declarante_distrito,
						lem.sigla declarante_estado_sigla,
						he.codigo empreendimento_codigo,
						he.denominador empreendimento_denominador,
						he.cnpj empreendimento_cnpj,
						hee.correspondencia emp_endereco_correspondencia,
						hee.cep empreendimento_cep,
						hee.logradouro empreendimento_logradouro,
						hee.bairro empreendimento_bairro,
						lme.texto empreendimento_municipio,
						hee.distrito empreendimento_distrito,
						lee.sigla empreendimento_estado_sigla,
						lct.texto emp_coordenada_texto,
						ld.texto emp_datum_texto,
						hec.northing_utm emp_northing,
						hec.easting_utm emp_easting,
						hec.fuso_utm emp_fuso,
						lfc.texto emp_local_coleta,
						llc.texto emp_forma_coleta,
						hcs.requerimento_id,
						hcsicar.situacao_envio situacao_sicar,
						hcsicar.codigo_imovel numero_sicar,
						hcsicar.pendencias pendencias_sicar,
						nvl(hcsicar.data_envio, hcsicar.data_gerado ) data_envio_sicar
					from hst_car_solicitacao            hcs,
						hst_pessoa                     hp,
						hst_pessoa_endereco            hpe,
						lov_estado                     lem,
						hst_empreendimento             he,
						hst_empreendimento_endereco    hee,
						lov_estado                     lee,
						hst_empreendimento_coord       hec,
						lov_empreendimento_local_colet llc,
						lov_empreendimento_forma_colet lfc,
						lov_coordenada_datum           ld,
						lov_coordenada_tipo            lct,
						lov_municipio                  lme,
						lov_municipio                  lmp,
						TAB_controle_sicar             hcsicar
					where hp.pessoa_id(+) = hcs.declarante_id
					and hp.tid(+) = hcs.declarante_tid
					and he.empreendimento_id(+) = hcs.empreendimento_id
					and he.tid(+) = hcs.empreendimento_tid
					and hpe.id_hst(+) = hp.id
					and lem.id(+) = hpe.estado_id
					and hee.id_hst(+) = he.id
					and hee.correspondencia(+) = 0
					and lee.id(+) = hee.estado_id
					and hec.id_hst(+) = he.id
					and llc.id(+) = hec.local_coleta_id
					and lfc.id(+) = hec.forma_coleta_id
					and ld.id(+) = hec.datum_id
					and lct.id(+) = hec.tipo_coordenada_id
					and lme.id(+) = hee.municipio_id
					and lmp.id(+) = hpe.municipio_id
					and hcsicar.solicitacao_car(+) = hcs.solicitacao_id
					--and hcsicar.solicitacao_car_tid(+) = hcs.tid
					and hcs.id = (select max(id) from hst_car_solicitacao hcs1 where hcs1.solicitacao_id = hcs.solicitacao_id)
                    and ((select min(hctrs.id) from TAB_controle_sicar hctrs where hctrs.solicitacao_car = hcs.solicitacao_id
							and hctrs.solicitacao_car_esquema = 2 /*and hctrs.data_execucao >= hcs.data_execucao*/) = hcsicar.id or hcsicar.id is null)
					and hcs.solicitacao_id = :id", "IDAFCREDENCIADO");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Numero = reader.GetValue<int>("numero");
						entidade.DataEmissao = reader.GetValue<string>("data_emissao");
						entidade.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						entidade.Motivo = reader.GetValue<string>("Motivo");
						entidade.DominialidadeId = reader.GetValue<int>("dominialidade_id");
						entidade.DominialidadeTid = reader.GetValue<string>("dominialidade_tid");

						entidade.DeclarantePossuiOutros = reader.GetValue<int>("possui_outros") > 0;
						entidade.Declarante.Tipo = reader.GetValue<int>("declarante_tipo");
						entidade.Declarante.TipoTexto = reader.GetValue<string>("declarante_tipo_texto");
						entidade.Declarante.NomeRazaoSocial = reader.GetValue<string>("declarante_nome_razao");
						entidade.Declarante.CPFCNPJ = reader.GetValue<string>("declarante_cpf_cnpj");
						entidade.Declarante.Endereco.Cep = reader.GetValue<string>("declarante_cep");
						entidade.Declarante.Endereco.Logradouro = reader.GetValue<string>("declarante_logradouro");
						entidade.Declarante.Endereco.Bairro = reader.GetValue<string>("declarante_bairro");
						entidade.Declarante.Endereco.Distrito = reader.GetValue<string>("declarante_distrito");
						entidade.Declarante.Endereco.MunicipioTexto = reader.GetValue<string>("declarante_municipio");
						entidade.Declarante.Endereco.EstadoSigla = reader.GetValue<string>("declarante_estado_sigla");

						entidade.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
						entidade.Empreendimento.NomeRazao = reader.GetValue<string>("empreendimento_denominador");
						entidade.Empreendimento.CNPJ = reader.GetValue<string>("empreendimento_cnpj");

						entidade.Empreendimento.Enderecos.Add(new EnderecoRelatorio()
						{
							Correspondencia = reader.GetValue<int?>("emp_endereco_correspondencia"),
							Cep = reader.GetValue<string>("empreendimento_cep"),
							Logradouro = reader.GetValue<string>("empreendimento_logradouro"),
							Bairro = reader.GetValue<string>("empreendimento_bairro"),
							Distrito = reader.GetValue<string>("empreendimento_distrito"),
							MunicipioTexto = reader.GetValue<string>("empreendimento_municipio"),
							EstadoSigla = reader.GetValue<string>("empreendimento_estado_sigla")
						});

						entidade.Empreendimento.Coordenada.Tipo.Texto = reader.GetValue<string>("emp_coordenada_texto");
						entidade.Empreendimento.Coordenada.NorthingUtm = reader.GetValue<double?>("emp_northing");
						entidade.Empreendimento.Coordenada.EastingUtm = reader.GetValue<double?>("emp_easting");
						entidade.Empreendimento.Coordenada.FusoUtm = reader.GetValue<int?>("emp_fuso");
						entidade.Empreendimento.Coordenada.LocalColetaTexto = reader.GetValue<string>("emp_local_coleta");
						entidade.Empreendimento.Coordenada.FormaColetaTexto = reader.GetValue<string>("emp_forma_coleta");
						entidade.Empreendimento.Coordenada.DatumTexto = reader.GetValue<string>("emp_datum_texto");

						entidade.Sicar.SituacaoId = reader.GetValue<int>("situacao_sicar");
						entidade.Sicar.NumeroSICAR = reader.GetValue<string>("numero_sicar");
						entidade.Sicar.Pendencias = reader.GetValue<string>("pendencias_sicar");
						entidade.Sicar.DataEnvio = reader.GetValue<string>("data_envio_sicar");
						entidade.RequerimentoNumero = reader.GetValue<Int32>("requerimento_id");

					}

					reader.Close();
				}

				#endregion Solicitação

				return entidade;
			}
		}

		internal Dominialidade ObterCaracterizacaoCredenciado(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, "IDAFCREDENCIADO"))
			{
				#region Dominialidade

				if (tid == null)
				{
					caracterizacao = ObterCaracterizacaoCredenciado(id, bancoDeDados, simplificado);
				}
				else
				{
					Comando comando = bancoDeDados.CriarComando(@"select count(s.id) existe from {0}crt_dominialidade s where s.id = :id and s.tid = :tid", "IDAFCREDENCIADO");

					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

					caracterizacao = (Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando))) ? ObterCaracterizacaoCredenciado(id, bancoDeDados, simplificado) : ObterHistoricoCaracterizacaoCredenciado(id, bancoDeDados, tid, simplificado);
				}

				#endregion
			}

			return caracterizacao;
		}

		internal Dominialidade ObterCaracterizacaoCredenciado(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, "IDAFCREDENCIADO"))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.empreendimento, ee.zona empreendimento_localizacao, d.possui_area_exced_matri, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, d.tid 
				from {0}crt_dominialidade d, {0}tab_empreendimento_endereco ee 
				where ee.correspondencia = 0 and d.empreendimento = ee.empreendimento and d.id = :id", "IDAFCREDENCIADO");

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
				#region ATP
				comando = bancoDeDados.CriarComando(@"SELECT (ATP.AREA_M2) ATP FROM CRT_PROJETO_GEO CRP
														  INNER JOIN  IDAFCREDENCIADOGEO.GEO_ATP   ATP ON ATP.PROJETO = CRP.ID  
														  INNER JOIN CRT_DOMINIALIDADE  CRD ON CRD.EMPREENDIMENTO = CRP.EMPREENDIMENTO
														WHERE CRD.ID  = :id", "IDAFCREDENCIADO");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.ATPCroqui = reader.GetValue<Decimal>("ATP");
					}
				}
				#endregion

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id, a.tipo, la.texto tipo_texto, a.valor, a.tid from {0}crt_dominialidade_areas a, {0}lov_crt_dominialidade_area la 
				where a.tipo = la.id and a.dominialidade = :dominialidade", "IDAFCREDENCIADO");

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
				{0}lov_crt_domin_comprovacao ldc where d.tipo = ldt.id and d.comprovacao = ldc.id(+) and d.dominialidade = :id", "IDAFCREDENCIADO");

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
							= lrl.id(+) and r.situacao_vegetal = lrsv.id(+) and r.cartorio = lrc.id(+) and r.dominio = :dominio", "IDAFCREDENCIADO");

						comando.AdicionarParametroEntrada("dominio", dominio.Id, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.ReservaLegal reserva = null;

							while (readerAux.Read())
							{
								reserva = new Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.ReservaLegal();
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

		private Dominialidade ObterHistoricoCaracterizacaoCredenciado(int id, BancoDeDados banco = null, string tid = null, bool simplificado = false)
		{
			Dominialidade caracterizacao = new Dominialidade();
			int hst = 0;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, "IDAFCREDENCIADO"))
			{
				#region Dominialidade

				Comando comando = bancoDeDados.CriarComando(@"select d.id, d.empreendimento_id, d.possui_area_exced_matri, 
				d.confrontante_norte, d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste, d.tid 
				from {0}hst_crt_dominialidade d where d.dominialidade_id = :id and d.tid = :tid", "IDAFCREDENCIADO");

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
				#region ATP
				comando = bancoDeDados.CriarComando(@"SELECT (ATP.AREA_M2) ATP FROM CRT_PROJETO_GEO CRP
														  INNER JOIN  IDAFGEO.GEO_ATP   ATP ON ATP.PROJETO = CRP.ID  
														  INNER JOIN CRT_DOMINIALIDADE  CRD ON CRD.EMPREENDIMENTO = CRP.EMPREENDIMENTO
														WHERE CRD.ID  = :id", "IDAFCREDENCIADO");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.ATPCroqui = reader.GetValue<Decimal>("ATP");
					}
				}
				#endregion

				#region Áreas

				comando = bancoDeDados.CriarComando(@"select a.id, a.tid, a.tipo_id, a.tipo_texto, a.valor
				from {0}hst_crt_dominialidade_areas a where a.id_hst = :id", "IDAFCREDENCIADO");

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
				d.confrontante_sul, d.confrontante_leste, d.confrontante_oeste from {0}hst_crt_dominialidade_dominio d where d.id_hst = :id", "IDAFCREDENCIADO");

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
						from {0}hst_crt_dominialidade_reserva r where r.id_hst = :id", "IDAFCREDENCIADO");

						comando.AdicionarParametroEntrada("id", hst, DbType.Int32);
						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.ReservaLegal reserva = null;

							while (readerAux.Read())
							{
								reserva = new Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloDominialidade.ReservaLegal();
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

		public MemoryStream GerarPdf(CARSolicitacaoRelatorio dataSource, string arquivoCaminho)
		{
			MemoryStream stream = new MemoryStream();
			ArquivoDocCaminho = arquivoCaminho;
			ObterArquivoTemplate();
			ConfigurarCabecarioRodape(0);
			dataSource = ConfigurarTabelas(dataSource);

			return GerarPdf(dataSource);
		}

		public CARSolicitacaoRelatorio ConfigurarTabelas(CARSolicitacaoRelatorio dataSource)
		{
			ConfiguracaoDefault.AddLoadAcao((doc, dataSrc) =>
			{
				if (string.IsNullOrEmpty(dataSource.Declarante.TipoTexto))
				{
					dataSource.Declarante.TipoTexto = AsposeData.Empty;
					dataSource.DoisPontos = AsposeData.Empty;
				}

				if (!dataSource.DeclarantePossuiOutros)
				{
					dataSource.DeclaranteOutros = AsposeData.Empty;
				}
			});

			return dataSource;
		}

		public void SalvarPdfSolicitacaoCar(MemoryStream pdf, CARSolicitacaoRelatorio solicitacao)
		{
			ArquivoBus _busArquivo = new ArquivoBus(solicitacao.Origem == 1 ? eExecutorTipo.Interno : eExecutorTipo.Credenciado);
			ArquivoDa _arquivoDa = new ArquivoDa();
			Arquivo arquivo = new Arquivo
			{
				Nome = "Solicitação CAR",
				Extensao = ".pdf",
				ContentType = "application/pdf",
				Buffer = pdf
			};

			_busArquivo.Salvar(arquivo);

			_arquivoDa.Salvar(arquivo, Values.UsuarioScheduler.Id, Values.UsuarioScheduler.Nome, Values.UsuarioScheduler.Login, solicitacao.Origem, Values.UsuarioScheduler.Tid);

			SalvarPdfSolicitacaoCar(solicitacao.Id, arquivo.Id ?? 0, solicitacao.Origem == 1 ? "IDAFCREDENCIADO" : "");
		}

		public static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[16 * 1024];
			int read;
			while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
			{
				output.Write(buffer, 0, read);
			}
		}

		public void SalvarPdfSolicitacaoCar(int solicitacaoId, int arquivoId, string EsquemaBanco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_car_solicitacao s set s.arquivo = :arquivo where s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", solicitacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo", arquivoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}
	}
}
