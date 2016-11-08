using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloAtividade;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloAtividade.Data
{
	public class AtividadeDa
	{
		#region Propriedade

		Historico _historico = new Historico();

		internal Historico Historico { get { return _historico; } }

		private string EsquemaBanco { get; set; }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public AtividadeDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public void AlterarSituacao(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = null;

				if ((atividade.IdRelacionamento ?? 0) <= 0)
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_atividades a set a.situacao = :situacao, a.motivo = :motivo, a.tid = :tid						
						where a.protocolo = :protocolo and a.atividade = :atividade returning a.id into :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
					comando.AdicionarParametroSaida("id", DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"update {0}tab_protocolo_atividades a set a.situacao = :situacao, a.motivo = :motivo, a.tid = :tid where a.id = :atividade_id_relacionamento", EsquemaBanco);
					comando.AdicionarParametroEntrada("atividade_id_relacionamento", atividade.IdRelacionamento, DbType.Int32);
				}

				comando.AdicionarParametroEntrada("situacao", atividade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", atividade.Motivo, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				if ((atividade.IdRelacionamento ?? 0) <= 0)
				{
					atividade.IdRelacionamento = Convert.ToInt32(comando.ObterValorParametro("id"));
				}

				Historico.Gerar(atividade.IdRelacionamento.Value, eHistoricoArtefato.protocoloatividade, eHistoricoAcao.alterarsituacao, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		internal Atividade ObterAtividadePorCodigo(int codigo)
		{
			Atividade retorno = new Atividade();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.atividade from {0}tab_atividade t where t.codigo = :codigo", EsquemaBanco);

				comando.AdicionarParametroEntrada("codigo", codigo);
				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					retorno = new Atividade()
					{
						Id = Convert.ToInt32(item["id"]),
						NomeAtividade = item["atividade"].ToString()
					};
				}
			}

			return retorno;
		}

		public Resultados<AtividadeListarFiltro> Filtrar(Filtro<AtividadeListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<AtividadeListarFiltro> retorno = new Resultados<AtividadeListarFiltro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				if (!string.IsNullOrWhiteSpace(filtros.Dados.AtividadeNome))
				{
					comandtxt += " and lower(trim(ta.atividade)) like '%'||:atividade||'%' ";
					comando.AdicionarParametroEntrada("atividade", filtros.Dados.AtividadeNome.ToLower().Trim(), DbType.String);
				}

				if (filtros.Dados.SetorId > 0)
				{
					comandtxt += " and ts.id = :setor ";
					comando.AdicionarParametroEntrada("setor", filtros.Dados.SetorId, DbType.Int32);
				}

				if (filtros.Dados.AgrupadorId > 0)
				{
					comandtxt += " and ta.agrupador = :agrupador ";
					comando.AdicionarParametroEntrada("agrupador", filtros.Dados.AgrupadorId, DbType.Int32);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "setor_texto", "atividade", "agrupador_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("setor_texto");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format(@"select count(*) from {0}tab_atividade ta, {0}tab_setor ts where ta.situacao=1 and ta.setor = ts.id " + comandtxt, (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));


				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Processo.NaoEncontrouRegistros);
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select ta.id, ta.atividade, ta.setor setor_id, ts.sigla setor_sigla, ts.nome setor_texto, taa.nome agrupador_texto 
				from {2}tab_atividade ta, {2}tab_setor ts, {2}tab_atividade_agrupador taa where ta.situacao=1 and ta.agrupador = taa.id and ta.setor = ts.id {0} {1}",
				comandtxt, DaHelper.Ordenar(colunas, ordenar), (string.IsNullOrEmpty(EsquemaBanco) ? "" : "."));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					AtividadeListarFiltro atividade;
					while (reader.Read())
					{
						atividade = new AtividadeListarFiltro();
						atividade.Id = Convert.ToInt32(reader["id"]);
						atividade.AtividadeNome = reader["atividade"].ToString();
						atividade.SetorId = Convert.ToInt32(reader["setor_id"]);
						atividade.SetorTexto = reader["setor_texto"].ToString();
						atividade.SetorSigla = reader["setor_sigla"].ToString();
						atividade.AgrupadorTexto = reader["agrupador_texto"].ToString();

						retorno.Itens.Add(atividade);
					}
					reader.Close();
				}
			}

			return retorno;
		}

		internal List<AtividadeSolicitada> ObterAtividadesLista(Protocolo protocolo, bool isApensadosJuntados = false)
		{
			List<AtividadeSolicitada> lst = new List<AtividadeSolicitada>();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				if (isApensadosJuntados)
				{
					comando = bancoDeDados.CriarComando(@" select distinct * from ( select l.id, l.atividade from {0}tab_protocolo_associado p, {0}tab_protocolo_atividades a, {0}tab_atividade l
						where p.protocolo = :id and a.protocolo in (p.protocolo, p.associado) and l.id = a.atividade union all select pa.id, pa.atividade from {0}tab_protocolo_atividades t,
						{0}tab_atividade pa where t.atividade = pa.id and t.protocolo = :id ) ", EsquemaBanco);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select pa.id, pa.atividade from {0}tab_protocolo_atividades t, {0}tab_atividade pa 
						where t.atividade = pa.id and t.protocolo = :id", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("id", protocolo.Id);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new AtividadeSolicitada()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["atividade"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		internal List<AtividadeSolicitada> ObterAtividadesListaReq(int requerimentoId)
		{
			var lst = new List<AtividadeSolicitada>();

			var sql = @"
                select ta.id Id, 
                       ta.atividade Texto, 
                       ta.situacao IsAtivo
                  from tab_requerimento_atividade tr, 
                       tab_atividade ta
                 where tr.atividade = ta.id
                   and tr.requerimento = :requerimentoId";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				using (var comando = bancoDeDados.CriarComando(sql, EsquemaBanco))
				{
					comando.AdicionarParametroEntrada("requerimentoId", requerimentoId);
					lst = bancoDeDados.ObterEntityList<AtividadeSolicitada>(comando);
				}
			}

			return lst;
		}

		internal List<AtividadeSolicitada> ObterAtividadesLista(int id)
		{
			List<AtividadeSolicitada> lst = new List<AtividadeSolicitada>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select pa.id, pa.atividade from {0}tab_protocolo_atividades t, {0}tab_atividade pa 
				where t.atividade = pa.id and t.protocolo = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id);
				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(new AtividadeSolicitada()
					{
						Id = Convert.ToInt32(item["id"]),
						Texto = item["atividade"].ToString(),
						IsAtivo = true
					});
				}
			}

			return lst;
		}

		public int ObterAtividadeProtocoloSituacao(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				if (atividade.IdRelacionamento > 0)
				{
					comando = bancoDeDados.CriarComando(@"select a.situacao atividade_situacao_id from {0}tab_protocolo_atividades a where a.id = :atividadeIdRelacionamento", EsquemaBanco);
					comando.AdicionarParametroEntrada("atividadeIdRelacionamento", atividade.IdRelacionamento, DbType.Int32);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select a.situacao atividade_situacao_id from {0}tab_protocolo_atividades a where a.atividade = :atividade and a.protocolo = :protocolo", EsquemaBanco);
					comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
				}

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		public string ObterSituacaoTexto(int situcaoId, BancoDeDados banco = null)
		{
			if (situcaoId > 0)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
				{
					Comando comando = bancoDeDados.CriarComando(@"select s.texto from {0}lov_atividade_situacao s where s.id = :situacao_id", EsquemaBanco);
					comando.AdicionarParametroEntrada("situacao_id", situcaoId, DbType.Int32);

					object retorno = bancoDeDados.ExecutarScalar(comando);

					if (!string.IsNullOrEmpty(retorno.ToString()))
					{
						return retorno.ToString();
					}
				}
			}
			return string.Empty;
		}

		private List<int> ObterModelosRequeridos(Atividade atividade, BancoDeDados banco = null)
		{
			Comando comando = null;
			List<int> modelosRequeridos = new List<int>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{

				comando = bancoDeDados.CriarComando(@"select f.modelo from {0}tab_protocolo_atividades t, {0}tab_protocolo_ativ_finalida f 
					where t.protocolo = :protocolo and t.atividade = :atividade and t.id = f.protocolo_ativ", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						modelosRequeridos.Add(Convert.ToInt32(reader["modelo"]));
					}

					reader.Close();
				}
			}

			return modelosRequeridos;
		}

		#endregion

		#region Validar / Verificar

		internal bool VerificarAtividadeAssociadaTitulo(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(t.id) from {0}tab_titulo_atividades tt, {0}tab_titulo t where t.id = tt.titulo
				and tt.atividade = :atividade and t.modelo = :modelo and ((t.situacao = 5 and t.situacao_motivo not in(1, 4)) or (t.situacao not in (1,5))) and tt.protocolo = :protocolo", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modelo, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		public bool ValidarAtividadeComTituloOuEncerrada(int protocolo, bool isProcesso, int atividade, int modelo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				//6 - Encerrada
				Comando comando = bancoDeDados.CriarComando(@"select (select count(d.id) from {0}tab_protocolo_atividades d where d.protocolo = :protocolo 
				and d.atividade = :atividade and d.situacao = 6) + (select count(t.id) from {0}tab_titulo_atividades ta, {0}tab_titulo t 
				where t.id = ta.titulo and t.modelo = :modelo and ta.atividade = :atividade and ta.protocolo = :protocolo
				and ((t.situacao = 5 and t.situacao_motivo not in(1, 4)) or (t.situacao not in (1,5)))) qtd from dual", EsquemaBanco);

				comando.AdicionarParametroEntrada("protocolo", protocolo, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade, DbType.Int32);
				comando.AdicionarParametroEntrada("modelo", modelo, DbType.Int32);

				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool VerificarEncerrar(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;

				List<int> modelosRequeridos = new List<int>();

				if (atividade.Finalidades != null && atividade.Finalidades.Count > 0)
				{
					modelosRequeridos.AddRange(atividade.Finalidades.Select(x => x.TituloModelo).ToList());
				}
				else
				{
					modelosRequeridos = ObterModelosRequeridos(atividade, banco);
				}

				comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_titulo_atividades ta, {0}tab_titulo tt 
				where ta.protocolo = :protocolo and ta.atividade = :atividade and ta.titulo = tt.id and tt.situacao = 5 and tt.situacao_motivo not in (1,4) ", EsquemaBanco);

				comando.DbCommand.CommandText += comando.AdicionarIn("and ", "tt.modelo", DbType.Int32, modelosRequeridos);
				comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) == modelosRequeridos.Count;
			}
		}

		internal bool VerificarDeferir(Atividade atividade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;
				List<int> modelosRequeridos = new List<int>();

				if (atividade.Finalidades != null && atividade.Finalidades.Count > 0)
				{
					modelosRequeridos.AddRange(atividade.Finalidades.Select(x => x.TituloModelo).ToList());
				}
				else
				{
					modelosRequeridos = ObterModelosRequeridos(atividade, banco);
				}

				foreach (int item in modelosRequeridos)
				{
					string colunafiltro = atividade.Protocolo.IsProcesso ? "processo" : "documento";

					comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_titulo t, {0}tab_titulo_atividades a 
					where a.titulo = t.id and a.protocolo = :protocolo and t.modelo = :modelo and a.atividade = :atividade and t.situacao in (3,6)", EsquemaBanco);//3 - Concluído | 6 - Prorrogado

					comando.AdicionarParametroEntrada("atividade", atividade.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("protocolo", atividade.Protocolo.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("modelo", item, DbType.Int32);

					if (Convert.ToInt32(bancoDeDados.ExecutarScalar(comando)) <= 0)
					{
						return false;
					}
				}

				return true;
			}
		}

		public string VerificarProcessoApensadoNumero(int apensado)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select tp.numero from tab_protocolo_associado r, tab_protocolo tp where r.protocolo = tp.id and r.associado = :apensadoId");
				comando.AdicionarParametroEntrada("apensadoId", apensado, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool ExisteAtividadeNoSetor(int atividadeId, int setorId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_atividade t where t.id = :atividade_id and t.setor = :setor_id");

				comando.AdicionarParametroEntrada("atividade_id", atividadeId, DbType.Int32);
				comando.AdicionarParametroEntrada("setor_id", setorId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool AtividadeAtiva(int atividadeId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_atividade t where t.situacao = 1 and t.id = :atividade_id", EsquemaBanco);
				comando.AdicionarParametroEntrada("atividade_id", atividadeId, DbType.Int32);
				return Convert.ToBoolean(bancoDeDados.ExecutarScalar(comando));
			}
		}

		#endregion
	}
}