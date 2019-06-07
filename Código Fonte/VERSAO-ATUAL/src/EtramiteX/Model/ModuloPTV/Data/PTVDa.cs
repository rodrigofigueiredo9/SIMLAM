using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Entities.WebService;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloPTV.Data
{
	public class PTVDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		Historico _historico = new Historico();
		public Historico Historico { get { return _historico; } }

		#endregion

		#region DML

		internal void Salvar(PTV PTV, BancoDeDados banco)
		{
			if (PTV == null)
			{
				throw new Exception("PTV é nulo.");
			}

			if (PTV.Id == 0)
			{
				Criar(PTV, banco);
			}
			else
			{
				Editar(PTV, banco);
			}
		}

		internal void Criar(PTV PTV, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				#region Insert

                string sqlCmd = @"insert into {0}tab_ptv
						(id, tid, tipo_numero, numero, data_emissao, situacao, empreendimento, partida_lacrada_origem, numero_lacre, numero_porao, 
						numero_container, destinatario, possui_laudo_laboratorial, tipo_transporte, veiculo_identificacao_numero, rota_transito_definida, itinerario, apresentacao_nota_fiscal, 
						numero_nota_fiscal, valido_ate, responsavel_tecnico, responsavel_emp, municipio_emissao, dua_numero,dua_tipo_pessoa,dua_cpf_cnpj, empreendimento_sem_doc, responsavel_sem_doc)
					values
						(seq_tab_ptv.nextval,:tid,:tipo_numero,:numero,:data_emissao,:situacao,:empreendimento,:partida_lacrada_origem,:numero_lacre,:numero_porao,
						:numero_container,:destinatario,:possui_laudo_laboratorial,:tipo_transporte,:veiculo_identificacao_numero,:rota_transito_definida,:itinerario,:apresentacao_nota_fiscal,
						:numero_nota_fiscal,:valido_ate,:responsavel_tecnico,:responsavel_emp,:municipio_emissao, :dua_numero,:dua_tipo_pessoa,:dua_cpf_cnpj, :empreendimento_sem_doc, :responsavel_sem_doc) returning id into :id";

                if (PTV.Produtos.Count > 0 && ( (PTV.Produtos[0].SemDoc) ||
                    PTV.Produtos[0].OrigemTipo > (int)eDocumentoFitossanitarioTipo.PTVOutroEstado) )
                {
                    sqlCmd = sqlCmd.Replace(":empreendimento,", "");
                    sqlCmd = sqlCmd.Replace(":responsavel_emp,", "");

                    sqlCmd = sqlCmd.Replace("empreendimento,", "");
                    sqlCmd = sqlCmd.Replace("responsavel_emp,", "");
        
                }


                comando = bancoDeDados.CriarComando(sqlCmd, EsquemaBanco);
				#endregion

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("tipo_numero", PTV.NumeroTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", PTV.Numero > 0 ? PTV.Numero : (object)DBNull.Value, DbType.Int64);
				comando.AdicionarParametroEntrada("data_emissao", PTV.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", PTV.Situacao, DbType.Int32);

                if (PTV.Produtos.Count > 0 && ( (!PTV.Produtos[0].SemDoc) &&
                     PTV.Produtos[0].OrigemTipo <= (int)eDocumentoFitossanitarioTipo.PTVOutroEstado) )
                {
                    comando.AdicionarParametroEntrada("empreendimento", PTV.Empreendimento, DbType.Int32);
                    comando.AdicionarParametroEntrada("responsavel_emp", PTV.ResponsavelEmpreendimento, DbType.Int32);
                }
               

				comando.AdicionarParametroEntrada("partida_lacrada_origem", PTV.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", PTV.LacreNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", PTV.PoraoNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_container", PTV.ContainerNumero, DbType.String);
				comando.AdicionarParametroEntrada("destinatario", PTV.DestinatarioID, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_laudo_laboratorial", PTV.PossuiLaudoLaboratorial, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_transporte", PTV.TransporteTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("veiculo_identificacao_numero", PTV.VeiculoIdentificacaoNumero, DbType.String);
				comando.AdicionarParametroEntrada("rota_transito_definida", PTV.RotaTransitoDefinida, DbType.Int32);
				comando.AdicionarParametroEntrada("itinerario", PTV.Itinerario, DbType.String);
				comando.AdicionarParametroEntrada("apresentacao_nota_fiscal", PTV.NotaFiscalApresentacao, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_nota_fiscal", PTV.NotaFiscalNumero, DbType.String);
				comando.AdicionarParametroEntrada("valido_ate", PTV.ValidoAte.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("responsavel_tecnico", PTV.ResponsavelTecnicoId, DbType.Int32);
				
				comando.AdicionarParametroEntrada("municipio_emissao", PTV.LocalEmissaoId, DbType.Int32);

                comando.AdicionarParametroEntrada("dua_numero", PTV.NumeroDua, DbType.String);
                comando.AdicionarParametroEntrada("dua_cpf_cnpj", PTV.CPFCNPJDUA, DbType.String);
                comando.AdicionarParametroEntrada("dua_tipo_pessoa", PTV.TipoPessoa, DbType.Int32);

                comando.AdicionarParametroEntrada("responsavel_sem_doc", PTV.ResponsavelSemDoc, DbType.String);
                comando.AdicionarParametroEntrada("empreendimento_sem_doc", PTV.EmpreendimentoSemDoc, DbType.String);
                


				//ID de retorno
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarScalar(comando);

				PTV.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				//validação
				if (!String.IsNullOrWhiteSpace(PTV.SemDocOrigem.Produtor) || !String.IsNullOrWhiteSpace(PTV.SemDocOrigem.responsavel))
				{
					InserirProdutoSemDocOrigem(PTV, banco); 
				}

				#region Produto PTV

				comando = bancoDeDados.CriarComando(@"insert into tab_ptv_produto(id, tid, ptv, origem_tipo, origem, numero_origem, cultura, cultivar, quantidade, unidade_medida, exibe_kilos)
													  values(seq_tab_ptv_produto.nextval,:tid,:ptv,:origem_tipo,:origem,:numero_origem,:cultura,:cultivar,:quantidade,:unidade_medida,:exibe_kilos)", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("origem_tipo", DbType.Int32);
				comando.AdicionarParametroEntrada("origem", DbType.Int64);
				comando.AdicionarParametroEntrada("numero_origem", DbType.Int64);
				comando.AdicionarParametroEntrada("cultura", DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
				comando.AdicionarParametroEntrada("quantidade", DbType.Decimal);
				comando.AdicionarParametroEntrada("unidade_medida", DbType.Int32);
                comando.AdicionarParametroEntrada("exibe_kilos", DbType.String,1);

				PTV.Produtos.ForEach(item =>
				{
					comando.SetarValorParametro("origem_tipo", item.OrigemTipo);

					if (item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC ||
						item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV || item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)
					{
						comando.SetarValorParametro("numero_origem", DBNull.Value);
						comando.SetarValorParametro("origem", item.Origem);
					}
					else
					{
						comando.SetarValorParametro("numero_origem", item.OrigemNumero);
						comando.SetarValorParametro("origem", DBNull.Value);
					}

					comando.SetarValorParametro("cultura", item.Cultura);
					comando.SetarValorParametro("cultivar", item.Cultivar);
					comando.SetarValorParametro("quantidade", item.Quantidade);
					comando.SetarValorParametro("unidade_medida", item.UnidadeMedida);
                    comando.SetarValorParametro("exibe_kilos", item.ExibeQtdKg ? "1" : "0");

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Nota Fiscal De Caixa

				InserirNotaFiscalDeCaixa(PTV.NotaFiscalDeCaixas, PTV.Id, bancoDeDados);
				
				#endregion

				Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptv, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Editar(PTV PTV, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				#region Update

				string sqlCmd = (@"update {0}tab_ptv
															set tid = :tid,
																empreendimento = :empreendimento,
																partida_lacrada_origem = :partida_lacrada_origem,
																numero_lacre = :numero_lacre,
																numero_porao = :numero_porao,
																numero_container = :numero_container,
																destinatario = :destinatario,
																possui_laudo_laboratorial = :possui_laudo_laboratorial,
																tipo_transporte = :tipo_transporte,
																veiculo_identificacao_numero = :veiculo_identificacao_numero,
																rota_transito_definida = :rota_transito_definida,
																itinerario = :itinerario,
																apresentacao_nota_fiscal = :apresentacao_nota_fiscal,
																numero_nota_fiscal = :numero_nota_fiscal,
																valido_ate = :valido_ate,
																responsavel_tecnico = :responsavel_tecnico,
																responsavel_emp = :responsavel_emp,
																municipio_emissao = :municipio_emissao,
                                                                dua_numero = :dua_numero,
                                                                dua_tipo_pessoa = :dua_tipo_pessoa,
                                                                dua_cpf_cnpj = :dua_cpf_cnpj,
                                                                empreendimento_sem_doc = :empreendimento_sem_doc,
                                                                responsavel_sem_doc = :responsavel_sem_doc
															where id = :id");
				#endregion


                if (PTV.Produtos.Count > 0 && ((PTV.Produtos[0].SemDoc) ||
                 PTV.Produtos[0].OrigemTipo > (int)eDocumentoFitossanitarioTipo.PTVOutroEstado))
                {
                    sqlCmd = sqlCmd.Replace("empreendimento = :empreendimento,", "");
                    sqlCmd = sqlCmd.Replace("responsavel_emp = :responsavel_emp,", "");

                }

                comando = bancoDeDados.CriarComando(sqlCmd, EsquemaBanco);
             
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				
				comando.AdicionarParametroEntrada("partida_lacrada_origem", PTV.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", PTV.LacreNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", PTV.PoraoNumero, DbType.String);

                if (PTV.Produtos.Count > 0 && ((!PTV.Produtos[0].SemDoc) &&
                    PTV.Produtos[0].OrigemTipo <= (int)eDocumentoFitossanitarioTipo.PTVOutroEstado))
                {
                    comando.AdicionarParametroEntrada("empreendimento", PTV.Empreendimento, DbType.Int32);
                    comando.AdicionarParametroEntrada("responsavel_emp", PTV.ResponsavelEmpreendimento, DbType.Int32);
                }
               

				comando.AdicionarParametroEntrada("numero_container", PTV.ContainerNumero, DbType.String);
				comando.AdicionarParametroEntrada("destinatario", PTV.DestinatarioID, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_laudo_laboratorial", PTV.PossuiLaudoLaboratorial, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_transporte", PTV.TransporteTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("veiculo_identificacao_numero", PTV.VeiculoIdentificacaoNumero, DbType.String);
				comando.AdicionarParametroEntrada("rota_transito_definida", PTV.RotaTransitoDefinida, DbType.Int32);
				comando.AdicionarParametroEntrada("itinerario", PTV.Itinerario, DbType.String);
				comando.AdicionarParametroEntrada("apresentacao_nota_fiscal", PTV.NotaFiscalApresentacao, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_nota_fiscal", PTV.NotaFiscalNumero, DbType.String);
				comando.AdicionarParametroEntrada("valido_ate", PTV.ValidoAte.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("responsavel_tecnico", PTV.ResponsavelTecnicoId, DbType.Int32);
				
				comando.AdicionarParametroEntrada("municipio_emissao", PTV.LocalEmissaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("id", PTV.Id, DbType.Int32);

                comando.AdicionarParametroEntrada("dua_numero", PTV.NumeroDua, DbType.String);
                comando.AdicionarParametroEntrada("dua_cpf_cnpj", PTV.CPFCNPJDUA, DbType.String);
                comando.AdicionarParametroEntrada("dua_tipo_pessoa", PTV.TipoPessoa, DbType.Int32);


                comando.AdicionarParametroEntrada("responsavel_sem_doc", PTV.ResponsavelSemDoc, DbType.String);
                comando.AdicionarParametroEntrada("empreendimento_sem_doc", PTV.EmpreendimentoSemDoc, DbType.String);

				bancoDeDados.ExecutarScalar(comando);

				if (!String.IsNullOrWhiteSpace(PTV.SemDocOrigem.Produtor) || !String.IsNullOrWhiteSpace(PTV.SemDocOrigem.responsavel))
				{
					AtualizarProdutoSemDocOrigem(PTV, banco);
				}

				#region Limpar Dados

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_ptv_produto ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ptv = :ptv {0}",
				comando.AdicionarNotIn("and", "id", DbType.Int32, PTV.Produtos.Select(p => p.Id).ToList()));
				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_ptv_nf_caixa ", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format("where ptv = :ptv");
				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				#endregion


				#region Produto PTV

				PTV.Produtos.ForEach(item =>
				{
					if (item.Id > 0)
					{
						comando = bancoDeDados.CriarComando(@"
						update {0}tab_ptv_produto set tid = :tid, ptv = :ptv, origem_tipo = :origem_tipo, origem = :origem, numero_origem = :numero_origem, 
						cultura = :cultura,cultivar = :cultivar, quantidade = :quantidade, unidade_medida = :unidade_medida, exibe_kilos= :exibe_kilos where id = :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("ptv", item.PTV, DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@"
						insert into {0}tab_ptv_produto(id, tid, ptv, origem_tipo, origem, numero_origem, cultura, cultivar, quantidade, unidade_medida, exibe_kilos)
						values(seq_tab_ptv_produto.nextval,:tid,:ptv,:origem_tipo,:origem, :numero_origem,:cultura,:cultivar,:quantidade,:unidade_medida, :exibe_kilos)", EsquemaBanco);

						comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
					}
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("origem_tipo", item.OrigemTipo, DbType.Int32);

					if (item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC ||
						item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV || item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)
					{
						comando.AdicionarParametroEntrada("numero_origem", DBNull.Value, DbType.Int32);
						comando.AdicionarParametroEntrada("origem", item.Origem, DbType.Int64);
					}
					else
					{
						comando.AdicionarParametroEntrada("numero_origem", item.OrigemNumero, DbType.Int64);
						comando.AdicionarParametroEntrada("origem", DBNull.Value, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("cultura", item.Cultura, DbType.Int32);
					comando.AdicionarParametroEntrada("cultivar", item.Cultivar, DbType.Int32);
					comando.AdicionarParametroEntrada("quantidade", item.Quantidade, DbType.Decimal);
					comando.AdicionarParametroEntrada("unidade_medida", item.UnidadeMedida, DbType.Int32);
                    comando.AdicionarParametroEntrada("exibe_kilos", item.ExibeQtdKg ? "1" : "0", DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Nota Fiscal de Caixa

				InserirNotaFiscalDeCaixa(PTV.NotaFiscalDeCaixas, PTV.Id, bancoDeDados);

				#endregion

				Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptv, eHistoricoAcao.atualizar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Ativar(PTV PTV, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				#region [ Declaracao Adicional ]

				var listDeclaracoesAdicionais = new List<string>();
                foreach (var item in PTV.Produtos.Where(xx => (xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)))
				{
					listDeclaracoesAdicionais.AddRange(ObterDeclaracaoAdicional(item.Origem, item.OrigemTipo, (int)ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedida), item.Cultivar, true));
				}
				var declaracaoAdicionalHtml = String.Join(" ", listDeclaracoesAdicionais.Distinct().ToList());

				listDeclaracoesAdicionais.Clear();
                foreach (var item in PTV.Produtos.Where(xx => (xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)))
				{
					listDeclaracoesAdicionais.AddRange(ObterDeclaracaoAdicional(item.Origem, item.OrigemTipo, (int)ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedida), item.Cultivar, false));
				}
				var declaracaoAdicional = String.Join(" ", listDeclaracoesAdicionais.Distinct().ToList());

				#endregion

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_ptv p set p.tid = :tid, p.situacao = :situacao, p.data_ativacao = :data_ativacao, 
					p.numero = :numero, p.declaracao_adicional = :declaracao_adicional, p.declaracao_adicional_formatado = :declaracao_adicional_formatado
					where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", PTV.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)ePTVSituacao.Valido, DbType.Int32);
				comando.AdicionarParametroEntrada("data_ativacao", PTV.DataAtivacao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("numero", PTV.Numero, DbType.Int64);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("declaracao_adicional", declaracaoAdicional, DbType.String);
				comando.AdicionarParametroEntrada("declaracao_adicional_formatado", declaracaoAdicionalHtml, DbType.String);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptv, eHistoricoAcao.ativar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void PTVCancelar(PTV PTV, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_ptv p set p.tid = :tid, p.situacao = :situacao, p.data_cancelamento = :data_cancelamento where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", PTV.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)ePTVSituacao.Cancelado, DbType.Int32);
				comando.AdicionarParametroEntrada("data_cancelamento", PTV.DataCancelamento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptv, eHistoricoAcao.cancelar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void Excluir(int id, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				//Atualiza o tid da tabela tab_ptv
				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_ptv set tid = :tid where id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.emitirptv, eHistoricoAcao.excluir, bancoDeDados);

				comando = bancoDeDados.CriarComandoPlSql(@"
				begin
					delete {0}tab_ptv_produto pr where pr.ptv = :id;
					delete {0}tab_ptv p where p.id = :id;
				end;", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		internal void Importar(PTV PTV, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				#region Insert

				
                

                string sqlCmd = @"insert into {0}tab_ptv
								(id, tid, tipo_numero, numero, data_emissao, situacao, data_ativacao, empreendimento, partida_lacrada_origem, numero_lacre, numero_porao, 
								numero_container, destinatario, possui_laudo_laboratorial, tipo_transporte, veiculo_identificacao_numero, rota_transito_definida, itinerario, apresentacao_nota_fiscal, 
								numero_nota_fiscal, valido_ate, responsavel_tecnico, responsavel_emp, municipio_emissao, eptv_id, dua_numero,dua_tipo_pessoa,dua_cpf_cnpj, empreendimento_sem_doc, responsavel_sem_doc)
							values
								(seq_tab_ptv.nextval,:tid,:tipo_numero,:numero,:data_emissao,:situacao, sysdate,:empreendimento,:partida_lacrada_origem,:numero_lacre,:numero_porao,
								:numero_container,:destinatario,:possui_laudo_laboratorial,:tipo_transporte,:veiculo_identificacao_numero,:rota_transito_definida,:itinerario,:apresentacao_nota_fiscal,
								:numero_nota_fiscal,:valido_ate,:responsavel_tecnico,:responsavel_emp,:municipio_emissao, :eptv_id, :dua_numero,:dua_tipo_pessoa,:dua_cpf_cnpj, :empreendimento_sem_doc, :responsavel_sem_doc) returning id into :id";



                if (PTV.Produtos.Count > 0 && ((PTV.Produtos[0].SemDoc) ||
                       PTV.Produtos[0].OrigemTipo > (int)eDocumentoFitossanitarioTipo.PTVOutroEstado))
                {
                    sqlCmd = sqlCmd.Replace(":empreendimento,", "");
                    sqlCmd = sqlCmd.Replace(":responsavel_emp,", "");

                    sqlCmd = sqlCmd.Replace("empreendimento,", "");
                    sqlCmd = sqlCmd.Replace("responsavel_emp,", "");

                }

                comando = bancoDeDados.CriarComando(sqlCmd, EsquemaBanco);

				#endregion

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("tipo_numero", PTV.NumeroTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("numero", PTV.Numero > 0 ? PTV.Numero : (object)DBNull.Value, DbType.Int64);
				comando.AdicionarParametroEntrada("data_emissao", PTV.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", (int)ePTVSituacao.Valido, DbType.Int32);

                if (PTV.Produtos.Count > 0 && ((!PTV.Produtos[0].SemDoc) &&
                    PTV.Produtos[0].OrigemTipo <= (int)eDocumentoFitossanitarioTipo.PTVOutroEstado))
                {
                    comando.AdicionarParametroEntrada("empreendimento", PTV.Empreendimento, DbType.Int32);
                    comando.AdicionarParametroEntrada("responsavel_emp", PTV.ResponsavelEmpreendimento, DbType.Int32);
                }
				
				comando.AdicionarParametroEntrada("partida_lacrada_origem", PTV.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", PTV.LacreNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", PTV.PoraoNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_container", PTV.ContainerNumero, DbType.String);
				comando.AdicionarParametroEntrada("destinatario", PTV.DestinatarioID, DbType.Int32);
				comando.AdicionarParametroEntrada("possui_laudo_laboratorial", PTV.PossuiLaudoLaboratorial, DbType.Int32);
				comando.AdicionarParametroEntrada("tipo_transporte", PTV.TransporteTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("veiculo_identificacao_numero", PTV.VeiculoIdentificacaoNumero, DbType.String);
				comando.AdicionarParametroEntrada("rota_transito_definida", PTV.RotaTransitoDefinida, DbType.Int32);
				comando.AdicionarParametroEntrada("itinerario", PTV.Itinerario, DbType.String);
				comando.AdicionarParametroEntrada("apresentacao_nota_fiscal", PTV.NotaFiscalApresentacao, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_nota_fiscal", PTV.NotaFiscalNumero, DbType.String);
				comando.AdicionarParametroEntrada("valido_ate", PTV.ValidoAte.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("responsavel_tecnico", PTV.ResponsavelTecnicoId, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio_emissao", PTV.LocalEmissaoId, DbType.Int32);
				comando.AdicionarParametroEntrada("eptv_id", PTV.Id, DbType.Int32);

                comando.AdicionarParametroEntrada("dua_numero", PTV.NumeroDua, DbType.String);
                comando.AdicionarParametroEntrada("dua_cpf_cnpj", PTV.CPFCNPJDUA, DbType.String);
                comando.AdicionarParametroEntrada("dua_tipo_pessoa", PTV.TipoPessoa, DbType.Int32);

                comando.AdicionarParametroEntrada("responsavel_sem_doc", PTV.ResponsavelSemDoc, DbType.String);
                comando.AdicionarParametroEntrada("empreendimento_sem_doc", PTV.EmpreendimentoSemDoc, DbType.String);

				//ID de retorno
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarScalar(comando);

				PTV.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Produto PTV

				comando = bancoDeDados.CriarComando(@"insert into tab_ptv_produto(id, tid, ptv, origem_tipo, origem, numero_origem, cultura, cultivar, quantidade, unidade_medida, exibe_kilos)
													  values(seq_tab_ptv_produto.nextval,:tid,:ptv,:origem_tipo,:origem,:numero_origem,:cultura,:cultivar,:quantidade,:unidade_medida, :exibe_kilos)", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("origem_tipo", DbType.Int32);
				comando.AdicionarParametroEntrada("origem", DbType.Int64);
				comando.AdicionarParametroEntrada("numero_origem", DbType.Int64);
				comando.AdicionarParametroEntrada("cultura", DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
				comando.AdicionarParametroEntrada("quantidade", DbType.Decimal);
				comando.AdicionarParametroEntrada("unidade_medida", DbType.Int32);
                comando.AdicionarParametroEntrada("exibe_kilos", DbType.String,1);

				PTV.Produtos.ForEach(item =>
				{
					comando.SetarValorParametro("origem_tipo", item.OrigemTipo);

					if (item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC ||
						item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV || item.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)
					{
						comando.SetarValorParametro("numero_origem", DBNull.Value);
						comando.SetarValorParametro("origem", item.Origem);
					}
					else
					{
						comando.SetarValorParametro("numero_origem", item.OrigemNumero);
						comando.SetarValorParametro("origem", DBNull.Value);
					}

					comando.SetarValorParametro("cultura", item.Cultura);
					comando.SetarValorParametro("cultivar", item.Cultivar);
					comando.SetarValorParametro("quantidade", item.Quantidade);
					comando.SetarValorParametro("unidade_medida", item.UnidadeMedida);
                    comando.SetarValorParametro("exibe_kilos", item.ExibeQtdKg ? "1" : "0");

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

				#region Arquivos

				if (PTV.Anexos != null && PTV.Anexos.Count > 0)
				{
					comando = bancoDeDados.CriarComando(@"insert into {0}tab_ptv_arquivo a (id, ptv, arquivo, ordem, descricao, tid) 
					values ({0}seq_ptv_arquivo.nextval, :ptv, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);

					comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("arquivo", DbType.Int32);
					comando.AdicionarParametroEntrada("ordem", DbType.Int32);
					comando.AdicionarParametroEntrada("descricao", DbType.String, 100);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

					foreach (Anexo item in PTV.Anexos)
					{
						if (item.Arquivo.Id > 0)
						{
							comando.SetarValorParametro("arquivo", item.Arquivo.Id);
							comando.SetarValorParametro("ordem", item.Ordem);
							comando.SetarValorParametro("descricao", item.Descricao);
							bancoDeDados.ExecutarNonQuery(comando);
						}
					}
				}

				#endregion

				Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptv, eHistoricoAcao.ativar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		internal void InserirNotaFiscalDeCaixa(List<NotaFiscalCaixa> lstNotaFiscalDeCaixa, int ptvID, BancoDeDados bancoDeDados)
		{
			lstNotaFiscalDeCaixa.ForEach(item =>
			{
				Comando comando;

				if (item.id <= 0)
				{
					var notaFiscal = VerificarNumeroNFCaixa(item);
					if (notaFiscal.id > 0)
						item.id = notaFiscal.id;
					else
					{
						comando = bancoDeDados.CriarComando(@"INSERT INTO TAB_NF_CAIXA (ID, TID, NUMERO, TIPO_CAIXA, SALDO_INICIAL,
													TIPO_PESSOA, CPF_CNPJ_ASSOCIADO)
												VALUES(SEQ_NF_CAIXA.NEXTVAL, :tid, :numero, :tipo, :saldoInicial,
													:tipoPessoa, :cpfCnpj) returning id into :id", EsquemaBanco);

						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
						comando.AdicionarParametroEntrada("numero", item.notaFiscalCaixaNumero, DbType.String);
						comando.AdicionarParametroEntrada("tipo", (int)item.tipoCaixaId, DbType.Int32);
						comando.AdicionarParametroEntrada("saldoInicial", item.saldoAtual, DbType.Int32);
						comando.AdicionarParametroEntrada("tipoPessoa", (int)item.PessoaAssociadaTipo, DbType.Int32);
						comando.AdicionarParametroEntrada("cpfCnpj", item.PessoaAssociadaCpfCnpj, DbType.String);

						comando.AdicionarParametroSaida("id", DbType.Int32);

						bancoDeDados.ExecutarScalar(comando);

						item.id = Convert.ToInt32(comando.ObterValorParametro("id"));

						Historico.Gerar(item.id, eHistoricoArtefato.notafiscalcaixa, eHistoricoAcao.criar, bancoDeDados);
					}
				}

				comando = bancoDeDados.CriarComando(@"INSERT INTO TAB_PTV_NF_CAIXA (ID, TID, NF_CAIXA, PTV, SALDO_ATUAL, NUMERO_CAIXAS)
														VALUES(SEQ_PTV_NF_CAIXA.NEXTVAL, :tid, :nfCaixa, :ptv, :saldoAtual, :nCaixas)", EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("nfCaixa", item.id, DbType.Int32);
				comando.AdicionarParametroEntrada("ptv", ptvID, DbType.Int32);
				comando.AdicionarParametroEntrada("saldoAtual", item.saldoAtual, DbType.Int32);
				comando.AdicionarParametroEntrada("nCaixas", item.numeroCaixas, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
			});
		}

		internal string ValidarNumeroNotaFiscalDeCaixa(NotaFiscalCaixa notaFiscal)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando;
				comando = bancoDeDados.CriarComando(@"
								SELECT TC.TEXTO TIPO_CAIXA FROM TAB_NF_CAIXA NF INNER JOIN LOV_TIPO_CAIXA TC ON NF.TIPO_CAIXA = TC.ID 
								WHERE NUMERO = :numero AND TIPO_CAIXA != :tipo AND ROWNUM <= 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", notaFiscal.notaFiscalCaixaNumero, DbType.String);
				comando.AdicionarParametroEntrada("tipo", notaFiscal.tipoCaixaId, DbType.Int32);

				return (string)bancoDeDados.ExecutarScalar(comando);
			}
		}
	
		internal void InserirProdutoSemDocOrigem(PTV ptv, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				string sqlCmd = @"INSERT INTO TAB_PTV_SEM_DOC_ORIGEM (ID, TID, PTV, EMPREENDIMENTO, ENDERECO, UF, MUNICIPIO, PRODUTOR, RESPONSAVEL, CPF_CNPJ) 
					VALUES(SEQ_PTV_SEM_DOC_ORIGEM.NEXTVAL, :tid, :ptv, :empreendimento, :endereco, :uf, :municipio, :produtor, :responsavel, :cpf_cnpj)";

				comando = bancoDeDados.CriarComando(sqlCmd, EsquemaBanco);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("ptv", ptv.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", ptv.EmpreendimentoTexto, DbType.String);
				comando.AdicionarParametroEntrada("endereco", ptv.SemDocOrigem.enderecoEmpreendimento, DbType.String);
				comando.AdicionarParametroEntrada("uf", ptv.SemDocOrigem.ufEndereco, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", ptv.SemDocOrigem.municipioEndereco, DbType.Int32);
				comando.AdicionarParametroEntrada("produtor", ptv.SemDocOrigem.Produtor, DbType.String);
				comando.AdicionarParametroEntrada("responsavel", ptv.SemDocOrigem.responsavel, DbType.String);
				comando.AdicionarParametroEntrada("cpf_cnpj", ptv.SemDocOrigem.cpfCnpjProdutor, DbType.String);

				bancoDeDados.ExecutarScalar(comando);

			}
		}

		internal void AtualizarProdutoSemDocOrigem(PTV ptv, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				string sqlCmd = @"UPDATE TAB_PTV_SEM_DOC_ORIGEM 
									SET EMPREENDIMENTO = :empreendimento, UF = :uf, MUNICIPIO = :municipio, PRODUTOR = :produtor,
									RESPONSAVEL = :responsavel, CPF_CNPJ = :cpf_cnpj 
									WHERE PTV = :ptv";

				comando = bancoDeDados.CriarComando(sqlCmd, EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv", ptv.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("empreendimento", ptv.EmpreendimentoTexto, DbType.String);
				comando.AdicionarParametroEntrada("endereco", ptv.SemDocOrigem.enderecoEmpreendimento, DbType.String);
				comando.AdicionarParametroEntrada("uf", ptv.SemDocOrigem.ufEndereco, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", ptv.SemDocOrigem.municipioEndereco, DbType.Int32);
				comando.AdicionarParametroEntrada("produtor", ptv.SemDocOrigem.Produtor, DbType.String);
				comando.AdicionarParametroEntrada("responsavel", ptv.SemDocOrigem.responsavel, DbType.String);
				comando.AdicionarParametroEntrada("cpf_cnpj", ptv.SemDocOrigem.cpfCnpjProdutor, DbType.String);

				bancoDeDados.ExecutarScalar(comando);

			}
		}
		#endregion

		#region Obter /Filtros

		internal PTV Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				PTV PTV = new PTV();

				Comando comando = bancoDeDados.CriarComando(@" select p.id, p.tid, p.tipo_numero, p.numero, p.data_emissao, p.situacao,
                    lst.texto situacao_texto, p.empreendimento, p.responsavel_emp, em.denominador, p.partida_lacrada_origem, p.numero_lacre,
                    p.numero_porao, p.numero_container, p.destinatario, p.possui_laudo_laboratorial, p.tipo_transporte, p.veiculo_identificacao_numero, p.rota_transito_definida, p.itinerario, p.apresentacao_nota_fiscal,
                    p.numero_nota_fiscal, p.valido_ate, p.responsavel_tecnico, f.nome responsavel_tecnico_nome, p.municipio_emissao, p.dua_numero,p.dua_tipo_pessoa,p.dua_cpf_cnpj, p.responsavel_sem_doc, p.empreendimento_sem_doc
                    from {0}tab_ptv p, {0}tab_empreendimento em, lov_ptv_situacao lst, {0}tab_funcionario f
                    where em.id(+) = p.empreendimento and lst.id = p.situacao and p.responsavel_tecnico = f.id and p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						PTV.Id = id;
						PTV.Tid = reader.GetValue<string>("tid");
						PTV.Numero = reader.GetValue<Int64>("numero");
						PTV.NumeroTipo = reader.GetValue<int>("tipo_numero");
						PTV.DataEmissao.Data = reader.GetValue<DateTime>("data_emissao");
						PTV.Situacao = reader.GetValue<int>("situacao");
						PTV.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						PTV.Empreendimento = reader.GetValue<int>("empreendimento");
						PTV.ResponsavelEmpreendimento = reader.GetValue<int>("responsavel_emp");
						PTV.EmpreendimentoTexto = reader.GetValue<string>("denominador");
						PTV.PartidaLacradaOrigem = reader.GetValue<int>("partida_lacrada_origem");
						PTV.LacreNumero = reader.GetValue<string>("numero_lacre");
						PTV.PoraoNumero = reader.GetValue<string>("numero_porao");
						PTV.ContainerNumero = reader.GetValue<string>("numero_container");
						PTV.DestinatarioID = reader.GetValue<int>("destinatario");
						PTV.PossuiLaudoLaboratorial = reader.GetValue<int>("possui_laudo_laboratorial");
						PTV.TransporteTipo = reader.GetValue<int>("tipo_transporte");
						PTV.VeiculoIdentificacaoNumero = reader.GetValue<string>("veiculo_identificacao_numero");
						PTV.RotaTransitoDefinida = reader.GetValue<int>("rota_transito_definida");
						PTV.Itinerario = reader.GetValue<string>("itinerario");
						PTV.NotaFiscalApresentacao = reader.GetValue<int>("apresentacao_nota_fiscal");
						PTV.NotaFiscalNumero = reader.GetValue<string>("numero_nota_fiscal");
						PTV.ValidoAte.Data = reader.GetValue<DateTime>("valido_ate");
						PTV.ResponsavelTecnicoId = reader.GetValue<int>("responsavel_tecnico");
						PTV.ResponsavelTecnicoNome = reader.GetValue<string>("responsavel_tecnico_nome");
						PTV.LocalEmissaoId = reader.GetValue<int>("municipio_emissao");
                        PTV.CPFCNPJDUA = reader.GetValue<string>("dua_cpf_cnpj");
                        PTV.NumeroDua = reader.GetValue<string>("dua_numero");
                        PTV.ResponsavelSemDoc = reader.GetValue<string>("responsavel_sem_doc");
                        PTV.EmpreendimentoSemDoc = reader.GetValue<string>("empreendimento_sem_doc");
					}

					reader.Close();
				}

				if (PTV.Id <= 0 || simplificado)
				{
					return PTV;
				}

				PTV = ObterSemDocOrigem(PTV);

				#region PTV Produto

				comando = bancoDeDados.CriarComando(@"select pr.id,
															 pr.tid,
															 pr.ptv,
															 pr.origem_tipo,
															 pr.origem,
															case pr.origem_tipo 
															    when 1 then (select to_char(t.numero) || case when t.serie is null then '' else '/' || t.serie end as numero from cre_cfo t where t.id = pr.origem) 
															    when 2 then (select to_char(t.numero) || case when t.serie is null then '' else '/' || t.serie end as numero from cre_cfoc t where t.id = pr.origem) 
															    when 3 then (select to_char(t.numero) from tab_ptv t where t.id = pr.origem) 
																when 4 then (select to_char(t.numero) from tab_ptv_outrouf t where t.id = pr.origem) 
															 else to_char(pr.numero_origem) end as origem_texto,
															 pr.numero_origem,
															 t.texto tipo_origem_texto,
															 pr.cultura,
															 pr.cultivar,
															 c.texto ||'/'||cc.cultivar as cultura_cultivar,
															 pr.quantidade,
															 pr.unidade_medida,
                                                             pr.exibe_kilos,
															 u.texto unidade_medida_texto
														from tab_ptv_produto pr, lov_doc_fitossanitarios_tipo t, tab_cultura c, tab_cultura_cultivar cc, lov_crt_uni_prod_uni_medida  u
														where t.id = pr.origem_tipo  
														  and c.id = pr.cultura
														  and cc.id = pr.cultivar
														  and u.id = pr.unidade_medida    
														  and pr.ptv = :ptv", EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						PTV.Produtos.Add(new PTVProduto()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							PTV = reader.GetValue<int>("ptv"),
							OrigemTipo = reader.GetValue<int>("origem_tipo"),
							OrigemTipoTexto = reader.GetValue<string>("tipo_origem_texto") + '-' + reader.GetValue<string>("origem_texto"),
							Origem = reader.GetValue<int>("origem"), //Origem ID
							OrigemNumero = reader.GetValue<string>("numero_origem"), //PTV outro Estado
							IsNumeroOrigem = String.IsNullOrEmpty(reader.GetValue<string>("origem")),
							Cultura = reader.GetValue<int>("cultura"),
							Cultivar = reader.GetValue<int>("cultivar"),
							CulturaCultivar = reader.GetValue<string>("cultura_cultivar"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							UnidadeMedida = reader.GetValue<int>("unidade_medida"),
							UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto"),
                            ExibeQtdKg = reader.GetValue<string>("exibe_kilos") == "1" ? true  :false
						});
					}

					reader.Close();
				}
				#endregion

				#region Arquivos

				comando = bancoDeDados.CriarComando(@"select a.id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho,
				a.tid from {0}tab_ptv_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.ptv = :ptv order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Anexo item;
					while (reader.Read())
					{
						item = new Anexo();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();

						item.Ordem = Convert.ToInt32(reader["ordem"]);
						item.Descricao = reader["descricao"].ToString();

						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();

						PTV.Anexos.Add(item);
					}
					reader.Close();
				}

				#endregion

				return PTV;
			}
		}

		internal PTV ObterPorIDCredenciado(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				PTV PTV = new PTV();

				Comando comando = bancoDeDados.CriarComando(@" select p.id, p.tid, p.tipo_numero, p.numero, p.data_emissao, p.situacao,
                    lst.texto situacao_texto, p.empreendimento, p.responsavel_emp, em.denominador, p.partida_lacrada_origem, p.numero_lacre,
                    p.numero_porao, p.numero_container, p.destinatario, p.possui_laudo_laboratorial, p.tipo_transporte, p.veiculo_identificacao_numero, p.rota_transito_definida, p.itinerario, p.apresentacao_nota_fiscal,
                    p.numero_nota_fiscal, p.valido_ate, p.responsavel_tecnico, f.nome responsavel_tecnico_nome, p.municipio_emissao, p.dua_numero,p.dua_tipo_pessoa,p.dua_cpf_cnpj, p.responsavel_sem_doc, p .empreendimento_sem_doc
                    from {0}tab_ptv p, {0}tab_empreendimento em, lov_ptv_situacao lst, {0}tab_funcionario f
                    where em.id(+) = p.empreendimento and lst.id = p.situacao and p.responsavel_tecnico = f.id and p.eptv_id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						PTV.Id = reader.GetValue<int>("id");
						PTV.Tid = reader.GetValue<string>("tid");
						PTV.Numero = reader.GetValue<Int64>("numero");
						PTV.NumeroTipo = reader.GetValue<int>("tipo_numero");
						PTV.DataEmissao.Data = reader.GetValue<DateTime>("data_emissao");
						PTV.Situacao = reader.GetValue<int>("situacao");
						PTV.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						PTV.Empreendimento = reader.GetValue<int>("empreendimento");
						PTV.ResponsavelEmpreendimento = reader.GetValue<int>("responsavel_emp");
						PTV.EmpreendimentoTexto = reader.GetValue<string>("denominador");
						PTV.PartidaLacradaOrigem = reader.GetValue<int>("partida_lacrada_origem");
						PTV.LacreNumero = reader.GetValue<string>("numero_lacre");
						PTV.PoraoNumero = reader.GetValue<string>("numero_porao");
						PTV.ContainerNumero = reader.GetValue<string>("numero_container");
						PTV.DestinatarioID = reader.GetValue<int>("destinatario");
						PTV.PossuiLaudoLaboratorial = reader.GetValue<int>("possui_laudo_laboratorial");
						PTV.TransporteTipo = reader.GetValue<int>("tipo_transporte");
						PTV.VeiculoIdentificacaoNumero = reader.GetValue<string>("veiculo_identificacao_numero");
						PTV.RotaTransitoDefinida = reader.GetValue<int>("rota_transito_definida");
						PTV.Itinerario = reader.GetValue<string>("itinerario");
						PTV.NotaFiscalApresentacao = reader.GetValue<int>("apresentacao_nota_fiscal");
						PTV.NotaFiscalNumero = reader.GetValue<string>("numero_nota_fiscal");
						PTV.ValidoAte.Data = reader.GetValue<DateTime>("valido_ate");
						PTV.ResponsavelTecnicoId = reader.GetValue<int>("responsavel_tecnico");
						PTV.ResponsavelTecnicoNome = reader.GetValue<string>("responsavel_tecnico_nome");
						PTV.LocalEmissaoId = reader.GetValue<int>("municipio_emissao");
						PTV.CPFCNPJDUA = reader.GetValue<string>("dua_cpf_cnpj");
						PTV.NumeroDua = reader.GetValue<string>("dua_numero");
						PTV.ResponsavelSemDoc = reader.GetValue<string>("responsavel_sem_doc");
						PTV.EmpreendimentoSemDoc = reader.GetValue<string>("empreendimento_sem_doc");
					}

					reader.Close();
				}

				if (PTV.Id <= 0 || simplificado)
				{
					return PTV;
				}

				PTV = ObterSemDocOrigem(PTV);

				#region PTV Produto

				comando = bancoDeDados.CriarComando(@"select pr.id,
															 pr.tid,
															 pr.ptv,
															 pr.origem_tipo,
															 pr.origem,
															case pr.origem_tipo 
															    when 1 then (select to_char(t.numero) || case when t.serie is null then '' else '/' || t.serie end as numero from cre_cfo t where t.id = pr.origem) 
															    when 2 then (select to_char(t.numero) || case when t.serie is null then '' else '/' || t.serie end as numero from cre_cfoc t where t.id = pr.origem) 
															    when 3 then (select to_char(t.numero) from tab_ptv t where t.id = pr.origem) 
																when 4 then (select to_char(t.numero) from tab_ptv_outrouf t where t.id = pr.origem) 
															 else to_char(pr.numero_origem) end as origem_texto,
															 pr.numero_origem,
															 t.texto tipo_origem_texto,
															 pr.cultura,
															 pr.cultivar,
															 c.texto ||'/'||cc.cultivar as cultura_cultivar,
															 pr.quantidade,
															 pr.unidade_medida,
                                                             pr.exibe_kilos,
															 u.texto unidade_medida_texto
														from tab_ptv_produto pr, lov_doc_fitossanitarios_tipo t, tab_cultura c, tab_cultura_cultivar cc, lov_crt_uni_prod_uni_medida  u
														where t.id = pr.origem_tipo  
														  and c.id = pr.cultura
														  and cc.id = pr.cultivar
														  and u.id = pr.unidade_medida    
														  and pr.ptv = :ptv", EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						PTV.Produtos.Add(new PTVProduto()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							PTV = reader.GetValue<int>("ptv"),
							OrigemTipo = reader.GetValue<int>("origem_tipo"),
							OrigemTipoTexto = reader.GetValue<string>("tipo_origem_texto") + '-' + reader.GetValue<string>("origem_texto"),
							Origem = reader.GetValue<int>("origem"), //Origem ID
							OrigemNumero = reader.GetValue<string>("numero_origem"), //PTV outro Estado
							IsNumeroOrigem = String.IsNullOrEmpty(reader.GetValue<string>("origem")),
							Cultura = reader.GetValue<int>("cultura"),
							Cultivar = reader.GetValue<int>("cultivar"),
							CulturaCultivar = reader.GetValue<string>("cultura_cultivar"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							UnidadeMedida = reader.GetValue<int>("unidade_medida"),
							UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto"),
							ExibeQtdKg = reader.GetValue<string>("exibe_kilos") == "1" ? true : false
						});
					}

					reader.Close();
				}
				#endregion

				#region Arquivos

				comando = bancoDeDados.CriarComando(@"select a.id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho,
				a.tid from {0}tab_ptv_arquivo a, {0}tab_arquivo b where a.arquivo = b.id and a.ptv = :ptv order by a.ordem", EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					Anexo item;
					while (reader.Read())
					{
						item = new Anexo();
						item.Id = Convert.ToInt32(reader["id"]);
						item.Tid = reader["tid"].ToString();

						item.Ordem = Convert.ToInt32(reader["ordem"]);
						item.Descricao = reader["descricao"].ToString();

						item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
						item.Arquivo.Caminho = reader["caminho"].ToString();
						item.Arquivo.Nome = reader["nome"].ToString();
						item.Arquivo.Extensao = reader["extensao"].ToString();

						PTV.Anexos.Add(item);
					}
					reader.Close();
				}

				#endregion

				return PTV;
			}
		}

		internal bool VerificarConfigNumero(int tipoNumero, Int64 PTVNumero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*)
																from {0}cnf_doc_fito_intervalo tt
															  where tt.tipo_documento = 3
																and tt.tipo = :tipoNumero
																and (tt.numero_inicial <= :numeroPTV and tt.numero_final >= :numeroPTV)", EsquemaBanco);
				comando.AdicionarParametroEntrada("tipoNumero", tipoNumero, DbType.Int32);
				comando.AdicionarParametroEntrada("numeroPTV", PTVNumero, DbType.Int64);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal bool VerificarNumeroPTV(Int64 ptvNumero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_ptv t where t.numero = :numero", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", ptvNumero, DbType.Int64);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal Dictionary<string, object> VerificarDocumentoOrigem(eDocumentoFitossanitarioTipo origemTipo, long numero, string serieNumeral = "")
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Dictionary<string, object> retorno = null;
				Comando comando = null;
                string strSql = "";

				switch (origemTipo)
				{
                    case eDocumentoFitossanitarioTipo.CFO:
                        strSql = @"select t.id, t.situacao, e.id empreendimento_id, e.denominador empreendimento_denominador 
						from {0}cre_cfo t, {0}tab_empreendimento e where t.empreendimento = e.id and t.numero = :numero";

                        if (!string.IsNullOrEmpty(serieNumeral))
                            strSql += " and serie = :serie ";
                        else
                            strSql += " and serie is null ";
                        comando = bancoDeDados.CriarComando(strSql, EsquemaBanco); break;

                    case eDocumentoFitossanitarioTipo.CFOC:
                        strSql = @"select t.id, t.situacao, e.id empreendimento_id, e.denominador empreendimento_denominador 
						from {0}cre_cfoc t, {0}tab_empreendimento e where t.empreendimento = e.id and t.numero = :numero";

                        if (!string.IsNullOrEmpty(serieNumeral))
                            strSql += " and serie = :serie ";
                        else
                            strSql += " and serie is null ";

                        comando = bancoDeDados.CriarComando(strSql, EsquemaBanco); break;
                    case eDocumentoFitossanitarioTipo.PTV:
                        comando = bancoDeDados.CriarComando(@"select t.id, t.situacao, e.id empreendimento_id, e.denominador empreendimento_denominador 
						from {0}tab_ptv t, {0}tab_empreendimento e where t.empreendimento = e.id and t.numero = :numero", EsquemaBanco); break;
                    case eDocumentoFitossanitarioTipo.PTVOutroEstado:
                        comando = bancoDeDados.CriarComando(@"select t.id, t.situacao, e.id empreendimento_id, e.denominador empreendimento_denominador
						from {0}tab_ptv_outrouf t, {0}tab_destinatario_ptv d, {0}tab_empreendimento e where t.destinatario = d.id and d.empreendimento_id = e.id
						and t.numero = :numero", EsquemaBanco); break;
					default:
						comando = bancoDeDados.CriarComando(@"SELECT P.ID, P.SITUACAO, 0 EMPREENDIMENTO_ID, SD.EMPREENDIMENTO EMPREEENDIMENTO_DENOMINADOR 
															  FROM TAB_PTV P 
																	INNER JOIN TAB_PTV_PRODUTO PR ON P.ID = PR.PTV
																	LEFT JOIN TAB_PTV_SEM_DOC_ORIGEM SD ON PR.ORIGEM = SD.ID
																WHERE P.NUMERO = :numero", EsquemaBanco);
						break;
				}

				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);

                if (!string.IsNullOrEmpty(serieNumeral))
                    comando.AdicionarParametroEntrada("serie", serieNumeral, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = new Dictionary<string, object>();
						retorno.Add("id", reader.GetValue<int>("id"));
						retorno.Add("situacao", reader.GetValue<int>("situacao"));
						retorno.Add("empreendimento_id", reader.GetValue<int>("empreendimento_id"));
						retorno.Add("empreendimento_denominador", reader.GetValue<string>("empreendimento_denominador"));
					}
				}

				return retorno;
			}
		}

		internal string ObterNumeroDigital()
		{
            string retorno = string.Empty;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare
					v_aux   number := 0;
					v_maior number := 0;
				begin
					select nvl((select max(d.numero) from tab_ptv d where d.tipo_numero = 2
									and to_char(d.numero) like '__'|| to_char(sysdate, 'yy') ||'%'),
						(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = 3 and c.tipo = 2
									and to_char(c.numero_inicial) like '__'|| to_char(sysdate, 'yy') ||'%'))
					into v_maior from dual;

					v_maior := v_maior + 1;

					select count(1) into v_aux from cnf_doc_fito_intervalo c 
					where c.tipo_documento = 3 and c.tipo = 2 
					and substr(c.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
					and substr(c.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
					and (v_maior between c.numero_inicial and c.numero_final);

					if (v_aux <= 0) then
						v_aux := v_maior;

						select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
						where t.tipo_documento = 3 and t.tipo = 2 
						and substr(t.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
						and substr(t.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3)
						and t.numero_inicial > v_maior;

						if(v_maior is null or v_aux = v_maior) then 
							v_maior := null;
						end if;
					end if;
					select v_maior into :numero_saida from dual;
				end;", EsquemaBanco);
				comando.AdicionarParametroSaida("numero_saida", DbType.Int64);
				bancoDeDados.ExecutarNonQuery(comando);

                retorno = Convert.ToString(comando.ObterValorParametro("numero_saida"));
            }

            //Credenciado
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
            {
                Comando comando = bancoDeDados.CriarComandoPlSql(@"
				declare
					v_aux   number := 0;
					v_maior number := 0;
				begin
					select nvl((select max(d.numero) from tab_ptv d where d.tipo_numero = 2
									and to_char(d.numero) like '__'|| to_char(sysdate, 'yy') ||'%'),
						(select min(c.numero_inicial) - 1 from cnf_doc_fito_intervalo c where c.tipo_documento = 3 and c.tipo = 2
									and to_char(c.numero_inicial) like '__'|| to_char(sysdate, 'yy') ||'%'))
					into v_maior from dual;

					v_maior := v_maior + 1;

					select count(1) into v_aux from cnf_doc_fito_intervalo c 
					where c.tipo_documento = 3 and c.tipo = 2 
					and substr(c.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
					and substr(c.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
					and (v_maior between c.numero_inicial and c.numero_final);

					if (v_aux <= 0) then
						v_aux := v_maior;

						select min(t.numero_inicial) into v_maior from cnf_doc_fito_intervalo t 
						where t.tipo_documento = 3 and t.tipo = 2 
						and substr(t.numero_inicial, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3) 
						and substr(t.numero_final, 3, 2) = substr(EXTRACT(YEAR FROM sysdate), 3)
						and t.numero_inicial > v_maior;

						if(v_maior is null or v_aux = v_maior) then 
							v_maior := null;
						end if;
					end if;
					select v_maior into :numero_saida from dual;
				end;", UsuarioCredenciado);
                comando.AdicionarParametroSaida("numero_saida", DbType.Int64);
                bancoDeDados.ExecutarNonQuery(comando);

                if (!Convert.IsDBNull(comando.ObterValorParametro("numero_saida")) && comando.ObterValorParametro("numero_saida") != null)
                {
                    string aux = Convert.ToString(comando.ObterValorParametro("numero_saida"));

                    if (string.IsNullOrEmpty(retorno) || Convert.ToInt64(aux) > Convert.ToInt64(retorno))
                    {
                        retorno = aux;
                    }
                }
            }

            return retorno;
		}

		internal List<ListaValor> ObterResponsaveisEmpreendimento(int empreendimentoID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<ListaValor> retorno = null;

				Comando comando = bancoDeDados.CriarComando(@"select r.responsavel as responsavel, nvl(p.nome, p.razao_social)as razao_social
															  from {0}tab_empreendimento_responsavel r, {0}tab_pessoa p
															 where p.id = r.responsavel
															   and r.empreendimento = :empreendimentoID", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<ListaValor>();
					while (reader.Read())
					{
						retorno.Add(new ListaValor() { Id = reader.GetValue<int>("responsavel"), Texto = reader.GetValue<string>("razao_social") });
					}

					reader.Close();
				}
				return retorno;
			}
		}

		internal List<Lista> ObterNumeroOrigem(eDocumentoFitossanitarioTipo origemTipo, int empreendimentoID)
		{
			using (BancoDeDados bancoDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;

				switch (origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
						comando = bancoDados.CriarComando(@"select t.id, t.numero from {0}cre_cfo t where t.empreendimento = :empreendimentoID and t.situacao = 2", EsquemaBanco); break;
					case eDocumentoFitossanitarioTipo.CFOC:
						comando = bancoDados.CriarComando(@"select t.id, t.numero from {0}cre_cfoc t where t.empreendimento = :empreendimentoID and t.situacao =2", EsquemaBanco); break;
					case eDocumentoFitossanitarioTipo.PTV:
						comando = bancoDados.CriarComando(@"select t.id, t.numero from {0}tab_ptv t where t.empreendimento = :empreendimentoID and t.situacao = 2", EsquemaBanco); break;
				}

				comando.AdicionarParametroEntrada("empreendimentoID", empreendimentoID, DbType.Int32);

				List<Lista> retorno = null;

				using (IDataReader reader = bancoDados.ExecutarReader(comando))
				{
					retorno = new List<Lista>();
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("numero") });
					}

					reader.Close();
				}

				return retorno;
			}
		}

		internal List<Lista> ObterCultura(int origemTipo, long numeroOrigem)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<Lista> retorno = null;
				Comando comando = null;

				#region Case's

				switch (origemTipo)
				{
					case (int)eDocumentoFitossanitarioTipo.CFO: comando = bancoDeDados.CriarComando(@"select distinct c.id cultura_id, c.texto  cultura 
															  from {0}tab_cfo_produto              cp,
																   {0}ins_crt_unidade_prod_unidade i,
																	  tab_cultura                  c,
																	  tab_cultura_cultivar         cc,
																	  lov_crt_uni_prod_uni_medida  lu
																where i.id = cp.unidade_producao
																  and c.id = i.cultura
																  and cc.id = i.cultivar
																  and i.estimativa_unid_medida = lu.id
																  and cp.cfo = :numeroOrigem", UsuarioCredenciado); break;
					case (int)eDocumentoFitossanitarioTipo.CFOC: comando = bancoDeDados.CriarComando(@"select distinct c.id cultura_id, c.texto cultura                
																from {0}tab_cfoc_produto    cp,
																	{0}tab_lote             l,
																	{0}tab_lote_item        li,
																	{0}tab_cultura          c,
																	{0}tab_cultura_cultivar cc
																where l.id = cp.lote
																  and li.lote = l.id
																  and c.id = li.cultura
																  and cc.id = li.cultivar
																  and cp.cfoc = :numeroOrigem", UsuarioCredenciado); break;
					case (int)eDocumentoFitossanitarioTipo.PTV: comando = bancoDeDados.CriarComando(@"
																select distinct c.id cultura_id, c.texto cultura from tab_ptv_produto t, tab_cultura c
																where c.id = t.cultura and t.ptv = :numeroOrigem", UsuarioCredenciado); break;
					case (int)eDocumentoFitossanitarioTipo.PTVOutroEstado: comando = bancoDeDados.CriarComando(@"
																		   select distinct c.id cultura_id, c.texto cultura from tab_ptv_outrouf_produto t, {0}tab_cultura c
																	 	   where c.id = t.cultura and t.ptv = :numeroOrigem", UsuarioCredenciado); break;
					case (int)eDocumentoFitossanitarioTipo.CFCFR: comando = bancoDeDados.CriarComando(@"select distinct c.id cultura_id, c.texto  cultura 
															  from {0}tab_cfo_produto cp,
																   {0}ins_crt_unidade_prod_unidade i,
																   {0}tab_cultura                  c,
																   {0}tab_cultura_cultivar         cc,
																	  lov_crt_uni_prod_uni_medida  lu
																where i.id = cp.unidade_producao
																  and c.id = i.cultura
																  and cc.id = i.cultivar
																  and i.estimativa_unid_medida = lu.id
																  and cp.cfo = :numeroOrigem", UsuarioCredenciado); break;
					case (int)eDocumentoFitossanitarioTipo.TF: comando = bancoDeDados.CriarComando(@"select distinct c.id cultura_id, c.texto  cultura 
															  from {0}tab_cfo_produto cp,
																   {0}ins_crt_unidade_prod_unidade i,
																   {0}tab_cultura                  c,
																   {0}tab_cultura_cultivar         cc,
																	  lov_crt_uni_prod_uni_medida  lu
																where i.id = cp.unidade_producao
																  and c.id = i.cultura
																  and cc.id = i.cultivar
																  and i.estimativa_unid_medida = lu.id
																  and cp.cfo = :numeroOrigem", UsuarioCredenciado); break;

				}

				#endregion

				comando.AdicionarParametroEntrada("numeroOrigem", numeroOrigem, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<Lista>();
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("cultura_id"), Texto = reader.GetValue<string>("cultura") });
					}

					reader.Close();
				}

				return retorno;
			}
		}

		internal PTV ObterSemDocOrigem(PTV ptv)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"SELECT ID, EMPREENDIMENTO, ENDERECO, UF, MUNICIPIO, PRODUTOR, CPF_CNPJ, RESPONSAVEL
																FROM TAB_PTV_SEM_DOC_ORIGEM WHERE PTV = :ptv", EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv", ptv.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						ptv.SemDocOrigem.id = Convert.ToInt32(reader["ID"]);
						ptv.EmpreendimentoTexto = reader["EMPREENDIMENTO"].ToString();
						ptv.SemDocOrigem.enderecoEmpreendimento = reader["ENDERECO"].ToString();
						ptv.SemDocOrigem.ufEndereco = Convert.ToInt32(reader["UF"]);
						ptv.SemDocOrigem.municipioEndereco = Convert.ToInt32(reader["MUNICIPIO"]);
						ptv.SemDocOrigem.Produtor = reader["PRODUTOR"].ToString();
						ptv.SemDocOrigem.cpfCnpjProdutor = reader["CPF_CNPJ"].ToString();
						ptv.SemDocOrigem.responsavel = reader["RESPONSAVEL"].ToString();
					}
					reader.Close();
				}
			}

			return ptv;

		}
		#region Identificação Produto2

		internal List<Lista> ObterCultura()
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<Lista> retorno = null;
				Comando comando = comando = bancoDeDados.CriarComando(@"select c.id cultura_id, c.texto cultura from {0}tab_cultura c", EsquemaBanco);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<Lista>();
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("cultura_id"), Texto = reader.GetValue<string>("cultura") });
					}
					reader.Close();
				}
				return retorno;
			}
		}

		internal List<Lista> ObterCultivar(int culturaID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<Lista> retorno = null;
				Comando comando = comando = bancoDeDados.CriarComando(@"select cc.id, cc.cultivar from {0}tab_cultura_cultivar cc where cc.cultura = :culturaID", EsquemaBanco);

				comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int64);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<Lista>();
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("cultivar") });
					}
					reader.Close();
				}
				return retorno;
			}
		}

		#endregion

		internal List<Lista> ObterCultivar(eDocumentoFitossanitarioTipo origemTipo, Int64 origemID, int culturaID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<Lista> retorno = null;
				Comando comando = null;
				switch (origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
						comando = bancoDeDados.CriarComando(@"select cc.id, cc.cultivar, cc.nf_caixa_obrigatoria
															  from {0}tab_cfo_produto t, crt_unidade_producao_unidade u, tab_cultura_cultivar cc
															  where u.id = t.unidade_producao
															    and cc.id = u.cultivar
															    and u.cultura = :cultura
																and t.cfo = :origemID", UsuarioCredenciado);
						comando.AdicionarParametroEntrada("cultura", culturaID, DbType.Int32);
						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int64);
						break;

					case eDocumentoFitossanitarioTipo.CFOC:
						comando = bancoDeDados.CriarComando(@"select distinct cc.id, cc.cultivar, cc.nf_caixa_obrigatoria            
															  from {0}tab_cfoc_produto     cp,
																   {0}tab_lote             l,
																   {0}tab_lote_item        li,
																   {0}tab_cultura          c,
																   {0}tab_cultura_cultivar cc
															 where l.id = cp.lote
															   and li.lote = l.id
															   and c.id = li.cultura
															   and cc.id = li.cultivar
															   and cp.cfoc = :origemID 
															   and c.id = :culturaID", UsuarioCredenciado);
						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int64);
						comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
						break;
					case eDocumentoFitossanitarioTipo.PTV:
						comando = bancoDeDados.CriarComando(@"select distinct cc.id, cc.cultivar, cc.nf_caixa_obrigatoria
															from {0}tab_ptv_produto t, {1}tab_cultura_cultivar cc
															  where cc.id = t.cultivar and t.cultura = :culturaID and t.ptv = :origemID", EsquemaBanco, UsuarioCredenciado);
						comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int64);
						break;
					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						comando = bancoDeDados.CriarComando(@"select distinct cc.id, cc.cultivar, cc.nf_caixa_obrigatoria
															from {0}tab_ptv_outrouf_produto t, {1}tab_cultura_cultivar cc
															  where cc.id = t.cultivar and t.cultura = :culturaID and t.ptv = :origemID", EsquemaBanco, UsuarioCredenciado);
						comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int64);
						break;
					default: //Recebe como parâmetro o id da cultura: CF/CFR, FT
						comando = bancoDeDados.CriarComando(@"select t.id, t.cultivar, t.nf_caixa_obrigatoria
															from {0}tab_cultura_cultivar t where t.cultura = :culturaID", UsuarioCredenciado);
						comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
						break;
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<Lista>();
					while (reader.Read())
					{
						retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("cultivar"), Tipo = reader.GetValue<int>("nf_caixa_obrigatoria") });
					}

					reader.Close();
				}

				return retorno;
			}
		}

		internal List<Lista> ObterUnidadeMedida(eDocumentoFitossanitarioTipo origemTipo, Int64 origemID, int culturaID, int cultivarID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;

				switch (origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
						comando = bancoDeDados.CriarComando(@"
						select distinct lu.id, lu.texto unidade_medida, exibe_kilos from {0}tab_cfo_produto cp, crt_unidade_producao_unidade i, lov_crt_uni_prod_uni_medida  lu
						where i.id = cp.unidade_producao and i.estimativa_unid_medida = lu.id and i.cultivar = :cultivarID and cp.cfo = :origemID", UsuarioCredenciado);

						comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);
						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
						break;
					case eDocumentoFitossanitarioTipo.CFOC:
						comando = bancoDeDados.CriarComando(@"
						select distinct lu.id, lu.texto unidade_medida, pr.exibe_kilos from lov_crt_uni_prod_uni_medida lu, cred_cfoc_produto pr, cred_lote_item li
						where lu.id = li.unidade_medida and li.lote = pr.lote and pr.cfoc = :origemID and li.cultivar = :cultivarID", EsquemaBanco);

						comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);
						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
						break;
					case eDocumentoFitossanitarioTipo.PTV:
						comando = bancoDeDados.CriarComando(@"
						select distinct lu.id, lu.texto unidade_medida,pr.exibe_kilos from lov_crt_uni_prod_uni_medida lu, tab_ptv_produto pr
						where lu.id = pr.unidade_medida and pr.ptv = :origemID and pr.cultura = :culturaID and pr.cultivar = :cultivarID", EsquemaBanco);

						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
						comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
						comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);
						break;
					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						comando = bancoDeDados.CriarComando(@"
						select distinct lu.id, lu.texto unidade_medida, 0 as exibe_kilos from lov_crt_uni_prod_uni_medida lu, {0}tab_ptv_outrouf_produto t
						where lu.id = t.unidade_medida and t.ptv = :origemID and t.cultura = :culturaID and t.cultivar = :cultivarID", EsquemaBanco);

						comando.AdicionarParametroEntrada("origemID", origemID, DbType.Int32);
						comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);
						comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);
						break;
					default://Recebe como parâmetro o id da cultura: PTV de outro Estado, CF/CFR, FT
						comando = bancoDeDados.CriarComando(@"
						select lu.id, lu.texto unidade_medida from lov_crt_uni_prod_uni_medida lu", UsuarioCredenciado);
						break;
				}

				List<Lista> retorno = null;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<Lista>();
					while (reader.Read())
					{
                        if ( (retorno.Count == 0) || 
                             (retorno.Count > 0 && !retorno.Any(x => x.Texto == reader.GetValue<string>("unidade_medida"))) )
						    retorno.Add(new Lista() { Id = reader.GetValue<string>("id"), Texto = reader.GetValue<string>("unidade_medida") });
					}

					reader.Close();
				}
				return retorno;
			}
		}

		/// <summary>
		///	Retorna a quantidade da origem	
		/// </summary>
		/// <param name="origemTipo"></param>
		/// <param name="origemID"></param>
		/// <returns></returns>
		internal decimal ObterOrigemQuantidade(eDocumentoFitossanitarioTipo origemTipo, int origemID, string origemNumero, int cultivarID, int unidadeMedida, int ptv)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				string query = string.Empty;

				switch (origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
					case eDocumentoFitossanitarioTipo.CFOC:
					case eDocumentoFitossanitarioTipo.PTV:
						query = @"
						select (
						/*LOTE*/
						nvl((select sum(i.quantidade) quantidade
						from tab_lote t, tab_lote_item i
						where i.lote = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem = :origem
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida), 0)
						+
						/*EPTV*/
						nvl((select sum(i.quantidade) quantidade
						from tab_ptv t, tab_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem = :origem
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and t.situacao != 3), 0)
						+
						/*PTV*/
						nvl((select sum(i.quantidade) quantidade
						from ins_ptv t, ins_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem = :origem
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and t.situacao != 3
						and t.id != :ptv), 0)) saldo_utilizado from dual";
						break;
					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						query = @"
						select (
						/*LOTE*/
						nvl((select sum(i.quantidade) quantidade
						from tab_lote t, tab_lote_item i
						where i.lote = t.id
						and i.origem_tipo = :origem_tipo
						and i.origem_numero = :origem_numero
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida), 0)
						+
						/*EPTV*/
						nvl((select sum(i.quantidade) quantidade
						from tab_ptv t, tab_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.numero_origem = :origem_numero
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and t.situacao != 3), 0)
						+
						/*PTV*/
						nvl((select sum(i.quantidade) quantidade
						from ins_ptv t, ins_ptv_produto i
						where i.ptv = t.id
						and i.origem_tipo = :origem_tipo
						and i.numero_origem = :origem_numero
						and i.cultivar = :cultivar
						and i.unidade_medida = :unidade_medida
						and t.situacao != 3
						and t.id != :ptv), 0)) saldo_utilizado from dual";
						break;
				}

				Comando comando = bancoDeDados.CriarComando(query, UsuarioCredenciado);

				comando.AdicionarParametroEntrada("ptv", ptv, DbType.Int32);
				comando.AdicionarParametroEntrada("origem_tipo", (int)origemTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", cultivarID, DbType.Int32);
				comando.AdicionarParametroEntrada("unidade_medida", unidadeMedida, DbType.Int32);

				switch (origemTipo)
				{
					case eDocumentoFitossanitarioTipo.CFO:
					case eDocumentoFitossanitarioTipo.CFOC:
					case eDocumentoFitossanitarioTipo.PTV:
						comando.AdicionarParametroEntrada("origem", origemID, DbType.Int32);
						break;
					case eDocumentoFitossanitarioTipo.PTVOutroEstado:
						comando.AdicionarParametroEntrada("origem_numero", origemNumero, DbType.String);
						break;
				}

				return Convert.ToDecimal(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal List<PTVProduto> ObterOrigens(List<PTVProduto> origens)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<PTVProduto> listaOrigem = new List<PTVProduto>();

				origens.ForEach(x =>
				{
					if (x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV)
					{
						Comando comando = bancoDeDados.CriarComando(@"select t.origem, t.origem_tipo from {0}tab_ptv_produto t where t.ptv = :Origem", EsquemaBanco);
						comando.AdicionarParametroEntrada("Origem", x.Origem, DbType.Int32);

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							while (reader.Read())
							{
								listaOrigem.Add(new PTVProduto() { Origem = reader.GetValue<int>("origem"), OrigemTipo = reader.GetValue<int>("origem_tipo") });
							}

							reader.Close();
						}

						if (listaOrigem.Any(y => y.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV))
						{
							listaOrigem.AddRange(ObterOrigens(listaOrigem.Where(y => y.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV).ToList()));
						}
					}
					else
					{
						listaOrigem.Add(new PTVProduto() { Origem = x.Origem, OrigemTipo = x.OrigemTipo });
					}
				});

				return listaOrigem.GroupBy(x => new { x.OrigemTipo, x.Origem }).Select(g => g.First()).ToList();
			}
		}

		internal List<LaudoLaboratorial> ObterLaudoLaboratorial(List<PTVProduto> origens, bool recursivo = false)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				List<PTVProduto> listaOrigem = ObterOrigens(origens);
				List<LaudoLaboratorial> retorno = new List<LaudoLaboratorial>();
				Comando comando = null;

				if (listaOrigem.Exists(x => x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO))
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.nome_laboratorio, t.numero_laudo_resultado_analise, t.estado, e.sigla estado_texto, t.municipio, m.texto municipio_texto
					from tab_cfo t, lov_estado e, lov_municipio m where e.id = t.estado and m.id = t.municipio and t.possui_laudo_laboratorial > 0 ", UsuarioCredenciado);
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "t.id", DbType.Int32, listaOrigem.Where(x => x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO).Select(x => x.Origem).ToList());

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							retorno.Add(new LaudoLaboratorial()
							{
								Id = reader.GetValue<int>("id"),
								Nome = reader.GetValue<string>("nome_laboratorio"),
								LaudoResultadoAnalise = reader.GetValue<string>("numero_laudo_resultado_analise"),
								Estado = reader.GetValue<int>("estado"),
								EstadoTexto = reader.GetValue<string>("estado_texto"),
								Municipio = reader.GetValue<int>("municipio"),
								MunicipioTexto = reader.GetValue<string>("municipio_texto")
							});
						}

						reader.Close();
					}
				}

				if (listaOrigem.Exists(x => x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC))
				{
					comando = bancoDeDados.CriarComando(@"select t.id, t.nome_laboratorio, t.numero_laudo_resultado_analise, t.estado, e.sigla estado_texto, t.municipio, m.texto municipio_texto
					from tab_cfoc t, lov_estado e, lov_municipio m where e.id = t.estado and m.id = t.municipio and t.possui_laudo_laboratorial > 0 ", UsuarioCredenciado);
					comando.DbCommand.CommandText += comando.AdicionarIn("and", "t.id", DbType.Int32, listaOrigem.Where(x => x.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC).Select(x => x.Origem).ToList());

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							retorno.Add(new LaudoLaboratorial()
							{
								Id = reader.GetValue<int>("id"),
								Nome = reader.GetValue<string>("nome_laboratorio"),
								LaudoResultadoAnalise = reader.GetValue<string>("numero_laudo_resultado_analise"),
								Estado = reader.GetValue<int>("estado"),
								EstadoTexto = reader.GetValue<string>("estado_texto"),
								Municipio = reader.GetValue<int>("municipio"),
								MunicipioTexto = reader.GetValue<string>("municipio_texto")
							});
						}

						reader.Close();
					}
				}

				return retorno;
			}
		}

		/// <summary>
		///	Método recursivo que busca os tratamentos Fitussanitários dos produtos relacionados, 
		///	trazendo os 5 primeiros de acordo com a ordem em que foram adicionados
		/// </summary>
		/// <param name="origens"></param>
		/// <returns>List<TratamentoFitossanitario></returns>
		/// 
		internal List<TratamentoFitossanitario> TratamentoFitossanitario(List<PTVProduto> origens)
		{
			List<TratamentoFitossanitario> retorno = new List<TratamentoFitossanitario>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				retorno = TratamentoFitossanitario(origens, bancoDeDados);
			}

			return retorno.GroupBy(p => new { p.ProdutoComercial, p.IngredienteAtivo, p.Dose, p.PragaProduto, p.ModoAplicacao }).Select(g => g.First()).ToList();
		}
		internal List<TratamentoFitossanitario> TratamentoFitossanitario(List<PTVProduto> origens, BancoDeDados bancoDeDados)
		{
			List<TratamentoFitossanitario> retorno = new List<TratamentoFitossanitario>();
			Comando comando = null;
			string parametro_in = string.Empty;
			int limiteTratamentosFitossanitarios = 5;

			foreach (var item in origens)
			{

				switch (item.OrigemTipo)
				{
					case (int)eDocumentoFitossanitarioTipo.CFO:
						#region Buscar tratamento CFO

						using (BancoDeDados bancoDeDadosCred = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							comando = bancoDeDadosCred.CriarComando(@"select t1.id,t1.cfo as cfo_cfoc,t1.produto_comercial,t1.ingrediente_ativo,t1.dose,t1.praga_produto,t1.modo_aplicacao
                            from {0}tab_cfo_trata_fitossa t1 where t1.cfo = :cfoID order by t1.id", UsuarioCredenciado);
							comando.AdicionarParametroEntrada("cfoID", item.Origem, DbType.Int32);

							using (IDataReader reader = bancoDeDadosCred.ExecutarReader(comando))
							{
								while (reader.Read())
								{
									retorno.Add(new TratamentoFitossanitario()
									{
										Id = reader.GetValue<int>("id"),
										ProdutoComercial = reader.GetValue<string>("produto_comercial"),
										IngredienteAtivo = reader.GetValue<string>("ingrediente_ativo"),
										Dose = reader.GetValue<decimal>("dose"),
										PragaProduto = reader.GetValue<string>("praga_produto"),
										ModoAplicacao = reader.GetValue<string>("modo_aplicacao")
									});
									if (retorno.Count == limiteTratamentosFitossanitarios)
									{
										break;
									}
								}
								reader.Close();
							}
						}
						#endregion
						break;
					case (int)eDocumentoFitossanitarioTipo.CFOC:
						#region Buscar tratamento CFOC

						using (BancoDeDados bancoDeDadosCred = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							comando = bancoDeDadosCred.CriarComando(@"select t1.id,t1.cfoc as cfo_cfoc,t1.produto_comercial,t1.ingrediente_ativo,t1.dose,t1.praga_produto,t1.modo_aplicacao 
                            from {0}tab_cfoc_trata_fitossa t1 where t1.cfoc = :cfocID order by t1.id", UsuarioCredenciado);
							comando.AdicionarParametroEntrada("cfocID", item.Origem, DbType.Int32);

							using (IDataReader reader = bancoDeDadosCred.ExecutarReader(comando))
							{
								while (reader.Read())
								{
									retorno.Add(new TratamentoFitossanitario()
									{
										Id = reader.GetValue<int>("id"),
										ProdutoComercial = reader.GetValue<string>("produto_comercial"),
										IngredienteAtivo = reader.GetValue<string>("ingrediente_ativo"),
										Dose = reader.GetValue<decimal>("dose"),
										PragaProduto = reader.GetValue<string>("praga_produto"),
										ModoAplicacao = reader.GetValue<string>("modo_aplicacao")
									});
									if (retorno.Count == limiteTratamentosFitossanitarios)
									{
										break;
									}
								}
								reader.Close();
							}
						}
						#endregion
						break;
					case (int)eDocumentoFitossanitarioTipo.PTV:
						#region Buscar tratamento PTV

						comando = bancoDeDados.CriarComando(@"select t.origem, t.origem_tipo, t.cultura, t.cultivar 
                            from {0}tab_ptv_produto t where t.ptv = :origemID and t.cultura = :culturaID and t.cultivar = :cultivarID order by t.id", EsquemaBanco);
						comando.AdicionarParametroEntrada("origemID", item.Origem, DbType.Int32);
						comando.AdicionarParametroEntrada("culturaID", item.Cultura, DbType.Int32);
						comando.AdicionarParametroEntrada("cultivarID", item.Cultivar, DbType.Int32);

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							while (reader.Read())
							{
								PTVProduto procuto = new PTVProduto
								{
									Origem = reader.GetValue<int>("origem"),
									OrigemTipo = reader.GetValue<int>("origem_tipo"),
									Cultura = reader.GetValue<int>("cultura"),
									Cultivar = reader.GetValue<int>("cultivar")
								};
								List<TratamentoFitossanitario> tratamentos = TratamentoFitossanitario(new List<PTVProduto> { procuto }, bancoDeDados);

								if (tratamentos != null && tratamentos.Count > 0)
								{
									foreach (var tratamento in tratamentos)
									{
										retorno.Add(tratamento);
										if (retorno.Count == limiteTratamentosFitossanitarios)
										{
											break;
										}
									}
								}
								if (retorno.Count == limiteTratamentosFitossanitarios)
								{
									break;
								}
							}
							reader.Close();
						}
						#endregion
						break;
				}

				if (retorno != null && retorno.Count == limiteTratamentosFitossanitarios)
				{
					break;
				}
			}
			return retorno;
		}

		internal List<ListaValor> ObterResponsavelTecnico(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select f.id, f.nome from {0}tab_hab_emi_ptv_operador t, {0}tab_hab_emi_ptv o, {0}tab_funcionario f 
				where t.habilitacao(+) = o.id and f.id = o.funcionario and (o.funcionario = :id or t.funcionario = :id) and rownum = 1", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				List<ListaValor> retorno = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<ListaValor>();
					while (reader.Read())
					{
						retorno.Add(new ListaValor() { Id = reader.GetValue<int>("id"), Texto = reader.GetValue<string>("nome") });
					}

					reader.Close();
				}

				return retorno;
			}
		}

		internal List<ListaValor> ObterLocalEmissao(int usuarioId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
			    select distinct m.id, m.texto from {0}tab_funcionario_setor f, {0}tab_setor s, {0}tab_setor_endereco se, lov_municipio m
			    where f.setor = s.id and s.id = se.setor and (m.id = se.municipio and m.ativo = 1) and f.funcionario = :usuarioId order by m.id", EsquemaBanco);

				comando.AdicionarParametroEntrada("usuarioId", usuarioId, DbType.Int32);

				List<ListaValor> retorno = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<ListaValor>();
					while (reader.Read())
					{
						retorno.Add(new ListaValor() { Id = reader.GetValue<int>("id"), Texto = reader.GetValue<string>("texto") });
					}
					reader.Close();
				}

				return retorno;
			}
		}

		internal Resultados<PTVListarResultado> Filtrar(Filtro<PTVListarFiltro> filtro)
		{
			Resultados<PTVListarResultado> retorno = new Resultados<PTVListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");
				string tabelaTipoDoc = String.Empty;
				string amarracaoTipoDoc = String.Empty;

				#region Adicionando Filtros

				comandtxt += comando.FiltroAndLike("pt.numero", "numero", filtro.Dados.Numero);

				if (!String.IsNullOrEmpty(filtro.Dados.Empreendimento))
				{
					comandtxt += comando.FiltroAndLike("em.denominador", "nome_fantasia", filtro.Dados.Empreendimento, true, true);
				}
				if (filtro.Dados.Situacao > 0)
				{
					comandtxt += comando.FiltroAnd("pt.situacao", "situacao", filtro.Dados.Situacao);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.Destinatario))
				{
					comandtxt += comando.FiltroAndLike("d.nome", "nome", filtro.Dados.Destinatario, true, true);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.CulturaCultivar))
				{
					comandtxt += comando.FiltroAndLike("c.texto||'/'||cc.cultivar", "cultura_cultivar", filtro.Dados.CulturaCultivar, true, true);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.Interessado))
				{
					var consulta = "(SELECT COALESCE(P.NOME, P.RAZAO_SOCIAL, PT.RESPONSAVEL_SEM_DOC) FROM TAB_PESSOA P WHERE P.ID = PT.RESPONSAVEL_EMP)";
					comandtxt += comando.FiltroAndLike(consulta, "interessado", filtro.Dados.Interessado, likeInicio: true);
				}
				if (filtro.Dados.TipoDocumento > 0)
				{
					comandtxt += comando.FiltroAnd("pr.origem_tipo", "tipoDocumento", filtro.Dados.TipoDocumento);
					var consulta = String.Empty;
					if (!String.IsNullOrEmpty(filtro.Dados.NumeroDocumento))
					{
						switch (filtro.Dados.TipoDocumento)
						{
							case (int)eDocumentoFitossanitarioTipo.CFO:
								consulta = "(SELECT case when CFO.serie is null then to_char(CFO.numero) else CFO.numero||'/'||CFO.serie end FROM IDAFCREDENCIADO.TAB_CFO CFO WHERE pr.origem = CFO.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;

							case (int)eDocumentoFitossanitarioTipo.CFOC:
								consulta = "(SELECT case when CFOC.serie is null then to_char(CFOC.numero) else CFOC.numero||'/'||CFOC.serie end FROM IDAFCREDENCIADO.TAB_CFOC CFOC WHERE pr.origem = CFOC.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;

							case (int)eDocumentoFitossanitarioTipo.PTV:
								consulta = "(SELECT PTV.numero FROM TAB_PTV PTV WHERE pr.origem = PTV.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;

							case (int)eDocumentoFitossanitarioTipo.PTVOutroEstado:
								consulta = "(SELECT PUF.NUMERO  FROM TAB_PTV_OUTROUF PUF WHERE pr.origem = PUF.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento, null, true);
								break;
							case (int)eDocumentoFitossanitarioTipo.CFCFR:
								comandtxt += comando.FiltroAnd("pr.numero_origem", "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;
							case (int)eDocumentoFitossanitarioTipo.TF:
								comandtxt += comando.FiltroAnd("pr.numero_origem", "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;
						}
					}
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "empreendimento", "situacao", "cultura_cultivar" };

				if (filtro.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText =
				"select count(*) from (" + String.Format(@"select pt.id
														from {0}tab_ptv pt,{0}tab_ptv_produto pr,{0}tab_empreendimento em,{0}lov_ptv_situacao st,{0}tab_cultura c,{0}tab_cultura_cultivar cc,{0}tab_destinatario_ptv d
														where pt.id(+) = pr.ptv
															and em.id(+) = pt.empreendimento 
															and st.id = pt.situacao 
															and c.id = pr.cultura
															and cc.id = pr.cultivar 
															and d.id = pt.destinatario
															and pt.eptv_id is null " + comandtxt + " group by pt.id) a ", esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				comandtxt = String.Format(@"select 
												pt.id,
												pt.numero,
												pt.tipo_numero,
												nvl(em.denominador, pt.empreendimento_sem_doc) as empreendimento,
												pt.situacao,
												st.texto as situacao_texto,
												pt.responsavel_tecnico,
												stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar
											from {0}tab_ptv pt, {0}tab_ptv_produto pr, {0}tab_empreendimento em, {0}lov_ptv_situacao st, {0}tab_cultura c, {0}tab_cultura_cultivar cc,{0}tab_destinatario_ptv d
											where pt.id(+) = pr.ptv
												and em.id(+) = pt.empreendimento
												and st.id = pt.situacao
												and c.id = pr.cultura
												and cc.id = pr.cultivar 
												and d.id = pt.destinatario
												and pt.eptv_id is null " + comandtxt + " group by pt.id, pt.numero, pt.tipo_numero, nvl(em.denominador, pt.empreendimento_sem_doc), pt.situacao, st.texto, pt.responsavel_tecnico " + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);
				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PTVListarResultado item;
					while (reader.Read())
					{
						item = new PTVListarResultado();
						item.ID = reader.GetValue<int>("id");
						item.Numero = reader.GetValue<string>("numero");
						item.NumeroTipo = reader.GetValue<int>("tipo_numero");
						item.Empreendimento = reader.GetValue<string>("empreendimento");
						item.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						item.Situacao = reader.GetValue<int>("situacao");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.ResponsavelTecnicoId = reader.GetValue<int>("responsavel_tecnico");

						retorno.Itens.Add(item);
					}
					reader.Close();
				}
			}
			return retorno;
		}

		internal Resultados<PTVListarResultado> FiltrarEPTV(Filtro<PTVListarFiltro> filtro)
		{
			Resultados<PTVListarResultado> retorno = new Resultados<PTVListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				//comandtxt += comando.FiltroAnd("pt.credenciado", "credenciado", User.FuncionarioId);

				comandtxt += comando.FiltroAndLike("pt.numero", "numero", filtro.Dados.Numero);

				comandtxt += comando.FiltroAndLike("pt.dua_numero", "dua_numero", filtro.Dados.DUANumero);

				comandtxt += comando.FiltroAndLike("pt.dua_cpf_cnpj", "dua_cpf_cnpj", filtro.Dados.DUACPFCNPJ);


				if (filtro.Dados.FuncionarioId > 0)
				{
					comandtxt += " and exists ( select 1 from tab_funcionario_setor s where s.setor = pt.local_vistoria and s.funcionario = :funcionario) ";


					comando.AdicionarParametroEntrada("funcionario", filtro.Dados.FuncionarioId);
				}

				if (!String.IsNullOrEmpty(filtro.Dados.Empreendimento))
				{
					comandtxt += comando.FiltroAndLike("nvl(em.denominador,pt.empreendimento_sem_doc)", "nome_fantasia", filtro.Dados.Empreendimento, true, true);
				}
				if (filtro.Dados.Situacao > 0)
				{
					if(filtro.Dados.Situacao == 3) //Válido
						comandtxt += comando.FiltroAnd("(case when pt.situacao = 3 then (select lps.texto from ins_ptv ip, lov_ptv_situacao lps where ip.situacao = lps.id and ip.eptv_id = pt.id) else st.texto end)", "situacao", "Válido");
					else if (filtro.Dados.Situacao == 7) //Inválido
						comandtxt += comando.FiltroAnd("(case when pt.situacao = 3 then (select lps.texto from ins_ptv ip, lov_ptv_situacao lps where ip.situacao = lps.id and ip.eptv_id = pt.id) else st.texto end)", "situacao", "Inválido");
					else
						comandtxt += comando.FiltroAnd("pt.situacao", "situacao", filtro.Dados.Situacao);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.Destinatario))
				{
					comandtxt += comando.FiltroAndLike("d.nome", "nome", filtro.Dados.Destinatario, true, true);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.CulturaCultivar))
				{
					comandtxt += comando.FiltroAndLike("c.texto||'/'||cc.cultivar", "cultura_cultivar", filtro.Dados.CulturaCultivar, true, true);
				}
				if (!String.IsNullOrEmpty(filtro.Dados.Interessado))
				{
					var consulta = "(SELECT COALESCE(P.NOME, P.RAZAO_SOCIAL, PT.RESPONSAVEL_SEM_DOC) FROM IDAF.TAB_PESSOA P WHERE P.ID = PT.RESPONSAVEL_EMP)";
					comandtxt += comando.FiltroAndLike(consulta, "interessado", filtro.Dados.Interessado, likeInicio: true);
				}
				if (filtro.Dados.TipoDocumento > 0)
				{
					comandtxt += comando.FiltroAnd("pr.origem_tipo", "tipoDocumento", filtro.Dados.TipoDocumento);
					if (!String.IsNullOrEmpty(filtro.Dados.NumeroDocumento))
					{
						var consulta = String.Empty;
						switch (filtro.Dados.TipoDocumento)
						{
							case (int)eDocumentoFitossanitarioTipo.CFO:
								consulta = "(SELECT case when CFO.serie is null then to_char(CFO.numero) else CFO.numero||'/'||CFO.serie end FROM IDAFCREDENCIADO.TAB_CFO CFO WHERE pr.origem = CFO.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;

							case (int)eDocumentoFitossanitarioTipo.CFOC:
								consulta = "(SELECT case when CFOC.serie is null then to_char(CFOC.numero) else CFOC.numero||'/'||CFOC.serie end FROM IDAFCREDENCIADO.TAB_CFOC CFOC WHERE pr.origem = CFOC.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;

							case (int)eDocumentoFitossanitarioTipo.PTV:
								consulta = "(SELECT PTV.NUMERO FROM IDAF.TAB_PTV PTV WHERE pr.origem = PTV.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;

							case (int)eDocumentoFitossanitarioTipo.PTVOutroEstado:
								consulta = "(SELECT PUF.NUMERO  FROM TAB_PTV_OUTROUF PUF WHERE pr.origem = PUF.ID)";
								comandtxt += comando.FiltroAnd(consulta, "numeroDocOrigem", filtro.Dados.NumeroDocumento);
								break;
						}
					}
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "empreendimento", "situacao", "cultura_cultivar" };

				if (filtro.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("numero");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText =
				"select count(*) from (" + String.Format(@"select pt.id
														from {0}tab_ptv pt,{0}tab_ptv_produto pr,{0}ins_empreendimento em,{0}lov_solicitacao_ptv_situacao st,{0}tab_cultura c,{0}tab_cultura_cultivar cc,{0}tab_destinatario_ptv d
														where pt.id(+) = pr.ptv
															and em.id(+) = pt.empreendimento 
															and st.id = pt.situacao 
															and c.id = pr.cultura
															and cc.id = pr.cultivar 
															and d.id = pt.destinatario
															and pt.situacao > 1 " + comandtxt + " group by pt.id) a ", esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				comandtxt = String.Format(@"select 
												pt.id,
												pt.numero,
												pt.tipo_numero,
												nvl(em.denominador, pt.empreendimento_sem_doc) as empreendimento,
											    pt.situacao,
												(case when pt.situacao = 3 then (select lps.texto from ins_ptv ip, lov_ptv_situacao lps where ip.situacao = lps.id and ip.eptv_id = pt.id) else st.texto end) as situacao_texto,
												pt.responsavel_tecnico,
												stragg(c.texto || '/' || trim(cc.cultivar)) as cultura_cultivar
											from {0}tab_ptv pt, {0}tab_ptv_produto pr, {0}ins_empreendimento em, {0}lov_solicitacao_ptv_situacao st, {0}tab_cultura c, {0}tab_cultura_cultivar cc,{0}tab_destinatario_ptv d
											where pt.id(+) = pr.ptv
											  and em.id(+) = pt.empreendimento
											  and st.id = pt.situacao
											  and c.id = pr.cultura
											  and cc.id = pr.cultivar 											  
                                              and pt.situacao > 1
										      and d.id = pt.destinatario " + comandtxt + " group by pt.id, pt.numero, pt.tipo_numero, nvl(em.denominador, pt.empreendimento_sem_doc), pt.situacao, st.texto, pt.responsavel_tecnico " + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);
				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PTVListarResultado item;
					while (reader.Read())
					{
						item = new PTVListarResultado();
						item.ID = reader.GetValue<int>("id");
						item.Numero = reader.GetValue<string>("numero");
						item.NumeroTipo = reader.GetValue<int>("tipo_numero");
						item.Empreendimento = reader.GetValue<string>("empreendimento");
						item.CulturaCultivar = reader.GetValue<string>("cultura_cultivar");
						item.Situacao = reader.GetValue<int>("situacao");
						item.SituacaoTexto = reader.GetValue<string>("situacao_texto");
						item.ResponsavelTecnicoId = reader.GetValue<int>("responsavel_tecnico");

						retorno.Itens.Add(item);
					}
					reader.Close();
				}
			}
			return retorno;
		}

		internal List<ListaValor> DiasHorasVistoriaEPTV(int ptvId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
									SELECT lv.id,
									       (d.texto || '(' || Ptv.Data_Vistoria || ') de ' || lv.hora_inicio || ' a ' || lv.hora_fim) texto
									from {0}Cnf_Local_Vistoria lv,
									     {1}lov_dia_semana d,
									     {1}tab_ptv ptv
									where lv.id = ptv.data_hora_vistoria
									      and d.id = lv.dia_semana
									      and ptv.id = :id", EsquemaBanco, UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", ptvId, DbType.Int32);

				List<ListaValor> retorno = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<ListaValor>();
					while (reader.Read())
					{
						retorno.Add(new ListaValor()
						{
							Id = reader.GetValue<int>("id"),
							Texto = reader.GetValue<string>("texto")
						});
					}
					reader.Close();
				}

				return retorno;
			}
		}

		internal List<ListaValor> DiasHorasVistoria(int setorId, DateTime? dataVistoria = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"SELECT COUNT(*) FROM cnf_local_vistoria_bloqueio WHERE setor = :setor", EsquemaBanco);
				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);

				int totRegistros = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));


				if (totRegistros > 0)
				{
					comando = bancoDeDados.CriarComando(@"
								SELECT lv.id,
								       d.texto || '(' || workday || ') de ' || lv.hora_inicio || ' a ' || lv.hora_fim   texto,
								       dia_semana,
								       lv.hora_inicio,
								       lv.hora_fim
								FROM ( SELECT TRUNC(to_date(:dataVistoria,'DD/MM/YY HH24:MI'), 'mm') + LEVEL - 1 workday
								       FROM DUAL
								            CONNECT BY TRUNC(to_date(:dataVistoria,'DD/MM/YY HH24:MI'), 'yy') + LEVEL - 1 <= LAST_DAY(to_date(:dataVistoria,'DD/MM/YY HH24:MI')) ),
								            cnf_local_vistoria lv,
								            lov_dia_semana d
								WHERE  TO_CHAR(workday,'D') = lv.dia_semana
								       AND lv.setor=:setor
								       and lv.situacao = 1
								       AND d.id = lv.dia_semana
								       AND to_timestamp(to_char(workday || ' ' || lv.hora_fim)) >= to_timestamp(to_char(:dataVistoria,'DD/MM/YY HH24:MI'))
								       AND not exists ( SELECT clvb.id
								                        FROM cnf_local_vistoria_bloqueio clvb
								                        WHERE clvb.setor = :setor
								                              and to_timestamp(workday || ' ' || lv.hora_inicio, 'dd/mm/yy hh24:mi') >= to_timestamp(nvl(to_char(clvb.dia_inicio, 'DD/MM/YY HH24:MI'), '01/01/01 00:00'))
								                              and to_timestamp(workday || ' ' || lv.hora_fim, 'dd/mm/yy hh24:mi') <= to_timestamp(nvl(to_char(clvb.dia_fim, 'DD/MM/YY HH24:MI'), '01/01/01 00:00')) )");



				}
				else
				{
					comando = bancoDeDados.CriarComando(@"
								SELECT lv.id,
								       (d.texto || '(' || workday || ') de ' || lv.hora_inicio || ' a ' || lv.hora_fim) texto,
								       dia_semana,
								       lv.hora_inicio,
								       lv.hora_fim
								FROM ( SELECT TRUNC(to_date(:dataVistoria,'DD/MM/YY HH24:MI'), 'mm') + LEVEL - 1 workday
								       FROM DUAL
								            CONNECT BY TRUNC(to_date(:dataVistoria,'DD/MM/YY HH24:MI'), 'yy') + LEVEL - 1 <= LAST_DAY(to_date(:dataVistoria,'DD/MM/YY HH24:MI'))),
								            cnf_local_vistoria lv,
								            lov_dia_semana d
								       WHERE  TO_CHAR(workday,'D') = lv.dia_semana
								              and lv.setor=:setor
											  and lv.situacao = 1
								              and d.id = lv.dia_semana
								              and to_timestamp(to_char(workday || ' ' || lv.hora_fim)) >= to_timestamp(to_char(:dataVistoria,'DD/MM/YY HH24:MI'))");
				}
				if (dataVistoria == null || dataVistoria > DateTime.Now) dataVistoria = DateTime.Now.AddHours(1);

				comando.AdicionarParametroEntrada("setor", setorId, DbType.Int32);
				comando.AdicionarParametroEntrada("dataVistoria", dataVistoria, DbType.DateTime);

				List<ListaValor> retorno = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					retorno = new List<ListaValor>();
					while (reader.Read())
					{
						retorno.Add(new ListaValor() { Id = reader.GetValue<int>("id"), Texto = reader.GetValue<string>("texto") });
					}
					reader.Close();
				}

				return retorno;
			}
		}

		internal List<String> ObterDeclaracaoAdicional(int origem, int origemTipo, int tipoProducaoID, int cultivarID, bool isFormatado)
		{
			List<String> retorno = new List<String>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				retorno = ObterDeclaracaoAdicional(origem, origemTipo, tipoProducaoID, cultivarID, isFormatado, bancoDeDados).Distinct().ToList();
			}
			return retorno;
		}

        private Dictionary<int, int> BuscaTipoDocOrigemCFOC(int origem, int Cultivar, BancoDeDados bancoDeDados = null)
        {
            int OrigemTipo = 0;
            Dictionary<int, int> lstTipoDocOrigem = new Dictionary<int, int>();

            using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
            {

                do
                {
                    string cmdSql = @"select distinct origem_tipo, origem
                                      from tab_cfoc cfoc, tab_cfoc_produto cp, tab_lote_item hli
                                       where cfoc.id = cp.cfoc and cp.lote = hli.lote and cfoc.id = {0} and hli.cultivar = {1} ";

                    cmdSql = string.Format(cmdSql, origem, Cultivar);

                    Comando comandoCred = bancoDeDadosCredenciado.CriarComando(cmdSql);


                    using (IDataReader reader = bancoDeDadosCredenciado.ExecutarReader(comandoCred))
                    {
                        while (reader.Read())
                        {
                            OrigemTipo = reader.GetValue<int>("origem_tipo");
                            origem = reader.GetValue<int>("origem");
                            if (!lstTipoDocOrigem.ContainsKey(OrigemTipo))
                                lstTipoDocOrigem.Add(OrigemTipo, origem);
                        }

                        reader.Close();
                    }

                } while (OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC);

            }

            return lstTipoDocOrigem;

        }

		private List<String> ObterDeclaracaoAdicional(int origem, int origemTipo, int tipoProducaoID, int cultivarID, bool isFormatado, BancoDeDados bancoDeDados)
		{
			Comando comando = null;
			Comando comandoCred = null;
			List<String> retorno = new List<String>();
			var colunaDeclaracaoAdicional = isFormatado ? "texto_formatado" : "texto";
			switch (origemTipo)
			{
				case (int)eDocumentoFitossanitarioTipo.CFO:
					#region Buscar tratamento CFO

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(UsuarioCredenciado))
					{
						comandoCred = bancoDeDadosCredenciado.CriarComando(@"select distinct lcda."+colunaDeclaracaoAdicional+@" DeclaracaoAdicionalTexto from tab_cfo cfo, tab_cfo_produto cp,       
                            ins_crt_unidade_prod_unidade i, tab_cultivar_configuracao cconf, lov_cultivar_declara_adicional lcda, tab_cfo_praga tcp where cfo.id = cp.cfo 
                            and cp.unidade_producao = i.id and i.cultivar = cconf.cultivar and cconf.declaracao_adicional = lcda.id and cfo.id = tcp.cfo and tcp.praga = cconf.praga
                            and cfo.id = :cfoId and cconf.tipo_producao = :tipoProducaoID and lcda.outro_estado='0' and i.cultivar = :cultivarID", UsuarioCredenciado);

						comandoCred.AdicionarParametroEntrada("cfoId", origem, DbType.Int32);
						comandoCred.AdicionarParametroEntrada("tipoProducaoID", tipoProducaoID, DbType.Int32);
						comandoCred.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

						using (IDataReader reader = bancoDeDadosCredenciado.ExecutarReader(comandoCred))
						{
							while (reader.Read())
							{
								retorno.Add(reader.GetValue<string>("DeclaracaoAdicionalTexto"));

							}
							reader.Close();
						}
					}
					#endregion
					break;
				case (int)eDocumentoFitossanitarioTipo.CFOC:
					#region Buscar tratamento CFOC

					 Dictionary<int,int> lstTipoDocOrigem = BuscaTipoDocOrigemCFOC(origem,cultivarID, bancoDeDados);


                    foreach (KeyValuePair<int, int> kv in lstTipoDocOrigem)
                    {
                        if (kv.Key == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)
                        {
                            using (BancoDeDados bancoDeDadosCred = BancoDeDados.ObterInstancia(UsuarioCredenciado))
                            {
                                string cmdSql = string.Format(@"select distinct ld.Texto as DeclaracaoAdicionalTexto 
                                                                from tab_cultivar_configuracao t, lov_cultivar_declara_adicional ld, tab_ptv_outrouf_declaracao ot, tab_cfoc_praga cfpraga
                                                                where t.cultivar = {1} and ld.id = ot.declaracao_adicional and ot.ptv = {0} and cfpraga.praga = ot.praga and t.tipo_producao = {2}
                                                                and cfpraga.cfoc = {3} and t.praga = ot.praga", kv.Value, cultivarID, tipoProducaoID, origem);

                                comandoCred = bancoDeDadosCred.CriarComando(cmdSql, UsuarioCredenciado);


                                using (IDataReader reader = bancoDeDadosCred.ExecutarReader(comandoCred))
                                {
                                    while (reader.Read())
                                    {
                                        retorno.Add(reader.GetValue<string>("DeclaracaoAdicionalTexto"));

                                    }
                                    reader.Close();
                                }
                            }
                        }

                        if (kv.Key == (int)eDocumentoFitossanitarioTipo.CFO)
                        {
                            using (BancoDeDados bancoDeDadosInst = BancoDeDados.ObterInstancia(UsuarioCredenciado))
                            {

                                comando = bancoDeDadosInst.CriarComando(@"select distinct lcda.Texto  DeclaracaoAdicionalTexto 
                                                                            from tab_cfoc cfoc, tab_cfoc_produto cp, tab_lote_item hli, tab_cultivar_configuracao cconf, lov_cultivar_declara_adicional lcda, tab_cfoc_praga tcp
                                                                            where cfoc.id = cp.cfoc and cp.lote = hli.lote and hli.cultivar = cconf.cultivar and cconf.declaracao_adicional = lcda.id and cfoc.id = tcp.cfoc
                                                                            and tcp.praga = cconf.praga and cfoc.id = :cfoc and cconf.tipo_producao = :tipoProducaoID and hli.cultivar = :cultivarID and lcda.outro_estado='0' ", UsuarioCredenciado);


                                comando.AdicionarParametroEntrada("tipoProducaoID", tipoProducaoID, DbType.Int32);
                                comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);
                                comando.AdicionarParametroEntrada("cfoc", origem, DbType.Int32);

                                using (IDataReader reader = bancoDeDadosInst.ExecutarReader(comando))
                                {
                                    while (reader.Read())
                                    {
                                        retorno.Add(reader.GetValue<string>("DeclaracaoAdicionalTexto"));

                                    }
                                    reader.Close();
                                }
                            }
                        }
                    }			

					#endregion
					break;
				case (int)eDocumentoFitossanitarioTipo.PTV:
					#region Buscar tratamento PTV

					comando = bancoDeDados.CriarComando(@"select pp.origem, pp.origem_tipo, pp.unidade_medida from tab_ptv_produto pp where pp.ptv = :origemId and pp.cultivar = :cultivarID", EsquemaBanco);
					comando.AdicionarParametroEntrada("origemId", origem, DbType.Int32);
					comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							int origemPTV = reader.GetValue<int>("origem");
							int origemTipoPTV = reader.GetValue<int>("origem_tipo");
							int unidadeMedidaIdPTV = reader.GetValue<int>("unidade_medida");

							retorno.AddRange(ObterDeclaracaoAdicional(origemPTV, origemTipoPTV, (int)ValidacoesGenericasBus.ObterTipoProducao(unidadeMedidaIdPTV), cultivarID,isFormatado, bancoDeDados));
						}
						reader.Close();
					}
					#endregion
					break;

                case (int)eDocumentoFitossanitarioTipo.PTVOutroEstado:
                    #region Buscar tratamento PTV Outro Estado
                    using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(EsquemaBanco))
                    {

                        comando = bancoDeDados.CriarComando(@"select lcda.texto DeclaracaoAdicionalTexto 
                            from tab_ptv_outrouf_declaracao t, lov_cultivar_declara_adicional lcda
                            where t.declaracao_adicional = lcda.id
                            and t.ptv = :origem and t.cultivar = :cultivarID ", EsquemaBanco);

                        comando.AdicionarParametroEntrada("origem", origem, DbType.Int32);
                        comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

                        using (IDataReader reader = bancoDeDadosCredenciado.ExecutarReader(comando))
                        {
                            while (reader.Read())
                            {
                                retorno.Add(reader.GetValue<string>("DeclaracaoAdicionalTexto"));

                            }
                            reader.Close();
                        }
                    }
                    #endregion
                    break;
			}
			return retorno;
		}

		internal NotaFiscalCaixa VerificarNumeroNFCaixa(NotaFiscalCaixa notaFiscal)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				NotaFiscalCaixa nf = new NotaFiscalCaixa();
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"
					SELECT ID,
					(NF.SALDO_INICIAL - 
						(SELECT NVL(SUM(PN.NUMERO_CAIXAS),0) FROM TAB_PTV_NF_CAIXA PN WHERE PN.NF_CAIXA = NF.ID) -
						(SELECT NVL(SUM(PN.NUMERO_CAIXAS),0) FROM IDAFCREDENCIADO.TAB_PTV_NF_CAIXA PN WHERE PN.NF_CAIXA = NF.ID) +
						NVL(NF.SALDO_RETIFICADO,0)
					)SALDO_ATUAL
					FROM TAB_NF_CAIXA NF
						WHERE NF.NUMERO = :numero AND NF.TIPO_CAIXA = :tipo AND NF.CPF_CNPJ_ASSOCIADO = :cpfCnpj AND
						ROWNUM <= 1 ORDER BY ID");
				comando.AdicionarParametroEntrada("numero", notaFiscal.notaFiscalCaixaNumero, DbType.String);
				comando.AdicionarParametroEntrada("tipo", notaFiscal.tipoCaixaId, DbType.String);
				comando.AdicionarParametroEntrada("cpfCnpj", notaFiscal.PessoaAssociadaCpfCnpj, DbType.String);
				
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						nf.id = reader.GetValue<int>("ID");
						nf.saldoAtual = reader.GetValue<int>("SALDO_ATUAL");
					}else
					{
						nf.id = 0;
						nf.saldoAtual = -1;
					}

					reader.Close();
				}
				return nf;
			}
		}

		internal List<NotaFiscalCaixa> ObterNFCaixas(int idPTV)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<NotaFiscalCaixa> lstNotaFiscalDeCaixa = new List<NotaFiscalCaixa>();
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"
								SELECT NF.ID, PNF.SALDO_ATUAL, PNF.NUMERO_CAIXAS, NF.NUMERO, NF.TIPO_CAIXA TIPO_ID,
									LC.TEXTO TIPO_TEXTO, NF.TIPO_PESSOA, NF.CPF_CNPJ_ASSOCIADO 
									FROM TAB_NF_CAIXA NF 
										INNER JOIN TAB_PTV_NF_CAIXA PNF ON PNF.NF_CAIXA = NF.ID
										INNER JOIN LOV_TIPO_CAIXA LC ON NF.TIPO_CAIXA = LC.ID 
									WHERE PNF.PTV = :ptv ORDER BY NF.ID");
				comando.AdicionarParametroEntrada("ptv", idPTV, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						NotaFiscalCaixa nf = new NotaFiscalCaixa();
						nf.id = reader.GetValue<int>("ID");
						nf.saldoAtual = reader.GetValue<int>("SALDO_ATUAL");
						nf.numeroCaixas = reader.GetValue<int>("NUMERO_CAIXAS");
						nf.notaFiscalCaixaNumero = reader.GetValue<string>("NUMERO");
						nf.tipoCaixaId = reader.GetValue<int>("TIPO_ID");
						nf.tipoCaixaTexto = reader.GetValue<string>("TIPO_TEXTO");
						nf.PessoaAssociadaTipo = (eTipoPessoa)reader.GetValue<int>("TIPO_PESSOA");
						nf.PessoaAssociadaCpfCnpj = reader.GetValue<string>("CPF_CNPJ_ASSOCIADO");
						lstNotaFiscalDeCaixa.Add(nf);
					}

					reader.Close();
				}
				return lstNotaFiscalDeCaixa;
				//return (bancoDeDados.ExecutarScalar<int>(comando));
			}
		}
		#endregion

		#region Comunicador

		internal PTVComunicador ObterComunicador(int idPTV, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{

				PTVComunicador comunicador = new PTVComunicador();
				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.ptv_id, p.numero ptv_numero, c.arquivo_interno, c.arquivo_credenciado, c.liberado_credenciado, c.tid 
					FROM TAB_PTV P LEFT JOIN TAB_PTV_COMUNICADOR c ON P.ID = c.PTV_ID 
					WHERE P.ID = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", idPTV, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						comunicador.Id = reader.GetValue<int>("id");
						comunicador.PTVId = idPTV;
						comunicador.PTVNumero = reader.GetValue<Int64>("ptv_numero");
						comunicador.ArquivoInternoId = reader.GetValue<int>("arquivo_interno");
						comunicador.ArquivoCredenciadoId = reader.GetValue<int>("arquivo_credenciado");
						comunicador.liberadoCredenciado = reader.GetValue<bool>("liberado_credenciado");
						comunicador.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#region Conversa
				comando = bancoDeDados.CriarComando(@"select c.id, c.data, c.texto, c.tipo_comunicador, c.funcionario, c.credenciado, c.nome_comunicador, c.arquivo, c.nome_arquivo, c.tid 
                                                        from {0}tab_ptv_comuni_conversa c where c.comunicador_id = :ptv order by c.data", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("ptv", comunicador.Id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PTVConversa conversa;
					while (reader.Read())
					{
						conversa = new PTVConversa();
						conversa.Id = reader.GetValue<int>("id");
						conversa.DataConversa.Data = reader.GetValue<DateTime>("data");
						conversa.Texto = reader.GetValue<string>("texto");
						conversa.TipoComunicador = reader.GetValue<int>("tipo_comunicador");
						conversa.FuncionarioId = reader.GetValue<int>("funcionario");
						conversa.CredenciadoId = reader.GetValue<int>("credenciado");
						conversa.NomeComunicador = reader.GetValue<string>("nome_comunicador");
						conversa.ArquivoId = reader.GetValue<int>("arquivo");
						conversa.ArquivoNome = reader.GetValue<string>("nome_arquivo");
						conversa.Tid = reader.GetValue<string>("tid");

						comunicador.Conversas.Add(conversa);
					}
					reader.Close();
				}

				#endregion

				return comunicador;
			}
		}


		internal int ObterIDComunicador(int idPTV, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id from {0}tab_ptv_comunicador c where c.ptv_id = :ptv_id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("ptv_id", idPTV, DbType.Int32);

				return Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));
			}
		}

		internal PTVComunicador ObterComunicadorSemConversas(int idPTV, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{

				PTVComunicador comunicador = new PTVComunicador();
				Comando comando = bancoDeDados.CriarComando(@" select c.id, c.ptv_id, p.numero ptv_numero, c.arquivo_interno, c.arquivo_credenciado, c.liberado_credenciado, c.tid 
                                                            from {0}TAB_PTV_COMUNICADOR c, {0}TAB_PTV P where c.ptv_id=p.id and c.ptv_id = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", idPTV, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						comunicador.Id = reader.GetValue<int>("id"); ;
						comunicador.PTVId = idPTV;
						comunicador.PTVNumero = reader.GetValue<Int64>("ptv_numero");
						comunicador.ArquivoInternoId = reader.GetValue<int>("arquivo_interno");
						comunicador.ArquivoCredenciadoId = reader.GetValue<int>("arquivo_credenciado");
						comunicador.liberadoCredenciado = reader.GetValue<bool>("liberado_credenciado");
						comunicador.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				return comunicador;
			}
		}


		internal int ExisteComunicadorPTV(PTVComunicador comunicador, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(c.ptv_id) qtd from {0}tab_ptv_comunicador c where c.ptv_id = :ptv_id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("ptv_id", comunicador.PTVId, DbType.Int32);
				return (bancoDeDados.ExecutarScalar<int>(comando));
			}
		}

		public bool PossuiAcessoComunicadorPTV(int funcionarioID, int LocalVistoriaSetorID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(fs.id) from {0}tab_funcionario f, {0}tab_funcionario_setor fs 
                                                                where fs.funcionario = f.id and f.id = :funcionario and :local_vistoria_setor in (fs.setor)", EsquemaBanco);

				comando.AdicionarParametroEntrada("funcionario", funcionarioID, DbType.Int32);
				comando.AdicionarParametroEntrada("local_vistoria_setor", LocalVistoriaSetorID, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal void CriarComunicador(PTVComunicador comunicador, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_ptv_comunicador 
                                                        (id, ptv_id, arquivo_interno, arquivo_credenciado, liberado_credenciado, tid)
                                                    values
                                                        (seq_tab_ptv_comunicador.nextval, :ptv_id, :arquivo_interno, :arquivo_credenciado, :liberado_credenciado, :tid) 
                                                    returning id into :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("ptv_id", comunicador.PTVId, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo_interno", comunicador.ArquivoInternoId, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo_credenciado", comunicador.ArquivoCredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("liberado_credenciado", comunicador.liberadoCredenciado, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				//ID de retorno
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarScalar(comando);

				comunicador.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Conversa

				PTVConversa conversa = comunicador.Conversas[0];

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_ptv_comuni_conversa
                                                        (id, comunicador_id, data, texto, tipo_comunicador, funcionario, credenciado, nome_comunicador, arquivo, nome_arquivo, tid )
													values
                                                        (seq_tab_ptv_comuni_conversa.nextval,:comunicador_id,:data, :texto, :tipo_comunicador, :funcionario, 
                                                         :credenciado, :nome_comunicador, :arquivo, :nome_arquivo, :tid) returning id into :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("comunicador_id", comunicador.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data", conversa.DataConversa.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("texto", conversa.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tipo_comunicador", conversa.TipoComunicador, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", conversa.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", conversa.CredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_comunicador", conversa.NomeComunicador, DbType.String);
				comando.AdicionarParametroEntrada("arquivo", conversa.ArquivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_arquivo", conversa.ArquivoNome, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				conversa.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
				#endregion

				Historico.Gerar(conversa.Id, eHistoricoArtefato.ptvcomunicador, eHistoricoAcao.enviar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}


		internal void EnviarConversa(PTVComunicador comunicador, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				comando = bancoDeDados.CriarComando(@"update {0}tab_ptv_comunicador
															set arquivo_interno = :arquivo_interno,
																arquivo_credenciado = :arquivo_credenciado,
																liberado_credenciado = :liberado_credenciado,
																tid = :tid
															where id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", comunicador.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo_interno", comunicador.ArquivoInternoId, DbType.Int32);
				comando.AdicionarParametroEntrada("arquivo_credenciado", comunicador.ArquivoCredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("liberado_credenciado", comunicador.liberadoCredenciado, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarScalar(comando);

				#region Conversa

				PTVConversa conversa = comunicador.Conversas[0];

				comando = bancoDeDados.CriarComando(@"insert into {0}tab_ptv_comuni_conversa
                                                        (id, comunicador_id, data, texto, tipo_comunicador, funcionario, credenciado, nome_comunicador, arquivo, nome_arquivo, tid )
													values
                                                        (seq_tab_ptv_comuni_conversa.nextval,:comunicador_id,:data, :texto, :tipo_comunicador, :funcionario, 
                                                         :credenciado, :nome_comunicador, :arquivo, :nome_arquivo, :tid) returning id into :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("comunicador_id", comunicador.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("data", conversa.DataConversa.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("texto", conversa.Texto, DbType.String);
				comando.AdicionarParametroEntrada("tipo_comunicador", conversa.TipoComunicador, DbType.Int32);
				comando.AdicionarParametroEntrada("funcionario", conversa.FuncionarioId, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", conversa.CredenciadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_comunicador", conversa.NomeComunicador, DbType.String);
				comando.AdicionarParametroEntrada("arquivo", conversa.ArquivoId, DbType.Int32);
				comando.AdicionarParametroEntrada("nome_arquivo", conversa.ArquivoNome, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				conversa.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#endregion

				Historico.Gerar(conversa.Id, eHistoricoArtefato.ptvcomunicador, eHistoricoAcao.enviar, bancoDeDados);

				bancoDeDados.Commit();
			}

		}

		internal string BuscarSituacaoPTV(int idPTV, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select s.texto from {0}tab_ptv p, {0}lov_solicitacao_ptv_situacao s where s.id=p.situacao and p.id=:idPTV", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("idPTV", idPTV, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<string>(comando));
			}
		}

		internal List<String> ObterEmailsCredenciado(int idPTV, BancoDeDados banco = null)
		{
			List<String> lst = new List<String>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select co.valor email from {0}tab_ptv ptv, {0}tab_credenciado cre, {0}tab_pessoa p, {0}tab_pessoa_meio_contato co 
                                                              where cre.id=ptv.credenciado and p.id=cre.pessoa and p.id = co.pessoa and co.meio_contato = 5 and ptv.id= :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", idPTV, DbType.Int32);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				foreach (var item in daReader)
				{
					lst.Add(item["email"].ToString());
				}
			}
			return lst;
		}

		#endregion

		#region EPTV

		internal void AnalisarEPTV(PTV eptv, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				bancoDeDados.IniciarTransacao();
				var sqlAprovado = (eptv.Situacao == (int)eSolicitarPTVSituacao.Valido) ? ", p.data_ativacao = sysdate " : string.Empty;
				var sqlExibirMensagemCredenciado = (eptv.Situacao == (int)eSolicitarPTVSituacao.Valido ||
					eptv.Situacao == (int)eSolicitarPTVSituacao.Rejeitado ||
					eptv.Situacao == (int)eSolicitarPTVSituacao.Bloqueado ||
					eptv.Situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao) ? ", p.exibir_msg_credenciado = 1 " : string.Empty;

				Comando comando = bancoDeDados.CriarComando(@"update {0}tab_ptv p set p.tid = :tid, p.situacao = :situacao,
					p.motivo = :motivo, p.local_fiscalizacao = :local_fiscalizacao, p.hora_fiscalizacao = :hora_fiscalizacao,
					p.informacoes_adicionais = :informacoes_adicionais, p.partida_lacrada_origem = :partida_lacrada_origem,
					p.numero_lacre = :numero_lacre, p.numero_porao = :numero_porao, p.numero_container = :numero_container,
					p.situacao_data = sysdate " + sqlAprovado + sqlExibirMensagemCredenciado  + " where p.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", eptv.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", eptv.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("motivo", DbType.String, 500, eptv.SituacaoMotivo ?? "");
				comando.AdicionarParametroEntrada("local_fiscalizacao", DbType.String, 500, eptv.LocalFiscalizacao);
				comando.AdicionarParametroEntrada("hora_fiscalizacao", eptv.HoraFiscalizacao, DbType.String);
				comando.AdicionarParametroEntrada("informacoes_adicionais", DbType.String, 500, eptv.InformacoesAdicionais);
				comando.AdicionarParametroEntrada("partida_lacrada_origem", eptv.PartidaLacradaOrigem, DbType.Int32);
				comando.AdicionarParametroEntrada("numero_lacre", eptv.LacreNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_porao", eptv.PoraoNumero, DbType.String);
				comando.AdicionarParametroEntrada("numero_container", eptv.ContainerNumero, DbType.String);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				var historicoAcao = eHistoricoAcao.analisar;

				if (eptv.Situacao == (int)eSolicitarPTVSituacao.Valido)
					historicoAcao = eHistoricoAcao.aprovar;/*TODO:--Aprovar*/
				else if (eptv.Situacao == (int)eSolicitarPTVSituacao.Rejeitado)
					historicoAcao = eHistoricoAcao.rejeitar;/*TODO:--Rejeitar*/
				else if (eptv.Situacao == (int)eSolicitarPTVSituacao.AgendarFiscalizacao)
					historicoAcao = eHistoricoAcao.agendarFiscalizacao;/*TODO:--AgendarFiscalizacao*/
				else if (eptv.Situacao == (int)eSolicitarPTVSituacao.Bloqueado)
					historicoAcao = eHistoricoAcao.bloquear;/*TODO:--Bloquear*/

				Historico.Gerar(eptv.Id, eHistoricoArtefato.emitirptv, historicoAcao, bancoDeDados);			
				
				bancoDeDados.Commit();
			}
		}

		internal bool EmpreendimentoPossuiEPTVBloqueado(int empreendimentoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(*) from tab_ptv eptv where eptv.situacao = 6/*Bloqueado*/ and eptv.empreendimento = :empreendimento", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal bool EmpreendimentoPossuiEPTVBloqueado(int idEPTV, int empreendimentoID, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(*) from tab_ptv eptv where eptv.situacao = 6/*Bloqueado*/ and eptv.empreendimento = :empreendimento and eptv.id <> :idEPTV", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoID, DbType.Int32);
				comando.AdicionarParametroEntrada("idEPTV", idEPTV, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal bool EPTVJaFoiImportado(int idEPTV, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(*) from tab_ptv ptv where ptv.eptv_id = :idEPTV");
				comando.AdicionarParametroEntrada("idEPTV", idEPTV, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		#endregion

		#region Alerta EPTV

		public string EPTVAguardandoAnaliseFuncionario(int idFuncionario, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;
				List<Int64> listaPtv = new List<Int64>();

				comando = bancoDeDados.CriarComando(@"select pt.numero
													  from {0}Tab_Ptv pt
													  where Pt.Local_Vistoria in ( select S.Setor
													                               from {1}Tab_Funcionario_Setor s
													                               where S.Funcionario = :idFuncionario )
													  and pt.situacao = 2", UsuarioCredenciado, EsquemaBanco);
				comando.AdicionarParametroEntrada("idFuncionario", idFuncionario, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						listaPtv.Add(reader.GetValue<Int64>("numero"));
					}
				}

				return String.Join(", ", listaPtv);
			}
		}

		public PTV ObterNumeroPTVExibirMensagemFuncionario(int idFuncionario, BancoDeDados banco = null)
		{
			var ptv = new PTV();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select pt.id, pt.numero
													  from {0}Tab_Ptv pt
													  where Pt.Local_Vistoria in ( select S.Setor
													                               from {1}Tab_Funcionario_Setor s
													                               where S.Funcionario = :idFuncionario )
													  and pt.exibir_mensagem = 1
													  and rownum = 1", UsuarioCredenciado, EsquemaBanco);
				comando.AdicionarParametroEntrada("idFuncionario", idFuncionario, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						ptv = new PTV()
						{
							Id = reader.GetValue<int>("id"),
							Numero = reader.GetValue<long>("numero")
						};
					}
				}

			}

			#region Exibir_Mensagem

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia("idafcredenciado"))
			{
				if (ptv?.Id > 0)
				{
					Comando comando = null;

					comando = bancoDeDados.CriarComando(@"update {0}Tab_Ptv pt set pt.exibir_mensagem = 0
													  where pt.id = :ptv_id", UsuarioCredenciado, EsquemaBanco);
					comando.AdicionarParametroEntrada("ptv_id", ptv.Id, DbType.Int32);
					bancoDeDados.ExecutarScalar(comando);
				}
			}

			#endregion Exibir_Mensagem

			return ptv;
		}

		#endregion Alerta EPTV

		#region Retificação Nota fiscal de caixa

		internal Resultados<RetificacaoNFCaixaListarResultado> FiltrarNFCaixa(Filtro<RetificacaoNFCaixaListarFiltro> filtro)
		{
			Resultados<RetificacaoNFCaixaListarResultado> retorno = new Resultados<RetificacaoNFCaixaListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");
				string tabelaTipoDoc = String.Empty;
				string amarracaoTipoDoc = String.Empty;

				#region Adicionando Filtros

				
				comandtxt += comando.FiltroAnd("NF.NUMERO", "NUMERO", filtro.Dados.NumeroNFCaixa);

				if (!String.IsNullOrEmpty(filtro.Dados.NFCaixaCPFCNPJ))
				{
					var tipoPessoa = filtro.Dados.NFCaixaIsCPF ? 1 : 2;
					comandtxt += comando.FiltroAnd("NF.TIPO_PESSOA", "TIPO_PESSOA", tipoPessoa);
					comandtxt += comando.FiltroAnd("NF.CPF_CNPJ_ASSOCIADO", "CPF_CNPJ_ASSOCIADO", filtro.Dados.NFCaixaCPFCNPJ);
				}
				if (!String.IsNullOrWhiteSpace(filtro.Dados.NumeroPTV))
				{
					comandtxt += @"AND (select ptv.numero from tab_ptv ptv inner join tab_ptv_nf_caixa pnf on pnf.PTV = ptv.id
									   where pnf.nf_caixa = nf.id and ptv.numero = :PTV
									   union 
									   select ptv.numero from idafcredenciado.tab_ptv ptv inner join idafcredenciado.tab_ptv_nf_caixa pnf
									on pnf.PTV = ptv.id where pnf.nf_caixa = nf.id
										and ptv.numero = :PTV) is not null";
					comando.AdicionarParametroEntrada("PTV", filtro.Dados.NumeroPTV);
				}
				if ((int)filtro.Dados.tipoDeCaixa > 0)
				{
					comandtxt += comando.FiltroAnd("NF.TIPO_CAIXA", "TIPO_CAIXA", (int)filtro.Dados.tipoDeCaixa);
				}
				

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "NUMERO", "CPF_CNPJ_ASSOCIADO", "TIPO_CAIXA", "SALDO_INICIAL", "SALDO_ATUAL" };

				if (filtro.OdenarPor > 0)
				{
					ordenar.Add(colunas.ElementAtOrDefault(filtro.OdenarPor - 1));
				}
				else
				{
					ordenar.Add("NUMERO");
				}

				#endregion

				#region Quantidade de registro do resultado

				comando.DbCommand.CommandText =
				"select count(*) from (" + String.Format(@"select	NF.ID
																FROM  TAB_NF_CAIXA NF where 1=1 " + comandtxt + " group by NF.ID) a ", esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				comandtxt = String.Format(@"SELECT	NF.ID,
													NF.NUMERO,
													NF.TIPO_PESSOA,
													NF.CPF_CNPJ_ASSOCIADO,
													NF.TIPO_CAIXA,
													LVC.TEXTO TIPO_CAIXA_TEXTO,
													NF.SALDO_INICIAL,
													(NF.SALDO_INICIAL - 
													  (SELECT NVL(SUM(PN.NUMERO_CAIXAS),0) FROM TAB_PTV_NF_CAIXA PN WHERE PN.NF_CAIXA = NF.ID) -
													  (SELECT NVL(SUM(PN.NUMERO_CAIXAS),0) FROM IDAFCREDENCIADO.TAB_PTV_NF_CAIXA PN WHERE PN.NF_CAIXA = NF.ID) +
													  NVL(NF.SALDO_RETIFICADO,0)
													)SALDO_ATUAL
												FROM  TAB_NF_CAIXA NF
												INNER JOIN LOV_TIPO_CAIXA LVC ON LVC.ID = NF.TIPO_CAIXA WHERE 1=1 " + comandtxt + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);
				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					RetificacaoNFCaixaListarResultado item;
					while (reader.Read())
					{
						item = new RetificacaoNFCaixaListarResultado();
						item.Id = reader.GetValue<int>("ID");
						item.NumeroNFCaixa = reader.GetValue<string>("NUMERO");
						item.TipoPessoa = reader.GetValue<int>("TIPO_PESSOA");
						item.CPFCNPJ = reader.GetValue<string>("CPF_CNPJ_ASSOCIADO");
						item.TipoCaixa = reader.GetValue<int>("TIPO_CAIXA");
						item.TipoCaixaTexto = reader.GetValue<string>("TIPO_CAIXA_TEXTO");
						item.SaldoInicial = reader.GetValue<int>("SALDO_INICIAL");
						item.SaldoAtual = reader.GetValue<int>("SALDO_ATUAL");

						retorno.Itens.Add(item);
					}
					reader.Close();
				}
			}
			return retorno;
		}

		internal bool VerificarExcluirNFCaixa(int id, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				//Atualiza o tid da tabela tab_ptv
				Comando comando = bancoDeDados.CriarComando(@"SELECT COUNT(1) FROM (                        
																	SELECT ID FROM TAB_PTV_NF_CAIXA WHERE NF_CAIXA = :nf
																	union all
																	SELECT ID FROM IDAFCREDENCIADO.TAB_PTV_NF_CAIXA WHERE NF_CAIXA = :nf
																)", EsquemaBanco);

				comando.AdicionarParametroEntrada("nf", id, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal void ExcluirNFCaixa(int id, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"DELETE TAB_NF_CAIXA WHERE ID = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}

		internal NotaFiscalCaixa ObterNFCaixa(int id, BancoDeDados banco= null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				NotaFiscalCaixa nFCaixa = new NotaFiscalCaixa();

				Comando comando = bancoDeDados.CriarComando(@"
					SELECT NF.NUMERO, NF.TIPO_PESSOA, NF.CPF_CNPJ_ASSOCIADO, NF.TIPO_CAIXA, NF.SALDO_INICIAL, 
					  (NF.SALDO_INICIAL - 
						  (SELECT NVL(SUM(PN.NUMERO_CAIXAS),0) FROM TAB_PTV_NF_CAIXA PN WHERE PN.NF_CAIXA = NF.ID) -
						  (SELECT NVL(SUM(PN.NUMERO_CAIXAS),0) FROM IDAFCREDENCIADO.TAB_PTV_NF_CAIXA PN WHERE PN.NF_CAIXA = NF.ID) +
						  NVL(NF.SALDO_RETIFICADO,0)
						)SALDO_ATUAL
					FROM TAB_NF_CAIXA NF 
					WHERE NF.ID =: id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						nFCaixa.id = id;
						nFCaixa.notaFiscalCaixaNumero = reader.GetValue<string>("NUMERO");
						nFCaixa.PessoaAssociadaTipo = (eTipoPessoa)reader.GetValue<int>("TIPO_PESSOA");
						nFCaixa.PessoaAssociadaCpfCnpj = reader.GetValue<string>("CPF_CNPJ_ASSOCIADO");
						nFCaixa.tipoCaixaId = reader.GetValue<int>("TIPO_CAIXA");
						nFCaixa.saldoInicial = reader.GetValue<int>("SALDO_INICIAL");
						nFCaixa.saldoAtual = reader.GetValue<int>("SALDO_ATUAL");
					}
					reader.Close();
				}
				bancoDeDados.Commit();
				return nFCaixa;
			}
		}

		internal Resultados<PTVNFCaixaResultado> ObterPTVNFCaixa(Filtro<int> filtro)
		{

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Resultados<PTVNFCaixaResultado> retorno = new Resultados<PTVNFCaixaResultado>();
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando("");
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");


				comando.DbCommand.CommandText =
				"select count(*) from (" + String.Format(@"SELECT NF.ID
					FROM TAB_NF_CAIXA               NF
						INNER JOIN TAB_PTV_NF_CAIXA   PNF   ON NF.ID = PNF.NF_CAIXA
						INNER JOIN TAB_PTV            PTV   ON PNF.PTV = PTV.ID
						LEFT JOIN TAB_EMPREENDIMENTO E     ON PTV.EMPREENDIMENTO = E.ID
						INNER JOIN LOV_PTV_SITUACAO   LVP   ON PTV.SITUACAO = LVP.ID
						LEFT JOIN TAB_PESSOA         P     ON PTV.RESPONSAVEL_EMP = P.ID
					WHERE NF.ID = :id  
					UNION
    
					SELECT NF.ID
					FROM TAB_NF_CAIXA												NF
						INNER JOIN IDAFCREDENCIADO.TAB_PTV_NF_CAIXA					PNF   ON NF.ID = PNF.NF_CAIXA
						INNER JOIN IDAFCREDENCIADO.TAB_PTV							PTV   ON PNF.PTV = PTV.ID
						LEFT JOIN TAB_EMPREENDIMENTO								E     ON PTV.EMPREENDIMENTO = E.ID
						INNER JOIN IDAFCREDENCIADO.LOV_SOLICITACAO_PTV_SITUACAO     LVP   ON PTV.SITUACAO = LVP.ID
						LEFT JOIN TAB_PESSOA										P     ON PTV.RESPONSAVEL_EMP = P.ID	
					WHERE NF.ID = :id) a ", esquemaBanco);
				comando.AdicionarParametroEntrada("id", filtro.Dados, DbType.Int32);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

				comandtxt = String.Format(@"SELECT * FROM (
					SELECT	PTV.NUMERO,
							NVL(E.DENOMINADOR, PTV.EMPREENDIMENTO_SEM_DOC) DENOMINADOR,
							'Institucional' ORIGEM,
							LVP.TEXTO SITUACAO,
							NVL(P.NOME, (SELECT PA.NOME FROM TAB_PESSOA PA WHERE PA.ID = PTV.RESPONSAVEL_SEM_DOC)) INTERESSADO,
							PNF.NUMERO_CAIXAS,
							PTV.DATA_EMISSAO
					FROM TAB_NF_CAIXA               NF
						INNER JOIN TAB_PTV_NF_CAIXA   PNF   ON NF.ID = PNF.NF_CAIXA
						INNER JOIN TAB_PTV            PTV   ON PNF.PTV = PTV.ID
						LEFT JOIN TAB_EMPREENDIMENTO E     ON PTV.EMPREENDIMENTO = E.ID
						INNER JOIN LOV_PTV_SITUACAO   LVP   ON PTV.SITUACAO = LVP.ID
						LEFT JOIN TAB_PESSOA         P     ON PTV.RESPONSAVEL_EMP = P.ID
					WHERE NF.ID = :id

					UNION
    
					SELECT	PTV.NUMERO,
							NVL(E.DENOMINADOR, PTV.EMPREENDIMENTO_SEM_DOC) DENOMINADOR,
							'Credenciado' ORIGEM,
							LVP.TEXTO SITUACAO,
							NVL(P.NOME, (SELECT PA.NOME FROM TAB_PESSOA PA WHERE PA.ID = PTV.RESPONSAVEL_SEM_DOC)) INTERESSADO,
							PNF.NUMERO_CAIXAS,
							PTV.DATA_EMISSAO
					FROM TAB_NF_CAIXA												NF
						INNER JOIN IDAFCREDENCIADO.TAB_PTV_NF_CAIXA					PNF   ON NF.ID = PNF.NF_CAIXA
						INNER JOIN IDAFCREDENCIADO.TAB_PTV							PTV   ON PNF.PTV = PTV.ID
						LEFT JOIN TAB_EMPREENDIMENTO								E     ON PTV.EMPREENDIMENTO = E.ID
						INNER JOIN IDAFCREDENCIADO.LOV_SOLICITACAO_PTV_SITUACAO     LVP   ON PTV.SITUACAO = LVP.ID
						LEFT JOIN TAB_PESSOA										P     ON PTV.RESPONSAVEL_EMP = P.ID	
					WHERE NF.ID = :id)" + comandtxt, esquemaBanco);
				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				comando.AdicionarParametroEntrada("id", filtro.Dados, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						PTVNFCaixaResultado item = new PTVNFCaixaResultado();
						item.NumeroPTV = reader.GetValue<string>("NUMERO");
						item.Empreendimento = reader.GetValue<string>("DENOMINADOR");
						item.Origem = reader.GetValue<string>("ORIGEM");
						item.Situacao = reader.GetValue<string>("SITUACAO");
						item.Interessado = reader.GetValue<string>("INTERESSADO");
						item.QtdCaixa = reader.GetValue<int>("NUMERO_CAIXAS");
						item.DataEmissao = reader.GetValue<DateTime>("DATA_EMISSAO");
						retorno.Itens.Add(item);
					}
					reader.Close();
				}
				return retorno;
			}
		}

		internal void SalvarNFCaixa(int id, int novoSalvo, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, EsquemaBanco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = bancoDeDados.CriarComando(@"UPDATE TAB_NF_CAIXA SET SALDO_RETIFICADO = :saldo_novo WHERE ID = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("saldo_novo", novoSalvo, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(id, eHistoricoArtefato.notafiscalcaixa, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}
		#endregion
		internal bool ExisteAssinaturaDigital(int funcionarioId)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;
				comando = bancoDeDados.CriarComando(@"select count(2)  from tab_funcionario t where t.arquivo is not null and t.id = :idfun");
				comando.AdicionarParametroEntrada("idfun", funcionarioId, DbType.Int32);
				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}
	}
}