using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloProjetoGeografico;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloProjetoGeografico.Data
{
	class ProjetoGeograficoInternoDa
	{
		#region Propriedades

		private ConfiguracaoSistema _configuracaoSistema = new ConfiguracaoSistema();

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());
		public String EsquemaBancoGeo
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioGeo); }
		}

		private string EsquemaBanco { get; set; }

		#endregion

		public ProjetoGeograficoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		internal ProjetoGeografico Obter(int id, BancoDeDados banco = null, bool simplificado = false, bool finalizado = false)
		{
			ProjetoGeografico projeto = new ProjetoGeografico();
			string tabela = finalizado ? "crt_projeto_geo" : "tmp_projeto_geo";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico

				Comando comando = bancoDeDados.CriarComando(@"select g.id, g.empreendimento, lc.id caracterizacao_id, lc.texto caracterizacao_texto, 
				ls.id situacao_id, ls.texto situacao_texto, ln.id nivel_precisao_id, ln.texto nivel_precisao_texto, lm.id mecanismo_elaboracao_id, 
				lm.texto mecanismo_elaboracao_texto, g.sobreposicoes_data, g.menor_x, g.menor_y, g.maior_x, g.maior_y, g.tid from {0}" + tabela + @" g, {0}lov_caracterizacao_tipo lc, 
				{0}lov_crt_projeto_geo_situacao ls, {0}lov_crt_projeto_geo_nivel ln, {0}lov_crt_projeto_geo_mecanismo lm where g.caracterizacao = lc.id 
				and g.situacao = ls.id and g.nivel_precisao = ln.id(+) and g.mecanismo_elaboracao = lm.id(+) and g.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						projeto = new ProjetoGeografico();

						projeto.Id = id;
						projeto.Tid = reader.GetValue<string>("tid");
						projeto.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						projeto.CaracterizacaoId = reader.GetValue<int>("caracterizacao_id");
						projeto.CaracterizacaoTexto = reader.GetValue<string>("caracterizacao_texto");
						projeto.SituacaoId = reader.GetValue<int>("situacao_id");
						projeto.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						projeto.NivelPrecisaoId = reader.GetValue<int>("nivel_precisao_id");
						projeto.NivelPrecisaoTexto = reader.GetValue<string>("nivel_precisao_texto");
						projeto.MecanismoElaboracaoId = reader.GetValue<int>("mecanismo_elaboracao_id");
						projeto.MecanismoElaboracaoTexto = reader.GetValue<string>("mecanismo_elaboracao_texto");

						projeto.MenorX = reader.GetValue<decimal>("menor_x");
						projeto.MenorY = reader.GetValue<decimal>("menor_y");
						projeto.MaiorX = reader.GetValue<decimal>("maior_x");
						projeto.MaiorY = reader.GetValue<decimal>("maior_y");

						projeto.CorrigirMbr();

						if (reader["sobreposicoes_data"] != null && !Convert.IsDBNull(reader["sobreposicoes_data"]))
						{
							projeto.Sobreposicoes.DataVerificacaoBanco = new DateTecno();
							projeto.Sobreposicoes.DataVerificacaoBanco.Data = reader.GetValue<DateTime>("sobreposicoes_data");
							projeto.Sobreposicoes.DataVerificacao = projeto.Sobreposicoes.DataVerificacaoBanco.Data.Value.ToString("dd/MM/yyyy - HH:mm", CultureInfo.CurrentCulture.DateTimeFormat);
						}
					}

					reader.Close();
				}

				#endregion

				if (projeto.Id <= 0 || simplificado)
				{
					return projeto;
				}

				#region Arquivos

				if (projeto.Id > 0)
				{
					projeto.Arquivos = ObterArquivos(projeto.Id, banco: bancoDeDados, finalizado: finalizado);
					projeto.ArquivosOrtofotos = ObterOrtofotos(projeto.Id, banco: bancoDeDados, finalizado: finalizado);

					if (projeto.MecanismoElaboracaoId == (int)eProjetoGeograficoMecanismo.Desenhador)
					{
						projeto.ArquivoEnviadoDesenhador = ObterArquivoDesenhador(projeto, banco);
					}

					projeto.Sobreposicoes.Itens = ObterSobreposicoes(projeto.Id, bancoDeDados, finalizado);
				}

				#endregion
			}

			return projeto;
		}

		public ArquivoProjeto ObterArquivoDesenhador(ProjetoGeografico projeto, BancoDeDados banco = null)
		{
			ArquivoProjeto arquivo = new ArquivoProjeto();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id, lc.id situacao_id, lc.texto situacao_texto from {1}tab_fila t, {0}lov_crt_projeto_geo_sit_proce lc
				where t.etapa = lc.etapa and t.situacao = lc.situacao and t.tipo in (3, 4) and t.mecanismo_elaboracao = 2 and t.projeto = :projeto", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projeto.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						arquivo.IdRelacionamento = reader.GetValue<int>("id");
						arquivo.Situacao = reader.GetValue<int>("situacao_id");
						arquivo.SituacaoTexto = reader.GetValue<string>("situacao_texto");
					}

					reader.Close();
				}
			}

			return arquivo;
		}

		public List<ArquivoProjeto> ObterArquivos(int projetoId, BancoDeDados banco = null, bool finalizado = false)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();
			string tabela = finalizado ? "crt_projeto_geo_arquivos" : "tmp_projeto_geo_arquivos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos

				Comando comando = bancoDeDados.CriarComando(@"select  tf.id, t.tipo, lc.texto  tipo_texto, tf.tipo fila_tipo, t.arquivo, t.valido, 
				lcp.id situacao_id, lcp.texto situacao_texto from {0}" + tabela + @" t, {0}lov_crt_projeto_geo_arquivos lc, {1}tab_fila tf, 
				{0}lov_crt_projeto_geo_sit_proce lcp where t.tipo = lc.id and t.projeto = tf.projeto(+) and t.tipo = tf.tipo(+) 
				and tf.etapa = lcp.etapa(+) and tf.situacao = lcp.situacao(+) and t.projeto = :projeto and t.valido = 1 and t.tipo <> 5 order by lc.id", EsquemaBanco, EsquemaBancoGeo);

				comando.AdicionarParametroEntrada("projeto", projetoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						arq.IdRelacionamento = reader.GetValue<int>("id");
						arq.Nome = reader.GetValue<string>("tipo_texto");
						arq.Tipo = reader.GetValue<int>("tipo");
						arq.FilaTipo = reader.GetValue<int>("fila_tipo");
						arq.Id = reader.GetValue<int>("arquivo");
						arq.Situacao = reader.GetValue<int>("situacao_id");
						arq.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						arq.isValido = reader.GetValue<bool>("valido");

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}

			return arquivos;
		}

		public List<ArquivoProjeto> ObterOrtofotos(int id, BancoDeDados banco = null, bool finalizado = false, bool todos = true)
		{
			List<ArquivoProjeto> arquivos = new List<ArquivoProjeto>();
			string tabela = finalizado ? "crt_projeto_geo_ortofoto" : "tmp_projeto_geo_ortofoto";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Projeto Geográfico/Arquivos Ortofoto

				Comando comando;

				if (todos)
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.caminho, t.chave, t.chave_data, 'application/zip' tipo from {0}" + tabela + " t where t.projeto = :projeto", EsquemaBanco);
				}
				else
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.caminho, t.chave, t.chave_data, 'application/zip' tipo from {0}" + tabela + " t where t.id = :projeto", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					string diretorio = _configuracaoSistema.DiretorioOrtoFotoMosaico[1];
					while (reader.Read())
					{
						ArquivoProjeto arq = new ArquivoProjeto();

						arq.Processamento.Situacao = (int)eProjetoGeograficoSituacaoProcessamento.Processado;
						arq.Id = reader.GetValue<int>("id");
						arq.Nome = reader.GetValue<string>("caminho");
						arq.ContentType = reader.GetValue<string>("tipo");
						arq.Chave = reader.GetValue<string>("chave");
						arq.ChaveData = reader.GetValue<DateTime>("chave_data");

						if (reader["caminho"] != null && !Convert.IsDBNull(reader["caminho"]))
						{
							arq.Caminho = diretorio + "\\" + reader.GetValue<string>("caminho");
						}

						arquivos.Add(arq);
					}

					reader.Close();
				}

				#endregion
			}

			return arquivos;
		}

		internal List<Sobreposicao> ObterSobreposicoes(int id, BancoDeDados banco = null, bool finalizado = false)
		{
			List<Sobreposicao> sobreposicoes = new List<Sobreposicao>();
			string tabela = finalizado ? "crt_projeto_geo_sobrepos" : "tmp_projeto_geo_sobrepos";

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Sobreposicao

				Comando comando = bancoDeDados.CriarComando(@"select s.id, s.base, s.tipo, s.identificacao, s.tid 
				from {0}" + tabela + @" s where s.projeto = :projeto", EsquemaBanco);

				comando.AdicionarParametroEntrada("projeto", id, DbType.Int32);

				Sobreposicao sobreposicao = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						sobreposicao = new Sobreposicao();

						sobreposicao.Base = reader.GetValue<int>("base");
						sobreposicao.Tipo = reader.GetValue<int>("tipo");
						sobreposicao.Identificacao = reader.GetValue<string>("identificacao");

						sobreposicoes.Add(sobreposicao);
					}

					reader.Close();
				}

				#endregion
			}

			return sobreposicoes;
		}
		
		#endregion

		#region Validações

		internal int ExisteProjetoGeografico(int empreendimentoId, int caracterizacaoTipo, bool finalizado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select t.id from {0}tmp_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo),
				(select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo)) projeto from dual", EsquemaBanco);

				if (finalizado)
				{
					comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_projeto_geo t where t.empreendimento = :empreendimento and t.caracterizacao = :tipo", EsquemaBanco);
				}

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo", caracterizacaoTipo, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				return (valor != null && !Convert.IsDBNull(valor)) ? Convert.ToInt32(valor) : 0;
			}
		}

		#endregion
	}
}