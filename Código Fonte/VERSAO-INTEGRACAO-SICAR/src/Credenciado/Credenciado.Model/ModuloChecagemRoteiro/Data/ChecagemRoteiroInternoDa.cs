using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloChecagemRoteiro;
using Tecnomapas.Blocos.Entities.Interno.ModuloRoteiro;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloChecagemRoteiro.Data
{
	public class ChecagemRoteiroInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public ChecagemRoteiroInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal ChecagemRoteiro Obter(int id)
		{
			ChecagemRoteiro checagem = new ChecagemRoteiro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBanco))
			{
				#region Checagem itens de roteiro

				Comando comando = bancoDeDados.CriarComando(@"select c.interessado, ls.id situacao_id, ls.texto situacao_texto, c.tid 
				from {0}tab_checagem c, {0}lov_checagem_situacao ls where c.situacao = ls.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						checagem.Id = id;
						checagem.Tid = reader.GetValue<string>("tid");
						checagem.Interessado = reader.GetValue<string>("interessado");
						checagem.Situacao = reader.GetValue<int>("situacao_id");
						checagem.SituacaoTexto = reader.GetValue<string>("situacao_texto");
					}

					reader.Close();
				}

				#endregion

				#region Roteiros

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.roteiro, r.numero, r.nome, c.tid, c.roteiro_tid, r.versao versao_cadastrada, (select hr.versao from {0}tab_roteiro hr where hr.id = c.roteiro) versao_atual, r.situacao_id 
				from {0}tab_checagem_roteiro c, {0}hst_roteiro r where c.roteiro = r.roteiro_id and c.roteiro_tid = r.tid and c.checagem = :checagem order by 4", EsquemaBanco);

				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Roteiro roteiro;
					while (reader.Read())
					{
						roteiro = new Roteiro();
						roteiro.Id = reader.GetValue<int>("roteiro");
						roteiro.IdRelacionamento = reader.GetValue<int>("id");
						roteiro.Tid = reader.GetValue<string>("roteiro_tid");
						roteiro.Nome = reader.GetValue<string>("nome");
						roteiro.VersaoAtual = reader.GetValue<int>("versao_atual");
						roteiro.Versao = reader.GetValue<int>("versao_cadastrada");
						roteiro.Situacao = reader.GetValue<int>("situacao_id");

						checagem.Roteiros.Add(roteiro);
					}

					reader.Close();
				}

				#endregion

				#region Itens

				comando = bancoDeDados.CriarComando(@"select tri.item_id id, tri.tid, tri.nome, tri.condicionante, tri.procedimento, tri.tipo_id, tri.tipo_texto, 
				ls.id situacao_id, ls.texto situacao_texto, i.motivo from {0}tab_checagem_itens i, {0}hst_roteiro_item tri, {0}lov_checagem_item_situacao ls 
				where i.item_tid = tri.tid and i.situacao = ls.id and i.checagem = :checagem", EsquemaBanco);

				comando.AdicionarParametroEntrada("checagem", checagem.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Item item;
					while (reader.Read())
					{
						item = new Item();
						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.Nome = reader.GetValue<string>("nome");
						item.Condicionante = reader.GetValue<string>("condicionante");
						item.ProcedimentoAnalise = reader.GetValue<string>("procedimento");
						item.Motivo = reader.GetValue<string>("motivo");
						item.Tipo = reader.GetValue<int>("tipo_id");
						item.TipoTexto = reader.GetValue<string>("tipo_texto");
						item.Situacao = reader.GetValue<int>("situacao_id");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");

						checagem.Itens.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return checagem;
		}
	}
}