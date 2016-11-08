using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data
{
	public class GrupoQuimicoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion 

		#region Ações DML

		internal void Salvar(ConfiguracaoVegetalItem grupoQuimico, BancoDeDados banco = null)
		{
			if (grupoQuimico == null)
			{
				throw new Exception("grupoQuimico é nulo.");
			}

			if (grupoQuimico.Id <= 0)
			{
				Criar(grupoQuimico, banco);
			}
			else
			{
				Editar(grupoQuimico, banco);
			}
		}

		private void Criar(ConfiguracaoVegetalItem grupoQuimico, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Grupo Químico

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_grupo_quimico (id, texto, tid) values
				(seq_grupo_quimico.nextval, :texto, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", grupoQuimico.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				grupoQuimico.Id = comando.ObterValorParametro<int>("id");

				#endregion

				#region Histórico

				Historico.Gerar(grupoQuimico.Id, eHistoricoArtefato.grupoquimico, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(ConfiguracaoVegetalItem grupoQuimico, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Grupo Químico

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_grupo_quimico set texto = :texto, 
				tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", grupoQuimico.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", grupoQuimico.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(grupoQuimico.Id, eHistoricoArtefato.grupoquimico, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion Histórico

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Listar

		internal ConfiguracaoVegetalItem Obter(int id)
		{
			ConfiguracaoVegetalItem grupoQuimico = new ConfiguracaoVegetalItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Grupo Químico

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_grupo_quimico where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						grupoQuimico.Id = id;
						grupoQuimico.Texto = reader.GetValue<string>("texto");						
						grupoQuimico.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return grupoQuimico;
		}

		internal List<ConfiguracaoVegetalItem> Listar()
		{
			List<ConfiguracaoVegetalItem> lista = new List<ConfiguracaoVegetalItem>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Grupos Químicos

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_grupo_quimico order by texto", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						lista.Add(new ConfiguracaoVegetalItem 
						{
							Id = reader.GetValue<int>("id"),
							Texto = reader.GetValue<string>("texto"),
							Tid = reader.GetValue<string>("tid")
						});						
					}

					reader.Close();
				}

				#endregion 
			}

			return lista;			
		}

		#endregion

		#region Validações

		public bool Existe(ConfiguracaoVegetalItem grupoQuimico, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_grupo_quimico where lower(texto) = :texto", EsquemaBanco);
				comando.AdicionarParametroEntrada("texto", DbType.String, 250, grupoQuimico.Texto.ToLower());

				int grupoQuimicoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return grupoQuimicoId > 0 && grupoQuimicoId != grupoQuimico.Id;
			}
		}

		#endregion
	}
}
