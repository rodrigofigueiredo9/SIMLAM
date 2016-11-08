using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloMotosserra;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloMotosserra.Data
{
	class MotosserraDa
	{
		#region Propriedades

		private Historico _historico = new Historico();
		private Historico Historico { get { return _historico; } }
		private string EsquemaBanco { get; set; }

		#endregion

		#region Ações de DML

		internal void Salvar(Motosserra motosserra, BancoDeDados banco = null)
		{
			if (motosserra == null)
			{
				throw new Exception("Motosserra é nulo.");
			}

			if (motosserra.Id <= 0)
			{
				Criar(motosserra, banco);
			}
			else
			{
				Editar(motosserra, banco);
			}
		}

		private void Criar(Motosserra motosserra, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Motosserra

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_motosserra (id, possui_registro, registro_numero, serie_numero, modelo, nota_fiscal_numero, 
				proprietario, situacao, tid) values ({0}seq_motosserra.nextval, :possui_registro, " + (motosserra.PossuiRegistro ? ":registro_numero" : "{0}seq_motosserra_numero.nextval") + @", 
				:serie_numero, :modelo, :nota_fiscal_numero, :proprietario, :situacao, :tid) returning id into :id", EsquemaBanco);

				if (motosserra.PossuiRegistro)
				{
					comando.AdicionarParametroEntrada("registro_numero", motosserra.RegistroNumero, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("possui_registro", motosserra.PossuiRegistro ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("serie_numero", motosserra.SerieNumero, DbType.String);
				comando.AdicionarParametroEntrada("modelo", motosserra.Modelo, DbType.String);
				comando.AdicionarParametroEntrada("nota_fiscal_numero", motosserra.NotaFiscalNumero, DbType.String);
				comando.AdicionarParametroEntrada("proprietario", motosserra.Proprietario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eMotosserraSituacao.Ativo, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				motosserra.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion Motosserra

				Historico.Gerar(motosserra.Id, eHistoricoArtefato.motosserra, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		private void Editar(Motosserra motosserra, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Motosserra

				Comando comando = comando = bancoDeDados.CriarComando(@"");

				if (motosserra.PossuiRegistro)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_motosserra t set t.possui_registro = :possui_registro, t.registro_numero = :registro_numero, 
														t.serie_numero = :serie_numero, t.modelo = :modelo, t.nota_fiscal_numero = :nota_fiscal_numero, 
														t.proprietario = :proprietario, t.situacao = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("registro_numero", motosserra.RegistroNumero, DbType.Int32);

				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_motosserra t set t.possui_registro = :possui_registro, t.registro_numero = " + 
														((motosserra.RegistroNumero > 0) ? ":registro_numero" : "{0}seq_motosserra_numero.nextval") + @", t.serie_numero = :serie_numero, t.modelo = :modelo, 
														t.nota_fiscal_numero = :nota_fiscal_numero, t.proprietario = :proprietario, t.situacao = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);

					if (motosserra.RegistroNumero > 0)
					{
						comando.AdicionarParametroEntrada("registro_numero", motosserra.RegistroNumero, DbType.Int32);
					}
				}

				comando.AdicionarParametroEntrada("possui_registro", motosserra.PossuiRegistro ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("serie_numero", motosserra.SerieNumero, DbType.String);
				comando.AdicionarParametroEntrada("modelo", motosserra.Modelo, DbType.String);
				comando.AdicionarParametroEntrada("nota_fiscal_numero", motosserra.NotaFiscalNumero, DbType.String);
				comando.AdicionarParametroEntrada("proprietario", motosserra.Proprietario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", motosserra.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", motosserra.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Motosserra

				Historico.Gerar(motosserra.Id, eHistoricoArtefato.motosserra, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(Motosserra motosserra, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Motosserra

				Comando comando = comando = bancoDeDados.CriarComando(@"update {0}tab_motosserra t set t.situacao = :situacao, t.tid = :tid where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("situacao", motosserra.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", motosserra.Id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Motosserra

				Historico.Gerar(motosserra.Id, eHistoricoArtefato.motosserra, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_motosserra c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.motosserra, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_motosserra t where t.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		internal Motosserra Obter(int id, BancoDeDados banco = null)
		{
			Motosserra motosserra = new Motosserra();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Motosserra

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.possui_registro, t.registro_numero, t.serie_numero, t.modelo, t.nota_fiscal_numero, 
				t.proprietario, p.tipo proprietario_tipo, nvl(p.nome, p.razao_social) proprietario_nome_razao, nvl(p.cpf, p.cnpj) proprietario_cpf_cnpj, t.situacao
				from tab_motosserra t, tab_pessoa p where t.proprietario = p.id and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						motosserra.Id = id;
						motosserra.Tid = reader.GetValue<string>("tid");
						motosserra.PossuiRegistro = reader.GetValue<bool>("possui_registro");
						motosserra.RegistroNumero = reader.GetValue<int>("registro_numero");
						motosserra.SerieNumero = reader.GetValue<string>("serie_numero");
						motosserra.Modelo = reader.GetValue<string>("modelo");
						motosserra.NotaFiscalNumero = reader.GetValue<string>("nota_fiscal_numero");
						motosserra.Proprietario.Id = reader.GetValue<int>("proprietario");
						motosserra.Proprietario.Tipo = reader.GetValue<int>("proprietario_tipo");
						motosserra.Proprietario.NomeRazaoSocial = reader.GetValue<string>("proprietario_nome_razao");
						motosserra.Proprietario.CPFCNPJ = reader.GetValue<string>("proprietario_cpf_cnpj");
						motosserra.SituacaoId = reader.GetValue<int>("situacao");
					}

					reader.Close();
				}

				#endregion Motosserra
			}

			return motosserra;
		}

		internal List<Motosserra> ObterPorNumero(string numero, BancoDeDados banco = null)
		{
			List<Motosserra> motosserras = new List<Motosserra>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select m.id from {0}tab_motosserra m where trim(m.serie_numero) = (:numero)", EsquemaBanco);
				comando.AdicionarParametroEntrada("numero", numero, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						motosserras.Add(Obter(reader.GetValue<int>("id")));
					}

					reader.Close();

				}
			}

			return motosserras;

		}

		internal Resultados<Motosserra> Filtrar(Filtro<MotosserraListarFiltros> filtros, BancoDeDados banco = null)
		{
			Resultados<Motosserra> retorno = new Resultados<Motosserra>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("t.registro_numero", "registro_numero", filtros.Dados.RegistroNumero);

				comandtxt += comando.FiltroAndLike("t.serie_numero", "serie_numero", filtros.Dados.SerieNumero, true);

				comandtxt += comando.FiltroAndLike("t.modelo", "modelo", filtros.Dados.Modelo, true);

				comandtxt += comando.FiltroAndLike("t.nota_fiscal_numero", "nota_fiscal_numero", filtros.Dados.NotaFiscalNumero, true);

				comandtxt += comando.FiltroAndLike("nvl(p.nome, p.razao_social)", "nome_razao_social", filtros.Dados.PessoaNomeRazao, true);

				comandtxt += comando.FiltroAndLike("nvl(p.cpf, p.cnpj)", "cpf_cnpj", filtros.Dados.PessoaCpfCnpj, true);

				comandtxt += comando.FiltroAnd("t.situacao", "situacao", filtros.Dados.Situacao);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "registro_numero", "serie_numero", "modelo", "nota_fiscal_numero", "proprietario_nome_razao", "situacao" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("registro_numero");
				}

				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format("select count(*) from {0}tab_motosserra t, {0}tab_pessoa p where t.proprietario = p.id {1}", EsquemaBanco, comandtxt);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select t.id, t.registro_numero, t.serie_numero, t.modelo, t.nota_fiscal_numero, p.tipo proprietario_tipo, 
				nvl(p.nome, p.razao_social) proprietario_nome_razao, t.situacao from {0}tab_motosserra t, {0}tab_pessoa p where t.proprietario = p.id {1} {2}",
					EsquemaBanco, comandtxt, DaHelper.Ordenar(colunas, ordenar));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Motosserra motosserra;
					while (reader.Read())
					{
						motosserra = new Motosserra();

						motosserra.Id = reader.GetValue<int>("id");
						motosserra.RegistroNumero = reader.GetValue<int>("registro_numero");
						motosserra.SerieNumero = reader.GetValue<string>("serie_numero");
						motosserra.Modelo = reader.GetValue<string>("modelo");
						motosserra.NotaFiscalNumero = reader.GetValue<string>("nota_fiscal_numero");
						motosserra.Proprietario.Tipo = reader.GetValue<int>("proprietario_tipo");
						motosserra.Proprietario.NomeRazaoSocial = reader.GetValue<string>("proprietario_nome_razao");
						motosserra.SituacaoId = reader.GetValue<int>("situacao");

						retorno.Itens.Add(motosserra);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion

		#region Validações

		public int MotosserraCadastrada(int registroNumero, int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_motosserra t where t.registro_numero = :registro_numero and t.id != :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("registro_numero", registroNumero, DbType.Int32);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				object retorno = bancoDeDados.ExecutarScalar(comando);

				if (retorno != null && !Convert.IsDBNull(retorno))
				{
					return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
				}

				return 0;
			}
		}

		internal List<string> ValidarAssociadoTitulo(int id)
		{
			List<string> lstTitulos = new List<string>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tm.sigla, n.numero, n.ano, p.numero protocolo_numero, p.ano protocolo_ano 
					from esp_licenca_motosserra m, tab_titulo t, tab_titulo_numero n, tab_titulo_modelo tm, tab_protocolo p 
					where m.motosserra = :motosserra and m.titulo = t.id and t.id = n.titulo(+) and t.modelo = tm.id and t.protocolo = p.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("motosserra", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						string formato = "{0} - Nº{1}/{2} Protocolo: {3}/{4}";

						if (String.IsNullOrEmpty(reader.GetValue<string>("numero")))
						{
							formato = "{0} Protocolo: {3}/{4}";
						}

						lstTitulos.Add(String.Format(formato,
							reader.GetValue<string>("sigla"),
							reader.GetValue<string>("numero"),
							reader.GetValue<string>("ano"),
							reader.GetValue<string>("protocolo_numero"),
							reader.GetValue<string>("protocolo_ano")
							));
					}

					reader.Close();
				}
			}

			return lstTitulos;
		}

		internal List<String> ValidarDesativarAssociadoTitulo(int motosserra, BancoDeDados banco = null)
		{
			List<String> numerosSituacoes = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tn.numero||'/'||tn.ano titulo_numero_texto, ls.texto titulo_situacao_texto
															from {0}tab_titulo t, {0}esp_licenca_motosserra e, {0}lov_titulo_situacao ls, {0}tab_titulo_numero tn,
															{0}tab_titulo_modelo tm where t.id = e.titulo and tm.id = t.modelo and t.situacao = ls.id and tn.titulo = t.id
															and  t.situacao not in (1, 5)/*Cadastrado e Encerrado*/ and e.motosserra = :motosserra", EsquemaBanco);

				comando.AdicionarParametroEntrada("motosserra", motosserra, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						numerosSituacoes.Add(reader.GetValue<String>("titulo_numero_texto") + "|" + reader.GetValue<String>("titulo_situacao_texto"));
					}

					reader.Close();
				}
			}

			return numerosSituacoes;
		}

		internal bool ExisteNumero(string numero, int id = 0, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando;

				if (id > 0)
				{
					comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_motosserra m where trim(m.serie_numero) = trim(:numero) and m.situacao = 1/*Ativo*/ and m.id <> :id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_motosserra m where trim(m.serie_numero) = trim(:numero) and m.situacao = 1/*Ativo*/", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("numero", numero, DbType.String);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion Validações
	}
}