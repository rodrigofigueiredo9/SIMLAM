using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloVegetal.Praga;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC;
using Tecnomapas.Blocos.Entities.Interno.ModuloConfiguracaoDocumentoFitossanitario;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;
using Tecnomapas.Blocos.Entities.Interno.ModuloPTV;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Data
{
	public class EmissaoDa
	{
		#region Propriedades

		private string EsquemaBanco { get; set; }
		GerenciadorConfiguracao<ConfiguracaoSistema> _configSys = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());

		Historico _historico = new Historico();
		internal Historico Historico { get { return _historico; } }

		Consulta _consulta = new Consulta();
		internal Consulta Consulta { get { return _consulta; } }

		private static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public String UsuarioCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#endregion

		public EmissaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		public String EsquemaBancoCredenciado
		{
			get { return _configSys.Obter<String>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}

		#region Obter

		internal EmissaoPTVRelatorio Obter(int id, bool simplificado = false)
		{
			EmissaoPTVRelatorio emissaoPTV = new EmissaoPTVRelatorio();
			List<String> listDeclaracoesAdicionais = new List<String>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;

				#region SQL PTV

				comando = bancoDeDados.CriarComando(@"
				select distinct t.id,
					nvl(t.eptv_id, 0) eptv_id,
					t.tid,
					t.numero,
					t.situacao,
					e.denominador,
					e.cnpj,
					le.sigla as uf,
					lm.texto as municipio,
					ee.logradouro,
					ee.bairro,
					ee.distrito,
					nvl(pr.nome, pr.razao_social) as resp_razao_social,
					pr.cpf as empreend_resp_cpf,
					t.partida_lacrada_origem,
					t.numero_lacre,
					t.numero_porao,
					t.numero_container,
					t.apresentacao_nota_fiscal,
					t.numero_nota_fiscal,
					t.tipo_transporte,
					t.rota_transito_definida,
					t.veiculo_identificacao_numero,
					t.itinerario,
					t.data_ativacao,
					t.valido_ate,
					t.responsavel_tecnico,
					d.nome as destinatario_nome,
					d.endereco as destinatario_endereco,
					led.sigla destinatario_uf,
					lmd.texto destinatario_mun,
					lme.texto as municipio_emissao,
					d.cpf_cnpj destinatario_cpfcnpj
				from tab_ptv                     t,
					tab_empreendimento           e,
					tab_empreendimento_endereco  ee,
					lov_estado                   le,
					lov_municipio                lm,
					lov_municipio                lme,
					tab_pessoa                   pr,
					tab_destinatario_ptv         d,
					lov_estado                   led,
					lov_municipio                lmd
				where e.id = t.empreendimento
				and (ee.empreendimento = e.id and ee.correspondencia = 0)
				and le.id = ee.estado
				and lm.id = ee.municipio
				and lme.id(+) = t.municipio_emissao
				and pr.id(+) = t.responsavel_emp
				and d.id = t.destinatario
				and led.id = d.uf
				and lmd.id = d.municipio
				and t.id = :id", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						emissaoPTV.Id = id;
						emissaoPTV.Tid = reader.GetValue<string>("tid");
						emissaoPTV.IsEPTV = reader.GetValue<bool>("eptv_id");
						emissaoPTV.Situacao = reader.GetValue<int>("situacao");
						emissaoPTV.FuncId = reader.GetValue<int>("responsavel_tecnico");
						emissaoPTV.NumeroPTV = reader.GetValue<string>("numero");
						emissaoPTV.PartidaLacrada = reader.GetValue<int>("partida_lacrada_origem");
						emissaoPTV.NumeroLacre = reader.GetValue<string>("numero_lacre");
						emissaoPTV.NumeroPorao = reader.GetValue<string>("numero_porao");
						emissaoPTV.NumeroConteiner = reader.GetValue<string>("numero_container");
						emissaoPTV.TipoTransporte = reader.GetValue<int>("tipo_transporte");
						emissaoPTV.Rota_transito_definida = reader.GetValue<int>("rota_transito_definida");
						emissaoPTV.ApresentacaoNotaFiscal = reader.GetValue<int>("apresentacao_nota_fiscal");
						emissaoPTV.NumeroNotaFiscal = reader.GetValue<string>("numero_nota_fiscal");
						emissaoPTV.VeiculoNumero = reader.GetValue<string>("veiculo_identificacao_numero");
						emissaoPTV.Itinerario = reader.GetValue<string>("itinerario");
						emissaoPTV.DataAtivacao = reader.GetValue<DateTime>("data_ativacao").ToString("dd/MM/yyyy");
						emissaoPTV.DataValidade = reader.GetValue<DateTime>("valido_ate").ToShortDateString();

						emissaoPTV.Destinatario.Nome = reader.GetValue<string>("destinatario_nome");
						emissaoPTV.Destinatario.Endereco = reader.GetValue<string>("destinatario_endereco");
						emissaoPTV.Destinatario.UF = reader.GetValue<string>("destinatario_uf");
						emissaoPTV.Destinatario.Municipio = reader.GetValue<string>("destinatario_mun");
						emissaoPTV.Destinatario.CPFCNPJ = reader.GetValue<string>("destinatario_cpfcnpj");
						emissaoPTV.MunicipioEmissao = reader.GetValue<string>("municipio_emissao");
						emissaoPTV.Empreendimento.ResponsavelRazaoSocial = reader.GetValue<string>("resp_razao_social");
						emissaoPTV.Empreendimento.NomeRazao = reader.GetValue<string>("denominador");
						emissaoPTV.Empreendimento.EndLogradouro = reader.GetValue<string>("logradouro");
						emissaoPTV.Empreendimento.EndBairro = reader.GetValue<string>("bairro");
						emissaoPTV.Empreendimento.EndDistrito = reader.GetValue<string>("distrito");
						emissaoPTV.Empreendimento.EndMunicipio = reader.GetValue<string>("municipio");
						emissaoPTV.Empreendimento.EndUF = reader.GetValue<string>("uf");
						emissaoPTV.Empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						emissaoPTV.Empreendimento.ResponsavelCPF = reader.GetValue<string>("empreend_resp_cpf");
					}

					reader.Close();
				}

				#endregion

				if (simplificado)
				{
					return emissaoPTV;
				}

				#region SQL Funcionário
				comando = bancoDeDados.CriarComando(@"
			    select f.nome, h.numero_habilitacao, h.numero_crea, h.uf_habilitacao, h.numero_visto_crea, f.arquivo arquivo_id from {0}tab_hab_emi_ptv h, {0}tab_funcionario f
			    where f.id = h.funcionario and f.id = :idfun", EsquemaBanco);

				comando.AdicionarParametroEntrada("idfun", emissaoPTV.FuncId, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						emissaoPTV.FuncionarioHabilitado.Nome = reader.GetValue<string>("nome");
						emissaoPTV.FuncionarioHabilitado.Numero = reader.GetValue<string>("numero_habilitacao");
						emissaoPTV.FuncionarioHabilitado.Registro = reader.GetValue<string>("numero_crea");
						emissaoPTV.FuncionarioHabilitado.ArquivoId = reader.GetValue<int>("arquivo_id");

                        emissaoPTV.FuncionarioHabilitado.UFHablitacao = reader.GetValue<int>("uf_habilitacao");
                        emissaoPTV.FuncionarioHabilitado.NumeroVistoCrea = reader.GetValue<string>("numero_visto_crea");
					}
					reader.Close();
				}

				#endregion

				#region SQL PTVProduto

				comando = bancoDeDados.CriarComando(@"select c.texto cultura,
															cc.cultivar,
															t.quantidade,
															decode(t.origem_tipo, 1, (select cfo.numero from cre_cfo cfo where cfo.id = t.origem), '') as numero_cfo,
															decode(t.origem_tipo, 2, (select cfoc.numero from cre_cfoc cfoc where cfoc.id = t.origem), '') as numero_cfoc,
															decode(t.origem_tipo, 3, (select ptv.numero from tab_ptv ptv where ptv.id = t.origem), 4, (select ptv.numero from tab_ptv_outrouf ptv where ptv.id = t.origem), '') as numero_ptv,
															decode(t.origem_tipo, 5, t.numero_origem, '') as numero_cf_cfr,
															decode(t.origem_tipo, 6, t.numero_origem, '') as numero_tf,
															u.texto as unidade_medida,
                                                            nvl(t.origem, 0) origem,
                                                            t.origem_tipo, 
                                                            t.cultura cultura_id,
                                                            t.cultivar cultivar_id
														from {0}tab_ptv_produto             t,
															 {0}tab_ptv                     p,
															 {0}tab_cultura                 c,
															 {0}tab_cultura_cultivar        cc,
															 lov_crt_uni_prod_uni_medida u
														where c.id = t.cultura
														and cc.id = t.cultivar
														and p.id = t.ptv
														and u.id = t.unidade_medida
														and t.ptv = :ptv", EsquemaBanco);
				comando.AdicionarParametroEntrada("ptv", emissaoPTV.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						emissaoPTV.Produtos.Add(new PTVProdutoRelatorio()
						{
							CulturaTexto = reader.GetValue<string>("cultura"),
							CulturaId = reader.GetValue<int>("cultura_id"),
							CultivarTexto = reader.GetValue<string>("cultivar"),
							CultivarId = reader.GetValue<int>("cultivar_id"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							NumeroCFO = reader.GetValue<string>("numero_cfo"),
							NumeroCFOC = reader.GetValue<string>("numero_cfoc"),
							NumeroPTV = reader.GetValue<string>("numero_ptv"),
							NumeroCFCFR = reader.GetValue<string>("numero_cf_cfr"),
							NumeroTF = reader.GetValue<string>("numero_tf"),
							UnidadeMedida = reader.GetValue<string>("unidade_medida"),
							Origem = reader.GetValue<int>("origem"),
							OrigemTipo = reader.GetValue<int>("origem_tipo")
						});
					}
					reader.Close();
				}

				List<LaudoLaboratorial> laudoLaboratoriais = ObterLaudoLaboratorial(emissaoPTV.Produtos);
				emissaoPTV.LaboratorioNome = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.Nome).ToList());
				emissaoPTV.NumeroLaudo = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.LaudoResultadoAnalise).ToList());
				emissaoPTV.LaboratorioMunicipio = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.MunicipioTexto).ToList());
				emissaoPTV.LaboratorioUF = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.EstadoTexto).ToList());

                foreach (var item in emissaoPTV.Produtos.Where(xx => (xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)).ToList())
				{
					List<String> listDeclaracoesAdicionaisAux = ObterDeclaracaoAdicional(item.Origem, item.OrigemTipo, (int)ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedida), item.CultivarId);
					item.DeclaracaoAdicional = String.Join(" ", listDeclaracoesAdicionaisAux.Distinct().ToList());
					listDeclaracoesAdicionais.AddRange(listDeclaracoesAdicionaisAux);
				}

				emissaoPTV.DeclaracaoAdicionalHtml = String.Join(" ", listDeclaracoesAdicionais.Distinct().ToList());

				#endregion

				#region Tratamento Fitossanitário

				emissaoPTV.Tratamentos = TratamentoFitossanitario(emissaoPTV.Produtos);

				if (emissaoPTV.Tratamentos == null || emissaoPTV.Tratamentos.Count <= 0)
				{
					emissaoPTV.Tratamentos = new List<TratamentosRelatorio>() { new TratamentosRelatorio() { } };
				}

				#endregion

				#region Arquivo

				if (emissaoPTV.FuncionarioHabilitado.ArquivoId.HasValue && emissaoPTV.FuncionarioHabilitado.ArquivoId > 0)
				{
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					emissaoPTV.AssinaturaDigital = _busArquivo.Obter(emissaoPTV.FuncionarioHabilitado.ArquivoId.Value);
				}

				#endregion
			}

			return emissaoPTV;
		}

		internal EmissaoPTVRelatorio ObterHistorico(int id, string Tid)
		{
			EmissaoPTVRelatorio emissaoPTV = new EmissaoPTVRelatorio();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = null;

				#region SQL PTV

				comando = bancoDeDados.CriarComando(@"
				select distinct t.id,
					nvl(t.eptv_id, 0) eptv_id,
					t.tid,
					t.numero,
					t.situacao_id,
					e.denominador,
					e.cnpj,
					le.sigla as uf,
					ee.municipio_texto as municipio,
					ee.logradouro,
					ee.bairro,
					ee.distrito,
					nvl(pr.nome, pr.razao_social) as resp_razao_social,
					pr.cpf as empreend_resp_cpf,
					t.partida_lacrada_origem,
					t.numero_lacre,
					t.numero_porao,
					t.numero_container,
					t.apresentacao_nota_fiscal,
					t.numero_nota_fiscal,
					t.tipo_transporte_id,
					t.rota_transito_definida,
					t.veiculo_identificacao_numero,
					t.itinerario,
					t.data_ativacao,
					t.valido_ate,
					t.responsavel_tecnico_id,
					t.responsavel_tecnico_tid,
					t.data_execucao,
					d.nome as destinatario_nome,
					d.endereco as destinatario_endereco,
					led.sigla destinatario_uf,
					d.municipio_texto destinatario_mun,
					t.municipio_emissao_texto as municipio_emissao,
					d.cpf_cnpj destinatario_cpfcnpj,
					t.declaracao_adicional_formatado
				from hst_ptv                     t,
					hst_empreendimento           e,
					hst_empreendimento_endereco  ee,
					lov_estado                   le,
					hst_pessoa                   pr,
					hst_destinatario_ptv         d,
					lov_estado                   led
				where e.empreendimento_id = t.empreendimento_id
				and e.tid = t.empreendimento_tid
				and (ee.id_hst = e.id and ee.correspondencia = 0)
				and le.id = ee.estado_id
				and pr.pessoa_id(+) = t.responsavel_emp_id
				and pr.tid(+) = t.responsavel_emp_tid
				and d.destinatario_ptv_id = t.destinatario_id
				and d.tid = t.destinatario_tid
				and led.id = d.uf_id
				and t.ptv_id = :ptv_id
				and t.tid = :tid", EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv_id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", Tid, DbType.String);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						emissaoPTV.Id = reader.GetValue<int>("id");
						emissaoPTV.Tid = reader.GetValue<string>("tid");
						emissaoPTV.IsEPTV = reader.GetValue<bool>("eptv_id");
						emissaoPTV.Situacao = reader.GetValue<int>("situacao_id");
						emissaoPTV.FuncId = reader.GetValue<int>("responsavel_tecnico_id");
						emissaoPTV.FuncTid = reader.GetValue<string>("responsavel_tecnico_tid");
						emissaoPTV.NumeroPTV = reader.GetValue<string>("numero");
						emissaoPTV.PartidaLacrada = reader.GetValue<int>("partida_lacrada_origem");
						emissaoPTV.NumeroLacre = reader.GetValue<string>("numero_lacre");
						emissaoPTV.NumeroPorao = reader.GetValue<string>("numero_porao");
						emissaoPTV.NumeroConteiner = reader.GetValue<string>("numero_container");
						emissaoPTV.TipoTransporte = reader.GetValue<int>("tipo_transporte_id");
						emissaoPTV.Rota_transito_definida = reader.GetValue<int>("rota_transito_definida");
						emissaoPTV.ApresentacaoNotaFiscal = reader.GetValue<int>("apresentacao_nota_fiscal");
						emissaoPTV.NumeroNotaFiscal = reader.GetValue<string>("numero_nota_fiscal");
						emissaoPTV.VeiculoNumero = reader.GetValue<string>("veiculo_identificacao_numero");
						emissaoPTV.Itinerario = reader.GetValue<string>("itinerario");
                        if (reader.GetValue<DateTime?>("data_ativacao") != null)
                            emissaoPTV.DataAtivacao = reader.GetValue<DateTime>("data_ativacao").ToString("dd/MM/yyyy");
                        else
                            emissaoPTV.DataAtivacao = "--/--/----";

						emissaoPTV.DataValidade = reader.GetValue<DateTime>("valido_ate").ToShortDateString();
						emissaoPTV.Destinatario.Nome = reader.GetValue<string>("destinatario_nome");
						emissaoPTV.Destinatario.Endereco = reader.GetValue<string>("destinatario_endereco");
						emissaoPTV.Destinatario.UF = reader.GetValue<string>("destinatario_uf");
						emissaoPTV.Destinatario.Municipio = reader.GetValue<string>("destinatario_mun");
						emissaoPTV.Destinatario.CPFCNPJ = reader.GetValue<string>("destinatario_cpfcnpj");
						emissaoPTV.MunicipioEmissao = reader.GetValue<string>("municipio_emissao");
						emissaoPTV.DataExecucao = reader.GetValue<DateTime>("data_execucao");
						emissaoPTV.Empreendimento.ResponsavelRazaoSocial = reader.GetValue<string>("resp_razao_social");
						emissaoPTV.Empreendimento.NomeRazao = reader.GetValue<string>("denominador");
						emissaoPTV.Empreendimento.EndLogradouro = reader.GetValue<string>("logradouro");
						emissaoPTV.Empreendimento.EndBairro = reader.GetValue<string>("bairro");
						emissaoPTV.Empreendimento.EndDistrito = reader.GetValue<string>("distrito");
						emissaoPTV.Empreendimento.EndMunicipio = reader.GetValue<string>("municipio");
						emissaoPTV.Empreendimento.EndUF = reader.GetValue<string>("uf");
						emissaoPTV.Empreendimento.CNPJ = reader.GetValue<string>("cnpj");
						emissaoPTV.Empreendimento.ResponsavelCPF = reader.GetValue<string>("empreend_resp_cpf");
						emissaoPTV.DeclaracaoAdicionalHtml = reader.GetValue<string>("declaracao_adicional_formatado");
					}

					reader.Close();
				}
				#endregion

				#region SQL Funcionário

				comando = bancoDeDados.CriarComando(@"   select x.nome,
                                                                y.numero_habilitacao,
                                                                y.numero_crea,
                                                                y.uf_habilitacao_id, 
                                                                y.numero_visto_crea,
                                                                nvl(x.arquivo_id, y.arquivo_id) arquivo_id
                                                            from (select hf.funcionario_id, hf.nome, hf.arquivo_id
                                                                    from hst_funcionario hf
                                                                    where hf.id in (select max(f.id)
                                                                                    from hst_funcionario f
                                                                                    where f.funcionario_id = :FuncId
                                                                                    and f.tid = :FuncTid)) x,
                                                                (select h2.funcionario_id,
                                                                        h2.numero_habilitacao,
                                                                        h2.numero_crea,
                                                                        h2.arquivo_id,
                                                                        h2.uf_habilitacao_id, 
                                                                        h2.numero_visto_crea
                                                                    from hst_hab_emi_ptv h2
                                                                    where h2.funcionario_id = :FuncId
                                                                    and h2.data_execucao =
                                                                        (select max(h.data_execucao)
                                                                            from hst_hab_emi_ptv h
                                                                            where h.funcionario_id = :FuncId
                                                                            and h.data_execucao <= :PTVDataExecucao)) y
                                                            where y.funcionario_id = x.funcionario_id", EsquemaBanco);

				comando.AdicionarParametroEntrada("FuncId", emissaoPTV.FuncId, DbType.Int32);
				comando.AdicionarParametroEntrada("FuncTid", emissaoPTV.FuncTid, DbType.String);
				comando.AdicionarParametroEntrada("PTVDataExecucao", emissaoPTV.DataExecucao, DbType.DateTime);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						emissaoPTV.FuncionarioHabilitado.Nome = reader.GetValue<string>("nome");
						emissaoPTV.FuncionarioHabilitado.Numero = reader.GetValue<string>("numero_habilitacao");
						emissaoPTV.FuncionarioHabilitado.Registro = reader.GetValue<string>("numero_crea");
						emissaoPTV.FuncionarioHabilitado.ArquivoId = reader.GetValue<int>("arquivo_id");

                        emissaoPTV.FuncionarioHabilitado.UFHablitacao = reader.GetValue<int>("uf_habilitacao_id");
                        emissaoPTV.FuncionarioHabilitado.NumeroVistoCrea = reader.GetValue<string>("numero_visto_crea");
					}

					reader.Close();
				}

				#endregion

				#region SQL PTVProduto

				comando = bancoDeDados.CriarComando(@"select c.texto cultura, c.cultura_id,
															cc.cultivar_nome cultivar, cc.cultivar_id,
															t.quantidade,
															decode(t.origem_tipo_id, 1, (select cfo.numero from cre_hst_cfo cfo where cfo.cfo_id = t.origem_id and cfo.tid = t.origem_tid), '') as numero_cfo,
                              decode(t.origem_tipo_id, 2, (select cfoc.numero from cre_hst_cfoc cfoc where cfoc.cfoc_id = t.origem_id and cfoc.tid = t.origem_tid), '') as numero_cfoc,
                              decode(t.origem_tipo_id, 3, (select ptv.numero from hst_ptv ptv where ptv.ptv_id = t.origem_id and ptv.tid = t.origem_tid), 4, (select ptv.numero from hst_ptv_outrouf ptv where ptv.ptv_id = t.origem_id and ptv.tid = t.origem_tid), '') as numero_ptv,
                              decode(t.origem_tipo_id, 5, t.numero_origem, '') as numero_cf_cfr,
                              decode(t.origem_tipo_id, 6, t.numero_origem, '') as numero_tf,
                              t.unidade_medida_texto as unidade_medida,
                              nvl(t.origem_id, 0) origem,
                              t.origem_tid,
                              t.origem_tipo_id origem_tipo
                            from hst_ptv_produto t,
                               hst_ptv p,
                               hst_cultura c,
                               hst_cultura_cultivar cc
                            where t.id_hst = p.id
                              and t.cultura_id = c.cultura_id
                              and t.cultura_tid = c.tid
                              and t.cultivar_id = cc.cultivar_id
                              and t.cultivar_tid = cc.tid and t.ptv_id = :ptv and t.id_hst = :id_hst", EsquemaBanco);

				comando.AdicionarParametroEntrada("ptv", id, DbType.Int32);
				comando.AdicionarParametroEntrada("id_hst", emissaoPTV.Id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						emissaoPTV.Produtos.Add(new PTVProdutoRelatorio()
						{
							CulturaTexto = reader.GetValue<string>("cultura"),
							CulturaId = reader.GetValue<int>("cultura_id"),
							CultivarTexto = reader.GetValue<string>("cultivar"),
							CultivarId = reader.GetValue<int>("cultivar_id"),
							Quantidade = reader.GetValue<decimal>("quantidade"),
							NumeroCFO = reader.GetValue<string>("numero_cfo"),
							NumeroCFOC = reader.GetValue<string>("numero_cfoc"),
							NumeroPTV = reader.GetValue<string>("numero_ptv"),
							NumeroCFCFR = reader.GetValue<string>("numero_cf_cfr"),
							NumeroTF = reader.GetValue<string>("numero_tf"),
							UnidadeMedida = reader.GetValue<string>("unidade_medida"),
							Origem = reader.GetValue<int>("origem"),
							OrigemTid = reader.GetValue<string>("origem_tid"),
							OrigemTipo = reader.GetValue<int>("origem_tipo")
						});
					}
					reader.Close();
				}

				List<string> listDeclaracoesAdicionais = new List<string>();
				List<LaudoLaboratorial> laudoLaboratoriais = ObterLaudoLaboratorial(emissaoPTV.Produtos);
				emissaoPTV.LaboratorioNome = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.Nome).ToList());
				emissaoPTV.NumeroLaudo = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.LaudoResultadoAnalise).ToList());
				emissaoPTV.LaboratorioMunicipio = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.MunicipioTexto).ToList());
				emissaoPTV.LaboratorioUF = Mensagem.Concatenar(laudoLaboratoriais.Select(x => x.EstadoTexto).ToList());

				if (String.IsNullOrEmpty(emissaoPTV.DeclaracaoAdicionalHtml))
				{
					foreach (var item in emissaoPTV.Produtos.Where(xx => (xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFO || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.CFOC || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTV || xx.OrigemTipo == (int)eDocumentoFitossanitarioTipo.PTVOutroEstado)).ToList())
					{
						List<String> listDeclaracoesAdicionaisAux = ObterDeclaracaoAdicionalHistorico(item.Origem, item.OrigemTid, (int)ValidacoesGenericasBus.ObterTipoProducao(item.UnidadeMedida), item.OrigemTipo, item.CultivarId);
						item.DeclaracaoAdicional = String.Join(" ", listDeclaracoesAdicionaisAux.Distinct().ToList());
						listDeclaracoesAdicionais.AddRange(listDeclaracoesAdicionaisAux);
					}
					emissaoPTV.DeclaracaoAdicionalHtml = String.Join(" ", listDeclaracoesAdicionais.Distinct().ToList());
				}

				#endregion

				#region Tratamento Fitossanitário

				emissaoPTV.Tratamentos = TratamentoFitossanitarioHistorico(emissaoPTV.Produtos);

				if (emissaoPTV.Tratamentos == null || emissaoPTV.Tratamentos.Count <= 0)
				{
					emissaoPTV.Tratamentos = new List<TratamentosRelatorio>() { new TratamentosRelatorio() { } };
				}

				#endregion

				#region Arquivo

				if (emissaoPTV.FuncionarioHabilitado.ArquivoId.HasValue && emissaoPTV.FuncionarioHabilitado.ArquivoId > 0)
				{
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					emissaoPTV.AssinaturaDigital = _busArquivo.Obter(emissaoPTV.FuncionarioHabilitado.ArquivoId.Value);
				}

				#endregion
			}

			return emissaoPTV;
		}

		internal List<String> ObterDeclaracaoAdicional(int origem, int origemTipo, int tipoProducaoID, int cultivarID)
		{
			List<String> retorno = new List<String>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				retorno = ObterDeclaracaoAdicional(origem, origemTipo,tipoProducaoID, cultivarID, bancoDeDados).Distinct().ToList();
			}
			return retorno;
		}
		private List<String> ObterDeclaracaoAdicional(int origem, int origemTipo, int tipoProducaoID, int cultivarID, BancoDeDados bancoDeDados)
		{
			Comando comando = null;
			Comando comandoCred = null;
			List<String> retorno = new List<String>();

			switch (origemTipo)
			{
				case (int)eDocumentoFitossanitarioTipo.CFO:
					#region Buscar tratamento CFO

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
						comandoCred = bancoDeDadosCredenciado.CriarComando(@"select distinct lcda.texto_formatado DeclaracaoAdicionalTexto from tab_cfo cfo, tab_cfo_produto cp,       
                            ins_crt_unidade_prod_unidade i, tab_cultivar_configuracao cconf, lov_cultivar_declara_adicional lcda, tab_cfo_praga tcp where cfo.id = cp.cfo 
                            and cp.unidade_producao = i.id and i.cultivar = cconf.cultivar and cconf.declaracao_adicional = lcda.id and cfo.id = tcp.cfo and tcp.praga = cconf.praga
                            and cfo.id = :cfoId and cconf.tipo_producao = :tipoProducaoID and i.cultivar = :cultivarID and lcda.outro_estado = '0' ", EsquemaBancoCredenciado);

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

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
                        comandoCred = bancoDeDadosCredenciado.CriarComando(@"select distinct DBMS_LOB.substr(cfoc.informacoes_complementares, 4000) as DeclaracaoAdicionalTexto
                            from tab_cfoc cfoc, tab_cfoc_produto cp, tab_lote_item hli, tab_cultivar_configuracao cconf, lov_cultivar_declara_adicional lcda, tab_cfoc_praga tcp
                            where cfoc.id = cp.cfoc and cp.lote = hli.lote and hli.cultivar = cconf.cultivar and cconf.declaracao_adicional = lcda.id and cfoc.id = tcp.cfoc
                            and tcp.praga = cconf.praga and cfoc.id = :cfocId and cconf.tipo_producao = :tipoProducaoID and hli.cultivar = :cultivarID and lcda.outro_estado = '0' ", EsquemaBancoCredenciado);

						comandoCred.AdicionarParametroEntrada("cfocId", origem, DbType.Int32);
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
				case (int)eDocumentoFitossanitarioTipo.PTV:
					#region Buscar tratamento PTV

					comando = bancoDeDados.CriarComando(@"select pp.origem, pp.origem_tipo, pp.unidade_medida from tab_ptv_produto pp where pp.ptv = :origemId and pp.cultivar = :cultivarID", EsquemaBanco);
					comando.AdicionarParametroEntrada("origemId", origem, DbType.Int32);
					comandoCred.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							int origemPTV = reader.GetValue<int>("origem");
							int origemTipoPTV = reader.GetValue<int>("origem_tipo");
							int unidadeMedidaIdPTV = reader.GetValue<int>("unidade_medida_id");

							retorno.AddRange(ObterDeclaracaoAdicional(origemPTV, origemTipoPTV, (int)ValidacoesGenericasBus.ObterTipoProducao(unidadeMedidaIdPTV), cultivarID, bancoDeDados));
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
                        //comando.AdicionarParametroEntrada("tipoProducaoID", tipoProducaoID, DbType.Int32);
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

		internal List<String> ObterDeclaracaoAdicionalHistorico(int origemId, string origemTid, int tipoProducaoID, int origemTipo, int cultivarID)
		{
			List<String> retorno = new List<String>();
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				retorno = ObterDeclaracaoAdicionalHistorico(origemId, origemTid, tipoProducaoID, origemTipo, cultivarID, bancoDeDados).Distinct().ToList();
			}
			return retorno;
		}
		private List<String> ObterDeclaracaoAdicionalHistorico(int origemId, string origemTid, int tipoProducaoID, int origemTipo, int cultivarID, BancoDeDados bancoDeDados)
		{
			Comando comando = null;
			Comando comandoCred = null;
			List<String> retorno = new List<String>();

			switch (origemTipo)
			{
				case (int)eDocumentoFitossanitarioTipo.CFO:
					#region Buscar tratamento CFO

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
						comandoCred = bancoDeDadosCredenciado.CriarComando(@"select distinct lcda.texto_formatado DeclaracaoAdicionalTexto
                            from hst_cfo cfo, hst_cfo_produto cp, ins_hst_crt_unidade_prod_unid  i, hst_cultura_cultivar hcc, 
                            hst_cultivar_configuracao cconf, lov_cultivar_declara_adicional lcda, hst_cfo_praga tcp where cfo.id = cp.id_hst 
                            and cp.unidade_producao_id = i.unidade_producao_unidade_id and cp.unidade_producao_tid = i.tid and i.cultivar_id = hcc.cultivar_id
                            and i.cultivar_tid = hcc.tid and hcc.id = cconf.id_hst and cconf.declaracao_adicional_id = lcda.id and cfo.id = tcp.id_hst 
                            and tcp.praga_id = cconf.praga_id and cfo.cfo_id = :cfoId and cfo.tid = :cfoTid and cconf.tipo_producao_id = :tipoProducaoID and i.cultivar_id = :cultivarID", EsquemaBancoCredenciado);
						comandoCred.AdicionarParametroEntrada("cfoId", origemId, DbType.Int32);
						comandoCred.AdicionarParametroEntrada("cfoTid", origemTid, DbType.String);
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

					using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(EsquemaBancoCredenciado))
					{
						comandoCred = bancoDeDadosCredenciado.CriarComando(@"select distinct lcda.texto_formatado DeclaracaoAdicionalTexto
                            from hst_cfoc cfoc, hst_cfoc_produto cp, hst_lote hl, hst_lote_item hli, hst_cultura_cultivar hcc, 
                            hst_cultivar_configuracao cconf, lov_cultivar_declara_adicional lcda, hst_cfoc_praga tcp where cfoc.id = cp.id_hst and cp.lote_id = hl.lote_id 
                            and cp.lote_tid = hl.tid and hl.id = hli.id_hst and hli.cultivar_id = hcc.cultivar_id and hli.cultivar_tid = hcc.tid and hcc.id = cconf.id_hst 
                            and cconf.declaracao_adicional_id = lcda.id and cfoc.id = tcp.id_hst and tcp.praga_id = cconf.praga_id 
                            and cfoc.cfoc_id = :cfocId and cfoc.tid = :cfocTid and cconf.tipo_producao_id = :tipoProducaoID and hli.cultivar_id = :cultivarID", EsquemaBancoCredenciado);
						comandoCred.AdicionarParametroEntrada("cfocId", origemId, DbType.Int32);
						comandoCred.AdicionarParametroEntrada("cfocTid", origemTid, DbType.String);
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
				case (int)eDocumentoFitossanitarioTipo.PTV:
					#region Buscar tratamento PTV

					comando = bancoDeDados.CriarComando(@"select pp.origem_id, pp.origem_tid, pp.origem_tipo_id, pp.unidade_medida_id from hst_ptv_produto pp, hst_ptv p 
                        where pp.id_hst = p.id and p.ptv_id = :origemId and p.tid = :origemTid and pp.cultivar_id = :cultivarID", EsquemaBanco);
					comando.AdicionarParametroEntrada("origemId", origemId, DbType.Int32);
					comando.AdicionarParametroEntrada("origemTid", origemTid, DbType.String);
					comando.AdicionarParametroEntrada("cultivarID", cultivarID, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							int origemPTV = reader.GetValue<int>("origem_id");
							string origemTidPTV = reader.GetValue<string>("origem_tid");
							int origemTipoPTV = reader.GetValue<int>("origem_tipo_id");
							int unidadeMedidaIdPTV = reader.GetValue<int>("unidade_medida_id");


							retorno.AddRange(ObterDeclaracaoAdicionalHistorico(origemPTV, origemTidPTV, (int)ValidacoesGenericasBus.ObterTipoProducao(unidadeMedidaIdPTV), origemTipoPTV, cultivarID, bancoDeDados));
						}
						reader.Close();
					}
					#endregion
					break;
                case (int)eDocumentoFitossanitarioTipo.PTVOutroEstado:
                    #region Buscar tratamento PTV Outro Estado
                    using (BancoDeDados bancoDeDadosCredenciado = BancoDeDados.ObterInstancia(EsquemaBanco))
                    {
                        comando = bancoDeDados.CriarComando(@"select distinct lcda.texto_formatado DeclaracaoAdicionalTexto 
                            from tab_ptv_outrouf_produto cp, tab_cultivar_configuracao cconf, lov_cultivar_declara_adicional lcda
                            where cp.cultivar = cconf.cultivar and cconf.declaracao_adicional = lcda.id and lcda.outro_estado = '1' 
                            and cconf.tipo_producao = :tipoProducaoID and cp.cultivar = :cultivarID ", EsquemaBanco);

                        //comando.AdicionarParametroEntrada("idPtvOutro", origem, DbType.Int32);
                        comando.AdicionarParametroEntrada("tipoProducaoID", tipoProducaoID, DbType.Int32);
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

		/// <summary>
		///	Método recursivo que busca os tratamentos Fitossanitários dos produtos relacionados, 
		///	trazendo os 5 primeiros de acordo com a ordem em que foram adicionados
		/// </summary>
		/// <param name="origens"></param>
		/// <returns>List<TratamentoFitossanitario></returns>
		/// 
		internal List<TratamentosRelatorio> TratamentoFitossanitario(List<PTVProdutoRelatorio> origens)
		{
			List<TratamentosRelatorio> retorno = new List<TratamentosRelatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				retorno = TratamentoFitossanitario(origens, bancoDeDados);
			}

			return retorno.GroupBy(p => new { p.ProdutoComercial, p.IngredienteAtivo, p.Dose, p.PragaProduto, p.ModoAplicacao }).Select(g => g.First()).ToList();
		}
		internal List<TratamentosRelatorio> TratamentoFitossanitario(List<PTVProdutoRelatorio> origens, BancoDeDados bancoDeDados)
		{
			List<TratamentosRelatorio> retorno = new List<TratamentosRelatorio>();
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
									retorno.Add(new TratamentosRelatorio()
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
									retorno.Add(new TratamentosRelatorio()
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
						comando.AdicionarParametroEntrada("culturaID", item.CulturaId, DbType.Int32);
						comando.AdicionarParametroEntrada("cultivarID", item.CultivarId, DbType.Int32);

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							while (reader.Read())
							{
								PTVProdutoRelatorio procuto = new PTVProdutoRelatorio
								{
									Origem = reader.GetValue<int>("origem"),
									OrigemTipo = reader.GetValue<int>("origem_tipo"),
									CulturaId = reader.GetValue<int>("cultura"),
									CultivarId = reader.GetValue<int>("cultivar")
								};
								List<TratamentosRelatorio> tratamentos = TratamentoFitossanitario(new List<PTVProdutoRelatorio> { procuto }, bancoDeDados);

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

		/// <summary>
		///	Método recursivo que busca os tratamentos Fitossanitários dos produtos relacionados do histórico, 
		///	trazendo os 5 primeiros de acordo com a ordem em que foram adicionados
		/// </summary>
		/// <param name="origens"></param>
		/// <returns>List<TratamentoFitossanitario></returns>
		/// 
		internal List<TratamentosRelatorio> TratamentoFitossanitarioHistorico(List<PTVProdutoRelatorio> origens)
		{
			List<TratamentosRelatorio> retorno = new List<TratamentosRelatorio>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				retorno = TratamentoFitossanitarioHistorico(origens, bancoDeDados);
			}

			return retorno.GroupBy(p => new { p.ProdutoComercial, p.IngredienteAtivo, p.Dose, p.PragaProduto, p.ModoAplicacao }).Select(g => g.First()).ToList();
		}
		internal List<TratamentosRelatorio> TratamentoFitossanitarioHistorico(List<PTVProdutoRelatorio> origens, BancoDeDados bancoDeDados)
		{
			int limiteTratamentosFitossanitarios = 5;
			List<TratamentosRelatorio> retorno = new List<TratamentosRelatorio>();
			Comando comando = null;

			foreach (var item in origens)
			{
				switch (item.OrigemTipo)
				{
					case (int)eDocumentoFitossanitarioTipo.CFO:
						#region Buscar tratamento CFO
						using (BancoDeDados bancoDeDadosCred = BancoDeDados.ObterInstancia(UsuarioCredenciado))
						{
							comando = bancoDeDadosCred.CriarComando(@"select t1.id, t1.produto_comercial, t1.ingrediente_ativo, t1.dose, t1.praga_produto, t1.modo_aplicacao 
							from hst_cfo_trata_fitossa t1, hst_cfo cfo where cfo.id = t1.id_hst and cfo.cfo_id = :cfoId and cfo.tid = :cfoTid order by t1.id", UsuarioCredenciado);
							comando.AdicionarParametroEntrada("cfoId", item.Origem, DbType.Int32);
							comando.AdicionarParametroEntrada("cfoTid", item.OrigemTid, DbType.String);

							using (IDataReader reader = bancoDeDadosCred.ExecutarReader(comando))
							{
								while (reader.Read())
								{
									retorno.Add(new TratamentosRelatorio()
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
							comando = bancoDeDadosCred.CriarComando(@"select t1.id, t1.produto_comercial, t1.ingrediente_ativo, t1.dose, t1.praga_produto, t1.modo_aplicacao 
							from hst_cfoc_trata_fitossa t1, hst_cfoc cfoc where cfoc.id = t1.id_hst and cfoc.cfoc_id = :cfocId and cfoc.tid = :cfocTid order by t1.id", UsuarioCredenciado);
							comando.AdicionarParametroEntrada("cfocId", item.Origem, DbType.Int32);
							comando.AdicionarParametroEntrada("cfocTid", item.OrigemTid, DbType.String);

							using (IDataReader reader = bancoDeDadosCred.ExecutarReader(comando))
							{
								while (reader.Read())
								{
									retorno.Add(new TratamentosRelatorio()
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

						comando = bancoDeDados.CriarComando(@"select t.origem_id, t.origem_tid, t.origem_tipo_id, t.cultura_id, t.cultivar_id
						from hst_ptv_produto t, hst_ptv ptv  where ptv.id = t.id_hst and ptv.ptv_id = :origemId and ptv.tid = :origemTid
						and t.cultura_id = :culturaID and t.cultivar_id = :cultivarID order by t.id", EsquemaBanco);
						comando.AdicionarParametroEntrada("origemId", item.Origem, DbType.Int32);
						comando.AdicionarParametroEntrada("origemTid", item.OrigemTid, DbType.String);
						comando.AdicionarParametroEntrada("culturaID", item.CulturaId, DbType.Int32);
						comando.AdicionarParametroEntrada("cultivarID", item.CultivarId, DbType.Int32);

						using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
						{
							while (reader.Read())
							{
								PTVProdutoRelatorio procuto = new PTVProdutoRelatorio
								{
									Origem = reader.GetValue<int>("origem_id"),
									OrigemTid = reader.GetValue<string>("origem_tid"),
									OrigemTipo = reader.GetValue<int>("origem_tipo_id"),
									CulturaId = reader.GetValue<int>("cultura_id"),
									CultivarId = reader.GetValue<int>("cultivar_id")
								};
								List<TratamentosRelatorio> tratamentos = TratamentoFitossanitario(new List<PTVProdutoRelatorio> { procuto }, bancoDeDados);

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
			}
			return retorno;
		}

		internal List<PTVProdutoRelatorio> ObterOrigens(List<PTVProdutoRelatorio> origens)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				List<PTVProdutoRelatorio> listaOrigem = new List<PTVProdutoRelatorio>();

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
								listaOrigem.Add(new PTVProdutoRelatorio() { Origem = reader.GetValue<int>("origem"), OrigemTipo = reader.GetValue<int>("origem_tipo") });
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
						listaOrigem.Add(new PTVProdutoRelatorio() { Origem = x.Origem, OrigemTipo = x.OrigemTipo });
					}
				});

				return listaOrigem.GroupBy(x => new { x.OrigemTipo, x.Origem }).Select(g => g.First()).ToList();
			}
		}

		internal List<LaudoLaboratorial> ObterLaudoLaboratorial(List<PTVProdutoRelatorio> origens)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
			{
				List<PTVProdutoRelatorio> listaOrigem = ObterOrigens(origens);
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

		#endregion
	}
}