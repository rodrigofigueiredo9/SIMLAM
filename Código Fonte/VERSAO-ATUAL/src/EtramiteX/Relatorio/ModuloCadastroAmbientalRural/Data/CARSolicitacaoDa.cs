using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Data
{
	public class CARSolicitacaoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public CARSolicitacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public int ObterSituacao(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.situacao from tab_car_solicitacao s where s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		public CARSolicitacaoRelatorio Obter(int id, BancoDeDados banco = null)
		{
			CARSolicitacaoRelatorio entidade = new CARSolicitacaoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"select s.tid,
				cd.id dominialidade_id,
				cd.tid dominialidade_tid,
				s.numero,
				s.data_emissao,
				lss.texto situacao_texto,
				s.situacao_data,
				(select count(*) from tab_empreendimento_responsavel er where er.empreendimento = e.id and er.responsavel != s.declarante) possui_outros,
				p.tipo declarante_tipo,
				(select lert.texto tipo_texto from tab_empreendimento_responsavel er, lov_empreendimento_tipo_resp lert 
				where er.tipo = lert.id and er.empreendimento = e.id and er.responsavel = s.declarante) declarante_tipo_texto,
				nvl(p.nome, p.razao_social) declarante_nome_razao,
				nvl(p.cpf, p.cnpj) declarante_cpf_cnpj,
				pe.cep declarante_cep,
				pe.logradouro declarante_logradouro,
				pe.bairro declarante_bairro,
				lmp.texto declarante_municipio,
				pe.distrito declarante_distrito,
				lep.sigla declarante_estado_sigla,
				e.denominador empreendimento_denominador,
				e.codigo empreendimento_codigo,
				e.cnpj empreendimento_cnpj,
				ee.correspondencia emp_endereco_correspondencia,
				ee.cep empreendimento_cep,
				ee.logradouro empreendimento_logradouro,
				ee.bairro empreendimento_bairro,
				lme.texto empreendimento_municipio,
				ee.distrito empreendimento_distrito,
				lee.sigla empreendimento_estado_sigla,
				lct.texto emp_coordenada_texto,
				lcd.texto emp_datum_texto,
				ec.northing_utm emp_northing,
				ec.easting_utm emp_easting,
				ec.fuso_utm emp_fuso,
				llc.texto emp_local_coleta,
				lfc.texto emp_forma_coleta,
				pt.requerimento,
                (select tcs.codigo_imovel from tab_controle_sicar tcs where tcs.solicitacao_car = s.id and tcs.solicitacao_car_esquema=1) numero_sicar,
                (select tcs.pendencias from tab_controle_sicar tcs where tcs.solicitacao_car = s.id and tcs.solicitacao_car_esquema=1) pendencias_sicar
                from tab_car_solicitacao s, lov_car_solicitacao_situacao lss, crt_dominialidade cd, tab_pessoa p, tab_pessoa_endereco pe, lov_estado lep, 
					lov_municipio lmp, tab_empreendimento e, tab_empreendimento_endereco ee, lov_estado lee, lov_municipio lme, tab_empreendimento_coord ec, 
					lov_empreendimento_forma_colet lfc, lov_empreendimento_local_colet llc, lov_coordenada_datum lcd, lov_coordenada_tipo lct,
					tab_protocolo pt
				where s.situacao = lss.id
				and s.empreendimento = cd.empreendimento
				and s.declarante = p.id 
				and p.id = pe.pessoa(+)
                and pe.estado = lep.id(+)
                and pe.municipio = lmp.id(+)
				and s.empreendimento = e.id
				and e.id = ee.empreendimento
				and ee.correspondencia = 0
				and ee.estado = lee.id
				and ee.municipio = lme.id
				and e.id = ec.empreendimento
				and ec.tipo_coordenada = lct.id
				and ec.datum = lcd.id
				and ec.local_coleta = llc.id
				and ec.forma_coleta = lfc.id
				and s.protocolo_selecionado = pt.id
				and s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Numero = reader.GetValue<int>("numero");
						entidade.DataEmissao = reader.GetValue<string>("data_emissao");
						entidade.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						entidade.SituacaoData = reader.GetValue<DateTime>("situacao_data");
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

						entidade.Sicar.NumeroSICAR = reader.GetValue<string>("numero_sicar");
						entidade.Sicar.Pendencias = reader.GetValue<string>("pendencias_sicar");
						entidade.RequerimentoNumero = reader.GetValue<Int32>("requerimento");
					}

					reader.Close();
				}

				#endregion Solicitação

				return entidade;
			}
		}

		public CARSolicitacaoRelatorio ObterHistorico(int id, BancoDeDados banco = null)
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
							   and her.responsavel_id != hcs.declarante_id)  possui_outros,
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
					   and hcs.solicitacao_id = :id", EsquemaBanco);

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
	}
}