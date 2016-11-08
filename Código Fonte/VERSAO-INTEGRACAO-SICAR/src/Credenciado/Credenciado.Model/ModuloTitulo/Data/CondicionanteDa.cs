using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data
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

		#endregion Ações de DML
	}
}