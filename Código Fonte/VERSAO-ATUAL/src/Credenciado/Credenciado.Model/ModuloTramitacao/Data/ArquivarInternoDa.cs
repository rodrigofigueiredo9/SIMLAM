using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloTramitacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTramitacao.Data
{
	public class ArquivarInternoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		#endregion

		public ArquivarInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		internal Arquivar ObterArquivamento(int tramitacaoId, BancoDeDados banco = null)
		{
			Arquivar arquivar = new Arquivar();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
									select f.id          funcionario_id,
											f.nome        funcionario_nome,
											ta.tramitacao tamitacao_id,
											a.id          arquivo_id,
											a.nome        arquivo_nome,
											te.id         estante_id,
											te.nome       estante_nome,
											tp.id         prateleira_id,
											tp.identificacao       prateleira_nome,
											s.id          setor_id,
											s.nome        setor_nome
										from {0}tab_funcionario               f,
											 {0}tab_tramitacao                t,
											 {0}tab_tramitacao_arquivar       ta,
											 {0}tab_tramitacao_arquivo        a,
											 {0}tab_tramitacao_arq_estante    te,
											 {0}tab_tramitacao_arq_prateleira tp,
											 {0}tab_setor                     s
										where te.id = ta.estante
										and tp.id = ta.prateleira
										and a.id = ta.arquivo
										and s.id = a.setor
										and t.id = ta.tramitacao
										and f.id = t.remetente
										and ta.tramitacao = :tramitacaoid", EsquemaBanco);

				comando.AdicionarParametroEntrada("tramitacaoid", tramitacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						arquivar.ArquivoId = Convert.ToInt32(reader["arquivo_id"]);
						arquivar.ArquivoNome = reader["arquivo_nome"].ToString();

						arquivar.EstanteId = Convert.ToInt32(reader["estante_id"]);
						arquivar.EstanteNome = reader["estante_nome"].ToString();

						arquivar.PrateleiraId = Convert.ToInt32(reader["prateleira_id"]);
						arquivar.PrateleiraNome = reader["prateleira_nome"].ToString();

						arquivar.SetorId = Convert.ToInt32(reader["setor_id"]);
						arquivar.SetorNome = reader["setor_nome"].ToString();

						arquivar.Funcionario.Id = Convert.ToInt32(reader["funcionario_id"]);
						arquivar.Funcionario.Nome = reader["funcionario_nome"].ToString();
					}

					reader.Close();
				}
			}

			return arquivar;
		}
	}
}