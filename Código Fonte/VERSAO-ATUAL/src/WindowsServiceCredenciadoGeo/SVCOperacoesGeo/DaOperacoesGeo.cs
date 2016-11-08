using System;
using System.Collections.Generic;
using System.Data;
using Tecnomapas.Blocos.Data;

namespace Tecnomapas.EtramiteX.WindowsService.SVCOperacoesGeo
{
	public class DaOperacoesGeo
	{
		private const int FILA_SITUACAO_AGUARDANDO = 1;
		private const int FILA_SITUACAO_EXECUTANTO = 2;
		private const int FILA_SITUACAO_ERRO = 3;
		private const int FILA_SITUACAO_CONCLUIDO = 4;

		public BancoDeDados banco { get; set; }

		public DaOperacoesGeo(BancoDeDados banco)
		{
			this.banco = banco;
		}

		internal ConfigurationParams BuscarConfiguracoes()
		{
			int interval = 10;
			int duration = 3600;
			string settings = "";

			string strSQL = @"
			begin
				select 
					(select to_number(t.valor) from cnf_sistema_geo t where t.campo = 'SERVICO_GEO_-_INTERVALO_DO_TIMER'),
					(select to_number(t.valor) from cnf_sistema_geo t where t.campo = 'SERVICO_GEO_-_MAXIMA_DURACAO_DO_PROCESSO'),
					(select t.valor from cnf_sistema_geo t where t.campo = 'SERVICO_GEO_-_CONFIGURACOES_DA_FILA')
					into :intervalo, :duracao, :configuracao
				from dual;
			end;";

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroSaida("intervalo", DbType.Int32);
				comando.AdicionarParametroSaida("duracao", DbType.Int32);
				comando.AdicionarParametroSaida("configuracao", DbType.String, 4000);
				this.banco.ExecutarNonQuery(comando);

				interval = Convert.ToInt32(comando.ObterValorParametro("intervalo"));
				duration = Convert.ToInt32(comando.ObterValorParametro("duracao"));
				settings = Convert.ToString(comando.ObterValorParametro("configuracao"));
			}

			return new ConfigurationParams(interval, duration, settings);
		}

		internal void InvalidarExecucoes()
		{
			string strSQL = @"
			begin
				update tab_fila f set f.situacao=" + FILA_SITUACAO_AGUARDANDO + @", f.data_fila = sysdate, f.data_inicio=null, f.data_fim=null where f.situacao = " + FILA_SITUACAO_EXECUTANTO + @";
			end;";

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				this.banco.IniciarTransacao();
				this.banco.ExecutarNonQuery(comando);
				this.banco.Commit();
			}

		}

		internal Ticket ReservarTicket(List<ItemFila> filtros)
		{
			if (filtros == null || filtros.Count == 0)
				return null;

			Ticket ticket = new Ticket();

			List<string> strFilterArray = new List<string>();

			int count = filtros.Count;
			for (int i = 0; i < count; i++)
			{
				if (filtros[i].tipo > 0)
				{
					if (filtros[i].etapa > 0)
						strFilterArray.Add("f.tipo=" + filtros[i].tipo + " or f.etapa=" + filtros[i].etapa);
					else
						strFilterArray.Add("f.tipo=" + filtros[i].tipo);
				}
				else if (filtros[i].etapa > 0)
				{
					strFilterArray.Add("f.etapa=" + filtros[i].etapa);
				}
			}

			string strFilter = (strFilterArray.Count > 0) ? "and ( " + String.Join(" and ", strFilterArray.ToArray()) + @" )" : "";

			string strSQL = @"
			begin
				:id := 0;
				:type := 0;
				:step := 0;

				for i in (select f.id, f.projeto, f.tipo, f.etapa
							from tab_fila f where f.situacao = " + FILA_SITUACAO_AGUARDANDO + " " + strFilter + @" 
							order by data_fila ) loop

				update tab_fila f set f.situacao=" + FILA_SITUACAO_EXECUTANTO + @", f.data_inicio=sysdate, f.data_fim = null where f.id = i.id;

				:id := i.projeto;
				:type := i.tipo;
				:step := i.etapa;

				exit;
				end loop;
			end;";

			strSQL = strSQL.Replace("\r", "").Replace("\n", "");

			this.banco.IniciarTransacao();
			using (Comando comando = this.banco.CriarComando(strSQL))
			{
				comando.AdicionarParametroSaida("id", DbType.Int32);
				comando.AdicionarParametroSaida("type", DbType.Int32);
				comando.AdicionarParametroSaida("step", DbType.Int32);

				this.banco.ExecutarNonQuery(comando);

				ticket.id = Convert.ToInt32(comando.ObterValorParametro("id"));
				ticket.tipo = Convert.ToInt32(comando.ObterValorParametro("type"));
				ticket.etapa = Convert.ToInt32(comando.ObterValorParametro("step"));
			}

			this.banco.Commit();

			return (ticket.tipo > 0) ? ticket : null;
		}

		public Dictionary<string, string> ObterParameters()
		{
			Comando com = this.banco.CriarComando("select * from v$nls_parameters");

			IDataReader reader = this.banco.ExecutarReader(com);

			Dictionary<string, string> dic = new Dictionary<string, string>();

			while (reader.Read())
			{
				dic.Add(reader[0].ToString(), reader[1].ToString());
			}

			reader.Close();

			return dic;
		}
	}
}