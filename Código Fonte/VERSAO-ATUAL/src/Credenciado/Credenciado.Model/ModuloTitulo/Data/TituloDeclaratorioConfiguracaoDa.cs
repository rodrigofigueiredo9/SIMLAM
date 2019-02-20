using System;
using System.Data;
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

		//public Resultados<RelatorioTituloDecListarResultado> Filtrar(Filtro<RelatorioTituloDecListarFiltro> filtros, BancoDeDados banco = null)
		//{
		//	Resultados<Titulo> retorno = new Resultados<Titulo>();
		//	using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
		//	{
		//		string comandtxt = string.Empty;
		//		Comando comando = bancoDeDados.CriarComando("");

		//		#region Adicionando Filtros

		//		comandtxt += comando.FiltroAnd("l.requerimento", "requerimento", filtros.Dados.RequerimentoID);

		//		if (!string.IsNullOrWhiteSpace(filtros.Dados.DataEmisssao))
		//		{
		//			comandtxt += " and exists (select null from tab_titulo t where t.id = l.titulo_id and trunc(t.data_emissao) = :data_emissao) ";
		//			comando.AdicionarParametroEntrada("data_emissao", filtros.Dados.DataEmisssao, DbType.DateTime);
		//		}

		//		comandtxt += comando.FiltroAnd("l.interessado_cpf_cnpj", "interessado_cpf_cnpj", filtros.Dados.InteressadoCPFCNPJ);

		//		comandtxt += comando.FiltroAndLike("l.interessado_nome_razao", "interessado_nome_razao", filtros.Dados.InteressadoNomeRazao, true, true);

		//		if (filtros.Dados.OrigemID == 1)//Institucional
		//		{
		//			comandtxt += " and l.credenciado is null ";
		//		}
		//		else if (filtros.Dados.OrigemID == 2)//Credenciado
		//		{
		//			comandtxt += " and l.credenciado is not null ";
		//		}

		//		comandtxt += comando.FiltroAnd("l.modelo_id", "modelo", filtros.Dados.Modelo);

		//		comandtxt += comando.FiltroAnd("l.situacao_id", "situacao", filtros.Dados.Situacao);

		//		comandtxt += comando.FiltroAnd("l.setor_id", "setor", filtros.Dados.Setor);

		//		comandtxt += comando.FiltroAndLike("l.numero || '/' || l.ano", "numero", filtros.Dados.Numero, true);

		//		comandtxt += comando.FiltroAndLike("l.protocolo_numero", "protocolo_numero", filtros.Dados.Protocolo.NumeroTexto, true);

		//		comandtxt += comando.FiltroAnd("l.empreendimento_codigo", "empreendimento_codigo", filtros.Dados.EmpreendimentoCodigo);

		//		comandtxt += comando.FiltroAndLike("l.empreendimento_denominador", "empreendimento", filtros.Dados.Empreendimento, true);

		//		if (filtros.Dados.Modelo <= 0 && filtros.Dados.ModeloFiltrar != null && filtros.Dados.ModeloFiltrar.Count > 0)
		//		{
		//			comandtxt += comando.AdicionarIn("and", "l.modelo_id", DbType.Int32, filtros.Dados.ModeloFiltrar.Select(x => x).ToList());
		//		}

		//		if (filtros.Dados.SituacoesFiltrar != null && filtros.Dados.SituacoesFiltrar.Count > 0)
		//		{
		//			comandtxt += comando.AdicionarIn("and", "l.situacao_id", DbType.Int32, filtros.Dados.SituacoesFiltrar.Select(x => x).ToList());
		//		}

		//		if (filtros.Dados.IsDeclaratorio)
		//		{
		//			comandtxt += " and l.requerimento is not null ";
		//		}
		//		else
		//		{
		//			comandtxt += " and l.requerimento is null ";
		//		}

		//		List<String> ordenar = new List<String>();
		//		List<String> colunas = new List<String>() { "numero", "modelo_sigla", "protocolo_numero", "empreendimento_denominador", "situacao_texto", "data_vencimento" };

		//		if (filtros.OdenarPor > 0)
		//		{
		//			ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
		//		}
		//		else
		//		{
		//			ordenar.Add("numero");
		//		}

		//		#endregion

		//		#region Quantidade de registro do resultado

		//		comando.DbCommand.CommandText = String.Format(@"
		//		select count(*) quantidade from lst_titulo l where l.credenciado is null " + comandtxt +
		//		"union all select count(*) quantidade from lst_titulo l where l.credenciado is not null and l.situacao_id != 7 " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

		//		comando.DbCommand.CommandText = "select sum(d.quantidade) from (" + comando.DbCommand.CommandText + ") d ";

		//		retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

		//		comando.AdicionarParametroEntrada("menor", filtros.Menor);
		//		comando.AdicionarParametroEntrada("maior", filtros.Maior);

		//		comandtxt = String.Format(@"
		//		select titulo_id, titulo_tid, numero, numero_completo, data_vencimento, autor_id, autor_nome, modelo_sigla, situacao_texto, situacao_id,
		//			modelo_id, modelo_nome, modelo_codigo, protocolo_id, protocolo protocolo_tipo, protocolo_numero, empreendimento_codigo, empreendimento_denominador, requerimento 
		//			from lst_titulo l where l.credenciado is null " + comandtxt +
		//		@" union all 
		//		select titulo_id, titulo_tid, numero, numero_completo, data_vencimento, autor_id, autor_nome, modelo_sigla, situacao_texto, situacao_id,
		//			modelo_id, modelo_nome, modelo_codigo, protocolo_id, protocolo protocolo_tipo, protocolo_numero, empreendimento_codigo, empreendimento_denominador, requerimento 
		//			from lst_titulo l where l.credenciado is not null and l.situacao_id != 7 and exists (select 1 from tab_requerimento r where r.id = l.requerimento) " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

		//		comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a " + DaHelper.Ordenar(colunas, ordenar) + ") where rnum <= :maior and rnum >= :menor";

		//		#endregion

		//		using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
		//		{
		//			#region Adicionando os dados na classe de retorno

		//			Titulo titulo;
		//			while (reader.Read())
		//			{
		//				titulo = new Titulo();
		//				titulo.Id = reader.GetValue<int>("titulo_id");
		//				titulo.Autor.Id = reader.GetValue<int>("autor_id");
		//				titulo.Autor.Nome = reader.GetValue<string>("autor_nome");
		//				titulo.Tid = reader.GetValue<string>("titulo_tid");
		//				titulo.Numero.Texto = reader.GetValue<string>("numero_completo");
		//				titulo.Modelo.Id = reader.GetValue<int>("modelo_id");
		//				titulo.Modelo.Sigla = reader.GetValue<string>("modelo_sigla");
		//				titulo.Modelo.Nome = reader.GetValue<string>("modelo_nome");
		//				titulo.Modelo.Codigo = reader.GetValue<int>("modelo_codigo");
		//				titulo.Situacao.Id = reader.GetValue<int>("situacao_id");
		//				titulo.Situacao.Nome = reader.GetValue<string>("situacao_texto");
		//				titulo.EmpreendimentoCodigo = reader.GetValue<long>("empreendimento_codigo");
		//				titulo.EmpreendimentoTexto = reader.GetValue<string>("empreendimento_denominador");
		//				titulo.DataVencimento.DataTexto = reader.GetValue<string>("data_vencimento");
		//				titulo.RequerimetoId = reader.GetValue<int>("requerimento");

		//				titulo.Protocolo.Id = reader.GetValue<int>("protocolo_id");
		//				if (titulo.Protocolo.Id.GetValueOrDefault() > 0)
		//				{
		//					ProtocoloNumero prot = new ProtocoloNumero(reader.GetValue<string>("protocolo_numero"));
		//					titulo.Protocolo.IsProcesso = (reader.GetValue<int>("protocolo_tipo") == 1);
		//					titulo.Protocolo.NumeroProtocolo = prot.Numero;
		//					titulo.Protocolo.Ano = prot.Ano;
		//				}
		//				if (titulo.Situacao.Id == (int)eTituloSituacao.Concluido && titulo.DataVencimento?.Data < DateTime.Now.Date)
		//				{
		//					if (titulo.Modelo.Codigo == (int)eTituloModeloCodigo.LaudoVistoriaFlorestal || titulo.Modelo.Codigo == (int)eTituloModeloCodigo.AutorizacaoExploracaoFlorestal)
		//						titulo.Situacao.Nome = "Vencido";
		//				}
		//				retorno.Itens.Add(titulo);
		//			}

		//			reader.Close();

		//			#endregion
		//		}
		//	}

		//	return retorno;
		//}

		#endregion
	}
}
