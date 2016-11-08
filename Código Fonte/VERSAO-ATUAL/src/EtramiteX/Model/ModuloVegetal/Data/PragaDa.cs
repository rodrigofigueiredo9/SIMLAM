using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;


namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data
{
	public class PragaDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		private CulturaDa _culturaDa = new CulturaDa();

		#endregion

		#region Ações DML

		internal void Salvar(Praga praga, BancoDeDados banco = null)
		{
			if (praga == null)
			{
				throw new Exception("Objeto é nulo.");
			}

			if (praga.Id <= 0)
			{
				Criar(praga, banco);
			}
			else
			{
				Editar(praga, banco);
			}
		}

		private void Criar(Praga praga, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Praga

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_praga (id, nome_cientifico, nome_comum, tid) values
				(seq_praga.nextval, :nome_cientifico, :nome_comum, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome_cientifico", praga.NomeCientifico, DbType.String);
				comando.AdicionarParametroEntrada("nome_comum", praga.NomeComum, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				praga.Id = comando.ObterValorParametro<int>("id");
				#endregion

				#region Histórico

				Historico.Gerar(praga.Id, eHistoricoArtefato.praga, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(Praga praga, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Praga

				Comando comando = bancoDeDados.CriarComando(@"update tab_praga set nome_cientifico = :nome_cientifico, nome_comum = :nome_comum, 
				tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("nome_cientifico", praga.NomeCientifico, DbType.String);
				comando.AdicionarParametroEntrada("nome_comum", praga.NomeComum, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", praga.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(praga.Id, eHistoricoArtefato.praga, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion Histórico

				bancoDeDados.Commit();
			}
		}

		public void AssociarCulturas(Praga praga, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				//Atualizar o tid para a nova ação
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_praga t set t.tid = :tid where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", praga.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				#region Deletando


				comando = bancoDeDados.CriarComando(@"delete from tab_praga_cultura pc where pc.praga = :praga_id ", EsquemaBanco);
                comando.DbCommand.CommandText += comando.AdicionarNotIn("and", "pc.id", DbType.Int32, praga.Culturas.Select(x =>x.IdRelacionamento).ToList());
                comando.AdicionarParametroEntrada("praga_id", praga.Id, DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion


				#region Inserindo e Alterando

				foreach (Cultura cult in praga.Culturas)
				{

                    if (cult.IdRelacionamento > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"update tab_praga_cultura set praga = :praga_id, cultura = :cultura_id, tid =:tid where id = :id_rel");
                        comando.AdicionarParametroEntrada("id_rel", cult.IdRelacionamento, DbType.Int32);
                    }
                    else 
                    {
                        comando = bancoDeDados.CriarComando("insert into tab_praga_cultura (id, praga, cultura, tid) values (seq_praga_cultura.nextval, :praga_id, :cultura_id, :tid)");
                    }

                    comando.AdicionarParametroEntrada("praga_id", praga.Id, DbType.String);
                    comando.AdicionarParametroEntrada("cultura_id", cult.Id ,DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(praga.Id, eHistoricoArtefato.praga, eHistoricoAcao.associar, bancoDeDados);

				bancoDeDados.Commit();

			}
		}

		#endregion

		#region Obter/Listar

		internal Praga Obter(int id, BancoDeDados banco = null)
		{
			Praga praga = new Praga();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region praga

				Comando comando = bancoDeDados.CriarComando(@"select id, nome_cientifico, nome_comum, tid from tab_praga where id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						praga.Id = id;
						praga.NomeCientifico = reader.GetValue<string>("nome_cientifico");
						praga.NomeComum = reader.GetValue<string>("nome_comum");
						praga.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion
			}

			return praga;
		}

		internal List<Praga> ObterPragas(int idCultura, BancoDeDados banco = null)
		{
			List<Praga> pragas = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select distinct t.id, t.nome_cientifico, t.nome_comum from {0}tab_praga t, {0}tab_praga_cultura pc
				where t.id = pc.praga and pc.cultura = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", idCultura, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					pragas = new List<Praga>();

					while(reader.Read())
					{
						pragas.Add(new Praga() { 
							Id = reader.GetValue<int>("id"),
							NomeCientifico = reader.GetValue<string>("nome_cientifico"),
							NomeComum = reader.GetValue<string>("nome_comum")
						});		
					}

					reader.Close();
				}
			}

			return pragas;
		}

		internal Resultados<Praga> Filtrar(Filtro<Praga> filtros, BancoDeDados banco = null)
		{
			Resultados<Praga> retorno = new Resultados<Praga>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt = comando.FiltroAndLike("p.nome_cientifico", "nome_cientifico", filtros.Dados.NomeCientifico, true, true);
				comandtxt += comando.FiltroAndLike("p.nome_comum", "nome_comum", filtros.Dados.NomeComum, true, true);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome_cientifico", "nome_comum" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome_cientifico");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from tab_praga p where p.id > 0" + comandtxt);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select p.id, p.nome_cientifico, p.nome_comum, stragg(c.texto) cultura, p.tid from tab_praga p, tab_praga_cultura pc, tab_cultura c where p.id = pc.praga(+) and pc.cultura = c.id(+) " + comandtxt + " group by p.id, p.nome_cientifico, p.nome_comum, p.tid " + DaHelper.Ordenar(colunas, ordenar));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Praga item;

					while (reader.Read())
					{
						item = new Praga();

						item.Id = reader.GetValue<int>("id");
						item.NomeCientifico = reader.GetValue<string>("nome_cientifico");
						item.NomeComum = reader.GetValue<string>("nome_comum");
						item.CulturasTexto = reader.GetValue<string>("cultura");
						item.Tid = reader.GetValue<string>("tid");
						retorno.Itens.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal List<Cultura> ObterCulturas(int pragaId, BancoDeDados banco = null)
		{
			List<Cultura> retorno = new List<Cultura>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select pc.id id_rel, c.id, c.texto nome from tab_praga p, tab_praga_cultura pc, tab_cultura c where pc.cultura = c.id 
				and pc.praga = p.id and p.id = :id");
				comando.AdicionarParametroEntrada("id", pragaId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Cultura item;

					while (reader.Read())
					{
						item = new Cultura();
						item = _culturaDa.Obter(reader.GetValue<int>("id"));
                        item.IdRelacionamento = reader.GetValue<int>("id_rel");
						retorno.Add(item);
					}
					reader.Close();
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		public bool Existe(Praga praga, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_praga where lower(nome_cientifico) = :nome_cientifico", EsquemaBanco);
				comando.AdicionarParametroEntrada("nome_cientifico", DbType.String, 100, praga.NomeCientifico.ToLower());

				int pragaId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return pragaId > 0 && pragaId != praga.Id;
			}
		}

		#endregion
	}
}