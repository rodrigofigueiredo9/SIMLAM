using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloRequerimento.Data
{
	public class RequerimentoInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public RequerimentoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal int ObterNovoID(BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select {0}seq_requerimento.nextval from dual", EsquemaBanco);
				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		internal List<TituloModeloLst> ObterNumerosTitulos(string numero, int modeloId, BancoDeDados banco = null)
		{
			List<TituloModeloLst> lst = new List<TituloModeloLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"	
									select tt.id id,
											ttn.numero || '/' || ttn.ano || ' - ' || tt.data_emissao texto,
											1 ativo,
											ttm.sigla
										from {0}tab_titulo tt, {0}tab_titulo_numero ttn, {0}tab_titulo_modelo ttm
										where tt.id = ttn.titulo
										and tt.situacao in (3, 6) -- Concluído e Prorrogado
										and ttn.numero || '/' || ttn.ano = :numero
										and tt.modelo = ttm.id
										and ttn.modelo = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", numero, DbType.String);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					int count = 1;
					while (reader.Read())
					{
						lst.Add(new TituloModeloLst()
						{
							Id = count++,
							Texto = reader["texto"].ToString(),
							IsAtivo = Convert.ToBoolean(reader["ativo"]),
							IdRelacionamento = Convert.ToInt32(reader["id"]),
							Sigla = reader["sigla"].ToString()
						});
					}

					reader.Close();
				}
			}
			return lst;
		}

		internal bool ValidarNumeroSemAnoExistente(string numero, int modeloId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"	
					select count(*) from {0}tab_titulo tt, {0}tab_titulo_numero ttn
                    where tt.id = ttn.titulo
                    and tt.situacao in (3, 6) -- Concluído e Prorrogado
                    and ttn.numero = :numero
                    and ttn.modelo = :modelo", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", numero, DbType.String);
				comando.AdicionarParametroEntrada("modelo", modeloId, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

	}
}