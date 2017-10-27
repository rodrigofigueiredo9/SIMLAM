using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Configuracao.Interno.Extensoes;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFO.Data
{
	public class EmissaoCFODa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		public string EsquemaCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public EmissaoCFODa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações DML's

		internal void Salvar(EmissaoCFO CFO, BancoDeDados banco = null)
		{
			if (CFO == null)
			{
				throw new Exception("CFO é nulo.");
			}

			if (CFO.Id == 0)
			{
				Criar(CFO, banco);
			}
			else
			{
				Editar(CFO, banco);
			}
		}

		private void Criar(EmissaoCFO CFO, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				insert into tab_cfo (id, tid, tipo_numero, numero, data_emissao, situacao, produtor, empreendimento, possui_laudo_laboratorial, nome_laboratorio, numero_laudo_resultado_analise, 
				estado, municipio, produto_especificacao, possui_trat_fito_fins_quaren, partida_lacrada_origem, numero_lacre, numero_porao, numero_container, validade_certificado, 
				informacoes_complementares, informacoes_complement_html, estado_emissao, municipio_emissao, credenciado, serie) 
				values (seq_tab_cfo.nextval, :tid, :tipo_numero, :numero, :data_emissao, :situacao, :produtor, :empreendimento, :possui_laudo_laboratorial, :nome_laboratorio, :numero_laudo_resultado_analise, 
				:estado, :municipio, :produto_especificacao, :possui_trat_fito_fins_quaren, :partida_lacrada_origem, :numero_lacre, :numero_porao, :numero_container, :validade_certificado, 
				:informacoes_complementares, :informacoes_complement_html, :estado_emissao, :municipio_emissao, :credenciado, :serie) 
				returning id into :id");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("tipo_numero", CFO.TipoNumero, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", CFO.Numero, DbType.Int64);
				comando.AdicionarParametroEntrada("data_emissao", CFO.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", CFO.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("produtor", CFO.ProdutorId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", CFO.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_laudo_laboratorial", CFO.PossuiLaudoLaboratorial, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_laboratorio", CFO.NomeLaboratorio, DbType.String);
				comando.AdicionarParametroEntrada("numero_laudo_resultado_analise", CFO.NumeroLaudoResultadoAnalise, DbType.String);
				comando.AdicionarParametroEntrada("estado", CFO.EstadoId > 0 ? CFO.EstadoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", CFO.MunicipioId > 0 ? CFO.MunicipioId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("produto_especificacao", CFO.ProdutoEspecificacao, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_trat_fito_fins_quaren", CFO.PossuiTratamentoFinsQuarentenario, DbType.Int32);
				comando.AdicionarParametroEntrada("partida_lacrada_origem", CFO.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", CFO.NumeroLacre, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", CFO.NumeroPorao, DbType.String);
				comando.AdicionarParametroEntrada("numero_container", CFO.NumeroContainer, DbType.String);
				comando.AdicionarParametroEntrada("validade_certificado", CFO.ValidadeCertificado, DbType.Int32);
				comando.AdicionarParametroEntClob("informacoes_complementares", CFO.DeclaracaoAdicional);
				comando.AdicionarParametroEntClob("informacoes_complement_html", CFO.DeclaracaoAdicionalHtml);
				comando.AdicionarParametroEntrada("estado_emissao", CFO.EstadoEmissaoId > 0 ? CFO.EstadoEmissaoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio_emissao", CFO.MunicipioEmissaoId > 0 ? CFO.MunicipioEmissaoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
                comando.AdicionarParametroEntrada("serie", CFO.Serie, DbType.String);

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				CFO.Id = comando.ObterValorParametro<int>("id");

				#region Produtos

				comando = bancoDeDados.CriarComando(@"
				insert into tab_cfo_produto (id, tid, cfo, unidade_producao, quantidade, inicio_colheita, fim_colheita, exibe_kilos) 
				values (seq_tab_cfo_produto.nextval, :tid, :cfo, :unidade_producao, :quantidade, :inicio_colheita, :fim_colheita, :exibe_kilos)");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("unidade_producao", DbType.Int32);
				comando.AdicionarParametroEntrada("quantidade", DbType.Decimal);
				comando.AdicionarParametroEntrada("inicio_colheita", DbType.DateTime);
				comando.AdicionarParametroEntrada("fim_colheita", DbType.DateTime);
                comando.AdicionarParametroEntrada("exibe_kilos", DbType.String,1);

				CFO.Produtos.ForEach(produto =>
				{
					comando.SetarValorParametro("unidade_producao", produto.UnidadeProducao);
					comando.SetarValorParametro("quantidade", produto.Quantidade);
					comando.SetarValorParametro("inicio_colheita", produto.DataInicioColheita.Data);
					comando.SetarValorParametro("fim_colheita", produto.DataFimColheita.Data);
                    comando.SetarValorParametro("exibe_kilos", produto.ExibeQtdKg ? "1" : "0");
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Pragas

				comando = bancoDeDados.CriarComando(@"insert into tab_cfo_praga (id, tid, cfo, praga) values (seq_tab_cfo_praga.nextval, :tid, :cfo, :praga)");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("praga", DbType.Int32);

				CFO.Pragas.ForEach(praga =>
				{
					comando.SetarValorParametro("praga", praga.Id);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Tratamentos Fitossanitarios

				comando = bancoDeDados.CriarComando(@"
				insert into tab_cfo_trata_fitossa (id, tid, cfo, produto_comercial, ingrediente_ativo, dose, praga_produto, modo_aplicacao)
				values (seq_tab_cfo_trata_fitossa.nextval, :tid, :cfo, :produto_comercial, :ingrediente_ativo, :dose, :praga_produto, :modo_aplicacao)");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("produto_comercial", DbType.String);
				comando.AdicionarParametroEntrada("ingrediente_ativo", DbType.String);
				comando.AdicionarParametroEntrada("dose", DbType.Decimal);
				comando.AdicionarParametroEntrada("praga_produto", DbType.String);
				comando.AdicionarParametroEntrada("modo_aplicacao", DbType.String);

				CFO.TratamentosFitossanitarios.ForEach(tratamento =>
				{
					comando.SetarValorParametro("produto_comercial", tratamento.ProdutoComercial);
					comando.SetarValorParametro("ingrediente_ativo", tratamento.IngredienteAtivo);
					comando.SetarValorParametro("dose", tratamento.Dose);
					comando.SetarValorParametro("praga_produto", tratamento.PragaProduto);
					comando.SetarValorParametro("modo_aplicacao", tratamento.ModoAplicacao);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				Historico.Gerar(CFO.Id, eHistoricoArtefato.emissaocfo, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		private void Editar(EmissaoCFO CFO, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				update tab_cfo set tid = :tid, numero = :numero, data_emissao = :data_emissao, situacao = :situacao, produtor = :produtor, empreendimento = :empreendimento, 
				possui_laudo_laboratorial = :possui_laudo_laboratorial, nome_laboratorio = :nome_laboratorio, numero_laudo_resultado_analise = :numero_laudo_resultado_analise, 
				estado = :estado, municipio = :municipio, produto_especificacao = :produto_especificacao, possui_trat_fito_fins_quaren = :possui_trat_fito_fins_quaren, 
				partida_lacrada_origem = :partida_lacrada_origem, numero_lacre = :numero_lacre, numero_porao = :numero_porao, numero_container = :numero_container, 
				validade_certificado = :validade_certificado, informacoes_complementares = :informacoes_complementares, informacoes_complement_html = :informacoes_complement_html, estado_emissao = :estado_emissao, municipio_emissao = :municipio_emissao, serie = :serie 
				where id = :id");

				comando.AdicionarParametroEntrada("id", CFO.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("numero", CFO.Numero, DbType.Int64);
				comando.AdicionarParametroEntrada("data_emissao", CFO.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", CFO.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("produtor", CFO.ProdutorId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", CFO.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_laudo_laboratorial", CFO.PossuiLaudoLaboratorial, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_laboratorio", CFO.NomeLaboratorio, DbType.String);
				comando.AdicionarParametroEntrada("numero_laudo_resultado_analise", CFO.NumeroLaudoResultadoAnalise, DbType.String);
				comando.AdicionarParametroEntrada("estado", CFO.EstadoId > 0 ? CFO.EstadoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", CFO.MunicipioId > 0 ? CFO.MunicipioId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("produto_especificacao", CFO.ProdutoEspecificacao, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_trat_fito_fins_quaren", CFO.PossuiTratamentoFinsQuarentenario, DbType.Int32);
				comando.AdicionarParametroEntrada("partida_lacrada_origem", CFO.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", CFO.NumeroLacre, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", CFO.NumeroPorao, DbType.String);
				comando.AdicionarParametroEntrada("numero_container", CFO.NumeroContainer, DbType.String);
				comando.AdicionarParametroEntrada("validade_certificado", CFO.ValidadeCertificado, DbType.Int32);
				comando.AdicionarParametroEntClob("informacoes_complementares", CFO.DeclaracaoAdicional);
				comando.AdicionarParametroEntClob("informacoes_complement_html", CFO.DeclaracaoAdicionalHtml);
				comando.AdicionarParametroEntrada("estado_emissao", CFO.EstadoEmissaoId > 0 ? CFO.EstadoEmissaoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio_emissao", CFO.MunicipioEmissaoId > 0 ? CFO.MunicipioEmissaoId : (object)DBNull.Value, DbType.Int32);
                comando.AdicionarParametroEntrada("serie", CFO.Serie, DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);

				#region Limpar Dados

				comando = bancoDeDados.CriarComando(@"delete from tab_cfo_produto ", EsquemaCredenciado);
				comando.DbCommand.CommandText += String.Format("where cfo = :cfo {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, CFO.Produtos.Select(p => p.Id).ToList()));
				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from tab_cfo_praga ", EsquemaCredenciado);
				comando.DbCommand.CommandText += String.Format("where cfo = :cfo {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, CFO.Pragas.Select(p => p.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from tab_cfo_trata_fitossa ", EsquemaCredenciado);
				comando.DbCommand.CommandText += String.Format("where cfo = :cfo {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, CFO.TratamentosFitossanitarios.Select(p => p.Id).ToList()));
				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Limpar Dados

				#region Produtos

				CFO.Produtos.ForEach(produto =>
				{
					if (produto.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_cfo_produto set tid =:tid, unidade_producao = :unidade_producao, 
						quantidade = :quantidade, inicio_colheita = :inicio_colheita, fim_colheita = :fim_colheita, exibe_kilos = :exibe_kilos where id = :id");

						comando.AdicionarParametroEntrada("id", produto.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into tab_cfo_produto (id, tid, cfo, unidade_producao, quantidade, inicio_colheita, fim_colheita, exibe_kilos) 
						values (seq_tab_cfo_produto.nextval, :tid, :cfo, :unidade_producao, :quantidade, :inicio_colheita, :fim_colheita, :exibe_kilos)");

						comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("unidade_producao", produto.UnidadeProducao, DbType.Int32);
					comando.AdicionarParametroEntrada("quantidade", produto.Quantidade, DbType.Decimal);
					comando.AdicionarParametroEntrada("inicio_colheita", produto.DataInicioColheita.Data, DbType.DateTime);
					comando.AdicionarParametroEntrada("fim_colheita", produto.DataFimColheita.Data, DbType.DateTime);
                    comando.AdicionarParametroEntrada("exibe_kilos", produto.ExibeQtdKg ? "1" : "0" , DbType.String);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Pragas

				CFO.Pragas.ForEach(praga =>
				{
					if (praga.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_cfo_praga set tid = :tid where id = :id");

						comando.AdicionarParametroEntrada("id", praga.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_cfo_praga (id, tid, cfo, praga) values (seq_tab_cfo_praga.nextval, :tid, :cfo, :praga)");

						comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("praga", praga.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Tratamentos Fitossanitarios

				CFO.TratamentosFitossanitarios.ForEach(tratamento =>
				{
					if (tratamento.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update tab_cfo_trata_fitossa set tid =:tid, produto_comercial =:produto_comercial, ingrediente_ativo = :ingrediente_ativo, 
						dose = :dose, praga_produto = :praga_produto, modo_aplicacao = :modo_aplicacao where id = :id");

						comando.AdicionarParametroEntrada("id", tratamento.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into tab_cfo_trata_fitossa (id, tid, cfo, produto_comercial, ingrediente_ativo, dose, praga_produto, modo_aplicacao)
						values (seq_tab_cfo_trata_fitossa.nextval, :tid, :cfo, :produto_comercial, :ingrediente_ativo, :dose, :praga_produto, :modo_aplicacao)");

						comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("produto_comercial", tratamento.ProdutoComercial, DbType.String);
					comando.AdicionarParametroEntrada("ingrediente_ativo", tratamento.IngredienteAtivo, DbType.String);
					comando.AdicionarParametroEntrada("dose", tratamento.Dose, DbType.Decimal);
					comando.AdicionarParametroEntrada("praga_produto", tratamento.PragaProduto, DbType.String);
					comando.AdicionarParametroEntrada("modo_aplicacao", tratamento.ModoAplicacao, DbType.String);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				Historico.Gerar(CFO.Id, eHistoricoArtefato.emissaocfo, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_cfo c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.emissaocfo, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					delete {0}tab_cfo_trata_fitossa where cfo = :id;
					delete {0}tab_cfo_praga where cfo = :id;
					delete {0}tab_cfo_produto where cfo = :id;
					delete {0}tab_cfo a where a.id = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void Ativar(EmissaoCFO CFO, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_cfo c set c.tid = :tid, c.situacao = :situacao, c.data_ativacao = :data_ativacao where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", CFO.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eDocumentoFitossanitarioSituacao.Valido, DbType.Int32);
				comando.AdicionarParametroEntrada("data_ativacao", CFO.DataAtivacao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(CFO.Id, eHistoricoArtefato.emissaocfo, eHistoricoAcao.ativar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Cancelar(EmissaoCFO CFO, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_cfo c set c.tid = :tid, c.situacao = :situacao where c.numero = :numero", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", CFO.Numero, DbType.Int64);
				comando.AdicionarParametroEntrada("situacao", (int)eDocumentoFitossanitarioSituacao.Cancelado, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(CFO.Id, eHistoricoArtefato.emissaocfo, eHistoricoAcao.cancelar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		internal EmissaoCFO ObterPorNumero(long numero, string serieNumero = "",  bool simplificado = false, bool credenciado = true, BancoDeDados banco = null)
		{
			EmissaoCFO retorno = new EmissaoCFO();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{

                string sqlCfo = @"select id from {0}tab_cfo where numero = :numero";

                if (!string.IsNullOrEmpty(serieNumero))
                    sqlCfo += " and serie = :serie";

				Comando comando = bancoDeDados.CriarComando(sqlCfo, EsquemaCredenciado);
				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);

                if (!string.IsNullOrEmpty(serieNumero))
                    comando.AdicionarParametroEntrada("serie", serieNumero, DbType.String);

				if (credenciado)
				{
					comando.DbCommand.CommandText += " and credenciado = :credenciado";
					comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int64);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = new EmissaoCFO();
						retorno.Id = reader.GetValue<int>("id");
					}

					reader.Close();
				}

				if (retorno != null)
				{
					return Obter(retorno.Id, simplificado, bancoDeDados);
				}
			}

			return retorno;
		}

		internal EmissaoCFO Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				EmissaoCFO CFO = new EmissaoCFO();

				#region Dados

				Comando comando = bancoDeDados.CriarComando(@"select c.tid, c.tipo_numero, c.numero, c.data_ativacao, c.data_emissao, c.situacao, c.produtor, c.empreendimento, c.possui_laudo_laboratorial, 
				c.nome_laboratorio, c.numero_laudo_resultado_analise, c.estado, c.municipio, c.produto_especificacao, c.possui_trat_fito_fins_quaren, c.partida_lacrada_origem, c.numero_lacre, 
				c.numero_porao, c.numero_container, c.validade_certificado, c.informacoes_complementares, c.estado_emissao, c.municipio_emissao, c.credenciado , c.serie
				from tab_cfo c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						CFO.Id = id;
						CFO.Tid = reader.GetValue<string>("tid");
						CFO.TipoNumero = reader.GetValue<int>("tipo_numero");
						CFO.Numero = reader.GetValue<string>("numero") + (string.IsNullOrEmpty(reader.GetValue<string>("serie")) ? "" : "/" + reader.GetValue<string>("serie") );
						CFO.DataEmissao.Data = reader.GetValue<DateTime>("data_emissao");
                        CFO.DataAtivacao.Data = reader.GetValue<DateTime>("data_ativacao");
						CFO.SituacaoId = reader.GetValue<int>("situacao");
						CFO.ProdutorId = reader.GetValue<int>("produtor");
						CFO.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						CFO.PossuiLaudoLaboratorial = reader.GetValue<int>("possui_laudo_laboratorial") > 0;
						CFO.NomeLaboratorio = reader.GetValue<string>("nome_laboratorio");
						CFO.NumeroLaudoResultadoAnalise = reader.GetValue<string>("numero_laudo_resultado_analise");
						CFO.EstadoId = reader.GetValue<int>("estado");
						CFO.MunicipioId = reader.GetValue<int>("municipio");
						CFO.ProdutoEspecificacao = reader.GetValue<int>("produto_especificacao");
						CFO.PossuiTratamentoFinsQuarentenario = reader.GetValue<int>("possui_trat_fito_fins_quaren") > 0;
						CFO.PartidaLacradaOrigem = reader.GetValue<int>("partida_lacrada_origem") > 0;
						CFO.NumeroLacre = reader.GetValue<string>("numero_lacre");
						CFO.NumeroPorao = reader.GetValue<string>("numero_porao");
						CFO.NumeroContainer = reader.GetValue<string>("numero_container");
						CFO.ValidadeCertificado = reader.GetValue<int>("validade_certificado");
						CFO.DeclaracaoAdicional = reader.GetValue<string>("informacoes_complementares");
						CFO.DeclaracaoAdicionalHtml = reader.GetValue<string>("informacoes_complementares");
						CFO.EstadoEmissaoId = reader.GetValue<int>("estado_emissao");
						CFO.MunicipioEmissaoId = reader.GetValue<int>("municipio_emissao");
						CFO.CredenciadoId = reader.GetValue<int>("credenciado");
                     
					}

					reader.Close();
				}

				#endregion

				if (CFO.Id <= 0 || simplificado)
				{
					return CFO;
				}

				#region Produtos

				comando = bancoDeDados.CriarComando(@"
				select cp.id, cp.tid, cp.unidade_producao, i.codigo_up, c.id cultura_id,c.texto cultura, cc.id cultivar_id, cc.cultivar, lu.id as unidade_medida, lu.texto unidade_medida_texto, cp.quantidade, cp.inicio_colheita, cp.fim_colheita, cp.exibe_kilos 
				from tab_cfo_produto cp, ins_crt_unidade_prod_unidade i, tab_cultura c, tab_cultura_cultivar cc, lov_crt_uni_prod_uni_medida lu 
				where i.id = cp.unidade_producao and c.id = i.cultura and cc.id = i.cultivar and i.estimativa_unid_medida = lu.id and cp.cfo = :cfo", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						CFO.Produtos.Add(new IdentificacaoProduto()
						{
							Id = reader.GetValue<int>("id"),
							UnidadeProducao = reader.GetValue<int>("unidade_producao"),
							CodigoUP = reader.GetValue<string>("codigo_up"),
							CulturaId = reader.GetValue<int>("cultura_id"),
							CulturaTexto = reader.GetValue<string>("cultura"),
							CultivarId = reader.GetValue<int>("cultivar_id"),
							CultivarTexto = reader.GetValue<string>("cultivar"),
							UnidadeMedidaId = reader.GetValue<int>("unidade_medida"),
							UnidadeMedida = reader.GetValue<string>("unidade_medida_texto"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
                            ExibeQtdKg = reader.GetValue<string>("exibe_kilos") == "1" ? true : false ,
							DataInicioColheita = new DateTecno() { Data = reader.GetValue<DateTime>("inicio_colheita") },
							DataFimColheita = new DateTecno() { Data = reader.GetValue<DateTime>("fim_colheita") }
						});
					}

					reader.Close();
				}

				#endregion

				#region Pragas

				comando = bancoDeDados.CriarComando(@"select cp.id, cp.praga, p.nome_cientifico, p.nome_comum from tab_cfo_praga cp, tab_praga p where p.id = cp.praga and cp.cfo = :cfo", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						CFO.Pragas.Add(new Praga()
						{
							Id = reader.GetValue<int>("praga"),
							IdRelacionamento = reader.GetValue<int>("id"),
							NomeCientifico = reader.GetValue<string>("nome_cientifico"),
							NomeComum = reader.GetValue<string>("nome_comum")
						});
					}

					reader.Close();
				}

				#endregion

				#region Tratamentos Fitossanitarios

				comando = bancoDeDados.CriarComando(@"select c.id, c.produto_comercial, c.ingrediente_ativo, c.dose, c.praga_produto, c.modo_aplicacao from tab_cfo_trata_fitossa c where c.cfo = :cfo", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfo", CFO.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						CFO.TratamentosFitossanitarios.Add(new TratamentoFitossanitario()
						{
							Id = reader.GetValue<int>("id"),
							ProdutoComercial = reader.GetValue<string>("produto_comercial"),
							IngredienteAtivo = reader.GetValue<string>("ingrediente_ativo"),
							Dose = reader.GetValue<decimal>("dose"),
							PragaProduto = reader.GetValue<string>("praga_produto"),
							ModoAplicacao = reader.GetValue<string>("modo_aplicacao")
						});
					}

					reader.Close();
				}

				#endregion

				return CFO;
			}
		}

        public List<IdentificacaoProduto> ObterHistorico(int id, string tid, BancoDeDados banco = null)
        {
            EmissaoCFO entidade = new EmissaoCFO();
            string credenciadoTID = string.Empty;
            int hst_id = 0;

            #region Credenciado

            string EsquemaBancoCredenciado = "idafcredenciado";

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBancoCredenciado))
            {
                #region Dados

                Comando comando = bancoDeDados.CriarComando(@"
				select t.id
				from hst_cfo t, lov_estado le, lov_estado lee
				where le.id(+) = t.estado_id
				and lee.id(+) = t.estado_emissao_id
				and t.cfo_id = :id
				and t.tid = :tid", EsquemaBanco);

                comando.AdicionarParametroEntrada("id", id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, tid);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        hst_id = reader.GetValue<int>("id");
                    }

                    reader.Close();
                }

                #endregion Dados

                #region Produtos

                comando = bancoDeDados.CriarComando(@"
				select cp.id,
					cp.tid,
					cp.unidade_producao_id,
					cp.unidade_producao_tid,
					cp.quantidade,
					cp.inicio_colheita,
					cp.fim_colheita,
                    cp.exibe_kilos
				from hst_cfo_produto cp
				where cp.id_hst = :hst_id", EsquemaBanco);

                comando.AdicionarParametroEntrada("hst_id", hst_id, DbType.Int32);

                using (IDataReader dr = bancoDeDados.ExecutarReader(comando))
                {
                    while (dr.Read())
                    {
                        entidade.Produtos.Add(new IdentificacaoProduto()
                        {
                            Id = dr.GetValue<int>("id"),
                            UnidadeProducaoID = dr.GetValue<int>("unidade_producao_id"),
                            UnidadeProducaoTID = dr.GetValue<string>("unidade_producao_tid"),
                            Quantidade = dr.GetValue<decimal>("quantidade"),
                            DataInicioColheita = new DateTecno() { Data = dr.GetValue<DateTime>("inicio_colheita") },
                            ExibeQtdKg = dr.GetValue<string>("exibe_kilos") == "1" ? true : false,
                            DataFimColheita = new DateTecno() { Data = dr.GetValue<DateTime>("fim_colheita") }
                        });
                    }

                    dr.Close();
                }

                #endregion
            }

            #endregion Credenciado

            #region Interno

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                #region Produtos

                entidade.Produtos.ForEach(produto =>
                {
                    Comando comando = bancoDeDados.CriarComando(@"
					select u.codigo_up,
						c.texto                        cultura,
						cc.cultivar_nome               cultivar,
						u.estimativa_unid_medida_texto unidade_medida
					from hst_crt_unidade_prod_unidade u,
						hst_cultura                   c,
						hst_cultura_cultivar          cc
					where c.cultura_id = u.cultura_id
					and c.tid = u.cultura_tid
					and cc.cultivar_id(+) = u.cultivar_id
					and cc.tid(+) = u.cultivar_tid
					and u.unidade_producao_unidade_id = :id
					and u.tid = :tid", EsquemaBanco);

                    comando.AdicionarParametroEntrada("id", produto.UnidadeProducaoID, DbType.Int32);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, produto.UnidadeProducaoTID);

                    using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                    {
                        if (reader.Read())
                        {
                            produto.CodigoUP = reader.GetValue<string>("codigo_up");
                            produto.CulturaTexto = reader.GetValue<string>("cultura");
                            produto.CultivarTexto = reader.GetValue<string>("cultivar");
                            produto.UnidadeMedida = reader.GetValue<string>("unidade_medida");
                        }

                        reader.Close();
                    }
                });

                #endregion Produtos
            }

            #endregion Interno

            return entidade.Produtos;
        }

		public Resultados<EmissaoCFO> Filtrar(Filtro<EmissaoCFO> filtros)
		{
			Resultados<EmissaoCFO> retorno = new Resultados<EmissaoCFO>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("d.credenciado", "credenciado", filtros.Dados.CredenciadoId);

				comandtxt += comando.FiltroAnd("d.numero", "numero", filtros.Dados.Numero);

				comandtxt += comando.FiltroAndLike("d.denominador", "denominador", filtros.Dados.EmpreendimentoTexto, true, true);

				comandtxt += comando.FiltroAndLike("d.produtor", "produtor", filtros.Dados.ProdutorTexto, true, true);

				comandtxt += comando.FiltroAndLike("d.cultura_cultivar", "cultura_cultivar", filtros.Dados.CulturaCultivar, true, true);

				comandtxt += comando.FiltroAnd("d.situacao", "situacao", filtros.Dados.SituacaoId);

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "denominador", "cultura_cultivar", "situacao_texto" };

				if (filtros.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtros.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText = String.Format(@"select count(*) from (
				select c.id, c.tid, c.credenciado, c.numero, ie.denominador, nvl(p.nome, p.razao_social) produtor, c.tipo_numero,
				(select stragg(cu.texto||'/'||cc.cultivar) from tab_cfo_produto cp, ins_crt_unidade_prod_unidade iu, tab_cultura cu, tab_cultura_cultivar cc 
				where iu.id = cp.unidade_producao and cu.id = iu.cultura and cc.id = iu.cultivar and cp.cfo = c.id) cultura_cultivar, c.situacao, ls.texto situacao_texto
				from tab_cfo c, ins_empreendimento ie, ins_pessoa p, lov_doc_fitossani_situacao ls 
				where ie.id = c.empreendimento and p.id = c.produtor and ls.id = c.situacao) d where d.id > 0 " + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select * from (
				select c.id, c.tid, c.credenciado, c.numero || case when c.serie is null then '' else '/' || c.serie end as numero , ie.denominador, nvl(p.nome, p.razao_social) produtor, c.tipo_numero,
				(select stragg(distinct cu.texto||'/'||cc.cultivar) from tab_cfo_produto cp, ins_crt_unidade_prod_unidade iu, tab_cultura cu, tab_cultura_cultivar cc 
				where iu.id = cp.unidade_producao and cu.id = iu.cultura and cc.id = iu.cultivar and cp.cfo = c.id) cultura_cultivar, c.situacao, ls.texto situacao_texto
				from tab_cfo c, ins_empreendimento ie, ins_pessoa p, lov_doc_fitossani_situacao ls 
				where ie.id = c.empreendimento and p.id = c.produtor and ls.id = c.situacao) d where d.id > 0 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					EmissaoCFO CFO;

					while (reader.Read())
					{
						CFO = new EmissaoCFO();
						CFO.Id = reader.GetValue<int>("id");
						CFO.Tid = reader.GetValue<string>("tid");
						CFO.Numero = reader.GetValue<string>("numero");
						CFO.EmpreendimentoTexto = reader.GetValue<string>("denominador");
						CFO.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						CFO.SituacaoId = reader.GetValue<int>("situacao");
						CFO.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						CFO.TipoNumero = reader.GetValue<int>("tipo_numero");
						retorno.Itens.Add(CFO);
					}

					reader.Close();

					#endregion
				}
			}

			return retorno;
		}

		internal string ObterNumeroDigital()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select min(numero) from tab_numero_cfo_cfoc t, tab_liberacao_cfo_cfoc l
				where l.id = t.liberacao and t.tipo_documento = 1 and t.tipo_numero = 2
				and not exists (select null from cre_cfo c where c.numero = t.numero)
				and t.situacao = 1 and l.responsavel_tecnico = :credenciado
                and to_char(numero) like '__'|| to_char(sysdate, 'yy') ||'%' ");

				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

                string numeroDigital = bancoDeDados.ExecutarScalar(comando).ToString();


                Comando comandoSerie = bancoDeDados.CriarComando(@"
				select serie from tab_numero_cfo_cfoc where numero = :numero ");

                comandoSerie.AdicionarParametroEntrada("numero", numeroDigital, DbType.Int64);

                string serieDigital = bancoDeDados.ExecutarScalar(comandoSerie).ToString();

                if (!string.IsNullOrEmpty(serieDigital))
                {
                    numeroDigital = numeroDigital + "/" + serieDigital; 
                }

                return numeroDigital;

			}
		}

		internal List<Lista> ObterProdutoresLista(int credenciadoID, BancoDeDados banco = null)
		{
			List<Lista> retorno = new List<Lista>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@" select distinct p.id, nvl(p.nome, p.razao_social) nome from crt_unidade_prod_un_produtor pp, tab_pessoa p where p.id = pp.produtor and 
				unidade_producao_unidade in (select unidade_producao_unidade from crt_unidade_prod_un_resp_tec where responsavel_tecnico = :credenciado)");

				comando.AdicionarParametroEntrada("credenciado", credenciadoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("nome") });
					}

					reader.Close();
				}
			}

			return retorno;
		}

        internal List<Lista> ObterEmpreendimentosListaEtramiteX(int produtorID, BancoDeDados banco = null)
        {
            List<Lista> retorno = new List<Lista>();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"
					select distinct e.id, e.denominador
					  from crt_unidade_producao c, tab_empreendimento e
					 where e.id = c.empreendimento
					   and c.id in
						   (select u.unidade_producao
							  from crt_unidade_producao_unidade u
							 where u.id in (select p.unidade_producao_unidade
											  from crt_unidade_prod_un_produtor p
											 where p.produtor = :produtor))");

                comando.AdicionarParametroEntrada("produtor", produtorID, DbType.Int32);
                
                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("denominador") });
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

		internal List<Lista> ObterEmpreendimentosLista(int produtorID, int credenciadoID, BancoDeDados banco = null)
		{
			List<Lista> retorno = new List<Lista>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					select distinct e.id, e.denominador
					  from crt_unidade_producao c, tab_empreendimento e
					 where e.id = c.empreendimento
					   and c.id in
						   (select u.unidade_producao
							  from crt_unidade_producao_unidade u
							 where u.id in (select p.unidade_producao_unidade
											  from crt_unidade_prod_un_produtor p
											 where p.produtor = :produtor)
							   and u.id in
								   (select r.unidade_producao_unidade
									  from crt_unidade_prod_un_resp_tec r
									 where r.responsavel_tecnico = :responsavel_tecnico))");

				comando.AdicionarParametroEntrada("produtor", produtorID, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel_tecnico", credenciadoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("denominador") });
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal List<Lista> ObterUnidadesProducaoLista(int empreendimentoID, int produtorID, int credenciadoID, BancoDeDados banco = null)
		{
			List<Lista> retorno = new List<Lista>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
                Comando comando = bancoDeDados.CriarComando(@" select u.id, u.codigo_up from crt_unidade_producao c, crt_unidade_producao_unidade u, tab_titulo ti, esp_aber_livro_up_unid uni, esp_abertura_livro_up esp
                                                                where u.unidade_producao = c.id and c.empreendimento = :empreendimento and esp.titulo = ti.id 
                                                                and ti.empreendimento = c.empreendimento and ti.situacao = 3 and uni.especificidade = esp.id 
                                                                and uni.unidade = u.id
                                                                and u.id in (select p.unidade_producao_unidade from crt_unidade_prod_un_produtor p where p.produtor = :produtor)
                                                                and u.id in (select r.unidade_producao_unidade from crt_unidade_prod_un_resp_tec r where r.responsavel_tecnico = :responsavel_tecnico) group by u.id, u.codigo_up ");

               

				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("produtor", produtorID, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel_tecnico", credenciadoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("codigo_up") });
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal Cultivar ObterCulturaUP(int unidadeProducaoId)
		{
			Cultivar cultivar = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select c.id cultura_id, c.texto cultura, cc.id cultivar_id, cc.cultivar, u.id unidade_medida_id, u.texto unidade_medida 
				from crt_unidade_producao_unidade t, tab_cultura c, tab_cultura_cultivar cc , lov_crt_uni_prod_uni_medida u 
				where c.id = t.cultura and cc.id(+) = t.cultivar and u.id = t.estimativa_unid_medida and  t.id = :up");

				comando.AdicionarParametroEntrada("up", unidadeProducaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					cultivar = new Cultivar();

					while (reader.Read())
					{
						cultivar.Id = reader.GetValue<int>("cultivar_id");
						cultivar.Nome = reader.GetValue<string>("cultivar");
						cultivar.CulturaId = reader.GetValue<int>("cultura_id");
						cultivar.CulturaTexto = reader.GetValue<string>("cultura");
						cultivar.UnidadeMedida = reader.GetValue<int>("unidade_medida_id");
						cultivar.UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida");
					}

					reader.Close();
				}

			}

			return cultivar;
		}

		internal List<Lista> ObterPragasLista(List<IdentificacaoProduto> produtos)
		{
			List<Lista> retorno = new List<Lista>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando("");

				int count = 0;
				string aux = string.Empty;
				foreach (var item in produtos.GroupBy(p => new { p.CultivarId, p.UnidadeMedidaId }).Select(g => g.First()))
				{
					++count;
					aux += string.Format("p.id in (select c.praga from tab_cultivar_configuracao c where c.cultivar = :cultivar{0} and c.tipo_producao = :tipo_producao{0}) or ", count);
					comando.AdicionarParametroEntrada("cultivar" + count, item.CultivarId, DbType.Int32);
					comando.AdicionarParametroEntrada("tipo_producao" + count, (int)ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedidaId), DbType.Int32);
				}

				aux = aux.Substring(0, aux.Length - 4);
				comando.DbCommand.CommandText = string.Format(@"select distinct p.* from tab_hab_emi_cfo_cfoc h, tab_hab_emi_cfo_cfoc_praga hp, tab_praga p, 
				tab_praga_cultura pc where pc.praga = p.id and p.id = hp.praga and h.id = hp.habilitar_emi_id and h.responsavel = :credenciado and({0})", aux);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new Lista()
						{
							Id = reader.GetValue<int>("id").ToString(),
							Texto = reader.GetValue<string>("nome_cientifico") + (string.IsNullOrEmpty(reader.GetValue<string>("nome_comum")) ? "" : " - " + reader.GetValue<string>("nome_comum"))
						});
					}

					reader.Close();
				}
			}

			return retorno;
		}

		public ResponsavelTecnico ObterResponsavelUC(int unidade)
		{
			ResponsavelTecnico responsavel = new ResponsavelTecnico();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.responsavel_tecnico, c.numero_hab_cfo_cfoc, c.numero_art, c.art_cargo_funcao, c.data_validade_art, c.tid, 
				nvl(tp.nome, tp.razao_social) nome_razao, nvl(tp.cpf, tp.cnpj) cpf_cnpj, pf.texto profissao, oc.orgao_sigla, pp.registro from {0}crt_unidade_prod_un_resp_tec c, 
				{0}tab_credenciado tc, {1}tab_pessoa tp, {1}tab_pessoa_profissao pp, {0}tab_profissao pf, {0}tab_orgao_classe oc where tc.id = c.responsavel_tecnico 
				and tp.id = tc.pessoa and pp.pessoa(+) = tp.id and pf.id(+) = pp.profissao and oc.id(+) = pp.orgao_classe 
				and c.responsavel_tecnico = :responsavel and c.unidade_producao_unidade = :unidade_producao_unidade", EsquemaBanco, EsquemaCredenciado);

				comando.AdicionarParametroEntrada("responsavel", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("unidade_producao_unidade", unidade, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						responsavel.IdRelacionamento = reader.GetValue<int>("id");
						responsavel.Id = reader.GetValue<int>("responsavel_tecnico");
						responsavel.NomeRazao = reader.GetValue<string>("nome_razao");
						responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
						responsavel.CFONumero = reader.GetValue<string>("numero_hab_cfo_cfoc");
						responsavel.NumeroArt = reader.GetValue<string>("numero_art");
						responsavel.ArtCargoFuncao = reader.GetValue<bool>("art_cargo_funcao");

						responsavel.ProfissaoTexto = reader.GetValue<string>("profissao");
						responsavel.OrgaoClasseSigla = reader.GetValue<string>("orgao_sigla");
						responsavel.NumeroRegistro = reader.GetValue<string>("registro");

						responsavel.DataValidadeART = reader.GetValue<string>("data_validade_art");
						if (!string.IsNullOrEmpty(responsavel.DataValidadeART))
						{
							responsavel.DataValidadeART = Convert.ToDateTime(responsavel.DataValidadeART).ToShortDateString();
						}
					}

					reader.Close();
				}
			}

			return responsavel;
		}

		internal UnidadeProducaoItem ObterUnidadeProducaoItem(int unidadeProducao, int empreendimento)
		{
			UnidadeProducaoItem retorno = new UnidadeProducaoItem();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select u.id, u.estimativa_quant_ano, u.renasem_data_validade 
				from crt_unidade_producao c, crt_unidade_producao_unidade u
				where u.unidade_producao = c.id and c.empreendimento = :empreendimento and u.id = :unidade_producao");

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("unidade_producao", unidadeProducao, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno.Id = reader.GetValue<int>("id");
						retorno.EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano");
						retorno.DataValidadeRenasem = reader.GetValue<string>("renasem_data_validade");

						if (!string.IsNullOrEmpty(retorno.DataValidadeRenasem))
						{
							retorno.DataValidadeRenasem = Convert.ToDateTime(retorno.DataValidadeRenasem).ToShortDateString();
						}
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal decimal ObterQuantidadeProduto(int empreendimento, int cultivar, eUnidadeProducaoTipoProducao tipoProducao, int unidade, int cfo, DateTime dataSaldo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
                //Situação 2 (ativo) e 3 (inativo) são as únicas situações de CFO que devem ser consideradas no cálculo
				Comando comando = bancoDeDados.CriarComando(@"
				select nvl((select sum(case when cp.exibe_kilos = 1 then cp.quantidade / 1000 else cp.quantidade end) 
                            from tab_cfo c,
                                 tab_cfo_produto cp,
                                 ins_crt_unidade_prod_unidade u
				            where cp.cfo = c.id
                                  and u.id = cp.unidade_producao
                                  and u.cultivar = :cultivar
                                  and u.tipo_producao = :tipo_producao
                                  and c.empreendimento = :empreendimento
                                  and cp.unidade_producao = :unidade
                                  and c.id != :cfo
                                  and c.data_ativacao >= :data_saldo_inicio
                                  and c.data_ativacao < :data_saldo_fim
                                  and (c.situacao = 2 or c.situacao = 3)),
                           0)
                from dual");

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", cultivar, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_producao", (int)tipoProducao, DbType.Int32);
				comando.AdicionarParametroEntrada("unidade", unidade, DbType.Int32);
				comando.AdicionarParametroEntrada("cfo", cfo, DbType.Int32);

				comando.AdicionarParametroEntrada("data_saldo_inicio", dataSaldo.ToShortDateString(), DbType.Date);
				comando.AdicionarParametroEntrada("data_saldo_fim", dataSaldo.AddYears(1).ToShortDateString(), DbType.Date);

                var valor = bancoDeDados.ExecutarScalar<decimal>(comando);

				return valor;
			}
		}

		public UnidadeProducao ObterUnidadeProducao(int id, string tid, BancoDeDados banco = null, bool simplificado = false)
		{
			int id_hst = 0;
			UnidadeProducao caracterizacao = new UnidadeProducao();
			//Sempre do interno

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id id_hst, c.tid, c.empreendimento_id, c.possui_cod_propriedade, 
				c.propriedade_codigo, e.codigo empreendimento_codigo, c.local_livro 
				from {0}hst_crt_unidade_producao c, {0}hst_empreendimento e where  e.empreendimento_id = c.empreendimento_id 
				and e.tid = c.empreendimento_tid and c.unidade_producao_id = :id and c.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						id_hst = reader.GetValue<int>("id_hst");

						caracterizacao.Id = id;
						caracterizacao.CredenciadoID = id;
						caracterizacao.Tid = reader.GetValue<string>("tid");

						caracterizacao.CodigoPropriedade = reader.GetValue<Int64>("propriedade_codigo");
						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento_id");
						caracterizacao.Empreendimento.Codigo = reader.GetValue<int?>("empreendimento_codigo");
						caracterizacao.LocalLivroDisponivel = reader.GetValue<string>("local_livro");
						caracterizacao.PossuiCodigoPropriedade = reader.GetValue<bool>("possui_cod_propriedade");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Unidades de produção

				comando = bancoDeDados.CriarComando(@"select c.id, c.tid, c.unidade_producao_id, c.unidade_producao_unidade_id, c.possui_cod_up, c.codigo_up, c.tipo_producao_id, c.renasem, c.renasem_data_validade, 
				c.area, c.ano_abertura, c.cultura_id, tc.texto cultura_texto, c.cultivar_id, cc.cultivar cultivar_nome, c.data_plantio_ano_producao, c.estimativa_quant_ano, 
				c.estimativa_unid_medida_id  from {0}hst_crt_unidade_prod_unidade c, {0}tab_cultura_cultivar cc, {0}hst_cultura tc where cc.id(+) = c.cultivar_id 
				and  tc.cultura_id = c.cultura_id and tc.tid = c.cultura_tid and c.id_hst = :id_hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						id_hst = reader.GetValue<int>("id");
						UnidadeProducaoItem item = new UnidadeProducaoItem();

						item.Id = reader.GetValue<int>("unidade_producao_unidade_id");
						item.Tid = reader.GetValue<string>("tid");
						item.PossuiCodigoUP = reader.GetValue<bool>("possui_cod_up");
						item.CodigoUP = reader.GetValue<long>("codigo_up");
						item.TipoProducao = reader.GetValue<int>("tipo_producao_id");
						item.RenasemNumero = reader.GetValue<string>("renasem");
						item.DataValidadeRenasem = string.IsNullOrEmpty(reader.GetValue<string>("renasem_data_validade")) ? "" : Convert.ToDateTime(reader.GetValue<string>("renasem_data_validade")).ToShortDateString();
						item.AreaHA = reader.GetValue<decimal>("area");
						item.DataPlantioAnoProducao = reader.GetValue<string>("data_plantio_ano_producao");
						item.EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano");
						item.CultivarId = reader.GetValue<int>("cultivar_id");
						item.CultivarTexto = reader.GetValue<string>("cultivar_nome");
						item.CulturaId = reader.GetValue<int>("cultura_id");
						item.CulturaTexto = reader.GetValue<string>("cultura_texto");
						item.AnoAbertura = reader.GetValue<string>("ano_abertura");

						#region Produtores

						comando = bancoDeDados.CriarComando(@"
						select h.id, h.tid, h.unidade_prod_produtor_id id_rel, h.produtor_id, nvl(hp.nome, hp.razao_social) nome_razao, nvl(hp.cpf, hp.cnpj) cpf_cnpj, hp.tipo 
						from {0}hst_crt_unid_prod_un_produtor h, {0}hst_pessoa hp where h.produtor_id = hp.pessoa_id and h.produtor_tid = hp.tid and h.id_hst = :id_hst", EsquemaBanco);

						comando.AdicionarParametroEntrada("id_hst", id_hst, DbType.Int32);

						using (IDataReader readerAux = bancoDeDados.ExecutarReader(comando))
						{
							while (readerAux.Read())
							{
								item.Produtores.Add(new Responsavel()
								{
									Id = readerAux.GetValue<int>("produtor_id"),
									NomeRazao = readerAux.GetValue<string>("nome_razao"),
									CpfCnpj = readerAux.GetValue<string>("cpf_cnpj"),
									IdRelacionamento = readerAux.GetValue<int>("id_rel"),
									Tipo = readerAux.GetValue<int>("tipo"),
									Tid = readerAux.GetValue<string>("tid")
								});
							}

							readerAux.Close();
						}

						#endregion

						caracterizacao.UnidadesProducao.Add(item);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		#endregion

		#region Validaçoes

		public int NumeroUtilizado(long numero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select nvl((select c.id from {0}tab_cfo c where c.numero = :numero), 0) from dual", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);

				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		internal bool NumeroJaExiste(string numero, int cfoId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_cfo c where c.numero = :numero and c.credenciado = :credenciado", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				if (cfoId > 0)
				{
					comando.DbCommand.CommandText += " and c.id <> :id";
					comando.AdicionarParametroEntrada("id", cfoId, DbType.Int32);
				}

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool NumeroLiberado(string numero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from tab_numero_cfo_cfoc n, tab_liberacao_cfo_cfoc l where 
				l.id = n.liberacao and l.responsavel_tecnico = :credenciado_id and n.numero = :numero and n.tipo_documento = 1 and n.tipo_numero = 1");

				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);
				comando.AdicionarParametroEntrada("credenciado_id", User.FuncionarioId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool NumeroCancelado(string numero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
                string numeroSem = numero;
                string serie = "";
                if (numero.IndexOf("/") >= 0)
                {
                    string[] arNum = numero.Split('/');
                    numeroSem = arNum[0];
                    serie = arNum[1];

                }

                Comando comando;
				
                if (string.IsNullOrEmpty(serie))
                    comando = bancoDeDados.CriarComando(@"select count(*) from tab_numero_cfo_cfoc n where n.tipo_documento = 1 and n.tipo_numero = 1 and n.situacao = 0 and n.numero = :numero");
                else
                {
                    comando = bancoDeDados.CriarComando(@"select count(*) from tab_numero_cfo_cfoc n where n.tipo_documento = 1 and n.tipo_numero = 1 and n.situacao = 0 and n.numero = :numero and serie = :serie");
                    comando.AdicionarParametroEntrada("serie", serie, DbType.String);
                }

                comando.AdicionarParametroEntrada("numero", numeroSem, DbType.Int64);
				return bancoDeDados.ExecutarScalar<int>(comando) <= 0;
			}
		}

		internal bool NumeroDigitalDisponivel()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from tab_numero_cfo_cfoc n, tab_liberacao_cfo_cfoc l 
				where l.id = n.liberacao and n.tipo_documento = 1 and n.tipo_numero = 2 and n.situacao = 1 and n.utilizado = 0 
				and not exists (select null from cre_cfo c where c.numero = n.numero) 
				and l.responsavel_tecnico = :credenciado 
                and to_char(n.numero) like '__'|| to_char(sysdate, 'yy') ||'%' "); 

				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool VerificarProdutorAssociado(int produtorId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select count(*) from (select produtor from crt_unidade_prod_un_produtor where unidade_producao_unidade in 
				(select unidade_producao_unidade from crt_unidade_prod_un_resp_tec where responsavel_tecnico = :credenciado)) a where :produtor = a.produtor");

				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("produtor", produtorId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool VerificarProdutorAssociadoEmpreendimento(int produtorId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from  tab_empreendimento_responsavel r where r.empreendimento in (select empreendimento from 
				crt_unidade_producao where id in (select unidade_producao from crt_unidade_producao_unidade where id in (select unidade_producao_unidade from 
				crt_unidade_prod_un_resp_tec where responsavel_tecnico = :credenciado))) and r.responsavel = :produtor");

				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("produtor", produtorId, DbType.Int32);

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion
	}
}