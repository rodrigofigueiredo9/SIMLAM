using System;
using System.Collections;
using System.Collections.Generic;

namespace Tecnomapas.EtramiteX.WindowsService.Utilitarios
{
	public class LogProcessamento
	{
		private DateTime dtStart;
		private DateTime dtEnd;
		private bool timeIniciado;
		private bool timeFinalizado;
		private string nomeRecurso;

		private List<Hashtable> execucoes;

		public LogProcessamento()
		{
			this.timeIniciado = false;
			this.timeFinalizado = false;
			this.nomeRecurso = string.Empty;
			this.execucoes = new List<Hashtable>();
		}

		public List<Hashtable> Execucoes
		{
			get { return execucoes; }
		}

		private void RegistrarExecucao()
		{
			if (!this.timeIniciado || !this.timeFinalizado)
			{
				throw new Exception("Tempo de execução não iniciado/finalizado.");
			}

			Hashtable ht = new Hashtable();

			ht.Add("NOME_RECURSO", this.nomeRecurso);
			ht.Add("TEMPO", dtEnd.Subtract(dtStart).TotalSeconds);

			execucoes.Add(ht);

			this.timeIniciado = false;
			this.timeFinalizado = false;
			this.nomeRecurso = string.Empty;
		}

		public void IniciarTime(string nomeRecurso)
		{
			this.nomeRecurso = nomeRecurso;
			this.timeIniciado = true;
			dtStart = DateTime.Now;
		}

		public void FinalizarTime()
		{
			if (!this.timeIniciado)
			{
				throw new Exception("Tempo de execução não iniciado.");
			}

			this.timeFinalizado = true;

			dtEnd = DateTime.Now;

			RegistrarExecucao();
		}
	}
}