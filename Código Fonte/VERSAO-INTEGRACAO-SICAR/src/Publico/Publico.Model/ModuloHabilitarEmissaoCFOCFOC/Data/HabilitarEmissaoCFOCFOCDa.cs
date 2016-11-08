using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloHabilitarEmissaoCFOCFOC.Data
{
	public class HabilitarEmissaoCFOCFOCDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		Consulta _consulta = new Consulta();
		internal Consulta Consulta { get { return _consulta; } }

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
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

		#region Obter / Filtrar

		internal HabilitarEmissaoCFOCFOC Obter(int id, bool simplificado = false, string _schemaBanco = null, bool isCredenciado = false)
		{
			HabilitarEmissaoCFOCFOC habilitar = null;
			PragaHabilitarEmissao praga = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Habilitar Emissão

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, cc.id pessoa, t.responsavel, t.responsavel_arquivo, aa.nome responsavel_arquivo_nome, cc.cpf responsavel_cpf, cc.nome responsavel_nome, 
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
						habilitar.Responsavel.Pessoa.Id = reader.GetValue<Int32>("pessoa");
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
					where pa.id = hp.praga and hp.praga = pc.praga(+) and pc.cultura = c.id(+) and hp.habilitar_emi_id = :id
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

					comando = bancoDeDados.CriarComando(@"select 
					(select c.valor from cre_pessoa_meio_contato c where c.pessoa = :id and c.meio_contato = 1) res,
					(select c.valor from cre_pessoa_meio_contato c where c.pessoa = :id and c.meio_contato = 2) cel,
					(select c.valor from cre_pessoa_meio_contato c where c.pessoa = :id and c.meio_contato = 3) fax,
					(select c.valor from cre_pessoa_meio_contato c where c.pessoa = :id and c.meio_contato = 4) com,
					(select c.valor from cre_pessoa_meio_contato c where c.pessoa = :id and c.meio_contato = 5) email
					from dual", UsuarioCredenciado);

					comando.AdicionarParametroEntrada("id", habilitar.Responsavel.Pessoa.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							habilitar.TelefoneResidencial = reader.GetValue<String>("res");
							habilitar.TelefoneCelular = reader.GetValue<String>("cel");
							habilitar.TelefoneFax = reader.GetValue<String>("fax");
							habilitar.TelefoneComercial = reader.GetValue<String>("com");
							habilitar.Email = reader.GetValue<String>("email");
						}
						reader.Close();
					}
				}

				#endregion
			}

			return habilitar;
		}

		public Resultados<Cred.ListarFiltro> Filtrar(Filtro<Cred.ListarFiltro> filtros)
		{
			Resultados<Cred.ListarFiltro> retorno = new Resultados<Cred.ListarFiltro>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string credenciado = String.IsNullOrEmpty(UsuarioCredenciado) ? "" : UsuarioCredenciado + ".";
				string comandtxt = string.Empty;
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("nvl(tp.nome, tp.razao_social)", "nome", filtros.Dados.NomeRazaoSocial, true, true);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.CpfCnpj))
				{
					if (ValidacoesGenericasBus.Cpf(filtros.Dados.CpfCnpj) || ValidacoesGenericasBus.Cnpj(filtros.Dados.CpfCnpj))
					{
						comandtxt += " and ((tp.cpf = :cpfcnpj) or (tp.cnpj = :cpfcnpj))";
					}
					else
					{
						comandtxt += " and ((tp.cpf like '%'|| :cpfcnpj ||'%') or (tp.cnpj = '%'|| :cpfcnpj ||'%'))";
					}

					comando.AdicionarParametroEntrada("cpfcnpj", filtros.Dados.CpfCnpj, DbType.String);
				}

				comandtxt += comando.FiltroAnd("cc.numero_habilitacao", "numero_habilitacao", filtros.Dados.NumeroHabilitacao);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.NomeComumPraga))
				{
					comandtxt += " and cc.id in (select ca.habilitar_emi_id from tab_hab_emi_cfo_cfoc_praga ca, tab_praga pa where ca.praga = pa.id and lower(pa.nome_cientifico) like '%'|| :praga ||'%')";
					comando.AdicionarParametroEntrada("praga", filtros.Dados.NomeComumPraga.ToLower(), DbType.String);
				}

				if (!string.IsNullOrWhiteSpace(filtros.Dados.NomeCultura))
				{
					comandtxt += @" and cc.id in ( select aa.habilitar_emi_id from tab_hab_emi_cfo_cfoc_praga aa, tab_praga_cultura bb, tab_cultura c where aa.praga = bb.praga and c.id = bb.cultura 
					and lower(c.texto) like  '%'|| :cultura ||'%' )";
					comando.AdicionarParametroEntrada("cultura", filtros.Dados.NomeCultura.ToLower(), DbType.String);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "nome", "numero_habilitacao", "situacao_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("nome");
				}
				#endregion

				#region Executa a pesquisa nas tabelas

				comando.DbCommand.CommandText = String.Format(@"select count(*) from tab_credenciado tc, cre_pessoa tp,
					{0}lov_credenciado_tipo lct, {0}lov_credenciado_situacao lcs, tab_hab_emi_cfo_cfoc cc where tc.pessoa = tp.id 
					and tc.tipo = lct.id and tc.situacao = lcs.id and cc.responsavel = tc.id" + comandtxt, credenciado);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				if (retorno.Quantidade < 1)
				{
					Validacao.Add(Mensagem.Funcionario.NaoEncontrouRegistros);
				}

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select cc.id, tc.tid, nvl(tp.nome, tp.razao_social) nome, nvl(tp.cpf, tp.cnpj) cpfcnpj, lct.texto tipo, 
				to_char(tc.data_cadastro, 'dd/mm/yyyy') ativacao, lcs.id situacao, lcs.texto situacao_texto, cc.numero_habilitacao from tab_credenciado tc, cre_pessoa tp,
				{2}lov_credenciado_tipo lct, lov_hab_emissao_cfo_situacao lcs, tab_hab_emi_cfo_cfoc cc where tc.pessoa = tp.id and tc.tipo = lct.id and cc.situacao = lcs.id
				and cc.responsavel = tc.id {0} {1}", comandtxt, DaHelper.Ordenar(colunas, ordenar), credenciado);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					Cred.ListarFiltro item;
					while (reader.Read())
					{
						item = new Cred.ListarFiltro();
						item.Id = reader.GetValue<String>("id");
						item.Tid = reader.GetValue<String>("tid");
						item.NomeRazaoSocial = reader.GetValue<String>("nome");
						item.CpfCnpj = reader.GetValue<String>("cpfcnpj");
						item.DataAtivacao = reader.GetValue<String>("ativacao");
						item.Situacao = reader.GetValue<Int32>("situacao");
						item.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						item.TipoTexto = reader.GetValue<String>("tipo");
						item.NumeroHabilitacao = reader.GetValue<String>("numero_habilitacao");

						retorno.Itens.Add(item);
					}

					reader.Close();
					#endregion
				}
			}

			return retorno;
		}

		#endregion

	}
}
