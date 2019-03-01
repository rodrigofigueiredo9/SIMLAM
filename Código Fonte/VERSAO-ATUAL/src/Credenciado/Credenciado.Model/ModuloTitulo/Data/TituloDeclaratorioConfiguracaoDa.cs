using System;
using System.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
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

		internal string ObterNomeArquivo(int id, BancoDeDados banco = null)
		{
			Comando comando = banco.CriarComando(@" select nome from tab_arquivo where id = :id ", EsquemaBanco);
			comando.AdicionarParametroEntrada("id", id, DbType.String);

			return banco.ExecutarScalar<string>(comando);
		}

		#endregion
	}
}
