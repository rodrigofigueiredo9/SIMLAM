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
using Tecnomapas.Blocos.Entities.Interno.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Cultura;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using System.Web;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Extensoes.Caracterizacoes.ModuloUnidadeConsolidacao.Data
{
	public class UnidadeConsolidacaoInternoDa
	{
		#region Propriedades

		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		private String EsquemaBanco { get; set; }

		private String EsquemaCredenciadoBanco { get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		#endregion

		public UnidadeConsolidacaoInternoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;

			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter/Filtrar

		internal UnidadeConsolidacao ObterPorEmpreendimento(int empreendimentoId, bool simplificado = false, BancoDeDados banco = null)
		{
			UnidadeConsolidacao caracterizacao = new UnidadeConsolidacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.id from {0}crt_unidade_consolidacao t where t.empreendimento = :empreendimento", EsquemaBanco);
				comando.AdicionarParametroEntrada("empreendimento", empreendimentoId, DbType.Int32);

				object valor = bancoDeDados.ExecutarScalar(comando);

				if (valor != null && !Convert.IsDBNull(valor))
				{
					caracterizacao = Obter(Convert.ToInt32(valor), bancoDeDados, simplificado);
				}
			}

			return caracterizacao;
		}

		private UnidadeConsolidacao Obter(int id, BancoDeDados banco = null, bool simplificado = false)
		{
			UnidadeConsolidacao caracterizacao = new UnidadeConsolidacao();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				#region Unidade de Produção

				Comando comando = bancoDeDados.CriarComando(@"
															select c.id,c.possui_codigo_uc, c.codigo_uc, c.local_livro_disponivel, c.tipo_apresentacao_produto, c.tid, 
															c.empreendimento, e.codigo empreendimento_codigo from {0}crt_unidade_consolidacao c, {0}tab_empreendimento e
															where c.empreendimento = e.id and c.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						caracterizacao.Id = reader.GetValue<int>("id");
						caracterizacao.Empreendimento.Id = reader.GetValue<int>("empreendimento");
						caracterizacao.Empreendimento.Codigo = reader.GetValue<int>("empreendimento_codigo");
						caracterizacao.PossuiCodigoUC = reader.GetValue<bool>("possui_codigo_uc");
						caracterizacao.CodigoUC = reader.GetValue<long>("codigo_uc");
						caracterizacao.LocalLivroDisponivel = reader.GetValue<string>("local_livro_disponivel");
						caracterizacao.TipoApresentacaoProducaoFormaIdentificacao = reader.GetValue<string>("tipo_apresentacao_produto");
						caracterizacao.Tid = reader.GetValue<string>("tid");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return caracterizacao;
				}

				#region Cultivares

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.tid, c.unidade_consolidacao, c.capacidade_mes, c.unidade_medida, lu.texto unidade_medida_texto, c.cultivar, cc.cultivar cultivar_nome, c.cultura,
				tc.texto cultura_texto from crt_unidade_cons_cultivar c, tab_cultura tc, tab_cultura_cultivar cc, lov_crt_un_conso_un_medida lu where tc.id = c.cultura 
				and cc.id(+) = c.cultivar and lu.id = c.unidade_medida and c.unidade_consolidacao = :unidade", EsquemaBanco);

				comando.AdicionarParametroEntrada("unidade", id, DbType.Int32);

				Cultivar cultivar = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						cultivar = new Cultivar();
						cultivar.CapacidadeMes = reader.GetValue<decimal>("capacidade_mes");
						cultivar.Id = reader.GetValue<int>("cultivar");
						cultivar.IdRelacionamento = reader.GetValue<int>("id");
						cultivar.Nome = reader.GetValue<string>("cultivar_nome");
						cultivar.Tid = reader.GetValue<string>("tid");
						cultivar.UnidadeMedida = reader.GetValue<int>("unidade_medida");
						cultivar.UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto");
						cultivar.CulturaId = reader.GetValue<int>("cultura");
						cultivar.CulturaTexto = reader.GetValue<string>("cultura_texto");

						caracterizacao.Cultivares.Add(cultivar);
					}

					reader.Close();
				}

				#endregion

				#region Responsáveis Técnicos

				comando = bancoDeDados.CriarComando(@"
				select c.id, c.unidade_consolidacao, c.responsavel_tecnico, c.numero_hab_cfo_cfoc, c.numero_art, c.art_cargo_funcao, c.data_validade_art, c.tid, 
				nvl(tp.nome, tp.razao_social) nome_razao, nvl(tp.cpf, tp.cnpj) cpf_cnpj, pf.texto profissao, oc.orgao_sigla, pp.registro, 
				(select h.extensao_habilitacao from tab_hab_emi_cfo_cfoc h where h.responsavel = c.responsavel_tecnico) extensao_habilitacao 
				from {0}crt_unida_conso_resp_tec c, {0}tab_credenciado tc, {1}tab_pessoa tp, {1}tab_pessoa_profissao pp, 
				{0}tab_profissao pf, {0}tab_orgao_classe oc where tc.id = c.responsavel_tecnico and tp.id = tc.pessoa and pp.pessoa(+) = tp.id 
				and pf.id(+) = pp.profissao and oc.id(+) = pp.orgao_classe and c.unidade_consolidacao = :unidade", EsquemaBanco, EsquemaCredenciadoBanco);

				comando.AdicionarParametroEntrada("unidade", id, DbType.Int32);

				ResponsavelTecnico responsavel = null;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						responsavel = new ResponsavelTecnico();
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

						if (Convert.ToBoolean(reader.GetValue<int>("extensao_habilitacao")))
						{
							responsavel.CFONumero += "-ES";
						}

						caracterizacao.ResponsaveisTecnicos.Add(responsavel);
					}

					reader.Close();
				}

				#endregion
			}

			return caracterizacao;
		}

		#endregion

		internal List<ListaValor> ObterEmpreendimentosResponsaveis()
		{
			List<ListaValor> retorno = new List<ListaValor>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"
				select e.id, e.denominador from crt_unidade_consolidacao c, tab_empreendimento e where e.id = c.empreendimento
				and c.id in (select r.unidade_consolidacao from crt_unida_conso_resp_tec r where r.responsavel_tecnico = :credenciado)", EsquemaBanco);

				comando.AdicionarParametroEntrada("credenciado", User.FuncionarioId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						retorno.Add(new ListaValor()
						{
							Id = reader.GetValue<int>("id"),
							Texto = reader.GetValue<string>("denominador"),
							IsAtivo = true
						});
					}

					reader.Close();
				}

				return retorno;
			}
		}
	}
}