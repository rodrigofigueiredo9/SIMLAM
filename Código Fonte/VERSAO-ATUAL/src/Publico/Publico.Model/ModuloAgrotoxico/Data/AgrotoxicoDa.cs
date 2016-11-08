using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Entities.Interno.ModuloAgrotoxico;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloAgrotoxico.Data
{
	public class AgrotoxicoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public AgrotoxicoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Buscas/Filtrar

		public Resultados<AgrotoxicoFiltro> Filtrar(Filtro<AgrotoxicoFiltro> filtros)
		{
			Resultados<AgrotoxicoFiltro> retorno = new Resultados<AgrotoxicoFiltro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("a.nome_comercial", "nome_comercial", filtros.Dados.NomeComercial,true,true);

				comandtxt += comando.FiltroAnd("a.numero_cadastro", "numero_cadastro", filtros.Dados.NumeroCadastro);

				comandtxt += comando.FiltroAnd("a.numero_registro_ministerio", "numero_registro_ministerio", filtros.Dados.NumeroRegistroMinisterio);

				if (filtros.Dados.Situacao!="0")
				{
					comandtxt += comando.FiltroAnd("a.cadastro_ativo", "cadastro_ativo", (filtros.Dados.Situacao!="1"?"0":"1"));
				}
				
				if(filtros.Dados.ClasseUso > 0)
				{
					comandtxt += comando.FiltroIn("a.id", "select t1.agrotoxico from tab_agrotoxico_classe_uso t1 where t1.classe_uso =:classe_uso", "classe_uso", filtros.Dados.ClasseUso);
				}

				if (filtros.Dados.ModalidadeAplicacao > 0)
				{
					comandtxt += comando.FiltroIn("a.id", "select t10.agrotoxico from tab_agro_cult_moda_aplicacao t9, tab_agrotoxico_cultura t10 where t9.agrotoxico_cultura = t10.id and t9.modalidade_aplicacao = :modalidade_aplicacao", "modalidade_aplicacao", filtros.Dados.ModalidadeAplicacao);
				}

				if (filtros.Dados.GrupoQuimico > 0)
				{
					comandtxt += comando.FiltroIn("a.id", "select t11.agrotoxico from tab_agrotoxico_grupo_quimico t11 where t11.grupo_quimico = :grupo_quimico", "grupo_quimico", filtros.Dados.GrupoQuimico);
				}

				comandtxt += comando.FiltroAnd("a.classificacao_toxicologica", "classificacao_toxicologica", filtros.Dados.ClassificacaoToxicologica);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.TitularRegistro))
				{
					comandtxt += comando.FiltroAndLike("nvl(p.nome, p.razao_social)", "titular_registro", filtros.Dados.TitularRegistro, true, true);
				}

				comandtxt += comando.FiltroAnd("a.numero_processo_sep", "numero_processo_sep", filtros.Dados.NumeroProcessoSep);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.IngredienteAtivo))
				{
					comandtxt += comando.FiltroIn("a.id", "select t3.agrotoxico from tab_ingrediente_ativo t2, tab_agrotoxico_ing_ativo t3 where t2.id = t3.ingrediente_ativo and upper(t2.texto) like upper('%'||:ingrediente||'%')", "ingrediente", filtros.Dados.IngredienteAtivo);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.Cultura))
				{
					comandtxt += comando.FiltroIn("a.id", "select t5.agrotoxico from tab_cultura t4, tab_agrotoxico_cultura t5 where t4.id = t5.cultura and upper(t4.texto) like upper('%'||:cultura||'%')", "cultura", filtros.Dados.Cultura);
				}

				if (!String.IsNullOrWhiteSpace(filtros.Dados.Praga))
				{
					comandtxt += comando.FiltroIn(@"a.id", "select t8.agrotoxico from tab_praga t6, tab_agrotoxico_cultura_praga t7, tab_agrotoxico_cultura t8 where t6.id = t7.praga and t7.agrotoxico_cultura = t8.id and upper(nvl(t6.nome_cientifico, t6.nome_comum)) like upper('%'||:praga||'%')", "praga", filtros.Dados.Praga);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero_cadastro", "nome_comercial", "titular_registro", "situacao"};

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero_cadastro");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format(@"select count(a.id) qtd from tab_agrotoxico a, tab_pessoa p where a.titular_registro = p.id " + comandtxt);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Funcionario.NaoEncontrouRegistros);
					return retorno;
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select a.id, (case when a.possui_cadastro>0 then 'Sim' else 'Não' end)  possui_cadastro,
				(case when a.cadastro_ativo>0 then 'Ativo' else 'Inativo' end) situacao, a.numero_cadastro, a.nome_comercial, 
				nvl(p.nome, p.razao_social) titular_registro, a.arquivo from tab_agrotoxico a, tab_pessoa p where a.titular_registro = p.id {0} {1}", 
				comandtxt, DaHelper.Ordenar(colunas, ordenar));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					AgrotoxicoFiltro agro;
					while (reader.Read())
					{
						agro = new AgrotoxicoFiltro();
						agro.Id = reader.GetValue<Int32>("id");
						agro.ArquivoId = reader.GetValue<Int32>("arquivo");
						agro.NumeroCadastro = reader.GetValue<String>("numero_cadastro");
						agro.NomeComercial = reader.GetValue<String>("nome_comercial");
						agro.TitularRegistro = reader.GetValue<String>("titular_registro");
						agro.Situacao = reader.GetValue<String>("situacao");
						
						retorno.Itens.Add(agro);
					}

					reader.Close();
					#endregion
				}
			}

			return retorno;
		}

		public List<Lista> ObterAgrotoxicoClasseUso()
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{				
				Comando comando = bancoDeDados.CriarComando("select b.id, b.texto from tab_classe_uso b order by b.texto");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{				
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = Convert.ToString(reader["id"]),
							Texto = Convert.ToString(reader["texto"]),
							IsAtivo = true
						});						
					}
					reader.Close();				
				}
			}

			return retorno;
		}

		public List<Lista> ObterAgrotoxicoModalidadeAplicacao()
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("select b.id, b.texto from tab_modalidade_aplicacao b order by b.texto");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = Convert.ToString(reader["id"]),
							Texto = Convert.ToString(reader["texto"]),
							IsAtivo = true
						});
					}
					reader.Close();
				}
			}

			return retorno;
		}

		public List<Lista> ObterAgrotoxicoGrupoQuimico()
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("select b.id, b.texto from tab_grupo_quimico b order by b.texto");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = Convert.ToString(reader["id"]),
							Texto = Convert.ToString(reader["texto"]),
							IsAtivo = true
						});
					}
					reader.Close();
				}
			}

			return retorno;
		}

		public List<Lista> ObterAgrotoxicoClassificacaoToxicologica()
		{
			List<Lista> retorno = new List<Lista>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("select b.id, b.texto from tab_class_toxicologica b order by b.texto");

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = Convert.ToString(reader["id"]),
							Texto = Convert.ToString(reader["texto"]),
							IsAtivo = true
						});
					}
					reader.Close();
				}
			}

			return retorno;
		}

		#endregion
	}
}