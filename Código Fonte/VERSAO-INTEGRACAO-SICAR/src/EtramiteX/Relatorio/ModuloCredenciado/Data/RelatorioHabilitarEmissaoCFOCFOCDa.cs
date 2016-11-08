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
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloCredenciado;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloCredenciado.Data
{
	public class RelatorioHabilitarEmissaoCFOCFOCDa
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

		public RelatorioHabilitarEmissaoCFOCFOCDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter / Filtrar

		internal HabilitarEmissaoCFOCFOCRelatorio Obter(int id)
		{
			HabilitarEmissaoCFOCFOCRelatorio habilitar = null;
			PragaHabilitarEmissaoRelatorio praga = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region Habilitar Emissão

				Comando comando = bancoDeDados.CriarComando(@"
				select t.id, t.tid, t.responsavel, t.responsavel_arquivo, aa.nome responsavel_arquivo_nome, p.nome responsavel_nome, p.cpf responsavel_cpf, p.rg responsavel_rg, 
				t.numero_habilitacao, trunc(t.validade_registro) validade_registro, trunc(t.situacao_data) situacao_data, t.situacao, ls.texto situacao_texto, t.motivo, 
				lm.texto motivo_texto, t.observacao, t.numero_dua, t.extensao_habilitacao, t.numero_habilitacao_ori, t.registro_crea, t.uf, t.numero_visto_crea, 
				(select h.data_execucao from hst_hab_emi_cfo_cfoc h where h.habilitar_emissao_id = t.id and h.acao_executada = 483) data_cadastro
				from tab_hab_emi_cfo_cfoc t, cre_pessoa p, tab_credenciado cc, lov_hab_emissao_cfo_situacao ls, lov_hab_emissao_cfo_motivo lm, tab_arquivo aa 
				where t.situacao = ls.id and t.motivo = lm.id(+) and t.responsavel_arquivo = aa.id(+) and t.responsavel = cc.id and cc.pessoa = p.id and t.id = :id", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					if (reader.Read())
					{
						habilitar = new HabilitarEmissaoCFOCFOCRelatorio();
						habilitar.Id = reader.GetValue<Int32>("id");
						habilitar.Tid = reader.GetValue<String>("tid");
						habilitar.Responsavel.Id = reader.GetValue<Int32>("responsavel");
						habilitar.Foto.Id = reader.GetValue<Int32>("responsavel_arquivo");
						habilitar.Foto.Nome = reader.GetValue<String>("responsavel_arquivo_nome");
						habilitar.Responsavel.NomeRazaoSocial = reader.GetValue<String>("responsavel_nome");
						habilitar.Responsavel.CPFCNPJ = reader.GetValue<String>("responsavel_cpf");
						habilitar.Responsavel.Fisica.RG = reader.GetValue<String>("responsavel_rg");
						habilitar.NumeroHabilitacao = reader.GetValue<String>("numero_habilitacao");
						habilitar.ValidadeRegistro = reader.GetValue<DateTime>("validade_registro").ToShortDateString();
						habilitar.SituacaoData = reader.GetValue<DateTime>("situacao_data").ToShortDateString();
						habilitar.SituacaoTexto = reader.GetValue<String>("situacao_texto");
						habilitar.MotivoTexto = reader.GetValue<String>("motivo_texto");
						habilitar.Observacao = reader.GetValue<String>("observacao");
						habilitar.NumeroDua = reader.GetValue<String>("numero_dua");
						habilitar.ExtensaoHabilitacaoBool = (reader.GetValue<Int32>("extensao_habilitacao") == 1);
						habilitar.ExtensaoHabilitacao = (habilitar.ExtensaoHabilitacaoBool ? "Sim" : "Não");
						habilitar.NumeroHabilitacaoOrigem = reader.GetValue<String>("numero_habilitacao_ori");
						habilitar.RegistroCrea = reader.GetValue<String>("registro_crea");
						habilitar.NumeroVistoCrea = reader.GetValue<String>("numero_visto_crea");
						habilitar.DataCadastro = reader.GetValue<DateTime>("data_cadastro");
					}

					reader.Close();
				}

				#endregion

				#region Pragas

				comando = bancoDeDados.CriarComando(@"
				select hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, trunc(hp.data_habilitacao_inicial) data_habilitacao_inicial,
				trunc(hp.data_habilitacao_final) data_habilitacao_final, hp.tid, stragg(c.texto) cultura
				from tab_hab_emi_cfo_cfoc_praga hp, tab_praga pa, tab_praga_cultura pc, tab_cultura c 
				where hp.praga = pa.id and hp.praga = pc.praga(+) and pc.cultura = c.id(+) and hp.habilitar_emi_id = :id
				group by hp.id, hp.praga, pa.nome_cientifico, pa.nome_comum, hp.data_habilitacao_inicial, hp.data_habilitacao_final, hp.tid", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						praga = new PragaHabilitarEmissaoRelatorio();
						praga.Id = reader.GetValue<Int32>("id");
						praga.Praga.Id = reader.GetValue<Int32>("praga");
						praga.Praga.NomeCientifico = reader.GetValue<String>("nome_cientifico");
						praga.Praga.NomeComum = reader.GetValue<String>("nome_comum");
						praga.DataInicialHabilitacao = reader.GetValue<DateTime>("data_habilitacao_inicial").ToShortDateString(); ;
						praga.DataFinalHabilitacao = reader.GetValue<DateTime>("data_habilitacao_final").ToShortDateString(); ;
						praga.Tid = reader.GetValue<String>("tid");
						praga.Cultura = reader.GetValue<String>("cultura");
						habilitar.Pragas.Add(praga);
					}
					reader.Close();
				}

				#endregion

				#region Arquivo

				if (habilitar.Foto.Id.HasValue && habilitar.Foto.Id > 0)
				{
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					habilitar.Foto = _busArquivo.Obter(habilitar.Foto.Id.Value);
				}

				#endregion
			}

			if (habilitar != null)
			{
				using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(UsuarioCredenciado))
				{
					#region Profissão/Endereço

					Comando comando = bancoDeDados.CriarComando(@"select pp.id, pp.registro, p.texto profissao, ee.logradouro, ee.numero, ee.cep, ee.bairro, ee.distrito, (select es.sigla from lov_estado es where es.id = ee.estado) estado,
					(select em.texto from lov_municipio em where em.id = ee.municipio) municipio, mm.valor email from tab_credenciado c,
					tab_pessoa_profissao pp, tab_profissao p, tab_pessoa_endereco ee, tab_pessoa_meio_contato mm
					where c.pessoa = pp.pessoa and p.id = pp.profissao and c.id = :id and c.pessoa = ee.pessoa(+) 
					and c.pessoa = mm.pessoa(+) and mm.meio_contato(+) = 5 and rownum = 1");

					comando.AdicionarParametroEntrada("id", habilitar.Responsavel.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						if (reader.Read())
						{
							habilitar.Profissao.ProfissaoId = reader.GetValue<Int32>("id");
							habilitar.Profissao.ProfissaoTexto = reader.GetValue<String>("profissao");
							habilitar.Profissao.Registro = reader.GetValue<String>("registro");

							habilitar.Responsavel.Endereco.Logradouro = reader.GetValue<String>("logradouro");
							habilitar.Responsavel.Endereco.Cep = reader.GetValue<String>("cep");
							habilitar.Responsavel.Endereco.Numero = reader.GetValue<String>("numero");
							habilitar.Responsavel.Endereco.Bairro = reader.GetValue<String>("bairro");
							habilitar.Responsavel.Endereco.Distrito = reader.GetValue<String>("distrito");
							habilitar.Responsavel.Endereco.EstadoTexto = reader.GetValue<String>("estado");
							habilitar.Responsavel.Endereco.MunicipioTexto = reader.GetValue<String>("municipio");
						}

						reader.Close();
					}

					#endregion

					#region Contato

					comando = bancoDeDados.CriarComando(@"select mm.meio_contato, mm.valor from tab_credenciado c, tab_pessoa_meio_contato mm
					where c.id = :id and c.pessoa = mm.pessoa");

					comando.AdicionarParametroEntrada("id", habilitar.Responsavel.Id, DbType.Int32);

					using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
					{
						while (reader.Read())
						{
							ContatoRelatorio contato = new ContatoRelatorio();
							contato.TipoContato = (Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.Entities.eTipoContato)reader.GetValue<Int32>("meio_contato");
							contato.Valor = reader.GetValue<String>("valor");
							habilitar.Responsavel.MeiosContatos.Add(contato);
						}

						reader.Close();
					}

					#endregion
				}
			}

			return habilitar;
		}

		#endregion
	}
}