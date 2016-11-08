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
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Business;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Configuracao;
using Cred = Tecnomapas.Blocos.Entities.Interno.ModuloCredenciado;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloLiberacaoNumeroCFOCFOC.Data
{
	public class LiberacaoNumeroCFOCFOCDa
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

		public LiberacaoNumeroCFOCFOCDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter / Filtrar

		internal ComprovanteLiberacaoNumeroCFOCFOCRelatorio Obter(int id)
		{
			ComprovanteLiberacaoNumeroCFOCFOCRelatorio comprovante = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select p.nome, p.cpf, l.numero_inicial_cfo, l.numero_final_cfo, l.numero_inicial_cfoc, l.numero_final_cfoc, l.qtd_num_cfo, l.qtd_num_cfoc  from tab_liberacao_cfo_cfoc l, tab_credenciado c, 
				{0}tab_pessoa p where l.responsavel_tecnico = c.id and c.pessoa = p.id and l.id = :id", UsuarioCredenciado);
				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						comprovante = new ComprovanteLiberacaoNumeroCFOCFOCRelatorio();
						comprovante.Nome = reader.GetValue<string>("nome");
						comprovante.NumeroBlocoFinalCFO = reader.GetValue<string>("numero_final_cfo");
						comprovante.NumeroBlocoInicialCFO = reader.GetValue<string>("numero_inicial_cfo");
						comprovante.NumeroBlocoInicialCFOC = reader.GetValue<string>("numero_inicial_cfoc");
						comprovante.NumeroBlocoFinalCFOC = reader.GetValue<string>("numero_final_cfoc");
						comprovante.QtdNumeroDigitalCFO = reader.GetValue<string>("qtd_num_cfo");
						comprovante.QtdNumeroDigitalCFOC = reader.GetValue<string>("qtd_num_cfoc");
						comprovante.CPF = reader.GetValue<string>("cpf");
					}
					reader.Close();
				}
			}
						

			return comprovante;
		}

		#endregion
	}
}