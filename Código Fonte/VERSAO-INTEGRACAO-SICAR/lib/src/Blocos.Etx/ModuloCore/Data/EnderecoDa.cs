using System;
using System.Data;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.Blocos.Etx.ModuloCore.Data
{
	class EnderecoDa
	{
		public int ObterSetorId(String sigla, BancoDeDados banco = null) 
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				Comando comando = bancoDeDados.CriarComando(@"select id from tab_setor where sigla = :sigla");
				comando.AdicionarParametroEntrada("sigla", sigla, DbType.String);

				return bancoDeDados.ExecutarScalar<Int32>(comando);
			}
		}
	}
}
