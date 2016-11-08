using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using Tecnomapas.Geobases.WebServices.Models.Entities;
using Tecnomapas.Geobases.WebServices.Models.Business;
using Tecnomapas.Geobases.WebServices.Models.Security;
using Tecnomapas.Blocos.Etx.ModuloValidacao;

namespace Tecnomapas.Geobases.WebServices.Controllers
{
    public class ArquivoController : Controller
    {   
        public JsonResult ValidarChaveArquivoOrtoFoto(string chave)
        {
            try
            {
                OrtofotoBus ortofotoBus = new OrtofotoBus();
               
                string dir = Convert.ToString(ConfigurationManager.AppSettings["diretorioOrtofotos"]);

                if (string.IsNullOrEmpty(dir))
                {
                    Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Diretório de arquivo de ortofotos não corretamente configurado. Contate o Administrador do sistema." });
                }
                else
                {
                    string diretorio = HttpContext.Server.MapPath("~/" + dir);

                    Ortofoto ortofoto = ortofotoBus.ValidarChaveOrtofoto(chave);
                    if (ortofoto == null)
                    {
                        Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Esta chave está expirada envie uma nova requisição de chave." });
                    }
                    else
                    {
						if (ArquivoBus.ExisteArquivo(ortofoto.ArquivoNome, diretorio))
                        {
                            Validacao.Add(new Mensagem() { Tipo = eTipoMensagem.Erro, Texto = "Arquivo não encontrado." });
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Validacao.AddErro(exc);
            }
			return Json(new { Msg = Validacao.Erros, EhValido = Validacao.EhValido }, JsonRequestBehavior.AllowGet);
        }

		public FileResult DownloadArquivoOrtoFoto(string chave)
		{
			OrtofotoBus ortofotoBus = new OrtofotoBus();
			Ortofoto ortofoto = ortofotoBus.ValidarChaveOrtofoto(chave);

			string diretorio = Path.Combine(ConfigurationManager.AppSettings["diretorioOrtofotos"], ortofoto.ArquivoNome);
			FilePathResult fileResult = new FilePathResult(diretorio, "application/octet-stream");
			fileResult.FileDownloadName = ortofoto.ArquivoNome;

			return fileResult;
		}
    }
}
