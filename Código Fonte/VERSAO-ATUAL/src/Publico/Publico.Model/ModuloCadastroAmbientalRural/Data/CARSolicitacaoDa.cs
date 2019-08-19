using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using System.Linq;

namespace Tecnomapas.EtramiteX.Publico.Model.ModuloCadastroAmbientalRural.Data
{
	class CARSolicitacaoDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public string UsuarioCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public string UsuarioInterno
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		private String EsquemaBanco { get; set; }

		#endregion

		public CARSolicitacaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		internal CARSolicitacao ObterInterno(int id, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select s.tid,
					s.numero,
					s.data_emissao,
					s.situacao_data,
					l.id situacao,
					l.texto situacao_texto,
					s.situacao_anterior,
					la.texto situacao_anterior_texto,
					s.situacao_anterior_data,
					p.id protocolo_id,
					p.protocolo,
					p.numero protocolo_numero,
					p.ano protocolo_ano,
					nvl(pes.nome, pes.razao_social) declarante_nome_razao,
					ps.id protocolo_selecionado_id,
					ps.protocolo protocolo_selecionado,
					ps.numero protocolo_selecionado_numero,
					ps.ano protocolo_selecionado_ano,
					s.requerimento,
					s.atividade,
					e.id empreendimento_id,
					e.denominador empreendimento_nome,
					e.codigo empreendimento_codigo,
					s.declarante,

					f.funcionario_id autor_id,
					f.nome autor_nome,
					(select stragg_barra(sigla) from hst_setor where 
					setor_id in (select fs.setor_id from hst_funcionario_setor fs where fs.id_hst = f.id)
					and tid in (select fs.setor_tid from hst_funcionario_setor fs where fs.id_hst = f.id )) autor_setor,
					'Institucional' autor_modulo,

					s.autor,
					s.motivo,
					s.arquivo,
					tr.data_criacao requerimento_data_cadastro
				from tab_car_solicitacao         s,
					lov_car_solicitacao_situacao l,
					lov_car_solicitacao_situacao la,
					tab_protocolo                p,
					tab_protocolo                ps,
					tab_empreendimento           e,
					tab_pessoa                   pes,
					tab_requerimento             tr,
					hst_funcionario              f
				where s.situacao = l.id
				and s.situacao_anterior = la.id(+)
				and s.protocolo = p.id
				and s.protocolo_selecionado = ps.id(+)
				and s.empreendimento = e.id
				and s.declarante = pes.id
				and s.requerimento = tr.id
				and f.funcionario_id = s.autor
				and f.tid = (select autor_tid from hst_car_solicitacao where acao_executada = 342 and solicitacao_id = s.id)
				and s.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = id;
						solicitacao.Tid = reader.GetValue<String>("tid");
						solicitacao.Numero = reader.GetValue<String>("numero");
						solicitacao.DataEmissao.DataTexto = reader.GetValue<String>("data_emissao");
						solicitacao.SituacaoId = reader.GetValue<Int32>("situacao");
						solicitacao.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						solicitacao.DataSituacao.DataTexto = reader.GetValue<String>("situacao_data");
						solicitacao.SituacaoAnteriorId = reader.GetValue<Int32>("situacao_anterior");
						solicitacao.SituacaoAnteriorTexto = reader.GetValue<String>("situacao_anterior_texto");
						solicitacao.DataSituacaoAnterior.DataTexto = reader.GetValue<String>("situacao_anterior_data");
						solicitacao.Protocolo.Id = reader.GetValue<Int32>("protocolo_id");
						solicitacao.Protocolo.IsProcesso = reader.GetValue<Int32>("protocolo") == 1;
						solicitacao.Protocolo.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_numero");
						solicitacao.Protocolo.Ano = reader.GetValue<Int32>("protocolo_ano");
						solicitacao.ProtocoloSelecionado.Id = reader.GetValue<Int32>("protocolo_selecionado_id");
						solicitacao.ProtocoloSelecionado.IsProcesso = reader.GetValue<Int32>("protocolo_selecionado") == 1;
						solicitacao.ProtocoloSelecionado.NumeroProtocolo = reader.GetValue<Int32?>("protocolo_selecionado_numero");
						solicitacao.ProtocoloSelecionado.Ano = reader.GetValue<Int32>("protocolo_selecionado_ano");
						solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
						solicitacao.Requerimento.DataCadastro = reader.GetValue<DateTime>("requerimento_data_cadastro");
						solicitacao.Atividade.Id = reader.GetValue<Int32>("atividade");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
						solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante");
						solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nome_razao");

						solicitacao.AutorId = reader.GetValue<Int32>("autor_id");
						solicitacao.AutorNome = reader.GetValue<String>("autor_nome");
						solicitacao.AutorSetorTexto = reader.GetValue<String>("autor_setor");
						solicitacao.AutorModuloTexto = reader.GetValue<String>("autor_modulo");

						solicitacao.DescricaoMotivo = reader.GetValue<String>("motivo");
						solicitacao.Arquivo = reader.GetValue<Int32>("arquivo");
					}

