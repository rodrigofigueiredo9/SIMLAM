using System;
using System.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloCadastroAmbientalRural.Data
{
	public class CARSolicitacaoInternoDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public string UsuarioCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		public string UsuarioInterno
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		#endregion

		public bool EmpreendimentoPossuiTituloCAREncerrado(int codigoEmpreendimento)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioInterno))
			{
				Comando comando;

				comando = bancoDeDados.CriarComando(@"select count(1) from tab_titulo t, tab_empreendimento te where t.empreendimento = te.id and t.modelo = 81 
				and t.situacao != 5 and te.codigo = :codigo /*Encerrado*/", UsuarioInterno);
				comando.AdicionarParametroEntrada("codigo", codigoEmpreendimento, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal Resultados<SolicitacaoListarResultados> Filtrar(Filtro<SolicitacaoListarFiltro> filtros, BancoDeDados banco = null)
		{
			Resultados<SolicitacaoListarResultados> retorno = new Resultados<SolicitacaoListarResultados>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = string.IsNullOrEmpty(UsuarioInterno) ? "" : ".";
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("l.solicitacao_numero", "solicitacao_numero", filtros.Dados.SolicitacaoNumero);

				comandtxt += comando.FiltroAnd("l.empreendimento_codigo", "empreendimento_codigo", filtros.Dados.EmpreendimentoCodigo);

				comandtxt += comando.FiltroAndLike("l.protocolo_numero_completo", "protocolo_numero_completo", filtros.Dados.Protocolo.NumeroTexto);

				comandtxt += comando.FiltroAndLike("l.declarante_nome_razao", "declarante_nome_razao", filtros.Dados.DeclaranteNomeRazao);

				comandtxt += comando.FiltroAnd("l.declarante_cpf_cnpj", "declarante_cpf_cnpj", filtros.Dados.DeclaranteCPFCNPJ);

				comandtxt += comando.FiltroAndLike("l.empreendimento_denominador", "empreendimento_denominador", filtros.Dados.EmpreendimentoDenominador);

				comandtxt += comando.FiltroAnd("l.municipio_id", "municipio_id", filtros.Dados.Municipio);

				comandtxt += comando.FiltroAnd("l.requerimento", "requerimento", filtros.Dados.Requerimento);

				comandtxt += comando.FiltroAnd("l.titulo_numero", "titulo_numero", filtros.Dados.Titulo.Inteiro);

				comandtxt += comando.FiltroAnd("l.titulo_ano", "titulo_ano", filtros.Dados.Titulo.Ano);

				comandtxt += comando.FiltroAnd("l.situacao_id", "situacao", filtros.Dados.Situacao);

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(1) from {0}lst_car_solic_tit l, {0}tab_requerimento tr where 1 = 1 and l.requerimento = tr.id and
				(l.requerimento in (select r.id from {0}tab_requerimento r, {0}tab_pessoa tp where r.interessado = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or 
				l.requerimento in (select r.id from {0}tab_requerimento r, {0}tab_pessoa tp, {0}tab_requerimento_responsavel trr where r.id = trr.requerimento and trr.responsavel = tp.id 
				and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or l.requerimento in (select r.id {0}from tab_requerimento r, {0}tab_pessoa tp, {0}tab_empreendimento te, 
				{0}tab_empreendimento_responsavel ter where r.empreendimento = te.id and te.id = ter.empreendimento and ter.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj)) " + comandtxt, esquemaBanco);

				comando.AdicionarParametroEntrada("cpfCnpj", filtros.Dados.AutorCPFCNPJ);
				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = @"select l.solic_tit_id, nvl(l.solicitacao_numero, l.titulo_numero) numero, l.titulo_ano ano, l.empreendimento_denominador, 
				l.municipio_texto, l.situacao_texto, l.tipo from lst_car_solic_tit l, {0}tab_requerimento tr where 1 = 1 and l.requerimento = tr.id and
				(l.requerimento in (select r.id from {0}tab_requerimento r, {0}tab_pessoa tp where r.interessado = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or 
				l.requerimento in (select r.id from {0}tab_requerimento r, {0}tab_pessoa tp, {0}tab_requerimento_responsavel trr where r.id = trr.requerimento and trr.responsavel = tp.id 
				and nvl(tp.cpf, tp.cnpj) = :cpfCnpj) or l.requerimento in (select r.id {0}from tab_requerimento r, {0}tab_pessoa tp, {0}tab_empreendimento te, 
				{0}tab_empreendimento_responsavel ter where r.empreendimento = te.id and te.id = ter.empreendimento and ter.responsavel = tp.id and nvl(tp.cpf, tp.cnpj) = :cpfCnpj)) " + comandtxt;

				comando.DbCommand.CommandText = String.Format(@"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor", esquemaBanco);

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando o retorno

					SolicitacaoListarResultados item;

					while (reader.Read())
					{
						item = new SolicitacaoListarResultados();
						item.Id = reader.GetValue<int>("solic_tit_id");
						item.Numero = reader.GetValue<string>("numero");
						item.Ano = reader.GetValue<string>("ano");
						item.EmpreendimentoDenominador = reader.GetValue<string>("empreendimento_denominador");
						item.MunicipioTexto = reader.GetValue<string>("municipio_texto");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.IsTitulo = reader.GetValue<int>("tipo") == 2;
						retorno.Itens.Add(item);
					}

					reader.Close();

					#endregion Adicionando o retorno
				}
			}

			return retorno;
		}

		internal CARSolicitacao Obter(int id, bool simplificado = false, BancoDeDados banco = null)
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
					ta.atividade atividade_nome,
					e.id empreendimento_id,
					e.denominador empreendimento_nome,
					e.codigo empreendimento_codigo,
					s.declarante,
					s.motivo,
					tr.data_criacao requerimento_data_cadastro,
					pg.id projeto_geo_id
					
				from tab_car_solicitacao         s,
					lov_car_solicitacao_situacao l,
					lov_car_solicitacao_situacao la,
					tab_protocolo                p,
					tab_protocolo                ps,
					tab_empreendimento           e,
					crt_projeto_geo              pg,
					tab_pessoa                   pes,
					tab_requerimento             tr,
					tab_atividade                ta
				where s.situacao = l.id
				and s.situacao_anterior = la.id(+)
				and s.protocolo = p.id
				and s.protocolo_selecionado = ps.id(+)
				and s.empreendimento = e.id
				and s.empreendimento = pg.empreendimento
				and s.declarante = pes.id
				and s.requerimento = tr.id
				and pg.caracterizacao = 1
				and s.atividade = ta.id
				and s.id = :id", UsuarioInterno);

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
						solicitacao.Atividade.NomeAtividade = reader.GetValue<String>("atividade_nome");
						solicitacao.Empreendimento.Id = reader.GetValue<Int32>("empreendimento_id");
						solicitacao.Empreendimento.NomeRazao = reader.GetValue<String>("empreendimento_nome");
						solicitacao.Empreendimento.Codigo = reader.GetValue<Int64?>("empreendimento_codigo");
						solicitacao.Declarante.Id = reader.GetValue<Int32>("declarante");
						solicitacao.Declarante.NomeRazaoSocial = reader.GetValue<String>("declarante_nome_razao");
						solicitacao.DescricaoMotivo = reader.GetValue<String>("motivo");
						solicitacao.ProjetoId = reader.GetValue<Int32>("projeto_geo_id");
						solicitacao.AutorModuloTexto = "Institucional";
					}

					reader.Close();
				}

				#endregion
			}
			
			return solicitacao;
		}

		internal int ObterNovoID(BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando("select {0}seq_car_solicitacao.nextval from dual", UsuarioInterno);
				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		internal string ObterSituacaoTituloCARExistente(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select ls.texto situacao_texto from tab_titulo t, lov_titulo_situacao ls
				where t.situacao <> 5 /*Encerrado*/ and t.modelo = (select id from tab_titulo_modelo where codigo = 49 /*Cadastro Ambiental Rural*/)
				and t.empreendimento = :empreendimento and ls.id = t.situacao", UsuarioInterno);

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

        internal string ObterSituacaoTituloCARCodigoEmp(Int64 empreendimentoCod, BancoDeDados banco = null)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
				select ls.texto situacao_texto from tab_titulo t
                    inner join lov_titulo_situacao ls on ls.id = t.situacao
                    inner join tab_empreendimento e on t.empreendimento = e.id
				where t.situacao <> 5 /*Encerrado*/ and t.modelo = (select id from tab_titulo_modelo where codigo = 49 /*Cadastro Ambiental Rural*/)
				and e.codigo = :empreendimento", UsuarioInterno);

                comando.AdicionarParametroEntrada("empreendimento", empreendimentoCod, DbType.Int32);

                return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
            }
        }

		internal string EmpreendimentoPossuiSolicitacao(int empreendimentoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select l.texto situacao from tab_car_solicitacao t, lov_car_solicitacao_situacao l 
				where t.situacao = l.id and t.empreendimento = :empreendimentoID and situacao in (1, 2, 4)", UsuarioInterno);

				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoId, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

        internal CARSolicitacao EmpreendimentoPossuiSolicitacaoProjetoDigital(int empreendimentoId, BancoDeDados banco = null)
        {
            CARSolicitacao car = new CARSolicitacao();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select t.id, t.situacao, pd.id projeto_digital from tab_car_solicitacao t, lov_car_solicitacao_situacao l, tmp_projeto_digital pd 
				where t.situacao = l.id and t.empreendimento = :empreendimentoID and situacao in (1, 2, 4) and t.empreendimento = pd.empreendimento_id", UsuarioInterno);

                comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        car.Id = reader.GetValue<Int32>("id");
                        car.SituacaoId = reader.GetValue<Int32>("situacao");
                        car.ProjetoId = reader.GetValue<Int32>("projeto_digital");
                    }
                    reader.Close();
                }

                return car;
            }
        }

		internal string EmpreendimentoPossuiSolicitacao(string CNPJ, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select ls.texto from tab_empreendimento t , tab_car_solicitacao c, lov_car_solicitacao_situacao ls 
				where t.id = c.empreendimento and c.situacao = ls.id and c.situacao in (1, 2, 4) and t.cnpj = :cnpj and rownum = 1", UsuarioInterno);

				comando.AdicionarParametroEntrada("cnpj", CNPJ, DbType.String);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal void InserirFilaArquivoCarSicar(int solicitacaoId, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados banco = null)
		{
			string requisicao_fila = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioInterno))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"select s.solicitacao_id solic_id, s.tid solic_tid, s.empreendimento_id emp_id, s.empreendimento_tid emp_tid from hst_car_solicitacao s where s.solicitacao_id = :idSolicitacao
					and s.tid = (select ss.tid from tab_car_solicitacao ss where ss.id= :idSolicitacao) order by id desc", UsuarioInterno);

				comando.AdicionarParametroEntrada("idSolicitacao", solicitacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						requisicao_fila = "'{\"origem\":" + (int)solicitacaoOrigem + ",\"empreendimento\":" +
							reader.GetValue<int>("emp_id") + ",\"empreendimento_tid\":" + reader.GetValue<string>("emp_tid") +
							",\"solicitacao_car\":" + reader.GetValue<int>("solic_id") + ",\"solicitacao_car_tid\":" + reader.GetValue<string>("solic_tid") + "\"}'"; ;
					}

					reader.Close();
				}

				if (requisicao_fila != string.Empty)
				{
					comando = bancoDeDados.CriarComando(@"
				    insert into tab_scheduler_fila (id, tipo, requisitante, requisicao, empreendimento, data_criacao, data_conclusao, resultado, sucesso) 
                    values (seq_tab_scheduler_fila.nextval, 'gerar-car', 0, :requisicao, 0, NULL, NULL, '', '')", UsuarioInterno);

					comando.AdicionarParametroEntrada("requisicao", requisicao_fila, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					SalvarControleArquivoCarSicar(solicitacaoId, eStatusArquivoSICAR.AguardandoEnvio, solicitacaoOrigem, banco);

					bancoDeDados.Commit();
				}
			}
		}

		internal void SalvarControleArquivoCarSicar(int solicitacaoId, eStatusArquivoSICAR statusArquivoSICAR, eCARSolicitacaoOrigem solicitacaoOrigem, BancoDeDados banco = null)
		{
			ControleArquivoSICAR controleArquivoSICAR = new ControleArquivoSICAR();
			controleArquivoSICAR.SolicitacaoCarId = solicitacaoId;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioInterno))
			{
				bancoDeDados.IniciarTransacao();

				#region Coleta de dados
				Comando comando = bancoDeDados.CriarComando(@"select tcs.id solic_id, tcs.tid solic_tid, te.id emp_id, te.tid emp_tid, tcrls.id controle_id
                    from tab_car_solicitacao tcs, tab_empreendimento te, (select tcsicar.id, tcsicar.solicitacao_car from tab_controle_sicar tcsicar 
                    where tcsicar.solicitacao_car_esquema = :esquema) tcrls where tcs.empreendimento = te.id and tcs.id = tcrls.solicitacao_car(+)
                    and tcs.id = :idSolicitacao", UsuarioInterno);

				comando.AdicionarParametroEntrada("esquema", (int)solicitacaoOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("idSolicitacao", controleArquivoSICAR.SolicitacaoCarId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						controleArquivoSICAR.SolicitacaoCarTid = reader.GetValue<String>("solic_tid");
						controleArquivoSICAR.EmpreendimentoId = reader.GetValue<Int32>("emp_id");
						controleArquivoSICAR.EmpreendimentoTid = reader.GetValue<String>("emp_tid");
						controleArquivoSICAR.Id = Convert.ToInt32(reader.GetValue<String>("controle_id"));
					}
					reader.Close();
				}

				#endregion

				if (controleArquivoSICAR.Id == 0)
				{
					#region Criar controle arquivo SICAR
					comando = bancoDeDados.CriarComando(@"
				    insert into tab_controle_sicar (id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio, solicitacao_car_esquema)
                    values
                    (seq_tab_controle_sicar.nextval, :tid, :empreendimento, :empreendimento_tid, :solicitacao_car, :solicitacao_car_tid, :situacao_envio, :solicitacao_car_esquema)
                     returning id into :id", UsuarioInterno);

					comando.AdicionarParametroEntrada("empreendimento", controleArquivoSICAR.EmpreendimentoId, DbType.Int32);
					comando.AdicionarParametroEntrada("empreendimento_tid", controleArquivoSICAR.EmpreendimentoTid, DbType.String);
					comando.AdicionarParametroEntrada("solicitacao_car", controleArquivoSICAR.SolicitacaoCarId, DbType.Int32);
					comando.AdicionarParametroEntrada("solicitacao_car_tid", controleArquivoSICAR.SolicitacaoCarTid, DbType.String);
					comando.AdicionarParametroEntrada("situacao_envio", (int)statusArquivoSICAR, DbType.Int32);
					comando.AdicionarParametroEntrada("solicitacao_car_esquema", (int)solicitacaoOrigem, DbType.Int32);

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroSaida("id", DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					controleArquivoSICAR.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

					#endregion
				}
				else
				{
					#region Editar controle arquivo SICAR

					comando = bancoDeDados.CriarComando(@"
				    update tab_controle_sicar r set r.empreendimento_tid = :empreendimento_tid, r.solicitacao_car_tid = :solicitacao_car_tid, r.situacao_envio = :situacao_envio, 
                    r.tid = :tid where r.id = :id", UsuarioInterno);

					comando.AdicionarParametroEntrada("empreendimento_tid", controleArquivoSICAR.EmpreendimentoTid, DbType.String);
					comando.AdicionarParametroEntrada("solicitacao_car_tid", controleArquivoSICAR.SolicitacaoCarTid, DbType.String);
					comando.AdicionarParametroEntrada("situacao_envio", (int)statusArquivoSICAR, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("id", controleArquivoSICAR.Id, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);

					#endregion
				}

				GerarHistoricoControleArquivoCarSicar(controleArquivoSICAR.Id, banco);

				bancoDeDados.Commit();
			}
		}

		internal void GerarHistoricoControleArquivoCarSicar(int controleArquivoId, BancoDeDados banco = null)
		{
			if (controleArquivoId > 0)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioInterno))
				{
					#region Histórico do controle de arquivo SICAR

					bancoDeDados.IniciarTransacao();

					Comando comando = bancoDeDados.CriarComando(@"begin" +
					"  for j in (select tcs.id, tcs.tid, tcs.empreendimento, tcs.empreendimento_tid, tcs.solicitacao_car, tcs.solicitacao_car_tid," +
					"                   tcs.situacao_envio, tcs.chave_protocolo, tcs.data_gerado, tcs.data_envio, tcs.arquivo, tcs.pendencias," +
					"                   tcs.codigo_imovel, tcs.url_recibo, tcs.status_sicar, tcs.condicao, tcs.solicitacao_car_esquema" +
					"            from tab_controle_sicar tcs" +
					"            where tcs.id = :id) loop  " +
					"     INSERT INTO HST_CONTROLE_SICAR" +
					"       (id, controle_sicar_id, tid, empreendimento, empreendimento_tid, solicitacao_car, solicitacao_car_tid, situacao_envio," +
					"        chave_protocolo, data_gerado, data_envio, arquivo, pendencias, codigo_imovel, url_recibo, status_sicar, condicao," +
					"        solicitacao_car_esquema, data_execucao)" +
					"     values" +
					"       (SEQ_HST_CONTROLE_SICAR.nextval, j.id, j.tid, j.empreendimento, j.empreendimento_tid, j.solicitacao_car, j.solicitacao_car_tid," +
					"        j.situacao_envio, j.chave_protocolo, j.data_gerado, j.data_envio, j.arquivo, j.pendencias, j.codigo_imovel, j.url_recibo," +
					"        j.status_sicar, j.condicao, j.solicitacao_car_esquema, CURRENT_TIMESTAMP);" +
					"  end loop;" +
					"end;", UsuarioInterno);

					comando.AdicionarParametroEntrada("id", controleArquivoId, DbType.Int32);

					bancoDeDados.ExecutarNonQuery(comando);
					bancoDeDados.Commit();

					#endregion
				}
			}
		}

		internal string ObterUrlGeracaoRecibo(int solicitacaoId, int schemaSolicitacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioInterno))
			{
				Comando comando = bancoDeDados.CriarComando(@"select tcs.url_recibo from tab_controle_sicar tcs where tcs.solicitacao_car = :solicitacaoId and tcs.solicitacao_car_esquema = :schemaSolicitacao", UsuarioInterno);

				comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("schemaSolicitacao", schemaSolicitacao, DbType.Int32);

				return bancoDeDados.ExecutarScalar<String>(comando);
			}
		}

        internal string ObterUrlGeracaoDemonstrativo(int solicitacaoId, int schemaSolicitacao)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioInterno))
            {
                Comando comando = bancoDeDados.CriarComando(@"select tcs.codigo_imovel from tab_controle_sicar tcs where tcs.solicitacao_car = :solicitacaoId and tcs.solicitacao_car_esquema = :schemaSolicitacao", UsuarioInterno);

                comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
                comando.AdicionarParametroEntrada("schemaSolicitacao", schemaSolicitacao, DbType.Int32);

                return bancoDeDados.ExecutarScalar<String>(comando);
            }
        }

		internal bool ExisteCredenciado(int solicitacaoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(1) from {0}tab_car_solicitacao where id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", solicitacaoId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal int ObterIdAquivoSICAR(int solicitacaoId, int schemaSolicitacao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioInterno))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl(to_number(tcs.arquivo), 0) from tab_controle_sicar tcs where tcs.solicitacao_car = :solicitacaoId and tcs.solicitacao_car_esquema = :schemaSolicitacao", UsuarioInterno);

				comando.AdicionarParametroEntrada("solicitacaoId", solicitacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("schemaSolicitacao", schemaSolicitacao, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando);
			}

		}
	}
}