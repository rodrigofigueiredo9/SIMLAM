﻿using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class LocalInfracaoDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }
        private String EsquemaBancoGeo { get; set; }

		#endregion

		public LocalInfracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
            EsquemaBancoGeo = "idafgeo";
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		public LocalInfracaoRelatorio Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			LocalInfracaoRelatorio objeto = new LocalInfracaoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select l.id Id,
													l.setor SetorId,
													nvl(l.responsavel, l.pessoa) AutuadoEmpResponsavelId,
													l.resp_propriedade PropResponsavelId,
													l.empreendimento EmpreendimentoId,
													nvl (emp.nome_fantasia, emp.denominador) EmpreendimentoNomeRazaoSocial,
													emp.cnpj EmpreendimentoCnpj,
													ct.texto SistemaCoordenada,
													l.fuso Fuso,
													d.texto Datum,
													l.lon_easting CoordenadaEasting,
													l.lat_northing CoordenadaNorthing,
													l.local Local,
													to_char(l.data, 'DD/MM/YYYY') DataFiscalizacao,
													m.texto Municipio,
													e.sigla UF
												from {0}tab_fisc_local_infracao l,
													{0}tab_empreendimento      emp,
													{0}lov_municipio           m,
													{0}lov_estado              e,
													{0}lov_coordenada_tipo     ct,
													{0}lov_coordenada_datum    d
												where l.municipio = m.id
												and e.id = m.estado
												and l.sis_coord = ct.id
												and l.datum = d.id
												and emp.id(+) = l.empreendimento
												and l.fiscalizacao = :fiscalizacaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<LocalInfracaoRelatorio>(comando);
				objeto.Autuado = ObterAutuado(objeto.AutuadoEmpResponsavelId, bancoDeDados);				
				objeto.EmpResponsavel = ObterAutuado(objeto.PropResponsavelId, bancoDeDados);
				
				if (objeto.EmpreendimentoId > 0)
				{
					objeto.EmpEndereco = ObterEmpEndereco(objeto.EmpreendimentoId, bancoDeDados);
				}

			}

			return objeto;
		}

        public LocalInfracaoRelatorioNovo ObterNovo(int fiscalizacaoId, BancoDeDados banco = null)
        {
            LocalInfracaoRelatorioNovo objeto = new LocalInfracaoRelatorioNovo();
            Comando comando = null;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                comando = bancoDeDados.CriarComando(@"select l.id Id,
													l.setor SetorId,
													nvl(l.responsavel, l.pessoa) AutuadoEmpResponsavelId,
													l.resp_propriedade PropResponsavelId,
													l.empreendimento EmpreendimentoId,
													nvl (emp.nome_fantasia, emp.denominador) EmpreendimentoNomeRazaoSocial,
													emp.cnpj EmpreendimentoCnpj,
                                                    emp.codigo CodEmp,
													ct.texto SistemaCoordenada,
													l.fuso Fuso,
													d.texto Datum,
													l.lon_easting CoordenadaEasting,
													l.lat_northing CoordenadaNorthing,
													l.local Local,
													to_char(Inf.Data_Constatacao, 'DD/MM/YYYY') DataFiscalizacao,
                                                    Inf.Hora_Constatacao HoraFiscalizacao,
													m.texto Municipio,
													e.sigla UF
												from {0}tab_fisc_local_infracao l,
													{0}tab_empreendimento      emp,
													{0}lov_municipio           m,
													{0}lov_estado              e,
													{0}lov_coordenada_tipo     ct,
													{0}lov_coordenada_datum    d,
                                                    {0}tab_fisc_infracao       inf
												where l.municipio = m.id
												and e.id = m.estado
												and l.sis_coord = ct.id
												and l.datum = d.id
												and emp.id(+) = l.empreendimento
                                                and inf.fiscalizacao(+) = l.fiscalizacao
												and l.fiscalizacao = :fiscalizacaoId", EsquemaBanco);

                comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

                objeto = bancoDeDados.ObterEntity<LocalInfracaoRelatorioNovo>(comando);
                objeto.Autuado = ObterAutuado(objeto.AutuadoEmpResponsavelId, bancoDeDados);
                objeto.EmpResponsavel = ObterAutuado(objeto.PropResponsavelId, bancoDeDados);

                comando = bancoDeDados.CriarComando(@"
                            select area_fiscalizacao area
                            from {0}tab_fisc_local_infracao
                            where fiscalizacao = :fiscalizacaoId", EsquemaBanco);
                comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);
                int area = -1;
                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        area = reader.GetValue<int>("area");
                    }

                    reader.Close();
                }

				//se a área for DDSIA, converte as coordenadas para Grau, Minuto e Segundo
				if (area == 0)
                {
                    comando = bancoDeDados.CriarComandoPlSql(@"
                                begin
                                    {0}coordenada.utm2gms(:datum, :easting, :northing, :fuso, 1, :longitude, :latitude);
                                end;", EsquemaBancoGeo);
                    comando.AdicionarParametroEntrada("datum", objeto.Datum, DbType.String);
                    comando.AdicionarParametroEntrada("easting", objeto.CoordenadaEasting, DbType.Int64);
                    comando.AdicionarParametroEntrada("northing", objeto.CoordenadaNorthing, DbType.Int64);
                    comando.AdicionarParametroEntrada("fuso", objeto.Fuso, DbType.Int32);
                    comando.AdicionarParametroSaida("longitude", DbType.String, 100);
                    comando.AdicionarParametroSaida("latitude", DbType.String, 100);

                    bancoDeDados.ExecutarNonQuery(comando);
                    
                    objeto.CoordenadaEasting = comando.ObterValorParametro<string>("longitude");
                    objeto.CoordenadaNorthing = comando.ObterValorParametro<string>("latitude");
                    objeto.SistemaCoordenada = "GMS";
					objeto.AreaFiscalizacao = "DDSIA";
				}
				else if(area == 1)
					objeto.AreaFiscalizacao = "DDSIV";
				else if(area == 2)
					objeto.AreaFiscalizacao = "DRNRE";

				if (objeto.EmpreendimentoId > 0)
                {
                    objeto.EmpEndereco = ObterEmpEndereco(objeto.EmpreendimentoId, bancoDeDados);

                    comando = bancoDeDados.CriarComando(@"
                                select letr.texto vinculo
                                from lov_empreendimento_tipo_resp letr,
                                     tab_empreendimento_responsavel ter
                                where ter.empreendimento = :empreendimento
                                      and ter.responsavel = :autuado
                                      and letr.id = ter.tipo", EsquemaBanco);

                    comando.AdicionarParametroEntrada("empreendimento", objeto.EmpreendimentoId, DbType.Int32);
                    comando.AdicionarParametroEntrada("autuado", objeto.AutuadoEmpResponsavelId, DbType.Int32);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        if (reader.Read())
                        {
                            objeto.Autuado.TipoTexto = reader.GetValue<string>("vinculo");
                        }

                        reader.Close();
                    }
                }

            }

            return objeto;
        }

		public LocalInfracaoRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
		{
			LocalInfracaoRelatorio objeto = new LocalInfracaoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"
					select l.id                          Id,
						l.setor_id                       SetorId,
						nvl(l.responsavel_id, l.pessoa_id)  AutuadoEmpResponsavelId,
						nvl(l.responsavel_tid, l.pessoa_tid)  AutuadoEmpResponsavelTid,
						l.resp_propriedade_id         PropResponsavelId,
						l.resp_propriedade_tid        PropResponsavelTid,
						l.empreendimento_id           EmpreendimentoId,
						l.empreendimento_tid		  EmpreendimentoTid,
						nvl (emp.nome_fantasia, emp.denominador) EmpreendimentoNomeRazaoSocial,
						emp.cnpj EmpreendimentoCnpj,
						l.sis_coord_texto             SistemaCoordenada,
						l.fuso                        Fuso,
						l.datum_texto                 Datum,
						l.lon_easting                 CoordenadaEasting,
						l.lat_northing                CoordenadaNorthing,
						l.local                       Local,
						to_char(l.data, 'DD/MM/YYYY') DataFiscalizacao,
						l.municipio_texto             Municipio,
						e.sigla                       UF
					from hst_fisc_local_infracao l,
						tab_empreendimento      emp,
						lov_municipio           m,
						lov_estado              e
						where l.municipio_id = m.id
						and e.id = m.estado
						and emp.id(+) = l.empreendimento_id
						and l.id_hst = :historicoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<LocalInfracaoRelatorio>(comando);
				objeto.Autuado = ObterAutuadoHistorico(objeto.AutuadoEmpResponsavelId, objeto.AutuadoEmpResponsavelTid, bancoDeDados);
				objeto.EmpResponsavel = ObterAutuadoHistorico(objeto.PropResponsavelId, objeto.PropResponsavelTid, bancoDeDados);

				if (objeto.EmpreendimentoId > 0)
				{
					objeto.EmpEndereco = ObterEmpEnderecoHistorico(objeto.EmpreendimentoId, objeto.EmpreendimentoTid, bancoDeDados);
				}

			}

			return objeto;
		}

		private EnderecoRelatorio ObterEmpEndereco(int empId, BancoDeDados banco = null)
		{
			EnderecoRelatorio objeto = new EnderecoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select ee.bairro Bairro, ee.cep Cep, ee.complemento Complemento, ee.distrito, e.sigla EstadoTexto, 
				ee.logradouro Logradouro, m.texto MunicipioTexto from {0}tab_empreendimento_endereco ee, {0}lov_estado e, {0}lov_municipio m 
				where ee.estado = e.id and ee.municipio = m.id and ee.correspondencia = 0 and ee.empreendimento = :empId", EsquemaBanco);

				comando.AdicionarParametroEntrada("empId", empId, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<EnderecoRelatorio>(comando);
			}

			return objeto;
		}

		private EnderecoRelatorio ObterEmpEnderecoHistorico(int empId, string empTid, BancoDeDados banco = null)
		{
			EnderecoRelatorio objeto = new EnderecoRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@"select ee.bairro Bairro, 
				   ee.cep Cep, 
				   ee.complemento Complemento, 
				   ee.distrito, 
				   e.sigla EstadoTexto, 
				   ee.logradouro Logradouro, 
				   ee.municipio_texto MunicipioTexto 
				  from {0}hst_empreendimento he, {0}hst_empreendimento_endereco ee, {0}lov_estado e 
				  where he.empreendimento_id = :empId
				  and he.tid = :empTid
				  and he.id = ee.id_hst
				  and ee.estado_id = e.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("empId", empId, DbType.Int32);
				comando.AdicionarParametroEntrada("empTid", empTid, DbType.String);

				objeto = bancoDeDados.ObterEntity<EnderecoRelatorio>(comando);
			}

			return objeto;
		}

		private PessoaRelatorio ObterEmpResponsavel(int empId, BancoDeDados banco = null)
		{
			PessoaRelatorio objeto = new PessoaRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select nvl(tp.cpf, tp.cnpj) CPF, nvl(tp.nome, tp.razao_social) Nome from tab_pessoa tp, tab_fisc_local_infracao tfli where 
					fli.pessoa = tp.id and tfli.id = :localIfracaoId union all select te.cnpj, te.denominador from tab_empreendimento te, tab_fisc_local_infracao tfli where tfli.empreendimento = te.id
					and tfli.id = :localIfracaoId  ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empId", empId, DbType.Int32);

				objeto.Fisica = bancoDeDados.ObterEntity<FisicaRelatorio>(comando);
			}

			return objeto;
		}

		private PessoaRelatorio ObterEmpResponsavelHistorico(int empId, string empTid, BancoDeDados banco = null)
		{
			PessoaRelatorio objeto = new PessoaRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select nvl(tp.cpf, tp.cnpj) CPF, nvl(tp.nome, tp.razao_social) Nome from tab_pessoa tp, tab_fisc_local_infracao tfli where 
					fli.pessoa = tp.id and tfli.id = :localIfracaoId union all select te.cnpj, te.denominador from tab_empreendimento te, tab_fisc_local_infracao tfli where tfli.empreendimento = te.id
					and tfli.id = :localIfracaoId  ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empId", empId, DbType.Int32);
				comando.AdicionarParametroEntrada("empTid", empTid, DbType.Int32);

				objeto.Fisica = bancoDeDados.ObterEntity<FisicaRelatorio>(comando);
			}

			return objeto;
		}

		private PessoaRelatorio ObterAutuado(int empResponsavelId, BancoDeDados banco = null)
		{
			PessoaRelatorio objeto = new PessoaRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select nvl(p.nome, p.razao_social) Nome, nvl(p.cpf, p.cnpj) CPF from {0}tab_pessoa p where p.id = :empResponsavelId  ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empResponsavelId", empResponsavelId, DbType.Int32);

				objeto.Fisica = bancoDeDados.ObterEntity<FisicaRelatorio>(comando);
			}

			return objeto;
		}
		
		private PessoaRelatorio ObterAutuadoHistorico(int empResponsavelId, string empResponsavelTid, BancoDeDados banco = null)
		{
			PessoaRelatorio objeto = new PessoaRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select nvl(p.nome, p.razao_social) Nome, nvl(p.cpf, p.cnpj) CPF from {0}hst_pessoa p where p.pessoa_id = :empResponsavelId and p.tid = :empResponsavelTid ", EsquemaBanco);

				comando.AdicionarParametroEntrada("empResponsavelId", empResponsavelId, DbType.Int32);
				comando.AdicionarParametroEntrada("empResponsavelTid", empResponsavelTid, DbType.String);

				objeto.Fisica = bancoDeDados.ObterEntity<FisicaRelatorio>(comando);
			}

			return objeto;
		}

		#endregion
	}
}
