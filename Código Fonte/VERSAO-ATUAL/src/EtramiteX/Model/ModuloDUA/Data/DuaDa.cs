using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Entities.Interno.ModuloDUA;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;
using Tecnomapas.EtramiteX.Configuracao;

namespace Tecnomapas.EtramiteX.Interno.Model.ModuloCadastroAmbientalRural.Data
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

				Comando comando = bancoDeDados.CriarComando(@"", UsuarioCredenciado);

				comando.AdicionarParametroEntrada("id", titulo, DbType.Int32);

				using (IDataReader reader = bancoDeDados.ExecutarReader(comando))
				{
					while (reader.Read())
					{
						Dua dua = new Dua();
						dua.Codigo = reader.GetValue<String>("codigo");
						dua.Valor = reader.GetValue<Decimal>("valor");
						dua.Situacao = reader.GetValue<String>("situacao");
						dua.Numero = reader.GetValue<String>("numero_dua");
						dua.Validade = reader.GetValue<DateTecno>("validade");

						retorno.Add(dua);
					}

					reader.Close();
				}

				#endregion
			}

			return retorno;
		}

	}
}