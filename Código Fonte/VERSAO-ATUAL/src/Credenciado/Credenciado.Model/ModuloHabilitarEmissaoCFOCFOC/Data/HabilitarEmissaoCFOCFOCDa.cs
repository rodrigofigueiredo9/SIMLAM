using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloHabilitarEmissaoCFOCFOC.Data
{
	public class HabilitarEmissaoCFOCFOCDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public HabilitarEmissaoCFOCFOCDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Buscas/Filtrar

		internal HabilitarEmissaoCFOCFOC Obter(int id, bool simplificado = false, string _schemaBanco = null, bool isCredenciado = false)
		{
			HabilitarEmissaoCFOCFOC habilitar = null;
			PragaHabilitarEmissao praga = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Habilitar Emissão

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.responsavel, t.responsavel_arquivo, aa.nome responsavel_arquivo_nome, cc.cpf responsavel_cpf, cc.nome responsavel_nome, 
				pc.registro registro_crea, t.numero_habilitacao, trunc(t.validade_registro) validade_registro, trunc(t.situacao_data) situacao_data, t.situacao, ls.texto situacao_texto, t.motivo,
				lm.texto motivo_texto, t.observacao, t.numero_dua, t.extensao_habilitacao, t.numero_habilitacao_ori, t.uf, t.numero_visto_crea, t.tid from tab_hab_emi_cfo_cfoc t, tab_credenciado tc, 
				cre_pessoa cc, cre_pessoa_profissao pc, lov_hab_emissao_cfo_situacao ls, lov_hab_emissao_cfo_motivo lm, tab_arquivo aa where t.situacao = ls.id and t.motivo = lm.id(+) 
				and t.responsavel_arquivo = aa.id(+) and tc.id = t.responsavel and tc.pessoa = cc.id and cc.id = pc.pessoa(+) and t.id = :id", UsuarioCredenciado);

				if (isCredenciado)
				{
					comando.DbCommand.CommandText = comando.DbCommand.CommandText.Replace("and t.id = :id", "and t.responsavel = :id");
				}

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						habilitar = new HabilitarEmissaoCFOCFOC();
						habilitar.Id = reader.GetValue<Int32>("id");
						habilitar.Tid = reader.GetValue<String>("tid");
						habilitar.Responsavel.Id = reader.GetValue<Int32>("responsavel");
						habilitar.Arquivo.Id = reader.GetValue<Int32>("responsavel_arquivo");
						habilitar.Arquivo.Nome = reader.GetValue<String>("responsavel_arquivo_nome");
						habilitar.Responsavel.Pessoa.NomeRazaoSocial = reader.GetValue<String>("responsavel_nome");
						habilitar.Responsavel.Pessoa.CPFCNPJ = reader.GetValue<String>("responsavel_cpf");
						habilitar.NumeroHabilitacao = reader.GetValue<String>("numero_habilitacao");
						habilitar.ValidadeRegistro = reader.GetValue<DateTime>("validade_registro").ToShortDateString();
						habilitar.SituacaoData = reader.GetValue<DateTime>("situacao_data").ToShortDateString();
						habilitar.Situacao = reader.GetValue<Int32>("situacao");
						habilitar.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						habilitar.Motivo = reader.GetValue<Int32>("motivo");
						habilitar.MotivoTexto = reader.GetValue<String>("motivo_texto");
						habilitar.Observacao = reader.GetValue<String>("observacao");
						habilitar.NumeroDua = reader.GetValue<String>("numero_dua");
						habilitar.ExtensaoHabilitacao = reader.GetValue<Int32>("extensao_habilitacao");
						habilitar.NumeroHabilitacaoOrigem = reader.GetValue<String>("numero_habilitacao_ori");
						habilitar.RegistroCrea = reader.GetValue<String>("registro_crea");
						habilitar.UF = reader.GetValue<Int32>("uf");
						habilitar.NumeroVistoCrea = reader.GetValue<String>("numero_visto_crea");
					}
					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return habilitar;
				}

				#region Pragas

				if (habilitar != null)
				{
					comando = bancoDeDados.CriarComando(@"
					select hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, trunc(hp.data_habilitacao_inicial) data_habilitacao_inicial, 
					trunc(hp.data_habilitacao_final) data_habilitacao_final, hp.tid, stragg(c.texto) cultura 
					from tab_hab_emi_cfo_cfoc_praga hp, tab_praga pa, tab_praga_cultura pc, tab_cultura c 
					where hp.praga = pa.id and hp.praga = pc.praga(+) and pc.cultura = c.id(+) and hp.habilitar_emi_id = :id 
					group by hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, hp.data_habilitacao_inicial, hp.data_habilitacao_final, hp.tid", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("id", habilitar.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							praga = new PragaHabilitarEmissao();
							praga.Id = reader.GetValue<Int32>("id");
							praga.Praga.Id = reader.GetValue<Int32>("praga");
							praga.Praga.NomeCientifico = reader.GetValue<String>("nome_cientifico");
							praga.Praga.NomeComum = reader.GetValue<String>("nome_comum");
							praga.DataInicialHabilitacao = reader.GetValue<DateTime>("data_habilitacao_inicial").ToShortDateString(); ;
							praga.DataFinalHabilitacao = reader.GetValue<DateTime>("data_habilitacao_final").ToShortDateString(); ;
							praga.Tid = reader.GetValue<String>("tid");
							praga.Cultura = reader.GetValue<String>("cultura");
							habilitar.Pragas.Add(praga);
						}
						reader.Close();
					}
				}

				#endregion
			}

			return habilitar;
		}

		public Resultados<Cred.PragaHabilitarEmissao> Filtrar(Filtro<Cred.ListarFiltro> filtros)
		{
			Resultados<PragaHabilitarEmissao> retorno = new Resultados<PragaHabilitarEmissao>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("rt.usuario", "id", filtros.Dados.Id);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome_cientifico", "nome_comum", "cultura", "data_habilitacao_inicial", "data_habilitacao_final" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome_cientifico");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format(@"select count(hp.id) qtd from tab_hab_emi_cfo_cfoc_praga hp, tab_hab_emi_cfo_cfoc cc, tab_credenciado rt
				where hp.habilitar_emi_id = cc.id and cc.responsavel = rt.id " + comandtxt);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comandtxt = String.Format(@"
				select hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, trunc(hp.data_habilitacao_inicial) data_habilitacao_inicial, 
				trunc(hp.data_habilitacao_final) data_habilitacao_final, hp.tid, stragg(c.texto) cultura 
				from tab_hab_emi_cfo_cfoc cc, tab_hab_emi_cfo_cfoc_praga hp, tab_praga pa, tab_praga_cultura pc, tab_cultura c, tab_credenciado rt
				where hp.habilitar_emi_id = cc.id and pa.id = hp.praga and hp.praga = pc.praga(+) and pc.cultura = c.id(+) and rt.id = cc.responsavel {0}
				group by hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, hp.data_habilitacao_inicial, hp.data_habilitacao_final, hp.tid {1}", 
				comandtxt, DaHelper.Ordenar(colunas, ordenar));

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a)";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					PragaHabilitarEmissao praga;
					while (reader.Read())
					{
						praga = new PragaHabilitarEmissao();
						praga.Id = reader.GetValue<Int32>("id");
						praga.Praga.Id = reader.GetValue<Int32>("praga");
						praga.Praga.NomeCientifico = reader.GetValue<String>("nome_cientifico");
						praga.Praga.NomeComum = reader.GetValue<String>("nome_comum");
						praga.DataInicialHabilitacao = reader.GetValue<DateTime>("data_habilitacao_inicial").ToShortDateString();
						praga.DataFinalHabilitacao = reader.GetValue<DateTime>("data_habilitacao_final").ToShortDateString();
						praga.Tid = reader.GetValue<String>("tid");
						praga.Cultura = reader.GetValue<String>("cultura");
						retorno.Itens.Add(praga);
					}

					reader.Close();
					#endregion
				}
			}

			return retorno;
		}

		internal HabilitarEmissaoCFOCFOC ObterPorCredenciado(int credenciadoId, bool simplificado = false, BancoDeDados banco = null)
		{
			HabilitarEmissaoCFOCFOC retorno = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select h.id from tab_hab_emi_cfo_cfoc h where h.responsavel = :credenciado_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("credenciado_id", credenciadoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = new HabilitarEmissaoCFOCFOC();
						retorno.Id = reader.GetValue<int>("id");

					}

					reader.Close();
				}

				if (retorno != null)
				{
					retorno = Obter(retorno.Id, simplificado);
				}
			}

			return retorno;
		}

		#endregion

		internal bool VerificarCredenciadoHabilitado()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_credenciado c , tab_hab_emi_cfo_cfoc h where 
				c.id = h.responsavel and h.situacao <> 3 and c.id = :id");

				comando.AdicionarParametroEntrada("id", User.FuncionarioId, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

        internal Boolean ValidarResponsavelHabilitado(Int32 id)
        {
            Boolean retorno = false;
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_hab_emi_cfo_cfoc c where c.responsavel = :id");

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                retorno = bancoDeDados.ExecutarScalar(comando).ToString() != "0";
            }
            return retorno;
        }
	}
}