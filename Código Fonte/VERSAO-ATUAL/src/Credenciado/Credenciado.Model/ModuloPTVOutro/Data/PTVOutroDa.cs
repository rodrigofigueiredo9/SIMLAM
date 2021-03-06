﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTVOutro;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Entities.Etx.ModuloArquivo;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTVOutro.Data
{
	public class PTVOutroDa
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
		private string Esquema { get; set; }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		#region DML

		internal void Salvar(PTVOutro PTV, BancoDeDados banco)
		{
			if (PTV == null)
			{
				throw new Exception("PTV outro estado é nulo.");
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

        internal void Editar(PTVOutro PTV, BancoDeDados banco)
        {
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
            {
                bancoDeDados.IniciarTransacao();
                Comando comando = null;

                #region Update

                comando = bancoDeDados.CriarComando(@"
				    update tab_ptv_outrouf set
					tid=:tid,
					numero=:numero,
					data_emissao=:data_emissao,
					data_ativacao=:data_ativacao,
					data_cancelamento=:data_cancelamento,
					situacao=:situacao,
					interessado=:interessado,
					interessado_cnpj_cpf=:interessado_cnpj_cpf,
					interessado_endereco=:interessado_endereco,
					interessado_estado=:interessado_estado,
                    interessado_municipio=:interessado_municipio,
					destinatario=:destinatario,
					valido_ate=:valido_ate,
					resp_tecnico=:resp_tecnico,
					resp_tecnico_num_hab=:resp_tecnico_num_hab,
					estado=:estado,
					municipio=:municipio,
					credenciado=:credenciado,
                    declaracao_adicional=:declaracao_adicional,
                    declaracao_adicional_formatado=:declaracao_adicional_formatado
			        where id=:id", Esquema);

                comando.AdicionarParametroEntrada("id", PTV.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("numero", PTV.Numero > 0 ? PTV.Numero : (object)DBNull.Value, DbType.Int64);
                comando.AdicionarParametroEntrada("data_emissao", PTV.DataEmissao.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("data_ativacao", PTV.DataAtivacao.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("data_cancelamento", PTV.DataCancelamento.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("situacao", PTV.Situacao, DbType.Int32);
                comando.AdicionarParametroEntrada("interessado", DbType.String, 120, PTV.Interessado);
                comando.AdicionarParametroEntrada("interessado_cnpj_cpf", DbType.String, 20, PTV.InteressadoCnpjCpf);
                comando.AdicionarParametroEntrada("interessado_endereco", DbType.String, 200, PTV.InteressadoEndereco);
                comando.AdicionarParametroEntrada("interessado_estado", PTV.InteressadoEstadoId, DbType.Int32);
                comando.AdicionarParametroEntrada("interessado_municipio", PTV.InteressadoMunicipioId, DbType.Int32);
                comando.AdicionarParametroEntrada("destinatario", PTV.DestinatarioID, DbType.Int32);
                comando.AdicionarParametroEntrada("valido_ate", PTV.ValidoAte.Data, DbType.DateTime);
                comando.AdicionarParametroEntrada("resp_tecnico", DbType.String, 120, PTV.RespTecnico);
                comando.AdicionarParametroEntrada("resp_tecnico_num_hab", DbType.String, 8, PTV.RespTecnicoNumHab);
                comando.AdicionarParametroEntrada("estado", PTV.Estado, DbType.Int32);
                comando.AdicionarParametroEntrada("municipio", PTV.Municipio, DbType.Int32);
                comando.AdicionarParametroEntrada("credenciado", PTV.CredenciadoId, DbType.Int32);
                comando.AdicionarParametroEntrada("declaracao_adicional", PTV.DeclaracaoAdicional, DbType.String);
                comando.AdicionarParametroEntrada("declaracao_adicional_formatado", PTV.DeclaracaoAdicional, DbType.String);

             
                bancoDeDados.ExecutarNonQuery(comando);
                #endregion

                #region Limpar Dados

                comando = bancoDeDados.CriarComando(@"delete from tab_ptv_outrouf_produto ", UsuarioCredenciado);
                comando.DbCommand.CommandText += String.Format("where ptv = :ptv {0}",
                comando.AdicionarNotIn("and", "id", DbType.Int32, PTV.Produtos.Select(p => p.Id).ToList()));
                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                comando = bancoDeDados.CriarComando(@"delete from tab_ptv_outro_arquivo ", UsuarioCredenciado);
                comando.DbCommand.CommandText += String.Format("where ptv = :ptv {0}",
                comando.AdicionarNotIn("and", "id", DbType.Int32, PTV.Anexos.Select(x => x.Id).ToList()));
                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);


                comando = bancoDeDados.CriarComando(@"delete from tab_ptv_outrouf_declaracao ", UsuarioCredenciado);
                comando.DbCommand.CommandText += "where ptv = :ptv";
                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                bancoDeDados.ExecutarNonQuery(comando);

                #endregion

                #region Produto PTV
                //
                //
                PTV.Produtos.ForEach(item =>
                {
                    if (item.Id > 0)
                    {
                        comando = bancoDeDados.CriarComando(@"
						update tab_ptv_outrouf_produto set tid = :tid, ptv = :ptv, origem_tipo = :origem_tipo, numero_origem = :numero_origem, 
						cultura = :cultura,cultivar = :cultivar, quantidade = :quantidade, unidade_medida = :unidade_medida where id = :id", UsuarioCredenciado);

                        comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
                        comando.AdicionarParametroEntrada("ptv", item.PTV, DbType.Int32);
                    }
                    else
                    {
                        comando = bancoDeDados.CriarComando(@"
						insert into tab_ptv_outrouf_produto(id, tid, ptv, origem_tipo, numero_origem, cultura, cultivar, quantidade, unidade_medida)
						values(seq_tab_ptv_outrouf_prod.nextval,:tid,:ptv,:origem_tipo,:numero_origem,:cultura,:cultivar,:quantidade,:unidade_medida)", UsuarioCredenciado);

                        comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                    }
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                    comando.AdicionarParametroEntrada("origem_tipo", item.OrigemTipo, DbType.Int32);
                    comando.AdicionarParametroEntrada("numero_origem", item.OrigemNumero, DbType.Int64);
                    comando.AdicionarParametroEntrada("cultura", item.Cultura, DbType.Int32);
                    comando.AdicionarParametroEntrada("cultivar", item.Cultivar, DbType.Int32);
                    comando.AdicionarParametroEntrada("quantidade", item.Quantidade, DbType.Decimal);
                    comando.AdicionarParametroEntrada("unidade_medida", item.UnidadeMedida, DbType.Int32);

                    bancoDeDados.ExecutarNonQuery(comando);
                });

                #endregion

                #region Declaracao PTV

                comando = bancoDeDados.CriarComando(@"
				insert into tab_ptv_outrouf_declaracao
					(id, tid, ptv, declaracao_adicional, cultivar, praga)
				values 
					(seq_tab_ptv_outrouf_declaracao.nextval, :tid, :ptv, :declaracao, :cultivar, :praga)", Esquema);

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("declaracao", DbType.Int32);
                comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
                comando.AdicionarParametroEntrada("praga", DbType.Int32);

                PTV.Declaracoes.ForEach(item =>
                {
                    comando.SetarValorParametro("declaracao", item.IdDeclaracao);
                    comando.SetarValorParametro("cultivar", item.IdCultivar);
                    comando.SetarValorParametro("praga", item.IdPraga);

                    bancoDeDados.ExecutarNonQuery(comando);
                });

                #endregion

                #region Arquivos

                if (PTV.Anexos != null && PTV.Anexos.Count > 0)
			    {
					foreach (Anexo item in PTV.Anexos)
					{
						if (item.Id > 0)
						{
							comando = bancoDeDados.CriarComando(@"update tab_ptv_outro_arquivo a set a.ptv = :ptv, a.arquivo = :arquivo, 
							a.ordem = :ordem, a.descricao = :descricao, a.tid = :tid where a.id = :id", EsquemaBanco);
							comando.AdicionarParametroEntrada("id", item.Id, DbType.Int32);
						}
						else
						{
                            comando = bancoDeDados.CriarComando(@"insert into tab_ptv_outro_arquivo a (id, ptv, arquivo, ordem, descricao, tid) 
							values ({0}seq_ptv_arquivo.nextval, :ptv, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);
						}

						comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("arquivo", item.Arquivo.Id, DbType.Int32);
						comando.AdicionarParametroEntrada("ordem", item.Ordem, DbType.Int32);
						comando.AdicionarParametroEntrada("descricao", DbType.String, 100, item.Descricao);
						comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

						bancoDeDados.ExecutarNonQuery(comando);
					}
                }

                #endregion

                Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptvoutro, eHistoricoAcao.atualizar, bancoDeDados);

                bancoDeDados.Commit();
            }

        }

		internal void Criar(PTVOutro PTV, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();
				Comando comando = null;

				#region Insert

				comando = bancoDeDados.CriarComando(@"
				insert into tab_ptv_outrouf
					(id,
					tid,
					numero,
					data_emissao,
					data_ativacao,
					data_cancelamento,
					situacao,
					interessado,
					interessado_cnpj_cpf,
					interessado_endereco,
					interessado_estado,
					interessado_municipio,
					destinatario,
					valido_ate,
					resp_tecnico,
					resp_tecnico_num_hab,
					estado,
					municipio,
					credenciado,
                    declaracao_adicional,
                    declaracao_adicional_formatado)
				values
					(seq_tab_ptv_outrouf.nextval,
					:tid,
					:numero,
					:data_emissao,
					:data_ativacao,
					:data_cancelamento,
					:situacao,
					:interessado,
					:interessado_cnpj_cpf,
					:interessado_endereco,
					:interessado_estado,
					:interessado_municipio,
					:destinatario,
					:valido_ate,
					:resp_tecnico,
					:resp_tecnico_num_hab,
					:estado,
					:municipio,
					:credenciado,
                    :declaracao_adicional,
                    :declaracao_adicional_formatado) returning id into :id", Esquema);
				#endregion

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("numero", PTV.Numero > 0 ? PTV.Numero : (object)DBNull.Value, DbType.Int64);
				comando.AdicionarParametroEntrada("data_emissao", PTV.DataEmissao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_ativacao", PTV.DataAtivacao.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("data_cancelamento", PTV.DataCancelamento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("situacao", PTV.Situacao, DbType.Int32);
				comando.AdicionarParametroEntrada("interessado", DbType.String, 120, PTV.Interessado);
				comando.AdicionarParametroEntrada("interessado_cnpj_cpf", DbType.String, 20, PTV.InteressadoCnpjCpf);
				comando.AdicionarParametroEntrada("interessado_endereco", DbType.String, 200, PTV.InteressadoEndereco);
				comando.AdicionarParametroEntrada("interessado_estado", PTV.InteressadoEstadoId, DbType.Int32);
				comando.AdicionarParametroEntrada("interessado_municipio", PTV.InteressadoMunicipioId, DbType.Int32);
				comando.AdicionarParametroEntrada("destinatario", PTV.DestinatarioID, DbType.Int32);
				comando.AdicionarParametroEntrada("valido_ate", PTV.ValidoAte.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("resp_tecnico", DbType.String, 120, PTV.RespTecnico);
				comando.AdicionarParametroEntrada("resp_tecnico_num_hab", DbType.String, 8, PTV.RespTecnicoNumHab);
				comando.AdicionarParametroEntrada("estado", PTV.Estado, DbType.Int32);
				comando.AdicionarParametroEntrada("municipio", PTV.Municipio, DbType.Int32);
				comando.AdicionarParametroEntrada("credenciado", PTV.CredenciadoId, DbType.Int32);
                comando.AdicionarParametroEntrada("declaracao_adicional", PTV.DeclaracaoAdicional, DbType.String);
                comando.AdicionarParametroEntrada("declaracao_adicional_formatado", PTV.DeclaracaoAdicional, DbType.String);

				//ID de retorno
				comando.AdicionarParametroSaida("id", DbType.Int32);

				bancoDeDados.ExecutarScalar(comando);

				PTV.Id = Convert.ToInt32(comando.ObterValorParametro("id"));

				#region Produto PTV
                //
                //
				comando = bancoDeDados.CriarComando(@"
				insert into tab_ptv_outrouf_produto
					(id, tid, ptv, origem_tipo, cultura, cultivar, quantidade, unidade_medida, tipo_producao, numero_origem)
				values 
					(seq_tab_ptv_outrouf_prod.nextval, :tid, :ptv, :origem_tipo, :cultura, :cultivar, :quantidade, :unidade_medida, :tipo_producao, :numero_origem)", Esquema);

				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("origem_tipo", DbType.Int32);
				
				comando.AdicionarParametroEntrada("cultura", DbType.Int32);
				comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
				comando.AdicionarParametroEntrada("quantidade", DbType.Decimal);
                comando.AdicionarParametroEntrada("unidade_medida", DbType.Int32);
                comando.AdicionarParametroEntrada("numero_origem", DbType.String);
                comando.AdicionarParametroEntrada("tipo_producao", DbType.Int32);

				PTV.Produtos.ForEach(item =>
				{
                    comando.SetarValorParametro("origem_tipo", item.OrigemTipo);
                    comando.SetarValorParametro("cultura", item.Cultura);
                    comando.SetarValorParametro("cultivar", item.Cultivar);
                    comando.SetarValorParametro("quantidade", item.Quantidade);
                    comando.SetarValorParametro("unidade_medida", item.UnidadeMedida);
                    comando.SetarValorParametro("numero_origem", item.OrigemNumero);
                    comando.SetarValorParametro("tipo_producao", item.ProducaoTipo);

					bancoDeDados.ExecutarNonQuery(comando);
				});

				#endregion

                #region Declaracao PTV

                comando = bancoDeDados.CriarComando(@"
				insert into tab_ptv_outrouf_declaracao
					(id, tid, ptv, declaracao_adicional, cultivar, praga)
				values 
					(seq_tab_ptv_outrouf_declaracao.nextval, :tid, :ptv, :declaracao, :cultivar, :praga)", Esquema);

                comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());
                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                comando.AdicionarParametroEntrada("declaracao",  DbType.Int32);
                comando.AdicionarParametroEntrada("cultivar", DbType.Int32);
                comando.AdicionarParametroEntrada("praga", DbType.Int32);
		       
                PTV.Declaracoes.ForEach(item =>
                {
                    comando.SetarValorParametro("declaracao", item.IdDeclaracao);
                    comando.SetarValorParametro("cultivar", item.IdCultivar);
                    comando.SetarValorParametro("praga", item.IdPraga);

                    bancoDeDados.ExecutarNonQuery(comando);
                });

                #region Arquivos

                if (PTV.Anexos != null && PTV.Anexos.Count > 0)
                {
                    comando = bancoDeDados.CriarComando(@"insert into tab_ptv_outro_arquivo a (id, ptv, arquivo, ordem, descricao, tid) 
					values ({0}seq_ptv_arquivo.nextval, :ptv, :arquivo, :ordem, :descricao, :tid)", EsquemaBanco);

                    comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                    comando.AdicionarParametroEntrada("arquivo", DbType.Int32);
                    comando.AdicionarParametroEntrada("ordem", DbType.Int32);
                    comando.AdicionarParametroEntrada("descricao", DbType.String, 100);
                    comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

                    foreach (Anexo item in PTV.Anexos)
                    {
                        comando.SetarValorParametro("arquivo", item.Arquivo.Id);
                        comando.SetarValorParametro("ordem", item.Ordem);
                        comando.SetarValorParametro("descricao", item.Descricao);
                        bancoDeDados.ExecutarNonQuery(comando);
                    }
                }

                #endregion
            

                #endregion

                Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptvoutro, eHistoricoAcao.criar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

        internal List<ListaValor> ObterListaDeclaracao(int pragaId, int[] cultivarId)
        {
            List<ListaValor> lst = new List<ListaValor>();

            string strCultivares = string.Join(",", cultivarId
                                           .Select(x => string.Format("'{0}'", x.ToString())));

            string Consulta = string.Format(@"select distinct d.id as id, d.texto as texto from lov_cultivar_declara_adicional d,tab_cultivar_configuracao c 
                                                where c.declaracao_adicional = d.id and d.outro_estado='1' and c.praga={0} and c.cultivar in ({1}) ", pragaId, strCultivares);
            IEnumerable<IDataReader> daReader = DaHelper.ObterLista(Consulta);
            foreach (var item in daReader)
            {
                lst.Add(new ListaValor()
                {
                    Id = Convert.ToInt32(item["id"]),
                    Texto = item["texto"].ToString(),
                    IsAtivo = true
                });
            }

            return lst;
        }

		internal void PTVCancelar(PTVOutro PTV, BancoDeDados banco)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, Esquema))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"
				update {0}tab_ptv_outrouf p
					set p.tid               = :tid,
						p.situacao          = :situacao,
						p.data_cancelamento = :data_cancelamento
					where p.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", PTV.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("situacao", (int)ePTVOutroSituacao.Cancelado, DbType.Int32);
				comando.AdicionarParametroEntrada("data_cancelamento", PTV.DataCancelamento.Data, DbType.DateTime);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);

				Historico.Gerar(PTV.Id, eHistoricoArtefato.emitirptvoutro, eHistoricoAcao.cancelar, bancoDeDados);

				bancoDeDados.Commit();
			}
		}

		#endregion

		#region Obter /Filtros

		internal PTVOutro ObterPorNumero(long numero, bool simplificado = false, bool credenciado = true, BancoDeDados banco = null)
		{
			PTVOutro retorno = new PTVOutro();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from {0}tab_ptv_outrouf where numero = :numero", Esquema);
				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);

				if (credenciado)
				{
					comando.DbCommand.CommandText += " and credenciado = :credenciado";
					comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int64);
				}

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						retorno = new PTVOutro();
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

        internal List<Lista> ObterPragasLista(List<PTVOutroProduto> produtos)
        {
            List<Lista> retorno = new List<Lista>();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando("");

                int count = 0;
                string aux = string.Empty;
                foreach (var item in produtos.GroupBy(p => new { p.Cultivar, p.UnidadeMedida }).Select(g => g.First()))
                {
                    ++count;
                    aux += string.Format("p.id in (select c.praga from tab_cultivar_configuracao c where c.cultivar = :cultivar{0} and c.tipo_producao = :tipo_producao{0}) or ", count);
                    comando.AdicionarParametroEntrada("cultivar" + count, item.Cultivar, DbType.Int32);
                    comando.AdicionarParametroEntrada("tipo_producao" + count, (int)ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedida), DbType.Int32);
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

        internal List<Praga> ObterPragasLista2(List<PTVOutroProduto> produtos)
        {
            List<Praga> retorno = new List<Praga>();
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando("");

                int count = 0;
                string aux = string.Empty;
                foreach (var item in produtos.GroupBy(p => new { p.Cultivar, p.UnidadeMedida }).Select(g => g.First()))
                {
                    ++count;
                    aux += string.Format("p.id in (select c.praga from tab_cultivar_configuracao c where c.cultivar = :cultivar{0} and c.tipo_producao = :tipo_producao{0}) or ", count);
                    comando.AdicionarParametroEntrada("cultivar" + count, item.Cultivar, DbType.Int32);
                    comando.AdicionarParametroEntrada("tipo_producao" + count, (int)ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedida), DbType.Int32);
                }

                aux = aux.Substring(0, aux.Length - 4);
                comando.DbCommand.CommandText = string.Format(@"select distinct p.* from tab_hab_emi_cfo_cfoc h, tab_hab_emi_cfo_cfoc_praga hp, tab_praga p, 
				tab_praga_cultura pc where pc.praga = p.id and p.id = hp.praga and h.id = hp.habilitar_emi_id and h.responsavel = :credenciado and({0})", aux);
                comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    while (reader.Read())
                    {
                        retorno.Add(new Praga()
                        {
                            Id = reader.GetValue<int>("id"),
                            NomeCientifico = reader.GetValue<string>("nome_cientifico"),
                            NomeComum = reader.GetValue<string>("nome_comum")
                        });
                    }

                    reader.Close();
                }
            }

            return retorno;
        }

		internal PTVOutro Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
            PTVOutro PTV = new PTVOutro();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				

				Comando comando = bancoDeDados.CriarComando(@"
				select p.tid                   Tid,
						p.id                    Id,
						p.numero                Numero,
						p.data_emissao          DataEmissaoText,
						p.data_ativacao         DataAtivacaoText,
						p.data_cancelamento     DataCancelamentoText,
						p.situacao              Situacao,
						ls.texto                SituacaoTexto,
						p.interessado           Interessado,
						p.interessado_cnpj_cpf  InteressadoCnpjCpf,
						p.interessado_endereco  InteressadoEndereco,
						p.interessado_estado    InteressadoEstadoId,
						lee.texto               InteressadoEstadoTexto,
						p.interessado_municipio InteressadoMunicipioId,
						lme.texto               InteressadoMunicipioTexto,
						p.destinatario          DestinatarioID,
						p.valido_ate            ValidoAteText,
						p.resp_tecnico          RespTecnico,
						p.resp_tecnico_num_hab  RespTecnicoNumHab,
						p.estado                Estado,
						le.texto                EstadoTexto,
						p.municipio             Municipio,
						lm.texto                MunicipioTexto,
						p.credenciado           CredenciadoId,
                        p.declaracao_adicional  DeclaracaoAdicional,
                        p.declaracao_adicional_formatado    DeclaracaoAdicionalHtml
					from tab_ptv_outrouf               p,
						lov_ptv_situacao              ls,
						lov_estado                    le,
						lov_estado                    lee,
						lov_municipio                 lm,
						lov_municipio                 lme
					where ls.id = p.situacao
					and le.id(+) = p.estado
					and lee.id(+) = p.interessado_estado
					and lm.id(+) = p.municipio
					and lme.id(+) = p.interessado_municipio
					and p.id = :id", Esquema);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				PTV = bancoDeDados.ObterEntity<PTVOutro>(comando);

				if (PTV.Id <= 0 || simplificado)
				{
					return PTV;
				}

				#region PTV Produto

				comando = bancoDeDados.CriarComando(@"
				select pr.id						Id,
					pr.tid							Tid,
					pr.ptv							PTV,
					pr.origem_tipo					OrigemTipo,
					pr.numero_origem				OrigemNumero,
					t.texto							OrigemTipoTexto,
					pr.cultura						Cultura,
					c.texto							CulturaTexto,
					pr.cultivar						Cultivar,
					cc.cultivar						CultivarTexto,
					c.texto || '/' || cc.cultivar	CulturaCultivar,
					pr.quantidade					Quantidade,
					pr.unidade_medida				UnidadeMedida,
					u.texto							UnidadeMedidaTexto,
					pr.tipo_producao				ProducaoTipo,
					ltp.texto						ProducaoTipoTexto
				from tab_ptv_outrouf_produto		pr,
					lov_doc_fitossanitarios_tipo	t,
					tab_cultura						c,
					tab_cultura_cultivar			cc,
					lov_crt_uni_prod_uni_medida		u,
					lov_tipo_producao				ltp
				where t.id = pr.origem_tipo
					and c.id = pr.cultura
					and cc.id = pr.cultivar
					and u.id = pr.unidade_medida
					and pr.tipo_producao = ltp.id(+)
					and pr.ptv = :ptv", Esquema);

				comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);

				PTV.Produtos = bancoDeDados.ObterEntityList<PTVOutroProduto>(comando);

                PTV.Pragas = ObterPragasLista2(PTV.Produtos);

				#endregion

                #region PTV Declaracoes


                comando = bancoDeDados.CriarComando(@"
				select d.praga              IdPraga,
                       p.nome_cientifico    NomeCientifico,
                       p.nome_comum         NomeComum,
                       c.Texto              DeclaracaoAdicional,
                       c.id                 IdDeclaracao,
                       d.cultivar           IdCultivar            
				from 
					tab_ptv_outrouf_declaracao	d,
					lov_cultivar_declara_adicional	c,
                    tab_praga p
				where d.declaracao_adicional = c.id
                    and d.praga = p.id
					and d.ptv = :ptv", Esquema);

                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);

                PTV.Declaracoes = bancoDeDados.ObterEntityList<PTVOutroDeclaracao>(comando);

                #endregion

                #region Arquivos


                string strSQL = @"select count(*)  from 
                                                        tab_ptv_outrouf ptv 
                                                        inner join tab_ptv_arquivo a on ptv.id = a.ptv
                                                        inner join tab_arquivo b on a.arquivo = b.id
                                                        where ptv.id = :ptv order by a.ordem";
                comando = bancoDeDados.CriarComando(strSQL);

                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);
                int Ret = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

                if (Ret == 0)
                    strSQL = @"select a.id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho,
				       a.tid from tab_ptv_outro_arquivo a, tab_arquivo b where a.arquivo = b.id and a.ptv = :ptv order by a.ordem";
                else
                    strSQL = @"select a.id, a.ordem, a.descricao, b.nome, b.extensao, b.id arquivo_id, b.caminho, a.tid  from 
                                                        tab_ptv_outrouf ptv 
                                                        inner join tab_ptv_arquivo a on ptv.id = a.ptv
                                                        inner join tab_arquivo b on a.arquivo = b.id
                                                        where ptv.id = :ptv order by a.ordem";

              

                comando = bancoDeDados.CriarComando(strSQL);

                comando.AdicionarParametroEntrada("ptv", PTV.Id, DbType.Int32);

                using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
                {
                    Anexo item;
                    while (reader.Read())
                    {
                        item = new Anexo();
                        item.Id = Convert.ToInt32(reader["id"]);
                        item.Ordem = Convert.ToInt32(reader["ordem"]);
                        item.Descricao = reader["descricao"].ToString();

                        item.Arquivo.Id = Convert.ToInt32(reader["arquivo_id"]);
                        item.Arquivo.Caminho = reader["caminho"].ToString();
                        item.Arquivo.Nome = reader["nome"].ToString();
                        item.Arquivo.Extensao = reader["extensao"].ToString();

                        item.Tid = reader["tid"].ToString();

                        PTV.Anexos.Add(item);
                    }
                    reader.Close();
                }

                #endregion


                return PTV;

			}



		}

        internal bool VerificaSePTVAssociadoLote(Int64 ptvNumero)
        {
           
            using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
            {
                Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_lote_item t where t.origem_numero = :numero", UsuarioCredenciado);

                comando.AdicionarParametroEntrada("numero", ptvNumero, DbType.Int64);

                return (bancoDeDados.ExecutarScalar<int>(comando) > 0);

            }
        }

		internal bool VerificarNumeroPTV(Int64 ptvNumero)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select count(*) from {0}tab_ptv_outrouf t where t.numero = :numero", EsquemaBanco);

				comando.AdicionarParametroEntrada("numero", ptvNumero, DbType.Int64);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);
			}
		}

		internal List<Lista> ObterCultivar(eDocumentoFitossanitarioTipo origemTipo, int culturaID)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<Lista> retorno = null;
				Comando comando = bancoDeDados.CriarComando(@"select t.id, t.cultivar from {0}tab_cultura_cultivar t where t.cultura = :culturaID", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("culturaID", culturaID, DbType.Int32);

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

		internal List<ListaValor> ObterResponsavelTecnico(int id)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select f.id, f.nome from {0}tab_hab_emi_ptv_operador t, {0}tab_hab_emi_ptv o, {0}tab_funcionario f 
				where t.habilitacao(+) = o.id and f.id = o.funcionario and (o.funcionario = :id or t.funcionario = :id )", Esquema);
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

		internal Resultados<PTVOutroListarResultado> Filtrar(Filtro<PTVOutroListarFiltro> filtro)
		{
			Resultados<PTVOutroListarResultado> retorno = new Resultados<PTVOutroListarResultado>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				string comandtxt = string.Empty;
				string esquemaBanco = (string.IsNullOrEmpty(EsquemaBanco) ? "" : EsquemaBanco + ".");
				Comando comando = bancoDeDados.CriarComando("");

				#region Adicionando Filtros

				comandtxt += comando.FiltroAnd("t.credenciado", "credenciado", filtro.Dados.CredenciadoID);

				comandtxt += comando.FiltroAndLike("t.numero", "numero", filtro.Dados.Numero);

				comandtxt += comando.FiltroAnd("t.situacao", "situacao", filtro.Dados.Situacao);

				comandtxt += comando.FiltroAndLike("d.nome", "nome", filtro.Dados.Destinatario, true, true);

				comandtxt += comando.FiltroAndLike("t.interessado", "interessado", filtro.Dados.Interessado, true, true);

				comandtxt += comando.FiltroAndLike("t.resp_tecnico", "resp_tecnico", filtro.Dados.ResponsavelTecnico, true, true);

				if (!String.IsNullOrEmpty(filtro.Dados.CulturaCultivar))
				{
					comandtxt += comando.FiltroAndLike("c.texto||'/'||cc.cultivar", "cultura_cultivar", filtro.Dados.CulturaCultivar, true, true);
				}

				List<String> ordenar = new List<String>();
				List<String> colunas = new List<String>() { "numero", "interessado", "destinatario", "situacao" };

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
				"select count(*) from (" + String.Format(@"select t.id from tab_ptv_outrouf t, tab_ptv_outrouf_produto p, tab_cultura c, tab_cultura_cultivar cc, lov_ptv_situacao st, tab_destinatario_ptv d 
				where p.ptv(+) = t.id and c.id(+) = p.cultura and cc.id(+) = p.cultivar and st.id = t.situacao and d.id(+) = t.destinatario " + comandtxt + " group by t.id) a ", esquemaBanco);

				retorno.Quantidade = Convert.ToInt32(bancoDeDados.ExecutarScalar(comando));

				comando.AdicionarParametroEntrada("menor", filtro.Menor);
				comando.AdicionarParametroEntrada("maior", filtro.Maior);

                comandtxt = String.Format(@"select t.id, t.numero, st.texto as situacao, st.id as situacao_id, d.nome destinatario, t.interessado from tab_ptv_outrouf t, tab_ptv_outrouf_produto p, tab_cultura c,
				tab_cultura_cultivar cc, lov_ptv_situacao st, tab_destinatario_ptv d
				where p.ptv(+) = t.id and c.id(+) = p.cultura and cc.id(+) = p.cultivar and st.id = t.situacao and d.id(+) = t.destinatario " + comandtxt +
                " group by t.id, t.numero, st.texto, d.nome, t.interessado, st.id  " + DaHelper.Ordenar(colunas, ordenar), esquemaBanco);

				comando.DbCommand.CommandText = @"select * from (select a.*, rownum rnum from ( " + comandtxt + @") a) where rnum <= :maior and rnum >= :menor";

				#endregion

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PTVOutroListarResultado item;
					while (reader.Read())
					{
						item = new PTVOutroListarResultado();
						item.ID = reader.GetValue<int>("id");
						item.Numero = reader.GetValue<string>("numero");
						item.Destinatario = reader.GetValue<string>("destinatario");
                        item.SituacaoID = reader.GetValue<int>("situacao_id");
						item.SituacaoTexto = reader.GetValue<string>("situacao");
						item.Interessado = reader.GetValue<string>("interessado");
						retorno.Itens.Add(item);
					}

					reader.Close();
				}
			}

			return retorno;
		}

		#endregion
	}
}