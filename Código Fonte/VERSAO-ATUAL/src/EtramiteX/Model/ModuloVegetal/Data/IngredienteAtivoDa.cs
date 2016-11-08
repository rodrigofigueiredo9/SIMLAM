using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloVegetal.Data
{
	public class IngredienteAtivoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion 

		#region Ações DML

		internal void Salvar(ConfiguracaoVegetalItem ingredienteAtivo, BancoDeDados banco = null)
		{
			if (ingredienteAtivo == null)
			{
				throw new Exception("Ingrediente Ativo é nulo.");
			}

			if (ingredienteAtivo.Id <= 0)
			{
				Criar(ingredienteAtivo, banco);
			}
			else
			{
				Editar(ingredienteAtivo, banco);
			}
		}

		private void Criar(ConfiguracaoVegetalItem ingredienteAtivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_ingrediente_ativo (id, situacao, texto, tid) values
				(seq_ingrediente_ativo.nextval, :situacao, :texto, :tid) returning id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", eIngredienteAtivoSituacao.Ativo, DbType.Int32);
				comando.AdicionarParametroEntrada("texto", DbType.String, 100, ingredienteAtivo.Texto);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				ingredienteAtivo.Id = comando.ObterValorParametro<int>("id");

				Historico.Gerar(ingredienteAtivo.Id, eHistoricoArtefato.ingredienteativo, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		private void Editar(ConfiguracaoVegetalItem ingredienteAtivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_ingrediente_ativo set texto = :texto, 
				tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", DbType.String, 100, ingredienteAtivo.Texto);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", ingredienteAtivo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(ingredienteAtivo.Id, eHistoricoArtefato.ingredienteativo, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(ConfiguracaoVegetalItem ingredienteAtivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = comando = bancoDeDados.CriarComando(@"update {0}tab_ingrediente_ativo t set t.situacao = :situacao, t.motivo = :motivo, t.tid = :tid 
				where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", ingredienteAtivo.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", DbType.String, 250, ingredienteAtivo.Motivo);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", ingredienteAtivo.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(ingredienteAtivo.Id, eHistoricoArtefato.ingredienteativo, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Listar

		internal ConfiguracaoVegetalItem Obter(int id)
		{
			ConfiguracaoVegetalItem ingredienteAtivo = new ConfiguracaoVegetalItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select i.id, i.tid, i.texto, i.situacao, s.texto situacao_texto, i.motivo 
				from tab_ingrediente_ativo i, lov_ingrediente_ativo_situacao s 
				where i.situacao = s.id and i.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						ingredienteAtivo.Id = id;
						ingredienteAtivo.Texto = reader.GetValue<string>("texto");
						ingredienteAtivo.SituacaoId = reader.GetValue<int>("situacao");
						ingredienteAtivo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						ingredienteAtivo.Motivo = reader.GetValue<string>("motivo");
						ingredienteAtivo.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}
			}

			return ingredienteAtivo;
		}

		internal void ObterValores(List<ConfiguracaoVegetalItem> itens)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select i.id, i.tid, i.texto, i.situacao, s.texto situacao_texto, i.motivo 
				from tab_ingrediente_ativo i, lov_ingrediente_ativo_situacao s 
				where i.situacao = s.id ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarIn("and", "i.id", DbType.Int32, itens.Select(x => x.Id).ToList());

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConfiguracaoVegetalItem ingredienteAtivo;

					while (reader.Read())
					{
						ingredienteAtivo = itens.Single(x => x.Id == reader.GetValue<int>("id"));

						ingredienteAtivo.Texto = reader.GetValue<string>("texto");
						ingredienteAtivo.SituacaoId = reader.GetValue<int>("situacao");
						ingredienteAtivo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						ingredienteAtivo.Motivo = reader.GetValue<string>("motivo");
						ingredienteAtivo.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}
			}
		}

		internal Resultados<ConfiguracaoVegetalItem> Filtrar(Filtro<ConfiguracaoVegetalItem> filtros)
		{
			Resultados<ConfiguracaoVegetalItem> retorno = new Resultados<ConfiguracaoVegetalItem>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("i.texto", "texto", filtros.Dados.Texto, true, true);

				comandtxt += comando.FiltroAnd("i.situacao", "situacao", filtros.Dados.SituacaoId);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "texto", "situacao" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("texto");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_ingrediente_ativo i where i.id > 0 " + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select i.id, i.tid, i.texto, i.situacao, s.texto situacao_texto, i.motivo 
				from {0}tab_ingrediente_ativo i, {0}lov_ingrediente_ativo_situacao s where i.situacao = s.id " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ConfiguracaoVegetalItem item;

					while (reader.Read())
					{
						item = new ConfiguracaoVegetalItem();

						item.Id = reader.GetValue<int>("id");
						item.Tid = reader.GetValue<string>("tid");
						item.Texto = reader.GetValue<string>("texto");
						item.SituacaoId = reader.GetValue<int>("situacao");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.Motivo = reader.GetValue<string>("motivo");

						retorno.Itens.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		public bool Existe(ConfiguracaoVegetalItem ingredienteAtivo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_ingrediente_ativo where lower(texto) = :texto", EsquemaBanco);
				comando.AdicionarParametroEntrada("texto", DbType.String, 250, ingredienteAtivo.Texto.ToLower());

				int retorno = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return retorno > 0 && retorno != ingredienteAtivo.Id;
			}
		}

		#endregion
	}
}