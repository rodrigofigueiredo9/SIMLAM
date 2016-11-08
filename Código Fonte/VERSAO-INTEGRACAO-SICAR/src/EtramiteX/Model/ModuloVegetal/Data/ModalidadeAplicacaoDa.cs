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
	public class ModalidadeAplicacaoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion 

		#region Ações DML

		internal void Salvar(ConfiguracaoVegetalItem modalidadeAplicacao, BancoDeDados banco = null)
		{
			if (modalidadeAplicacao == null)
			{
				throw new Exception("modalidadeAplicacao é nulo.");
			}

			if (modalidadeAplicacao.Id <= 0)
			{
				Criar(modalidadeAplicacao, banco);
			}
			else
			{
				Editar(modalidadeAplicacao, banco);
			}
		}

		private void Criar(ConfiguracaoVegetalItem modalidadeAplicacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Modalidade de Aplicação

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_modalidade_aplicacao (id, texto, tid) values
				(seq_modalidade_aplicacao.nextval, :texto, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", modalidadeAplicacao.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				modalidadeAplicacao.Id = comando.ObterValorParametro<int>("id");

				#endregion

				#region Histórico

				Historico.Gerar(modalidadeAplicacao.Id, eHistoricoArtefato.modalidadeaplicacao, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(ConfiguracaoVegetalItem modalidadeAplicacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Modalidade de Aplicação

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_modalidade_aplicacao set texto = :texto, 
				tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", modalidadeAplicacao.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", modalidadeAplicacao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(modalidadeAplicacao.Id, eHistoricoArtefato.modalidadeaplicacao, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion Histórico

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Listar

		internal ConfiguracaoVegetalItem Obter(int id)
		{
			ConfiguracaoVegetalItem modalidade = new ConfiguracaoVegetalItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Modalidade de Aplicação

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_modalidade_aplicacao where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						modalidade.Id = id;
						modalidade.Texto = reader.GetValue<string>("texto");
						modalidade.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return modalidade;
		}

		internal List<ConfiguracaoVegetalItem> Listar()
		{
			List<ConfiguracaoVegetalItem> lista = new List<ConfiguracaoVegetalItem>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Modalidades de Aplicação

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, tid from tab_modalidade_aplicacao order by texto", EsquemaBanco);

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

		public bool Existe(ConfiguracaoVegetalItem modalidadeAplicacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_modalidade_aplicacao where lower(texto) = :texto", EsquemaBanco);
				comando.AdicionarParametroEntrada("texto", DbType.String, 250, modalidadeAplicacao.Texto.ToLower());

				int modalidadeAplicacaoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return modalidadeAplicacaoId > 0 && modalidadeAplicacaoId != modalidadeAplicacao.Id;
			}
		}

		#endregion
	}
}
