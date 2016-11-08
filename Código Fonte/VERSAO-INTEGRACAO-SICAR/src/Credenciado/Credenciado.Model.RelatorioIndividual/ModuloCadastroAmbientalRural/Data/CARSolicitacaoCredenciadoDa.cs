using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCadastroAmbientalRural.Data
{
	public class CARSolicitacaoCredenciadoDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private string EsquemaBanco { get; set; }

		#endregion

		public CARSolicitacaoCredenciadoDa()
		{
			EsquemaBanco = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
		}

		public int ObterSituacao(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.situacao from tab_car_solicitacao s where s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		public CARSolicitacaoRelatorio Obter(int id, BancoDeDados banco = null)
		{
			CARSolicitacaoRelatorio entidade = new CARSolicitacaoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select s.tid,
				cd.id dominialidade_id,
				cd.tid dominialidade_tid,
				s.numero,
				s.data_emissao,
				lss.texto situacao_texto,
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
				e.codigo empreendimento_codigo,
				e.denominador empreendimento_denominador,
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
                tcs.codigo_imovel numero_sicar,
                tcs.pendencias pendencias_sicar
				from tab_car_solicitacao s, lov_car_solicitacao_situacao lss, crt_dominialidade cd, tab_pessoa p, tab_pessoa_endereco pe, lov_estado lep, 
					lov_municipio lmp, tab_empreendimento e, tab_empreendimento_endereco ee, lov_estado lee, lov_municipio lme, tab_empreendimento_coord ec, 
					lov_empreendimento_forma_colet lfc, lov_empreendimento_local_colet llc, lov_coordenada_datum lcd, lov_coordenada_tipo lct, tab_controle_sicar tcs 
				where s.situacao = lss.id
				and s.empreendimento = cd.empreendimento
				and s.declarante = p.id 
				and p.id = pe.pessoa
				and pe.estado = lep.id
				and pe.municipio = lmp.id
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
                and s.id = tcs.solicitacao_car(+)
                and nvl(tcs.solicitacao_car_esquema, 2) = 2
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

						if (!Convert.IsDBNull(reader["empreendimento_codigo"]))
							entidade.Empreendimento.Codigo = Convert.ToInt32(reader["empreendimento_codigo"]);

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

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select hcs.tid,
						hcs.dominialidade_id,
						hcs.dominialidade_tid,
						hcs.numero,
						hcs.data_emissao,
						hcs.situacao_texto,
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
						hcsicar.codigo_imovel numero_sicar,
						hcsicar.pendencias pendencias_sicar
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
						hst_controle_sicar             hcsicar
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
					and hcsicar.solicitacao_car_tid(+) = hcs.tid
					and hcs.id = (select max(id) from hst_car_solicitacao hcs1 where hcs1.solicitacao_id = hcs.solicitacao_id)
					and ((select min(hctrs.id) from hst_controle_sicar hctrs where hctrs.solicitacao_car = hcs.solicitacao_id
							and hctrs.solicitacao_car_esquema = 2 and hctrs.data_execucao >= hcs.data_execucao) = hcsicar.id or hcsicar.id is null)
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
					}

					reader.Close();
				}

				#endregion Solicitação

				return entidade;
			}
		}
	}
}