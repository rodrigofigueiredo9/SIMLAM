using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.ModuloFiscalizacao;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloFiscalizacao.Data
{
	public class MaterialApreendidoDa
	{
		#region Propriedade e Atributos

		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();
		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }

		private string EsquemaBanco { get; set; }

		#endregion

		public MaterialApreendidoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Ações de DML

		public MaterialApreendido Salvar(MaterialApreendido materialApreendido, BancoDeDados banco = null)
		{
			if (materialApreendido == null)
			{
				throw new Exception("Material apreendido é nulo.");
			}

			if (materialApreendido.Id <= 0)
			{
				materialApreendido = Criar(materialApreendido, banco);
			}
			else
			{
				materialApreendido = Editar(materialApreendido, banco);
			}

			return materialApreendido;
		}

		public MaterialApreendido Criar(MaterialApreendido materialApreendido, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Material Apreendido

				Comando comando = bancoDeDados.CriarComando(@" insert into tab_fisc_material_apreendido (id, fiscalizacao, houve_material, tad_gerado, tad_numero, tad_data, serie, 
					descricao, valor_produtos, depositario, endereco_logradouro, endereco_bairro, endereco_distrito, endereco_estado, endereco_municipio, opiniao, arquivo, tid) values (seq_fisc_material_apreendido.nextval, 
					:fiscalizacao, :houve_material, :tad_gerado, :tad_numero, :tad_data, :serie, :descricao, :valor_produtos, :depositario, :endereco_logradouro, :endereco_bairro, 
					:endereco_distrito, :endereco_estado, :endereco_municipio, :opiniao, :arquivo, :tid) returning id into :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", materialApreendido.FiscalizacaoId, DbType.Int32);
                //comando.AdicionarParametroEntrada("houve_material", materialApreendido.IsApreendido, DbType.Int32);
				comando.AdicionarParametroEntrada("tad_gerado", materialApreendido.IsTadGeradoSistema, DbType.Int32);
				comando.AdicionarParametroEntrada("serie", materialApreendido.SerieId, DbType.Int32);
				comando.AdicionarParametroEntrada("depositario", materialApreendido.Depositario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("endereco_estado", materialApreendido.Depositario.Estado, DbType.Int32);
				comando.AdicionarParametroEntrada("endereco_municipio", materialApreendido.Depositario.Municipio, DbType.Int32);
				comando.AdicionarParametroEntrada("valor_produtos", materialApreendido.ValorProdutos, DbType.Decimal);
				comando.AdicionarParametroEntrada("opiniao", materialApreendido.Opiniao, DbType.String);
                //comando.AdicionarParametroEntrada("tad_numero", materialApreendido.NumeroTad, DbType.String);
				comando.AdicionarParametroEntrada("descricao", materialApreendido.Descricao, DbType.String);
				comando.AdicionarParametroEntrada("endereco_logradouro", materialApreendido.Depositario.Logradouro, DbType.String);
				comando.AdicionarParametroEntrada("endereco_bairro", materialApreendido.Depositario.Bairro, DbType.String);
				comando.AdicionarParametroEntrada("endereco_distrito", materialApreendido.Depositario.Distrito, DbType.String);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroSaida("id", DbType.Int32);

				if (materialApreendido.Arquivo == null)
				{
					comando.AdicionarParametroEntrada("arquivo", DBNull.Value, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("arquivo", materialApreendido.Arquivo.Id, DbType.Int32);
				}

				if (materialApreendido.DataLavratura.IsEmpty)
				{
					comando.AdicionarParametroEntrada("tad_data", DBNull.Value, DbType.Date);
				}
				else
				{
					comando.AdicionarParametroEntrada("tad_data", materialApreendido.DataLavratura.Data.Value, DbType.Date);
				}

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				materialApreendido.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Materiais

				comando = bancoDeDados.CriarComando(@" insert into tab_fisc_mater_apree_material (id, material_apreendido, tipo, especificacao, tid) values (seq_fisc_mater_apree_material.nextval, 
					:material_apreendido, :tipo, :especificacao, :tid) ", EsquemaBanco);

				comando.AdicionarParametroEntrada("material_apreendido", materialApreendido.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("tipo", DbType.Int32);
				comando.AdicionarParametroEntrada("especificacao", DbType.String);

				foreach (MaterialApreendidoMaterial material in materialApreendido.Materiais)
				{
					comando.SetarValorParametro("tipo", material.TipoId);
					comando.SetarValorParametro("especificacao", material.Especificacao);

					bancoDeDados.ExecutarNonQuery(comando);
				}

				#endregion

				Historico.Gerar(materialApreendido.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(materialApreendido.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}
			return materialApreendido;
		}

		public MaterialApreendido Editar(MaterialApreendido materialApreendido, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				#region Material Apreendido

				Comando comando = bancoDeDados.CriarComando(@" update tab_fisc_material_apreendido set fiscalizacao = :fiscalizacao, houve_material = :houve_material,
					tad_gerado = :tad_gerado, tad_numero = :tad_numero, tad_data = :tad_data, serie = :serie, descricao = :descricao, valor_produtos = :valor_produtos, depositario = :depositario, 
					endereco_logradouro = :endereco_logradouro, endereco_bairro = :endereco_bairro, endereco_distrito = :endereco_distrito, endereco_estado = :endereco_estado, endereco_municipio =
					:endereco_municipio, opiniao = :opiniao, arquivo = :arquivo, tid = :tid where id = :id ", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", materialApreendido.FiscalizacaoId, DbType.Int32);
                //comando.AdicionarParametroEntrada("houve_material", materialApreendido.IsApreendido, DbType.Int32);
				comando.AdicionarParametroEntrada("tad_gerado", materialApreendido.IsTadGeradoSistema, DbType.Int32);
				comando.AdicionarParametroEntrada("serie", materialApreendido.SerieId, DbType.Int32);
				comando.AdicionarParametroEntrada("depositario", materialApreendido.Depositario.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("endereco_estado", materialApreendido.Depositario.Estado, DbType.Int32);
				comando.AdicionarParametroEntrada("endereco_municipio", materialApreendido.Depositario.Municipio, DbType.Int32);
				comando.AdicionarParametroEntrada("valor_produtos", materialApreendido.ValorProdutos, DbType.Decimal);
				comando.AdicionarParametroEntrada("opiniao", materialApreendido.Opiniao, DbType.String);
                //comando.AdicionarParametroEntrada("tad_numero", materialApreendido.NumeroTad, DbType.String);
				comando.AdicionarParametroEntrada("descricao", materialApreendido.Descricao, DbType.String);
				comando.AdicionarParametroEntrada("endereco_logradouro", materialApreendido.Depositario.Logradouro, DbType.String);
				comando.AdicionarParametroEntrada("endereco_bairro", materialApreendido.Depositario.Bairro, DbType.String);
				comando.AdicionarParametroEntrada("endereco_distrito", materialApreendido.Depositario.Distrito, DbType.String);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("id", materialApreendido.Id, DbType.Int32);

				if (materialApreendido.Arquivo == null)
				{
					comando.AdicionarParametroEntrada("arquivo", DBNull.Value, DbType.Int32);
				}
				else
				{
					comando.AdicionarParametroEntrada("arquivo", materialApreendido.Arquivo.Id, DbType.Int32);
				}

				if (materialApreendido.DataLavratura.IsEmpty)
				{
					comando.AdicionarParametroEntrada("tad_data", DBNull.Value, DbType.Date);
				}
				else
				{
					comando.AdicionarParametroEntrada("tad_data", materialApreendido.DataLavratura.Data.Value, DbType.Date);
				}

				bancoDeDados.ExecutarNonQuery(comando);

				#endregion

				#region Materiais

				comando = bancoDeDados.CriarComando(@"delete from {0}tab_fisc_mater_apree_material c where c.material_apreendido = :material_apreendido", EsquemaBanco);
				comando.DbCommand.CommandText += String.Format(" {0}", comando.AdicionarNotIn("and", "c.id", DbType.Int32, materialApreendido.Materiais.Select(x => x.Id).ToList()));
				comando.AdicionarParametroEntrada("material_apreendido", materialApreendido.Id, DbType.Int32);
				bancoDeDados.ExecutarNonQuery(comando);

				foreach (MaterialApreendidoMaterial material in materialApreendido.Materiais)
				{
					if (material.Id == 0)
					{
						comando = bancoDeDados.CriarComando(@" insert into tab_fisc_mater_apree_material (id, material_apreendido, tipo, especificacao, tid) values (seq_fisc_mater_apree_material.nextval, 
							:material_apreendido, :tipo, :especificacao, :tid) returning id into :id ", EsquemaBanco);
						comando.AdicionarParametroSaida("id", DbType.Int32);
					}
					else
					{
						comando = bancoDeDados.CriarComando(@" update tab_fisc_mater_apree_material set material_apreendido = :material_apreendido, tipo = :tipo, especificacao = :especificacao, tid = :tid
							where id = :id ", EsquemaBanco);
						comando.AdicionarParametroEntrada("id", material.Id, DbType.Int32);
					}

					comando.AdicionarParametroEntrada("material_apreendido", materialApreendido.Id, DbType.Int32);
					comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
					comando.AdicionarParametroEntrada("tipo", material.TipoId, DbType.Int32);
					comando.AdicionarParametroEntrada("especificacao", material.Especificacao, DbType.String);

					bancoDeDados.ExecutarNonQuery(comando);

					if (material.Id == 0)
					{
						material.Id = Convert.ToInt32(comando.ObterValorParametro("id"));
					}
				}

				#endregion

				Historico.Gerar(materialApreendido.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, eHistoricoAcao.atualizar, bancoDeDados);

				Consulta.Gerar(materialApreendido.FiscalizacaoId, eHistoricoArtefato.fiscalizacao, bancoDeDados);

				bancoDeDados.Commit();
			}
			return materialApreendido;
		}

		public void Excluir(int fiscalizacaoId, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(
													"begin "
														+ "delete {0}tab_fisc_mater_apree_material t where t.material_apreendido = (select id from {0}tab_fisc_material_apreendido where fiscalizacao = :fiscalizacao); "														
														+ "delete {0}tab_fisc_material_apreendido t where t.fiscalizacao = :fiscalizacao; "
													+ "end;", EsquemaBanco);
				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				bancoDeDados.ExecutarNonQuery(comando);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter / Filtrar

		public MaterialApreendido Obter(int fiscalizacaoId, BancoDeDados banco = null)
		{
			MaterialApreendido materialApreendido = new MaterialApreendido();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Material Apreendido

				Comando comando = bancoDeDados.CriarComando(@" select t.id, t.fiscalizacao, f.situacao situacao_id, t.houve_material, t.tad_gerado, t.tad_numero, t.tad_data, 
															t.serie, t.descricao, t.valor_produtos, t.depositario,  nvl(p.nome, p.razao_social) nome, 
															nvl(p.cpf, p.cnpj) cpf, t.endereco_logradouro, t.endereco_bairro, t.endereco_distrito, 
															t.endereco_estado, t.endereco_municipio, t.opiniao, t.arquivo, a.nome arquivo_nome, t.tid 
															from tab_fisc_material_apreendido t, tab_pessoa p, tab_arquivo a, tab_fiscalizacao f where t.depositario = p.id(+) 
															and t.arquivo = a.id(+) and t.fiscalizacao = :fiscalizacao and f.id = t.fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacaoId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						materialApreendido = new MaterialApreendido
						{
							Id = reader.GetValue<int>("id"),
							FiscalizacaoId = reader.GetValue<int>("fiscalizacao"),
							SerieId = reader.GetValue<int>("serie"),
                            //IsApreendido = reader.GetValue<bool>("houve_material"),
                            //NumeroTad = reader.GetValue<string>("tad_numero"),
							ValorProdutos = reader.GetValue<string>("valor_produtos"),
							Descricao = reader.GetValue<string>("descricao"),
							Opiniao = reader.GetValue<string>("opiniao"),
							FiscalizacaoSituacaoId = reader.GetValue<int>("situacao_id"),
							Tid = reader.GetValue<string>("tid"),
							Depositario = new MaterialApreendidoDepositario
							{
								Id = reader.GetValue<int>("depositario"),
								NomeRazaoSocial = reader.GetValue<string>("nome"),
								CPFCNPJ = reader.GetValue<string>("cpf"),
								Logradouro = reader.GetValue<string>("endereco_logradouro"),
								Bairro = reader.GetValue<string>("endereco_bairro"),
								Distrito = reader.GetValue<string>("endereco_distrito"),
								Estado = reader.GetValue<int>("endereco_estado"),
								Municipio = reader.GetValue<int>("endereco_municipio")
							}
						};

						materialApreendido.Arquivo = new Arquivo
						{
							Id = reader.GetValue<int>("arquivo"),
							Nome = reader.GetValue<string>("arquivo_nome")
						};

						if (!Convert.IsDBNull(reader["tad_gerado"]))
						{
							materialApreendido.IsTadGeradoSistema = reader.GetValue<bool>("tad_gerado");
						}

						if (!Convert.IsDBNull(reader["tad_data"]))
						{
							materialApreendido.DataLavratura.DataTexto = reader.GetValue<string>("tad_data");
						}
					}
					reader.Close();
				}

				#endregion

				#region Materiais

				comando = bancoDeDados.CriarComando(@" select tfmam.id, tfmam.material_apreendido, tfmam.tipo, lfmat.texto tipo_texto, tfmam.especificacao, tfmam.tid from tab_fisc_mater_apree_material
					tfmam, lov_fisc_mate_apreendido_tipo lfmat where tfmam.tipo = lfmat.id and tfmam.material_apreendido = :material_apreendido ", EsquemaBanco);
				comando.AdicionarParametroEntrada("material_apreendido", materialApreendido.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					materialApreendido.Materiais = new List<MaterialApreendidoMaterial>();
					while (reader.Read())
					{
						materialApreendido.Materiais.Add(new MaterialApreendidoMaterial
						{
							Id = reader.GetValue<int>("id"),
							TipoId = reader.GetValue<int>("tipo"),
							TipoTexto = reader.GetValue<string>("tipo_texto"),
							Especificacao = reader.GetValue<string>("especificacao")
						});
					}
					reader.Close();
				}

				#endregion
			}

			return materialApreendido;
		}

		internal int ObterID(int fiscalizacao, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from tab_fisc_material_apreendido t where t.fiscalizacao = :fiscalizacao", EsquemaBanco);

				comando.AdicionarParametroEntrada("fiscalizacao", fiscalizacao, DbType.Int32);

				var retorno = bancoDeDados.ExecutarScalar(comando);

				return (retorno == null || Convert.IsDBNull(retorno)) ? 0 : Convert.ToInt32(retorno);
			}
		}

        internal List<ProdutoApreendidoLst> ObterProdutosApreendidosLst(BancoDeDados banco = null)
        {
            List<ProdutoApreendidoLst> listaProdutos = new List<ProdutoApreendidoLst>();

            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                Comando comando = null;

                bancoDeDados.IniciarTransacao();

                comando = bancoDeDados.CriarComando(@"select id, item, unidade, ativo, tid
                                                      from cnf_fisc_infracao_produto
                                                      where ativo = 1
                                                      order by item, unidade", EsquemaBanco);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        ProdutoApreendidoLst produto = new ProdutoApreendidoLst();

                        produto.Id = reader.GetValue<int>("id");
                        produto.Texto = reader.GetValue<string>("item");
                        produto.Unidade = reader.GetValue<string>("unidade");
                        produto.IsAtivo = reader.GetValue<bool>("ativo");

                        listaProdutos.Add(produto);
                    }
                }
            }

            return listaProdutos;
        }

		#endregion
	}
}