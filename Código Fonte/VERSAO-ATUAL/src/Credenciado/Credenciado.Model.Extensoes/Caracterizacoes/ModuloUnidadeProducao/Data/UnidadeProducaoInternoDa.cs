using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeProducao;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.EtramiteX.Configuracao.Interno.Extensoes;
using Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloCaracterizacao.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using System.Data;
using Tecnomapas.Blocos.Entities.Interno.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Etx.ModuloGeo;
using Tecnomapas.Blocos.Entities.Interno.ModuloProtocolo;
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloCaracterizacao;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeProducao.Data
{
	public class UnidadeProducaoInternoDa
	{
		#region Propriedades

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		private String EsquemaBanco { get; set; }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		#endregion

		public UnidadeProducaoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter/Filtrar

		internal UnidadeProducao ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeProducao caracterizacao = new UnidadeProducao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_unidade_producao t where t.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		internal UnidadeProducao Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			UnidadeProducao caracterizacao = new UnidadeProducao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.empreendimento, c.possui_cod_propriedade, c.propriedade_codigo, e.codigo empreendimento_codigo, c.local_livro 
				from {0}crt_unidade_producao c, tab_empreendimento e where c.empreendimento = e.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = id;
						caracterizacao.CodigoPropriedade = reader.GetValue<Int64>("propriedade_codigo");
						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						caracterizacao.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
						caracterizacao.LocalLivroDisponivel = reader.GetValue<string>("local_livro");
						caracterizacao.PossuiCodigoPropriedade = reader.GetValue<bool>("possui_cod_propriedade");
						caracterizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Unidades de produção

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.unidade_producao, c.possui_cod_up, c.codigo_up, c.tipo_producao, c.renasem, c.renasem_data_validade, c.area, c.ano_abertura, 
				c.cultura, c.cultivar, tc.texto cultura_texto, cc.cultivar cultivar_nome, c.data_plantio_ano_producao, c.estimativa_quant_ano, c.estimativa_unid_medida 
				from crt_unidade_producao_unidade c, tab_cultura_cultivar cc, tab_cultura tc 
				where cc.id(+) = c.cultivar and tc.id = c.cultura and c.unidade_producao = :unidade_producao", EsquemaBanco);

				comando.AdicionarParametroEntrada("unidade_producao", caracterizacao.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						caracterizacao.UnidadesProducao.Add(new UnidadeProducaoItem()
						{
							Id = reader.GetValue<int>("id"),
							Tid = reader.GetValue<string>("tid"),
							PossuiCodigoUP = reader.GetValue<bool>("possui_cod_up"),
							CodigoUP = reader.GetValue<long>("codigo_up"),
							TipoProducao = reader.GetValue<int>("tipo_producao"),
							RenasemNumero = reader.GetValue<string>("renasem"),
							DataValidadeRenasem = string.IsNullOrEmpty(reader.GetValue<string>("renasem_data_validade")) ? "" : Convert.ToDateTime(reader.GetValue<string>("renasem_data_validade")).ToShortDateString(),
							AreaHA = reader.GetValue<decimal>("area"),
							DataPlantioAnoProducao = reader.GetValue<string>("data_plantio_ano_producao"),
							EstimativaProducaoQuantidadeAno = reader.GetValue<decimal>("estimativa_quant_ano"),
							CultivarId = reader.GetValue<int>("cultivar"),
							CultivarTexto = reader.GetValue<string>("cultivar_nome"),
							CulturaId = reader.GetValue<int>("cultura"),
							CulturaTexto = reader.GetValue<string>("cultura_texto"),
							AnoAbertura = reader.GetValue<string>("ano_abertura")
						});
					}

					reader.Close();
				}

				foreach (var item in caracterizacao.UnidadesProducao)
				{
					#region Coordenadas

					comando = bancoDeDados.CriarComando(@"
					select id, tid, unidade_producao_unidade, tipo_coordenada, datum, easting_utm, northing_utm, fuso_utm,
					hemisferio_utm, municipio from crt_unidade_producao_un_coord where unidade_producao_unidade = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						Coordenada coordenada = null;

						if (reader.Read())
						{
							coordenada = new Coordenada();
							coordenada.Id = reader.GetValue<int>("id");
							coordenada.Tipo.Id = reader.GetValue<int>("tipo_coordenada");
							coordenada.Datum.Id = reader.GetValue<int>("datum");
							coordenada.EastingUtmTexto = reader.GetValue<string>("easting_utm");
							coordenada.NorthingUtmTexto = reader.GetValue<string>("northing_utm");
							coordenada.FusoUtm = reader.GetValue<int>("fuso_utm");
							coordenada.HemisferioUtm = reader.GetValue<int>("hemisferio_utm");
							coordenada.HemisferioUtmTexto = reader.GetValue<string>("hemisferio_utm");

							item.Municipio.Id = reader.GetValue<int>("municipio");
							item.Coordenada = coordenada;
						}

						reader.Close();
					}

					#endregion

					#region Produtores

					comando = bancoDeDados.CriarComando(@"
					select c.id, c.tid, c.produtor, nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj, p.tipo 
					from crt_unidade_prod_un_produtor c, tab_pessoa p where p.id = c.produtor and c.unidade_producao_unidade = :id", EsquemaBanco);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							item.Produtores.Add(new Responsavel()
							{
								Id = reader.GetValue<int>("produtor"),
								NomeRazao = reader.GetValue<string>("nome_razao"),
								CpfCnpj = reader.GetValue<string>("cpf_cnpj"),
								IdRelacionamento = reader.GetValue<int>("id"),
								Tipo = reader.GetValue<int>("tipo"),
								Tid = reader.GetValue<string>("tid")
							});
						}

						reader.Close();
					}

					#endregion

					#region Responsáveis Técnicos

					comando = bancoDeDados.CriarComando(@"
					select c.id, c.tid, c.unidade_producao_unidade, c.responsavel_tecnico, nvl(p.nome, p.razao_social) nome_razao, nvl(p.cpf, p.cnpj) cpf_cnpj, 
					pf.texto profissao, oc.orgao_sigla, pp.registro, c.numero_hab_cfo_cfoc, c.numero_art, c.art_cargo_funcao, c.data_validade_art, 
					(select h.extensao_habilitacao from tab_hab_emi_cfo_cfoc h where h.responsavel = c.responsavel_tecnico) extensao_habilitacao 
					from {0}crt_unidade_prod_un_resp_tec c, {0}tab_credenciado tc, {1}tab_pessoa p, {1}tab_pessoa_profissao pp, {0}tab_profissao pf, {0}tab_orgao_classe oc 
					where tc.id = c.responsavel_tecnico and p.id = tc.pessoa and pp.pessoa(+) = p.id and pf.id(+)  = pp.profissao and oc.id(+) = pp.orgao_classe 
					and c.unidade_producao_unidade = :id", EsquemaBanco, EsquemaCredenciadoBanco);

					comando.AdicionarParametroEntrada("id", item.Id, DbType.String);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						ResponsavelTecnico responsavel = null;

						while (reader.Read())
						{
							responsavel = new ResponsavelTecnico();
							responsavel.IdRelacionamento = reader.GetValue<int>("id");
							responsavel.Id = reader.GetValue<int>("responsavel_tecnico");
							responsavel.NomeRazao = reader.GetValue<string>("nome_razao");
							responsavel.CpfCnpj = reader.GetValue<string>("cpf_cnpj");
							responsavel.CFONumero = reader.GetValue<string>("numero_hab_cfo_cfoc");
							responsavel.ArtCargoFuncao = reader.GetValue<bool>("art_cargo_funcao");
							responsavel.NumeroArt = reader.GetValue<string>("numero_art");

							responsavel.ProfissaoTexto = reader.GetValue<string>("profissao");
							responsavel.OrgaoClasseSigla = reader.GetValue<string>("orgao_sigla");
							responsavel.NumeroRegistro = reader.GetValue<string>("registro");

							responsavel.DataValidadeART = reader.GetValue<string>("data_validade_art");
							if (!string.IsNullOrEmpty(responsavel.DataValidadeART))
							{
								responsavel.DataValidadeART = Convert.ToDateTime(responsavel.DataValidadeART).ToShortDateString();
							}

							if (Convert.ToBoolean(reader.GetValue<int>("extensao_habilitacao")))
							{
								responsavel.CFONumero += "-ES";
							}

							item.ResponsaveisTecnicos.Add(responsavel);
						}

						reader.Close();
					}

					#endregion
				}

				#endregion
			}

			return caracterizacao;
		}

		#endregion
	}
}