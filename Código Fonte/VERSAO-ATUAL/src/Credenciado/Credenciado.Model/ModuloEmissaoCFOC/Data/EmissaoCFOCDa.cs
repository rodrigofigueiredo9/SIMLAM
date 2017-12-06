using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloEmissaoCFOC.Data
{
	public class EmissaoCFOCDa
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

		public EmissaoCFOCDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações DML's

		internal void Salvar(EmissaoCFOC entidade, BancoDeDados banco = null)
		{
			if (entidade == null)
			{
				throw new Exception("CFOC é nulo.");
			}

			if (entidade.Id == 0)
			{
				Criar(entidade, banco);
			}
			else
			{
				Editar(entidade, banco);
			}
		}

		private void Criar(EmissaoCFOC entidade, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				insert into tab_cfoc (id, tid, tipo_numero, numero, data_emissao, situacao, empreendimento, possui_laudo_laboratorial, nome_laboratorio, numero_laudo_resultado_analise, 
				estado, municipio, produto_especificacao, possui_trat_fito_fins_quaren, partida_lacrada_origem, numero_lacre, numero_porao, numero_container, validade_certificado, 
				informacoes_complementares, informacoes_complement_html, estado_emissao, municipio_emissao, credenciado, serie) 
				values (seq_tab_cfoc.nextval, :tid, :tipo_numero, :numero, :data_emissao, :situacao, :empreendimento, :possui_laudo_laboratorial, :nome_laboratorio, :numero_laudo_resultado_analise, 
				:estado, :municipio, :produto_especificacao, :possui_trat_fito_fins_quaren, :partida_lacrada_origem, :numero_lacre, :numero_porao, :numero_container, :validade_certificado, 
				:informacoes_complementares, :informacoes_complement_html, :estado_emissao, :municipio_emissao, :credenciado, :serie) 
				returning id into :id");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("tipo_numero", entidade.TipoNumero, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", entidade.Numero, DbType.Int64);
				comando.AdicionarParametroEntrada("data_emissao", entidade.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", entidade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", entidade.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_laudo_laboratorial", entidade.PossuiLaudoLaboratorial, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_laboratorio", entidade.NomeLaboratorio, DbType.String);
				comando.AdicionarParametroEntrada("numero_laudo_resultado_analise", entidade.NumeroLaudoResultadoAnalise, DbType.String);
				comando.AdicionarParametroEntrada("estado", entidade.EstadoId > 0 ? entidade.EstadoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", entidade.MunicipioId > 0 ? entidade.MunicipioId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("produto_especificacao", entidade.ProdutoEspecificacao, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_trat_fito_fins_quaren", entidade.PossuiTratamentoFinsQuarentenario, DbType.Int32);
				comando.AdicionarParametroEntrada("partida_lacrada_origem", entidade.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", entidade.NumeroLacre, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", entidade.NumeroPorao, DbType.String);
				comando.AdicionarParametroEntrada("numero_container", entidade.NumeroContainer, DbType.String);
				comando.AdicionarParametroEntrada("validade_certificado", entidade.ValidadeCertificado, DbType.Int32);
				comando.AdicionarParametroEntClob("informacoes_complementares", entidade.DeclaracaoAdicional);
				comando.AdicionarParametroEntClob("informacoes_complement_html", entidade.DeclaracaoAdicionalHtml);
				comando.AdicionarParametroEntrada("estado_emissao", entidade.EstadoEmissaoId > 0 ? entidade.EstadoEmissaoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio_emissao", entidade.MunicipioEmissaoId > 0 ? entidade.MunicipioEmissaoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
                comando.AdicionarParametroEntrada("serie", entidade.Serie, DbType.String);
                

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				entidade.Id = comando.ObterValorParametro<int>("id");

				#region Produtos

				comando = bancoDeDados.CriarComando(@"
				insert into tab_cfoc_produto (id, tid, cfoc, lote, quantidade, exibe_kilos) values (seq_tab_cfoc_produto.nextval, :tid, :cfoc, :lote, :quantidade, :exibe_kilos)");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("lote", DbType.Int32);
                comando.AdicionarParametroEntrada("quantidade", DbType.Decimal);
                comando.AdicionarParametroEntrada("exibe_kilos", DbType.String);

				entidade.Produtos.ForEach(produto =>
				{
					comando.SetarValorParametro("lote", produto.LoteId);
                    comando.SetarValorParametro("quantidade", produto.Quantidade);
                    comando.SetarValorParametro("exibe_kilos", produto.ExibeQtdKg ? "1" : "0");
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Pragas

				comando = bancoDeDados.CriarComando(@"insert into tab_cfoc_praga (id, tid, cfoc, praga) values (seq_tab_cfoc_praga.nextval, :tid, :cfoc, :praga)");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("praga", DbType.Int32);

				entidade.Pragas.ForEach(praga =>
				{
					comando.SetarValorParametro("praga", praga.Id);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Tratamentos Fitossanitarios

				comando = bancoDeDados.CriarComando(@"
				insert into tab_cfoc_trata_fitossa (id, tid, cfoc, produto_comercial, ingrediente_ativo, dose, praga_produto, modo_aplicacao)
				values (seq_tab_cfoc_trata_fitossa.nextval, :tid, :cfoc, :produto_comercial, :ingrediente_ativo, :dose, :praga_produto, :modo_aplicacao)");

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("produto_comercial", DbType.String);
				comando.AdicionarParametroEntrada("ingrediente_ativo", DbType.String);
				comando.AdicionarParametroEntrada("dose", DbType.Decimal);
				comando.AdicionarParametroEntrada("praga_produto", DbType.String);
				comando.AdicionarParametroEntrada("modo_aplicacao", DbType.String);

				entidade.TratamentosFitossanitarios.ForEach(tratamento =>
				{
					comando.SetarValorParametro("produto_comercial", tratamento.ProdutoComercial);
					comando.SetarValorParametro("ingrediente_ativo", tratamento.IngredienteAtivo);
					comando.SetarValorParametro("dose", tratamento.Dose);
					comando.SetarValorParametro("praga_produto", tratamento.PragaProduto);
					comando.SetarValorParametro("modo_aplicacao", tratamento.ModoAplicacao);
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				Historico.Gerar(entidade.Id, eHistoricoArtefato.emissaocfoc, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		private void Editar(EmissaoCFOC entidade, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				update tab_cfoc set tid = :tid, numero = :numero, data_emissao = :data_emissao, situacao = :situacao, 
				empreendimento = :empreendimento, possui_laudo_laboratorial = :possui_laudo_laboratorial, nome_laboratorio = :nome_laboratorio, 
				numero_laudo_resultado_analise = :numero_laudo_resultado_analise, estado = :estado, municipio = :municipio, produto_especificacao = :produto_especificacao, 
				possui_trat_fito_fins_quaren = :possui_trat_fito_fins_quaren, partida_lacrada_origem = :partida_lacrada_origem, numero_lacre = :numero_lacre, numero_porao = :numero_porao, 
				numero_container = :numero_container, validade_certificado = :validade_certificado, informacoes_complementares = :informacoes_complementares, informacoes_complement_html = :informacoes_complement_html, estado_emissao = :estado_emissao, 
				municipio_emissao = :municipio_emissao, serie = :serie where id = :id");

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("numero", entidade.Numero, DbType.Int64);
				comando.AdicionarParametroEntrada("data_emissao", entidade.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", entidade.SituacaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", entidade.EmpreendimentoId, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_laudo_laboratorial", entidade.PossuiLaudoLaboratorial, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_laboratorio", entidade.NomeLaboratorio, DbType.String);
				comando.AdicionarParametroEntrada("numero_laudo_resultado_analise", entidade.NumeroLaudoResultadoAnalise, DbType.String);
				comando.AdicionarParametroEntrada("estado", entidade.EstadoId > 0 ? entidade.EstadoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", entidade.MunicipioId > 0 ? entidade.MunicipioId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("produto_especificacao", entidade.ProdutoEspecificacao, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_trat_fito_fins_quaren", entidade.PossuiTratamentoFinsQuarentenario, DbType.Int32);
				comando.AdicionarParametroEntrada("partida_lacrada_origem", entidade.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", entidade.NumeroLacre, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", entidade.NumeroPorao, DbType.String);
				comando.AdicionarParametroEntrada("numero_container", entidade.NumeroContainer, DbType.String);
				comando.AdicionarParametroEntrada("validade_certificado", entidade.ValidadeCertificado, DbType.Int32);
				comando.AdicionarParametroEntClob("informacoes_complementares", entidade.DeclaracaoAdicional);
				comando.AdicionarParametroEntClob("informacoes_complement_html", entidade.DeclaracaoAdicionalHtml);				
				comando.AdicionarParametroEntrada("estado_emissao", entidade.EstadoEmissaoId > 0 ? entidade.EstadoEmissaoId : (object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio_emissao", entidade.MunicipioEmissaoId > 0 ? entidade.MunicipioEmissaoId : (object)DBNull.Value, DbType.Int32);
                comando.AdicionarParametroEntClob("serie", entidade.Serie);				

				bancoDeDados.ExecutarNonQuery(comando);

				#region Limpar Dados

				comando = bancoDeDados.CriarComando(@"delete from tab_cfoc_produto ", EsquemaCredenciado);
				comando.DbCommand.CommandText += String.Format("where cfoc = :cfoc {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, entidade.Produtos.Select(p => p.Id).ToList()));
				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from tab_cfoc_praga ", EsquemaCredenciado);
				comando.DbCommand.CommandText += String.Format("where cfoc = :cfoc {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, entidade.Pragas.Select(p => p.IdRelacionamento).ToList()));
				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from tab_cfoc_trata_fitossa ", EsquemaCredenciado);
				comando.DbCommand.CommandText += String.Format("where cfoc = :cfoc {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, entidade.TratamentosFitossanitarios.Select(p => p.Id).ToList()));
				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion Limpar Dados

				#region Produtos

				entidade.Produtos.ForEach(produto =>
				{
					if (produto.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_cfoc_produto set tid =:tid, lote = :lote, quantidade = :quantidade, exibe_kilos = :exibe_kilos where id = :id");

						comando.AdicionarParametroEntrada("id", produto.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into tab_cfoc_produto (id, tid, cfoc, lote, quantidade, exibe_kilos) values (seq_tab_cfoc_produto.nextval, :tid, :cfoc, :lote, :quantidade, :exibe_kilos)");

						comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("lote", produto.LoteId, DbType.Int32);
                    comando.AdicionarParametroEntrada("quantidade", produto.Quantidade, DbType.Decimal);
                    comando.AdicionarParametroEntrada("exibe_kilos", DbType.String, 1, produto.ExibeQtdKg ? "1" : "0");
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Pragas

				entidade.Pragas.ForEach(praga =>
				{
					if (praga.IdRelacionamento > 0)
					{
						comando = bancoDeDados.CriarComando(@"update tab_cfoc_praga set tid = :tid where id = :id");

						comando.AdicionarParametroEntrada("id", praga.IdRelacionamento, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"insert into tab_cfoc_praga (id, tid, cfoc, praga) values (seq_tab_cfoc_praga.nextval, :tid, :cfoc, :praga)");

						comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("praga", praga.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Tratamentos Fitossanitarios

				entidade.TratamentosFitossanitarios.ForEach(tratamento =>
				{
					if (tratamento.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update tab_cfoc_trata_fitossa set tid =:tid, produto_comercial =:produto_comercial, ingrediente_ativo = :ingrediente_ativo, 
						dose = :dose, praga_produto = :praga_produto, modo_aplicacao = :modo_aplicacao where id = :id");

						comando.AdicionarParametroEntrada("id", tratamento.Id, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into tab_cfoc_trata_fitossa (id, tid, cfoc, produto_comercial, ingrediente_ativo, dose, praga_produto, modo_aplicacao)
						values (seq_tab_cfoc_trata_fitossa.nextval, :tid, :cfoc, :produto_comercial, :ingrediente_ativo, :dose, :praga_produto, :modo_aplicacao)");

						comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);
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

				Historico.Gerar(entidade.Id, eHistoricoArtefato.emissaocfoc, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_cfoc c set c.tid = :tid where c.id = :id", EsquemaBanco);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.emissaocfoc, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					delete {0}tab_cfoc_trata_fitossa where cfoc = :id;
					delete {0}tab_cfoc_praga where cfoc = :id;
					delete {0}tab_cfoc_produto where cfoc = :id;
					delete {0}tab_cfoc a where a.id = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void Ativar(EmissaoCFOC entidade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				Comando comando = null;
				bancoDeDados.IniciarTransacao();

				comando = bancoDeDados.CriarComando(@"update {0}tab_cfoc c set c.tid = :tid, c.situacao = :situacao, c.data_ativacao = :data_ativacao where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)eDocumentoFitossanitarioSituacao.Valido, DbType.Int32);
				comando.AdicionarParametroEntrada("data_ativacao", entidade.DataAtivacao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(entidade.Id, eHistoricoArtefato.emissaocfoc, eHistoricoAcao.ativar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Cancelar(EmissaoCFOC entidade, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_cfoc c set c.tid = :tid, c.situacao = :situacao where c.numero = :numero", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", entidade.Numero, DbType.Int64);
				comando.AdicionarParametroEntrada("situacao", (int)eDocumentoFitossanitarioSituacao.Cancelado, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(entidade.Id, eHistoricoArtefato.emissaocfoc, eHistoricoAcao.cancelar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter/Filtrar

		internal EmissaoCFOC ObterPorNumero(long numero, string serieNumero = "", bool simplificado = false, bool credenciado = true, BancoDeDados banco = null)
		{
			EmissaoCFOC retorno = new EmissaoCFOC();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{

                string sqlCfo = @"select id from {0}tab_cfoc where numero = :numero";

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
						retorno = new EmissaoCFOC();
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

		internal EmissaoCFOC Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaCredenciado))
			{
				EmissaoCFOC entidade = new EmissaoCFOC();

				#region Dados

				Comando comando = bancoDeDados.CriarComando(@"select c.tid, c.tipo_numero, c.numero, c.data_ativacao, c.data_emissao, c.situacao, c.empreendimento, c.possui_laudo_laboratorial, 
				c.nome_laboratorio, c.numero_laudo_resultado_analise, c.estado, c.municipio, c.produto_especificacao, c.possui_trat_fito_fins_quaren, c.partida_lacrada_origem, 
				c.numero_lacre, c.numero_porao, c.numero_container, c.validade_certificado, c.informacoes_complementares, c.estado_emissao, c.municipio_emissao, c.credenciado, c.serie 
				from tab_cfoc c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						entidade.Id = id;
						entidade.Tid = reader.GetValue<string>("tid");
						entidade.TipoNumero = reader.GetValue<int>("tipo_numero");
                        entidade.Numero = reader.GetValue<string>("numero") + (string.IsNullOrEmpty(reader.GetValue<string>("serie")) ? "" : "/" + reader.GetValue<string>("serie")); 
						entidade.DataEmissao.Data = reader.GetValue<DateTime>("data_emissao");
                        entidade.DataAtivacao.Data = reader.GetValue<DateTime>("data_ativacao");
						entidade.SituacaoId = reader.GetValue<int>("situacao");
						entidade.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						entidade.PossuiLaudoLaboratorial = reader.GetValue<int>("possui_laudo_laboratorial") > 0;
						entidade.NomeLaboratorio = reader.GetValue<string>("nome_laboratorio");
						entidade.NumeroLaudoResultadoAnalise = reader.GetValue<string>("numero_laudo_resultado_analise");
						entidade.EstadoId = reader.GetValue<int>("estado");
						entidade.MunicipioId = reader.GetValue<int>("municipio");
						entidade.ProdutoEspecificacao = reader.GetValue<int>("produto_especificacao");
						entidade.PossuiTratamentoFinsQuarentenario = reader.GetValue<int>("possui_trat_fito_fins_quaren") > 0;
						entidade.PartidaLacradaOrigem = reader.GetValue<int>("partida_lacrada_origem") > 0;
						entidade.NumeroLacre = reader.GetValue<string>("numero_lacre");
						entidade.NumeroPorao = reader.GetValue<string>("numero_porao");
						entidade.NumeroContainer = reader.GetValue<string>("numero_container");
						entidade.ValidadeCertificado = reader.GetValue<int>("validade_certificado");
						entidade.DeclaracaoAdicional = reader.GetValue<string>("informacoes_complementares");
						entidade.DeclaracaoAdicionalHtml = reader.GetValue<string>("informacoes_complementares");
						entidade.EstadoEmissaoId = reader.GetValue<int>("estado_emissao");
						entidade.MunicipioEmissaoId = reader.GetValue<int>("municipio_emissao");
						entidade.CredenciadoId = reader.GetValue<int>("credenciado");
					}

					reader.Close();
				}

				#endregion

				if (entidade.Id <= 0 || simplificado)
				{
					return entidade;
				}

				#region Produtos

				comando = bancoDeDados.CriarComando(@"
				select distinct d.id, d.tid, d.lote, d.codigo_lote, d.data_criacao, d.cultura_id, d.cultura, d.cultivar_id, d.cultivar, d.quantidade, d.unidade_medida, d.exibe_kilos, d.unidade_medida_texto 
				from (select cp.id, cp.tid, cp.lote, l.codigo_uc || l.ano || lpad(l.numero, 4, '0') codigo_lote, l.data_criacao, c.id cultura_id, c.texto cultura, cc.id cultivar_id, cc.cultivar, case when cp.exibe_kilos is null then li.quantidade else cp.quantidade end as quantidade , 
					li.unidade_medida, cp.exibe_kilos, (select lu.texto from lov_crt_uni_prod_uni_medida lu where lu.id = li.unidade_medida) unidade_medida_texto 
					from tab_cfoc_produto cp, tab_lote l, tab_lote_item li, tab_cultura c, tab_cultura_cultivar cc
					where l.id = cp.lote and li.lote = l.id and c.id = li.cultura and cc.id = li.cultivar and cp.cfoc = :id) d 
					", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", entidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						entidade.Produtos.Add(new IdentificacaoProduto()
						{
							Id = reader.GetValue<int>("id"),
							LoteId = reader.GetValue<int>("lote"),
							LoteCodigo = reader.GetValue<string>("codigo_lote"),
							CulturaId = reader.GetValue<int>("cultura_id"),
							CulturaTexto = reader.GetValue<string>("cultura"),
							CultivarId = reader.GetValue<int>("cultivar_id"),
							CultivarTexto = reader.GetValue<string>("cultivar"),
							UnidadeMedidaId = reader.GetValue<int>("unidade_medida"),
							UnidadeMedida = reader.GetValue<string>("unidade_medida_texto"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							DataConsolidacao = new DateTecno() { Data = reader.GetValue<DateTime>("data_criacao") },
                            ExibeQtdKg = reader.GetValue<string>("exibe_kilos") == "1" ? true  : false
						});
					}

					reader.Close();
				}

				#endregion

				#region Pragas

				comando = bancoDeDados.CriarComando(@"select cp.id, cp.praga, p.nome_cientifico, p.nome_comum from tab_cfoc_praga cp, tab_praga p where p.id = cp.praga and cp.cfoc = :cfoc", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						entidade.Pragas.Add(new Praga()
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

				comando = bancoDeDados.CriarComando(@"select c.id, c.produto_comercial, c.ingrediente_ativo, c.dose, c.praga_produto, c.modo_aplicacao from tab_cfoc_trata_fitossa c where c.cfoc = :cfoc", EsquemaBanco);

				comando.AdicionarParametroEntrada("cfoc", entidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						entidade.TratamentosFitossanitarios.Add(new TratamentoFitossanitario()
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

				return entidade;
			}
		}

		public Resultados<EmissaoCFOC> Filtrar(Filtro<EmissaoCFOC> filtros)
		{
			Resultados<EmissaoCFOC> retorno = new Resultados<EmissaoCFOC>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("d.credenciado", "credenciado", filtros.Dados.CredenciadoId);

				comandtxt += comando.FiltroAnd("d.numero", "numero", filtros.Dados.Numero);

				comandtxt += comando.FiltroAndLike("d.denominador", "denominador", filtros.Dados.EmpreendimentoTexto, true, true);

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
				select c.id, c.tid, c.credenciado, c.numero, ie.denominador, c.tipo_numero,
				(select stragg(cu.texto||'/'||cc.cultivar) from tab_cfoc_produto cp, tab_lote l, tab_lote_item li, tab_cultura cu, tab_cultura_cultivar cc 
				where l.id = cp.lote and li.lote = l.id and cu.id = li.cultura and cc.id = li.cultivar and cp.cfoc = c.id) cultura_cultivar, c.situacao, ls.texto situacao_texto
				from tab_cfoc c, ins_empreendimento ie, lov_doc_fitossani_situacao ls 
				where ie.id = c.empreendimento and ls.id = c.situacao) d where d.id > 0 " + comandtxt, esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtros.Menor);
				comando.AdicionarParametroEntrada("maior", filtros.Maior);

				comandtxt = String.Format(@"select * from (
				select c.id, c.tid, c.credenciado, c.numero || case when c.serie is null then '' else '/' || c.serie end as numero, ie.denominador, c.tipo_numero,
				(select stragg(cu.texto||'/'||cc.cultivar) from tab_cfoc_produto cp, tab_lote l, tab_lote_item li, tab_cultura cu, tab_cultura_cultivar cc 
				where l.id = cp.lote and li.lote = l.id and cu.id = li.cultura and cc.id = li.cultivar and cp.cfoc = c.id) cultura_cultivar, c.situacao, ls.texto situacao_texto
				from tab_cfoc c, ins_empreendimento ie, lov_doc_fitossani_situacao ls 
				where ie.id = c.empreendimento and ls.id = c.situacao) d where d.id > 0 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					#region Adicionando os dados na classe de retorno

					EmissaoCFOC entidade;

					while (reader.Read())
					{
						entidade = new EmissaoCFOC();
						entidade.Id = reader.GetValue<int>("id");
						entidade.Tid = reader.GetValue<string>("tid");
						entidade.Numero = reader.GetValue<string>("numero");
						entidade.EmpreendimentoTexto = reader.GetValue<string>("denominador");
						entidade.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						entidade.SituacaoId = reader.GetValue<int>("situacao");
						entidade.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						entidade.TipoNumero = reader.GetValue<int>("tipo_numero");
						retorno.Itens.Add(entidade);
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
                                    select (t.numero||'/'||t.serie) numero
                                    from tab_numero_cfo_cfoc t,
                                         tab_liberacao_cfo_cfoc l
                                    where l.id = t.liberacao
                                          and t.tipo_documento = 2
                                          and t.tipo_numero = 2
                                          and not exists( select null
                                                          from cre_cfoc c
                                                          where c.numero = t.numero
                                                                and ( ( c.serie = t.serie ) or ( c.serie is null and t.serie is null ) ) )
                                          and t.situacao = 1
                                          and t.utilizado = 0
                                          and l.responsavel_tecnico = :credenciado
                                          and to_char(numero) like '__'|| to_char(sysdate, 'yy') ||'%'
                                    order by nvl(t.serie, ' '), t.numero_inicial");

                comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

                string numeroDigital = string.Empty;

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    if (reader.Read())
                    {
                        numeroDigital = reader.GetValue<string>("numero");
                    }

                    reader.Close();
                }

                return numeroDigital;
				
			}
		}

        internal List<Lista> ObterEmpreendimentosListaEtramiteX(BancoDeDados banco = null)
        {
            List<Lista> retorno = null;
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = bancoDeDados.CriarComando(@"select distinct emp.id, emp.denominador from crt_unidade_consolidacao c, crt_unidade_cons_cultivar u,
                tab_empreendimento emp, tab_titulo t, esp_abertura_livro_uc uc, esp_aber_livro_uc_cultura ucu  
                where  emp.id = c.empreendimento and c.id = u.unidade_consolidacao and ucu.especificidade = uc.id
                and c.empreendimento = t.empreendimento ");


                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    retorno = new List<Lista>();

                    while (reader.Read())
                    {
                        retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("denominador") });
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

		internal List<Lista> ObterEmpreendimentosLista(int credenciadoID, BancoDeDados banco = null)
		{
			List<Lista> retorno = null;
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select distinct emp.id, emp.denominador from crt_unidade_consolidacao c, crt_unidade_cons_cultivar u,
                tab_empreendimento emp, tab_titulo t, esp_abertura_livro_uc uc, esp_aber_livro_uc_cultura ucu  
                where  emp.id = c.empreendimento and c.id = u.unidade_consolidacao and ucu.especificidade = uc.id
                and c.empreendimento = t.empreendimento and t.situacao = 3
				and c.id in (select unidade_consolidacao from crt_unida_conso_resp_tec where responsavel_tecnico = :credenciado)");

              

				comando.AdicionarParametroEntrada("credenciado", credenciadoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<Lista>();

					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("denominador") });
					}

					reader.Close();
				}
			}

			return retorno;
		}

		internal List<Lista> ObterPragasLista(List<IdentificacaoProduto> produtos)
		{
			List<Lista> retorno = null;
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
					retorno = new List<Lista>();

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

		internal Cultivar CultivarAssociadaUC(int empreendimento, int cultivar)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select cc.cultivar Id, cc.cultura CulturaId, cc.capacidade_mes CapacidadeMes
				from crt_unidade_consolidacao c, crt_unidade_cons_cultivar cc
				where cc.unidade_consolidacao = c.id
				and c.empreendimento = :empreendimento
				and cc.cultivar = :cultivar");

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", cultivar, DbType.Int32);

				return bancoDeDados.ObterEntity<Cultivar>(comando);
			}
		}

		public ResponsavelTecnico ObterResponsavelUC(int empreendimento)
		{
			ResponsavelTecnico responsavel = new ResponsavelTecnico();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.unidade_consolidacao, c.responsavel_tecnico, c.numero_hab_cfo_cfoc, c.numero_art, c.art_cargo_funcao, c.data_validade_art, c.tid, 
				nvl(tp.nome, tp.razao_social) nome_razao, nvl(tp.cpf, tp.cnpj) cpf_cnpj, pf.texto profissao, oc.orgao_sigla, pp.registro from {0}crt_unida_conso_resp_tec c, 
				{0}tab_credenciado tc, {1}tab_pessoa tp, {1}tab_pessoa_profissao pp, {0}tab_profissao pf, {0}tab_orgao_classe oc where tc.id = c.responsavel_tecnico 
				and tp.id = tc.pessoa and pp.pessoa(+) = tp.id and pf.id(+) = pp.profissao and oc.id(+) = pp.orgao_classe 
				and c.responsavel_tecnico = :responsavel and c.unidade_consolidacao = (select t.id from crt_unidade_consolidacao t where t.empreendimento = :empreendimento)", EsquemaBanco, EsquemaCredenciado);

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("responsavel", User.FuncionarioId, DbType.Int32);

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


        internal decimal ObterSaldoRemanescente(int lote, int empreendimento)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
            {
                Comando comando = bancoDeDados.CriarComando(@"  select nvl(sum(li.quantidade),0) - ( select nvl(sum(case when p.exibe_kilos = 1 then p.quantidade / 1000 else p.quantidade end),0) from tab_cfoc cf 
                                                                inner join tab_cfoc_produto p on p.cfoc = cf.id
                                                                where cf.situacao=2 and p.lote = :lote )                                
                                                                from tab_lote l, tab_lote_item li 
                                                                where  li.lote = l.id and l.id = :lote
                                                                group by li.lote ");

                //comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
                //comando.AdicionarParametroEntrada("cultivar", cultivar, DbType.Int32);
                comando.AdicionarParametroEntrada("lote", lote, DbType.Int32);

                return bancoDeDados.ExecutarScalar<decimal>(comando);
            }
        }

		internal decimal ObterCapacidadeMes(int cfoc, int empreendimento, int cultivar)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select nvl((select sum(case when cp.exibe_kilos = 1 then cp.quantidade / 1000 else cp.quantidade end) from tab_cfoc c, tab_cfoc_produto cp, tab_lote l, tab_lote_item li 
				where c.situacao != 4 and cp.cfoc = c.id and l.id = cp.lote and li.lote = l.id and li.cultivar = :cultivar and c.empreendimento = :empreendimento and c.id != :cfoc), 0) from dual");

				comando.AdicionarParametroEntrada("empreendimento", empreendimento, DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", cultivar, DbType.Int32);
				comando.AdicionarParametroEntrada("cfoc", cfoc, DbType.Int32);

				return bancoDeDados.ExecutarScalar<decimal>(comando);
			}
		}

		#endregion

		#region Validaçoes

		public int NumeroUtilizado(long numero, string serie)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
                Comando comando = bancoDeDados.CriarComando(@"select nvl((select c.id from {0}tab_cfoc c where c.numero = :numero and ( c.serie = :serie or ( c.serie is null or :serie is null ) )), 0) from dual", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);
                comando.AdicionarParametroEntrada("serie", serie, DbType.String);

				return bancoDeDados.ExecutarScalar<int>(comando);
			}
		}

		internal bool NumeroJaExiste(string numero, int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_cfoc c where c.numero = :numero and c.credenciado = :credenciado", EsquemaCredenciado);

				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);
				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				if (id > 0)
				{
					comando.DbCommand.CommandText += " and c.id <> :id";
					comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				}

				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal bool NumeroLiberado(string numero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
                                    select count(*)
                                    from tab_numero_cfo_cfoc n,
                                         tab_liberacao_cfo_cfoc l
                                    where l.id = n.liberacao
                                          and l.responsavel_tecnico = :credenciado_id
                                          and n.numero = :numero
                                          and n.serie is null
                                          and n.tipo_documento = 2
                                          and n.tipo_numero = 1");

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
                    comando = bancoDeDados.CriarComando(@"select count(*) from tab_numero_cfo_cfoc n where n.tipo_documento = 2 and n.tipo_numero = 2 and n.situacao = 0 and n.numero = :numero and n.serie is null");
                else
                {
                    comando = bancoDeDados.CriarComando(@"select count(*) from tab_numero_cfo_cfoc n where n.tipo_documento = 2 and n.tipo_numero = 2 and n.situacao = 0 and n.numero = :numero and n.serie = :serie");
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
                                     select count(*)
                                     from tab_numero_cfo_cfoc n,
                                          tab_liberacao_cfo_cfoc l 
                                     where l.id = n.liberacao
                                           and n.tipo_documento = 2
                                           and n.tipo_numero = 2
                                           and n.situacao = 1
                                           and n.utilizado = 0 
                                           and not exists ( select null
                                                            from cre_cfoc c
                                                            where c.numero = n.numero
                                                                  and ( ( c.serie = n.serie ) or ( c.serie is null and n.serie is null ) ) ) 
                                           and l.responsavel_tecnico = :credenciado 
                                           and to_char(n.numero) like '__'|| to_char(sysdate, 'yy') ||'%' "); 

				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		internal string LoteUtilizado(int lote, int cfoc)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select distinct c.numero from tab_cfoc c, tab_cfoc_produto cp where cp.cfoc = c.id and cp.lote = :lote and cp.cfoc != :cfoc");

				comando.AdicionarParametroEntrada("lote", lote, DbType.Int32);
				comando.AdicionarParametroEntrada("cfoc", cfoc, DbType.Int32);

				return Convert.ToString(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal bool LotePossuiOrigemCancelada(int lote)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select 
				(select count(*)
				from tab_lote_item l, tab_cfo c
				where c.id = l.origem and l.origem_tipo = 1 
				and c.situacao = 4 and l.lote = :lote)
				+
				(select count(*)
				from tab_lote_item l, tab_cfoc c
				where c.id = l.origem and l.origem_tipo = 2 
				and c.situacao = 4 and l.lote = :lote)
				from dual");

				comando.AdicionarParametroEntrada("lote", lote, DbType.Int32);
				return bancoDeDados.ExecutarScalar<int>(comando) > 0;
			}
		}

		#endregion
	}
}