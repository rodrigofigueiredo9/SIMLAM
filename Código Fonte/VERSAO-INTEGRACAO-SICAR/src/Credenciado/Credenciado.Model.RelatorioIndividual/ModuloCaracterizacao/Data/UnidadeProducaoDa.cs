using System;
using System.Data;
using System.Linq;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.ModuloProjetoDigital;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloPessoa;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloRequerimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeProducao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data
{
	public class UnidadeProducaoDa
	{
		#region Propriedades
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private string EsquemaBancoCredenciado { get; set; }

		private string EsquemaBanco { get; set; }

		#endregion

		public UnidadeProducaoDa()
		{
			EsquemaBancoCredenciado = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
			EsquemaBanco = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno);
		}

		internal UnidadeProducaoRelatorio Obter(int projetoDigitalId)
		{
			UnidadeProducaoRelatorio unidade = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select c.id, c.propriedade_codigo, c.local_livro, e.denominador, e.cnpj, ee.cep, ee.logradouro, ee.bairro,  ee.distrito,
				ee.numero, lm.texto municipio_texto, le.sigla estado_sigla,  ee.corrego,  ee.complemento, tp.situacao projeto_situacao from crt_unidade_producao c, tab_empreendimento e,
				tab_empreendimento_endereco ee, lov_municipio lm, lov_estado le, tab_projeto_digital tp where e.id = tp.empreendimento and c.empreendimento = e.id and e.id = 
				ee.empreendimento  and ee.correspondencia = 0 and le.id = ee.estado and lm.id = ee.municipio and tp.id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", projetoDigitalId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						unidade = new UnidadeProducaoRelatorio();
						unidade.Id = reader.GetValue<int>("id");
						unidade.CodigoPropriedade = reader.GetValue<int>("propriedade_codigo").ToString("D4");
						unidade.LocalLivro = reader.GetValue<string>("local_livro");
						unidade.Empreendimento.Denominador = reader.GetValue<string>("denominador");
						unidade.Empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						unidade.Situacao = (eProjetoDigitalSituacao)reader.GetValue<int>("projeto_situacao");
						unidade.Empreendimento.Enderecos.Add(new EnderecoRelatorio()
						{
							Cep = reader.GetValue<string>("cep"),
							Logradouro = reader.GetValue<string>("logradouro"),
							Bairro = reader.GetValue<string>("bairro"),
							Distrito = reader.GetValue<string>("distrito"),
							Numero = reader.GetValue<string>("numero"),
							MunicipioTexto = reader.GetValue<string>("municipio_texto"),
							EstadoSigla = reader.GetValue<string>("estado_sigla"),
							Corrego = reader.GetValue<string>("corrego"),
							Complemento = reader.GetValue<string>("complemento"),
							Correspondencia = 0
						});
					}

					reader.Close();
				}

				#region Unidade de produção

				comando = bancoDeDados.CriarComando(@"select c.id, c.codigo_up, cc.easting_utm, cc.northing_utm, c.area,  cu.cultivar, tc.texto cultura, c.data_plantio_ano_producao,
				 c.estimativa_quant_ano, lc.texto estimativa from crt_unidade_producao_unidade  c, crt_unidade_producao_un_coord cc, tab_cultura_cultivar cu, lov_crt_uni_prod_uni_medida lc,
				 tab_cultura tc where  tc.id(+) = cu.cultura and c.estimativa_unid_medida = lc.id and c.cultivar = cu.id(+) and c.id = cc.unidade_producao_unidade and c.unidade_producao = :unidade", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("unidade", unidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					UnidadeProducaoItemRelatorio item;
					while (reader.Read())
					{
						item = new UnidadeProducaoItemRelatorio();
						item.Id = reader.GetValue<int>("id");
						item.CodigoUP = reader.GetValue<string>("codigo_up");
						item.Latitude = reader.GetValue<string>("easting_utm");
						item.Longitude = reader.GetValue<string>("northing_utm");
						item.AreaHa = reader.GetValue<string>("area");
						item.CultivarNome = reader.GetValue<string>("cultivar");
						item.CulturaNome = reader.GetValue<string>("cultura");
						item.DataPlantio = reader.GetValue<string>("data_plantio_ano_producao");
						item.QuantidadeAno = reader.GetValue<string>("estimativa_quant_ano");
						item.UnidadeMedida = reader.GetValue<string>("estimativa");
						unidade.UP.Add(item);
					}

					reader.Close();
				}

				#endregion

				#region Produtores

				comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) nome, nvl(p.cpf, p.cnpj) cpf_cnpj, e.cep, e.logradouro, e.bairro, e.distrito, e.numero, lm.texto municipio_texto, le.sigla estado_sigla,
					e.complemento, (select m.valor from tab_pessoa_meio_contato m where m.pessoa = p.id and m.meio_contato = 1) tel_residencial, (select m.valor from tab_pessoa_meio_contato m where m.pessoa = p.id and m.meio_contato = 2) tel_celular,
					(select m.valor from tab_pessoa_meio_contato m where m.pessoa = p.id and m.meio_contato = 3) tel_fax, (select m.valor from tab_pessoa_meio_contato m where m.pessoa = p.id and m.meio_contato = 4) tel_comercial,
					(select m.valor from tab_pessoa_meio_contato m where m.pessoa = p.id and m.meio_contato = 5) email from crt_unidade_prod_un_produtor c , tab_pessoa p, tab_pessoa_endereco e, lov_municipio lm, lov_estado le
					where c.produtor = p.id and p.id = e.pessoa and e.municipio = lm.id and e.estado = le.id and c.unidade_producao_unidade in 
					(select id from crt_unidade_producao_unidade where unidade_producao = :unidade)", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("unidade", unidade.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					PessoaRelatorio produtor = null;
					while (reader.Read())
					{
						if (unidade.Produtores.Exists(x => x.CPFCNPJ == reader.GetValue<string>("cpf_cnpj")))
						{
							continue;
						}

						produtor = new PessoaRelatorio();
						produtor.NomeRazaoSocial = reader.GetValue<string>("nome");
						produtor.CPFCNPJ = reader.GetValue<string>("cpf_cnpj");
						produtor.Endereco.Cep = reader.GetValue<string>("cep");
						produtor.Endereco.Logradouro = reader.GetValue<string>("logradouro");
						produtor.Endereco.Bairro = reader.GetValue<string>("bairro");
						produtor.Endereco.Distrito = reader.GetValue<string>("distrito");
						produtor.Endereco.Numero = reader.GetValue<string>("numero");
						produtor.Endereco.MunicipioTexto = reader.GetValue<string>("municipio_texto");
						produtor.Endereco.EstadoSigla = reader.GetValue<string>("estado_sigla");
						produtor.Endereco.Complemento = reader.GetValue<string>("complemento");
						produtor.MeiosContatos.Add(new ContatoRelatorio() { Valor = reader.GetValue<string>("tel_residencial"), TipoContato = eTipoContato.TelefoneResidencial });
						produtor.MeiosContatos.Add(new ContatoRelatorio() { Valor = reader.GetValue<string>("tel_celular"), TipoContato = eTipoContato.TelefoneCelular });
						produtor.MeiosContatos.Add(new ContatoRelatorio() { Valor = reader.GetValue<string>("tel_fax"), TipoContato = eTipoContato.TelefoneFax });
						produtor.MeiosContatos.Add(new ContatoRelatorio() { Valor = reader.GetValue<string>("tel_comercial"), TipoContato = eTipoContato.TelefoneComercial });
						produtor.MeiosContatos.Add(new ContatoRelatorio() { Valor = reader.GetValue<string>("email"), TipoContato = eTipoContato.Email });
						unidade.Produtores.Add(produtor);
					}

					reader.Close();
				}

				#endregion

				#region Responsaveis Tecnicos

				comando = bancoDeDados.CriarComando(@"select r.unidade_producao_unidade, nvl(p.nome, p.razao_social) nome, h.numero_habilitacao, 
				(select t.extensao_habilitacao from {0}tab_hab_emi_cfo_cfoc t where t.responsavel = c.id) extensao_habilitacao 
				from {0}crt_unidade_prod_un_resp_tec r, {0}tab_credenciado c, {0}tab_pessoa p, 
				{1}tab_hab_emi_cfo_cfoc h where  r.responsavel_tecnico = h.responsavel and r.responsavel_tecnico = c.id and c.pessoa = p.id and r.unidade_producao_unidade 
				in (select id from {0}crt_unidade_producao_unidade where unidade_producao = :unidade)", EsquemaBancoCredenciado, EsquemaBanco);
				comando.AdicionarParametroEntrada("unidade", unidade.Id, DbType.Int32);

				bool extensao = false;
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					ResponsavelTecnicoRelatorio item = null;

					while (reader.Read())
					{
						extensao = reader.GetValue<bool>("extensao_habilitacao");

						item = new ResponsavelTecnicoRelatorio();
						item.NomeRazao = reader.GetValue<string>("nome");
						item.NumeroHabilitacao = extensao ? reader.GetValue<string>("numero_habilitacao") + " - ES" : reader.GetValue<string>("numero_habilitacao");
						unidade.UP.Single(x => x.Id == reader.GetValue<int>("unidade_producao_unidade")).Responsaveis.Add(item);
					}

					reader.Close();
				}

				#endregion Responsaveis Tecnicos
			}

			return unidade;
		}
	}
}