					reader.Close();
				}

				#endregion
			}

			return solicitacao;
		}

		internal CARSolicitacao ObterCredenciado(int id, BancoDeDados banco = null)
		{
			CARSolicitacao solicitacao = new CARSolicitacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
				select s.tid,
					s.numero,
					s.data_emissao,
					s.situacao_data,
					l.id situacao,
					l.texto situacao_texto,
					s.situacao_anterior,
					la.texto situacao_anterior_texto,
					s.situacao_anterior_data,
					nvl(pes.nome, pes.razao_social) declarante_nome_razao,
					s.requerimento,
					s.atividade,
					e.id empreendimento_id,
					e.denominador empreendimento_nome,
					e.codigo empreendimento_codigo,
					s.declarante,
					s.motivo,
					tr.data_criacao requerimento_data_cadastro,
					s.projeto_digital,
					s.arquivo
				from 
					tab_car_solicitacao          s,
					lov_car_solicitacao_situacao l,
					lov_car_solicitacao_situacao la,
					tab_empreendimento           e,
					tab_pessoa                   pes,
					tab_requerimento             tr
				where s.situacao = l.id
				and s.situacao_anterior = la.id(+)
				and s.empreendimento = e.id
				and s.declarante = pes.id
				and s.requerimento = tr.id
				and s.empreendimento = e.id
				and s.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						solicitacao.Id = id;
						solicitacao.Tid = reader.GetValue<String>("tid");
						solicitacao.Numero = reader.GetValue<String>("numero");
						solicitacao.DataEmissao.DataTexto = reader.GetValue<String>("data_emissao");
						solicitacao.SituacaoId = reader.GetValue<Int32>("situacao");
						solicitacao.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						solicitacao.DataSituacao.DataTexto = reader.GetValue<String>("situacao_data");
						solicitacao.SituacaoAnteriorId = reader.GetValue<Int32>("situacao_anterior");
						solicitacao.SituacaoAnteriorTexto = reader.GetValue<String>("situacao_anterior_texto");
						solicitacao.DataSituacaoAnterior.DataTexto = reader.GetValue<String>("situacao_anterior_data");
						solicitacao.Requerimento.Id = reader.GetValue<Int32>("requerimento");
						solicitacao.Requerimento.DataCadastro = reader.GetValue<DateTime>("requerimento_data_cadastro");
						solicitacao.Atividade.Id = reader.GetValue<Int32>("atividade");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
						solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante");
						solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nome_razao");
						solicitacao.DescricaoMotivo = reader.GetValue<String>("motivo");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_digital");
						solicitacao.Arquivo = reader.GetValue<Int32>("arquivo");
					}

					reader.Close();
				}

				#endregion
			}

			return solicitacao;
		}

		internal List<PessoaLst> ObterDeclarantes(int requerimentoId)
		{
			List<PessoaLst> retorno = new List<PessoaLst>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl(r.interessado,0) id, 'Interessado' || ' - ' || nvl(tp.nome, tp.razao_social) texto from {0}tab_requerimento r, 
				{0}tab_pessoa tp where r.interessado = tp.id and r.id = :id union all select nvl(er.responsavel,0) id, ler.texto || ' - ' || nvl(tp.nome, tp.razao_social) texto from 
				{0}tab_requerimento r, {0}tab_empreendimento_responsavel er, {0}tab_pessoa tp, {0}lov_empreendimento_tipo_resp ler where er.responsavel = tp.id and er.tipo = ler.id and r.id = :id
				and r.empreendimento = er.empreendimento union all select nvl(rr.responsavel,0) id, 'Responsável técnico' || ' - ' || nvl(tp.nome, tp.razao_social) texto from 
				{0}tab_requerimento_responsavel rr, {0}tab_pessoa tp where rr.responsavel = tp.id and rr.requerimento = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", requerimentoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaLst pes;
					while (reader.Read())
					{
						pes = new PessoaLst();
						pes.Id = reader.GetValue<int>("id");
						pes.Texto = reader.GetValue<string>("texto");
						pes.IsAtivo = true;
						retorno.Add(pes);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal Resultados<SolicitacaoListarResultados> Filtrar(Filtro<SolicitacaoListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<SolicitacaoListarResultados> retorno = new Resultados<SolicitacaoListarResultados>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = string.IsNullOrEmpty(EsquemaBanco) ? "" : ".";
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("l.solicitacao_numero", "solicitacao_numero", filtros.Dados.SolicitacaoNumero);
				comandtxt += comando.FiltroAnd("l.declarante_cpf_cnpj", "declarante_cpf_cnpj", filtros.Dados.DeclaranteCPFCNPJ);

				comandtxt += comando.FiltroAnd("l.empreendimento_codigo", "empreendimento_codigo", filtros.Dados.EmpreendimentoCodigo);
				comandtxt += comando.FiltroAnd("l.responsavel", "responsavel_cpf_cnpj", filtros.Dados.ResponsavelEmpreendimentoCPFCNPJ);


				comandtxt += comando.FiltroAndLike("l.protocolo_numero_completo", "protocolo_numero_completo", filtros.Dados.Protocolo.NumeroTexto);
				comandtxt += comando.FiltroAndLike("l.declarante_nome_razao", "declarante_nome_razao", filtros.Dados.DeclaranteNomeRazao, true);
				comandtxt += comando.FiltroAndLike("l.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoDenominador, true);
				comandtxt += comando.FiltroAnd("l.municipio_id", "municipio_id", filtros.Dados.Municipio);
				comandtxt += comando.FiltroAnd("l.requerimento", "requerimento", filtros.Dados.Requerimento);
				comandtxt += comando.FiltroAnd("l.titulo_numero", "titulo_numero", filtros.Dados.Titulo.Inteiro);
				comandtxt += comando.FiltroAnd("l.titulo_ano", "titulo_ano", filtros.Dados.Titulo.Ano);
				comandtxt += comando.FiltroAnd("l.origem", "origem", filtros.Dados.Origem);

				if (!String.IsNullOrWhiteSpace(filtros.Dados.Situacao))
				{
					comandtxt += comando.FiltroAnd("l.situacao_id", "situacao", filtros.Dados.Situacao);
					comandtxt += comando.FiltroAnd("l.tipo", "tipo", 1);//Solicitacao
				}

				comandtxt += String.Format(" and l.situacao in ({0})", String.Join(",", "2, 3, 4, 5"));

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "solicitacao_numero", "empreendimento_denominador" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("solicitacao_numero");
				}

				#endregion

				#region Query
				string query = @"
					select 	t.id, 
						t.numero solicitacao_numero, 
						'' responsavel,
						nvl(pe.cpf, pe.cnpj) declarante_cpf_cnpj,
						null titulo_numero, 
						null titulo_ano,
						e.codigo empreendimento_codigo,
						e.denominador empreendimento_denominador,
						t.situacao, 
						1 tipo,
						1 origem
       	 
				   from 	tab_car_solicitacao t 
       						inner join idaf.tab_empreendimento              e  on e.id = t.empreendimento
							inner join idaf.tab_pessoa                      pe on pe.id = t.declarante 
				   union all 
				   select ht.titulo_id id, 
						  null solicitacao_numero, 
						  nvl(hp.cpf, hp.cnpj) responsavel,
						  '' declarante_cpf_cnpj,
						  tn.numero titulo_numero, 
						  tn.ano titulo_ano,
						  he.codigo empreendimento_codigo,
						  he.denominador empreendimento_denominador, 
						  ht.situacao_id, 
						  2 tipo,
						  null origem

				   from   hst_titulo ht
						  inner join tab_titulo_numero tn               ON ht.titulo_id = tn.titulo
						  inner join hst_empreendimento he              ON he.empreendimento_id = ht.empreendimento_id and he.tid = ht.empreendimento_tid
						  inner join hst_empreendimento_responsavel her ON  her.id_hst = he.id
						  inner join hst_pessoa hp                      ON  hp.pessoa_id = her.responsavel_id and hp.tid = her.responsavel_tid
              
				   where ht.modelo_id = 66 
				   union all 
				   select        t.id, 
						  t.numero solicitacao_numero, 
						  '' responsavel,
						  nvl(pe.cpf, pe.cnpj) declarante_cpf_cnpj,
						  null titulo_numero, 
						  null titulo_ano,
						  e.codigo empreendimento_codigo,
						  e.denominador empreendimento_denominador,
						  t.situacao, 
						  1 tipo,
						  2 origem
               
				   from   idafcredenciado.tab_car_solicitacao t 
						  inner join idafcredenciado.tab_empreendimento              e  on e.id = t.empreendimento
						  inner join idafcredenciado.tab_pessoa                      pe on pe.id = t.declarante ";

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format($@"
				select count(1) from ( {query} ) l where 1 = 1" + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = $@"select l.id, l.solicitacao_numero, l.responsavel , l.declarante_cpf_cnpj, l.titulo_ano ano, l.empreendimento_denominador, 
				l.situacao, l.origem, l.tipo from        
				(
				{query}
				) l where 1 = 1" + comandtxt + DaHelper.Ordenar(colunas, ordenar);

				comando.DbCommand.CommandText = String.Format(@"select distinct
				id, responsavel, solicitacao_numero, ano, empreendimento_denominador, situacao, origem, tipo
				from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", esquemaBanco);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando o retorno

					SolicitacaoListarResultados item;

					while (reader.Read())
					{
						item = new SolicitacaoListarResultados();
						item.Id = reader.GetValue<int>("id");
						item.Numero = reader.GetValue<string>("solicitacao_numero");
						item.Ano = reader.GetValue<string>("ano");
						item.EmpreendimentoDenominador = reader.GetValue<string>("empreendimento_denominador");
						//item.MunicipioTexto = reader.GetValue<string>("municipio_texto");
						item.SituacaoID = reader.GetValue<int>("situacao");
						//item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.IsTitulo = reader.GetValue<int>("tipo") == 2;
						//item.CredenciadoId = reader.GetValue<int>("credenciado");
						item.Origem = (eCARSolicitacaoOrigem)(reader.GetValue<int>("origem"));
						//item.SituacaoArquivoCarID = reader.GetValue<int>("situacao_envio");
						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion Adicionando o retorno
				}
			}

			return retorno;
		}

		internal string ObterUrlGeracaoDemonstrativo(int id, int schemaSolicitacao, bool isTitulo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando;
				if (!isTitulo)
				{
					comando = bancoDeDados.CriarComando(@"select tcs.codigo_imovel from tab_controle_sicar tcs where tcs.solicitacao_car = :id and tcs.solicitacao_car_esquema = :schemaSolicitacao");
					comando.AdicionarParametroEntrada("schemaSolicitacao", schemaSolicitacao, DbType.Int32);
				}
				else
				{

					comando = bancoDeDados.CriarComando(@"SELECT CODIGO_IMOVEL FROM (SELECT  CS.CODIGO_IMOVEL, TT.ID TITULO FROM TAB_TITULO TT 
															INNER JOIN TAB_CONTROLE_SICAR CS ON TT.EMPREENDIMENTO = CS.EMPREENDIMENTO
														  WHERE TT.SITUACAO = 3 /*Concluído*/ AND CS.SOLICITACAO_CAR_ESQUEMA = 1 AND TT.ID = :id
														UNION ALL
														SELECT CS.CODIGO_IMOVEL, TT.ID TITULO FROM TAB_TITULO TT 																
															INNER JOIN TAB_CONTROLE_SICAR CS ON TT.EMPREENDIMENTO = (select e.id from IDAF.TAB_EMPREENDIMENTO e
															where e.codigo = (select ec.codigo from IDAFCREDENCIADO.TAB_EMPREENDIMENTO ec where ec.id = CS.EMPREENDIMENTO)) 
														  WHERE TT.SITUACAO = 3 /*Concluído*/ AND CS.SOLICITACAO_CAR_ESQUEMA = 2 AND TT.ID = :id)
														  WHERE ROWNUM = 1 ORDER BY TITULO DESC");

				}
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

		#endregion

		#region Validacao

		internal bool ExisteCredenciado(int solicitacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_car_solicitacao where id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", solicitacaoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion
	}
}