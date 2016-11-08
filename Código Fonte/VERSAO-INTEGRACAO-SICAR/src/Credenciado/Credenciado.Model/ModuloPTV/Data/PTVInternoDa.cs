using System;
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
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloPTV.Data
{
	public class PTVInternoDa
	{
		#region Propriedades

		private string Esquema { get; set; }

		#endregion

		#region Obter /Filtros

		internal PTV Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				PTV PTV = new PTV();

				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.tipo_numero, p.numero, p.data_emissao, p.situacao,
				lst.texto situacao_texto, p.empreendimento, p.responsavel_emp, em.denominador, p.partida_lacrada_origem, p.numero_lacre,
				p.numero_porao, p.numero_container, p.destinatario, p.possui_laudo_laboratorial, p.tipo_transporte, 
				p.veiculo_identificacao_numero, p.rota_transito_definida, p.itinerario, p.apresentacao_nota_fiscal,
				p.numero_nota_fiscal, p.valido_ate, p.responsavel_tecnico, f.nome responsavel_tecnico_nome, p.municipio_emissao 
				from {0}tab_ptv p, {0}tab_empreendimento em, lov_ptv_situacao lst, {0}tab_funcionario f
				where em.id = p.empreendimento and lst.id = p.situacao and p.responsavel_tecnico = f.id and p.id = :id", Esquema);

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
					}

					reader.Close();
				}

				if (PTV.Id <= 0 || simplificado)
				{
					return PTV;
				}

				#region PTV Produto

				comando = bancoDeDados.CriarComando(@"select pr.id,
															 pr.tid,
															 pr.ptv,
															 pr.origem_tipo,
															 pr.origem,
															 case pr.origem_tipo 
															    when 1 then (select t.numero from cre_cfo t where t.id = pr.origem) 
															    when 2 then (select t.numero from cre_cfoc t where t.id = pr.origem) 
															    when 3 then (select t.numero from tab_ptv t where t.id = pr.origem) 
																when 4 then (select t.numero from tab_ptv_outrouf t where t.id = pr.origem) 
															 else pr.numero_origem end as origem_texto,
															 pr.numero_origem,
															 t.texto tipo_origem_texto,
															 pr.cultura,
															 pr.cultivar,
															 c.texto ||'/'||cc.cultivar as cultura_cultivar,
															 pr.quantidade,
															 pr.unidade_medida,
															 u.texto unidade_medida_texto
														from tab_ptv_produto pr, lov_doc_fitossanitarios_tipo t, tab_cultura c, tab_cultura_cultivar cc, lov_crt_uni_prod_uni_medida  u
														where t.id = pr.origem_tipo  
														  and c.id = pr.cultura
														  and cc.id = pr.cultivar
														  and u.id = pr.unidade_medida    
														  and pr.ptv = :ptv", Esquema);

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
							UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto")
						});
					}

					reader.Close();
				}
				#endregion

				return PTV;
			}
		}
		internal PTV ObterPorNumero(long numero, bool simplificado, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				PTV PTV = new PTV();

				Comando comando = bancoDeDados.CriarComando(@"select p.id, p.tid, p.tipo_numero, p.numero, p.data_emissao, p.situacao,
				lst.texto situacao_texto, p.empreendimento, p.responsavel_emp, em.denominador, p.partida_lacrada_origem, p.numero_lacre,
				p.numero_porao, p.numero_container, p.destinatario, p.possui_laudo_laboratorial, p.tipo_transporte, 
				p.veiculo_identificacao_numero, p.rota_transito_definida, p.itinerario, p.apresentacao_nota_fiscal,
				p.numero_nota_fiscal, p.valido_ate, p.responsavel_tecnico, f.nome responsavel_tecnico_nome, p.municipio_emissao 
				from {0}tab_ptv p, {0}tab_empreendimento em, lov_ptv_situacao lst, {0}tab_funcionario f
				where em.id = p.empreendimento and lst.id = p.situacao and p.responsavel_tecnico = f.id and p.numero = :numero", Esquema);

				comando.AdicionarParametroEntrada("numero", numero, DbType.Int64);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						PTV.Id = reader.GetValue<int>("id"); ;
						PTV.Tid = reader.GetValue<string>("tid");
						PTV.Numero = Convert.ToInt64(reader["numero"]);
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
					}

					reader.Close();
				}

				if (PTV.Id <= 0 || simplificado)
				{
					return PTV;
				}

				#region PTV Produto

				comando = bancoDeDados.CriarComando(@"select pr.id,
															 pr.tid,
															 pr.ptv,
															 pr.origem_tipo,
															 pr.origem,
															 case pr.origem_tipo 
															    when 1 then (select t.numero from cre_cfo t where t.id = pr.origem) 
															    when 2 then (select t.numero from cre_cfoc t where t.id = pr.origem) 
															    when 3 then (select t.numero from tab_ptv t where t.id = pr.origem) 
																when 4 then (select t.numero from tab_ptv_outrouf t where t.id = pr.origem) 
															 else pr.numero_origem end as origem_texto,
															 pr.numero_origem,
															 t.texto tipo_origem_texto,
															 pr.cultura,
															 pr.cultivar,
															 c.texto cultura_texto,
															 cc.cultivar cultivar_texto,
															 c.texto ||'/'||cc.cultivar as cultura_cultivar,
															 pr.quantidade,
															 pr.unidade_medida,
															 u.texto unidade_medida_texto
														from tab_ptv_produto pr, lov_doc_fitossanitarios_tipo t, tab_cultura c, tab_cultura_cultivar cc, lov_crt_uni_prod_uni_medida  u
														where t.id = pr.origem_tipo  
														  and c.id = pr.cultura
														  and cc.id = pr.cultivar
														  and u.id = pr.unidade_medida    
														  and pr.ptv = :ptv", Esquema);

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
							CulturaTexto = reader.GetValue<string>("cultura_texto"),
							Cultivar = reader.GetValue<int>("cultivar"),
							CultivarTexto = reader.GetValue<string>("cultivar_texto"),
							CulturaCultivar = reader.GetValue<string>("cultura_cultivar"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							UnidadeMedida = reader.GetValue<int>("unidade_medida"),
							UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto")
						});
					}

					reader.Close();
				}
				#endregion

				return PTV;
			}
		}

		#endregion
	}
}