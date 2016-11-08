using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloProfissao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloProfissao.Data
{
	class ProfissaoDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		#region Ações de DML

		internal void Salvar(Profissao profissao, BancoDeDados banco = null)
		{
			if (profissao == null)
			{
				throw new Exception("Profissao é nulo.");
			}

			if (profissao.Id <= 0)
			{
				Criar(profissao, banco);
			}
			else
			{
				Editar(profissao, banco);
			}
		}

		private void Criar(Profissao profissao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Profissão

				Comando comando = bancoDeDados.CriarComando(@"insert into tab_profissao p (id, codigo, texto, origem, tid, texto_fonema)
				values(seq_profissao.nextval, (select 'IDAF-' || lpad(nvl((select max(substr(p.codigo, 6, 3)) from tab_profissao p
				where origem = 2), 0) + 1, 3, '0') from dual), :texto, :origem, :tid, metaphone.gerarCodigo(:texto)) 
				returning p.id, p.codigo into :id, :codigo", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", profissao.Texto, DbType.String);				
				comando.AdicionarParametroEntrada("origem", (int)eProfissaoOrigem.IDAF, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("id", DbType.Int32);
				comando.AdicionarParametroSaida("codigo", DbType.String, 36);

				bancoDeDados.ExecutarNonQuery(comando);

				profissao.Id = comando.ObterValorParametro<int>("id");
				profissao.Codigo = comando.ObterValorParametro<string>("codigo");

				#endregion

				#region Histórico
				
				Historico.Gerar(profissao.Id, eHistoricoArtefato.profissao, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		private void Editar(Profissao profissao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Profissão

				Comando comando = comando = bancoDeDados.CriarComando(@"update tab_profissao set texto = :texto, 
				texto_fonema = metaphone.gerarCodigo(:texto), tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("texto", profissao.Texto, DbType.String);								
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", profissao.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(profissao.Id, eHistoricoArtefato.profissao, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion Histórico

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		internal Profissao Obter(int id, BancoDeDados banco = null)
		{
			Profissao profissao = new Profissao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Profissão

				Comando comando = bancoDeDados.CriarComando(@"select id, texto, codigo, origem, tid from tab_profissao where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						profissao.Id = id;
						profissao.Texto = reader.GetValue<string>("texto");
						profissao.Codigo = reader.GetValue<string>("codigo");						
						profissao.OrigemId = reader.GetValue<int>("origem");
						profissao.Tid = reader.GetValue<string>("tid");												
					}

					reader.Close();
				}

				#endregion
			}

			return profissao;
		}
		
		internal Resultados<Profissao> Filtrar(Filtro<ProfissaoListarFiltros> filtros, BancoDeDados banco = null)
		{
			Resultados<Profissao> retorno = new Resultados<Profissao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("p.texto", "texto", filtros.Dados.Texto, true, true);

				List<String> ordenar = new List<String>() { "texto" };
				List<String> colunas = new List<String>() { "texto" };
				
				#endregion

				if (!string.IsNullOrEmpty(filtros.Dados.Texto))
				{
					comandtxt += @" union all select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, max(trunc(metaphone.jaro_winkler(:filtro_fonetico,p.texto),5)) 
								similaridade from tab_profissao p, lov_profissao_origem o where p.origem = o.id and p.texto_fonema like upper('%' || upper(metaphone.gerarCodigo(:filtro_fonetico)) || '%') 
								and metaphone.jaro_winkler(:filtro_fonetico,p.texto) >= to_number(:limite_similaridade) group by p.id, p.texto, p.codigo, p.tid, p.origem, o.texto";

					comando.AdicionarParametroEntrada("filtro_fonetico", filtros.Dados.Texto, DbType.String);
					comando.AdicionarParametroEntrada("limite_similaridade", ConfiguracaoSistema.LimiteSimilaridade, DbType.String);
					colunas[0] = "similaridade";
					ordenar[0] = "similaridade";
				}

				#region Executa a pesquisa nas tabelas
				comando.DbCommand.CommandText = "select count(*) from (select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, 0 similaridade from tab_profissao p, lov_profissao_origem o where p.origem = o.id " + comandtxt + ")";

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select p.id, p.texto, p.codigo, p.tid, p.origem, o.texto origem_texto, 1 similaridade 
				from tab_profissao p, lov_profissao_origem o where p.origem = o.id {0} {1}", comandtxt, DaHelper.Ordenar(colunas, ordenar, !string.IsNullOrEmpty(filtros.Dados.Texto)));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Profissao profissao;

					while (reader.Read())
					{
						profissao = new Profissao();
						profissao.Id = Convert.ToInt32(reader["id"]);

						if (retorno.Itens.Exists(x => x.Id == profissao.Id))
						{
							continue;
						}

						profissao.Tid = reader["tid"].ToString();
						profissao.Texto = reader["texto"].ToString();
						profissao.Codigo = reader["codigo"].ToString();
						profissao.OrigemId = Convert.ToInt32(reader["origem"]);
						
						retorno.Itens.Add(profissao);
					}

					reader.Close();
				}
			}
			
			return retorno;
		}
		
		#endregion

		#region Validações

		public bool Existe(Profissao profissao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_profissao where lower(texto) = :texto", EsquemaBanco);
				comando.AdicionarParametroEntrada("texto", DbType.String, 250, profissao.Texto.ToLower());

				int profissaoId = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				return profissaoId > 0 && profissaoId != profissao.Id;
			}
		}

		#endregion
	}
}