using System;
using System.Web;
using System.Web.Mvc;
using Tecnomapas.Blocos.Arquivo;
using Tecnomapas.Blocos.Entities.Etx.ModuloCore;
using Tecnomapas.Blocos.Entities.Etx.ModuloSecurity;
using Tecnomapas.Blocos.Etx.ModuloArquivo.Business;
using Tecnomapas.Blocos.Etx.ModuloValidacao;
using Tecnomapas.EtramiteX.Credenciado.Model.Security;
using Tecnomapas.EtramiteX.Credenciado.ViewModels;

namespace Tecnomapas.EtramiteX.Credenciado.Controllers
{
    public class ArquivoController : DefaultController
    {
        [HttpPost]
        [Permite(Tipo = ePermiteTipo.Logado)]
        public string Arquivo(HttpPostedFileBase file)
        {
            string msg = string.Empty;
            Arquivo arquivo = null;
            try
            {
                ArquivoBus _bus;
                string url = HttpContext.Request.UrlReferrer.ToString();
                if (url.IndexOf("PTVOutro") >= 0)
                    _bus = new ArquivoBus(eExecutorTipo.Interno);
                else
                    _bus = new ArquivoBus(eExecutorTipo.Credenciado);

                arquivo = _bus.SalvarTemp(file);
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }

            return ViewModelHelper.JsSerializer.Serialize(new { Msg = Validacao.Erros, Arquivo = arquivo });
        }

        [Permite(Tipo = ePermiteTipo.Logado)]
        public FileResult Baixar(int id)
        {
            string url = HttpContext.Request.UrlReferrer.ToString();
            if (url.IndexOf("PTVOutro") >= 0)
                return ViewModelHelper.BaixarArquivoInterno(id);
            else
                return ViewModelHelper.BaixarArquivo(id);

        }


        public FileResult BaixarInterno(int id)
        {
            return ViewModelHelper.BaixarArquivoInterno(id);
        }


        [Permite(Tipo = ePermiteTipo.Logado)]
        public FileResult BaixarTemporario(string nomeTemporario, string contentType)
        {
            return ViewModelHelper.BaixarArquivoTemporario(nomeTemporario, contentType);
        }
    }
}