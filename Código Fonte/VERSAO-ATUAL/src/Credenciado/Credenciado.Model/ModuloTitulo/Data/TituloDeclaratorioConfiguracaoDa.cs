using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloTitulo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloTitulo.Data
{
	public class TituloDeclaratorioConfiguracaoDa
	{
		#region Propriedades

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		private string EsquemaBanco { get; set; }

		#endregion

		public TituloDeclaratorioConfiguracaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		internal void Salvar(TituloDeclaratorioConfiguracao configuracao, BancoDeDados banco)
		{
			if (configuracao == null)
				throw new Exception("A configuração é nula.");

			if (configuracao.Id <= 0)
				Criar(configuracao, banco);
			else
				Editar(configuracao, banco);
		}

		internal int? Criar(TituloDeclaratorioConfiguracao configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Configuracacao

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}tab_titulo_configuracao c (id, area_alagada, volume_armazenado, arquivo_sem_app, arquivo_com_app, tid)
				values (seq_titulo_configuracao.nextval, :area_alagada, :volume_armazenado, :arquivo_sem_app, :arquivo_com_app, :tid)
				returning c.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("area_alagada", configuracao.MaximoAreaAlagada, DbType.Decimal);
				comando.AdicionarParametroEntrada("volume_armazenado", configuracao.MaximoVolumeArmazenado, DbType.Decimal);
				comando.AdicionarParametroEntrada("arquivo_sem_app", configuracao.BarragemSemAPP.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo_com_app", configuracao.BarragemComAPP.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				configuracao.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Histórico

				//Historico.Gerar(configuracao.Id, eHistoricoArtefato.titulo, eHistoricoAcao.criar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();

				return configuracao.Id;
			}
		}

		internal void Editar(TituloDeclaratorioConfiguracao configuracao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Configuracao

				bancoDeDados.IniciarTransacao();

				if(configuracao.MaximoAreaAlagada > 0)
					AtualizarConfiguracao("area_alagada", configuracao.MaximoAreaAlagada.ToString(), bancoDeDados);
				if (configuracao.MaximoVolumeArmazenado > 0)
					AtualizarConfiguracao("volume_armazenado", configuracao.MaximoVolumeArmazenado.ToString(), bancoDeDados);
				if (configuracao.BarragemSemAPP.Id > 0)
					AtualizarConfiguracao("arquivo_sem_app", configuracao.BarragemSemAPP.Id.ToString(), bancoDeDados);
				if (configuracao.BarragemComAPP.Id > 0)
					AtualizarConfiguracao("arquivo_com_app", configuracao.BarragemComAPP.Id.ToString(), bancoDeDados);

				#endregion

				#region Histórico

				//Historico.Gerar(configuracao.Id, eHistoricoArtefato.titulo, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		internal void AtualizarConfiguracao(string codigo, string valor, BancoDeDados banco = null)
		{
			String sql = @" update {0}tab_titulo_configuracao t
					set t.valor	 = :valor where t.codigo = :codigo";

			Comando comando = banco.CriarComando(sql, EsquemaBanco);
			comando.AdicionarParametroEntrada("valor", valor, DbType.String);
			comando.AdicionarParametroEntrada("codigo", codigo, DbType.String);

			banco.ExecutarNonQuery(comando);
		}

		#endregion

		#region Obter / Filtrar

		internal TituloDeclaratorioConfiguracao Obter(int id = 0, BancoDeDados banco = null)
		{
			var configuracao = new TituloDeclaratorioConfiguracao();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select c.id, c.codigo, c.valor
					from {0}tab_titulo_configuracao c ", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						configuracao.Id = reader.GetValue<int>("id");
						switch (configuracao.Id)
						{
							case 1:
								if (reader["valor"] != null && !Convert.IsDBNull(reader["valor"]))
									configuracao.MaximoAreaAlagada = reader.GetValue<decimal>("valor");
								break;
							case 2:
								if (reader["valor"] != null && !Convert.IsDBNull(reader["valor"]))
									configuracao.MaximoVolumeArmazenado = reader.GetValue<decimal>("valor");
								break;
							case 3:
								if (reader["valor"] != null && !Convert.IsDBNull(reader["valor"]))
								{
									configuracao.BarragemSemAPP.Id = Convert.ToInt32(reader["valor"]);
									configuracao.BarragemSemAPP.Nome = ObterNomeArquivo(configuracao.BarragemSemAPP.Id ?? 0, bancoDeDados);
								}
								break;
							case 4:
								if (reader["valor"] != null && !Convert.IsDBNull(reader["valor"]))
								{
									configuracao.BarragemComAPP.Id = Convert.ToInt32(reader["valor"]);
									configuracao.BarragemComAPP.Nome = ObterNomeArquivo(configuracao.BarragemSemAPP.Id ?? 0, bancoDeDados);
								}
								break;

						}
					}
					reader.Close();
				}


			}

			return configuracao;
		}

		public Resultados<RelatorioTituloDecListarResultado> Filtrar(Filtro<RelatorioTituloDecListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<RelatorioTituloDecListarResultado> retorno = new Resultados<RelatorioTituloDecListarResultado>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("l.numero_titulo", "numero_titulo", filtros.Dados.NumeroTitulo);
				comandtxt += comando.FiltroAnd("l.executor_login", "executor_login", filtros.Dados.Login);
				comandtxt += comando.FiltroAndLike("upper(l.executor_nome)", "executor_nome", filtros.Dados.NomeUsuario);
				comandtxt += comando.FiltroAndLike("l.nome_interessado", "nome_interessado", filtros.Dados.NomeInteressado, true, true);
				comandtxt += comando.FiltroAnd("l.cpfcnpj_interessado", "cpfcnpj_interessado", filtros.Dados.InteressadoCpfCnpj);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.DataSituacaoAtual.DataTexto))
				{
					comandtxt += comando.FiltroAnd("to_char(l.data_situacao, 'dd/mm/yyyy')", "data_situacao", filtros.Dados.DataSituacaoAtual.DataTexto);
				}
				if (filtros.Dados.Situacao > 0)
				{
					comandtxt += comando.FiltroAnd("l.situacao_id", "situacao_id", filtros.Dados.Situacao);
				}
				comandtxt += comando.FiltroAnd("l.executor_ip", "executor_ip", filtros.Dados.IP);
				

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero_titulo", "executor_login", "executor_nome", "nome_interessado", "cpfcnpj_interessado", "data_situacao", "situacao_id", "situacao_texto", "executor_ip" };

				ordenar.Add("data_situacao");

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"
				select count(*) quantidade
				from (select ht.id,
							 (select lt.numero || '/' || lt.ano
							 	from tab_titulo_numero lt
							 	where lt.titulo = ht.titulo_id) numero_titulo,
							 ht.executor_login,
							 ht.executor_nome,
							 nvl(tp.nome, tpc.nome) nome_interessado,
							 nvl(nvl(tp.cpf, tp.cnpj), nvl(tpc.cpf, tpc.cnpj)) cpfcnpj_interessado,
							 ht.data_situacao,
							 ht.situacao_id,
							 ht.situacao_texto,
							 ht.executor_ip
					  from hst_titulo ht 
					  inner join tab_titulo_modelo htm on ht.modelo_id = htm.id
					  left join hst_requerimento hr on ht.requerimento_id = hr.requerimento_id  and ht.requerimento_tid = hr.tid
					  left join tab_pessoa tp on hr.interessado_id = tp.id 
					  left join idafcredenciado.hst_requerimento hrc on ht.requerimento_id = hrc.requerimento_id and ht.requerimento_tid = hrc.tid
					  left join idafcredenciado.tab_pessoa tpc on hrc.interessado_id = tpc.id
					  where htm.documento = 2) l
				where l.id = l.id " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = "select sum(d.quantidade) from (" + comando.DbCommand.CommandText + ") d ";

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"
				select l.id,
					   l.numero_titulo,
					   l.executor_login,
					   l.executor_nome,
					   l.nome_interessado,
					   l.cpfcnpj_interessado,
					   l.data_situacao,
					   l.situacao_id,
					   l.situacao_texto,
					   l.executor_ip
				from (select ht.id,
							 (select lt.numero || '/' || lt.ano
							 	from tab_titulo_numero lt
							 	where lt.titulo = ht.titulo_id) numero_titulo,
							 ht.executor_login,
							 ht.executor_nome,
							 nvl(tp.nome, tpc.nome) nome_interessado,
							 nvl(nvl(tp.cpf, tp.cnpj), nvl(tpc.cpf, tpc.cnpj)) cpfcnpj_interessado,
							 ht.data_situacao,
							 ht.situacao_id,
							 ht.situacao_texto,
							 ht.executor_ip
					  from hst_titulo ht 
					  inner join tab_titulo_modelo htm on ht.modelo_id = htm.id
					  left join hst_requerimento hr on ht.requerimento_id = hr.requerimento_id  and ht.requerimento_tid = hr.tid
					  left join tab_pessoa tp on hr.interessado_id = tp.id 
					  left join idafcredenciado.hst_requerimento hrc on ht.requerimento_id = hrc.requerimento_id and ht.requerimento_tid = hrc.tid
					  left join idafcredenciado.tab_pessoa tpc on hrc.interessado_id = tpc.id
					  where htm.documento = 2) l
				where l.id = l.id " + comandtxt + " order by l.data_situacao desc", (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a " + DaHelper.Ordenar(colunas, ordenar) + " desc) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					RelatorioTituloDecListarResultado titulo;
					while (reader.Read())
					{
						titulo = new RelatorioTituloDecListarResultado();

						titulo.ID = reader.GetValue<int>("id");
						titulo.NumeroTitulo = reader.GetValue<string>("numero_titulo");
						titulo.Login = reader.GetValue<string>("executor_login");
						titulo.NomeUsuario = reader.GetValue<string>("executor_nome");
						titulo.NomeInteressado = reader.GetValue<string>("nome_interessado");
						titulo.CPFCNPJInteressado = reader.GetValue<string>("cpfcnpj_interessado");
						titulo.DataSituacao = reader.GetValue<DateTime>("data_situacao");
						titulo.Situacao = reader.GetValue<int>("situacao_id");
						titulo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						titulo.IP = reader.GetValue<string>("executor_ip");

						retorno.Itens.Add(titulo);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal string ObterNomeArquivo(int id, BancoDeDados banco = null)
		{
			Comando comando = banco.CriarComando(@" select nome from tab_arquivo where id = :id ", EsquemaBanco);
			comando.AdicionarParametroEntrada("id", id, DbType.String);

			return banco.ExecutarScalar<string>(comando);
		}

		#endregion
	}
}
