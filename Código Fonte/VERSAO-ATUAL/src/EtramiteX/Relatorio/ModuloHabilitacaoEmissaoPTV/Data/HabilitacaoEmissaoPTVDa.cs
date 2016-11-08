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
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloHabilitacaoEmissaoPTV.Data
{
	public class HabilitacaoEmissaoDa
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

		public HabilitacaoEmissaoDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		internal HabilitacaoEmissaoPTVRelatorio Obter(int id)
		{
			HabilitacaoEmissaoPTVRelatorio habilitacao = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				#region SQL
				Comando comando = bancoDeDados.CriarComando(@"select 
																t.arquivo, 
																t.id, 
																t.numero_habilitacao, 
																t.numero_matricula, 
																f.nome, 
																p.texto profissao, 
																o.orgao_sigla orgao_classe, 
																f.cpf, 
																t.rg, 
																t.registro_orgao_classe, 
																m.texto municipio, 
																e.sigla estado, 
																t.cep, 
																t.telefone_residencial, 
																t.telefone_celular, 
																t.telefone_comercial, 
																(t.logradouro || ' ' || t.numero || ' ' || t.bairro_gleba || ' ' || t.distrito_localidade) endereco, 
																f.email, 
																t.numero_visto_crea 
															from tab_hab_emi_ptv t, tab_funcionario f, tab_profissao p, tab_orgao_classe o, lov_municipio m, lov_estado e 
															where f.id(+) = t.funcionario 
															  and p.id(+) = t.profissao 
															  and o.id(+) = t.orgao_classe 
															  and m.id(+) = t.municipio 
															  and e.id(+) = t.estado 
															  and t.id    =:id");
				#endregion

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						habilitacao = new HabilitacaoEmissaoPTVRelatorio();
						habilitacao.CEP = reader.GetValue<string>("cep");
						habilitacao.CPF = reader.GetValue<string>("cpf");
						habilitacao.CreaNumero = reader.GetValue<string>("numero_visto_crea");
						habilitacao.CreaRegistro = reader.GetValue<string>("registro_orgao_classe");
						habilitacao.Email = reader.GetValue<string>("email");
						habilitacao.Endereco = reader.GetValue<string>("endereco");
						habilitacao.HabilitacaoNumero = reader.GetValue<string>("numero_habilitacao");
						habilitacao.Id = reader.GetValue<int>("id");
						habilitacao.MatriculaNumero = reader.GetValue<string>("numero_matricula");
						habilitacao.Municipio = reader.GetValue<string>("municipio");
						habilitacao.Nome = reader.GetValue<string>("nome");
						habilitacao.Profissao = reader.GetValue<string>("profissao");
						habilitacao.RG = reader.GetValue<string>("rg");
						habilitacao.OrgaoClasse = reader.GetValue<string>("orgao_classe");
						habilitacao.TelCelular = reader.GetValue<string>("telefone_celular");
						habilitacao.TelComercial = reader.GetValue<string>("telefone_comercial");
						habilitacao.TelResidencial = reader.GetValue<string>("telefone_residencial");
						habilitacao.UF = reader.GetValue<string>("estado");
						habilitacao.Foto.Id = reader.GetValue<int>("arquivo");

						habilitacao.TermoNumero = habilitacao.HabilitacaoNumero.Substring(habilitacao.HabilitacaoNumero.Length - 4) + "/" + habilitacao.HabilitacaoNumero.Substring(habilitacao.HabilitacaoNumero.Length - 6, 2);
					}
					reader.Close();
				}

				#region Arquivo
				
				if (habilitacao.Foto.Id.HasValue && habilitacao.Foto.Id > 0)
				{
					ArquivoBus _busArquivo = new ArquivoBus(eExecutorTipo.Interno);

					habilitacao.Foto = _busArquivo.Obter(habilitacao.Foto.Id.Value);
				}
				 
				#endregion
			}

			return habilitacao;
		}

		#endregion
	}
}