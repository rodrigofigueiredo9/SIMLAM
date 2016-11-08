using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Tecnomapas.EtramiteX.WindowsService.Utilitarios.ControleProcesso
{
    public class Processo
    {
        Process process;
        Semaphore mutexServico; 
        Semaphore mutexProcesso;
        bool terminou;

        private int _exitCode=0;
        public int ExitCode
        {
            get { return _exitCode; }
        }

        public Processo(string nomeArquivo, string mutexServico, string mutexProcesso)
        {
            this.mutexServico = new Semaphore(1, 1, mutexServico);
            this.mutexServico.WaitOne();
            this.mutexProcesso = new Semaphore(1, 1, mutexProcesso);
            this.process = new Process();
            this.process.StartInfo = new ProcessStartInfo();
            this.process.StartInfo.FileName = nomeArquivo;
            this.process.StartInfo.UseShellExecute = false;
            this.process.StartInfo.CreateNoWindow = true;
        }

        public void IniciarExecutar()
        {
            if (terminou) return;
            process.Start();
        }

        public void AguardarExecutar()
        {
            if (terminou) return;
            process.Start();
            process.WaitForExit();
            _exitCode = process.ExitCode;
            process.Dispose();
            mutexServico.Release();
            terminou = true;
        }

        public void Encerrar()
        {
            Encerrar(-1);            
        }

        public void Encerrar(int timeout)
        {
            mutexServico.Release();
            bool terminou = false;
            if (timeout < 0)
            {
                terminou = mutexProcesso.WaitOne();
            }
            else
            {
                terminou = mutexProcesso.WaitOne(timeout * 1000);
            }                
            if (!terminou)
            {
                process.Kill();
            }
            mutexProcesso.Close();
            process.Dispose();
            terminou = false;
        }
    }
}
