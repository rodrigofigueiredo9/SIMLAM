using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloInformacaoCorte;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloInformacaoCorte.Data
{
	public class InformacaoCorteInternoDa
	{
		#region Propriedades

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		private String EsquemaBanco { get; set; }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		#endregion

		public InformacaoCorteInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter/Filtrar

		internal InformacaoCorte Obter(int id, bool simplificado = false, BancoDeDados banco = null)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Informação de Corte

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.empreendimento, c.data_informacao, c.area_flor_plantada
				from {0}crt_informacao_corte c where c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.EmpreendimentoId = reader.GetValue<int>("empreendimento");
						caracterizacao.DataInformacao = new DateTecno() { Data = reader.GetValue<DateTime>("data_informacao") };
						caracterizacao.AreaFlorestaPlantada = reader.GetValue<decimal>("area_flor_plantada");
						caracterizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Informação de Corte Licença

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.corte_id, c.licenca, c.tipo_licenca, c.numero_licenca, c.atividade, c.area_licenca, c.data_vencimento
				from {0}crt_inf_corte_licenca c
				where c.corte_id = :corte_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.InformacaoCorteLicenca.Add(new InformacaoCorteLicenca()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							Corte = reader.GetValue<int>("corte_id"),
							Licenca = reader.GetValue<int>("licenca"),
							TipoLicenca = reader.GetValue<string>("tipo_licenca"),
							NumeroLicenca = reader.GetValue<string>("numero_licenca"),
							Atividade = reader.GetValue<string>("atividade"),
							AreaLicenca = reader.GetValue<decimal>("area_licenca"),
							DataVencimento = new DateTecno() { Data = reader.GetValue<DateTime>("data_vencimento") }
						});
					}

					reader.Close();
				}

				#endregion

				#region Informação de Corte Tipo

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.corte_id, c.tipo_corte, c.especie, c.area_corte, c.idade_plantio
				from {0}crt_inf_corte_tipo c
				where c.corte_id = :corte_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("corte_id", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						var informacaoCorteTipo = new InformacaoCorteTipo()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							Corte = reader.GetValue<int>("corte_id"),
							TipoCorte = reader.GetValue<int>("tipo_corte"),
							TipoCorteTexto = Enum.ToObject(typeof(eTipoCorte), reader.GetValue<int>("tipo_corte")).ToDescription(),
							EspecieInformada = reader.GetValue<int>("especie"),
							EspecieInformadaTexto = Enum.ToObject(typeof(eEspecieInformada), reader.GetValue<int>("especie")).ToDescription(),
							AreaCorte = reader.GetValue<decimal>("area_corte"),
							IdadePlantio = reader.GetValue<int>("idade_plantio")
						};

						#region Informação Corte Destinação

						comando = bancoDeDados.CriarComando(@"
						select c.id, c.tid, c.tipo_corte_id, c.dest_material, c.produto, c.quantidade, c.inf_codigo_sefaz,
							lv.texto as dest_material_texto, lvp.texto as produto_texto
						from {0}crt_inf_corte_dest_material c
						left join {0}lov_crt_inf_corte_inf_dest_mat lv
							on(c.dest_material = lv.id)
						left join {0}lov_crt_produto_inf_corte lvp
							on(c.produto = lvp.id)
						where c.tipo_corte_id = :tipo_corte_id", EsquemaBanco);

						comando.AdicionarParametroEntrada("tipo_corte_id", informacaoCorteTipo.Id, DbType.Int32);

						using (IDataReader readerDestinacao = bancoDeDados.ExecutarReader(comando))
						{
							while (readerDestinacao.Read())
							{
								informacaoCorteTipo.InformacaoCorteDestinacao.Add(new InformacaoCorteDestinacao()
								{
									Id = readerDestinacao.GetValue<int>("id"),
									Tid = readerDestinacao.GetValue<string>("tid"),
									TipoCorteId = readerDestinacao.GetValue<int>("tipo_corte_id"),
									DestinacaoMaterial = readerDestinacao.GetValue<int>("dest_material"),
									DestinacaoMaterialTexto = readerDestinacao.GetValue<string>("dest_material_texto"),
									Produto = readerDestinacao.GetValue<int>("produto"),
									ProdutoTexto = readerDestinacao.GetValue<string>("produto_texto"),
									Quantidade = readerDestinacao.GetValue<int>("quantidade"),
									CodigoSefazId = readerDestinacao.GetValue<int>("inf_codigo_sefaz")
								});
							}

							readerDestinacao.Close();
						}

						#endregion

						caracterizacao.InformacaoCorteTipo.Add(informacaoCorteTipo);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		internal InformacaoCorte ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			InformacaoCorte caracterizacao = new InformacaoCorte();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_informacao_corte t where t.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), simplificado, bancoDeDados);
				}
			}

			return caracterizacao;
		}

		#endregion
	}
}