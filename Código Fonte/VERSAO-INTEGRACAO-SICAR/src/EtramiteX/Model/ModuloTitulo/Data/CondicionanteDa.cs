using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloTitulo.Data
{
	public class CondicionanteDa
	{
		#region Propriedades

		Historico _historico = new Historico();

		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public CondicionanteDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TituloCondicionante condicionante, BancoDeDados banco)
		{
			if (condicionante == null)
			{
				throw new Exception("A Condicionante é nula.");
			}

			if (condicionante.Id <= 0)
			{
				Criar(condicionante, banco);
			}
			else
			{
				Editar(condicionante, banco);
			}
		}

		internal int? Criar(TituloCondicionante condicionante, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Condicionante de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_condicionantes c (id, titulo, situacao, descricao, possui_prazo, prazo, periodicidade, 
				periodo, periodo_tipo, data_vencimento, ordem, tid) values ({0}seq_titulo_condicionantes.nextval, :titulo, :situacao, :descricao, :possui_prazo, :prazo, :periodicidade, 
				:periodo, :periodo_tipo, :data_vencimento, :ordem, :tid) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("titulo", condicionante.tituloId, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", condicionante.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntClob("descricao", condicionante.Descricao);
				comando.AdicionarParametroEntrada("possui_prazo", (condicionante.PossuiPrazo) ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("prazo", (condicionante.Prazo.HasValue) ? condicionante.Prazo : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("periodicidade", (condicionante.PossuiPeriodicidade) ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("periodo", (condicionante.PeriodicidadeValor.HasValue) ? condicionante.PeriodicidadeValor : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("periodo_tipo", (condicionante.PeriodicidadeTipo.Id > 0) ? condicionante.PeriodicidadeTipo.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("data_vencimento", (condicionante.DataVencimento.Data.HasValue) ? condicionante.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("ordem", condicionante.Ordem, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				condicionante.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Histórico

				Historico.Gerar(condicionante.Id, eHistoricoArtefato.titulocondicionante, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return condicionante.Id;
			}
		}

		internal void Editar(TituloCondicionante condicionante, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Condicionante de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_condicionantes c set c.situacao = :situacao, c.descricao = :descricao, c.possui_prazo = :possui_prazo, 
				c.prazo = :prazo, c.periodicidade = :periodicidade, c.periodo = :periodo, c.periodo_tipo = :periodo_tipo, c.data_vencimento = :data_vencimento, c.ordem = :ordem, c.tid = :tid where c.id =: id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", condicionante.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", condicionante.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntClob("descricao", condicionante.Descricao);
				comando.AdicionarParametroEntrada("possui_prazo", (condicionante.PossuiPrazo) ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("prazo", (condicionante.Prazo.HasValue) ? condicionante.Prazo : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("periodicidade", (condicionante.PossuiPeriodicidade) ? 1 : 0, DbType.Int32);
				comando.AdicionarParametroEntrada("periodo", (condicionante.PeriodicidadeValor.HasValue) ? condicionante.PeriodicidadeValor : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("periodo_tipo", (condicionante.PeriodicidadeTipo.Id > 0) ? condicionante.PeriodicidadeTipo.Id : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("data_vencimento", (condicionante.DataVencimento.Data.HasValue) ? condicionante.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("ordem", condicionante.Ordem, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(condicionante.Id, eHistoricoArtefato.titulocondicionante, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(TituloCondicionante condicionante, TituloCondicionantePeriodicidade periodicidade = null, BancoDeDados banco = null)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Condicionante de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = null;

				if (periodicidade == null)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_condicionantes c set c.situacao = :situacao, c.tid = :tid where c.id =: id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", condicionante.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("situacao", condicionante.Situacao.Id, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_condicionantes_peri c set c.situacao = :situacao, c.tid = :tid where c.id =: id", EsquemaBanco);
					comando.AdicionarParametroEntrada("id", periodicidade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("situacao", periodicidade.Situacao.Id, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(condicionante.Id, eHistoricoArtefato.titulocondicionante, eHistoricoAcao.alterarsituacao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Ativar(TituloCondicionante condicionante, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Condicionante de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_condicionantes c set c.situacao = :situacao, c.data_vencimento = :data_vencimento, 
					data_inicio = :data_inicio, c.tid = :tid where c.id =: id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", condicionante.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", condicionante.Situacao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data_inicio", (condicionante.DataInicioPrazo.Data.HasValue) ? condicionante.DataInicioPrazo.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_vencimento", (condicionante.DataVencimento.Data.HasValue) ? condicionante.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);


				//Limpa periodicidade
				comando = bancoDeDados.CriarComando(@"delete {0}tab_titulo_condicionantes_peri c where c.condicionante =: condicionante", EsquemaBanco);
				comando.AdicionarParametroEntrada("condicionante", condicionante.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);


				//Limpa gerar novas 
				comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_condicionantes_peri (id, titulo, condicionante, situacao, dias_prorrogados, 
					data_inicio, data_vencimento, tid) values (seq_titulo_condicionante_per.nextval, :titulo, :condicionante, :situacao, :dias_prorrogados, 
					:data_inicio, :data_vencimento, :tid)", EsquemaBanco);

				foreach (var item in condicionante.Periodicidades)
				{
					comando.DbCommand.Parameters.Clear();

					comando.AdicionarParametroEntrada("titulo", condicionante.tituloId, DbType.Int32);
					comando.AdicionarParametroEntrada("condicionante", condicionante.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("situacao", item.Situacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("dias_prorrogados", DBNull.Value, DbType.Int32);
					comando.AdicionarParametroEntrada("data_inicio", (item.DataInicioPrazo.Data.HasValue) ? item.DataInicioPrazo.Data : (object)DBNull.Value, DbType.DateTime);
					comando.AdicionarParametroEntrada("data_vencimento", (item.DataVencimento.Data.HasValue) ? item.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					bancoDeDados.ExecutarNonQuery(comando);
				}
				#endregion

				#region Histórico

				Historico.Gerar(condicionante.Id, eHistoricoArtefato.titulocondicionante, eHistoricoAcao.alterarsituacao, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Prorrogar(TituloCondicionante condicionante, TituloCondicionantePeriodicidade periodicidade, BancoDeDados banco = null)
		{
			GerenciadorTransacao.ObterIDAtual();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Condicionante de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = null;

				if (periodicidade == null)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_condicionantes c set c.possui_prazo = 1, situacao = :situacao,
						c.dias_prorrogados = :diasprorrogados, c.data_vencimento = :data_vencimento, c.tid = :tid where c.id =: id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", condicionante.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("situacao", condicionante.Situacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("diasprorrogados", (condicionante.DiasProrrogados.HasValue) ? condicionante.DiasProrrogados : 0, DbType.Int32);
					comando.AdicionarParametroEntrada("data_vencimento", (condicionante.DataVencimento.Data.HasValue) ? condicionante.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update tab_titulo_condicionantes_peri p set p.situacao = :situacao, 
						p.dias_prorrogados = :diasprorrogados, p.data_vencimento = :data_vencimento, p.tid = :tid where p.id =: id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", periodicidade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("situacao", periodicidade.Situacao.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("diasprorrogados", (periodicidade.DiasProrrogados.HasValue) ? periodicidade.DiasProrrogados : 0, DbType.Int32);
					comando.AdicionarParametroEntrada("data_vencimento", (periodicidade.DataVencimento.Data.HasValue) ? periodicidade.DataVencimento.Data : (object)DBNull.Value, DbType.DateTime);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				}

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(condicionante.Id, eHistoricoArtefato.titulocondicionante, eHistoricoAcao.prorrogar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Atualizar o tid para a nova ação

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_condicionantes c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);
				#endregion

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.titulocondicionante, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da condicionante

				comando = bancoDeDados.CriarComando(@"begin delete from {0}tab_titulo_condicionantes e where e.id = :condicionante; end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("condicionante", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		internal void DescricaoSalvar(TituloCondicionanteDescricao descricao, BancoDeDados banco = null)
		{
			if (descricao == null)
			{
				throw new Exception("A Descrição da Condicionante é nula.");
			}

			if (descricao.Id <= 0)
			{
				DescricaoCriar(descricao, banco);
			}
			else
			{
				DescricaoEditar(descricao, banco);
			}
		}

		internal int? DescricaoCriar(TituloCondicionanteDescricao descricao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Condicionante de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}tab_titulo_condicionante_desc c (id, descricao, tid) 
				values ({0}seq_titulo_condicionantes.nextval, :descricao, :tid) returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("descricao", descricao.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				descricao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Histórico

				Historico.Gerar(descricao.Id, eHistoricoArtefato.titulocondicionantedescricao, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return descricao.Id;
			}
		}

		internal void DescricaoEditar(TituloCondicionanteDescricao descricao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				#region Condicionante de Título

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_titulo_condicionante_desc c set c.descricao = :descricao, c.tid = :tid where c.id =: id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", descricao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("descricao", descricao.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				Historico.Gerar(descricao.Id, eHistoricoArtefato.titulocondicionantedescricao, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void DescricaoExcluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Histórico

				Historico.Gerar(id, eHistoricoArtefato.titulocondicionantedescricao, eHistoricoAcao.excluir, bancoDeDados);

				#endregion

				#region Apaga os dados da condicionante

				Comando comando = bancoDeDados.CriarComando(@"begin delete from {0}tab_titulo_condicionante_desc e where e.id = :condicionante; end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("condicionante", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();

				#endregion
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Resultados<TituloCondicionante> Filtrar(Filtro<TituloCondicionanteFiltro> filtros)
		{
			Resultados<TituloCondicionante> retorno = new Resultados<TituloCondicionante>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("c.descricao", "descricao", filtros.Dados.Descricao, true);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "titulo_numero", "situacao_texto", "descricao" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("titulo_numero");
				}
				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_titulo_condicionantes c, tab_titulo t, lov_titulo_cond_situacao ls
				where c.situacao = ls.id and c.titulo = t.id" + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select c.id, c.titulo, t.numero titulo_numero, ls.id situacao_id, ls.texto situacao_texto, c.descricao, c.possui_prazo, 
				c.prazo, c.periodicidade, c.periodo, c.tid from {0}tab_titulo_condicionantes c, {0}tab_titulo t, {0}lov_titulo_cond_situacao ls
				where c.situacao = ls.id and c.titulo = t.id" + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno
					TituloCondicionante condicionante;
					while (reader.Read())
					{
						condicionante = new TituloCondicionante();
						condicionante.Id = Convert.ToInt32(reader["id"]);
						condicionante.Tid = reader["tid"].ToString();
						condicionante.tituloId = Convert.ToInt32(reader["titulo"]);
						condicionante.tituloNumero = reader["titulo_numero"].ToString();
						condicionante.Situacao.Id = Convert.ToInt32(reader["situacao_id"]);
						condicionante.Situacao.Texto = reader["situacao_texto"].ToString();
						condicionante.Descricao = reader["descricao"].ToString();

						condicionante.PossuiPrazo = Convert.ToInt32(reader["possui_prazo"]) > 0;

						if (reader["prazo"] != null && !Convert.IsDBNull(reader["prazo"]))
						{
							condicionante.Prazo = Convert.ToInt32(reader["prazo"]);
						}

						condicionante.PossuiPeriodicidade = Convert.ToInt32(reader["periodicidade"]) > 0;

						if (reader["periodo"] != null && !Convert.IsDBNull(reader["periodo"]))
						{
							condicionante.PeriodicidadeValor = Convert.ToInt32(reader["periodo"]);
						}

						retorno.Itens.Add(condicionante);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal Resultados<TituloCondicionante> FiltrarDescricao(Filtro<TituloCondicionanteFiltro> filtros)
		{
			Resultados<TituloCondicionante> retorno = new Resultados<TituloCondicionante>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("descricao", "descricao", filtros.Dados.Descricao, true);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "to_char(descricao)" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("to_char(descricao)");
				}
				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_titulo_condicionante_desc c where c.id > 0 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Padrao.NaoEncontrouRegistros);
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select id, descricao, tid from {0}tab_titulo_condicionante_desc c where c.id > 0 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno
					TituloCondicionante condicionante;
					while (reader.Read())
					{
						condicionante = new TituloCondicionante();

						condicionante.Id = Convert.ToInt32(reader["id"]);
						condicionante.Tid = reader["tid"].ToString();
						condicionante.Descricao = reader["descricao"].ToString();

						retorno.Itens.Add(condicionante);
					}

					reader.Close();

					#endregion
				}
			}
			return retorno;
		}

		internal TituloCondicionante Obter(int id, BancoDeDados banco = null)
		{
			TituloCondicionante condicionante = new TituloCondicionante();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Condicionante de Título

				Comando comando = bancoDeDados.CriarComando(@"select 
				(select min(htc.data_execucao) idc from hst_titulo_condicionantes htc where htc.condicionante_id = c.id) data_criacao,
				c.id, c.titulo, (case when tn.numero is not null then tn.numero||'/'||tn.ano else null end) titulo_numero,
				ls.id situacao_id, ls.texto situacao_texto, c.descricao, c.possui_prazo, c.data_vencimento, c.data_inicio, c.dias_prorrogados,
				c.prazo, c.periodicidade, c.periodo, lp.id periodo_tipo_id, lp.texto periodo_tipo_texto, c.tid from {0}tab_titulo_condicionantes c, {0}tab_titulo t, {0}tab_titulo_numero tn, 
				{0}lov_titulo_cond_situacao ls, {0}lov_titulo_cond_periodo_tipo lp where t.id = tn.titulo(+) and c.titulo = t.id and c.situacao = ls.id and c.periodo_tipo = lp.id(+) and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						condicionante.Id = id;
						condicionante.Tid = reader["tid"].ToString();
						condicionante.tituloId = Convert.ToInt32(reader["titulo"]);
						condicionante.tituloNumero = reader["titulo_numero"].ToString();

						condicionante.Situacao.Id = Convert.ToInt32(reader["situacao_id"]);
						condicionante.Situacao.Texto = reader["situacao_texto"].ToString();
						condicionante.Descricao = reader["descricao"].ToString();

						condicionante.PossuiPrazo = Convert.ToInt32(reader["possui_prazo"]) > 0;

						if (reader["prazo"] != null && !Convert.IsDBNull(reader["prazo"]))
						{
							condicionante.Prazo = Convert.ToInt32(reader["prazo"]);
						}

						if (reader["dias_prorrogados"] != null && !Convert.IsDBNull(reader["dias_prorrogados"]))
						{
							condicionante.DiasProrrogados = Convert.ToInt32(reader["dias_prorrogados"]);
						}

						if (reader["data_inicio"] != null && !Convert.IsDBNull(reader["data_inicio"]))
						{
							condicionante.DataInicioPrazo.Data = Convert.ToDateTime(reader["data_inicio"]);
						}

						if (reader["data_criacao"] != null && !Convert.IsDBNull(reader["data_criacao"]))
						{
							condicionante.DataCriacao.Data = Convert.ToDateTime(reader["data_criacao"]);
						}

						if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
						{
							condicionante.DataVencimento.Data = Convert.ToDateTime(reader["data_vencimento"]);
						}

						condicionante.PossuiPeriodicidade = Convert.ToInt32(reader["periodicidade"]) > 0;

						if (reader["periodo"] != null && !Convert.IsDBNull(reader["periodo"]))
						{
							condicionante.PeriodicidadeValor = Convert.ToInt32(reader["periodo"]);
						}

						if (reader["periodo_tipo_id"] != null && !Convert.IsDBNull(reader["periodo_tipo_id"]))
						{
							condicionante.PeriodicidadeTipo.Id = Convert.ToInt32(reader["periodo_tipo_id"]);
							condicionante.PeriodicidadeTipo.Texto = reader["periodo_tipo_texto"].ToString();
						}
					}
					reader.Close();
				}

				comando = bancoDeDados.CriarComando(@"select p.id, p.situacao, ls.texto situacao_texto, p.dias_prorrogados, p.data_inicio, p.data_vencimento, p.tid
					from {0}tab_titulo_condicionantes_peri p, {0}lov_titulo_cond_situacao ls where p.situacao = ls.id and p.condicionante = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					TituloCondicionantePeriodicidade periodicidade = null;
					while (reader.Read())
					{
						periodicidade = new TituloCondicionantePeriodicidade();
						periodicidade.Id = Convert.ToInt32(reader["id"]);
						periodicidade.Tid = reader["tid"].ToString();
						periodicidade.Situacao.Id = Convert.ToInt32(reader["situacao"]);
						periodicidade.Situacao.Texto = reader["situacao_texto"].ToString();

						if (reader["dias_prorrogados"] != null && !Convert.IsDBNull(reader["dias_prorrogados"]))
						{
							periodicidade.DiasProrrogados = Convert.ToInt32(reader["dias_prorrogados"]);
						}

						if (reader["data_inicio"] != null && !Convert.IsDBNull(reader["data_inicio"]))
						{
							periodicidade.DataInicioPrazo.Data = Convert.ToDateTime(reader["data_inicio"]);
						}

						if (reader["data_vencimento"] != null && !Convert.IsDBNull(reader["data_vencimento"]))
						{
							periodicidade.DataVencimento.Data = Convert.ToDateTime(reader["data_vencimento"]);
						}

						condicionante.Periodicidades.Add(periodicidade);
					}
					reader.Close();
				}

				#endregion
			}
			return condicionante;
		}

		internal TituloCondicionanteDescricao DescricaoObter(int id, BancoDeDados banco = null)
		{
			TituloCondicionanteDescricao desc = new TituloCondicionanteDescricao();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Condicionante de Título
				Comando comando = bancoDeDados.CriarComando(@"select id, descricao, tid from {0}tab_titulo_condicionante_desc c where c.id = :id ", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						desc.Id = id;
						desc.Tid = reader["tid"].ToString();
						desc.Texto = reader["descricao"].ToString();
					}
					reader.Close();
				}
				#endregion
			}
			return desc;
		}

		#endregion
	}
}