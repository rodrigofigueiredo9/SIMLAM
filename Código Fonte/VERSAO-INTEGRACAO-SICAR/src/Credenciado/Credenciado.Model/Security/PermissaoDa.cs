using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Credenciado.Security;
using Tecnomapas.Blocos.Etx.ModuloCore.Data;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Credenciado.Model.Security
{
	public class PermissaoDa
	{
		public string ObterNome(ePermissao permissao)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.nome nome from lov_autenticacao_permissao t where t.codigo = :codigo");
				comando.AdicionarParametroEntrada("codigo", permissao.ToString(), DbType.String);

				Object retorno = bancoDeDados.ExecutarScalar(comando);

				if (!Convert.IsDBNull(retorno) && retorno != null)
				{
					return retorno.ToString();
				}

				return String.Empty;
			}
		}

		public List<string> ObterGrupoNome(List<ePermissao> permissoes)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia())
			{
				Comando comando = bancoDeDados.CriarComando(@"select t.grupo||' - '||t.nome nome from lov_autenticacao_permissao t where ");
				comando.DbCommand.CommandText += comando.AdicionarIn("", "t.codigo", DbType.String, permissoes);

				IEnumerable<IDataReader> daReader = DaHelper.ObterLista(comando, bancoDeDados);

				List<string> lstPermissoes = new List<string>();

				foreach (var item in daReader)
				{
					lstPermissoes.Add(item["nome"].ToString());
				}

				return lstPermissoes;
			}
		}
	}
}
