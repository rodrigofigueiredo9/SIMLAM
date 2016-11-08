using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloOrgaoParceiroConveniado;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloOrgaoParceiroConveniado.Data
{
	public class OrgaoParceiroConveniadoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		#region Obter/Filtrar

		internal OrgaoParceiroConveniado Obter(int id, BancoDeDados banco = null)
		{
			OrgaoParceiroConveniado entidade = new OrgaoParceiroConveniado();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Orgao Parceiro/ Conveniado

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.orgao_sigla, t.orgao_nome, t.termo_numero_ano, t.diario_oficial_data, 
															t.situacao, l.texto situacao_texto, t.situacao_motivo, t.situacao_data, t.tid 
															from tab_orgao_parc_conv t, lov_orgao_parc_conv_situacao l 
															where t.id = :id and l.id = t.situacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Sigla = reader.GetValue<String>("orgao_sigla");
						entidade.Nome = reader.GetValue<String>("orgao_nome");
						entidade.TermoNumeroAno = reader.GetValue<String>("termo_numero_ano");
						entidade.DiarioOficialData.DataTexto = reader.GetValue<String>("diario_oficial_data");
						entidade.SituacaoId = reader.GetValue<Int32>("situacao");
						entidade.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						entidade.SituacaoMotivo = reader.GetValue<String>("situacao_motivo");
						entidade.SituacaoData.DataTexto = reader.GetValue<String>("situacao_data");
						entidade.Tid = reader.GetValue<String>("tid");
					}

					reader.Close();

				}

				#endregion

				#region Sigla/ Unidades

				comando = bancoDeDados.CriarComando(@"select u.id, u.orgao_parc_conv, u.sigla, u.nome_local, u.tid from tab_orgao_parc_conv_sigla_unid u 
													where u.orgao_parc_conv = :orgao_parc_conv", EsquemaBanco);

				comando.AdicionarParametroEntrada("orgao_parc_conv", entidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Unidade unidade = null;

					while (reader.Read())
					{
						unidade = new Unidade();
						unidade.Id = reader.GetValue<Int32>("id");
						unidade.Sigla = reader.GetValue<String>("sigla");
						unidade.Nome = reader.GetValue<String>("nome_local");
						unidade.Tid = reader.GetValue<String>("tid");

						entidade.Unidades.Add(unidade);

					}

					reader.Close();

				}

				#endregion
			}

			return entidade;
		}
		
		#endregion

		#region Validacoes

		internal bool ExisteUnidade(int orgaoId, BancoDeDados banco = null)
		{
			bool existeUnidade = false;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_orgao_parc_conv_sigla_unid u 
															where u.orgao_parc_conv = :orgao_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("orgao_id", orgaoId, DbType.Int32);

				existeUnidade = Convert.ToBoolean(bancoDeDados.ExecutarScalar<Int32>(comando));
			}

			return existeUnidade;
		}

		#endregion
	}
}
