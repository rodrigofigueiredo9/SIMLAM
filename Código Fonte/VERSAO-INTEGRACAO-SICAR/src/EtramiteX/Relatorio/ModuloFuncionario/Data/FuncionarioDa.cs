using System.Data;
using Tecnomapas.Blocos.Data;
using Tecnomapas.Blocos.Entities.Interno.RelatorioIndividual.ModuloFuncionario;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.RelatorioIndividual.ModuloFuncionario.Data
{
	public class FuncionarioDa
	{
		#region Propriedade e Atributos

		private string EsquemaBanco { get; set; }

		#endregion

		public FuncionarioDa(string strBancoDeDados = null)
		{
			EsquemaBanco = string.Empty;
			if (!string.IsNullOrEmpty(strBancoDeDados))
			{
				EsquemaBanco = strBancoDeDados;
			}
		}

		#region Obter

		public FuncionarioRelatorio Obter(int id, BancoDeDados banco = null)
		{
			FuncionarioRelatorio objeto = new FuncionarioRelatorio();
			Comando comando = null;
			
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select tf.id Id, tf.nome Nome, stragg(tc.nome) CargoTexto from {0}tab_funcionario tf, {0}tab_funcionario_cargo tfc, {0}tab_cargo tc where tf.id = :id
					and tf.id = tfc.funcionario and tfc.cargo = tc.id group by tf.id,tf.nome ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);

				objeto = bancoDeDados.ObterEntity<FuncionarioRelatorio>(comando);
			}

			return objeto;
		}


		public FuncionarioRelatorio ObterHistorico(int id, string tid, BancoDeDados banco = null)
		{
			FuncionarioRelatorio objeto = new FuncionarioRelatorio();
			Comando comando = null;

			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				comando = bancoDeDados.CriarComando(@" select tf.id Id, tf.nome Nome, stragg(tc.nome) CargoTexto 
					from hst_funcionario tf, hst_funcionario_cargo tfc, tab_cargo tc 
					where tf.funcionario_id = :id
					  and tf.tid = :tid
					  and tf.id = tfc.id_hst 
					  and tfc.cargo_id = tc.id
					group by tf.id, tf.nome ", EsquemaBanco);

				comando.AdicionarParametroEntrada("id", id, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", tid, DbType.String);

				objeto = bancoDeDados.ObterEntity<FuncionarioRelatorio>(comando);
			}

			return objeto;
		}

		#endregion
	}
}
