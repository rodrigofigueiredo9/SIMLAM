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

				Comando comando = bancoDeDados.CriarComando(@"
				update {0}tab_titulo_configuracao t
					set t.arquivo_sem_app	 = :arquivo_sem_app,
						t.arquivo_com_app	 = :arquivo_com_app,
						t.tid				 = :tid, 
						t.area_alagada		 = (case when :area_alagada > 0 then :area_alagada else t.area_alagada end),
						t.volume_armazenado  = (case when :volume_armazenado > 0 then :volume_armazenado else t.volume_armazenado end)
					where t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", configuracao.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("area_alagada", configuracao.MaximoAreaAlagada, DbType.Decimal);
				comando.AdicionarParametroEntrada("volume_armazenado", configuracao.MaximoVolumeArmazenado, DbType.Decimal);
				comando.AdicionarParametroEntrada("arquivo_sem_app", configuracao.BarragemSemAPP.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo_com_app", configuracao.BarragemComAPP.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Histórico

				//Historico.Gerar(configuracao.Id, eHistoricoArtefato.titulo, eHistoricoAcao.atualizar, bancoDeDados);

				#endregion

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal TituloDeclaratorioConfiguracao Obter(int id = 0, BancoDeDados banco = null)
		{
			var configuracao = new TituloDeclaratorioConfiguracao();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id,
						c.tid, c.area_alagada, c.volume_armazenado, 
						c.arquivo_sem_app, c.arquivo_com_app,
						ta_sem_app.nome nome_sem_app,
						ta_com_app.nome nome_com_app
					from {0}tab_titulo_configuracao c
					left join {0}tab_arquivo ta_sem_app
						on ta_sem_app.id = c.arquivo_sem_app
					left join {0}tab_arquivo ta_com_app
						on ta_com_app.id = c.arquivo_com_app " +
					(id > 0 ? "where c.id = :id" : ""), EsquemaBanco);

				if(id > 0) comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						configuracao.Id = reader.GetValue<int>("id");
						configuracao.Tid = reader["tid"].ToString();
						configuracao.MaximoAreaAlagada = reader.GetValue<decimal>("area_alagada");
						configuracao.MaximoVolumeArmazenado = reader.GetValue<decimal>("volume_armazenado");
						
						if (reader["arquivo_sem_app"] != null && !Convert.IsDBNull(reader["arquivo_sem_app"]))
						{
							configuracao.BarragemSemAPP.Id = Convert.ToInt32(reader["arquivo_sem_app"]);
							configuracao.BarragemSemAPP.Nome = reader["nome_sem_app"].ToString();
						}

						if (reader["arquivo_com_app"] != null && !Convert.IsDBNull(reader["arquivo_com_app"]))
						{
							configuracao.BarragemComAPP.Id = Convert.ToInt32(reader["arquivo_com_app"]);
							configuracao.BarragemComAPP.Nome = reader["nome_com_app"].ToString();
						}

					}
					reader.Close();
				}
			}

			return configuracao;
		}

		#endregion
	}
}
