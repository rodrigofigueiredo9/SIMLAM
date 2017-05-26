using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCredenciado.Data
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

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

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

		#region Ações de DML

		public void Salvar(HabilitarEmissaoCFOCFOC habilitar, BancoDeDados banco = null, Executor executor = null)
		{
			if (habilitar == null)
			{
				throw new Exception("Habilitar emissão é nulo.");
			}

			if (habilitar.Id <= 0)
			{
				Criar(habilitar, banco, executor);
			}
			else
			{
				Editar(habilitar, banco, executor);
			}
		}

		internal int Criar(HabilitarEmissaoCFOCFOC habilitar, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region HabilitarEmissaoCFOCFOC

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				insert into {0}tab_hab_emi_cfo_cfoc a (id, responsavel, responsavel_arquivo, numero_habilitacao, validade_registro, situacao, 
				numero_dua, extensao_habilitacao, numero_habilitacao_ori, registro_crea, uf, numero_visto_crea, situacao_data, tid) values
				({0}seq_tab_hab_emi_cfo_cfoc.nextval, :responsavel, :responsavel_arquivo, :numero_habilitacao, :validade_registro, :situacao, 
				:numero_dua, :extensao_habilitacao, :numero_habilitacao_ori, :registro_crea, :uf, :numero_visto_crea, sysdate, :tid) returning a.id into :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("responsavel", habilitar.Responsavel.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel_arquivo", (habilitar.Arquivo != null && habilitar.Arquivo.Id > 0) ? habilitar.Arquivo.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_habilitacao", DbType.String, 30, habilitar.NumeroHabilitacao);
				comando.AdicionarParametroEntrada("validade_registro", habilitar.ValidadeRegistro, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", 1, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_dua", DbType.String, 30, habilitar.NumeroDua);
				comando.AdicionarParametroEntrada("extensao_habilitacao", habilitar.ExtensaoHabilitacao, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_habilitacao_ori", DbType.String, 30, habilitar.NumeroHabilitacaoOrigem);
				comando.AdicionarParametroEntrada("registro_crea", DbType.String, 30, habilitar.RegistroCrea);
				comando.AdicionarParametroEntrada("uf", habilitar.UF, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_visto_crea", DbType.String, 30, habilitar.NumeroVistoCrea);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				habilitar.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				#region Pragas

				if (habilitar.Pragas.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"
					insert into {0}tab_hab_emi_cfo_cfoc_praga a (id, habilitar_emi_id, praga, data_habilitacao_inicial, data_habilitacao_final, tid) 
					values ({0}seq_hab_emi_cfo_cfoc_praga.nextval, :habilitar, :praga, :data_habilitacao_inicial, :data_habilitacao_final, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("habilitar", habilitar.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("praga", DbType.Int32);
					comando.AdicionarParametroEntrada("data_habilitacao_inicial", DbType.DateTime);
					comando.AdicionarParametroEntrada("data_habilitacao_final", DbType.DateTime);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (var item in habilitar.Pragas)
					{
						comando.SetarValorParametro("praga", item.Praga.Id);
						comando.SetarValorParametro("data_habilitacao_inicial", item.DataInicialHabilitacao);
						comando.SetarValorParametro("data_habilitacao_final", item.DataFinalHabilitacao);
						bancoDeDados.ExecutarNonQuery(comando);
					}
				}

				#endregion

				Historico.Gerar(habilitar.Id, eHistoricoArtefato.habilitaremissaocfocfoc, eHistoricoAcao.criar, bancoDeDados, executor);

				bancoDeDados.Commit();

				return habilitar.Id;
			}
		}

		internal void Editar(HabilitarEmissaoCFOCFOC habilitar, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region HabilitarEmissaoCFOCFOC

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update tab_hab_emi_cfo_cfoc p set p.responsavel_arquivo = :responsavel_arquivo, p.numero_habilitacao = :numero_habilitacao, p.validade_registro = :validade_registro, 
				p.motivo = :motivo, p.observacao = :observacao, p.numero_dua = :numero_dua, p.extensao_habilitacao = :extensao_habilitacao, p.numero_habilitacao_ori = :numero_habilitacao_ori, 
				p.registro_crea = :registro_crea, p.uf = :uf, p.numero_visto_crea = :numero_visto_crea, p.tid = :tid where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", habilitar.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel_arquivo", (habilitar.Arquivo != null && habilitar.Arquivo.Id > 0) ? habilitar.Arquivo.Id : null, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_habilitacao", DbType.String, 30, habilitar.NumeroHabilitacao);
				comando.AdicionarParametroEntrada("validade_registro", habilitar.ValidadeRegistro, DbType.DateTime);
				comando.AdicionarParametroEntrada("motivo", (habilitar.Motivo.HasValue) ? habilitar.Motivo : null, DbType.Int32);
				comando.AdicionarParametroEntrada("observacao", DbType.String, 250, habilitar.Observacao);
				comando.AdicionarParametroEntrada("numero_dua", DbType.String, 30, habilitar.NumeroDua);
				comando.AdicionarParametroEntrada("extensao_habilitacao", habilitar.ExtensaoHabilitacao, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_habilitacao_ori", DbType.String, 30, (habilitar.ExtensaoHabilitacao == 1) ? habilitar.NumeroHabilitacaoOrigem : null);
				comando.AdicionarParametroEntrada("registro_crea", DbType.String, 30, habilitar.RegistroCrea);
				comando.AdicionarParametroEntrada("uf", habilitar.UF, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_visto_crea", DbType.String, 30, (habilitar.UF != 8) ? habilitar.NumeroVistoCrea : null);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Limpar os dados do banco

				//Pragas
				comando = bancoDeDados.CriarComando(@"delete {0}tab_hab_emi_cfo_cfoc_praga ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where habilitar_emi_id = :id {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, habilitar.Pragas.Select(x => x.Id).ToList()));

				comando.AdicionarParametroEntrada("id", habilitar.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Pragas

				if (habilitar.Pragas.Count > 0)
				{
					habilitar.Pragas.ForEach(item =>
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"
							update {0}tab_hab_emi_cfo_cfoc_praga a set tid = :tid, a.data_habilitacao_inicial = :data_habilitacao_inicial, 
							a.data_habilitacao_final = :data_habilitacao_final where id = :id", EsquemaBanco);

							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
							comando = bancoDeDados.CriarComando(@"
							insert into {0}tab_hab_emi_cfo_cfoc_praga a (id, tid, habilitar_emi_id, praga, data_habilitacao_inicial, data_habilitacao_final)
							values ({0}seq_hab_emi_cfo_cfoc_praga.nextval, :tid, :habilitar, :praga, :data_habilitacao_inicial, :data_habilitacao_final)", EsquemaBanco);

							comando.AdicionarParametroEntrada("habilitar", habilitar.Id, DbType.Int32);
							comando.AdicionarParametroEntrada("praga", item.Praga.Id, DbType.Int32);
						}

						comando.AdicionarParametroEntrada("data_habilitacao_inicial", item.DataInicialHabilitacao, DbType.DateTime);
						comando.AdicionarParametroEntrada("data_habilitacao_final", item.DataFinalHabilitacao, DbType.DateTime);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						bancoDeDados.ExecutarNonQuery(comando);
					});
				}

				#endregion

				Historico.Gerar(habilitar.Id, eHistoricoArtefato.habilitaremissaocfocfoc, eHistoricoAcao.atualizar, bancoDeDados, executor);

				bancoDeDados.Commit();
			}
		}

		internal void AlterarSituacao(HabilitarEmissaoCFOCFOC habilitar, BancoDeDados banco = null, Executor executor = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region HabilitarEmissaoCFOCFOC

				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update tab_hab_emi_cfo_cfoc p
                set p.motivo = :motivo,
                    p.observacao = :observacao,
                    p.situacao_data =:situacao_data, 
				    p.situacao = :situacao,
                    p.numero_dua = :numero_dua,
                    p.validade_registro = :validade_registro,
                    p.numero_processo = :numero_processo,
                    p.tid = :tid
                where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", habilitar.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", (habilitar.Motivo.HasValue && habilitar.Motivo > 0) ? habilitar.Motivo : null, DbType.Int32);
				comando.AdicionarParametroEntrada("observacao", DbType.String, 250, habilitar.Observacao);
				comando.AdicionarParametroEntrada("situacao_data", habilitar.SituacaoData, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", habilitar.Situacao, DbType.Int32);
                comando.AdicionarParametroEntrada("numero_dua", DbType.String, 30, habilitar.NumeroDua);
                comando.AdicionarParametroEntrada("validade_registro", Convert.ToDateTime(habilitar.ValidadeRegistro), DbType.DateTime);
                comando.AdicionarParametroEntrada("numero_processo", DbType.String, 30, habilitar.NumeroProcesso);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				Historico.Gerar(habilitar.Id, eHistoricoArtefato.habilitaremissaocfocfoc, eHistoricoAcao.alterarsituacao, bancoDeDados, executor);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Validações

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

        //Verificar se a situação da habilitação pode ser alterado para Ativa, caso a inativação tenha ocorrido por suspensão ou descredenciamento
        internal Boolean ValidarPodeAtivar(Int32 id)
        {
            Boolean retorno = true;

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                DateTime dataDesc = DateTime.Now.AddMonths(-18);
                DateTime dataSusp = DateTime.Now.AddMonths(-3);

                Comando comando = bancoDeDados.CriarComando(@"
                                    select count(th.situacao)
                                    from tab_hab_emi_cfo_cfoc th
                                    where th.situacao = 3
                                          and ((th.motivo = 1 and th.situacao_data >= '" + dataDesc.ToShortDateString() + // :dataDescredenciamento)
                                              @"') or (th.motivo = 2 and th.situacao >= '" + dataSusp.ToShortDateString() + // :dataSuspensao))
                                          @"')) and th.id = " + id);

                //comando.AdicionarParametroEntrada("dataDescredenciamento", dataDesc, DbType.Date);
                //comando.AdicionarParametroEntrada("dataSuspensao", dataSusp, DbType.Date);
                //comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                retorno = bancoDeDados.ExecutarScalar(comando).ToString() == "0";
            }
            return retorno;
        }

		internal Boolean ValidarNumeroHabilitacao(Int32 id, String numero)
		{
			Boolean retorno = false;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_hab_emi_cfo_cfoc c where c.id != :id and lower(replace(c.numero_habilitacao,' ')) = lower(replace(:numero_habilitacao,' '))");

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_habilitacao", DbType.String, 30, numero);

				retorno = bancoDeDados.ExecutarScalar(comando).ToString() != "0";
			}
			return retorno;
		}

		#endregion

		#region Obter / Filtrar

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

                if (filtros.Dados.Situacao != 0)
                {
                    comandtxt += " and cc.situacao = :situacao_habilitacao";

                    comando.AdicionarParametroEntrada("situacao_habilitacao", filtros.Dados.Situacao, DbType.Int32);
                }
                if (filtros.Dados.Motivo != 0)
                {
                    comandtxt += " and cc.motivo = :motivo_habilitacao";

                    comando.AdicionarParametroEntrada("motivo_habilitacao", filtros.Dados.Motivo, DbType.Int32);
                }

				comandtxt += comando.FiltroAnd("cc.numero_habilitacao", "numero_habilitacao", filtros.Dados.NumeroHabilitacao);

				if (!string.IsNullOrWhiteSpace(filtros.Dados.NomeComumPraga))
				{
					comandtxt += " and cc.id in (select ca.habilitar_emi_id from tab_hab_emi_cfo_cfoc_praga ca, tab_praga pa where ca.praga = pa.id and lower(pa.nome_cientifico) like lower('%'|| :praga ||'%') )";
					comando.AdicionarParametroEntrada("praga", filtros.Dados.NomeComumPraga, DbType.String);
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

				comandtxt = String.Format(@"
                            select cc.id,
                                   tc.tid,
                                   nvl(tp.nome, tp.razao_social) nome,
                                   nvl(tp.cpf, tp.cnpj) cpfcnpj,
                                   lct.texto tipo, 
				                   to_char(tc.data_cadastro, 'dd/mm/yyyy') ativacao,
                                   lcs.id situacao,
                                   lcs.texto situacao_texto,
                                   (case when cc.motivo is null then 0 else lcm.id end) motivo,
                                   (case when cc.motivo is null then '' else lcm.texto end) motivo_texto,
                                   (case when cc.extensao_habilitacao = 1 then cc.numero_habilitacao||'-ES' else cc.numero_habilitacao end) numero_habilitacao 
				            from tab_credenciado tc,
                                 cre_pessoa tp,
                                 {2}lov_credenciado_tipo lct,
                                 lov_hab_emissao_cfo_situacao lcs,
                                 tab_hab_emi_cfo_cfoc cc
                            left join lov_hab_emissao_cfo_motivo lcm
                            on cc.motivo = lcm.id
                            where tc.pessoa = tp.id
                                  and tc.tipo = lct.id
                                  and cc.situacao = lcs.id
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
                        item.Motivo = reader.GetValue<Int32>("motivo");
                        item.MotivoTexto = reader.GetValue<String>("motivo_texto");
						item.TipoTexto = reader.GetValue<String>("tipo");
						item.NumeroHabilitacao = reader.GetValue<String>("numero_habilitacao");

						retorno.Itens.Add(item);
                        int teste = 1;

                        if (item.Motivo == 0)
                        {
                            
                            teste++;
                        }
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal HabilitarEmissaoCFOCFOC Obter(int id, bool simplificado = false, string _schemaBanco = null, bool isCredenciado = false)
		{
			HabilitarEmissaoCFOCFOC habilitar = null;
			PragaHabilitarEmissao praga = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Habilitar Emissão

				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.tid, t.responsavel, t.responsavel_arquivo, aa.nome responsavel_arquivo_nome, cc.cpf responsavel_cpf, cc.nome responsavel_nome, 
				pc.registro registro_crea, t.numero_habilitacao, trunc(t.validade_registro) validade_registro, trunc(t.situacao_data) situacao_data, t.situacao, ls.texto situacao_texto, t.motivo,
				lm.texto motivo_texto, t.observacao, t.numero_dua, t.extensao_habilitacao, t.numero_habilitacao_ori, t.uf, t.numero_visto_crea, t.numero_processo, t.tid from tab_hab_emi_cfo_cfoc t, tab_credenciado tc, 
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
                        habilitar.NumeroProcesso = reader.GetValue<String>("numero_processo");
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

        internal List<HistoricoEmissaoCFOCFOC> ObterHistoricoHabilitacoes(int id, bool simplificado = false, string _schemaBanco = null)
        {
            List<HistoricoEmissaoCFOCFOC> historico = new List<HistoricoEmissaoCFOCFOC>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                #region Historico

                Comando comando = bancoDeDados.CriarComando(@"
                                    select hc.id,
                                           hc.tid,
                                           hc.situacao_id,
                                           hc.situacao_texto,
                                           hc.motivo_id,
                                           hc.motivo_texto,
                                           (case when hc.situacao_id = 3 then to_char(hc.situacao_data, 'DD/MM/YYYY') else '' end) penalidade_data,
                                           hc.numero_processo,
                                           to_char(hc.data_execucao, 'DD/MM/YYYY') data_execucao
                                    from hst_hab_emi_cfo_cfoc hc
                                    where hc.habilitar_emissao_id = :id
                                    order by hc.data_execucao desc", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);

                int cont = 0;
                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        HistoricoEmissaoCFOCFOC item = new HistoricoEmissaoCFOCFOC();

                        item = new HistoricoEmissaoCFOCFOC();
                        item.Id = reader.GetValue<Int32>("id");
                        item.Tid = reader.GetValue<String>("tid");
                        item.Situacao = reader.GetValue<Int32>("situacao_id");
                        item.SituacaoTexto = reader.GetValue<String>("situacao_texto");
                        item.Motivo = reader.GetValue<Int32>("motivo_id");
                        item.MotivoTexto = reader.GetValue<String>("motivo_texto");
                        item.SituacaoData = reader.GetValue<String>("penalidade_data");
                        item.NumeroProcesso = reader.GetValue<String>("numero_processo");
                        item.HistoricoData = reader.GetValue<String>("data_execucao");

                        //só adiciona o item se houve mudança na situação
                        if (cont == 0 || item.Situacao != historico[cont-1].Situacao)
                        {
                            historico.Add(item);
                            cont++;
                        }
                    }
                    reader.Close();
                }

                #endregion
            }

            return historico;
        }

		internal HabilitarEmissaoCFOCFOC ObterCredenciado(int id)
		{
			HabilitarEmissaoCFOCFOC habilitar = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Habilitar Emissão

				Comando comando = bancoDeDados.CriarComando(@"select tc.id responsavel, cc.cpf responsavel_cpf, cc.nome responsavel_nome, pc.registro registro_crea from
				tab_credenciado tc, cre_pessoa cc, cre_pessoa_profissao pc where tc.pessoa = cc.id and cc.id = pc.pessoa(+) and tc.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						habilitar = new HabilitarEmissaoCFOCFOC();
						habilitar.Responsavel.Id = reader.GetValue<Int32>("responsavel");
						habilitar.Responsavel.Pessoa.NomeRazaoSocial = reader.GetValue<String>("responsavel_nome");
						habilitar.Responsavel.Pessoa.CPFCNPJ = reader.GetValue<String>("responsavel_cpf");
						habilitar.RegistroCrea = reader.GetValue<String>("registro_crea");
					}

					reader.Close();
				}

				#endregion
			}

			return habilitar;
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
	}
}