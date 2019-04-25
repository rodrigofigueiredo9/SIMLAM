using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Configuracao.Interno;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloCadastroAmbientalRural;
using Tecnomapas.Blocos.Entities.Interno.ModuloDUA;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Entities;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Credenciado.Model.ModuloDUA.Data
{
	public class DuaDa
	{
		#region Propriedades

		GerenciadorConfiguracao<ConfiguracaoSistema> _configSis = new GerenciadorConfiguracao<ConfiguracaoSistema>(new ConfiguracaoSistema());
		Historico _historico = new Historico();
		Consulta _consulta = new Consulta();

		public static EtramiteIdentity User
		{
			get { return (HttpContext.Current.User as EtramitePrincipal).EtramiteIdentity; }
		}

		public string UsuarioCredenciado
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioCredenciado); }
		}
		public string UsuarioInterno
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}
		public string UsuarioConsulta
		{
			get { return _configSis.Obter<string>(ConfiguracaoSistema.KeyUsuarioConsulta); }
		}

		internal Historico Historico { get { return _historico; } }
		internal Consulta Consulta { get { return _consulta; } }
		#endregion

		public List<Dua> Obter(int titulo, BancoDeDados banco = null)
		{
			List<Dua> retorno = new List<Dua>();

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco, UsuarioCredenciado))
			{
				#region Solicitação

				Comando comando = bancoDeDados.CriarComando(@"
					SELECT  D.ID, LPAD(D.ID, 4, '0') || ' - ' || TO_CHAR(D.DATAHORA,'DD/MM/YYYY') codigo,
							D.VALORTOTALDUA valor, D.SITUACAO, D.NUMERODUA numero_dua, TO_CHAR(E.DATA_VENCIMENTO,'DD/MM/YYYY') validade,
							E.CNPJ_PESSOA
						FROM {0}TAB_INFCORTE_SEFAZDUA D 
							INNER JOIN {0}TAB_INFCORTE_EMISSAO_DUA E ON E.ID = D.EMISSAO_DUA
						WHERE TITULO = :titulo", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Dua dua = new Dua();
						dua.Codigo = reader.GetValue<String>("codigo");
						dua.Valor = reader.GetValue<Decimal>("valor");
						dua.Situacao = (eSituacaoDua)reader.GetValue<int>("situacao");
						dua.SituacaoTexto = ((eSituacaoDua)reader.GetValue<int>("situacao")).ToDescription();
						dua.Numero = reader.GetValue<String>("numero_dua");
						dua.Validade = new DateTecno() { Data = reader.GetValue<DateTime>("validade") };
						dua.CpfCnpj = reader.GetValue<String>("cnpj_pessoa").Replace("-", "").Replace("/", "").Replace(".", "");

						retorno.Add(dua);
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}
		
		internal bool ExisteDuaTitulo(int titulo)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(null, UsuarioCredenciado))
			{
				Comando comando = bancoDeDados.CriarComando(@"
					SELECT count(1) FROM TAB_INFCORTE_SEFAZDUA WHERE TITULO = :titulo");

				comando.AdicionarParametroEntrada("titulo", titulo, DbType.Int32);

				return (bancoDeDados.ExecutarScalar<int>(comando) > 0);

			}
		}

	}
}