using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Tecnomapas.Blocos.Data;
using Tecnomapas.EtramiteX.Configuracao;
using Tecnomapas.Blocos.Etx.ModuloExtensao.Data;

namespace Tecnomapas.EtramiteX.Interno.Model.Security
{
	class ControleAcessoDa
	{
		GerenciadorConfiguracao _config = new GerenciadorConfiguracao(new ConfiguracaoSistema());

		public String EsquemaBanco
		{
			get { return _config.Obter<String>(ConfiguracaoSistema.KeyUsuarioInterno); }
		}

		public void Gerar(Controle controle, BancoDeDados banco = null)
		{
			using (BancoDeDados bancoDeDados = BancoDeDados.ObterInstancia(banco))
			{
				bancoDeDados.IniciarTransacao();

				Comando comando = bancoDeDados.CriarComando(@"insert into {0}log_controle_acesso (id, tid, artefato_id, ip_acesso, caracterizacao_tipo, 
															executor_id, executor_tid, executor_nome, executor_login, executor_tipo_id, 
															executor_tipo_texto, acao_executada, data_execucao)  values ({0}seq_controle_acesso.nextval, 
															:tid, :artefato_id, :ip_acesso, :caracterizacao_tipo, :executor_id, :executor_tid, :executor_nome, 
															:executor_login, :executor_tipo_id, (select ltf.texto from lov_executor_tipo ltf where ltf.id = :executor_tipo_id), 
															(select la.id from  lov_historico_artefatos_acoes la where la.acao = :acao and la.artefato = :artefato), 
															systimestamp)", EsquemaBanco);

				comando.AdicionarParametroEntrada("ip_acesso", controle.Ip, DbType.String);
				comando.AdicionarParametroEntrada("artefato_id", controle.ArtefatoId, DbType.Int32);
				comando.AdicionarParametroEntrada("executor_id", controle.Executor.Id, DbType.Int32);
				comando.AdicionarParametroEntrada("executor_tid", controle.Executor.Tid, DbType.String);
				comando.AdicionarParametroEntrada("executor_nome", controle.Executor.Nome, DbType.String);
				comando.AdicionarParametroEntrada("executor_login", controle.Executor.Login, DbType.String);
				comando.AdicionarParametroEntrada("executor_tipo_id", (int)controle.Executor.Tipo, DbType.Int32);
				comando.AdicionarParametroEntrada("acao", controle.Acao, DbType.Int32);
				comando.AdicionarParametroEntrada("artefato", controle.ArtefatoTipo, DbType.Int32);
				comando.AdicionarParametroEntrada("caracterizacao_tipo", controle.Caracterizacao > 0 ? controle.Caracterizacao : (Object)DBNull.Value, DbType.Int32);
				comando.AdicionarParametroEntrada("tid", DbType.String, 36, GerenciadorTransacao.ObterIDAtual());

				bancoDeDados.ExecutarNonQuery(comando);
				bancoDeDados.Commit();
			}
		}
	}
}
