using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFiscalizacao;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFiscalizacao.Data
{
	public class OutrasPenalidadesDa
	{
		#region Propriedade e Atributos

		private String EsquemaBanco { get; set; }

		#endregion

		public OutrasPenalidadesDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		public OutrasPenalidadesRelatorio Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
            OutrasPenalidadesRelatorio objeto = new OutrasPenalidadesRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
                comando = bancoDeDados.CriarComando(@"select count(id) existe
                                                      from {0}Tab_Fisc_Outras_Penalidades
                                                      where fiscalizacao = :fiscalizacaoId", EsquemaBanco);
                comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);


                int existe = 0;

                IDataReader readerCount = bancoDeDados.ExecutarReader(comando);
                if (readerCount.Read())
                {
                    existe = Convert.ToInt32(readerCount["existe"].ToString());
                }
                readerCount.Close();

                if (existe == 0)
                {
                    return null;
                }

                comando = bancoDeDados.CriarComando(@"select Tfop.Iuf_Numero NumeroIUF,
                                                             Lfs.Texto SerieTexto,
                                                             to_char(tfop.iuf_data, 'DD/MM/YYYY') DataLavraturaIUF,
                                                             Tfop.Descricao Descricao
                                                      from {0}Tab_Fisc_Outras_Penalidades tfop,
                                                           {0}Lov_Fiscalizacao_Serie lfs
                                                      where (tfop.serie is null or Tfop.Serie = lfs.id)
                                                            and fiscalizacao = :fiscalizacaoId", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacaoId", fiscalizacaoId, DbType.Int32);

                objeto = bancoDeDados.ObterEntity<OutrasPenalidadesRelatorio>(comando);
			}

			return objeto;
		}

//        public LocalInfracaoRelatorio ObterHistorico(int historicoId, BancoDeDados banco = null)
//        {
//            LocalInfracaoRelatorio objeto = new LocalInfracaoRelatorio();
//            Comando comando = null;

//            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
//            {
//                comando = bancoDeDados.CriarComando(@"
//					select l.id                          Id,
//						l.setor_id                       SetorId,
//						nvl(l.responsavel_id, l.pessoa_id)  AutuadoEmpResponsavelId,
//						nvl(l.responsavel_tid, l.pessoa_tid)  AutuadoEmpResponsavelTid,
//						l.resp_propriedade_id         PropResponsavelId,
//						l.resp_propriedade_tid        PropResponsavelTid,
//						l.empreendimento_id           EmpreendimentoId,
//						l.empreendimento_tid		  EmpreendimentoTid,
//						nvl (emp.nome_fantasia, emp.denominador) EmpreendimentoNomeRazaoSocial,
//						emp.cnpj EmpreendimentoCnpj,
//						l.sis_coord_texto             SistemaCoordenada,
//						l.fuso                        Fuso,
//						l.datum_texto                 Datum,
//						l.lon_easting                 CoordenadaEasting,
//						l.lat_northing                CoordenadaNorthing,
//						l.local                       Local,
//						to_char(l.data, 'DD/MM/YYYY') DataFiscalizacao,
//						l.municipio_texto             Municipio,
//						e.sigla                       UF
//					from hst_fisc_local_infracao l,
//						tab_empreendimento      emp,
//						lov_municipio           m,
//						lov_estado              e
//						where l.municipio_id = m.id
//						and e.id = m.estado
//						and emp.id(+) = l.empreendimento_id
//						and l.id_hst = :historicoId", EsquemaBanco);

//                comando.AdicionarParametroEntrada("historicoId", historicoId, DbType.Int32);

//                objeto = bancoDeDados.ObterEntity<LocalInfracaoRelatorio>(comando);
//                objeto.Autuado = ObterAutuadoHistorico(objeto.AutuadoEmpResponsavelId, objeto.AutuadoEmpResponsavelTid, bancoDeDados);
//                objeto.EmpResponsavel = ObterAutuadoHistorico(objeto.PropResponsavelId, objeto.PropResponsavelTid, bancoDeDados);

//                if (objeto.EmpreendimentoId > 0)
//                {
//                    objeto.EmpEndereco = ObterEmpEnderecoHistorico(objeto.EmpreendimentoId, objeto.EmpreendimentoTid, bancoDeDados);
//                }

//            }

//            return objeto;
//        }

		#endregion
	}
}
