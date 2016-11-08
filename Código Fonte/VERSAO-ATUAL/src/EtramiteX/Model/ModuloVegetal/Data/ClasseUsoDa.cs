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
	public class ClasseUsoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		#region Ações DML

		internal void Salvar(ConfiguracaoVegetalItem classeUso, BancoDeDados banco = null)
		{
			if (classeUso == null)
			{
				throw new Exception("classeUso é nulo.");
			}

			if (classeUso.Id <= 0)
			{
				Criar(classeUso, banco);
			}
			else
			{
				Editar(classeUso, banco);
			}
		}

		private void Criar(ConfiguracaoVegetalItem classeUso, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Classe de Uso

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_classe_uso (id, texto, tid) values
				(seq_classe_uso.nextval, :texto, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", classeUso.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				classeUso.Id = comando.ObterValorParametro<int>("id");

				#endregion

				#region Histórico

				Historico.Gerar(classeUso.Id, eHistoricoArtefato.classeuso, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(ConfiguracaoVegetalItem classeUso, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Classe de Uso

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_classe_uso set texto = :texto, 
				tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", classeUso.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", classeUso.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(classeUso.Id, eHistoricoArtefato.classeuso, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion Histórico

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Listar

		internal ConfiguracaoVegetalItem Obter(int id)
		{
			ConfiguracaoVegetalItem classeUso = new ConfiguracaoVegetalItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Classe de Uso

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_classe_uso where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						classeUso.Id = id;
						classeUso.Texto = reader.GetValue<string>("texto");
						classeUso.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return classeUso;
		}

		internal List<ConfiguracaoVegetalItem> Listar()
		{
			List<ConfiguracaoVegetalItem> lista = new List<ConfiguracaoVegetalItem>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Classes de Uso

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_classe_uso order by texto", EsquemaBanco);

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

		public bool Existe(ConfiguracaoVegetalItem classeUso, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_classe_uso where lower(texto) = :texto", EsquemaBanco);
				comando.AdicionarParametroEntrada("texto", DbType.String, 250, classeUso.Texto.ToLower());

				int classeUsoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return classeUsoId > 0 && classeUsoId != classeUso.Id;
			}
		}

		#endregion
	}
}