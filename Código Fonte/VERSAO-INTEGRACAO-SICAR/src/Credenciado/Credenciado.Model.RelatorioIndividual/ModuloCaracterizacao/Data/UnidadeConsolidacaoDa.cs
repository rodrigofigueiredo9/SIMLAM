using System;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloEmpreendimento;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloUnidadeConsolidacao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.RelatorioIndividual.ModuloCaracterizacao.Data
{
	public class UnidadeConsolidacaoDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		private string EsquemaBancoCredenciado { get; set; }

		private string EsquemaBanco { get; set; }

		#endregion

		public UnidadeConsolidacaoDa()
		{
			EsquemaBancoCredenciado = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado);
			EsquemaBanco = _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno);
		}

		internal UnidadeConsolidacaoRelatorio Obter(int projetoDigitalId)
		{
			UnidadeConsolidacaoRelatorio unidadeConsolidacaoRelatorio = new UnidadeConsolidacaoRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"select uc.id, te.id empreendimentoID, te.denominador, te.cnpj, uc.codigo_uc, uc.local_livro_disponivel, tp.situacao, 
					uc.tipo_apresentacao_produto, tec.easting_utm, tec.northing_utm from {0}tab_empreendimento te, {0}tab_empreendimento_coord tec, {0}crt_unidade_consolidacao uc, 
					{0}tab_projeto_digital tp where uc.empreendimento = te.id and tec.empreendimento = te.id and tp.empreendimento = uc.empreendimento and tp.id = :id", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("id", projetoDigitalId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						unidadeConsolidacaoRelatorio.Id = reader.GetValue<int>("id");
						unidadeConsolidacaoRelatorio.CodigoUc = reader.GetValue<long>("codigo_uc");
						unidadeConsolidacaoRelatorio.LocalLivro = reader.GetValue<string>("local_livro_disponivel");
						unidadeConsolidacaoRelatorio.TipoApresentacao = reader.GetValue<string>("tipo_apresentacao_produto");
						unidadeConsolidacaoRelatorio.Situacao = reader.GetValue<int>("situacao");
						unidadeConsolidacaoRelatorio.Empreendimento.Id = reader.GetValue<int>("empreendimentoID");
						unidadeConsolidacaoRelatorio.Empreendimento.Denominador = reader.GetValue<string>("denominador");
						unidadeConsolidacaoRelatorio.Empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						unidadeConsolidacaoRelatorio.Empreendimento.EastingUtm = reader.GetValue<int>("easting_utm");
						unidadeConsolidacaoRelatorio.Empreendimento.NorthingUtm = reader.GetValue<int>("northing_utm");
					}
				}

				#region Responsaveis do empreendimento

				comando = bancoDeDados.CriarComando(@"
				select nvl(p.razao_social, p.nome) NomeRazao, nvl(p.cpf, p.cnpj) CpfCnpj, lt.texto TipoTexto 
				from tab_empreendimento_responsavel e, tab_pessoa p, lov_empreendimento_tipo_resp lt 
				where p.id = e.responsavel and e.tipo = lt.id and e.empreendimento = :empreendimento");

				comando.AdicionarParametroEntrada("empreendimento", unidadeConsolidacaoRelatorio.Empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ResponsavelRelatorio responsavel = new ResponsavelRelatorio();
						responsavel.NomeRazao = reader.GetValue<string>("NomeRazao");
						responsavel.CpfCnpj = reader.GetValue<string>("CpfCnpj");
						responsavel.TipoTexto = reader.GetValue<string>("TipoTexto");

						unidadeConsolidacaoRelatorio.Empreendimento.Responsaveis.Add(responsavel);
					}
				}

				#endregion

				#region Endereço do empreendimento

				comando = bancoDeDados.CriarComando(@"select e.cep, e.logradouro, e.bairro, e.distrito, e.numero, m.texto MunicipioTexto, uf.sigla EstadoSigla, e.corrego, e.complemento, e.correspondencia
					from {0}tab_empreendimento_endereco e, {0}lov_municipio m, {0}lov_estado uf where m.id = e.municipio and uf.id = m.estado and e.empreendimento = :empreendimento", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("empreendimento", unidadeConsolidacaoRelatorio.Empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{

						EnderecoRelatorio endereco = new EnderecoRelatorio()
						{
							Cep = reader.GetValue<string>("cep"),
							Logradouro = reader.GetValue<string>("logradouro"),
							Bairro = reader.GetValue<string>("bairro"),
							Distrito = reader.GetValue<string>("distrito"),
							Numero = reader.GetValue<string>("numero"),
							MunicipioTexto = reader.GetValue<string>("MunicipioTexto"),
							EstadoSigla = reader.GetValue<string>("EstadoSigla"),
							Corrego = reader.GetValue<string>("corrego"),
							Complemento = reader.GetValue<string>("complemento"),
							Correspondencia = reader.GetValue<int>("correspondencia")
						};

						unidadeConsolidacaoRelatorio.Empreendimento.Enderecos.Add(endereco);
					}
				}

				#endregion

				#region Meio Contato

				comando = bancoDeDados.CriarComando(@"select t.valor, t.meio_contato, m.texto meio_contato_texto from {0}tab_empreendimento_contato t, 
					{0}tab_meio_contato m where m.id = t.meio_contato and t.empreendimento = :empreendimento", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("empreendimento", unidadeConsolidacaoRelatorio.Empreendimento.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						ContatoRelatorio contato = new ContatoRelatorio()
						{
							Valor = reader.GetValue<string>("valor"),
							TipoTexto = reader.GetValue<string>("meio_contato_texto")
						};

						switch (reader.GetValue<int>("meio_contato"))
						{
							case 1:
								contato.TipoContato = eTipoContato.TelefoneResidencial;
								break;
							case 2:
								contato.TipoContato = eTipoContato.TelefoneCelular;
								break;
							case 3:
								contato.TipoContato = eTipoContato.TelefoneFax;
								break;
							case 4:
								contato.TipoContato = eTipoContato.TelefoneComercial;
								break;
							case 5:
								contato.TipoContato = eTipoContato.Email;
								break;
							case 6:
								contato.TipoContato = eTipoContato.NomeContato;
								break;
						}

						unidadeConsolidacaoRelatorio.Empreendimento.MeiosContatos.Add(contato);
					}
				}

				#endregion

				#region Cultura

				comando = bancoDeDados.CriarComando(@"select c.capacidade_mes, c.unidade_medida, lu.texto unidade_medida_texto, tc.texto CulturaNome, cc.cultivar CultivarNome from {0}crt_unidade_cons_cultivar c, 
					{0}tab_cultura tc, {0}tab_cultura_cultivar cc, {0}lov_crt_un_conso_un_medida lu where tc.id = c.cultura and cc.id(+) = c.cultivar and lu.id = c.unidade_medida 
					and c.unidade_consolidacao = :unidade", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("unidade", unidadeConsolidacaoRelatorio.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{

						CultivarRelatorio cultivar = new CultivarRelatorio()
						{
							CulturaNome = reader.GetValue<string>("CulturaNome"),
							CapacidadeMes = reader.GetValue<string>("capacidade_mes"),
							UnidadeMedida = reader.GetValue<int>("unidade_medida"),
							UnidadeMedidaTexto = reader.GetValue<string>("unidade_medida_texto"),
							CultivarNome = reader.GetValue<string>("CultivarNome")
						};

						unidadeConsolidacaoRelatorio.Cultivar.Add(cultivar);
					}
				}

				#endregion

				#region Responsaveis Tecnico

				comando = bancoDeDados.CriarComando(@"select nvl(p.nome, p.razao_social) NomeRazao, r.numero_hab_cfo_cfoc Habilitacao, 
				(select t.extensao_habilitacao from {0}tab_hab_emi_cfo_cfoc t where t.responsavel = c.id) ExtensaoHabilitacao 
				from {0}crt_unida_conso_resp_tec r, {0}crt_unidade_consolidacao u, {0}tab_credenciado c, {0}tab_pessoa p 
				where u.id = r.unidade_consolidacao and c.id = r.responsavel_tecnico and p.id = c.pessoa and u.empreendimento = :empreendimento", EsquemaBancoCredenciado);

				comando.AdicionarParametroEntrada("empreendimento", unidadeConsolidacaoRelatorio.Empreendimento.Id, DbType.Int32);
				bool extensao = false;

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						extensao = reader.GetValue<bool>("ExtensaoHabilitacao");

						ResponsavelRelatorio responsavel = new ResponsavelRelatorio()
						{
							NomeRazao = reader.GetValue<string>("NomeRazao"),
							Habilitacao = extensao ? reader.GetValue<string>("Habilitacao") + " - ES" : reader.GetValue<string>("Habilitacao")
						};

						unidadeConsolidacaoRelatorio.ResponsaveisTecnicos.Add(responsavel);
					}
				}

				#endregion
			}

			return unidadeConsolidacaoRelatorio;
		}
	}
}