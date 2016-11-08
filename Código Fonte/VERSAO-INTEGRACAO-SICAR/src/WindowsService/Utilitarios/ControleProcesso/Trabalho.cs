using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Tecnomapas.EtramiteX.WindowsService.Utilitarios.ControleProcesso
{
    public abstract class Trabalho
    {
        Semaphore mutexServico;
        Semaphore mutexProcesso;

        public Trabalho(string mutexServico, string mutexProcesso)
        {
            this.mutexProcesso = new Semaphore(1, 1, mutexProcesso);
            this.mutexProcesso.WaitOne();

            this.mutexServico = new Semaphore(1, 1, mutexServico);
            
        }

        public void Executar()
        {
            try
            {
                ExecutarProcesso();
            }
            finally
            {
                LiberarRecursos();
            }
        }

        protected abstract void ExecutarProcesso();


        protected bool EncerrarProcesso()
        {
            bool meu = mutexServico.WaitOne(500);
            return meu;
        }
        
        void LiberarRecursos()
        {
            mutexServico.Close();
            mutexProcesso.Release();
        }
    }
}
