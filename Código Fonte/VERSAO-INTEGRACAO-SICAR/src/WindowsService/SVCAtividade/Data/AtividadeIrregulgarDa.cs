using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using System.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;

namespace Tecnomapas.EtramiteX.WindowsService.SVCAtividade.Data
{
	class AtividadeIrregulgarDa
	{
		public String EsquemaBanco { get; set; }

		public AtividadeIrregulgarDa(string esquema = null)
		{
			EsquemaBanco = esquema;
		}

		internal List<Atividade> ObterTitulosVencidos()
		{
			Comando comando = null;
			List<Atividade> lstAtividades = new List<Atividade>();
			Atividade atividade = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				comando = bancoDeDados.CriarComando(@"select pa.protocolo procdoc, tp.protocolo tipo, pa.atividade, pa.situacao atividade_situacao, t.id titulo, t.situacao titulo_situacao
					  from tab_titulo t, tab_titulo_atividades ta,  tab_protocolo_atividades pa, tab_protocolo_ativ_finalida af, tab_protocolo tp 
					  where t.data_vencimento < trunc(sysdate) 
					  and t.id = ta.titulo  
					  and ta.protocolo = tp.id
					  and ta.protocolo = pa.protocolo
					  and ta.atividade = pa.atividade          
					  and pa.situacao not in (6,8) 
					  and t.situacao <> 5 
					  and pa.id = af.protocolo_ativ
					  and af.modelo = t.modelo");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						atividade = new Atividade();

						atividade.Id = Convert.ToInt32(reader["atividade"]);
						atividade.SituacaoId = Convert.ToInt32(reader["atividade_situacao"]);

						atividade.Protocolo.Id = Convert.ToInt32(reader["procdoc"]);
						atividade.Protocolo.IsProcesso = Convert.ToInt32(reader["tipo"]) == 1;

						lstAtividades.Add(atividade);
					}

					reader.Close();
				}
			}

			return lstAtividades;
		}
	}
}
