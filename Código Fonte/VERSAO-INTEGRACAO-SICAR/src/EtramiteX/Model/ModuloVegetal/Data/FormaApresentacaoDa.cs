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
	public class FormaApresentacaoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion 

		#region Ações DML

		internal void Salvar(ConfiguracaoVegetalItem formaApresentacao, BancoDeDados banco = null)
		{
			if (formaApresentacao == null)
			{
				throw new Exception("formaApresentacao é nulo.");
			}

			if (formaApresentacao.Id <= 0)
			{
				Criar(formaApresentacao, banco);
			}
			else
			{
				Editar(formaApresentacao, banco);
			}
		}

		private void Criar(ConfiguracaoVegetalItem formaApresentacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Forma de apresentação

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_forma_apresentacao (id, texto, tid) values
				(seq_forma_apresentacao.nextval, :texto, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", formaApresentacao.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				formaApresentacao.Id = comando.ObterValorParametro<int>("id");

				#endregion 

				#region Histórico

				Historico.Gerar(formaApresentacao.Id, eHistoricoArtefato.formaapresentacao, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(ConfiguracaoVegetalItem formaApresentacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Forma de apresentação

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_forma_apresentacao set texto = :texto, 
				tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", formaApresentacao.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", formaApresentacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion 

				#region Histórico

				Historico.Gerar(formaApresentacao.Id, eHistoricoArtefato.formaapresentacao, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion Histórico

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Listar

		internal ConfiguracaoVegetalItem Obter(int id)
		{
			ConfiguracaoVegetalItem formaApresentacao = new ConfiguracaoVegetalItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Forma de apresentação

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_forma_apresentacao where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						formaApresentacao.Id = id;
						formaApresentacao.Texto = reader.GetValue<string>("texto");
						formaApresentacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return formaApresentacao;
		}

		internal List<ConfiguracaoVegetalItem> Listar()
		{
			List<ConfiguracaoVegetalItem> lista = new List<ConfiguracaoVegetalItem>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Formas de apresentação

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_forma_apresentacao order by texto", EsquemaBanco);

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

		public bool Existe(ConfiguracaoVegetalItem formaApresentacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_forma_apresentacao where lower(texto) = :texto", EsquemaBanco);
				comando.AdicionarParametroEntrada("texto", DbType.String, 250, formaApresentacao.Texto.ToLower());

				int formaApresentacaoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return formaApresentacaoId > 0 && formaApresentacaoId != formaApresentacao.Id;
			}
		}

		#endregion
	}
}
